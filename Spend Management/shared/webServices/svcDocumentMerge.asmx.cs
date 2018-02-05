using System.Web;
using SpendManagementLibrary.DocumentMerge;

namespace Spend_Management
{
    #region Using Directives

    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Configuration;
    using System.Data;
    using System.Data.SqlClient;
    using System.Globalization;
    using System.Linq;
    using System.Web.Script.Serialization;
    using System.Web.Script.Services;
    using System.Web.Services;
    using System.Web.UI.WebControls;

    using SpendManagementLibrary;
    using SpendManagementLibrary.Helpers;

    using Spend_Management.shared.code;

    #endregion

    /// <summary>
    ///     Summary description for svcDocumentMerge
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [ToolboxItem(false)]
    [ScriptService]
    public class svcDocumentMerge : WebService
    {
        private const string SortedIndicator = @" - (Sorted)";

        #region Public Methods and Operators

        /// <summary>
        /// 
        /// </summary>
        /// <param name="projectid"></param>
        /// <param name="sGrid"></param>
        /// <param name="defaultConfiguration"></param>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public object[] ProcessGridRequest(int projectid, string sGrid, string defaultConfiguration = "")
        {
            string[] arrGridInfo = sGrid.Split(',');
            var oRet = new List<object>();
            string[] gridData;
            int defaultId;
            switch (arrGridInfo[0])
            {
                case "reports":
                    gridData = this.GetReportSourcesGrid(projectid);
                    oRet.Add(gridData[0]);
                    oRet.Add(gridData[1]);
                    oRet.Add(gridData[2]);
                    oRet.Add(gridData[3]);
                    break;

                case "documentconfigs":
                    defaultId = -1;
                    int.TryParse(defaultConfiguration, out defaultId);
                    gridData = this.GetDocumentGroupingConfigurationsGrid(projectid, Convert.ToInt32(defaultId));
                    oRet.Add(gridData[0]);
                    oRet.Add(gridData[1]);
                    oRet.Add(gridData[2]);
                    break;
            }

            return oRet.ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="projectid"></param>
        /// <param name="reportid"></param>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public object[] AddReportSource(int projectid, Guid reportid)
        {
            CurrentUser curUser = cMisc.GetCurrentUser();
            if (!curUser.CheckAccessRole(AccessRoleType.Add, SpendManagementElement.DocumentConfigurations, true))
            {
                return new object[] { 0, Guid.Empty, "", "" };
            }

            var db = new DBConnection(cAccounts.getConnectionString(curUser.AccountID));

            db.sqlexecute.Parameters.Add("@ReturnId", SqlDbType.Int);
            db.sqlexecute.Parameters["@ReturnId"].Direction = ParameterDirection.ReturnValue;
            db.sqlexecute.Parameters.AddWithValue("@projectId", projectid);
            db.sqlexecute.Parameters.AddWithValue("@reportSourceId", reportid);
            db.sqlexecute.Parameters.AddWithValue("@userId", curUser.EmployeeID);
            db.sqlexecute.Parameters.AddWithValue("@CUemployeeID", curUser.EmployeeID);
            if (curUser.isDelegate)
            {
                db.sqlexecute.Parameters.AddWithValue("@CUdelegateID", curUser.Delegate.EmployeeID);
            }
            else
            {
                db.sqlexecute.Parameters.AddWithValue("@CUdelegateID", DBNull.Value);
            }
            db.ExecuteProc("saveDocumentMergeSource");

            var mergesourceid = (int)db.sqlexecute.Parameters["@ReturnId"].Value;

            db.sqlexecute.Parameters.Clear();

            // cache dependency should refresh class
            var clsreports =
                (IReports)
                    Activator.GetObject(
                        typeof(IReports),
                        ConfigurationManager.AppSettings["ReportsServicePath"] + "/reports.rem");

            cReport rep = clsreports.getReportById(curUser.AccountID, reportid);

            return new object[] { mergesourceid, rep.reportid, rep.reportname, rep.description };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mergesourceid"></param>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public int DeleteDocumentMergeSource(int mergesourceid)
        {
            CurrentUser curUser = cMisc.GetCurrentUser();
            if (!curUser.CheckAccessRole(AccessRoleType.Delete, SpendManagementElement.DocumentConfigurations, true))
            {
                return 0;
            }

            var db = new DBConnection(cAccounts.getConnectionString(curUser.AccountID));
            db.sqlexecute.Parameters.AddWithValue("@mergeSourceId", mergesourceid);
            db.sqlexecute.Parameters.AddWithValue("@CUemployeeID", curUser.EmployeeID);
            if (curUser.isDelegate)
            {
                db.sqlexecute.Parameters.AddWithValue("@CUdelegateID", curUser.Delegate.EmployeeID);
            }
            else
            {
                db.sqlexecute.Parameters.AddWithValue("@CUdelegateID", DBNull.Value);
            }
            db.ExecuteProc("deleteDocumentMergeSource");

            db.sqlexecute.Parameters.Clear();
            return mergesourceid;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="mergeprojectid"></param>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public object[] DeleteMergeProject(int mergeprojectid)
        {
            CurrentUser curUser = cMisc.GetCurrentUser();
            if (!curUser.CheckAccessRole(AccessRoleType.Delete, SpendManagementElement.DocumentConfigurations, true))
            {
                return new object[] { 0, "You do not have permission to perform this operation." };
            }

            var mergeProjects = new DocumentMergeProject(curUser);

            string sResult = mergeProjects.DeleteProject(mergeprojectid, curUser.EmployeeID);
            return new object[] { mergeprojectid, sResult };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="documentId"></param>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public int DeleteTemplate(int documentId)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();

            if (!currentUser.CheckAccessRole(AccessRoleType.Delete, SpendManagementElement.DocumentConfigurations, true))
            {
                return 0;
            }

            using (var connection = new DatabaseConnection(cAccounts.getConnectionString(currentUser.AccountID)))
            {
                connection.sqlexecute.Parameters.AddWithValue("@documentid", documentId);
                connection.sqlexecute.Parameters.AddWithValue("@CUemployeeID", currentUser.EmployeeID);
                if (currentUser.isDelegate)
                {
                    connection.sqlexecute.Parameters.AddWithValue("@CUdelegateID", currentUser.Delegate.EmployeeID);
                }
                else
                {
                    connection.sqlexecute.Parameters.AddWithValue("@CUdelegateID", DBNull.Value);
                }
                connection.ExecuteProc("deleteDocumentTemplate");
            }

            return 0;
        }

        /// <summary>
        /// Delete Document Filters Configuration Columns by the configurationId
        /// </summary>
        /// <param name="configurationId"></param>
        /// <returns></returns>
        ///  
        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public int DeleteDocumentFiltersConfigurationColumnsByConfigurationId(int configurationId)
        {
            CurrentUser user = cMisc.GetCurrentUser();
            var documentMergeProject = new DocumentMergeProject(user);
            return documentMergeProject.DeleteDocumentFiltersConfigurationColumnsByConfigurationId(configurationId);
        }

        /// <summary>
        /// Delete Document Sorting Configuration Columns by the configurationId
        /// </summary>
        /// <param name="configurationId"></param>
        /// <returns></returns>
        ///  
        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public int DeleteDocumentSortingConfigurationColumnsByConfigurationId(int configurationId)
        {
            CurrentUser user = cMisc.GetCurrentUser();
            var documentMergeProject = new DocumentMergeProject(user);
            return documentMergeProject.DeleteDocumentSortingConfigurationColumnsByConfigurationId(configurationId);
        }

        /// <summary>
        ///  Deletes all report sorting configurations for a configurationId
        /// </summary>
        /// <param name="configurationId"></param>
        /// <returns></returns>
        /// 
        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public int DeleteDocumentReportSortingConfigurationColumnsByConfigurationId(int configurationId)
        {
            CurrentUser user = cMisc.GetCurrentUser();
            var documentMergeProject = new DocumentMergeProject(user);
            return documentMergeProject.DeleteDocumentReportSortingConfigurationColumnsByConfigurationId(configurationId);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="reportCategoryId"></param>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public List<ListItem> GetAvailableReports(int projectId, Guid reportCategoryId)
        {
            CurrentUser curUser = cMisc.GetCurrentUser();
            var reportlist = new Dictionary<Guid, string>();
            var db = new DBConnection(cAccounts.getConnectionString(curUser.AccountID));

            string strsql = "select reportid, reportname from reports ";
            if (reportCategoryId != Guid.Empty)
            {
                strsql += "where folderid = @folderid ";
                db.sqlexecute.Parameters.AddWithValue("@folderid", reportCategoryId);
            }
            else
            {
                strsql += "where folderid is null ";
            }
            strsql +=
                "and reportid not in (select reportid from document_mergesources where mergeprojectid = @mergeProjectId)";
            strsql += "order by reportname";
            db.sqlexecute.Parameters.AddWithValue("@mergeProjectId", projectId);

            using (SqlDataReader reader = db.GetReader(strsql))
            {
                while (reader.Read())
                {
                    Guid repId = reader.GetGuid(reader.GetOrdinal("reportid"));
                    string repname = reader.GetString(reader.GetOrdinal("reportname"));
                    reportlist.Add(repId, repname);
                }
                reader.Close();
            }
            db.sqlexecute.Parameters.Clear();

            return reportlist.Select(i => new ListItem(i.Value, i.Key.ToString())).ToList();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public string[] GetMergeProjectsGrid()
        {
            CurrentUser user = cMisc.GetCurrentUser();
            var projects = new DocumentMergeProject(user);

            var clsgrid = new cGridNew(
                user.AccountID,
                user.EmployeeID,
                "docmergeprojects",
                projects.DocMergeTemplatesGridSql);
            clsgrid.getColumnByName("mergeprojectid").hidden = true;
            clsgrid.getColumnByName("project_name").hidden = false;
            clsgrid.getColumnByName("createddate").hidden = true;
            clsgrid.getColumnByName("createdby").hidden = true;
            clsgrid.KeyField = "mergeprojectid";
            clsgrid.EmptyText = "There are currently no document configurations defined";

            clsgrid.enabledeleting = user.CheckAccessRole(
                AccessRoleType.Delete,
                SpendManagementElement.DocumentConfigurations,
                false);
            clsgrid.deletelink = "javascript:SEL.DocMerge.DeleteProject({mergeprojectid});";
            clsgrid.enableupdating = user.CheckAccessRole(
                AccessRoleType.Edit,
                SpendManagementElement.DocumentConfigurations,
                false);
            clsgrid.editlink = "admindocmerge.aspx?mpid={mergeprojectid}";

            clsgrid.CssClass = "datatbl";
            clsgrid.PagerPosition = PagerPosition.TopAndBottom;
            clsgrid.showfooters = false;
            clsgrid.showheaders = true;
            clsgrid.enabledeleting = user.CheckAccessRole(
                AccessRoleType.Delete,
                SpendManagementElement.DocumentConfigurations,
                false);
            clsgrid.enableupdating = user.CheckAccessRole(
                AccessRoleType.Edit,
                SpendManagementElement.DocumentConfigurations,
                false);
            clsgrid.SortedColumn = clsgrid.getColumnByName("project_name");

            return clsgrid.generateGrid();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mergeProjectId"></param>
        /// <param name="mergeRequestNumber"></param>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public object[] GetMergeStatus(int mergeProjectId, Guid mergeRequestNumber)
        {
            var currentUser = cMisc.GetCurrentUser();
            var debugLog = new DebugLogger(currentUser, (HttpContext.Current != null ? HttpContext.Current.Request : null))
                .Log("svcDocumentMerge.GetMergeStatus", String.Empty, mergeProjectId, mergeRequestNumber);

            try
            {
                debugLog.Log("svcDocumentMerge.GetMergeStatus", "Calling TorchMergeState.Get", mergeProjectId, mergeRequestNumber, currentUser.EmployeeID);
                var mergeStatus = TorchMergeState.Get(mergeProjectId, mergeRequestNumber, currentUser);

                debugLog.Log("svcDocumentMerge.GetMergeStatus", "Returning new object[]", mergeProjectId, mergeRequestNumber, mergeStatus.ProgressCount, mergeStatus.Status, mergeStatus.TotalToProcess, mergeStatus.ErrorMessage);
                return new object[]
                {
                    mergeProjectId,
                    mergeRequestNumber,
                    mergeStatus.ProgressCount,
                    mergeStatus.Status,
                    mergeStatus.TotalToProcess,
                    mergeStatus.ErrorMessage
                };
            }
            catch (Exception ex)
            {
                debugLog.Log("svcDocumentMerge.GetMergeStatus", ex);
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public string[] GetReportSourcesGrid(int projectId)
        {
            CurrentUser user = cMisc.GetCurrentUser();
            TorchProject mergeProject;
            cGridNew grid;
            var fields = new cFields(user.AccountID);
            var mergeProjects = new DocumentMergeProject(user);

            if (projectId > 0)
            {
                mergeProject = mergeProjects.GetProjectById(projectId);
                grid = new cGridNew(user.AccountID, user.EmployeeID, "mergesources", mergeProject.GetCurrentReportSql);
            }
            else
            {
                mergeProject = new TorchProject(0, "", "", null, null, null, null, null, user.AccountID, string.Empty);
                grid = new cGridNew(user.AccountID, user.EmployeeID, "mergesources", mergeProject.GetCurrentReportSql);
            }

            grid.addTwoStateEventColumn(
                "groupingsource",
                (cFieldColumn)grid.getColumnByName("groupingsource"),
                false,
                true,
                GlobalVariables.StaticContentLibrary + "/icons/16/new-icons/checkbox_unchecked.png",
                "javascript:SEL.DocMerge.ChangeGroupingReportStatus({mergesourceid});",
                "Click to use report in grouping collection.",
                "Add report source to grouping collection",
                GlobalVariables.StaticContentLibrary + "/icons/16/plain/checkbox.png",
                "javascript:SEL.DocMerge.ChangeGroupingReportStatus({mergesourceid});",
                "Click to remove report from grouping collection.",
                "Remove report source from grouping collection");
            grid.getColumnByName("groupingsource").HeaderText = "Use in Grouping";
            grid.SortedColumn = grid.getColumnByName("reportname");
            grid.KeyField = "mergesourceid";
            grid.getColumnByName("mergesourceid").hidden = true;
            grid.getColumnByName("reportid").hidden = true;
            grid.CssClass = "datatbl";
            grid.EmptyText = "There are currently no reports selected";
            grid.deletelink = "javascript:SEL.DocMerge.DeleteReportSource({mergesourceid});";
            grid.enableupdating = user.CheckAccessRole(AccessRoleType.Edit, SpendManagementElement.DocumentConfigurations, false);
            grid.enablearchiving = user.CheckAccessRole(
                AccessRoleType.Edit,
                SpendManagementElement.DocumentConfigurations,
                false);
            grid.enabledeleting = user.CheckAccessRole(
                AccessRoleType.Delete,
                SpendManagementElement.DocumentConfigurations,
                false);
            grid.addFilter(
                fields.GetFieldByID(new Guid(ReportKeyFields.MergeSources_MergeProjectId)),
                ConditionType.Equals,
                new object[] { projectId },
                new object[] { },
                ConditionJoiner.None);

            var retVals = new List<string> { grid.GridID };
            retVals.AddRange(grid.generateGrid());
            retVals.Add(mergeProject.GetReportSources().Count.ToString(CultureInfo.InvariantCulture));
            return retVals.ToArray();
        }


        /// <summary>
        /// Populates a grid with Document Grouping Configurations
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="defaultConfiguration"></param>
        /// <param name="recordId">The ID of the Entity record.</param>
        /// <param name="entityid">The ID of the GreeenLight entity.</param>
        /// <param name="noDelete">If true, no delete or default column</param>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public string[] GetDocumentGroupingConfigurationsGrid(int projectId, int defaultConfiguration, int recordId = 0,
            int entityid = 0, bool noDelete = false)
        {
            CurrentUser user = cMisc.GetCurrentUser();
            const string sql =
                "select GroupingId, Label, description from [dbo].DocumentGroupingConfigurations";

            var grid = new cGridNew(user.AccountID, user.EmployeeID, "gridMergeDocConfigs", sql);
            if (!noDelete)
            {
                grid.addTwoStateEventColumn(
                 "defaultConfig",
                            (cFieldColumn)grid.getColumnByName("GroupingId"),
                 null,
                 defaultConfiguration,
                 GlobalVariables.StaticContentLibrary + "/icons/16/new-icons/checkbox_unchecked.png",
                "javascript: SEL.DocMerge.DefaultConfigCheckboxClick();",
                 "",
                 "",
                 GlobalVariables.StaticContentLibrary + "/icons/16/plain/checkbox.png",
                 "javascript: SEL.DocMerge.DefaultConfigCheckboxClick();",
                 "This is the default configuration",
                 "This is the default configuration", false);
                grid.getColumnByName("defaultConfig").HeaderText = "Is the default configuration";
                grid.deletelink = "javascript:SEL.DocMerge.DeleteGroupingConfiguration({GroupingId});";
                grid.enabledeleting = user.CheckAccessRole(
                 AccessRoleType.Delete,
                 SpendManagementElement.DocumentConfigurations,
                 false);
            }
            else
            {
                grid.pagesize = 7;
                grid.enabledeleting = false;
                grid.addEventColumn("mergeConfig", "/shared/images/icons/16/plain/documents_gear.png",
                    "javascript:$('.dialogHolder').dialog('close');SEL.DocMerge.PerformMerge(" + projectId + "," +
                    defaultConfiguration + "," + recordId + "," + entityid + ", {GroupingId} );",
                    "Merge using this configuration.", "Merge using this configuration.");
            }

            grid.WhereClause = string.Format("mergeprojectId = '{0}'", projectId);

            grid.SortedColumn = grid.getColumnByName("label");
            grid.KeyField = "GroupingId";
            grid.getColumnByName("GroupingId").hidden = true;
            grid.CssClass = "datatbl";
            grid.EmptyText = "There are currently no document grouping configurations";

            grid.enableupdating = user.CheckAccessRole(
                AccessRoleType.Edit,
                SpendManagementElement.DocumentConfigurations,
                false);
            grid.editlink = "javascript:SEL.DocMerge.LaunchGroupingModal({GroupingId});";

            var retVals = new List<string> { grid.GridID };
            retVals.AddRange(grid.generateGrid());

            return retVals.ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public List<ListItem> GetReportsCategories()
        {
            CurrentUser curUser = cMisc.GetCurrentUser();
            var folders = new Dictionary<Guid, string>();
            var db = new DBConnection(cAccounts.getConnectionString(curUser.AccountID));

            const string SQL =
                "select folderid, employeeid, foldername, personal from report_folders where (personal = 1 and employeeid = @empId) or personal = 0";
            // , reportArea, employeeid
            db.sqlexecute.Parameters.AddWithValue("@empId", curUser.EmployeeID);
            using (SqlDataReader reader = db.GetReader(SQL))
            {
                db.sqlexecute.Parameters.Clear();

                while (reader.Read())
                {
                    Guid folderid = reader.GetGuid(reader.GetOrdinal("folderid"));
                    string folder = reader.GetString(reader.GetOrdinal("foldername"));

                    folders.Add(folderid, folder);
                }
                reader.Close();
            }

            return folders.Select(i => new ListItem(i.Value, i.Key.ToString())).ToList();
        }

        /// <summary>
        /// Save Document Merge Project.
        /// </summary>
        /// <param name="projectid"></param>
        /// <param name="configName"></param>
        /// <param name="configDescription"></param>
        /// <param name="defaultDocumentGroupingId"></param>
        /// <param name="blankTableText"></param>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public int SaveProject(int projectid, string configName, string configDescription, int defaultDocumentGroupingId,
            string blankTableText)
        {
            CurrentUser user = cMisc.GetCurrentUser();
            var mergeProjects = new DocumentMergeProject(user);

            var project = new TorchProject(
                projectid,
                configName.Trim(),
                configDescription.Trim(),
                DateTime.Now,
                user.EmployeeID,
                DateTime.Now,
                user.EmployeeID,
                defaultDocumentGroupingId, user.AccountID,
                blankTableText);

            return mergeProjects.UpdateProject(project, user.EmployeeID);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="projectid"></param>
        /// <param name="configName"></param>
        /// <param name="configDescription"></param>
        /// <param name="defaultDocumentGroupingId"></param>
        /// <param name="torchConfiguration"></param>
        /// <param name="blankTableText"></param>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public int SaveConfiguration(int projectid, string configName, string configDescription,
            int defaultDocumentGroupingId, TorchGroupingConfiguration torchConfiguration, string blankTableText)
        {
            var user = cMisc.GetCurrentUser();
            var newId = this.SaveProject(projectid, configName, configDescription, defaultDocumentGroupingId,
                blankTableText);
            var documentMergeProject = new DocumentMergeProject(user);
            var groupingSources = torchConfiguration.GroupingSources;
            var groupConfigurationId = 0;
            if (torchConfiguration.GroupingConfigurationId == 0)
            {
                groupConfigurationId = SaveGroupingConfiguration(newId, 0, torchConfiguration.ConfigurationLabel,
                    false,
                    torchConfiguration.GroupingColumnsList, torchConfiguration.ConfigurationDescription);
                var newTorchConfiguration = documentMergeProject.GetGroupingConfiguration(newId, groupConfigurationId);
                groupingSources = newTorchConfiguration.GroupingSources;
                torchConfiguration.GroupingConfigurationId = groupConfigurationId;
                if (groupConfigurationId == -1)
                {
                    return groupConfigurationId;
                }
            }
            else
            {
                groupConfigurationId = documentMergeProject.SaveGroupingConfiguration(torchConfiguration.GroupingColumnsList, newId, torchConfiguration.ConfigurationLabel, groupingConfigurationId: torchConfiguration.GroupingConfigurationId, description: torchConfiguration.ConfigurationDescription);
                if (groupConfigurationId == -1)
                {
                    return groupConfigurationId;
                }
            }
            DeleteDocumentFiltersConfigurationColumnsByConfigurationId(torchConfiguration.GroupingConfigurationId);
            DeleteDocumentReportSortingConfigurationColumnsByConfigurationId(torchConfiguration.GroupingConfigurationId);
            DeleteDocumentSortingConfigurationColumnsByConfigurationId(torchConfiguration.GroupingConfigurationId);

            var index = 0;
            if (torchConfiguration.GroupingColumnsList.Count > 0)
            {
                foreach (var torchGroupingFieldFilter in torchConfiguration.FilteringColumns)
                {
                    documentMergeProject.SaveFilteringConfiguration(newId, torchConfiguration.GroupingConfigurationId,
                        index, torchGroupingFieldFilter);
                    index++;
                }
            }


            var reportSortingConfigId = documentMergeProject.SaveReportSortingConfiguration(newId,
                torchConfiguration.GroupingConfigurationId);
            foreach (TorchReportSorting reportSortingConfiguration in torchConfiguration.ReportSortingConfigurations)
            {
                if (groupingSources.Contains(reportSortingConfiguration.ReportName))
                {
                    documentMergeProject.SaveReportColumnSortingConfiguration(reportSortingConfigId,
                        reportSortingConfiguration.ReportName,
                        reportSortingConfiguration.TorchReportSortingColumns.ToArray());
                }
            }

            if (torchConfiguration.GroupingColumnsList.Count > 0)
            {
                index = 0;

                foreach (SortingColumn sortingColumn in torchConfiguration.SortingColumnsList)
                {
                    documentMergeProject.SaveSortingConfiguration(newId, torchConfiguration.GroupingConfigurationId,
                        index,
                        sortingColumn.Name, Convert.ToInt32(sortingColumn.DocumentSortType));
                    index++;
                }
            }

            return torchConfiguration.GroupingConfigurationId;
        }

        /// <summary>
        /// Swaps the value of the bit field that represents whether the report source is to be used in the grouping definition.
        /// </summary>
        /// <param name="mergeSourceId"></param>
        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public void SwapMergeSourceGroupingStatus(int mergeSourceId)
        {
            CurrentUser user = cMisc.GetCurrentUser();
            using (var connection = new DatabaseConnection(cAccounts.getConnectionString(user.AccountID)))
            {
                connection.sqlexecute.Parameters.AddWithValue("mergeSourceId", mergeSourceId);
                connection.ExecuteProc("TorchSwapReportSourceGroupingStatus");
            }
        }

        /// <summary>
        /// Gets the common fields for all the reports marked groupingsource = 1 for the selected report sources.
        /// </summary>
        /// <param name="projectId">The merge project id</param>
        [WebMethod(EnableSession = true)]
        public TorchGroupingConfiguration GetCommonFieldsForGroupingSources(int projectId)
        {
            CurrentUser user = cMisc.GetCurrentUser();
            List<string> groupingFields = TorchProject.GetGroupingFields(projectId, user);
            var result = new TorchGroupingConfiguration(-1, projectId, groupingFields, new List<SortingColumn>(),
              new List<TorchGroupingFieldFilter>(), new List<string>(), new List<TorchReportSorting>());
            return result;
        }

        /// <summary>
        /// Gets the common fields for all the reports marked groupingsource = 1 for the selected report sources.
        /// </summary>
        /// <param name="projectId">The merge project id</param>
        /// <param name="configurationId">The id value for the grouping configuration</param>
        /// <param name="label">The label for the configuration (user defined)</param>
        /// <param name="archived">Sets whether the configuration will be archived</param>
        /// <param name="groupingColumns">A list of the names of the grouping columns in the configuration</param>
        /// <param name="description"></param>
        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public int SaveGroupingConfiguration(
            int projectId,
            int configurationId,
            string label,
            bool archived,
            List<string> groupingColumns,
            string description)
        {
            CurrentUser user = cMisc.GetCurrentUser();
            var documentMergeProject = new DocumentMergeProject(user);
            return documentMergeProject.SaveGroupingConfiguration(
                groupingColumns,
                projectId,
                label,
                archived,
                configurationId,
                description);
        }

        /// <summary>
        // Saves the sorting details for a column
        /// </summary>
        /// <param name="mergeProjectId"></param>
        /// <param name="documentGroupingConfigurationId"></param>
        /// <param name="sequenceOrder"></param>
        /// <param name="sortingColumn"></param>
        /// <param name="sortingOrder"></param>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public int SaveSortingConfiguration(
            int mergeProjectId,
            int documentGroupingConfigurationId,
            int sequenceOrder,
            string sortingColumn,
            int sortingOrder)
        {
            CurrentUser user = cMisc.GetCurrentUser();
            var documentMergeProject = new DocumentMergeProject(user);
            return documentMergeProject.SaveSortingConfiguration(mergeProjectId, documentGroupingConfigurationId,
                sequenceOrder,
                   sortingColumn, sortingOrder);
        }

        /// <summary>
        /// Save a report sorting configuration
        /// </summary>       
        /// <param name="documentGroupingConfigurationId"></param> 
        /// <param name="mergeProjectId"></param>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public int SaveReportSortingConfiguration(
         int documentGroupingConfigurationId,
         int mergeProjectId
        )
        {
            CurrentUser user = cMisc.GetCurrentUser();
            var documentMergeProject = new DocumentMergeProject(user);
            return documentMergeProject.SaveReportSortingConfiguration(mergeProjectId, documentGroupingConfigurationId);
        }

        /// <summary>
        /// Save sorting columns for a report
        /// </summary>
        /// <param name="reportSortingConfigurationId"></param>
        /// <param name="reportName"></param>
        /// <param name="reportColumns"></param>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public int SaveReportColumnSortingConfiguration(
         int reportSortingConfigurationId,
         string reportName,
         string reportColumns
        )
        {

            var jss = new JavaScriptSerializer();
            var reportColumnConfigurations = jss.Deserialize<TorchReportSortingColumn[]>(reportColumns);

            CurrentUser user = cMisc.GetCurrentUser();
            var documentMergeProject = new DocumentMergeProject(user);
            return documentMergeProject.SaveReportColumnSortingConfiguration(reportSortingConfigurationId, reportName,
                reportColumnConfigurations);
        }

        /// <summary>
        /// Archives or unarchives a grouping configuration for this project
        /// </summary>
        /// <param name="projectId">The merge project</param>
        /// <param name="configurationId">The configuration id</param>
        /// <param name="archived">Archived or unarchived</param>
        /// <returns>Status 0 for success</returns>
        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public int ArchiveGroupingConfiguration(int projectId, int configurationId, bool archived = true)
        {
            CurrentUser user = cMisc.GetCurrentUser();
            var documentMergeProject = new DocumentMergeProject(user);
            return documentMergeProject.ArchiveGroupingConfiguration(projectId, configurationId, archived);
        }

        /// <summary>
        /// Deletes a grouping configuration for this project
        /// </summary>
        /// <param name="projectId">The merge project</param>
        /// <param name="groupingConfigurationId">The configuration id</param>
        /// <returns>Status 0 for success</returns>
        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public int DeleteGroupingConfiguration(int projectId, int groupingConfigurationId)
        {
            CurrentUser user = cMisc.GetCurrentUser();
            var documentMergeProject = new DocumentMergeProject(user);
            return documentMergeProject.DeleteGroupingConfiguration(projectId, groupingConfigurationId);
        }

        /// <summary>
        /// Retrieves a list of configuration labels and associated id's
        /// </summary>
        /// <param name="projectId">The merge project</param>
        /// <returns>List of labels and id's</returns>
        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public List<ListItem> GetGroupingConfigurations(int projectId)
        {
            CurrentUser user = cMisc.GetCurrentUser();
            var documentMergeProject = new DocumentMergeProject(user);
            return documentMergeProject.GetGroupingConfigurations(projectId);
        }

        /// <summary>
        /// Retrieves a configuration of grouping / filtering and sorting from a grouping configuration id.
        /// </summary>
        /// <param name="projectId">The merge project</param>
        /// <param name="groupingConfigurationId"></param>
        /// <returns>A TorchGroupingConfiguration object</returns>
        [WebMethod(EnableSession = true)]
        public TorchGroupingConfiguration GetGroupingConfiguration(int projectId, int groupingConfigurationId)
        {
            CurrentUser user = cMisc.GetCurrentUser();
            var documentMergeProject = new DocumentMergeProject(user);
            return documentMergeProject.GetGroupingConfiguration(projectId, groupingConfigurationId, true);
        }

        /// <summary>
        /// Get the Field type, list or precision and fieldid.
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        public object[] GetFieldInfoForFilter(int projectId, string fieldName)
        {
            var user = cMisc.GetCurrentUser();
            var ces = new cCustomEntities(user);

            var fieldId = TorchProject.GetFieldIdFromCommonColumn(projectId, fieldName.Replace("'", string.Empty), user);
            var result = ces.GetFieldInfoForFilter(fieldId.Item1, user.AccountID, user.CurrentSubAccountId).ToList();

            result.Add(fieldId.Item1);
            return result.ToArray();
        }

        /// <summary>
        /// Get the current report sources for the given project and return as a list of List Items.
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public List<ListItem> GetReportSources(int projectId, int groupingConfigurationId)
        {
            var user = cMisc.GetCurrentUser();
            var mergeProjects = new DocumentMergeProject(user);
            var mergeProject = mergeProjects.GetProjectById(projectId);
            var config = GetGroupingConfiguration(projectId, groupingConfigurationId);
            var list = (from reportSource in mergeProject.GetReportSourcesSummary()
                        where reportSource.Value != null && mergeProject.GroupingReportSources.Contains(reportSource.Key)
                        select new ListItem(reportSource.Value.ReportName, reportSource.Key.ToString())).ToList();

            list.Sort((lt1, lt2) => System.String.Compare(lt1.Text, lt2.Text, System.StringComparison.Ordinal));
            foreach (ListItem listItem in list)
            {
                foreach (TorchReportSorting reportSortingConfiguration in config.ReportSortingConfigurations)
                {
                    if (reportSortingConfiguration.ReportName == listItem.Text &&
                        reportSortingConfiguration.TorchReportSortingColumns.Count > 0)
                    {
                        listItem.Text = listItem.Text + SortedIndicator;
                    }
                }
            }
            return list;
        }

        /// <summary>
        /// Get the columns for a report source on project id
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="reportId"></param>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        public List<string> GetReportColumns(int projectId, int reportId)
        {
            var user = cMisc.GetCurrentUser();
            var mergeProjects = new DocumentMergeProject(user);
            var mergeProject = mergeProjects.GetProjectById(projectId);
            List<string> result = mergeProject.GetReportSourceColumns(reportId);
            result.Sort();
            return result;
        }

        [WebMethod(EnableSession = true)]
        public TorchProject GetProject(int projectId)
        {
            var documentMergeProject = new DocumentMergeProject(cMisc.GetCurrentUser());
            TorchProject project = documentMergeProject.GetProjectById(projectId);
            return project;
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public Guid PerformDocumentMerge(int mergeprojectid, int documentid, int recordid, int entityid, int configId, bool storeDoc)
        {
            var mergeRequestNumber = Guid.NewGuid();
            Session["currentMergeRequestNumber"] = mergeRequestNumber;
            
            CurrentUser curUser = cMisc.GetCurrentUser();
            var projects = new DocumentMergeProject(curUser);
            TorchProject project = projects.GetProjectById(mergeprojectid);
            var entities = new cCustomEntities(curUser);
            cCustomEntity entity = entities.getEntityById(entityid);
            var criteria = new cReportCriterion(Guid.Empty, Guid.Empty, entity.table.GetPrimaryKey(),
                ConditionType.Equals, new object[] { recordid }, new object[] { }, ConditionJoiner.None, 99, false, 0);
            Session["MergeStatus_" + mergeprojectid + "_" + mergeRequestNumber] = 1;
            var executor = new DocumentMergeExecutor(curUser, documentid, project, criteria,
                mergeRequestNumber, configId, recordid,entityid, storeDoc, new cAuditLog(curUser.AccountID, curUser.EmployeeID));
            var docTemplates = new DocumentTemplate(curUser.AccountID, curUser.CurrentSubAccountId, curUser.EmployeeID);
            cDocumentTemplate docTemplate = docTemplates.getTemplateById(documentid);
            var outputDocType = TorchExportDocumentType.MS_Word_DOC;

            switch (docTemplate.DocumentContentType)
            {
                case "application/msword":
                    outputDocType = TorchExportDocumentType.MS_Word_DOC;
                    break;
                case "application/vnd.openxmlformats-officedocument.wordprocessingml.document":
                    outputDocType = TorchExportDocumentType.MS_Word_DOCX;
                    break;
            }

            executor.DoMerge(outputDocType);
            return mergeRequestNumber;
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public Guid PerformTempDocumentMerge(int mergeprojectid, int documentid, int recordid, int entityid, TorchGroupingConfiguration config, bool storeDoc)
        {
            Guid mergeRequestNumber = Guid.NewGuid();
            Session["currentMergeRequestNumber"] = mergeRequestNumber;

            CurrentUser curUser = cMisc.GetCurrentUser();
            var projects = new DocumentMergeProject(curUser);
            TorchProject project = projects.GetProjectById(mergeprojectid);
            var entities = new cCustomEntities(curUser);
            cCustomEntity entity = entities.getEntityById(entityid);
            var criteria = new cReportCriterion(Guid.Empty, Guid.Empty, entity.table.GetPrimaryKey(),
                ConditionType.Equals, new object[] { recordid }, new object[] { }, ConditionJoiner.None, 99, false, 0);
            Session["MergeStatus_" + mergeprojectid + "_" + mergeRequestNumber] = 1;
            var executor = new DocumentMergeExecutor(curUser, documentid, project, criteria,
                mergeRequestNumber, config.GroupingConfigurationId, recordid, entityid, storeDoc);
            config.GroupingSources = new List<string>();
            var projectGroupingSources = project.GetGroupingSources();
            foreach (KeyValuePair<int, cReport> keyValue in project.GetReportSources())
            {
                if (projectGroupingSources.Contains(keyValue.Key))
                {
                    config.GroupingSources.Add(keyValue.Value.reportname);
                }
            }
                
            var reportSourcesToDelete = new List<TorchReportSorting>();
            foreach (TorchReportSorting reportSortingConfiguration in config.ReportSortingConfigurations)
            {
                if (!config.GroupingSources.Contains(reportSortingConfiguration.ReportName))
                {
                    reportSourcesToDelete.Add(reportSortingConfiguration);
                }
            }

            foreach (TorchReportSorting reportSorting in reportSourcesToDelete)
            {
                config.ReportSortingConfigurations.Remove(reportSorting);
            }

            executor.TorchGroupingConfiguration = config;
            var docTemplates = new DocumentTemplate(curUser.AccountID, curUser.CurrentSubAccountId, curUser.EmployeeID);
            cDocumentTemplate docTemplate = docTemplates.getTemplateById(documentid);
            var outputDocType = TorchExportDocumentType.MS_Word_DOC;

            switch (docTemplate.DocumentContentType)
            {
                case "application/msword":
                    outputDocType = TorchExportDocumentType.MS_Word_DOC;
                    break;
                case "application/vnd.openxmlformats-officedocument.wordprocessingml.document":
                    outputDocType = TorchExportDocumentType.MS_Word_DOCX;
                    break;
            }

            executor.DoMerge(outputDocType);
            return mergeRequestNumber;
        }
        #endregion
    }

}