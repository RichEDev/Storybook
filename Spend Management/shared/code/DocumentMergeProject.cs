using SpendManagementLibrary.DocumentMerge;
using Utilities.DistributedCaching;

namespace Spend_Management.shared.code
{
    #region Using Directives

    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data;
    using System.Data.SqlClient;
    using System.Globalization;
    using System.Linq;
    using System.Web.UI.WebControls;

    using SpendManagementLibrary;
    using SpendManagementLibrary.Helpers;

    #endregion

    /// <summary>
    /// Document Merge Project Class
    /// </summary>
    [Serializable]
    public class DocumentMergeProject
    {
        private Dictionary<int, TorchProject> _projects;

        private readonly string _connectionString;

        private Cache _cache = new Cache();

        public const string CacheArea = "DocumentMergeProjects";

        #region Constructors and Destructors

        /// <summary>
        ///     cDocumentMergeProjects, loads and caches merge configurations for the specified customer database
        /// </summary>
        public DocumentMergeProject(ICurrentUser currentUser)
        {
            this.CurrentUser = currentUser;
            this.AccountId = currentUser.AccountID;
            this.EmployeeId = currentUser.EmployeeID;
            this.SubAccountId = currentUser.CurrentSubAccountId;
            this._connectionString = cAccounts.getConnectionString(this.AccountId);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the account id
        /// </summary>
        public int AccountId { get; set; }

        /// <summary>
        /// </summary>
        public int EmployeeId { get; set; }

        /// <summary>
        /// Gets or sets the sub account id
        /// </summary>
        public int? SubAccountId { get; set; }

        /// <summary>
        /// Gets or sets the current user
        /// </summary>
        public ICurrentUser CurrentUser { get; set; }

        /// <summary>
        ///     Get the SQL query for cGridNew to display
        /// </summary>
        public string DocMergeTemplatesGridSql
        {
            get
            {
                return
                    "select mergeprojectid, project_name, project_description, createddate, createdby, modifieddate  from document_mergeprojects";
            }
        }

        /// <summary>
        /// Gets or sets the SQL query for cGridNew to display
        /// </summary>
        public string GridSql
        {
            get
            {
                return "select documentid, doc_name, mergeprojectid from document_templates";
            }
        }

        /// <summary>
        /// Gets or sets the projects.
        /// </summary>
        public Dictionary<int, TorchProject> Projects
        {
            get
            {
                return this._projects ?? (this._projects = this.GetProjects());
            }
        }

        #endregion


        #region Public Methods and Operators

        /// <summary>
        ///     DeleteProject: Deletes a current merge configuration from the database
        /// </summary>
        /// <param name="projectid">Database ID of the project</param>
        /// <param name="employeeid">Employee Id of the user requesting the deletion</param>
        /// <returns>Empty String if ok, otherwise, reason or error</returns>
        public string DeleteProject(int projectid, int employeeid)
        {
            string strReturn = string.Empty;

            try
            {
                if (this.Projects.ContainsKey(projectid))
                {
                    var db = new DBConnection(cAccounts.getConnectionString(this.AccountId));

                    db.sqlexecute.Parameters.AddWithValue("@projectid", projectid);
                    db.sqlexecute.Parameters.AddWithValue("@userid", employeeid);
                    db.sqlexecute.Parameters.AddWithValue("@CUemployeeID", this.CurrentUser.EmployeeID);
                    if (this.CurrentUser.isDelegate)
                    {
                        db.sqlexecute.Parameters.AddWithValue("@CUdelegateID", this.CurrentUser.Delegate.EmployeeID);
                    }
                    else
                    {
                        db.sqlexecute.Parameters.AddWithValue("@CUdelegateID", DBNull.Value);
                    }

                    using (SqlDataReader reader = db.GetStoredProcReader("deleteDocumentMergeProject"))
                    {
                        while (reader.Read())
                        {
                            if (reader.GetInt32(0) == -1)
                            {
                                strReturn =
                                    "Document configuration cannot be deleted as it is currently being used by a document template.";
                            }
                        }
                    }
                }
            }
            catch
            {
                strReturn =
                    "An error uccurred deleting the document configuration. Please contact your administrator if the problem persists.";
            }

            if (strReturn == string.Empty)
            {
                this._projects = null;
            }

            _cache.Delete(this.AccountId, CacheArea, string.Empty);
            return strReturn;
        }

        /// <summary>
        ///     SetMergeProjectComplete: tags the project as complete once creation wizard completed
        ///     <param name="projectid">Project Id of project to be updated</param>
        /// </summary>
        public void SetMergeProjectComplete(int projectid)
        {
            var db = new DBConnection(cAccounts.getConnectionString(this.AccountId));

            db.sqlexecute.Parameters.AddWithValue("@projectId", projectid);
            const string Sql = "update document_mergeprojects set complete = 1 where mergeprojectid = @projectid";
            db.ExecuteSQL(Sql);

            db.sqlexecute.Parameters.Clear();
        }

        /// <summary>
        ///  UpdateProject: updates the project definition fields of the merge configuration
        /// </summary>
        /// <param name="project">Project structure in cDocumentMergeProject class structure</param>
        /// <param name="employeeid">Employee ID updating the project</param>
        /// <returns>Project Id from the database</returns>
        public int UpdateProject(TorchProject project, int employeeid)
        {
            var db = new DBConnection(cAccounts.getConnectionString(this.AccountId));

            // must be adding a new project
            db.sqlexecute.Parameters.Add("@ReturnId", SqlDbType.Int);
            db.sqlexecute.Parameters["@ReturnId"].Direction = ParameterDirection.ReturnValue;
            db.sqlexecute.Parameters.AddWithValue("@projectid", project.MergeProjectId);
            db.sqlexecute.Parameters.AddWithValue("@projectName", project.MergeProjectName);
            db.sqlexecute.Parameters.AddWithValue("@projectDescription", project.MergeProjectDescription);
            db.sqlexecute.Parameters.AddWithValue("@userId", employeeid);
            db.sqlexecute.Parameters.AddWithValue("@blankTabletext", project.BlankTableText);

            db.sqlexecute.Parameters.AddWithValue("@CUemployeeID", this.CurrentUser.EmployeeID);
            if (this.CurrentUser.isDelegate)
            {
                db.sqlexecute.Parameters.AddWithValue("@CUdelegateID", this.CurrentUser.Delegate.EmployeeID);
            }
            else
            {
                db.sqlexecute.Parameters.AddWithValue("@CUdelegateID", DBNull.Value);
            }

            db.sqlexecute.Parameters.AddWithValue("@defaultDocumentGroupingConfigId", project.DefaultDocumentGroupingConfigId);

            db.ExecuteProc("saveDocumentMergeProject");
            var returnId = (int)db.sqlexecute.Parameters["@ReturnId"].Value;

            var newprj = new TorchProject(
                returnId,
                project.MergeProjectName,
                project.MergeProjectDescription,
                DateTime.Now,
                employeeid,
                null,
                null,
                project.DefaultDocumentGroupingConfigId,
                this.AccountId,
                project.BlankTableText);

            this.GetProjectById(returnId);
            if (this._projects.ContainsKey(returnId))
            {
                this._projects[returnId] = newprj;
            }
            else
            {
                if (returnId != -1)
                {
                    this._projects.Add(returnId, newprj);
                }
            }

            db.sqlexecute.Parameters.Clear();
            _cache.Delete(this.AccountId, CacheArea, string.Empty);

            return returnId;
        }

        /// <summary>
        /// GetDocumentMergeGrid: retrieves the dataset of existing document merge configurations for use by cGrid class
        /// </summary>
        /// <returns></returns>
        public DataSet GetDocumentMergeGrid()
        {
            var ds = new DataSet();
            var tbl = new DataTable();

            tbl.Columns.Add("mergeprojectid", typeof(Int32));
            tbl.Columns.Add("project_name", typeof(String));
            tbl.Columns.Add("modified", typeof(DateTime));

            foreach (KeyValuePair<int, TorchProject> p in this.Projects)
            {
                var prj = p.Value;
                const string Status = "Unknown";

                tbl.Rows.Add(new object[] { prj.MergeProjectId, prj.MergeProjectName, Status, prj.ModifiedDate });
            }

            ds.Tables.Add(tbl);

            return ds;
        }

        /// <summary>
        /// GetProjectById: Retrieve merge configuration record from the cached collection
        /// </summary>
        /// <param name="mergeProjectId">Project ID of project to retrieve</param>
        /// <returns>cDocumentMergeProject class structure if found, otherwise returns NULL</returns>
        public TorchProject GetProjectById(int mergeProjectId)
        {
            return this.Projects.ContainsKey(mergeProjectId) ? this.Projects[mergeProjectId] : null;
        }

        /// <summary>
        /// GetReportSourceProject: returns the merge configuration that 'owns' the merge source ID provided
        /// </summary>
        /// <param name="mergesourceid">ID of the merge source</param>
        /// <returns>The merge configuration in the cDocumentMergeProject class structure</returns>
        public TorchProject GetReportSourceProject(int mergesourceid)
        {
            return this.Projects.Select(i => i.Value).FirstOrDefault(curPrj => curPrj.GetReportSources().ContainsKey(mergesourceid));
        }

        #endregion

        #region Methods

        private Dictionary<int, TorchProject> GetProjects()
        {
            if (this._projects == null)
            {
                this._projects = new Dictionary<int, TorchProject>();
            }

            var result = _cache.Get(this.AccountId, CacheArea, string.Empty);
            if (result != null)
            {
                this._projects = (Dictionary<int, TorchProject>)result;
                return _projects;
            }

            var db = new DBConnection(this._connectionString);
            SqlDataReader reader;
            const string Strsql =
                "select mergeprojectid, project_name, project_description, createddate, createdby, modifieddate, modifiedby, DefaultDocumentGroupingConfigId, blankTableText from dbo.document_mergeprojects";
            db.sqlexecute.CommandText = Strsql;

            using (reader = db.GetReader(Strsql))
            {
                int mergeProjectIdOrdinal = reader.GetOrdinal("mergeprojectid");
                int projectNameOrdinal = reader.GetOrdinal("project_name");
                int projectDescriptionOrdinal = reader.GetOrdinal("project_description");
                int createdDateOrdinal = reader.GetOrdinal("createddate");
                int createdByOrdinal = reader.GetOrdinal("createdby");
                int modifiedDateOrdinal = reader.GetOrdinal("modifieddate");
                int modifiedByOrdinal = reader.GetOrdinal("modifiedby");
                int defaultDocumentGroupingConfigIdOrdinal = reader.GetOrdinal("DefaultDocumentGroupingConfigId");
                int blankTableTextOrdinal = reader.GetOrdinal("blankTableText");

                while (reader.Read())
                {
                    int projid = reader.GetInt32(mergeProjectIdOrdinal);
                    string name = reader.GetString(projectNameOrdinal);
                    string desc = "";

                    if (!reader.IsDBNull(projectDescriptionOrdinal))
                    {
                        desc = reader.GetString(projectDescriptionOrdinal);
                    }

                    DateTime? created = null;
                    if (!reader.IsDBNull(createdDateOrdinal))
                    {
                        created = reader.GetDateTime(createdDateOrdinal);
                    }

                    int? createdby = null;
                    if (!reader.IsDBNull(createdByOrdinal))
                    {
                        createdby = reader.GetInt32(createdByOrdinal);
                    }

                    DateTime? modified = null;
                    if (!reader.IsDBNull(modifiedDateOrdinal))
                    {
                        modified = reader.GetDateTime(modifiedDateOrdinal);
                    }

                    int? modifiedby = null;
                    if (!reader.IsDBNull(modifiedByOrdinal))
                    {
                        modifiedby = reader.GetInt32(modifiedByOrdinal);
                    }

                    int? defaultDocumentGroupingConfigId = null;

                    if (!reader.IsDBNull(defaultDocumentGroupingConfigIdOrdinal))
                    {
                        defaultDocumentGroupingConfigId = reader.GetInt32(defaultDocumentGroupingConfigIdOrdinal);
                    }

                    var blankTableText = reader.IsDBNull(blankTableTextOrdinal) ? string.Empty : reader.GetString(blankTableTextOrdinal);

                    var proj = new TorchProject(
                        projid,
                        name,
                        desc,
                        created,
                        createdby,
                        modified,
                        modifiedby,
                        defaultDocumentGroupingConfigId,
                        this.AccountId,
                        blankTableText);

                    this._projects.Add(projid, proj);
                }

                reader.Close();
            }


            _cache.Add(this.AccountId, CacheArea, string.Empty, _projects);
            return this._projects;
        }


        /// <summary>
        /// Save the grouping column information with "label" or update the configuration.
        /// </summary>
        /// <param name="groupingColumns">A list of the names of the grouping columns in the configuration</param>
        /// <param name="mergeProjectId">The merge project id</param>
        /// <param name="label">The label for the configuration (user defined)</param>
        /// <param name="archived">Sets whether the configuration will be archived</param>
        /// <param name="groupingConfigurationId">The id value for the grouping configuration</param>
        /// <param name="description">Description of the config.</param>
        /// <returns>0 for success</returns>
        public int SaveGroupingConfiguration(List<string> groupingColumns, int mergeProjectId, string label, bool archived = false, int groupingConfigurationId = 0, string description = "")
        {
            int returnId;

            using (var connection = new DatabaseConnection(this._connectionString))
            {
                connection.sqlexecute.Parameters.Clear();
                connection.sqlexecute.Parameters.Add("@ReturnId", SqlDbType.Int);
                connection.sqlexecute.Parameters["@ReturnId"].Direction = ParameterDirection.ReturnValue;

                int i = 0;
                var table = new DataTable();
                table.Columns.Add("StringValue", typeof(string));
                table.Columns.Add("Sequence", typeof(int));

                foreach (var groupingColumn in groupingColumns)
                {
                    table.Rows.Add(groupingColumn, i);
                    i++;
                }

                var parameter = new SqlParameter("@GroupingColumns", SqlDbType.Structured)
                {
                    TypeName =
                        "dbo.StringTable",
                    Value = table
                };

                connection.sqlexecute.Parameters.Add(parameter);
                connection.sqlexecute.Parameters.AddWithValue("@GroupingConfigurationId", groupingConfigurationId);
                connection.sqlexecute.Parameters.AddWithValue("@MergeProjectId", mergeProjectId);
                connection.sqlexecute.Parameters.AddWithValue("@Label", label);
                connection.sqlexecute.Parameters.AddWithValue("@UserId", this.CurrentUser.EmployeeID);
                connection.sqlexecute.Parameters.AddWithValue("@Archived", archived);
                connection.sqlexecute.Parameters.AddWithValue("@Description", description);
                connection.ExecuteProc("UpdateGroupingConfiguration");
                returnId = (int)connection.sqlexecute.Parameters["@ReturnId"].Value;
            }

            return returnId;
        }

        /// <summary>
        /// Save the filter details for filter column
        /// </summary>
        /// <param name="mergeProjectId"></param>
        /// <param name="documentGroupingConfigurationId"></param>
        /// <param name="filterColumn"></param>  
        /// <param name="sequenceOrder"></param>
        /// <param name="condition"></param>
        /// <param name="valueOne"></param>
        /// <param name="valueTwo"></param>
        /// <param name="torchGroupingFieldFilter"></param>
        /// <returns></returns>
        public int SaveFilteringConfiguration(int mergeProjectId, int documentGroupingConfigurationId, int sequenceOrder, TorchGroupingFieldFilter torchGroupingFieldFilter)
        {
            int returnId;

            using (var connection = new DatabaseConnection(this._connectionString))
            {
                connection.sqlexecute.Parameters.Clear();
                connection.sqlexecute.Parameters.Add("@ReturnId", SqlDbType.Int);
                connection.sqlexecute.Parameters["@ReturnId"].Direction = ParameterDirection.ReturnValue;

                connection.sqlexecute.Parameters.AddWithValue("@MergeProjectId", mergeProjectId);
                connection.sqlexecute.Parameters.AddWithValue("@DocumentGroupingConfigurationId", documentGroupingConfigurationId);
                connection.sqlexecute.Parameters.AddWithValue("@FilterColumn", torchGroupingFieldFilter.ColumnName);
                connection.sqlexecute.Parameters.AddWithValue("@SequenceOrder", sequenceOrder);
                connection.sqlexecute.Parameters.AddWithValue("@Condition", torchGroupingFieldFilter.conditionType);
                connection.sqlexecute.Parameters.AddWithValue("@ValueOne", torchGroupingFieldFilter.criterionOne);
                connection.sqlexecute.Parameters.AddWithValue("@ValueTwo", torchGroupingFieldFilter.criterionTwo);
                connection.sqlexecute.Parameters.AddWithValue("@TypeText", string.Empty);
                connection.sqlexecute.Parameters.AddWithValue("@FieldType", torchGroupingFieldFilter.fieldType);
                connection.ExecuteProc("UpdateFilteringConfiguration");
                returnId = (int)connection.sqlexecute.Parameters["@ReturnId"].Value;
            }

            return returnId;
        }

        /// <summary>
        /// Save the sorting details for sorting column
        /// </summary>
        /// <param name="mergeProjectId"></param>
        /// <param name="documentGroupingConfigurationId"></param>
        /// <param name="sequenceOrder"></param>
        /// <param name="sortingColumn"></param>
        /// <param name="sortingOrder"></param>
        /// <returns></returns>
        public int SaveSortingConfiguration(int mergeProjectId, int documentGroupingConfigurationId, int sequenceOrder, string sortingColumn, int sortingOrder)
        {
            int returnId;

            using (var connection = new DatabaseConnection(this._connectionString))
            {
                connection.sqlexecute.Parameters.Clear();
                connection.sqlexecute.Parameters.Add("@ReturnId", SqlDbType.Int);
                connection.sqlexecute.Parameters["@ReturnId"].Direction = ParameterDirection.ReturnValue;

                connection.sqlexecute.Parameters.AddWithValue("@MergeProjectId", mergeProjectId);
                connection.sqlexecute.Parameters.AddWithValue("@DocumentGroupingConfigurationId", documentGroupingConfigurationId);
                connection.sqlexecute.Parameters.AddWithValue("@SequenceOrder", sequenceOrder);
                connection.sqlexecute.Parameters.AddWithValue("@SortingColumn", sortingColumn);
                connection.sqlexecute.Parameters.AddWithValue("@SortingOrder", sortingOrder);
                connection.ExecuteProc("UpdateSortingConfiguration");
                returnId = (int)connection.sqlexecute.Parameters["@ReturnId"].Value;
            }

            return returnId;
        }

        /// <summary>
        /// Saves the report sorting configuration 
        /// </summary>
        /// <param name="mergeProjectId"></param>
        /// <param name="documentGroupingConfigurationId"></param>
        /// <returns>the Id of the new row (reportSortingConfigurationId)</returns>
        public int SaveReportSortingConfiguration(int mergeProjectId, int documentGroupingConfigurationId)
        {
            int returnId;

            using (var connection = new DatabaseConnection(this._connectionString))
            {
                connection.sqlexecute.Parameters.Clear();
                connection.sqlexecute.Parameters.Add("@ReturnId", SqlDbType.Int);
                connection.sqlexecute.Parameters["@ReturnId"].Direction = ParameterDirection.ReturnValue;

                connection.sqlexecute.Parameters.AddWithValue("@MergeProjectId", mergeProjectId);
                connection.sqlexecute.Parameters.AddWithValue("@DocumentGroupingConfigurationId", documentGroupingConfigurationId);
                connection.ExecuteProc("SaveReportSortingConfiguration");
                returnId = (int)connection.sqlexecute.Parameters["@ReturnId"].Value;
            }

            return returnId;
        }

        /// <summary>
        /// Saves the sorting columns for a report
        /// </summary>   
        /// <param name="reportSortingConfigurationId"></param>
        /// <param name="reportName"></param>
        /// <param name="reportColumns"></param>
        /// <returns></returns>
        public int SaveReportColumnSortingConfiguration(int reportSortingConfigurationId, string reportName, TorchReportSortingColumn[] reportColumns)
        {
            int returnId = -1;
            int i = 0;
            var table = new DataTable();
            table.Columns.Add("ColumnName", typeof(string));
            table.Columns.Add("SortingOrder", typeof(int));
            table.Columns.Add("Sequence", typeof(int));

            foreach (var groupingColumn in reportColumns)
            {
                table.Rows.Add(groupingColumn.ColumnName, groupingColumn.SortingOrder, i);
                i++;
            }

            if (table.Rows.Count > 0)
            {
                var parameter = new SqlParameter("@ReportSortingColumns", SqlDbType.Structured)
                {
                    TypeName =
                        "dbo.ReportSortingColumn",
                    Value = table
                };

                using (var connection = new DatabaseConnection(this._connectionString))
                {
                    connection.sqlexecute.Parameters.Clear();
                    connection.sqlexecute.Parameters.Add(parameter);
                    connection.sqlexecute.Parameters.Add("@ReturnId", SqlDbType.Int);
                    connection.sqlexecute.Parameters["@ReturnId"].Direction = ParameterDirection.ReturnValue;
                    connection.sqlexecute.Parameters.AddWithValue("@ReportSortingConfigurationId", reportSortingConfigurationId);
                    connection.sqlexecute.Parameters.AddWithValue("@ReportName", reportName);

                    connection.ExecuteProc("UpdateReportSortingConfiguration");
                    returnId = (int)connection.sqlexecute.Parameters["@ReturnId"].Value;

                }
            }

            table = null;
            return returnId;
        }

        #endregion

        /// <summary>
        /// Archives or unarchives a grouping configuration for this project
        /// </summary>
        /// <param name="projectId">The merge project</param>
        /// <param name="configurationId">The configuration id</param>
        /// <param name="archived">Archived or unarchived</param>
        /// <returns>Status</returns>
        /// <exception cref="NotImplementedException"></exception>
        public int ArchiveGroupingConfiguration(int projectId, int configurationId, bool archived)
        {
            //TODO Richard
            throw new NotImplementedException();
        }

        /// <summary>
        /// Deletes a grouping configuration for this project
        /// </summary>
        /// <param name="mergeProjectId">The merge project</param>
        /// <param name="groupingConfigurationId">The configuration id</param>
        /// <returns>Status</returns>
        /// <exception cref="NotImplementedException"></exception>
        public int DeleteGroupingConfiguration(int mergeProjectId, int groupingConfigurationId)
        {
            int returnId;

            using (var connection = new DatabaseConnection(this._connectionString))
            {
                connection.sqlexecute.Parameters.Clear();
                connection.sqlexecute.Parameters.Add("@ReturnId", SqlDbType.Int);
                connection.sqlexecute.Parameters["@ReturnId"].Direction = ParameterDirection.ReturnValue;
                connection.sqlexecute.Parameters.AddWithValue("@GroupingConfigurationId", groupingConfigurationId);
                connection.sqlexecute.Parameters.AddWithValue("@MergeProjectId", mergeProjectId);
                connection.ExecuteScalar<int>("dbo.DeleteGroupingConfiguration", CommandType.StoredProcedure);
                returnId = (int)connection.sqlexecute.Parameters["@ReturnId"].Value;
            }

            return returnId;
        }

        /// <summary>
        ///  Deletes all filter conditions for a configurationId
        /// </summary>
        /// <param name="groupingConfigurationId"></param>
        /// <returns></returns>
        public int DeleteDocumentFiltersConfigurationColumnsByConfigurationId(int groupingConfigurationId)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();

            using (var connection = new DatabaseConnection(cAccounts.getConnectionString(currentUser.AccountID)))
            {
                connection.sqlexecute.Parameters.AddWithValue("@DocumentGroupingConfigurationId", groupingConfigurationId);
                connection.ExecuteProc("DeleteDocumentFiltersConfigurationColumnsByConfigurationId");
            }

            return 0;
        }


