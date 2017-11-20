namespace SpendManagementLibrary
{
    #region Using Directives

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.Text;
    using SpendManagementLibrary.Definitions.JoinVia;
    using System.Text.RegularExpressions;

    #endregion

    /// <summary>
    /// The c report.
    /// </summary>
    [Serializable]
    public class cReport : ICloneable
    {
        #region Fields

        protected int[] arrAccessLevelRoles;

        protected ArrayList arrColumns;

        protected ArrayList arrCriteria;

        protected bool bClaimantreport;

        protected bool bIsfooter;

        protected bool bReadonly;

        protected bool bRuntimeCriteriaSet = false;

        protected cTable clsBasetable;

        protected cExportOptions clsexportoptions;

        protected AccessRoleLevel eAccessRoleLevel;

        /// <summary>
        /// The list of accessrole assigned to the Employee running the report.
        /// </summary>
        protected List<cAccessRole> accessRolesAssignedToTheEmployee;

        protected Modules eReportArea;

        protected ReportType eReportType = ReportType.None;

        protected Guid? folderID;

        protected Guid gReportid;

        protected int nAccountid;

        protected int? nEmployeeid;

        protected short nLimit;

        protected int? nSubAccountId;

        protected string sDescription;

        protected string sJoinSQL;

        protected string sReportname;

        protected string sStaticReportSQL;

        /// <summary>
        /// A <see cref="Dictionary{TKey,TValue}"/> of tablename and table alias extracted from the JoinsSql field
        /// </summary>
        private Dictionary<string, string> _joinAliases;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initialises a new instance of the <see cref="cReport"/> class.
        /// </summary>
        public cReport()
        {
        }

        public cReport(
            Guid reportId, 
            int? subaccountid, 
            string reportName, 
            string description, 
            Guid folderid, 
            cTable reportOn, 
            bool claimantReport, 
            bool readOnlyReport, 
            ArrayList columns, 
            ArrayList criteria, 
            string staticReportSql, 
            short limit, 
            Modules reportArea,
            bool useJoinVia = false, ShowChartFlag showChart = ShowChartFlag.Always)
        {
            this.gReportid = reportId;
            this.sReportname = reportName;
            this.sDescription = description;
            this.folderID = folderid;
            this.clsBasetable = reportOn;
            this.bClaimantreport = claimantReport;
            this.bReadonly = readOnlyReport;
            this.arrColumns = columns;
            this.arrCriteria = criteria;
            this.sStaticReportSQL = staticReportSql;
            this.nLimit = limit;
            this.eReportArea = reportArea;
            this.nSubAccountId = subaccountid;
            this.UseJoinVia = useJoinVia;
            this.ShowChart = showChart;
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="cReport"/> class.
        /// </summary>
        /// <param name="accountid">
        /// The accountid.
        /// </param>
        /// <param name="subaccountid">
        /// The subaccountid.
        /// </param>
        /// <param name="reportid">
        /// The reportid.
        /// </param>
        /// <param name="employeeid">
        /// The employeeid.
        /// </param>
        /// <param name="reportname">
        /// The reportname.
        /// </param>
        /// <param name="description">
        /// The description.
        /// </param>
        /// <param name="folderid">
        /// The folderid.
        /// </param>
        /// <param name="reporton">
        /// The reporton.
        /// </param>
        /// <param name="claimantreport">
        /// The claimantreport.
        /// </param>
        /// <param name="readonlyrpt">
        /// The readonlyrpt.
        /// </param>
        /// <param name="columns">
        /// The columns.
        /// </param>
        /// <param name="criteria">
        /// The criteria.
        /// </param>
        /// <param name="limit">
        /// The limit.
        /// </param>
        /// <param name="reportArea">
        /// The report area.
        /// </param>
        /// <param name="useJoinVia">
        /// True if the reports uses JOINVIAS instead of joins.
        /// </param>
        public cReport(
            int accountid, 
            int? subaccountid, 
            Guid reportid, 
            int employeeid, 
            string reportname, 
            string description, 
            Guid? folderid, 
            cTable reporton, 
            bool claimantreport, 
            bool readonlyrpt, 
            ArrayList columns, 
            ArrayList criteria, 
            short limit, 
            Modules reportArea,
            bool useJoinVia = false,
            ShowChartFlag showChart = ShowChartFlag.Always)
        {
            this.nAccountid = accountid;
            this.gReportid = reportid;
            this.nEmployeeid = employeeid;
            this.sReportname = reportname;
            this.sDescription = description;
            this.folderID = folderid;
            this.clsBasetable = reporton;
            this.arrColumns = columns;
            this.arrCriteria = criteria;
            this.bReadonly = readonlyrpt;
            this.bClaimantreport = claimantreport;
            this.nLimit = limit;
            this.eReportArea = reportArea;
            this.nSubAccountId = subaccountid;
            this.clsexportoptions = new cExportOptions(
                employeeid, 
                reportid, 
                true, 
                true, 
                true, 
                new SortedList<Guid, int>(), 
                null, 
                Guid.Empty, 
                FinancialApplication.CustomReport, 
                ",", 
                false, 
                true,
                false);
            this.UseJoinVia = useJoinVia;
            this.ShowChart = showChart;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     The access level to be applied to this report. All data, line manager or role based
        /// </summary>
        public AccessRoleLevel AccessLevel
        {
            get
            {
                return this.eAccessRoleLevel;
            }

            set
            {
                this.eAccessRoleLevel = value;
            }
        }

        /// <summary>
        ///     The list of access roles assigned to the current user.
        /// </summary>
        public List<cAccessRole> ListOfAccessRolesAssignedToTheUser
        {
            get
            {
                return this.accessRolesAssignedToTheEmployee;
            }

            set
            {
                this.accessRolesAssignedToTheEmployee = value;
            }
        }

        /// <summary>
        /// Gets or sets the access level roles.
        /// </summary>
        public int[] AccessLevelRoles
        {
            get
            {
                return this.arrAccessLevelRoles;
            }

            set
            {
                this.arrAccessLevelRoles = value;
            }
        }

        /// <summary>
        ///     Gets or sets the folder the report belongs to
        /// </summary>
        public Guid? FolderID
        {
            get
            {
                return this.folderID;
            }

            set
            {
                this.folderID = value;
            }
        }

        /// <summary>
        /// Gets or sets the join sql.
        /// </summary>
        public string JoinSQL
        {
            get
            {
                return this.sJoinSQL;
            }

            set
            {
                this.sJoinSQL = value;
            }
        }

        /// <summary>
        /// Gets or sets the limit.
        /// </summary>
        public short Limit
        {
            get
            {
                return this.nLimit;
            }

            set
            {
                this.nLimit = value;
            }
        }

        /// <summary>
        /// Gets or sets the static report sql.
        /// </summary>
        public string StaticReportSQL
        {
            get
            {
                return this.sStaticReportSQL;
            }

            set
            {
                this.sStaticReportSQL = value;
            }
        }

        /// <summary>
        /// Gets or sets the sub account id.
        /// </summary>
        public int? SubAccountID
        {
            get
            {
                return this.nSubAccountId;
            }

            set
            {
                this.nSubAccountId = value;
            }
        }

        /// <summary>
        /// Gets or sets the accountid.
        /// </summary>
        public int accountid
        {
            get
            {
                return this.nAccountid;
            }

            set
            {
                this.nAccountid = value;
            }
        }

        /// <summary>
        /// Gets the basetable.
        /// </summary>
        public cTable basetable
        {
            get
            {
                return this.clsBasetable;
            }
        }

        /// <summary>
        /// Gets a value indicating whether claimantreport.
        /// </summary>
        public bool claimantreport
        {
            get
            {
                return this.bClaimantreport;
            }
        }

        /// <summary>
        /// Gets the columns.
        /// </summary>
        public ArrayList columns
        {
            get
            {
                return this.arrColumns;
            }
        }

        /// <summary>
        /// Gets the criteria.
        /// </summary>
        public ArrayList criteria => this.arrCriteria;

        /// <summary>
        /// Gets the description.
        /// </summary>
        public string description
        {
            get
            {
                return this.sDescription;
            }
        }

        /// <summary>
        /// Gets or sets the employeeid.
        /// </summary>
        public int? employeeid
        {
            get
            {
                return this.nEmployeeid;
            }

            set
            {
                this.nEmployeeid = value;
            }
        }

        /// <summary>
        /// Gets or sets the exportoptions.
        /// </summary>
        public cExportOptions exportoptions
        {
            get
            {
                return this.clsexportoptions;
            }

            set
            {
                this.clsexportoptions = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether is footer.
        /// </summary>
        public bool isFooter
        {
            get
            {
                return this.bIsfooter;
            }

            set
            {
                this.bIsfooter = value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether readonlyrpt.
        /// </summary>
        public bool readonlyrpt
        {
            get
            {
                return this.bReadonly;
            }
        }

        /// <summary>
        /// Gets the report area.
        /// </summary>
        public Modules reportArea
        {
            get
            {
                return this.eReportArea;
            }
        }

        /// <summary>
        /// Gets the reportid.
        /// </summary>
        public Guid reportid
        {
            get
            {
                return this.gReportid;
            }
        }

        /// <summary>
        /// Gets or sets the reportname.
        /// </summary>
        public string reportname
        {
            get
            {
                return this.sReportname;
            }

            set
            {
                this.sReportname = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether runtimecriteriaset.
        /// </summary>
        public bool runtimecriteriaset
        {
            get
            {
                return this.bRuntimeCriteriaSet;
            }

            set
            {
                this.bRuntimeCriteriaSet = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this report uses JOINVIAS.
        /// If false, this report uses joins, if true, this report uses JOINVIAS.
        /// </summary>
        public bool UseJoinVia { get; set; }

        public ShowChartFlag ShowChart { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Checks whether access level filtering is appropriate for the report base
        /// </summary>
        /// <param name="baseTableId">
        /// ID of the report base table
        /// </param>
        /// <returns>
        /// True if AccessRoleLevel filtering can be performed (e.g. is related to employee and duty of care data)
        /// </returns>
        public static bool canFilterByRole(Guid baseTableId)
        {
            bool canFilter = baseTableId == new Guid(ReportTable.Claims) ||
                             baseTableId== new Guid(ReportTable.SavedExpenses) ||
                             baseTableId == new Guid(ReportTable.Cars) ||
                             baseTableId == new Guid(ReportTable.DrivingLicences) ||
                             baseTableId == new Guid(ReportTable.VehicleDocumentsView) ||
                             baseTableId == new Guid(ReportTable.VehicleDocuments) ||
                             baseTableId == new Guid(ReportTable.Employees);

            return canFilter;
        }

        public object Clone()
        {
            var formatter = new BinaryFormatter();
            cReport rpt;
            using (var stream = new MemoryStream())
            {
                formatter.Serialize(stream, this);
                formatter = new BinaryFormatter();
                stream.Seek(0, SeekOrigin.Begin);
                rpt = (cReport)formatter.Deserialize(stream);
            }

            return rpt;
        }

        public void addCriteria(cReportCriterion criteria)
        {
            this.arrCriteria.Add(criteria);
        }

        public string createReport()
        {
            string rptsql = string.Empty;
            ReportType reportType = this.getReportType();
            this._joinAliases = this.PopulateJoinAliases();

            switch (reportType)
            {
                case ReportType.Item:
                    rptsql = this.generateItemReport();
                    break;
                case ReportType.Summary:
                    rptsql = this.generateSummaryReport();
                    break;
            }

            // Replace any table names that have been "aliased"
            foreach (KeyValuePair<string, string> keyValuePair in this._joinAliases)
            {
                rptsql = rptsql.Replace(keyValuePair.Key + ".", "[" + keyValuePair.Value + "].");
                rptsql = rptsql.Replace("[" + keyValuePair.Key + "].", "[" + keyValuePair.Value + "].");
            }

            return rptsql;
        }

        public string createReportCount(ViaBranch trunk = null, JoinVia employeeJoinVia = null)
        {
            var output = new StringBuilder();
            this._joinAliases = this.PopulateJoinAliases();
            if (this.AccessLevel == AccessRoleLevel.EmployeesResponsibleFor && canFilterByRole(this.basetable.TableID))
            {
                output.Append("declare @linemanagers as LineManagers;");
            }

            output.Append("select count(*) from [dbo].[" + this.basetable.TableName + "] ");
            output.Append(this.JoinSQL);
            output.Append(this.GenerateExportHistoryJoin());
            output.Append(this.generateCriteria(trunk, employeeJoinVia));
            // Replace any table names that have been "aliased"
            foreach (KeyValuePair<string, string> keyValuePair in this._joinAliases)
            {
                output = output.Replace(keyValuePair.Key + ".", "[" + keyValuePair.Value + "].");
                output = output.Replace("[" + keyValuePair.Key + "].", "[" + keyValuePair.Value + "].");
            }
            return output.ToString();
        }

        /// <summary>
        /// Generate the SQL join that limits an export
        ///  to expense/greenlight items from a specific financial export
        /// </summary>
        /// <returns>String with the join to export history</returns>
        public string GenerateExportHistoryJoin()
        {
            if (this.IsExpenseItemFinancialExportRerun())
            {
                return " INNER JOIN dbo.exporteditems ei ON dbo.savedexpenses.expenseid = ei.expenseid ";
            }

            if (this.IsGreenLightReport() && this.IsFinancialExportRerun())
            {
                return string.Format(" INNER JOIN dbo.customEntityExportedItems ceei ON {0}.{1} = ceei.customEntityRecordID ", this.basetable.TableName, this.basetable.GetPrimaryKey().FieldName);
            }

            return string.Empty;
        }

        public string generateCriteria(ViaBranch trunk = null, JoinVia employeeJoinVia = null)
        {
            var output = new StringBuilder();
            cReportCriterion criteria;
            int groupnum = -1;
            bool hasConDetails = false;
            bool addCriteriaEndBracket = false;
            switch (this.basetable.TableID.ToString().ToUpper())
            {
                case ReportTable.ContractDetails:
                case ReportTable.SupplierDetails:
                case ReportTable.ProductDetails:
                    output.Append("[" + this.basetable.TableName + "].[subAccountID] = @subaccountid ");
                    break;
                case ReportTable.ContractProductDetails:
                case ReportTable.InvoiceDetails:
                case ReportTable.InvoiceForecasts:

                    output.Append("[contract_details].[subAccountID] = @subaccountid ");
                    break;
                case ReportTable.SupplierContacts:
                    output.Append("[supplier_details].[subAccountID] = @subaccountid ");
                    break;
            }


            if (this.IsExpenseItemFinancialExportRerun())
            {
                output.AppendFormat("WHERE ei.exporthistoryid = {0}", this.exportoptions.exporthistoryid);

                return output.ToString();
            }

            if (this.basetable.TableID == new Guid(ReportTable.SavedExpenses) && this.exportoptions != null
                && this.exportoptions.isfinancialexport && this.exportoptions.exporthistoryid == 0)
            {
                output.AppendFormat("dbo.savedexpenses.expenseid NOT IN (SELECT ei.expenseid FROM dbo.exporteditems ei INNER JOIN dbo.exporthistory eh ON eh.exporthistoryid = ei.exporthistoryid WHERE eh.financialexportid = {0})", this.exportoptions.financialexport.financialexportid);

                if (this.exportoptions.financialexport.application == FinancialApplication.ESR)
                {
                    output.Append(" AND dbo.savedexpenses.esrAssignID is not null ");
                }
            }

            // check through the report fields to see if Contract Access needs to be checked when not already a contract details report
            if (this.clsBasetable.TableID.ToString().ToUpper() != ReportTable.ContractDetails)
            {
                foreach (cReportColumn curCol in this.arrColumns)
                {
                    if (curCol.columntype == ReportColumnType.Standard)
                    {
                        var standard = (cStandardColumn)curCol;
                        if (standard.field != null
                            && standard.field.TableID.ToString().ToUpper() == ReportTable.ContractDetails)
                        {
                            hasConDetails = true;
                            break;
                        }
                    }
                }
            }

            if ((this.clsBasetable.TableID.ToString().ToUpper() == ReportTable.ContractDetails || hasConDetails))
            {
                if (output.Length > 0)
                {
                    output.Append(" AND");
                }
                // must put clause to check contract access in case they are not permitted to view contract
                output.Append($" dbo.CheckContractAccess({this.employeeid},contract_details.[ContractId], @subaccountid) > 0 ");
                //hasConDetails = false;
            }

            if (this.IsGreenLightReport())
            {
                if (this.IsFinancialExportRerun())
                {
                    output.AppendFormat("WHERE ceei.exportHistoryID = {0}", this.exportoptions.exporthistoryid);

                    return output.ToString();
                }

                if (this.exportoptions != null && this.exportoptions.isfinancialexport && this.exportoptions.exporthistoryid == 0)
                {
                    output.AppendFormat("{0}.{1} NOT IN (SELECT ceei.customEntityRecordID FROM dbo.customEntityExportedItems ceei INNER JOIN dbo.exporthistory eh ON eh.exporthistoryid = ceei.exportHistoryID WHERE eh.financialexportid = {2})", this.basetable.TableName, this.basetable.GetPrimaryKey().FieldName, this.exportoptions.financialexport.financialexportid);
                }
            }

            switch (this.AccessLevel)
            {
                case AccessRoleLevel.EmployeesResponsibleFor:
                    if (canFilterByRole(this.basetable.TableID))
                    {
                        if (output.Length > 0)
                        {
                            output.Append("and ");
                        }
                        var tableName = "[employees]";
                        if (employeeJoinVia != null)
                        {
                            tableName = "[" + ReplaceTableNameWithJoinViaAlias(employeeJoinVia, "[employees]") + "]";
                        }
                        
                        output.Append($" {tableName}.employeeid in (select employeeid from dbo.resolveReport({this.employeeid},@linemanagers))");
                    }
                    else if (this.basetable.TableID.ToString().ToLower() == ReportTable.VehicleDocuments.ToLower() || this.basetable.TableID.ToString().ToLower() == ReportTable.VehicleDocumentsView.ToLower())
                    {
                        if (output.Length > 0)
                        {
                            output.Append("and ");
                        }

                        output.Append($" cars.employeeid in (select employeeid from dbo.resolveReportForDutyOfCare({this.employeeid}))");
                    }
                    else if (this.basetable.TableID.ToString().ToLower() == ReportTable.DrivingLicences.ToLower())
                    {
                        if (output.Length > 0)
                        {
                            output.Append("and ");
                        }
                        var drivingLicenceEmployeeField = new cFields(this.accountid).GetFieldByID(new Guid(ReportFields.DrivingLicenceEmployeeId));
                        
                        output.Append($" {this.basetable.TableName}.{drivingLicenceEmployeeField.FieldName} in (select employeeid from dbo.resolveReportForDutyOfCare({this.employeeid}))");
                    }

                    break;
                case AccessRoleLevel.SelectedRoles:
                    if (canFilterByRole(this.basetable.TableID))
                    {
                        if (this.AccessLevelRoles.Length > 0)
                        {
                            if (output.Length > 0)
                            {
                                output.Append("and ");
                            }

                            var ehr = new StringBuilder();
                            string theOr = string.Empty;
                            ehr.Append(" (");
                            for (int x = 0; x < this.AccessLevelRoles.Length; x++)
                            {
                                ehr.Append(
                                    $"{theOr} dbo.employeeHasRole({this.AccessLevelRoles[x]}, [{ReplaceTableNameWithJoinViaAlias(employeeJoinVia, "employees")}].employeeid, {this.SubAccountID.Value}) = 1 ");
                                theOr = " or ";
                            }

                            ehr.Append(")");
                            output.Append(ehr);
                        }
                    }

                    break;
            }

            if (output.Length > 0 && this.criteria.Count > 0)
            {
                output.Append(" AND ");
            }

            for (int i = 0; i < this.criteria.Count; i++)
            {
                criteria = (cReportCriterion)this.criteria[i];

                if (criteria.groupnumber != groupnum)
                {
                    if (i != 0)
                    {
                        output.Append(")");
                        switch (criteria.joiner)
                        {
                            case ConditionJoiner.None:
                            case ConditionJoiner.And:
                                output.Append(" AND ");
                                break;
                            case ConditionJoiner.Or:
                                output.Append(" OR ");
                                break;
                        }
                    }

                    if (this.claimantreport && this.criteria.Count > 1)
                    {
                        output.Append("((");
                    }
                    else
                    {
                        output.Append("(");
                    }

                    groupnum = criteria.groupnumber;
                }
                else
                {
                    if (i != 0)
                    {
                        if (this.claimantreport && i == this.criteria.Count - 1)
                        {
                            output.Append(")");
                        }

                        switch (criteria.joiner)
                        {
                            case ConditionJoiner.None:
                            case ConditionJoiner.And:
                                output.Append(" AND ");
                                break;
                            case ConditionJoiner.Or:
                                output.Append(" OR ");
                                break;
                        }
                    }
                }

                // field
                bool nToOneValueList = criteria.field.FieldSource != cField.FieldSourceType.Metabase
                                         && criteria.field.FieldType == "N" && criteria.field.GetRelatedTable() != null
                                         && criteria.field.IsForeignKey;

                if ((criteria.field.ValueList || criteria.field.GenList || nToOneValueList)
                    && criteria.drilldown == false)
                {
                    var splitCriteria = new string[0];
                    cField criteriaField = criteria.field;
                    cTable criteriaTable = criteria.field.GetParentTable();
                    string aliasTable = ReplaceTableNameWithJoinViaAlias(criteria.JoinVia, criteria.field.GetParentTable().TableName);

                    if (criteria.field.FieldType == "R" || nToOneValueList)
                    {
                        criteriaField = criteriaField.GetRelatedTable().GetKeyField();
                        criteriaTable = criteria.field.GetRelatedTable();
                        aliasTable = "rel" + criteria.field.FieldName;
                    }

                    if (criteria.field.GenList && criteria.value1[0] == null)
                    {
                        criteria.value1[0] = string.Empty;
                    }

                    if (criteria.value1[0] != null)
                    {
                        splitCriteria = criteria.value1[0].ToString().Split(",".ToCharArray());
                    }

                    if (splitCriteria.Length > 0)
                    {
                        output.Append("(");
                        if (criteria.condition == ConditionType.ContainsData && criteria.field.StringField())
                        {
                            output.Append(" ISNULL(");
                        }
                        for (int x = 0; x < splitCriteria.Length; x++)
                        {
                            int keyValue;
                            var criteriaIsString = !int.TryParse(splitCriteria[x], out keyValue);
                            if (criteriaField.ValueList || criteriaIsString)
                            {
                                output.Append("[" + criteriaTable.TableName + "].[" + criteriaField.FieldName + "] ");
                            }
                            else
                            {
                                output.Append("[" + aliasTable + "].[" + criteriaTable.GetPrimaryKey().FieldName + "] ");
                            }

                            switch (criteria.condition)
                            {
                                case ConditionType.Equals:
                                    if (criteriaField.ValueList || criteriaIsString)
                                    {
                                        output.Append(" = '" + splitCriteria[x] + "' ");
                                    }
                                    else
                                    {
                                        output.Append(" = " + splitCriteria[x] + " ");
                                    }

                                    break;
                                case ConditionType.DoesNotEqual:
                                    if (criteriaField.ValueList || criteriaIsString)
                                    {
                                        output.Append(" <> '" + splitCriteria[x] + "' ");
                                    }
                                    else
                                    {
                                        output.Append(" <> " + splitCriteria[x] + " ");
                                    }

                                    break;
                                case ConditionType.ContainsData:
                                    if (criteriaField.StringField())
                                    {
                                        output.Append(", '') <> '' ");
                                    }
                                    else
                                    {
                                        output.Append("is not null ");
                                    }
                                    
                                    break;
                                case ConditionType.DoesNotContainData:
                                    output.Append("is null ");
                                    break;
                            }

                            if (x < (splitCriteria.Length - 1))
                            {
                                switch (criteria.condition)
                                {
                                    case ConditionType.Equals:
                                    case ConditionType.ContainsData:
                                        output.Append(" OR ");
                                        break;
                                    case ConditionType.DoesNotContainData:
                                    case ConditionType.DoesNotEqual:
                                        output.Append(" AND ");
                                        break;
                                }
                            }
                        }

                        output.Append(") ");
                        addCriteriaEndBracket = true;
                    }
                    else
                    {
                        if (criteriaField.ValueList)
                        {
                            if (criteria.condition == ConditionType.ContainsData && criteria.field.StringField())
                            {
                                output.Append(" ISNULL(");
                            }

                            if (criteria.JoinVia == null)
                            {
                                output.Append(
                                    "[" + criteriaTable.TableName + "].[" + criteriaField.FieldName + "]");
                            }
                            else
                            {
                                output.Append(
                                    "[" + criteria.JoinVia.TableAlias + "].[" + criteria.field.FieldName
                                    + "] ");
                            }

                            switch (criteria.condition)
                            {
                                case ConditionType.ContainsData:
                                    if (criteriaField.StringField())
                                    {
                                        output.Append(", '') <> '' ");
                                    }
                                    else
                                    {
                                        output.Append("is not null ");
                                    }

                                    addCriteriaEndBracket = true;
                                    break;
                                case ConditionType.DoesNotContainData:
                                    output.Append("is null ");
                                    addCriteriaEndBracket = true;
                                    break;
                            }
                        }

                        if (output.ToString().EndsWith(" OR "))
                        {
                            output.Remove(output.Length - 4, 4);
                        }
                        else if (output.ToString().EndsWith(" OR ("))
                        {
                            output.Remove(output.Length - 5, 5);
                        }
                        else if (output.ToString().EndsWith(" AND "))
                        {
                            output.Remove(output.Length - 5, 5);
                        }
                        else if (output.ToString().EndsWith(" AND ("))
                        {
                            output.Remove(output.Length - 6, 6);
                        }
                    }
                }
                else
                {
                    addCriteriaEndBracket = true;

                    const string FUNC_START = "dbo.GetHoursAndMinutesFromDateAsInt(";
                    const string FUNC_END = ") ";

                    

                    if (criteria.condition == ConditionType.ContainsData && criteria.field.StringField())
                    {
                        output.Append(" ISNULL(");
                    }

                    switch (criteria.field.FieldType)
                    {
                        case "FD":
                        case "FU":
                        case "FS":
                        case "FI":
                        case "FC":
                        case "FX":
                            output.Append(ReplaceTableNameWithJoinViaAlias(criteria.JoinVia, criteria.field, criteria.field.FieldName));
                            break;
                        case "T":
                            output.Append(FUNC_START);
                            if (criteria.JoinVia == null)
                            {
                                output.Append(
                                    "[" + criteria.field.GetParentTable().TableName + "].[" + criteria.field.FieldName
                                    + "]");
                            }
                            else
                            {
                                output.Append(
                                     "[" + criteria.JoinVia.TableAlias + "].[" + criteria.field.FieldName
                                    + "] ");
                            }
                            output.Append(FUNC_END);
                            break;
                        default:
                            if (criteria.JoinVia == null)
                            {
                                output.Append(
                                    "[" + criteria.field.GetParentTable().TableName + "].[" + criteria.field.FieldName
                                    + "]");
                            }
                            else
                            {
                                output.Append(
                                    "[" + criteria.JoinVia.TableAlias + "].[" + criteria.field.FieldName
                                    + "] ");
                            }
                            break;
                    }

                    switch (criteria.condition)
                    {
                        case ConditionType.Equals:
                        case ConditionType.On:
                            if (criteria.field.FieldType == "T")
                            {
                                output.Append("= " + FUNC_START + "@value1_" + criteria.order + FUNC_END);
                            }
                            else
                            {
                                output.Append("= @value1_" + criteria.order);
                            }

                            break;
                        case ConditionType.DoesNotEqual:
                        case ConditionType.NotOn:
                            if (criteria.field.FieldType == "T")
                            {
                                output.Append("<> " + FUNC_START + "@value1_" + criteria.order + FUNC_END);
                            }
                            else
                            {
                                output.Append("<> @value1_" + criteria.order);
                            }

                            break;
                        case ConditionType.GreaterThan:
                        case ConditionType.After:
                            if (criteria.field.FieldType == "T")
                            {
                                output.Append("> " + FUNC_START + "@value1_" + criteria.order + FUNC_END);
                            }
                            else
                            {
                                output.Append("> @value1_" + criteria.order);
                            }

                            break;
                        case ConditionType.LessThan:
                        case ConditionType.Before:
                            if (criteria.field.FieldType == "T")
                            {
                                output.Append("< " + FUNC_START + "@value1_" + criteria.order + FUNC_END);
                            }
                            else
                            {
                                output.Append("< @value1_" + criteria.order);
                            }

                            break;
                        case ConditionType.GreaterThanEqualTo:
                        case ConditionType.OnOrAfter:
                            if (criteria.field.FieldType == "T")
                            {
                                output.Append(">= " + FUNC_START + "@value1_" + criteria.order + FUNC_END);
                            }
                            else
                            {
                                output.Append(">= @value1_" + criteria.order);
                            }

                            break;
                        case ConditionType.LessThanEqualTo:
                        case ConditionType.OnOrBefore:
                            if (criteria.field.FieldType == "T")
                            {
                                output.Append("<= " + FUNC_START + "@value1_" + FUNC_END + criteria.order);
                            }
                            else
                            {
                                output.Append("<= @value1_" + criteria.order);
                            }

                            break;
                        case ConditionType.Like:
                            output.Append("like @value1_" + criteria.order);
                            break;
                        case ConditionType.NotLike:
                            output.Append("not like @value1_" + criteria.order);
                            break;
                        case ConditionType.ContainsData:
                            if (criteria.field.StringField())
                            {
                                output.Append(", '') <> '' ");
                            }
                            else
                            {
                                output.Append("is not null ");
                            }
                            break;
                        case ConditionType.DoesNotContainData:
                            output.Append("is null");
                            break;
                        case ConditionType.Between:
                        case ConditionType.Yesterday:
                        case ConditionType.Today:
                        case ConditionType.Tomorrow:
                        case ConditionType.Last7Days:
                        case ConditionType.LastTaxYear:
                        case ConditionType.LastFinancialYear:
                        case ConditionType.LastMonth:
                        case ConditionType.LastWeek:
                        case ConditionType.LastXDays:
                        case ConditionType.LastXMonths:
                        case ConditionType.LastXWeeks:
                        case ConditionType.LastXYears:
                        case ConditionType.LastYear:
                        case ConditionType.Next7Days:
                        case ConditionType.NextTaxYear:
                        case ConditionType.NextFinancialYear:
                        case ConditionType.NextMonth:
                        case ConditionType.NextWeek:
                        case ConditionType.NextXDays:
                        case ConditionType.NextXMonths:
                        case ConditionType.NextXWeeks:
                        case ConditionType.NextXYears:
                        case ConditionType.NextYear:
                        case ConditionType.ThisTaxYear:
                        case ConditionType.ThisFinancialYear:
                        case ConditionType.ThisMonth:
                        case ConditionType.ThisWeek:
                        case ConditionType.ThisYear:
                            if (criteria.field.FieldType == "T")
                            {
                                output.Append(
                                    "between " + FUNC_START + "@value1_" + criteria.order + FUNC_END + " and " + FUNC_START
                                    + "@value2_" + criteria.order + FUNC_END);
                            }
                            else
                            {
                                output.Append("between @value1_" + criteria.order + " and @value2_" + criteria.order);
                            }

                            break;
                        case ConditionType.OnOrAfterToday:
                            output.Append(">= @value1_" + criteria.order);
                            break;
                        case ConditionType.OnOrBeforeToday:
                            output.Append("<= @value1_" + criteria.order);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }

            if (output.Length > 0)
            {
                output.Insert(0, "WHERE ");
            }

            if (this.arrCriteria.Count != 0)
            {
                // This can only be added if the criteria can be found
                if (addCriteriaEndBracket)
                {
                    output.Append(")");
                }
            }

            return output.ToString();
        }

        public string generateItemReport()
        {
            var output = new StringBuilder();

            output.Append(this.generateItemColumnSQL());
            output.Append(this.JoinSQL);
            output.Append(this.GenerateExportHistoryJoin());
            output.Append(this.generateCriteria());
            output.Append(this.generateOrder());
            Debug.WriteLine(output.ToString());
            return output.ToString();
        }

        public string generateOrder()
        {
            var output = new StringBuilder();
            for (int i = 0; i < this.columns.Count; i++)
            {
                var column = (cReportColumn)this.columns[i];
                if (column.columntype == ReportColumnType.Standard)
                {
                    var standard = (cStandardColumn)column;
                    if (standard.funcavg || standard.funccount || standard.funcmax || standard.funcmin
                        || standard.funcsum)
                    {
                        string function = string.Empty;
                        if (standard.funcsum)
                        {
                            function = "SUM";
                        }
                        else if (standard.funcavg)
                        {
                            function = "AVG";
                        }
                        else if (standard.funccount)
                        {
                            function = "COUNT";
                        }
                        else if (standard.funcmax)
                        {
                            function = "MAX";
                        }
                        else if (standard.funcmin)
                        {
                            function = "MIN";
                        }

                        switch (column.sort)
                        {
                            case ColumnSort.Ascending:
                                switch (standard.field.FieldType)
                                {
                                    case "FS":
                                    case "FD":
                                    case "FI":
                                    case "FU":
                                    case "FC":
                                        output.Append(function + "(" + standard.field.FieldName + ") asc, ");
                                        break;
                                    case "R":
                                        output.Append(
                                            function + "([rel" + standard.field.FieldName + "].["
                                            + standard.field.GetRelatedTable().GetKeyField().FieldName + "]) asc, ");
                                        break;
                                    default:
                                        output.Append(
                                            function + "([" + this.GetTableNameFromStandardColumn(standard) + "].["
                                            + standard.field.FieldName + "]) asc, ");
                                        break;
                                }

                                break;
                            case ColumnSort.Descending:
                                switch (standard.field.FieldType)
                                {
                                    case "FS":
                                    case "FD":
                                    case "FI":
                                    case "FU":
                                    case "FC":
                                        output.Append(function + "(" + standard.field.FieldName + ") desc, ");
                                        break;
                                    case "R":
                                        output.Append(
                                            function + "([rel" + standard.field.FieldName + "].["
                                            + standard.field.GetRelatedTable().GetKeyField().FieldName + "]) desc, ");
                                        break;
                                    default:
                                        output.Append(
                                            function + "([" + this.GetTableNameFromStandardColumn(standard) + "].["
                                            + standard.field.FieldName + "]) desc, ");
                                        break;
                                }

                                break;
                        }
                    }
                    else
                    {
                        if (column.sort != ColumnSort.None)
                        {
                            string sortFieldStr;
                            switch (standard.field.FieldType)
                            {
                                case "R":
                                    sortFieldStr = "[rel" + standard.field.FieldName + "].["
                                                   + standard.field.GetRelatedTable().GetKeyField().FieldName + "] ";
                                    break;
                                case "FC":
                                case "FD":
                                case "FI":
                                case "FS":
                                case "FU":
                                case "FX":
                                    sortFieldStr = standard.field.FieldName + " ";
                                    break;
                                default:
                                    sortFieldStr = "[" + this.GetTableNameFromStandardColumn(standard) + "].["
                                                   + standard.field.FieldName + "] ";
                                    break;
                            }

                            if (!output.ToString().Contains(sortFieldStr))
                            {
                                switch (column.sort)
                                {
                                    case ColumnSort.Ascending:
                                        output.Append(sortFieldStr + "asc, ");
                                        break;
                                    case ColumnSort.Descending:
                                        output.Append(sortFieldStr + "desc, ");
                                        break;
                                }
                            }
                        }
                    }
                }
            }

            if (output.Length != 0)
            {
                output.Remove(output.Length - 2, 2);
                output.Insert(0, " ORDER BY ");
            }

            return output.ToString();
        }

        public ArrayList getGroupableColumns()
        {
            var groupablecolumns = new ArrayList();

            for (int i = 0; i < this.columns.Count; i++)
            {
                var column = (cReportColumn)this.columns[i];
                if (column.columntype == ReportColumnType.Standard)
                {
                    var standard = (cStandardColumn)column;
                    if (standard.funcsum == false && standard.funcmax == false && standard.funcmin == false
                        && standard.funcavg == false && standard.funccount == false)
                    {
                        groupablecolumns.Add(standard);
                    }
                }
            }

            return groupablecolumns;
        }

        public cReportColumn getReportColumnById(Guid id)
        {
            return this.arrColumns.Cast<cReportColumn>().FirstOrDefault(column => column.reportcolumnid == id);
        }

        public ReportType getReportType()
        {
            if (this.eReportType == ReportType.None)
            {

                // if any columns contain agg function must be summary
                var type = getReportType(this.columns);

                this.eReportType = type;
            }

            return this.eReportType;         
        }

        public static ReportType getReportType(ArrayList columns)
        {
            // if any columns contain agg function must be summary
            var type = ReportType.Item;
            for (int i = 0; i < columns.Count; i++)
            {
                var column = (cReportColumn)columns[i];
                if (column.columntype == ReportColumnType.Standard)
                {
                    var standard = (cStandardColumn)column;
                    if (standard.funcmin || standard.funcmax || standard.funccount || standard.funcavg
                        || standard.funcsum)
                    {
                        type = ReportType.Summary;
                        break;
                    }
                }
            }

            return type;
        }

        public bool hasRuntimeCriteria()
        {
            for (int i = 0; i < this.columns.Count; i++)
            {
                var column = (cReportColumn)this.columns[i];
                if (column.columntype == ReportColumnType.Static)
                {
                    var staticcol = (cStaticColumn)column;
                    if (staticcol.runtime)
                    {
                        return true;
                    }
                }
            }

            return this.arrCriteria.Cast<cReportCriterion>().Any(criterion => criterion.runtime);
        }

        public void setReportID(Guid id)
        {
            this.gReportid = id;
        }

        public void swapCriteria(ArrayList newCriteria)
        {
            this.arrCriteria.Clear();
            for (int i = 0; i < newCriteria.Count; i++)
            {
                this.arrCriteria.Add(newCriteria[i]);
            }
        }

        public void updateColumns(ArrayList columns)
        {
            this.arrColumns = columns;
        }

        public void updateCriteria(ArrayList criteria)
        {
            this.arrCriteria = criteria;
        }

        public void updateLimit(short limit)
        {
            this.nLimit = limit;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Get restricted value format to display in reports view.
        /// </summary>
        /// <param name="columnId">
        /// The column name whcih has to be hidden from the view.
        /// </param>
        /// <returns>
        /// The <see cref="string"/> output format to display a column as restricted.
        /// </returns>
        private static string GetRestrictedValueFormat(int columnId)
        {
            return "'Restricted' AS [" + columnId + "]";
        }

        private string generateGroupBySQL()
        {
            ArrayList groupcolumns = this.getGroupableColumns();

            var output = new StringBuilder();
            for (int i = 0; i < groupcolumns.Count; i++)
            {
                var column = (cReportColumn)groupcolumns[i];
                if (column.columntype == ReportColumnType.Standard)
                {
                    var standard = (cStandardColumn)column;

                    if (standard.field.FieldType == "FS" || standard.field.FieldType == "FD"
                        || standard.field.FieldType == "FI" || standard.field.FieldType == "FU"
                        || standard.field.FieldType == "FC"
                        || (standard.field.ValueList && standard.field.FieldType == "N"))
                    {
                        var fieldname = standard.field.FieldName;
                        if (standard.JoinVia != null)
                        {
                            fieldname = ReplaceTableNameWithJoinViaAlias(standard, fieldname);
                        }

                        output.Append(fieldname + ", ");
                    }
                    else
                    {
                        if (standard.field.FieldType == "R")
                        {
                            output.Append(
                                "[rel" + standard.field.FieldName + "].["
                                + standard.field.GetRelatedTable().GetKeyField().FieldName + "], ");
                        }
                        else
                        {
                            var fieldname = standard.field.FieldName;
                            if (standard.JoinVia != null)
                            {
                                fieldname = ReplaceTableNameWithJoinViaAlias(standard, fieldname);
                            }

                            output.Append(
                                "[" + this.GetTableNameFromStandardColumn(standard) + "].[" + fieldname + "], ");
                        }
                    }
                }
            }

            if (output.Length != 0)
            {
                output.Remove(output.Length - 2, 2);
                output.Insert(0, " GROUP BY ");
            }

            return output.ToString();
        }

        /// <summary>
        /// Check if the reporting field is accessible
        /// </summary>
        /// <param name="listOfAccessRoleAssigned">
        /// The list of accessrole assigned to the user.
        /// </param>
        /// <param name="fieldId">
        /// The Field ID of the reporting field.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/> if field is accessible.
        /// </returns>
        private bool IsFieldDataAccessible(List<cAccessRole> listOfAccessRoleAssigned, Guid fieldId)
        {
            if (listOfAccessRoleAssigned != null)
            {
                foreach (var currentAccessRole in listOfAccessRoleAssigned)
                {
                    if (currentAccessRole.ExclusionType == 0 && currentAccessRole.reportableFieldsEnabled.Contains(fieldId))
                    {
                        return false;
                    }

                    if (currentAccessRole.ExclusionType == 1)
                    {
                        if (currentAccessRole.reportableFieldsEnabled.Contains(fieldId))
                        {
                            return true;
                        }

                        return false;
                    }
                }
            }

            return true;
        }

        private string generateItemColumnSQL()
        {
            var output = new StringBuilder();
           
            switch (this.AccessLevel)
            {
                case AccessRoleLevel.EmployeesResponsibleFor:
                    if (canFilterByRole(this.basetable.TableID))
                    {
                        output.Append("declare @linemanagers as LineManagers;");
                    }

                    break;
                case AccessRoleLevel.SelectedRoles:

                    break;
            }

            output.Append("select ");

            if (this.isFooter)
            {
                output.Append(" TOP 1");
            }

            if (this.Limit > 0)
            {
                output.Append(" TOP " + this.Limit + " ");
            }

            // determine report type

            // if it's a summary we need to know how many groups to create;
            for (int i = 0; i < this.columns.Count; i++)
            {
                var column = (cReportColumn)this.columns[i];
                var skipComma = false;
                switch (column.columntype)
                {
                    case ReportColumnType.Standard:
                        var standard = (cStandardColumn)column;

                        // don't crash out if the field is null
                        if (standard.field != null)
                        {
                            if (this.IsFieldDataAccessible(
                                this.accessRolesAssignedToTheEmployee,
                                standard.field.FieldID))
                            {
                                if (standard.field.ValueList)
                                {
                                    if (standard.field.ListItems.Count > 0)
                                    {
                                        output.Append("[" + standard.columnid + "] = CASE ");
                                        foreach (KeyValuePair<object, string> kvp in standard.field.ListItems)
                                        {
                                            if (standard.JoinVia != null)
                                            {
                                                output.Append(
                                                    " WHEN [" + standard.JoinVia.TableAlias + "].["
                                                    + standard.field.FieldName + "] = " + kvp.Key + " THEN '"
                                                    + kvp.Value.ToString().Replace("'", "''") + "' ");
                                            }
                                            else
                                            {
                                                output.Append(
                                                    " WHEN [" + standard.field.GetParentTable().TableName + "].["
                                                    + standard.field.FieldName + "] = " + kvp.Key + " THEN '"
                                                    + kvp.Value.ToString().Replace("'", "''") + "' ");
                                            }
                                        }
                                        // put else in to handle if there are no list items. Will error with just CASE END
                                        output.Append("ELSE '' ");
                                        output.Append("END");
                                    }
                                    else
                                    {
                                            output.Append("'' AS [" + standard.columnid + "]");
                                    }
                                }
                                else
                                {
                                    string fieldname;
                                    switch (standard.field.FieldType)
                                    {
                                        case "FD":
                                        case "FS":
                                        case "FU":
                                        case "FI":
                                        case "FC":
                                        case "FX":
                                            fieldname = standard.field.FieldName + " as [" + standard.columnid + "]";
                                            if (standard.JoinVia != null)
                                            {
                                                fieldname = ReplaceTableNameWithJoinViaAlias(standard, fieldname);
                                            }

                                            output.Append(fieldname);

                                            break;
                                        case "R":
                                            output.Append(
                                                "[rel" + standard.field.FieldName + "].["
                                                + standard.field.GetRelatedTable().GetKeyField().FieldName
                                                + "] AS [" + standard.columnid + "]");

                                            break;
                                        case "X":
                                        case "Y":
                                             fieldname = standard.field.FieldName;
                                                if (standard.JoinVia != null)
                                                {
                                                    fieldname = ReplaceTableNameWithJoinViaAlias(standard, fieldname);
                                                }

                                                output.AppendFormat(
                                                    "CAST(CASE [{0}].[{1}] WHEN 1 THEN 1 WHEN 0 THEN 0 ELSE NULL END AS BIT) AS [{2}]",
                                                    this.GetTableNameFromStandardColumn(standard),
                                                    fieldname,
                                                    standard.columnid);

                                            break;
                                        default:
                                            fieldname = standard.field.FieldName;
                                            if (standard.JoinVia != null)
                                            {
                                                fieldname = ReplaceTableNameWithJoinViaAlias(standard, fieldname);
                                            }

                                            output.AppendFormat(
                                                "[{0}].[{1}] AS [{2}]",
                                                this.GetTableNameFromStandardColumn(standard),
                                                fieldname,
                                                standard.columnid);

                                            break;
                                    }
                                }
                            }
                            else
                            {
                                output.Append(GetRestrictedValueFormat(standard.columnid));
                            }
                        }
                        else
                        {
                            skipComma = true;
                        }

                        break;
                    case ReportColumnType.Static:
                        var staticcol = (cStaticColumn)column;

                        output.Append("'" + staticcol.literalvalue.Replace("'", "''") + "'");

                        output.Append(" AS [" + staticcol.columnid + "]");
                        break;
                    case ReportColumnType.Calculated:
                        var calculatedcol = (cCalculatedColumn)column;
                        output.Append("null AS [" + calculatedcol.columnid + "]");
                        break;
                }

                if (!skipComma)
                {
                    output.Append(", ");
                }
            }

            output.Remove(output.Length - 2, 2);

            // get the base table
            output.Append(" from [" + this.basetable.TableName + "]");

            return output.ToString();
        }

        /// <summary>
        /// The generate summary column sql.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string generateSummaryColumnSQL()
        {
            var output = new StringBuilder();

            if (this.AccessLevel == AccessRoleLevel.EmployeesResponsibleFor && canFilterByRole(this.basetable.TableID))
            {
                output.Append("declare @linemanagers as LineManagers;");
            }

            output.Append("select ");

            if (this.isFooter)
            {
                output.Append(" TOP 1");
            }

            if (this.Limit > 0)
            {
                output.Append(" TOP " + this.Limit + " ");
            }

            // determine report type

            // if it's a summary we need to know how many groups to create;
            for (int i = 0; i < this.columns.Count; i++)
            {
                var column = (cReportColumn)this.columns[i];
                switch (column.columntype)
                {
                    case ReportColumnType.Standard:
                        var standard = (cStandardColumn)column;
                        if (this.IsFieldDataAccessible(this.accessRolesAssignedToTheEmployee, standard.field.FieldID))
                        {
                            if (standard.field.ValueList)
                            {
                                if (standard.field.ListItems.Count > 0)
                                {
                                    output.Append("[" + standard.columnid);
                                    if (standard.funccount)
                                    {
                                        output.Append("_COUNT");
                                    }

                                    output.Append("] = ");
                                    if (standard.funccount)
                                    {
                                        output.Append("COUNT(");
                                    }

                                    output.Append("CASE ");

                                    foreach (KeyValuePair<object, string> kvp in standard.field.ListItems)
                                    {
                                        output.Append(
                                            " WHEN [" + this.GetTableNameFromStandardColumn(standard) + "].["
                                            + standard.field.FieldName + "] = " + kvp.Key + " THEN '"
                                            + kvp.Value.Replace("'", "''") + "' ");
                                    }

                                    output.Append("ELSE '' ");
                                    output.Append("END");
                                    if (standard.funccount)
                                    {
                                        output.Append(")");
                                    }
                                }
                                else
                                {
                                    output.Append("'' AS [" + standard.columnid + "]");
                                }
                            }
                            else
                            {
                                if (standard.funcsum || standard.funcmax || standard.funcmin || standard.funcavg
                                    || standard.funccount)
                                {
                                    if (standard.funcsum)
                                    {
                                        AddFunctionColumnToOutput(standard, output, "SUM");
                                    }

                                    if (standard.funcavg)
                                    {
                                        AddFunctionColumnToOutput(standard, output, "AVG");
                                    }

                                    if (standard.funccount)
                                    {
                                        AddFunctionColumnToOutput(standard, output, "COUNT");
                                    }

                                    if (standard.funcmax)
                                    {
                                        AddFunctionColumnToOutput(standard, output, "MAX");
                                    }

                                    if (standard.funcmin)
                                    {
                                        AddFunctionColumnToOutput(standard, output, "MIN");
                                    }

                                    output.Remove(output.Length - 1, 1);
                                }
                                else
                                {
                                    switch (standard.field.FieldType)
                                    {
                                        case "FD":
                                        case "FS":
                                        case "FU":
                                        case "FI":
                                        case "FC":
                                        case "FX":
                                            var fieldname =
                                                standard.field.FieldName + " as [" + standard.columnid + "]";
                                            if (standard.JoinVia != null)
                                            {
                                                fieldname = ReplaceTableNameWithJoinViaAlias(standard, fieldname);
                                            }

                                            output.Append(fieldname);

                                            break;
                                        case "R":
                                            output.Append(
                                                "[rel" + standard.field.FieldName + "].["
                                                + standard.field.GetRelatedTable().GetKeyField().FieldName
                                                + "] AS [" + standard.columnid + "]");
                                            break;
                                        default:
                                            output.Append(
                                                "[" + this.GetTableNameFromStandardColumn(standard) + "].["
                                                + standard.field.FieldName + "] AS [" + standard.columnid + "]");
                                            break;
                                    }
                                }
                            }
                        }
                        else
                        {
                            output.Append(GetRestrictedValueFormat(standard.columnid));
                        }

                        break;
                    case ReportColumnType.Static:
                        var staticcol = (cStaticColumn)column;

                        output.Append("'" + staticcol.literalvalue + "'");

                        output.Append(" AS [" + staticcol.columnid + "]");
                        break;
                    case ReportColumnType.Calculated:
                        var calculatedcol = (cCalculatedColumn)column;
                        output.Append("null AS [" + calculatedcol.columnid + "]");
                        break;
                }

                output.Append(", ");
            }

            output.Remove(output.Length - 2, 2);

            // get the base table
            output.Append(" from [" + this.basetable.TableName + "]");
            return output.ToString();
        }

        /// <summary>
        /// Add a Function() type column to the output object.
        /// </summary>
        /// <param name="standard"></param>
        /// <param name="output"></param>
        /// <param name="function"></param>
        private void AddFunctionColumnToOutput(cStandardColumn standard, StringBuilder output, string function)
        {
            switch (standard.field.FieldType)
            {
                case "FC":
                case "FD":
                case "FI":
                case "FS":
                case "FU":
                case "FX":
                    var fieldName = standard.field.FieldName;
                    if (standard.JoinVia != null)
                    {
                        fieldName = ReplaceTableNameWithJoinViaAlias(standard, fieldName);
                    }

                    output.AppendFormat(
                        "{0}({1}) AS [{2}_{0}],",function,  fieldName, standard.columnid);
                    break;
                default:
                    output.AppendFormat(
                        "{0}([{1}].[{2}]) AS [{3}_{0}],", function, this.GetTableNameFromStandardColumn(standard), standard.field.FieldName, standard.columnid);
                    break;
            }
        }

        /// <summary>
        /// The generate summary report.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string generateSummaryReport()
        {
            var output = new StringBuilder();

            output.Append(this.generateSummaryColumnSQL());

            // output.Append(createJoinSQL());
            output.Append(this.JoinSQL);
            output.Append(this.GenerateExportHistoryJoin());
            output.Append(this.generateCriteria());
            output.Append(this.generateGroupBySQL());
            output.Append(this.generateOrder());
            return output.ToString();
        }

        /// <summary>
        /// Create a <see cref="Dictionary{TKey,TValue}"/> where the key is the table name and the value is the (first) alias used in the join
        /// </summary>
        /// <returns>A dictionary of tablename , alias name</returns>
        private Dictionary<string, string> PopulateJoinAliases()
        {
            var result = new Dictionary<string, string>();
            var regex = new Regex(@"JOIN \[.*? AS .*?\]");
            var matches = regex.Matches(this.JoinSQL);
            foreach (Match match in matches)
            {
                var split = match.Value.Replace(" AS ", ",").Replace("JOIN ", string.Empty).Replace("[", string.Empty).Replace("]", string.Empty).Split(',');
                if (result.ContainsKey(split[0]) || split.Length != 2)
                {
                    continue;
                }

                if (split[0] == "employees" && this.basetable.TableID == new Guid(ReportTable.Employees))
                {
                    continue;
                }

                result.Add(split[0], split[1]);
            }

            return result;
        }

        private string GetTableNameFromStandardColumn(cStandardColumn column)
        {
            return column.JoinVia != null ? column.JoinVia.TableAlias : column.field.GetParentTable().TableName;
        }

        /// <summary>
        /// Indicates whether or not the currently running report is a financial export based on expense items from the history
        /// </summary>
        /// <returns>Indication of truthyness</returns>
        private bool IsExpenseItemFinancialExportRerun()
        {
            return this.basetable.TableID == new Guid(ReportTable.SavedExpenses) // expense items table
                && this.IsFinancialExportRerun();
        }

        /// <summary>
        /// Indicates whether or not the currently running report is based on a greenlight table
        /// </summary>
        /// <returns>Indication of truthyness</returns>
        private bool IsGreenLightReport()
        {
            return (this.basetable.TableName.Length >= 7 && this.basetable.TableName.Substring(0, 7) == "custom_") // a custom entity table name format
                && (this.basetable.GetPrimaryKey().FieldName.Length >= 3 && this.basetable.GetPrimaryKey().FieldName.Substring(0, 3) == "att"); // primary key looks like an attribute
        }

        /// <summary>
        /// Indicates whether or not the currently running report is a financial export rerun from the history
        /// </summary>
        /// <returns>Indication of truthyness</returns>
        private bool IsFinancialExportRerun()
        {
            return this.exportoptions != null && this.exportoptions.isfinancialexport && this.exportoptions.exporthistoryid > 0;
        }

        #endregion

        #region Public Static Methods

        /// <summary>
        /// When compiling the SQL, if a field is a function field, replace the tablename with the Join Via Alias.
        /// </summary>
        /// <param name="queryField">Teh query field to adjust for join via</param>
        /// <param name="fieldname">The field name to replace</param>
        /// <returns>The field name with table alias</returns>
        public static string ReplaceTableNameWithJoinViaAlias(cQueryField queryField, string fieldname)
        {
            return ReplaceTableNameWithJoinViaAlias(queryField.JoinVia, queryField.field, fieldname);
        }

        /// <summary>
        /// When compiling the SQL, if a field is a function field, replace the tablename with the Join Via Alias.
        /// </summary>
        /// <param name="joinVia">The join via for this column</param>
        /// <param name="field">The <see cref="cField"/>for this column</param>
        /// <param name="fieldname">The field name to replace</param>
        /// <returns>The alias table name</returns>
        public static string ReplaceTableNameWithJoinViaAlias(JoinVia joinVia, cField field, string fieldname)
        {
            if (joinVia == null)
            {
                return fieldname;
            }

            var tableName = field.GetParentTable().TableName.ToLower();
            fieldname = fieldname.ToLower();
            if (fieldname.Contains("[" + tableName + "]"))
            {
                fieldname = fieldname.Replace(tableName + ".", joinVia.TableAlias + ".");
            }
            else
            {
                fieldname = fieldname.Replace(tableName + ".", "[" + joinVia.TableAlias + "].");
            }

            return fieldname;
        }

        /// <summary>
        /// When compiling the SQL, if a field is a function field, replace the tablename with the Join Via Alias.
        /// </summary>
        /// <param name="standard">The <see cref="cStandardColumn"/>for this column</param>
        /// <param name="fieldname">The field name to display</param>
        /// <returns>The alias table name</returns>
        public static string ReplaceTableNameWithJoinViaAlias(cStandardColumn standard, string fieldname)
        {
            return ReplaceTableNameWithJoinViaAlias(standard.JoinVia, standard.field, fieldname);
        }

        /// <summary>
        /// The replace table name with join via alias.
        /// </summary>
        /// <param name="joinVia">
        /// The <see cref="JoinVia"/> (if any).
        /// </param>
        /// <param name="tableName">
        /// The table name to replace.
        /// </param>
        /// <returns>
        /// The <see cref="string"/> The Alias table if possible.
        /// </returns>
        public static string ReplaceTableNameWithJoinViaAlias(JoinVia joinVia, string tableName)
        {
            if (joinVia == null)
            {
                return tableName;
            }

            return tableName.Replace(tableName, joinVia.TableAlias);
            
        }

        #endregion

        public void resetReportType()
        {
            this.eReportType = ReportType.None;
        }
    }

    /// <summary>
    /// The report area.
    /// </summary>
    public enum ReportArea
    {
        Global = 1, 

        Custom = 2, 

        All = 3
    }
}