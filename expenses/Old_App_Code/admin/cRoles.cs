using System;
using System.Collections;
using expenses.Old_App_Code;
using System.Web.Caching;
using ExpensesLibrary;
using System.Collections.Generic;
using SpendManagementLibrary;

namespace expenses
{
	/// <summary>
	/// Summary description for roles.
	/// </summary>
    /// 
    
	public class cRoles
    {
        #region moved to new stuffz
        /*
        string strsql;
		int nAccountid = 0;
		
		System.Collections.SortedList list;
		System.Web.Caching.Cache Cache = System.Web.HttpRuntime.Cache;

		string auditcat = "Roles";

		public int accountid
		{
			get {return nAccountid;}
		}

		public cRoles(int accountid)
		{
			nAccountid = accountid;
			
			InitialiseData();
		}

		private void InitialiseData()
		{
            list = (System.Collections.SortedList)Cache["roles" + accountid];
			if (list == null)
			{
				list = CacheList();
			}
			
		}

		System.Collections.SortedList CacheList()
		{
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
			System.Data.SqlClient.SqlDataReader reader;
			System.Collections.SortedList list = new System.Collections.SortedList();
			int roleid;
			string rolename, description;
			int masterrole;
			bool employeeadmin, setup, employeeaccounts, reports, qedesign;
			bool checkandpay, creditcard,  approvals, exports;
			byte accesstype;
			bool costcodesreadonly, departmentsreadonly, projectcodesreadonly;
			bool reportsreadonly, auditlog, purchasecards;
			decimal minclaim, maxclaim;
			cRole reqrole;
            DateTime createdon, modifiedon;
            int createdby, modifiedby;

			strsql = "select roleid, rolename, description, masterrole, setup, employeeadmin, employeeaccounts, reports, checkandpay, qedesign, accesstype, creditcard, approvals, exports, costcodesreadonly, departmentsreadonly, projectcodesreadonly, minclaim, maxclaim, reportsreadonly, auditlog, CreatedOn, CreatedBy, ModifiedOn, ModifiedBy from dbo.roles";
			
            expdata.sqlexecute.CommandText = strsql;
            SqlCacheDependency dep = new SqlCacheDependency(expdata.sqlexecute);
			reader = expdata.GetReader(strsql);
			expdata.sqlexecute.Parameters.Clear();
			while (reader.Read())
			{
				roleid = reader.GetInt32(reader.GetOrdinal("roleid"));
				rolename = reader.GetString(reader.GetOrdinal("rolename"));
				if (reader.IsDBNull(reader.GetOrdinal("description")) == false)
				{
					description = reader.GetString(reader.GetOrdinal("description"));
				}
				else
				{
					description = "";
				}
				masterrole = reader.GetInt32(reader.GetOrdinal("masterrole"));
				employeeadmin = reader.GetBoolean(reader.GetOrdinal("employeeadmin"));
				setup = reader.GetBoolean(reader.GetOrdinal("setup"));
				employeeaccounts = reader.GetBoolean(reader.GetOrdinal("employeeaccounts"));
				reports = reader.GetBoolean(reader.GetOrdinal("reports"));
				qedesign = reader.GetBoolean(reader.GetOrdinal("qedesign"));
				creditcard = reader.GetBoolean(reader.GetOrdinal("creditcard"));
				checkandpay = reader.GetBoolean(reader.GetOrdinal("checkandpay"));
				approvals = reader.GetBoolean(reader.GetOrdinal("approvals"));
				exports = reader.GetBoolean(reader.GetOrdinal("exports"));
				accesstype = reader.GetByte(reader.GetOrdinal("accesstype"));
				costcodesreadonly = reader.GetBoolean(reader.GetOrdinal("costcodesreadonly"));
				departmentsreadonly = reader.GetBoolean(reader.GetOrdinal("departmentsreadonly"));
				projectcodesreadonly = reader.GetBoolean(reader.GetOrdinal("projectcodesreadonly"));
				minclaim = reader.GetDecimal(reader.GetOrdinal("minclaim"));
				maxclaim = reader.GetDecimal(reader.GetOrdinal("maxclaim"));
				reportsreadonly = reader.GetBoolean(reader.GetOrdinal("reportsreadonly"));
                auditlog = reader.GetBoolean(reader.GetOrdinal("auditlog"));
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
                reqrole = new cRole(roleid, rolename, description, masterrole, employeeadmin, setup, employeeaccounts, reports, checkandpay, qedesign, accesstype, creditcard, approvals, exports, costcodesreadonly, projectcodesreadonly, departmentsreadonly, minclaim, maxclaim, reportsreadonly, auditlog, createdon, createdby, modifiedon, modifiedby, getRoleAccess(roleid), cAccounts.getConnectionString(accountid));
				list.Add(roleid, reqrole);
			}
			reader.Close();
			

			Cache.Insert("roles" + accountid,list,dep, System.Web.Caching.Cache.NoAbsoluteExpiration, System.Web.Caching.Cache.NoSlidingExpiration, System.Web.Caching.CacheItemPriority.NotRemovable, null);
			return list;
			
		}

		

		private void CreateDependency()
		{
			if (Cache["rolesdependency" + accountid] == null)
			{
				Cache.Insert("rolesdependency" + accountid,1);
				
			}
		}
		System.Web.Caching.CacheDependency getDependency()
		{
			System.Web.Caching.CacheDependency dep;
			String[] dependency;
			dependency = new string[1];
			dependency[0] = "rolesdependency" + accountid;
			dep = new System.Web.Caching.CacheDependency(null,dependency);
			return dep;
		}

		private void InvalidateCache()
		{
			Cache.Remove("rolesdependency" + accountid);
			CreateDependency();
        }



        public System.Data.DataSet getGrid()
		{
			System.Collections.SortedList sorted = sortRoles();
			cRole reqrole;
			int i;
			object[] values;
			System.Data.DataSet ds = new System.Data.DataSet();
			System.Data.DataTable tbl = new System.Data.DataTable();
			tbl.Columns.Add("roleid",System.Type.GetType("System.Int32"));
			tbl.Columns.Add("rolename",System.Type.GetType("System.String"));
			tbl.Columns.Add("description",System.Type.GetType("System.String"));
			
			for (i = 0; i < sorted.Count; i++)
			{
				reqrole = (cRole)sorted.GetByIndex(i);
				values = new object[3];
				values[0] = reqrole.roleid;
				values[1] = reqrole.rolename;
				values[2] = reqrole.description;
				tbl.Rows.Add(values);
			}
			ds.Tables.Add(tbl);
			return ds;
		}

		private System.Collections.SortedList sortRoles()
		{

			System.Collections.SortedList sorted = new System.Collections.SortedList();
			int i;
			cRole reqrole;

            for (i = 0; i < list.Count; i++)
            {
                reqrole = (cRole)list.GetByIndex(i);

                sorted.Add(reqrole.rolename, reqrole);

            }
			return sorted;
        }
        
        public cRole getRoleById(int roleid)
		{
			return (cRole)list[roleid];
        }
         
         */
        #endregion


