using System;
using System.Collections.Generic;
using System.Collections;
using ExpensesLibrary;
using expenses.Old_App_Code;
using System.Web.Caching;
using expenses.Old_App_Code.admin;
using SpendManagementLibrary;
namespace expenses
{
	/// <summary>
	/// Summary description for cReasons.
	/// Caches
	/// [reasonslist + accountid] = SortedList collection of reasons
	/// [reasonsdd + accountid] = string of reasons dropdown
	/// </summary>
	public class cReasons
	{
        int nAccountid;
        System.Web.Caching.Cache Cache = System.Web.HttpRuntime.Cache;
        System.Collections.SortedList list;
        string strsql;

		public cReasons(int accountid)
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

        
        public System.Collections.SortedList CacheList()
		{
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            DateTime createdon, modifiedon;
            int createdby, modifiedby;
			int reasonid = 0;
			string reason = "";
			string description = "";
            string accountcodevat, accountcodenovat;
			cReason curreason;
			System.Data.SqlClient.SqlDataReader reader;
			System.Collections.SortedList list = new System.Collections.SortedList();

            cFilterRules clsrules = new cFilterRules(accountid);

            strsql = "SELECT     reasonid, reason, description, accountcodevat, accountcodenovat, CreatedOn, CreatedBy, ModifiedOn, ModifiedBy FROM dbo.reasons";
            expdata.sqlexecute.CommandText = strsql;
            SqlCacheDependency dep = new SqlCacheDependency(expdata.sqlexecute);

            reader = expdata.GetReader(strsql);
			while (reader.Read())
			{
				reasonid = reader.GetInt32(reader.GetOrdinal("reasonid"));
				reason = reader.GetString(reader.GetOrdinal("reason"));
				if (reader.IsDBNull(reader.GetOrdinal("description")) == false)
				{
					description = reader.GetString(reader.GetOrdinal("description"));
				}
				else
				{
					description = "";
				}
                if (reader.IsDBNull(reader.GetOrdinal("accountcodevat")) == false)
                {
                    accountcodevat = reader.GetString(reader.GetOrdinal("accountcodevat"));
                }
                else
                {
                    accountcodevat = "";
                }
                if (reader.IsDBNull(reader.GetOrdinal("accountcodenovat")) == false)
                {
                    accountcodenovat = reader.GetString(reader.GetOrdinal("accountcodenovat"));
                }
                else
                {
                    accountcodenovat = "";
                }

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
                curreason = new cReason(accountid, reasonid, reason, description, accountcodevat, accountcodenovat, createdon, createdby, modifiedon, modifiedby);
				list.Add(reasonid, curreason);
			}
			reader.Close();
			
			expdata.sqlexecute.Parameters.Clear();
			Cache.Insert("reasonslist" + accountid,list, dep, System.Web.Caching.Cache.NoAbsoluteExpiration, System.Web.Caching.Cache.NoSlidingExpiration, System.Web.Caching.CacheItemPriority.NotRemovable, null);
			return list;
		}

        public List<System.Web.UI.WebControls.ListItem> CreateDropDown()
        {
            cReason reason;
            SortedList sorted = sortList();
            List<System.Web.UI.WebControls.ListItem> items = new List<System.Web.UI.WebControls.ListItem>();
            
            items.Add(new System.Web.UI.WebControls.ListItem("","0"));
            for (int i = 0; i < sorted.Count; i++)
            {
                reason = (cReason)sorted.GetByIndex(i);
                
                items.Add(new System.Web.UI.WebControls.ListItem(reason.reason, reason.reasonid.ToString()));
            }            
            
            return items;


        }

        private SortedList sortList()
        {
            SortedList sorted = new SortedList();
            cReason reason;

            for (int i = 0; i < list.Count; i++)
            {
                reason = (cReason)list.GetByIndex(i);
                sorted.Add(reason.reason, reason);
            }
            return sorted;
        }
		public void InitialiseData()
		{
			list = (System.Collections.SortedList)Cache["reasonslist" + accountid];
			if (list == null)
			{
				list = CacheList();
			}
			
			

			
		}

		public System.Data.DataTable getGrid()
		{
			System.Data.DataTable reasons = new System.Data.DataTable();
			
			object[] values;
			
			reasons.Columns.Add("reasonid",System.Type.GetType("System.Int32"));
			reasons.Columns.Add("reason",System.Type.GetType("System.String"));
			reasons.Columns.Add("description", System.Type.GetType("System.String"));

			cReason clsreason;

			System.Collections.IDictionaryEnumerator enumer = list.GetEnumerator();

			while (enumer.MoveNext())
			{
				clsreason = (cReason)enumer.Value;
				values = new object[3];
				values[0] = clsreason.reasonid;
				values[1] = clsreason.reason;
				values[2] = clsreason.description;
				reasons.Rows.Add(values);
			}
			
			return reasons;
			
		}

        public byte addReason(string reason, string description, string accountcodevat, string accountcodenovat, int userid)
		{
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));

            if (exists(reason, 0, 0) == true)
            {
                return 1;
            }

            int reasonid;
            
            
            expdata.addParameter("@reasonid", 0);
            expdata.addParameter("@reason", reason);
            expdata.addParameter("@description", description);
            expdata.addParameter("@accountcodevat", accountcodevat);
            expdata.addParameter("@accountcodenovat", accountcodenovat);
            expdata.addParameter("@date", DateTime.Now.ToUniversalTime());
            expdata.addParameter("@userid", userid);
            reasonid = expdata.executeSaveCommand("saveReason");
			
			

