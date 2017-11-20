using System;

using System.Collections.Generic;

using System.Collections;

using SpendManagementLibrary;
using SpendManagementLibrary.Employees;
using Spend_Management;
using System.Web.UI.WebControls;
using System.Web.Caching;
using System.Data.SqlClient;

namespace Spend_Management
{
	/// <summary>
	/// Summary description for cViews.
	/// </summary>
	public class cViews
	{
		DBConnection expdata;
		string strsql = "";
		int employeeid;
		int nAccountid;

		public cViews(int accountid, int iEmployeeid)
		{
            nAccountid = accountid;
			employeeid = iEmployeeid;
			
            expdata = new DBConnection(cAccounts.getConnectionString(accountid));
        }

        #region properties
        public int accountid
        {
            get { return nAccountid; }
        }
        #endregion


		public SortedList<int, cField> getDefaultView()
		{
			SortedList<int, cField> lstFields = System.Web.HttpRuntime.Cache["defaultview_" + accountid] as SortedList<int, cField>;

			if (lstFields == null)
			{
				cFields clsFields = new cFields(accountid);
				lstFields = new SortedList<int, cField>();
				strsql = "SELECT fieldid, [order], createdon, createdby from dbo.default_views";
				expdata.sqlexecute.CommandText = strsql;
                if (GlobalVariables.GetAppSettingAsBoolean("EnableBrokers"))
                {
                    SqlCacheDependency dep = new SqlCacheDependency(expdata.sqlexecute);
                    System.Web.HttpRuntime.Cache.Insert("defaultview_" + accountid, lstFields, dep, Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(15));
                }

				using (SqlDataReader reader = expdata.GetReader(strsql))
				{
					while (reader.Read())
					{
						Guid fieldID = reader.GetGuid(reader.GetOrdinal("fieldid"));
						int order = reader.GetInt32(reader.GetOrdinal("order"));
						cField reqField = clsFields.GetFieldByID(fieldID);

						if (reqField != null)
						{
							lstFields.Add(order, reqField);
						}
					}

					reader.Close();
				}

			}

			return lstFields;
		}

        public SortedList<int, cField> getDefaultPrintView()
        {
            SortedList<int, cField> lstFields;

            if (System.Web.HttpRuntime.Cache["defaultprintview_" + accountid] != null)
            {
                lstFields = (SortedList<int, cField>)System.Web.HttpRuntime.Cache["defaultprintview_" + accountid];
            }
            else
            {
                int order;
                Guid fieldID;
                cField reqField;
                cFields clsFields = new cFields(accountid);
                lstFields = new SortedList<int, cField>();
                System.Data.SqlClient.SqlDataReader reader;
                strsql = "SELECT fieldid, [order], createdon, createdby from dbo.print_views";
                expdata.sqlexecute.CommandText = strsql;
                if (GlobalVariables.GetAppSettingAsBoolean("EnableBrokers"))
                {
                    var dep = new SqlCacheDependency(expdata.sqlexecute);
                    System.Web.HttpRuntime.Cache.Insert("defaultprintview_" + accountid, lstFields, dep, System.Web.Caching.Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(15));
                }

                using (reader = expdata.GetReader(strsql))
                {
                    while (reader.Read())
                    {
                        fieldID = reader.GetGuid(reader.GetOrdinal("fieldid"));
                        order = reader.GetInt32(reader.GetOrdinal("order"));
                        reqField = clsFields.GetFieldByID(fieldID);

                        if (reqField != null)
                        {
                            lstFields.Add(order, reqField);
                        }
                    }

                    reader.Close();
                }
            }

            return lstFields;
        }