        public cRole getRoleByName(string name)
        {
            cRole reqRole = null;
            foreach (cRole clsRole in list.Values)
            {
                if (clsRole.rolename == name)
                {
                    reqRole = clsRole;
                    break;
                }
            }
            return reqRole;
        }


		public System.Web.UI.WebControls.ListItem[] CreateMasterRolesDropDown(int roleid)
		{
			int i = 0;
			System.Collections.ArrayList masterroles = new System.Collections.ArrayList();
			cRole reqrole;
			for (i = 0; i < list.Count; i++)
			{
				reqrole = (cRole)list.GetByIndex(i);
				if (reqrole.masterrole == 0)
				{
					masterroles.Add(reqrole);
				}
			}
			
			
			
			
			
			System.Web.UI.WebControls.ListItem[] tempitems = new System.Web.UI.WebControls.ListItem[masterroles.Count+1];

			
			tempitems[0] = new System.Web.UI.WebControls.ListItem();
			tempitems[0].Value = "0";
			tempitems[0].Text = "";
			for (i = 0; i < masterroles.Count; i++)
			{
				reqrole = (cRole)masterroles[i];
			
				tempitems[i+1] = new System.Web.UI.WebControls.ListItem();
				tempitems[i+1].Value = reqrole.roleid.ToString();
				tempitems[i+1].Text = reqrole.rolename;
				if (roleid == reqrole.roleid)
				{
					tempitems[i+1].Selected = true;
				}

			}
			
			return tempitems;
		}
        */
		public System.Data.DataSet getRoles(int roleid)
		{
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
			System.Data.DataSet rcdstroleaccess = new System.Data.DataSet();
			
			expdata.sqlexecute.Parameters.AddWithValue("@roleid",roleid);
			strsql = "select rolename, roleid from roles where roleid <> @roleid order by rolename";
			rcdstroleaccess = expdata.GetDataSet(strsql);

			
			expdata.sqlexecute.Parameters.Clear();							
			
			return rcdstroleaccess;
		}

