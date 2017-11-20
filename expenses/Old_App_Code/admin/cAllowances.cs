using System;
using System.Collections;
using System.Collections.Generic;
using ExpensesLibrary;
using expenses.Old_App_Code;
using System.Web.Caching;
using SpendManagementLibrary;
namespace expenses
{
	/// <summary>
	/// Summary description for cAllowances.
	/// </summary>
	public class cAllowances
	{
        SortedList list;
		string strsql;
		int accountid;

		System.Web.Caching.Cache Cache = (System.Web.Caching.Cache)System.Web.HttpRuntime.Cache;

		public cAllowances(int nAccountid)
		{
			accountid = nAccountid;
            
			
			InitialiseData();
		}

		private bool exists (string allowance, int allowanceid, int action)
		{
			int i;
            cAllowance clsallowance;
			for (i = 0; i < list.Count; i++)
			{
                clsallowance = (cAllowance)list.GetByIndex(i);
				if (action == 2)
				{
					if ((string)clsallowance.allowance.ToLower() == allowance.ToLower() && clsallowance.allowanceid != allowanceid)
					{
						return true;
					}
				}
				else
				{
					if (clsallowance.allowance.ToLower() == allowance.ToLower())
					{
						return true;
					}
				}
			}
			return false;
		}
		public int addAllowance(string allowance, string description, int nighthours, decimal nightrate, object[,] breakdown, int currencyid)
		{
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
			cAuditLog clsaudit = new cAuditLog();

            CurrentUser user = new CurrentUser();
            user = cMisc.getCurrentUser(System.Web.HttpContext.Current.User.Identity.Name);
            int userid = user.employeeid;
            DateTime createdon = DateTime.Now.ToUniversalTime();

			if (exists(allowance,0,0) == true)
			{
				return 1;
			}
			int allowanceid;
			strsql = "insert into allowances (allowance, description, nighthours, nightrate, currencyid, createdon, createdby) " + 
				"values (@allowance, @description,@nighthours,@nightrate, @currencyid, @createdon, @userid);select @identity = scope_identity()";

            expdata.sqlexecute.Parameters.AddWithValue("@allowance", allowance);
            expdata.sqlexecute.Parameters.AddWithValue("@description", description);
            expdata.sqlexecute.Parameters.AddWithValue("@nighthours", nighthours);
            expdata.sqlexecute.Parameters.AddWithValue("@nightrate", nightrate);
            expdata.sqlexecute.Parameters.AddWithValue("@createdon", createdon);
            expdata.sqlexecute.Parameters.AddWithValue("@userid", userid);
			if (currencyid == 0)
			{
                expdata.sqlexecute.Parameters.AddWithValue("@currencyid", DBNull.Value);
			}
			else
			{
                expdata.sqlexecute.Parameters.AddWithValue("@currencyid", currencyid);
			}
            expdata.sqlexecute.Parameters.AddWithValue("@identity", System.Data.SqlDbType.Int);
            expdata.sqlexecute.Parameters["@identity"].Direction = System.Data.ParameterDirection.Output;
			expdata.ExecuteSQL(strsql);
            allowanceid = (int)expdata.sqlexecute.Parameters["@identity"].Value;
			
			insertBreakdown(breakdown, allowanceid);

			expdata.sqlexecute.Parameters.Clear();
            
			clsaudit.addRecord("Allowances",allowance);
			return 0;

		}

