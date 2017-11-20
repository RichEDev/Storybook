using System.Web;
using Utilities.StringManipulation;

namespace Spend_Management.shared.code
{
    #region Using Directives

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;

    using Infragistics.WebUI.CalcEngine;
    using Infragistics.WebUI.UltraWebCalcManager;

    using SpendManagementLibrary;
    using SpendManagementLibrary.GreenLight;
    using SpendManagementLibrary.DocumentMerge;

    using Syncfusion.DocIO;
    using Syncfusion.DocIO.DLS;
    using Syncfusion.DocToPDFConverter;

    #endregion

    /// <summary>
    /// Document Merge executor class
    /// </summary>
    public class DocumentMergeExecutor
    {
        #region Fields

        private readonly int _accountId;

        private readonly TorchProject torchProject;

        private readonly ICurrentUser _currentUser;

        private readonly TorchMergeState _mergeStatus;

        private readonly int _documentId;

        private readonly cReportCriterion _reportCriterion;

        private readonly UltraWebCalcManager _calcManager;

        private readonly int _torchConfigId;

        private readonly int _projectRecordId;

        private readonly int _entityId;

        private readonly bool _storeDocument;

        #endregion
       
        public TorchGroupingConfiguration TorchGroupingConfiguration { get; set; }

        #region Constructors and Destructors

        /// <summary>
        /// cDocumentMergeExec constructor
        /// </summary>
        /// <param name="accountId">Customer Account ID</param>
        /// <param name="documentId">Document Template ID</param>
        /// <param name="project">Project to perform merge for</param>
        /// <param name="reportCriterion">Additional report criteria to be applied to before each report source is executed</param>
        /// <param name="requestNumber">Merge Request Number used for tracking merge progress</param>
        /// <param name="currentUser">Current active user</param>
        /// <param name="torchConfigId">the config ID to use for the merge.  If -1, use default</param>
        /// <param name="projectRecordId">The record id for the project</param>
        /// <param name="entityId">The custom entity id for the project</param>
        /// <param name="store">A value indicating whether or not a copy of the generated document should be saved</param>
        public DocumentMergeExecutor(
            ICurrentUser currentUser,
            int documentId,
            TorchProject project,
            cReportCriterion reportCriterion,
            Guid requestNumber,
            int torchConfigId,
            int projectRecordId,
            int entityId,
            bool store)
        {
            this._accountId = currentUser.AccountID;
            this._documentId = documentId;
            this.torchProject = project;
            this._reportCriterion = reportCriterion;
            this._projectRecordId = projectRecordId;
            this._entityId = entityId;
            this._storeDocument = store;

            this._mergeStatus = TorchMergeState.Get(project.MergeProjectId, requestNumber, currentUser);
            this._mergeStatus.TotalToProcess = this.torchProject.GetReportSources().Count;
            this._mergeStatus.Save();

            this._currentUser = currentUser;
            this._torchConfigId = torchConfigId;
            _calcManager = new UltraWebCalcManager();
            var fnTxt = new cText();
            _calcManager.RegisterUserDefinedFunction(fnTxt);
        }

        #endregion

        #region Delegates

