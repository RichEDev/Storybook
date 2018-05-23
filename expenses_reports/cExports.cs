namespace Expenses_Reports
{
    #region Using Directives

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data;
    using System.Data.SqlClient;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.Remoting.Channels;
    using System.Runtime.Remoting.Channels.Http;
    using System.Runtime.Serialization.Formatters;
    using System.Text;
    using System.Text.RegularExpressions;

    using BusinessLogic.Modules;

    using Infragistics.WebUI.UltraWebCalcManager;
    using Infragistics.WebUI.UltraWebGrid;
    using Microsoft.JScript;
    using Microsoft.JScript.Vsa;
    using SpendManagementLibrary;
    using SpendManagementLibrary.Employees;
    using SpendManagementLibrary.Helpers;
    using SpendManagementLibrary.Logic_Classes.Fields;

    using Spend_Management;

    using Syncfusion.XlsIO;

    using Convert = System.Convert;
    using Infragistics.WebUI.CalcEngine;

    using SpendManagementLibrary.Extentions;

    #endregion

    /// <summary>
    ///     Delegate function to export to excel as a delegate.
    /// </summary>
    /// <param name="request">
    ///     The report request object
    /// </param>
    public delegate byte[] exportExcelDelegate(cReportRequest request);

    /// <summary>
    ///     The cExports class containing the functions required to export to different formats.
    /// </summary>
    public class cExports
    {
        #region Fields

        /// <summary>
        ///     The payroll numbers.
        /// </summary>
        private readonly SortedList<int, string> payrollnumbers = new SortedList<int, string>();

        /// <summary>
        ///     The report request object.
        /// </summary>
        private cReportRequest request;

        private Relabler<cField> _relabeler;

        /// <summary>
        /// Renamed fields for use in export formulas
        /// </summary>
        private Dictionary<string, string> FieldLabels = new Dictionary<string, string>();

        public cExports(Relabler<cField> relabeler)
        {
            this._relabeler = relabeler;
        }

        const string CostAllocationKeyflex = "Cost Allocation Keyflex";

        #endregion

        #region Public Methods and Operators

        public void calculateFormula(ref string formula, DataSet dataSet, int row, int col, bool exportHeader)
        {
            if (formula.Contains("ROW()"))
            {
                formula = formula.Replace("ROW()", row.ToString(CultureInfo.InvariantCulture));
            }

            if (formula.Contains("COLUMN()"))
            {
                formula = formula.Replace("COLUMN()", col.ToString(CultureInfo.InvariantCulture));
            }

            while (formula.Contains("ADDRESS("))
            {
                var vsa = VsaEngine.CreateEngine();

                int startIndexForAddress = formula.IndexOf("ADDRESS(", StringComparison.Ordinal);
                int startIndex = startIndexForAddress + 8;
                int addressRowEndIndex = formula.IndexOf(',', startIndex);

                // Get the comma seperating the the arguments in address so we can find the next instance of a closing bracket below
                int addressColEndIndex = formula.IndexOf(')', addressRowEndIndex);
                string formulaVal = formula.Substring(startIndex, addressColEndIndex - startIndex);
                string newFormulaVal = formulaVal.Replace(" ", string.Empty);
                int indexOfComma = newFormulaVal.IndexOf(',');

                string rowExpression = newFormulaVal.Substring(0, indexOfComma);
                object rowIndex = null;
                int rowIndexInt = 1;
                try
                {
                    rowIndex = Eval.JScriptEvaluate(rowExpression, vsa);
                    int.TryParse(rowIndex.ToString(), out rowIndexInt);
                }
                catch (IndexOutOfRangeException e)
                {
                    rowIndexInt = (int)rowExpression.Calculate();
                }

                if (rowIndexInt <= 0)
                {
                    rowIndexInt = 1;
                }
                else
                {
                    if (exportHeader)
                    {
                        if (rowIndexInt == 1)
                        {
                            rowIndexInt = 2;
                        }
                    }
                }

                string colExpression = newFormulaVal.Substring(indexOfComma + 1);
                int columnIndexInt = 1;
                try
                {
                    object colIndex = Eval.JScriptEvaluate(colExpression, vsa);
                    
                    int.TryParse(colIndex.ToString(), out columnIndexInt);
                    if (columnIndexInt <= 0)
                    {
                        columnIndexInt = 1;
                    }
                }
                catch (IndexOutOfRangeException ex)
                {
                    columnIndexInt = (int)colExpression.Calculate();
                }

                string value;
                try
                {
                    value = exportHeader
                        ? dataSet.Tables[0].Rows[rowIndexInt - 2][columnIndexInt - 1].ToString()
                        : dataSet.Tables[0].Rows[rowIndexInt - 1][columnIndexInt - 1].ToString();
                }
                catch (Exception)
                {
                    value = "NULL()";
                }

                formula = formula.Replace("ADDRESS(" + formulaVal + ")", value);
            }
        }

        /// <summary>
        /// The export csv function.
        /// </summary>
        /// <param name="reportRequest">
        /// The report request.
        /// </param>
        /// <returns>
        /// The file to be exported as a byte array.
        /// </returns>
        /// <exception cref="Exception">
        /// </exception>
        public byte[] exportCSV(cReportRequest reportRequest)
        {
            this.request = reportRequest;

            // current user only used for export relationship fields for CEs & UDFs
            ICurrentUser currentUser = new CurrentUser(
                this.request.accountid, this.request.employeeid, -1, Modules.SpendManagement, this.request.SubAccountId);

            var output = new StringBuilder();

            var clsreports = new cReports(this.request.accountid);
            var file = new byte[0];
            var historyIDs = new List<int>();
            bool historyCreated = false;
            DataSet dataReport = null;
            var calcman = CreateCalcManager();
            var fields = new cFields(this.request.accountid);
            var joins = new cJoins(this.request.accountid);

            try
            {
                if (reportRequest.report.exportoptions.isfinancialexport && this.request.report.exportoptions.exporthistoryid == 0)
                {
                    foreach (cReportColumn rc in reportRequest.report.columns)
                    {
                        if (rc.GetType() == typeof(cStandardColumn)
                            && ((cStandardColumn)rc).field.FieldID == reportRequest.report.basetable.GetPrimaryKey().FieldID)
                        {
                            reportRequest.report.exportoptions.PrimaryKeyIndex = rc.order;
                            reportRequest.report.exportoptions.PrimaryKeyInReport = true;
                        }
                    }

                    if (reportRequest.report.exportoptions.PrimaryKeyIndex == -1)
                    {
                        int order = ((cReportColumn)reportRequest.report.columns[reportRequest.report.columns.Count - 1]).order + 1;
                        reportRequest.report.columns.Add(
                            new cStandardColumn(
                                new Guid(),
                                reportRequest.report.reportid,
                                ReportColumnType.Standard,
                                ColumnSort.None,
                                order,
                                reportRequest.report.basetable.GetPrimaryKey(),
                                false,
                                false,
                                false,
                                false,
                                false,
                                false,
                                true));

                        reportRequest.report.exportoptions.PrimaryKeyIndex = order;
                        reportRequest.report.exportoptions.PrimaryKeyInReport = false;
                    }
                }

                dataReport = clsreports.createReport(this.request, fields, joins, new UltraWebGrid(), calcman);

                if (dataReport.Tables.Count == 0)
                {
                    var ex = new Exception("SQL Timeout");
                    this.request.Status = ReportRequestStatus.Failed;
                    this.request.Exception = ex;
                    throw ex;
                }

                this.request.RowCount = dataReport.Tables[0].Rows.Count;

                if (dataReport.Tables[0].Rows.Count == 0)
                {
                    dataReport.Dispose();
                    return new byte[0];
                }

                if (reportRequest.report.exportoptions.isfinancialexport && reportRequest.report.exportoptions.exporthistoryid == 0)
                {
                    var negativeBalances = this.FilterNegativeBalances(dataReport.Tables[0], reportRequest.report);

                    // flag records
                    var exportHistoryEntries = this.CreateExportHistoryEntries(reportRequest, dataReport, historyIDs, negativeBalances);

                    if (exportHistoryEntries > 0)
                    {
                        historyCreated = true;
                    }
                    else
                    {
                        return new byte[0];
                    }

                    // remove primary key column for main export
                    if (reportRequest.report.exportoptions.PrimaryKeyInReport == false)
                    {
                        reportRequest.report.columns.RemoveAt(reportRequest.report.exportoptions.PrimaryKeyIndex - 1);
                    }

                    dataReport = clsreports.createReport(this.request, fields, joins, new UltraWebGrid(), calcman);
                }

                this.request.RowCount = dataReport.Tables[0].Rows.Count;

                var clstext = new cText();
                var clsexcel = new cExcel();
                var clsRow = new cRowFunction();
                var clsColumn = new cColumnFunction();
                var clsAddress = new cAddressFunction();
                var clsCarriage = new cCarriageReturn();
                var clsReplaceText = new cReplaceTextFunction();

                calcman.RegisterUserDefinedFunction(clstext);
                calcman.RegisterUserDefinedFunction(clsexcel);
                calcman.RegisterUserDefinedFunction(clsRow);
                calcman.RegisterUserDefinedFunction(clsColumn);
                calcman.RegisterUserDefinedFunction(clsAddress);
                calcman.RegisterUserDefinedFunction(clsCarriage);
                calcman.RegisterUserDefinedFunction(clsReplaceText);

                var lstColumnIndexes = new Dictionary<string, int>();
                var lstRegex = new Dictionary<Guid, MatchCollection>();
                int row = 1;
                int col = 1;

                if (string.IsNullOrEmpty(this.request.report.StaticReportSQL))
                {
                    cStandardColumn standard;
                    cCalculatedColumn calculatedcol;
                    if (this.request.report.exportoptions.showheaderscsv)
                    {
                        foreach (cReportColumn column in this.request.report.columns)
                        {
                            if (column.hidden)
                            {
                                continue;
                            }

                            switch (column.columntype)
                            {
                                case ReportColumnType.Standard:
                                    standard = (cStandardColumn)column;

                                    string fieldDescription = this._relabeler.Relabel(standard.field).Description;

                                    if (standard.funcavg || standard.funccount || standard.funcmax || standard.funcmin
                                        || standard.funcsum)
                                    {
                                        if (standard.funcsum)
                                        {
                                            output.Append("SUM of " + fieldDescription + ",");
                                        }

                                        if (standard.funcavg)
                                        {
                                            output.Append("AVG of " + fieldDescription + ",");
                                        }

                                        if (standard.funccount)
                                        {
                                            output.Append("COUNT of " + fieldDescription + ",");
                                        }

                                        if (standard.funcmax)
                                        {
                                            output.Append("MAX of " + fieldDescription + ",");
                                        }

                                        if (standard.funcmin)
                                        {
                                            output.Append("MIN of " + fieldDescription + ",");
                                        }

                                        if (output[output.Length - 1] == ',')
                                        {
                                            output.Remove(output.Length - 1, 1);
                                        }
                                    }
                                    else
                                    {
                                        output.Append(fieldDescription);
                                    }

                                    break;
                                case ReportColumnType.Static:
                                    var staticcol = (cStaticColumn)column;
                                    output.Append(staticcol.literalname);
                                    break;
                                case ReportColumnType.Calculated:
                                    calculatedcol = (cCalculatedColumn)column;
                                    output.Append(calculatedcol.columnname);
                                    break;
                            }

                            output.Append(reportRequest.report.exportoptions.Delimiter);
                            col++;
                        }

                        if (output.Length != 0)
                        {
                            // remove last ,
                            output.Remove(output.Length - 1, 1);
                        }

                        output.Append("\r\n");
                        row++;
                    }

                    DateTime date;
                    decimal num;
                    int dataSetCol;

                    var clsGlobalCurrencies = new cGlobalCurrencies();
                    var clsCurrencies = new cCurrencies(this.request.accountid, this.request.SubAccountId);

                    foreach (DataRow dataRow in dataReport.Tables[0].Rows)
                    {
                        col = 1;
                        dataSetCol = 1;
                        foreach (cReportColumn column in this.request.report.columns)
                        {
                            int x = this.request.report.columns.IndexOf(column);
                            if (!column.hidden)
                            {
                                switch (column.columntype)
                                {
                                    case ReportColumnType.Standard:
                                        standard = (cStandardColumn)column;
                                        if (standard.funcavg || standard.funccount || standard.funcmax
                                            || standard.funcmin || standard.funcsum)
                                        {
                                            if (standard.funcsum)
                                            {
                                                switch (standard.field.FieldType)
                                                {
                                                    case "M":
                                                    case "FD":
                                                    case "F":
                                                        if (dataRow[x] != DBNull.Value)
                                                        {
                                                            num = Convert.ToDecimal(dataRow[x]);
                                                            output.Append(num.ToString("########0.00"));
                                                        }

                                                        break;
                                                    case "C":
                                                        if (dataRow[x] != DBNull.Value)
                                                        {
                                                            num = Convert.ToDecimal(dataRow[x]);
                                                            output.Append(num.ToString("#######0.00"));
                                                        }

                                                        break;
                                                    case "N":
                                                        if (dataRow[x] != DBNull.Value)
                                                        {
                                                            output.Append(dataRow[x]);
                                                        }

                                                        break;
                                                }

                                                output.Append(reportRequest.report.exportoptions.Delimiter);
                                                col++;
                                            }

                                            if (standard.funcavg)
                                            {
                                                switch (standard.field.FieldType)
                                                {
                                                    case "M":
                                                    case "FD":
                                                    case "F":
                                                        if (dataRow[x] != DBNull.Value)
                                                        {
                                                            num = Convert.ToDecimal(dataRow[x]);
                                                            output.Append(num.ToString("########0.00"));
                                                        }

                                                        break;
                                                    case "C":
                                                        if (dataRow[x] != DBNull.Value)
                                                        {
                                                            num = Convert.ToDecimal(dataRow[x]);
                                                            output.Append(num.ToString("########0.00"));
                                                        }

                                                        break;
                                                    case "N":
                                                        if (dataRow[x] != DBNull.Value)
                                                        {
                                                            output.Append(dataRow[x]);
                                                        }

                                                        break;
                                                }

                                                output.Append(reportRequest.report.exportoptions.Delimiter);
                                                col++;
                                            }

                                            if (standard.funccount)
                                            {
                                                output.Append(dataRow[x]);
                                                output.Append(reportRequest.report.exportoptions.Delimiter);
                                                col++;
                                            }

                                            if (standard.funcmax)
                                            {
                                                switch (standard.field.FieldType)
                                                {
                                                    case "M":
                                                    case "FD":
                                                    case "F":
                                                        if (dataRow[x] != DBNull.Value)
                                                        {
                                                            num = Convert.ToDecimal(dataRow[x]);
                                                            output.Append(num.ToString("########0.00"));
                                                        }

                                                        break;
                                                    case "C":
                                                        if (dataRow[x] != DBNull.Value)
                                                        {
                                                            num = Convert.ToDecimal(dataRow[x]);
                                                            output.Append(num.ToString("########0.00"));
                                                        }

                                                        break;
                                                    case "N":
                                                        if (dataRow[x] != DBNull.Value)
                                                        {
                                                            output.Append(dataRow[x]);
                                                        }

                                                        break;
                                                    case "D":
                                                        if (dataRow[x] != DBNull.Value)
                                                        {
                                                            date = (DateTime)dataRow[x];
                                                            output.Append(date.ToShortDateString());
                                                        }

                                                        break;
                                                    case "DT":
                                                        if (dataRow[x] != DBNull.Value)
                                                        {
                                                            date = (DateTime)dataRow[x];
                                                            output.Append(date);
                                                        }

                                                        break;
                                                    case "T":
                                                        if (dataRow[x] != DBNull.Value)
                                                        {
                                                            date = (DateTime)dataRow[x];
                                                            output.Append(
                                                                date.Hour.ToString("00") + ":"
                                                                + date.Minute.ToString("00"));
                                                        }

                                                        break;
                                                }

                                                output.Append(reportRequest.report.exportoptions.Delimiter);
                                                col++;
                                            }

                                            if (standard.funcmin)
                                            {
                                                switch (standard.field.FieldType)
                                                {
                                                    case "M":
                                                    case "FD":
                                                    case "F":
                                                        if (dataRow[x] != DBNull.Value)
                                                        {
                                                            num = Convert.ToDecimal(dataRow[x]);
                                                            output.Append(num.ToString("########0.00"));
                                                        }

                                                        break;
                                                    case "C":
                                                        if (dataRow[x] != DBNull.Value)
                                                        {
                                                            num = Convert.ToDecimal(dataRow[x]);
                                                            output.Append(num.ToString("########0.00"));
                                                        }

                                                        break;
                                                    case "N":
                                                        if (dataRow[x] != DBNull.Value)
                                                        {
                                                            output.Append(dataRow[x]);
                                                        }

                                                        break;
                                                    case "D":
                                                        if (dataRow[x] != DBNull.Value)
                                                        {
                                                            date = (DateTime)dataRow[x];
                                                            output.Append(date.ToShortDateString());
                                                        }

                                                        break;
                                                    case "DT":
                                                        if (dataRow[x] != DBNull.Value)
                                                        {
                                                            date = (DateTime)dataRow[x];
                                                            output.Append(date);
                                                        }

                                                        break;
                                                    case "T":
                                                        if (dataRow[x] != DBNull.Value)
                                                        {
                                                            date = (DateTime)dataRow[x];
                                                            output.Append(
                                                                date.Hour.ToString("00") + ":"
                                                                + date.Minute.ToString("00"));
                                                        }

                                                        break;
                                                }

                                                output.Append(reportRequest.report.exportoptions.Delimiter);
                                                col++;
                                            }
                                        }
                                        else
                                        {
                                            switch (standard.field.FieldType)
                                            {
                                                case "S":
                                                case "FS":
                                                case "LT":
                                                case "R":
                                                    if (dataRow[x] != DBNull.Value)
                                                    {
                                                        if (reportRequest.report.exportoptions.EncloseInSpeechMarks)
                                                        {
                                                            output.Append("\"");
                                                        }

                                                        output.Append(
                                                            this.removeCarriageReturns(
                                                                reportRequest.report.exportoptions.RemoveCarriageReturns,
                                                                dataRow[x].ToString()));
                                                        if (reportRequest.report.exportoptions.EncloseInSpeechMarks)
                                                        {
                                                            output.Append("\"");
                                                        }
                                                    }

                                                    break;
                                                case "D":
                                                    if (dataRow[x] != DBNull.Value)
                                                    {
                                                        date = (DateTime)dataRow[x];
                                                        output.Append(date.ToShortDateString());
                                                    }

                                                    break;
                                                case "DT":
                                                    if (dataRow[x] != DBNull.Value)
                                                    {
                                                        date = (DateTime)dataRow[x];
                                                        output.Append(date);
                                                    }

                                                    break;
                                                case "T":
                                                    if (dataRow[x] != DBNull.Value)
                                                    {
                                                        date = (DateTime)dataRow[x];
                                                        output.Append(
                                                            date.Hour.ToString("00") + ":" + date.Minute.ToString("00"));
                                                    }

                                                    break;
                                                case "X":
                                                    if (dataRow[x] != DBNull.Value)
                                                    {
                                                        output.Append(dataRow[x]);
                                                    }

                                                    break;
                                                case "M":
                                                case "FD":
                                                case "F":
                                                    if (dataRow[x] != DBNull.Value)
                                                    {
                                                        num = Convert.ToDecimal(dataRow[x]);
                                                        output.Append(num.ToString("########0.00"));
                                                    }

                                                    break;
                                                case "C":
                                                    if (dataRow[x] != DBNull.Value)
                                                    {
                                                        num = Convert.ToDecimal(dataRow[x]);
                                                        output.Append(num.ToString("########0.00"));
                                                    }

                                                    break;
                                                case "N":
                                                case "I":
                                                    if (dataRow[x] != DBNull.Value)
                                                    {
                                                        // check that not relationship field. If so, retrieve text value
                                                        if (standard.field.FieldSource
                                                            != cField.FieldSourceType.Metabase
                                                            && (standard.field.IsForeignKey
                                                                && standard.field.RelatedTableID != Guid.Empty))
                                                        {
                                                            if (reportRequest.report.exportoptions.EncloseInSpeechMarks)
                                                            {
                                                                output.Append("\"");
                                                            }

                                                            output.Append(
                                                                this.removeCarriageReturns(
                                                                    reportRequest.report.exportoptions.RemoveCarriageReturns,
                                                                    Spend_Management.cReports.getRelationshipValueText(
                                                                        currentUser,
                                                                        standard.field.FieldID,
                                                                        (int)dataRow[x])));
                                                            if (reportRequest.report.exportoptions.EncloseInSpeechMarks)
                                                            {
                                                                output.Append("\"");
                                                            }
                                                        }
                                                        else
                                                        {
                                                            output.Append(dataRow[x]);
                                                        }
                                                    }

                                                    break;
                                                case "CL":
                                                    if (dataRow[x] != DBNull.Value)
                                                    {
                                                        cCurrency currency = clsCurrencies.getCurrencyById((int)dataRow[x]);

                                                        if (reportRequest.report.exportoptions.EncloseInSpeechMarks)
                                                        {
                                                            output.Append("\"");
                                                        }

                                                        output.Append(
                                                            this.removeCarriageReturns(
                                                                reportRequest.report.exportoptions.RemoveCarriageReturns,
                                                                currency == null
                                                                    ? string.Empty
                                                                    : clsGlobalCurrencies.getGlobalCurrencyById(
                                                                        currency.globalcurrencyid).label));
                                                        if (reportRequest.report.exportoptions.EncloseInSpeechMarks)
                                                        {
                                                            output.Append("\"");
                                                        }
                                                    }

                                                    break;
                                                default:
                                                    cEventlog.LogEntry(
                                                        string.Format(
                                                            "Reports : cExports : ExportCSV : Unexpected standard column type '{0}' encountered for report ID {1}",
                                                            standard.field.FieldType,
                                                            reportRequest.report.reportid));
                                                    break;
                                            }

                                            col++;
                                            output.Append(reportRequest.report.exportoptions.Delimiter);
                                        }

                                        break;
                                    case ReportColumnType.Static:

                                        output.Append(dataRow[x]);
                                        col++;
                                        output.Append(reportRequest.report.exportoptions.Delimiter);
                                        break;
                                    case ReportColumnType.Calculated:
                                        calculatedcol = (cCalculatedColumn)column;
                                        string formula = calculatedcol.formula;

                                        this.calculateFormula(
                                            ref formula,
                                            dataReport,
                                            row,
                                            dataSetCol,
                                            this.request.report.exportoptions.showheaderscsv);

                                        UltraCalcValue val;
                                        try
                                        {
                                            if (string.IsNullOrWhiteSpace(formula))
                                            {
                                                val = new UltraCalcValue(formula);
                                            }
                                            else
                                            {
                                                val =
                                                    calcman.Calculate(
                                                        this.convertExportFormula(
                                                            this.request.report.columns,
                                                            formula,
                                                            dataRow,
                                                            Aggregate.None,
                                                            ref lstColumnIndexes,
                                                            ref lstRegex,
                                                            calculatedcol.reportcolumnid));
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            throw new Exception(
                                                string.Format("Calculated column failed to export\n\nReport Column: {0}(no. {1})\n\nReport Row: {2}\n\nFormula: {3}\n\n\nInfragistics Message: \n\n{4}", calculatedcol.columnname, col, row, calculatedcol.formula, ex.Message));
                                        }

                                        this.parseCalculatedValueForIncrementalNumber(
                                            val.ToString(CultureInfo.InvariantCulture),
                                            dataReport,
                                            row,
                                            dataSetCol,
                                            this.request.report.exportoptions.showheaderscsv);

                                        output.Append(val);
                                        col++;
                                        if (formula.ToUpper() != "CARRIAGERETURN()")
                                        {
                                            output.Append(reportRequest.report.exportoptions.Delimiter);
                                        }

                                        break;
                                }
                            }

                            dataSetCol++;
                        }

                        if (output.Length != 0)
                        {
                            // remove last ,
                            output.Remove(output.Length - 1, 1);
                        }

                        output.Append("\r\n");
                        row++;
                        reportRequest.ProcessedRows++;
                    }

                    if (reportRequest.report.exportoptions.footerreport != null)
                    {
                        // set footer options from main report
                        reportRequest.report.exportoptions.footerreport.exportoptions = reportRequest.report.exportoptions;
                        dataReport =
                            clsreports.createReport(
                                new cReportRequest(
                                    reportRequest.accountid,
                                    reportRequest.SubAccountId,
                                    reportRequest.requestnum,
                                    reportRequest.report.exportoptions.footerreport,
                                    ExportType.Excel,
                                    reportRequest.report.exportoptions,
                                    reportRequest.claimantreport,
                                    reportRequest.employeeid,
                                    reportRequest.AccessLevel),
                                fields,
                                joins,
                                new UltraWebGrid(),
                                calcman);
                        foreach (DataRow dataRow in dataReport.Tables[0].Rows)
                        {
                            col = 1;
                            dataSetCol = 1;
                            foreach (cReportColumn column in this.request.report.exportoptions.footerreport.columns)
                            {
                                int x = reportRequest.report.exportoptions.footerreport.columns.IndexOf(column);
                                if (!column.hidden)
                                {
                                    switch (column.columntype)
                                    {
                                        case ReportColumnType.Standard:
                                            standard = (cStandardColumn)column;
                                            if (standard.funcavg || standard.funccount || standard.funcmax
                                                || standard.funcmin || standard.funcsum)
                                            {
                                                if (standard.funcsum)
                                                {
                                                    switch (standard.field.FieldType)
                                                    {
                                                        case "M":
                                                        case "FD":
                                                        case "F":
                                                            if (dataRow[x] != DBNull.Value)
                                                            {
                                                                num = (decimal)dataRow[x];
                                                                output.Append(num.ToString("########0.00"));
                                                            }

                                                            break;
                                                        case "C":
                                                            if (dataRow[x] != DBNull.Value)
                                                            {
                                                                num = (decimal)dataRow[x];
                                                                output.Append(num.ToString("#######0.00"));
                                                            }

                                                            break;
                                                        case "N":
                                                        case "I":
                                                            if (dataRow[x] != DBNull.Value)
                                                            {
                                                                output.Append(dataRow[x]);
                                                            }

                                                            break;
                                                    }

                                                    output.Append(reportRequest.report.exportoptions.Delimiter);
                                                    col++;
                                                }

                                                if (standard.funcavg)
                                                {
                                                    switch (standard.field.FieldType)
                                                    {
                                                        case "M":
                                                        case "FD":
                                                        case "F":
                                                            if (dataRow[x] != DBNull.Value)
                                                            {
                                                                num = (decimal)dataRow[x];
                                                                output.Append(num.ToString("########0.00"));
                                                            }

                                                            break;
                                                        case "C":
                                                            if (dataRow[x] != DBNull.Value)
                                                            {
                                                                num = (decimal)dataRow[x];
                                                                output.Append(num.ToString("########0.00"));
                                                            }

                                                            break;
                                                        case "N":
                                                        case "I":
                                                            if (dataRow[x] != DBNull.Value)
                                                            {
                                                                output.Append(dataRow[x]);
                                                            }

                                                            break;
                                                    }

                                                    output.Append(reportRequest.report.exportoptions.Delimiter);
                                                    col++;
                                                }

                                                if (standard.funccount)
                                                {
                                                    output.Append(dataRow[x]);
                                                    output.Append(reportRequest.report.exportoptions.Delimiter);
                                                    col++;
                                                }

                                                if (standard.funcmax)
                                                {
                                                    switch (standard.field.FieldType)
                                                    {
                                                        case "M":
                                                        case "FD":
                                                        case "F":
                                                            if (dataRow[x] != DBNull.Value)
                                                            {
                                                                num = (decimal)dataRow[x];
                                                                output.Append(num.ToString("########0.00"));
                                                            }

                                                            break;
                                                        case "C":
                                                            if (dataRow[x] != DBNull.Value)
                                                            {
                                                                num = (decimal)dataRow[x];
                                                                output.Append(num.ToString("########0.00"));
                                                            }

                                                            break;
                                                        case "N":
                                                        case "I":
                                                            if (dataRow[x] != DBNull.Value)
                                                            {
                                                                output.Append(dataRow[x]);
                                                            }

                                                            break;
                                                        case "D":
                                                            if (dataRow[x] != DBNull.Value)
                                                            {
                                                                output.Append(dataRow[x]);
                                                            }

                                                            break;
                                                        case "DT":
                                                            if (dataRow[x] != DBNull.Value)
                                                            {
                                                                date = (DateTime)dataRow[x];
                                                                output.Append(date);
                                                            }

                                                            break;
                                                        case "T":
                                                            if (dataRow[x] != DBNull.Value)
                                                            {
                                                                date = (DateTime)dataRow[x];
                                                                output.Append(
                                                                    date.Hour.ToString("00") + ":"
                                                                    + date.Minute.ToString("00"));
                                                            }

                                                            break;
                                                    }

                                                    output.Append(reportRequest.report.exportoptions.Delimiter);
                                                    col++;
                                                }

                                                if (standard.funcmin)
                                                {
                                                    switch (standard.field.FieldType)
                                                    {
                                                        case "M":
                                                        case "FD":
                                                        case "F":
                                                            if (dataRow[x] != DBNull.Value)
                                                            {
                                                                num = (decimal)dataRow[x];
                                                                output.Append(num.ToString("########0.00"));
                                                            }

                                                            break;
                                                        case "C":
                                                            if (dataRow[x] != DBNull.Value)
                                                            {
                                                                num = (decimal)dataRow[x];
                                                                output.Append(num.ToString("########0.00"));
                                                            }

                                                            break;
                                                        case "N":
                                                        case "I":
                                                            if (dataRow[x] != DBNull.Value)
                                                            {
                                                                output.Append(dataRow[x]);
                                                            }

                                                            break;
                                                        case "D":
                                                            if (dataRow[x] != DBNull.Value)
                                                            {
                                                                date = (DateTime)dataRow[x];
                                                                output.Append(date.ToShortDateString());
                                                            }

                                                            break;
                                                        case "DT":
                                                            if (dataRow[x] != DBNull.Value)
                                                            {
                                                                date = (DateTime)dataRow[x];
                                                                output.Append(date);
                                                            }

                                                            break;
                                                        case "T":
                                                            if (dataRow[x] != DBNull.Value)
                                                            {
                                                                date = (DateTime)dataRow[x];
                                                                output.Append(
                                                                    date.Hour.ToString("00") + ":"
                                                                    + date.Minute.ToString("00"));
                                                            }

                                                            break;
                                                    }

                                                    output.Append(reportRequest.report.exportoptions.Delimiter);
                                                    col++;
                                                }
                                            }
                                            else
                                            {
                                                switch (standard.field.FieldType)
                                                {
                                                    case "S":
                                                    case "FS":
                                                    case "LT":
                                                    case "R":
                                                        if (dataRow[x] != DBNull.Value)
                                                        {
                                                            if (reportRequest.report.exportoptions.EncloseInSpeechMarks)
                                                            {
                                                                output.Append("\"");
                                                            }

                                                            output.Append((string)dataRow[x]);
                                                            if (reportRequest.report.exportoptions.EncloseInSpeechMarks)
                                                            {
                                                                output.Append("\"");
                                                            }
                                                        }

                                                        break;
                                                    case "D":
                                                        if (dataRow[x] != DBNull.Value)
                                                        {
                                                            date = (DateTime)dataRow[x];
                                                            output.Append(date.ToShortDateString());
                                                        }

                                                        break;
                                                    case "DT":
                                                        if (dataRow[x] != DBNull.Value)
                                                        {
                                                            date = (DateTime)dataRow[x];
                                                            output.Append(date);
                                                        }

                                                        break;
                                                    case "T":
                                                        if (dataRow[x] != DBNull.Value)
                                                        {
                                                            date = (DateTime)dataRow[x];
                                                            output.Append(
                                                                date.Hour.ToString("00") + ":"
                                                                + date.Minute.ToString("00"));
                                                        }

                                                        break;
                                                    case "X":
                                                        if (dataRow[x] != DBNull.Value)
                                                        {
                                                            output.Append(dataRow[x]);
                                                        }

                                                        break;
                                                    case "M":
                                                    case "FD":
                                                    case "F":
                                                        if (dataRow[col - 1] != DBNull.Value)
                                                        {
                                                            num = (decimal)dataRow[x];
                                                            output.Append(num.ToString("########0.00"));
                                                        }

                                                        break;
                                                    case "C":
                                                        if (dataRow[col - 1] != DBNull.Value)
                                                        {
                                                            num = (decimal)dataRow[x];
                                                            output.Append(num.ToString("########0.00"));
                                                        }

                                                        break;
                                                    case "N":
                                                    case "I":
                                                        if (dataRow[x] != DBNull.Value)
                                                        {
                                                            // check that not relationship field. If so, retrieve text value
                                                            if (standard.field.FieldSource
                                                                != cField.FieldSourceType.Metabase
                                                                && (standard.field.IsForeignKey
                                                                    && standard.field.RelatedTableID != Guid.Empty))
                                                            {
                                                                if (reportRequest.report.exportoptions.EncloseInSpeechMarks)
                                                                {
                                                                    output.Append("\"");
                                                                }

                                                                output.Append(
                                                                    Spend_Management.cReports.getRelationshipValueText(
                                                                        currentUser,
                                                                        standard.field.FieldID,
                                                                        (int)dataRow[x]));
                                                                if (reportRequest.report.exportoptions.EncloseInSpeechMarks)
                                                                {
                                                                    output.Append("\"");
                                                                }
                                                            }
                                                            else
                                                            {
                                                                output.Append(dataRow[x]);

                                                                // worksheet.Range[row, col].NumberFormat = "#,##0.00;-#,##0.00";
                                                            }
                                                        }

                                                        break;
                                                    case "CL":
                                                        if (dataRow[x] != DBNull.Value)
                                                        {
                                                            cCurrency currency =
                                                                clsCurrencies.getCurrencyById((int)dataRow[x]);

                                                            output.Append(
                                                                this.removeCarriageReturns(
                                                                    reportRequest.report.exportoptions.RemoveCarriageReturns,
                                                                    currency == null
                                                                        ? string.Empty
                                                                        : clsGlobalCurrencies.getGlobalCurrencyById(
                                                                            currency.globalcurrencyid).label));
                                                        }

                                                        break;
                                                    default:
                                                        cEventlog.LogEntry(
                                                            string.Format(
                                                                "Reports : cExports : ExportExcel : Unexpected standard column type '{0}' encountered for footer report ID {1}. Main report ID is {2}",
                                                                standard.field.FieldType,
                                                                reportRequest.report.exportoptions.footerreport.reportid,
                                                                reportRequest.report.reportid));
                                                        break;
                                                }

                                                col++;
                                                output.Append(reportRequest.report.exportoptions.Delimiter);
                                            }

                                            break;
                                        case ReportColumnType.Static:
                                            output.Append(dataRow[x]);
                                            col++;
                                            output.Append(reportRequest.report.exportoptions.Delimiter);
                                            break;
                                        case ReportColumnType.Calculated:
                                            calculatedcol = (cCalculatedColumn)column;
                                            string calculatedFormula = calculatedcol.formula;

                                            this.calculateFormula(
                                                ref calculatedFormula, dataReport, row, dataSetCol, false);

                                            UltraCalcValue val =
                                                calcman.Calculate(
                                                    this.convertExportFormula(
                                                        this.request.report.exportoptions.footerreport.columns,
                                                        calculatedFormula,
                                                        dataRow,
                                                        Aggregate.None,
                                                        ref lstColumnIndexes,
                                                        ref lstRegex,
                                                        calculatedcol.reportcolumnid));

                                            output.Append(val);

                                            col++;
                                            output.Append(reportRequest.report.exportoptions.Delimiter);
                                            break;
                                    }
                                }

                                dataSetCol++;
                            }

                            if (output.Length != 0)
                            {
                                // remove last ,
                                output.Remove(
                                    output.Length - reportRequest.report.exportoptions.Delimiter.Length,
                                    reportRequest.report.exportoptions.Delimiter.Length);
                            }

                            output.Append("\r\n");
                            row++;
                        }
                    }
                }
                else
                {
                    // Static SQL Report export
                    row = 1;
                    col = 1;
                    foreach (DataColumn dataColumn in dataReport.Tables[0].Columns)
                    {
                        output.Append(dataColumn.ColumnName);
                        output.Append(reportRequest.report.exportoptions.Delimiter);
                        col++;
                    }

                    row++;
                    if (output.Length != 0)
                    {
                        // remove last ,
                        output.Remove(output.Length - 1, 1);
                    }

                    output.Append("\r\n");

                    foreach (DataRow dataSetRow in dataReport.Tables[0].Rows)
                    {
                        col = 1;

                        foreach (object columnObject in dataSetRow.ItemArray)
                        {
                            if (columnObject != DBNull.Value)
                            {
                                switch (columnObject.GetType().ToString())
                                {
                                    case "System.String":
                                        output.Append(columnObject);
                                        break;
                                    case "System.DateTime":
                                        output.Append(((DateTime)columnObject).ToShortDateString());
                                        break;
                                    case "System.Int16":
                                    case "System.Int32":
                                    case "System.Double":
                                        output.Append(columnObject);
                                        break;
                                    case "System.Boolean":
                                        if ((bool)columnObject)
                                        {
                                            output.Append("True");
                                        }
                                        else
                                        {
                                            output.Append("False");
                                        }

                                        break;
                                    default:
                                        output.Append(columnObject);
                                        break;
                                }
                            }

                            output.Append(reportRequest.report.exportoptions.Delimiter);
                            col++;
                        }

                        if (output.Length != 0)
                        {
                            // remove last ,
                            output.Remove(output.Length - 1, 1);
                        }

                        output.Append("\r\n");
                        row++;
                        reportRequest.ProcessedRows++;
                    }
                }

                Encoding unicode = Encoding.UTF8;

                file = unicode.GetBytes(output.ToString());

                // set the status of the export to exported
                if (historyCreated)
                {
                    this.UpdateHistoryExported(reportRequest);
                }
            }
            catch (Exception ex)
            {
                reportRequest.Exception = ex;
                if (reportRequest.report.exportoptions.exporthistoryid != 0 && historyCreated)
                {
                    this.DeleteHistory(reportRequest);
                }

                string logMessage = "An error occurred exporting to CSV.";
                if (ex.Message.IndexOf("Timeout", StringComparison.InvariantCultureIgnoreCase) >= 0)
                {
                    logMessage = "Timeout on dataset retrieval.";
                }

                cEventlog.LogEntry(
                    string.Format(
                        "Reports : cExports : exportFlatfile : {2} : {0}\n{1}", ex.Message, ex.StackTrace, logMessage));
                throw new Exception(logMessage, ex);
            }
            finally
            {
                if (calcman != null)
                {
                    calcman.Dispose();
                }

                if (dataReport != null)
                {
                    if (dataReport.Tables != null && dataReport.Tables[0] != null)
                    {
                        dataReport.Tables[0].Clear();
                        dataReport.Tables[0].Dispose();
                    }

                    dataReport.Clear();
                    dataReport.Dispose();
                }
            }

            return file;
        }

        /// <summary>
        /// Create a new instance of <see cref="UltraWebCalcManager"/> with the custom functions added.
        /// </summary>
        /// <returns>An instance of <see cref="UltraWebCalcManager"/></returns>
        private static UltraWebCalcManager CreateCalcManager()
        {
            var calcman = new UltraWebCalcManager();
            var clstext = new cText();
            var clsexcel = new cExcel();
            var clsRow = new cRowFunction();
            var clsColumn = new cColumnFunction();
            var column = new ColFunction();
            var clsAddress = new cAddressFunction();
            var clsCarriage = new cCarriageReturn();
            var clsReplaceText = new cReplaceTextFunction();

            calcman.RegisterUserDefinedFunction(clstext);
            calcman.RegisterUserDefinedFunction(clsexcel);
            calcman.RegisterUserDefinedFunction(clsRow);
            calcman.RegisterUserDefinedFunction(clsColumn);
            calcman.RegisterUserDefinedFunction(column);
            calcman.RegisterUserDefinedFunction(clsAddress);
            calcman.RegisterUserDefinedFunction(clsCarriage);
            calcman.RegisterUserDefinedFunction(clsReplaceText);
            return calcman;
        }

        /// <summary>
        /// The export to esr report function.
        /// </summary>
        /// <param name="reportRequest">
        /// The report request.
        /// </param>
        /// <returns>
        /// The export as a byte array.
        /// </returns>
        /// <exception cref="Exception">
        /// </exception>
        public byte[] exportESR(cReportRequest reportRequest)
        {
            var clientProvider = new BinaryClientFormatterSinkProvider();
            var serverProvider = new BinaryServerFormatterSinkProvider { TypeFilterLevel = TypeFilterLevel.Full };

            IDictionary props = new Hashtable();
            props["port"] = 0;
            props["typeFilterLevel"] = TypeFilterLevel.Full;
            var chan = new HttpChannel(props, clientProvider, serverProvider);
            int chanCount = ChannelServices.RegisteredChannels.Count(c => c.ChannelName == chan.ChannelName);
            if (chanCount == 0)
            {
                ChannelServices.RegisterChannel(chan, false);
            }

            DataSet dataSet = null;
            var clsESR =
                (IESR)Activator.GetObject(typeof(IESR), ConfigurationManager.AppSettings["ESRService"] + "/ESR.rem");
            this.request = reportRequest;
            var clsreports = new cReports(this.request.accountid);
            cReport rpt = this.request.report;
            var file = new byte[0];
            var historyIDs = new List<int>();
            bool historyCreated = false;
            var lstColumnIndexes = new Dictionary<string, int>();
            var lstColumns = new Dictionary<Guid, cReportColumn>();
            var calcman = CreateCalcManager();
            var fields = new cFields(this.request.accountid);
            var joins = new cJoins(this.request.accountid);

            try
            {
                if (reportRequest.report.exportoptions.isfinancialexport && this.request.report.exportoptions.exporthistoryid == 0)
                {
                    foreach (cReportColumn rc in reportRequest.report.columns)
                    {
                        if (rc.GetType() == typeof(cStandardColumn)
                            && ((cStandardColumn)rc).field.FieldID == reportRequest.report.basetable.GetPrimaryKey().FieldID)
                        {
                            reportRequest.report.exportoptions.PrimaryKeyIndex = rc.order;
                            reportRequest.report.exportoptions.PrimaryKeyInReport = true;
                        }
                    }

                    if (reportRequest.report.exportoptions.PrimaryKeyIndex == -1)
                    {
                        int order = ((cReportColumn)reportRequest.report.columns[reportRequest.report.columns.Count - 1]).order + 1;
                        reportRequest.report.columns.Insert(0,
                            new cStandardColumn(
                                new Guid(),
                                reportRequest.report.reportid,
                                ReportColumnType.Standard,
                                ColumnSort.None,
                                order,
                                reportRequest.report.basetable.GetPrimaryKey(),
                                false,
                                false,
                                false,
                                false,
                                false,
                                false,
                                true));

                        reportRequest.report.exportoptions.PrimaryKeyIndex = order;
                        reportRequest.report.exportoptions.PrimaryKeyInReport = false;
                    }
                }

                dataSet = clsreports.createReport(this.request, fields, joins, new UltraWebGrid(), calcman);

                if (dataSet.Tables.Count == 0)
                {
                    var ex = new Exception("SQL Timeout");
                    this.request.Status = ReportRequestStatus.Failed;
                    this.request.Exception = ex;
                    throw ex;
                }

                DataTable dataToExport = dataSet.Tables[0];

                if (dataToExport.Rows.Count == 0)
                {
                    dataSet.Dispose();
                    return new byte[0];
                }

                var subAccounts = new cAccountSubAccounts(reportRequest.accountid);

                cAccountProperties reqProperties =  reportRequest.SubAccountId == 0 ? subAccounts.getFirstSubAccount().SubAccountProperties : subAccounts.getSubAccountById(reportRequest.SubAccountId).SubAccountProperties;

                if (reportRequest.report.exportoptions.isfinancialexport && reportRequest.report.exportoptions.exporthistoryid == 0)
                {
                    // flag records
                    foreach (DataRow dr in dataToExport.Rows)
                    {
                        if (historyIDs.Contains((int)dr[reportRequest.report.exportoptions.PrimaryKeyIndex.ToString()]) == false)
                        {
                            historyIDs.Add((int)dr[reportRequest.report.exportoptions.PrimaryKeyIndex.ToString()]);
                        }
                    }

                    int exporthistoryid = this.CreateHistory(historyIDs, reportRequest);
                    reportRequest.report.exportoptions.exporthistoryid = exporthistoryid;
                    if (reportRequest.report.exportoptions.footerreport != null)
                    {
                        reportRequest.report.exportoptions.footerreport.exportoptions.exporthistoryid = exporthistoryid;
                    }

                    if (exporthistoryid > 0)
                    {
                        historyCreated = true;
                    }

                    // remove primary key column for main export
                    if (reportRequest.report.exportoptions.PrimaryKeyInReport == false)
                    {
                        reportRequest.report.columns.RemoveAt(0);
                    }

                    if (reqProperties.SummaryEsrInboundFile)
                    {
                        foreach (cReportColumn column in reportRequest.report.columns)
                        {
                            if (column.columntype == ReportColumnType.Standard)
                            {
                                var standardColumn = (cStandardColumn)column;
                                if (!standardColumn.field.CanTotal)
                                {
                                    if (!standardColumn.field.FieldType.StartsWith("F"))
                                    {
                                        standardColumn.sort = ColumnSort.Ascending;
                                    }
                                }
                            }
                        }
                    }

                    dataSet = clsreports.createReport(this.request, fields, joins, new UltraWebGrid(), calcman);
                    dataToExport = dataSet.Tables[0];
                }

                reportRequest.RowCount = dataToExport.Rows.Count;

                // field order
                // expenseid 0
                // subcatid 1
                int recordcount = 0;



                string now = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString("00")
                             + DateTime.Now.Day.ToString("00") + DateTime.Now.Hour.ToString("00")
                             + DateTime.Now.Minute.ToString("00") + DateTime.Now.Second.ToString("00");
                var lstRegex = new Dictionary<Guid, MatchCollection>();

                DateTime taxdate = DateTime.Today;
                int taxyear;
                int taxperiod;

                if (taxdate < new DateTime(DateTime.Today.Year, 04, 06))
                {
                    taxperiod = DateTime.Today.Month + 8;
                    taxyear = int.Parse(DateTime.Today.ToString("yy"));
                }
                else
                {
                    taxperiod = DateTime.Today.Month - 3;
                    taxyear = int.Parse(DateTime.Today.AddYears(1).Year.ToString().Substring(2, 2));
                }

                cESRTrust trustRecord = null;
                var lstSubcatElements = new Dictionary<int, List<cESRElement>>();
                try
                {
                    trustRecord = clsESR.GetESRTrustByID(
                        reportRequest.accountid, reportRequest.report.exportoptions.financialexport.NHSTrustID);
                }
                catch (Exception ex)
                {
                    cEventlog.LogEntry(ex.Message + " " + ex.StackTrace);
                    throw new Exception("Error retrieving ESR Trust", ex);
                }

                var esrInboundFormatter = new EsrInboundFormatting(reqProperties);
                var output = new EsrInboundBuilder(reqProperties.SummaryEsrInboundFile, esrInboundFormatter, reqProperties.EsrRounding);

                Encoding unicode = Encoding.ASCII;

                // If the trust is archived then dont process
                if (!trustRecord.Archived)
                {
                    SortedList<int, cGlobalESRElement> lstGlobalElements = clsESR.getListOfGlobalElements(
                        reportRequest.accountid, reportRequest.report.exportoptions.financialexport.NHSTrustID);

                    cGlobalESRElementField globalElementField = null;
                    string filename = reportRequest.report.exportoptions.ExportFileName;

                    output.Append("HDR,");
                    output.Append(filename + ",");
                    output.Append(now + ",");
                    output.Append("SEL,");
                    output.Append(trustRecord.TrustVPD + ",");
                    output.Append(trustRecord.PeriodType + ",");
                    output.Append(trustRecord.PeriodRun + ",");
                    output.Append(taxperiod.ToString("00"));
                    output.Append("\r");

                    #region Body

                    // check for custom entity based export
                    if (reportRequest.report.basetable.TableName.Substring(0, 7) == "custom_"
                        && reportRequest.report.basetable.GetPrimaryKey().FieldName.Substring(0, 3) == "att")
                    {
                        #region Custom Entity pre-formatted report

                        try
                        {
                            byte[] tmpBytes = this.exportCSV(reportRequest);
                            if (tmpBytes.Length > 0)
                            {
                                using (var stream = new MemoryStream(tmpBytes))
                                {
                                    using (var streamreader = new StreamReader(stream))
                                    {
                                        output.Append(streamreader.ReadToEnd().Replace(((char)10).ToString(), string.Empty));
                                        recordcount = reportRequest.ProcessedRows;
                                        streamreader.Close();
                                    }

                                    stream.Close();
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            throw new Exception("Error retrieving Custom Entity preformatted report", ex);
                        }

                        #endregion Custom Entity pre-formatted report
                    }
                    else
                    {
                        #region ESR mapped report
                        var subcatColumnIndex = dataToExport.Columns.Count - 4;
                        var assignmentNumberColumnIndex = dataToExport.Columns.Count - 3;
                        var costcodeColumnIndex = dataToExport.Columns.Count - 2;
                        var derivedCostcodeColumnIndex = dataToExport.Columns.Count - 1;

                        foreach (DataRow row in dataToExport.Rows)
                        {
                            List<cESRElement> lstElements;
                            int subcatValue;
                            int.TryParse(row[subcatColumnIndex].ToString(), out subcatValue);
                            if (lstSubcatElements.TryGetValue(subcatValue, out lstElements) == false)
                            {
                                lstElements = clsESR.getESRElementsBySubcatID(
                                    reportRequest.accountid,
                                    reportRequest.report.exportoptions.financialexport.NHSTrustID,
                                    subcatValue);
                                lstSubcatElements.Add(subcatValue, lstElements);
                            }

                            if (lstElements != null)
                            {
                                foreach (cESRElement element in lstElements)
                                {
                                    bool outputRow = true;
                                    int valCount = 0;
                                    var ESRRowOutput = new StringBuilder();
                                    var assignmentNumber = (string)row[assignmentNumberColumnIndex];

                                    cGlobalESRElement globalElement = null;
                                    lstGlobalElements.TryGetValue(element.GlobalElementID, out globalElement);

                                    if (globalElement != null)
                                    {
                                        ESRRowOutput.Append("ATT,");
                                        ESRRowOutput.Append(
                                            DateTime.Today.Year.ToString("00") + DateTime.Today.Month.ToString("00")
                                            + DateTime.Today.Day.ToString("00") + ",");
                                        ESRRowOutput.Append("ADD,");
                                        ESRRowOutput.Append(",");
                                        ESRRowOutput.Append(assignmentNumber + ",");
                                        ESRRowOutput.Append(",");
                                        ESRRowOutput.Append("\"" + globalElement.GlobalElementName + "\",");
                                        
                                        foreach (cESRElementField field in element.Fields)
                                        {
                                            globalElementField = null;
                                            foreach (cGlobalESRElementField globField in globalElement.Fields)
                                            {
                                                if (globField.globalElementFieldID == field.GlobalElementFieldID)
                                                {
                                                    globalElementField = globField;
                                                    break;
                                                }
                                            }

                                            if (globalElementField != null)
                                            {
                                                cReportColumn column;
                                                if (lstColumns.TryGetValue(field.ReportColumnID, out column) == false)
                                                {
                                                    column =
                                                        this.request.report.getReportColumnById(field.ReportColumnID);
                                                    lstColumns.Add(field.ReportColumnID, column);
                                                }

                                                ESRRowOutput.Append(
                                                    "\"" + globalElementField.globalElementFieldName + "\",");
                                                decimal roundedValue;
                                                int index;
                                                switch (column.columntype)
                                                {
                                                    case ReportColumnType.Standard:
                                                        var standardcol = (cStandardColumn)column;


                                                        if (
                                                            lstColumnIndexes.TryGetValue(
                                                                column.reportcolumnid + "," + field.Aggregate.ToString(),
                                                                out index) == false)
                                                        {
                                                            index = this.getColumnIndex(
                                                                standardcol,
                                                                this.request.report.columns,
                                                                field.Aggregate);
                                                            lstColumnIndexes.Add(
                                                                column.reportcolumnid + "," + field.Aggregate.ToString(),
                                                                index);
                                                        }

                                                        decimal num;
                                                        switch (standardcol.field.FieldType)
                                                        {
                                                            case "S":
                                                            case "FS":
                                                            case "LT":
                                                            case "R":
                                                                if (row[index] != DBNull.Value)
                                                                {
                                                                    ESRRowOutput.Append((string)row[index]);
                                                                    valCount++;
                                                                }

                                                                break;
                                                            case "D":
                                                                if (row[index] != DBNull.Value)
                                                                {
                                                                    var date = (DateTime)row[index];
                                                                    ESRRowOutput.Append(date.ToString("dd-MMM-yyyy"));
                                                                    valCount++;
                                                                }

                                                                break;
                                                            case "X":
                                                                if (row[index] != DBNull.Value)
                                                                {
                                                                    ESRRowOutput.Append(row[index]);
                                                                    valCount++;
                                                                }

                                                                break;
                                                            case "M":
                                                            case "FD":
                                                            case "F":
                                                            case "C":
                                                                roundedValue = esrInboundFormatter.FormatNumeric(
                                                                    row[index],
                                                                    ESRRowOutput,
                                                                    globalElementField);
                                                                if (globalElementField.controlColumn && roundedValue == 0)
                                                                {
                                                                    outputRow = false;
                                                                }
                                                                else
                                                                {
                                                                    valCount++;
                                                                }

                                                                break;
                                                            case "N":
                                                            case "I":
                                                                if (row[index] != DBNull.Value)
                                                                {
                                                                    ESRRowOutput.Append(row[index]);
                                                                    valCount++;
                                                                }

                                                                break;
                                                            default:
                                                                cEventlog.LogEntry(
                                                                    string.Format(
                                                                        "Reports : cExports : ExportESR : Unexpected standard column type '{0}' encountered for report ID {1}",
                                                                        standardcol.field.FieldType,
                                                                        reportRequest.report.reportid.ToString()));
                                                                break;
                                                        }

                                                        ESRRowOutput.Append(",");
                                                        break;

                                                    case ReportColumnType.Static:
                                                        var staticcol = (cStaticColumn)column;
                                                        ESRRowOutput.Append(staticcol.literalvalue + ",");
                                                        if (staticcol.literalvalue != string.Empty)
                                                        {
                                                            valCount++;
                                                        }

                                                        break;

                                                    case ReportColumnType.Calculated:

                                                        // index = getColumnIndex(field.label, request.report.columns, Aggregate.None);
                                                        var calculatedcol = (cCalculatedColumn)column;
                                                        UltraCalcValue val =
                                                            calcman.Calculate(
                                                                this.convertExportFormula(
                                                                    this.request.report.columns,
                                                                    calculatedcol.formula,
                                                                    row,
                                                                    field.Aggregate,
                                                                    ref lstColumnIndexes,
                                                                    ref lstRegex,
                                                                    calculatedcol.reportcolumnid));
                                                        roundedValue = esrInboundFormatter.FormatNumeric(val.Value, ESRRowOutput, globalElementField);

                                                        if (globalElementField.controlColumn && roundedValue == 0)
                                                        {
                                                            outputRow = false;
                                                        }

                                                        if (roundedValue == -1)
                                                        {
                                                            ESRRowOutput.Append(val.Value);
                                                        }

                                                        if (val.ToString() != string.Empty)
                                                        {
                                                            valCount++;
                                                        }

                                                        ESRRowOutput.Append(",");
                                                        break;
                                                }
                                            }
                                        }

                                        if (outputRow && valCount > 0)
                                        {
                                            ESRRowOutput.Append(string.Empty.PadRight(32 - (valCount * 2), ',')); // Add blank fields to make sure that the costcode is added to column 39
                                            recordcount++;
                                            string costcodeName = row[derivedCostcodeColumnIndex] == DBNull.Value
                                                                   ? string.Empty
                                                                   : (string)row[derivedCostcodeColumnIndex];

                                            ESRRowOutput.Append(costcodeName + "\r");
                                            output.Append(ESRRowOutput, globalElement);

                                        }
                                    }
                                }
                            }

                            reportRequest.ProcessedRows++;
                        }

                        #endregion ESR mapped report
                    }

                    #endregion

                    #region Footer

                    output.Append("FTR,");
                    output.Append(recordcount);
                    output.Append(",0");

                    #endregion

                    file = unicode.GetBytes(output.ToString());
                }
                else
                {
                    output.Append(string.Format(
                        "This trust {0} with VPD {1} is archived. Cannot process data for inbound export.", trustRecord.TrustName, trustRecord.TrustVPD));
                    file = unicode.GetBytes(output.ToString());
                }

                if (historyCreated)
                {
                    this.UpdateHistoryExported(reportRequest);
                }
            }
            catch (Exception ex)
            {
                reportRequest.Exception = ex;
                if (reportRequest.report.exportoptions.exporthistoryid != 0 && historyCreated)
                {
                    this.DeleteHistory(reportRequest);
                }

                string logMessage = "An error occurred exporting to ESR.";
                if (ex.Message == "SQL Timeout")
                {
                    logMessage = "Timeout on dataset retrieval.";
                }

                if (ex.Message == "Error retrieving Custom Entity preformatted report.")
                {
                    logMessage = "Error retrieving Custom Entity preformatted report.";
                }

                cEventlog.LogEntry(
                    string.Format(
                        "Reports : cExports : exportESR : {2} : {0}\n{1}", ex.Message, ex.StackTrace, logMessage));
                throw new Exception(logMessage, ex);
            }
            finally
            {
                if (calcman != null)
                {
                    calcman.Dispose();
                }

                if (dataSet != null)
                {
                    if (dataSet.Tables != null && dataSet.Tables[0] != null)
                    {
                        dataSet.Tables[0].Clear();
                        dataSet.Tables[0].Dispose();
                    }

                    dataSet.Clear();
                    dataSet.Dispose();
                }
            }

            return file;
        }

        /// <summary>
        /// The export to excel function.
        /// </summary>
        /// <param name="reportRequest">
        /// The report request.
        /// </param>
        /// <returns>
        /// The excel report as a byte array.
        /// </returns>
        /// <exception cref="Exception">
        /// </exception>
        public byte[] exportExcel(cReportRequest reportRequest)
        {
            this.request = reportRequest;

            // current user only used for export relationship fields for CEs & UDFs
            ICurrentUser currentUser = new CurrentUser(
                this.request.accountid, this.request.employeeid, -1, Modules.SpendManagement, this.request.SubAccountId);
            var clsreports = new cReports(this.request.accountid);
            var file = new byte[0];
            var historyIDs = new List<int>();
            var lstRegex = new Dictionary<Guid, MatchCollection>();
            bool historyCreated = false;
            var clsGlobalCurrencies = new cGlobalCurrencies();
            var clsCurrencies = new cCurrencies(this.request.accountid, this.request.SubAccountId);
            DataSet dataReport = null;
            var calcman = CreateCalcManager();
            var exceleng = new ExcelEngine { ThrowNotSavedOnDestroy = false };
            var fields = new cFields(this.request.accountid);
            var joins = new cJoins(this.request.accountid);

            try
            {
                if (reportRequest.report.exportoptions.isfinancialexport && this.request.report.exportoptions.exporthistoryid == 0)
                {
                    foreach (cReportColumn rc in reportRequest.report.columns)
                    {
                        if (rc.GetType() == typeof(cStandardColumn)
                            && ((cStandardColumn)rc).field.FieldID == reportRequest.report.basetable.GetPrimaryKey().FieldID)
                        {
                            reportRequest.report.exportoptions.PrimaryKeyIndex = rc.order;
                            reportRequest.report.exportoptions.PrimaryKeyInReport = true;
                        }
                    }

                    if (reportRequest.report.exportoptions.PrimaryKeyIndex == -1)
                    {
                        int order = ((cReportColumn)reportRequest.report.columns[reportRequest.report.columns.Count - 1]).order + 1;
                        reportRequest.report.columns.Add(
                            new cStandardColumn(
                                new Guid(),
                                reportRequest.report.reportid,
                                ReportColumnType.Standard,
                                ColumnSort.None,
                                order,
                                reportRequest.report.basetable.GetPrimaryKey(),
                                false,
                                false,
                                false,
                                false,
                                false,
                                false,
                                true));

                        reportRequest.report.exportoptions.PrimaryKeyIndex = order;
                        reportRequest.report.exportoptions.PrimaryKeyInReport = false;
                    }
                }

                dataReport = clsreports.createReport(this.request, fields, joins, new UltraWebGrid(), calcman);

                if (dataReport.Tables.Count == 0)
                {
                    var ex = new Exception("SQL Timeout");
                    this.request.Status = ReportRequestStatus.Failed;
                    this.request.Exception = ex;
                    throw ex;
                }

                if (dataReport.Tables[0].Rows.Count > 65536)
                {
                    var ex = new Exception("Row count is > 65536");
                    this.request.Status = ReportRequestStatus.Failed;
                    this.request.Exception = ex;
                    throw ex;
                }

                this.request.RowCount = dataReport.Tables[0].Rows.Count;

                if (dataReport.Tables[0].Rows.Count == 0)
                {
                    dataReport.Dispose();
                    return new byte[0];
                }

                IWorkbook workbook = ExcelUtils.CreateWorkbook(1);
                IWorksheet worksheet = workbook.Worksheets[0];
                workbook.Activate();

                calcman = CreateCalcManager();
                if (reportRequest.report.exportoptions.isfinancialexport && reportRequest.report.exportoptions.exporthistoryid == 0)
                {
                    var negativeBalances = this.FilterNegativeBalances(dataReport.Tables[0], reportRequest.report);

                    // flag records
                    var exporthistoryid = this.CreateExportHistoryEntries(reportRequest, dataReport, historyIDs, negativeBalances);

                    if (exporthistoryid > 0)
                    {
                        historyCreated = true;
                    }
                    else
                    {
                        return new byte[0];
                    }

                    // remove primary key column for main export
                    if (reportRequest.report.exportoptions.PrimaryKeyInReport == false)
                    {
                        reportRequest.report.columns.RemoveAt(reportRequest.report.columns.Count - 1);
                    }

                    dataReport = clsreports.createReport(this.request, fields, joins, new UltraWebGrid(), calcman);
                }

                int row = 1;
                int col = 1;
                var lstColumnIndexes = new Dictionary<string, int>();
                if (string.IsNullOrEmpty(this.request.report.StaticReportSQL))
                {
                    cStandardColumn standard;
                    cCalculatedColumn calculatedcol;
                    if (this.request.report.exportoptions.showheadersexcel)
                    {
                        foreach (cReportColumn headerCol in this.request.report.columns)
                        {
                            if (headerCol.hidden)
                            {
                                continue;
                            }

                            switch (headerCol.columntype)
                            {
                                case ReportColumnType.Standard:
                                    standard = (cStandardColumn)headerCol;

                                    string fieldDescription = this._relabeler.Relabel(standard.field).Description;

                                    if (standard.funcavg || standard.funccount || standard.funcmax || standard.funcmin
                                        || standard.funcsum)
                                    {
                                        if (standard.funcsum)
                                        {
                                            worksheet.Range[row, col].Text = "SUM of " + fieldDescription;
                                            col++;
                                        }

                                        if (standard.funcavg)
                                        {
                                            worksheet.Range[row, col].Text = "AVG of " + fieldDescription;
                                            col++;
                                        }

                                        if (standard.funccount)
                                        {
                                            worksheet.Range[row, col].Text = "COUNT of " + fieldDescription;
                                            col++;
                                        }

                                        if (standard.funcmax)
                                        {
                                            worksheet.Range[row, col].Text = "MAX of " + fieldDescription;
                                            col++;
                                        }

                                        if (standard.funcmin)
                                        {
                                            worksheet.Range[row, col].Text = "MIN of " + fieldDescription;
                                            col++;
                                        }
                                    }
                                    else
                                    {
                                        worksheet.Range[row, col].Text = fieldDescription;
                                        col++;
                                    }

                                    break;
                                case ReportColumnType.Static:
                                    var staticcol = (cStaticColumn)headerCol;
                                    worksheet.Range[row, col].Text = staticcol.literalname;
                                    col++;
                                    break;
                                case ReportColumnType.Calculated:
                                    calculatedcol = (cCalculatedColumn)headerCol;
                                    worksheet.Range[row, col].Text = calculatedcol.columnname;
                                    col++;
                                    break;
                            }
                        }

                        row++;
                    }

                    DateTime date;
                    cReportColumn column;
                    int dataSetCol;
                    foreach (DataRow dataRow in dataReport.Tables[0].Rows)
                    {
                        dataSetCol = 1;
                        col = 1;
                        for (int x = 0; x < this.request.report.columns.Count; x++)
                        {
                            column = (cReportColumn)this.request.report.columns[x];
                            if (!column.hidden)
                            {
                                switch (column.columntype)
                                {
                                    case ReportColumnType.Standard:
                                        standard = (cStandardColumn)column;
                                        if (standard.funcavg || standard.funccount || standard.funcmax
                                            || standard.funcmin || standard.funcsum)
                                        {
                                            if (standard.funcsum)
                                            {
                                                switch (standard.field.FieldType)
                                                {
                                                    case "M":
                                                    case "FD":
                                                    case "F":
                                                        if (dataRow[x] != DBNull.Value)
                                                        {
                                                            worksheet.Range[row, col].Value = dataRow[x].ToString();
                                                            worksheet.Range[row, col].NumberFormat =
                                                                "#,##0.00;-#,##0.00";
                                                        }

                                                        break;
                                                    case "C":
                                                    case "FC":
                                                    case "A":
                                                        if (dataRow[x] != DBNull.Value)
                                                        {
                                                            worksheet.Range[row, col].Value = dataRow[x].ToString();
                                                            worksheet.Range[row, col].NumberFormat =
                                                                "£#,##0.00;-£#,##0.00";
                                                        }

                                                        break;
                                                    case "N":
                                                    case "I":
                                                        if (dataRow[x] != DBNull.Value)
                                                        {
                                                            worksheet.Range[row, col].Value = dataRow[x].ToString();
                                                            worksheet.Range[row, col].NumberFormat = "###000;-###000";
                                                        }

                                                        break;
                                                }

                                                col++;
                                            }

                                            if (standard.funcavg)
                                            {
                                                switch (standard.field.FieldType)
                                                {
                                                    case "M":
                                                    case "FD":
                                                    case "F":
                                                        if (dataRow[x] != DBNull.Value)
                                                        {
                                                            worksheet.Range[row, col].Value = dataRow[x].ToString();
                                                            worksheet.Range[row, col].NumberFormat =
                                                                "#,##0.00;-#,##0.00";
                                                        }

                                                        break;
                                                    case "C":
                                                    case "FC":
                                                    case "A":
                                                        if (dataRow[x] != DBNull.Value)
                                                        {
                                                            worksheet.Range[row, col].Value = dataRow[x].ToString();
                                                            worksheet.Range[row, col].NumberFormat =
                                                                "£#,##0.00;-£#,##0.00";
                                                        }

                                                        break;
                                                    case "N":
                                                    case "I":
                                                        if (dataRow[x] != DBNull.Value)
                                                        {
                                                            worksheet.Range[row, col].Value = dataRow[x].ToString();
                                                            worksheet.Range[row, col].NumberFormat = "###000;-###000";
                                                        }

                                                        break;
                                                }

                                                col++;
                                            }

                                            if (standard.funccount)
                                            {
                                                worksheet.Range[row, col].Value = dataRow[x].ToString();
                                                worksheet.Range[row, col].NumberFormat = "###000;-###000";
                                                col++;
                                            }

                                            if (standard.funcmax)
                                            {
                                                switch (standard.field.FieldType)
                                                {
                                                    case "M":
                                                    case "FD":
                                                    case "F":
                                                        if (dataRow[x] != DBNull.Value)
                                                        {
                                                            worksheet.Range[row, col].Value = dataRow[x].ToString();
                                                            worksheet.Range[row, col].NumberFormat =
                                                                "#,##0.00;-#,##0.00";
                                                        }

                                                        break;
                                                    case "C":
                                                    case "FC":
                                                    case "A":
                                                        if (dataRow[x] != DBNull.Value)
                                                        {
                                                            worksheet.Range[row, col].Value = dataRow[x].ToString();
                                                            worksheet.Range[row, col].NumberFormat =
                                                                "£#,##0.00;-£#,##0.00";
                                                        }

                                                        break;
                                                    case "N":
                                                    case "I":
                                                        if (dataRow[x] != DBNull.Value)
                                                        {
                                                            worksheet.Range[row, col].Value = dataRow[x].ToString();
                                                            worksheet.Range[row, col].NumberFormat = "###000;-###000";
                                                        }

                                                        break;
                                                    case "D":
                                                        if (dataRow[x] != DBNull.Value)
                                                        {
                                                            date = (DateTime)dataRow[x];
                                                            worksheet.Range[row, col].Value = date.ToShortDateString();
                                                            worksheet.Range[row, col].NumberFormat = "dd/MM/yyyy";
                                                        }

                                                        break;
                                                    case "DT":
                                                        if (dataRow[x] != DBNull.Value)
                                                        {
                                                            date = (DateTime)dataRow[x];
                                                            worksheet.Range[row, col].DateTime =
                                                                DateTime.Parse(
                                                                    date.Day.ToString("00") + "/"
                                                                    + date.Month.ToString("00") + "/"
                                                                    + date.Year.ToString("0000") + " "
                                                                    + date.Hour.ToString("00") + ":"
                                                                    + date.Minute.ToString("00") + ":"
                                                                    + date.Second.ToString("00"));
                                                            worksheet.Range[row, col].NumberFormat =
                                                                "dd/MM/yyyy HH:mm:ss";
                                                        }

                                                        break;
                                                    case "T":
                                                        if (dataRow[x] != DBNull.Value)
                                                        {
                                                            date = (DateTime)dataRow[x];
                                                            worksheet.Range[row, col].Value = date.Hour.ToString("00")
                                                                                              + ":"
                                                                                              + date.Minute.ToString(
                                                                                                  "00");
                                                            worksheet.Range[row, col].NumberFormat = "HH:mm";
                                                        }

                                                        break;
                                                }

                                                col++;
                                            }

                                            if (standard.funcmin)
                                            {
                                                switch (standard.field.FieldType)
                                                {
                                                    case "M":
                                                    case "FD":
                                                    case "F":
                                                        if (dataRow[x] != DBNull.Value)
                                                        {
                                                            worksheet.Range[row, col].Value = dataRow[x].ToString();
                                                            worksheet.Range[row, col].NumberFormat =
                                                                "#,##0.00;-#,##0.00";
                                                        }

                                                        break;
                                                    case "C":
                                                    case "FC":
                                                    case "A":
                                                        if (dataRow[x] != DBNull.Value)
                                                        {
                                                            worksheet.Range[row, col].Value = dataRow[x].ToString();
                                                            worksheet.Range[row, col].NumberFormat =
                                                                "£#,##0.00;-£#,##0.00";
                                                        }

                                                        break;
                                                    case "N":
                                                    case "I":
                                                        if (dataRow[x] != DBNull.Value)
                                                        {
                                                            worksheet.Range[row, col].Value = dataRow[x].ToString();
                                                            worksheet.Range[row, col].NumberFormat = "###000;-###000";
                                                        }

                                                        break;
                                                    case "D":
                                                        if (dataRow[x] != DBNull.Value)
                                                        {
                                                            date = (DateTime)dataRow[x];
                                                            worksheet.Range[row, col].Value = date.ToShortDateString();
                                                            worksheet.Range[row, col].NumberFormat = "dd/MM/yyyy";
                                                        }

                                                        break;
                                                    case "DT":
                                                        if (dataRow[x] != DBNull.Value)
                                                        {
                                                            date = (DateTime)dataRow[x];
                                                            worksheet.Range[row, col].DateTime =
                                                                DateTime.Parse(
                                                                    date.Day.ToString("00") + "/"
                                                                    + date.Month.ToString("00") + "/"
                                                                    + date.Year.ToString("0000") + " "
                                                                    + date.Hour.ToString("00") + ":"
                                                                    + date.Minute.ToString("00") + ":"
                                                                    + date.Second.ToString("00"));
                                                            worksheet.Range[row, col].NumberFormat =
                                                                "dd/MM/yyyy HH:mm:ss";
                                                        }

                                                        break;
                                                    case "T":
                                                        if (dataRow[x] != DBNull.Value)
                                                        {
                                                            date = (DateTime)dataRow[x];
                                                            worksheet.Range[row, col].Value = date.Hour.ToString("00")
                                                                                              + ":"
                                                                                              + date.Minute.ToString(
                                                                                                  "00");
                                                            worksheet.Range[row, col].NumberFormat = "HH:mm";
                                                        }

                                                        break;
                                                }

                                                col++;
                                            }
                                        }
                                        else
                                        {
                                            switch (standard.field.FieldType)
                                            {
                                                case "S":
                                                case "FS":
                                                case "LT":
                                                case "R":
                                                    if (dataRow[x] != DBNull.Value)
                                                    {
                                                        worksheet.Range[row, col].Text =
                                                            this.removeCarriageReturns(
                                                                reportRequest.report.exportoptions.RemoveCarriageReturns,
                                                                dataRow[x].ToString());
                                                    }

                                                    break;
                                                case "D":
                                                    if (dataRow[x] != DBNull.Value)
                                                    {
                                                        date = (DateTime)dataRow[x];
                                                        worksheet.Range[row, col].Value = date.ToShortDateString();
                                                        worksheet.Range[row, col].NumberFormat = "dd/MM/yyyy";
                                                    }

                                                    break;
                                                case "DT":
                                                    if (dataRow[x] != DBNull.Value)
                                                    {
                                                        date = (DateTime)dataRow[x];
                                                        worksheet.Range[row, col].DateTime =
                                                            DateTime.Parse(
                                                                date.Day.ToString("00") + "/"
                                                                + date.Month.ToString("00") + "/"
                                                                + date.Year.ToString("0000") + " "
                                                                + date.Hour.ToString("00") + ":"
                                                                + date.Minute.ToString("00") + ":"
                                                                + date.Second.ToString("00"));
                                                        worksheet.Range[row, col].NumberFormat = "dd/MM/yyyy HH:mm:ss";
                                                    }

                                                    break;
                                                case "T":
                                                    if (dataRow[x] != DBNull.Value)
                                                    {
                                                        date = (DateTime)dataRow[x];
                                                        worksheet.Range[row, col].Text = date.Hour.ToString("00") + ":"
                                                                                         + date.Minute.ToString("00");
                                                        worksheet.Range[row, col].NumberFormat = "HH:mm";
                                                    }

                                                    break;
                                                case "X":
                                                    if (dataRow[x] != DBNull.Value)
                                                    {
                                                        worksheet.Range[row, col].Value = dataRow[x].ToString();
                                                    }

                                                    break;
                                                case "M":
                                                case "FD":
                                                case "F":
                                                    if (dataRow[x] != DBNull.Value)
                                                    {
                                                        worksheet.Range[row, col].Value = dataRow[x].ToString();
                                                        worksheet.Range[row, col].NumberFormat = "#,##0.00;-#,##0.00";
                                                    }

                                                    break;
                                                case "C":
                                                case "FC":
                                                case "A":
                                                    if (dataRow[x] != DBNull.Value)
                                                    {
                                                        worksheet.Range[row, col].Value = dataRow[x].ToString();
                                                        worksheet.Range[row, col].NumberFormat = "£#,##0.00;-£#,##0.00";
                                                    }

                                                    break;
                                                case "N":
                                                case "I":
                                                    if (dataRow[x] != DBNull.Value)
                                                    {
                                                        // check that not relationship field. If so, retrieve text value
                                                        if (standard.field.FieldSource
                                                            != cField.FieldSourceType.Metabase
                                                            && (standard.field.IsForeignKey
                                                                && standard.field.RelatedTableID != Guid.Empty))
                                                        {
                                                            worksheet.Range[row, col].Text =
                                                                this.removeCarriageReturns(
                                                                    reportRequest.report.exportoptions.RemoveCarriageReturns,
                                                                    Spend_Management.cReports.getRelationshipValueText(
                                                                        currentUser, standard.field.FieldID, dataRow[x]));
                                                        }
                                                        else
                                                        {
                                                            worksheet.Range[row, col].Value = dataRow[x].ToString();
                                                            worksheet.Range[row, col].NumberFormat = "###000;-###000";
                                                        }
                                                    }

                                                    break;
                                                case "Y":
                                                    if (dataRow[x] != DBNull.Value)
                                                    {
                                                        worksheet.Range[row, col].Value = (string)dataRow[x];
                                                    }

                                                    break;
                                                case "CL":
                                                    if (dataRow[x] != DBNull.Value)
                                                    {
                                                        cCurrency currency = clsCurrencies.getCurrencyById((int)dataRow[x]);
                                                        if (currency != null)
                                                        {
                                                            worksheet.Range[row, col].Value =
                                                                clsGlobalCurrencies.getGlobalCurrencyById(
                                                                    currency.globalcurrencyid).label;
                                                        }
                                                    }

                                                    break;
                                                default:
                                                    cEventlog.LogEntry(
                                                        string.Format(
                                                            "Reports : cExports : ExportExcel : Unexpected standard column type '{0}' encountered for report ID {1}",
                                                            standard.field.FieldType,
                                                            reportRequest.report.reportid));
                                                    break;
                                            }

                                            col++;
                                        }

                                        break;
                                    case ReportColumnType.Static:
                                        if (dataRow[x].ToString().Length > 0)
                                        {
                                            if (dataRow[x].ToString().Substring(0, 1) == "=")
                                            {
                                                worksheet.Range[row, col].Formula =
                                                    dataRow[x].ToString()
                                                        .Substring(1, dataRow[x].ToString().Length - 1)
                                                        .ToUpper();
                                            }
                                            else
                                            {
                                                worksheet.Range[row, col].Value = dataRow[x].ToString();
                                            }
                                        }

                                        col++;
                                        break;
                                    case ReportColumnType.Calculated:
                                        calculatedcol = (cCalculatedColumn)column;
                                        string formula = calculatedcol.formula;
                                        this.calculateFormula(
                                            ref formula,
                                            dataReport,
                                            row,
                                            dataSetCol,
                                            this.request.report.exportoptions.showheadersexcel);
                                        UltraCalcValue val;
                                        try
                                        {
                                            val =
                                                calcman.Calculate(
                                                    this.convertExportFormula(
                                                        this.request.report.columns,
                                                        formula,
                                                        dataRow,
                                                        Aggregate.None,
                                                        ref lstColumnIndexes,
                                                        ref lstRegex,
                                                        calculatedcol.reportcolumnid));
                                        }
                                        catch (Exception ex)
                                        {
                                            throw new Exception(
                                                string.Format("Calculated column failed to export\n\nReport Column: {0}(no. {1})\n\nReport Row: {2}\n\nFormula: {3}\n\n\nInfragistics Message: \n\n{4}", calculatedcol.columnname, col, row, calculatedcol.formula, ex.Message));
                                        }

                                        this.parseCalculatedValueForIncrementalNumber(
                                            val.ToString(),
                                            dataReport,
                                            row,
                                            dataSetCol,
                                            this.request.report.exportoptions.showheadersexcel);

                                        worksheet.Range[row, col].Value = val.ToString(CultureInfo.InvariantCulture);

                                        col++;
                                        break;
                                }
                            }

                            dataSetCol++;
                        }

                        row++;
                        reportRequest.ProcessedRows++;
                    }

                    if (reportRequest.report.exportoptions.footerreport != null)
                    {
                        reportRequest.report.exportoptions.footerreport.exportoptions = reportRequest.report.exportoptions;
                        dataReport =
                            clsreports.createReport(
                                new cReportRequest(
                                    reportRequest.accountid,
                                    this.request.SubAccountId,
                                    reportRequest.requestnum,
                                    reportRequest.report.exportoptions.footerreport,
                                    ExportType.Excel,
                                    reportRequest.report.exportoptions,
                                    reportRequest.claimantreport,
                                    reportRequest.employeeid,
                                    reportRequest.AccessLevel),
                            fields,
                            joins,
                            new UltraWebGrid(),
                            calcman);

                        foreach (DataRow dataRow in dataReport.Tables[0].Rows)
                        {
                            col = 1;
                            dataSetCol = 1;
                            for (int x = 0; x < dataReport.Tables[0].Columns.Count; x++)
                            {
                                column = (cReportColumn)this.request.report.exportoptions.footerreport.columns[x];
                                if (!column.hidden)
                                {
                                    switch (column.columntype)
                                    {
                                        case ReportColumnType.Standard:
                                            standard = (cStandardColumn)column;
                                            if (standard.funcavg || standard.funccount || standard.funcmax
                                                || standard.funcmin || standard.funcsum)
                                            {
                                                if (standard.funcsum)
                                                {
                                                    switch (standard.field.FieldType)
                                                    {
                                                        case "M":
                                                        case "FD":
                                                        case "F":
                                                            if (dataRow[x] != DBNull.Value)
                                                            {
                                                                worksheet.Range[row, col].Value = dataRow[x].ToString();
                                                                worksheet.Range[row, col].NumberFormat =
                                                                    "#,##0.00;-#,##0.00";
                                                            }

                                                            break;
                                                        case "C":
                                                        case "FC":
                                                        case "A":
                                                            if (dataRow[x] != DBNull.Value)
                                                            {
                                                                worksheet.Range[row, col].Value = dataRow[x].ToString();
                                                                worksheet.Range[row, col].NumberFormat =
                                                                    "£#,##0.00;-£#,##0.00";
                                                            }

                                                            break;
                                                        case "N":
                                                        case "I":
                                                            if (dataRow[x] != DBNull.Value)
                                                            {
                                                                worksheet.Range[row, col].Value = dataRow[x].ToString();
                                                                worksheet.Range[row, col].NumberFormat =
                                                                    "###000;-###000";
                                                            }

                                                            break;
                                                    }

                                                    col++;
                                                }

                                                if (standard.funcavg)
                                                {
                                                    switch (standard.field.FieldType)
                                                    {
                                                        case "M":
                                                        case "FD":
                                                        case "F":
                                                            if (dataRow[x] != DBNull.Value)
                                                            {
                                                                worksheet.Range[row, col].Value = dataRow[x].ToString();
                                                                worksheet.Range[row, col].NumberFormat =
                                                                    "#,##0.00;-#,##0.00";
                                                            }

                                                            break;
                                                        case "C":
                                                        case "FC":
                                                        case "A":
                                                            if (dataRow[x] != DBNull.Value)
                                                            {
                                                                worksheet.Range[row, col].Value = dataRow[x].ToString();
                                                                worksheet.Range[row, col].NumberFormat =
                                                                    "£#,##0.00;-£#,##0.00";
                                                            }

                                                            break;
                                                        case "N":
                                                        case "I":
                                                            if (dataRow[x] != DBNull.Value)
                                                            {
                                                                worksheet.Range[row, col].Value = dataRow[x].ToString();
                                                                worksheet.Range[row, col].NumberFormat =
                                                                    "###000;-###000";
                                                            }

                                                            break;
                                                    }

                                                    col++;
                                                }

                                                if (standard.funccount)
                                                {
                                                    worksheet.Range[row, col].Value = dataRow[x].ToString();
                                                    worksheet.Range[row, col].NumberFormat = "###000;-###000";
                                                    col++;
                                                }

                                                if (standard.funcmax)
                                                {
                                                    switch (standard.field.FieldType)
                                                    {
                                                        case "M":
                                                        case "FD":
                                                        case "F":
                                                            if (dataRow[x] != DBNull.Value)
                                                            {
                                                                worksheet.Range[row, col].Value = dataRow[x].ToString();
                                                                worksheet.Range[row, col].NumberFormat =
                                                                    "#,##0.00;-#,##0.00";
                                                            }

                                                            break;
                                                        case "C":
                                                        case "FC":
                                                        case "A":
                                                            if (dataRow[x] != DBNull.Value)
                                                            {
                                                                worksheet.Range[row, col].Value = dataRow[x].ToString();
                                                                worksheet.Range[row, col].NumberFormat =
                                                                    "£#,##0.00;-£#,##0.00";
                                                            }

                                                            break;
                                                        case "N":
                                                        case "I":
                                                            if (dataRow[x] != DBNull.Value)
                                                            {
                                                                worksheet.Range[row, col].Value = dataRow[x].ToString();
                                                                worksheet.Range[row, col].NumberFormat =
                                                                    "###000;-###000";
                                                            }

                                                            break;
                                                        case "D":
                                                            if (dataRow[x] != DBNull.Value)
                                                            {
                                                                date = (DateTime)dataRow[x];
                                                                worksheet.Range[row, col].Value =
                                                                    date.ToShortDateString();
                                                                worksheet.Range[row, col].NumberFormat = "dd/MM/yyyy";
                                                            }

                                                            break;
                                                        case "DT":
                                                            if (dataRow[x] != DBNull.Value)
                                                            {
                                                                date = (DateTime)dataRow[x];
                                                                worksheet.Range[row, col].DateTime =
                                                                    DateTime.Parse(
                                                                        date.Day.ToString("00") + "/"
                                                                        + date.Month.ToString("00") + "/"
                                                                        + date.Year.ToString("0000") + " "
                                                                        + date.Hour.ToString("00") + ":"
                                                                        + date.Minute.ToString("00") + ":"
                                                                        + date.Second.ToString("00"));
                                                                worksheet.Range[row, col].NumberFormat =
                                                                    "dd/MM/yyyy HH:mm:ss";
                                                            }

                                                            break;
                                                        case "T":
                                                            if (dataRow[x] != DBNull.Value)
                                                            {
                                                                date = (DateTime)dataRow[x];
                                                                worksheet.Range[row, col].Text = date.Hour.ToString(
                                                                    "00") + ":" + date.Minute.ToString("00");
                                                                worksheet.Range[row, col].NumberFormat = "HH:mm";
                                                            }

                                                            break;
                                                    }

                                                    col++;
                                                }

                                                if (standard.funcmin)
                                                {
                                                    switch (standard.field.FieldType)
                                                    {
                                                        case "M":
                                                        case "FD":
                                                        case "F":
                                                            if (dataRow[x] != DBNull.Value)
                                                            {
                                                                worksheet.Range[row, col].Value = dataRow[x].ToString();
                                                                worksheet.Range[row, col].NumberFormat =
                                                                    "#,##0.00;-#,##0.00";
                                                            }

                                                            break;
                                                        case "C":
                                                        case "FC":
                                                        case "A":
                                                            if (dataRow[x] != DBNull.Value)
                                                            {
                                                                worksheet.Range[row, col].Value = dataRow[x].ToString();
                                                                worksheet.Range[row, col].NumberFormat =
                                                                    "£#,##0.00;-£#,##0.00";
                                                            }

                                                            break;
                                                        case "N":
                                                        case "I":
                                                            if (dataRow[x] != DBNull.Value)
                                                            {
                                                                worksheet.Range[row, col].Value = dataRow[x].ToString();
                                                                worksheet.Range[row, col].NumberFormat =
                                                                    "###000;-###000";
                                                            }

                                                            break;
                                                        case "D":
                                                            if (dataRow[x] != DBNull.Value)
                                                            {
                                                                date = (DateTime)dataRow[x];
                                                                worksheet.Range[row, col].Value =
                                                                    date.ToShortDateString();
                                                                worksheet.Range[row, col].NumberFormat = "dd/MM/yyyy";
                                                            }

                                                            break;
                                                        case "DT":
                                                            if (dataRow[x] != DBNull.Value)
                                                            {
                                                                date = (DateTime)dataRow[x];
                                                                worksheet.Range[row, col].DateTime =
                                                                    DateTime.Parse(
                                                                        date.Day.ToString("00") + "/"
                                                                        + date.Month.ToString("00") + "/"
                                                                        + date.Year.ToString("0000") + " "
                                                                        + date.Hour.ToString("00") + ":"
                                                                        + date.Minute.ToString("00") + ":"
                                                                        + date.Second.ToString("00"));
                                                                worksheet.Range[row, col].NumberFormat =
                                                                    "dd/MM/yyyy HH:mm:ss";
                                                            }

                                                            break;
                                                        case "T":
                                                            if (dataRow[x] != DBNull.Value)
                                                            {
                                                                date = (DateTime)dataRow[x];
                                                                worksheet.Range[row, col].Text = date.Hour.ToString(
                                                                    "00") + ":" + date.Minute.ToString("00");
                                                                worksheet.Range[row, col].NumberFormat = "HH:mm";
                                                            }

                                                            break;
                                                    }

                                                    col++;
                                                }
                                            }
                                            else
                                            {
                                                switch (standard.field.FieldType)
                                                {
                                                    case "S":
                                                    case "FS":
                                                    case "LT":
                                                    case "R":
                                                        if (dataRow[x] != DBNull.Value)
                                                        {
                                                            worksheet.Range[row, col].Text =
                                                                this.removeCarriageReturns(
                                                                    reportRequest.report.exportoptions.RemoveCarriageReturns,
                                                                    (string)dataRow[x]);
                                                        }

                                                        break;
                                                    case "D":
                                                        if (dataRow[x] != DBNull.Value)
                                                        {
                                                            date = (DateTime)dataRow[x];
                                                            worksheet.Range[row, col].Value = date.ToShortDateString();
                                                            worksheet.Range[row, col].NumberFormat = "dd/MM/yyyy";
                                                        }

                                                        break;
                                                    case "DT":
                                                        if (dataRow[x] != DBNull.Value)
                                                        {
                                                            date = (DateTime)dataRow[x];
                                                            worksheet.Range[row, col].DateTime =
                                                                DateTime.Parse(
                                                                    date.Day.ToString("00") + "/"
                                                                    + date.Month.ToString("00") + "/"
                                                                    + date.Year.ToString("0000") + " "
                                                                    + date.Hour.ToString("00") + ":"
                                                                    + date.Minute.ToString("00") + ":"
                                                                    + date.Second.ToString("00"));
                                                            worksheet.Range[row, col].NumberFormat =
                                                                "dd/MM/yyyy HH:mm:ss";
                                                        }

                                                        break;
                                                    case "T":
                                                        if (dataRow[x] != DBNull.Value)
                                                        {
                                                            date = (DateTime)dataRow[x];
                                                            worksheet.Range[row, col].Text = date.Hour.ToString("00")
                                                                                             + ":"
                                                                                             + date.Minute.ToString(
                                                                                                 "00");
                                                            worksheet.Range[row, col].NumberFormat = "HH:mm";
                                                        }

                                                        break;
                                                    case "X":
                                                        if (dataRow[x] != DBNull.Value)
                                                        {
                                                            worksheet.Range[row, col].Value = dataRow[x].ToString();
                                                        }

                                                        break;
                                                    case "M":
                                                    case "FD":
                                                    case "F":
                                                        if (dataRow[x] != DBNull.Value)
                                                        {
                                                            worksheet.Range[row, col].Value = dataRow[x].ToString();
                                                            worksheet.Range[row, col].NumberFormat =
                                                                "#,##0.00;-#,##0.00";
                                                        }

                                                        break;
                                                    case "C":
                                                    case "FC":
                                                    case "A":
                                                        if (dataRow[x] != DBNull.Value)
                                                        {
                                                            worksheet.Range[row, col].Value = dataRow[x].ToString();
                                                            worksheet.Range[row, col].NumberFormat =
                                                                "£#,##0.00;-£#,##0.00";
                                                        }

                                                        break;
                                                    case "N":
                                                    case "I":
                                                        if (dataRow[x] != DBNull.Value)
                                                        {
                                                            // check that not relationship field. If so, retrieve text value
                                                            if (standard.field.FieldSource
                                                                != cField.FieldSourceType.Metabase
                                                                && (standard.field.IsForeignKey
                                                                    && standard.field.RelatedTableID != Guid.Empty))
                                                            {
                                                                worksheet.Range[row, col].Text =
                                                                    this.removeCarriageReturns(
                                                                        reportRequest.report.exportoptions.RemoveCarriageReturns,
                                                                        Spend_Management.cReports
                                                                                        .getRelationshipValueText(
                                                                                            currentUser,
                                                                                            standard.field.FieldID,
                                                                                            dataRow[x]));
                                                            }
                                                            else
                                                            {
                                                                worksheet.Range[row, col].Value = dataRow[x].ToString();
                                                                worksheet.Range[row, col].NumberFormat =
                                                                    "#,##0.00;-#,##0.00";
                                                            }
                                                        }

                                                        break;
                                                    case "Y":
                                                        if (dataRow[x] != DBNull.Value)
                                                        {
                                                            worksheet.Range[row, col].Value = (string)dataRow[x];
                                                        }

                                                        break;
                                                    case "CL":
                                                        if (dataRow[x] != DBNull.Value)
                                                        {
                                                            cCurrency currency =
                                                                clsCurrencies.getCurrencyById((int)dataRow[x]);
                                                            if (currency != null)
                                                            {
                                                                worksheet.Range[row, col].Value =
                                                                    clsGlobalCurrencies.getGlobalCurrencyById(
                                                                        currency.globalcurrencyid).label;
                                                            }
                                                        }

                                                        break;
                                                    default:
                                                        cEventlog.LogEntry(
                                                            string.Format(
                                                                "Reports : cExports : ExportExcel : Unexpected standard column type '{0}' encountered for footer report ID {1}. Main report ID is {2}",
                                                                standard.field.FieldType,
                                                                reportRequest.report.exportoptions.footerreport.reportid,
                                                                reportRequest.report.reportid));
                                                        break;
                                                }

                                                col++;
                                            }

                                            break;
                                        case ReportColumnType.Static:
                                            if (dataRow[x].ToString().Substring(0, 1) == "=")
                                            {
                                                worksheet.Range[row, col].Formula =
                                                    dataRow[x].ToString()
                                                              .Substring(1, dataRow[x].ToString().Length - 1)
                                                              .ToUpper();
                                            }
                                            else
                                            {
                                                worksheet.Range[row, col].Value = dataRow[x].ToString();
                                            }

                                            col++;
                                            break;
                                        case ReportColumnType.Calculated:
                                            calculatedcol = (cCalculatedColumn)column;

                                            string formula = calculatedcol.formula;

                                            this.calculateFormula(ref formula, dataReport, row, dataSetCol, false);

                                            UltraCalcValue val =
                                                calcman.Calculate(
                                                    this.convertExportFormula(
                                                        this.request.report.exportoptions.footerreport.columns,
                                                        formula,
                                                        dataRow,
                                                        Aggregate.None,
                                                        ref lstColumnIndexes,
                                                        ref lstRegex,
                                                        calculatedcol.reportcolumnid));

                                            worksheet.Range[row, col].Value = val.ToString();
                                            col++;
                                            break;
                                    }
                                }

                                dataSetCol++;
                            }

                            row++;
                        }
                    }
                }
                else
                {
                    // Static SQL Report export
                    row = 1;
                    col = 1;
                    foreach (DataColumn dataColumn in dataReport.Tables[0].Columns)
                    {
                        worksheet.Range[row, col].Text = dataColumn.ColumnName;
                        col++;
                    }

                    row++;

                    foreach (DataRow dataRow in dataReport.Tables[0].Rows)
                    {
                        col = 1;

                        foreach (object column in dataRow.ItemArray)
                        {
                            if (column != DBNull.Value)
                            {
                                switch (column.GetType().ToString())
                                {
                                    case "System.String":
                                        worksheet.Range[row, col].Text = column.ToString();
                                        break;
                                    case "System.DateTime":
                                        worksheet.Range[row, col].DateTime = (DateTime)column;
                                        break;
                                    case "System.Int16":
                                    case "System.Int32":
                                    case "System.Double":
                                        worksheet[row, col].Value = column.ToString();
                                        break;
                                    case "System.Boolean":
                                        if ((bool)column)
                                        {
                                            worksheet[row, col].Text = "True";
                                        }
                                        else
                                        {
                                            worksheet[row, col].Text = "False";
                                        }

                                        break;
                                    default:
                                        worksheet.Range[row, col].Text = column.ToString();
                                        break;
                                }
                            }

                            col++;
                        }

                        row++;
                        reportRequest.ProcessedRows++;
                    }
                }

                using (Stream stream = new MemoryStream())
                {
                    workbook.SaveAs(stream, ExcelSaveType.SaveAsXLS);
                    workbook.Close();

                    // convert stream to bytes;
                    file = new byte[stream.Length];
                    stream.Seek(0, SeekOrigin.Begin);

                    using (var reader = new BinaryReader(stream))
                    {
                        reader.Read(file, 0, (int)stream.Length);
                        reader.Close();
                    }
                }

                // set the status of the export to exported
                if (historyCreated)
                {
                    this.UpdateHistoryExported(reportRequest);
                }
            }
            catch (Exception ex)
            {
                reportRequest.Exception = ex;
                if (reportRequest.report.exportoptions.exporthistoryid != 0 && historyCreated)
                {
                    this.DeleteHistory(reportRequest);
                }

                string logMessage = "An error occurred exporting to excel.";
                if (ex.Message == "SQL Timeout")
                {
                    logMessage = "Timeout on dataset retrieval.";
                }

                if (ex.Message == "Row count is > 65536")
                {
                    logMessage = "Export to excel cancelled as report generates more than 65536 rows.";
                }

                cEventlog.LogEntry(
                    string.Format(
                        "Reports : cExports : exportExcel : {2} : {0}\n{1}", ex.Message, ex.StackTrace, logMessage));
                throw new Exception(logMessage, ex);
            }
            finally
            {
                if (exceleng != null)
                {
                    exceleng.Dispose();
                }

                if (calcman != null)
                {
                    calcman.Dispose();
                }

                if (dataReport != null)
                {
                    if (dataReport.Tables != null && dataReport.Tables[0] != null)
                    {
                        dataReport.Tables[0].Clear();
                        dataReport.Tables[0].Dispose();
                    }

                    dataReport.Clear();
                    dataReport.Dispose();
                }
            }

            return file;
        }

        /// <summary>
        /// If export options prevent negative payments, find the balance for each claimant and if less than zero filter out of table.
        /// </summary>
        /// <param name="dataTable">
        /// The table to summarize and filter
        /// </param>
        /// <param name="report">
        /// The report for the current export.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>A list of Expense ID's to filter out.
        /// </returns>
        private List<int> FilterNegativeBalances(DataTable dataTable, cReport report)
        {
            var result = new List<int>();
            if (!report.exportoptions.PreventNegativePayment)
            {
                return result;
            }

            var amountToPay = new Guid("319372B8-E3B6-4B93-927B-89D71FC8135B");
            var net = new Guid("28B402FB-E030-49A2-9128-825859CE0A11");
            var total = new Guid("C3C64EB9-C0E1-4B53-8BE9-627128C55011");
            var expenseId = new Guid("A528DE93-3037-46F6-974C-A76BD0C8642A");
            var amountToPayColumn = -1;
            var totalColumn = -1;
            var netColumn = -1;
            var index = -1;
            var summaryColumns = new List<int>();

            foreach (cReportColumn column in report.columns)
            {
                index++;
                if (column.columntype == ReportColumnType.Standard)
                {
                    var standardColumn = (cStandardColumn)column;

                    if (standardColumn.funcsum)
                    {
                        if (standardColumn.field.FieldID == amountToPay)
                        {
                            amountToPayColumn = index;
                        }
                        else
                        if (standardColumn.field.FieldID == net)
                        {
                            netColumn = index;
                        }
                        else
                        if (standardColumn.field.FieldID == total)
                        {
                            totalColumn = index;
                        }
                    }
                    else if (standardColumn.field.FieldID != expenseId)
                    {
                        summaryColumns.Add(index);
                    }

                }
            }

            var summary = new Dictionary<string, decimal>();
            foreach (DataRow dr in dataTable.Rows)
            {
                var summaryString = CreateSummaryString(summaryColumns, dr);

                if (!summary.ContainsKey(summaryString))
                {
                    summary.Add(summaryString, 0);
                }

                var currentValue = summary[summaryString];
                if (amountToPayColumn > -1)
                {
                    currentValue += (decimal)dr[amountToPayColumn];
                }
                else if (netColumn > -1)
                {
                    currentValue += (decimal)dr[netColumn];
                }
                else if (totalColumn > -1)
                {
                    currentValue += (decimal)dr[totalColumn];
                }

                summary[summaryString] = currentValue;
            }

            foreach (DataRow dr in dataTable.Rows)
            {
                var summaryString = CreateSummaryString(summaryColumns, dr);
                if (summary.ContainsKey(summaryString))
                {
                    if (summary[summaryString] < 0)
                    {
                        if (result.Contains((int)dr[report.exportoptions.PrimaryKeyIndex.ToString(CultureInfo.InvariantCulture)]) == false)
                        {
                            result.Add((int)dr[report.exportoptions.PrimaryKeyIndex.ToString(CultureInfo.InvariantCulture)]);
                        }
                    }
                }
            }


            return result;
        }

        /// <summary>
        /// Create a string that is used to "index" a data table and create a summary.
        /// </summary>
        /// <param name="summaryColumns">The columns to use to create the string</param>
        /// <param name="dataRow">The data row to process</param>
        /// <returns>A string containg the values for each of the summary columns delimited by a pipe.</returns>
        private static string CreateSummaryString(List<int> summaryColumns, DataRow dataRow)
        {
            var summaryString = new StringBuilder();
            foreach (var summaryColumn in summaryColumns)
            {
                summaryString.Append(dataRow[summaryColumn]);
                summaryString.Append("|");
            }
            return summaryString.ToString();
        }

        /// <summary>
        /// The export flat file function.
        /// </summary>
        /// <param name="reportRequest">
        /// The report request.
        /// </param>
        /// <returns>
        /// A flat file represented as a byte array./>.
        /// </returns>
        /// <exception cref="Exception">
        /// </exception>
        public byte[] exportFlatfile(cReportRequest reportRequest)
        {
            this.request = reportRequest;
            var file = new byte[0];

            // current user only used for export relationship fields for CEs & UDFs
            ICurrentUser currentUser = new CurrentUser(
                this.request.accountid, this.request.employeeid, -1, Modules.SpendManagement, this.request.SubAccountId);

            var output = new StringBuilder();
            DataSet dataReport = null;
            var clsreports = new cReports(this.request.accountid);
            var historyIDs = new List<int>();
            bool historyCreated = false;
            var clsGlobalCurrencies = new cGlobalCurrencies();
            var clsCurrencies = new cCurrencies(this.request.accountid, this.request.SubAccountId);
            var calcman = CreateCalcManager();
            var fields = new cFields(this.request.accountid);
            var joins = new cJoins(this.request.accountid);
            if (this.request.report.exportoptions.flatfile.Count == 0)
            {
                this.request.report.exportoptions.flatfile =
                    clsreports.getFlatFileOptions(currentUser.EmployeeID, this.request.report);
            }

            try
            {
                if (reportRequest.report.exportoptions.isfinancialexport && this.request.report.exportoptions.exporthistoryid == 0)
                {
                    foreach (cReportColumn rc in reportRequest.report.columns)
                    {
                        if (rc.GetType() == typeof(cStandardColumn)
                            && ((cStandardColumn)rc).field.FieldID == reportRequest.report.basetable.GetPrimaryKey().FieldID)
                        {
                            reportRequest.report.exportoptions.PrimaryKeyIndex = rc.order;
                            reportRequest.report.exportoptions.PrimaryKeyInReport = true;
                        }
                    }

                    if (reportRequest.report.exportoptions.PrimaryKeyIndex == -1)
                    {
                        int order = ((cReportColumn)reportRequest.report.columns[reportRequest.report.columns.Count - 1]).order + 1;
                        reportRequest.report.columns.Add(
                            new cStandardColumn(
                                new Guid(),
                                reportRequest.report.reportid,
                                ReportColumnType.Standard,
                                ColumnSort.None,
                                order,
                                reportRequest.report.basetable.GetPrimaryKey(),
                                false,
                                false,
                                false,
                                false,
                                false,
                                false,
                                true));

                        reportRequest.report.exportoptions.PrimaryKeyIndex = order;
                        reportRequest.report.exportoptions.PrimaryKeyInReport = false;
                    }
                }

                dataReport = clsreports.createReport(this.request, fields, joins, new UltraWebGrid(), calcman);

                if (dataReport.Tables.Count == 0)
                {
                    var ex = new Exception("SQL Timeout");
                    this.request.Status = ReportRequestStatus.Failed;
                    this.request.Exception = ex;
                    throw ex;
                }

                this.request.RowCount = dataReport.Tables[0].Rows.Count;

                if (dataReport.Tables[0].Rows.Count == 0)
                {
                    dataReport.Dispose();
                    return new byte[0];
                }

                if (reportRequest.report.exportoptions.isfinancialexport && reportRequest.report.exportoptions.exporthistoryid == 0)
                {
                    var negativeBalances = this.FilterNegativeBalances(dataReport.Tables[0], reportRequest.report);

                    // flag records
                    var exporthistoryid = this.CreateExportHistoryEntries(reportRequest, dataReport, historyIDs, negativeBalances);

                    if (exporthistoryid > 0)
                    {
                        historyCreated = true;
                    }
                    else
                    {
                        return new byte[0];
                    }


                    // remove primary key column for main export
                    if (reportRequest.report.exportoptions.PrimaryKeyInReport == false)
                    {
                        reportRequest.report.columns.RemoveAt(reportRequest.report.exportoptions.PrimaryKeyIndex - 1);
                    }

                    dataReport = clsreports.createReport(this.request, fields, joins, new UltraWebGrid(), calcman);
                }

                this.request.RowCount = dataReport.Tables[0].Rows.Count;

                int row = 1;
                int col = 1;
                var lstColumnIndexes = new Dictionary<string, int>();
                var lstRegex = new Dictionary<Guid, MatchCollection>();

                if (string.IsNullOrEmpty(this.request.report.StaticReportSQL))
                {
                    cStandardColumn standard;
                    cCalculatedColumn calculatedcol;
                    if (this.request.report.exportoptions.showheadersflatfile)
                    {
                        foreach (cReportColumn headerColumn in this.request.report.columns)
                        {
                            if (headerColumn.hidden)
                            {
                                continue;
                            }

                            switch (headerColumn.columntype)
                            {
                                case ReportColumnType.Standard:
                                    standard = (cStandardColumn)headerColumn;

                                    string fieldDescription = this._relabeler.Relabel(standard.field).Description;

                                    if (standard.funcavg || standard.funccount || standard.funcmax || standard.funcmin
                                        || standard.funcsum)
                                    {
                                        if (standard.funcsum)
                                        {
                                            output.Append(
                                                addTrailingSpaces(
                                                    "SUM of " + fieldDescription,
                                                    this.request.report.exportoptions.flatfile[headerColumn.reportcolumnid]));
                                        }

                                        if (standard.funcavg)
                                        {
                                            output.Append(
                                                addTrailingSpaces(
                                                    "AVG of " + fieldDescription,
                                                    this.request.report.exportoptions.flatfile[headerColumn.reportcolumnid]));
                                        }

                                        if (standard.funccount)
                                        {
                                            output.Append(
                                                addTrailingSpaces(
                                                    "COUNT of " + fieldDescription,
                                                    this.request.report.exportoptions.flatfile[headerColumn.reportcolumnid]));
                                        }

                                        if (standard.funcmax)
                                        {
                                            output.Append(
                                                addTrailingSpaces(
                                                    "MAX of " + fieldDescription,
                                                    this.request.report.exportoptions.flatfile[headerColumn.reportcolumnid]));
                                        }

                                        if (standard.funcmin)
                                        {
                                            output.Append(
                                                addTrailingSpaces(
                                                    "MIN of " + fieldDescription,
                                                    this.request.report.exportoptions.flatfile[headerColumn.reportcolumnid]));
                                        }
                                    }
                                    else
                                    {
                                        output.Append(
                                            addTrailingSpaces(
                                                fieldDescription,
                                                this.request.report.exportoptions.flatfile[headerColumn.reportcolumnid]));
                                    }

                                    break;
                                case ReportColumnType.Static:
                                    var staticcol = (cStaticColumn)headerColumn;

                                    output.Append(
                                        addTrailingSpaces(
                                            staticcol.literalname,
                                            this.request.report.exportoptions.flatfile[headerColumn.reportcolumnid]));
                                    break;
                                case ReportColumnType.Calculated:
                                    calculatedcol = (cCalculatedColumn)headerColumn;

                                    output.Append(
                                        addTrailingSpaces(
                                            calculatedcol.columnname,
                                            this.request.report.exportoptions.flatfile[headerColumn.reportcolumnid]));
                                    break;
                            }

                            col++;
                        }

                        output.Append("\r\n");
                        row++;
                    }

                    DateTime date;
                    cReportColumn column;
                    int dataSetCol;
                    for (int i = 0; i < dataReport.Tables[0].Rows.Count; i++)
                    {
                        col = 1;
                        dataSetCol = 1;

                        for (int x = 0; x < this.request.report.columns.Count; x++)
                        {
                            column = (cReportColumn)this.request.report.columns[x];
                            if (!column.hidden)
                            {
                                switch (column.columntype)
                                {
                                    case ReportColumnType.Standard:
                                        standard = (cStandardColumn)column;
                                        if (standard.field.ValueList)
                                        {
                                            if (dataReport.Tables[0].Rows[i][x] != DBNull.Value)
                                            {
                                                output.Append(
                                                    this.createFlatFormatForString(
                                                        Convert.ToString(dataReport.Tables[0].Rows[i][x]),
                                                        reportRequest.report.exportoptions.flatfile[column.reportcolumnid]));
                                            }
                                            else
                                            {
                                                output.Append(
                                                    this.createFlatFormatForString(
                                                        string.Empty,
                                                        reportRequest.report.exportoptions.flatfile[column.reportcolumnid]));
                                            }

                                            col++;
                                        }
                                        else
                                        {
                                            if (standard.funcavg || standard.funccount || standard.funcmax
                                                || standard.funcmin || standard.funcsum)
                                            {
                                                if (standard.funcsum)
                                                {
                                                    switch (standard.field.FieldType)
                                                    {
                                                        case "M":
                                                        case "FD":
                                                        case "F":
                                                            if (dataReport.Tables[0].Rows[i][x] != DBNull.Value)
                                                            {
                                                                output.Append(
                                                                    addLeadingZeroes(
                                                                        dataReport.Tables[0].Rows[i][x].ToString(),
                                                                        reportRequest.report.exportoptions.flatfile[
                                                                            column.reportcolumnid]));
                                                            }

                                                            break;
                                                        case "C":
                                                        case "FC":
                                                        case "A":
                                                            if (dataReport.Tables[0].Rows[i][x] != DBNull.Value)
                                                            {
                                                                output.Append(
                                                                    addLeadingZeroes(
                                                                        dataReport.Tables[0].Rows[i][x].ToString(),
                                                                        reportRequest.report.exportoptions.flatfile[
                                                                            column.reportcolumnid]));
                                                            }

                                                            break;
                                                        case "N":
                                                        case "I":
                                                            if (dataReport.Tables[0].Rows[i][x] != DBNull.Value)
                                                            {
                                                                output.Append(
                                                                    addLeadingZeroes(
                                                                        dataReport.Tables[0].Rows[i][x].ToString(),
                                                                        reportRequest.report.exportoptions.flatfile[
                                                                            column.reportcolumnid]));
                                                            }

                                                            break;
                                                    }

                                                    col++;
                                                }

                                                if (standard.funcavg)
                                                {
                                                    switch (standard.field.FieldType)
                                                    {
                                                        case "M":
                                                        case "FD":
                                                        case "F":
                                                            if (dataReport.Tables[0].Rows[i][x] != DBNull.Value)
                                                            {
                                                                output.Append(
                                                                    addLeadingZeroes(
                                                                        dataReport.Tables[0].Rows[i][x].ToString(),
                                                                        reportRequest.report.exportoptions.flatfile[
                                                                            column.reportcolumnid]));
                                                            }

                                                            break;
                                                        case "C":
                                                        case "FC":
                                                        case "A":
                                                            if (dataReport.Tables[0].Rows[i][x] != DBNull.Value)
                                                            {
                                                                output.Append(
                                                                    addLeadingZeroes(
                                                                        dataReport.Tables[0].Rows[i][x].ToString(),
                                                                        reportRequest.report.exportoptions.flatfile[
                                                                            column.reportcolumnid]));
                                                            }

                                                            break;
                                                        case "N":
                                                        case "I":
                                                            if (dataReport.Tables[0].Rows[i][x] != DBNull.Value)
                                                            {
                                                                output.Append(
                                                                    addLeadingZeroes(
                                                                        dataReport.Tables[0].Rows[i][x].ToString(),
                                                                        reportRequest.report.exportoptions.flatfile[
                                                                            column.reportcolumnid]));
                                                            }

                                                            break;
                                                    }

                                                    col++;
                                                }

                                                if (standard.funccount)
                                                {
                                                    output.Append(
                                                        addLeadingZeroes(
                                                            dataReport.Tables[0].Rows[i][x].ToString(),
                                                            reportRequest.report.exportoptions.flatfile[column.reportcolumnid]));
                                                    col++;
                                                }

                                                if (standard.funcmax)
                                                {
                                                    switch (standard.field.FieldType)
                                                    {
                                                        case "M":
                                                        case "FD":
                                                        case "F":
                                                            if (dataReport.Tables[0].Rows[i][x] != DBNull.Value)
                                                            {
                                                                output.Append(
                                                                    addLeadingZeroes(
                                                                        dataReport.Tables[0].Rows[i][x].ToString(),
                                                                        reportRequest.report.exportoptions.flatfile[
                                                                            column.reportcolumnid]));
                                                            }

                                                            break;
                                                        case "C":
                                                        case "FC":
                                                        case "A":
                                                            if (dataReport.Tables[0].Rows[i][x] != DBNull.Value)
                                                            {
                                                                output.Append(
                                                                    addLeadingZeroes(
                                                                        dataReport.Tables[0].Rows[i][x].ToString(),
                                                                        reportRequest.report.exportoptions.flatfile[
                                                                            column.reportcolumnid]));
                                                            }

                                                            break;
                                                        case "N":
                                                        case "I":
                                                            if (dataReport.Tables[0].Rows[i][x] != DBNull.Value)
                                                            {
                                                                output.Append(
                                                                    addLeadingZeroes(
                                                                        dataReport.Tables[0].Rows[i][x].ToString(),
                                                                        reportRequest.report.exportoptions.flatfile[
                                                                            column.reportcolumnid]));
                                                            }

                                                            break;
                                                        case "D":
                                                            if (dataReport.Tables[0].Rows[i][x] != DBNull.Value)
                                                            {
                                                                date = (DateTime)dataReport.Tables[0].Rows[i][x];
                                                                output.Append(date.ToShortDateString());
                                                            }

                                                            break;
                                                        case "DT":
                                                            if (dataReport.Tables[0].Rows[i][x] != DBNull.Value)
                                                            {
                                                                date = (DateTime)dataReport.Tables[0].Rows[i][x];
                                                                output.Append(date);
                                                            }

                                                            break;
                                                        case "T":
                                                            if (dataReport.Tables[0].Rows[i][x] != DBNull.Value)
                                                            {
                                                                date = (DateTime)dataReport.Tables[0].Rows[i][x];
                                                                output.Append(
                                                                    date.Hour.ToString("00") + ":"
                                                                    + date.Minute.ToString("00"));
                                                            }

                                                            break;
                                                    }

                                                    col++;
                                                }

                                                if (standard.funcmin)
                                                {
                                                    switch (standard.field.FieldType)
                                                    {
                                                        case "M":
                                                        case "FD":
                                                        case "F":
                                                            if (dataReport.Tables[0].Rows[i][x] != DBNull.Value)
                                                            {
                                                                output.Append(
                                                                    addLeadingZeroes(
                                                                        dataReport.Tables[0].Rows[i][x].ToString(),
                                                                        reportRequest.report.exportoptions.flatfile[
                                                                            column.reportcolumnid]));
                                                            }

                                                            break;
                                                        case "C":
                                                        case "FC":
                                                        case "A":
                                                            if (dataReport.Tables[0].Rows[i][x] != DBNull.Value)
                                                            {
                                                                output.Append(
                                                                    addLeadingZeroes(
                                                                        dataReport.Tables[0].Rows[i][x].ToString(),
                                                                        reportRequest.report.exportoptions.flatfile[
                                                                            column.reportcolumnid]));
                                                            }

                                                            break;
                                                        case "N":
                                                        case "I":
                                                            if (dataReport.Tables[0].Rows[i][x] != DBNull.Value)
                                                            {
                                                                output.Append(
                                                                    addLeadingZeroes(
                                                                        dataReport.Tables[0].Rows[i][x].ToString(),
                                                                        reportRequest.report.exportoptions.flatfile[
                                                                            column.reportcolumnid]));
                                                            }

                                                            break;
                                                        case "D":
                                                            if (dataReport.Tables[0].Rows[i][x] != DBNull.Value)
                                                            {
                                                                date = (DateTime)dataReport.Tables[0].Rows[i][x];
                                                                output.Append(date.ToShortDateString());
                                                            }

                                                            break;
                                                        case "DT":
                                                            if (dataReport.Tables[0].Rows[i][x] != DBNull.Value)
                                                            {
                                                                date = (DateTime)dataReport.Tables[0].Rows[i][x];
                                                                output.Append(date);
                                                            }

                                                            break;
                                                        case "T":
                                                            if (dataReport.Tables[0].Rows[i][x] != DBNull.Value)
                                                            {
                                                                date = (DateTime)dataReport.Tables[0].Rows[i][x];
                                                                output.Append(
                                                                    date.Hour.ToString("00") + ":"
                                                                    + date.Minute.ToString("00"));
                                                            }

                                                            break;
                                                    }

                                                    col++;
                                                }
                                            }
                                            else
                                            {
                                                switch (standard.field.FieldType)
                                                {
                                                    case "S":
                                                    case "FS":
                                                    case "LT":
                                                    case "R":
                                                        if (dataReport.Tables[0].Rows[i][x] != DBNull.Value)
                                                        {
                                                            output.Append(
                                                                this.removeCarriageReturns(
                                                                    reportRequest.report.exportoptions.RemoveCarriageReturns,
                                                                    this.createFlatFormatForString(
                                                                        Convert.ToString(
                                                                            dataReport.Tables[0].Rows[i][x]),
                                                                        reportRequest.report.exportoptions.flatfile[
                                                                            column.reportcolumnid])));
                                                        }
                                                        else
                                                        {
                                                            output.Append(
                                                                this.removeCarriageReturns(
                                                                    reportRequest.report.exportoptions.RemoveCarriageReturns,
                                                                    this.createFlatFormatForString(
                                                                        string.Empty,
                                                                        reportRequest.report.exportoptions.flatfile[
                                                                            column.reportcolumnid])));
                                                        }

                                                        break;
                                                    case "D":
                                                        if (dataReport.Tables[0].Rows[i][x] != DBNull.Value)
                                                        {
                                                            date = (DateTime)dataReport.Tables[0].Rows[i][x];
                                                            output.Append(
                                                                this.createFlatFormatForString(
                                                                    date.ToShortDateString(),
                                                                    reportRequest.report.exportoptions.flatfile[
                                                                        column.reportcolumnid]));
                                                        }

                                                        break;
                                                    case "DT":
                                                        if (dataReport.Tables[0].Rows[i][x] != DBNull.Value)
                                                        {
                                                            date = (DateTime)dataReport.Tables[0].Rows[i][x];
                                                            output.Append(
                                                                this.createFlatFormatForString(
                                                                    date.ToString(),
                                                                    reportRequest.report.exportoptions.flatfile[
                                                                        column.reportcolumnid]));
                                                        }

                                                        break;
                                                    case "T":
                                                        if (dataReport.Tables[0].Rows[i][x] != DBNull.Value)
                                                        {
                                                            date = (DateTime)dataReport.Tables[0].Rows[i][x];
                                                            output.Append(
                                                                this.createFlatFormatForString(
                                                                    date.Hour.ToString("00") + ":"
                                                                    + date.Minute.ToString("00"),
                                                                    reportRequest.report.exportoptions.flatfile[
                                                                        column.reportcolumnid]));
                                                        }

                                                        break;
                                                    case "X":
                                                        if (dataReport.Tables[0].Rows[i][x] != DBNull.Value)
                                                        {
                                                            output.Append(dataReport.Tables[0].Rows[i][x]);
                                                        }

                                                        break;
                                                    case "M":
                                                    case "FD":
                                                    case "F":
                                                        if (dataReport.Tables[0].Rows[i][x] != DBNull.Value)
                                                        {
                                                            output.Append(dataReport.Tables[0].Rows[i][x]);
                                                        }

                                                        break;
                                                    case "C":
                                                    case "FC":
                                                    case "A":
                                                        if (dataReport.Tables[0].Rows[i][x] != DBNull.Value)
                                                        {
                                                            output.Append(
                                                                addLeadingZeroes(
                                                                    dataReport.Tables[0].Rows[i][x].ToString(),
                                                                    reportRequest.report.exportoptions.flatfile[
                                                                        column.reportcolumnid]));
                                                        }

                                                        break;
                                                    case "N":
                                                    case "I":
                                                        if (dataReport.Tables[0].Rows[i][x] != DBNull.Value)
                                                        {
                                                            // check that not relationship field. If so, retrieve text value
                                                            if (standard.field.FieldSource
                                                                != cField.FieldSourceType.Metabase
                                                                && (standard.field.IsForeignKey
                                                                    && standard.field.RelatedTableID != Guid.Empty))
                                                            {
                                                                output.Append(
                                                                    this.removeCarriageReturns(
                                                                        reportRequest.report.exportoptions.RemoveCarriageReturns,
                                                                        this.createFlatFormatForString(
                                                                            Spend_Management.cReports
                                                                                            .getRelationshipValueText(
                                                                                                currentUser,
                                                                                                standard.field.FieldID,
                                                                                                (int)
                                                                                                dataReport.Tables[0]
                                                                                                    .Rows[i][x]),
                                                                            reportRequest.report.exportoptions.flatfile[
                                                                                column.reportcolumnid])));
                                                            }
                                                            else
                                                            {
                                                                output.Append(
                                                                    addLeadingZeroes(
                                                                        dataReport.Tables[0].Rows[i][x].ToString(),
                                                                        reportRequest.report.exportoptions.flatfile[
                                                                            column.reportcolumnid]));
                                                            }
                                                        }

                                                        break;
                                                    case "Y":
                                                        if (dataReport.Tables[0].Rows[i][x] != DBNull.Value)
                                                        {
                                                            output.Append(
                                                                this.createFlatFormatForString(
                                                                    (string)dataReport.Tables[0].Rows[i][x],
                                                                    reportRequest.report.exportoptions.flatfile[
                                                                        column.reportcolumnid]));
                                                        }
                                                        else
                                                        {
                                                            output.Append(
                                                                this.createFlatFormatForString(
                                                                    "unknown",
                                                                    reportRequest.report.exportoptions.flatfile[
                                                                        column.reportcolumnid]));
                                                        }

                                                        break;
                                                    case "CL":
                                                        if (dataReport.Tables[0].Rows[i][x] != DBNull.Value)
                                                        {
                                                            cCurrency currency =
                                                                clsCurrencies.getCurrencyById(
                                                                    (int)dataReport.Tables[0].Rows[i][x]);
                                                            output.Append(
                                                                this.removeCarriageReturns(
                                                                    reportRequest.report.exportoptions.RemoveCarriageReturns,
                                                                    this.createFlatFormatForString(
                                                                        currency == null
                                                                            ? string.Empty
                                                                            : clsGlobalCurrencies.getGlobalCurrencyById(
                                                                                currency.globalcurrencyid).label,
                                                                        reportRequest.report.exportoptions.flatfile[
                                                                            column.reportcolumnid])));
                                                        }
                                                        else
                                                        {
                                                            output.Append(
                                                                this.removeCarriageReturns(
                                                                    reportRequest.report.exportoptions.RemoveCarriageReturns,
                                                                    this.createFlatFormatForString(
                                                                        string.Empty,
                                                                        reportRequest.report.exportoptions.flatfile[
                                                                            column.reportcolumnid])));
                                                        }

                                                        break;
                                                    default:
                                                        cEventlog.LogEntry(
                                                            string.Format(
                                                                "Reports : cExports : ExportExcel : Unexpected standard column type '{0}' encountered for report ID {1}",
                                                                standard.field.FieldType,
                                                                reportRequest.report.reportid.ToString()));
                                                        break;
                                                }

                                                col++;
                                            }
                                        }

                                        break;

                                    case ReportColumnType.Static:
                                        output.Append(dataReport.Tables[0].Rows[i][x]);
                                        col++;
                                        break;

                                    case ReportColumnType.Calculated:
                                        calculatedcol = (cCalculatedColumn)column;
                                        string formula = calculatedcol.formula;

                                        this.calculateFormula(
                                            ref formula,
                                            dataReport,
                                            row,
                                            dataSetCol,
                                            this.request.report.exportoptions.showheadersflatfile);

                                        UltraCalcValue val;
                                        try
                                        {
                                            val =
                                                calcman.Calculate(
                                                    this.convertExportFormula(
                                                        this.request.report.columns,
                                                        formula,
                                                        dataReport.Tables[0].Rows[i],
                                                        Aggregate.None,
                                                        ref lstColumnIndexes,
                                                        ref lstRegex,
                                                        calculatedcol.reportcolumnid));
                                        }
                                        catch (Exception ex)
                                        {
                                            throw new Exception(
                                                string.Format("Calculated column failed to export\n\nReport Column: {0}(no. {1})\n\nReport Row: {2}\n\nFormula: {3}\n\n\nInfragistics Message: \n\n{4}", calculatedcol.columnname, col, row, calculatedcol.formula, ex.Message));
                                        }

                                        this.parseCalculatedValueForIncrementalNumber(
                                            val.ToString(CultureInfo.InvariantCulture),
                                            dataReport,
                                            row,
                                            dataSetCol,
                                            this.request.report.exportoptions.showheadersflatfile);

                                        output.Append(val);
                                        col++;
                                        break;
                                }
                            }

                            dataSetCol++;
                        }

                        output.Append("\r\n");
                        row++;
                        reportRequest.ProcessedRows++;
                    }

                    if (reportRequest.report.exportoptions.footerreport != null)
                    {
                        reportRequest.report.exportoptions.footerreport.exportoptions = reportRequest.report.exportoptions;
                        dataReport =
                            clsreports.createReport(
                                new cReportRequest(
                                    reportRequest.accountid,
                                    reportRequest.SubAccountId,
                                    reportRequest.requestnum,
                                    reportRequest.report.exportoptions.footerreport,
                                    ExportType.Excel,
                                    reportRequest.report.exportoptions,
                                    reportRequest.claimantreport,
                                    reportRequest.employeeid,
                                    reportRequest.AccessLevel),
                                fields,
                                joins,
                                new UltraWebGrid(),
                                calcman);
                        cReportColumn reqReportColumn;

                        for (int i = 0; i < dataReport.Tables[0].Rows.Count; i++)
                        {
                            col = 1;
                            dataSetCol = 1;

                            for (int x = 0; x < reportRequest.report.exportoptions.footerreport.columns.Count; x++)
                            {
                                column = (cReportColumn)this.request.report.exportoptions.footerreport.columns[x];
                                reqReportColumn = (cReportColumn)this.request.report.columns[x];

                                if (!column.hidden)
                                {
                                    switch (column.columntype)
                                    {
                                        case ReportColumnType.Standard:
                                            standard = (cStandardColumn)column;
                                            if (standard.funcavg || standard.funccount || standard.funcmax
                                                || standard.funcmin || standard.funcsum)
                                            {
                                                if (standard.funcsum)
                                                {
                                                    switch (standard.field.FieldType)
                                                    {
                                                        case "M":
                                                        case "FD":
                                                        case "F":
                                                            if (dataReport.Tables[0].Rows[i][x] != DBNull.Value)
                                                            {
                                                                output.Append(
                                                                    addLeadingZeroes(
                                                                        dataReport.Tables[0].Rows[i][x].ToString(),
                                                                        reportRequest.report.exportoptions.footerreport.exportoptions.flatfile[reqReportColumn.reportcolumnid]));
                                                            }

                                                            break;
                                                        case "C":
                                                        case "FC":
                                                        case "A":
                                                            if (dataReport.Tables[0].Rows[i][x] != DBNull.Value)
                                                            {
                                                                output.Append(
                                                                    addLeadingZeroes(
                                                                        dataReport.Tables[0].Rows[i][x].ToString(),
                                                                        reportRequest.report.exportoptions.footerreport.exportoptions.flatfile[reqReportColumn.reportcolumnid]));
                                                            }

                                                            break;
                                                        case "N":
                                                        case "I":
                                                            if (dataReport.Tables[0].Rows[i][x] != DBNull.Value)
                                                            {
                                                                output.Append(
                                                                    addLeadingZeroes(
                                                                        dataReport.Tables[0].Rows[i][x].ToString(),
                                                                        reportRequest.report.exportoptions.footerreport.exportoptions.flatfile[reqReportColumn.reportcolumnid]));
                                                            }

                                                            break;
                                                    }

                                                    col++;
                                                }

                                                if (standard.funcavg)
                                                {
                                                    switch (standard.field.FieldType)
                                                    {
                                                        case "M":
                                                        case "FD":
                                                        case "F":
                                                            if (dataReport.Tables[0].Rows[i][x] != DBNull.Value)
                                                            {
                                                                output.Append(
                                                                    addLeadingZeroes(
                                                                        dataReport.Tables[0].Rows[i][x].ToString(),
                                                                        reportRequest.report.exportoptions.footerreport.exportoptions.flatfile[reqReportColumn.reportcolumnid]));
                                                            }

                                                            break;
                                                        case "C":
                                                        case "FC":
                                                        case "A":
                                                            if (dataReport.Tables[0].Rows[i][x] != DBNull.Value)
                                                            {
                                                                output.Append(
                                                                    addLeadingZeroes(
                                                                        dataReport.Tables[0].Rows[i][x].ToString(),
                                                                        reportRequest.report.exportoptions.footerreport.exportoptions.flatfile[reqReportColumn.reportcolumnid]));
                                                            }

                                                            break;
                                                        case "N":
                                                        case "I":
                                                            if (dataReport.Tables[0].Rows[i][x] != DBNull.Value)
                                                            {
                                                                output.Append(
                                                                    addLeadingZeroes(
                                                                        dataReport.Tables[0].Rows[i][x].ToString(),
                                                                        reportRequest.report.exportoptions.footerreport.exportoptions.flatfile[reqReportColumn.reportcolumnid]));
                                                            }

                                                            break;
                                                    }

                                                    col++;
                                                }

                                                if (standard.funccount)
                                                {
                                                    output.Append(
                                                        addLeadingZeroes(
                                                            dataReport.Tables[0].Rows[i][x].ToString(),
                                                            reportRequest.report.exportoptions.footerreport.exportoptions.flatfile[reqReportColumn.reportcolumnid]));
                                                    col++;
                                                }

                                                if (standard.funcmax)
                                                {
                                                    switch (standard.field.FieldType)
                                                    {
                                                        case "M":
                                                        case "FD":
                                                        case "F":
                                                            if (dataReport.Tables[0].Rows[i][x] != DBNull.Value)
                                                            {
                                                                output.Append(
                                                                    addLeadingZeroes(
                                                                        dataReport.Tables[0].Rows[i][x].ToString(),
                                                                        reportRequest.report.exportoptions.footerreport.exportoptions.flatfile[reqReportColumn.reportcolumnid]));
                                                            }

                                                            break;
                                                        case "C":
                                                        case "FC":
                                                        case "A":
                                                            if (dataReport.Tables[0].Rows[i][x] != DBNull.Value)
                                                            {
                                                                output.Append(
                                                                    addLeadingZeroes(
                                                                        dataReport.Tables[0].Rows[i][x].ToString(),
                                                                        reportRequest.report.exportoptions.footerreport.exportoptions.flatfile[reqReportColumn.reportcolumnid]));
                                                            }

                                                            break;
                                                        case "N":
                                                        case "I":
                                                            if (dataReport.Tables[0].Rows[i][x] != DBNull.Value)
                                                            {
                                                                output.Append(
                                                                    addLeadingZeroes(
                                                                        dataReport.Tables[0].Rows[i][x].ToString(),
                                                                        reportRequest.report.exportoptions.footerreport.exportoptions.flatfile[reqReportColumn.reportcolumnid]));
                                                            }

                                                            break;
                                                        case "D":
                                                            if (dataReport.Tables[0].Rows[i][x] != DBNull.Value)
                                                            {
                                                                output.Append(dataReport.Tables[0].Rows[i][x]);
                                                            }

                                                            break;
                                                        case "DT":
                                                            if (dataReport.Tables[0].Rows[i][x] != DBNull.Value)
                                                            {
                                                                date = (DateTime)dataReport.Tables[0].Rows[i][x];
                                                                output.Append(date);
                                                            }

                                                            break;
                                                        case "T":
                                                            if (dataReport.Tables[0].Rows[i][x] != DBNull.Value)
                                                            {
                                                                date = (DateTime)dataReport.Tables[0].Rows[i][x];
                                                                output.Append(
                                                                    date.Hour.ToString("00") + ":"
                                                                    + date.Minute.ToString("00"));
                                                            }

                                                            break;
                                                    }

                                                    col++;
                                                }

                                                if (standard.funcmin)
                                                {
                                                    switch (standard.field.FieldType)
                                                    {
                                                        case "M":
                                                        case "FD":
                                                        case "F":
                                                            if (dataReport.Tables[0].Rows[i][x] != DBNull.Value)
                                                            {
                                                                output.Append(
                                                                    addLeadingZeroes(
                                                                        dataReport.Tables[0].Rows[i][x].ToString(),
                                                                        reportRequest.report.exportoptions.footerreport.exportoptions.flatfile[reqReportColumn.reportcolumnid]));
                                                            }

                                                            break;
                                                        case "C":
                                                        case "FC":
                                                        case "A":
                                                            if (dataReport.Tables[0].Rows[i][x] != DBNull.Value)
                                                            {
                                                                output.Append(
                                                                    addLeadingZeroes(
                                                                        dataReport.Tables[0].Rows[i][x].ToString(),
                                                                        reportRequest.report.exportoptions.footerreport.exportoptions.flatfile[reqReportColumn.reportcolumnid]));
                                                            }

                                                            break;
                                                        case "N":
                                                        case "I":
                                                            if (dataReport.Tables[0].Rows[i][x] != DBNull.Value)
                                                            {
                                                                output.Append(
                                                                    addLeadingZeroes(
                                                                        dataReport.Tables[0].Rows[i][x].ToString(),
                                                                        reportRequest.report.exportoptions.footerreport.exportoptions.flatfile[reqReportColumn.reportcolumnid]));
                                                            }

                                                            break;
                                                        case "D":
                                                            if (dataReport.Tables[0].Rows[i][x] != DBNull.Value)
                                                            {
                                                                output.Append(dataReport.Tables[0].Rows[i][x]);
                                                            }

                                                            break;
                                                        case "DT":
                                                            if (dataReport.Tables[0].Rows[i][x] != DBNull.Value)
                                                            {
                                                                date = (DateTime)dataReport.Tables[0].Rows[i][x];
                                                                output.Append(date);
                                                            }

                                                            break;
                                                        case "T":
                                                            if (dataReport.Tables[0].Rows[i][x] != DBNull.Value)
                                                            {
                                                                date = (DateTime)dataReport.Tables[0].Rows[i][x];
                                                                output.Append(
                                                                    date.Hour.ToString("00") + ":"
                                                                    + date.Minute.ToString("00"));
                                                            }

                                                            break;
                                                    }

                                                    col++;
                                                }
                                            }
                                            else
                                            {
                                                switch (standard.field.FieldType)
                                                {
                                                    case "S":
                                                    case "FS":
                                                    case "LT":
                                                    case "R":
                                                        if (dataReport.Tables[0].Rows[i][x] != DBNull.Value)
                                                        {
                                                            output.Append(
                                                                this.removeCarriageReturns(
                                                                    reportRequest.report.exportoptions.RemoveCarriageReturns,
                                                                    addTrailingSpaces(
                                                                        (string)dataReport.Tables[0].Rows[i][x],
                                                                        reportRequest.report.exportoptions.footerreport.exportoptions.flatfile[reqReportColumn.reportcolumnid])));
                                                        }

                                                        break;
                                                    case "D":
                                                        if (dataReport.Tables[0].Rows[i][x] != DBNull.Value)
                                                        {
                                                            date = (DateTime)dataReport.Tables[0].Rows[i][x];
                                                            output.Append(date);
                                                        }

                                                        break;
                                                    case "Y":
                                                        if (dataReport.Tables[0].Rows[i][x] != DBNull.Value)
                                                        {
                                                            output.Append(
                                                                addTrailingSpaces(
                                                                    (string)dataReport.Tables[0].Rows[i][x],
                                                                    reportRequest.report.exportoptions.footerreport.exportoptions.flatfile[reqReportColumn.reportcolumnid]));
                                                        }

                                                        break;
                                                    case "DT":
                                                        if (dataReport.Tables[0].Rows[i][x] != DBNull.Value)
                                                        {
                                                            date = (DateTime)dataReport.Tables[0].Rows[i][x];
                                                            output.Append(date);
                                                        }

                                                        break;
                                                    case "T":
                                                        if (dataReport.Tables[0].Rows[i][x] != DBNull.Value)
                                                        {
                                                            date = (DateTime)dataReport.Tables[0].Rows[i][x];
                                                            output.Append(
                                                                date.Hour.ToString("00") + ":"
                                                                + date.Minute.ToString("00"));
                                                        }

                                                        break;
                                                    case "X":
                                                        if (dataReport.Tables[0].Rows[i][x] != DBNull.Value)
                                                        {
                                                            output.Append(dataReport.Tables[0].Rows[i][x]);
                                                        }

                                                        break;
                                                    case "M":
                                                    case "FD":
                                                    case "F":
                                                        if (dataReport.Tables[0].Rows[i][x] != DBNull.Value)
                                                        {
                                                            output.Append(dataReport.Tables[0].Rows[i][x]);

                                                            // worksheet.Range[row, col].NumberFormat = "#,##0.00;-#,##0.00";
                                                        }

                                                        break;
                                                    case "C":
                                                    case "FC":
                                                    case "A":
                                                        if (dataReport.Tables[0].Rows[i][x] != DBNull.Value)
                                                        {
                                                            output.Append(
                                                                addLeadingZeroes(
                                                                    dataReport.Tables[0].Rows[i][x].ToString(),
                                                                    reportRequest.report.exportoptions.footerreport.exportoptions
                                                                       .flatfile[column.reportcolumnid]));

                                                            // worksheet.Range[row, col].NumberFormat = "£#,##0.00;-£#,##0.00";
                                                        }

                                                        break;
                                                    case "N":
                                                    case "I":
                                                        if (dataReport.Tables[0].Rows[i][x] != DBNull.Value)
                                                        {
                                                            // check that not relationship field. If so, retrieve text value
                                                            if (standard.field.FieldSource
                                                                != cField.FieldSourceType.Metabase
                                                                && (standard.field.IsForeignKey
                                                                    && standard.field.RelatedTableID != Guid.Empty))
                                                            {
                                                                output.Append(
                                                                    this.removeCarriageReturns(
                                                                        reportRequest.report.exportoptions.footerreport
                                                                           .exportoptions.RemoveCarriageReturns,
                                                                        addTrailingSpaces(
                                                                            Spend_Management.cReports
                                                                                            .getRelationshipValueText(
                                                                                                currentUser,
                                                                                                standard.field.FieldID,
                                                                                                (int)
                                                                                                dataReport.Tables[0]
                                                                                                    .Rows[i][x]),
                                                                            reportRequest.report.exportoptions.footerreport.exportoptions.flatfile[reqReportColumn.reportcolumnid])));
                                                            }
                                                            else
                                                            {
                                                                output.Append(
                                                                    addLeadingZeroes(
                                                                        dataReport.Tables[0].Rows[i][x].ToString(),
                                                                        reportRequest.report.exportoptions.footerreport.exportoptions.flatfile[reqReportColumn.reportcolumnid]));
                                                            }
                                                        }

                                                        break;
                                                    case "CL":
                                                        if (dataReport.Tables[0].Rows[i][x] != DBNull.Value)
                                                        {
                                                            cCurrency currency =
                                                                clsCurrencies.getCurrencyById(
                                                                    (int)dataReport.Tables[0].Rows[i][x]);

                                                            output.Append(
                                                                this.removeCarriageReturns(
                                                                    reportRequest.report.exportoptions.footerreport.exportoptions
                                                                       .RemoveCarriageReturns,
                                                                    addTrailingSpaces(
                                                                        currency == null
                                                                            ? string.Empty
                                                                            : clsGlobalCurrencies.getGlobalCurrencyById(
                                                                                currency.globalcurrencyid).label,
                                                                        reportRequest.report.exportoptions.footerreport.exportoptions.flatfile[reqReportColumn.reportcolumnid])));
                                                        }

                                                        break;
                                                    default:
                                                        cEventlog.LogEntry(
                                                            string.Format(
                                                                "Reports : cExports : ExportFlatFile : Unexpected standard column type '{0}' encountered for footer report ID {1}. Main report ID is {2}",
                                                                standard.field.FieldType,
                                                                reportRequest.report.exportoptions.footerreport.reportid,
                                                                reportRequest.report.reportid));
                                                        break;
                                                }

                                                col++;
                                            }

                                            break;

                                        case ReportColumnType.Static:
                                            output.Append(dataReport.Tables[0].Rows[i][x]);
                                            col++;
                                            break;

                                        case ReportColumnType.Calculated:
                                            calculatedcol = (cCalculatedColumn)column;
                                            string formula = calculatedcol.formula;
                                            this.calculateFormula(ref formula, dataReport, row, dataSetCol, false);

                                            UltraCalcValue val =
                                                calcman.Calculate(
                                                    this.convertExportFormula(
                                                        this.request.report.exportoptions.footerreport.columns,
                                                        formula,
                                                        dataReport.Tables[0].Rows[i],
                                                        Aggregate.None,
                                                        ref lstColumnIndexes,
                                                        ref lstRegex,
                                                        calculatedcol.reportcolumnid));

                                            output.Append(val);
                                            col++;
                                            break;
                                    }
                                }

                                dataSetCol++;
                            }

                            output.Append("\r\n");
                            row++;
                        }
                    }
                }
                else
                {
                    // Static SQL Report export
                    row = 1;
                    col = 1;

                    foreach (DataColumn dataColumn in dataReport.Tables[0].Columns)
                    {
                        output.Append(addTrailingSpaces(dataColumn.ColumnName, 50));
                        col++;
                    }

                    row++;

                    foreach (DataRow dataRow in dataReport.Tables[0].Rows)
                    {
                        col = 1;

                        foreach (object column in dataRow.ItemArray)
                        {
                            if (column != DBNull.Value)
                            {
                                switch (column.GetType().ToString())
                                {
                                    case "System.String":
                                        output.Append(addTrailingSpaces(column.ToString(), 50));
                                        break;
                                    case "System.DateTime":
                                        output.Append(addTrailingSpaces(((DateTime)column).ToShortDateString(), 50));
                                        break;
                                    case "System.Int16":
                                    case "System.Int32":
                                    case "System.Int64":
                                    case "System.Double":
                                        output.Append(addLeadingZeroes(column.ToString(), 50));
                                        break;
                                    case "System.Boolean":
                                        if ((bool)column)
                                        {
                                            output.Append(addTrailingSpaces("True", 50));
                                        }
                                        else
                                        {
                                            output.Append(addTrailingSpaces("False", 50));
                                        }

                                        break;
                                    default:
                                        output.Append(addTrailingSpaces(column.ToString(), 50));
                                        break;
                                }
                            }
                            else
                            {
                                switch (column.GetType().ToString())
                                {
                                    case "System.Int16":
                                    case "System.Int32":
                                    case "System.Int64":
                                    case "System.Double":
                                        output.Append(addLeadingZeroes("0", 50));
                                        break;
                                    default:
                                        output.Append(addTrailingSpaces(string.Empty, 50));
                                        break;
                                }
                            }

                            col++;
                        }
                        output.Append(Environment.NewLine);
                        row++;
                        reportRequest.ProcessedRows++;
                    }
                }

                Encoding unicode = Encoding.UTF8;

                file = unicode.GetBytes(output.ToString());

                // set the status of the export to exported
                if (historyCreated)
                {
                    this.UpdateHistoryExported(reportRequest);
                }
            }
            catch (Exception ex)
            {
                reportRequest.Exception = ex;
                if (reportRequest.report.exportoptions.exporthistoryid != 0 && historyCreated)
                {
                    this.DeleteHistory(reportRequest);
                }

                string logMessage = "An error occurred exporting to Flat file.";
                if (ex.Message == "SQL Timeout")
                {
                    logMessage = "Timeout on dataset retrieval.";
                }

                cEventlog.LogEntry(
                    string.Format(
                        "Reports : cExports : exportFlatfile : {2} : {0}\n{1}", ex.Message, ex.StackTrace, logMessage));
                throw new Exception(logMessage, ex);
            }
            finally
            {
                if (calcman != null)
                {
                    calcman.Dispose();
                }

                if (dataReport != null)
                {
                    if (dataReport.Tables != null && dataReport.Tables[0] != null)
                    {
                        dataReport.Tables[0].Clear();
                        dataReport.Tables[0].Dispose();
                    }

                    dataReport.Clear();
                    dataReport.Dispose();
                }
            }

            return file;
        }

        private int CreateExportHistoryEntries(
            cReportRequest reportRequest,
            DataSet dataReport,
            List<int> historyIDs,
            List<int> negativeBalances)
        {
            foreach (DataRow dr in dataReport.Tables[0].Rows)
            {
                if (reportRequest != null && dr[reportRequest.report.exportoptions.PrimaryKeyIndex.ToString(CultureInfo.InvariantCulture)].ToString() == "Restricted")
                {
                    continue;
                }

                var expenseId =
                    (int)dr[reportRequest.report.exportoptions.PrimaryKeyIndex.ToString(CultureInfo.InvariantCulture)];
                if (historyIDs.Contains(expenseId) == false)
                {
                    if (negativeBalances.Contains(expenseId) == false)
                    {
                        historyIDs.Add(expenseId);
                    }
                }
            }

            int exporthistoryid = this.CreateHistory(historyIDs, reportRequest);
            reportRequest.report.exportoptions.exporthistoryid = exporthistoryid;
            if (reportRequest.report.exportoptions.footerreport != null)
            {
                reportRequest.report.exportoptions.footerreport.exportoptions.exporthistoryid = exporthistoryid;
            }
            return exporthistoryid;
        }

        /// <summary>
        /// The export to pivot table function.
        /// </summary>
        /// <param name="reportRequest">
        /// The req.
        /// </param>
        /// <returns>
        /// The pivot table export as a byte array.
        /// </returns>
        /// <exception cref="Exception">
        /// </exception>
        public byte[] exportPivot(cReportRequest reportRequest)
        {
            this.request = reportRequest;

            // current user only used for export relationship fields for CEs & UDFs
            ICurrentUser currentUser = new CurrentUser(
                this.request.accountid, this.request.employeeid, -1, Modules.SpendManagement, this.request.SubAccountId);
            byte[] file;
            DataSet dataReport = null;
            var calcman = CreateCalcManager();
            var exceleng = new ExcelEngine { ThrowNotSavedOnDestroy = false };

            try
            {
                var clsGlobalCurrencies = new cGlobalCurrencies();
                var clsCurrencies = new cCurrencies(this.request.accountid, this.request.SubAccountId);
                var clsreports = new cReports(this.request.accountid);

                dataReport = clsreports.createReport(this.request, new cFields(this.request.accountid), new cJoins(this.request.accountid), new UltraWebGrid(), calcman);

                if (dataReport.Tables.Count == 0)
                {
                    var ex = new Exception("SQL Timeout");
                    this.request.Status = ReportRequestStatus.Failed;
                    this.request.Exception = ex;
                    throw ex;
                }

                this.request.RowCount = dataReport.Tables[0].Rows.Count;

                if (this.request.RowCount > 65536)
                {
                    var ex = new Exception("Row count is > 65536");
                    this.request.Status = ReportRequestStatus.Failed;
                    this.request.Exception = ex;
                    throw ex;
                }

                if (dataReport.Tables[0].Rows.Count == 0)
                {
                    dataReport.Dispose();
                    return new byte[0];
                }

                this.request.RowCount = dataReport.Tables[0].Rows.Count;

                exceleng.ThrowNotSavedOnDestroy = false;

                IWorkbook workbook = ExcelUtils.CreateWorkBookUsingTemplate(this.getPath());
                IWorksheet worksheet = workbook.Worksheets[1];

                workbook.Activate();

                var lstColumnIndexes = new Dictionary<string, int>();
                var lstRegex = new Dictionary<Guid, MatchCollection>();
                int col = 1;
                int row = 1;

                if (string.IsNullOrEmpty(this.request.report.StaticReportSQL))
                {
                    cStandardColumn standard;
                    cCalculatedColumn calculatedcol;
                    foreach (cReportColumn headerColumn in this.request.report.columns)
                    {
                        if (headerColumn.system)
                        {
                            continue;
                        }

                        switch (headerColumn.columntype)
                        {
                            case ReportColumnType.Standard:
                                standard = (cStandardColumn)headerColumn;

                                string fieldDescription = this._relabeler.Relabel(standard.field).Description;

                                if (standard.funcavg || standard.funccount || standard.funcmax || standard.funcmin
                                    || standard.funcsum)
                                {
                                    if (standard.funcsum)
                                    {
                                        worksheet.Range[row, col].Text = "SUM of " + fieldDescription;
                                        col++;
                                    }

                                    if (standard.funcavg)
                                    {
                                        worksheet.Range[row, col].Text = "AVG of " + fieldDescription;
                                        col++;
                                    }

                                    if (standard.funccount)
                                    {
                                        worksheet.Range[row, col].Text = "COUNT of " + fieldDescription;
                                        col++;
                                    }

                                    if (standard.funcmax)
                                    {
                                        worksheet.Range[row, col].Text = "MAX of " + fieldDescription;
                                        col++;
                                    }

                                    if (standard.funcmin)
                                    {
                                        worksheet.Range[row, col].Text = "MIN of " + fieldDescription;
                                        col++;
                                    }
                                }
                                else
                                {
                                    worksheet.Range[row, col].Text = fieldDescription;
                                    col++;
                                }

                                break;
                            case ReportColumnType.Static:
                                var staticcol = (cStaticColumn)headerColumn;
                                worksheet.Range[row, col].Text = staticcol.literalname;
                                col++;
                                break;
                            case ReportColumnType.Calculated:
                                calculatedcol = (cCalculatedColumn)headerColumn;
                                worksheet.Range[row, col].Text = calculatedcol.columnname;
                                col++;
                                break;
                        }
                    }

                    row++;

                    for (int i = 0; i < dataReport.Tables[0].Rows.Count; i++)
                    {
                        col = 1;
                        int dataSetCol = 1;

                        for (int x = 0; x < this.request.report.columns.Count; x++)
                        {
                            var column = (cReportColumn)this.request.report.columns[x];
                            if (column.system == false)
                            {
                                switch (column.columntype)
                                {
                                    case ReportColumnType.Standard:
                                        standard = (cStandardColumn)column;
                                        DateTime date;
                                        if (standard.funcavg || standard.funccount || standard.funcmax || standard.funcmin
                                            || standard.funcsum)
                                        {
                                            if (standard.funcsum)
                                            {
                                                switch (standard.field.FieldType)
                                                {
                                                    case "M":
                                                    case "FD":
                                                    case "F":
                                                        if (dataReport.Tables[0].Rows[i][x] != DBNull.Value)
                                                        {
                                                            worksheet.Range[row, col].Value =
                                                                dataReport.Tables[0].Rows[i][x].ToString();
                                                            worksheet.Range[row, col].NumberFormat = "#,##0.00;-#,##0.00";
                                                        }

                                                        break;
                                                    case "C":
                                                    case "FC":
                                                    case "A":
                                                        if (dataReport.Tables[0].Rows[i][x] != DBNull.Value)
                                                        {
                                                            worksheet.Range[row, col].Value =
                                                                dataReport.Tables[0].Rows[i][x].ToString();
                                                            worksheet.Range[row, col].NumberFormat = "£#,##0.00;-£#,##0.00";
                                                        }

                                                        break;
                                                    case "N":
                                                    case "I":
                                                        if (dataReport.Tables[0].Rows[i][x] != DBNull.Value)
                                                        {
                                                            worksheet.Range[row, col].Value =
                                                                dataReport.Tables[0].Rows[i][x].ToString();
                                                            worksheet.Range[row, col].NumberFormat = "###000;-###000";
                                                        }

                                                        break;
                                                }

                                                col++;
                                            }

                                            if (standard.funcavg)
                                            {
                                                switch (standard.field.FieldType)
                                                {
                                                    case "M":
                                                    case "FD":
                                                    case "F":
                                                        if (dataReport.Tables[0].Rows[i][x] != DBNull.Value)
                                                        {
                                                            worksheet.Range[row, col].Value =
                                                                dataReport.Tables[0].Rows[i][x].ToString();
                                                            worksheet.Range[row, col].NumberFormat = "#,##0.00;-#,##0.00";
                                                        }

                                                        break;
                                                    case "C":
                                                    case "FC":
                                                    case "A":
                                                        if (dataReport.Tables[0].Rows[i][x] != DBNull.Value)
                                                        {
                                                            worksheet.Range[row, col].Value =
                                                                dataReport.Tables[0].Rows[i][x].ToString();
                                                            worksheet.Range[row, col].NumberFormat = "£#,##0.00;-£#,##0.00";
                                                        }

                                                        break;
                                                    case "N":
                                                    case "I":
                                                        if (dataReport.Tables[0].Rows[i][x] != DBNull.Value)
                                                        {
                                                            worksheet.Range[row, col].Value =
                                                                dataReport.Tables[0].Rows[i][x].ToString();
                                                            worksheet.Range[row, col].NumberFormat = "###000;-###000";
                                                        }

                                                        break;
                                                }

                                                col++;
                                            }

                                            if (standard.funccount)
                                            {
                                                worksheet.Range[row, col].Value =
                                                    dataReport.Tables[0].Rows[i][x].ToString();
                                                worksheet.Range[row, col].NumberFormat = "###000;-###000";
                                                col++;
                                            }

                                            if (standard.funcmax)
                                            {
                                                switch (standard.field.FieldType)
                                                {
                                                    case "M":
                                                    case "FD":
                                                    case "F":
                                                        if (dataReport.Tables[0].Rows[i][x] != DBNull.Value)
                                                        {
                                                            worksheet.Range[row, col].Value =
                                                                dataReport.Tables[0].Rows[i][x].ToString();
                                                            worksheet.Range[row, col].NumberFormat = "#,##0.00;-#,##0.00";
                                                        }

                                                        break;
                                                    case "C":
                                                    case "FC":
                                                    case "A":
                                                        if (dataReport.Tables[0].Rows[i][x] != DBNull.Value)
                                                        {
                                                            worksheet.Range[row, col].Value =
                                                                dataReport.Tables[0].Rows[i][x].ToString();
                                                            worksheet.Range[row, col].NumberFormat = "£#,##0.00;-£#,##0.00";
                                                        }

                                                        break;
                                                    case "N":
                                                    case "I":
                                                        if (dataReport.Tables[0].Rows[i][x] != DBNull.Value)
                                                        {
                                                            worksheet.Range[row, col].Value =
                                                                dataReport.Tables[0].Rows[i][x].ToString();
                                                            worksheet.Range[row, col].NumberFormat = "###000;-###000";
                                                        }

                                                        break;
                                                    case "D":
                                                        if (dataReport.Tables[0].Rows[i][x] != DBNull.Value)
                                                        {
                                                            date = (DateTime)dataReport.Tables[0].Rows[i][x];
                                                            worksheet.Range[row, col].Value = date.ToShortDateString();
                                                            worksheet.Range[row, col].NumberFormat = "dd/MM/yyyy";
                                                        }

                                                        break;
                                                    case "DT":
                                                        if (dataReport.Tables[0].Rows[i][x] != DBNull.Value)
                                                        {
                                                            date = (DateTime)dataReport.Tables[0].Rows[i][x];
                                                            worksheet.Range[row, col].DateTime =
                                                                DateTime.Parse(
                                                                    date.Day.ToString("00") + "/"
                                                                    + date.Month.ToString("00") + "/"
                                                                    + date.Year.ToString("0000") + " "
                                                                    + date.Hour.ToString("00") + ":"
                                                                    + date.Minute.ToString("00") + ":"
                                                                    + date.Second.ToString("00"));
                                                            worksheet.Range[row, col].NumberFormat = "dd/MM/yyyy HH:mm:ss";
                                                        }

                                                        break;
                                                    case "T":
                                                        if (dataReport.Tables[0].Rows[i][x] != DBNull.Value)
                                                        {
                                                            date = (DateTime)dataReport.Tables[0].Rows[i][x];
                                                            worksheet.Range[row, col].Value = date.Hour.ToString("00") + ":"
                                                                                              + date.Minute.ToString("00");
                                                            worksheet.Range[row, col].NumberFormat = "HH:mm";
                                                        }

                                                        break;
                                                }

                                                col++;
                                            }

                                            if (standard.funcmin)
                                            {
                                                switch (standard.field.FieldType)
                                                {
                                                    case "M":
                                                    case "FD":
                                                    case "F":
                                                        if (dataReport.Tables[0].Rows[i][x] != DBNull.Value)
                                                        {
                                                            worksheet.Range[row, col].Value =
                                                                dataReport.Tables[0].Rows[i][x].ToString();
                                                            worksheet.Range[row, col].NumberFormat = "#,##0.00;-#,##0.00";
                                                        }

                                                        break;
                                                    case "C":
                                                    case "FC":
                                                    case "A":
                                                        if (dataReport.Tables[0].Rows[i][x] != DBNull.Value)
                                                        {
                                                            worksheet.Range[row, col].Value =
                                                                dataReport.Tables[0].Rows[i][x].ToString();
                                                            worksheet.Range[row, col].NumberFormat = "£#,##0.00;-£#,##0.00";
                                                        }

                                                        break;
                                                    case "N":
                                                    case "I":
                                                        if (dataReport.Tables[0].Rows[i][x] != DBNull.Value)
                                                        {
                                                            worksheet.Range[row, col].Value =
                                                                dataReport.Tables[0].Rows[i][x].ToString();
                                                            worksheet.Range[row, col].NumberFormat = "###000;-###000";
                                                        }

                                                        break;
                                                    case "D":
                                                        if (dataReport.Tables[0].Rows[i][x] != DBNull.Value)
                                                        {
                                                            date = (DateTime)dataReport.Tables[0].Rows[i][x];
                                                            worksheet.Range[row, col].Value = date.ToShortDateString();
                                                            worksheet.Range[row, col].NumberFormat = "dd/MM/yyyy";
                                                        }

                                                        break;
                                                    case "DT":
                                                        if (dataReport.Tables[0].Rows[i][x] != DBNull.Value)
                                                        {
                                                            date = (DateTime)dataReport.Tables[0].Rows[i][x];
                                                            worksheet.Range[row, col].DateTime =
                                                                DateTime.Parse(
                                                                    date.Day.ToString("00") + "/"
                                                                    + date.Month.ToString("00") + "/"
                                                                    + date.Year.ToString("0000") + " "
                                                                    + date.Hour.ToString("00") + ":"
                                                                    + date.Minute.ToString("00") + ":"
                                                                    + date.Second.ToString("00"));
                                                            worksheet.Range[row, col].NumberFormat = "dd/MM/yyyy HH:mm:ss";
                                                        }

                                                        break;
                                                    case "T":
                                                        if (dataReport.Tables[0].Rows[i][x] != DBNull.Value)
                                                        {
                                                            date = (DateTime)dataReport.Tables[0].Rows[i][x];
                                                            worksheet.Range[row, col].Value = date.Hour.ToString("00") + ":"
                                                                                              + date.Minute.ToString("00");
                                                            worksheet.Range[row, col].NumberFormat = "HH:mm";
                                                        }

                                                        break;
                                                }

                                                col++;
                                            }
                                        }
                                        else
                                        {
                                            switch (standard.field.FieldType)
                                            {
                                                case "S":
                                                case "FS":
                                                case "LT":
                                                case "R":
                                                    if (dataReport.Tables[0].Rows[i][x] != DBNull.Value)
                                                    {
                                                        worksheet.Range[row, col].Text =
                                                            (string)dataReport.Tables[0].Rows[i][x];
                                                    }

                                                    break;
                                                case "D":
                                                    if (dataReport.Tables[0].Rows[i][x] != DBNull.Value)
                                                    {
                                                        date = (DateTime)dataReport.Tables[0].Rows[i][x];
                                                        worksheet.Range[row, col].Value = date.ToShortDateString();
                                                        worksheet.Range[row, col].NumberFormat = "dd/MM/yyyy";
                                                    }

                                                    break;
                                                case "DT":
                                                    if (dataReport.Tables[0].Rows[i][x] != DBNull.Value)
                                                    {
                                                        date = (DateTime)dataReport.Tables[0].Rows[i][x];
                                                        worksheet.Range[row, col].DateTime =
                                                            DateTime.Parse(
                                                                date.Day.ToString("00") + "/" + date.Month.ToString("00")
                                                                + "/" + date.Year.ToString("0000") + " "
                                                                + date.Hour.ToString("00") + ":"
                                                                + date.Minute.ToString("00") + ":"
                                                                + date.Second.ToString("00"));
                                                        worksheet.Range[row, col].NumberFormat = "dd/MM/yyyy HH:mm:ss";
                                                    }

                                                    break;
                                                case "Y":
                                                    if (dataReport.Tables[0].Rows[i][x] != DBNull.Value)
                                                    {
                                                        worksheet.Range[row, col].Text =
                                                            (string)dataReport.Tables[0].Rows[i][x];
                                                    }

                                                    break;
                                                case "T":
                                                    if (dataReport.Tables[0].Rows[i][x] != DBNull.Value)
                                                    {
                                                        date = (DateTime)dataReport.Tables[0].Rows[i][x];
                                                        worksheet.Range[row, col].Value = date.Hour.ToString("00") + ":"
                                                                                          + date.Minute.ToString("00");
                                                        worksheet.Range[row, col].NumberFormat = "HH:mm";
                                                    }

                                                    break;
                                                case "X":
                                                    if (dataReport.Tables[0].Rows[i][x] != DBNull.Value)
                                                    {
                                                        worksheet.Range[row, col].Value =
                                                            dataReport.Tables[0].Rows[i][x].ToString();
                                                    }

                                                    break;
                                                case "M":
                                                case "FD":
                                                case "F":
                                                    if (dataReport.Tables[0].Rows[i][x] != DBNull.Value)
                                                    {
                                                        worksheet.Range[row, col].Value2 =
                                                            dataReport.Tables[0].Rows[i][x].ToString();
                                                        worksheet.Range[row, col].NumberFormat = "#,##0.00;-#,##0.00";
                                                    }

                                                    break;
                                                case "C":
                                                case "FC":
                                                case "A":
                                                    if (dataReport.Tables[0].Rows[i][x] != DBNull.Value)
                                                    {
                                                        worksheet.Range[row, col].Value2 =
                                                            dataReport.Tables[0].Rows[i][x].ToString();
                                                        worksheet.Range[row, col].NumberFormat = "£#,##0.00;-£#,##0.00";
                                                    }

                                                    break;
                                                case "N":
                                                case "I":
                                                    if (dataReport.Tables[0].Rows[i][x] != DBNull.Value)
                                                    {
                                                        // check that not relationship field. If so, retrieve text value
                                                        if (standard.field.FieldSource != cField.FieldSourceType.Metabase
                                                            && (standard.field.IsForeignKey
                                                                && standard.field.RelatedTableID != Guid.Empty))
                                                        {
                                                            worksheet.Range[row, col].Text =
                                                                Spend_Management.cReports.getRelationshipValueText(
                                                                    currentUser,
                                                                    standard.field.FieldID,
                                                                    dataReport.Tables[0].Rows[i][x]);
                                                        }
                                                        else
                                                        {
                                                            worksheet.Range[row, col].Value2 =
                                                                dataReport.Tables[0].Rows[i][x].ToString();
                                                            worksheet.Range[row, col].NumberFormat = "#,##0.00;-#,##0.00";
                                                        }
                                                    }

                                                    break;
                                                case "CL":
                                                    if (dataReport.Tables[0].Rows[i][x] != DBNull.Value)
                                                    {
                                                        cCurrency currency =
                                                            clsCurrencies.getCurrencyById(
                                                                (int)dataReport.Tables[0].Rows[i][x]);
                                                        worksheet.Range[row, col].Text = currency == null
                                                                                             ? string.Empty
                                                                                             : clsGlobalCurrencies
                                                                                                   .getGlobalCurrencyById(
                                                                                                       currency
                                                                                                   .globalcurrencyid).label;
                                                    }

                                                    break;
                                            }

                                            col++;
                                        }

                                        break;
                                    case ReportColumnType.Static:
                                        if (dataReport.Tables[0].Rows[i][x].ToString().Substring(0, 1) == "=")
                                        {
                                            worksheet.Range[row, col].Formula =
                                                dataReport.Tables[0].Rows[i][x].ToString()
                                                                                .Substring(
                                                                                    1,
                                                                                    dataReport.Tables[0].Rows[i][x].ToString().Length - 1)
                                                                                .ToUpper();
                                        }
                                        else
                                        {
                                            worksheet.Range[row, col].Value = dataReport.Tables[0].Rows[i][x].ToString();
                                        }

                                        col++;
                                        break;
                                    case ReportColumnType.Calculated:
                                        calculatedcol = (cCalculatedColumn)column;
                                        string formula = calculatedcol.formula;
                                        this.calculateFormula(ref formula, dataReport, row, dataSetCol, false);
                                        UltraCalcValue val;
                                        try
                                        {
                                            val =
                                                calcman.Calculate(
                                                    this.convertExportFormula(
                                                        this.request.report.columns,
                                                        formula,
                                                        dataReport.Tables[0].Rows[i],
                                                        Aggregate.None,
                                                        ref lstColumnIndexes,
                                                        ref lstRegex,
                                                        calculatedcol.reportcolumnid));
                                        }
                                        catch (Exception ex)
                                        {
                                            throw new Exception(
                                                "Calculated column failed to export (as Pivot Table)\n\nReport Column: "
                                                + calculatedcol.columnname + "(no. " + col + ")\n\nReport Row: " + row
                                                + "\n\nFormula: " + calculatedcol.formula
                                                + "\n\n\nInfragistics Message: \n\n" + ex.Message);
                                        }

                                        this.parseCalculatedValueForIncrementalNumber(
                                            val.ToString(CultureInfo.InvariantCulture), dataReport, row, dataSetCol, true);

                                        // MW - changed to TRUE as pivot always has headers and errors if calc columns present
                                        worksheet.Range[row, col].Value = val.ToString();

                                        col++;
                                        break;
                                }
                            }

                            dataSetCol++;
                        }

                        row++;
                        reportRequest.ProcessedRows++;
                    }
                }
                else
                {
                    // Static SQL Report export
                    row = 1;
                    col = 1;
                    foreach (DataColumn dataColumn in dataReport.Tables[0].Columns)
                    {
                        worksheet.Range[row, col].Text = dataColumn.ColumnName;
                        col++;
                    }

                    row++;

                    foreach (DataRow dataRow in dataReport.Tables[0].Rows)
                    {
                        col = 1;

                        foreach (object column in dataRow.ItemArray)
                        {
                            if (column != DBNull.Value)
                            {
                                switch (column.GetType().ToString())
                                {
                                    case "System.String":
                                        worksheet.Range[row, col].Text = column.ToString();
                                        break;
                                    case "System.DateTime":
                                        worksheet.Range[row, col].DateTime = (DateTime)column;
                                        break;
                                    case "System.Int16":
                                    case "System.Int32":
                                    case "System.Double":
                                        worksheet[row, col].Value = column.ToString();
                                        break;
                                    case "System.Boolean":
                                        if ((bool)column)
                                        {
                                            worksheet[row, col].Text = "True";
                                        }
                                        else
                                        {
                                            worksheet[row, col].Text = "False";
                                        }

                                        break;
                                    default:
                                        worksheet.Range[row, col].Text = column.ToString();
                                        break;
                                }
                            }

                            col++;
                        }

                        row++;
                        reportRequest.ProcessedRows++;
                    }
                }

                workbook.Names["PivotTable"].RefersToRange = worksheet.Range[1, 1, row - 1, col - 1];

                using (Stream stream = new MemoryStream())
                {
                    workbook.SaveAs(stream, ExcelSaveType.SaveAsXLS);
                    workbook.Close();
                    exceleng.Dispose();

                    // convert stream to bytes;
                    file = new byte[stream.Length];
                    stream.Seek(0, SeekOrigin.Begin);

                    using (var reader = new BinaryReader(stream))
                    {
                        reader.Read(file, 0, (int)stream.Length);
                        reader.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                reportRequest.Exception = ex;
                string logMessage = "An error occurred exporting to pivot table.";
                if (ex.Message == "SQL Timeout")
                {
                    logMessage = "Timeout on dataset retrieval.";
                }

                if (ex.Message == "Row count is > 65536")
                {
                    logMessage = "Export to pivot cancelled as report generates more than 65536 rows.";
                }

                cEventlog.LogEntry(
    string.Format(
        "Reports : cExports : exportPivot : {2} : {0}\n{1}", ex.Message, ex.StackTrace, logMessage));
                throw new Exception(logMessage, ex);
            }
            finally
            {
                if (exceleng != null)
                {
                    exceleng.Dispose();
                }

                if (calcman != null)
                {
                    calcman.Dispose();
                }

                if (dataReport != null)
                {
                    if (dataReport.Tables != null && dataReport.Tables[0] != null)
                    {
                        dataReport.Tables[0].Clear();
                        dataReport.Tables[0].Dispose();
                    }

                    dataReport.Clear();
                    dataReport.Dispose();
                }
            }

            return file;
        }

        /// <summary>
        /// Returns the company details in the sCompanyDetails structure.
        /// </summary>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <returns>
        /// The <see cref="sCompanyDetails"/>.
        /// </returns>
        public sCompanyDetails getCompanyDetails(int accountid)
        {
            var expdata = new DBConnection(cReportsSvc.GetConnectionString(accountid));
            var temp = new sCompanyDetails();
            string strsql;
            SqlDataReader compreader;

            strsql = "select * from dbo.companydetails";
            compreader = expdata.GetReader(strsql);
            while (compreader.Read())
            {
                if (compreader.IsDBNull(compreader.GetOrdinal("companyname")) == false)
                {
                    temp.companyname = compreader.GetString(compreader.GetOrdinal("companyname"));
                }

                if (compreader.IsDBNull(compreader.GetOrdinal("address1")) == false)
                {
                    temp.address1 = compreader.GetString(compreader.GetOrdinal("address1"));
                }

                if (compreader.IsDBNull(compreader.GetOrdinal("address2")) == false)
                {
                    temp.address2 = compreader.GetString(compreader.GetOrdinal("address2"));
                }

                if (compreader.IsDBNull(compreader.GetOrdinal("city")) == false)
                {
                    temp.city = compreader.GetString(compreader.GetOrdinal("city"));
                }

                if (compreader.IsDBNull(compreader.GetOrdinal("county")) == false)
                {
                    temp.county = compreader.GetString(compreader.GetOrdinal("county"));
                }

                if (compreader.IsDBNull(compreader.GetOrdinal("postcode")) == false)
                {
                    temp.postcode = compreader.GetString(compreader.GetOrdinal("postcode"));
                }

                if (compreader.IsDBNull(compreader.GetOrdinal("telno")) == false)
                {
                    temp.telno = compreader.GetString(compreader.GetOrdinal("telno"));
                }

                if (compreader.IsDBNull(compreader.GetOrdinal("faxno")) == false)
                {
                    temp.faxno = compreader.GetString(compreader.GetOrdinal("faxno"));
                }

                if (compreader.IsDBNull(compreader.GetOrdinal("email")) == false)
                {
                    temp.email = compreader.GetString(compreader.GetOrdinal("email"));
                }

                if (compreader.IsDBNull(compreader.GetOrdinal("companynumber")) == false)
                {
                    temp.companynumber = compreader.GetString(compreader.GetOrdinal("companynumber"));
                }
            }

            compreader.Close();
            strsql = "select * from [company_bankdetails]";
            compreader = expdata.GetReader(strsql);
            while (compreader.Read())
            {
                if (compreader.IsDBNull(compreader.GetOrdinal("bankreference")) == false)
                {
                    temp.bankref = compreader.GetString(compreader.GetOrdinal("bankreference"));
                }

                if (compreader.IsDBNull(compreader.GetOrdinal("accountnumber")) == false)
                {
                    temp.accoutno = compreader.GetString(compreader.GetOrdinal("accountnumber"));
                }

                if (compreader.IsDBNull(compreader.GetOrdinal("accounttype")) == false)
                {
                    temp.accounttype = compreader.GetString(compreader.GetOrdinal("accounttype"));
                }

                if (compreader.IsDBNull(compreader.GetOrdinal("sortcode")) == false)
                {
                    temp.sortcode = compreader.GetString(compreader.GetOrdinal("sortcode"));
                }
            }

            compreader.Close();
            expdata.sqlexecute.Parameters.Clear();
            return temp;
        }

        /// <summary>
        /// Version of getExpenseIds for use with custom entity based financial exports
        /// </summary>
        /// <param name="reportRequest">
        /// request object
        /// </param>
        /// <returns>
        /// An array list of greenlight id's.
        /// </returns>
        public ArrayList getCustomEntityIds(cReportRequest reportRequest)
        {
            var ids = new ArrayList();

            using (var connection = new DatabaseConnection(cReportsSvc.GetConnectionString(reportRequest.accountid)))
            {
                var clsreports = new cReports(reportRequest.accountid);
                var clsjoins = new cJoins(reportRequest.accountid);
                var clsfields = new cFields(reportRequest.accountid);

                var lstFields = new List<cField>();

                foreach (cReportColumn column in reportRequest.report.columns)
                {
                    if (column.GetType() == typeof(cStandardColumn))
                    {
                        if (!lstFields.Contains(((cStandardColumn)column).field))
                        {
                            lstFields.Add(((cStandardColumn)column).field);
                        }
                    }
                }

                foreach (cReportCriterion criteria in reportRequest.report.criteria)
                {
                    if (!lstFields.Contains(criteria.field))
                    {
                        lstFields.Add(criteria.field);
                    }
                }

                string joinsql = clsjoins.createReportJoinSQL(lstFields, this.request.report.basetable.TableID, this.request.AccessLevel);
                string strsql = string.Format("SELECT {0}.{1} FROM {0} {2}{3}{4}", this.request.report.basetable.TableName, this.request.report.basetable.GetPrimaryKey().FieldName, joinsql, reportRequest.report.GenerateExportHistoryJoin(), reportRequest.report.generateCriteria());

                clsreports.setCriteriaParameters(reportRequest, connection);
                using (IDataReader reader = connection.GetReader(strsql))
                {
                    connection.sqlexecute.Parameters.Clear();
                    while (reader.Read())
                    {
                        ids.Add(reader.GetInt32(0));
                    }

                    reader.Close();
                }
            }

            return ids;
        }

        /// <summary>
        /// Gets the expense id's in a report request.
        /// </summary>
        /// <param name="reportRequest">
        /// The request object.
        /// </param>
        /// <returns>
        /// An array list of expense id's for the report request
        /// </returns>
        public ArrayList getExpenseIds(cReportRequest reportRequest)
        {
            var clsreports = new cReports(reportRequest.accountid);
            var ids = new ArrayList();
            var clsjoins = new cJoins(reportRequest.accountid);
            var clsfields = new cFields(reportRequest.accountid);
            var lstFields = new List<cField>();

            foreach (cReportColumn column in reportRequest.report.columns)
            {
                if (column.GetType() == typeof(cStandardColumn))
                {
                    if (!lstFields.Contains(((cStandardColumn)column).field))
                    {
                        lstFields.Add(((cStandardColumn)column).field);
                    }
                }
            }

            foreach (cReportCriterion criteria in reportRequest.report.criteria)
            {
                if (!lstFields.Contains(criteria.field))
                {
                    lstFields.Add(criteria.field);
                }
            }

            string joinsql = clsjoins.createReportJoinSQL(lstFields, this.request.report.basetable.TableID, this.request.AccessLevel);
            string strsql = string.Format("select savedexpenses.expenseid from dbo.savedexpenses {0}{1}{2}", joinsql, reportRequest.report.GenerateExportHistoryJoin(), reportRequest.report.generateCriteria());

            using (var connection = new DatabaseConnection(cReportsSvc.GetConnectionString(reportRequest.accountid)))
            {
                clsreports.setCriteriaParameters(reportRequest, connection);
                using (IDataReader reader = connection.GetReader(strsql))
                {
                    connection.sqlexecute.Parameters.Clear();
                    while (reader.Read())
                    {
                        ids.Add(reader.GetInt32(0));
                    }

                    reader.Close();
                }
            }

            return ids;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Returns the string with the specified number of leading zeros to make the string the same size as the wanted field length.
        /// </summary>
        /// <param name="strval">
        /// The string to add the zeros to.
        /// </param>
        /// <param name="fieldlength">
        /// The length the field should be.
        /// </param>
        /// <returns>
        /// The string maxed up to the fieldlength with preceding zeros.
        /// </returns>
        private static string addLeadingZeroes(string strval, int fieldlength)
        {
            if (strval.Length > fieldlength || fieldlength == 0)
            {
                return strval;
            }

            var newval = new StringBuilder();
            newval.Append(strval);
            while (newval.Length != fieldlength)
            {
                newval.Insert(0, "0");
            }

            return newval.ToString();
        }

        /// <summary>
        /// Adds trailing spaces to a string to make it up to the required field length.
        /// </summary>
        /// <param name="strval">
        /// The strval.
        /// </param>
        /// <param name="fieldlength">
        /// The desired field length.
        /// </param>
        /// <returns>
        /// The string made up to the size of fieldlength with trailing spaces.
        /// </returns>
        private static string addTrailingSpaces(string strval, int fieldlength)
        {
            if (strval.Length > fieldlength || fieldlength == 0)
            {
                return strval;
            }

            var newval = new StringBuilder();
            newval.Append(strval);

            // add trailing spaces if necessary
            while (newval.Length != fieldlength)
            {
                newval.Append(" ");
            }

            return newval.ToString();
        }

        /// <summary>
        /// Creates a history item in the database.
        /// </summary>
        /// <param name="historyIDs">
        /// The history id's.
        /// </param>
        /// <param name="reportRequest">
        /// The report Request.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        private int CreateHistory(List<int> historyIDs, cReportRequest reportRequest)
        {
            int exporthistoryid = 0;

            if (historyIDs.Count == 0)
            {
                return 0;
            }

            int exportType = 1;
            if (reportRequest.report.basetable.TableName.Length >= 7 && reportRequest.report.basetable.GetPrimaryKey().FieldName.Length >= 3
                && reportRequest.report.basetable.TableName.Substring(0, 7) == "custom_"
                && reportRequest.report.basetable.GetPrimaryKey().FieldName.Substring(0, 3) == "att")
            {
                exportType = 2;
            }

            using (var connection = new DatabaseConnection(cReportsSvc.GetConnectionString(reportRequest.accountid)))
            {
                connection.AddWithValue("@historyIDs", historyIDs);

                // insert history record
                connection.AddWithValue("@financialExportID", reportRequest.report.exportoptions.financialexport.financialexportid);
                connection.AddWithValue("@exportNum", reportRequest.report.exportoptions.financialexport.curexportnum);

                if (reportRequest.employeeid > 0)
                {
                    connection.AddWithValue("@employeeID", reportRequest.employeeid);
                }

                connection.AddWithValue("@dateExported", string.Format("{0}/{1}/{2} {3}:{4}:{5}", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second));
                connection.AddWithValue("@exportType", exportType);
                connection.sqlexecute.Parameters.Add("@exportHistoryID", SqlDbType.Int);
                connection.sqlexecute.Parameters["@exportHistoryID"].Direction = ParameterDirection.ReturnValue;
                connection.ExecuteProc("SaveFinancialExportHistory");
                exporthistoryid = (int)connection.sqlexecute.Parameters["@exportHistoryID"].Value;
                connection.sqlexecute.Parameters.Clear();
            }

            return exporthistoryid;
        }

        /// <summary>
        /// Deletes a history item from the database.
        /// </summary>
        /// <param name="reportRequest">
        /// The report request.
        /// </param>
        private void DeleteHistory(cReportRequest reportRequest)
        {
            if (reportRequest.report.exportoptions.exporthistoryid == 0)
            {
                return;
            }

            int exportType = 1;
            if (reportRequest.report.basetable.TableName.Length >= 7 && reportRequest.report.basetable.GetPrimaryKey().Length >= 3
                && reportRequest.report.basetable.TableName.Substring(0, 7) == "custom_"
                && reportRequest.report.basetable.GetPrimaryKey().FieldName.Substring(0, 3) == "att")
            {
                exportType = 2;
            }

            using (var connection = new DatabaseConnection(cReportsSvc.GetConnectionString(reportRequest.accountid)))
            {
                // delete history record and items
                connection.sqlexecute.Parameters.AddWithValue(
                    "@exporthistoryid", reportRequest.report.exportoptions.exporthistoryid);
                connection.sqlexecute.Parameters.AddWithValue("@exportType", exportType);
                connection.ExecuteProc("DeleteFinancialExportHistory");
                connection.sqlexecute.Parameters.Clear();
            }
        }

        /// <summary>
        /// Updates the status of the history item in the database.
        /// </summary>
        /// <param name="reportRequest">
        /// The report request.
        /// </param>
        /// <returns>
        /// The success of the function.
        /// </returns>
        private bool UpdateHistoryExported(cReportRequest reportRequest)
        {
            if (reportRequest.report.exportoptions.exporthistoryid == 0)
            {
                return false;
            }

            var status = FinancialExportStatus.None;
            switch (reportRequest.report.exportoptions.financialexport.application)
            {
                case FinancialApplication.ESR:
                    status = FinancialExportStatus.AwaitingESRInboundTransfer;
                    break;
                case FinancialApplication.CustomReport:
                    status = FinancialExportStatus.ExportSucceeded;
                    break;
            }

            using (var connection = new DatabaseConnection(cReportsSvc.GetConnectionString(reportRequest.accountid)))
            {
                // Update the status after the exported items have been added
                connection.sqlexecute.Parameters.AddWithValue(
                    "@exportHistoryID", reportRequest.report.exportoptions.exporthistoryid);
                connection.sqlexecute.Parameters.AddWithValue("@exportStatus", (byte)status);
                connection.ExecuteProc("UpdateFinancialExportHistory");
                connection.sqlexecute.Parameters.Clear();
            }

            return true;
        }

        private string convertExportFormula(
            ArrayList columns,
            string formula,
            DataRow row,
            Aggregate aggregate,
            ref Dictionary<string, int> lstColumnIndexes,
            ref Dictionary<Guid, MatchCollection> lstMatches,
            Guid columnId)
        {
            MatchCollection matches;

            if (lstMatches.TryGetValue(columnId, out matches) == false)
            {
                matches = Regex.Matches(formula, @"\[(?<fieldname>.*?)\]");

                lstMatches.Add(columnId, matches);
            }

            if (matches != null)
            {
                foreach (Match match in matches)
                {
                    bool columnValid = false;
                    string column = match.Groups[1].Value;
                    foreach (cReportColumn rptcolumn in columns)
                    {
                        switch (rptcolumn.columntype)
                        {
                            case ReportColumnType.Standard:
                                var standard = (cStandardColumn)rptcolumn;
                                string fieldDescription = standard.field.Description;
                                if (standard.field.ReLabel)
                                {
                                    if (
                                        !FieldLabels.TryGetValue(standard.field.RelabelParam.ToUpper(),
                                            out fieldDescription))
                                    {
                                        fieldDescription = this._relabeler.Relabel(standard.field).Description;
                                        FieldLabels.Add(standard.field.RelabelParam.ToUpper(), fieldDescription);
                                    }
                                }
                                if (fieldDescription.ToLower()
                                    == column.Replace("SUM of ", string.Empty)
                                        .Replace("COUNT of ", string.Empty)
                                        .Replace("AVG of ", string.Empty)
                                        .Replace("MAX of ", string.Empty)
                                        .Replace("MIN of ", string.Empty).ToLower())
                                {
                                    column = match.Groups[0].Value;
                                    int index;
                                    if (
                                        lstColumnIndexes.TryGetValue(
                                            standard.reportcolumnid + "," + aggregate, out index) == false)
                                    {
                                        index = this.getColumnIndex(standard, columns, aggregate);
                                        lstColumnIndexes.Add(
                                            standard.reportcolumnid + "," + aggregate, index);
                                    }

                                    string columnvalue;
                                    if (row[index].GetType() == typeof(DateTime))
                                    {
                                        var date = (DateTime)row[index];
                                        columnvalue = string.Format("'{0}'", date.ToString("yyyy/MM/dd"));
                                    }
                                    else
                                    {
                                        string rowString = row[index].ToString();
                                        rowString = rowString.Replace("\"", "'");
                                        columnvalue = "\"" + rowString + "\"";
                                    }

                                    formula = formula.Replace(column, columnvalue);
                                    columnValid = true;
                                }

                                break;
                        }

                        // if we've found a valid column, why continue to look?
                        if (columnValid)
                        {
                            break;
                        }
                    }
                }
            }

            return formula.Replace("a0", " ");
        }

        private void convertToSubQueries(ref cReport rpt)
        {
            foreach (cReportColumn column in rpt.columns)
            {
                if (column.columntype == ReportColumnType.Standard)
                {
                    var standardcol = (cStandardColumn)column;
                    if (standardcol.funcavg || standardcol.funccount || standardcol.funcmax || standardcol.funcmin
                        || standardcol.funcsum)
                    {
                        standardcol.subquery = true;
                    }
                }
            }
        }

        /// <summary>
        /// Alters the string for flat format.
        /// </summary>
        /// <param name="strval">
        /// The string to be altered.
        /// </param>
        /// <param name="length">
        /// The length of the field.
        /// </param>
        /// <returns>
        /// The falt formatted string.
        /// </returns>
        private string createFlatFormatForString(string strval, int length)
        {
            strval = strval.Length > length ? strval.Substring(0, length) : addTrailingSpaces(strval, length);

            return strval;
        }

        private int getAggregateIndex(cStandardColumn column, int baseindex, Aggregate aggregate)
        {
            if (aggregate == Aggregate.SUM)
            {
                return baseindex;
            }

            if (aggregate == Aggregate.AVG)
            {
                if (column.funcsum)
                {
                    baseindex++;
                }

                return baseindex;
            }

            if (aggregate == Aggregate.COUNT)
            {
                if (column.funcsum)
                {
                    baseindex++;
                }

                if (column.funcavg)
                {
                    baseindex++;
                }

                return baseindex;
            }

            if (aggregate == Aggregate.MIN)
            {
                if (column.funcsum)
                {
                    baseindex++;
                }

                if (column.funcavg)
                {
                    baseindex++;
                }

                if (column.funccount)
                {
                    baseindex++;
                }

                return baseindex;
            }

            if (aggregate == Aggregate.MAX)
            {
                if (column.funcsum)
                {
                    baseindex++;
                }

                if (column.funcavg)
                {
                    baseindex++;
                }

                if (column.funccount)
                {
                    baseindex++;
                }

                if (column.funcmin)
                {
                    baseindex++;
                }

                return baseindex;
            }

            return -1;
        }

        private int getColumnIndex(cReportColumn column, ArrayList columns, Aggregate aggregate)
        {
            cStandardColumn standardcol;

            int columnindex = 0;
            int dsindex = columns.IndexOf(column);
            var tempcolumn = (cReportColumn)columns[0];
            while (tempcolumn != column)
            {
                switch (tempcolumn.columntype)
                {
                    case ReportColumnType.Standard:
                        standardcol = (cStandardColumn)tempcolumn;
                        if (standardcol.funcsum || standardcol.funcavg || standardcol.funccount || standardcol.funcmax
                            || standardcol.funcmin)
                        {
                            if (standardcol.funcsum)
                            {
                                dsindex++;
                            }

                            if (standardcol.funcavg)
                            {
                                dsindex++;
                            }

                            if (standardcol.funccount)
                            {
                                dsindex++;
                            }

                            if (standardcol.funcmax)
                            {
                                dsindex++;
                            }

                            if (standardcol.funcmin)
                            {
                                dsindex++;
                            }

                            dsindex--; // remove for just one
                        }

                        break;
                }

                columnindex++;
                tempcolumn = (cReportColumn)columns[columnindex];
            }

            if (aggregate != Aggregate.None)
            {
                standardcol = (cStandardColumn)column;
                dsindex = this.getAggregateIndex(standardcol, dsindex, aggregate);
            }

            return dsindex;
        }

        /// <summary>
        ///     Gets the path of the executable.
        /// </summary>
        /// <returns>
        ///     The path of the executable.
        /// </returns>
        private string getPath()
        {
            Assembly ass = Assembly.GetAssembly(typeof(cReports));
            string location = ass.Location.ToLower().Replace("expenses_reports.exe", string.Empty);
            if (location.Substring(location.Length - 1, 1) != "\\")
            {
                location += "\\";
            }

            location += "Pivot_Table.xlt";

            return location;
        }

        private string getPayrollNumber(int accountid, int id)
        {
            string payroll = string.Empty;
            if (this.payrollnumbers.ContainsKey(id))
            {
                this.payrollnumbers.TryGetValue(id, out payroll);
            }
            else
            {
                const string SQL = "select payroll from dbo.employees where employeeid = @employeeid";
                using (var connection = new DatabaseConnection(cReportsSvc.GetConnectionString(accountid)))
                {
                    connection.sqlexecute.Parameters.AddWithValue("@employeeid", id);
                    using (IDataReader reader = connection.GetReader(SQL))
                    {
                        connection.sqlexecute.Parameters.Clear();
                        while (reader.Read())
                        {
                            payroll = reader.IsDBNull(0) == false ? reader.GetString(0) : string.Empty;

                            this.payrollnumbers.Add(id, payroll);
                        }

                        reader.Close();
                    }
                }
            }

            return payroll;
        }

        private void parseCalculatedValueForIncrementalNumber(
            string val, DataSet ds, int row, int col, bool exportHeader)
        {
            if (val != string.Empty)
            {
                int tempVal = 0;
                string tempStr;
                int startInd;
                startInd = val.IndexOf('"', 1);

                if (startInd > 0)
                {
                    tempStr = val.Substring(1, startInd - 1);

                    // tempStr = tempStr.Replace(@"\", "");
                    if (int.TryParse(tempStr, out tempVal))
                    {
                        if (exportHeader)
                        {
                            if (row == 1)
                            {
                                row++;
                            }

                            ds.Tables[0].Rows[row - 2][col - 1] = tempVal;
                        }
                        else
                        {
                            ds.Tables[0].Rows[row - 1][col - 1] = tempVal;
                        }
                    }
                }
                else
                {
                    tempStr = val;

                    if (int.TryParse(tempStr, out tempVal))
                    {
                        if (exportHeader)
                        {
                            if (row == 1)
                            {
                                row++;
                            }

                            ds.Tables[0].Rows[row - 2][col - 1] = tempVal;
                        }
                        else
                        {
                            ds.Tables[0].Rows[row - 1][col - 1] = tempVal;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Removes carriage returns and line breaks from the given string.
        /// </summary>
        /// <param name="remove">
        /// To remove or not.
        /// </param>
        /// <param name="val">
        /// The string to manipulate.
        /// </param>
        /// <returns>
        /// The original string minus carriage returns and line breaks.
        /// </returns>
        private string removeCarriageReturns(bool remove, string val)
        {
            if (remove)
            {
                val = val.Replace("\r\n", "     ");
                val = val.Replace("\r", "     ");
                val = val.Replace("\n", "     ");
            }

            return val;
        }

        #endregion
    }
}