		public int updateAllowance(int allowanceid, string allowance, string description, int nighthours, decimal nightrate, object[,] breakdown, int currencyid)
		{
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
			cAuditLog clsaudit = new cAuditLog();
			cAllowance reqallowance = getAllowanceById(allowanceid);
			if (exists(allowance,allowanceid,2) == true)
			{
				return 1;
			}

            CurrentUser user = new CurrentUser();
            user = cMisc.getCurrentUser(System.Web.HttpContext.Current.User.Identity.Name);
            int userid = user.employeeid;
            DateTime modifiedon = DateTime.Now.ToUniversalTime();

			strsql = "update allowances set allowance = @allowance, description = @description, nighthours = @nighthours, nightrate = @nightrate, currencyid = @currencyid, modifiedon = @modifiedon, modifiedby = @modifiedby where allowanceid = @allowanceid";
			
			expdata.sqlexecute.Parameters.AddWithValue("@allowance",allowance);
            expdata.sqlexecute.Parameters.AddWithValue("@description", description);
            expdata.sqlexecute.Parameters.AddWithValue("@nighthours", nighthours);
            expdata.sqlexecute.Parameters.AddWithValue("@nightrate", nightrate);
            expdata.sqlexecute.Parameters.AddWithValue("@allowanceid", allowanceid);
			if (currencyid == 0)
			{
                expdata.sqlexecute.Parameters.AddWithValue("@currencyid", DBNull.Value);
			}
			else
			{
                expdata.sqlexecute.Parameters.AddWithValue("@currencyid", currencyid);
			}
            expdata.sqlexecute.Parameters.AddWithValue("@modifiedon", modifiedon);
            expdata.sqlexecute.Parameters.AddWithValue("@modifiedby", userid);
			expdata.ExecuteSQL(strsql);
			expdata.sqlexecute.Parameters.Clear();
			insertBreakdown(breakdown,allowanceid);
            
			#region auditlog
			if (reqallowance.allowance != null)
			{
				if (allowance != reqallowance.allowance)
				{
					clsaudit.editRecord(allowance,"Allowance","Allowances",reqallowance.allowance,allowance);
				}
				if (description != reqallowance.description)
				{
					clsaudit.editRecord(allowance,"Description","Allowances", reqallowance.description,description);
				}
				if (nighthours != reqallowance.nighthours)
				{
					clsaudit.editRecord(allowance,"Night Hours","Allowances",reqallowance.nighthours.ToString(),nighthours.ToString());
				}
				if (nightrate != reqallowance.nightrate)
				{
					clsaudit.editRecord(allowance,"Night Rate","Allowances",reqallowance.nightrate.ToString("£###,###,##0.00"),nightrate.ToString("£###,###,##0.00"));
				}
			}
			#endregion
			return 0;
		}

		public int deleteAllowance(int allowanceid)
		{
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
			cAllowance reqallowance = getAllowanceById(allowanceid);
			cAuditLog clsaudit = new cAuditLog();
            int count;
            //has it already been used on expense items
            strsql = "select count(*) from savedexpenses where allowanceid = @allowanceid";
            expdata.sqlexecute.Parameters.AddWithValue("@allowanceid", allowanceid);
            count = expdata.getcount(strsql);

            if (count > 0)
            {
                expdata.sqlexecute.Parameters.Clear();
                return -1;
            }


			strsql = "delete from allowances where allowanceid = @allowanceid";
			
			expdata.ExecuteSQL(strsql);
			expdata.sqlexecute.Parameters.Clear();

			
			clsaudit.deleteRecord("Allowances",reqallowance.allowance);
            return 0;
		}

        private void insertBreakdown(object[,] breakdown, int allowanceid)
		{
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
			int i;

			deleteBreakdown(allowanceid);
			strsql = "";

			expdata.sqlexecute.Parameters.AddWithValue("@allowanceid",allowanceid);
			for (i = 0; i < breakdown.GetLength(0); i++)
			{
				strsql += "insert into allowancebreakdown (allowanceid, hours, rate) " +
					"values (@allowanceid,@breakdownhours" + i + ",@breakdownrate" + i + ")" +
                    ";";
                expdata.sqlexecute.Parameters.AddWithValue("@breakdownhours" + i, breakdown[i, 0]);
                expdata.sqlexecute.Parameters.AddWithValue("@breakdownrate" + i, breakdown[i, 1]);
			}
			if (strsql != "")
			{
				expdata.ExecuteSQL(strsql);
			}
			expdata.sqlexecute.Parameters.Clear();
		}

		private void deleteBreakdown(int allowanceid)
		{
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
			strsql = "delete from allowancebreakdown where allowanceid = @allowanceid";
			expdata.sqlexecute.Parameters.AddWithValue("@allowanceid",allowanceid);
			expdata.ExecuteSQL(strsql);
			expdata.sqlexecute.Parameters.Clear();
		}

		private void InitialiseData()
		{
            list = (SortedList)Cache["allowances" + accountid];
			if (list == null)
			{
				list = CacheList();
			}
			
		}

