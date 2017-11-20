using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using ExpensesLibrary;
using expenses.Old_App_Code;
using System.Web.Caching;
using System.Data.SqlClient;
using System.Collections;
using SpendManagementLibrary;

namespace expenses
{



    public class cClaims
    {
        
        string strsql;
        System.Web.Caching.Cache Cache = (System.Web.Caching.Cache)System.Web.HttpRuntime.Cache;
        private int nAccountid;

        public cClaims(int accountid)
        {
            nAccountid = accountid;
        }

        #region properties
        public int accountid
        {
            get { return nAccountid; }
        }
        #endregion
        public cClaim getClaimById(int claimid)
        {
            cClaim claim = (cClaim)System.Web.HttpRuntime.Cache["claim" + accountid + claimid];
            if (claim == null)
            {
                claim = getClaimFromDB(claimid);
            }
            return claim;
        }

        private cClaim  getClaimFromDB(int claimid)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            Dictionary<int, object> userdefined;
            cClaim newclaim = null;
            cNewUserDefined clsuserdefined = null;
            int claimno, empid;
            bool approved;

            bool paid;
            DateTime datesubmitted, datepaid;
            string description;
            ClaimStatus status;
            int teamid, checkerid, stage, currencyid, createdby, modifiedby;
            DateTime createdon, modifiedon;
            bool submitted;
            string name;

            AggregateCacheDependency aggdep = new AggregateCacheDependency();
            

            System.Data.SqlClient.SqlDataReader reader;

            expdata.sqlexecute.Parameters.AddWithValue("@claimid", claimid);
            SortedList<string, object> parameters = new SortedList<string, object>();
            parameters.Add("@claimid", claimid);
            strsql = "SELECT     claimhistoryid, claimid, datestamp, stage, comment, refnum, employeeid FROM dbo.claimhistory where claimid = @claimid";
            SqlCacheDependency historydep = expdata.CreateSQLCacheDependency(strsql, parameters);
            strsql = "SELECT     expenseid, bmiles, pmiles, reason, receipt, net, vat, total, subcatid, date, staff, others, companyid, returned, home, refnum, claimid, plitres, blitres, allowanceamount, currencyid, attendees, tip, countryid, foreignvat, convertedtotal, exchangerate, tempallow, reasonid, normalreceipt, receiptattached, allowancestartdate, allowanceenddate, carid, allowancededuct, allowanceid, nonights, quantity, directors, amountpayable, hotelid, primaryitem, norooms, vatnumber, personalguests, remoteworkers, accountcode, pencepermile, basecurrency, globalexchangerate, globalbasecurrency, globaltotal, itemtype, mileageid, transactionid, journey_unit, CreatedOn, CreatedBy, ModifiedOn, ModifiedBy FROM dbo.savedexpenses_current where claimid = @claimid";
            SqlCacheDependency savedcurrentdep = expdata.CreateSQLCacheDependency(strsql, parameters);

            strsql = "SELECT claimid, claimno, employeeid, approved, paid, datesubmitted, datepaid, description, status, teamid, checkerid, stage, submitted, name, currencyid, CreatedOn, CreatedBy, ModifiedOn, ModifiedBy FROM dbo.claims_base where claimid = @claimid";
            SqlCacheDependency claimsdep = expdata.CreateSQLCacheDependency(strsql, parameters);

           

            reader = expdata.GetReader(strsql);
            expdata.sqlexecute.Parameters.Clear();

            while (reader.Read())
            {
                if (clsuserdefined == null)
                {
                    clsuserdefined = new cNewUserDefined(accountid, cAccounts.getConnectionString(accountid), new cTables(accountid)); ;
                }
                claimid = reader.GetInt32(reader.GetOrdinal("claimid"));
                claimno = reader.GetInt32(reader.GetOrdinal("claimno"));
                empid = reader.GetInt32(reader.GetOrdinal("employeeid"));

                approved = reader.GetBoolean(reader.GetOrdinal("approved"));

                paid = reader.GetBoolean(reader.GetOrdinal("paid"));
                if (reader.IsDBNull(reader.GetOrdinal("datesubmitted")) == false)
                {
                    datesubmitted = reader.GetDateTime(reader.GetOrdinal("datesubmitted"));
                }
                else
                {
                    datesubmitted = new DateTime(1900, 01, 01);
                }
                if (reader.IsDBNull(reader.GetOrdinal("datepaid")) == false)
                {
                    datepaid = reader.GetDateTime(reader.GetOrdinal("datepaid"));
                }
                else
                {
                    datepaid = new DateTime(1900, 01, 01);
                   
                }
                if (reader.IsDBNull(reader.GetOrdinal("description")) == false)
                {
                    description = reader.GetString(reader.GetOrdinal("description"));
                }
                else
                {
                    description = "";
                }
                status = (ClaimStatus)reader.GetByte(reader.GetOrdinal("status"));
                if (reader.IsDBNull(reader.GetOrdinal("teamid")) == false)
                {
                    teamid = reader.GetInt32(reader.GetOrdinal("teamid"));
                }
                else
                {
                    teamid = 0;
                }
                if (reader.IsDBNull(reader.GetOrdinal("checkerid")) == false)
                {
                    checkerid = reader.GetInt32(reader.GetOrdinal("checkerid"));
                }
                else
                {
                    checkerid = 0;
                }
                stage = reader.GetInt32(reader.GetOrdinal("stage"));
                submitted = reader.GetBoolean(reader.GetOrdinal("submitted"));
                name = reader.GetString(reader.GetOrdinal("name"));



                if (reader.IsDBNull(reader.GetOrdinal("currencyid")) == false)
                {
                    currencyid = reader.GetInt32(reader.GetOrdinal("currencyid"));
                }
                else
                {
                    currencyid = 0;
                }

                if (reader.IsDBNull(reader.GetOrdinal("CreatedOn")) == true)
                {
                    createdon = new DateTime(1900, 01, 01);
                }
                else
                {
                    createdon = reader.GetDateTime(reader.GetOrdinal("CreatedOn"));
                }
                if (reader.IsDBNull(reader.GetOrdinal("CreatedBy")) == true)
                {
                    createdby = 0;
                }
                else
                {
                    createdby = reader.GetInt32(reader.GetOrdinal("CreatedBy"));
                }
                if (reader.IsDBNull(reader.GetOrdinal("ModifiedOn")) == true)
                {
                    modifiedon = new DateTime(1900, 01, 01);
                }
                else
                {
                    modifiedon = reader.GetDateTime(reader.GetOrdinal("ModifiedOn"));
                }
                if (reader.IsDBNull(reader.GetOrdinal("ModifiedBy")) == true)
                {
                    modifiedby = 0;
                }
                else
                {
                    modifiedby = reader.GetInt32(reader.GetOrdinal("ModifiedBy"));
                }
                userdefined = clsuserdefined.getValues(AppliesTo.Claim, claimid);
                newclaim = new cClaim(accountid, claimid, claimno, empid, name, description, stage, approved, paid, datesubmitted, datepaid, status, teamid, checkerid, submitted, createdon, createdby, modifiedon, modifiedby, currencyid, userdefined, cAccounts.getConnectionString(accountid), getExpenseItemsFromDB(claimid,paid, empid));
                aggdep.Add(new CacheDependency[] { claimsdep, historydep, savedcurrentdep }); //,
                Cache.Insert("claim" + accountid + claimid, newclaim, aggdep, System.Web.Caching.Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(15));

            }
            reader.Close();
            

            return newclaim;
        }

        public int getDefaultClaim(ClaimStage stage, int employeeid)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            int claimid = 0;

            if (stage == ClaimStage.Current)
            {
                if (Cache["defaultclaim" + employeeid] != null)
                {
                    claimid = (int)Cache["defaultclaim" + employeeid];
                }
            }

            if (claimid == 0)
            {
                System.Data.SqlClient.SqlDataReader reader;




                switch (stage)
                {
                    case ClaimStage.Current:
                        strsql = "select top 1 claimid from claims_base where employeeid = @employeeid and submitted = 0 order by claimno";
                        break;
                    case ClaimStage.Submitted:
                        strsql = "select top 1 claimid from claims_base where employeeid = @employeeid and submitted = 1 and paid = 0 order by claimno";
                        break;
                    case ClaimStage.Previous:
                        strsql = "select top 1 claimid from claims_base where employeeid = @employeeid and submitted = 1 and paid = 1 order by claimno";
                        break;
                }
                expdata.sqlexecute.Parameters.AddWithValue("@employeeid", employeeid);
                reader = expdata.GetReader(strsql);
                while (reader.Read())
                {
                    if (reader.IsDBNull(0) == false)
                    {
                        claimid = reader.GetInt32(0);
                    }
                }
                expdata.sqlexecute.Parameters.Clear();
                reader.Close();
                
                if (claimid > 0 && stage == ClaimStage.Current)
                {
                    Cache.Insert("defaultclaim" + employeeid, claimid, null, System.Web.Caching.Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(15), System.Web.Caching.CacheItemPriority.NotRemovable, null);
                }
                else
                {
                    return claimid;
                }

            }

