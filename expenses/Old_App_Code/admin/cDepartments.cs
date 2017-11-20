using System;
using System.Collections.Generic;
using System.Collections;
using ExpensesLibrary;
using expenses.Old_App_Code;
using System.Web.Caching;
using SpendManagementLibrary;
namespace expenses
{
	/// <summary>
	/// Summary description for departments.
	/// [departmentslist + accountid] = sorted list of departments
	/// [departmentsdd + accountid = string of departments dropdown;
	/// [departmentsvlist + accountid] = value list of departments;
	/// [departmentsddown + accountid] = System.web.ui.webcontrols.listitem[]
	/// </summary>
	public class cDepartments
	{

        SortedList<int, cDepartment> list;
        DBConnection expdata;
        string strsql;
		string auditcat = "Departments";
        public System.Web.Caching.Cache Cache = System.Web.HttpRuntime.Cache;
        private int nAccountid;
		public cDepartments(int accountid)
		{
            nAccountid = accountid;
            expdata = new DBConnection(cAccounts.getConnectionString(accountid));
			InitialiseData();
        }

        #region properties
        public int accountid
        {
            get { return nAccountid; }
        }
        #endregion
        public SortedList<int,cDepartment> CacheList()
		{
            cNewUserDefined clsuserdefined = new cNewUserDefined(accountid, cAccounts.getConnectionString(accountid), new cTables(accountid)); ;
            Dictionary<int, object> userdefined;

			int departmentid = 0;
			string department = "";
			string description = "";
			bool archived;
            DateTime createdon, modifiedon;
            int createdby, modifiedby;

			cDepartment curdepartment;
			System.Data.SqlClient.SqlDataReader reader;
			SortedList<int,cDepartment> list = new SortedList<int,cDepartment>();
			

            strsql = "SELECT     departmentid, department, description, archived, CreatedOn, CreatedBy, ModifiedOn, ModifiedBy FROM dbo.departments";
            expdata.sqlexecute.CommandText = strsql;
            SqlCacheDependency dep = new SqlCacheDependency(expdata.sqlexecute);
            
            reader = expdata.GetReader(strsql);
            expdata.sqlexecute.Parameters.Clear();
			while (reader.Read())
			{
				departmentid = reader.GetInt32(reader.GetOrdinal("departmentid"));
				department = reader.GetString(reader.GetOrdinal("department"));
				if (reader.IsDBNull(reader.GetOrdinal("description")) == false)
				{
					description = reader.GetString(reader.GetOrdinal("description"));
				}
				else
				{
					description = "";
				}
				archived = reader.GetBoolean(reader.GetOrdinal("archived"));
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
                userdefined = clsuserdefined.getValues(AppliesTo.Department, departmentid);
                curdepartment = new cDepartment(departmentid, department, description, archived, createdon, createdby, modifiedon, modifiedby, userdefined);
				list.Add(departmentid,curdepartment);		
			}
			reader.Close();
			
			
			Cache.Insert("departmentslist" + accountid, list, dep, System.Web.Caching.Cache.NoAbsoluteExpiration, System.Web.Caching.Cache.NoSlidingExpiration, System.Web.Caching.CacheItemPriority.NotRemovable, null);

			return list;
		}

		public void InitialiseData()
		{

			list = (SortedList<int,cDepartment>)Cache["departmentslist" + accountid];
			if (list == null)
			{
				list = CacheList();
			}
			
		}

		

        
		public System.Data.DataTable getGrid(byte filter)
		{
			System.Data.DataTable departments = new System.Data.DataTable();
			object[] values;
			bool display;
			departments.Columns.Add("departmentid",System.Type.GetType("System.Int32"));
			departments.Columns.Add("department",System.Type.GetType("System.String"));
			departments.Columns.Add("description",System.Type.GetType("System.String"));
			departments.Columns.Add("archived",System.Type.GetType("System.Boolean"));

			
            foreach (cDepartment clsdepartment in list.Values)
			{
				

				display = false;
				switch (filter)
				{
					case 1:
						display = true;
						break;
					case 2:
						if (clsdepartment.archived == true)
						{
							display = true;
						}
						break;
					case 3:
						if (clsdepartment.archived == false)
						{
							display = true;
						}
						break;
				}
				if (display == true)
				{
					values = new object[4];
					values[0] = clsdepartment.departmentid;
					values[1] = clsdepartment.department;
					values[2] = clsdepartment.description;
					values[3] = clsdepartment.archived;
					departments.Rows.Add(values);
				}
				
			}

			return departments;
		}