        /// <summary>
        /// Delegate declaration for PerformMerge
        /// </summary>
        /// <param name="exportDocType">The export document type</param>
        public delegate void PerformMerge(TorchExportDocumentType exportDocType);

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Performs the data merge into the target document
        /// </summary>
        /// <param name="exportDocType">DOC or DOCX (not currently used)</param>
        /// <returns>Word Document with data merged</returns>
        public void MergeDocument(TorchExportDocumentType exportDocType)
        {
            var documentMergeProject = new DocumentMergeProject(this._currentUser);

            TorchGroupingConfiguration torchGroupingConfiguration = this.TorchGroupingConfiguration ??
                documentMergeProject.GetGroupingConfiguration(this.torchProject.MergeProjectId, this.GetConfigId());
            using (var torchDocIoMergEngine = new TorchDocIoMergeEngine(torchGroupingConfiguration, this._accountId))
            {
                torchDocIoMergEngine.NumberOfSummaries = 25;
                torchDocIoMergEngine.SummarySortColumns[0] = "FlagIssue";
                torchDocIoMergEngine.SummarySortColumns[1] = "NameofCompany";
                torchDocIoMergEngine.SummarySortColumns[2] = "NameofCompany";
                this._mergeStatus.Status = 1;
                this._mergeStatus.ProgressCount = 0;
                this._mergeStatus.Save();
                var criteriaId = new Guid("1CBA7F8E-B0E6-4123-ABC5-BF87D346F37E");

                var templates = new DocumentTemplate(
                    this._currentUser.AccountID,
                    this._currentUser.CurrentSubAccountId,
                    this._currentUser.EmployeeID);
                cDocumentTemplate documentTemplate = templates.getTemplateById(this._documentId);

                if (documentTemplate != null)
                {
                    FormatType formatType;
                    switch (exportDocType)
                    {
                        case TorchExportDocumentType.MS_Word_DOC:
                            formatType = FormatType.Doc;
                            break;
                        case TorchExportDocumentType.MS_Word_DOCX:
                            formatType = FormatType.Word2013;
                            break;
                        default:
                            this._mergeStatus.Status = 3;
                            this._mergeStatus.ErrorMessage = "Document type must be DOC or DOCX";
                            this._mergeStatus.Save();
                            return;
                    }

                    this._mergeStatus.OutputDocType = exportDocType;
                    this._mergeStatus.Save();

                    WordDocument document;

                    try
                    {
                        document = templates.GetDocumentForMergeProject(
                            this._documentId,
                            this.torchProject.MergeProjectId,
                            formatType);
                    }
                    catch (Exception ex)
                    {
                        this._mergeStatus.Status = 3;
                        this._mergeStatus.ErrorMessage = $"Failed to load document. {ex.Message}.";
                        this._mergeStatus.Save();
                        return;
                    }
                    try
                    {

                    var reportsPath = ConfigurationManager.AppSettings["ReportsServicePath"] + "/reports.rem";
                    var reports = (IReports)Activator.GetObject(typeof(IReports), reportsPath);

                    document.XHTMLValidateOption = XHTMLValidationType.None;

                    Dictionary<int, cReport> reportSources = this.torchProject.GetReportSources();

                    foreach (
                        cReport report in
                            reportSources.Select(reportSourceKeyValuePair => reportSourceKeyValuePair.Value))
                    {
                        if (this._reportCriterion != null)
                        {
                            var entityFilter = new cReportCriterion(
                                criteriaId,
                                report.reportid,
                                this._reportCriterion.field,
                                this._reportCriterion.condition,
                                this._reportCriterion.value1,
                                this._reportCriterion.value2,
                                this._reportCriterion.joiner,
                                this._reportCriterion.order,
                                this._reportCriterion.runtime,
                                (byte)this._reportCriterion.groupnumber);

                            report.criteria.Add(entityFilter);
                        }

                        // run report
                        var reportRequest = new cReportRequest(
                            this._currentUser.AccountID,
                            this._currentUser.CurrentSubAccountId,
                            1,
                            report,
                            ExportType.Viewer,
                            null,
                            false,
                            this._currentUser.EmployeeID,
                            AccessRoleLevel.AllData);

                        using (DataSet dataSet = reports.createSynchronousReport(reportRequest))
                        {
                            if (dataSet != null && dataSet.Tables.Count > 0)
                            {
                                DataTable processedDataTable;
                                var chart = ReportChart.Get(report.reportid, this._currentUser);

                                if (chart.XAxis > -1 && chart.YAxis > -1 && dataSet.Tables.Count > 0)
                                {
                                    processedDataTable = dataSet.Tables[0];
                                    int idColCount = GetIdColumns(report.columns);
                                    processedDataTable = ProcessChartDataSet(
                                        processedDataTable,
                                        reportRequest,
                                        idColCount,
                                        chart.XAxis,
                                        chart.YAxis,
                                        chart.GroupBy,
                                        torchDocIoMergEngine.GroupingConfiguration);
                                    processedDataTable = this.ProcessReportDataSet(report, processedDataTable, this._projectRecordId);
                                }
                                else
                                {
                                    processedDataTable = this.ProcessReportDataSet(report, dataSet.Tables[0], this._projectRecordId);
                                }

                                if (processedDataTable != null)
                                {
                                    torchDocIoMergEngine.DataSet.Tables.Add(processedDataTable);
                                }

                                dataSet.Tables[0].Reset();
                                dataSet.Tables[0].Dispose();
                            }
                        }

                        this._mergeStatus.ProgressCount++;
                        this._mergeStatus.Save();
                    }

                        string documentPostFix;

                        switch (formatType)
                        {
                            case FormatType.Doc:
                                documentPostFix = ".doc";
                                break;
                            case FormatType.Dot:
                                documentPostFix = ".dot";
                                break;
                            case FormatType.Docx:
                                documentPostFix = ".docx";
                                break;
                            case FormatType.Word2007:
                                documentPostFix = ".doc";
                                break;
                            case FormatType.Word2010:
                                documentPostFix = ".docx";
                                break;
                            case FormatType.Word2013:
                                documentPostFix = ".docx";
                                break;
                            case FormatType.EPub:
                                documentPostFix = ".epub";
                                break;
                            case FormatType.Xml:
                                documentPostFix = ".xml";
                                break;
                            case FormatType.Txt:
                                documentPostFix = ".txt";
                                break;
                            case FormatType.Html:
                                documentPostFix = ".html";
                                break;
                            case FormatType.Rtf:
                                documentPostFix = ".rtf";
                                break;
                            case FormatType.Automatic:
                                documentPostFix = ".docx";
                                break;
                            default:
                                this._mergeStatus.Status = 3;
                                this._mergeStatus.Save();
                                throw new ArgumentOutOfRangeException();
                        }

                        Guid documentIdentifier = Guid.NewGuid();
                        this._mergeStatus.DocumentIdentifier = documentIdentifier;
                        this._mergeStatus.Save();

                        string documentVirtualDirectory =
                            ConfigurationManager.AppSettings["ReportsOutputVirtualDirectory"];

                        string hostName = HostManager.GetHostName(
                            this._currentUser.Account.HostnameIds,
                            this._currentUser.CurrentActiveModule,
                            this._currentUser.Account.companyid);
                        string documentUrl =
                            $"http://{hostName}/{documentVirtualDirectory}/{this._mergeStatus.DocumentIdentifier}{documentPostFix}";

                        torchDocIoMergEngine.ExecuteNestedMerge(document);
                        this.ProcessImagesAndRichText(document);
                        torchDocIoMergEngine.PostProcessDocument(document, this.torchProject.BlankTableText);
                        this._mergeStatus.DocumentPath = this.SaveWordDocument(
                            this._currentUser,
                            document,
                            documentIdentifier.ToString(),
                            documentPostFix,
                            formatType,
                            documentTemplate);
                        this._mergeStatus.DocumentUrl = documentUrl;
                        this._mergeStatus.Status = 2;
                        this._mergeStatus.Save();
                    }
                    catch (Exception ex)
                    {
                        string message = $"Torch: {Environment.NewLine}{ex.Message}{Environment.NewLine}";

                        if (ex.InnerException != null)
                        {
                            message += ex.InnerException.Message + Environment.NewLine;
                        }

                        message += ex.StackTrace;
                        cEventlog.LogEntry(message, true, EventLogEntryType.Error);
                        this._mergeStatus.Status = 3;
                        this._mergeStatus.ErrorMessage = ex.Message;
                        this._mergeStatus.Save();
                    }
                    finally
                    {
                        document.Close();
                        document = null;
                        this._calcManager.Dispose();
                    }
                }
                else
                {
                    this._mergeStatus.Status = 2;
                    this._mergeStatus.Save();
                }
            }
        }