        public System.Web.UI.WebControls.ListItem[] getSelectedItems(int viewid)
		{
            cMisc clsmisc = new cMisc(accountid);
            cFields clsfields = new cFields(accountid);
            cField field;
            Guid fieldid;
			expdata.sqlexecute.Parameters.AddWithValue("@viewid",viewid);
			expdata.sqlexecute.Parameters.AddWithValue("@employeeid",employeeid);
			
			int count = 0;
			bool usingDefault = false;
			int i = 0;
			System.Data.SqlClient.SqlDataReader view;
			if (viewid == -1)
			{
				strsql = "select count(*) from default_views";	
			}
			else if (viewid == -2)
			{
				strsql = "select count(*) from print_views";	
			}
			else
			{
				strsql = "select count(*) from views where viewid = @viewid and employeeid = @employeeid";	
			}
			count = expdata.getcount(strsql);
			if (count == 0 && viewid != -1 && viewid != -2)
			{
				strsql = "select count(*) from default_views";	
				count = expdata.getcount(strsql);
				usingDefault = true;
			}
            List<ListItem> items = new List<ListItem>();

			if (viewid == -1 || usingDefault == true)
			{
                strsql = "select fieldid from default_views order by [order]";
			}
			else if (viewid == -2)
			{
                strsql = "select fieldid from print_views order by [order]";
			}
			else
			{
				strsql = "select fieldid from views where employeeid = @employeeid and viewid = @viewid order by [order]";
			}

            using (view = expdata.GetReader(strsql))
            {
                while (view.Read())
                {
                    fieldid = view.GetGuid(0);
                    field = clsfields.GetFieldByID(fieldid);

                    if (field != null)
                    {
                        items.Add(new ListItem(field.Description, field.FieldID.ToString()));
                    }
                }

                view.Close();
            }

            expdata.sqlexecute.Parameters.Clear();
            return items.ToArray();
		}









        public void updateUserView(int viewid, System.Collections.ArrayList fields, bool offline)
        {
            System.Data.SqlClient.SqlDataReader reader;
            cAuditLog clsaudit;

            if (offline)
            {
                clsaudit = new cAuditLog(accountid, employeeid);
            }
            else
            {
                clsaudit = new cAuditLog();
            }

            cFields clsfields = new cFields(accountid);
            cField reqfield;
			int i = 0;
			int order = 1;

            CurrentUser user = new CurrentUser();
            user = cMisc.GetCurrentUser();
            int userid = user.EmployeeID;
            DateTime createdon = DateTime.Now.ToUniversalTime();

            expdata.sqlexecute.Parameters.AddWithValue("@createdon", createdon);
            expdata.sqlexecute.Parameters.AddWithValue("@createdby", userid);
			expdata.sqlexecute.Parameters.AddWithValue("@viewid",viewid);
			expdata.sqlexecute.Parameters.AddWithValue("@employeeid",employeeid);
			//delete the current view
			string currentfields = "";
			strsql = "select fieldid from [views] where viewid = @viewid and employeeid = @employeeid";
            using (reader = expdata.GetReader(strsql))
            {
                while (reader.Read())
                {
                    currentfields += reader.GetGuid(0) + ",";
                }
                reader.Close();
            }

			if (currentfields.Length != 0)
			{
				currentfields = currentfields.Remove(currentfields.Length-1,1);
			}

			strsql = "delete from [views] where viewid = @viewid and employeeid = @employeeid";
			expdata.ExecuteSQL(strsql);

			clsaudit.deleteRecord(SpendManagementElement.Views,viewid, "View=" + viewid + ",fields=" + currentfields);

            List<cField> tblfields;
            if (viewid == 3)
            {
                tblfields = clsfields.GetFieldsByTableID(new Guid("d70d9e5f-37e2-4025-9492-3bcf6aa746a8"));
            }
            else
            {
                tblfields = clsfields.GetFieldsByTableID(new Guid("d70d9e5f-37e2-4025-9492-3bcf6aa746a8"));
            }
			for (i = 0; i < fields.Count; i++)
			{
                reqfield = clsfields.GetFieldByID(new Guid(fields[i].ToString()));
                if (reqfield.GetParentTable().TableID == new Guid("d70d9e5f-37e2-4025-9492-3bcf6aa746a8")) //update
                {
                    foreach (cField tblfield in tblfields)
                    {
                        if (tblfield.FieldName == reqfield.FieldName)
                        {
                            fields[i] = tblfield.FieldID;
                            break;
                        }
                    }
                }
				expdata.sqlexecute.Parameters.AddWithValue("@fieldid" + i,fields[i]);
                strsql = "insert into [views] (viewid, employeeid, fieldid, [order], createdon, createdby) " +
                    "values (@viewid,@employeeid,@fieldid" + i + "," + order + ", @createdon, @createdby)";
				expdata.ExecuteSQL(strsql);
                clsaudit.addRecord(SpendManagementElement.Views, "Field=" + fields[i], viewid);
				order++;
			}
			expdata.sqlexecute.Parameters.Clear();

            //delete the current view
            cEmployees clsemployees = new cEmployees(accountid);
            Employee reqemp = clsemployees.GetEmployeeById(employeeid);
            UserView view = (UserView)viewid;
            reqemp.GetViews().Remove(view);
			
			UncacheEmployee(employeeid);
		}