        /*
		public byte addRole (string rolename, string description, int masterrole, bool setup, bool employeeadmin, bool employeeaccounts, bool reports, bool checkandpay, bool qedesign, int accesstype, bool creditcard, sRoleAccess[] roleaccess, bool approvals, bool exports, bool costcodesreadonly, bool departmentsreadonly, bool projectcodesreadonly, decimal minclaim, decimal maxclaim, bool reportsreadonly, bool auditlog, int employeeid)
		{
			int roleid = 0;
			int i = 0;

			if (alreadyExists(0,0,rolename) == true)
			{
				return 1;
			}

            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));

            expdata.sqlexecute.Parameters.AddWithValue("@rolename", rolename);
            expdata.sqlexecute.Parameters.AddWithValue("@description", description);
            expdata.sqlexecute.Parameters.AddWithValue("@masterrole", masterrole);
            expdata.sqlexecute.Parameters.AddWithValue("@setup", Convert.ToByte(setup));
            expdata.sqlexecute.Parameters.AddWithValue("@employeeadmin", Convert.ToByte(employeeadmin));
            expdata.sqlexecute.Parameters.AddWithValue("@employeeaccounts", Convert.ToByte(employeeaccounts));
            expdata.sqlexecute.Parameters.AddWithValue("@reports", Convert.ToByte(reports));
            expdata.sqlexecute.Parameters.AddWithValue("@checkandpay", Convert.ToByte(checkandpay));
            expdata.sqlexecute.Parameters.AddWithValue("@qedesign", Convert.ToByte(qedesign));
            expdata.sqlexecute.Parameters.AddWithValue("@accesstype", accesstype);
            expdata.sqlexecute.Parameters.AddWithValue("@creditcard", Convert.ToByte(creditcard));
            expdata.sqlexecute.Parameters.AddWithValue("@approvals", Convert.ToByte(approvals));
            expdata.sqlexecute.Parameters.AddWithValue("@exports", Convert.ToByte(exports));
            
            expdata.sqlexecute.Parameters.AddWithValue("@costcodesreadonly", Convert.ToByte(costcodesreadonly));
            expdata.sqlexecute.Parameters.AddWithValue("@projectcodesreadonly", Convert.ToByte(projectcodesreadonly));
            expdata.sqlexecute.Parameters.AddWithValue("@departmentsreadonly", Convert.ToByte(departmentsreadonly));
            expdata.sqlexecute.Parameters.AddWithValue("@reportsreadonly", Convert.ToByte(reportsreadonly));
            expdata.sqlexecute.Parameters.AddWithValue("@minclaim", minclaim);
            expdata.sqlexecute.Parameters.AddWithValue("@maxclaim", maxclaim);
            expdata.sqlexecute.Parameters.AddWithValue("@auditlog", Convert.ToByte(auditlog));
            expdata.sqlexecute.Parameters.AddWithValue("@createdon", DateTime.Now.ToUniversalTime());
            expdata.sqlexecute.Parameters.AddWithValue("@createdby", employeeid);
			strsql = "insert into roles (rolename, description, masterrole, setup, employeeadmin, employeeaccounts, reports, checkandpay, qedesign, accesstype, creditcard, approvals, exports, costcodesreadonly, departmentsreadonly, projectcodesreadonly, minclaim, maxclaim, reportsreadonly, auditlog, createdon, createdby) " +
				"values (@rolename,@description,@masterrole,@setup,@employeeadmin,@employeeaccounts,@reports,@checkandpay,@qedesign,@accesstype,@creditcard,@approvals, @exports, @costcodesreadonly, @departmentsreadonly, @projectcodesreadonly, @minclaim, @maxclaim, @reportsreadonly, @auditlog, @createdon, @createdby)";
			expdata.ExecuteSQL(strsql);

			strsql = "select roleid from roles where rolename = @rolename";
			roleid = expdata.getcount(strsql);

			
			for (i = 0; i < roleaccess.Length; i++)
			{
				addRoleAccess(roleaccess[i],roleid);
			}

			expdata.sqlexecute.Parameters.Clear();

			cAuditLog clsaudit = new cAuditLog(accountid, employeeid);
			clsaudit.addRecord(auditcat,rolename);
			InvalidateCache();
			InitialiseData();
			return 0;
		}
        */