        private int GetConfigId()
        {
            var result = this._torchConfigId;
            if (result == -1)
            {
                result = this.torchProject.DefaultDocumentGroupingConfigId.HasValue && this.torchProject.DefaultDocumentGroupingConfigId.Value > 0
                    ? this.torchProject.DefaultDocumentGroupingConfigId.Value
                    : this.torchProject.GetFirstGroupingConfig();
            }

            return result;
        }

        private string SaveWordDocument(
            ICurrentUserBase user,
            WordDocument document,
            string documentIdentifier,
            string documentPostFix,
            FormatType formatType,
            cDocumentTemplate template)
        {
            string documentOutputFolder = ConfigurationManager.AppSettings["ReportsOutputFilePath"];
            string documentPath = documentOutputFolder.EndsWith(@"\")
                ? string.Format("{0}{1}{2}", documentOutputFolder, documentIdentifier, documentPostFix)
                : string.Format("{0}\\{1}{2}", documentOutputFolder, documentIdentifier, documentPostFix);

            if (File.Exists(documentPath))
            {
                File.Delete(documentPath);
            }

            document.Save(documentPath, formatType);

            if (this._storeDocument)
            {
                using (var pdfMemoryStream = new MemoryStream())
                {
                    // for some reason saving the word document to disk, then re-opening it before PDFing makes for better conversion (page breaks are maintained, section headings aren't missing, etc)
                    var documentToConvert = new WordDocument(documentPath);
                    new DocToPDFConverter().ConvertToPDF(documentToConvert).Save(pdfMemoryStream);
                    documentToConvert.Close();
                    documentToConvert = null;

                    int documentAssociationId = template.DocumentMergeAssociations.Where(association => association.DocumentId == this._documentId && association.RecordId == this._projectRecordId && association.EntityId == this._entityId).Select(association => association.DocMergeAssociationId).FirstOrDefault();

                    int? delegateId = null;
                    if (user.isDelegate)
                    {
                        delegateId = user.Delegate.EmployeeID;
                    }

                    var attachments = new cAttachments(user.AccountID, user.EmployeeID, user.CurrentSubAccountId, delegateId);
                    var attachmentFilename = string.Format("{0}-{1}.pdf", template.DocumentName, DateTime.Now.ToString("ddMMyyyyHHmmss"));
                    var torchStoredDocument = new cAttachment(0, Guid.Empty, documentAssociationId, template.DocumentName, template.DocumentDescription, attachmentFilename, attachments.checkMimeType("pdf"), DateTime.Now, user.EmployeeID, null, null, pdfMemoryStream.ToArray(), false, true);
                    attachments.saveAttachment(string.Format("custom_{0}_attachments", this._entityId), "id", documentAssociationId, torchStoredDocument, torchStoredDocument.AttachmentData);
                }
            }

            return documentPath;
        }

        private string GetFieldValue(IList reportColumns, DataRow dataRow, int reportColumnIdx)
        {
            string fieldValue = null;
            var column = (cReportColumn)reportColumns[reportColumnIdx];

            if (dataRow[reportColumnIdx] != DBNull.Value)
            {
                fieldValue = dataRow[reportColumnIdx].ToString();

                switch (column.columntype)
                {
                    case ReportColumnType.Standard:
                        var stdcol = (cStandardColumn)column;
                        switch (stdcol.field.FieldType)
                        {
                            case "D":
                            case "DT":
                                DateTime dateTime = Convert.ToDateTime(fieldValue);
                                fieldValue = dateTime.ToShortDateString();
                                if (stdcol.field.FieldType == "DT")
                                {
                                    fieldValue += " " + dateTime.ToShortTimeString();
                                }

                                break;
                            case "X":
                                var tf = Convert.ToBoolean(fieldValue);
                                fieldValue = tf ? "Yes" : "No";
                                break;
                            case "N":
                                fieldValue = stdcol.field.ListItems.Count > 0
                                    ? fieldValue
                                    : string.IsNullOrEmpty(fieldValue)
                                        ? string.Empty
                                        : Convert.ToInt32(fieldValue).ToString(CultureInfo.InvariantCulture);

                                break;
                            case "CL":
                                int currencyId;
                                if (int.TryParse(fieldValue, out currencyId))
                                {
                                    var globalCurrencies = new cGlobalCurrencies();
                                    var currencies = new cCurrencies(_accountId, this._currentUser.CurrentSubAccountId);
                                    cCurrency currency = currencies.getCurrencyById(currencyId);
                                    cGlobalCurrency globalCurrency =
                                        globalCurrencies.getGlobalCurrencyById(currency.globalcurrencyid);
                                    fieldValue = globalCurrency.label;
                                }

                                break;
                            case "C":
                            case "A":
                            case "F":
                            case "FD":
                            case "M":
                                fieldValue =
                                    Math.Round(Convert.ToDecimal(fieldValue), 2).ToString(CultureInfo.InvariantCulture);
                                break;
                            case "G":
                            case "AT":
                                Guid fileId;
                                if (Guid.TryParse(fieldValue, out fileId))
                                {
                                    fieldValue = fileId == Guid.Empty ? "Error" : this.ProcessAttachmentImage(fileId);
                                }

                                break;
                        }
                        break;
                    case ReportColumnType.Static:
                        var stcol = (cStaticColumn)column;
                        fieldValue = stcol.literalvalue;
                        break;
                    case ReportColumnType.Calculated:
                        var calcol = (cCalculatedColumn)column;
                        string convertedFormula = ConvertFormula(reportColumns, calcol.formula, dataRow);
                        UltraCalcValue val = _calcManager.Calculate(convertedFormula);
                        fieldValue = val.ToString(CultureInfo.InvariantCulture);
                        break;
                }
            }

            return fieldValue ?? string.Empty;
        }

        /// <summary>
        /// Re order the data table columns so that the ID columns appear first and in the correct order ID0, ID1, ID2 etc...
        /// </summary>
        /// <param name="report">The report containing the id columns</param>
        /// <returns>A matrix of the ID column positions and values</returns>
        private static List<KeyValuePair<int, int>> GetIdColumnMatrix(cReport report)
        {
            var identifierColumnMatrix = new List<KeyValuePair<int, int>>();

            for (int i = 0; i < report.columns.Count; i++)
            {
                var column = report.columns[i] as cReportColumn;

                if (column != null
                    && (column.columntype == ReportColumnType.Standard
                        && ((cStandardColumn)column).field.Description.ToUpper() == "ID"))
                {
                    int position = column.JoinVia == null ? 0 : column.JoinVia.JoinViaList.Count;
                    identifierColumnMatrix.Add(new KeyValuePair<int, int>(i, position));
                }
            }

            return identifierColumnMatrix;
        }

        private void CopyColumn(DataTable source, DataTable destination, string column)
        {
            if (destination.Rows.Count == source.Rows.Count)
            {
                for (int i = 0; i < source.Rows.Count; i++)
                {
                    destination.Rows[i][column] = source.Rows[i][column];
                }
            }
        }

        private DataTable ProcessReportDataSet(cReport report, DataTable reportTable, int projectRecordId)
        {
            if (reportTable.Columns.Count > 0)
            {
                DataTable chartRecords = null;
                bool isChart = false;
                reportTable.TableName = report.reportname;
                List<KeyValuePair<int, int>> identifierColumnMatrix = GetIdColumnMatrix(report);
                int initialNumberofColumns = report.columns.Count;
                var copiedColumns = new Dictionary<int, string>();
                var textColumns = new Dictionary<int, string>();

                if (reportTable.Columns.Contains("Chart"))
                {
                    isChart = true;
                    chartRecords = reportTable.Copy();
                    reportTable.Columns.Remove("Chart");
                    reportTable.AcceptChanges();
                }

                for (int i = 0; i < initialNumberofColumns; i++)
                {
                    var column = report.columns[i] as cReportColumn;

                    if (column == null)
                    {
                        continue;
                    }

                    string columnName = string.Empty;
                    bool createNewColumn = false;

                    switch (column.columntype)
                    {
                        case ReportColumnType.Standard:
                            var stdcol = (cStandardColumn)column;

                            columnName = stdcol.field.Description;
                            columnName = columnName == "ID"
                                ? columnName + identifierColumnMatrix.FirstOrDefault(x => x.Key == i).Value
                                : columnName;

                            switch (stdcol.field.FieldType)
                            {
                                case "LT":
                                    textColumns.Add(i, columnName.Trim());
                                    break;
                                case "S":
                                case "FS":
                                    break;
                                default:
                                    createNewColumn = true;
                                    break;
                            }

                            break;
                        case ReportColumnType.Static:
                            var stcol = (cStaticColumn)column;
                            columnName = stcol.literalname;
                            createNewColumn = true;
                            break;
                        case ReportColumnType.Calculated:
                            var calcol = (cCalculatedColumn)column;
                            columnName = calcol.columnname;
                            createNewColumn = true;
                            break;
                        default:
                            createNewColumn = true;
                            break;
                    }

                    columnName = columnName.Replace("’", string.Empty).Trim();

                    if (createNewColumn && !reportTable.Columns.Contains(columnName))
                    {
                        copiedColumns.Add(i, columnName);
                    }
                    else
                    {
                        if ((reportTable.ExtendedProperties["chart"] != null) && (reportTable.Columns.Contains(columnName))
                            && (!columnName.StartsWith("ID")))
                        {
                            reportTable.Columns[columnName].ColumnName = "ChartValue" + columnName;
                        }

                        if (!reportTable.Columns.Contains(columnName))
                        {
                            reportTable.Columns[i].ColumnName = columnName;
                        }
                    }
                }

                foreach (var copiedColumn in copiedColumns)
                {
                    reportTable.Columns.Add(copiedColumn.Value, typeof(string));
                }

                foreach (DataRow dataRow in reportTable.Rows)
                {
                    int columnIndex = initialNumberofColumns;

                    foreach (var copiedColumn in copiedColumns)
                    {
                        string fieldValue = this.GetFieldValue(report.columns, dataRow, copiedColumn.Key);
                        var currentColumn = report.columns[copiedColumn.Key] as cStandardColumn;
                        if ((!string.IsNullOrEmpty(fieldValue) && (currentColumn != null && currentColumn.field.FieldType == "AT"))
                            || copiedColumn.Value.ToLower().Replace(" ", "") == "fileid")
                        {
                            reportTable.Columns[columnIndex].ColumnName = "TorchImage";
                        }

                        dataRow[columnIndex++] = fieldValue;
                    }
                }

                foreach (var copiedColumn in copiedColumns.OrderByDescending(x => x.Key))
                {
                    reportTable.Columns.Remove(reportTable.Columns[copiedColumn.Key]);
                }

                if (reportTable.Rows.Count == 1) //TODO: This will need to change or be removed later
                {
                    var foundData = false;
                    var allIdFieldsHaveValue = true;
                    var columnIndex = 0;
                    foreach (object o in reportTable.Rows[0].ItemArray)
                    {
                        if (!reportTable.Columns[columnIndex].ColumnName.StartsWith("ID"))
                        {
                            if ((!string.IsNullOrEmpty(o.ToString())))
                            {
                                foundData = true;
                            }
                        }
                        else
                        {
                            if (string.IsNullOrEmpty(o.ToString()))
                            {
                                allIdFieldsHaveValue = false;
                                break;
                            }
                        }

                        columnIndex++;
                    }

                    if (!foundData || !allIdFieldsHaveValue)
                    {
                        reportTable.Rows.RemoveAt(0);
                        reportTable.AcceptChanges();
                    }
                }

                if (isChart)
                {
                    reportTable.Columns.Add("Chart", typeof(string));
                    CopyColumn(chartRecords, reportTable, "Chart");
                }

                if (chartRecords != null)
                {
                    chartRecords.Rows.Clear();
                    chartRecords.Clear();
                    chartRecords.Dispose();
                }

                foreach (DataColumn dataColumn in reportTable.Columns)
                {
                    foreach (DataRow dataRow in reportTable.Rows)
                    {
                        if (identifierColumnMatrix.All(x => x.Key != dataColumn.Ordinal))
                        {
                            if (dataRow[dataColumn.Ordinal] is DBNull)
                            {
                                dataRow[dataColumn.Ordinal] = string.Empty;
                            }
                            else
                            {
                                dataRow[dataColumn.Ordinal] = dataRow[dataColumn.Ordinal].ToString().Trim();
                            }
                        }
                    }
                }

                // change the data type of all columns to string
                DataTable cloned = reportTable.Clone();
                foreach (DataColumn column in cloned.Columns)
                {
                    column.DataType = typeof(string);
                }

                foreach (DataRow row in reportTable.Rows)
                {
                    cloned.ImportRow(row);
                }

                reportTable.Rows.Clear();
                reportTable.Clear();
                reportTable.Dispose();

                if (cloned.Rows.Count == 0 && identifierColumnMatrix.Count == 1 && !cloned.TableName.ToLower().Contains("chart"))
                {
                    DataRow row = cloned.NewRow();
                    foreach (DataColumn dataColumn in cloned.Columns)
                    {
                        row[dataColumn] = " ";
                    }

                    row["ID0"] = projectRecordId.ToString(CultureInfo.InvariantCulture);

                    cloned.Rows.Add(row);
                }

                return cloned;
            }

            return null;
        }

        /// <summary>
        /// The DoMerge method
        /// </summary>
        /// <param name="exportDocumentType"></param>
        public void DoMerge(TorchExportDocumentType exportDocumentType)
        {
            var mergeProcess = new PerformMerge(this.MergeDocument);
            mergeProcess.BeginInvoke(exportDocumentType, MergeComplete, null);
        }

        #endregion

        #region Methods

        private string ExtractImagesFromHtml(string fieldValue)
        {
            GetReportImages(this._accountId, fieldValue);

            if (fieldValue.Contains("SoftwareEurope?=".ToLower()))
            {
                fieldValue = fieldValue.Replace(
                    "SoftwareEurope?=".ToLower(),
                    ConfigurationManager.AppSettings["tempDocMergeImageLocation"]);
            }

            return fieldValue;
        }

        /// <summary>
        ///     Extracts the FileID GUIDs and create the files on disk
        ///     <param name="accountId"></param>
        ///     <param name="fieldValue"></param>
        /// </summary>
        private static void GetReportImages(int accountId, string fieldValue)
        {
            const string RegEx =
                @"((?:(?:\s*\{*\s*(?:0x[\dA-F]+)\}*\,?)+)|(?<![a-f\d])[a-f\d]{32}(?![a-f\d])|"
                + @"(?:\{\(|)(?<![A-F\d])[A-F\d]{8}(?:\-[A-F\d]{4}){3}\-[A-F\d]{12}(?![A-F\d])(?:\}|\)|))";

            var matches = Regex.Matches(fieldValue.ToUpper(), RegEx);

            foreach (Match fileId in matches)
            {
                var entities = new cCustomEntities();
                entities.SaveHTMLEditorImagesToDisk(accountId, fileId.Value);
            }
        }

        /// <summary>
        /// Processes an attachment based on its fileID.
        /// </summary>
        /// <param name="fileId"></param>
        /// <returns>Error or html image tag</returns>
        private string ProcessAttachmentImage(Guid fileId)
        {
            var customEntities = new cCustomEntities();
            HTMLImageData fileData = customEntities.GetCustomEntityAttachmentData(
                this._currentUser.AccountID,
                fileId.ToString());

            if (fileData != null)
            {
                var entities = new cCustomEntities();

                if (customEntities.CheckFileTypeIsImage(fileData.fileType))
                {
                    entities.SaveHTMLEditorImagesToDisk(this._currentUser.AccountID, fileId.ToString());
                    //build html image tag
                    return string.Format(
                        "{0}{1}.{2}",
                        ConfigurationManager.AppSettings["tempDocMergeImageLocation"],
                        fileData.fileID,
                        fileData.fileType);
                }

                if (fileData.fileType.ToLower() == "docx" || fileData.fileType.ToLower() == "doc")
                {
                    var imageData = entities.GetWordAttachmentsData(this._currentUser.AccountID, fileId.ToString());
                    string htmlData;
                    var location = 0;

                    using (var stream = new MemoryStream(imageData))
                    {
                        try
                        {
                            var newDoc = new WordDocument(
                                stream,
                                fileData.fileType.ToLower() == "docx" ? FormatType.Word2010 : FormatType.Word2007);
                            var htmlExport = new HTMLExport();

                            using (var htmlStream = new MemoryStream())
                            {
                                htmlExport.SaveAsXhtml(newDoc, htmlStream);
                                htmlData = Encoding.UTF8.GetString(htmlStream.ToArray());
                                location = htmlData.IndexOf("<body>");
                                htmlData = htmlData.Replace("</html>", "");
                                htmlStream.Close();
                                newDoc.Close();
                                newDoc = null;
                            }

                            stream.Close();
                        }
                        catch (Exception)
                        {
                            htmlData = "Error";
                        }
                    }

                    return htmlData.Substring((location));
                }

                return "Error";
            }

            return "Error";
        }

        /// <summary>
        /// Uses REGEX to find any HTML tags in each cell.  If found, they are removed and an AppendHTML done instead.
        /// </summary>
        /// <param name="wordDoc">The document to clean up</param>
        private void ProcessImagesAndRichText(IWordDocument wordDoc)
        {
            var htmlTagRegex = new Regex(
                @"\</?\w+((\s+\w+(\s*=\s*(?:“.*?”|‘.*?’|[^‘“>\s]+))?)+\s*|\s*)/?>",
                RegexOptions.Compiled | RegexOptions.IgnoreCase);
            var htmlCharEntityRegex = new Regex(@"&[^;].\w{0,};", RegexOptions.Compiled | RegexOptions.IgnoreCase);

            foreach (IWSection currentSection in wordDoc.Sections)
            {
                // Read the section text body and child entities
                WTextBody body = currentSection.Body;
                var items = body.ChildEntities as BodyItemCollection;

                if (items == null)
                {
                    continue;
                }

                // Iterate the section's child entities, ignore anything isn't a 3-column table 
                for (int childEntityIndex = 0; childEntityIndex < items.Count; childEntityIndex++)
                {
                    if (items[childEntityIndex].EntityType != EntityType.Table)
                    {
                        var valueCell = ((WParagraph)items[childEntityIndex]);

                        if (htmlTagRegex.IsMatch(valueCell.Text) || htmlCharEntityRegex.IsMatch(valueCell.Text))
                        {
                            var celltext = valueCell.Text;
                            celltext = this.ExtractImagesFromHtml(celltext);

                            AppendHtml(valueCell, celltext, items);
                        }

                        continue;
                    }

                    var table = (IWTable)items[childEntityIndex];

                    if (table.Rows.Count > 0)
                    {
                        if (table.Rows.Count == 1 && table.LastCell.Tables.Count > 0)
                        {
                            // switch to the nested table
                            table = table.LastCell.Tables[0];
                        }

                        // Iterate the table's rows (in reverse so that rowIndex doesn't get mangled when the collection is altered)
                        for (int rowIndex = table.Rows.Count - 1; rowIndex > -1; rowIndex--)
                        {
                            WTableRow row = table.Rows[rowIndex];

                            for (int i = 0; i < row.Cells.Count; i++)
                            {
                                WTableCell valueCell = row.Cells[i];

                                if (valueCell.Paragraphs.Count == 0 || (!htmlTagRegex.IsMatch(valueCell.Paragraphs[0].Text)
                                    && !htmlCharEntityRegex.IsMatch(valueCell.Paragraphs[0].Text)))
                                {
                                    continue;
                                }
                                var celltext = valueCell.Paragraphs[0].Text;

                                celltext = this.ExtractImagesFromHtml(celltext);
                                valueCell.Paragraphs[0].Text = "";

                                try
                                {
                                    AppendHtml(valueCell.Paragraphs[0], celltext, items);
                                }
                                catch (Exception ex)
                                {
                                    HtmlUtility util = HtmlUtility.Instance;
                                    celltext = util.FixPTags(celltext);
                                    try
                                    {
                                        AppendHtml(valueCell.Paragraphs[0], celltext, items);
                                    }
                                    catch (Exception e)
                                    {
                                    valueCell.Paragraphs[0].Text = string.Format(
                                        "Error merging attachment.[{0}]",
                                        ex.Message);
                                }
                            }
                        }
                    }
                }
            }
        }
        }

        private static void AppendHtml(WParagraph valueCell, string celltext, BodyItemCollection items)
        {
            var lastIndexOfDelimiter = celltext.LastIndexOf(">", StringComparison.Ordinal);
            if (!celltext.TrimEnd().EndsWith(">") && lastIndexOfDelimiter > -1)
            {
                celltext = EncodeTrailingText(celltext, lastIndexOfDelimiter);
            }


            var valueCellIndex = 0;
            var currentIndex = 0;
            IEntity nextPara = null;
            foreach (WParagraph paragraph in valueCell.OwnerTextBody.Paragraphs)
            {
                if (paragraph == valueCell)
                {
                    valueCellIndex = currentIndex;
                    nextPara = paragraph.NextSibling;
                }
                currentIndex++;
            }


            valueCell.Text = "";
            var fontSize = ((WTextRange) valueCell.ChildEntities.FirstItem).CharacterFormat.FontSize;

            valueCell.ChildEntities.Clear();
            try
            {
                valueCell.AppendHTML(celltext);
                currentIndex = 0;
                if (valueCell.OwnerTextBody != null)
                {
                    foreach (WParagraph paragraph in valueCell.OwnerTextBody.Paragraphs)
                    {
                        if (currentIndex >= valueCellIndex)
                        {
                            if (nextPara != null && paragraph == nextPara)
                            {
                                break;
                            }
                            foreach (IEntity childEntity in paragraph.ChildEntities)
                            {
                                if (childEntity.EntityType == EntityType.TextRange)
                                {
                                    ((WTextRange) childEntity).CharacterFormat.FontSize = fontSize;
                                }
                            }
                        }

                        currentIndex++;
                    }
                }
                else
                {
                    foreach (WParagraph wParagraph in items)
                    {
                        if (currentIndex >= valueCellIndex)
                        {
                            if (nextPara != null && wParagraph == nextPara)
                            {
                                break;
                            }
                            foreach (IEntity childEntity in wParagraph.ChildEntities)
                            {
                                if (childEntity.EntityType == EntityType.TextRange)
                                {
                                    ((WTextRange) childEntity).CharacterFormat.FontSize = fontSize;
                                }
                            }
                        }

                        currentIndex++;
                    }
                }
            }
            catch (Exception ex)
            {
                try
                {
                    HtmlUtility util = HtmlUtility.Instance;
                    celltext = util.FixPTags(celltext);
                    valueCell.AppendHTML(celltext);
                }
                catch (Exception e)
                {
                    valueCell.Text = $"Error merging attachment.[{ex.Message}] {Environment.NewLine}{celltext}";
                }
            }
        }

        /// <summary>
        /// Encode HTML any text after the last '>' character
        /// </summary>
        /// <param name="celltext">The HTML text to encide</param>
        /// <param name="lastIndexOfDelimiter">The index of the last instance of '>' in the celltext</param>
        /// <returns>The celltext with any trailing text encoded.</returns>
        private static string EncodeTrailingText(string celltext, int lastIndexOfDelimiter)
        {
            var replaceText = celltext.Substring(lastIndexOfDelimiter + 1);
            replaceText = HttpUtility.HtmlEncode(replaceText);
            return celltext.Substring(0, lastIndexOfDelimiter + 1) + replaceText;

        }

        private static string ConvertFormula(IList columns, string formula, DataRow row)
        {
            int start = 0;

            while (start < formula.Length)
            {
                int startIndex = formula.IndexOf('[', start);

                if (startIndex == -1)
                {
                    break;
                }

                int endIndex = formula.IndexOf(']', startIndex);

                if (endIndex == -1)
                {
                    break;
                }

                string column = formula.Substring(startIndex + 1, endIndex - startIndex - 1);

                //replace with index
                bool columnfound = false;

                for (int i = 0; i < columns.Count; i++)
                {
                    var rptcolumn = (cReportColumn)columns[i];

                    switch (rptcolumn.columntype)
                    {
                        case ReportColumnType.Standard:
                            var standard = (cStandardColumn)rptcolumn;

                            if (standard.field.Description
                                == column.Replace("SUM of ", "")
                                    .Replace("COUNT of ", "")
                                    .Replace("AVG of ", "")
                                    .Replace("MAX of ", "")
                                    .Replace("MIN of ", ""))
                            {
                                column = "[" + column + "]";
                                string columnvalue;

                                if (row[i] != DBNull.Value)
                                {
                                    if (row[i] is DateTime)
                                    {
                                        var date = (DateTime)row[i];
                                        columnvalue = "'" + date.Year + "/" + date.Month + "/" + date.Day + "'";
                                    }
                                    else
                                    {
                                        columnvalue = string.Format("'{0}'", row[i]);
                                    }
                                }
                                else
                                {
                                    columnvalue = "''";
                                }

                                formula = formula.Replace(column, columnvalue);
                                columnfound = true;
                            }

                            break;
                    }

                    if (columnfound)
                    {
                        break;
                    }
                }

                if (!columnfound)
                {
                    formula = formula.Replace(column, "[! Invalid column reference !]");
                }

                start = 0;
            }

            return formula;
        }

        /// <summary>
        ///   Generates the chart image from the gathered chart data in chartDataSet.
        ///   Builds a datatable of the hierarchy and path to the report image. Used when undertaking the document merge.
        /// </summary>
        /// <param name="reportRequest"></param>
        /// <param name="recordIdColumnPosition"></param>
        /// <param name="idColumnCount"></param>
        /// <param name="reportTable"></param>
        /// <param name="rowId"></param>
        private static void ProcessChartData(
            cReportRequest reportRequest,
            DataTable reportTable,
            int rowId,
            int recordIdColumnPosition,
            int idColumnCount)
        {
            var chartDataSet = new DataSet("ChartDataSet");
            string highestIdColumnName = string.Format("ID{0}", idColumnCount - 1);
            reportTable.Columns[recordIdColumnPosition].ColumnName = highestIdColumnName;
            DataTable filteredTable = reportTable.Copy();
            filteredTable.TableName = "ChartMergeData";
            filteredTable.Delete(string.Format("{0} <> {1}", highestIdColumnName, rowId));
            filteredTable.AcceptChanges();
            chartDataSet.Tables.Add(filteredTable);
            var clsreports =
                (IReports)
                    Activator.GetObject(
                        typeof(IReports),
                        string.Format("{0}/reports.rem", ConfigurationManager.AppSettings["ReportsServicePath"]));
            DataSet processedChartData = clsreports.GetChartData(reportRequest, chartDataSet);
            string fileName = processedChartData.Tables[1].Rows[0].ItemArray[0].ToString();
            fileName = fileName.Substring(0, fileName.LastIndexOf("?", StringComparison.Ordinal));
            string chartPath = ConfigurationManager.AppSettings["tempDocMergeImageLocation"];
            chartPath = chartPath.EndsWith("\\")
                ? string.Format("{0}{1}", chartPath, fileName)
                : string.Format("{0}\\{1}", chartPath, fileName);
            DataRow[] rows = reportTable.Select(string.Format("{0} = {1}", highestIdColumnName, rowId));
            foreach (DataRow dataRow in rows)
            {
                dataRow["Chart"] = chartPath;
            }

            filteredTable.Rows.Clear();
            filteredTable.Clear();
            filteredTable.Dispose();
            chartDataSet.Tables.Clear();
            chartDataSet.Dispose();
        }

        private static DataTable GetUniqueCombinationsOfGroupingColumnsFromChartDataTable(
            cReport report,
            DataTable dataTable,
            TorchGroupingConfiguration torchGroupingConfiguration)
        {
            var filteredColumns =
                torchGroupingConfiguration.FilteringColumns.Select(
                    filteringConfig => filteringConfig.ColumnName).ToList();
            var sortedColumns =
                torchGroupingConfiguration.SortingColumnsList.Select(
                    sortingConfig => sortingConfig.Name).ToList();
            string[] groupedFilteredAndSortedCommonColumnsList =
                torchGroupingConfiguration.GroupingColumnsList.Union(filteredColumns.Union(sortedColumns)).ToArray();

            if (torchGroupingConfiguration.GroupingColumnsList.Contains(""))
            {
                throw new Exception("Grouping columns are invalid. Please provide a valid configuration");
            }

            for (int i = report.columns.Count - 1; i >= 0; i--)
            {
                var reportColumn = (cReportColumn)report.columns[i];
                if (reportColumn.columntype == ReportColumnType.Standard)
                {
                    string columnName = ((cStandardColumn)reportColumn).field.Description;
                    if (groupedFilteredAndSortedCommonColumnsList.Contains(columnName))
                    {
                        if (!dataTable.Columns.Contains(columnName))
                        {
                            dataTable.Columns[i].ColumnName = columnName;
                        }
                    }
                }
            }

            DataView dataView = dataTable.DefaultView;
            return dataView.ToTable(true, groupedFilteredAndSortedCommonColumnsList);
        }

        private static DataTable ProcessChartDataSet(
            DataTable reportTable,
            cReportRequest reportRequest,
            int idColCount,
            int xAxis,
            int yAxis,
            int groupbyColumn,
            TorchGroupingConfiguration torchGroupingConfiguration)
        {
            reportTable.Columns.Add("Chart", typeof(string));
            var collectionTable = reportTable.Clone();
            List<KeyValuePair<int, int>> idMatrix = GetIdColumnMatrix(reportRequest.report);
            int recordIdColumnPosition = idMatrix.FirstOrDefault(x => x.Value == idColCount - 1).Key;
            bool goodToGo = reportTable.Rows.Count > 0
                            && !string.IsNullOrEmpty(reportTable.Rows[0][recordIdColumnPosition].ToString());

            if (goodToGo)
            {
                DataTable uniqueGroupingSets = GetUniqueCombinationsOfGroupingColumnsFromChartDataTable(
                    reportRequest.report,
                    reportTable,
                    torchGroupingConfiguration);
                for (int i = 0; i < uniqueGroupingSets.Rows.Count; i++)
                {
                    DataTable refinedTable = reportTable.Copy();
                    foreach (DataRow row in reportTable.Rows)
                    {
                        for (int j = 0; j < uniqueGroupingSets.Columns.Count; j++)
                        {
                            string commonColumnName = uniqueGroupingSets.Columns[j].ColumnName;
                            if (row[commonColumnName].ToString()
                                != uniqueGroupingSets.Rows[i][commonColumnName].ToString())
                            {
                                int matchLocation = FindRowIndexMatch(refinedTable, row);
                                if (matchLocation != -1)
                                {
                                    refinedTable.Rows.RemoveAt(matchLocation);
                                }
                                break;
                            }
                        }
                    }

                    refinedTable.AcceptChanges();

                    foreach (DataRow groupedRow in refinedTable.Rows)
                    {

                        if (refinedTable.Rows.Count > 0)
                        {
                            var rowId = 0;

                            if (!int.TryParse(groupedRow[recordIdColumnPosition].ToString(), out rowId))
                            {
                                continue;
                            }

                            if (!string.IsNullOrEmpty(groupedRow[yAxis].ToString())
                                && !string.IsNullOrEmpty(groupedRow[xAxis].ToString()))
                            {
                                string xAxisValue = groupedRow[xAxis].ToString();
                                string yAxisValue = groupedRow[yAxis].ToString();
                                string groupByValue = string.Empty;

                                if (groupbyColumn != -1)
                                {
                                    groupByValue = groupedRow[groupbyColumn].ToString();
                                }

                                groupedRow[xAxis] = xAxisValue;
                                groupedRow[yAxis] = yAxisValue;

                                if (groupbyColumn != -1)
                                {
                                    groupedRow[groupbyColumn] = groupByValue;
                                }
                            }

                            ProcessChartData(reportRequest, refinedTable, rowId, recordIdColumnPosition, idColCount);
                        }
                    }

                    foreach (DataRow dataRow in refinedTable.Rows)
                    {
                        if (!collectionTable.Select(string.Format("Chart='{0}'", dataRow["Chart"])).Any())
                        {
                            collectionTable.Rows.Add(dataRow.ItemArray);
                        }
                    }

                    refinedTable.Rows.Clear();
                    refinedTable.Clear();
                    refinedTable.Dispose();
                }

                uniqueGroupingSets.Rows.Clear();
                uniqueGroupingSets.Clear();
                uniqueGroupingSets.Dispose();
                reportTable = collectionTable.Copy();
                reportTable.AcceptChanges();
                collectionTable.Rows.Clear();
                collectionTable.Clear();
                collectionTable.Dispose();
            }

            reportTable.ExtendedProperties.Add("chart", true);
            return reportTable;
        }

        private static int FindRowIndexMatch(DataTable tableToSearch, DataRow rowToMatch)
        {
            for (int rowIndex = 0; rowIndex < tableToSearch.Rows.Count; rowIndex++)
            {
                if (tableToSearch.Rows[rowIndex].ItemArray.SequenceEqual(rowToMatch.ItemArray))
                {
                    return rowIndex;
                }
            }

            return -1;
        }

        /// <summary>
        ///     Gets the columns from the report that flagged as ID. This is used to determine the hierarchy drill down when building
        ///     the chart data.  
        /// </summary>
        /// <param name="reportColumns">The report columns</param>
        private static int GetIdColumns(IEnumerable reportColumns)
        {
            return (from cReportColumn reportColumn in reportColumns where reportColumn.columntype == ReportColumnType.Standard select (cStandardColumn)reportColumn).Count(standardColumn => standardColumn.field.Description == "ID" && standardColumn.field.IDField);
        }

        private static void MergeComplete(IAsyncResult a)
        {

        }

        #endregion
    }
}