		private void UncacheEmployee(int employeeId)
		{
			DBConnection db = new DBConnection(cAccounts.getConnectionString(nAccountid));
			db.sqlexecute.Parameters.AddWithValue("@tablename", "employees");
			db.sqlexecute.Parameters.AddWithValue("@tablecolumn", "employeeid");
			db.sqlexecute.Parameters.AddWithValue("@id", employeeId);
			db.ExecuteProc("updateCacheExpiry");
			db.sqlexecute.Parameters.Clear();
		}

		public void updateDefaultView(System.Collections.ArrayList fields)
		{
			int i = 0;
			int order = 1;
            CurrentUser user = new CurrentUser();
            user = cMisc.GetCurrentUser();
            int userid = user.EmployeeID;
            DateTime createdon = DateTime.Now.ToUniversalTime();

            expdata.sqlexecute.Parameters.AddWithValue("@createdon", createdon);
            expdata.sqlexecute.Parameters.AddWithValue("@createdby", userid);
			//delete the current view
			strsql = "delete from [default_views]";
			expdata.ExecuteSQL(strsql);

			for (i = 0; i < fields.Count; i++)
			{
				expdata.sqlexecute.Parameters.AddWithValue("@fieldid" + i,fields[i]);
				strsql = "insert into [default_views] (fieldid, [order], createdon, createdby) " +
					"values (@fieldid" + i + "," + order + ", @createdon, @createdby)";
				expdata.ExecuteSQL(strsql);
				order++;
			}
			expdata.sqlexecute.Parameters.Clear();
		}

		public void updatePrintView(System.Collections.ArrayList fields)
		{
			int i = 0;
			int order = 1;
            CurrentUser user = new CurrentUser();
            user = cMisc.GetCurrentUser();
            int userid = user.EmployeeID;
            DateTime createdon = DateTime.Now.ToUniversalTime();

            expdata.sqlexecute.Parameters.AddWithValue("@createdon", createdon);
            expdata.sqlexecute.Parameters.AddWithValue("@createdby", userid);
			
			//delete the current view
			strsql = "delete from [print_views]";
			expdata.ExecuteSQL(strsql);

			for (i = 0; i < fields.Count; i++)
			{
				expdata.sqlexecute.Parameters.AddWithValue("@fieldid" + i,fields[i]);
                strsql = "insert into [print_views] (fieldid, [order], createdon, createdby) " +
                    "values (@fieldid" + i + "," + order + ", @createdon, @createdby)";
				expdata.ExecuteSQL(strsql);
				order++;
			}
			expdata.sqlexecute.Parameters.Clear();
		}
		
            

