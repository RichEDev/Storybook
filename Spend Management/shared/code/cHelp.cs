using System;
using System.Configuration;
using System.Linq;
using System.Web.Caching;
using System.Data;
using System.Collections.Generic;
using SpendManagementLibrary;


namespace Spend_Management
{
	/// <summary>
	/// Summary description for cHelp.
	/// </summary>
	public class cHelp
	{

		public SortedList<Guid, cHelpItem> list;
		
		public System.Web.Caching.Cache Cache = (System.Web.Caching.Cache)System.Web.HttpContext.Current.Cache;
        private int nAccountid;

		string strsql;

        public cHelp()
        {
            InitialiseGeneralData();
        }

        private void InitialiseGeneralData()
        {
            list = (SortedList<Guid, cHelpItem>)Cache["help"];
            if (list == null)
            {
                list = CacheGeneralList();
            }
        }

        private SortedList<Guid, cHelpItem> CacheGeneralList()
        {
            DBConnection expdata = new DBConnection(ConfigurationManager.ConnectionStrings["metabase"].ConnectionString);
            SortedList<Guid, cHelpItem> list = new SortedList<Guid, cHelpItem>();            
            Guid tooltipID;
            string helptext, description, page, tooltipArea;
            int? moduleID;
            cHelpItem reqhelp;
            System.Data.SqlClient.SqlDataReader reader;

            strsql = "select page, description, helptext, tooltipID, tooltipArea, moduleID from dbo.help_text";
            expdata.sqlexecute.CommandText = strsql;
            if (GlobalVariables.GetAppSettingAsBoolean("EnableBrokers"))
            {
                var dep = new SqlCacheDependency(expdata.sqlexecute);
                Cache.Insert("help", list, dep, System.Web.Caching.Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes((int)Caching.CacheTimeSpans.Permanent), CacheItemPriority.Default, null);
            }

            using (reader = expdata.GetReader(strsql))
            {
                while (reader.Read())
                {                    
                    if (reader.IsDBNull(reader.GetOrdinal("page")) == true)
                    {
                        page = "";
                    }
                    else
                    {
                        page = reader.GetString(reader.GetOrdinal("page"));
                    }
                    if (reader.IsDBNull(reader.GetOrdinal("helptext")) == true)
                    {
                        helptext = "";
                    }
                    else
                    {
                        helptext = reader.GetString(reader.GetOrdinal("helptext"));
                        //helptext = helptext.Replace("'", "\\'").Replace("\\\\'", "\\'");
                        //helptext = helptext.Replace("\"", "&quot;");
                    }

                    tooltipArea = reader.GetString(reader.GetOrdinal("tooltipArea"));
                    tooltipID = reader.GetGuid(reader.GetOrdinal("tooltipID"));

                    if (reader.IsDBNull(reader.GetOrdinal("moduleID")) == false)
                    {
                        moduleID = reader.GetInt32(reader.GetOrdinal("moduleID"));
                    }
                    else
                    {
                        moduleID = null;
                    }

                    description = reader.GetString(reader.GetOrdinal("description"));                    

                    reqhelp = new cHelpItem(page, description, helptext, tooltipID, tooltipArea, moduleID);

                    list.Add(tooltipID, reqhelp);
                }

                reader.Close();
            }

            return list;
        }
		public cHelp(int accountid)
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
        private void InitialiseData()
		{
            list = (SortedList<Guid, cHelpItem>)Cache["help" + accountid];
			if (list == null)
			{
				list = CacheList();
			}
		}

        /// <summary>
        /// Force the cache to reset
        /// </summary>
        private void ResetCache()
        {
            Cache.Remove("help" + accountid);
            list = null;
            InitialiseData();
        }

        private SortedList<Guid, cHelpItem> CacheList()
		{
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            SortedList<Guid, cHelpItem> list = new SortedList<Guid, cHelpItem>();			
            Guid tooltipID;
			string helptext, description, page, tooltipArea;
            int? moduleID;
			cHelpItem reqhelp;
			System.Data.SqlClient.SqlDataReader reader;

            AggregateCacheDependency aggdep = null;
            if (GlobalVariables.GetAppSettingAsBoolean("EnableBrokers"))
            {
                DBConnection metadata =
                    new DBConnection(ConfigurationManager.ConnectionStrings["metabase"].ConnectionString);
                strsql = "select page, description, helptext, tooltipID, tooltipArea, moduleID from dbo.help_text";
                metadata.sqlexecute.CommandText = strsql;
                SqlCacheDependency metadep = metadata.CreateSQLCacheDependency(strsql, new SortedList<string, object>());

                DBConnection depdata = new DBConnection(cAccounts.getConnectionString(accountid));
                strsql =
                    "select page, description, helptext, tooltipID, tooltipArea, moduleID from dbo.customised_help_text";
                depdata.sqlexecute.CommandText = strsql;
                SqlCacheDependency dep = depdata.CreateSQLCacheDependency(strsql, new SortedList<string, object>());
                aggdep = new AggregateCacheDependency();
                aggdep.Add(new CacheDependency[] {metadep, dep});
            }
            strsql = "select page, description, helptext, tooltipID, tooltipArea, moduleID from dbo.help_text";

            using (reader = expdata.GetReader(strsql))
            {
                while (reader.Read())
                {
                    page = reader.IsDBNull(reader.GetOrdinal("page")) ? string.Empty : reader.GetString(reader.GetOrdinal("page"));

                    helptext = reader.IsDBNull(reader.GetOrdinal("helptext")) ? string.Empty : reader.GetString(reader.GetOrdinal("helptext"));

                    tooltipArea = reader.GetString(reader.GetOrdinal("tooltipArea"));
                    tooltipID = reader.GetGuid(reader.GetOrdinal("tooltipID"));

                    description = reader.GetString(reader.GetOrdinal("description"));

                    if (reader.IsDBNull(reader.GetOrdinal("moduleID")) == false)
                    {
                        moduleID = reader.GetInt32(reader.GetOrdinal("moduleID"));
                    }
                    else
                    {
                        moduleID = null;
                    }

                    reqhelp = new cHelpItem(page, description, helptext, tooltipID, tooltipArea, moduleID);

                    list.Add(tooltipID, reqhelp);
                }

                reader.Close();
            }

            if (aggdep != null)
            {
                Cache.Insert("help" + this.accountid, list, aggdep, Cache.NoAbsoluteExpiration,
                    TimeSpan.FromMinutes((int) Caching.CacheTimeSpans.Permanent), CacheItemPriority.Default, null);
            }

			return list;
		}