        /*
		public bool alreadyExists(int action, int roleid, string rolename)
		{
			int count = 0;
			if (action == 2)
			{
				strsql = "select count(*) from roles where rolename = @rolename and roleid <> @roleid";
			}
			else
			{
				strsql = "select count(*) from roles where rolename = @rolename";
			}

            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
			expdata.sqlexecute.Parameters.AddWithValue("@rolename",rolename);
			expdata.sqlexecute.Parameters.AddWithValue("@roleid",roleid);
			
			count = expdata.getcount(strsql);
			expdata.sqlexecute.Parameters.Clear();

			if (count == 0)
			{
				return false;
			}
			else
			{
				return true;
			}
		}
        */

        /*
		public byte updateRole (int roleid, string rolename, string description, int masterrole, bool setup, bool employeeadmin, bool employeeaccounts, bool reports, bool checkandpay, bool qedesign, int accesstype, bool creditcard, sRoleAccess[] roleaccess, bool approvals, bool exports, bool costcodesreadonly, bool departmentsreadonly, bool projectcodesreadonly, decimal minclaim, decimal maxclaim, bool reportsreadonly, bool auditlog, int employeeid)
		{
			if (alreadyExists(2,roleid,rolename) == true)
			{
				return 1;
			}

			cRole reqrole = getRoleById(roleid);
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            expdata.sqlexecute.Parameters.AddWithValue("@rolename", rolename);
            expdata.sqlexecute.Parameters.AddWithValue("@description", description);
            expdata.sqlexecute.Parameters.AddWithValue("@masterrole", masterrole);
            expdata.sqlexecute.Parameters.AddWithValue("@setup", Convert.ToByte(setup));
            expdata.sqlexecute.Parameters.AddWithValue("@employeeadmin", Convert.ToByte(employeeadmin));
            expdata.sqlexecute.Parameters.AddWithValue("@employeeaccounts", Convert.ToByte(employeeaccounts));
            expdata.sqlexecute.Parameters.AddWithValue("@reports", Convert.ToByte(reports));
            expdata.sqlexecute.Parameters.AddWithValue("@checkandpay", Convert.ToByte(checkandpay));
            expdata.sqlexecute.Parameters.AddWithValue("@qedesign", Convert.ToByte(qedesign));
            expdata.sqlexecute.Parameters.AddWithValue("@accesstype", accesstype);
            expdata.sqlexecute.Parameters.AddWithValue("@creditcard", Convert.ToByte(creditcard));
            expdata.sqlexecute.Parameters.AddWithValue("@approvals", Convert.ToByte(approvals));
            expdata.sqlexecute.Parameters.AddWithValue("@exports", Convert.ToByte(exports));
            expdata.sqlexecute.Parameters.AddWithValue("@roleid", roleid);
            expdata.sqlexecute.Parameters.AddWithValue("@costcodesreadonly", Convert.ToByte(costcodesreadonly));
            expdata.sqlexecute.Parameters.AddWithValue("@projectcodesreadonly", Convert.ToByte(projectcodesreadonly));
            expdata.sqlexecute.Parameters.AddWithValue("@departmentsreadonly", Convert.ToByte(departmentsreadonly));
            expdata.sqlexecute.Parameters.AddWithValue("@minclaim", minclaim);
            expdata.sqlexecute.Parameters.AddWithValue("@maxclaim", maxclaim);
            expdata.sqlexecute.Parameters.AddWithValue("@reportsreadonly", Convert.ToByte(reportsreadonly));
            expdata.sqlexecute.Parameters.AddWithValue("@auditlog", Convert.ToByte(auditlog));
            expdata.sqlexecute.Parameters.AddWithValue("@modifiedon", DateTime.Now.ToUniversalTime());
            expdata.sqlexecute.Parameters.AddWithValue("@modifiedby", employeeid);
			int i = 0;
			strsql = "update roles set rolename = @rolename, description = @description, masterrole = @masterrole, setup = @setup, employeeadmin = @employeeadmin, employeeaccounts = @employeeaccounts, reports = @reports, checkandpay = @checkandpay, qedesign = @qedesign, accesstype = @accesstype, creditcard = @creditcard, approvals = @approvals, exports = @exports, costcodesreadonly = @costcodesreadonly, departmentsreadonly = @departmentsreadonly, projectcodesreadonly = @projectcodesreadonly, minclaim = @minclaim, maxclaim = @maxclaim, reportsreadonly = @reportsreadonly, auditlog = @auditlog, modifiedon = @modifiedon, modifiedby = @modifiedby where roleid = @roleid";
			expdata.ExecuteSQL(strsql);
			expdata.sqlexecute.Parameters.Clear();

			
			
			deleteRoleAccess(roleid);
			
			
			for (i = 0; i < roleaccess.Length; i++)
			{
				addRoleAccess(roleaccess[i],roleid);
			}
			
			#region Audit Log
				cAuditLog clsaudit = new cAuditLog(accountid, employeeid);
			if (reqrole.rolename != rolename)
			{
				clsaudit.editRecord(rolename,"Role Name",auditcat,reqrole.rolename, rolename);
			}
			if (reqrole.description != description)
			{
				clsaudit.editRecord(rolename,"Description",auditcat,reqrole.description, description);
			}
			#endregion
			InvalidateCache();
			InitialiseData();
			return 0;
		}
        */

