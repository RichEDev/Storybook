namespace Spend_Management
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Text;
    using System.Web.UI.WebControls;

    using SpendManagementLibrary;
    using SpendManagementLibrary.Employees;
    using SpendManagementLibrary.Enumerators;

    using Utilities.DistributedCaching;
    using SpendManagementLibrary.Interfaces;
    using SpendManagementLibrary.Helpers;
    using Spend_Management.expenses.code;

    /// <summary>
    /// Summary description for floats.
    /// </summary>
    public class cFloats
    {
        #region Fields

        private readonly Cache cache = new Cache();

        readonly int nAccountid;

        readonly int nSubAccountId;

        SortedList<int, cFloat> list;

        /// <summary>
        /// The cache area label for distributed caching
        /// </summary>
        /// <remarks>This is also used manually in SpendManagementLibrary.Employees.User.Delete and should be changed there too if changed</remarks>
        public const string CacheArea = "advances";

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Constructor for floats, caches
        /// </summary>
        /// <param name="accountid"></param>
        public cFloats(int accountid)
        {
            this.nAccountid = accountid;
            cAccountSubAccounts subaccs = new cAccountSubAccounts(accountid);

            // Will need modifying when sub-accounts fully supported
            this.nSubAccountId = subaccs.getFirstSubAccount().SubAccountID;

            this.InitialiseData();
        }

        #endregion

        #region Properties

        private int accountid
        {
            get { return this.nAccountid; }
        }

        #endregion

        #region Public Methods and Operators

        public List<ListItem> CreateDropDown(int employeeid, int currencyid, int floatid)
        {
            // ONLY WORKS FOR A SINGLE SUBACCOUNT AT PRESENT.
            cAccountSubAccounts subaccs = new cAccountSubAccounts(this.accountid);
            int subAccountID = subaccs.getFirstSubAccount().SubAccountID;

            cGlobalCurrencies clsglobalcurrencies = new cGlobalCurrencies();
            cCurrencies clscurrencies = new cCurrencies(this.accountid, subAccountID);
         
            List<ListItem> items = new List<ListItem>();
        
            foreach (cFloat advance in this.list.Values)
            {
                bool outcome = this.ValidateAdvance(advance, employeeid, currencyid, floatid, clscurrencies);

                if (outcome)
                {
                    cCurrency reqcur = clscurrencies.getCurrencyById(advance.currencyid);
                    items.Add(new ListItem(advance.name + " (" + clsglobalcurrencies.getGlobalCurrencyById(reqcur.globalcurrencyid).label + ")", advance.floatid.ToString()));
                }
            }

            if (items.Count > 0)
            {
                items.Insert(0, new ListItem("", "0"));
            }

            return items;
        }

        /// <summary>
        /// Gets the available advances for the supplied criteria.
        /// </summary>
        /// <param name="employeeId">
        /// The employee id.
        /// </param>
        /// <param name="currencyId">
        /// The currency id.
        /// </param>
        /// <param name="floatId">
        /// The float id.
        /// </param>
        /// <param name="subAccountId">
        /// The sub account id.
        /// </param>
        /// <returns>
        /// A list of <see cref="cFloat">cFloat</see>
        /// </returns>
        public List<cFloat> GetAvailableAdvances(int employeeId, int currencyId, int floatId, int subAccountId)
        {
            var advances = new List<cFloat>();
            cCurrencies currencies = new cCurrencies(this.accountid, subAccountId);

            foreach (cFloat advance in this.list.Values)
            {
                bool outcome = this.ValidateAdvance(advance, employeeId, currencyId, floatId, currencies);

                if (outcome)
                {
                    advances.Add(advance);
                }
            }

            return advances;
        }

        /// <summary>
        /// Gets the available advances for the supplied criteria.
        /// </summary>
        /// <param name="employeeId">
        /// The employee id.
        /// </param>
        /// <param name="subAccountId">
        /// The sub account id.
        /// </param>
        /// <returns>
        /// A list of <see cref="cFloat">cFloat</see>
        /// </returns>
        public List<cFloat> GetAvailableAdvancesForUser(int employeeId, int subAccountId)
        {
            var advances = new List<cFloat>();
            cCurrencies currencies = new cCurrencies(this.accountid, subAccountId);

            foreach (cFloat advance in this.list.Values)
            {
                bool outcome = this.ValidateAdvanceForUser(advance, employeeId, currencies);

                if (outcome)
                {
                    advances.Add(advance);
                }
            }

            return advances;
        }

        public cFloat GetFloatById(int floatid)
        {
            return this.list[floatid];
        }

        public int SendClaimToNextStage(cFloat reqfloat, bool submitting, int authoriser, int employeeid)
        {
            //return codes
            //1 = user not assigned to a group
            //2 = reached last stage, claim now needs paying

            cStage reqstage;
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(this.accountid));

            bool isOnHoliday = false;
            int stage = reqfloat.stage - 1;
            SignoffType signofftype;
            int onholiday = 0;
            int relid = 0;
            StageInclusionType include = StageInclusionType.None;
            int notify = 0;
            decimal maxamount = 0;
            decimal claimamount = 0;

            CurrentUser user = cMisc.GetCurrentUser();
            cEmployees clsemployees = new cEmployees(this.accountid);
            var notifications = new NotificationTemplates(user);
            cGroups clsgroups = new cGroups(user.AccountID);
            Employee claimemp = clsemployees.GetEmployeeById(employeeid);
            cGroup reqgroup = clsgroups.GetGroupById(claimemp.AdvancesSignOffGroup);




            //get the stages the claim has to go through
            if (reqgroup == null)
            {
                return 3;
            }
            if (reqgroup.stages.Count == 0)
            {
                return 3;
            }



            //increment stage

            if (reqfloat.stage >= (reqgroup.stages.Count) && submitting == false) //reached last stage, now needs payin
            {
                this.approveFloat(reqfloat.floatid);
                var recieverId = new int[] {employeeid};
                notifications.SendMessage(SendMessageEnum.GetEnumDescription(SendMessageDescription.SendToAClaimantToNotifyThemThatTheirAdvanceHasBeenApproved), authoriser, recieverId, reqfloat.floatid);
                return 2;
            }

            for (stage = reqfloat.stage; stage < reqgroup.stages.Count; stage++)
            {
                reqstage = reqgroup.stages.Values[stage];
                bool includestage = true;
                signofftype = reqstage.signofftype;
                relid = reqstage.relid;
                //is the stage on holiday?
                isOnHoliday = this.userOnHoliday(signofftype, relid);
                if (isOnHoliday)
                {
                    onholiday = reqstage.onholiday;
                    switch (onholiday)
                    {
                        case 1: //take no action - wait for user to get back of their hols
                            includestage = true;
                            break;
                        case 2: //skip stage

                            includestage = false;
                            break;
                        case 3: //assign to someoneelse
                            signofftype = reqstage.holidaytype;
                            relid = reqstage.relid;
                            includestage = true;
                            break;

                    }
                }

                if (clsemployees.CheckAccessRole(AccessRoleType.View, SpendManagementElement.Advances, relid, this.nSubAccountId) == false)
                {
                    includestage = false;
                }

                //got the stage we need. 
                if (includestage)
                {
                    notify = reqstage.notify;
                    include = reqstage.include;

                    switch (include)
                    {
                        case StageInclusionType.ClaimTotalExceeds: //claim exceeded specified amount
                            maxamount = reqstage.amount;
                            if (claimamount < maxamount)
                            {
                                includestage = false;
                            }
                            break;
                    }
                }

                //check to see if limit existed. if this was exceeded includestage will have been changed to false
                if (includestage)
                {
                    int[] nextcheckerid = new int[1];
                    nextcheckerid[0] = this.getNextCheckerId(reqfloat, signofftype, relid);
                    if (nextcheckerid[0] == 0)
                    {
                        return 1;
                    }

                    reqfloat.stage++;
                    if (notify == 1) //just notify of claim by email
                    {
                        notifications.SendMessage(SendMessageEnum.GetEnumDescription(SendMessageDescription.SentToAnAdministratorToNotifyThemOfAAdvanceBeingRequested), employeeid, nextcheckerid, reqfloat.floatid);
                    }
                    else
                    {
                        string sql = "update [floats] set stage = @stage,";
                        expdata.sqlexecute.Parameters.AddWithValue("@stage", stage + 1);

                        if (signofftype == SignoffType.Team)
                        {
                            sql = sql + "approver = null";
                            sql = sql + ", teamid = @teamid";
                            expdata.sqlexecute.Parameters.AddWithValue("@teamid", relid);
                        }
                        else
                        {
                            sql = sql + "approver = @checkerid";
                            expdata.sqlexecute.Parameters.AddWithValue("@checkerid", nextcheckerid[0]);

                            reqfloat.approver = nextcheckerid[0];
                        }
                        sql = sql + " where floatid = @floatid";


                        expdata.sqlexecute.Parameters.AddWithValue("@floatid", reqfloat.floatid);
                        expdata.sqlexecute.Parameters.AddWithValue("@employeeid", employeeid);
                        expdata.ExecuteSQL(sql);
                        notifications.SendMessage(SendMessageEnum.GetEnumDescription(SendMessageDescription.SentToAnAdministratorToRequestAnAdvance), employeeid, nextcheckerid, reqfloat.floatid);
                        int[] arremp = new int[1];
                        arremp[0] = employeeid;

                        this.InvalidateCache(false);

                        break;
                    }

                }

            }
            expdata.sqlexecute.Parameters.Clear();
            return 0;
        }

        /// <summary>
        /// Unsettle advance.
        /// </summary>
        /// <param name="floatID">
        /// Float id.
        /// </param>
        public void UnsettleAdvance(int floatID)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(this.accountid));
            string strSQL = "UPDATE [floats] SET settled = 0 WHERE floatid = @floatid";
            cFloat reqFloat = this.GetFloatById(floatID);
            expdata.sqlexecute.Parameters.AddWithValue("@floatid", floatID);
            expdata.ExecuteSQL(strSQL);
            expdata.sqlexecute.Parameters.Clear();

            this.InvalidateCache(false);
        }

        public void addAllocation(int floatid, int expenseid, decimal allocation)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(this.accountid));
            string strsql;
            strsql = "insert into float_allocations (floatid, expenseid, [amount]) " +
                     "values (@floatid, @expenseid, @amount)";
            expdata.sqlexecute.Parameters.AddWithValue("@floatid", floatid);
            expdata.sqlexecute.Parameters.AddWithValue("@expenseid", expenseid);
            expdata.sqlexecute.Parameters.AddWithValue("@amount", allocation);
            expdata.ExecuteSQL(strsql);
            expdata.sqlexecute.Parameters.Clear();

            this.InvalidateCache(false);
        }

        /// <summary>
        /// Change amount.
        /// </summary>
        /// <param name="floatid">
        /// Float id.
        /// </param>
        /// <param name="newAmount">
        /// New amount.
        /// </param>
        public void changeAmount(int floatid, decimal newAmount)
        {
            decimal foreignAmount = Math.Round(newAmount, 2, MidpointRounding.AwayFromZero);
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(this.accountid));
            decimal amount;
            cFloat reqfloat = this.GetFloatById(floatid);
            if (reqfloat.currencyid == 0)
            {
                amount = newAmount;
            }
            else
            {
                if (reqfloat.exchangerate != 0)
                {
                    amount = newAmount * (decimal)reqfloat.exchangerate;
                }
                else
                {
                    amount = newAmount;
                }
            }

            amount = Math.Round(amount, 2, MidpointRounding.AwayFromZero);
            const string SQL = "update [floats] set [float] = @amount, foreignAmount = @foreignAmount where floatid = @floatid";
            expdata.sqlexecute.Parameters.AddWithValue("@amount", amount);
            expdata.sqlexecute.Parameters.AddWithValue("@foreignAmount", foreignAmount);
            expdata.sqlexecute.Parameters.AddWithValue("@floatid", floatid);
            expdata.ExecuteSQL(SQL);
            expdata.sqlexecute.Parameters.Clear();

            this.InvalidateCache(false);
        }

        /// <summary>
        /// Delete allocation.
        /// </summary>
        /// <param name="expenseid">
        /// Expense id.
        /// </param>
        /// <param name="floatid">
        /// Float id.
        /// </param>
        public void deleteAllocation(int expenseid, int floatid)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(this.accountid));
            string strsql;
            decimal amount = 0;

            strsql = "select [amount] from float_allocations where expenseid = @expenseid";
            expdata.sqlexecute.Parameters.AddWithValue("@expenseid", expenseid);
            using (SqlDataReader reader = expdata.GetReader(strsql))
            {
                while (reader.Read())
                {
                    if (reader.IsDBNull(0) == false)
                    {
                        amount = reader.GetDecimal(0);
                    }
                }
                reader.Close();
            }

            if (amount != 0)
            {
                strsql = "delete from float_allocations where expenseid = @expenseid";
                expdata.ExecuteSQL(strsql);

                this.InvalidateCache(false);
            }

            expdata.sqlexecute.Parameters.Clear();
        }

        /// <summary>
        /// Delete float.
        /// </summary>
        /// <param name="floatid">
        /// Float id.
        /// </param>
        public void deleteFloat(int floatid)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(this.accountid));
            expdata.sqlexecute.Parameters.AddWithValue("@floatid", floatid);
            expdata.ExecuteSQL("delete from floats where floatid = @floatid");

            this.InvalidateCache();
        }

        /// <summary>
        /// Dispute advance.
        /// </summary>
        /// <param name="accountid">
        /// Account id.
        /// </param>
        /// <param name="reason">
        /// Reason.
        /// </param>
        /// <param name="floatid">
        /// Float id.
        /// </param>
        public void disputeAdvance(int accountid, string reason, int floatid)
        {
            cFloat reqfloat = this.GetFloatById(floatid);
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            string strsql;
            int[] recipient = new int[1];
            strsql = "update [floats] set disputed = 1, dispute = @dispute where floatid = @floatid";

            if (reason.Length >= 4000)
            {
                reason = reason.Substring(0, 3998);
            }


            expdata.sqlexecute.Parameters.AddWithValue("@dispute", reason);
            expdata.sqlexecute.Parameters.AddWithValue("@floatid", floatid);
            expdata.ExecuteSQL(strsql);
            expdata.sqlexecute.Parameters.Clear();

            var notifications = new NotificationTemplates(this.accountid, 0, string.Empty, 0, Modules.expenses);

            recipient[0] = reqfloat.approver;
            notifications.SendMessage(SendMessageEnum.GetEnumDescription(SendMessageDescription.SentToAnAdministratorToNotifyThemAnAdvanceHasBeenDispute), reqfloat.employeeid, recipient, floatid);

            this.InvalidateCache(false);
        }

        public cFloat getFloatByName(int employeeid, string name)
        {
            return this.list.Values.FirstOrDefault(reqfloat => reqfloat.employeeid == employeeid && reqfloat.name == name);
        }

        /// <summary>
        /// Get float count.
        /// </summary>
        /// <param name="employeeid">
        /// Employee id.
        /// </param>
        /// <param name="currencyid">
        /// Currency id.
        /// </param>
        /// <returns>
        /// For details <see cref="int"/>.
        /// </returns>
        public int getFloatCount(int employeeid, int currencyid)
        {
            return this.list.Values.Count(reqfloat => reqfloat.employeeid == employeeid && reqfloat.approved && reqfloat.floatavailable > 0);
        }

        /// <summary>
        /// Get a list floats/advances associated to a particular employee
        /// </summary>
        /// <param name="employeeid">Employee to retrieve floats/advances for</param>
        /// <param name="paidFloats">FALSE if outstanding/current float/advances are to be retrieved</param>
        /// <returns>List of cFloat records</returns>
        public List<cFloat> getFloatsByEmployeeId(int employeeid, bool paidFloats)
        {
            return this.list.Values.Where(reqfloat => reqfloat.employeeid == employeeid).Where(reqfloat => reqfloat.paid == paidFloats).ToList();
        }

        /// <summary>
        /// The get grid.
        /// </summary>
        /// <param name="approver">
        /// The approver.
        /// </param>
        /// <param name="employeeid">
        /// The employeeid.
        /// </param>
        /// <param name="active">
        /// The active.
        /// </param>
        /// <returns>
        /// The <see cref="DataSet"/>.
        /// </returns>
        public DataSet getGrid(bool approver, int employeeid, bool active)
        {
            DataSet ds = new DataSet();
            int i;
            DataTable tbl = new DataTable();
            tbl.Columns.Add("floatid", Type.GetType("System.Int32"));
            tbl.Columns.Add("employeeid", Type.GetType("System.Int32"));
            tbl.Columns.Add("name", Type.GetType("System.String"));
            tbl.Columns.Add("reason", Type.GetType("System.String"));
            tbl.Columns.Add("originalcurrency", Type.GetType("System.Int32"));
            tbl.Columns.Add("exchangerate", Type.GetType("System.Double"));
            tbl.Columns.Add("Total Prior To Convert", Type.GetType("System.Decimal"));
            tbl.Columns.Add("floatamount", Type.GetType("System.Decimal"));
            tbl.Columns.Add("floatused", Type.GetType("System.Decimal"));
            tbl.Columns.Add("floatavailable", Type.GetType("System.Decimal"));
            tbl.Columns.Add("requiredby", Type.GetType("System.DateTime"));
            tbl.Columns.Add("approver", Type.GetType("System.Int32"));
            tbl.Columns.Add("approved", Type.GetType("System.Boolean"));
            tbl.Columns.Add("stage", Type.GetType("System.Byte"));
            tbl.Columns.Add("rejected", Type.GetType("System.Boolean"));
            tbl.Columns.Add("rejectreason", Type.GetType("System.String"));
            tbl.Columns.Add("disputed", Type.GetType("System.Boolean"));
            tbl.Columns.Add("dispute", Type.GetType("System.String"));
            tbl.Columns.Add("paid", Type.GetType("System.Boolean"));
            tbl.Columns.Add("issuenum", Type.GetType("System.Int32"));
            tbl.Columns.Add("basecurrency", Type.GetType("System.Int32"));
            tbl.Columns.Add("settled", typeof(Boolean));

            foreach (cFloat reqfloat in this.list.Values)
            {
                bool display = false;

                if (approver)
                {
                    if (reqfloat.approver == employeeid && reqfloat.paid == false && active == false)
                    {

                        if (reqfloat.rejected == false || (reqfloat.rejected && reqfloat.disputed))
                        {
                            display = true;
                        }


                    }
                    else if (reqfloat.approver == employeeid && reqfloat.paid && active && !reqfloat.settled)
                    {

                        display = true;

                    }
                }
                else
                {
                    if (reqfloat.employeeid == employeeid && !reqfloat.settled)
                    {
                        display = true;
                    }
                }
                if (display)
                {
                    object[] values = new object[22];
                    values[0] = reqfloat.floatid;
                    values[1] = reqfloat.employeeid;
                    values[2] = reqfloat.name;
                    values[3] = reqfloat.reason;
                    values[4] = reqfloat.currencyid;
                    values[5] = Math.Round(reqfloat.exchangerate, 4, MidpointRounding.AwayFromZero);
                    if (reqfloat.exchangerate > 0)
                    {
                        values[6] = Math.Round(reqfloat.foreignAmount, 2, MidpointRounding.AwayFromZero);
                    }
                    values[7] = reqfloat.floatamount;
                    values[8] = reqfloat.floatused;
                    values[9] = reqfloat.floatamount - reqfloat.floatused;
                    values[10] = reqfloat.requiredby;
                    values[11] = reqfloat.approver;
                    values[12] = reqfloat.approved;
                    values[13] = reqfloat.stage;
                    values[14] = reqfloat.rejected;
                    values[15] = reqfloat.rejectreason;
                    values[16] = reqfloat.disputed;
                    values[17] = reqfloat.dispute;
                    values[18] = reqfloat.paid;
                    values[19] = reqfloat.issuenum;
                    values[20] = reqfloat.basecurrency;
                    values[21] = reqfloat.settled;

                    tbl.Rows.Add(values);
                }
            }

            ds.Tables.Add(tbl);
            return ds;
        }

        /// <summary>
        /// Get modified float info.
        /// </summary>
        /// <param name="date">
        /// Date.
        /// </param>
        /// <returns>
        /// For details <see cref="sFloatInfo"/>.
        /// </returns>
        public sFloatInfo getModifiedFloatInfo(DateTime date)
        {
            sFloatInfo floatinfo = new sFloatInfo();
            Dictionary<int, cFloat> lstfloats = new Dictionary<int, cFloat>();
            List<int> lstfloatids = new List<int>();

            foreach (cFloat val in this.list.Values)
            {
                lstfloatids.Add(val.floatid);

                if (val.createdon > date || val.modifiedon > date)
                {
                    lstfloats.Add(val.floatid, val);
                }
            }

            floatinfo.lstfloats = lstfloats;
            floatinfo.lstfloatids = lstfloatids;

            return floatinfo;
        }

        /// <summary>
        /// Get settled grid.
        /// </summary>
        /// <param name="approver">
        /// Approver.
        /// </param>
        /// <param name="employeeid">
        /// Employee id.
        /// </param>
        /// <param name="active">
        /// Active.
        /// </param>
        /// <returns>
        /// For details <see cref="DataSet"/>.
        /// </returns>
        public DataSet getSettledGrid(bool approver, int employeeid, bool active)
        {
            DataSet ds = new DataSet();

            DataTable tbl = new DataTable();
            tbl.Columns.Add("floatid", Type.GetType("System.Int32"));
            tbl.Columns.Add("employeeid", Type.GetType("System.Int32"));
            tbl.Columns.Add("name", Type.GetType("System.String"));
            tbl.Columns.Add("reason", Type.GetType("System.String"));
            tbl.Columns.Add("originalcurrency", Type.GetType("System.Int32"));
            tbl.Columns.Add("exchangerate", Type.GetType("System.Double"));
            tbl.Columns.Add("Total Prior To Convert", Type.GetType("System.Decimal"));
            tbl.Columns.Add("floatamount", Type.GetType("System.Decimal"));
            tbl.Columns.Add("floatused", Type.GetType("System.Decimal"));
            tbl.Columns.Add("floatavailable", Type.GetType("System.Decimal"));
            tbl.Columns.Add("requiredby", Type.GetType("System.DateTime"));
            tbl.Columns.Add("approver", Type.GetType("System.Int32"));
            tbl.Columns.Add("approved", Type.GetType("System.Boolean"));
            tbl.Columns.Add("stage", Type.GetType("System.Byte"));
            tbl.Columns.Add("rejected", Type.GetType("System.Boolean"));
            tbl.Columns.Add("rejectreason", Type.GetType("System.String"));
            tbl.Columns.Add("disputed", Type.GetType("System.Boolean"));
            tbl.Columns.Add("dispute", Type.GetType("System.String"));
            tbl.Columns.Add("paid", Type.GetType("System.Boolean"));
            tbl.Columns.Add("issuenum", Type.GetType("System.Int32"));
            tbl.Columns.Add("basecurrency", Type.GetType("System.Int32"));
            tbl.Columns.Add("settled", typeof(Boolean));

            foreach (cFloat reqfloat in this.list.Values)
            {
                bool display = false;

                if (approver)
                {
                    if (reqfloat.approver == employeeid && reqfloat.paid && active && reqfloat.settled)
                    {
                        display = true;
                    }
                }

                if (display)
                {
                    object[] values = new object[22];
                    values[0] = reqfloat.floatid;
                    values[1] = reqfloat.employeeid;
                    values[2] = reqfloat.name;
                    values[3] = reqfloat.reason;
                    values[4] = reqfloat.currencyid;
                    values[5] = Math.Round(reqfloat.exchangerate, 4, MidpointRounding.AwayFromZero);
                    values[6] = Math.Round(reqfloat.foreignAmount, 2, MidpointRounding.AwayFromZero);
                    values[7] = reqfloat.floatamount;
                    values[8] = reqfloat.floatused;
                    values[9] = reqfloat.floatamount - reqfloat.floatused;
                    values[10] = reqfloat.requiredby;
                    values[11] = reqfloat.approver;
                    values[12] = reqfloat.approved;
                    values[13] = reqfloat.stage;
                    values[14] = reqfloat.rejected;
                    values[15] = reqfloat.rejectreason;
                    values[16] = reqfloat.disputed;
                    values[17] = reqfloat.dispute;
                    values[18] = reqfloat.paid;
                    values[19] = reqfloat.issuenum;
                    values[20] = reqfloat.basecurrency;
                    values[21] = reqfloat.settled;

                    tbl.Rows.Add(values);
                }
            }

            ds.Tables.Add(tbl);
            return ds;
        }

        /// <summary>
        /// Get string float dropdown.
        /// </summary>
        /// <param name="output">
        /// Output.
        /// </param>
        /// <param name="employeeid">
        /// Employee id.
        /// </param>
        /// <param name="currencyid">
        /// Currency id.
        /// </param>
        /// <param name="floatid">
        /// Float id.
        /// </param>
        public void getStringFloatDropdown(ref StringBuilder output, int employeeid, int currencyid, int floatid)
        {
            cGlobalCurrencies clsglobalcurrencies = new cGlobalCurrencies();

            // ONLY WORKS FOR A SINGLE SUBACCOUNT AT PRESENT.
            cAccountSubAccounts subaccs = new cAccountSubAccounts(this.accountid);
            int subAccountID = subaccs.getFirstSubAccount().SubAccountID;

            cCurrencies clscurrencies = new cCurrencies(this.accountid, subAccountID);
            cCurrency reqcur;
            decimal floatavailable;

            output.Append("<option value=\"0\"></option>");
            foreach (cFloat reqfloat in this.list.Values)
            {
                if (reqfloat.employeeid == employeeid && reqfloat.paid)
                {
                    floatavailable = reqfloat.floatamount - reqfloat.floatused;

                    if (floatavailable > 0)
                    {
                        output.Append("<option value=\"" + reqfloat.floatid + "\"");
                        if (reqfloat.floatid == floatid)
                        {
                            output.Append(" selected");
                        }
                        output.Append(">");
                        output.Append(reqfloat.name);
                        if (reqfloat.currencyid == 0)
                        {
                            output.Append(" (GBP)");
                        }
                        else
                        {
                            reqcur = clscurrencies.getCurrencyById(reqfloat.currencyid);
                            output.Append(" (" + clsglobalcurrencies.getGlobalCurrencyById(reqcur.globalcurrencyid).label + ")");
                        }
                        output.Append("</option>");
                    }
                }
            }
        }

        public void payAdvance(int accountid, int floatid)
        {
            cFloat reqfloat = this.GetFloatById(floatid);
            int issuenum = 0;
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            string strsql;
            int[] recipient = new int[1];

            strsql = "select max(issuenum) from [floats]";

            using (SqlDataReader reader = expdata.GetReader(strsql))
            {
                while (reader.Read())
                {
                    if (reader.IsDBNull(0) == false)
                    {
                        issuenum = reader.GetInt32(0);
                    }
                }
                reader.Close();
            }

            issuenum++;
            strsql = "update [floats] set paid = 1, datepaid = @datepaid, issuenum = @issuenum where floatid = @floatid";

            expdata.sqlexecute.Parameters.AddWithValue("@floatid", floatid);
            expdata.sqlexecute.Parameters.AddWithValue("@datepaid", DateTime.Now);
            expdata.sqlexecute.Parameters.AddWithValue("@issuenum", issuenum);
            expdata.ExecuteSQL(strsql);
            expdata.sqlexecute.Parameters.Clear();

            reqfloat.payAdvance();

            var notifications = new NotificationTemplates(this.accountid, 0, string.Empty, 0, Modules.expenses);

            recipient[0] = reqfloat.employeeid;

            notifications.SendMessage(SendMessageEnum.GetEnumDescription(SendMessageDescription.SentToAClaimantToNotifyThemTheirAdvanceHasBeenPaid), reqfloat.approver, recipient, floatid);
            this.InvalidateCache();
        }

        /// <summary>
        /// Reject advance.
        /// </summary>
        /// <param name="accountid">
        /// Account id.
        /// </param>
        /// <param name="reason">
        /// Reason.
        /// </param>
        /// <param name="floatid">
        /// TFloat id.
        /// </param>
        public void rejectAdvance(int accountid, string reason, int floatid)
        {
            cFloat reqfloat = this.GetFloatById(floatid);
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            string strsql;
            int[] recipient = new int[1];

            if (reason.Length >= 3999)
            {
                reason = reason.Substring(0, 3998);
            }
            strsql = "update [floats] set rejected = 1, rejectreason = @rejectreason, disputed=0 where floatid = @floatid";
            expdata.sqlexecute.Parameters.AddWithValue("@rejectreason", reason);
            expdata.sqlexecute.Parameters.AddWithValue("@floatid", floatid);
            expdata.ExecuteSQL(strsql);
            expdata.sqlexecute.Parameters.Clear();

            var notifications = new NotificationTemplates(this.accountid, 0, string.Empty, 0, Modules.expenses);

            recipient[0] = reqfloat.employeeid;
            reqfloat.rejected = true;
            try
            {
                notifications.SendMessage(SendMessageEnum.GetEnumDescription(SendMessageDescription.SendToAClaimantToNotifyThemThatTheirAdvanceHasBeenRejected), reqfloat.approver, recipient, floatid);
            }
            catch
            {

            }
            this.InvalidateCache();

        }

        /// <summary>
        /// Request float.
        /// </summary>
        /// <param name="employeeid">
        /// Employee id.
        /// </param>
        /// <param name="name">
        /// Name.
        /// </param>
        /// <param name="reason">
        /// Reason.
        /// </param>
        /// <param name="amount">
        /// Amount.
        /// </param>
        /// <param name="currencyid">
        /// Currency id.
        /// </param>
        /// <param name="requiredby">
        /// Required by.
        /// </param>
        /// <param name="basecurrency">
        /// Base currency.
        /// </param>
        /// <returns>
        /// For details <see cref="int"/>.
        /// </returns>
        public int requestFloat(int employeeid, string name, string reason, decimal amount, int currencyid, string requiredby, int basecurrency)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(this.accountid));
            decimal exchangerate = 0;
            DateTime createdon = DateTime.Now.ToUniversalTime();
            decimal foreignAmount = Math.Round(amount, 2, MidpointRounding.AwayFromZero);

            expdata.sqlexecute.Parameters.AddWithValue("@name", name);

            bool hasRows = expdata.getcount("SELECT COUNT(*) FROM floats WHERE name=@name") > 0;

            expdata.sqlexecute.Parameters.Clear();

            if (hasRows)
            {
                return 2;
            }

            expdata.sqlexecute.Parameters.AddWithValue("@employeeid", employeeid);

            if (currencyid == basecurrency)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@currencyid", basecurrency);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@currencyid", currencyid);

                //convert to sterling
                decimal newamount = this.convertTotals(employeeid, currencyid, amount);
                exchangerate = newamount / amount;
                if (exchangerate == 0)
                {
                    return 1;
                }

                amount = newamount;
            }

            amount = Math.Round(amount, 2, MidpointRounding.AwayFromZero);
            expdata.sqlexecute.Parameters.AddWithValue("@amount", amount);
            expdata.sqlexecute.Parameters.AddWithValue("@foreignAmount", foreignAmount);
            expdata.sqlexecute.Parameters.AddWithValue("@exchangerate", exchangerate);
            expdata.sqlexecute.Parameters.AddWithValue("@name", name);
            expdata.sqlexecute.Parameters.AddWithValue(
                "@reason", reason.Length > 4000 ? reason.Substring(0, 3999) : reason);

            DateTime dtrequiredby;
            if (requiredby == "")
            {
                expdata.sqlexecute.Parameters.AddWithValue("@requiredby", DBNull.Value);
                dtrequiredby = new DateTime(1900, 01, 01);
            }
            else
            {

                dtrequiredby = DateTime.Parse(requiredby);
                expdata.sqlexecute.Parameters.AddWithValue("@requiredby", dtrequiredby.Year + "/" + dtrequiredby.Month + "/" + dtrequiredby.Day);

            }

            expdata.sqlexecute.Parameters.AddWithValue("@basecurrency", basecurrency);
            expdata.sqlexecute.Parameters.AddWithValue("@identity", SqlDbType.Int);
            expdata.sqlexecute.Parameters["@identity"].Direction = ParameterDirection.Output;
            expdata.sqlexecute.Parameters.AddWithValue("@createdon", createdon);
            expdata.sqlexecute.Parameters.AddWithValue("@createdby", employeeid);

            expdata.ExecuteSQL("INSERT INTO [floats] (employeeid, currencyid, [float], [name], reason, requiredby, exchangerate, basecurrency, createdon, createdby, foreignAmount) VALUES (@employeeid, @currencyid, @amount, @name, @reason, @requiredby, @exchangerate,  @basecurrency, @createdon, @createdby, @foreignAmount);SET @identity = SCOPE_IDENTITY();");
            int floatid = (int)expdata.sqlexecute.Parameters["@identity"].Value;
            cFloat reqfloat = new cFloat(this.accountid, floatid, employeeid, currencyid, name, reason, dtrequiredby, false, 0, amount, Convert.ToDouble(exchangerate), 0, false, "", false, "", false, new DateTime(1900, 01, 01), 0, basecurrency, false, new SortedList<int, decimal>(), 0, createdon, employeeid, new DateTime(1900, 01, 01), 0, foreignAmount);

            //reqfloat = GetFloatById(floatid);
            this.SendClaimToNextStage(reqfloat, true, 0, employeeid);

            this.InvalidateCache(false);

            return floatid;
        }

        /// <summary>
        /// Return remainder.
        /// </summary>
        /// <param name="floatID">
        /// Float id.
        /// </param>
        public void returnRemainder(int floatID)
        {
            cFloat reqFloat = this.GetFloatById(floatID);
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(this.accountid));
            expdata.sqlexecute.Parameters.AddWithValue("@newTotal", (reqFloat.floatamount - reqFloat.floatavailable));
            expdata.sqlexecute.Parameters.AddWithValue("@floatID", reqFloat.floatid);
            expdata.ExecuteSQL("UPDATE [floats] SET float = @newTotal WHERE floatid = @floatID");
            expdata.sqlexecute.Parameters.Clear();

            this.InvalidateCache(false);
        }

        /// <summary>
        /// Settle advance.
        /// </summary>
        /// <param name="floatid">
        /// Float id.
        /// </param>
        public void settleAdvance(int floatid)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(this.accountid));

            cFloat reqfloat = this.GetFloatById(floatid);
            reqfloat.settleAdvance();

            expdata.sqlexecute.Parameters.AddWithValue("@floatid", floatid);

            expdata.ExecuteSQL("update [floats] set settled = 1 where floatid = @floatid");
            expdata.sqlexecute.Parameters.Clear();

            this.InvalidateCache();
        }

        /// <summary>
        /// Top up advance.
        /// </summary>
        /// <param name="floatid">
        /// Float id.
        /// </param>
        /// <param name="fAmount">
        /// Float amount.
        /// </param>
        /// <returns>
        /// For details <see cref="bool"/>.
        /// </returns>
        public bool topUpAdvance(int floatid, decimal fAmount)
        {
            decimal foreignAmount = Math.Round(fAmount, 2, MidpointRounding.AwayFromZero);
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(this.accountid));
            cFloat reqfloat = this.GetFloatById(floatid);
            decimal convertedForeignAmount;

            if (reqfloat.exchangerate > 0)
            {
                convertedForeignAmount = (fAmount / (1 / decimal.Parse(reqfloat.exchangerate.ToString())));
            }
            else
            {
                convertedForeignAmount = fAmount;
            }

            decimal newtotal = reqfloat.floatamount + convertedForeignAmount;
            decimal newForeignTotal = reqfloat.foreignAmount + foreignAmount;

            if (newtotal > 0)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@newtotal", newtotal);
                expdata.sqlexecute.Parameters.AddWithValue("@newforeigntotal", newForeignTotal);
                expdata.sqlexecute.Parameters.AddWithValue("@floatid", floatid);
                expdata.ExecuteSQL("update [floats] set [float] = @newtotal, [foreignAmount] = @newforeigntotal where floatid = @floatid");
                expdata.sqlexecute.Parameters.Clear();
                this.InvalidateCache();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Update allocation.
        /// </summary>
        /// <param name="floatid">
        /// Floatid.
        /// </param>
        /// <param name="expenseid">
        /// Expense id.
        /// </param>
        /// <param name="allocation">
        /// Allocation value.
        /// </param>
        public void updateAllocation(int floatid, int expenseid, decimal allocation)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(this.accountid));

            expdata.sqlexecute.Parameters.AddWithValue("@floatid", floatid);
            expdata.sqlexecute.Parameters.AddWithValue("@expenseid", expenseid);
            expdata.sqlexecute.Parameters.AddWithValue("@amount", allocation);
            expdata.ExecuteSQL("update float_allocations set [amount] = @amount where floatid = @floatid and expenseid = @expenseid");
            expdata.sqlexecute.Parameters.Clear();

            this.InvalidateCache();
        }

        /// <summary>
        /// Update float.
        /// </summary>
        /// <param name="employeeid">
        /// Employee id.
        /// </param>
        /// <param name="floatid">
        /// Float id.
        /// </param>
        /// <param name="name">
        /// Name.
        /// </param>
        /// <param name="reason">
        /// Reason.
        /// </param>
        /// <param name="amount">
        /// Amount.
        /// </param>
        /// <param name="currencyid">
        /// Currencyid.
        /// </param>
        /// <param name="requiredby">
        /// Required by.
        /// </param>
        /// <param name="baseCurrency">
        /// Base Currency.
        /// </param>
        /// <returns>
        /// For details <see cref="byte"/>.
        /// </returns>
        public byte updateFloat(int employeeid, int floatid, string name, string reason, decimal amount, int currencyid, string requiredby,int baseCurrency = 0)
        {
            decimal foreignAmount = Math.Round(amount, 2, MidpointRounding.AwayFromZero);
            var expdata = new DBConnection(cAccounts.getConnectionString(this.accountid));

            expdata.sqlexecute.Parameters.AddWithValue("@name", name);
            expdata.sqlexecute.Parameters.AddWithValue("@floatID", floatid);

            bool hasRows = expdata.getcount("SELECT COUNT(*) FROM floats WHERE name = @name AND floatid <> @floatID") > 0;

            expdata.sqlexecute.Parameters.Clear();



            if (hasRows)
            {
                return 2;
            }

            decimal exchangerate = 0;

            if (currencyid == baseCurrency)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@currencyid", baseCurrency);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@currencyid", currencyid);

                //convert to sterling
                var newamount = this.convertTotals(employeeid, currencyid, amount);
                exchangerate = newamount / amount;
                if (exchangerate == 0)
                {
                    return 1;
                }

                amount = newamount;
            }

            amount = Math.Round(amount, 2, MidpointRounding.AwayFromZero);

            expdata.sqlexecute.Parameters.AddWithValue("@amount", amount);
            expdata.sqlexecute.Parameters.AddWithValue("@foreignAmount", foreignAmount);
            expdata.sqlexecute.Parameters.AddWithValue("@exchangerate", exchangerate);
            expdata.sqlexecute.Parameters.AddWithValue("@name", name);
            expdata.sqlexecute.Parameters.AddWithValue("@reason", reason.Length > 4000 ? reason.Substring(0, 3999) : reason);
            expdata.sqlexecute.Parameters.AddWithValue("@floatid", floatid);

            if (requiredby == "")
            {
                expdata.sqlexecute.Parameters.AddWithValue("@requiredby", DBNull.Value);
            }
            else
            {
                DateTime dtrequiredby = DateTime.Parse(requiredby);
                expdata.sqlexecute.Parameters.AddWithValue("@requiredby", string.Format("{0}/{1}/{2}", dtrequiredby.Year, dtrequiredby.Month, dtrequiredby.Day));
            }

            expdata.ExecuteSQL("update [floats] set disputed = 1, [name] = @name, currencyid = @currencyid, exchangerate = @exchangerate, [float] = @amount, requiredby = @requiredby, reason = @reason, foreignAmount = @foreignAmount where floatid = @floatid");
            expdata.sqlexecute.Parameters.Clear();

            this.InvalidateCache(false);

            return 0;
        }

        /// <summary>
        /// Gets a list of unsettled <see cref="cFloat"/>s for the supplied employeeId.
        /// </summary>
        /// <param name="employeeId">
        /// The employee id.
        /// </param>
        /// <returns>
        /// The <see cref="List"/> of <see cref="cFloat"/>s.
        /// </returns>
        public List<cFloat> GetUnsettledAdvancesByEmployee(int employeeId)
        {
            return this.list.Values.Where(advance => advance.employeeid == employeeId).Where(advance => advance.settled == false).ToList();        
        }

        #endregion

        #region Methods

        /// <summary>
        /// Cache list.
        /// </summary>
        /// <returns>
        /// For details <see cref="SortedList"/>.
        /// </returns>
        private SortedList<int, cFloat> CacheList()
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(this.accountid));
            SortedList<int, cFloat> advances = new SortedList<int, cFloat>();

            SortedList<int, SortedList<int, decimal>> lstAllocations = this.getFloatAllocations();
            SortedList<int, decimal> lstUsed = this.getFloatUsed();

            using (SqlDataReader reader = expdata.GetReader("select floatid, employeeid, currencyid, float, name, reason, requiredby, approved, approver, exchangerate, stage, rejected, rejectreason, disputed, dispute, paid, datepaid, issuenum, basecurrency, settled, createdon, createdby, modifiedon, modifiedby, foreignAmount from dbo.[floats]"))
            {
                while (reader.Read())
                {
                    int floatid = reader.GetInt32(reader.GetOrdinal("floatid"));
                    int employeeid = reader.GetInt32(reader.GetOrdinal("employeeid"));
                    int currencyid;
                    if (reader.IsDBNull(reader.GetOrdinal("currencyid")))
                    {
                        currencyid = 0;
                    }
                    else
                    {
                        currencyid = reader.GetInt32(reader.GetOrdinal("currencyid"));
                    }
                    decimal floatamount = reader.GetDecimal(reader.GetOrdinal("float"));
                    decimal foreignAmount = reader.GetDecimal(reader.GetOrdinal("foreignAmount"));
                    string name = reader.GetString(reader.GetOrdinal("name"));
                    string reason;
                    if (reader.IsDBNull(reader.GetOrdinal("reason")))
                    {
                        reason = "";
                    }
                    else
                    {
                        reason = reader.GetString(reader.GetOrdinal("reason"));
                    }
                    DateTime requiredby;
                    if (reader.IsDBNull(reader.GetOrdinal("requiredby")))
                    {
                        requiredby = new DateTime(1900, 01, 01);
                    }
                    else
                    {
                        requiredby = reader.GetDateTime(reader.GetOrdinal("requiredby"));
                    }
                    bool approved = reader.GetBoolean(reader.GetOrdinal("approved"));
                    int approver = reader.GetInt32(reader.GetOrdinal("approver"));
                    double exchangerate = reader.GetDouble(reader.GetOrdinal("exchangerate"));
                    byte stage = reader.GetByte(reader.GetOrdinal("stage"));
                    bool rejected = reader.GetBoolean(reader.GetOrdinal("rejected"));
                    bool disputed = reader.GetBoolean(reader.GetOrdinal("disputed"));
                    string rejectreason;
                    if (reader.IsDBNull(reader.GetOrdinal("rejectreason")) == false)
                    {
                        rejectreason = reader.GetString(reader.GetOrdinal("rejectreason"));
                    }
                    else
                    {
                        rejectreason = "";
                    }
                    string dispute;
                    if (reader.IsDBNull(reader.GetOrdinal("dispute")) == false)
                    {
                        dispute = reader.GetString(reader.GetOrdinal("dispute"));
                    }
                    else
                    {
                        dispute = "";
                    }
                    bool paid = reader.GetBoolean(reader.GetOrdinal("paid"));
                    DateTime datepaid;
                    if (reader.IsDBNull(reader.GetOrdinal("datepaid")) == false)
                    {
                        datepaid = reader.GetDateTime(reader.GetOrdinal("datepaid"));
                    }
                    else
                    {
                        datepaid = new DateTime(1900, 01, 01);
                    }
                    int issuenum;
                    if (reader.IsDBNull(reader.GetOrdinal("issuenum")) == false)
                    {
                        issuenum = reader.GetInt32(reader.GetOrdinal("issuenum"));
                    }
                    else
                    {
                        issuenum = 0;
                    }
                    int basecurrency;
                    if (reader.IsDBNull(reader.GetOrdinal("basecurrency")) == false)
                    {
                        basecurrency = reader.GetInt32(reader.GetOrdinal("basecurrency"));
                    }
                    else
                    {
                        basecurrency = 0;
                    }
                    DateTime createdon;
                    if (reader.IsDBNull(reader.GetOrdinal("createdon")))
                    {
                        createdon = new DateTime(1900, 01, 01);
                    }
                    else
                    {
                        createdon = reader.GetDateTime(reader.GetOrdinal("createdon"));
                    }
                    int createdby;
                    if (reader.IsDBNull(reader.GetOrdinal("createdby")))
                    {
                        createdby = 0;
                    }
                    else
                    {
                        createdby = reader.GetInt32(reader.GetOrdinal("createdby"));
                    }
                    DateTime modifiedon;
                    if (reader.IsDBNull(reader.GetOrdinal("modifiedon")))
                    {
                        modifiedon = new DateTime(1900, 01, 01);
                    }
                    else
                    {
                        modifiedon = reader.GetDateTime(reader.GetOrdinal("modifiedon"));
                    }
                    int modifiedby;
                    if (reader.IsDBNull(reader.GetOrdinal("modifiedby")))
                    {
                        modifiedby = 0;
                    }
                    else
                    {
                        modifiedby = reader.GetInt32(reader.GetOrdinal("modifiedby"));
                    }
                    bool settled = reader.GetBoolean(reader.GetOrdinal("settled"));
                    SortedList<int, decimal> allocations;
                    lstAllocations.TryGetValue(floatid, out allocations);
                    if (allocations == null)
                    {
                        allocations = new SortedList<int, decimal>();
                    }
                    decimal used;
                    if (lstUsed.ContainsKey(floatid))
                    {
                        lstUsed.TryGetValue(floatid, out used);
                    }
                    else
                    {
                        used = 0;
                    }
                    cFloat reqfloat = new cFloat(this.accountid, floatid, employeeid, currencyid, name, reason, requiredby, approved, approver, floatamount, exchangerate, stage, rejected, rejectreason, disputed, dispute, paid, datepaid, issuenum, basecurrency, settled, allocations, used, createdon, createdby, modifiedon, modifiedby, foreignAmount);
                    advances.Add(floatid, reqfloat);
                }

                reader.Close();
            }

            this.cache.Add(this.accountid, CacheArea, "0", advances);
            
            return advances;
        }

        /// <summary>
        /// Initialise data.
        /// </summary>
        private void InitialiseData()
        {
            this.list = this.cache.Get(this.accountid, CacheArea, "0") as SortedList<int, cFloat> ?? this.CacheList();
        }

        private void InvalidateCache(bool recache = true)
        {
            this.cache.Delete(this.accountid, CacheArea, "0");
            this.list = null;
            if (recache)
            {
                this.InitialiseData();
            }
        }

        /// <summary>
        /// Approve float.
        /// </summary>
        /// <param name="floatid">
        /// Float id.
        /// </param>
        private void approveFloat(int floatid)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(this.accountid));

            cFloat reqfloat = this.GetFloatById(floatid);
            reqfloat.approveAdvance();
            expdata.sqlexecute.Parameters.AddWithValue("@floatid", floatid);

            expdata.ExecuteSQL("update [floats] set approved = 1 where floatid = @floatid");
            expdata.sqlexecute.Parameters.Clear();

            this.InvalidateCache(false);
        }

        /// <summary>
        /// Convert totals.
        /// </summary>
        /// <param name="employeeid">
        /// Employee id.
        /// </param>
        /// <param name="currencyid">
        /// Currency id.
        /// </param>
        /// <param name="floatamount">
        /// Floatamount.
        /// </param>
        /// <returns>
        /// For details <see cref="decimal"/>.
        /// </returns>
        private decimal convertTotals(int employeeid, int currencyid, decimal floatamount)
        {
            cEmployees clsemployees = new cEmployees(this.accountid);
            Employee reqemp = clsemployees.GetEmployeeById(employeeid);
            // ONLY WORKS FOR A SINGLE SUBACCOUNT AT PRESENT.
            int subAccountID = reqemp.DefaultSubAccount; // subaccs.getFirstSubAccount().SubAccountID;

            double exchangerate = 0;
            cCurrencies clscurrencies = new cCurrencies(this.accountid, subAccountID);

            cCurrency reqcurrency = clscurrencies.getCurrencyById(currencyid);

            cMisc clsmisc = new cMisc(this.accountid);
            cGlobalProperties clsproperties = clsmisc.GetGlobalProperties(this.accountid);

            int basecurrency;

            if (reqemp.PrimaryCurrency != 0)
            {
                basecurrency = reqemp.PrimaryCurrency;
            }
            else
            {
                basecurrency = clsproperties.basecurrency;
            }

            reqcurrency = clscurrencies.getCurrencyById(basecurrency);

            int reqAccountBaseCurrency = clsmisc.GetGlobalProperties(this.accountid).basecurrency;

            if (reqAccountBaseCurrency == currencyid && reqemp.PrimaryCurrency == currencyid)
            {
                return floatamount;
            }

            exchangerate = reqcurrency.getExchangeRate(currencyid, DateTime.Today);

            if (exchangerate == 0)
            {
                return 0;
            }

            exchangerate = reqcurrency.getExchangeRate(currencyid, DateTime.Today);

            decimal convertedtotal = floatamount;

            decimal total = convertedtotal * (1 / (decimal)exchangerate);

            return total;
        }

        /// <summary>
        /// Get float allocations.
        /// </summary>
        /// <returns>
        /// For details <see cref="SortedList"/>.
        /// </returns>
        private SortedList<int, SortedList<int, decimal>> getFloatAllocations()
        {
            SortedList<int, SortedList<int, decimal>> lst = new SortedList<int, SortedList<int, decimal>>();
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(this.accountid));

            using (SqlDataReader reader = expdata.GetReader("select * from float_allocations"))
            {
                expdata.sqlexecute.Parameters.Clear();

                while (reader.Read())
                {
                    int floatid = reader.GetInt32(reader.GetOrdinal("floatid"));
                    int expenseid = reader.GetInt32(reader.GetOrdinal("expenseid"));
                    decimal amount = reader.GetDecimal(reader.GetOrdinal("amount"));
                    SortedList<int, decimal> allocations;
                    lst.TryGetValue(floatid, out allocations);
                    if (allocations == null)
                    {
                        allocations = new SortedList<int, decimal>();
                        lst.Add(floatid, allocations);
                    }
                    allocations.Add(expenseid, amount);
                }
                reader.Close();
            }
            return lst;
        }

        private SortedList<int, decimal> getFloatUsed()
        {
            SortedList<int, decimal> lstAllocated = new SortedList<int, decimal>();
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(this.accountid));

            using (SqlDataReader reader = expdata.GetReader("select floatid, sum(amount) from float_allocations group by floatid"))
            {
                while (reader.Read())
                {
                    if (reader.IsDBNull(0) == false)
                    {
                        int floatid = reader.GetInt32(0);
                        decimal floatused = reader.GetDecimal(1);
                        lstAllocated.Add(floatid, floatused);
                    }
                }
                reader.Close();
            }

            return lstAllocated;
        }

        private int getNextCheckerId(cFloat reqfloat, SignoffType signofftype, int relid)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(this.accountid));
            int nextcheckerid = 0;

            switch (signofftype)
            {
                case SignoffType.BudgetHolder:
                    expdata.sqlexecute.Parameters.AddWithValue("@relid", relid);
                    nextcheckerid = expdata.getcount("select employeeid from budgetholders where budgetholderid = @relid");
                    expdata.sqlexecute.Parameters.Clear();
                    break;
                case SignoffType.Employee:
                    nextcheckerid = relid;
                    break;
                case SignoffType.Team:
                case SignoffType.CostCodeOwner:
                case SignoffType.AssignmentSignOffOwner:
                    nextcheckerid = 0;
                    break;
                case SignoffType.LineManager:
                    cEmployees clsemployees = new cEmployees(this.accountid);
                    Employee reqemp = clsemployees.GetEmployeeById(reqfloat.employeeid);
                    nextcheckerid = reqemp.LineManager;
                    break;
            }

            return nextcheckerid;
        }

        private bool userOnHoliday(SignoffType signofftype, int relid)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(this.accountid));
            string strsql;
            bool userOnHoliday = false;
            int count = 0;
            DateTime today = DateTime.Today;
            expdata.sqlexecute.Parameters.AddWithValue("@relid", relid);
            switch (signofftype)
            {
                case SignoffType.BudgetHolder: //cost code
                    strsql = "select count(*) from holidays inner join budgetholders on budgetholders.employeeid = holidays.employeeid where budgetholders.employeeid = @relid and ('" + today.Year + "/" + today.Month + "/" + today.Day + "' between startdate and enddate)";
                    count = expdata.getcount(strsql);
                    if (count != 0)
                    {
                        userOnHoliday = true;
                    }
                    break;
                case SignoffType.Employee: //employee
                    strsql = "select count(*) as holidaycount from holidays where employeeid = @relid and ('" + today.Year + "/" + today.Month + "/" + today.Day + "' between startdate and enddate)";
                    count = expdata.getcount(strsql);
                    if (count != 0)
                    {
                        userOnHoliday = true;
                    }
                    break;
            }

            expdata.sqlexecute.Parameters.Clear();

            return userOnHoliday;
        }

        /// <summary>
        /// Validate an advance to dertermine if it is valid.
        /// </summary>
        /// <param name="advance">
        /// The instance of <see cref="cFloat"/>
        /// </param>
        /// <param name="employeeId">
        /// The employee id.
        /// </param>
        /// <param name="currencyId">
        /// The currency id.
        /// </param>
        /// <param name="floatId">
        /// The float id.
        /// </param>
        /// <param name="currencies">
        /// The an instance of <see cref="cCurrencies"/>.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool ValidateAdvance(cFloat advance, int employeeId, int currencyId, int floatId, cCurrencies currencies)
        {
            if (advance.employeeid == employeeId && advance.paid && advance.currencyid == currencyId)
            {
                decimal floatAvailable = advance.floatamount - advance.floatused;

                if (floatAvailable > 0 || advance.floatid == floatId)
                {
                    cCurrency currency = currencies.getCurrencyById(advance.currencyid);

                    if (currency != null)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Validate an advance to dertermine if it is valid.
        /// </summary>
        /// <param name="advance">
        /// The instance of <see cref="cFloat"/>
        /// </param>
        /// <param name="employeeId">
        /// The employee id.
        /// </param>
        /// <param name="currencies">
        /// The an instance of <see cref="cCurrencies"/>.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool ValidateAdvanceForUser(cFloat advance, int employeeId, cCurrencies currencies)
        {
            if (advance.employeeid == employeeId && advance.paid)
            {
                cCurrency currency = currencies.getCurrencyById(advance.currencyid);

                if (currency != null)
                {
                    return true;
                }
            }

            return false;
        }

        #endregion
    }
}