        public int addDepartment(string department, string description, int userid, Dictionary<int, object> userdefinedfields)
		{
			
			if (alreadyExists(department,false,0) == true)
			{
				return 1;
			}
            
            
            expdata.sqlexecute.Parameters.AddWithValue("@department", department);
            expdata.sqlexecute.Parameters.AddWithValue("@description", description);
            expdata.sqlexecute.Parameters.AddWithValue("@createdon", DateTime.Now.ToUniversalTime());
            expdata.sqlexecute.Parameters.AddWithValue("@createdby", userid);
            expdata.sqlexecute.Parameters.Add("@returnvalue", System.Data.SqlDbType.Int);
            expdata.sqlexecute.Parameters["@returnvalue"].Direction = System.Data.ParameterDirection.ReturnValue;
            expdata.ExecuteProc("addDepartment");
            int departmentid = (int)expdata.sqlexecute.Parameters["@returnvalue"].Value;
            expdata.sqlexecute.Parameters.Clear();

            cNewUserDefined clsuserdefined = new cNewUserDefined(accountid, cAccounts.getConnectionString(accountid), new cTables(accountid)); ;
            clsuserdefined.addValues(AppliesTo.Department, departmentid, userdefinedfields);

            list.Add(departmentid, new cDepartment(departmentid, department, description, false,DateTime.Now.ToUniversalTime(),userid, new DateTime(1900,01,01), 0, userdefinedfields));
			cAuditLog clsaudit = new cAuditLog(accountid, userid);
			clsaudit.addRecord(auditcat,department);
			return 0;
		}

		public  bool alreadyExists(string department, bool update, int id)
		{
			bool exists = false;
			
            foreach (cDepartment clsdepartment in list.Values)
			{
				
				if (clsdepartment.department.ToLower().Trim() == department.ToLower().Trim())
				{
					if (update == false || (update == true && clsdepartment.departmentid != id))
					{
						exists = true;
					}
					break;
				}
			}

			return exists;

		}

        public int updateDepartment(int departmentid, string department, string description, int userid, Dictionary<int, object> userdefinedfields)
		{
			if (alreadyExists(department,true,departmentid) == true)
			{
				return 1;
			}
            //System.Web.HttpApplication appinfo = (System.Web.HttpApplication)System.Web.HttpContext.Current.ApplicationInstance;
			cDepartment reqdep = GetDepartmentById(departmentid);

            

            expdata.sqlexecute.Parameters.AddWithValue("@departmentid", departmentid);
            expdata.sqlexecute.Parameters.AddWithValue("@department", department);
            expdata.sqlexecute.Parameters.AddWithValue("@description", description);
            expdata.sqlexecute.Parameters.AddWithValue("@modifiedon", DateTime.Now.ToUniversalTime());
            expdata.sqlexecute.Parameters.AddWithValue("@modifiedby", userid);
            expdata.ExecuteProc("updateDepartment");
            expdata.sqlexecute.Parameters.Clear();

            cNewUserDefined clsuserdefined = new cNewUserDefined(accountid, cAccounts.getConnectionString(accountid), new cTables(accountid)); ;
            clsuserdefined.addValues(AppliesTo.Department, departmentid, userdefinedfields);

			cAuditLog clsaudit = new cAuditLog(accountid, userid);
			if (reqdep.department != department)
			{
				clsaudit.editRecord(department,"Department",auditcat,reqdep.department,department);
			}
			if (reqdep.description != description)
			{
				clsaudit.editRecord(department,"Department",auditcat,reqdep.description,description);
			}

            if (list.ContainsKey(departmentid))
            {
                cDepartment dep = GetDepartmentById(departmentid);
                list[departmentid] = new cDepartment(departmentid, department, description, dep.archived, reqdep.createdon, reqdep.createdby, DateTime.Now.ToUniversalTime(),userid, userdefinedfields);
            }
			return 0;
		}