        /*
		public bool deleteRole(int roleid)
		{
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
			cRole reqrole = getRoleById(roleid);
			int count = 0;
			expdata.sqlexecute.Parameters.AddWithValue("@roleid",roleid);
			strsql = "select count(*) from employees where roleid = @roleid";
			count = expdata.getcount(strsql);
			if (count > 0)
			{
				expdata.sqlexecute.Parameters.Clear();
				return false;
			}
			strsql = "delete from roles where roleid = @roleid";
			expdata.ExecuteSQL(strsql);
			expdata.sqlexecute.Parameters.Clear();

			cAuditLog clsaudit = new cAuditLog();
			clsaudit.deleteRecord(auditcat,reqrole.rolename);
			InvalidateCache();
			InitialiseData();
			return true;
		}
        */

        /*
		private void addRoleAccess(sRoleAccess roleaccess, int roleid)
		{
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
			expdata.sqlexecute.Parameters.AddWithValue("@roleid",roleid);
			expdata.sqlexecute.Parameters.AddWithValue("@accessid",roleaccess.accessid);
			strsql = "insert into roleaccess (roleid, accessid) values (@roleid,@accessid)";
			expdata.ExecuteSQL(strsql);
			expdata.sqlexecute.Parameters.Clear();
		}	
         */

