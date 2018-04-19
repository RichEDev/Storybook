namespace Spend_Management
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Web.UI.WebControls;

    using SpendManagementLibrary;
    using SpendManagementLibrary.Helpers;
    using SpendManagementLibrary.Interfaces;

    using BusinessLogic.GeneralOptions.AddEditExpense;

    using Utilities.DistributedCaching;

    /// <summary>
    /// cESRAssignments class
    /// </summary>
    public class cESRAssignments
    {
        /// <summary>
        /// reference to application cache
        /// </summary>
        public Cache Cache;
        /// <summary>
        /// Account Id
        /// </summary>
        private int nAccountId;
        /// <summary>
        /// Employee Id whose assignments are in the collection
        /// </summary>
        private int nEmployeeId;
        /// <summary>
        /// Current collection ESR Assignment records
        /// </summary>
        Dictionary<int, cESRAssignment> list;

        public const string CacheKey = "esrAssignments";

        #region properties
        /// <summary>
        /// Gets the current customer account id
        /// </summary>
        public int AccountID
        {
            get { return nAccountId; }
        }
        /// <summary>
        /// Gets the current employee id
        /// </summary>
        public int EmployeeID
        {
            get { return nEmployeeId; }
        }
        /// <summary>
        /// Gets number of assignments associated with the employee
        /// </summary>
        public int Count
        {
            get { return list.Count; }
        }

        #endregion

        /// <summary>
        /// cESRAssignments constructor
        /// </summary>
        /// <param name="accountid">Customer account ID to obtain assignments for</param>
        /// <param name="employeeid">Employee ID to obtain assignment numbers for</param>
        public cESRAssignments(int accountid, int employeeid)
        {
            nAccountId = accountid;
            nEmployeeId = employeeid;

            Cache = new Cache();

            InitialiseData();

            return;
        }

        /// <summary>
        /// Populate collection from cache (or database)
        /// </summary>
        private void InitialiseData()
        {
            list = Cache.Get(AccountID, CacheKey, EmployeeID.ToString()) as Dictionary<int, cESRAssignment> ?? CacheAssignments();
            return;
        }

        /// <summary>
        /// Populate collection from database and store in cache
        /// </summary>
        /// <returns>Collection of cESRAssignment class entities</returns>
        private Dictionary<int, cESRAssignment> CacheAssignments()
        {
            var assignments = new Dictionary<int, cESRAssignment>();
            using (var db = new DatabaseConnection(cAccounts.getConnectionString(AccountID)))
            {
                const string sql = "getEsrAssignments";
                db.sqlexecute.Parameters.AddWithValue("@empId", EmployeeID);

                using (IDataReader reader = db.GetReader(sql, CommandType.StoredProcedure))
                {
                    while (reader.Read())
                    {
                        int sysintID = reader.GetInt32(reader.GetOrdinal("esrAssignID"));

                        long assignmentId = 0;
                        if (!reader.IsDBNull(reader.GetOrdinal("AssignmentID")))
                        {
                            assignmentId = reader.GetInt64(reader.GetOrdinal("AssignmentID"));
                        }
                        string asgnumber = reader.GetString(reader.GetOrdinal("AssignmentNumber"));
                        DateTime earlieststart;

                        if (reader.IsDBNull(reader.GetOrdinal("EarliestAssignmentStartDate")))
                        {
                            earlieststart = new DateTime(1900, 01, 01);
                        }
                        else
                        {
                            earlieststart = reader.GetDateTime(reader.GetOrdinal("EarliestAssignmentStartDate"));
                        }

                        DateTime? finalasgdate = null;
                        if (!reader.IsDBNull(reader.GetOrdinal("FinalAssignmentEndDate")))
                        {
                            finalasgdate = reader.GetDateTime(reader.GetOrdinal("FinalAssignmentEndDate"));
                        }

                        ESRAssignmentStatus status = ESRAssignmentStatus.NotSpecified;
                        if (!reader.IsDBNull(reader.GetOrdinal("AssignmentStatus")))
                        {
                            string asgstatus = reader.GetString(reader.GetOrdinal("AssignmentStatus"));
                            switch (asgstatus)
                            {
                                case "Acting Up":
                                    status = ESRAssignmentStatus.ActingUp;
                                    break;
                                case "Active Assignment":
                                    status = ESRAssignmentStatus.ActiveAssignment;
                                    break;
                                case "Assignment Costing Deletion":
                                    status = ESRAssignmentStatus.AssignmentCostingDeletion;
                                    break;
                                case "Career Break":
                                    status = ESRAssignmentStatus.CareerBreak;
                                    break;
                                case "Internal Secondment":
                                    status = ESRAssignmentStatus.InternalSecondment;
                                    break;
                                case "Maternity":
                                    status = ESRAssignmentStatus.Maternity;
                                    break;
                                case "Out on External Secondment - Paid":
                                    status = ESRAssignmentStatus.OutOnExternalSecondment_Paid;
                                    break;
                                case "Out on External Secondment - Unpaid":
                                    status = ESRAssignmentStatus.OutOnExternalSecondment_Unpaid;
                                    break;
                                case "Suspend Assignment":
                                    status = ESRAssignmentStatus.SuspendAssignment;
                                    break;
                                case "Suspend No Pay":
                                    status = ESRAssignmentStatus.SuspendNoPay;
                                    break;
                                case "Suspend With Pay":
                                    status = ESRAssignmentStatus.SuspendWithPay;
                                    break;
                                case "Terminated":
                                    status = ESRAssignmentStatus.Terminator;
                                    break;
                                case "Offer Accepted":
                                    status = ESRAssignmentStatus.OfferAccepted;
                                    break;
                                case "Inactive Not Worked":
                                    status = ESRAssignmentStatus.InactiveNotWorked;
                                    break;
                            }
                        }

                        string paytype = string.Empty;
                        if (!reader.IsDBNull(reader.GetOrdinal("PayrollPayType")))
                        {
                            paytype = reader.GetString(reader.GetOrdinal("PayrollPayType"));
                        }

                        string prollname = string.Empty;
                        if (!reader.IsDBNull(reader.GetOrdinal("PayrollName")))
                        {
                            prollname = reader.GetString(reader.GetOrdinal("PayrollName"));
                        }

                        string prolltype = string.Empty;
                        if (!reader.IsDBNull(reader.GetOrdinal("PayrollPeriodType")))
                        {
                            prolltype = reader.GetString(reader.GetOrdinal("PayrollPeriodType"));
                        }

                        string addr1 = string.Empty;
                        if (!reader.IsDBNull(reader.GetOrdinal("AssignmentAddressLine1")))
                        {
                            addr1 = reader.GetString(reader.GetOrdinal("AssignmentAddressLine1"));
                        }

                        string addr2 = string.Empty;
                        if (!reader.IsDBNull(reader.GetOrdinal("AssignmentAddressLine2")))
                        {
                            addr2 = reader.GetString(reader.GetOrdinal("AssignmentAddressLine2"));
                        }

                        string town = string.Empty;
                        if (!reader.IsDBNull(reader.GetOrdinal("AssignmentAddressTown")))
                        {
                            town = reader.GetString(reader.GetOrdinal("AssignmentAddressTown"));
                        }

                        string county = string.Empty;
                        if (!reader.IsDBNull(reader.GetOrdinal("AssignmentAddressCounty")))
                        {
                            county = reader.GetString(reader.GetOrdinal("AssignmentAddressCounty"));
                        }

                        string pcode = string.Empty;
                        if (!reader.IsDBNull(reader.GetOrdinal("AssignmentAddressPostcode")))
                        {
                            pcode = reader.GetString(reader.GetOrdinal("AssignmentAddressPostcode"));
                        }

                        string country = string.Empty;
                        if (!reader.IsDBNull(reader.GetOrdinal("AssignmentAddressCountry")))
                        {
                            country = reader.GetString(reader.GetOrdinal("AssignmentAddressCountry"));
                        }

                        bool sflag = false;
                        if (!reader.IsDBNull(reader.GetOrdinal("SupervisorFlag")))
                        {
                            var textFlag = reader.GetString(reader.GetOrdinal("SupervisorFlag"));
                            if (string.IsNullOrEmpty(textFlag))
                            {
                                textFlag = "0";
                            }

                            if (textFlag == "Y")
                            {
                                textFlag = "1";
                            }

                            if (textFlag == "N")
                            {
                                textFlag = "0";
                            }

                            sflag = Convert.ToBoolean(Convert.ToInt32(textFlag));
                        }

                        string sasgnum = string.Empty;
                        if (!reader.IsDBNull(reader.GetOrdinal("SupervisorAssignmentNumber")))
                        {
                            sasgnum = reader.GetString(reader.GetOrdinal("SupervisorAssignmentNumber"));
                        }

                        string sempnum = string.Empty;
                        if (!reader.IsDBNull(reader.GetOrdinal("SupervisorEmployeeNumber")))
                        {
                            sempnum = reader.GetString(reader.GetOrdinal("SupervisorEmployeeNumber"));
                        }

                        string sfname = string.Empty;
                        if (!reader.IsDBNull(reader.GetOrdinal("SupervisorFullName")))
                        {
                            sfname = reader.GetString(reader.GetOrdinal("SupervisorFullName"));
                        }

                        string accru = string.Empty;
                        if (!reader.IsDBNull(reader.GetOrdinal("AccrualPlan")))
                        {
                            accru = reader.GetString(reader.GetOrdinal("AccrualPlan"));
                        }

                        string empcat = string.Empty;
                        if (!reader.IsDBNull(reader.GetOrdinal("EmployeeCategory")))
                        {
                            empcat = reader.GetString(reader.GetOrdinal("EmployeeCategory"));
                        }

                        string asgcat = string.Empty;
                        if (!reader.IsDBNull(reader.GetOrdinal("AssignmentCategory")))
                        {
                            asgcat = reader.GetString(reader.GetOrdinal("AssignmentCategory"));
                        }

                        bool primaryasg = false;
                        if (!reader.IsDBNull(reader.GetOrdinal("PrimaryAssignment")))
                        {
                            primaryasg = reader.GetBoolean(reader.GetOrdinal("PrimaryAssignment"));
                        }

                        string esrprimaryasgstring = string.Empty;
                        if (!reader.IsDBNull(reader.GetOrdinal("PrimaryAssignmentString")))
                        {
                            esrprimaryasgstring = reader.GetString(reader.GetOrdinal("PrimaryAssignmentString"));
                        }

                        decimal nhours = 0;
                        if (!reader.IsDBNull(reader.GetOrdinal("NormalHours")))
                        {
                            nhours = Convert.ToDecimal(reader.GetDouble(reader.GetOrdinal("NormalHours")));
                        }

                        string nhfreq = string.Empty;
                        if (!reader.IsDBNull(reader.GetOrdinal("NormalHoursFrequency")))
                        {
                            nhfreq = reader.GetString(reader.GetOrdinal("NormalHoursFrequency"));
                        }

                        decimal gradehours = 0;
                        if (!reader.IsDBNull(reader.GetOrdinal("GradeContractHours")))
                        {
                            gradehours = Convert.ToDecimal(reader.GetDouble(reader.GetOrdinal("GradeContractHours")));
                        }

                        decimal nosess = 0;
                        if (!reader.IsDBNull(reader.GetOrdinal("NoOfSessions")))
                        {
                            nosess = Convert.ToDecimal(reader.GetDouble(reader.GetOrdinal("NoOfSessions")));
                        }

                        string sessfreq = string.Empty;
                        if (!reader.IsDBNull(reader.GetOrdinal("SessionsFrequency")))
                        {
                            sessfreq = reader.GetString(reader.GetOrdinal("SessionsFrequency"));
                        }

                        string workpatdetails = string.Empty;
                        if (!reader.IsDBNull(reader.GetOrdinal("WorkPatternDetails")))
                        {
                            workpatdetails = reader.GetString(reader.GetOrdinal("WorkPatternDetails"));
                        }

                        string workpatstart = string.Empty;
                        if (!reader.IsDBNull(reader.GetOrdinal("WorkPatternStartDay")))
                        {
                            workpatstart = reader.GetString(reader.GetOrdinal("WorkPatternStartDay"));
                        }

                        string flexworkpat = string.Empty;
                        if (!reader.IsDBNull(reader.GetOrdinal("FlexibleWorkingPattern")))
                        {
                            flexworkpat = reader.GetString(reader.GetOrdinal("FlexibleWorkingPattern"));
                        }

                        string asched = string.Empty;
                        if (!reader.IsDBNull(reader.GetOrdinal("AvailabilitySchedule")))
                        {
                            asched = reader.GetString(reader.GetOrdinal("AvailabilitySchedule"));
                        }

                        string org = string.Empty;
                        if (!reader.IsDBNull(reader.GetOrdinal("Organisation")))
                        {
                            org = reader.GetString(reader.GetOrdinal("Organisation"));
                        }

                        string legal = string.Empty;
                        if (!reader.IsDBNull(reader.GetOrdinal("LegalEntity")))
                        {
                            legal = reader.GetString(reader.GetOrdinal("LegalEntity"));
                        }

                        string pos = string.Empty;
                        if (!reader.IsDBNull(reader.GetOrdinal("PositionName")))
                        {
                            pos = reader.GetString(reader.GetOrdinal("PositionName"));
                        }

                        string jobrole = string.Empty;
                        if (!reader.IsDBNull(reader.GetOrdinal("JobRole")))
                        {
                            jobrole = reader.GetString(reader.GetOrdinal("JobRole"));
                        }

                        string occcode = string.Empty;
                        if (!reader.IsDBNull(reader.GetOrdinal("OccupationCode")))
                        {
                            occcode = reader.GetString(reader.GetOrdinal("OccupationCode"));
                        }

                        string asgloc = string.Empty;
                        if (!reader.IsDBNull(reader.GetOrdinal("AssignmentLocation")))
                        {
                            asgloc = reader.GetString(reader.GetOrdinal("AssignmentLocation"));
                        }

                        string grade = string.Empty;
                        if (!reader.IsDBNull(reader.GetOrdinal("Grade")))
                        {
                            grade = reader.GetString(reader.GetOrdinal("Grade"));
                        }

                        string jobname = string.Empty;
                        if (!reader.IsDBNull(reader.GetOrdinal("JobName")))
                        {
                            jobname = reader.GetString(reader.GetOrdinal("JobName"));
                        }

                        string group = string.Empty;
                        if (!reader.IsDBNull(reader.GetOrdinal("Group")))
                        {
                            group = reader.GetString(reader.GetOrdinal("Group"));
                        }

                        string tanda = string.Empty;
                        if (!reader.IsDBNull(reader.GetOrdinal("TAndAFlag")))
                        {
                            tanda = reader.GetString(reader.GetOrdinal("TAndAFlag"));
                        }

                        string workoptout = string.Empty;
                        if (!reader.IsDBNull(reader.GetOrdinal("NightWorkerOptOut")))
                        {
                            workoptout = reader.GetString(reader.GetOrdinal("NightWorkerOptOut"));
                        }

                        //string projectedhiredate = "";
                        DateTime? projectedhiredate = null;
                        if (!reader.IsDBNull(reader.GetOrdinal("ProjectedHireDate")))
                        {
                            projectedhiredate = reader.GetDateTime(reader.GetOrdinal("ProjectedHireDate"));
                            //projectedhiredate = reader.GetString(reader.GetOrdinal("ProjectedHireDate"));
                            //hiredate = new DateTime(Convert.ToInt32(projectedhiredate.Substring(0, 4)), Convert.ToInt32(projectedhiredate.Substring(5, 2)), Convert.ToInt32(projectedhiredate.Substring(7, 2)));
                        }

                        int? vacancyid = null;
                        if (!reader.IsDBNull(reader.GetOrdinal("VacancyID")))
                        {
                            vacancyid = reader.GetInt32(reader.GetOrdinal("VacancyID"));
                        }

                        ///esrLocationId
                        long? esrLocationId = null;
                        if (!reader.IsDBNull(reader.GetOrdinal("esrlocationid")))
                        {
                            esrLocationId = reader.GetInt64(reader.GetOrdinal("esrlocationid"));
                        }

                        bool active = reader.GetBoolean(reader.GetOrdinal("Active"));
                        DateTime? createdon = null;
                        if (!reader.IsDBNull(reader.GetOrdinal("CreatedOn")))
                        {
                            createdon = reader.GetDateTime(reader.GetOrdinal("CreatedOn"));
                        }

                        int? createdby = null;
                        if (!reader.IsDBNull(reader.GetOrdinal("CreatedBy")))
                        {
                            createdby = reader.GetInt32(reader.GetOrdinal("CreatedBy"));
                        }

                        DateTime? modifiedon = null;
                        if (!reader.IsDBNull(reader.GetOrdinal("ModifiedOn")))
                        {
                            modifiedon = reader.GetDateTime(reader.GetOrdinal("ModifiedOn"));
                        }

                        int? modifiedby = null;
                        if (!reader.IsDBNull(reader.GetOrdinal("ModifiedBy")))
                        {
                            modifiedby = reader.GetInt32(reader.GetOrdinal("ModifiedBy"));
                        }

                        DateTime? effectiveStartDate = null;
                        if (!reader.IsDBNull(reader.GetOrdinal("effectiveStartDate")))
                        {
                            effectiveStartDate = reader.GetDateTime(reader.GetOrdinal("effectiveStartDate"));
                        }

                        DateTime? effectiveEndDate = null;
                        if (!reader.IsDBNull(reader.GetOrdinal("effectiveEndDate")))
                        {
                            effectiveEndDate = reader.GetDateTime(reader.GetOrdinal("effectiveEndDate"));
                        }

                        IOwnership signOffOwner = null;
                        if (reader["SignOffOwner"] != DBNull.Value)
                        {
                            signOffOwner = Ownership.Parse(nAccountId, null, (string)reader["SignOffOwner"]);
                        }

                        var ass = new cESRAssignment(
                            assignmentId,
                            sysintID,
                            asgnumber,
                            earlieststart,
                            finalasgdate,
                            status,
                            paytype,
                            prollname,
                            prolltype,
                            addr1,
                            addr2,
                            town,
                            county,
                            pcode,
                            country,
                            sflag,
                            sasgnum,
                            sempnum,
                            sfname,
                            accru,
                            empcat,
                            asgcat,
                            primaryasg,
                            esrprimaryasgstring,
                            nhours,
                            nhfreq,
                            gradehours,
                            nosess,
                            sessfreq,
                            workpatdetails,
                            workpatstart,
                            flexworkpat,
                            asched,
                            org,
                            legal,
                            pos,
                            jobrole,
                            occcode,
                            asgloc,
                            grade,
                            jobname,
                            group,
                            tanda,
                            workoptout,
                            projectedhiredate,
                            vacancyid,
                            esrLocationId,
                            active,
                            signOffOwner,
                            createdon,
                            createdby,
                            modifiedon,
                            modifiedby,
                            effectiveStartDate,
                            effectiveEndDate);
                        assignments.Add(sysintID, ass);
                    }
                    reader.Close();
                }
            }

            Cache.Add(AccountID, CacheKey, EmployeeID.ToString(), assignments);

            return assignments;
        }

        /// <summary>
        /// Force an update of the cache in this object
        /// </summary>
        public static void ResetCache(int accountId, int employeeId)
        {
            new Cache().Delete(accountId, CacheKey, employeeId.ToString());

        }

        /// <summary>
        /// Obtain a particular assignment by its unique id
        /// </summary>
        /// <param name="esrAssignID">esrAssignID to retrieve</param>
        /// <returns></returns>
        public cESRAssignment getAssignmentById(int esrAssignID)
        {
            if (list.ContainsKey(esrAssignID))
            {
                return list[esrAssignID];
            }
            return null;
        }

        /// <summary>
        /// Get the Assignment object by the Assigment Number
        /// </summary>
        /// <param name="AssignmentNumber">Unique Assignment Number</param>
        /// <returns>ESR Assignment Object</returns>
        public cESRAssignment getAssignmentByAssignmentNumber(string AssignmentNumber)
        {
            foreach (cESRAssignment assignment in list.Values)
            {
                if (assignment.assignmentnumber == AssignmentNumber)
                {
                    return assignment;
                }
            }
            return null;
        }

        /// <summary>
        /// Gets list item array for drop down list (only includes items that are active and 
        /// </summary>
        /// <param name="includeNone">Whether to add "None" to the list</param>
        /// <param name="claimDate">The date of the claim</param>
        /// <returns>Array of ListItems</returns>
        public ListItem[] GetAvailableAssignmentListItems(bool includeNone, DateTime claimDate)
        {
            var items = new Dictionary<string, ListItem>();
            var subAccounts = new cAccountSubAccounts(this.AccountID);
            var subAccountProperties = subAccounts.getFirstSubAccount().SubAccountProperties;

            foreach (KeyValuePair<int, cESRAssignment> item in this.list)
            {
                var assignment = item.Value;

                if (items.ContainsKey(assignment.assignmentnumber))
                {
                    continue;
                }

                if (this.IsAssignmentIsValidForExpenseDate(claimDate, assignment))
                {
                    string assignmentText = GenerateAssignmentText(assignment, subAccountProperties);
                    var newItem = new ListItem(assignmentText, assignment.sysinternalassignmentid.ToString());
                    items.Add(assignment.assignmentnumber, newItem);
                }
            }

            List<ListItem> assignments = items.Values.ToList();

            if (includeNone)
            {
                assignments.Insert(0, new ListItem("[None]", "0"));
            }

            return assignments.ToArray();
        }

        /// <summary>
        /// Checks if the assignment is valid for the expense date
        /// </summary>
        /// <param name="expenseDate">The date of the expense</param>
        /// <param name="assignment">The <see cref="cESRAssignment">cESRAssignment</see></param>
        /// <returns></returns>
        public bool IsAssignmentIsValidForExpenseDate(DateTime expenseDate, cESRAssignment assignment)
        {
            if (assignment.EffectiveStartDate == null)
            {
                if (!assignment.active || assignment.earliestassignmentstartdate > expenseDate)
                {
                    return false;
                }
            }
            else
            {
                if (assignment.EffectiveStartDate > expenseDate
                    || (assignment.EffectiveEndDate != null && assignment.EffectiveEndDate < expenseDate))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Generates the assignment text to be shown in the assignment drop down list on add/edit expense
        /// </summary>
        /// <param name="assignment">The <see cref="cESRAssignment">cESRAssignment</see></param>
        /// <param name="subAccountProperties">The <see cref="cAccountProperties">cAccountProperties</see> </param>
        /// <returns>The assignment text</returns>
        public string GenerateAssignmentText(cESRAssignment assignment, cAccountProperties subAccountProperties)
        {
            string assignmentText = assignment.assignmentnumber;
            string extraInfo;

            switch (subAccountProperties.IncludeAssignmentDetails)
            {
                case IncludeEsrDetails.None:
                    extraInfo = assignment.assignmentaddress1;
                    break;
                case IncludeEsrDetails.PayPoint:
                    extraInfo = assignment.@group.Split('|')[0];
                    break;
                case IncludeEsrDetails.JobRole:
                    extraInfo = assignment.jobname;
                    break;
                case IncludeEsrDetails.PositionNameAndCategory:
                    extraInfo = $"{assignment.positionname}".Trim();
                    break;
                default:
                    extraInfo = string.Empty;
                    break;
            }

            if (!string.IsNullOrEmpty(extraInfo))
            {
                assignmentText = $"{assignment.assignmentnumber} [{extraInfo}]";
            }

            return assignmentText;
        }

        /// <summary>
        /// Archive a particular ESR Assignment record
        /// </summary>
        /// <param name="assignmentRecId"></param>
        public void archiveAssignmentNumber(int assignmentRecId)
        {
            DBConnection db = new DBConnection(cAccounts.getConnectionString(AccountID));

            const string sql = "setEsrAssignmentActive";
            db.sqlexecute.Parameters.AddWithValue("@active", 0);
            db.sqlexecute.Parameters.AddWithValue("@recId", assignmentRecId);
            db.ExecuteProc(sql);
            ResetCache(AccountID, EmployeeID);
            InitialiseData();
            return;
        }

        /// <summary>
        /// Archive ALL assignment numbers associated with the employee
        /// </summary>
        public void archiveAllEmployeeAssignmentNumbers()
        {
            foreach (KeyValuePair<int, cESRAssignment> e in list)
            {
                cESRAssignment easg = (cESRAssignment)e.Value;

                archiveAssignmentNumber(easg.sysinternalassignmentid);
            }
            return;
        }

        /// <summary>
        /// getCurrentAssignmentsGrid: gets a read only grid of ESR Assignment records currently assigned to the employee
        /// </summary>
        /// <returns>HTML Table</returns>
        public string[] getCurrentAssignmentsGrid(bool updateable)
        {
            var user = cMisc.GetCurrentUser();
            string sql = string.Format("select esrAssignID, AssignmentNumber, EarliestAssignmentStartDate, Active{0}, PrimaryAssignment, EffectiveStartDate, EffectiveEndDate from dbo.esr_assignments", user.Account.IsNHSCustomer ? ", AssignmentID" : "");
            var fields = new cFields(AccountID);

            var grid = new cGridNew(AccountID, EmployeeID, "ESRAssignmentGrid", sql);
            var employeeId = fields.GetFieldByID(new Guid("0CFDCDCD-1F51-4578-BE89-90610D6D7F7D"));
            grid.addFilter(employeeId, ConditionType.Equals, new object[] { EmployeeID }, null, ConditionJoiner.None);
            grid.getColumnByName("esrAssignID").hidden = true;
            grid.KeyField = "esrAssignID";
            grid.EmptyText = "There are not currently any assignments associated";
            ((cFieldColumn)grid.getColumnByName("Active")).addValueListItem(0, "No");
            ((cFieldColumn)grid.getColumnByName("Active")).addValueListItem(1, "Yes");
            ((cFieldColumn)grid.getColumnByName("PrimaryAssignment")).addValueListItem(0, "No");
            ((cFieldColumn)grid.getColumnByName("PrimaryAssignment")).addValueListItem(1, "Yes");
            if (user.Account.IsNHSCustomer)
            {
                grid.addTwoStateEventColumn("EsrDetails", (cFieldColumn)grid.getColumnByName("AssignmentID"), 0, null, "/static/icons/16/plain/magnifying_glass.png", "javascript:showEsrDetailsModal(2,{value});", "Show ESR Details", "Show Esr Details", "", "", "", "");
            }

            if (updateable)
            {
                grid.enableupdating = true;
                grid.enabledeleting = true;
                grid.deletelink = "javascript:deleteESRAssignment({esrAssignID});";
                grid.editlink = "javascript:addNewESRAssignment = false;editESRAssignment({esrAssignID});";
            }
            grid.EnableSorting = true;

            List<string> retVals = new List<string>();
            retVals.Add(grid.GridID);
            retVals.AddRange(grid.generateGrid());
            return retVals.ToArray();
        }

        

        /// <summary>
        /// Gets ESR Assignment Records currently associated to the employee
        /// </summary>
        /// <returns>Dictionary collection of ESR Assignment class entities</returns>
        public Dictionary<int, cESRAssignment> getAssignmentsAssociated()
        {
            return list;
        }

        /// <summary>
        /// Get a count of the active asssignments for the employee
        /// </summary>
        /// <returns></returns>
        public int ActiveAssignmentCount()
        {
            int counter = 0;

            foreach (cESRAssignment Ass in list.Values)
            {
                if (Ass.active && Ass.earliestassignmentstartdate <= DateTime.Today)
                {
                    counter++;
                }
            }
            return counter;
        } 
        /// <summary>
        /// Get a count of the active asssignments for the employee
        /// </summary>
        /// <returns></returns>
        public int ActiveAssignmentCount(DateTime date)
        {
            int counter = 0;

            foreach (cESRAssignment Ass in list.Values)
            {
                if (Ass.esrLocationId == null)
                {
                    if (Ass.active && Ass.earliestassignmentstartdate <= date && (Ass.finalassignmentenddate == null || Ass.finalassignmentenddate >= date))
                    {
                        counter++;
                    }
                }
                else
                {
                    if (Ass.EffectiveStartDate <= date && (Ass.EffectiveEndDate == null || Ass.EffectiveEndDate >= date))
                    {
                        counter++;
                    }
                }

            }
            return counter;
        }
        /// <summary>
        /// Save the ESR Assignment to the database
        /// </summary>
        /// <param name="assignment"></param>
        /// <returns></returns>
        public int saveESRAssignment(cESRAssignment assignment)
        {
            int esrassignid;
            DBConnection data = new DBConnection(cAccounts.getConnectionString(AccountID));
            data.sqlexecute.Parameters.AddWithValue("@esrAssignID", assignment.sysinternalassignmentid);
            data.sqlexecute.Parameters.AddWithValue("@employeeid", EmployeeID);
            data.sqlexecute.Parameters.AddWithValue("@assignmentNumber", assignment.assignmentnumber);
            data.sqlexecute.Parameters.AddWithValue("@active", Convert.ToByte(assignment.active));
            data.sqlexecute.Parameters.AddWithValue("@primaryAssignment", Convert.ToByte(assignment.primaryassignment));
            data.sqlexecute.Parameters.AddWithValue("@earliestassignmentstartdate", assignment.earliestassignmentstartdate);

            if (assignment.finalassignmentenddate == null)
            {
                data.sqlexecute.Parameters.AddWithValue("@finalassignmentenddate", DBNull.Value);
            }
            else
            {
                data.sqlexecute.Parameters.AddWithValue("@finalassignmentenddate", assignment.finalassignmentenddate);
            }
            if (assignment.ModifiedBy == null)
            {
                data.sqlexecute.Parameters.AddWithValue("@userid", assignment.CreatedBy);
                data.sqlexecute.Parameters.AddWithValue("@date", assignment.CreatedOn);
            }
            else
            {
                data.sqlexecute.Parameters.AddWithValue("@userid", assignment.ModifiedBy);
                data.sqlexecute.Parameters.AddWithValue("@date", assignment.ModifiedOn);
            }
            data.sqlexecute.Parameters.AddWithValue("@supervisorAssignmentNumber", assignment.supervisorassignmentnumber);
            data.sqlexecute.Parameters.AddWithValue("@signOffOwner", (assignment.SignOffOwner != null ? (object)assignment.SignOffOwner.CombinedItemKey : DBNull.Value));
            CurrentUser currentUser = cMisc.GetCurrentUser();

            if (currentUser != null)
            {

                data.sqlexecute.Parameters.AddWithValue("@CUemployeeID", currentUser.EmployeeID);
                if (currentUser.isDelegate)
                {
                    data.sqlexecute.Parameters.AddWithValue("@CUdelegateID", currentUser.Delegate.EmployeeID);
                }
                else
                {
                    data.sqlexecute.Parameters.AddWithValue("@CUdelegateID", DBNull.Value);
                }
            }
            else
            {
                data.sqlexecute.Parameters.AddWithValue("@CUemployeeID", -1);
                data.sqlexecute.Parameters.AddWithValue("@CUdelegateID", DBNull.Value);
            }

            data.sqlexecute.Parameters.Add("@identity", System.Data.SqlDbType.Int);
            data.sqlexecute.Parameters["@identity"].Direction = System.Data.ParameterDirection.ReturnValue;
            data.ExecuteProc("saveESRAssignmentNumber");
            esrassignid = (int)data.sqlexecute.Parameters["@identity"].Value;
            data.sqlexecute.Parameters.Clear();
            ResetCache(AccountID, EmployeeID);
            InitialiseData();
            return esrassignid;
        }

        public void deleteESRAssignment(int id)
        {
            DBConnection data = new DBConnection(cAccounts.getConnectionString(AccountID));
            data.sqlexecute.Parameters.Add("@esrAssignID", id);
            CurrentUser currentUser = cMisc.GetCurrentUser();
            data.sqlexecute.Parameters.AddWithValue("@CUemployeeID", currentUser.EmployeeID);
            if (currentUser.isDelegate == true)
            {
                data.sqlexecute.Parameters.AddWithValue("@CUdelegateID", currentUser.Delegate.EmployeeID);
            }
            else
            {
                data.sqlexecute.Parameters.AddWithValue("@CUdelegateID", DBNull.Value);
            }
            data.ExecuteProc("deleteESRAssignment");
            data.sqlexecute.Parameters.Clear();
            ResetCache(AccountID, EmployeeID);
            InitialiseData();
        }

        /// <summary>
        /// Returns the list of cESRAssignments from the cache.
        /// This method is currently only used by SpendManagementApi, for gets.
        /// The public list shouldn't be edited.
        /// </summary>
        public Dictionary<int, cESRAssignment> GetCacheList()
        {
            return list;
        }

        /// <summary>
        /// Get the Assignment object by the Assigment ID
        /// </summary>
        /// <param name="AssignmentID">Assignment ID</param>
        /// <returns>ESR Assignment Object</returns>
        public cESRAssignment GetAssignmentByAssignmentId(int AssignmentID)
        {
            foreach (cESRAssignment assignment in this.list.Values)
            {
                if (assignment.assignmentid == AssignmentID)
                {
                    return assignment;
                }
            }
            return null;
        }
    }
}
