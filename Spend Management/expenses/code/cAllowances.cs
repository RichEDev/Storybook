namespace Spend_Management
{
    using System;
    using System.Collections.Generic;
    using System.Web.UI.WebControls;

    using BusinessLogic.DataConnections;
    using BusinessLogic.GeneralOptions;

    using SpendManagementLibrary;
    using SpendManagementLibrary.Employees;

    using Utilities.DistributedCaching;

    /// <summary>
	/// Summary description for cAllowances.
	/// </summary>
	public class cAllowances
	{
        SortedList<int,cAllowance> list;
		private readonly Cache cache = new Cache();
	    
        /// <summary>
	    /// The Key in the cache for Allowances.
	    /// </summary>
	    public const string CacheAreaAllowance = "allowances";

	    readonly int accountid;

        public cAllowances(int nAccountid)
		{
			accountid = nAccountid;
            
			InitialiseData();
		}

        /// <summary>
        /// Returns the list of cAllowances from the cache.
        /// This method is currently only used by SpendManagementApi, for gets.
        /// The public list shouldn't be edited.
        /// </summary>
        public SortedList<int, cAllowance> GetCacheList()
        {
            return list;
        }
		
        public int saveAllowance(cAllowance allowance)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();

            int allowanceid;
            DBConnection data = new DBConnection(cAccounts.getConnectionString(accountid));
            data.sqlexecute.Parameters.AddWithValue("@allowanceid", allowance.allowanceid);
            data.sqlexecute.Parameters.AddWithValue("@allowance", allowance.allowance);
            if (allowance.description == "")
            {
                data.sqlexecute.Parameters.AddWithValue("@description", DBNull.Value);
            }
            else
            {
                data.sqlexecute.Parameters.AddWithValue("@description", allowance.description);
            }
            data.sqlexecute.Parameters.AddWithValue("@nighthours", allowance.nighthours);
            data.sqlexecute.Parameters.AddWithValue("@nightrate", allowance.nightrate);
            data.sqlexecute.Parameters.AddWithValue("@currencyid", allowance.currencyid);
            if (allowance.allowanceid == 0)
            {
                data.sqlexecute.Parameters.AddWithValue("@userid", allowance.createdby);
                data.sqlexecute.Parameters.AddWithValue("@date", allowance.createdon);
            }
            else
            {
                data.sqlexecute.Parameters.AddWithValue("@userid", allowance.modifiedby);
                data.sqlexecute.Parameters.AddWithValue("@date", allowance.modifiedon);
            }
            if (currentUser.isDelegate == true)
            {
                data.sqlexecute.Parameters.AddWithValue("@delegateID", currentUser.Delegate.EmployeeID);
            }
            else
            {
                data.sqlexecute.Parameters.AddWithValue("@delegateID", DBNull.Value);
            }

            data.sqlexecute.Parameters.Add("@identity", System.Data.SqlDbType.Int);
            data.sqlexecute.Parameters["@identity"].Direction = System.Data.ParameterDirection.ReturnValue;
            data.ExecuteProc("saveAllowance");
            allowanceid = (int)data.sqlexecute.Parameters["@identity"].Value;
            data.sqlexecute.Parameters.Clear();

            InvalidateCache();

            return allowanceid;
        }
		public int deleteAllowance(int allowanceid)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();

            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            int returnvalue;
            expdata.sqlexecute.Parameters.AddWithValue("@allowanceid", allowanceid);
            expdata.sqlexecute.Parameters.AddWithValue("@employeeID", currentUser.EmployeeID);
            if (currentUser.isDelegate == true)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@delegateID", currentUser.Delegate.EmployeeID);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@delegateID", DBNull.Value);
            }
            expdata.sqlexecute.Parameters.Add("@returnvalue", System.Data.SqlDbType.Int);
            expdata.sqlexecute.Parameters["@returnvalue"].Direction = System.Data.ParameterDirection.ReturnValue;
            expdata.ExecuteProc("deleteAllowance");
            returnvalue = (int)expdata.sqlexecute.Parameters["@returnvalue"].Value;
			expdata.sqlexecute.Parameters.Clear();

            InvalidateCache();

            return returnvalue;
			
		}

        
		
		private void InitialiseData()
		{
            list = cache.Get(accountid, CacheAreaAllowance, "0") as SortedList<int, cAllowance> ?? CacheList();
		}

        private void InvalidateCache()
        {
            cache.Delete(accountid, CacheAreaAllowance, "0");
            list = null;
        }

        public SortedList<string, cAllowance> sortList()
        {
            SortedList<string,cAllowance> sorted = new SortedList<string,cAllowance>();
            
            foreach (cAllowance allowance in list.Values)
            {
                sorted.Add(allowance.allowance, allowance);
            }
            return sorted;
        }
        public string createGrid()
        {
            return "select allowanceid, allowance, description from allowances";
        }
        
        public string getBreakdown()
		{
            return "select breakdownid, hours, rate from allowancebreakdown";
		}

		private SortedList<int,cAllowance> CacheList()
		{
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            SortedList<int, cAllowance> list = new SortedList<int, cAllowance>();
            int allowanceid, currencyid, nighthours;
            string allowance, description;
            decimal nightrate;
            DateTime createdon;
            DateTime? modifiedon;
            int createdby;
            int? modifiedby;
            cAllowance newallowance;
            
            SortedList<int, List<cAllowanceBreakdown>> lstBreakdown = getAllowanceBreakdown();
            List<cAllowanceBreakdown> breakdown = new List<cAllowanceBreakdown>();
            SortedList<string, object> parameters = new SortedList<string, object>();

		    var strsql = "SELECT allowanceid, allowance, description, nighthours, nightrate, currencyid, CreatedOn, CreatedBy, ModifiedOn, ModifiedBy FROM dbo.allowances";
            using (System.Data.SqlClient.SqlDataReader reader = expdata.GetReader(strsql))
		    {
		        while (reader.Read())
		        {
		            allowanceid = reader.GetInt32(reader.GetOrdinal("allowanceid"));
		            allowance = reader.GetString(reader.GetOrdinal("allowance"));
		            if (reader.IsDBNull(reader.GetOrdinal("description")) == false)
		            {
		                description = reader.GetString(reader.GetOrdinal("description"));
		            }
		            else
		            {
		                description = "";
		            }
		            if (reader.IsDBNull(reader.GetOrdinal("currencyid")) == false)
		            {
		                currencyid = reader.GetInt32(reader.GetOrdinal("currencyid"));
		            }
		            else
		            {
		                currencyid = 0;
		            }
		            nighthours = reader.GetInt32(reader.GetOrdinal("nighthours"));
		            nightrate = reader.GetDecimal(reader.GetOrdinal("nightrate"));
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
		                modifiedon = null;
		            }
		            else
		            {
		                modifiedon = reader.GetDateTime(reader.GetOrdinal("modifiedon"));
		            }
		            if (reader.IsDBNull(reader.GetOrdinal("modifiedby")) == true)
		            {
		                modifiedby = null;
		            }
		            else
		            {
		                modifiedby = reader.GetInt32(reader.GetOrdinal("modifiedby"));
		            }
		            lstBreakdown.TryGetValue(allowanceid, out breakdown);
		            if (breakdown == null)
		            {
		                breakdown = new List<cAllowanceBreakdown>();
		            }
		            newallowance = new cAllowance(accountid, allowanceid, allowance, description, currencyid, nighthours, nightrate, createdon, createdby, modifiedon, modifiedby, breakdown);
		            list.Add(allowanceid, newallowance);
		        }
		        reader.Close();
		    }

            cache.Add(accountid, CacheAreaAllowance, "0", list);
            expdata.sqlexecute.Parameters.Clear();
            
            return list;
		}

        public SortedList<int,List<cAllowanceBreakdown>> getAllowanceBreakdown()
        {
            SortedList<int, List<cAllowanceBreakdown>> allowances = new SortedList<int, List<cAllowanceBreakdown>>();
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            string strsql;
            int allowanceid;
            int breakdownid, hours;
            decimal rate;

            List<cAllowanceBreakdown> breakdown;
            cAllowanceBreakdown clsbreakdown;

            strsql = "select * from allowancebreakdown order by hours";
            using (System.Data.SqlClient.SqlDataReader reader = expdata.GetReader(strsql))
            {
                while (reader.Read())
                {
                    allowanceid = reader.GetInt32(reader.GetOrdinal("allowanceid"));
                    breakdownid = reader.GetInt32(reader.GetOrdinal("breakdownid"));
                    hours = reader.GetInt32(reader.GetOrdinal("hours"));
                    rate = reader.GetDecimal(reader.GetOrdinal("rate"));
                    clsbreakdown = new cAllowanceBreakdown(breakdownid, allowanceid, hours, rate);
                    allowances.TryGetValue(allowanceid, out breakdown);
                    if (breakdown == null)
                    {
                        breakdown = new List<cAllowanceBreakdown>();
                        allowances.Add(allowanceid, breakdown);
                    }
                    breakdown.Add(clsbreakdown);
                }
                reader.Close();
            }

            expdata.sqlexecute.Parameters.Clear();
            return allowances;

        }

		public cAllowance getAllowanceById(int allowanceid)
		{
            cAllowance allowance = null;
		    try
		    {
                list.TryGetValue(allowanceid, out allowance);
		    }
		    catch (Exception)
		    {
		        return null;
		    }
		    return allowance;
		}

		public ListItem[] CreateDropDown()
		{
            
            SortedList<string,cAllowance> sorted = sortList();
            List<ListItem> items = new List<ListItem>();

            foreach (cAllowance allowance in sorted.Values)
            {
                items.Add(new System.Web.UI.WebControls.ListItem(allowance.allowance, allowance.allowanceid.ToString()));
            }

			
			return items.ToArray();
		}

        public List<System.Web.UI.WebControls.ListItem> CreateDropDown(List<int> ids)
        {
            List<System.Web.UI.WebControls.ListItem> items = new List<System.Web.UI.WebControls.ListItem>();
            cAllowance allowance;

            foreach (int allowanceid in ids)
            {
                allowance = getAllowanceById(allowanceid);
                items.Add(new System.Web.UI.WebControls.ListItem(allowance.allowance, allowanceid.ToString()));
            }
            return items;
        }

        /// <summary>
        /// Calculate daily allowance
        /// </summary>
        /// <param name="employeeid">The employee id</param>
        /// <param name="reqitem">The item</param>
        /// <param name="generalOptionsFactory">An instance of <see cref="IDataFactory{IGeneralOptions,Int32}"/> to get a <see cref="IGeneralOptions"/></param>
        public void calculateDailyAllowance(int employeeid, ref cExpenseItem reqitem, IDataFactory<IGeneralOptions, int> generalOptionsFactory)
        {
            //calculate num hours

            cEmployees clsemployees = new cEmployees(accountid);
            Employee reqemp = clsemployees.GetEmployeeById(employeeid);
            int subAccountID = reqemp.DefaultSubAccount;

            cAllowance reqallowance;
            DateTime startdate;
            DateTime enddate;
            cSubcats clssubcats = new cSubcats(accountid);

            bool gotRate = false;
            SubcatBasic reqsubcat = clssubcats.GetSubcatBasic(reqitem.subcatid);
            if (reqsubcat == null)
            {
                return;
            }
            if (reqsubcat.CalculationType != CalculationType.DailyAllowance)
            {
                return;
            }

            int numhours;
            int i = 0;
            decimal total = 0;

            int basecurrency;

            if (reqemp.PrimaryCurrency != 0)
            {
                basecurrency = reqemp.PrimaryCurrency;
            }
            else
            {
                var generalOptions = generalOptionsFactory[cMisc.GetCurrentUser().CurrentSubAccountId].WithCurrency();
                basecurrency = (int)generalOptions.Currency.BaseCurrency;
            }

            startdate = reqitem.allowancestartdate;
            enddate = reqitem.allowanceenddate;
            reqallowance = getAllowanceById(reqitem.allowanceid);
            TimeSpan ts = enddate - startdate;

            numhours = ts.Hours + (24 * ts.Days);

            if (reqallowance.nighthours != 0)
            {
                while (numhours >= reqallowance.nighthours)
                {
                    total += reqallowance.nightrate;
                    numhours -= reqallowance.nighthours;
                }
            }
            List<cAllowanceBreakdown> breakdown = reqallowance.breakdown;

            cAllowanceBreakdown currentbreakdown;
            cAllowanceBreakdown nextbreakdown;
            cAllowanceBreakdown lastbreakdown;
            for (i = 0; i < breakdown.Count; i++)
            {
                currentbreakdown = (cAllowanceBreakdown)breakdown[i];
                switch (i)
                {
                    case 0:

                        if (numhours < currentbreakdown.hours)
                        {
                            break;
                        }
                        else
                        {
                            if (breakdown.Count > 1)
                            {
                                nextbreakdown = (cAllowanceBreakdown)breakdown[i + 1];
                                if (numhours >= currentbreakdown.hours && numhours < nextbreakdown.hours)
                                {
                                    total += currentbreakdown.rate;
                                    gotRate = true;
                                }
                            }
                            else if (breakdown.Count == 1)
                            {
                                if (numhours >= currentbreakdown.hours)
                                {
                                    total += currentbreakdown.rate;
                                    gotRate = true;
                                }
                            }
                        }
                        break;
                    default:
                        if (i == (breakdown.Count - 1))
                        {
                            if (currentbreakdown.hours <= numhours)
                            {
                                total += currentbreakdown.rate;
                                gotRate = true;
                                break;
                            }
                        }
                        else
                        {
                            lastbreakdown = (cAllowanceBreakdown)breakdown[i - 1];
                            nextbreakdown = (cAllowanceBreakdown)breakdown[i + 1];

                            if (numhours >= currentbreakdown.hours && numhours < nextbreakdown.hours)
                            {
                                total += currentbreakdown.rate;
                                gotRate = true;
                                break;
                            }
                            else if (numhours < currentbreakdown.hours && numhours >= lastbreakdown.hours)
                            {
                                total += currentbreakdown.rate;
                                gotRate = true;
                                break;
                            }
                            else if (numhours == currentbreakdown.hours)
                            {
                                total += currentbreakdown.rate;
                                gotRate = true;
                                break;
                            }
                        }
                        break;
                }

                reqitem.total = total;
                reqitem.amountpayable = total;
                if (gotRate == true)
                {
                    break;
                }

            }

            if (reqallowance.currencyid != basecurrency)
            {
                cCurrencies clscurrencies = new cCurrencies(accountid, subAccountID);
                cCurrency reqcurrency;
                double exchangerate = 0;
                decimal convertedtotal;

                reqcurrency = clscurrencies.getCurrencyById(basecurrency);
                exchangerate = reqcurrency.getExchangeRate(reqallowance.currencyid, reqitem.date);
                if (exchangerate == 0)
                {
                    reqitem.convertedtotal = 0;
                    reqitem.total = 0;
                    reqitem.amountpayable = 0;
                    return;
                }
                convertedtotal = reqitem.total;
                total = convertedtotal * (1 / (decimal)exchangerate);

                reqitem.convertedtotal = convertedtotal;
                reqitem.total = total;
                
                reqitem.amountpayable = total;
            }

            reqitem.total -= reqitem.allowancededuct;
            reqitem.amountpayable -= reqitem.allowancededuct;

        }

        public Dictionary<int, cAllowance> getModifiedAllowances(DateTime date)
        {
            Dictionary<int, cAllowance> lst = new Dictionary<int, cAllowance>();

            foreach (cAllowance val in list.Values)
            {
                if (val.createdon > date || val.modifiedon > date)
                {
                    lst.Add(val.allowanceid, val);
                }
            }
            return lst;
        }

        public List<int> getAllowanceIds()
        {
            List<int> lstallowanceids = new List<int>();

            foreach (cAllowance val in list.Values)
            {
                lstallowanceids.Add(val.allowanceid);
            }

            return lstallowanceids;
        }



        public int saveRate(cAllowanceBreakdown breakdown)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();

            int breakdownid;
            DBConnection data = new DBConnection(cAccounts.getConnectionString(accountid));
            data.sqlexecute.Parameters.AddWithValue("@breakdownid", breakdown.breakdownid);
            data.sqlexecute.Parameters.AddWithValue("@allowanceid", breakdown.allowanceid);
            data.sqlexecute.Parameters.AddWithValue("@hours", breakdown.hours);
            data.sqlexecute.Parameters.AddWithValue("@rate", breakdown.rate);
            data.sqlexecute.Parameters.AddWithValue("@employeeID", currentUser.EmployeeID);
            if (currentUser.isDelegate == true)
            {
                data.sqlexecute.Parameters.AddWithValue("@delegateID", currentUser.Delegate.EmployeeID);
            }
            else
            {
                data.sqlexecute.Parameters.AddWithValue("@delegateID", DBNull.Value);
            }
            data.sqlexecute.Parameters.Add("@returnvalue", System.Data.SqlDbType.Int);
            data.sqlexecute.Parameters["@returnvalue"].Direction = System.Data.ParameterDirection.ReturnValue;
            data.ExecuteProc("saveAllowanceBreakdown");
            breakdownid = (int)data.sqlexecute.Parameters["@returnvalue"].Value;
            data.sqlexecute.Parameters.Clear();
            
            InvalidateCache();
            
            return breakdownid;
        }

        public void deleteAllowanceBreakdown(int rateID)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();

            DBConnection data = new DBConnection(cAccounts.getConnectionString(accountid));
            data.sqlexecute.Parameters.AddWithValue("@breakdownid", rateID);
            data.sqlexecute.Parameters.AddWithValue("@employeeID", currentUser.EmployeeID);
            if (currentUser.isDelegate == true)
            {
                data.sqlexecute.Parameters.AddWithValue("@delegateID", currentUser.Delegate.EmployeeID);
            }
            else
            {
                data.sqlexecute.Parameters.AddWithValue("@delegateID", DBNull.Value);
            }
            data.ExecuteProc("deleteAllowanceBreakdown");
            data.sqlexecute.Parameters.Clear();
            
            InvalidateCache();
        }
    }

   
}

