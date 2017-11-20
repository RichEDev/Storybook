namespace Expenses_Reports
{
    #region Using Directives

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using Infragistics.WebUI.CalcEngine;
    using Infragistics.WebUI.UltraWebCalcManager;
    using Infragistics.WebUI.UltraWebGrid;

    using SpendManagementLibrary;
    using SpendManagementLibrary.Definitions.JoinVia;
    using SpendManagementLibrary.Helpers;
    using SpendManagementLibrary.Interfaces;

    using Spend_Management;
    using System.Configuration;
    using System.Drawing;
    using System.Windows.Forms.DataVisualization.Charting;
    using System.Text.RegularExpressions;
    using System.Diagnostics;
    using Expenses_Reports.Formula;
    using SpendManagementLibrary.Logic_Classes.Fields;


    using Convert = System.Convert;

    #endregion

    /// <summary>
    ///     Summary description for cReports
    /// </summary>
    public class cReports
    {
        #region Fields

        /// <summary>
        /// The account id.
        /// </summary>
        private readonly int nAccountid;

        /// <summary>
        /// The sub account id.
        /// </summary>
        private int? nSubAccountId;

        /// <summary>
        /// Helper object for moving claims view to in memory table.
        /// </summary>
        private readonly ClaimsViewInMemory _claimsViewInMemory;

        /// <summary>
        /// The joinvia that joins to employees (if any).
        /// </summary>
        private JoinVia employeeJoinVia = null;

        /// <summary>
        /// The joinvia that joins to contract details (if any).
        /// </summary>
        private JoinVia contractDetailsJoinVia = null;


        /// <summary>
        /// The joinvia that joins to supplier details (if any).
        /// </summary>
        private JoinVia supplierDetailsJoinVia = null;
        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initialises a new instance of the cReports class.
        /// </summary>
        /// <param name="accountid">
        /// The accountid.
        /// </param>
        public cReports(int accountid)
        {
            this.nAccountid = accountid;
            this._claimsViewInMemory = new ClaimsViewInMemory();
        }

        #endregion

        #region Enums

        /// <summary>
        /// The cache invoked for.
        /// </summary>
        private enum CacheInvokedFor
        {
            /// <summary>
            /// The none.
            /// </summary>
            None,

            /// <summary>
            /// The instantiation.
            /// </summary>
            Instantiation,

            /// <summary>
            /// The reports.
            /// </summary>
            Reports,

            /// <summary>
            /// The report folders.
            /// </summary>
            ReportFolders
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the account id.
        /// </summary>
        public int accountid
        {
            get
            {
                return this.nAccountid;
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the cache key.
        /// </summary>
        private string cacheKey
        {
            get
            {
                return string.Format("{0}_{1}", this.nAccountid, this.nSubAccountId);
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Create the report data.
        /// </summary>
        /// <param name="request">
        /// The report request.
        /// </param>
        /// <param name="fields">
        /// The fields class.
        /// </param>
        /// <param name="joins">
        /// The joins class.
        /// </param>
        /// <param name="grid">
        /// The UltraCalc grid.
        /// </param>
        /// <param name="calcMan">
        /// The calcumation manager for the grid..
        /// </param>
        /// <param name="useClaimsInMemoryTable">
        /// If an in memory table should be used instead of the claims view (when claims related columns are part of the report).
        /// </param>
        /// <returns>
        /// The <see cref="DataSet"/>.
        /// The data defined by the report request.
        /// </returns>
        public DataSet createReport(cReportRequest request, IFields fields, cJoins joins, UltraWebGrid grid, IUltraCalcManager calcMan, bool useClaimsInMemoryTable = false)
        {
            string strsql = string.Empty;
            request.report.AccessLevel = request.AccessLevel;
            request.report.AccessLevelRoles = request.AccessLevelRoles;
            var user = cMisc.GetCurrentUser($"{request.accountid},{request.employeeid}");
            var roles = new cAccessRoles(this.accountid, cAccounts.getConnectionString(this.accountid));
            var lstAccessRoles = request.SubAccountId == 0  || request.employeeid == 0 ? new List<int>() : user.Employee.GetAccessRoles().GetBy(request.SubAccountId);
            if (lstAccessRoles != null)
            {
                var reportableAccessRoleDetails = lstAccessRoles.Select(employeeAccessRole => roles.GetAccessRoleByID(employeeAccessRole)).ToList();
                request.report.ListOfAccessRolesAssignedToTheUser = reportableAccessRoleDetails;
            }
            
            if (request.report.AccessLevel == AccessRoleLevel.SelectedRoles && request.report.AccessLevelRoles == null)
            {
                var reportRoles = new List<int>();
                foreach (int emp_arId in lstAccessRoles)
                {
                    cAccessRole empRole = roles.GetAccessRoleByID(emp_arId);
                    foreach (int arId in empRole.AccessRoleLinks)
                    {
                        if (!reportRoles.Contains(arId))
                        {
                            reportRoles.Add(arId);
                        }
                    }
                }
                request.report.AccessLevelRoles = reportRoles.ToArray();
            }
            request.report.employeeid = request.employeeid;
            DataSet ds;
            var claimsFields = new List<cField>();

            if (string.IsNullOrEmpty(request.report.StaticReportSQL))
            {
                if (request.claimantreport && cReport.canFilterByRole(request.report.basetable.TableID))
                {
                    // add employeeid clause but only if relevant to employees

                    var reqfield = fields.GetFieldByID(new Guid(ReportKeyFields.EmployeesEmployeeId));
                    var criteria = new cReportCriterion(
                        new Guid(),
                        request.report.reportid,
                        reqfield,
                        ConditionType.Equals,
                        new object[] {request.employeeid},
                        new object[0],
                        ConditionJoiner.And,
                        request.report.criteria.Count + 1,
                        false,
                        0);
                    request.report.addCriteria(criteria);
                }

                if (request.report.basetable.TableID == new Guid(ReportTable.Tasks)
                    && request.SubAccountId > -1)
                {
                    // add subAccountId filter for task based reports
                    var reqField = fields.GetFieldByID(new Guid(ReportKeyFields.TasksSubAccountId));

                    // tasks.subAccountId
                    var criteria = new cReportCriterion(
                        new Guid(),
                        request.report.reportid,
                        reqField,
                        ConditionType.Equals,
                        new object[] {request.SubAccountId},
                        new object[0],
                        ConditionJoiner.And,
                        request.report.criteria.Count + 1,
                        false,
                        0);
                    request.report.addCriteria(criteria);
                }

                var lstFields = new SortedList<Guid, cField>();
                foreach (cReportColumn column in request.report.columns)
                {
                    if (column.GetType() == typeof(cStandardColumn))
                    {
                        cField fieldFound = ((cStandardColumn) column).field;

                        if (!lstFields.ContainsKey(((cStandardColumn) column).field.FieldID))
                        {
                            lstFields.Add(fieldFound.FieldID, fieldFound);

                            if (useClaimsInMemoryTable)
                            {
                                this._claimsViewInMemory.BuildClaimsFieldList(ref claimsFields, fieldFound, fields);
                            }
                        }
                    }
                }

                foreach (cReportCriterion criteria in request.report.criteria)
                {
                    if (!lstFields.ContainsKey(criteria.field.FieldID))
                    {
                        lstFields.Add(criteria.field.FieldID, criteria.field);

                        if (useClaimsInMemoryTable)
                        {
                            this._claimsViewInMemory.BuildClaimsFieldList(ref claimsFields, criteria.field, fields);
                        }
                    }
                }

                string joinsql = string.Empty;
                var trunk = new ViaBranch(request.report.basetable.TableName);
                var joinString = new List<string>();
                if (request.report.UseJoinVia)
                {
                    var joinViaList = new Dictionary<JoinVia, cField>();
                    var tableids = new List<Guid>();
                    foreach (cReportColumn column in request.report.columns)
                    {
                        if (column is cStandardColumn)
                        {
                            var standardColumn = (cStandardColumn) column;
                            if (column.JoinVia != null &&
                                !joinViaList.Keys.Any(j => j.JoinViaID == column.JoinVia.JoinViaID))
                            {
                                joinViaList.Add(column.JoinVia, standardColumn.field);
                                if (!tableids.Contains(standardColumn.field.TableID))
                                {
                                    tableids.Add(standardColumn.field.TableID);
                                }
                            }

                        }
                    }

                    trunk = new ViaBranch(request.report.basetable.TableName);
                    foreach (cReportCriterion column in request.report.criteria)
                    {
                        if (column.JoinVia != null &&
                            !joinViaList.Keys.Any(j => j.JoinViaID == column.JoinVia.JoinViaID))
                        {
                            joinViaList.Add(column.JoinVia, column.field);
                            if (!tableids.Contains(column.field.TableID))
                            {
                                tableids.Add(column.field.TableID);
                            }
                        }
                    }
                    foreach (KeyValuePair<JoinVia, cField> keyValuePair in joinViaList.OrderBy(j => j.Key.JoinViaList
                        .Count))
                    {
                        joins.GetJoinViaSQL(
                            ref joinString,
                            ref trunk,
                            keyValuePair.Key,
                            keyValuePair.Value,
                            request.report.basetable.TableID);
                    }

                    joinString = this.AddMissingJoins(request, joins, joinString, ref trunk, fields, new JoinVias(user),
                        tableids);

                    var sb = new StringBuilder();
                    foreach (string strjoin in joinString)
                    {
                        sb.Append(strjoin);
                    }

                    joinsql = sb.ToString();
                }
                else
                {
                    joinsql = joins.createReportJoinSQL(lstFields.Values.ToList(), request.report.basetable.TableID,
                        request.AccessLevel);
                }


                request.report.JoinSQL = joinsql;
                strsql = request.report.createReport();
            }
            else
            {
                if (request.report.AccessLevel == AccessRoleLevel.EmployeesResponsibleFor
                    && cReport.canFilterByRole(request.report.basetable.TableID))
                {
                    strsql = "declare @linemanagers as LineManagers; ";
                }

                strsql += request.report.StaticReportSQL;
                if (cReport.canFilterByRole(request.report.basetable.TableID))
                {
                    // need to identify position of WHERE in the SQL in order to insert required filter (-1 if not found)
                    int whereIdx = strsql.IndexOf("<% WHERE_INSERT %>", StringComparison.Ordinal);
                    int andIdx = strsql.IndexOf("<% AND_INSERT %>", StringComparison.Ordinal);

                    if (whereIdx > -1)
                    {
                        switch (request.report.AccessLevel)
                        {
                            case AccessRoleLevel.EmployeesResponsibleFor:
                                strsql = strsql.Replace("<% WHERE_INSERT %>",
                                    string.Format(
                                        " dbo.employees.employeeid in (select employeeid from dbo.resolveReport({0},@linemanagers))",
                                        request.employeeid));

                                // additional parameter used if static SQL doesn't have a WHERE clause and needs the 'WHERE' keyword inserting 
                                strsql = strsql.Replace("<% WHERECMD_INSERT %>", " WHERE ");
                                if (andIdx > -1)
                                {
                                    strsql = strsql.Replace("<% AND_INSERT %>", " AND ");
                                }

                                break;
                            case AccessRoleLevel.SelectedRoles:

                                // additional parameter used if static SQL doesn't have a WHERE clause and needs the 'WHERE' keyword inserting 
                                strsql = strsql.Replace("<% WHERECMD_INSERT %>", " WHERE ");
                                if (andIdx > -1)
                                {
                                    strsql = strsql.Replace("<% AND_INSERT %>", " AND ");
                                }

                                if (request.report.AccessLevelRoles != null
                                    && request.report.AccessLevelRoles.Length > 0)
                                {
                                    var ehrBuilder = new StringBuilder();
                                    string theOr = string.Empty;
                                    ehrBuilder.Append(" (");
                                    for (int x = 0; x < request.report.AccessLevelRoles.Length; x++)
                                    {
                                        ehrBuilder.Append(
                                            theOr + "dbo.employeeHasRole("
                                            + request.report.AccessLevelRoles[x].ToString()
                                            + ", dbo.employees.employeeid, " + request.SubAccountId + ") = 1 ");
                                        theOr = " or ";
                                    }

                                    ehrBuilder.Append(")");
                                    strsql = strsql.Replace("<% WHERE_INSERT %>", ehrBuilder.ToString());
                                }
                                else
                                {
                                    // no roles selected, so set clause with access role of zero, so no data returned (but valid SQL)
                                    strsql = strsql.Replace(
                                        "<% WHERE_INSERT %>",
                                        " (dbo.employeeHasRole(0, dbo.employees.employeeid, " + request.SubAccountId
                                        + ") = 1)");
                                }

                                break;
                            default:
                                strsql = strsql.Replace("<% WHERE_INSERT %>", string.Empty);
                                strsql = strsql.Replace("<% AND_INSERT %>", string.Empty);
                                strsql = strsql.Replace("<% WHERECMD_INSERT %>", string.Empty);
                                break;
                        }
                    }
                }
                else
                {
                    strsql = strsql.Replace("<% WHERE_INSERT %>", string.Empty);
                    strsql = strsql.Replace("<% AND_INSERT %>", string.Empty);
                    strsql = strsql.Replace("<% WHERECMD_INSERT %>", string.Empty);
                }
            }


            if (useClaimsInMemoryTable)
            {
                this._claimsViewInMemory.ModifySqlWithClaimsViewInMemoryTable(ref strsql, claimsFields);
            }

            cEventlog.LogEntry(
                "ReportEngine : AccountID: " + this.accountid + "\nReportID: " + request.report.reportid
                + "\nEmployeeID: " + request.employeeid + "\nSQL:\n" + strsql);

            double timeoutMinutes;
            if (!double.TryParse(ConfigurationManager.AppSettings["SqlTimeoutMinutes"], out timeoutMinutes))
            {
                timeoutMinutes = 0.5;
            }
            var timeoutSeconds = (int)Math.Ceiling(timeoutMinutes * 60);

            if (request.reportRunFrom == ReportRunFrom.PrimaryServer)
            {
                using (var connection = new DatabaseConnection(cReportsSvc.GetConnectionString(this.accountid)))
                {
                    this.setCriteriaParameters(request, connection);
                    connection.sqlexecute.CommandTimeout = timeoutSeconds;
                    ds = connection.GetDataSet(strsql, true);
                    this.LogRequest(request, strsql, connection);
                }
            }
            else
            {
                using (var reportConnection = new DatabaseConnection(cReportsSvc.GetConnectionString(this.accountid)))
                {
                    this.setCriteriaParameters(request, reportConnection);
                    reportConnection.sqlexecute.CommandTimeout = timeoutSeconds;
                    ds = reportConnection.GetDataSet(strsql, true);
                    this.LogRequest(request, strsql, reportConnection);
                }
            }

            if (request.SchedulerRequestID == Guid.Empty && request.report.SubAccountID.HasValue
                && (request.report.exportoptions != null && request.report.exportoptions.isfinancialexport == false))
            {
                // only update if has subaccount and request has NOT come from scheduler
                using (var connection2 = new DatabaseConnection(cReportsSvc.GetConnectionString(this.accountid)))
                {
                    connection2.sqlexecute.Parameters.AddWithValue("@employeeId", request.employeeid);
                    string sql =
                        "SELECT ISNULL(accessId, 0) FROM dbo.subAccountAccess WHERE employeeId=@employeeId AND subAccountId=@subAccountId";

                    connection2.sqlexecute.Parameters.AddWithValue("@subAccountId", request.SubAccountId);

                    int accessId = 0;
                    try
                    {
                        accessId = connection2.ExecuteScalar<int>(sql);
                    }
                    catch (Exception ex)
                    {
                        LogError(ex, "createReport");
                    }

                    connection2.sqlexecute.Parameters.Clear();
                    connection2.sqlexecute.Parameters.AddWithValue("@lastReportId", request.report.reportid);

                    if (accessId == 0)
                    {
                        connection2.sqlexecute.Parameters.AddWithValue("@employeeId", request.employeeid);
                        connection2.sqlexecute.Parameters.AddWithValue("@subAccountId", request.SubAccountId);
                        sql =
                            "INSERT INTO dbo.subAccountAccess (subAccountId, employeeId, lastReportId) VALUES (@subAccountId, @employeeId, @lastReportId);";
                        connection2.ExecuteSQL(sql);
                    }
                    else
                    {
                        connection2.sqlexecute.Parameters.AddWithValue("@accessId", accessId);
                        sql = "UPDATE dbo.subAccountAccess SET lastReportId=@lastReportId WHERE accessId=@accessId";
                        connection2.ExecuteSQL(sql);
                    }
                }
            }

            if (string.IsNullOrEmpty(request.report.StaticReportSQL) && request.exporttype == ExportType.Preview)
            {
                ds = this.ReCreateDatasetWithCalculatedFields(request, ds, calcMan);
            }

            return this.GenerateChart(request, ds); 
        }

        /// <summary>
        /// The re create the dataset with calculated fields filled in.
        /// </summary>
        /// <param name="request">
        /// The report request.
        /// </param>
        /// <param name="ds">
        /// The report dataset.
        /// </param>
        /// <param name="calcMan">
        /// The calculation manager for the grid.
        /// </param>
        /// <returns>
        /// The <see cref="DataSet"/> with the "null" fields for calculated filled in.
        /// </returns>
        private DataSet ReCreateDatasetWithCalculatedFields(cReportRequest request, DataSet ds, IUltraCalcManager calcMan)
        {
            var ultraCalcMan = calcMan as UltraWebCalcManager;
            var resultTable = new DataTable(ds.Tables[0].TableName);
            var calculationFactory = new CalculationFactory(ds.Tables[0].Columns, request.report.columns);
            request.RowCount = ds.Tables[0].Rows.Count;
            var tableData = (from DataRow dataRow in ds.Tables[0].Rows select dataRow.ItemArray).ToList();
            this.CalculateColumnValues(tableData, calculationFactory, ultraCalcMan, request);
            SetOutputTableColumnTypes(calculationFactory, resultTable, tableData);
            ds = CreateNewTableFromOldTableAndCalculations(tableData, calculationFactory, resultTable);
            return ds;
        }

        /// <summary>
        /// Create a new data set with the correct data types.
        /// </summary>
        /// <param name="tableData">The tableData including any calculated values</param>
        /// <param name="calculationFactory">An instance of <see cref="CalculationFactory"/></param>
        /// <param name="resultTable">A <see cref="DataTable"/> used in the output <seealso cref="DataSet"/></param>
        /// <returns></returns>
        private static DataSet CreateNewTableFromOldTableAndCalculations(List<object[]> tableData, CalculationFactory calculationFactory, DataTable resultTable)
        {
            DataSet ds = null;
            if (calculationFactory.ColumnLookups.Count > 0 && tableData.Count > 0)
            {
                foreach (object[] row in tableData)
                {
                    var newRow = resultTable.NewRow();
                    newRow.ItemArray = row;
                    resultTable.Rows.Add(newRow);
                }
            }

            ds = new DataSet();
            ds.Tables.Add(resultTable);
            return ds;
        }

        /// <summary>
        /// Calculate the values for any Calculated columns
        /// </summary>
        /// <param name="tableData">The data for the table</param>
        /// <param name="calculationFactory">An instance of <see cref="CalculationFactory"/> </param>
        /// <param name="ultraCalcMan">An instance of <see cref="UltraWebCalcManager"/></param>
        /// <param name="request">The current instance of <see cref="cReportRequest"/></param>
        private void CalculateColumnValues(List<object[]> tableData, CalculationFactory calculationFactory, UltraWebCalcManager ultraCalcMan, cReportRequest request)
        {
            if (ultraCalcMan == null)
            {
                return;
            }

            var showHeader = true;
            switch (request.exporttype)
            {
                case ExportType.CSV:
                    showHeader = request.report.exportoptions.showheaderscsv;
                    break;
                case ExportType.Viewer:
                case ExportType.Preview:
                    break;
                case ExportType.Excel:
                    showHeader = request.report.exportoptions.showheadersexcel;
                    break;
                case ExportType.FlatFile:
                    showHeader = request.report.exportoptions.showheadersflatfile;
                    break;
                case ExportType.Pivot:
                    showHeader = request.report.exportoptions.showheadersexcel;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            for (int rowIndex = 0; rowIndex < tableData.Count; rowIndex++)
            {
                foreach (ColumnLookup columnLookup in calculationFactory.ColumnLookups.Where(c => c.Calculation != null))
                {
                    this.CalculateColumnValue(ultraCalcMan, columnLookup, tableData, rowIndex, showHeader);
                }

                request.ProcessedRows++;
            }
        }

        /// <summary>
        /// Add the columns into the returning <see cref="DataTable"/>
        /// </summary>
        /// <param name="calculationFactory">An instance of <see cref="CalculationFactory"/></param>
        /// <param name="resultTable">An instance of <see cref="DataTable"/></param>
        /// <param name="tableData">The current report data</param>
        private static void SetOutputTableColumnTypes(CalculationFactory calculationFactory, DataTable resultTable, List<object[]> tableData)
        {
            foreach (ColumnLookup lookup in calculationFactory.ColumnLookups)
            {
                DataColumn newColumn = null;

                switch (lookup.ReportColumn.columntype)
                {
                    case ReportColumnType.Calculated:

                        newColumn = new DataColumn(lookup.DataColumn.ColumnName, calculationFactory.GetColumnType(tableData, lookup.DataColumn.Ordinal));


                        break;
                    case ReportColumnType.Static:
                        newColumn = new DataColumn(lookup.DataColumn.ColumnName, typeof(string));

                        break;
                    case ReportColumnType.Standard:
                        var oldColumn = calculationFactory.ColumnLookups[lookup.DataColumn.Ordinal].DataColumn;
                        newColumn = new DataColumn(oldColumn.ColumnName, oldColumn.DataType);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                if (newColumn != null)
                {
                    resultTable.Columns.Add(newColumn);
                }
            }
        }

        /// <summary>
        /// Calculate the value of the given formula
        /// </summary>
        /// <param name="ultraCalcMan">An instance of <see cref="UltraWebCalcManager"/></param>
        /// <param name="columnLookup"></param>
        /// <param name="table">The <see cref="DataTable"/></param>
        /// <param name="rowIndex">The current Row Index</param>
        /// <param name="showHeader">True if the header is shown (and included in the row count)</param>
        private void CalculateColumnValue(UltraWebCalcManager ultraCalcMan, ColumnLookup columnLookup, List<object[]> table, int rowIndex, bool showHeader)
        {
            var row = table[rowIndex];
            var newFormula = string.Empty;
            try
            {
                newFormula = columnLookup.Calculation.ToString(row, columnLookup.DataColumn.Ordinal, showHeader ? rowIndex + 1 : rowIndex);
                newFormula = ReplaceAddress(newFormula, table, columnLookup);
                if (newFormula != null)
                {
                    var val = ultraCalcMan.Calculate(newFormula);
                    if (val.Value is UltraCalcErrorValue)
                    {

                        row[columnLookup.DataColumn.Ordinal] =
                            $"{val.Value} - {((UltraCalcErrorValue) val.Value).Message}";
                    }   
                    else
                    {
                        row[columnLookup.DataColumn.Ordinal] = val.Value;
                    }

                }
                else
                {
                    row[columnLookup.DataColumn.Ordinal] = newFormula;
                }

            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Calculated column failed to export\n\nReport Row: {rowIndex}\n\nFormula: {newFormula}\n\n\nInfragistics Message: \n\n{ex.Message}");
                row[columnLookup.DataColumn.Ordinal] = newFormula;
            }
        }

        /// <summary>
        /// Replace any instanced of "ADDRESS" with the data from the indicated cell
        /// </summary>
        /// <param name="replacementValue">The source text which may include ADDRESS</param>
        /// <param name="newTable">The <see cref="DataTable"/>which is the source of the data</param>
        /// <param name="columnLookup"></param>
        /// <returns>The given value with the replacement of any ADDRESS fields.</returns>
        private static string ReplaceAddress(string replacementValue, List<object[]> newTable, ColumnLookup columnLookup)
        {
            if (replacementValue.Contains("ADDRESS("))
            {
                var reg = new Regex("ADDRESS\\(\"?.+?\"?,\"?.+?\"?\\)");
                foreach (Match match in reg.Matches(replacementValue))
                {
                    var parts =
                        match.Value.Replace("ADDRESS(", string.Empty)
                            .Replace(")", string.Empty)
                            .Replace("\"", string.Empty)
                            .Split(',');

                    var rowIdx = int.Parse(new DataTable().Compute(parts[0], null).ToString()) - 1;
                    var columnIdx = int.Parse(new DataTable().Compute(parts[1], null).ToString()) - 1;
                    if (rowIdx < 0 || rowIdx >= newTable.Count || columnIdx < 0 || columnIdx > newTable[0].Length)
                    {
                        replacementValue = replacementValue
                            .Replace(match.Value, "NULL()");
                    }
                    else
                    {
                        var newValue = newTable[rowIdx][columnIdx];
                        var newValueType = TypeSelectorFactory.New(newValue);

                        replacementValue = replacementValue
                            .Replace(match.Value, $"{newValueType.Delimiter} {newTable[rowIdx][columnIdx]}{newValueType.Delimiter}");
                    }
                }
            }

            return replacementValue;
        }


        /// <summary>
        /// The add missing joins to the report where required for business logic or functions.
        /// </summary>
        /// <param name="request">
        /// The request.
        /// </param>
        /// <param name="clsjoins">
        /// The joins class.
        /// </param>
        /// <param name="joins">
        /// The SQL joins.
        /// </param>
        /// <param name="trunk">
        /// The trunk.
        /// </param>
        /// <param name="fields">
        /// The fields.
        /// </param>
        /// <param name="joinVias">
        /// The join VIAS class.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// The SQL joins including the required joins for that BASETABLE.
        /// </returns>
        private List<string> AddMissingJoins(cReportRequest request, cJoins clsjoins, List<string> joins, ref ViaBranch trunk, IFields fields, JoinVias joinVias, List<Guid> tableids)
        {
            var joinViaList = new SortedList<int, JoinViaPart>();
            JoinViaPart joinPart;
            JoinVia joinVia = null;
            int joinViaId;

            if (request.AccessLevel == AccessRoleLevel.EmployeesResponsibleFor || request.AccessLevel == AccessRoleLevel.SelectedRoles)
            {
                if (!tableids.Contains(new Guid(ReportTable.Employees)))
                {
                    if (request.report.basetable.TableID == new Guid(ReportTable.Claims))
                    {
                        tableids.Add(new Guid(ReportTable.Employees));
                        joinPart = new JoinViaPart(new Guid(ReportFields.ClaimsEmployee), JoinViaPart.IDType.Field);
                        joinViaList.Add(0, joinPart);
                        joinVia = new JoinVia(0, "Claims : Employee", Guid.Empty, joinViaList);
                    }
                    if (request.report.basetable.TableID == new Guid(ReportTable.SavedExpenses))
                    {
                        tableids.Add(new Guid(ReportTable.Employees));
                        joinPart = new JoinViaPart(
                            new Guid(ReportKeyFields.SavedexpensesClaimId),
                            JoinViaPart.IDType.Field);
                        joinViaList.Add(0, joinPart);
                        joinPart = new JoinViaPart(
                            new Guid(ReportFields.ClaimsEmployee),
                            JoinViaPart.IDType.RelatedTable);
                        joinViaList.Add(1, joinPart);
                        joinVia = new JoinVia(0, "SavedExpenses : Employee", Guid.Empty, joinViaList);
                    }
                    if (request.report.basetable.TableID == new Guid(ReportTable.Holidays))
                    {
                        tableids.Add(new Guid(ReportTable.Employees));
                        joinPart = new JoinViaPart(new Guid(ReportFields.ClaimsEmployee), JoinViaPart.IDType.Field);
                        joinViaList.Add(0, joinPart);
                        joinVia = new JoinVia(0, "Holidays : Employee", Guid.Empty, joinViaList);
                    }

                    if (joinVia != null)
                    {
                        joinViaId = joinVias.SaveJoinVia(joinVia);
                        this.employeeJoinVia = joinVias.GetJoinViaByID(joinViaId);
                        if (!string.Join(",", joins).Contains(this.employeeJoinVia.JoinViaAS.ToString()))
                        {
                            clsjoins.GetJoinViaSQL(
                                ref joins,
                                ref trunk,
                                this.employeeJoinVia,
                                fields.GetFieldByID(new Guid(ReportKeyFields.EmployeesEmployeeId)),
                                request.report.basetable.TableID);
                        }
                    }
                }
            }

            // add joins to contract details for framework for default subaccount criteria
            if ((request.report.basetable.TableID.ToString().ToUpper() == ReportTable.ContractDetails.ToUpper()
                || request.report.basetable.TableID.ToString().ToUpper() == ReportTable.InvoiceDetails.ToUpper()
                || request.report.basetable.TableID.ToString().ToUpper() == ReportTable.InvoiceForecasts.ToUpper()) && !tableids.Contains(new Guid(ReportTable.ContractDetails)))
            {
                if (request.report.basetable.TableID == new Guid(ReportTable.ContractProductDetails))
                {
                    tableids.Add(new Guid(ReportTable.ContractDetails));
                    joinPart = new JoinViaPart(new Guid(ReportFields.ContractProductDetailsContractId),
                        JoinViaPart.IDType.Field);
                    joinViaList.Add(0, joinPart);
                    joinVia = new JoinVia(0, "Contract Product Details : Contract Details", Guid.Empty, joinViaList);
                }

                if (request.report.basetable.TableID == new Guid(ReportTable.InvoiceDetails))
                {
                    tableids.Add(new Guid(ReportTable.ContractDetails));
                    joinPart = new JoinViaPart(
                        new Guid(ReportFields.InvoiceDetailsContractId),
                        JoinViaPart.IDType.Field);
                    joinViaList.Add(0, joinPart);
                    joinVia = new JoinVia(0, "Invoice Details : Contract Details", Guid.Empty, joinViaList);
                }

                if (request.report.basetable.TableID == new Guid(ReportTable.InvoiceForecasts))
                {
                    tableids.Add(new Guid(ReportTable.ContractDetails));
                    joinPart = new JoinViaPart(new Guid(ReportFields.InvoiceForecastsContractId),
                        JoinViaPart.IDType.Field);
                    joinViaList.Add(0, joinPart);
                    joinVia = new JoinVia(0, "Invoice Forecast Details : Contract Details", Guid.Empty, joinViaList);
                }

                if (joinVia != null)
                    {
                        joinViaId = joinVias.SaveJoinVia(joinVia);
                        this.contractDetailsJoinVia = joinVias.GetJoinViaByID(joinViaId);
                        if (!string.Join(",", joins).Contains(this.contractDetailsJoinVia.JoinViaAS.ToString()))
                        {
                            clsjoins.GetJoinViaSQL(
                                ref joins,
                                ref trunk,
                                this.contractDetailsJoinVia,
                                fields.GetFieldByID(new Guid(ReportKeyFields.ContractDetails_ContractId)),
                                request.report.basetable.TableID);
                        }
                    }
            }
                
            // add joins to suppliers for framework for default subaccount criteria
            if (!tableids.Contains(new Guid(ReportTable.SupplierDetails)) && request.report.basetable.TableID.ToString().ToUpper() == ReportTable.SupplierContacts)
            {
                if (request.report.basetable.TableID == new Guid(ReportTable.SupplierContacts))
                {
                    tableids.Add(new Guid(ReportTable.SupplierDetails));
                    joinPart = new JoinViaPart(new Guid(ReportFields.SupplierContactsSupplierId),
                        JoinViaPart.IDType.Field);
                    joinViaList.Add(0, joinPart);
                    joinVia = new JoinVia(0, "Supplier Contacts : Supplier Details", Guid.Empty, joinViaList);
                }

                if (joinVia != null)
                {
                    joinViaId = joinVias.SaveJoinVia(joinVia);
                    this.supplierDetailsJoinVia = joinVias.GetJoinViaByID(joinViaId);
                    if (!string.Join(",", joins).Contains(this.supplierDetailsJoinVia.JoinViaAS.ToString()))
                    {
                        clsjoins.GetJoinViaSQL(
                            ref joins,
                            ref trunk,
                            this.supplierDetailsJoinVia,
                            fields.GetFieldByID(new Guid(ReportKeyFields.SupplierDetails_SupplierId)),
                            request.report.basetable.TableID);
                    }
                }
            }

            // this join is added to fix the missing join for function field with two parameters
            if (request.report.columns.OfType<cStandardColumn>().Any(column => column.field.FieldName.Contains("savedexpenses_costcodes")))
            {
                var reg = new Regex(@"left join \[savedexpenses_costcodes\].*ON \[savedexpenses\]", RegexOptions.Compiled | RegexOptions.IgnoreCase);
                var found = joins.Any(@join => reg.IsMatch(@join));

                if (!found)
                {
                    joins.Add("LEFT JOIN [savedexpenses_costcodes] on [savedexpenses_costcodes].expenseid = [savedexpenses].expenseid ");
                }
                
            }

            return joins;
        }

        /// <summary>
        /// Generate the chart image from the given dataset.
        /// </summary>
        /// <param name="request">
        /// The report request.
        /// </param>
        /// <param name="ds">
        /// The data set.
        /// </param>
        /// <returns>
        /// The <see cref="DataSet"/>.
        /// </returns>
        public DataSet GenerateChart(cReportRequest request, DataSet ds)
        {
            var chart = request.ReportChart == null ?  ReportChart.Get(
                request.report.reportid,
                new CurrentUser(this.accountid, 0, 0, GlobalVariables.DefaultModule, 0)) : request.ReportChart;
            if (chart.XAxis == -1 || chart.YAxis == -1)
            {
                return ds;
            }

            try
            {
                if (chart.DisplayType == SpendManagementLibrary.Chart.ChartType.Pie || chart.DisplayType == SpendManagementLibrary.Chart.ChartType.Donut ||
                    chart.DisplayType == SpendManagementLibrary.Chart.ChartType.Funnel)
                {
                    chart.GroupBy = -1;
                }

                if (chart.GroupBy > -1)
                {
                    int groupBy = chart.GroupBy;
                    chart.GroupBy = chart.XAxis;
                    chart.XAxis = groupBy;
                }

                var cc = new ColorConverter();
                var textFont = new Font("Arial", chart.TextFont == 0 ? 10 : chart.TextFont);
                var convertFrom = cc.ConvertFrom('#' + (string.IsNullOrEmpty(chart.TextFontColour) ? "000000" : chart.TextFontColour));
                if (convertFrom != null)
                {
                    var textColour = (Color)convertFrom;
                    var textBackColour = (Color)cc.ConvertFrom('#' + (string.IsNullOrEmpty(chart.TextBackgroundColour) ? "FFFFFF" : chart.TextBackgroundColour));
                    var titleFont = new Font("Arial", chart.ChartTitleFont == 0 ? 10 : chart.ChartTitleFont);
                    var titleColour = (Color) cc.ConvertFrom('#' + (string.IsNullOrEmpty(chart.ChartTitleColour) ? "000000" : chart.ChartTitleColour));
            
                    var chartArea1 = CreateChartArea(request, chart, textFont, textColour, textBackColour);

                    var legend1 = CreateChartLegend(request, chart, textFont, textColour, textBackColour);

                    SeriesChartType chartType;
                    var chart1 = CreateChart(chart, textFont, textColour, chartArea1, legend1, out chartType);

                    var newTable = ds.Tables[0].Clone();
                    SetDefaultTableValues(newTable);

                    RemoveUnwantedColumns(request, chart, newTable);

                    var columns = ds.Tables[0].Columns;

                    var viewSort = this.ConsolidateUniqueValuesInTable(ds, columns, chart, newTable);

                    var currentView = new DataView(newTable) { Sort = viewSort };

                    SetDefaultValuesInTable(newTable);

                    if (chart.GroupBy == -1)
                    {
                        chart1.DataBindTable(currentView, GetColumnName(columns, chart.XAxis));
                    }
                    else
                    {
                        chart1.DataBindCrossTable(currentView, GetColumnName(columns, chart.GroupBy), GetColumnName(columns, chart.XAxis), GetColumnName(columns, chart.YAxis), string.Empty);
                    }

                    foreach (var series in chart1.Series)
                    {
                        FormatSeries(series, chartType, chart, request, ds.Tables[0].Columns);
                    }

                    CreateChartTitle(chart, titleFont, titleColour, chart1);

                    FormatChartArea(chart1, chart);

                    if (chart.GroupBy  != -1)
                    {
                        try
                        {
                            chart1.AlignDataPointsByAxisLabel();
                        }
                        catch (Exception)
                        {
                            // This is deliberate, there are some data sets that cannot be indexed, but the user would still want to see a chart.
                        }
                    }

                    var imageFileName = ExportChartImage(request.report.reportid, ds.Tables[0].TableName, chart1);
                    chart1.Invalidate();
                    AddTableToOutputDataset(ds, imageFileName, chart);
                    newTable.Reset();
                    newTable.Dispose();
                
                    currentView.Dispose();
                }
            }
            catch (Exception ex)
            {
                LogError(ex, "GenerateChart");
                return ds;
            }
            return ds;
        }

        /// <summary>
        /// On sucessful chart creation, add the chart link as a new table to the returned dataset.
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="imageFileName"></param>
        /// <param name="chart"></param>
        private static void AddTableToOutputDataset(DataSet ds, Guid imageFileName, ReportChart chart)
        {
            var link = string.Format("{0}.{1}?time={2}", imageFileName, "jpg", DateTime.UtcNow.TimeOfDay);
            var chartTable = new DataTable("ReportChart");
            chartTable.Columns.Add(chart.ChartTitle);
            chartTable.Rows.Add(link);
            ds.Tables.Add(chartTable);
        }

        /// <summary>
        /// Export the chart as a image into the folder defined in the config file.
        /// </summary>
        /// <param name="reportId">The Id of the report, used as the file name if a report viewer request.</param>
        /// <param name="tableName">the table name of the data table</param>
        /// <param name="chart1">The chart object to render.</param>
        /// <returns></returns>
        private static Guid ExportChartImage(Guid reportId, string tableName, System.Windows.Forms.DataVisualization.Charting.Chart chart1)
        {
            var sharedDirectory = ConfigurationManager.AppSettings["tempDocMergeImageLocation"].TrimEnd('\\');
            Guid imageFileName = tableName == "ChartMergeData" ? Guid.NewGuid() : reportId;
            chart1.SaveImage(string.Format(@"{0}\{1}.jpg", sharedDirectory, imageFileName), ChartImageFormat.Jpeg);
            return imageFileName;
        }

        /// <summary>
        /// Set formatting optons for the Chart Area.
        /// </summary>
        /// <param name="chart1">The output chart</param>
        /// <param name="chart">The expenses chart object.</param>
        private static void FormatChartArea(System.Windows.Forms.DataVisualization.Charting.Chart chart1, ReportChart chart)
        {
            chart1.ChartAreas[0].AxisX.LabelStyle.Enabled = chart.GroupBy > -1 || !chart.ShowLegend;
            chart1.ChartAreas[0].AxisX.LabelStyle.Interval = 1;
            chart1.ChartAreas[0].AxisX.IsLabelAutoFit = true;
            chart1.ChartAreas[0].AxisX.LabelAutoFitStyle = LabelAutoFitStyles.LabelsAngleStep30;
        }

        /// <summary>
        /// Create a title for the current chart
        /// </summary>
        /// <param name="chart">The expenses chart object</param>
        /// <param name="titleFont">The font for the title</param>
        /// <param name="titleColour">The colour for the title</param>
        /// <param name="chart1">The ourput chart object.</param>
        private static void CreateChartTitle(ReportChart chart, Font titleFont, Color titleColour, System.Windows.Forms.DataVisualization.Charting.Chart chart1)
        {
            var title = new Title(chart.ChartTitle, Docking.Top, titleFont, titleColour);
            chart1.Titles.Add(title);
        }

        /// <summary>
        /// Create a new output chart object.
        /// </summary>
        /// <param name="chart">Expenses chart object</param>
        /// <param name="textFont"></param>
        /// <param name="textColour"></param>
        /// <param name="chartArea1"></param>
        /// <param name="legend1"></param>
        /// <param name="chartType"></param>
        /// <returns></returns>
        private static System.Windows.Forms.DataVisualization.Charting.Chart CreateChart(ReportChart chart, Font textFont, Color textColour, ChartArea chartArea1,
            Legend legend1, out SeriesChartType chartType)
        {
            var size = (chart.Size * 100);

            var chart1 = new System.Windows.Forms.DataVisualization.Charting.Chart
            {
                Font = textFont,
                ForeColor = textColour,
                Name = "chart1",
                Size = new Size(size, (int)(size * .8)),
                TabIndex = 0,
                Text = "chart1"
            };

            chart1.Series.Clear();

            chart1.ChartAreas.Add(chartArea1);
            chart1.Legends.Add(legend1);


            chartType = SeriesChartType.Pie;
            switch (chart.DisplayType)
            {
                case SpendManagementLibrary.Chart.ChartType.Area:
                    chartType = SeriesChartType.Area;
                    break;
                case SpendManagementLibrary.Chart.ChartType.Bar:
                    chartType = SeriesChartType.Bar;
                    break;
                case SpendManagementLibrary.Chart.ChartType.Column:
                    chartType = SeriesChartType.Column;
                    break;
                case SpendManagementLibrary.Chart.ChartType.Donut:
                    chartType = SeriesChartType.Doughnut;
                    break;
                case SpendManagementLibrary.Chart.ChartType.Dot:
                    chartType = SeriesChartType.Point;
                    break;
                case SpendManagementLibrary.Chart.ChartType.Line:
                    chartType = SeriesChartType.Line;
                    break;
                case SpendManagementLibrary.Chart.ChartType.Pie:
                    chartType = SeriesChartType.Pie;
                    break;
                case SpendManagementLibrary.Chart.ChartType.Funnel:
                    chartType = SeriesChartType.Funnel;
                    break;
            }
            return chart1;
        }

        /// <summary>
        /// Create and format a legend for the chart.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="chart"></param>
        /// <param name="textFont"></param>
        /// <param name="textColour"></param>
        /// <param name="textBackColour"></param>
        /// <returns></returns>
        private static Legend CreateChartLegend(cReportRequest request, ReportChart chart, Font textFont, Color textColour,
            Color textBackColour)
        {
            var legendField = chart.GroupBy;
            if (legendField == -1)
            {
                switch (chart.DisplayType)
                {
                    case SpendManagementLibrary.Chart.ChartType.Donut:
                    case SpendManagementLibrary.Chart.ChartType.Funnel:
                    case SpendManagementLibrary.Chart.ChartType.Pie:
                        legendField = chart.XAxis;
                        break;
                    default:
                        legendField = chart.YAxis;
                        break;
                }
            }

            var docking = Docking.Bottom;
            var alignment = StringAlignment.Center;

            switch (chart.LegendPosition)
            {
                case SpendManagementLibrary.Chart.Position.TopRight:
                    docking = Docking.Right;
                    alignment = StringAlignment.Near;
                    break;
                case SpendManagementLibrary.Chart.Position.TopLeft:
                    docking = Docking.Left;
                    alignment = StringAlignment.Near;
                    break;
                case SpendManagementLibrary.Chart.Position.BottomRight:
                    docking = Docking.Right;
                    alignment = StringAlignment.Far;
                    break;
                case SpendManagementLibrary.Chart.Position.BottomLeft:
                    docking = Docking.Left;
                    alignment = StringAlignment.Far;
                    break;
            }

            var legend1 = new Legend
            {
                Name = "Legend1",
                Title = GetFieldTitle(request, legendField, chart.GroupBy),
                Docking = docking,
                Alignment = alignment,
                DockedToChartArea = "ChartArea1",
                Font = textFont,
                ForeColor = textColour,
                BackColor = textBackColour,
                TitleFont = textFont,
                TitleForeColor = textColour,
                IsDockedInsideChartArea = false
            };
            return legend1;
        }

        /// <summary>
        /// Create a new chart area for the current chart.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="chart"></param>
        /// <param name="textFont"></param>
        /// <param name="textColour"></param>
        /// <param name="textBackColour"></param>
        /// <returns></returns>
        private static ChartArea CreateChartArea(cReportRequest request, ReportChart chart, Font textFont, Color textColour,
            Color textBackColour)
        {
            var chartArea1 = new ChartArea
            {
                AxisY =
                {
                    Title = GetFieldTitle(request, chart.YAxis, chart.GroupBy),
                    TitleFont = textFont,
                    TitleForeColor = textColour,
                    LabelStyle = {Font = textFont, ForeColor = textColour}
                },
                AxisX =
                {
                    TitleFont = textFont,
                    TitleForeColor = textColour,
                    Title = GetFieldTitle(request, chart.XAxis, chart.GroupBy),
                    LabelStyle = {Font = textFont, ForeColor = textColour}
                },
                BackColor = textBackColour,
                Name = "ChartArea1",
                BorderWidth = 2,
                BorderColor = Color.Black
            };

            if (chart.DisplayType == SpendManagementLibrary.Chart.ChartType.Pie ||
                chart.DisplayType == SpendManagementLibrary.Chart.ChartType.Donut)
            {
                chartArea1.Area3DStyle.Enable3D = true;
                chartArea1.Area3DStyle.Inclination = 0;
            }
            return chartArea1;
        }

        /// <summary>
        /// If a cell is empty, set the default value to prevent errors in chart generation.
        /// </summary>
        /// <param name="newTable"></param>
        private static void SetDefaultValuesInTable(DataTable newTable)
        {
            foreach (DataRow dataRow in newTable.Rows)
            {
                foreach (DataColumn dataColumn in newTable.Columns)
                {
                    if (dataRow[dataColumn.ColumnName].ToString() == string.Empty)
                    {
                        dataRow[dataColumn.ColumnName] = dataColumn.DefaultValue;
                    }
                }
            }
        }

        /// <summary>
        /// Find the unique values in the x and group columns and colsolidate the value (y) column.
        /// </summary>
        /// <param name="ds">the dataset to process</param>
        /// <param name="columns">The report columns that match the data source</param>
        /// <param name="chart">The chat class that defines the output required</param>
        /// <param name="newTable">The new dataset including a link to the generated chart.</param>
        private string ConsolidateUniqueValuesInTable(DataSet ds, DataColumnCollection columns, ReportChart chart,
            DataTable newTable)
        {
            string xAxisColumnName = null;
            string yAxisColumnName = null;
            string groupByColumnName = null;
            if (ds == null || ds.Tables.Count == 0)
            {
                return string.Empty;
            }

            foreach (DataRow row in ds.Tables[0].Rows)
            {
                if (xAxisColumnName == null)
                {
                    xAxisColumnName = GetColumnName(columns, chart.XAxis);
                    yAxisColumnName = GetColumnName(columns, chart.YAxis);
                    groupByColumnName = GetColumnName(columns, chart.GroupBy);
                }
                var xValue = row[xAxisColumnName];
                var yValue = row[yAxisColumnName];
                var groupValue = chart.GroupBy == -1 ? null : row[groupByColumnName];

                // find yValue (and group) in new Table and update or create new row
                var found = false;
                foreach (DataRow newDataRow in newTable.Rows)
                {
                    if (newDataRow[xAxisColumnName].ToString() == xValue.ToString() &&
                        (groupValue == null ||
                         newDataRow[groupByColumnName].ToString() == groupValue.ToString()))
                    {
                        var currentValue = newDataRow[yAxisColumnName];

                        newDataRow[yAxisColumnName] = this.AddValues(currentValue.GetType(), currentValue, yValue);
                        found = true;
                    }
                }

                if (!found)
                {
                    var newRow = newTable.NewRow();
                    newRow[xAxisColumnName] = row[xAxisColumnName];
                    newRow[yAxisColumnName] = row[yAxisColumnName];
                    if (chart.GroupBy > -1)
                    {
                        newRow[groupByColumnName] = row[groupByColumnName];
                    }

                    newTable.Rows.Add(newRow);
                }
            }

            return string.Format("{0}{1}{2}", groupByColumnName,string.IsNullOrEmpty(groupByColumnName) ? string.Empty : ",", xAxisColumnName);
        }

        /// <summary>
        /// Get the column name from a given index.
        /// </summary>
        /// <param name="columns">
        /// The columns.
        /// </param>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="string"/> Column name.
        /// </returns>
        private static string GetColumnName(DataColumnCollection columns, int index)
        {
            var axisColumn = GetColumn(columns, index);
            return axisColumn != null ? axisColumn.ColumnName : string.Empty;
        }

        /// <summary>
        /// Remove all columns from the table apart from x y and group by.
        /// </summary>
        /// <param name="request">The report request to process</param>
        /// <param name="chart">The current chart</param>
        /// <param name="newTable">The new table with the required columns.</param>
        private static void RemoveUnwantedColumns(cReportRequest request, ReportChart chart, DataTable newTable)
        {
            for (int i = request.report.columns.Count; i > 0; i--)
            {
                var column = (cReportColumn)request.report.columns[i - 1];

                var columnId = column.columnid - 1;
                if (columnId != chart.XAxis && columnId != chart.YAxis && columnId != chart.GroupBy)
                {
                    newTable.Columns.RemoveAt(i - 1);
                }
            }
        }

        /// <summary>
        /// Define default values for empty cells in the chart data table.
        /// </summary>
        /// <param name="newTable"></param>
        private static void SetDefaultTableValues(DataTable newTable)
        {
            foreach (DataColumn dataColumn in newTable.Columns)
            {
                switch (dataColumn.DataType.Name)
                {
                    case "String":
                        dataColumn.DefaultValue = "[None]";
                        break;
                    case "DateTime":
                        dataColumn.DefaultValue = new DateTime(1900, 1, 1);
                        break;
                    default:
                        dataColumn.DefaultValue = 0;
                        break;
                }
            }
        }

        /// <summary>
        /// Add values to summary table.
        /// </summary>
        /// <param name="getType">The type of the column.</param>
        /// <param name="currentValue">Current cell value.</param>
        /// <param name="yValue">Additional value.</param>
        /// <returns></returns>
        private object AddValues(Type getType, object currentValue, object yValue)
        {
            switch (getType.Name)
            {
                case "Int32":
                    return (int) currentValue + (int) yValue;
                case "Int16":
                    return (short)currentValue + (short)yValue;
                case "Double":
                    return (double)currentValue + (double)yValue;
                case "Float":
                    return (float)currentValue + (float)yValue;
                case "Decimal":
                    return (decimal)currentValue + (decimal)yValue;
            }

            return currentValue;
        }

        /// <summary>
        /// Set the options for a series based on the defined chart.
        /// </summary>
        /// <param name="series">the chart series to format</param>
        /// <param name="chartType">The current chart type</param>
        /// <param name="chart">The chart class</param>
        /// <param name="request">The report request</param>
        /// <param name="columns">The current DATASETCOLUMNS</param>
        private static void FormatSeries(Series series, SeriesChartType chartType, ReportChart chart, cReportRequest request, DataColumnCollection columns)
        {
            series.ChartType = chartType;
            series.IsValueShownAsLabel = chart.ShowValues;
            series.SmartLabelStyle.AllowOutsidePlotArea = LabelOutsidePlotAreaStyle.Partial;
            var label = string.Empty;
            var collectedLabel = "Other ";
            var cc = new ColorConverter();
            var textFont = new Font("Arial", chart.TextFont == 0 ? 10 : chart.TextFont);
            var textColour = (Color)cc.ConvertFrom('#' + (string.IsNullOrEmpty(chart.TextFontColour) ? "000000" : chart.TextFontColour));

            series.Font = textFont;
            series.LabelForeColor = textColour;
            series.IsVisibleInLegend = chart.ShowLegend;

            switch (chart.DisplayType)
            {
                case SpendManagementLibrary.Chart.ChartType.Area:                    
                case SpendManagementLibrary.Chart.ChartType.Bar:                    
                case SpendManagementLibrary.Chart.ChartType.Column:                    
                case SpendManagementLibrary.Chart.ChartType.Dot:                    
                case SpendManagementLibrary.Chart.ChartType.Line:                    
                var column = GetColumn(request, chart.YAxis);
                if (chart.GroupBy == -1)
                {
                    series.LegendText = GetFieldFunction(column);
                }
                else
                {
                    var columnName = GetColumnName(columns, chart.GroupBy);
                    var groupColumn = GetColumn(request, chart.GroupBy);
                    if (groupColumn != null)
                    {
                        var legendText = series.Name.Replace(columnName, GetFieldName(groupColumn));
                        series.LegendText = legendText;
                        series.Name = legendText;
                    }
                    else
                    {
                        series.LegendText = GetFieldFunction(column);
                    }
                }
                break;
            }

            if (chart.ShowLabels)
            {
                label = chart.GroupBy == -1 ? "#VALX" : "#SERIESNAME";
            }

            if (chart.ShowValues)
            {
                label += "\n#VALY";
            }

            if (chart.ShowPercent)
            {
                label += "\n#PERCENT{P2}";
                collectedLabel += "\n#PERCENT{P2}";
                series.LegendText = "#VALX";
            }

            series.BorderWidth = 5;

            if (!chart.ShowLabels && !chart.ShowValues && !chart.ShowPercent)
            {
                series["PieLabelStyle"] = "Disabled";
            }
            else
            {
                series.Label = label;

                if (chart.DisplayType == SpendManagementLibrary.Chart.ChartType.Pie || chart.DisplayType == SpendManagementLibrary.Chart.ChartType.Donut || chart.DisplayType == SpendManagementLibrary.Chart.ChartType.Funnel)
                {
                    series["PieLabelStyle"] = "Outside";
                    series.BorderWidth = 1;
                    series.BorderDashStyle = ChartDashStyle.Solid;
                    series.BorderColor = Color.Black;
                }
            }

            series["CollectedThreshold"] = chart.CombineOthersPercentage.ToString();
            series["CollectedThresholdUsePercent"] = (chart.CombineOthersPercentage > 0).ToString();
            series["CollectedLabel"] = collectedLabel;
            series.MarkerSize = 15;

            if (chart.DisplayType == SpendManagementLibrary.Chart.ChartType.Donut || chart.DisplayType == SpendManagementLibrary.Chart.ChartType.Pie || chart.DisplayType == SpendManagementLibrary.Chart.ChartType.Funnel)
            {
                series.LegendText = "#VALX";
            }
        }

        /// <summary>
        /// Get the Title for the given field.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="chartAxis"></param>
        /// <param name="groupBy"></param>
        /// <returns></returns>
        private static string GetFieldTitle(cReportRequest request, int chartAxis, int groupBy)
        {
            var column = GetColumn(request, chartAxis);

            if (column is cStandardColumn)
            {
                var standardColumn = (cStandardColumn)column;
                var function = GetFieldFunction(standardColumn);
                if (string.IsNullOrEmpty(function))
                {
                    return standardColumn.field.Description;
                }

                return function + "of " + standardColumn.field.Description;    
            }

            if (column is cCalculatedColumn)
            {
                return ((cCalculatedColumn)column).columnname;    
            }

            return ((cStaticColumn)column).literalname;
        }

        /// <summary>
        /// Return the column from the report.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private static cReportColumn GetColumn(cReportRequest request, int index)
        {
            if (index > request.report.columns.Count -1 || index < 0)
            {
                return null;
            }

            return request.report.columns[index] as cReportColumn;
        }

        private static DataColumn GetColumn(DataColumnCollection columns, int index)
        {
            if (index > columns.Count - 1 || index < 0)
            {
                return null;
            }

            return columns[index];
        }

        /// <summary>
        /// Get the function used for the field.
        /// </summary>
        /// <param name="column"></param>
        /// <returns></returns>
        private static string GetFieldFunction(cReportColumn column)
        {
            if (column is cStandardColumn)
            {
                var standardColumn = (cStandardColumn)column;
                if (standardColumn.funccount)
                {
                    return "Count ";
                }

                if (standardColumn.funcsum)
                {
                    return "Sum ";
                }

                if (standardColumn.funcavg)
                {
                    return "Average ";
                }

                if (standardColumn.funcmax)
                {
                    return "Maximum ";
                }

                if (standardColumn.funcmin)
                {
                    return "Minumum ";
                } 
            }

            return string.Empty;
        }

        /// <summary>
        /// Get the fieldname, column name or literal name from a given report column
        /// </summary>
        /// <param name="reportColumn">The report column to extract the name from</param>
        /// <returns>Column name</returns>
        internal static string GetFieldName(cReportColumn reportColumn)
        {
            if (reportColumn is cStandardColumn)
            {
                var standardColumn = (cStandardColumn)reportColumn;
                return string.IsNullOrEmpty(standardColumn.DisplayName) ? standardColumn.field.Description : standardColumn.DisplayName;
            }

            if (reportColumn is cCalculatedColumn)
            {
                return ((cCalculatedColumn)reportColumn).columnname;
            }

            return ((cStaticColumn)reportColumn).literalname;
        }

        /// <summary>
        /// Get the current Export options for the given employee and report
        /// </summary>
        /// <param name="employeeid">The ID of the employee</param>
        /// <param name="report">The instance of <see cref="cReport"/>to reference</param>
        /// <param name="isFinancialExport">True if this is a financial export report</param>
        /// <returns>A new instance of <see cref="cExportOptions"/>with either the stored options or a default set of options.</returns>
        public cExportOptions getExportOptions(int employeeid, cReport report, bool isFinancialExport = false)
        {
            bool headersDefault = !isFinancialExport;
            bool headersexcel = headersDefault;
            bool headerscsv = headersDefault;
            bool headersflatfile = headersDefault;
            bool removecarriagereturns = false;
            bool encloseinspeechmarks = true;
            Guid footerid = Guid.Empty;
            Guid drilldownreport = Guid.Empty;
            string delimiter = string.Empty;

            using (var connection = new DatabaseConnection(cReportsSvc.GetConnectionString(this.accountid)))
            {
                const string Sql =
                    "select * from dbo.reports_export_options where employeeid = @employeeid and reportid = @reportid";
                connection.sqlexecute.Parameters.AddWithValue("@employeeid", employeeid);
                connection.sqlexecute.Parameters.AddWithValue("@reportid", report.reportid);
                using (var reader = connection.GetReader(Sql))
                {
                    connection.sqlexecute.Parameters.Clear();
                    while (reader.Read())
                    {
                        headersexcel = reader.GetBoolean(reader.GetOrdinal("excelheader"));
                        headerscsv = reader.GetBoolean(reader.GetOrdinal("csvheader"));
                        headersflatfile = reader.GetBoolean(reader.GetOrdinal("flatfileheader"));
                        if (reader.IsDBNull(reader.GetOrdinal("footerid")) == false)
                        {
                            footerid = reader.GetGuid(reader.GetOrdinal("footerid"));
                        }

                        if (reader.IsDBNull(reader.GetOrdinal("drilldownreport")) == false)
                        {
                            drilldownreport = reader.GetGuid(reader.GetOrdinal("drilldownreport"));
                        }

                        delimiter = reader.IsDBNull(reader.GetOrdinal("delimiter"))
                                        ? ","
                                        : reader.GetString(reader.GetOrdinal("delimiter"));
                        removecarriagereturns = reader.GetBoolean(reader.GetOrdinal("removeCarriageReturns"));
                        encloseinspeechmarks = reader.GetBoolean(reader.GetOrdinal("encloseInSpeechMarks"));
                    }

                    reader.Close();
                }
            }

            cReport footer = this.getReportById(footerid);
            var clsoptions = new cExportOptions(
                employeeid,
                report.reportid,
                headersexcel,
                headerscsv,
                headersflatfile,
                this.getFlatFileOptions(employeeid, report),
                footer,
                drilldownreport,
                FinancialApplication.CustomReport,
                delimiter,
                removecarriagereturns,
                encloseinspeechmarks,
                false);

            return clsoptions;
        }

        public DataSet getHistoryGrid(Guid reportid)
        {
            DataSet history;
            using (var connection = new DatabaseConnection(cReportsSvc.GetConnectionString(this.accountid)))
            {
                const string SQL =
                    "select exporthistoryid, exportnum, dateexported, employees.surname + ', ' + employees.title + ' ' + employees.firstname as employee from dbo.exporthistory inner join dbo.employees on employees.employeeid = exporthistory.employeeid where reportid = @reportid order by dateexported desc";
                connection.sqlexecute.Parameters.AddWithValue("@reportid", reportid);
                history = connection.GetDataSet(SQL);
                connection.sqlexecute.Parameters.Clear();
            }

            return history;
        }

        public cReport getReportById(Guid reportid)
        {
            return this.GetReportFromDb(reportid);
        }

        /// <summary>
        /// Gets the number of records in the requested report..
        /// </summary>
        /// <param name="request">
        /// The report request object.
        /// </param>
        /// <param name="accountId">
        /// The account id.
        /// </param>
        /// <param name="fields">
        /// The fields class
        /// </param>
        /// <param name="clsjoins">
        /// The Joins class.
        /// </param>
        /// <param name="joinVias">
        /// The join vias class
        /// </param>
        /// <returns>
        /// The number of records returned for the requested report.
        /// </returns>
        public int getReportCount(cReportRequest request, int accountId, cFields fields, cJoins clsjoins, JoinVias joinVias)
        {
            string strsql = string.Empty;
            int count = 0;
            using (var connection = new DatabaseConnection(cReportsSvc.GetConnectionString(this.accountid)))
            {
                if (string.IsNullOrEmpty(request.report.StaticReportSQL))
                {
                    if (request.report.Limit > 0)
                    {
                        return request.report.Limit;
                    }

                    if (request.claimantreport && cReport.canFilterByRole(request.report.basetable.TableID))
                    {
                        // add employeeid clause
                        cField reqfield = fields.GetFieldByID(new Guid(ReportKeyFields.EmployeesEmployeeId));
                        var criteria = new cReportCriterion(
                            new Guid(),
                            request.report.reportid,
                            reqfield,
                            ConditionType.Equals,
                            new object[] { request.employeeid },
                            new object[0],
                            ConditionJoiner.And,
                            request.report.criteria.Count + 1,
                            false,
                            0);
                        request.report.addCriteria(criteria);
                    }

                    string joinsql = string.Empty;
                    var trunk = new ViaBranch(request.report.basetable.TableName);
                    var joins = new List<string>();
                    if (request.report.UseJoinVia)
                    {
                        var joinViaList = new Dictionary<JoinVia, cField>();
                        var tableids = new List<Guid>();
                        foreach (cReportColumn column in request.report.columns)
                        {
                            if (column is cStandardColumn)
                            {
                                var standardColumn = (cStandardColumn)column;
                                if (column.JoinVia != null && !joinViaList.ContainsKey(column.JoinVia))
                                {
                                    joinViaList.Add(column.JoinVia, standardColumn.field);
                                    if (!tableids.Contains(standardColumn.field.TableID))
                                    {
                                        tableids.Add(standardColumn.field.TableID);
                                    }
                                }
                            }
                        }

                        trunk = new ViaBranch(request.report.basetable.TableName);
                        
                        foreach (cReportCriterion column in request.report.criteria)
                        {
                            if (column.JoinVia != null && !joinViaList.ContainsKey(column.JoinVia))
                            {
                                joinViaList.Add(column.JoinVia, column.field);
                                if (!tableids.Contains(column.field.TableID))
                                {
                                    tableids.Add(column.field.TableID);
                                }
                            }
                        }

                        foreach (KeyValuePair<JoinVia, cField> keyValuePair in joinViaList.OrderBy(j => j.Key.JoinViaList.Count))
                        {
                            clsjoins.GetJoinViaSQL(
                                ref joins,
                                ref trunk,
                                keyValuePair.Key,
                                keyValuePair.Value,
                                request.report.basetable.TableID);
                        }
                        joins = this.AddMissingJoins(request, clsjoins, joins, ref trunk, fields, joinVias, tableids);
                    }
                    else
                    {
                        var lstFields = request.report.columns.OfType<cStandardColumn>().Select(column => (column).field).ToList();
                        lstFields.AddRange(request.report.criteria.OfType<cReportCriterion>().Select(column => (column).field).ToList());
                        joins.Add(clsjoins.createReportJoinSQL(lstFields, request.report.basetable.TableID, request.AccessLevel));
                    }

                    var sb = new StringBuilder();
                    foreach (string strjoin in joins)
                    {
                        sb.Append(strjoin);
                    }

                    joinsql = sb.ToString();

                    request.report.JoinSQL = joinsql;
                    request.report.AccessLevel = request.AccessLevel;
                    request.report.AccessLevelRoles = request.AccessLevelRoles;
                    request.report.employeeid = request.employeeid;
                    strsql = request.report.createReportCount(trunk, this.employeeJoinVia);

                    this.setCriteriaParameters(request, connection);

                    cEventlog.LogEntry(
                        string.Format("ReportEngine: getReportCount : Sql : {0}{1}", Environment.NewLine, strsql));

                    count = connection.ExecuteScalar<int>(strsql);
                }
                else
                {
                    if (request.report.AccessLevel == AccessRoleLevel.EmployeesResponsibleFor
                        && cReport.canFilterByRole(request.report.basetable.TableID))
                    {
                        strsql = "declare @linemanagers as LineManagers; ";
                    }

                    strsql += request.report.StaticReportSQL;
                    if (cReport.canFilterByRole(request.report.basetable.TableID))
                    {
                        // need to identify position of WHERE in the SQL in order to insert required filter (-1 if not found)
                        int whereIdx = strsql.IndexOf("<% WHERE_INSERT %>", StringComparison.Ordinal);
                        int andIdx = strsql.IndexOf("<% AND_INSERT %>", StringComparison.Ordinal);

                        if (whereIdx > -1)
                        {
                            switch (request.report.AccessLevel)
                            {
                                case AccessRoleLevel.EmployeesResponsibleFor:
                                    strsql = strsql.Replace(
                                        "<% WHERE_INSERT %>",
                                        string.Format(
                                            " employees.employeeid in (select employeeid from dbo.resolveReport({0},@linemanagers))",
                                            request.employeeid));

                                    // additional parameter used if static SQL doesn't have a WHERE clause and needs the 'WHERE' keyword inserting 
                                    strsql = strsql.Replace("<% WHERECMD_INSERT %>", " WHERE ");
                                    if (andIdx > -1)
                                    {
                                        strsql = strsql.Replace("<% AND_INSERT %>", " AND ");
                                    }

                                    break;
                                case AccessRoleLevel.SelectedRoles:

                                    // additional parameter used if static SQL doesn't have a WHERE clause and needs the 'WHERE' keyword inserting 
                                    strsql = strsql.Replace("<% WHERECMD_INSERT %>", " WHERE ");
                                    if (andIdx > -1)
                                    {
                                        strsql = strsql.Replace("<% AND_INSERT %>", " AND ");
                                    }

                                    if (request.report.AccessLevelRoles != null
                                        && request.report.AccessLevelRoles.Length > 0)
                                    {
                                        var ehrBuilder = new StringBuilder();
                                        string theOr = string.Empty;
                                        ehrBuilder.Append(" (");
                                        for (int x = 0; x < request.report.AccessLevelRoles.Length; x++)
                                        {
                                            ehrBuilder.Append(
                                                theOr + "dbo.employeeHasRole("
                                                + request.report.AccessLevelRoles[x]
                                                    .ToString(CultureInfo.InvariantCulture)
                                                + ", dbo.employees.employeeid, " + request.SubAccountId + ") = 1 ");
                                            theOr = " or ";
                                        }

                                        ehrBuilder.Append(")");
                                        strsql = strsql.Replace("<% WHERE_INSERT %>", ehrBuilder.ToString());
                                    }
                                    else
                                    {
                                        // no roles selected, so set clause with access role of zero, so no data returned (but valid SQL)
                                        strsql = strsql.Replace(
                                            "<% WHERE_INSERT %>",
                                            " (dbo.employeeHasRole(0, dbo.employees.employeeid, " + request.SubAccountId
                                            + ") = 1)");
                                    }

                                    break;
                                default:
                                    strsql = strsql.Replace("<% WHERE_INSERT %>", string.Empty);
                                    strsql = strsql.Replace("<% AND_INSERT %>", string.Empty);
                                    strsql = strsql.Replace("<% WHERECMD_INSERT %>", string.Empty);
                                    break;
                            }
                        }
                    }
                    else
                    {
                        strsql = strsql.Replace("<% WHERE_INSERT %>", string.Empty);
                        strsql = strsql.Replace("<% AND_INSERT %>", string.Empty);
                        strsql = strsql.Replace("<% WHERECMD_INSERT %>", string.Empty);
                    }
                    using (DataSet ds = connection.GetDataSet(strsql))
                    {
                        if (ds != null && ds.Tables.Count > 0 && ds.Tables[0] != null)
                        {
                            count = ds.Tables[0].Rows.Count;
                            ds.Clear();
                        }
                    }
                }

                if (string.IsNullOrEmpty(request.report.StaticReportSQL) == false && count > 10000
                    && request.report.Limit > 0)
                {
                    count = request.report.Limit;
                }

                connection.sqlexecute.Parameters.Clear();
            }

            return count;
        }

        public void setCriteriaParameters(cReportRequest request, IDBConnection connection)
        {
            //if (request.report.basetable.SubAccountIDFieldID != Guid.Empty && request.SubAccountId != null)
            {
                connection.sqlexecute.Parameters.AddWithValue("@subaccountid", request.SubAccountId);
            }

            for (int i = 0; i < request.report.criteria.Count; i++)
            {
                var criteria = (cReportCriterion)request.report.criteria[i];
                if (criteria.field.GenList && criteria.drilldown == false)
                {
                    if (criteria.condition != ConditionType.ContainsData
                        && criteria.condition != ConditionType.DoesNotContainData)
                    {
                        for (int x = 0; x < criteria.value1.GetLength(0); x++)
                        {
                            if (criteria.value1[x] != null)
                            {
                                this.AddUniqueParameterToSqlCommand(ref connection, string.Format("@value1_{0}_{1}", criteria.order, x), criteria.value1[x]);
                            }
                        }
                    }
                }
                else
                {
                    DateTime startdate, enddate;

                    DateTime[] tempdates;
                    switch (criteria.condition)
                    {
                        case ConditionType.Equals:
                        case ConditionType.On:
                        case ConditionType.DoesNotEqual:
                        case ConditionType.NotOn:
                        case ConditionType.GreaterThan:
                        case ConditionType.After:
                        case ConditionType.LessThan:
                        case ConditionType.Before:
                        case ConditionType.GreaterThanEqualTo:
                        case ConditionType.OnOrAfter:
                        case ConditionType.LessThanEqualTo:
                        case ConditionType.OnOrBefore:
                        case ConditionType.Like:
                        case ConditionType.NotLike:
                            if (criteria.field.FieldType == "T")
                            {
                                startdate = GetDateTimeParameterFromTimeCriteria(criteria.value1[0].ToString());
                                this.AddUniqueParameterToSqlCommand(ref connection, "@value1_" + criteria.order, startdate);
                            }
                            else
                            {
                                if (criteria.value1[0] is string)
                                {
                                    DateTime dateFromString;
                                    if (DateTime.TryParse(criteria.value1[0].ToString(), out dateFromString))
                                    {
                                        this.AddUniqueParameterToSqlCommand(
                                            ref connection,
                                            string.Format("@value1_{0}", criteria.order),
                                            dateFromString);
                                        break;
                                    }
                                }

                                this.AddUniqueParameterToSqlCommand(
                                    ref connection,
                                    string.Format("@value1_{0}", criteria.order),
                                    criteria.value1[0]);
                            }

                            break;
                        case ConditionType.Between:
                            if (criteria.field.FieldType == "T")
                            {
                                startdate = GetDateTimeParameterFromTimeCriteria(criteria.value1[0].ToString());
                                enddate = GetDateTimeParameterFromTimeCriteria(criteria.value2[0].ToString());
                                this.AddUniqueParameterToSqlCommand(ref connection, "@value1_" + criteria.order, startdate);
                                this.AddUniqueParameterToSqlCommand(ref connection, "@value2_" + criteria.order, enddate);
                            }
                            else
                            {
                                this.AddUniqueParameterToSqlCommand(ref connection, "@value1_" + criteria.order, criteria.value1[0]);
                                this.AddUniqueParameterToSqlCommand(ref connection, "@value2_" + criteria.order, criteria.value2[0]);
                            }

                            break;
                        case ConditionType.Today:
                            this.AddUniqueParameterToSqlCommand(ref connection, "@value1_" + criteria.order, DateTime.Today);
                            this.AddUniqueParameterToSqlCommand(ref connection, "@value2_" + criteria.order, DateTime.Today.AddDays(1).AddSeconds(-1));
                            break;
                        case ConditionType.OnOrAfterToday:
                        case ConditionType.OnOrBeforeToday:
                            this.AddUniqueParameterToSqlCommand(ref connection, "@value1_" + criteria.order, DateTime.Today);
                            break;
                        case ConditionType.Tomorrow:
                            this.AddUniqueParameterToSqlCommand(ref connection, "@value1_" + criteria.order, DateTime.Today.AddDays(1));
                            this.AddUniqueParameterToSqlCommand(ref connection, "@value2_" + criteria.order, DateTime.Today.AddDays(2).AddSeconds(-1));
                            break;
                        case ConditionType.Yesterday:
                            this.AddUniqueParameterToSqlCommand(ref connection, "@value1_" + criteria.order, DateTime.Today.AddDays(-1));
                            this.AddUniqueParameterToSqlCommand(ref connection, "@value2_" + criteria.order, DateTime.Today.AddSeconds(-1));
                            break;
                        case ConditionType.ThisYear:
                            startdate = new DateTime(DateTime.Today.Year, 01, 01);
                            enddate = new DateTime(DateTime.Today.Year, 12, 31, 23, 59, 59);
                            this.AddUniqueParameterToSqlCommand(ref connection, "@value1_" + criteria.order, startdate);
                            this.AddUniqueParameterToSqlCommand(ref connection, "@value2_" + criteria.order, enddate);
                            break;
                        case ConditionType.NextYear:
                            startdate = new DateTime(DateTime.Today.Year + 1, 01, 01);
                            enddate = new DateTime(DateTime.Today.Year + 1, 12, 31, 23, 59, 59);
                            this.AddUniqueParameterToSqlCommand(ref connection, "@value1_" + criteria.order, startdate);
                            this.AddUniqueParameterToSqlCommand(ref connection, "@value2_" + criteria.order, enddate);
                            break;
                        case ConditionType.LastYear:
                            startdate = new DateTime(DateTime.Today.Year - 1, 01, 01);
                            enddate = new DateTime(DateTime.Today.Year - 1, 12, 31, 23, 59, 59);
                            this.AddUniqueParameterToSqlCommand(ref connection, "@value1_" + criteria.order, startdate);
                            this.AddUniqueParameterToSqlCommand(ref connection, "@value2_" + criteria.order, enddate);
                            break;
                        case ConditionType.LastXYears:
                            startdate = new DateTime(DateTime.Today.Year - CastCriteriaValue(criteria.value1[0]), 01, 01);
                            enddate = new DateTime(DateTime.Today.Year, 12, 31, 23, 59, 59);
                            this.AddUniqueParameterToSqlCommand(ref connection, "@value1_" + criteria.order, startdate);
                            this.AddUniqueParameterToSqlCommand(ref connection, "@value2_" + criteria.order, enddate);
                            break;
                        case ConditionType.ThisTaxYear:
                            if (DateTime.Today.Month >= 4)
                            {
                                startdate = new DateTime(DateTime.Today.Year, 04, 06);
                                enddate = new DateTime(DateTime.Today.Year + 1, 04, 05, 23, 59, 59);
                            }
                            else
                            {
                                startdate = new DateTime(DateTime.Today.Year - 1, 04, 06);
                                enddate = new DateTime(DateTime.Today.Year, 04, 05, 23, 59, 59);
                            }

                            this.AddUniqueParameterToSqlCommand(ref connection, "@value1_" + criteria.order, startdate);
                            this.AddUniqueParameterToSqlCommand(ref connection, "@value2_" + criteria.order, enddate);
                            break;
                        case ConditionType.ThisFinancialYear:
                            tempdates = this.getFinancialYear();
                            startdate = tempdates[0];
                            enddate = tempdates[1];
                            this.AddUniqueParameterToSqlCommand(ref connection, "@value1_" + criteria.order, startdate);
                            this.AddUniqueParameterToSqlCommand(ref connection, "@value2_" + criteria.order, enddate);
                            break;
                        case ConditionType.LastTaxYear:
                            if (DateTime.Today.Month >= 4)
                            {
                                startdate = new DateTime(DateTime.Today.Year - 1, 04, 06);
                                enddate = new DateTime(DateTime.Today.Year, 04, 05, 23, 59, 59);
                            }
                            else
                            {
                                startdate = new DateTime(DateTime.Today.Year - 2, 04, 06);
                                enddate = new DateTime(DateTime.Today.Year - 1, 04, 05, 23, 59, 59);
                            }

                            this.AddUniqueParameterToSqlCommand(ref connection, "@value1_" + criteria.order, startdate);
                            this.AddUniqueParameterToSqlCommand(ref connection, "@value2_" + criteria.order, enddate);
                            break;
                        case ConditionType.LastFinancialYear:
                            tempdates = this.getFinancialYear();
                            startdate = tempdates[0];
                            enddate = tempdates[1];
                            startdate = startdate.AddYears(-1);
                            enddate = enddate.AddYears(-1);
                            this.AddUniqueParameterToSqlCommand(ref connection, "@value1_" + criteria.order, startdate);
                            this.AddUniqueParameterToSqlCommand(ref connection, "@value2_" + criteria.order, enddate);
                            break;
                        case ConditionType.NextTaxYear:
                            if (DateTime.Today.Month >= 4)
                            {
                                startdate = new DateTime(DateTime.Today.Year + 1, 04, 06);
                                enddate = new DateTime(DateTime.Today.Year + 2, 04, 05, 23, 59, 59);
                            }
                            else
                            {
                                startdate = new DateTime(DateTime.Today.Year, 04, 06);
                                enddate = new DateTime(DateTime.Today.Year + 1, 04, 05, 23, 59, 59);
                            }

                            this.AddUniqueParameterToSqlCommand(ref connection, "@value1_" + criteria.order, startdate);
                            this.AddUniqueParameterToSqlCommand(ref connection, "@value2_" + criteria.order, enddate);
                            break;
                        case ConditionType.NextFinancialYear:
                            tempdates = this.getFinancialYear();
                            startdate = tempdates[0];
                            enddate = tempdates[1];
                            startdate = startdate.AddYears(1);
                            enddate = enddate.AddYears(1);
                            this.AddUniqueParameterToSqlCommand(ref connection, "@value1_" + criteria.order, startdate);
                            this.AddUniqueParameterToSqlCommand(ref connection, "@value2_" + criteria.order, enddate);
                            break;
                        case ConditionType.NextXYears:
                            startdate = new DateTime(DateTime.Today.Year, 01, 01);
                            enddate = new DateTime(DateTime.Today.Year + CastCriteriaValue(criteria.value1[0]), 12, 31, 23, 59, 59);
                            this.AddUniqueParameterToSqlCommand(ref connection, "@value1_" + criteria.order, startdate);
                            this.AddUniqueParameterToSqlCommand(ref connection, "@value2_" + criteria.order, enddate);
                            break;
                        case ConditionType.ThisMonth:
                            startdate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 01);
                            enddate = new DateTime(
                                startdate.Year,
                                startdate.Month,
                                DateTime.DaysInMonth(startdate.Year, startdate.Month),
                                23,
                                59,
                                59);
                            this.AddUniqueParameterToSqlCommand(ref connection, "@value1_" + criteria.order, startdate);
                            this.AddUniqueParameterToSqlCommand(ref connection, "@value2_" + criteria.order, enddate);
                            break;
                        case ConditionType.LastMonth:
                            startdate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 01);
                            startdate = startdate.AddMonths(-1);
                            enddate = new DateTime(
                                startdate.Year,
                                startdate.Month,
                                DateTime.DaysInMonth(startdate.Year, startdate.Month),
                                23,
                                59,
                                59);
                            this.AddUniqueParameterToSqlCommand(ref connection, "@value1_" + criteria.order, startdate);
                            this.AddUniqueParameterToSqlCommand(ref connection, "@value2_" + criteria.order, enddate);
                            break;
                        case ConditionType.NextMonth:
                            startdate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 01);
                            startdate = startdate.AddMonths(1);
                            enddate = new DateTime(
                                startdate.Year,
                                startdate.Month,
                                DateTime.DaysInMonth(startdate.Year, startdate.Month),
                                23,
                                59,
                                59);
                            this.AddUniqueParameterToSqlCommand(ref connection, "@value1_" + criteria.order, startdate);
                            this.AddUniqueParameterToSqlCommand(ref connection, "@value2_" + criteria.order, enddate);
                            break;
                        case ConditionType.LastXMonths:
                            startdate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 01);
                            startdate = startdate.AddMonths(CastCriteriaValue(criteria.value1[0]) / -1);
                            enddate = DateTime.Today.AddDays(1).AddSeconds(-1);
                            this.AddUniqueParameterToSqlCommand(ref connection, "@value1_" + criteria.order, startdate);
                            this.AddUniqueParameterToSqlCommand(ref connection, "@value2_" + criteria.order, enddate);
                            break;
                        case ConditionType.NextXMonths:
                            startdate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 01);
                            enddate = startdate.AddMonths(CastCriteriaValue(criteria.value1[0])).AddSeconds(-1);
                            this.AddUniqueParameterToSqlCommand(ref connection, "@value1_" + criteria.order, startdate);
                            this.AddUniqueParameterToSqlCommand(ref connection, "@value2_" + criteria.order, enddate);
                            break;
                        case ConditionType.Last7Days:
                            startdate = DateTime.Today.AddDays(-7);
                            enddate = DateTime.Today.AddDays(1).AddSeconds(-1);
                            this.AddUniqueParameterToSqlCommand(ref connection, "@value1_" + criteria.order, startdate);
                            this.AddUniqueParameterToSqlCommand(ref connection, "@value2_" + criteria.order, enddate);
                            break;
                        case ConditionType.LastWeek:
                            startdate = this.getStartOfWeek();
                            startdate = startdate.AddDays(-7);
                            enddate = startdate.AddDays(7).AddSeconds(-1);
                            this.AddUniqueParameterToSqlCommand(ref connection, "@value1_" + criteria.order, startdate);
                            this.AddUniqueParameterToSqlCommand(ref connection, "@value2_" + criteria.order, enddate);
                            break;
                        case ConditionType.LastXDays:
                            startdate = DateTime.Today.AddDays(CastCriteriaValue(criteria.value1[0]) / -1);
                            enddate = DateTime.Today.AddDays(1).AddSeconds(-1);
                            this.AddUniqueParameterToSqlCommand(ref connection, "@value1_" + criteria.order, startdate);
                            this.AddUniqueParameterToSqlCommand(ref connection, "@value2_" + criteria.order, enddate);
                            break;
                        case ConditionType.LastXWeeks:
                            startdate = DateTime.Today;
                            startdate = startdate.AddDays(-7 * CastCriteriaValue(criteria.value1[0]));
                            enddate = DateTime.Today.AddDays(1).AddSeconds(-1);
                            this.AddUniqueParameterToSqlCommand(ref connection, "@value1_" + criteria.order, startdate);
                            this.AddUniqueParameterToSqlCommand(ref connection, "@value2_" + criteria.order, enddate);
                            break;
                        case ConditionType.Next7Days:
                            startdate = DateTime.Today;
                            enddate = DateTime.Today.AddDays(8).AddSeconds(-1);
                            this.AddUniqueParameterToSqlCommand(ref connection, "@value1_" + criteria.order, startdate);
                            this.AddUniqueParameterToSqlCommand(ref connection, "@value2_" + criteria.order, enddate);
                            break;
                        case ConditionType.NextWeek:
                            startdate = this.getStartOfWeek().AddDays(7);
                            enddate = startdate.AddDays(7).AddSeconds(-1);
                            this.AddUniqueParameterToSqlCommand(ref connection, "@value1_" + criteria.order, startdate);
                            this.AddUniqueParameterToSqlCommand(ref connection, "@value2_" + criteria.order, enddate);
                            break;
                        case ConditionType.NextXDays:
                            startdate = DateTime.Today;
                            enddate = DateTime.Today.AddDays(CastCriteriaValue(criteria.value1[0]) + 1).AddSeconds(-1);
                            this.AddUniqueParameterToSqlCommand(ref connection, "@value1_" + criteria.order, startdate);
                            this.AddUniqueParameterToSqlCommand(ref connection, "@value2_" + criteria.order, enddate);
                            break;
                        case ConditionType.NextXWeeks:
                            startdate = DateTime.Today;
                            enddate = startdate.AddDays((7 * CastCriteriaValue(criteria.value1[0])) + 1).AddSeconds(-1);
                            this.AddUniqueParameterToSqlCommand(ref connection, "@value1_" + criteria.order, startdate);
                            this.AddUniqueParameterToSqlCommand(ref connection, "@value2_" + criteria.order, enddate);
                            break;
                        case ConditionType.ThisWeek:
                            startdate = this.getStartOfWeek();
                            enddate = startdate.AddDays(7).AddSeconds(-1);
                            this.AddUniqueParameterToSqlCommand(ref connection, "@value1_" + criteria.order, startdate);
                            this.AddUniqueParameterToSqlCommand(ref connection, "@value2_" + criteria.order, enddate);
                            break;
                    }
                }
            }
        }

        private static int CastCriteriaValue(object criteria)
        {
            var result = 0;
            int.TryParse(criteria.ToString(), out result);
            return result;
        }

        /// <summary>
        /// Update the drilldown report for the current report and employee
        /// </summary>
        /// <param name="employeeid">The ID of the employee </param>
        /// <param name="report">An instance of <see cref="cReport"/>to update the xport option for</param>
        /// <param name="drilldown">The ID <see cref="Guid"/>of the Report to use for drilling down</param>
        public void updateDrillDownReport(int employeeid, cReport report, Guid drilldown)
        {
            using (var connection = new DatabaseConnection(cReportsSvc.GetConnectionString(this.accountid)))
            {
                string strsql =
                    "select count(*) from dbo.reports_export_options where employeeid = @employeeid and reportid = @reportid";
                connection.sqlexecute.Parameters.AddWithValue("@employeeid", employeeid);
                connection.sqlexecute.Parameters.AddWithValue("@reportid", report.reportid);

                var count = connection.ExecuteScalar<int>(strsql);

                strsql = count == 0 ? "insert into dbo.reports_export_options (employeeid, reportid, drilldownreport) values (@employeeid, @reportid, @drilldown)" : "update dbo.reports_export_options set drilldownreport = @drilldown where employeeid = @employeeid and reportid = @reportid";

                connection.sqlexecute.Parameters.AddWithValue("@drilldown", drilldown);
                connection.ExecuteSQL(strsql);
                connection.sqlexecute.Parameters.Clear();
            }

            cExportOptions options = this.getExportOptions(employeeid, report);
            options.drilldownreport = drilldown;
        }

        public void updateExportOptions(cExportOptions options)
        {
            using (var connection = new DatabaseConnection(cReportsSvc.GetConnectionString(this.accountid)))
            {
                string strsql =
                    "select count(*) from dbo.reports_export_options where employeeid = @employeeid and reportid = @reportid";
                connection.sqlexecute.Parameters.AddWithValue("@employeeid", options.employeeid);
                connection.sqlexecute.Parameters.AddWithValue("@reportid", options.reportid);
                var count = connection.ExecuteScalar<int>(strsql);
                connection.sqlexecute.Parameters.Clear();
                if (count == 0)
                {
                    strsql =
                        "insert into dbo.reports_export_options (employeeid, reportid, excelheader, csvheader, flatfileheader, footerid, delimiter, removeCarriageReturns, encloseInSpeechMarks) "
                        + "values (@employeeid, @reportid, @excelheader, @csvheader, @flatfileheader, @footerid, @delimiter, @removeCarriageReturns, @encloseInSpeechMarks)";
                }
                else
                {
                    strsql =
                        "update dbo.reports_export_options set excelheader = @excelheader, csvheader = @csvheader, flatfileheader = @flatfileheader, footerid = @footerid, delimiter = @delimiter, removeCarriageReturns = @removeCarriageReturns, encloseInSpeechMarks = @encloseInSpeechMarks where employeeid = @employeeid and reportid = @reportid";
                }

                connection.sqlexecute.Parameters.AddWithValue("@employeeid", options.employeeid);
                connection.sqlexecute.Parameters.AddWithValue("@reportid", options.reportid);
                connection.sqlexecute.Parameters.AddWithValue("@excelheader", Convert.ToByte(options.showheadersexcel));
                connection.sqlexecute.Parameters.AddWithValue("@csvheader", Convert.ToByte(options.showheaderscsv));
                connection.sqlexecute.Parameters.AddWithValue("@flatfileheader", Convert.ToByte(options.showheadersflatfile));
                if (options.footerreport == null)
                {
                    connection.sqlexecute.Parameters.AddWithValue("@footerid", DBNull.Value);
                }
                else
                {
                    connection.sqlexecute.Parameters.AddWithValue("@footerid", options.footerreport.reportid);
                }

                connection.sqlexecute.Parameters.AddWithValue("@delimiter", options.Delimiter);
                connection.sqlexecute.Parameters.AddWithValue(
                    "@removeCarriageReturns", Convert.ToByte(options.RemoveCarriageReturns));
                connection.sqlexecute.Parameters.AddWithValue(
                    "@encloseInSpeechMarks", Convert.ToByte(options.EncloseInSpeechMarks));
                connection.ExecuteSQL(strsql);
                connection.sqlexecute.Parameters.Clear();
            }

            this.addFlatFileOptions(options);
        }

        #endregion

        #region Methods

        /// <summary>
        /// The get date time parameter from time criteria.
        /// </summary>
        /// <param name="timeCriteria">
        /// The time criteria.
        /// </param>
        /// <returns>
        /// The <see cref="DateTime"/>.
        /// </returns>
        private static DateTime GetDateTimeParameterFromTimeCriteria(string timeCriteria)
        {
            DateTime now = DateTime.Today;
            string[] hoursandmins = timeCriteria.Split(':');
            var ts = new TimeSpan(Convert.ToInt32(hoursandmins[0]), Convert.ToInt32(hoursandmins[1]), 0);
            return now.Date + ts;
        }

        private static void LogError(Exception ex, string methodName, string methodMessage = "")
        {
            string message = ex.Message;
            string methodInfo = methodMessage == string.Empty ? "Error" : methodMessage;
            if (ex.InnerException != null)
            {
                message = string.Format("{0}{1}{1}{2}", ex.Message, Environment.NewLine, ex.InnerException.Message);
            }

            cEventlog.LogEntry(
                string.Format(
                    "ReportEngine : cReportsSvc : {0} : {1} : {2}{3}{4}{5}",
                    methodName,
                    methodInfo,
                    Environment.NewLine,
                    message,
                    Environment.NewLine,
                    ex.StackTrace));
        }

        /// <summary>
        /// Read the specified report from the database
        /// </summary>
        /// <param name="reportId">The <see cref="Guid"/>ID to read.</param>
        /// <returns>An instance of <see cref="cReport"/></returns>
        private cReport GetReportFromDb(Guid reportId)
        {
            if (reportId == Guid.Empty)
            {
                return null;
            }

            cReport reqreport = new cReport();
            
            var clstables = new cTables(this.nAccountid);
            var fields = new cFields(this.nAccountid);
            ICurrentUserBase user = cMisc.GetCurrentUser($"{this.nAccountid},{0}");
            var joinVias = new JoinVias(user);
            var joinToJoinViaFactory = new JoinToJoinViaFactory(joinVias, fields, clstables, new cJoins(this.nAccountid), new DebugLogger(user));
            var employeeid = 0;
            var reportname = string.Empty;
            var description = String.Empty;
            var basetable = Guid.Empty;
            cTable clsbasetable = null;
            var folderid = Guid.Empty;
            var reportreadonly = false;
            var forclaimants = false;
            short limit = 0;
            var reportArea = Modules.None;
            var staticSql = string.Empty;
            int? subaccountid = null;
            var useJoinVia = false;
            var showChart = ShowChartFlag.Never;
            using (var connection = new DatabaseConnection(cReportsSvc.GetConnectionString(this.accountid)))
            {
                const string SQL = "select  reportid, employeeid, reportname, description, personalreport, basetable, curexportnum, lastexportdate, footerreport, folderid, readonly, forclaimants, allowexport, exporttype, CreatedOn, CreatedBy, ModifiedOn, ModifiedBy, limit, reportArea, staticReportSQL, subAccountID, useJoinVia, showChart, globalReport from dbo.reportsview where reportid = @reportid";

                connection.sqlexecute.Parameters.AddWithValue("@reportid", reportId);

                using (IDataReader reader = connection.GetReader(SQL))
                {
                    var employeeIdOrd = reader.GetOrdinal("employeeid");
                    var reportNameOrd = reader.GetOrdinal("reportname");
                    var descriptionOrd = reader.GetOrdinal("description");
                    var baseTableOrd = reader.GetOrdinal("basetable");
                    var folderIdOrd = reader.GetOrdinal("folderid");
                    var readOnlyOrd = reader.GetOrdinal("readonly");
                    var forClaimantsOrd = reader.GetOrdinal("forclaimants");
                    var limitOrd = reader.GetOrdinal("limit");
                    var reportAreaOrd = reader.GetOrdinal("reportArea");
                    var staticReportSqlOrd = reader.GetOrdinal("staticReportSQL");
                    var subAccountIdOrd = reader.GetOrdinal("subAccountID");
                    var useJoinViaOrd = reader.GetOrdinal("useJoinVia");
                    var showChartOrd = reader.GetOrdinal("showChart");
                    var globalReportOrd = reader.GetOrdinal("globalReport");
                    while (reader.Read())
                    {
                        employeeid = reader.IsDBNull(employeeIdOrd) ? 0 : reader.GetInt32(employeeIdOrd);

                        reportname = reader.GetString(reportNameOrd);
                        description = reader.IsDBNull(descriptionOrd) == false ? reader.GetString(descriptionOrd) : string.Empty;

                        basetable = reader.GetGuid(baseTableOrd);
                        clsbasetable = clstables.GetTableByID(basetable);

                        folderid = reader.IsDBNull(folderIdOrd) == false ? reader.GetGuid(folderIdOrd) : Guid.Empty;

                        reportreadonly = reader.GetBoolean(readOnlyOrd);
                        forclaimants = reader.GetBoolean(forClaimantsOrd);
                        limit = reader.GetInt16(limitOrd);

                        reportArea = reader.IsDBNull(reportAreaOrd) ? Modules.None : (Modules)reader.GetByte(reportAreaOrd);
                        
                        if (!reader.IsDBNull(staticReportSqlOrd))
                        {
                            staticSql = reader.GetString(staticReportSqlOrd);
                        }
                        
                        if (reader.IsDBNull(subAccountIdOrd))
                        {
                            subaccountid = null;
                        }
                        else
                        {
                            subaccountid = reader.GetInt32(subAccountIdOrd);
                        }
                        

                        if (!reader.IsDBNull(useJoinViaOrd))
                        {
                            useJoinVia = reader.GetBoolean(useJoinViaOrd);
                        }
                        

                        if (!reader.IsDBNull(showChartOrd))
                        {
                            showChart = (ShowChartFlag) reader.GetByte(showChartOrd);
                        }

                    }

                    reader.Close();
                }

                connection.sqlexecute.Parameters.Clear();
            }

            ArrayList columns, criteria;
            IFields accountFields = null;
            if (subaccountid.HasValue)
            {
                cAccountSubAccounts clsSubAccounts = new cAccountSubAccounts(this.accountid);
                cAccountProperties accountProperties = clsSubAccounts.getSubAccountById(subaccountid.Value).SubAccountProperties;
                accountFields = new SubAccountFields(fields, new FieldRelabler(accountProperties));
            }
            else
            {
                accountFields = fields;
            }
            

            try
            {
                columns = this.GetColumnsFromDb(reportId, accountFields);
                criteria = this.GetCriteriaFromDb(reportId, accountFields);
            }
            catch (Exception ex)
            {
                var clsEmails = new cEmails();
                clsEmails.sendErrorMail(ex, this.accountid);
                LogError(ex, "GetReportFromDB", "Error encountered in GetReportFromDB");
                return null;
            }

            if (!useJoinVia && string.IsNullOrEmpty(staticSql))
            {
                
                foreach (cReportColumn column in columns)
                {
                    if (column is cStandardColumn && column.JoinVia == null)
                    {
                        var standardColumn = (cStandardColumn) column;
                        if (standardColumn.field.TableID != basetable)
                        {
                            column.JoinVia = joinToJoinViaFactory.Convert(standardColumn.field.TableID, basetable);
                        }
                    }
                    
                }

                foreach (cReportCriterion criterion in criteria)
                {
                    if (criterion.JoinVia == null && criterion.field.TableID != basetable)
                    {
                    criterion.JoinVia = joinToJoinViaFactory.Convert(criterion.field.TableID, basetable);
                    }
                }

                Spend_Management.cReports.AddSystemColumns(columns, fields, reportId, joinVias, clsbasetable);
                useJoinVia = true;
            }


            if (employeeid > 0)
            {
                reqreport = new cReport(
                    this.accountid,
                    subaccountid,
                    reportId,
                    employeeid,
                    reportname,
                    description,
                    folderid,
                    clsbasetable,
                    forclaimants,
                    reportreadonly,
                    columns,
                    criteria,
                    limit,
                    reportArea,
                    useJoinVia,
                    showChart);
            }
            else
            {
                reqreport = new cReport(
                    reportId,
                    subaccountid,
                    reportname,
                    description,
                    folderid,
                    clsbasetable,
                    forclaimants,
                    reportreadonly,
                    columns,
                    criteria,
                    staticSql,
                    limit,
                    reportArea,
                    useJoinVia,
                    showChart);
            }
                   

            return reqreport;
        }

        /// <summary>
        /// Logs information about a report request to the database
        /// </summary>
        /// <param name="request">
        /// The report request
        /// </param>
        /// <param name="sql">
        /// The report SQL containing parameters
        /// </param>
        /// <param name="connection">
        /// The database connection
        /// </param>
        private void LogRequest(cReportRequest request, string sql, IDBConnection connection)
        {
            var financialExport = request.report.exportoptions != null && request.report.exportoptions.isfinancialexport;

            // substitute the @parameters in the SQL string for their actual values, so that the query can be pulled from the log and ran again
            try
            {
                foreach (SqlParameter parameter in connection.sqlexecute.Parameters)
                {
                    var value = parameter.Value.ToString();

                    // DateTime values must be formatted
                    DateTime dateTimeValue;
                    if (parameter.SqlDbType == SqlDbType.DateTime
                        && DateTime.TryParse(parameter.Value.ToString(), out dateTimeValue))
                    {
                        value = dateTimeValue.ToString("s");
                    }

                    sql = sql.Replace(parameter.ParameterName, string.Format("'{0}'", value));
                }
            }
            catch (Exception exception)
            {
                sql = string.Format("{0}\n\n{1}\n\n{2}", "An error occured generating the SQL", exception, sql);
            }

            using (var logData = new DatabaseConnection(cReportsSvc.GetConnectionString(this.accountid)))
            {
                var logParameters = logData.sqlexecute.Parameters;
                logParameters.AddWithValue("@reportId", request.report.reportid);
                logParameters.AddWithValue("@employeeId", request.employeeid);
                logParameters.AddWithValue("@reportName", request.report.reportname);
                logParameters.AddWithValue("@baseTableId", request.report.basetable.TableID);
                logParameters.AddWithValue("@limit", request.report.Limit);
                logParameters.AddWithValue("@subAccountId", request.report.SubAccountID);
                logParameters.AddWithValue("@forClaimants", request.report.claimantreport);
                logParameters.AddWithValue("@requestNumber", request.requestnum);
                logParameters.AddWithValue("@isFinancialExport", financialExport);
                logParameters.AddWithValue("@sql", sql);

                logData.ExecuteProc("AddReportsLogEntry");
                logParameters.Clear();
            }
        }

        private void addFlatFileOptions(cExportOptions options)
        {
            using (var connection = new DatabaseConnection(cReportsSvc.GetConnectionString(this.accountid)))
            {
                string strsql =
                    "delete from dbo.reportcolumns_flatfile where reportcolumnid in (select reportcolumnid from dbo.reportcolumns where reportid = @reportid) and employeeid = @employeeid";
                connection.sqlexecute.Parameters.AddWithValue("@reportid", options.reportid);
                connection.sqlexecute.Parameters.AddWithValue("@employeeid", options.employeeid);
                connection.ExecuteSQL(strsql);
                connection.sqlexecute.Parameters.Clear();

                for (int i = 0; i < options.flatfile.Count; i++)
                {
                    strsql = "insert into dbo.reportcolumns_flatfile (reportcolumnid, employeeid, columnlength) "
                             + "values (@reportcolumnid, @employeeid, @columnlength)";

                    connection.sqlexecute.Parameters.AddWithValue("@reportcolumnid", options.flatfile.Keys[i]);
                    connection.sqlexecute.Parameters.AddWithValue("@employeeid", options.employeeid);
                    connection.sqlexecute.Parameters.AddWithValue("@columnlength", options.flatfile.Values[i]);
                    connection.ExecuteSQL(strsql);
                    connection.sqlexecute.Parameters.Clear();
                }
            }
        }

        /// <summary>
        /// Get the list of columns for the specified report
        /// </summary>
        /// <param name="reportId">The <see cref="Guid"/>ID of the report</param>
        /// <param name="fields">An instance of <see cref="IFields"/></param>
        /// <returns>An <see cref="ArrayList"/>of <seealso cref="cReportColumn"/></returns>
        private ArrayList GetColumnsFromDb(Guid reportId, IFields fields)
        {
            var columns = new ArrayList();
            cReportColumn column = null;
            const string SQL = "select fieldid, groupby, sort, [order], aggfunction, funcsum, funcmax, funcmin, funcavg, funccount, isLiteral, literalname, literalvalue, length, format, removedecimals, pivottype, pivotorder, runtime, columntype, hidden, reportcolumnid, reportid, joinViaID, [system] from dbo.reportcolumnsview where reportid = @reportId order by [order]";
            var joinVias = new JoinVias(new CurrentUser(this.accountid, 0, 0, GlobalVariables.DefaultModule, 0));

            using (var connection = new DatabaseConnection(cReportsSvc.GetConnectionString(this.accountid)))
            {
                connection.sqlexecute.Parameters.AddWithValue("@reportId", reportId);

                using (IDataReader reader = connection.GetReader(SQL))
                {
                    var joinViaIdOrd = reader.GetOrdinal("joinViaID");
                    var systemOrd = reader.GetOrdinal("system");
                    while (reader.Read())
                    {
                        try
                        {
                            Guid reportid = reader.GetGuid(reader.GetOrdinal("reportid"));
                            var columntype = (ReportColumnType)reader.GetByte(reader.GetOrdinal("columntype"));
                            Guid columnid = reader.GetGuid(reader.GetOrdinal("reportcolumnid"));
                            var sort = (ColumnSort)reader.GetByte(reader.GetOrdinal("sort"));
                            int order = reader.GetInt32(reader.GetOrdinal("order"));
                            bool hidden = reader.GetBoolean(reader.GetOrdinal("hidden"));
                            string literalname;
                            string literalvalue;
                            int? joinViaID = null;

                            if (!reader.IsDBNull(joinViaIdOrd))
                            {
                                joinViaID = reader.GetInt32(joinViaIdOrd);
                            }
                            
                            var system = false;
                            if (!reader.IsDBNull(systemOrd))
                            {
                                system = reader.GetBoolean(systemOrd);
                            }

                            switch (columntype)
                            {
                                case ReportColumnType.Standard:
                                    bool groupby = reader.GetBoolean(reader.GetOrdinal("groupby"));
                                    bool max = reader.GetBoolean(reader.GetOrdinal("funcmax"));
                                    bool min = reader.GetBoolean(reader.GetOrdinal("funcmin"));
                                    bool sum = reader.GetBoolean(reader.GetOrdinal("funcsum"));
                                    bool avg = reader.GetBoolean(reader.GetOrdinal("funcavg"));
                                    bool count = reader.GetBoolean(reader.GetOrdinal("funccount"));
                                    var format = reader.IsDBNull(reader.GetOrdinal("format")) ? string.Empty : reader.GetString(reader.GetOrdinal("format"));

                                    if (reader.IsDBNull(reader.GetOrdinal("fieldid")))
                                    {
                                        column = null;
                                    }
                                    else
                                    {
                                        cField field = fields.GetFieldByID(reader.GetGuid(reader.GetOrdinal("fieldid")));
                                        if (field == null || field.FieldID == Guid.Empty)
                                        {
                                            column = null;
                                        }
                                        else
                                        {
                                            column = new cStandardColumn(
                                                columnid,
                                                reportid,
                                                ReportColumnType.Standard,
                                                sort,
                                                order,
                                                field,
                                                groupby,
                                                sum,
                                                min,
                                                max,
                                                avg,
                                                count,
                                                hidden,
                                                format,
                                                system);    
                                        }
                                        
                                    }

                                    break;
                                case ReportColumnType.Static:
                                    literalname = reader.GetString(reader.GetOrdinal("literalname"));
                                    literalvalue = reader.IsDBNull(reader.GetOrdinal("literalvalue")) == false ? reader.GetString(reader.GetOrdinal("literalvalue")) : string.Empty;

                                    bool runtime = reader.GetBoolean(reader.GetOrdinal("runtime"));
                                    column = new cStaticColumn(
                                        columnid,
                                        reportid,
                                        ReportColumnType.Static,
                                        sort,
                                        order,
                                        literalname,
                                        literalvalue,
                                        runtime,
                                        hidden);
                                    break;
                                case ReportColumnType.Calculated:
                                    literalname = reader.GetString(reader.GetOrdinal("literalname"));
                                    literalvalue = reader.IsDBNull(reader.GetOrdinal("literalvalue")) == false ? reader.GetString(reader.GetOrdinal("literalvalue")) : string.Empty;

                                    column = new cCalculatedColumn(
                                        columnid,
                                        reportid,
                                        ReportColumnType.Calculated,
                                        sort,
                                        order,
                                        literalname,
                                        literalvalue);
                                    break;
                            }

                            if (column != null)
                            {
                                if (joinViaID.HasValue)
                                {
                                    column.JoinVia = joinVias.GetJoinViaByID((int)joinViaID.Value);
                                }

                                columns.Add(column);
                            }
                        }
                        catch (Exception ex)
                        {
                            LogError(ex, "getColumnsFromDb");
                        }
                    }

                    reader.Close();
                }
            }

            return columns;
        }

        /// <summary>
        /// Get the criteria for the specified report
        /// </summary>
        /// <param name="reportID">The <see cref="Guid"/>ID of the report</param>
        /// <param name="fields">An instance of <see cref="IFields"/></param>
        /// <returns>An <see cref="ArrayList"/>of <seealso cref="cReportCriterion"/></returns>
        private ArrayList GetCriteriaFromDb(Guid reportID, IFields fields)
        {
            var criteria = new ArrayList();
            const string SQL = "select reportid, criteriaid, fieldid, condition, [order], runtime, andor, groupnumber,value1, value2, joinViaId from dbo.reportcriteriaview where reportId = @reportId order by [order]";
            var joinVias = new JoinVias(new CurrentUser(this.accountid, 0, 0, GlobalVariables.DefaultModule, this.nSubAccountId != null ? (int)this.nSubAccountId : 0));

            using (var connection = new DatabaseConnection(cReportsSvc.GetConnectionString(this.accountid)))
            {
                connection.sqlexecute.Parameters.AddWithValue("@reportId", reportID);
                using (IDataReader reader = connection.GetReader(SQL))
                {
                    while (reader.Read())
                    {
                        try
                        {
                            var reportid = reader.GetGuid(reader.GetOrdinal("reportid"));
                            var criteriaid = reader.GetGuid(reader.GetOrdinal("criteriaid"));
                            var fieldGuid = reader.IsDBNull(reader.GetOrdinal("fieldid")) ? Guid.Empty : reader.GetGuid(reader.GetOrdinal("fieldid"));
                            cField field = fieldGuid == Guid.Empty ? null : fields.GetFieldByID(fieldGuid);
                            if (field.FieldID == Guid.Empty)
                            {
                                field = null;
                            }

                            var condition = (ConditionType)reader.GetByte(reader.GetOrdinal("condition"));
                            var order = reader.GetInt32(reader.GetOrdinal("order"));
                            var runtime = reader.GetBoolean(reader.GetOrdinal("runtime"));
                            ConditionJoiner joiner;
                            if (reader.IsDBNull(reader.GetOrdinal("andor")))
                            {
                                joiner = ConditionJoiner.None;
                            }
                            else
                            {
                                joiner = (ConditionJoiner)reader.GetByte(reader.GetOrdinal("andor"));
                            }

                            byte groupnumber = reader.IsDBNull(reader.GetOrdinal("groupnumber")) == false ? reader.GetByte(reader.GetOrdinal("groupnumber")) : Convert.ToByte(0);

                            var value1 = new object[1];
                            var value2 = new object[1];
                            if (runtime == false && field != null && condition != ConditionType.ContainsData && condition != ConditionType.DoesNotContainData)
                            {
                                switch (field.FieldType)
                                {
                                    case "T":
                                    case "S":
                                    case "FS":
                                    case "LT":
                                        if (reader.IsDBNull(reader.GetOrdinal("value1")))
                                        {
                                            value1[0] = string.Empty;
                                        }
                                        else
                                        {
                                            value1[0] = reader.GetString(reader.GetOrdinal("value1"));
                                        }

                                        break;
                                    case "C":
                                    case "FC":
                                    case "M":
                                    case "FD":
                                    case "A":
                                    case "F":
                                        if (!reader.IsDBNull(reader.GetOrdinal("value1")))
                                        {
                                            value1[0] = decimal.Parse(reader.GetString(reader.GetOrdinal("value1")));
                                        }
                                        else
                                        {
                                            value1[0] = string.Empty;
                                        }

                                        break;
                                    case "N":
                                    case "I":
                                    case "FI":
                                        if (field.ValueList
                                            || (field.FieldSource != cField.FieldSourceType.Metabase
                                                && field.GetRelatedTable() != null && field.IsForeignKey))
                                        {
                                            if (reader.IsDBNull(reader.GetOrdinal("value1")))
                                            {
                                                value1[0] = string.Empty;
                                            }
                                            else
                                            {
                                                value1[0] = reader.GetString(reader.GetOrdinal("value1"));
                                            }
                                        }
                                        else
                                        {
                                            value1[0] = int.Parse(reader.GetString(reader.GetOrdinal("value1")));
                                        }

                                        break;
                                    case "X":
                                        value1[0] = byte.Parse(reader.GetString(reader.GetOrdinal("value1")));
                                        break;
                                    case "Y":
                                        value1[0] = reader.GetString(reader.GetOrdinal("value1"));
                                        break;
                                    case "D":
                                    case "DT":
                                        switch (condition)
                                        {
                                            case ConditionType.LastXDays:
                                            case ConditionType.LastXMonths:
                                            case ConditionType.LastXWeeks:
                                            case ConditionType.LastXYears:
                                            case ConditionType.NextXDays:
                                            case ConditionType.NextXMonths:
                                            case ConditionType.NextXWeeks:
                                            case ConditionType.NextXYears:
                                                value1[0] = int.Parse(reader.GetString(reader.GetOrdinal("value1")));
                                                break;
                                            default:
                                                if (reader.IsDBNull(reader.GetOrdinal("value1")) == false)
                                                {
                                                    value1[0] =
                                                        DateTime.Parse(reader.GetString(reader.GetOrdinal("value1")));
                                                }

                                                break;
                                        }

                                        break;
                                }

                                if (condition == ConditionType.Between)
                                {
                                    switch (field.FieldType)
                                    {
                                        case "T":
                                        case "S":
                                        case "FS":
                                        case "LT":
                                            value2[0] = reader.GetString(reader.GetOrdinal("value2"));
                                            break;
                                        case "C":
                                        case "M":
                                        case "FD":
                                        case "A":
                                        case "F":
                                            value2[0] = decimal.Parse(reader.GetString(reader.GetOrdinal("value2")));
                                            break;
                                        case "N":
                                            value2[0] = int.Parse(reader.GetString(reader.GetOrdinal("value2")));
                                            break;
                                        case "DT":
                                        case "D":
                                            value2[0] = DateTime.Parse(reader.GetString(reader.GetOrdinal("value2")));
                                            break;
                                    }
                                }
                            }

                            JoinVia joinVia = null;

                            if (!reader.IsDBNull(reader.GetOrdinal("joinViaID")))
                            {
                                joinVia = joinVias.GetJoinViaByID(reader.GetInt32(reader.GetOrdinal("joinViaID")));
                            }

                            if (field != null || condition == ConditionType.GroupAnd || condition == ConditionType.GroupOr)
                            {
                                var criterion = new cReportCriterion(
                                    criteriaid,
                                    reportid,
                                    field,
                                    condition,
                                    value1,
                                    value2,
                                    joiner,
                                    order,
                                    runtime,
                                    groupnumber,
                                    joinVia);
                                criteria.Add(criterion);
                            }
                        }
                        catch (Exception ex)
                        {
                            LogError(ex, "getCriteriaFromDb");
                        }
                    }

                    reader.Close();
                }
            }

            return criteria;
        }

        private DateTime[] getFinancialYear()
        {

            string start = "06/04";
            string end = "05/04";
            const string SQL = "select yearstart, yearend from FinancialYears where [Primary] = 1";

            using (var connection = new DatabaseConnection(cReportsSvc.GetConnectionString(this.accountid)))
            {
                using (IDataReader reader = connection.GetReader(SQL))
                {
                    int financialYearStart = reader.GetOrdinal("yearstart");
                    int financialYearEnd = reader.GetOrdinal("yearend");

                    while (reader.Read())
                    {
                        if (!reader.IsDBNull(financialYearStart))
                        {
                            start = reader.GetDateTime(financialYearStart).ToString("dd/MM");
                        }

                        if (!reader.IsDBNull(financialYearEnd))
                        {
                            end = reader.GetDateTime(financialYearEnd).ToString("dd/MM");
                        }
                    }

                    reader.Close();
                }
            }

            string[] startitems = start.Split('/');
            string[] enditems = end.Split('/');

            var financialyearstart = new DateTime(
                DateTime.Today.Year, int.Parse(startitems[1]), int.Parse(startitems[0]));
            var financialyearend = new DateTime(
                DateTime.Today.AddYears(1).Year, int.Parse(enditems[1]), int.Parse(enditems[0]), 23, 59, 59);
            if (int.Parse(enditems[1]) > int.Parse(startitems[1]))
            {
                financialyearend = financialyearend.AddYears(-1);
            }

            return new[] { financialyearstart, financialyearend };
        }

        /// <summary>
        /// Get the flat file export options for the current report and employee
        /// </summary>
        /// <param name="employeeid">The ID of the current employee</param>
        /// <param name="report">An instance of <see cref="cReport"/>to get / recreate the flat file export options for</param>
        /// <returns>a <see cref="SortedList{TKey,TValue}"/>of Report Column Id and width as an <see cref="int"/></returns>
        internal SortedList<Guid, int> getFlatFileOptions(int employeeid, cReport report)
        {
            var flatfile = new SortedList<Guid, int>();
            bool gotFFileColumns = false;

            using (var connection = new DatabaseConnection(cReportsSvc.GetConnectionString(this.accountid)))
            {
                string strsql =
                    "select * from dbo.reportcolumns_flatfile where reportcolumnid in (select reportcolumnid from dbo.reportcolumns where reportid = @reportid) and employeeid = @employeeid";
                connection.sqlexecute.Parameters.AddWithValue("@employeeid", employeeid);
                connection.sqlexecute.Parameters.AddWithValue("@reportid", report.reportid);

                using (var reader = connection.GetReader(strsql))
                {
                    connection.sqlexecute.Parameters.Clear();
                    var reportColumnIdOrd = reader.GetOrdinal("reportcolumnid");
                    var columnLengthOrd = reader.GetOrdinal("columnlength");
                    while (reader.Read())
                    {
                        var reportcolumnid = reader.GetGuid(reportColumnIdOrd);
                        var columnlength = reader.GetInt32(columnLengthOrd);
                        flatfile.Add(reportcolumnid, columnlength);
                        gotFFileColumns = true;
                    }

                    reader.Close();
                }

                if (!gotFFileColumns)
                {
                    // no flat file options, so set to default values
                    foreach (cReportColumn reportColumn in report.columns)
                    {
                        if (!flatfile.ContainsKey(reportColumn.reportcolumnid))
                        {
                            var length = 10;
                            if (reportColumn is cStandardColumn)
                            {
                                var standard = reportColumn as cStandardColumn;
                                if (standard.field.Length > 0)
                                {
                                    length = standard.field.Length;
                                }
                            }

                            flatfile.Add(reportColumn.reportcolumnid, length);
                        }
                    }
                }
            }

            return flatfile;
        }

        /// <summary>
        /// Gets the date of the monday in the week.
        /// </summary>
        /// <returns>
        /// The DateTime object for the Monday in this week.
        /// </returns>
        private DateTime getStartOfWeek()
        {
            DateTime date = DateTime.Today;

            while (date.DayOfWeek != DayOfWeek.Sunday)
            {
                date = date.AddDays(-1);
            }

            return date;
        }

        /// <summary>
        /// The add unique parameter to sql command.
        /// If already exists in parameters, do not add.
        /// </summary>
        /// <param name="parameters">
        /// The SqlparameterCollection to modify.
        /// </param>
        /// <param name="parameterName">
        /// The parameter name.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        private void AddUniqueParameterToSqlCommand(ref IDBConnection connection, string parameterName, object value)
        {
            if (!connection.sqlexecute.Parameters.Contains(parameterName))
            {
                connection.sqlexecute.Parameters.AddWithValue(parameterName, value ?? DBNull.Value);
            }
        }

        #endregion
    }
}