        /*
		private void deleteRoleAccess(int roleid)
		{
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
			expdata.sqlexecute.Parameters.AddWithValue("@roleid",roleid);
			strsql = "delete from roleaccess where roleid = @roleid";
			expdata.ExecuteSQL(strsql);
			expdata.sqlexecute.Parameters.Clear();
		}
        */

        /*
		public void tickRoles(ref System.Web.UI.WebControls.CheckBoxList templist, int roleid)
		{
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
			System.Data.DataSet rcdstroleaccess = new System.Data.DataSet();
			expdata.sqlexecute.Parameters.AddWithValue("@roleid",roleid);
			strsql = "select * from roleaccess where roleid = @roleid";
			rcdstroleaccess = expdata.GetDataSet(strsql);
			expdata.sqlexecute.Parameters.Clear();
			foreach(System.Data.DataRow row in rcdstroleaccess.Tables[0].Rows)
			{
				templist.Items.FindByValue(row["accessid"].ToString()).Selected = true;
			}
		}
         */

        /*
		public System.Web.UI.WebControls.ListItem[] CreateDropDown(int roleid, bool blank)
		{
			int count = list.Count;
			cRole reqrole;
			System.Collections.SortedList sorted = sortRoles();
			if (blank == true)
			{
				count++;	
			}
			System.Web.UI.WebControls.ListItem[] tempItems = new System.Web.UI.WebControls.ListItem[count];
			
			int i = 0;
			
			count = 0;
			if (blank == true)
			{
				tempItems[count] = new System.Web.UI.WebControls.ListItem("","0");
				count++;
			}
			
			for (i = 0; i < sorted.Count; i++)
			{
				reqrole = (cRole)sorted.GetByIndex(i);
				tempItems[count] = new System.Web.UI.WebControls.ListItem();
				tempItems[count].Text = reqrole.rolename;
				tempItems[count].Value = reqrole.roleid.ToString();
				if (roleid == reqrole.roleid)
				{
					tempItems[count].Selected = true;
				}
				count++;
			}
			return tempItems;
		}
		
         */ 

        /*
        public SortedList rolelist
        {
            get { return list; }
        }
         */ 

        /*
        public ArrayList filterCheckandPayRoles()
        {
            SortedList sorted = sortRoles();
            ArrayList roles = new ArrayList();
            cRole reqrole;
            for (int i = 0; i < sorted.Count; i++)
            {
                reqrole = (cRole)sorted.GetByIndex(i);
                if (reqrole.checkandpay)
                {
                    roles.Add(reqrole);
                }
            }

            return roles;
        }
        */ 


        /*
        public Dictionary<int, cRole> getModifiedRoles(DateTime date)
        {
            Dictionary<int, cRole> lst = new Dictionary<int, cRole>();
            foreach (cRole val in list.Values)
            {
                if (val.createdon > date || val.modifiedon > date)
                {
                    lst.Add(val.roleid, val);
                }
            }
            return lst;
        }
        */

        /*
        public List<int> getRoleIds()
        {
            List<int> ids = new List<int>();
            foreach (cRole val in list.Values)
            {
                ids.Add(val.roleid);
            }
            return ids;
        }
         */
 
        /*
        private int[] getRoleAccess(int roleid)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            string strsql;
            int count;
            int[] access;
            System.Data.SqlClient.SqlDataReader reader;
            strsql = "select count(*) from roleaccess where roleid = " + roleid;
            count = expdata.getcount(strsql);

            access = new int[count];
            if (count != 0)
            {
                count = 0;
                strsql = "select accessid from roleaccess where roleid = " + roleid;
                reader = expdata.GetReader(strsql);
                while (reader.Read())
                {
                    access[count] = reader.GetInt32(0);
                    count++;
                }
                reader.Close();
                
            }

            return access;

        }
         
    }

	public struct sRoleAccess
	{
		public int roleid;
		public int accessid;
	}

	

    
}

