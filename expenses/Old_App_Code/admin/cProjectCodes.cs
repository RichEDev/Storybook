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
	/// Summary description for cProjectCodes.
	/// </summary>
	public class cProjectCodes
	{
		string strsql;
		int nAccountid = 0;
		
		DBConnection expdata;
		System.Collections.SortedList list;
        System.Web.Caching.Cache Cache = System.Web.HttpRuntime.Cache;

		

		public cProjectCodes(int accountid)
		{
			nAccountid = accountid;
            expdata = new DBConnection(cAccounts.getConnectionString(accountid));
			
			InitialiseData();
		}

		public int accountid
		{
			get {return nAccountid;}
		}

		#region cache
		private void CreateDependency()
		{
			if (Cache["projectcodesdependency" + accountid] == null)
			{
				Cache.Insert("projectcodesdependency" + accountid,1);
				
			}
		}
		
		private System.Web.Caching.CacheDependency getDependency()
		{
			System.Web.Caching.CacheDependency dep;
			String[] dependency;
			dependency = new string[1];
			dependency[0] = "projectcodesdependency" + accountid;
			dep = new System.Web.Caching.CacheDependency(null,dependency);
			return dep;
		}

		private void InvalidateCache()
		{
			Cache.Remove("projectcodesdependency" + accountid);
			CreateDependency();
		}
		#endregion
		private void InitialiseData()
		{
            list = (System.Collections.SortedList)Cache["projectcodes" + accountid];
			if (list == null)
			{
				list = CacheList();
			}
			
		}

		private System.Collections.SortedList CacheList()
		{
			System.Collections.SortedList list = new System.Collections.SortedList();
			System.Data.SqlClient.SqlDataReader reader;

            cNewUserDefined clsuserdefined = new cNewUserDefined(accountid, cAccounts.getConnectionString(accountid), new cTables(accountid)); ;
            Dictionary<int, object> userdefined;
            DateTime createdon, modifiedon;
            int createdby, modifiedby;
			int projectcodeid;
			string projectcode, description;
			bool archived, rechargeable;
			cProjectCode newcode;
			strsql = "select projectcodeid, projectcode, description, archived, rechargeable, CreatedOn, CreatedBy, ModifiedBy, ModifiedOn from dbo.project_codes";
			
            expdata.sqlexecute.CommandText = strsql;
            SqlCacheDependency dep = new SqlCacheDependency(expdata.sqlexecute);
			reader = expdata.GetReader(strsql);
            expdata.sqlexecute.Parameters.Clear();
			while (reader.Read())
			{
				projectcodeid = reader.GetInt32(reader.GetOrdinal("projectcodeid"));
				projectcode = reader.GetString(reader.GetOrdinal("projectcode"));
				if (reader.IsDBNull(reader.GetOrdinal("description")) == false)
				{
					description = reader.GetString(reader.GetOrdinal("description"));
				}
				else
				{
					description = "";
				}
				archived = reader.GetBoolean(reader.GetOrdinal("archived"));
                rechargeable = reader.GetBoolean(reader.GetOrdinal("rechargeable"));
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
                userdefined = clsuserdefined.getValues(AppliesTo.Projectcode, projectcodeid);
                newcode = new cProjectCode(projectcodeid, projectcode, description, archived, rechargeable, createdon, createdby, modifiedon, modifiedby, userdefined);
				list.Add(projectcodeid,newcode);
			}	

			reader.Close();
			

			Cache.Insert("projectcodes" + accountid,list,dep,System.Web.Caching.Cache.NoAbsoluteExpiration,TimeSpan.FromMinutes(15), System.Web.Caching.CacheItemPriority.NotRemovable, null);
			return list;
		}

        public byte addProjectCode(string projectcode, string description, bool rechargeable, int userid, Dictionary<int, object> userdefinedfields)
		{
			if (alreadyExists(projectcode,0,0) == true)
			{
				return 1;
			}
			int projectcodeid;
            

			strsql = "insert into project_codes (projectcode, description, rechargeable, createdon, createdby) " +
				"values (@projectcode, @description, @rechargeable, @createdon, @createdby);set @identity = @@identity;";
            
            expdata.sqlexecute.Parameters.AddWithValue("@projectcode", projectcode);
            expdata.sqlexecute.Parameters.AddWithValue("@description", description);
            expdata.sqlexecute.Parameters.AddWithValue("@rechargeable", Convert.ToByte(rechargeable));
            expdata.sqlexecute.Parameters.AddWithValue("@createdon", DateTime.Now.ToUniversalTime());
            expdata.sqlexecute.Parameters.AddWithValue("@createdby", userid);
			expdata.sqlexecute.Parameters.AddWithValue("@identity",System.Data.SqlDbType.Int);
			expdata.sqlexecute.Parameters["@identity"].Direction = System.Data.ParameterDirection.Output;
			expdata.ExecuteSQL(strsql);

			projectcodeid = (int)expdata.sqlexecute.Parameters["@identity"].Value;
			expdata.sqlexecute.Parameters.Clear();
            cNewUserDefined clsuserdefined = new cNewUserDefined(accountid, cAccounts.getConnectionString(accountid), new cTables(accountid)); ;
            clsuserdefined.addValues(AppliesTo.Projectcode, projectcodeid, userdefinedfields);
			cProjectCode newcode = new cProjectCode(projectcodeid,projectcode,description,false, rechargeable, DateTime.Now.ToUniversalTime(), userid, new DateTime(1900,01,01), 0, userdefinedfields);

			list.Add(projectcodeid,newcode);
			return 0;
		}

        public byte updateProjectCode(int projectcodeid, string projectcode, string description, bool rechargeable, int userid, Dictionary<int, object> userdefinedfields)
		{
			cProjectCode oldcode;
			bool archived;

			if (alreadyExists(projectcode,projectcodeid,2))
			{
				return 1;
			}

			oldcode = getProjectCodeById(projectcodeid);
			archived = oldcode.archived;
            
			strsql = "update project_codes set projectcode = @projectcode, description = @description, rechargeable = @rechargeable, modifiedon = @modifiedon, modifiedby = @modifiedby " +
				"where projectcodeid = @projectcodeid";
            expdata.sqlexecute.Parameters.AddWithValue("@projectcode", projectcode);
            expdata.sqlexecute.Parameters.AddWithValue("@description", description);
            expdata.sqlexecute.Parameters.AddWithValue("@projectcodeid", projectcodeid);
            expdata.sqlexecute.Parameters.AddWithValue("@rechargeable", Convert.ToByte(rechargeable));
            expdata.sqlexecute.Parameters.AddWithValue("@modifiedon", DateTime.Now.ToUniversalTime());
            expdata.sqlexecute.Parameters.AddWithValue("@modifiedby", userid);
			expdata.ExecuteSQL(strsql);
			expdata.sqlexecute.Parameters.Clear();

            cNewUserDefined clsuserdefined = new cNewUserDefined(accountid, cAccounts.getConnectionString(accountid), new cTables(accountid)); ;
            clsuserdefined.addValues(AppliesTo.Projectcode, projectcodeid, userdefinedfields);
			list[projectcodeid] = new cProjectCode(projectcodeid, projectcode, description, archived, rechargeable, oldcode.createdon,oldcode.createdby,DateTime.Now.ToUniversalTime(),userid, userdefinedfields);
			return 0;
		}

		public void deleteProjectCode(int projectcodeid)
		{
            expdata.sqlexecute.Parameters.AddWithValue("@projectcodeid", projectcodeid);
            strsql = "update [savedexpenses_costcodes] set [projectcodeid] = null where projectcodeid = @projectcodeid";
            expdata.ExecuteSQL(strsql);

            strsql = "update employee_costcodes set projectcodeid = null where projectcodeid = @projectcodeid";
            expdata.ExecuteSQL(strsql);

			strsql = "delete from project_codes where projectcodeid = @projectcodeid";
			
			expdata.ExecuteSQL(strsql);
			expdata.sqlexecute.Parameters.Clear();

			list.Remove(projectcodeid);
		}

		public void changeStatus(int projectcodeid, bool archive)
		{
			expdata.sqlexecute.Parameters.AddWithValue("@projectcodeid",projectcodeid);
			if (archive == true)
			{
				strsql = "update project_codes set archived = 1 where projectcodeid = @projectcodeid";
			}
			else
			{
				strsql = "update project_codes set archived = 0 where projectcodeid = @projectcodeid";
			}
			expdata.ExecuteSQL(strsql);
			expdata.sqlexecute.Parameters.Clear();
			
			cProjectCode code = getProjectCodeById(projectcodeid);
			code.updateStatus(archive);
		}

        private System.Web.UI.WebControls.ListItemCollection CacheDropDown()
        {

            cProjectCode reqprojectcode;
            //			System.Web.UI.WebControls.ListItem[] tempItems = new System.Web.UI.WebControls.ListItem[list.Count + 1];
            System.Web.UI.WebControls.ListItemCollection tempItems = new System.Web.UI.WebControls.ListItemCollection();
            System.Web.UI.WebControls.ListItem item;
            System.Collections.SortedList sortedlst = new System.Collections.SortedList();
            System.Web.Caching.CacheDependency dep;
            String[] dependency;
            dependency = new string[1];
            dependency[0] = "projectcodesdependency" + accountid;
            dep = new System.Web.Caching.CacheDependency(null, dependency);

            int i;

            for (i = 0; i < list.Count; i++)
            {
                reqprojectcode = (cProjectCode)list.GetByIndex(i);
                if (reqprojectcode.archived == false)
                {
                    sortedlst.Add(reqprojectcode.projectcode, reqprojectcode.projectcodeid);
                }
            }


            item = new System.Web.UI.WebControls.ListItem("", "");
            tempItems.Add(item);

            for (i = 0; i < sortedlst.Count; i++)
            {
                tempItems.Add(new System.Web.UI.WebControls.ListItem((string)sortedlst.GetKey(i), sortedlst.GetByIndex(i).ToString()));
            }

            //Cache.Add("departmentsddown" + accountid,tempItems,dep);
            Cache.Add("projectcodesddown" + accountid, tempItems, dep, System.Web.Caching.Cache.NoAbsoluteExpiration, System.Web.Caching.Cache.NoSlidingExpiration, System.Web.Caching.CacheItemPriority.Default, null);

            return tempItems;
        }

        public List<System.Web.UI.WebControls.ListItem> CreateDropDown(bool usedesc)
        {
            List<System.Web.UI.WebControls.ListItem> items = new List<System.Web.UI.WebControls.ListItem>();
            cProjectCode reqcode;

            SortedList sorted;
            if (usedesc)
            {
                sorted = sortListByDescription();
            }
            else
            {
                sorted = sortList();
            }

            for (int i = 0; i < sorted.Count; i++)
            {
                reqcode = (cProjectCode)sorted.GetByIndex(i);
                if (!reqcode.archived)
                {
                    if (usedesc)
                    {
                        items.Add(new System.Web.UI.WebControls.ListItem(reqcode.description, reqcode.projectcodeid.ToString()));
                    }
                    else
                    {
                        items.Add(new System.Web.UI.WebControls.ListItem(reqcode.projectcode, reqcode.projectcodeid.ToString()));
                    }
                }
            }

            return items;


        }
		public System.Data.DataSet getGrid(byte filter)
		{
			System.Collections.SortedList sorted = sortList();
			System.Data.DataSet ds = new System.Data.DataSet();
			cProjectCode code;
			bool display;
			object[] values;
			int i;
			System.Data.DataTable projectcodes = new System.Data.DataTable();
			projectcodes.Columns.Add("projectcodeid",System.Type.GetType("System.Int32"));
			projectcodes.Columns.Add("projectcode",System.Type.GetType("System.String"));
			projectcodes.Columns.Add("description",System.Type.GetType("System.String"));
			projectcodes.Columns.Add("archived",System.Type.GetType("System.Boolean"));

			for (i = 0; i < sorted.Count; i++)
			{
				display = false;
				code = (cProjectCode)sorted.GetByIndex(i);
				switch (filter)
				{
					case 1:
						display = true;
						break;
					case 2:
						if (code.archived == true)
						{
							display = true;
						}
						break;
					case 3:
						if (code.archived == false)
						{
							display = true;
						}
						break;
				}

				if (display == true)
				{
					values = new object[4];
					values[0] = code.projectcodeid;
					values[1] = code.projectcode;
					values[2] = code.description;
					values[3] = code.archived;
					projectcodes.Rows.Add(values);
				}
			}

			ds.Tables.Add(projectcodes);
			return ds;
		}

		private System.Collections.SortedList sortList()
		{
			System.Collections.SortedList sorted = new System.Collections.SortedList();
			cProjectCode code;
			int i;

			for (i = 0; i < list.Count; i++)
			{
				code = (cProjectCode)list.GetByIndex(i);
				sorted.Add(code.projectcode,code);
			}
			return sorted;
		}
        
        private SortedList sortListByDescription()
        {
            System.Collections.SortedList sorted = new System.Collections.SortedList();
            cProjectCode code;
            int i;

            for (i = 0; i < list.Count; i++)
            {
                code = (cProjectCode)list.GetByIndex(i);
                sorted.Add(code.description, code);
            }
            return sorted;
        }
		public cColumnList getColumnList()
		{
			bool usedesc;
			System.Web.HttpApplication appinfo = (System.Web.HttpApplication)System.Web.HttpContext.Current.ApplicationInstance;
            cMisc clsmisc = new cMisc(accountid);
            cGlobalProperties clsproperties = clsmisc.getGlobalProperties(accountid);
            usedesc = clsproperties.useprojectcodedesc;
			cColumnList collist = new cColumnList();
			cProjectCode reqcode;
			int i;
			collist.addItem(0,"");
			for (i = 0; i < list.Count; i++)
			{
				reqcode = (cProjectCode)list.GetByIndex(i);
				if (usedesc == true)
				{
					collist.addItem(reqcode.projectcodeid,reqcode.description);
				}
				else
				{
					collist.addItem(reqcode.projectcodeid,reqcode.projectcode);
				}
			}

			return collist;
		}
		public cProjectCode getProjectCodeById(int projectcodeid)
		{
			return (cProjectCode)list[projectcodeid];
		}

        public cProjectCode getProjectCodeByString(string projectcode)
        {
            cProjectCode reqprojcode = null;
            foreach (cProjectCode clsproj in list.Values)
            {
                if (clsproj.projectcode == projectcode)
                {
                    reqprojcode = clsproj;
                    break;
                }
            }
            return reqprojcode;
        }

        public cProjectCode getProjectCodeByDesc(string desc)
        {
            cProjectCode reqprojcode = null;
            foreach (cProjectCode clsproj in list.Values)
            {
                if (clsproj.description == desc)
                {
                    reqprojcode = clsproj;
                    break;
                }
            }
            return reqprojcode;
        }

		private bool alreadyExists(string projectcode, int projectcodeid, int action)
		{
			int i;
			cProjectCode code;
			for (i = 0; i < list.Count; i++)
			{
				code = (cProjectCode)list.GetByIndex(i);
				if (action == 0)
				{
					if (code.projectcode.ToLower() == projectcode.ToLower())
					{
						return true;
					}
				}
				else
				{
					if (action == 2 && projectcode.ToLower() == code.projectcode.ToLower() && projectcodeid != code.projectcodeid)
					{
						return true;
					}
				}
			}

			return false;
		}

		public string CreateStringDropDown(int projectcodeid, bool readOnly, bool blank)
		{
			System.Web.HttpApplication appinfo = (System.Web.HttpApplication)System.Web.HttpContext.Current.ApplicationInstance;
			bool usedesc;
			cProjectCode reqcode;
			int i;
			System.Text.StringBuilder output = new System.Text.StringBuilder();
			System.Collections.SortedList sortedlst = new System.Collections.SortedList();

            cMisc clsmisc = new cMisc(accountid);
            cGlobalProperties clsproperties = clsmisc.getGlobalProperties(accountid);
            usedesc = clsproperties.useprojectcodedesc;

			output.Append("<select name=projectcode");
			if (readOnly == true)
			{
				output.Append(" disabled");
			}
			output.Append(">");
			if (blank == true)
			{
				output.Append("<option value=\"0\"");
				if (projectcodeid == 0)
				{
					output.Append(" selected");
				}
				output.Append("></option>");
			}

			for (i = 0; i < list.Count; i++)
			{
				reqcode = (cProjectCode)list.GetByIndex(i);
                if (reqcode.archived == false)
                {
                    if (usedesc == true)
                    {
                        sortedlst.Add(reqcode.description, reqcode);
                    }
                    else
                    {
                        sortedlst.Add(reqcode.projectcode, reqcode);
                    }
                }
			}

			for (i = 0; i < sortedlst.Count; i++)
			{
				reqcode = (cProjectCode)sortedlst.GetByIndex(i);
				output.Append("<option value=\"" + reqcode.projectcodeid + "\"");
				if (projectcodeid == reqcode.projectcodeid)
				{
					output.Append(" selected");
				}
				output.Append(">");
				if (usedesc == false)
				{
					output.Append(reqcode.projectcode);
				}
				else
				{
					output.Append(reqcode.description);
				}
				output.Append("</option>");
				
			}
			output.Append("</select>");
			return output.ToString();
		}

        public int count
        {
            get { return list.Count; }

        }

        public ArrayList getModifiedProjectCodes(DateTime date)
        {
            ArrayList lst = new ArrayList();
            foreach (cProjectCode val in list.Values)
            {
                if (val.createdon > date || val.modifiedon > date)
                {
                    lst.Add(val);
                }
            }
            return lst;
        }

        public ArrayList getProjectcodeIds()
        {
            ArrayList ids = new ArrayList();
            foreach (cProjectCode val in list.Values)
            {
                ids.Add(val.projectcodeid);
            }
            return ids;
        }
	}

	
}