		public System.Data.DataSet getFloatAllocationView(int viewid, int floatid)
		{
			string viewsql = "";

            cEmployees clsemployees = new cEmployees(accountid);
            Employee reqemp = clsemployees.GetEmployeeById(employeeid);
            
			System.Data.DataSet rcdsttemp = new System.Data.DataSet();
            viewsql = clsemployees.getUserView(reqemp.EmployeeID,(UserView)viewid,false).sql;
            
			viewsql += " where float_allocations.floatid = @floatid";
			expdata.sqlexecute.Parameters.AddWithValue("@floatid",floatid);

            
            rcdsttemp = expdata.GetDataSet(viewsql);
			expdata.sqlexecute.Parameters.Clear();
			return rcdsttemp;
		}

				

			
		private bool checkViewExists(int viewid)
		{
			int count = 0;
			expdata.sqlexecute.Parameters.AddWithValue("@viewid",viewid);
			expdata.sqlexecute.Parameters.AddWithValue("@employeeid",employeeid);
			strsql = "select count(*) from views where viewid = @viewid and employeeid = @employeeid";
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
			
				
					
				
		public void createAvailableTreeStructure(ref Infragistics.WebUI.UltraWebNavigator.UltraWebTree tree, int viewid)
		{
			Infragistics.WebUI.UltraWebNavigator.Node curnode;
			int parentid = 0;
			int viewgroupid;
			string groupname = "";
			System.Data.SqlClient.SqlDataReader reader;

			expdata.sqlexecute.Parameters.AddWithValue("@viewid",viewid);
			expdata.sqlexecute.Parameters.AddWithValue("@employeeid",employeeid);
			
            strsql = "select distinct viewgroups_base.viewgroupid, viewgroups_base.groupname, viewgroups_base.parentid, viewgroups_base.level from viewgroups_base inner join [fields] on fields_base.viewgroupid = viewgroups_base.viewgroupid where viewgroups_base.viewgroupid = 13 or (normalview = 1 and idfield = 0 and fieldid not in (select fieldid from views where viewid = @viewid and employeeid = @employeeid)) ";

		    using (reader = expdata.GetReader(strsql))
		    {
		        expdata.sqlexecute.Parameters.Clear();

		        while (reader.Read())
		        {
		            if (reader.IsDBNull(reader.GetOrdinal("parentid")) == false)
		            {
		                parentid = reader.GetInt32(reader.GetOrdinal("parentid"));
		            }
		            else
		            {
		                parentid = 0;
		            }

		            groupname = reader.GetString(reader.GetOrdinal("groupname"));
		            viewgroupid = reader.GetInt32(reader.GetOrdinal("viewgroupid"));

		            if (parentid == 0)
		            {
		                curnode = tree.Nodes.Add(groupname, "head" + viewgroupid);
		                curnode.CheckBox = Infragistics.WebUI.UltraWebNavigator.CheckBoxes.False;
		            }
		            else
		            {
		                curnode = tree.Find("head" + parentid);
		                curnode = curnode.Nodes.Add(groupname, "head" + viewgroupid);
		                curnode.CheckBox = Infragistics.WebUI.UltraWebNavigator.CheckBoxes.False;
		            }
		        }

		        reader.Close();
		    }
		}

        public sViewInfo getModifiedViews(DateTime date, int employeeid)
        {
            sViewInfo viewinfo = new sViewInfo();
            cFields clsfields = new cFields(accountid);
            int fieldid;
            List<int> viewids = new List<int>();
            List<cUserView> userviews = new List<cUserView>();
            List<int> defaultviewfields = new List<int>();
            List<int> printviewfields = new List<int>();

            System.Data.SqlClient.SqlDataReader reader;

            expdata.sqlexecute.Parameters.AddWithValue("@createdon", date);
            expdata.sqlexecute.Parameters.AddWithValue("@employeeid", employeeid);

            strsql = "SELECT viewid FROM [views] WHERE createdon > @createdon AND employeeid = @employeeid";
            int viewid = 0;

            using (reader = expdata.GetReader(strsql))
            {
                while (reader.Read())
                {
                    viewid = reader.GetInt32(reader.GetOrdinal("viewid"));

                    if (!viewids.Contains(viewid))
                    {
                        viewids.Add(viewid);
                    }
                }

                reader.Close();
            }

            expdata.sqlexecute.Parameters.Clear();

            SortedList<int, List<int>> userfields = new SortedList<int, List<int>>();

            foreach (int id in viewids)
            {
                List<int> fieldids = new List<int>();

                expdata.sqlexecute.Parameters.AddWithValue("@viewid", id);
                expdata.sqlexecute.Parameters.AddWithValue("@employeeid", employeeid);
                strsql = "SELECT fieldid FROM [views] WHERE viewid = @viewid AND employeeid = @employeeid";

                using (reader = expdata.GetReader(strsql))
                {
                    while (reader.Read())
                    {
                        fieldids.Add(reader.GetInt32(reader.GetOrdinal("fieldid")));
                    }

                    reader.Close();
                }

                userfields.Add(id, fieldids);
               
                expdata.sqlexecute.Parameters.Clear();
            }

            expdata.sqlexecute.Parameters.AddWithValue("@createdon", date);
            strsql = "SELECT * FROM default_views WHERE createdon > @createdon;";

            using (reader = expdata.GetReader(strsql))
            {
                while (reader.Read())
                {
                    fieldid = reader.GetInt32(reader.GetOrdinal("fieldid"));
                    defaultviewfields.Add(fieldid);
                }

                reader.Close();
            }

            strsql = "SELECT * FROM print_views WHERE createdon > @createdon;";

            using (reader = expdata.GetReader(strsql))
            {
                while (reader.Read())
                {
                    fieldid = reader.GetInt32(reader.GetOrdinal("fieldid"));
                    printviewfields.Add(fieldid);
                }

                reader.Close();
            }

            viewinfo.lstviews = userfields;
            viewinfo.lstdefviewfields = defaultviewfields;
            viewinfo.lstprintviewfields = printviewfields;

            return viewinfo;
        }


		
	}


}
