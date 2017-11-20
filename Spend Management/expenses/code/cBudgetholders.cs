using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Caching;
using System.Data;
using System.Web.UI.WebControls;
using SpendManagementLibrary;
using SpendManagementLibrary.Employees;

namespace Spend_Management
{
	/// <summary>
	/// Summary description for budgetholders.
	/// </summary>
	public class cBudgetholders
	{
		string strsql;
		int nAccountid = 0;
        System.Web.Caching.Cache Cache = System.Web.HttpRuntime.Cache;
        SortedList<int, cBudgetHolder> list;
		public cBudgetholders(int accountid)
		{
			nAccountid = accountid;
            
			InitialiseData();
        }

        #region properties
        public int accountid
        {
            get { return nAccountid; }
        }
        #endregion
        public void InitialiseData()
		{
            
            list = (SortedList<int, cBudgetHolder>)Cache["budgetholders" + accountid];
            if (list == null)
            {
                list = CacheList();
            }
            
		}

        private SortedList<int,cBudgetHolder> CacheList()
        {
            SortedList<int, cBudgetHolder> list = new SortedList<int, cBudgetHolder>();
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            cBudgetHolder holder;
            int budgetholderid, employeeid;
            int? createdby, modifiedby;
            string budgetholder, description;
            DateTime? createdon, modifiedon;
            
            strsql = "SELECT     budgetholderid, budgetholder, description, employeeid, CreatedOn, CreatedBy, ModifiedOn, ModifiedBy FROM dbo.budgetholders";
            expdata.sqlexecute.CommandText = strsql;
            if (GlobalVariables.GetAppSettingAsBoolean("EnableBrokers"))
            {
                var dep = new SqlCacheDependency(expdata.sqlexecute);
                Cache.Insert("budgetholders" + accountid, list, dep, System.Web.Caching.Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes((double)Caching.CacheTimeSpans.Short), CacheItemPriority.Default, null);
            }

            using (System.Data.SqlClient.SqlDataReader reader = expdata.GetReader(strsql))
            {
                while (reader.Read())
                {
                    budgetholderid = reader.GetInt32(reader.GetOrdinal("budgetholderid"));
                    budgetholder = reader.GetString(reader.GetOrdinal("budgetholder"));
                    if (reader.IsDBNull(reader.GetOrdinal("description")) == true)
                    {
                        description = "";
                    }
                    else
                    {
                        description = reader.GetString(reader.GetOrdinal("description"));
                    }
                    employeeid = reader.GetInt32(reader.GetOrdinal("employeeid"));
                    if (reader.IsDBNull(reader.GetOrdinal("createdon")) == true)
                    {
                        createdon = null;
                    }
                    else
                    {
                        createdon = reader.GetDateTime(reader.GetOrdinal("createdon"));
                    }
                    if (reader.IsDBNull(reader.GetOrdinal("modifiedon")) == true)
                    {
                        modifiedon = null;
                    }
                    else
                    {
                        modifiedon = reader.GetDateTime(reader.GetOrdinal("modifiedon"));
                    }
                    if (reader.IsDBNull(reader.GetOrdinal("createdby")) == true)
                    {
                        createdby = null;
                    }
                    else
                    {
                        createdby = reader.GetInt32(reader.GetOrdinal("createdby"));
                    }
                    if (reader.IsDBNull(reader.GetOrdinal("modifiedby")) == true)
                    {
                        modifiedby = null;
                    }
                    else
                    {
                        modifiedby = reader.GetInt32(reader.GetOrdinal("modifiedby"));
                    }
                    holder = new cBudgetHolder(budgetholderid, budgetholder, description, employeeid, createdby, createdon, modifiedby, modifiedon);
                    list.Add(budgetholderid, holder);
                }
                reader.Close();
            }

            return list;
        }

        /// <summary>
        /// Gets a list of all the budget holders from the cache.
        /// </summary>
        /// <returns></returns>
        public List<cBudgetHolder> GetList()
        {
            if (list == null)
            {
                InitialiseData();
            }

            // ReSharper disable once PossibleNullReferenceException
            return list.Values.ToList();
        }