        private SortedList sortList()
        {
            SortedList sorted = new SortedList();
            cAllowance allowance;
            for (int i = 0; i < list.Count; i++)
            {
                allowance = (cAllowance)list.GetByIndex(i);
                sorted.Add(allowance.allowance, allowance);
            }
            return sorted;
        }
        public System.Data.DataSet getGrid()
		{
            SortedList sorted = sortList();
            object[] values;
            cAllowance allowance;
            System.Data.DataSet ds = new System.Data.DataSet();
            System.Data.DataTable tbl = new System.Data.DataTable();

            tbl.Columns.Add("allowanceid", System.Type.GetType("System.Int32"));
            tbl.Columns.Add("allowance", System.Type.GetType("System.String"));
            tbl.Columns.Add("description", System.Type.GetType("System.String"));

            for (int i = 0; i < sorted.Count; i++)
            {
                allowance = (cAllowance)sorted.GetByIndex(i);
                values = new object[3];
                values[0] = allowance.allowanceid;
                values[1] = allowance.allowance;
                values[2] = allowance.description;
                tbl.Rows.Add(values);
            }

            ds.Tables.Add(tbl);
            return ds;
		}

        public System.Data.DataSet getBreakdown(int allowanceid)
		{
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
			System.Data.DataSet rcdsttemp = new System.Data.DataSet();

			strsql = "select hours, rate from allowancebreakdown where allowanceid = @allowanceid order by hours";
			expdata.sqlexecute.Parameters.AddWithValue("@allowanceid",allowanceid);
			rcdsttemp = expdata.GetDataSet(strsql);
			expdata.sqlexecute.Parameters.Clear();
			return rcdsttemp;
		}
		private SortedList CacheList()
		{
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            SortedList list = new SortedList();
            int allowanceid, currencyid, nighthours;
            string allowance, description;
            decimal nightrate;
            DateTime createdon, modifiedon;
            int createdby, modifiedby;
            cAllowance newallowance;
            System.Data.SqlClient.SqlDataReader reader;

            SortedList<string, object> parameters = new SortedList<string, object>();

            AggregateCacheDependency aggdep = new AggregateCacheDependency();

            strsql = "SELECT breakdownid, allowanceid, hours, rate FROM dbo.allowancebreakdown";
            SqlCacheDependency breakdowndep = expdata.CreateSQLCacheDependency(strsql, parameters);

			strsql = "SELECT     allowanceid, allowance, description, nighthours, nightrate, currencyid, CreatedOn, CreatedBy, ModifiedOn, ModifiedBy FROM dbo.allowances";
            expdata.sqlexecute.CommandText = strsql;
            SqlCacheDependency allowancedep = expdata.CreateSQLCacheDependency(strsql, parameters);

            aggdep.Add(new CacheDependency[] { breakdowndep, allowancedep });

            reader = expdata.GetReader(strsql);

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
                    modifiedon = new DateTime(1900, 01, 01);
                }
                else
                {
                    modifiedon = reader.GetDateTime(reader.GetOrdinal("modifiedon"));
                }
                if (reader.IsDBNull(reader.GetOrdinal("modifiedby")) == true)
                {
                    modifiedby = 0;
                }
                else
                {
                    modifiedby = reader.GetInt32(reader.GetOrdinal("modifiedby"));
                }

                newallowance = new cAllowance(accountid, allowanceid, allowance, description, currencyid, nighthours, nightrate, createdon, createdby, modifiedon, modifiedby, cAccounts.getConnectionString(accountid), getAllowanceBreakdown(allowanceid));
                list.Add(allowanceid, newallowance);
            }
            reader.Close();
			
			Cache.Insert("allowances" + accountid,list,aggdep, System.Web.Caching.Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(15), CacheItemPriority.NotRemovable, null);
            expdata.sqlexecute.Parameters.Clear();
            