        /// <summary>
        ///  Deletes all sorting configurations for a configurationId
        /// </summary>
        /// <param name="groupingConfigurationId"></param>
        /// <returns></returns>
        public int DeleteDocumentSortingConfigurationColumnsByConfigurationId(int groupingConfigurationId)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();

            using (var connection = new DatabaseConnection(cAccounts.getConnectionString(currentUser.AccountID)))
            {
                connection.sqlexecute.Parameters.AddWithValue("@DocumentGroupingConfigurationId", groupingConfigurationId);
                connection.ExecuteProc("DeleteDocumentSortingConfigurationColumnsByConfigurationId");
            }

            return 0;
        }

        /// <summary>
        ///  Deletes all report sorting configurations for a configurationId
        /// </summary>
        /// <param name="groupingConfigurationId"></param>
        /// <returns></returns>
        public int DeleteDocumentReportSortingConfigurationColumnsByConfigurationId(int groupingConfigurationId)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();

            using (var connection = new DatabaseConnection(cAccounts.getConnectionString(currentUser.AccountID)))
            {
                connection.sqlexecute.Parameters.AddWithValue("@DocumentGroupingConfigurationId", groupingConfigurationId);
                connection.ExecuteProc("DeleteDocumentReportSortingConfigurationColumnsByConfigurationId");
            }

