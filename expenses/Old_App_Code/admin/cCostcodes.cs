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
	/// Summary description for costcodes.
	/// Cache["costcodeslist" + accountid] = costcodelist
	/// </summary>
	public class cCostcodes : admin.cDefItems
	{
		System.Data.DataSet rcdstcostcodes = new System.Data.DataSet();
        
		public cCostcodes(int nAccountid)
		{
			accountid = nAccountid;
            
			
			InitialiseData();
		}

		public override System.Collections.SortedList CacheList()
		{
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            cNewUserDefined clsuserdefined = new cNewUserDefined(accountid, cAccounts.getConnectionString(accountid), new cTables(accountid)); ;
			System.Collections.SortedList list = new System.Collections.SortedList();
			System.Data.SqlClient.SqlDataReader reader;
			cCostCode curcostcode;
            DateTime createdon, modifiedon;
            int createdby, modifiedby;
			int costcodeid = 0;
			string costcode = "";
			string description = "";
			bool archived = false;
            Dictionary<int, object> userdefined;


            strsql = "SELECT ModifiedBy, ModifiedOn, CreatedBy, CreatedOn, archived, description, costcode, costcodeid from dbo.[costcodes] order by costcode";
            expdata.sqlexecute.CommandText = strsql;
            SqlCacheDependency dep = new SqlCacheDependency(expdata.sqlexecute);
			reader = expdata.GetReader(strsql);

			while (reader.Read())
			{
				costcodeid = reader.GetInt32(reader.GetOrdinal("costcodeid"));
				costcode = reader.GetString(reader.GetOrdinal("costcode"));
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
                userdefined = clsuserdefined.getValues(AppliesTo.Costcode, costcodeid);
                curcostcode = new cCostCode(costcodeid, costcode, description, archived, createdon, createdby, modifiedon, modifiedby, userdefined);
				list.Add(costcodeid, curcostcode);
			}

			reader.Close();
			
			expdata.sqlexecute.Parameters.Clear();
			Cache.Insert("costcodeslist" + accountid,list,dep, System.Web.Caching.Cache.NoAbsoluteExpiration,System.Web.Caching.Cache.NoSlidingExpiration, System.Web.Caching.CacheItemPriority.NotRemovable,null);
			return list;
		}
		public override void InitialiseData()
		{
            list = (System.Collections.SortedList)Cache["costcodeslist" + accountid];
			if (list == null)
			{
				list = CacheList();
			}
			
		}

		public byte changeStatus(int costcodeid, bool archive)
		{
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            //cannot remove if assigned to groups

            expdata.sqlexecute.Parameters.AddWithValue("@costcodeid", costcodeid);
            
			if (archive == true)
			{
                int count;
                strsql = "select count(*) from signoffs where include = 4 and includeid = @costcodeid";

                
                count = expdata.getcount(strsql);
                if (count != 0)
                {
                    expdata.sqlexecute.Parameters.Clear();
                    return 1;
                }
				strsql = "update costcodes set archived = 1 where costcodeid = @costcodeid";
			}
			else
			{
				strsql = "update costcodes set archived = 0 where costcodeid = @costcodeid";
			}
			expdata.ExecuteSQL(strsql);
			expdata.sqlexecute.Parameters.Clear();
			InvalidateCache("costcodes",accountid);
			InitialiseData();

            return 0;
		}


		private Infragistics.WebUI.UltraWebGrid.ValueList CacheVList()
		{
			System.Web.HttpApplication appinfo = (System.Web.HttpApplication)System.Web.HttpContext.Current.ApplicationInstance;
			int i = 0;
			int costcodeid = 0;
			string costcode = "";
			System.Collections.SortedList sortedlst = new System.Collections.SortedList();
			Infragistics.WebUI.UltraWebGrid.ValueList temp = new Infragistics.WebUI.UltraWebGrid.ValueList();
			
			bool usecostcodedesc = false;
			cCostCode reqcostcode;

            cMisc clsmisc = new cMisc(accountid);
            cGlobalProperties clsproperties = clsmisc.getGlobalProperties(accountid);

            usecostcodedesc = clsproperties.usecostcodedesc;
			for (i = 0; i < list.Count; i++)
			{
				reqcostcode = (cCostCode)list.GetByIndex(i);
				if (reqcostcode.archived == false)
				{
					costcodeid = reqcostcode.costcodeid;
					if (usecostcodedesc == false)
					{
						costcode = reqcostcode.costcode;
					}
					else
					{
						costcode = reqcostcode.description;
					}
					sortedlst.Add(costcode,costcodeid);
					
				}

			}

			for (i = 0; i < sortedlst.Count; i++)
			{
				costcodeid = (int)sortedlst.GetByIndex(i);
				costcode = (string)sortedlst.GetKey(i);
				temp.ValueListItems.Add (costcodeid,costcode);
			}

			Cache.Insert("costcodesvlist" + accountid, temp,getDependency("costcodes"));
			return temp;
		}
		public Infragistics.WebUI.UltraWebGrid.ValueList getVList()
		{
			Infragistics.WebUI.UltraWebGrid.ValueList templist;
			if (Cache["costcodesvlist" + accountid] == null)
			{
				templist = CacheVList();
			}
			else
			{
				templist = (Infragistics.WebUI.UltraWebGrid.ValueList)Cache["costcodesvlist" + accountid];
			}
			

			return templist;
		}


		public cColumnList getColumnList()
		{
			bool usedesc;
			System.Web.HttpApplication appinfo = (System.Web.HttpApplication)System.Web.HttpContext.Current.ApplicationInstance;
            cMisc clsmisc = new cMisc(accountid);
            cGlobalProperties clsproperties = clsmisc.getGlobalProperties(accountid);
            usedesc = clsproperties.usedepartmentdesc;
			cColumnList collist = new cColumnList();
			cCostCode reqcode;
			int i;

			collist.addItem(0,"");
			for (i = 0; i < list.Count; i++)
			{
				reqcode = (cCostCode)list.GetByIndex(i);
				if (usedesc == true)
				{
					collist.addItem(reqcode.costcodeid,reqcode.description);
				}
				else
				{
					collist.addItem(reqcode.costcodeid,reqcode.costcode);
				}
			}
			return collist;
		}
		public override bool alreadyExists(string costcode, bool update, int id)
		{
			bool exists = false;
			cCostCode clscostcode;

			System.Collections.IDictionaryEnumerator enumer = list.GetEnumerator();
			while (enumer.MoveNext())
			{
				clscostcode = (cCostCode)enumer.Value;
				if (clscostcode.costcode == costcode)
				{
					if (update == false || (update == true && clscostcode.costcodeid != id))
					{
						exists = true;
					}
					break;
				}
			}

			return exists;

		}



        public int addCostCode(string costcode, string description, int userid, Dictionary<int, object> userdefinedfields)
		{
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
			System.Collections.SortedList values = new System.Collections.SortedList();
			if (alreadyExists(costcode,false,0) == true)
			{
				
				return 1;
			}

            

			values.Add("costcode",costcode);
			values.Add("description",description);
			
            values.Add("createdon", DateTime.Now.ToUniversalTime());
            values.Add("createdby", userid);
			addItem("costcodes",values);

            cNewUserDefined clsuserdefined = new cNewUserDefined(accountid, cAccounts.getConnectionString(accountid), new cTables(accountid)); ;
            clsuserdefined.addValues(AppliesTo.Costcode, 0, userdefinedfields);

			cAuditLog clsaudit = new cAuditLog(accountid, userid);
			clsaudit.addRecord("Cost Codes",costcode);

			return 0;

		}

        public int updateCostCode(int costcodeid, string costcode, string description, int userid, Dictionary<int, object> userdefinedfields)
		{
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
			System.Collections.SortedList values = new System.Collections.SortedList();
			
			cCostCode reqcostcode = GetCostCodeById(costcodeid);
			if (alreadyExists(costcode,true,costcodeid) == true)
			{
				return 1;
			}

            cNewUserDefined clsuserdefined = new cNewUserDefined(accountid, cAccounts.getConnectionString(accountid), new cTables(accountid)); ;
            clsuserdefined.addValues(AppliesTo.Costcode, costcodeid, userdefinedfields);

			values.Add("costcode",costcode);
			values.Add("description",description);
            values.Add("modifiedon", DateTime.Now.ToUniversalTime());
            values.Add("modifiedby", userid);
			updateItem("costcodes","costcodeid",costcodeid,values);

			cAuditLog clsaudit = new cAuditLog(accountid, userid);
			if (reqcostcode.costcode != costcode)
			{
				clsaudit.editRecord(costcode, "Cost Code","Cost Codes",reqcostcode.costcode,costcode);
			}
			if (reqcostcode.description != description)
			{
				clsaudit.editRecord(costcode, "Description", "Cost Codes",reqcostcode.description,description);
			}
			

			return 0;
		}

		public byte deleteCostCode(int costcodeid)
		{
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
			cCostCode reqcostcode = GetCostCodeById(costcodeid);
			expdata.sqlexecute.Parameters.AddWithValue("@costcodeid",costcodeid);

            int count;
            strsql = "select count(*) from signoffs where include = 4 and includeid = @costcodeid";


            count = expdata.getcount(strsql);
            if (count != 0)
            {
                expdata.sqlexecute.Parameters.Clear();
                return 1;
            }

			strsql = "update [savedexpenses_costcodes] set [costcodeid] = null where costcodeid = @costcodeid";
			expdata.ExecuteSQL(strsql);

			strsql = "update employee_costcodes set costcodeid = null where costcodeid = @costcodeid";
			expdata.ExecuteSQL(strsql);

			expdata.sqlexecute.Parameters.Clear();
			
			deleteItem("costcodes","costcodeid",costcodeid);

			cAuditLog clsaudit = new cAuditLog();
			clsaudit.deleteRecord("Cost Codes", reqcostcode.costcode);
			InitialiseData();

            return 0;
		}

		public override System.Data.DataTable getGrid()
		{
			return null;
		}
		public System.Data.DataTable getGrid(byte filter)
		{
			System.Data.DataTable costcodes = new System.Data.DataTable();
			object[] values;
			bool display = false;
			costcodes.Columns.Add("costcodeid",System.Type.GetType("System.Int32"));
			costcodes.Columns.Add("costcode",System.Type.GetType("System.String"));
			costcodes.Columns.Add("description",System.Type.GetType("System.String"));
			costcodes.Columns.Add("archived",System.Type.GetType("System.Boolean"));
			cCostCode clscostcode;

			System.Collections.IDictionaryEnumerator enumer = list.GetEnumerator();

			while (enumer.MoveNext())
			{
				clscostcode = (cCostCode)enumer.Value;
				display = false;
				switch (filter)
				{
					case 1:
						display = true;
						break;
					case 2:
						if (clscostcode.archived == true)
						{
							display = true;
						}
						break;
					case 3:
						if (clscostcode.archived == false)
						{
							display = true;
						}
						break;
				}

				if (display == true)
				{
					values = new object[4];
					values[0] = clscostcode.costcodeid;
					values[1] = clscostcode.costcode;
					values[2] = clscostcode.description;

					values[3] = clscostcode.archived;
					costcodes.Rows.Add(values);
				}
			}

			return costcodes;
		}

		public cCostCode GetCostCodeById(int costcodeid)
		{
			return (cCostCode)list[costcodeid];
		}

        public cCostCode getCostCodeByString(string costcode)
        {
            cCostCode reqcostcode = null;
            foreach (cCostCode clscostcode in list.Values)
            {
                if (clscostcode.costcode == costcode)
                {
                    reqcostcode = clscostcode;
                    break;
                }
            }
            return reqcostcode;
        }

        public cCostCode getCostCodeByDesc(string desc)
        {
            cCostCode reqcostcode = null;
            foreach (cCostCode clscostcode in list.Values)
            {
                if (clscostcode.description == desc)
                {
                    reqcostcode = clscostcode;
                    break;
                }
            }
            return reqcostcode;
        }

		public string CreateStringDropDown(int costcodeid, bool readOnly, bool blank)
		{
			
			System.Web.HttpApplication appinfo = (System.Web.HttpApplication)System.Web.HttpContext.Current.ApplicationInstance;
			System.Collections.SortedList sortedlst = new System.Collections.SortedList();
			bool usedesc;
			cCostCode reqcode;
			int i;
			System.Text.StringBuilder output = new System.Text.StringBuilder();
            cMisc clsmisc = new cMisc(accountid);
            cGlobalProperties clsproperties = clsmisc.getGlobalProperties(accountid);
            usedesc = clsproperties.usecostcodedesc;
			output.Append("<select name=costcode");
			if (readOnly == true)
			{
				output.Append(" disabled");
			}
			output.Append(">");
			if (blank == true)
			{
				output.Append("<option value=\"0\"");
				if (costcodeid == 0)
				{
					output.Append(" selected");
				}
				output.Append("></option>");
			}

			for (i = 0; i < list.Count; i++)
			{
				reqcode = (cCostCode)list.GetByIndex(i);
				if (usedesc == true)
				{
					sortedlst.Add(reqcode.description,reqcode);
				}
				else
				{
					sortedlst.Add(reqcode.costcode, reqcode);
				}
			}
			for (i = 0; i < sortedlst.Count; i++)
			{
				reqcode = (cCostCode)sortedlst.GetByIndex(i);
				if (reqcode.archived == false)
				{
					output.Append("<option value=\"" + reqcode.costcodeid + "\"");
					if (costcodeid == reqcode.costcodeid)
					{
						output.Append(" selected");
					}
					output.Append(">");
					if (usedesc == false)
					{
						output.Append(reqcode.costcode);
					}
					else
					{
						output.Append(reqcode.description);
					}
					output.Append("</option>");
				}
				
			}
			output.Append("</select>");
			return output.ToString();
		}

		private System.Web.UI.WebControls.ListItemCollection CacheDropDown()
		{
			System.Web.HttpApplication appinfo = (System.Web.HttpApplication)System.Web.HttpContext.Current.ApplicationInstance;
			cCostCode reqcostcode;
			//			System.Web.UI.WebControls.ListItem[] tempItems = new System.Web.UI.WebControls.ListItem[list.Count + 1];
			System.Web.UI.WebControls.ListItemCollection tempItems = new System.Web.UI.WebControls.ListItemCollection();
			System.Web.UI.WebControls.ListItem item;
			System.Collections.SortedList sortedlst = new System.Collections.SortedList();

            cMisc clsmisc = new cMisc(accountid);
            cGlobalProperties clsproperties = clsmisc.getGlobalProperties(accountid);

            bool usedesc = clsproperties.usecostcodedesc;
			int i;

			for (i = 0; i < list.Count; i++)
			{
				reqcostcode = (cCostCode)list.GetByIndex(i);
				if (reqcostcode.archived == false)
				{
					if (usedesc == true)
					{
						sortedlst.Add(reqcostcode.description,reqcostcode.costcodeid);
					}
					else
					{
						sortedlst.Add(reqcostcode.costcode, reqcostcode.costcodeid);
					}
				}
			}

			
			item = new System.Web.UI.WebControls.ListItem("","");
			tempItems.Add(item);

			for (i = 0; i < sortedlst.Count; i++)
			{
				tempItems.Add(new System.Web.UI.WebControls.ListItem((string)sortedlst.GetKey(i),sortedlst.GetByIndex(i).ToString()));
			}
			
			//Cache.Add("departmentsddown" + accountid,tempItems,dep);
			Cache.Add("costcodesddown" + accountid,tempItems,getDependency("costcodes"),System.Web.Caching.Cache.NoAbsoluteExpiration, System.Web.Caching.Cache.NoSlidingExpiration,System.Web.Caching.CacheItemPriority.Default,null);
			
			return tempItems;
		}


        public List<System.Web.UI.WebControls.ListItem> CreateDropDown(bool usedesc)
        {
            List<System.Web.UI.WebControls.ListItem> items = new List<System.Web.UI.WebControls.ListItem>();
            cCostCode reqcostcode;

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
                reqcostcode = (cCostCode)sorted.GetByIndex(i);
                if (!reqcostcode.archived)
                {
                    if (usedesc)
                    {
                        items.Add(new System.Web.UI.WebControls.ListItem(reqcostcode.description, reqcostcode.costcodeid.ToString()));
                    }
                    else
                    {
                        items.Add(new System.Web.UI.WebControls.ListItem(reqcostcode.costcode, reqcostcode.costcodeid.ToString()));
                    }
                }
            }

            return items;


        }

        public SortedList sortList()
        {
            SortedList sorted = new SortedList();
            cCostCode costcode;
            for (int i = 0; i < list.Count; i++)
            {
                costcode = (cCostCode)list.GetByIndex(i);
                sorted.Add(costcode.costcode, costcode);
            }
            return sorted;
        }

        private SortedList sortListByDescription()
        {
            SortedList sorted = new SortedList();
            cCostCode costcode;
            for (int i = 0; i < list.Count; i++)
            {
                costcode = (cCostCode)list.GetByIndex(i);
                sorted.Add(costcode.description, costcode);
            }
            return sorted;
        }

        public int count
        {
            get { return list.Count; }
        }

        public ArrayList getModifiedCategories(DateTime date)
        {
            ArrayList lst = new ArrayList();
            foreach (cCostCode val in list.Values)
            {
                if (val.createdon > date || val.modifiedon > date)
                {
                    lst.Add(val);
                }
            }
            return lst;
        }

        

        public ArrayList getCostcodeIds()
        {
            ArrayList ids = new ArrayList();
            foreach (cCostCode val in list.Values)
            {
                ids.Add(val.costcodeid);
            }
            return ids;
        }
	}

	


}