		public void deleteDepartment(int departmentid)
		{
			cDepartment reqdep = GetDepartmentById(departmentid);
			expdata.sqlexecute.Parameters.AddWithValue("@departmentid",departmentid);
            expdata.ExecuteProc("deleteDepartment");
            expdata.sqlexecute.Parameters.Clear();

            list.Remove(departmentid);
			cAuditLog clsaudit = new cAuditLog();
			clsaudit.deleteRecord(auditcat,reqdep.department);
			
			
		}

		public void changeStatus(int departmentid, bool archive)
		{
			if (archive == true)
			{
				strsql = "update departments set archived = 1 where departmentid = @departmentid";
			}
			else
			{
				strsql = "update departments set archived = 0 where departmentid = @departmentid";
			}
			expdata.sqlexecute.Parameters.AddWithValue("@departmentid",departmentid);
			expdata.ExecuteSQL(strsql);
			expdata.sqlexecute.Parameters.Clear();

            cDepartment dep = GetDepartmentById(departmentid);
            dep.archived = !dep.archived;
			
		}
		public cDepartment GetDepartmentById(int departmentid)
		{
			return (cDepartment)list[departmentid];
		}

        public cDepartment getDepartmentByString(string department)
        {
            cDepartment reqdep = null;
            foreach (cDepartment clsdep in list.Values)
            {
                if (clsdep.department == department)
                {
                    reqdep = clsdep;
                    break;
                }
            }
            return reqdep;
        }

        public cDepartment getDepartmentByDesc(string desc)
        {
            cDepartment reqdep = null;
            foreach (cDepartment clsdep in list.Values)
            {
                if (clsdep.description == desc)
                {
                    reqdep = clsdep;
                    break;
                }
            }
            return reqdep;
        }

		public cColumnList getColumnList()
		{
			bool usedesc;
			System.Web.HttpApplication appinfo = (System.Web.HttpApplication)System.Web.HttpContext.Current.ApplicationInstance;
            cMisc clsmisc = new cMisc(accountid);
            cGlobalProperties clsproperties = clsmisc.getGlobalProperties(accountid);
            usedesc = clsproperties.usedepartmentdesc;
			cColumnList collist = new cColumnList();
			
			int i;
			collist.addItem(0,"");
			foreach (cDepartment reqdepartment in list.Values)
			{
				
				if (usedesc == true)
				{
					collist.addItem(reqdepartment.departmentid,reqdepartment.description);
				}
				else
				{
					collist.addItem(reqdepartment.departmentid,reqdepartment.department);
				}
			}
			return collist;
		}
		public string CreateStringDropDown(int departmentid, bool readOnly, bool blank)
		{
			bool usedesc;
			SortedList<string,cDepartment> sortedlst = new SortedList<string,cDepartment>();
			System.Web.HttpApplication appinfo = (System.Web.HttpApplication)System.Web.HttpContext.Current.ApplicationInstance;
			
			int i;
			System.Text.StringBuilder output = new System.Text.StringBuilder();

            cMisc clsmisc = new cMisc(accountid);
            cGlobalProperties clsproperties = clsmisc.getGlobalProperties(accountid);

            usedesc = clsproperties.usedepartmentdesc;
			output.Append("<select name=\"department\" id=\"department\"");
			if (readOnly == true)
			{
				output.Append(" disabled");
			}
			output.Append(">");
			if (blank == true)
			{
				output.Append("<option value=\"0\"");
				if (departmentid == 0)
				{
					output.Append(" selected");
				}
				output.Append("></option>");
			}

			foreach (cDepartment reqdepartment in list.Values)
			{
				
				if (reqdepartment.archived == false)
				{
					if (usedesc == true)
					{
						sortedlst.Add(reqdepartment.description, reqdepartment);
					}
					else
					{
						sortedlst.Add(reqdepartment.department, reqdepartment);
					}
				}
			}

			foreach (cDepartment reqdepartment in sortedlst.Values)
			{
				
				output.Append("<option value=\"" + reqdepartment.departmentid + "\"");
				if (reqdepartment.departmentid == departmentid)
				{
					output.Append(" selected");
				}
				output.Append(">");

				if (usedesc == false)
				{
					output.Append(reqdepartment.department);
				}
				else
				{
					output.Append(reqdepartment.description);
				}
				output.Append("</option>");
			}
			output.Append("</select>");

			return output.ToString();
		}