            return 0;
        }

        /// <summary>
        /// Retrieves a list of configuration labels and associated id's
        /// </summary>
        /// <param name="projectId">The merge project</param>
        /// <param name="includeArchived">If set true, will return all configurations</param>
        /// <returns>List of labels and id's</returns>
        public List<ListItem> GetGroupingConfigurations(int projectId, bool includeArchived = false)
        {
            var listItems = new List<ListItem>();

            using (var connection = new DatabaseConnection(this._connectionString))
            {
                connection.sqlexecute.Parameters.Clear();
                connection.sqlexecute.Parameters.AddWithValue("@MergeProjectid", projectId);
                connection.sqlexecute.Parameters.AddWithValue("@IncludeArchived", includeArchived);
                using (IDataReader reader = connection.GetReader("dbo.TorchGetGroupingConfigurations", CommandType.StoredProcedure))
                {
                    while (reader.Read())
                    {
                        listItems.Add(new ListItem(reader.GetString(1), reader.GetInt32(0).ToString(CultureInfo.InvariantCulture)));
                    }

                    reader.Close();
                }
            }

            return listItems;
        }

        /// <summary>
        /// Retrieves a configuration of grouping, filtering, sorting and report sorting from a grouping configuration id.
        /// </summary>
        /// <param name="projectId">The merge project</param>
        /// <param name="groupingConfigurationId">The document configuration</param>
        /// <param name="addBlankItem">If true, add a "BLANK" entry to the report sources to enable the javascript object to work correctly.</param>
        /// <returns>A TorchGroupingConfiguration object</returns>
        public TorchGroupingConfiguration GetGroupingConfiguration(int projectId, int groupingConfigurationId, bool addBlankItem = false)
        {
            var result = GetConfiguration(projectId, groupingConfigurationId);

            result.GroupingColumnsList = GetGroupingColumnListById(groupingConfigurationId);
            result.SortingColumnsList = GetSortingColumnListById(projectId, groupingConfigurationId);
            result.FilteringColumns = GetGroupingFieldFilters(projectId, groupingConfigurationId);
            result.GroupingSources = GetGroupingSources(projectId, groupingConfigurationId);
            List<TorchReportSorting> reportSortingConfigurations = GetReportSortingConfigurations(projectId,
                 groupingConfigurationId);
            if (reportSortingConfigurations.Count == 0 && addBlankItem)
            {
                reportSortingConfigurations.Add(new TorchReportSorting("blank", new List<TorchReportSortingColumn> { new TorchReportSortingColumn("blank", 0) }));
            }

            result.ReportSortingConfigurations = reportSortingConfigurations;
            return result;
        }

        /// <summary>
        /// Gets the reports and their sorted columns for a merge project/configuration
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="groupingConfigurationId"></param>
        /// <returns>A list of reports and their associated sorting columns</returns>
        private List<TorchReportSorting> GetReportSortingConfigurations(int projectId, int groupingConfigurationId)
        {
            List<TorchSortingReports> sortingReports = GetSortingReports(projectId, groupingConfigurationId);
            var reportSortingConfigurations = new List<TorchReportSorting>();

            foreach (var report in sortingReports)
            {
                var fieldFilters = new List<TorchReportSortingColumn>();

                using (var connection = new DatabaseConnection(this._connectionString))
                {
                    connection.sqlexecute.Parameters.Clear();
                    connection.sqlexecute.Parameters.AddWithValue("@ReportSortingConfigurationId", report.ReportId);

                    using (IDataReader reader = connection.GetReader("dbo.TorchGetReportSortingColumns", CommandType.StoredProcedure))
                    {
                        while (reader.Read())
                        {
                            var columnNameOrdinal = reader.GetOrdinal("ColumnName");
                            var documentSortTypeIdOrdinal = reader.GetOrdinal("DocumentSortTypeId");
                            var columnName = reader.GetString(columnNameOrdinal);
                            var sortingOrder = (TorchSummaryColumnSortDirection)reader.GetInt32(documentSortTypeIdOrdinal);
                            var fieldFilter = new TorchReportSortingColumn(columnName, sortingOrder);

                            fieldFilters.Add(fieldFilter);
                        }
                        reader.Close();
                    }
                }

                reportSortingConfigurations.Add(new TorchReportSorting(report.ReportName, fieldFilters));

            }

            return reportSortingConfigurations;
        }

        /// <summary>
        /// Gets a list of sorting reports
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="groupingConfigurationId"></param>
        /// <returns>A list of TorchSortingReports</returns>
        private List<TorchSortingReports> GetSortingReports(int projectId, int groupingConfigurationId)
        {
            var sortingReports = new List<TorchSortingReports>();

            using (var connection = new DatabaseConnection(this._connectionString))
            {
                connection.sqlexecute.Parameters.Clear();
                connection.sqlexecute.Parameters.AddWithValue("@MergeProjectId", projectId);
                connection.sqlexecute.Parameters.AddWithValue("@GroupingConfigurationId", groupingConfigurationId);

                using (IDataReader reader = connection.GetReader("dbo.TorchGetSortingReports", CommandType.StoredProcedure))
                {
                    while (reader.Read())
                    {
                        var documentSortingReportIdOrdinal = reader.GetOrdinal("DocumentSortingReportId");
                        var reportNameOrdinal = reader.GetOrdinal("ReportName");
                        var reportId = reader.GetInt32(documentSortingReportIdOrdinal);
                        var reportName = reader.GetString(reportNameOrdinal);

                        sortingReports.Add(new TorchSortingReports(reportId, reportName));

                    }
                    reader.Close();
                }
            }

            return sortingReports;
        }

        /// <summary>
        /// Gets the label for the grouping configuration
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="groupingConfigurationId"></param>
        /// <returns>The label for the grouping configuration</returns>
        private TorchGroupingConfiguration GetConfiguration(int projectId, int groupingConfigurationId)
        {
            var result = new TorchGroupingConfiguration
            {
                ConfigurationLabel = string.Empty,
                ConfigurationDescription = string.Empty
            };

            using (var connection = new DatabaseConnection(this._connectionString))
            {
                connection.sqlexecute.Parameters.Clear();
                connection.sqlexecute.Parameters.AddWithValue("@MergeProjectId", projectId);
                connection.sqlexecute.Parameters.AddWithValue("@GroupingConfigurationId", groupingConfigurationId);

                using (IDataReader reader = connection.GetReader("dbo.[TorchGetGroupingConfigurationLabel]", CommandType.StoredProcedure))
                {
                    while (reader.Read())
                    {
                        result.MergeProjectId = projectId;
                        result.GroupingConfigurationId = groupingConfigurationId;
                        result.ConfigurationLabel = reader.GetString(0);
                        result.ConfigurationDescription = reader.GetString(1);
                    }
                    reader.Close();
                    connection.sqlexecute.Parameters.Clear();
                }
            }

            return result;
        }

        /// <summary>
        /// Retrieves Field Filters for a mergeProject/Config
        /// </summary>
        /// <param name="mergeProjectId">The merge project</param>
        /// <param name="groupingConfigurationId">The document configuration</param>
        /// <returns>A list of TorchGroupingFieldFilter</returns>
        private List<TorchGroupingFieldFilter> GetGroupingFieldFilters(int mergeProjectId, int groupingConfigurationId)
        {
            var fieldFilters = new List<TorchGroupingFieldFilter>();
            var configurationFilters = new List<TorchGroupingFieldFilter>();

            using (var connection = new DatabaseConnection(this._connectionString))
            {
                connection.sqlexecute.Parameters.Clear();
                connection.sqlexecute.Parameters.AddWithValue("@MergeProjectId", mergeProjectId);
                connection.sqlexecute.Parameters.AddWithValue("@GroupingConfigurationId", groupingConfigurationId);

                using (IDataReader reader = connection.GetReader("dbo.[TorchGetGroupingFieldFilters]", CommandType.StoredProcedure))
                {
                    while (reader.Read())
                    {
                        var conditionalOrdinal = reader.GetOrdinal("Condition");
                        var value1Ordinal = reader.GetOrdinal("ValueOne");
                        var value2Ordinal = reader.GetOrdinal("ValueTwo");
                        var filterColumnOrdinal = reader.GetOrdinal("FilterColumn");
                        var typeTextOrdinal = reader.GetOrdinal("TypeText");
                        var fieldTypeOrdinal = reader.GetOrdinal("FieldType");

                        var condition = (ConditionType)reader.GetByte(conditionalOrdinal);
                        var valueOne = reader.GetString(value1Ordinal);
                        var valueTwo = reader.GetString(value2Ordinal);
                        var filterColumn = reader.GetString(filterColumnOrdinal);
                        var typeText = reader.GetString(typeTextOrdinal);
                        var fieldType = reader.GetString(fieldTypeOrdinal);

                        var fieldFilter = new TorchGroupingFieldFilter(filterColumn, condition, valueOne, valueTwo, typeText, fieldType);
                        fieldFilters.Add(fieldFilter);
                        configurationFilters.Add(fieldFilter);
                    }
                    reader.Close();
                }
            }

            return configurationFilters;
        }

        /// <summary>
        /// Gets the report names for sources associated with the mergeProject/Config 
        /// </summary>
        /// <param name="mergeProjectId">The merge project</param>
        /// <param name="groupingConfigurationId">The document configuration</param>
        /// <returns>A list of grouping sources</returns>
        private List<string> GetGroupingSources(int mergeProjectId, int groupingConfigurationId)
        {
            var groupingColumns = new List<string>();
            using (var connection = new DatabaseConnection(this._connectionString))
            {
                connection.sqlexecute.Parameters.Clear();
                connection.sqlexecute.Parameters.AddWithValue("@MergeProjectId", mergeProjectId);
                connection.sqlexecute.Parameters.AddWithValue("@GroupingConfigurationId", groupingConfigurationId);

                using (IDataReader reader = connection.GetReader("dbo.TorchGetGroupingSources", CommandType.StoredProcedure))
                {
                    while (reader.Read())
                    {
                        var reportNameOrdinal = reader.GetOrdinal("reportname");
                        groupingColumns.Add(reader.GetString(reportNameOrdinal));
                    }
                    reader.Close();
                }
            }

            return groupingColumns;
        }

        /// <summary>
        ///  Retrieves a list of sorting columns for a grouping config
        /// </summary>
        /// <param name="mergeProjectId">The merge project</param>
        /// <param name="groupingConfigurationId">The document configuration</param>
        /// <returns>A list of Sorting Columns</returns>
        private List<SortingColumn> GetSortingColumnListById(int mergeProjectId, int groupingConfigurationId)
        {
            var sortingColumns = new List<SortingColumn>();
            using (var connection = new DatabaseConnection(this._connectionString))
            {
                connection.sqlexecute.Parameters.Clear();
                connection.sqlexecute.Parameters.AddWithValue("@GroupingConfigurationId", groupingConfigurationId);
                connection.sqlexecute.Parameters.AddWithValue("@MergeProjectId", mergeProjectId);

                using (IDataReader reader = connection.GetReader("dbo.[TorchGetSortingColumnsById]", CommandType.StoredProcedure))
                {
                    while (reader.Read())
                    {
                        var sortingColumnOrdinal = reader.GetOrdinal("SortingColumn");
                        var sortingOrderOrdinal = reader.GetOrdinal("DocumentSortType");
                        sortingColumns.Add(new SortingColumn(reader.GetString(sortingColumnOrdinal), reader.GetString(sortingOrderOrdinal)));
                    }
                    reader.Close();
                }
            }

            return sortingColumns;

        }

        /// <summary>
        /// Retrieves a list of grouping columns for a grouping config
        /// </summary>
        /// <param name="groupingConfigurationId">The document configuration</param>
        /// <returns>A list of Grouping Columns</returns>
        private List<string> GetGroupingColumnListById(int groupingConfigurationId)
        {
            var groupingColumns = new List<string>();
            using (var connection = new DatabaseConnection(this._connectionString))
            {
                connection.sqlexecute.Parameters.Clear();
                connection.sqlexecute.Parameters.AddWithValue("@GroupingConfigurationId", groupingConfigurationId);

                using (IDataReader reader = connection.GetReader("dbo.[TorchGetGroupingColumnsById]", CommandType.StoredProcedure))
                {
                    while (reader.Read())
                    {
                        var groupingColumnOrdinal = reader.GetOrdinal("GroupingColumn");
                        groupingColumns.Add(reader.GetString(groupingColumnOrdinal));
                    }
                    reader.Close();
                }
            }

            return groupingColumns;
        }

    }

}