            return list;
		}

        #region caching
        System.Web.Caching.CacheDependency getDependency()
        {
            System.Web.Caching.CacheDependency dep;
            String[] dependency;
            dependency = new string[1];
            dependency[0] = "allowancesdependency" + accountid;
            dep = new System.Web.Caching.CacheDependency(null, dependency);
            return dep;
        }
        private void CreateDependency()
        {
            if (Cache["allowancesdependency" + accountid] == null)
            {
                Cache.Insert("allowancesdependency" + accountid, 1);
            }
        }

        private void InvalidateCache()
        {
            Cache.Remove("allowancesdependency" + accountid);
            CreateDependency();
        }
        #endregion


        public List<cAllowanceBreakdown> getAllowanceBreakdown(int allowanceid)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            string strsql;

            int breakdownid, hours;
            decimal rate;
            System.Data.SqlClient.SqlDataReader reader;
            expdata.sqlexecute.Parameters.AddWithValue("@allowanceid", allowanceid);

            List<cAllowanceBreakdown> breakdown = new List<cAllowanceBreakdown>();
            cAllowanceBreakdown clsbreakdown;

            strsql = "select * from allowancebreakdown where allowanceid = @allowanceid order by hours";
            reader = expdata.GetReader(strsql);

            while (reader.Read())
            {
                breakdownid = reader.GetInt32(reader.GetOrdinal("breakdownid"));
                hours = reader.GetInt32(reader.GetOrdinal("hours"));
                rate = reader.GetDecimal(reader.GetOrdinal("rate"));
                clsbreakdown = new cAllowanceBreakdown(breakdownid, allowanceid, hours, rate);
                breakdown.Add(clsbreakdown);
            }
            reader.Close();


            expdata.sqlexecute.Parameters.Clear();
            return breakdown;

        }

		public cAllowance getAllowanceById(int allowanceid)
		{
            return (cAllowance)list[allowanceid];

		}

		public System.Web.UI.WebControls.ListItem[] CreateDropDown()
		{
            cAllowance allowance;
            SortedList sorted = sortList();
            System.Web.UI.WebControls.ListItem[] items = new System.Web.UI.WebControls.ListItem[sorted.Count];

            for (int i = 0; i < sorted.Count; i++)
            {
                allowance = (cAllowance)sorted.GetByIndex(i);
                items[i] = new System.Web.UI.WebControls.ListItem(allowance.allowance, allowance.allowanceid.ToString());
            }

			
			return items;
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
		public string CreateStringDropDown(int[] allowanceids)
		{
			System.Text.StringBuilder output = new System.Text.StringBuilder();
			int i = 0;
			int x;
			
			bool display;
            cAllowance allowance;
			if (allowanceids == null)
			{
				return "";
			}

            SortedList sorted = sortList();
			for (i = 0; i < sorted.Count; i++)
			{
                allowance = (cAllowance)sorted.GetByIndex(i);
				display = false;
				
				for (x = 0; x < allowanceids.Length; x++)
				{
					if (allowanceids[x] == allowance.allowanceid)
					{
						display = true;
						break;
					}
				}

				if (display == true)
				{
					output.Append("<option value=\"" + allowance.allowanceid + "\">");
					output.Append(allowance.allowance + "</option>");
				}
			}

			return output.ToString();
		}





        public void calculateDailyAllowance(int employeeid, ref cExpenseItem reqitem)
        {
            //calculate num hours

            cAllowance reqallowance;
            DateTime startdate;
            DateTime enddate;
            cSubcats clssubcats = new cSubcats(accountid);
            cSubcat reqsubcat;

            bool gotRate = false;
            reqsubcat = clssubcats.getSubcatById(reqitem.subcatid);
            if (reqsubcat == null)
            {
                return;
            }
            if (reqsubcat.calculation != CalculationType.DailyAllowance)
            {
                return;
            }

            int numhours;
            int i = 0;
            decimal total = 0;

            cMisc clsmisc = new cMisc(accountid);
            cGlobalProperties clsproperties = clsmisc.getGlobalProperties(accountid);

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
                cCurrencies clscurrencies = new cCurrencies(accountid);
                cCurrency reqcurrency;
                double exchangerate = 0;
                decimal convertedtotal;

                

                

                reqcurrency = clscurrencies.getCurrencyById(basecurrency);

				switch (clscurrencies.currencytype)
				{
					case CurrencyType.Static:
						cStaticCurrency reqstandard;
						reqstandard = (cStaticCurrency)reqcurrency;
						exchangerate = reqstandard.getExchangeRate(reqallowance.currencyid);
						break;
                    case CurrencyType.Monthly:
						cMonthlyCurrency reqmonthly;
						reqmonthly = (cMonthlyCurrency)reqcurrency;
						exchangerate = reqmonthly.getExchangeRate((byte)reqitem.date.Month,reqitem.date.Year, reqallowance.currencyid);
						break;
                    case CurrencyType.Range:
						cRangeCurrency reqrange;
						reqrange = (cRangeCurrency)reqcurrency;
						exchangerate = reqrange.getExchangeRate(reqitem.date, reqallowance.currencyid);
						break;
				}
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
    }

   
}