        public cHelpItem GetToolTipByIntAndArea(string contextKey)
        {
            string[] temp = contextKey.Split(',');

            Guid tooltipId = Guid.Parse(temp[0]);

            string tooltipArea = temp[1];

            return
                this.list.Values.FirstOrDefault(
                    help => help.ToolTipID == tooltipId && tooltipArea.ToLower() == help.ToolTipArea.ToLower());
        }

		public cHelpItem getHelpById(Guid tooltipID)
		{
            cHelpItem item = null;
            list.TryGetValue(tooltipID, out item);
            return item;
		}

        public DataTable getGrid(int activeModuleID)
        {
            var tbl = new DataTable();            
            tbl.Columns.Add("page", typeof(string));
            tbl.Columns.Add("description", typeof(string));
            tbl.Columns.Add("text", typeof(string));
            tbl.Columns.Add("tooltipID", typeof(Guid));
            tbl.Columns.Add("tooltipArea", typeof(string));

            foreach (cHelpItem item in this.list.Values)
            {
                if (item.ModuleID.HasValue == false || (item.ModuleID.HasValue && activeModuleID == item.ModuleID.Value))
                {
                    if (item.page.ToLower() != "logon" && item.page.ToLower() != "forgotten password" && item.page.ToLower() != "self registration")
                    {
                        tbl.Rows.Add(
                            new object[]
                                {                                    
                                    item.page, item.description, item.helptext, item.ToolTipID,
                                    item.ToolTipArea
                                });
                    }
                }
            }
            return tbl;
        }

        public bool containsTip(Guid id)
        {
            cHelpItem item;


            list.TryGetValue(id, out item);
            if (item == null)
            {
                return false;
            }

            if (item.helptext.Trim().Length == 0)
            {
                return false;
            }

            return true;
        }

        public void saveTooltip(Guid tooltipID, string text)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));

            expdata.sqlexecute.Parameters.AddWithValue("@text", text);
            expdata.sqlexecute.Parameters.AddWithValue("@tooltipID", tooltipID);
            CurrentUser currentUser = cMisc.GetCurrentUser();
            expdata.sqlexecute.Parameters.AddWithValue("@CUemployeeID", currentUser.EmployeeID);
            if (currentUser.isDelegate == true)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@CUdelegateID", currentUser.Delegate.EmployeeID);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@CUdelegateID", DBNull.Value);
            }
            expdata.ExecuteProc("saveTooltip");
            expdata.sqlexecute.Parameters.Clear();
        }

        public void restoreTooltip(Guid tooltipID)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));

            expdata.sqlexecute.Parameters.AddWithValue("@tooltipID", tooltipID);
            CurrentUser currentUser = cMisc.GetCurrentUser();
            expdata.sqlexecute.Parameters.AddWithValue("@CUemployeeID", currentUser.EmployeeID);
            if (currentUser.isDelegate == true)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@CUdelegateID", currentUser.Delegate.EmployeeID);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@CUdelegateID", DBNull.Value);
            }
            expdata.ExecuteProc("restoreDefaultTooltip");
            expdata.sqlexecute.Parameters.Clear();
        }

        /// <summary>
        /// Replaces line breaks from the tooltip help text when loading for edit from the DB.
        /// </summary>
        public string ReplaceLineBreak(string inputText)
        {
            string outputText;
            outputText = inputText.Replace("<br />", System.Environment.NewLine);
            return outputText;
        }

        /// <summary>
        /// Replaces environment NewLine (\r\n) from the tooltip help text when saving back to the DB. 
        /// </summary>
        public string ReplaceNewLine(string inputText)
        {
            string outputText;
            outputText = inputText.Replace(System.Environment.NewLine, "<br />");
            return outputText;
        }
	}

	public class cHelpItem
	{		
        private string sPage;
		private string sDescription;
		private string sHelptext;
        private Guid gToolTipID;
        private string sTooltipArea;

		public cHelpItem (string page, string description, string helptext, Guid toolTipID, string tooltipArea, int? moduleID)
		{			
            sPage = page;
			sDescription = description;
			sHelptext = helptext; //.Replace("'","\\'");
            gToolTipID = toolTipID;
            sTooltipArea = tooltipArea;
            this.ModuleID = moduleID;
		}

		#region properties
        public string page
        {
            get { return sPage; }
        }
		public string description
		{
			get {return sDescription;}
		}
		public string helptext
		{
			get {return sHelptext;}
		}

        public Guid ToolTipID
        {
            get { return gToolTipID; }
        }

        public string ToolTipArea
        {
            get { return sTooltipArea; }
        }

        /// <summary>
        /// Which module this tooltip belongs, null if it is shared
        /// </summary>
        public int? ModuleID { get; set; }
		#endregion
	}
}
