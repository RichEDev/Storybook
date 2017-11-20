using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using SpendManagementLibrary.Helpers;

namespace SpendManagementLibrary.DocumentMerge
{
    public enum TorchStatus
    {
        ReadyToMerge = 1,

        ReportModified,

        TemplateModified,

        BothModified
    }

    [Serializable]
    public class TorchProject
    {
        public int AccountId { get; set; }

        private readonly int _mergeProjectId;

        private readonly string _mergeProjectName;

        private readonly string _mergeProjectDescription;

        private Dictionary<int, cReport> _reportSources;

        private readonly DateTime? _createdDate;

        private readonly int? _createdBy;

        private readonly DateTime? _modifiedDate;

        private readonly int? _modifiedBy;

        private readonly int? _defaultDocumentGroupingConfigId;

        private readonly string _connectionString;

        private List<int> _groupingReportSources ;

        #region properties

        /// <summary>
        /// Returns the ID of the current merge configuration instance
        /// </summary>
        public int MergeProjectId
        {
            get
            {
                return _mergeProjectId;
        }
        }

        /// <summary>
        /// Returns the current merge configuration title
        /// </summary>
        public string MergeProjectName
        {
            get
            {
                return _mergeProjectName;
        }
        }

        /// <summary>
        /// Returns the extended description of the current merge configuration instance
        /// </summary>
        public string MergeProjectDescription
        {
            get
            {
                return _mergeProjectDescription;
        }
        }

        /// <summary>
        /// Returns the currently selected report source selections for the project instance
        /// </summary>
        public Dictionary<int, cReport> GetReportSources()
        {
            return _reportSources ?? (_reportSources = this.GetReportSources(this.MergeProjectId));
        }

        /// <summary>
        /// Returns the merge configuration creation date
        /// </summary>
        public DateTime? CreatedDate
        {
            get
            {
                return _createdDate;
        }
        }

        /// <summary>
        /// Returns the merge configuration creator user ID
        /// </summary>
        public int? CreatedBy
        {
            get
            {
                return _createdBy;
        }
        }

        /// <summary>
        /// Returns the merge configuration last modified date
        /// </summary>
        public DateTime? ModifiedDate
        {
            get
            {
                return _modifiedDate;
            }
        }

        /// <summary>
        /// Returns the merge configuration modifier user ID
        /// </summary>
        public int? ModifiedBy
        {
            get
            {
                return _modifiedBy;
            }
        }

        /// <summary>
        /// Returns the merge configuration default document grouping ID
        /// </summary>
        public int? DefaultDocumentGroupingConfigId
        {
            get
            {
                return _defaultDocumentGroupingConfigId;
            }
        }

        public List<int> GroupingReportSources
        {
            get
            {
                if (_groupingReportSources == null)
                {
                    _groupingReportSources = GetGroupingSources();
                }

                return _groupingReportSources;
            }
            private set
            {
                if (value == null) throw new ArgumentNullException("value");
                _groupingReportSources = value;
            }
        }

        public string GetReportSourceSql
        {
            get
            {
                return
                    "select mergesourceid, reports.reportid, reports.reportname createddate, createdby from dbo.document_mergesources where mergeprojectid = "
                    + this._mergeProjectId;
            }
        }

        #endregion

        /// <summary>
        /// cDocumentMergeProject, instantiates an instance of a document merge configuration entity
        /// </summary>
        /// <param name="projectId">ProjectID, unique database ID of merge configuration record</param>
        /// <param name="projectName">Merge project title</param>
        /// <param name="projectDescription">Extended description detailing purpose and scope of merge exercise</param>
        /// <param name="createdDate">Date project was initially created</param>
        /// <param name="createdBy">User ID of individual who created the configuration</param>
        /// <param name="modifieddate">Date the project was last modified or updated</param>
        /// <param name="modifiedby">User ID of individual who last modified the configuration</param>
        /// <param name="defaultDocumentGroupingConfigId">The default document grouping Id for the merge project</param>
        /// <param name="accountId"></param>
        /// <param name="blankTableText">The text to use in place of empty summary tables.</param>
        public TorchProject(
            int projectId,
            string projectName,
            string projectDescription,
            DateTime? createdDate,
            int? createdBy,
            DateTime? modifieddate,
            int? modifiedby,
            int? defaultDocumentGroupingConfigId,
            int accountId,
            string blankTableText)
        {
            AccountId = accountId;
            BlankTableText = blankTableText;
            _mergeProjectId = projectId;
            _mergeProjectName = projectName;
            _mergeProjectDescription = projectDescription;
            _createdDate = createdDate;
            _createdBy = createdBy;
            _modifiedDate = modifieddate;
            _modifiedBy = modifiedby;
            _defaultDocumentGroupingConfigId = defaultDocumentGroupingConfigId;
            _groupingReportSources = null;
            
            this._connectionString = cAccounts.getConnectionString(this.AccountId);
        }

        /// <summary>
        /// Gets the SQL for cGridNew for current report source selections
        /// </summary>
        public string GetCurrentReportSql
        {
            get
            {
                return
                    "select mergesourceid, reportid, groupingsource, reports.reportname, reports.description from dbo.document_mergesources";
            }
        }

        public string BlankTableText { get; set; }

        public static List<string> GetGroupingFields(int mergeProjectId, ICurrentUserBase user)
        {
            var fieldRecords = new List<string>();

            using (var connection = new DatabaseConnection(cAccounts.getConnectionString(user.AccountID)))
            {
                connection.AddWithValue("mergeProjectId", mergeProjectId);
                using (
                    IDataReader reader = connection.GetReader(
                        "TorchGetCommonGroupingColumns",
                        CommandType.StoredProcedure))
                {
                    while (reader.Read())
                    {
                        string columnName = reader.GetString(0);

                        if (!fieldRecords.Contains(columnName))
                        {
                            fieldRecords.Add(columnName);
                        }
                    }

                    reader.Close();
                }
            }

            return fieldRecords;
        }