            claimid = (int)Cache["defaultclaim" + employeeid];
            return claimid;
        }

        public int insertDefaultClaim(int employeeid)
        {
            int claimid;
            System.Collections.Specialized.HybridDictionary userdefined = new System.Collections.Specialized.HybridDictionary();
            cEmployees clsemployees = new cEmployees(accountid);
            cEmployee reqemp;
            reqemp = clsemployees.GetEmployeeById(employeeid);

            string name, description;

            name = reqemp.firstname[0] + reqemp.surname + (reqemp.curclaimno);
            description = "";
            claimid = addClaim(employeeid, name, description, new Dictionary<int, object>());

            return claimid;

        }

        public System.Data.DataSet getClaimHistory(int claimid)
        {

            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            string strsql;
            System.Data.DataSet temp = new System.Data.DataSet();
            expdata.sqlexecute.Parameters.AddWithValue("@claimid", claimid);
            strsql = "select datestamp, title + ' ' + firstname + ' ' + surname as employee, comment, stage, refnum from claimhistory left join employees on employees.employeeid = claimhistory.employeeid where claimhistory.claimid = @claimid";
            temp = expdata.GetDataSet(strsql);
            expdata.sqlexecute.Parameters.Clear();
            return temp;

        }

        public bool hasReturnedItem(int claimid)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            string strsql;
            int count = 0;
            strsql = "select count(*) from savedexpenses_current inner join returnedexpenses on returnedexpenses.expenseid = savedexpenses_current.expenseid where returned = 1 and corrected = 0 and claimid = " + claimid;
            count = expdata.getcount(strsql);
            if (count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public int addClaim(int employeeid, string name, string description, Dictionary<int, object> userdefined)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            cMisc clsmisc = new cMisc(accountid);
            cGlobalProperties clsproperties = clsmisc.getGlobalProperties(accountid);
            DateTime createdon = DateTime.Now.ToUniversalTime();
            int userid = 0;

            

            int claimno = 0;
            //System.Web.HttpApplication appinfo = (System.Web.HttpApplication)System.Web.HttpContext.Current.ApplicationInstance;

            int claimid;
            cEmployees clsemployees = new cEmployees(accountid);
            cEmployee reqemp = clsemployees.GetEmployeeById(employeeid);

            int basecurrency;
            if (reqemp.primarycurrency != 0)
            {

                basecurrency = reqemp.primarycurrency;
            }
            else
            {
                basecurrency = clsproperties.basecurrency;
            }

            //get the last claim no and increment
            expdata.sqlexecute.Parameters.AddWithValue("@employeeid", employeeid);
            claimno = reqemp.curclaimno;


            expdata.sqlexecute.Parameters.AddWithValue("@claimno", claimno);
            expdata.sqlexecute.Parameters.AddWithValue("@name", name);
            expdata.sqlexecute.Parameters.AddWithValue("@description", description);


            expdata.sqlexecute.Parameters.AddWithValue("@basecurrency", basecurrency);
            expdata.sqlexecute.Parameters.AddWithValue("@date", createdon);
            expdata.sqlexecute.Parameters.AddWithValue("@userid", userid);
            expdata.sqlexecute.Parameters.AddWithValue("@identity", System.Data.SqlDbType.Int);
            expdata.sqlexecute.Parameters["@identity"].Direction = System.Data.ParameterDirection.Output;

            strsql = "insert into claims_base (employeeid, claimno, name, description, currencyid, createdon, createdby) " +
                "values (@employeeid,@claimno,@name,@description,@basecurrency, @date, @userid);select @identity = @@identity";
            
            expdata.ExecuteSQL(strsql);
            claimid = (int)expdata.sqlexecute.Parameters["@identity"].Value;
            clsemployees.incrementClaimNo(reqemp.employeeid);



            expdata.sqlexecute.Parameters.Clear();

            cNewUserDefined clsuserdefined = new cNewUserDefined(accountid, cAccounts.getConnectionString(accountid), new cTables(accountid)); ;
            clsuserdefined.addValues(AppliesTo.Claim, claimid, userdefined);

            cAuditLog clsaudit = new cAuditLog(accountid, employeeid);
            clsaudit.addRecord("Claims", name);
            return claimid;
        }

        public int updateClaim(int claimid, string name, string description, Dictionary<int, object> userdefined, int userid)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            DateTime modifiedon = DateTime.Now.ToUniversalTime();
            

            

            expdata.sqlexecute.Parameters.AddWithValue("@name", name);
            expdata.sqlexecute.Parameters.AddWithValue("@description", description);

            expdata.sqlexecute.Parameters.AddWithValue("@claimid", claimid);
            expdata.sqlexecute.Parameters.AddWithValue("@modifiedon", modifiedon);
            expdata.sqlexecute.Parameters.AddWithValue("@userid", userid);
            strsql = "update claims_base set name = @name, description = @description, ModifiedOn = @modifiedon, ModifiedBy = @userid where claimid = @claimid";
            expdata.ExecuteSQL(strsql);
            expdata.sqlexecute.Parameters.Clear();
            if (userdefined != null)
            {
                cNewUserDefined clsuserdefined = new cNewUserDefined(accountid, cAccounts.getConnectionString(accountid), new cTables(accountid)); ;
                clsuserdefined.addValues(AppliesTo.Claim, claimid, userdefined);
            }
            Cache.Remove("claim" + claimid);
            //InvalidateCache();
            //InitialiseData();
            return 0;
        }
        public System.Data.DataSet getClaimSummary(int employeeid, ClaimStage claimtype)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            System.Data.DataSet ds = new System.Data.DataSet();

            switch (claimtype)
            {
                case ClaimStage.Current: //current claims
                    strsql = "select claimid, [name], description, noitems, total as claimtotal, currencyid as basecurrency from claims where employeeid = @employeeid and submitted = 0";
                    break;
                case ClaimStage.Submitted: //submitted
                    strsql = "select claimid, [name], description, noitems, total as claimtotal, datesubmitted, currencyid as basecurrency from claims where employeeid = @employeeid and submitted = 1 and paid = 0";
                    break;
                case ClaimStage.Previous: //previous
                    strsql = "select claimid, [name], description, noitems, total as claimtotal, datesubmitted, datepaid, currencyid as basecurrency from claims where employeeid = @employeeid and submitted = 1 and paid = 1 and approved = 1";
                    break;
            }
            expdata.sqlexecute.Parameters.AddWithValue("@employeeid", employeeid);
            ds = expdata.GetDataSet(strsql);
            expdata.sqlexecute.Parameters.Clear();
            return ds;

        }

        public void changeStatus(cClaim reqclaim, ClaimStatus status, int userid)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            DateTime modifiedon = DateTime.Now.ToUniversalTime();

            

            expdata.sqlexecute.Parameters.AddWithValue("@claimid", reqclaim.claimid);
            expdata.sqlexecute.Parameters.AddWithValue("@status", (byte)status);
            expdata.sqlexecute.Parameters.AddWithValue("@modifiedon", modifiedon);
            expdata.sqlexecute.Parameters.AddWithValue("@userid", userid);
            strsql = "update claims_base set status = @status, ModifiedOn = @modifiedon, ModifiedBy = @userid where claimid = @claimid";
            expdata.ExecuteSQL(strsql);
            expdata.sqlexecute.Parameters.Clear();
            reqclaim.changeStatus(status);
        }
        public string getPreviousClaimsDropDown(int employeeid)
        {
            System.Text.StringBuilder output = new System.Text.StringBuilder();
            System.Data.SqlClient.SqlDataReader reader;


            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));

            output.Append("<select onchange=\"previousclaims_onchange(" + employeeid + ");\" name=previousclaims id=previousclaims>");
            output.Append("<option>Please select a claim</option>");

            strsql = "select claimid, [name] from claims where employeeid = @employeeid and submitted = 1 and approved = 1 and paid = 1";
            expdata.sqlexecute.Parameters.AddWithValue("@employeeid", employeeid);
            reader = expdata.GetReader(strsql);
            expdata.sqlexecute.Parameters.Clear();





            while (reader.Read())
            {


                output.Append("<option value=\"" + reader.GetInt32(0) + "\">" + reader.GetString(1) + "</option>");

            }

            reader.Close();
            
            output.Append("</select>");
            return output.ToString();

        }

        private SortedList<int, cExpenseItem> getExpenseItemsFromDB(int claimid, bool paid, int employeeid)
        {
            
            SortedList<int, cExpenseItem> items = new SortedList<int, cExpenseItem>();
            cExpenseItem newitem;

            int parentid;
            cNewUserDefined clsuserdefined = null;
            Dictionary<int, object> userdefined = null;
            int expenseid, blitres, companyid, countryid, currencyid, plitres, reasonid, subcatid, carid, allowanceid, hotelid, floatid;
            byte nonights, staff, others, directors, norooms, personalguests, remoteworkers;
            decimal allowanceamount, convertedtotal, foreignvat, net, tip, total, vat, allowancededuct, amountpayable, bmiles, pmiles;
            string attendees, reason, refnum, vatnumber;
            bool creditcard, home, normalreceipt, receipt, returned, tempallow;
            double exchangerate, quantity;
            string accountcode;
            bool primaryitem;
            bool purchasecard, corrected;
            int basecurrency, globalbasecurrency, mileageid;
            int transactionid;
            double globalexchangerate;
            decimal globaltotal;
            cExpenseItem parentItem;
            DateTime date, allowancestartdate, allowanceenddate;
            bool receiptattached;
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            System.Data.SqlClient.SqlDataReader reader;
            ItemType itemtype;
            DateTime createdon, modifiedon;
            int createdby, modifiedby;
            MileageUOM journeyunit;
            string assignmentnum;

            cExpenseItems clsitems = new cExpenseItems(accountid, employeeid);

            SortedList<int, SortedList<int, cJourneyStep>> lstJourneySteps = getJourneySteps(claimid, employeeid);
            SortedList<int, cJourneyStep> steps;

            SortedList<int, Dictionary<FlagType, cFlaggedItem>> lstFlags = clsitems.getFlags(claimid);
            Dictionary<FlagType, cFlaggedItem> flags;

            SortedList<int, List<cDepCostItem>> lstBreakdown = clsitems.getCostCodeBreakdown(claimid);
            List<cDepCostItem> breakdown;

            string strsql;
            if (paid)
            {
                strsql = "select savedexpenses_previous.*, (select floatid from float_allocations where expenseid = savedexpenses_previous.expenseid) as floatid, (select corrected from returnedexpenses where expenseid = savedexpenses_previous.expenseid) as corrected, transactionid from savedexpenses_previous where claimid = @claimid order by primaryitem desc";
            }
            else
            {
                strsql = "select savedexpenses_current.*, (select floatid from float_allocations where expenseid = savedexpenses_current.expenseid) as floatid, (select corrected from returnedexpenses where expenseid = savedexpenses_current.expenseid) as corrected, transactionid from savedexpenses_current where claimid = @claimid order by primaryitem desc";
            }


            expdata.sqlexecute.Parameters.AddWithValue("@claimid", claimid);
            reader = expdata.GetReader(strsql);
            expdata.sqlexecute.Parameters.Clear();

            while (reader.Read())
            {
                if (clsuserdefined == null)
                {
                    clsuserdefined = new cNewUserDefined(accountid, cAccounts.getConnectionString(accountid), new cTables(accountid)); ;
                }
                expenseid = reader.GetInt32(reader.GetOrdinal("expenseid")); ;
                itemtype = (ItemType)reader.GetByte(reader.GetOrdinal("itemtype"));
                if (reader.IsDBNull(reader.GetOrdinal("allowanceamount")) == false)
                {
                    allowanceamount = reader.GetDecimal(reader.GetOrdinal("allowanceamount"));
                }
                else
                {
                    allowanceamount = 0;
                }
                if (reader.IsDBNull(reader.GetOrdinal("attendees")) == false)
                {
                    attendees = reader.GetString(reader.GetOrdinal("attendees"));
                }
                else
                {
                    attendees = "";
                }

                if (reader.IsDBNull(reader.GetOrdinal("blitres")) == false)
                {
                    blitres = reader.GetInt32(reader.GetOrdinal("blitres"));
                }
                else
                {
                    blitres = 0;
                }
                if (reader.IsDBNull(reader.GetOrdinal("bmiles")) == false)
                {
                    bmiles = reader.GetDecimal(reader.GetOrdinal("bmiles"));
                }
                else
                {
                    bmiles = 0;
                }

                if (reader.IsDBNull(reader.GetOrdinal("companyid")) == false)
                {
                    companyid = reader.GetInt32(reader.GetOrdinal("companyid"));
                }
                else
                {
                    companyid = 0;
                }
                convertedtotal = reader.GetDecimal(reader.GetOrdinal("convertedtotal"));
                if (reader.IsDBNull(reader.GetOrdinal("countryid")) == false)
                {
                    countryid = reader.GetInt32(reader.GetOrdinal("countryid"));
                }
                else
                {
                    countryid = 0;
                }

                

                if (reader.IsDBNull(reader.GetOrdinal("currencyid")) == false)
                {
                    currencyid = reader.GetInt32(reader.GetOrdinal("currencyid"));
                }
                else
                {
                    currencyid = 0;
                }
                date = reader.GetDateTime(reader.GetOrdinal("date"));

                exchangerate = reader.GetDouble(reader.GetOrdinal("exchangerate"));


                foreignvat = reader.GetDecimal(reader.GetOrdinal("foreignvat"));
                

                home = reader.GetBoolean(reader.GetOrdinal("home"));


                
                net = reader.GetDecimal(reader.GetOrdinal("net"));
                normalreceipt = reader.GetBoolean(reader.GetOrdinal("normalreceipt"));
                if (reader.IsDBNull(reader.GetOrdinal("others")) == false)
                {
                    others = reader.GetByte(reader.GetOrdinal("others"));
                }
                else
                {
                    others = 0;
                }
                if (reader.IsDBNull(reader.GetOrdinal("plitres")) == false)
                {
                    plitres = reader.GetInt32(reader.GetOrdinal("plitres"));
                }
                else
                {
                    plitres = 0;
                }
                if (reader.IsDBNull(reader.GetOrdinal("pmiles")) == false)
                {
                    pmiles = reader.GetDecimal(reader.GetOrdinal("pmiles"));
                }
                else
                {
                    pmiles = 0;
                }
                if (reader.IsDBNull(reader.GetOrdinal("reason")) == false)
                {
                    reason = reader.GetString(reader.GetOrdinal("reason"));
                }
                else
                {
                    reason = "";
                }
                if (reader.IsDBNull(reader.GetOrdinal("reasonid")) == false)
                {
                    reasonid = reader.GetInt32(reader.GetOrdinal("reasonid"));
                }
                else
                {
                    reasonid = 0;
                }
                receipt = reader.GetBoolean(reader.GetOrdinal("receipt"));
                refnum = reader.GetString(reader.GetOrdinal("refnum"));


                returned = reader.GetBoolean(reader.GetOrdinal("returned"));
                if (reader.IsDBNull(reader.GetOrdinal("corrected")) == false)
                {
                    corrected = reader.GetBoolean(reader.GetOrdinal("corrected"));
                }
                else
                {
                    corrected = false;
                }
                if (reader.IsDBNull(reader.GetOrdinal("staff")) == false)
                {
                    staff = reader.GetByte(reader.GetOrdinal("staff"));
                }
                else
                {
                    staff = 0;
                }

                subcatid = reader.GetInt32(reader.GetOrdinal("subcatid"));


                tempallow = reader.GetBoolean(reader.GetOrdinal("tempallow"));
                if (reader.IsDBNull(reader.GetOrdinal("tip")) == false)
                {
                    tip = reader.GetDecimal(reader.GetOrdinal("tip"));
                }
                else
                {
                    tip = 0;
                }

                total = reader.GetDecimal(reader.GetOrdinal("total"));

                vat = reader.GetDecimal(reader.GetOrdinal("vat"));
                if (reader.IsDBNull(reader.GetOrdinal("nonights")) == false)
                {
                    nonights = reader.GetByte(reader.GetOrdinal("nonights"));
                }
                else
                {
                    nonights = 0;
                }
                if (reader.IsDBNull(reader.GetOrdinal("norooms")) == false)
                {
                    norooms = reader.GetByte(reader.GetOrdinal("norooms"));
                }
                else
                {
                    norooms = 0;
                }
                if (reader.IsDBNull(reader.GetOrdinal("allowancestartdate")) == false)
                {
                    allowancestartdate = reader.GetDateTime(reader.GetOrdinal("allowancestartdate"));
                }
                else
                {
                    allowancestartdate = DateTime.Parse("01/01/1900");
                }
                if (reader.IsDBNull(reader.GetOrdinal("allowanceenddate")) == false)
                {
                    allowanceenddate = reader.GetDateTime(reader.GetOrdinal("allowanceenddate"));
                }
                else
                {
                    allowanceenddate = DateTime.Parse("01/01/1900");
                }
                
                if (reader.IsDBNull(reader.GetOrdinal("carid")) == false)
                {
                    carid = reader.GetInt32(reader.GetOrdinal("carid"));
                }
                else
                {
                    carid = 0;
                }
                if (reader.IsDBNull(reader.GetOrdinal("allowancededuct")) == false)
                {
                    allowancededuct = (decimal)reader.GetDecimal(reader.GetOrdinal("allowancededuct"));
                }
                else
                {
                    allowancededuct = 0;
                }
                if (reader.IsDBNull(reader.GetOrdinal("allowanceid")) == false)
                {
                    allowanceid = reader.GetInt32(reader.GetOrdinal("allowanceid"));
                }
                else
                {
                    allowanceid = 0;
                }

                if (reader.IsDBNull(reader.GetOrdinal("quantity")) == false)
                {
                    quantity = reader.GetDouble(reader.GetOrdinal("quantity"));
                }
                else
                {
                    quantity = 0;
                }
                if (reader.IsDBNull(reader.GetOrdinal("directors")) == false)
                {
                    directors = reader.GetByte(reader.GetOrdinal("directors"));
                }
                else
                {
                    directors = 0;
                }
                if (reader.IsDBNull(reader.GetOrdinal("amountpayable")) == false)
                {
                    amountpayable = reader.GetDecimal(reader.GetOrdinal("amountpayable"));
                }
                else
                {
                    amountpayable = 0;
                }
                if (reader.IsDBNull(reader.GetOrdinal("hotelid")) == false)
                {
                    hotelid = reader.GetInt32(reader.GetOrdinal("hotelid"));
                }
                else
                {
                    hotelid = 0;
                }
                if (reader.IsDBNull(reader.GetOrdinal("vatnumber")) == false)
                {
                    vatnumber = reader.GetString(reader.GetOrdinal("vatnumber"));
                }
                else
                {
                    vatnumber = "";
                }
                if (reader.IsDBNull(reader.GetOrdinal("personalguests")) == false)
                {
                    personalguests = reader.GetByte(reader.GetOrdinal("personalguests"));
                }
                else
                {
                    personalguests = 0;
                }
                if (reader.IsDBNull(reader.GetOrdinal("remoteworkers")) == false)
                {
                    remoteworkers = reader.GetByte(reader.GetOrdinal("remoteworkers"));
                }
                else
                {
                    remoteworkers = 0;
                }
                if (reader.IsDBNull(reader.GetOrdinal("accountcode")) == true)
                {
                    accountcode = "";
                }
                else
                {
                    accountcode = reader.GetString(reader.GetOrdinal("accountcode"));
                }
                if (reader.IsDBNull(reader.GetOrdinal("basecurrency")) == true)
                {
                    basecurrency = 0;
                }
                else
                {
                    basecurrency = reader.GetInt32(reader.GetOrdinal("basecurrency"));
                }
                if (reader.IsDBNull(reader.GetOrdinal("globalbasecurrency")) == true)
                {
                    globalbasecurrency = 0;
                }
                else
                {
                    globalbasecurrency = reader.GetInt32(reader.GetOrdinal("globalbasecurrency"));
                }
                if (reader.IsDBNull(reader.GetOrdinal("globalexchangerate")) == true)
                {
                    globalexchangerate = 0;
                }
                else
                {
                    globalexchangerate = reader.GetDouble(reader.GetOrdinal("globalexchangerate"));
                }
                if (reader.IsDBNull(reader.GetOrdinal("globaltotal")) == true)
                {
                    globaltotal = 0;
                }
                else
                {
                    globaltotal = reader.GetDecimal(reader.GetOrdinal("globaltotal"));
                }
                if (reader.IsDBNull(reader.GetOrdinal("floatid")) == false)
                {
                    floatid = reader.GetInt32(reader.GetOrdinal("floatid"));
                }
                else
                {
                    floatid = 0;
                }
                
                primaryitem = reader.GetBoolean(reader.GetOrdinal("primaryitem"));
                receiptattached = reader.GetBoolean(reader.GetOrdinal("receiptattached"));

                if (reader.IsDBNull(reader.GetOrdinal("createdon")) == true)
                {
                    createdon = new DateTime(1900, 01, 01);
                }
                else
                {
                    createdon = reader.GetDateTime(reader.GetOrdinal("createdon"));
                }
                if (reader.IsDBNull(reader.GetOrdinal("createdby")) == true)
                {
                    createdby = 0;
                }
                else
                {
                    createdby = reader.GetInt32(reader.GetOrdinal("createdby"));
                }
                if (reader.IsDBNull(reader.GetOrdinal("modifiedon")) == true)
                {
                    modifiedon = new DateTime(1900, 01, 01);
                }
                else
                {
                    modifiedon = reader.GetDateTime(reader.GetOrdinal("modifiedOn"));
                }
                if (reader.IsDBNull(reader.GetOrdinal("modifiedby")) == true)
                {
                    modifiedby = 0;
                }
                else
                {
                    modifiedby = reader.GetInt32(reader.GetOrdinal("modifiedby"));
                }
                if (reader.IsDBNull(reader.GetOrdinal("mileageid")) == true)
                {
                    mileageid = 0;
                }
                else
                {
                    mileageid = reader.GetInt32(reader.GetOrdinal("mileageid"));
                }
                if (reader.IsDBNull(reader.GetOrdinal("journey_unit")) == false)
                {
                    journeyunit = (MileageUOM)reader.GetByte(reader.GetOrdinal("journey_unit"));
                }
                else
                {
                    journeyunit = 0;
                }
                if (reader.IsDBNull(reader.GetOrdinal("transactionid")) == true)
                {
                    transactionid = 0;
                }
                else
                {
                    transactionid = reader.GetInt32(reader.GetOrdinal("transactionid"));
                }
                if (reader.IsDBNull(reader.GetOrdinal("AssignmentNumber")) == false)
                {
                    assignmentnum = reader.GetString(reader.GetOrdinal("AssignmentNumber"));
                }
                else
                {
                    assignmentnum = "";
                }
                userdefined = clsuserdefined.getValues(AppliesTo.ExpenseItem, expenseid);

                lstJourneySteps.TryGetValue(expenseid, out steps);
                if (steps == null)
                {
                    steps = new SortedList<int, cJourneyStep>();
                }
                lstFlags.TryGetValue(expenseid, out flags);
                if (flags == null)
                {
                    flags = new Dictionary<FlagType, cFlaggedItem>();
                }
                lstBreakdown.TryGetValue(expenseid, out breakdown);
                if (breakdown == null)
                {
                    breakdown = new List<cDepCostItem>();
                }
                newitem = new cExpenseItem(accountid, expenseid, itemtype, bmiles, pmiles, reason, receipt, net, vat, total, subcatid, date, staff, others, companyid, returned, home, refnum, claimid, plitres, blitres, currencyid, attendees, tip, countryid, foreignvat, convertedtotal, exchangerate, tempallow, reasonid, normalreceipt, allowancestartdate, allowanceenddate, carid, allowancededuct, allowanceid, nonights, quantity, directors, amountpayable, hotelid, primaryitem, norooms, vatnumber, personalguests, remoteworkers, accountcode, basecurrency, globalbasecurrency, globalexchangerate, globaltotal, userdefined, floatid, corrected, receiptattached, transactionid, createdon, createdby, modifiedon, modifiedby, cAccounts.getConnectionString(accountid), mileageid, steps, journeyunit, flags, breakdown, assignmentnum);
                if (!primaryitem) //add to split items
                {
                    parentid = getParentId(expenseid);
                    items.TryGetValue(parentid, out parentItem);
                    if (parentItem == null) //must be split of a split
                    {
                        foreach (cExpenseItem temp in items.Values)
                        {
                            foreach (cExpenseItem split in temp.splititems)
                            {
                                if (split.expenseid == parentid)
                                {
                                    parentItem = split;
                                    break;
                                }

                            }
                            if (parentItem != null)
                            {
                                break;
                            }
                        }
                    }
                    newitem.setPrimaryItem(parentItem);
                    if (parentItem != null)
                    {
                        parentItem.splititems.Add(newitem);
                    }
                }
                else
                {
                    items.Add(expenseid, newitem);
                }


            }
            reader.Close();
            
            return items;
        }

        private SortedList<int, SortedList<int, cJourneyStep>> getJourneySteps(int claimid, int employeeid)
        {
            cCompanies clscompanies = new cCompanies(accountid);
            int stepid, expenseid;
            SortedList<int, SortedList<int, cJourneyStep>> lst = new SortedList<int, SortedList<int, cJourneyStep>>();
            SortedList<int, cJourneyStep> steps;
            byte stepnumber, numpassengers;
            cCompany startlocation, endlocation;
            decimal nummiles, recmiles;
            bool exceededrecommendedmileage;
            string exceededrecommendedmileagecomment;
            System.Data.SqlClient.SqlDataReader reader;
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            strsql = "select savedexpenses_journey_steps.* from savedexpenses_journey_steps inner join savedexpenses on savedexpenses.expenseid = savedexpenses_journey_steps.expenseid where savedexpenses.claimid = @claimid";
            expdata.sqlexecute.Parameters.AddWithValue("@claimid", claimid);
            reader = expdata.GetReader(strsql);
            expdata.sqlexecute.Parameters.Clear();

            while (reader.Read())
            {
                recmiles = 0;
                expenseid = reader.GetInt32(reader.GetOrdinal("expenseid"));
                stepnumber = reader.GetByte(reader.GetOrdinal("step_number"));
                nummiles = reader.GetDecimal(reader.GetOrdinal("num_miles"));
                if (reader.IsDBNull(reader.GetOrdinal("start_location")) == true)
                {
                    startlocation = null;
                }
                else
                {
                    startlocation = clscompanies.GetCompanyById(reader.GetInt32(reader.GetOrdinal("start_location")));
                }
                if (reader.IsDBNull(reader.GetOrdinal("end_location")) == true)
                {
                    endlocation = null;
                }
                else
                {
                    endlocation = clscompanies.GetCompanyById(reader.GetInt32(reader.GetOrdinal("end_location")));
                }
                if (reader.IsDBNull(reader.GetOrdinal("num_passengers")) == true)
                {
                    numpassengers = 0;
                }
                else
                {
                    numpassengers = reader.GetByte(reader.GetOrdinal("num_passengers"));
                }
                exceededrecommendedmileage = reader.GetBoolean(reader.GetOrdinal("exceeded_recommended_mileage"));
                if (reader.IsDBNull(reader.GetOrdinal("exceeded_recommended_mileage_comment")) == false)
                {
                    exceededrecommendedmileagecomment = reader.GetString(reader.GetOrdinal("exceeded_recommended_mileage_comment"));
                }
                else
                {
                    exceededrecommendedmileagecomment = "";
                }

                if (startlocation != null && endlocation != null)
                {
                    recmiles = clscompanies.getDistance(startlocation.companyid, endlocation.companyid, employeeid);
                }
                lst.TryGetValue(expenseid, out steps);
                if (steps == null)
                {
                    steps = new SortedList<int, cJourneyStep>();
                    lst.Add(expenseid, steps);
                }
                steps.Add(stepnumber, new cJourneyStep(expenseid, startlocation, endlocation, nummiles, recmiles, numpassengers, stepnumber, exceededrecommendedmileage, exceededrecommendedmileagecomment));
            }
            reader.Close();

            return lst;
        }
        private int getParentId(int expenseid)
        {
            System.Data.SqlClient.SqlDataReader reader;
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            string strsql;
            int id = 0;
            strsql = "select primaryitem from savedexpenses_splititems where splititem = @splititem";
            expdata.sqlexecute.Parameters.AddWithValue("@splititem", expenseid);
            reader = expdata.GetReader(strsql);
            while (reader.Read())
            {
                id = reader.GetInt32(0);
            }
            reader.Close();
            expdata.sqlexecute.Parameters.Clear();
            return id;
        }
        public byte deleteExpense(cClaim reqclaim, cExpenseItem reqitem, bool approver)
        {

            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            cSubcats clssubcats = new cSubcats(accountid);
            cSubcat subcat;
            cValidate clsvalidate = new cValidate(accountid, null);
            int[] expenseids = new int[1];
            int employeeid = reqclaim.employeeid;







            clsvalidate.deleteDuplicateFlag(employeeid, reqitem.date, reqitem.subcatid, reqitem.total);
            //deleteDuplicateFlag(reqitem.employeeid,reqitem.date,reqitem.subcatid,reqitem.total);

            List<cExpenseItem> items = new List<cExpenseItem>();
            items.Add(reqitem);

            if (reqclaim.submitted == true && reqitem.returned == true) //add delete comment
            {
                cEmails clsemails = new cEmails(accountid);

                int[] recipients = new int[1];

                recipients[0] = reqclaim.checkerid;
                expenseids[0] = reqitem.expenseid;
                addDeleteComment(reqclaim, reqitem);
                updateReturned(reqclaim, reqitem, employeeid);
                //addDeleteComment(expenseid, reqitem.claimid, reqitem.refnum);
                //updateReturned(expenseid);
                clsemails.claim = reqclaim;

                clsemails.items = items;
                clsemails.sendMessage(4, reqclaim.employeeid, recipients);
            }

            subcat = clssubcats.getSubcatById(reqitem.subcatid);
            //if (reqitem.miles != 0 && subcat.calculation != CalculationType.PencePerMileReceipt && subcat.calculation != CalculationType.NormalItem && subcat.calculation != CalculationType.FuelReceipt)
            //{
            //    cCar reqcar;
            //    cEmployees clsemployees = new cEmployees(accountid);
            //    cMileagecats clsmileage = new cMileagecats(accountid);


            //    cEmployee reqemp = clsemployees.GetEmployeeById(employeeid);
            //    reqcar = reqemp.getCarById(reqitem.carid);

            //    cMileageCat reqmileage = clsmileage.GetMileageCatById(reqitem.mileageid);
            //    if (reqmileage != null)
            //    {
            //        if (reqmileage.calcmilestotal == true)
            //        {
            //            int year = reqitem.date.Year;



            //            if (reqitem.date < new DateTime(year, 04, 06))
            //            {
            //                year--;
            //            }

            //            strsql = "update employee_mileagetotals set mileagetotal = (mileagetotal - @nummiles) where employeeid = @employeeid and financial_year = @year";
            //            expdata.sqlexecute.Parameters.AddWithValue("@nummiles", reqitem.miles);
            //            expdata.sqlexecute.Parameters.AddWithValue("@employeeid", employeeid);
            //            expdata.sqlexecute.Parameters.AddWithValue("@year", year);
            //            expdata.ExecuteSQL(strsql);
            //            expdata.sqlexecute.Parameters.Clear();

            //        }
            //    }
            //}

            if (approver == true) //do we need to send email to claimant?
            {
                int[] recipient = new int[1];

                expenseids[0] = reqitem.expenseid;
                recipient[0] = employeeid;

                cEmails clsmessages = new expenses.cEmails(accountid);
                clsmessages.claim = reqclaim;
                clsmessages.items = items;
                clsmessages.sendMessage(10, reqclaim.checkerid, recipient);
            }

            if (reqitem.floatid != 0)
            {
                cFloats clsfloats = new cFloats(accountid);
                cFloat reqfloat = clsfloats.GetFloatById(reqitem.floatid);
                clsfloats.deleteAllocation(reqitem.expenseid, reqitem.floatid);
            }
            foreach (cExpenseItem splititem in reqitem.splititems)
            {
                deleteExpense(reqclaim, splititem, approver);
            }



            expdata.sqlexecute.Parameters.AddWithValue("@expenseid", reqitem.expenseid);
            //strsql = "delete from cardmatches where expenseid = @expenseid;";

            strsql = "delete from receipts where expenseid = @expenseid;";
            strsql += "delete from savedexpenses_splititems where primaryitem = @expenseid;";
            strsql += "delete from savedexpenses_journey_steps where expenseid = @expenseid;";
            strsql += "delete from savedexpenses_current where expenseid = @expenseid;";

            expdata.ExecuteSQL(strsql);
            expdata.sqlexecute.Parameters.Clear();


            cSubcat reqsubcat = clssubcats.getSubcatById(reqitem.subcatid);
            cAuditLog clsaudit = new cAuditLog(accountid, reqclaim.employeeid);
            string auditcat = "Expense Items";
            clsaudit.deleteRecord(auditcat, reqitem.expenseid + "_" + reqitem.date.ToShortDateString() + "_" + reqsubcat.subcat + "_" + reqitem.total.ToString("£###,##,##0.00"));

            reqclaim.deleteExpense(reqitem.expenseid);
            //do we need to delete the claim if no items left?
            if (reqclaim.submitted == true)
            {


                int itemcount = 0;

                itemcount = reqclaim.expenseitems.Count;

                if (itemcount == 0)
                {
                    deleteClaim(reqclaim);
                    return 1;
                }
            }




            return 0;


        }

        public void addDeleteComment(cClaim reqclaim, cExpenseItem item)
        {
            int employeeid;
            System.Web.HttpApplication appinfo = (System.Web.HttpApplication)System.Web.HttpContext.Current.ApplicationInstance;
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            CurrentUser user = cMisc.getCurrentUser(appinfo.User.Identity.Name);

            DateTime datestamp = DateTime.Now;
            string comment;

            if (appinfo.Session["myid"] == null)
            {
                employeeid = user.employeeid;
            }
            else
            {
                employeeid = (int)appinfo.Session["myid"];
            }
            comment = "This expense has been deleted from the claim";

            expdata.sqlexecute.Parameters.AddWithValue("@claimid", item.claimid);
            expdata.sqlexecute.Parameters.AddWithValue("@comment", comment);
            expdata.sqlexecute.Parameters.AddWithValue("@refnum", item.refnum);
            expdata.sqlexecute.Parameters.AddWithValue("@stage", reqclaim.stage);
            expdata.sqlexecute.Parameters.AddWithValue("@employeeid", employeeid);
            strsql = "insert into claimhistory (claimid, datestamp, comment, stage, refnum, employeeid) " +
                "values (@claimid,'" + datestamp.Year + "/" + datestamp.Month + "/" + datestamp.Day + " " + datestamp.Hour + ":" + datestamp.Minute + ":" + datestamp.Second + "',@comment,@stage,@refnum, @employeeid)";
            expdata.ExecuteSQL(strsql);
            expdata.sqlexecute.Parameters.Clear();
        }
        public void addComment(cClaim claim, int employeeid, string comment, string refnum)
        {

            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            DateTime datestamp = DateTime.Now;




            expdata.sqlexecute.Parameters.AddWithValue("@claimid", claim.claimid);
            expdata.sqlexecute.Parameters.AddWithValue("@comment", comment);
            expdata.sqlexecute.Parameters.AddWithValue("@refnum", refnum);
            expdata.sqlexecute.Parameters.AddWithValue("@stage", claim.stage);
            expdata.sqlexecute.Parameters.AddWithValue("@employeeid", employeeid);
            strsql = "insert into claimhistory (claimid, datestamp, comment, stage, refnum, employeeid) " +
                "values (@claimid,'" + datestamp.Year + "/" + datestamp.Month + "/" + datestamp.Day + " " + datestamp.Hour + ":" + datestamp.Minute + ":" + datestamp.Second + "',@comment,@stage,@refnum, @employeeid)";
            expdata.ExecuteSQL(strsql);
            expdata.sqlexecute.Parameters.Clear();
        }
        public void updateReturned(cClaim reqclaim, cExpenseItem reqitem, int userid)
        {

            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            int count = 0;

            addReturnComment(reqclaim, reqitem);

            expdata.sqlexecute.Parameters.AddWithValue("@expenseid", reqitem.expenseid);
            strsql = "update returnedexpenses set corrected = 1 where expenseid = @expenseid";
            expdata.ExecuteSQL(strsql);
            reqitem.corrected = true;




            cEmails clsemails = new expenses.cEmails(accountid);

            int[] recipients = new int[1];
            int[] expenseids = new int[1];
            expenseids[0] = reqitem.expenseid;
            List<cExpenseItem> items = new List<cExpenseItem>();
            items.Add(reqitem);

            recipients[0] = reqclaim.checkerid;

            strsql = "select count(*) from returnedexpenses where corrected = 0 and expenseid = " + reqitem.expenseid;
            count = expdata.getcount(strsql);
            if (count == 0)
            {
                clsemails.claim = reqclaim;
                clsemails.items = items;
                clsemails.sendMessage(4, reqclaim.employeeid, recipients);
                changeStatus(reqclaim, ClaimStatus.ItemCorrectedAwaitingApprover, userid);
            }
            expdata.sqlexecute.Parameters.Clear();
        }

        public void addReturnComment(cClaim reqclaim, cExpenseItem reqitem, string reason)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            System.Web.HttpApplication appinfo = (System.Web.HttpApplication)System.Web.HttpContext.Current.ApplicationInstance;
            CurrentUser user = cMisc.getCurrentUser(appinfo.User.Identity.Name);
            int employeeid;

            string strsql;
            expdata.sqlexecute.Parameters.Clear();
            DateTime datestamp = DateTime.Now;


            string refnum = "";

            string comment;

            expdata.sqlexecute.Parameters.AddWithValue("@reason", reason);
            if (reqitem == null)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@expenseid", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@expenseid", reqitem.expenseid);
            }
            expdata.sqlexecute.Parameters.AddWithValue("@claimid", reqclaim.claimid);
            expdata.sqlexecute.Parameters.AddWithValue("@stage", reqclaim.stage);


            if (reqitem != null)
            {
                refnum = reqitem.refnum;
            }
            comment = "Expense Returned: " + reason;

            if (appinfo.Session["myid"] == null)
            {
                employeeid = user.employeeid;
            }
            else
            {
                employeeid = (int)appinfo.Session["myid"];
            }
            expdata.sqlexecute.Parameters.AddWithValue("@employeeid", employeeid);
            strsql = "insert into claimhistory (claimid, datestamp, comment, stage, refnum, employeeid) " +
                "values (@claimid,'" + datestamp.Year + "/" + datestamp.Month + "/" + datestamp.Day + " " + datestamp.Hour + ":" + datestamp.Minute + ":" + datestamp.Second + "',@reason,@stage,'" + refnum + "',@employeeid)";

            expdata.ExecuteSQL(strsql);
            expdata.sqlexecute.Parameters.Clear();
        }

        public void addReturnComment(cClaim reqclaim, cExpenseItem reqitem)
        {

            System.Web.HttpApplication appinfo = (System.Web.HttpApplication)System.Web.HttpContext.Current.ApplicationInstance;

            DateTime datestamp = DateTime.Now;
            string comment;
            CurrentUser user = cMisc.getCurrentUser(appinfo.User.Identity.Name);

            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));

            int employeeid;
            if (appinfo.Session["myid"] == null)
            {
                employeeid = user.employeeid;
            }
            else
            {
                employeeid = (int)appinfo.Session["myid"];
            }

            comment = "This expense has been amended.";
            expdata.sqlexecute.Parameters.AddWithValue("@claimid", reqitem.claimid);
            expdata.sqlexecute.Parameters.AddWithValue("@comment", comment);
            expdata.sqlexecute.Parameters.AddWithValue("@refnum", reqitem.refnum);
            expdata.sqlexecute.Parameters.AddWithValue("@stage", reqclaim.stage);
            expdata.sqlexecute.Parameters.AddWithValue("@employeeid", reqclaim.employeeid);
            strsql = "insert into claimhistory (claimid, datestamp, comment, stage, refnum, employeeid) " +
                "values (@claimid,'" + datestamp.Year + "/" + datestamp.Month + "/" + datestamp.Day + " " + datestamp.Hour + ":" + datestamp.Minute + ":" + datestamp.Second + "',@comment,@stage,@refnum,@employeeid)";
            expdata.ExecuteSQL(strsql);
            expdata.sqlexecute.Parameters.Clear();
        }

        public int getCount(int employeeid, ClaimStage claimtype)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            int count = 0;

            switch (claimtype)
            {
                case ClaimStage.Current:
                    strsql = "select count(*) from claims_base where employeeid = @employeeid and submitted = 0";
                    break;
                case ClaimStage.Submitted:
                    strsql = "select count(*) from claims where employeeid = @employeeid and submitted = 1 and paid = 0";
                    break;
                case ClaimStage.Previous:
                    strsql = "select count(*) from claims where employeeid = @employeeid and submitted = 1 and approved = 1 and paid = 1";
                    break;
            }
            expdata.sqlexecute.Parameters.AddWithValue("@employeeid", employeeid);
            count = expdata.getcount(strsql);
            expdata.sqlexecute.Parameters.Clear();

            return count;

        }

        public List<ListItem> CreateDropDown(int employeeid)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            System.Data.SqlClient.SqlDataReader reader;

            List<ListItem> items = new List<ListItem>();

            strsql = "select claimid, [name] from claims_base where employeeid = @employeeid and submitted = 0 order by claimno";
            expdata.sqlexecute.Parameters.AddWithValue("@employeeid", employeeid);

            reader = expdata.GetReader(strsql);
            expdata.sqlexecute.Parameters.Clear();


            while (reader.Read())
            {
                items.Add(new ListItem(reader.GetString(1), reader.GetInt32(0).ToString()));


            }
            reader.Close();
            



            return items;
        }

        public void unApproveItem(cExpenseItem item)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            expdata.sqlexecute.Parameters.AddWithValue("@expenseid", item.expenseid);
            strsql = "update savedexpenses_current set tempallow = 0 where expenseid = @expenseid";
            expdata.ExecuteSQL(strsql);
            expdata.sqlexecute.Parameters.Clear();
            item.tempallow = false;
        }


        public void deleteClaim(cClaim claim)
        {

            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));

            #region delete the expense items one by one so mileage totals are updated correctly
            for (int i = claim.expenseitems.Count - 1; i >= 0; i--)
            {
                deleteExpense(claim, claim.expenseitems.Values[i], false);
            }
            #endregion

            expdata.sqlexecute.Parameters.AddWithValue("@claimid", claim.claimid);

            

            strsql = "delete from claims_base where claimid = @claimid";

            expdata.ExecuteSQL(strsql);
            expdata.sqlexecute.Parameters.Clear();

            Cache.Remove("claim" + claim.claimid);

            //remove default claim if necessary
            if (Cache["defaultclaim" + claim.employeeid] != null)
            {
                Cache.Remove("defaultclaim" + claim.employeeid);
            }
            cAuditLog clsaudit = new cAuditLog(accountid, claim.employeeid);
            clsaudit.deleteRecord("Claims", claim.claimid + "_" + claim.name);
        }

        private void updateClaimHistory(cClaim claim, string comment, int employeeid)
        {
            //System.Web.HttpApplication appinfo = (System.Web.HttpApplication)System.Web.HttpContext.Current.ApplicationInstance;
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));

            //claim history
            
            strsql = "insert into claimhistory (claimid, stage, comment, datestamp,employeeid, createdon) " +
                "values (@claimid,@stage,@comment,@datestamp, @employeeid, @createdon)";
            expdata.sqlexecute.Parameters.AddWithValue("@claimid", claim.claimid);
            expdata.sqlexecute.Parameters.AddWithValue("@stage", claim.stage);
            expdata.sqlexecute.Parameters.AddWithValue("@comment", comment);
            expdata.sqlexecute.Parameters.AddWithValue("@datestamp", DateTime.Now);
            expdata.sqlexecute.Parameters.AddWithValue("@employeeid", employeeid);
            expdata.sqlexecute.Parameters.AddWithValue("@createdon", DateTime.Now.ToUniversalTime());
            expdata.ExecuteSQL(strsql);
            expdata.sqlexecute.Parameters.Clear();
        }

        public void unallocateClaim(int claimid, int userid)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            cClaim reqclaim = getClaimById(claimid);
            cEmployees clsemployees = new cEmployees(accountid);
            cGroups clsgroups = new cGroups(accountid);

            cEmployee reqemp = clsemployees.GetEmployeeById(reqclaim.employeeid);
            cGroup reqgroup = clsgroups.GetGroupById(reqemp.groupid);
            SortedList stages = clsgroups.sortStages(reqgroup);
            cStage reqstage = (cStage)stages.GetByIndex(reqclaim.stage - 1);

            DateTime modifiedon = DateTime.Now.ToUniversalTime();

            


            strsql = "update claims_base set checkerid = null, teamid = @teamid, ModifiedOn = @modifiedon, ModifiedBy = @userid where claimid = @claimid";
            expdata.sqlexecute.Parameters.AddWithValue("@teamid", reqstage.relid);
            expdata.sqlexecute.Parameters.AddWithValue("@claimid", claimid);
            expdata.sqlexecute.Parameters.AddWithValue("@modifiedon", modifiedon);
            expdata.sqlexecute.Parameters.AddWithValue("@userid", userid);
            expdata.ExecuteSQL(strsql);
            expdata.sqlexecute.Parameters.Clear();

            reqclaim.checkerid = 0;
            reqclaim.teamid = reqstage.relid;
        }
        public System.Data.DataSet getUnallocatedClaims(int employeeid, string surname, byte filter)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            System.Data.DataSet rcdsttemp;
            expdata.sqlexecute.Parameters.AddWithValue("@employeeid", employeeid);
            strsql = "select * from unallocatedclaims where teamemployeeid = @employeeid";
            if (surname != "")
            {
                strsql += " and employeeid in (select employeeid from employees where employees.surname like @surname)";
                expdata.sqlexecute.Parameters.AddWithValue("@surname", surname + "%");
                
            }
            if (filter != 0)
            {
                strsql += " and claimtype = @claimtype";
                expdata.sqlexecute.Parameters.AddWithValue("@claimtype", filter);
            }
            rcdsttemp = expdata.GetDataSet(strsql);
            expdata.sqlexecute.Parameters.Clear();
            return rcdsttemp;
        }

        public System.Data.DataSet getClaimsToCheck(int employeeid, string surname, byte filter)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            expdata.sqlexecute.Parameters.AddWithValue("@employeeid", employeeid);
            System.Data.DataSet rcdsttemp = new System.Data.DataSet();
            strsql = "select * from checkandpay where checkerid = @employeeid";
            if (surname != "")
            {
                strsql += " and employeeid in (select employeeid from employees where surname like @surname)";
                expdata.sqlexecute.Parameters.AddWithValue("@surname", surname + "%");
                
            }
            if (filter != 0)
            {
                strsql += " and claimtype = @claimtype";
                expdata.sqlexecute.Parameters.AddWithValue("@claimtype", filter);
            }
            rcdsttemp = expdata.GetDataSet(strsql);
            expdata.sqlexecute.Parameters.Clear();
            if (rcdsttemp.Tables[0].Rows.Count == 0)
            {
                return null;
            }

            return rcdsttemp;

        }

        public void allocateClaim(int claimid, int employeeid)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            cEmployees clsemployees = new cEmployees(accountid);
            cClaim reqclaim = getClaimById(claimid);

            cEmployee reqemp = clsemployees.GetEmployeeById(employeeid);

            DateTime modifiedon = DateTime.Now.ToUniversalTime();
            

            expdata.sqlexecute.Parameters.AddWithValue("@claimid", claimid);
            expdata.sqlexecute.Parameters.AddWithValue("@employeeid", employeeid);
            expdata.sqlexecute.Parameters.AddWithValue("@modifiedon", modifiedon);
            expdata.sqlexecute.Parameters.AddWithValue("@userid", employeeid);
            strsql = "update claims_base set checkerid = @employeeid, teamid = null, ModifiedOn = @modifiedon, ModifiedBy = @userid where claimid = @claimid";
            expdata.ExecuteSQL(strsql);
            expdata.sqlexecute.Parameters.Clear();

            //claim history
            strsql = "insert into claimhistory (claimid, stage, comment, datestamp) " +
                "values (@claimid,@stage,@comment,@datestamp)";
            expdata.sqlexecute.Parameters.AddWithValue("@claimid", claimid);
            expdata.sqlexecute.Parameters.AddWithValue("@stage", reqclaim.stage);
            expdata.sqlexecute.Parameters.AddWithValue("@comment", "The claim has been allocated and awaiting approval by " + reqemp.title + " " + reqemp.firstname + " " + reqemp.surname);
            expdata.sqlexecute.Parameters.AddWithValue("@datestamp", DateTime.Now);
            expdata.ExecuteSQL(strsql);
            expdata.sqlexecute.Parameters.Clear();
            reqclaim.checkerid = employeeid;
        }

        public decimal calculatePencePerMile(int employeeid, cClaim claim)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            cEmployees clsemployees = new cEmployees(accountid);
            cEmployee reqemp = clsemployees.GetEmployeeById(employeeid);
            cMisc clsmisc = new cMisc(accountid);
            cGlobalProperties clsproperties = clsmisc.getGlobalProperties(accountid);
            cCar[] cars;
            System.Data.SqlClient.SqlDataReader reader;
            decimal fuelpurchased = 0;
            decimal itemtotal;
            decimal businessmiles = 0;
            decimal totalmiles = 0;
            decimal pencepermile;
            //calculate fuel purchased
            strsql = "select sum(total) from savedexpenses_current inner join subcats on subcats.subcatid = savedexpenses_current.subcatid where claimid = @claimid and subcats.calculation = 5";
            expdata.sqlexecute.Parameters.AddWithValue("@claimid", claim.claimid);
            reader = expdata.GetReader(strsql);
            while (reader.Read())
            {
                if (reader.IsDBNull(0) == false)
                {
                    fuelpurchased = reader.GetDecimal(0);
                }
            }
            reader.Close();

            //calculate business miles
            strsql = "select sum(num_miles) from savedexpenses_journey_steps inner join savedexpenses_current on savedexpenses_current.expenseid = savedexpenses_journey_steps.expenseid inner join subcats on subcats.subcatid = savedexpenses_current.subcatid where claimid = @claimid and subcats.calculation = 6";
            reader = expdata.GetReader(strsql);
            expdata.sqlexecute.Parameters.Clear();
            while (reader.Read())
            {
                if (reader.IsDBNull(0) == false)
                {
                    businessmiles = reader.GetDecimal(0);
                }
            }
            reader.Close();
            if (fuelpurchased == 0 && businessmiles != 0) //need to enter fuel receitps
            {
                return -1;
            }

            cars = reqemp.getCarArray(true);
            cOdometerReading reading;
            for (int i = 0; i < cars.GetLength(0); i++)
            {
                reading = cars[i].getLastOdometerReading();
                totalmiles += (reading.newreading - reading.oldreading);
            }

            if (totalmiles == 0)
            {
                return 0;
            }
            //calculate pence per mile
            pencepermile = fuelpurchased / totalmiles;

            //update the relevant transactions and calculate the mileage

            cSubcats clssubcats = new cSubcats(accountid);
            cSubcat subcat;
            cVat clsvat;
            cExpenseItem vatitem;
            foreach (cExpenseItem item in claim.expenseitems.Values)
            {

                subcat = clssubcats.getSubcatById(item.subcatid);
                if (subcat.calculation == CalculationType.PencePerMileReceipt)
                {
                    //update the total;
                    itemtotal = pencepermile * item.miles;
                    itemtotal = Math.Round(itemtotal, 2);
                    item.updateVAT(itemtotal, 0, itemtotal);
                    vatitem = item;
                    clsvat = new cVat(accountid, ref vatitem, reqemp, clsmisc, clsproperties, null);
                    clsvat.calculateVAT();
                    strsql = "update savedexpenses_current set net = @net, vat = @vat, total = @total, amountpayable = @total, pencepermile = @pencepermile where expenseid = @expenseid";
                    expdata.sqlexecute.Parameters.AddWithValue("@net", Math.Round(item.net, 2));
                    expdata.sqlexecute.Parameters.AddWithValue("@vat", Math.Round(item.vat, 2));
                    expdata.sqlexecute.Parameters.AddWithValue("@total", Math.Round(item.total, 2));
                    expdata.sqlexecute.Parameters.AddWithValue("@expenseid", item.expenseid);
                    expdata.sqlexecute.Parameters.AddWithValue("@pencepermile", pencepermile);
                    expdata.ExecuteSQL(strsql);
                    expdata.sqlexecute.Parameters.Clear();
                }
            }

            return 0;
        }

        #region Data Sync Methods

        public sOnlineClaimInfo getModifiedClaims(DateTime date, int employeeid, bool prevItemsSynched)
        {
            sOnlineClaimInfo onlineClaimInfo = new sOnlineClaimInfo();
            Dictionary<int, cClaim> lst = new Dictionary<int, cClaim>();
            List<int> lstclaimids = getClaimIds(employeeid, prevItemsSynched, date);

            if (prevItemsSynched)
            {
                getModifiedPreviousClaims(ref lstclaimids, date, employeeid);
            }

            foreach (int id in lstclaimids)
            {
                cClaim claim = getClaimFromDB(id);

                if (claim.createdon > date || claim.modifiedon > date)
                {
                    lst.Add(claim.claimid, claim);
                }
            }

            onlineClaimInfo.lstonlineclaims = lst;
            onlineClaimInfo.lstclaimids = lstclaimids;

            return onlineClaimInfo;
        }

        public List<int> getClaimIds(int employeeid, bool prevItemsSynched, DateTime date)
        {
            System.Data.SqlClient.SqlDataReader reader;

            List<int> lstclaimids = new List<int>();
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            expdata.sqlexecute.Parameters.AddWithValue("@employeeid", employeeid);

            if (prevItemsSynched)
            {
                strsql = "SELECT claimid FROM claims_base where employeeid = @employeeid AND paid = 0";
            }
            else
            {
                strsql = "SELECT claimid FROM claims_base where employeeid = @employeeid";
            }

            int claimid = 0;
            reader = expdata.GetReader(strsql);
            expdata.sqlexecute.Parameters.Clear();

            while (reader.Read())
            {
                claimid = reader.GetInt32(reader.GetOrdinal("claimid"));
                lstclaimids.Add(claimid);
            }
            reader.Close();
            return lstclaimids;
        }

        public void getModifiedPreviousClaims(ref List<int> lstClaimIds, DateTime date, int employeeid)
        {
            System.Data.SqlClient.SqlDataReader reader;

            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            expdata.sqlexecute.Parameters.AddWithValue("@employeeid", employeeid);
            expdata.sqlexecute.Parameters.AddWithValue("@modifiedon", date);
            
            strsql = "SELECT claimid FROM claims_base where employeeid = @employeeid AND paid = 1 AND modifiedon > @modifiedon";

            int claimid = 0;
            reader = expdata.GetReader(strsql);
            expdata.sqlexecute.Parameters.Clear();

            while (reader.Read())
            {
                claimid = reader.GetInt32(reader.GetOrdinal("claimid"));
                lstClaimIds.Add(claimid);
            }
            reader.Close();
        }

        public sOnlineItemInfo getModifiedItems(DateTime date, int employeeid, bool prevItemsSynched)
        {
            sOnlineItemInfo onlineItem = new sOnlineItemInfo();
            Dictionary<int, cExpenseItem> lstCurModItems = new Dictionary<int, cExpenseItem>();
            Dictionary<int, cExpenseItem> lstPrevModItems = new Dictionary<int, cExpenseItem>();
            List<int> lstCurItemIds = new List<int>();
            List<int> lstclaimids = getClaimIds(employeeid, prevItemsSynched, date);

            foreach (int id in lstclaimids)
            {
                cClaim claim = getClaimById(id);

                if (claim.paid == false)
                {
                    foreach (cExpenseItem item in claim.expenseitems.Values)
                    {
                        lstCurItemIds.Add(item.expenseid);

                        if (item.createdon > date || item.modifiedon > date)
                        {
                            lstCurModItems.Add(item.expenseid, item);
                        }
                    }
                }
                else
                {
                    
                    foreach (cExpenseItem item in claim.expenseitems.Values)
                    {
                        if (item.createdon > date || item.modifiedon > date)
                        {
                            lstPrevModItems.Add(item.expenseid, item);
                        }
                    }
                    
                }
            }
            onlineItem.lstCurModItems = lstCurModItems;
            onlineItem.lstCurItemIds = lstCurItemIds;
            onlineItem.lstPrevModItems = lstPrevModItems;

            return onlineItem;
        }

        #endregion

        #region claim submittal
        public void payClaim(cClaim reqclaim, int employeeid)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            decimal amounttopay;
            int[] recipients = new int[1];
            cEmails clsemail = new expenses.cEmails(accountid);
            DateTime date;
            date = DateTime.Today;
            expdata.sqlexecute.Parameters.AddWithValue("@claimid", reqclaim.claimid);
            expdata.sqlexecute.Parameters.AddWithValue("@modifiedon", DateTime.UtcNow);
            expdata.sqlexecute.Parameters.AddWithValue("@userid", employeeid);
            expdata.ExecuteProc("transferPaidItemsToPrevious");


            expdata.sqlexecute.Parameters.Clear();
            reqclaim.claimPaid();

            if (reqclaim.amountpayable != 0)
            {
                recipients[0] = reqclaim.employeeid;

                clsemail.claim = reqclaim;
                clsemail.sendMessage(6, reqclaim.checkerid, recipients);
            }
            updateClaimHistory(reqclaim, "This claim has been authorised and is due to be paid in the next payment run", employeeid);

        }
        public int submitClaim(cClaim claim, bool cash, bool credit, bool purchase, int authoriser, int employeeid, int? delegateid)
        {
            int returncode = 0;
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            cEmployees clsemployees = new cEmployees(accountid);
            cEmployee reqemp = clsemployees.GetEmployeeById(claim.employeeid);

            string strsql;
            int count = 0;
            //check item count first
            if (claim.noitems == 0)
            {
                return 1;
            }
            int? commenter;
            if (delegateid == null)
            {
                commenter = employeeid;
            }
            else
            {
                commenter = delegateid;
            }

            //check claim total
            returncode = checkClaimTotal(claim, cash, credit, purchase);
            switch (returncode)
            {
                case 1:
                    return 4;
                case 2:
                    return 5;
            }

            if ((cash == false || credit == false || purchase == false) && claim.containsCashAndCredit() == true)
            {

                int newclaimid;
                cClaim newclaim;
                newclaimid = insertDefaultClaim(claim.employeeid);
                moveItems(claim.claimid, newclaimid, cash, credit, purchase);
                newclaim = getClaimById(newclaimid);


                submitClaim(newclaim, cash, credit, purchase, authoriser, employeeid, delegateid);
                return newclaimid;

            }




            if (returncode == 2)
            {
                return 5;
            }

            if (checkFrequency(claim.employeeid) == false)
            {
                return 8;
            }
            DateTime datestamp = DateTime.Now;

            cMisc clsmisc = new cMisc(accountid);
            cGlobalProperties clsproperties = clsmisc.getGlobalProperties(accountid);

            int groupid = reqemp.groupid;

            if (clsproperties.partsubmittal == true && clsproperties.onlycashcredit)
            {
                if (credit == true)
                {
                    groupid = reqemp.groupidcc;
                }
                if (purchase == true)
                {
                    groupid = reqemp.groupidpc;
                }
            }
            returncode = SendClaimToNextStage(claim, true, authoriser, employeeid, commenter);


            if (returncode != 0)
            {
                return returncode;
            }

            DateTime modifiedon = DateTime.Now.ToUniversalTime();
            

            //insert first entry into claim history
            expdata.sqlexecute.Parameters.AddWithValue("@claimid", claim.claimid);
            expdata.sqlexecute.Parameters.AddWithValue("@modifiedon", modifiedon);
            expdata.sqlexecute.Parameters.AddWithValue("@userid", employeeid);

            strsql = "update claims_base set submitted = 1, status = 1, datesubmitted = '" + datestamp.Year + "/" + datestamp.Month + "/" + datestamp.Day + "', ModifiedOn = @modifiedon, ModifiedBy = @userid where claimid = @claimid";
            expdata.ExecuteSQL(strsql);

            expdata.sqlexecute.Parameters.Clear();

            //remove default claim if necessary
            if (Cache["defaultclaim" + claim.employeeid] != null)
            {
                Cache.Remove("defaultclaim" + claim.employeeid);
            }
            claim.submitClaim();
            return 0;


        }

        public byte checkClaimTotal(cClaim claim, bool cash, bool credit, bool purchase)
        {
            //System.Web.HttpApplication appinfo = (System.Web.HttpApplication)System.Web.HttpContext.Current.ApplicationInstance;
            cEmployees clsemployees = new cEmployees(accountid);

            cEmployee reqemp = clsemployees.GetEmployeeById(claim.employeeid);
            cRoles clsroles = new cRoles(reqemp.accountid);
            cRole reqrole = clsroles.getRoleById(reqemp.roleid);
            decimal minclaim, maxclaim;

            minclaim = reqrole.minclaim;
            maxclaim = reqrole.maxclaim;

            if (cash == false && (credit == true || purchase == true))
            {
                return 0;
            }

            if (minclaim != 0 || maxclaim != 0)
            {

                string strsql;
                decimal total;
                total = claim.total;

                if (cash != credit)
                {
                    total -= claim.creditcardtotal;


                }
                if (cash != purchase)
                {
                    total -= claim.purchasecardtotal;
                }

                if (minclaim != 0)
                {
                    if (total < minclaim)
                    {
                        return 1;
                    }
                }
                if (maxclaim != 0)
                {
                    if (total > maxclaim)
                    {
                        return 2;
                    }
                }

            }

            return 0;
        }
        public void moveItems(int oldclaimid, int newclaimid, bool cash, bool credit, bool purchase)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            strsql = "update savedexpenses_current set claimid = @newclaimid where claimid = @oldclaimid and (";
            if (cash)
            {
                strsql += "itemtype = 1";
            }
            if (credit)
            {
                if (cash)
                {
                    strsql += " or ";
                }
                strsql += "itemtype = 2";
            }
            if (purchase)
            {
                if (cash || credit)
                {
                    strsql += " or ";
                }
                strsql += "itemtype = 3";
            }
            strsql += ")";

            expdata.sqlexecute.Parameters.AddWithValue("@newclaimid", newclaimid);
            expdata.sqlexecute.Parameters.AddWithValue("@oldclaimid", oldclaimid);
            expdata.sqlexecute.Parameters.AddWithValue("@credit", Convert.ToByte(credit));
            expdata.sqlexecute.Parameters.AddWithValue("@purchase", Convert.ToByte(purchase));
            expdata.ExecuteSQL(strsql);
            expdata.sqlexecute.Parameters.Clear();


        }
        private bool checkFrequency(int employeeid)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            string strsql;

            cMisc clsmisc = new cMisc(accountid);
            cGlobalProperties clsproperties = clsmisc.getGlobalProperties(accountid);

            //System.Web.HttpApplication appinfo = (System.Web.HttpApplication)System.Web.HttpContext.Current.ApplicationInstance;
            if (clsproperties.limitfrequency == false)
            {
                return true;
            }

            byte frequencytype = clsproperties.frequencytype;
            int frequencyvalue = clsproperties.frequencyvalue;

            DateTime startdate;

            if (frequencytype == 1) //monthly
            {
                startdate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 01);
            }
            else //weekly
            {
                startdate = DateTime.Today;
                while (startdate.DayOfWeek != DayOfWeek.Monday)
                {
                    startdate = startdate.AddDays(-1);
                }
            }

            int count;
            //get count
            strsql = "select count(*) from claims_base where employeeid = @employeeid and datesubmitted >= @date";
            expdata.sqlexecute.Parameters.AddWithValue("@employeeid", employeeid);
            expdata.sqlexecute.Parameters.AddWithValue("@date", startdate.Year + "/" + startdate.Month + "/" + startdate.Day);
            count = expdata.getcount(strsql);
            expdata.sqlexecute.Parameters.Clear();

            if (count < frequencyvalue)
            {
                return true;
            }
            else
            {
                return false;
            }


        }
        
        public void approveItems(List<cExpenseItem> items)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            if (items.Count == 0)
            {
                return;
            }
            int i = 0;
            string returnsql;
            strsql = "update savedexpenses_current set returned = 0, tempallow = 1 where ";
            returnsql = "delete from returnedexpenses where ";
            foreach (cExpenseItem item in items)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@expenseid" + i, item.expenseid);
                strsql = strsql + "expenseid = @expenseid" + i + " or ";
                returnsql = returnsql + "expenseid = @expenseid" + i + " or ";
                i++;
                item.returned = false;
                item.tempallow = true;
            }
            strsql = strsql.Remove(strsql.Length - 4, 4);
            returnsql = returnsql.Remove(returnsql.Length - 4, 4);
            expdata.ExecuteSQL(returnsql);
            expdata.ExecuteSQL(strsql);
            expdata.sqlexecute.Parameters.Clear();


        }

        public byte unSubmitclaim(cClaim reqclaim, bool approver, int employeeid, int? delegateid)
        {

            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            string strsql;
            
            //System.Web.HttpApplication appinfo = (System.Web.HttpApplication)System.Web.HttpContext.Current.ApplicationInstance;
            cEmployees clsemployees = new cEmployees(accountid);

            cEmployee reqemp;

            int commenter;
            if (delegateid == null)
            {
                commenter = employeeid;
            }
            else
            {
                commenter = (int)delegateid;
            }
            reqemp = clsemployees.GetEmployeeById(employeeid);
            if (reqclaim.status == ClaimStatus.None || reqclaim.status == ClaimStatus.Submitted || reqclaim.status == ClaimStatus.AwaitingAllocation)
            {
                if (approver == true)
                {
                    updateClaimHistory(reqclaim, "The claim has been unsubmitted by your approver", commenter);

                    //send email


                    cEmails clsemails = new cEmails(reqemp.accountid);
                    clsemails.claim = reqclaim;
                    clsemails.sendMessage(17, reqemp.employeeid, new int[] { reqclaim.employeeid });
                }
                else
                {
                    updateClaimHistory(reqclaim, "Claim unsubmitted by claimant", commenter);
                }
                expdata.sqlexecute.Parameters.AddWithValue("@claimid", reqclaim.claimid);
                strsql = "update savedexpenses_current set tempallow = 0 where claimid = @claimid";
                expdata.ExecuteSQL(strsql);

                DateTime modifiedon = DateTime.Now.ToUniversalTime();
                

                expdata.sqlexecute.Parameters.AddWithValue("@modifiedon", modifiedon);
                expdata.sqlexecute.Parameters.AddWithValue("@userid", employeeid);
                strsql = "update claims_base set checkerid = null, teamid = null, datesubmitted = null, submitted = 0, approved = 0, stage = 0, ModifiedOn = @modifiedon, ModifiedBy = @userid where claimid = @claimid";
                expdata.ExecuteSQL(strsql);

                cMisc clsmisc = new cMisc(accountid);
                cGlobalProperties clspropertes = clsmisc.getGlobalProperties(accountid);

                if (clspropertes.enterodometeronsubmit)
                {
                    cCar[] odocars = reqemp.getCarArray(true);
                    cOdometerReading odoreading;
                    for (int i = 0; i < odocars.GetLength(0); i++)
                    {
                        odoreading = odocars[i].getLastOdometerReading();
                        clsemployees.deleteOdometerReading(reqemp.employeeid, odocars[i].carid, odoreading.odometerid);
                    }
                }
                else
                {
                    int businessmilescount;
                    strsql = "select count(*) from savedexpenses_current where claimid = @claimid and subcatid in (select subcatid from subcats where calculation = 6)";
                    
                    businessmilescount = expdata.getcount(strsql);
                    if (businessmilescount != 0)
                    {
                        cCar[] cars = reqemp.getCarArray(true);
                        cOdometerReading reading;
                        for (int i = 0; i < cars.GetLength(0); i++)
                        {
                            reading = cars[i].getLastOdometerReading();
                            clsemployees.deleteOdometerReading(reqemp.employeeid, cars[i].carid, reading.odometerid);
                        }
                        //undo the mileage
                        strsql = "update savedexpenses_current set net = 0, vat = 0, total = 0, amountpayable = 0 where claimid = @claimid and subcatid in (select subcatid from subcats where calculation = 6)";

                        expdata.ExecuteSQL(strsql);

                    }
                }

                reqclaim.unSubmitClaim();


                return 0;
            }

            expdata.sqlexecute.Parameters.Clear();
            return 1;
        }

        public void returnExpenses(cClaim reqclaim, List<cExpenseItem> items, string reason, int employeeid, int? delegateid)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            int i = 0;
            int count = 0;
            cEmails clsemail = new cEmails(accountid);
        
            int[] recipientid = new int[1];

            string strwhere = "";
            int commenter;
            if (delegateid == null)
            {
                commenter = employeeid;
            }
            else{
                commenter = (int)delegateid;
            }
            int[] expenseids = new int[items.Count];
            recipientid[0] = reqclaim.employeeid;

            if (reqclaim.expenseitems.Count == items.Count) //return the whole claim
            {
                //delete from returned expenses
                strsql = "delete from returnedexpenses where expenseid in (";
                foreach (cExpenseItem item in items)
                {
                    strsql += item.expenseid + ",";
                    item.tempallow = false;
                    item.returned = false;
                }
                strsql = strsql.Remove(strsql.Length - 1, 1);
                strsql += ")";
                expdata.ExecuteSQL(strsql);

                //update savedexpenses
                strsql = "update savedexpenses_current set tempallow = 0, returned = 0 where expenseid in (";
                foreach (cExpenseItem item in items)
                {
                    strsql += item.expenseid + ",";
                }
                strsql = strsql.Remove(strsql.Length - 1, 1);
                strsql += ")";
                expdata.ExecuteSQL(strsql);

                DateTime modifiedon = DateTime.Now.ToUniversalTime();
                

                //update the claim
                strsql = "update claims_base set datepaid = null, approved = 0, paid = 0, datesubmitted = null, status = 0, teamid = null, checkerid = null, stage = 0, submitted = 0, ModifiedOn = @modifiedon, ModifiedBy = @userid where claimid = @claimid";
                expdata.sqlexecute.Parameters.AddWithValue("@claimid", reqclaim.claimid);
                expdata.sqlexecute.Parameters.AddWithValue("@modifiedon", modifiedon);
                expdata.sqlexecute.Parameters.AddWithValue("@userid", employeeid);
                expdata.ExecuteSQL(strsql);
                expdata.sqlexecute.Parameters.Clear();

                addReturnComment(reqclaim, null, reason);
                reqclaim.changeStatus(ClaimStatus.None);
                unSubmitclaim(reqclaim, true, employeeid, delegateid);
            }
            else
            {
                foreach (cExpenseItem item in items)
                {

                    expdata.sqlexecute.Parameters.AddWithValue("@reason", reason);
                    strsql = "select count(*) from returnedexpenses where expenseid = " + item.expenseid;
                    count = expdata.getcount(strsql);

                    if (count == 0) //hasn't bee returned before, insert record
                    {
                        strsql = "insert into returnedexpenses (note, corrected, expenseid) " +
                            "values (@reason,0," + item.expenseid + ")";
                        expdata.ExecuteSQL(strsql);
                    }
                    else
                    {
                        strsql = "update returnedexpenses set note = @reason, corrected = 0 where expenseid = " + item.expenseid;
                        expdata.ExecuteSQL(strsql);
                    }
                    expdata.sqlexecute.Parameters.RemoveAt("@reason");
                    //insert comment into claim history
                    addReturnComment(reqclaim, item, reason);


                    strwhere = strwhere + "expenseid = " + item.expenseid + " or ";

                    item.returned = true;
                    item.corrected = false;
                }
                strwhere = strwhere.Remove(strwhere.Length - 4, 4);
                strsql = "update savedexpenses_current set returned = -1 where " + strwhere;
                expdata.sqlexecute.Parameters.AddWithValue("@reason", reason);

                expdata.ExecuteSQL(strsql);
                expdata.sqlexecute.Parameters.Clear();
                changeStatus(reqclaim, ClaimStatus.ItemReturnedAwaitingEmployee, employeeid);

                clsemail.claim = reqclaim;
                clsemail.items = items;
                clsemail.sendMessage(3, employeeid, recipientid);
            }





        }

        public void disputeExpense(cClaim reqclaim, cExpenseItem reqitem, string dispute)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            System.Web.HttpApplication appinfo = (System.Web.HttpApplication)System.Web.HttpContext.Current.ApplicationInstance;
            CurrentUser user = cMisc.getCurrentUser(appinfo.User.Identity.Name);
            cEmployees clsemployees = new cEmployees(accountid);
            cEmployee reqemp;
            reqemp = clsemployees.GetEmployeeById(user.employeeid);

            int[] recipient = new int[1];



            cEmails clsemails = new cEmails(reqemp.accountid);
            addDisputeComment(reqclaim, reqitem, dispute);

            expdata.sqlexecute.Parameters.AddWithValue("@dispute", dispute);
            expdata.sqlexecute.Parameters.AddWithValue("@expenseid", reqitem.expenseid);
            strsql = "update returnedexpenses set corrected = 1, dispute = @dispute where expenseid = @expenseid";
            expdata.ExecuteSQL(strsql);


            reqitem.corrected = true;

            recipient[0] = reqclaim.checkerid;
            //**todo** claim and expenses
            clsemails.sendMessage(4, user.employeeid, recipient);

            reqclaim.changeStatus(ClaimStatus.ItemCorrectedAwaitingApprover);
            Cache.Remove("claim" + reqclaim.claimid);
            expdata.sqlexecute.Parameters.Clear();
        }
        private void addDisputeComment(cClaim reqclaim, cExpenseItem item, string dispute)
        {
            System.Web.HttpApplication appinfo = (System.Web.HttpApplication)System.Web.HttpContext.Current.ApplicationInstance;
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            CurrentUser user = cMisc.getCurrentUser(appinfo.User.Identity.Name);

            int employeeid;


            DateTime datestamp = DateTime.Now;




            string comment;
            if (appinfo.Session["myid"] == null)
            {
                employeeid = user.employeeid;
            }
            else
            {
                employeeid = (int)appinfo.Session["myid"];
            }

            comment = "The returned expense has been disputed: " + dispute;

            expdata.sqlexecute.Parameters.AddWithValue("@claimid", reqclaim.claimid);
            expdata.sqlexecute.Parameters.AddWithValue("@comment", comment);
            expdata.sqlexecute.Parameters.AddWithValue("@expenseid", item.expenseid);
            expdata.sqlexecute.Parameters.AddWithValue("@stage", reqclaim.stage);
            expdata.sqlexecute.Parameters.AddWithValue("@refnum", item.refnum);
            expdata.sqlexecute.Parameters.AddWithValue("@employeeid", employeeid);
            strsql = "insert into claimhistory (claimid, datestamp, comment, stage, refnum, employeeid) " +
                "values (@claimid,'" + datestamp.Year + "/" + datestamp.Month + "/" + datestamp.Day + " " + datestamp.Hour + ":" + datestamp.Minute + ":" + datestamp.Second + "',@comment,@stage,@refnum,@employeeid)";
            expdata.ExecuteSQL(strsql);
            expdata.sqlexecute.Parameters.Clear();
        }
        private void resetApproval(cClaim claim)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            expdata.sqlexecute.Parameters.Clear();

            expdata.sqlexecute.Parameters.AddWithValue("@claimid", claim.claimid);
            strsql = "update savedexpenses_current set tempallow = 0 where claimid = @claimid";
            expdata.ExecuteSQL(strsql);
            expdata.sqlexecute.Parameters.Clear();
            claim.resetApproval();
        }
        public int SendClaimToNextStage(cClaim claim, bool submitting, int authoriser, int employeeid, int? delegateid)
        {
            //return codes
            //1 = user not assigned to a group
            //2 = reached last stage, claim now needs paying

            cStage reqstage;
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            expdata.sqlexecute.Parameters.Clear();
            System.Data.DataSet rcdststages = new System.Data.DataSet();
            bool includestage = false;
            bool isOnHoliday = false;
            int stage = claim.stage - 1;
            byte signofftype = 0;
            int onholiday = 0;
            int relid = 0;
            StageInclusionType include = StageInclusionType.None;
            int notify = 0;
            int[] nextcheckerid;
            decimal maxamount = 0;
            decimal minamount = 0;
            decimal claimamount = 0;
            string sql = "";
            cEmployees clsemployees = new cEmployees(accountid);
            cEmployee reqemp;


            int commenter;
            if (delegateid == null)
            {
                commenter = employeeid;
            }
            else
            {
                commenter = (int)delegateid;
            }
            try
            {
                System.Web.HttpApplication appinfo = (System.Web.HttpApplication)System.Web.HttpContext.Current.ApplicationInstance;
                CurrentUser user = cMisc.getCurrentUser(appinfo.User.Identity.Name);
                
                reqemp = clsemployees.GetEmployeeById(user.employeeid);
            }
            catch
            {             
                reqemp = clsemployees.GetEmployeeById(employeeid);
            }
            cEmails clsemails = new cEmails(reqemp.accountid);
            cGroups clsgroups = new cGroups(reqemp.accountid);
            cEmployee claimemp = clsemployees.GetEmployeeById(claim.employeeid);
            cGroup reqgroup = clsgroups.GetGroupById(claimemp.groupid);
            cMisc clsmisc = new cMisc(reqemp.accountid);
            cGlobalProperties clsproperties = clsmisc.getGlobalProperties(reqemp.accountid);
            cEmployee nextchecker;
            cTeams clsteams = new cTeams(accountid);
            
            cTeam reqteam;
            


            //determine the group
            if (clsproperties.onlycashcredit && clsproperties.partsubmittal)
            {
                if (claim.claimtype == ClaimType.Credit)
                {
                    reqgroup = clsgroups.GetGroupById(claimemp.groupidcc);
                }
                else if (claim.claimtype == ClaimType.Purchase)
                {
                    reqgroup = clsgroups.GetGroupById(claimemp.groupidpc);
                }
            }

            if (claim.stage > 0)
            {
                if (reqgroup.stages.Values[claim.stage - 1] != null)
                {
                    cStage reqApprovalStage = reqgroup.stages.Values[claim.stage - 1];

                    if(claim.checkerid != employeeid) 
                    {
                        return 12;
                    }
                    
                    //switch (reqApprovalStage.signofftype)
                    //{
                    //    case 1: //Budget holder
                    //        cBudgetholders budgetHolder = new cBudgetholders(accountid);
                    //        cBudgetHolder reqHolder = budgetHolder.getBudgetHolderById(reqApprovalStage.relid);

                    //        if (reqHolder.employeeid != employeeid)
                    //        {
                    //            return 12;
                    //        }
                    //        break;

                    //    case 2: //Employee
                    //        if (reqApprovalStage.relid != employeeid)
                    //        {
                    //            return 12;
                    //        }
                    //        break;

                    //    case 3: //Team

                    //        if (claim.checkerid != employeeid || claim.teamid != 0)
                    //        {
                    //            return 12;
                    //        }

                    //        break;
                            
                    //    case 4: //Line Manager

                    //        if (claimemp.linemanager != employeeid)
                    //        {
                    //            return 12;
                    //        }

                    //        break;

                    //    default:
                    //        return 12;
                    //        break;
                    //}

                    

                }
            }

            


            //get the claimamount


            claimamount = claim.total;

            //get the stages the claim has to go through


            if (reqgroup == null)
            {
                return 3;
            }
            if (reqgroup.stages.Count == 0)
            {
                return 3;
            }

            if (submitting == true)
            {
                updateClaimHistory(claim, "Claim Submitted.", commenter);
            }

            //increment stage
            
            if (claim.stage >= (reqgroup.stages.Count) && submitting == false) //reached last stage, now needs payin
            {
                    approveClaim(claim, employeeid, delegateid);
                    return 2;
            }
            resetApproval(claim);

            for (stage = claim.stage; stage < reqgroup.stages.Count; stage++)
            {
                reqstage = (cStage)reqgroup.stages.Values[stage];
                includestage = true;
                signofftype = reqstage.signofftype;
                relid = reqstage.relid;
                //is the stage on holiday?
                isOnHoliday = userOnHoliday(signofftype, relid);
                if (isOnHoliday == true)
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
                            signofftype = (byte)reqstage.holidaytype;
                            relid = reqstage.holidayid;
                            if (userOnHoliday(signofftype, relid) == true) //cannot go any further
                            {
                                updateClaimHistory(claim, "This claim may be delayed as the approver is currently on holiday.", commenter);

                            }
                            includestage = true;
                            break;

                    }
                }

                //got the stage we need. 
                if (includestage == true)
                {
                    notify = reqstage.notify;
                    include = reqstage.include;
                    claim.changeStatus(ClaimStatus.NextStageAwaitingAction);
                    switch (include)
                    {
                        case StageInclusionType.ClaimTotalExceeds: //claim exceeded specified amount
                            maxamount = reqstage.amount;
                            if (claimamount < maxamount)
                            {
                                includestage = false;
                            }
                            break;
                        case StageInclusionType.ClaimTotalBelow:
                            minamount = reqstage.amount;
                            if (claimamount > minamount)
                            {
                                includestage = false;
                            }
                            break;
                        case StageInclusionType.ExpenseItemExceeds: //an item flagged for going over
                            includestage = claim.checkFlagExceeded();
                            break;
                        case StageInclusionType.IncludesCostCode: //item assigned to costcode
                            includestage = checkCostCodeIncluded(claim, reqstage.includeid);
                            break;
                        case StageInclusionType.IncludesExpenseItem:
                            includestage = checkExpenseItemIncluded(claim, reqstage.includeid);
                            break;
                        case StageInclusionType.OlderThanDays:
                            includestage = includesItemOlderThanXDays(claim, (int)reqstage.amount);
                            break;
                    }


                }

                //check to see if limit existed. if this was exceeded includestage will have been changed to false
                if (includestage == true)
                {
                    nextcheckerid = new int[1];
                    if (signofftype == 5)
                    {
                        nextcheckerid[0] = authoriser;
                    }
                    else
                    {
                        nextcheckerid[0] = getNextCheckerId(claim, signofftype, relid);
                    }
                    if (signofftype != 3 && signofftype != 5 && nextcheckerid[0] == 0)
                    {
                        return 6;
                    }
                    claim.stage = stage + 1;


                    claim.checkerid = nextcheckerid[0];
                    if (notify == 1) //just notify of claim by email
                    {
                        nextchecker = clsemployees.GetEmployeeById(claim.checkerid);
                        if (submitting == true)
                        {
                            updateClaimHistory(claim, "Claim No " + claim.claimno + " submitted." + nextchecker.title + " " + nextchecker.firstname + " " + nextchecker.surname + " has been notified of this claim.", commenter);
                        }
                        else
                        {
                            if (signofftype == 3)
                            {
                                reqteam = clsteams.GetTeamById(relid);
                                

                                nextcheckerid = new int[reqteam.teammembers.Count];
                                foreach (int t in reqteam.teammembers)
                                {
                                    
                                     nextcheckerid[reqteam.teammembers.IndexOf(t)] =t;
                                }
                            }
                            clsemails.claim = claim;
                            clsemails.sendMessage(8, claim.employeeid, nextcheckerid);
                        }
                    }
                    else
                    {
                        DateTime modifiedon = DateTime.Now.ToUniversalTime();

                        


                        sql = "update claims_base set status = 3, stage = " + (stage + 1) + ", ModifiedOn = @modifiedon, ModifiedBy = @userid, ";
                        

                        if (signofftype == 3)
                        {
                            sql = sql + "checkerid = null";
                            sql = sql + ", teamid = " + relid;
                            claim.teamid = relid;
                            claim.checkerid = 0;

                            reqteam = clsteams.GetTeamById(relid);
                            

                            nextcheckerid = new int[reqteam.teammembers.Count];
                            foreach (int t in reqteam.teammembers)
                            {
                                
                                nextcheckerid[reqteam.teammembers.IndexOf(t)] = t;
                            }
                            updateClaimHistory(claim, "The claim has been sent to the next stage and is waiting to be allocated to an approver", commenter);
                        }
                        else
                        {
                            sql = sql + "checkerid = " + nextcheckerid[0];
                            nextchecker = clsemployees.GetEmployeeById(nextcheckerid[0]);
                            if (submitting == true)
                            {
                                updateClaimHistory(claim, "Claim No " + claim.claimno + " submitted. The claim has been sent to the next stage and is awaiting approval by " + nextchecker.title + " " + nextchecker.firstname + " " + nextchecker.surname, commenter);
                            }
                            else
                            {
                                updateClaimHistory(claim, "The claim has been sent to the next stage and is awaiting approval by " + nextchecker.title + " " + nextchecker.firstname + " " + nextchecker.surname, commenter);
                            }
                        }
                        sql = sql + " where claimid = @claimid";
                        expdata.sqlexecute.Parameters.AddWithValue("@modifiedon", modifiedon);
                        expdata.sqlexecute.Parameters.AddWithValue("@userid", employeeid);
                        expdata.sqlexecute.Parameters.AddWithValue("@claimid", claim.claimid);
                        expdata.sqlexecute.Parameters.AddWithValue("@employeeid", claim.employeeid);
                        expdata.ExecuteSQL(sql);

                        if (reqstage.sendmail == true)
                        {
                            clsemails.claim = claim;
                            clsemails.sendMessage(2, claim.employeeid, nextcheckerid);
                        }
                        int[] arremp = new int[1];
                        arremp[0] = claim.employeeid;
                        claim.checkerid = nextcheckerid[0];
                        if (reqstage.claimantmail == true)
                        {
                            if (nextcheckerid[0] == 0)
                            {

                                nextcheckerid[0] = 0;
                            }
                            clsemails.claim = claim;
                            clsemails.sendMessage(11, nextcheckerid[0], arremp);
                        }


                        break;
                    }

                }

            }
            expdata.sqlexecute.Parameters.Clear();



            return 0;
        }
        private bool userOnHoliday(byte signofftype, int relid)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            bool userOnHoliday = false;
            int count = 0;
            DateTime today = DateTime.Today;
            expdata.sqlexecute.Parameters.AddWithValue("@relid", relid);
            switch (signofftype)
            {
                case 1: //cost code
                    strsql = "select count(*) from holidays inner join budgetholders on budgetholders.employeeid = holidays.employeeid where holidays.employeeid = (select employeeid from budgetholders where  budgetholders.budgetholderid = @relid) and ('" + today.Year + "/" + today.Month + "/" + today.Day + "' between startdate and enddate)";
                    count = expdata.getcount(strsql);
                    if (count != 0)
                    {
                        userOnHoliday = true;
                    }
                    break;
                case 2: //employee
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

        private int getNextCheckerId(cClaim claim, byte signofftype, int relid)
        {

            string strsql;
            int nextcheckerid = 0;
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            switch (signofftype)
            {
                case 1: //costcode
                    expdata.sqlexecute.Parameters.AddWithValue("@relid", relid);
                    strsql = "select employeeid from budgetholders where budgetholderid = @relid";
                    nextcheckerid = expdata.getcount(strsql);
                    expdata.sqlexecute.Parameters.Clear();
                    break;
                case 2:
                    nextcheckerid = relid;
                    break;
                case 3:
                    nextcheckerid = 0;

                    break;
                case 4:
                    cEmployees clsemployees = new cEmployees(accountid);
                    cEmployee reqemp = clsemployees.GetEmployeeById(claim.employeeid);
                    nextcheckerid = reqemp.linemanager;
                    break;
            }

            return nextcheckerid;


        }
        private void approveClaim(cClaim claim, int employeeid, int? delegateid)
        {
            int commenter;
            if (delegateid == null)
            {
                commenter = employeeid;
            }
            else
            {
                commenter = (int)delegateid;
            }

            DateTime modifiedon = DateTime.Now.ToUniversalTime();

            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            expdata.sqlexecute.Parameters.Clear();
            expdata.sqlexecute.Parameters.AddWithValue("@claimid", claim.claimid);
            expdata.sqlexecute.Parameters.AddWithValue("@modifiedon", modifiedon);
            expdata.sqlexecute.Parameters.AddWithValue("@userid", employeeid);
            strsql = "update savedexpenses_current set tempallow = 1, returned = 0 where claimid = @claimid";
            expdata.ExecuteSQL(strsql);
            strsql = "update claims_base set approved = 1, status = 6, ModifiedOn = @modifiedon, ModifiedBy = @userid where claimid = @claimid";
            expdata.ExecuteSQL(strsql);
            expdata.sqlexecute.Parameters.Clear();
            changeStatus(claim, ClaimStatus.ClaimApproved, employeeid);
            updateClaimHistory(claim, "The claim has been approved and is awaiting payment", commenter);

            claim.approveClaim();
        }
        public bool checkCostCodeIncluded(cClaim claim, int costcodeid)
        {
            foreach (cExpenseItem item in claim.expenseitems.Values)
            {
                foreach (cDepCostItem breakdown in item.costcodebreakdown)
                {
                    if (breakdown.costcodeid == costcodeid)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        public bool checkExpenseItemIncluded(cClaim claim, int subcatid)
        {
            foreach (cExpenseItem item in claim.expenseitems.Values)
            {
                if (item.subcatid == subcatid)
                {
                    return true;
                }
            }
            return false;
        }
        public bool includesItemOlderThanXDays(cClaim claim, int days)
        {
            DateTime date = DateTime.Today;
            date = date.AddDays(days / -1);
            foreach (cExpenseItem item in claim.expenseitems.Values)
            {
                if (item.date < date)
                {
                    return true;
                }
            }
            return false;
        }
        #endregion

        public Dictionary<int, cClaimHistory> getModifiedClaimHistory(DateTime date, int employeeid)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            Dictionary<int, cClaimHistory> lstclaimhistory = new Dictionary<int, cClaimHistory>();
            List<int> lstClaimids = new List<int>();
            SqlDataReader reader;
            int claimhistoryid, claimid, stage;
     
            string comment;
            DateTime datestamp;

            expdata.sqlexecute.Parameters.AddWithValue("@date", date);
            expdata.sqlexecute.Parameters.AddWithValue("@employeeid", employeeid);
            strsql = "SELECT claimid from claimhistory WHERE employeeid = @employeeid AND createdon > @date;";
            reader = expdata.GetReader(strsql);

            while (reader.Read())
            {
                claimid = reader.GetInt32(reader.GetOrdinal("claimid"));
                if (!lstClaimids.Contains(claimid))
                {
                    lstClaimids.Add(claimid);
                }
            }
            reader.Close();
            


            foreach (int id in lstClaimids)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@claimid", id);
                strsql = "SELECT claimhistoryid, claimid, datestamp, comment, stage FROM claimhistory WHERE claimid = @claimid;";
                reader = expdata.GetReader(strsql);

                while (reader.Read())
                {
                    claimhistoryid = reader.GetInt32(reader.GetOrdinal("claimhistoryid"));
                    claimid = reader.GetInt32(reader.GetOrdinal("claimid"));
                    if (reader.IsDBNull(reader.GetOrdinal("datestamp")) == false)
                    {
                        datestamp = reader.GetDateTime(reader.GetOrdinal("datestamp"));
                    }
                    else
                    {
                        datestamp = new DateTime(1900, 01, 01);
                    }
                    comment = reader.GetString(reader.GetOrdinal("comment"));
                    stage = reader.GetByte(reader.GetOrdinal("stage"));

                    cClaimHistory claimhistory = new cClaimHistory(claimhistoryid, claimid, comment, employeeid, datestamp, stage);
                    lstclaimhistory.Add(claimhistory.claimhistoryid, claimhistory);
                }
                expdata.sqlexecute.Parameters.Clear();
                reader.Close();
                
            }
            return lstclaimhistory;
        }
    }
}

        