        public SortedList<string, cDepartment> sortList()
        {
            SortedList<string, cDepartment> sorted = new SortedList<string, cDepartment>();
            
            foreach (cDepartment dep in list.Values)
            {
            
                sorted.Add(dep.department, dep);
            }
            return sorted;
        }
        private SortedList<string, cDepartment> sortListByDescription()
        {
            SortedList<string, cDepartment> sorted = new SortedList<string, cDepartment>();
            
            foreach (cDepartment dep in list.Values)
            {
                
                sorted.Add(dep.description, dep);
            }
            return sorted;
        }
        
        public List<System.Web.UI.WebControls.ListItem> CreateDropDown(bool usedesc)
		{
            List<System.Web.UI.WebControls.ListItem> items = new List<System.Web.UI.WebControls.ListItem>();
            

            SortedList<string,cDepartment> sorted;
            if (usedesc)
            {
                sorted = sortListByDescription();
            }
            else
            {
                sorted = sortList();
            }

            foreach (cDepartment reqdep in sorted.Values)
            {
                
                if (!reqdep.archived)
                {
                    if (usedesc)
                    {
                        items.Add(new System.Web.UI.WebControls.ListItem(reqdep.description, reqdep.departmentid.ToString()));
                    }
                    else
                    {
                        items.Add(new System.Web.UI.WebControls.ListItem(reqdep.department, reqdep.departmentid.ToString()));
                    }
                }
            }

            return items;			
			
			
		}

		private Infragistics.WebUI.UltraWebGrid.ValueList CacheVList()
		{
			System.Web.HttpApplication appinfo = (System.Web.HttpApplication)System.Web.HttpContext.Current.ApplicationInstance;
			System.Web.Caching.CacheDependency dep;
			String[] dependency;


			dependency = new string[1];
			dependency[0] = "departmentsdependency" + accountid;
			dep = new System.Web.Caching.CacheDependency(null,dependency);

			int i = 0;
			int departmentid = 0;
			string department = "";

			bool usedepartmentdesc = false;
			
			Infragistics.WebUI.UltraWebGrid.ValueList temp = new Infragistics.WebUI.UltraWebGrid.ValueList();
			
			System.Collections.SortedList sortedlst = new System.Collections.SortedList();
            cMisc clsmisc = new cMisc(accountid);
            cGlobalProperties clsproperties = clsmisc.getGlobalProperties(accountid);

            usedepartmentdesc = clsproperties.usedepartmentdesc;
			foreach (cDepartment reqdepartment in list.Values)
			{
				
				if (usedepartmentdesc == true)
				{
					department = reqdepartment.description;
				}
				else
				{
					department = reqdepartment.department;
				}
				sortedlst.Add(department, reqdepartment.departmentid);
			}

			
			foreach (cDepartment reqdepartment in sortedlst.Values)
			{
                departmentid = reqdepartment.departmentid;
                department = reqdepartment.department;
				temp.ValueListItems.Add (departmentid,department);
				

			}
			Cache.Insert("departmentsvlist" + accountid,temp,dep);

			return temp;
		}
		
        public int count
        {
            get { return list.Count; }
        }

        public ArrayList getModifiedDepartments(DateTime date)
        {
            ArrayList lst = new ArrayList();
            foreach (cDepartment val in list.Values)
            {
                if (val.createdon > date || val.modifiedon > date)
                {
                    lst.Add(val);
                }
            }
            return lst;
        }

        public ArrayList getDepartmentIds()
        {
            ArrayList ids = new ArrayList();
            foreach (cDepartment val in list.Values)
            {
                ids.Add(val.departmentid);
            }
            return ids;
        }
	}

	

}