        /// <summary>
        /// Return a Tuple with the Field ID and Field type of the common column, based on the column name (description).
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="commonColumnName"></param>
        /// <param name="user"></param>
        /// <returns>Tuple (Field ID[Guid], Field Type[string])</returns>
        public static Tuple<Guid, string> GetFieldIdFromCommonColumn(
            int projectId,
            string commonColumnName,
            ICurrentUserBase user)
        {
            var result = new Tuple<Guid, string>(new Guid(), string.Empty);
            using (var connection = new DatabaseConnection(cAccounts.getConnectionString(user.AccountID)))
            {
                connection.AddWithValue("mergeProjectId", projectId);
                connection.AddWithValue("commonColumnName", commonColumnName.TrimStart().TrimEnd());
                using (var reader = connection.GetReader("TorchGetFieldIdFromCommonColumn", CommandType.StoredProcedure)
                    )
                {
                    while (reader.Read())
                    {
                        var fieldId = reader.GetGuid(0);
                        var fieldType = reader.GetString(1);

                        result = new Tuple<Guid, string>(fieldId, fieldType);
                    }

                    reader.Close();
                }
            }

            return result;

        }

        public List<int> GetGroupingSources()
        {
            var result = new List<int>();
            using (var db = new DatabaseConnection(this._connectionString))
            {
                const string sql =
                   "[dbo].[GetGroupingSources]";
                db.sqlexecute.Parameters.AddWithValue("@mergeprojectid", this.MergeProjectId);
                using (var reader = db.GetReader(sql, CommandType.StoredProcedure))
                {
                    while (reader.Read())
                    {
                        result.Add(reader.GetInt32(0));
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Return the first (by date) grouping config.  Used if no single config is set as default.
        /// </summary>
        /// <returns></returns>
        public int GetFirstGroupingConfig()
        {
            var result = -1;
            using (var db = new DatabaseConnection(this._connectionString))
            {
                const string sql =
                   "select groupingId from DocumentGroupingConfigurations WHERE MergeProjectId = @mergeprojectId order by createdon, modifiedon";
                db.sqlexecute.Parameters.AddWithValue("@mergeprojectid", this.MergeProjectId);
                using (var reader = db.GetReader(sql, CommandType.Text))
                {
                    while (reader.Read())
                    {
                        result = reader.GetInt32(0);
                    }
                }
            }

            return result;
        }



        private Dictionary<int, cReport> GetReportSources(int mergeProjectid)
        {
            Dictionary<int, cReport> repColl;
            using (var db = new DatabaseConnection(this._connectionString))
            {
            var clsreports =
                (IReports)
                    Activator.GetObject(
                        typeof(IReports),
                        ConfigurationManager.AppSettings["ReportsServicePath"] + "/reports.rem");
                repColl = new Dictionary<int, cReport>();
                const string Sql =
                    "select mergesourceid, reportid from dbo.document_mergesources where mergeprojectid = @mergeprojectid";

            db.sqlexecute.Parameters.AddWithValue("@mergeprojectid", mergeProjectid);

                using (var reader = db.GetReader(Sql))
            {
                while (reader.Read())
                {
                    int mergesourceid = reader.GetInt32(0);
                    Guid reportid = reader.GetGuid(1);

                    cReport rep = clsreports.getReportById(this.AccountId, reportid);
                    repColl.Add(mergesourceid, rep);
                }

                reader.Close();
            }
            }

            return repColl;
        }

        public Dictionary<int, PartialReport> GetReportSourcesSummary()
        {
            Dictionary<int, PartialReport> repColl;
            using (var db = new DatabaseConnection(this._connectionString))
            {
                repColl = new Dictionary<int, PartialReport>();
                const string Sql = "dbo.GetReportSourcesSummary";

                db.sqlexecute.Parameters.AddWithValue("@mergeprojectid", this.MergeProjectId);

                using (var reader = db.GetReader(Sql, CommandType.StoredProcedure))
                {
                    while (reader.Read())
                    {
                        int mergesourceid = reader.GetInt32(0);
                        var reportid = reader.GetGuid(1);
                        var reportName = reader.GetString(2);
                        var partialReport = new PartialReport(reportid, reportName);
                        repColl.Add(mergesourceid, partialReport);
                    }

                    reader.Close();
                }
            }

            return repColl;
        }

        public List<string> GetReportSourceColumns(int reportId)
        {
            List<string> repColl;
            using (var db = new DatabaseConnection(this._connectionString))
            {
                repColl = new List<string>();
                const string Sql =
                    "select fields.description from dbo.document_mergesources inner join reportcolumns on reportcolumns.reportid = document_mergesources.reportid inner join fields on fields.fieldid = reportcolumns.fieldID where mergeprojectid = @mergeprojectid and mergesourceid = @mergesourceid";

                db.sqlexecute.Parameters.AddWithValue("@mergeprojectid", this.MergeProjectId);
                db.sqlexecute.Parameters.AddWithValue("@mergesourceid", reportId);

                using (var reader = db.GetReader(Sql))
                {
                    while (reader.Read())
                    {
                        var reportName = reader.GetString(0);
                        
                        repColl.Add(reportName);
                    }

                    reader.Close();
                }
            }

            return repColl;
        }
    }

    public class PartialReport
    {
        public Guid Reportid { get; set; }
        public string ReportName { get; set; }

        public PartialReport(Guid reportid, string reportName)
        {
            Reportid = reportid;
            ReportName = reportName;
        }
    }
}