			cAuditLog clsaudit = new cAuditLog(accountid, userid);
			clsaudit.addRecord("Reason Categories",reason);

			return 0;
		}

		
		public bool alreadyExists(string reason, bool update, int id)
		{
			bool exists = false;
			cReason clsreason;

			System.Collections.IDictionaryEnumerator enumer = list.GetEnumerator();
			while (enumer.MoveNext())
			{
				clsreason = (cReason)enumer.Value;
				if (clsreason.reason == reason)
				{
					if (update == false || (update == true && clsreason.reasonid != id))
					{
						exists = true;
					}
					break;
				}
			}

			return exists;

		}

		public byte updateReason(int reasonid, string reason, string description, string accountcodevat, string accountcodenovat, int userid)
		{
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));

			if (exists(reason,reasonid,2) == true)
			{
				return 1;
			}

            expdata.addParameter("@reasonid", reasonid);
            expdata.addParameter("@reason", reason);
            expdata.addParameter("@description", description);
            expdata.addParameter("@accountcodevat", accountcodevat);
            expdata.addParameter("@accountcodenovat", accountcodenovat);
            expdata.addParameter("@date", DateTime.Now.ToUniversalTime());
            expdata.addParameter("@userid", userid);
            expdata.executeSaveCommand("saveReason");

            cAuditLog clsaudit = new cAuditLog(accountid, userid);
            cReason reqreason = getReasonById(reasonid);

			if (reqreason.reason != reason)
			{
				clsaudit.editRecord(reason, "Reason","Reason Categories",reqreason.reason,reason);
			}
			if (reqreason.description != description)
			{
				clsaudit.editRecord(reason, "Description","Reason Categories",reqreason.description, description);
			}

			return 0;
		}

		public void deleteReason(int reasonid)
		{
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));

            expdata.sqlexecute.Parameters.AddWithValue("@reasonid", reasonid);
            expdata.ExecuteProc("deleteReason");
            

			cAuditLog clsaudit = new cAuditLog();
			cReason clsreason = getReasonById(reasonid);
			clsaudit.deleteRecord("Reason Categories", clsreason.reason);
		}

		public cReason getReasonById(int reasonid)
		{
			return (cReason)list[reasonid];
		}

        public cReason getReasonByString(string reason)
        {
            cReason reqreason = null;
            foreach (cReason clsreason in list.Values)
            {
                if (clsreason.reason == reason)
                {
                    reqreason = clsreason;
                    break;
                }
            }
            return reqreason;
        }

		public string[] getArray()
		{
			cReason reqreason;
			string[] reasons = new string[list.Count];
			int i;

			for (i = 0; i < list.Count; i++)
			{
				reqreason = (cReason)list.GetByIndex(i);
				reasons[i] = reqreason.reason;
			}
			return reasons;
		}
		private System.Text.StringBuilder CacheStringDropDown()
		{
			
			System.Web.Caching.CacheDependency dep;
			String[] dependency;
			dependency = new string[1];
			dependency[0] = "reasonsdependency" + accountid;
			dep = new System.Web.Caching.CacheDependency(null,dependency);
			
			System.Collections.SortedList sortedlst = new System.Collections.SortedList();
			System.Text.StringBuilder output = new System.Text.StringBuilder();
			cReason curreason;

			int i = 0;
			output.Append("<option value=\"0\"></option>");
			for (i = 0; i < list.Count; i++)
			{
				curreason = (cReason)list.GetByIndex(i);
				sortedlst.Add(curreason.reason,curreason.reasonid);
			}

			for (i = 0; i < sortedlst.Count; i++)
			{
				output.Append("<option value=\"" + sortedlst.GetByIndex(i) + "\"");
				
				output.Append(">" + (string)sortedlst.GetKey(i) + "</option>");
				
			}

			Cache.Insert("reasonsdd" + accountid,output,dep);
			return output;
		}
		public System.Text.StringBuilder CreateStringDropDown()
		{
			System.Text.StringBuilder output;
			if (Cache["reasonsdd" + accountid] == null)
			{
				output = CacheStringDropDown();
			}
			else
			{
				output = (System.Text.StringBuilder)Cache["reasonsdd" + accountid];
			}
			

			return output;
		}

		
		private bool exists (string reason, int reasonid, int action)
		{
			int i;
			cReason reqreason;
			for (i = 0; i < list.Count; i++)
			{
				reqreason = (cReason)list.GetByIndex(i);
				if (action == 2)
				{
					if (reqreason.reason.ToLower() == reason.ToLower() && reasonid != reqreason.reasonid)
					{
						return true;
					}
				}
				else
				{
					if (reqreason.reason.ToLower() == reason.ToLower())
					{
						return true;
					}
				}
			}

			return false;
		}

        public ArrayList getModifiedReasons(DateTime date)
        {
            ArrayList lst = new ArrayList();
            foreach (cReason val in list.Values)
            {
                if (val.createdon > date || val.modifiedon > date)
                {
                    lst.Add(val);
                }
            }
            return lst;
        }

        public int count
        {
            get { return list.Count; }
        }

        public ArrayList getReasonIds()
        {
            ArrayList ids = new ArrayList();
            foreach (cReason val in list.Values)
            {
                ids.Add(val.reasonid);
            }
            return ids;
        }
	}


	

	
}