		public System.Data.DataSet getGrid()
		{
            
            cEmployees clsemployees = new cEmployees(accountid);
            object[] values;
		    DataSet ds = new DataSet();
            DataTable tbl = new DataTable();
            tbl.Columns.Add("budgetholderid",typeof(System.Int32));
            tbl.Columns.Add("budgetholder",typeof(System.String));
            tbl.Columns.Add("description", typeof(System.String));
            tbl.Columns.Add("employee", typeof(System.String));

            foreach (cBudgetHolder holder in list.Values)
            {
                values = new object[4];
                values[0] = holder.budgetholderid;
                values[1] = holder.budgetholder;
                values[2] = holder.description;
                Employee reqemp = clsemployees.GetEmployeeById(holder.employeeid);
                values[3] = reqemp.Surname + ", " + reqemp.Title + " " + reqemp.Forename;
                tbl.Rows.Add(values);
            }
            ds.Tables.Add(tbl);
            return ds;
		}
		public int saveBudgetHolder(cBudgetHolder holder)
		{
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
		    CurrentUser currentUser = cMisc.GetCurrentUser();
            expdata.sqlexecute.Parameters.AddWithValue("@budgetholderid", holder.budgetholderid);
            expdata.sqlexecute.Parameters.AddWithValue("@budgetholder", holder.budgetholder);
            expdata.sqlexecute.Parameters.AddWithValue("@userID", currentUser.EmployeeID);
            if (currentUser.isDelegate == true)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@delegateID", currentUser.Delegate.EmployeeID);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@delegateID", DBNull.Value);
            }
            if (holder.description.Length > 4000)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@description", holder.description.Substring(0,3999));
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@description", holder.description);
            }
            expdata.sqlexecute.Parameters.AddWithValue("@employeeid", holder.employeeid);
            expdata.sqlexecute.Parameters.AddWithValue("@date", DateTime.Now);
            expdata.sqlexecute.Parameters.Add("@identity", SqlDbType.Int);
            expdata.sqlexecute.Parameters["@identity"].Direction = ParameterDirection.ReturnValue;

            expdata.ExecuteProc("saveBudgetHolder");
            int budgetholderid = (int)expdata.sqlexecute.Parameters["@identity"].Value;

            ResetCache();

            return budgetholderid;
		}

		public int deleteBudgetHolder(int budgetholderid)
		{
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));

            CurrentUser currentUser = cMisc.GetCurrentUser();
			expdata.sqlexecute.Parameters.AddWithValue("@budgetholderid",budgetholderid);
            expdata.sqlexecute.Parameters.AddWithValue("@employeeID", currentUser.EmployeeID);
            if (currentUser.isDelegate)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@delegateID", currentUser.Delegate.EmployeeID);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@delegateID", DBNull.Value);
            }
            expdata.sqlexecute.Parameters.Add("@identity", SqlDbType.Int);
            expdata.sqlexecute.Parameters["@identity"].Direction = ParameterDirection.ReturnValue;
            expdata.ExecuteProc("deleteBudgetHolder");
            int returnvalue = (int)expdata.sqlexecute.Parameters["@identity"].Value;
			expdata.sqlexecute.Parameters.Clear();

            ResetCache();

            return returnvalue;
		}

		public cBudgetHolder getBudgetHolderById(int budgetholderid)
		{
            cBudgetHolder holder = null;
            list.TryGetValue(budgetholderid, out holder);
            return holder;
		}

        public cBudgetHolder getBudgetHolderByName(string name)
        {
            cBudgetHolder reqBudgetHolder = null;
            foreach (cBudgetHolder clsBudgetHolder in list.Values)
            {
                if (clsBudgetHolder.budgetholder == name)
                {
                    reqBudgetHolder = clsBudgetHolder;
                    break;
                }
            }
            return reqBudgetHolder;
        }

        public int getBudgetHolderidByName(string name)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            int budgetholderid = 0;
            expdata.sqlexecute.Parameters.AddWithValue("@name", name);

            strsql = "select budgetholderid from budgetholders where budgetholder = @name";
            using (System.Data.SqlClient.SqlDataReader reader = expdata.GetReader(strsql))
            {
                while (reader.Read())
                {
                    if (reader.IsDBNull(0) == false)
                    {
                        budgetholderid = reader.GetInt32(reader.GetOrdinal("budgetholderid"));
                    }
                }
                reader.Close();
            }

            expdata.sqlexecute.Parameters.Clear();

            return budgetholderid;
        }

		public List<ListItem> CreateDropDown()
		{
            List<ListItem> tempItems = new List<ListItem>();

            SortedList<string, int> sorted = sortList();
			
				
			foreach (KeyValuePair<string,int> i in sorted)
			{
                tempItems.Add(new ListItem(i.Key, i.Value.ToString()));
				
			}
			return tempItems;
		}

        private SortedList<string, int> sortList()
        {
            SortedList<string, int> tmp = new SortedList<string, int>();
            foreach (cBudgetHolder holder in list.Values)
            {
                tmp.Add(holder.budgetholder, holder.budgetholderid);
            }
            return tmp;
        }

        private void ResetCache()
        {
            this.Cache.Remove("budgetholders" + nAccountid);
            list = null;
        }
	}

}
