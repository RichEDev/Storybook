namespace SpendManagementLibrary
{
    using System;
    using System.Text.RegularExpressions;
    using SpendManagementLibrary.Definitions.JoinVia;

    [Serializable()]
    public class cReportColumn
    {
        protected Guid gColumnid;
        protected Guid gReportcolumnid;
        protected Guid gReportid;
        protected ReportColumnType ctColumntype;
        protected ColumnSort csSort;
        protected int nOrder;
        protected bool bHidden;
        protected bool bRuntime;
        protected bool bSystem;
        protected string sNotes;

        #region properties
        public int columnid
        {
            get { return nOrder; }
        }
        public Guid reportcolumnid
        {
            get { return gReportcolumnid; }
            set { gReportcolumnid = value; }
        }
        public Guid reportid
        {
            get { return gReportid; }
        }
        public ReportColumnType columntype
        {
            get { return ctColumntype; }
        }
        public ColumnSort sort
        {
            get { return csSort; }
            set
            {
                this.csSort = value;
            }
        }

        public int order
        {
            get { return nOrder; }
        }
        public bool hidden
        {
            get { return bHidden; }
        }
        public bool runtime
        {
            get { return bRuntime; }
        }
        public bool system
        {
            get { return bSystem; }
            set { bSystem = value; }
        }
        public string notes
        {
            get { return sNotes; }
            set { sNotes = value; }
        }

        /// <summary>
        /// Gets or sets the join via for this column from the base table of the report.
        /// </summary>
        public JoinVia JoinVia { get; set; }

        #endregion

        public void setRuntime(bool runtime)
        {
            bRuntime = runtime;
        }
    }

    [Serializable()]
    public class cStandardColumn : cReportColumn
    {
        public string DisplayName { get; set; }

        private cField clsfield;
        private bool bGroupby;
        private bool bFuncsum;
        private bool bFuncavg;
        private bool bFuncmax;
        private bool bFuncmin;
        private bool bFunccount;
        private bool bSubquery;

        /// <summary>
        /// Initialises a new instance of the <see cref="cStandardColumn"/> class.
        /// </summary>
        /// <param name="columnid">
        ///     The column ID.
        /// </param>
        /// <param name="reportid">
        ///     The report ID.
        /// </param>
        /// <param name="columntype">
        ///     The column type.
        /// </param>
        /// <param name="sort">
        ///     The sort.
        /// </param>
        /// <param name="order">
        ///     The order.
        /// </param>
        /// <param name="field">
        ///     The field.
        /// </param>
        /// <param name="groupby">
        ///     The group by.
        /// </param>
        /// <param name="funcsum">
        ///     The sum function.
        /// </param>
        /// <param name="funcmin">
        ///     The min function.
        /// </param>
        /// <param name="funcmax">
        ///     The max function.
        /// </param>
        /// <param name="funcavg">
        ///     The average function.
        /// </param>
        /// <param name="funccount">
        ///     The count function.
        /// </param>
        /// <param name="hidden">
        ///     The hidden flag.
        /// </param>
        /// <param name="displayName">
        ///     The column display text if different from the field description.
        /// </param>
        /// <param name="systemColumn">True if this is a system column, i.e. not editable by the user.</param>
        public cStandardColumn(Guid columnid, Guid reportid, ReportColumnType columntype, ColumnSort sort, int order, cField field, bool groupby, bool funcsum, bool funcmin, bool funcmax, bool funcavg, bool funccount, bool hidden, string displayName = "", bool systemColumn = false)
        {
            this.DisplayName = displayName == "NULL" ? string.Empty : displayName;
            this.gColumnid = columnid;
            this.gReportcolumnid = columnid;
            this.gReportid = reportid;
            this.bGroupby = groupby;
            this.ctColumntype = columntype;
            this.csSort = sort;
            this.nOrder = order;
            this.clsfield = field;
            this.bFuncsum = funcsum;
            this.bFuncavg = funcavg;
            this.bFuncmax = funcmax;
            this.bFuncmin = funcmin;
            this.bFunccount = funccount;
            this.bHidden = hidden;
            this.bSystem = systemColumn;
        }

        #region properties
        public cField field
        {
            get { return this.clsfield; }
        }
        public bool groupby
        {
            get { return this.bGroupby; }
        }
        public bool funcsum
        {
            get { return this.bFuncsum; }
            set { this.bFuncsum = value; }
        }
        public bool funcavg
        {
            get { return this.bFuncavg; }
            set { this.bFuncavg = value; }
        }
        public bool funcmax
        {
            get { return this.bFuncmax; }
            set { this.bFuncmax = value; }
        }
        public bool funcmin
        {
            get { return this.bFuncmin; }
            set { this.bFuncmin = value; }
        }
        public bool funccount
        {
            get { return this.bFunccount; }
            set { this.bFunccount = value; }
        }
        public bool subquery
        {
            get { return this.bSubquery; }
            set { this.bSubquery = value; }
        }
        #endregion
    }
    [Serializable()]
    public class cStaticColumn : cReportColumn
    {
        private string sLiteralname;
        private string sLiteralvalue;
        private bool bRuntime;
        public cStaticColumn(Guid columnid, Guid reportid, ReportColumnType columntype, ColumnSort sort, int order, string literalname, string literalvalue, bool runtime, bool hidden)
        {
            gColumnid = columnid;
            gReportcolumnid = columnid;
            gReportid = reportid;
            ctColumntype = columntype;
            csSort = sort;
            nOrder = order;
            sLiteralname = literalname;
            sLiteralvalue = literalvalue;
            bRuntime = runtime;
            bHidden = hidden;
        }

        public void setValue(string val)
        {
            sLiteralvalue = val;
        }
        #region properties
        public string literalname
        {
            get { return sLiteralname; }
        }
        public string literalvalue
        {
            get { return sLiteralvalue; }
        }
        public bool runtime
        {
            get { return bRuntime; }
        }
        #endregion
    }

    [Serializable()]
    public class cCalculatedColumn : cReportColumn
    {
        private string sColumnname;
        private string sFormula;

        public cCalculatedColumn(Guid columnid, Guid reportid, ReportColumnType columntype, ColumnSort sort, int order, string columnname, string formula)
        {
            gColumnid = columnid;
            gReportcolumnid = columnid;
            gReportid = reportid;
            ctColumntype = columntype;
            csSort = sort;
            nOrder = order;
            sColumnname = columnname;
            sFormula = formula;
        }

        #region properties
        public string columnname
        {
            get { return sColumnname; }
        }

        /// <summary>
        /// Gets the formula for use in calculations.
        /// </summary>
        public string formula
        {
            get
            {
                if (sFormula == null)
                {
                    return string.Empty;
                }

                var cleanFormula = Regex.Replace(sFormula, "<span.*?>", string.Empty);
                cleanFormula = Regex.Replace(cleanFormula, "<font.*?>", string.Empty);
                cleanFormula = cleanFormula.Replace("</span>", string.Empty);
                cleanFormula = cleanFormula.Replace("</font>", string.Empty);
                cleanFormula = cleanFormula.Replace("</p>", string.Empty);
                cleanFormula = cleanFormula.Replace("<p>", string.Empty);

                // get rid of quoted (i.e. "[ ]") formula fields
                cleanFormula = cleanFormula.Replace("\"[", "[");
                cleanFormula = cleanFormula.Replace("]\"", "]");
                return cleanFormula;
            }
        }

        /// <summary>
        /// Gets the formatted formula - used to store the formatting.
        /// </summary>
        public string formattedFormula
        {
            get
            {
                return sFormula;
            }
        }

        #endregion
    }
    
    [Serializable()]
    public class cReportCriterion
    {
        private Guid gCriteriaid;
        private Guid gReportid;
        private cField clsfield;
        private ConditionType ctCondition;
        private object[] oValue1;
        private object[] oValue2;
        private ConditionJoiner cjJoiner;
        private int nOrder;
        private bool bRuntime;
        private int nGroupnumber;
        private bool bDrilldown;

        public cReportCriterion(Guid criteriaid, Guid reportid, cField field, ConditionType conditiontype, object[] value1, object[] value2, ConditionJoiner joiner, int order, bool runtime, byte groupnumber, JoinVia joinVia = null)
        {
            gCriteriaid = criteriaid;
            gReportid = reportid;
            clsfield = field;
            ctCondition = conditiontype;
            oValue1 = value1;
            oValue2 = value2;
            cjJoiner = joiner;
            nOrder = order;
            bRuntime = runtime;
            nGroupnumber = groupnumber;
            this.JoinVia = joinVia;
        }

        public void changeToDrilldown()
        {
            bDrilldown = true;
        }
        public void updateValues(object[] value1, object[] value2)
        {
            oValue1 = value1;
            oValue2 = value2;
        }
        #region properties
        public Guid criteriaid
        {
            get { return gCriteriaid; }
            set { gCriteriaid = value; }
        }
        public Guid reportid
        {
            get { return gReportid; }
        }
        public cField field
        {
            get { return clsfield; }
        }
        public ConditionType condition
        {
            get { return ctCondition; }
        }
        public object[] value1
        {
            get { return oValue1; }
        }
        public object[] value2
        {
            get { return oValue2; }
        }
        public ConditionJoiner joiner
        {
            get { return cjJoiner; }
        }
        public int order
        {
            get { return nOrder; }
        }
        public bool runtime
        {
            get { return bRuntime; }
        }
        public int groupnumber
        {
            get { return nGroupnumber; }
            set { nGroupnumber = value; }
        }
        public bool drilldown
        {
            get { return bDrilldown; }
        }

        /// <summary>
        /// Gets or sets the join via for this criteria from the base table of the report.
        /// </summary>
        public JoinVia JoinVia { get; set; }

        #endregion

    }

    [Serializable()]
    public class cReportFolder
    {
        private Guid gFolderid;
        private string sFolder;
        private int? nEmployeeid;
        private bool bPersonal;
        private ReportArea eReportArea;

        /// <summary>
        /// Custom Reports constructor
        /// </summary>
        /// <param name="folderid"></param>
        /// <param name="folder"></param>
        /// <param name="employeeid"></param>
        /// <param name="personal"></param>
        public cReportFolder(Guid folderid, string folder, int? employeeid, bool personal, ReportArea reportArea)
        {
            gFolderid = folderid;
            sFolder = folder;
            nEmployeeid = employeeid;
            bPersonal = personal;
            eReportArea = reportArea;
        }

        #region properties
        public Guid folderid
        {
            get { return gFolderid; }
        }
        public string folder
        {
            get { return sFolder; }
        }
        public int? employeeid
        {
            get { return nEmployeeid; }
        }
        public bool personal
        {
            get { return bPersonal; }
        }

        public ReportArea reportArea
        {
            get { return eReportArea; }
        }
        #endregion
    }

    public enum ConditionType
    {
        Equals = 1,
        DoesNotEqual = 2,
        GreaterThan = 3,
        LessThan = 4,
        GreaterThanEqualTo = 5,
        LessThanEqualTo = 6,
        Like = 7,
        Between = 8,
        ContainsData = 9,
        DoesNotContainData = 10,
        Yesterday = 11,
        Today,
        Tomorrow,
        Next7Days,
        Last7Days,
        NextWeek,
        LastWeek,
        ThisWeek,
        NextMonth,
        LastMonth,
        ThisMonth,
        NextYear,
        LastYear,
        ThisYear,
        NextFinancialYear,
        LastFinancialYear,
        ThisFinancialYear,
        LastXDays,
        NextXDays,
        LastXWeeks,
        NextXWeeks,
        LastXMonths,
        NextXMonths,
        LastXYears,
        NextXYears,
        AnyTime,
        On,
        NotOn,
        After,
        Before,
        OnOrAfter,
        OnOrBefore,
        NextTaxYear,
        LastTaxYear,
        ThisTaxYear,
        NotLike,
        OnOrAfterToday,
        OnOrBeforeToday,       
        GroupAnd = 249,
        GroupOr = 250,
        AtMyCostCodeHierarchy = 250,
        CompleteHierarchy = 251,
        AtMe = 254,
        AtMyHierarchy = 255,
        AtMyClaimsHierarchy = 253,
        WithAccessRoles = 252,
        In = 253
    }

    public enum ConditionJoiner
    {
        None = 0,
        And = 1,
        Or,
        AsWellAs
    }

    public enum ReportColumnType
    {
        Standard = 1,
        Static,
        Calculated
    }

    public enum ReportType
    {
        Item = 1,
        Summary,
        None
    }

    public enum ExportType
    {
        Viewer = 1,
        Excel,
        CSV,
        FlatFile,
        Pivot,
        Preview
    }

    public enum ColumnSort
    {
        None,
        Ascending,
        Descending
    }

    public class ReportTable
    {
        public const string ContractDetails = "998E51FA-2C23-467E-B90F-75C44D1838BC";  //25,
        public const string InvoiceForecasts = "9C73B5DA-EC86-438F-8513-524B6AB08C57"; //26,
        public const string ContractProductDetails = "35C52038-F99B-4FEB-85C2-5D1755C91CA8"; //31,
        public const string InvoiceDetails = "74477541-68D3-42EB-8B4B-EAA30A545125"; //95,
        public const string ProductDetails = "676DF92B-386A-4E39-BE7D-A54AB7D6D168"; //51,
        public const string EmployeeDetails = "7501ABCA-B6E0-4821-A2DF-1ECFB3BB3D14"; //61,
        public const string SupplierDetails = "299FF396-6947-4D46-A4DF-1983CD311A77"; //102,
        public const string SupplierContacts = "BCD8272E-3D91-42F1-B6D2-FB79F27176DB"; //103
        public const string DocumentMergeProjects = "1D3EF4DE-3F3F-4E23-BC29-779F502CCA9A";
        public const string DocumentMergeSources = "BE95CEAC-68C2-47AF-B2EA-799303AB45ED";
        public const string DocumentMappings = "D86B4B79-23AF-410D-8187-969DE66C231B";
        public const string RechargeAssociations = "021d4c9b-4d97-454c-80a1-a8b32d9c1355";
        public const string Tasks = "308188CA-E5EE-4769-A25A-7D19CFBB8423";
        public const string Claims = "0efa50b5-da7b-49c7-a9aa-1017d5f741d0";
        public const string SavedExpenses = "d70d9e5f-37e2-4025-9492-3bcf6aa746a8";
        public const string Cars = "A184192F-74B6-42F7-8FDB-6DCF04723CEF";
        public const string Employees = "618DB425-F430-4660-9525-EBAB444ED754";
        public const string Holidays = "A6FF86D3-808F-406F-9DD6-E21B7B9A8D67";
        public const string Addresses = "A3713E93-0ABE-4EAC-9533-6A22AA4C1F62";
        public const string VehicleUdfs = "7e9e6bee-f8ca-45d8-b914-1a9b105e47b2";
        public const string SupplierDetailsSummaryView = "3C128918-AB18-4D60-BAD6-EECB42299CB0";
        public const string VehicleDocuments = "28D592D7-4596-49C4-96B8-45655D8DF797";
        public const string VehicleDocumentsView = "C3069AAF-37A8-487B-A06F-77778C2A3F38";
        public const string DrivingLicences = "F066EF8F-705F-4CD7-8DD7-FDEFBF8F3821";
        public const string CardProviders = "78C92D47-2CF1-4970-93F7-5DC972683F88";
        public const string P11DCategories = "5D2D9191-83EA-4ED5-8A46-0AABB8190392";
        public const string Reasons = "83077E08-FE7D-4C1A-A306-BE4327C349C1";
    }

    public class ViewGroups
    {
        public const string RechargeTemplates = "C7DBA7DF-DADE-4D94-A75B-975B7BE57C47";
        public const string RechargeClients = "21AA8FD7-0A71-4E8A-9B09-95D02887B8C8";
        public const string RechargePayments = "B2F10DD0-C1EE-4035-8294-C242A3019FF0";
    }

    public class ReportKeyFields
    {
        public const string SavedExpensesCostCodesExpenseId = "B2CC184D-8BAF-4C09-8080-68B687957AD2";
        public const string SavedExpensesCostCodesCostCodeId = "9B3E8A13-97BB-4386-9D98-CCC3EB713D5F";
        public const string SubCatsSubCatId = "D4ED76BD-605C-45CE-B075-4C6018A50B08";
        public const string EsrAssignId = "e7850cca-7eef-4af2-98bf-f6e7089fdb15";
        public const string ContractDetails_ContractId = "03A70595-B107-48A5-9BCA-E850EA7F1239"; //58,
        public const string ContractProducts_ConProdId = "EB3D9CA9-A1C9-4B63-AB6A-1D5A3BAD8921"; //128,
        public const string SupplierDetails_SupplierId = "847ACDE0-B2FF-4FD4-B3BF-C5FE895399AE"; //300,
        public const string ProductDetails_ProductId = "52C44EFB-AB86-4084-867C-1444F2BA81AA"; //222,
        public const string InvoiceDetails_InvoiceId = "699433C5-8DE7-4ED5-A7A1-446E6F89146F"; //160,
        public const string InvoiceForecast_ForecastId = "51674753-802C-470B-88CB-77B79FD27502"; //94,
        public const string StaffDetails_StaffId = "FB3368D9-57D5-416B-A082-4344179485A4"; //264
        public const string MergeProjects_MergeProjectId = "E761CCDD-C92C-4B4F-9D2D-11F17F5654A3";
        public const string MergeSources_MergeProjectId = "D1299BB4-F995-4B45-88CB-D1A7AD265F86";
        public const string MergeMappings_MergeProjectId = "4252B899-1D44-4D9A-A0BB-418C73CFF200";
        public const string MergeMappings_MergeTypeId = "9ED1B053-3C39-4ACA-B4FC-51E153E6CC87";
        //public const string MergeMappings_MappingId = "";
        public const string Tasks_TaskId = "B1885C78-266C-45E2-93FD-0D05E894A3EE";
        public const string SavedexpensesClaimId = "34012174-7CE8-4F67-8B91-6C44AC1A4845";
        public const string SavedexpensesExpenseId = "a528de93-3037-46f6-974c-a76bd0c8642a";
        public const string ClaimsClaimId = "E3AF2B67-A613-437E-AABF-6853C4553977";
        public const string TasksSubAccountId = "CB449C35-669F-4845-A3DD-7DE1B30EE091";
        public const string EmployeesEmployeeId = "EDA990E3-6B7E-4C26-8D38-AD1D77FB2FBF";
        public const string ContractDetailsSupplierId = "1DBA1666-0CF8-4570-99DD-CD878D529B51";
        public const string CardProvidersCardProviderId = "3F29EC51-DFE2-47BB-B8ED-C0DC45826B55";
    }

    public class ReportFields
    {
        public const string SavedExpensesClaimId = "34012174-7CE8-4F67-8B91-6C44AC1A4845";
        public const string SavedExpensesSubCatId = "8F61ABE2-96DE-4D3F-9E91-FDF2D47800CB";
        public const string SavedExpensesDateOfExpense = "A52B4423-C766-47BB-8BF3-489400946B4C";
        public const string EsrAssignmentEsrAssignId = "D1124F25-7F80-4D64-98AE-DFF161CA161D";
        public const string CostCodesEsrCostCode = "43380435-B728-417E-968C-FF44BEF69012";
        public const string SavedExpensesDeriveCostcodeForEsrReport = "9DD93EF9-F396-425D-B786-A2DF7AA115EA";
        public const string EsrAssignmentAssignmentNumber = "c23858b8-7730-440e-b481-c43fe8a1dbef";
        public const string ClaimsEmployee = "2501BE3D-AA94-437D-98BB-A28788A35DC4";
        public const string CostCodesCostCode = "359DFAC9-74E6-4BE5-949F-3FB224B1CBFC";
        public const string DepartmentsDepartment = "9617A83E-6621-4B73-B787-193110511C17";
        public const string ProjectCodesProjectCode = "6D06B15E-A157-4F56-9FF2-E488D7647219";
        public const string SupplierDetailsSubAccountId = "4A7B292D-B501-4DC8-AB81-ACCF5E0A3DDA";
        public const string ContractDetailsSupplierId = "1DBA1666-0CF8-4570-99DD-CD878D529B51";
        public const string SupplierContactsSupplierId = "EB5F9ECC-8846-486B-8A17-F9B3BE432836";

        public const string EmployeesCurrentRefNumber = "30827090-7D91-4D2C-8E64-DDC77B57B5E3";

        public const string EmployeesCurrentClaimNumber = "6DADBF48-DA43-45B5-892D-F3290B1F57A0";
        public const string EmployeesApplicantActiveStatusFlag = "8D8C73D2-FA4F-47ED-866F-2CEE890B80F9";
        public const string EmployeesEsrEffectiveEndDate = "02B90A6A-D1F0-4B99-A3A4-06C6BAE02560";
        public const string EmployeesEsrEffectiveStartDate = "7626D350-D51A-42EE-84C9-127FB1EDC580";
        public const string EmployeesCountry = "8816CAEC-B520-4223-B738-47D2F22F3E1A";
        public const string EmployeesApplicantNumber = "A93ED9A2-6202-46F2-88DB-C0B5A6863DF5";
        public const string EmployeesEsrPersonType = "1883FF7E-12CE-4E81-A1A1-FFF6142FF13F";
        public const string EmployeesGetEmployeeDepartmentNameFromEmployeeId = "13E3F7D5-2D08-43D0-9BB4-6F233F036898";
        public const string EmployeesGetEmployeeJobTitleFromEmployeeId= "1C67516E-BCFD-4393-9B72-1DBC5EA1CCA3";
        public const string EmployeesEsrPersonId = "1622032E-E1AA-42C4-A8FA-173B16CD4957";
        public const string InvoiceDetailsContractId = "B95CD99A-9C1E-42F5-BF56-F19B9CFDACE5";
        public const string ContractProductDetailsContractId = "B7A2A17B-4350-405C-A9EA-514A36D4A08F";
        public const string InvoiceForecastsContractId = "20A1CD12-C7CC-408E-93FD-EA8C701DBC4D";
        public const string TaskCreatorId = "E0F87C43-F55B-47A6-98E1-A3C84F34979E";

        public const string TeamEmployeeId = "6dd9864e-df3a-4503-8999-5ad4b65b6c07";
        public const string TeamLeaderId = "992A7FAF-FE98-4A0A-82FD-7C3CF10DD0DD";
        public const string BudgetHolderEmployeeId = "4B9B73B6-789A-4272-86A0-05950A1095B3";
        public const string EsrAssignmentSignOffEmployeeId = "CB072285-3096-48AD-B695-B5C9C854FFDC";

        public const string DrivingLicenceEmployeeId = "52F62612-871C-42D7-9035-7A90E79E7F08";

        public const string CardProvidersAutoImport = "5A1D5B37-AB24-48B5-84F7-6FDABA7C641E";
        public const string CardProvidersCardType = "5C4D2FC4-2E2E-4911-96E3-AC6D25F1C159";
        public const string CardProvidersCardProvider = "5956859B-9463-4D06-A14F-1FE7048FB9F5";
        public const string CardProvidersPurchaseCard = "AD3C99C6-9796-488E-8BEC-14366AF56FC7";
        public const string CardProvidersClaimantsSettleBill = "C85799A2-9D17-4217-AC63-70C7CC75C457";

        public const string P11DCategoriesPDName = "E9B28E60-5AC5-47D0-BCB2-66195B2A5901";

        public const string ReasonName = "AF839FE7-8A52-4BD1-962C-8A87F22D4A10";
    }

}
