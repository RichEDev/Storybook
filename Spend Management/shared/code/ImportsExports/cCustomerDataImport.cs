using System.Data;
using SpendManagementLibrary.Helpers;

namespace Spend_Management
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Runtime.Remoting.Messaging;
    using System.Web;
    using System.Xml.Linq;

    using SpendManagementLibrary;
    using SpendManagementLibrary.Addresses;
    using SpendManagementLibrary.Employees;
    using SpendManagementLibrary.Employees.DutyOfCare;
    using SpendManagementLibrary.Enumerators;
    using SpendManagementLibrary.FinancialYears;
    using Syncfusion.XlsIO;
    using SpendManagementLibrary.Enumerators;
    using BusinessLogic.P11DCategories;
    using BusinessLogic;
    using BusinessLogic.DataConnections;


    /// <summary>
    /// Class used to import all data from the implementations spreadsheet 
    /// </summary>
    public class cCustomerDataImport
    {
        private int nAccountID;
        private int nEmployeeID;
        private int nLogID;
        private DateTime importStartDate;
        private int expectedrecordcount = 0;
        private int processedrecordcount = 0;
        private bool bSpreadsheetInvalid = false;
        cLogging clsLogging;

        /// <summary>
        /// 
        /// </summary>
        [Dependency]
        public IDataFactory<IP11DCategory, int> P11DCategoriesRepository { get; set; }

        /// <summary>
        /// Constructor for cCustomerDataImport
        /// </summary>
        /// <param name="AccountID">ID of the logged in account</param>
        /// <param name="employeeid">ID of the user</param>
        public cCustomerDataImport(int AccountID, int employeeid, int logID)
        {
            nAccountID = AccountID;
            nEmployeeID = employeeid;
            nLogID = logID;
            clsLogging = new cLogging(AccountID);
        }

        #region properties

        /// <summary>
        /// ID of the logged in account
        /// </summary>
        public int AccountID
        {
            get { return nAccountID; }
        }

        /// <summary>
        /// ID of the user
        /// </summary>
        public int employeeid
        {
            get { return nEmployeeID; }
        }

        /// <summary>
        /// ID of the Log
        /// </summary>
        public int logID
        {
            get { return nLogID; }
        }

        /// <summary>
        /// Get or set whether the spreadsheet is valid or invalid
        /// </summary>
        public bool spreadsheetInvalid
        {
            get { return bSpreadsheetInvalid; }
            set { bSpreadsheetInvalid = value; }
        }


        #endregion


        /// <summary>
        /// Read the excel workbook into memory and then get the required worksheets for the import from the first sheet of the spreadsheet
        /// </summary>
        /// <param name="fileData">Byte array of the spreadsheet data</param>
        public ImportProcessData ReadExcelTemplate(byte[] fileData)
        {
            ExcelEngine exceleng = new ExcelEngine();

            MemoryStream stream = new MemoryStream(fileData);

            XName name;
            cWorksheetMappingTemplate mappingTemp;
            IWorksheet worksheet;
            Dictionary<string, List<Dictionary<string, object>>> lstWorksheets = new Dictionary<string, List<Dictionary<string, object>>>();
            IApplication app = exceleng.Excel;

            IWorkbook workbook = app.Workbooks.Open(stream);

            cImportInfo importInfo = null;

            importInfo = new cImportInfo(0, "", ImportStatus.Validating, new SortedList<int, ImportStatusValues>());

            name = XName.Get("Instructions");
            //Put path in app settings in web config
            mappingTemp = ReadXMLTemplateWorksheetMapping(XDocument.Load(ConfigurationManager.AppSettings["ImplementationSpreadsheetXMLTemplatePath"] + "Excel_Import_Template_Mappings.xml").Root.Element(name), 0);//HttpContext.Current.Server.MapPath(@"\XMLImportMappings\Excel_Import_Template_Mappings.xml")).Root.Element(name), 0);

            worksheet = workbook.Worksheets[mappingTemp.worksheetName];

            validateExcelWorksheet(ref lstWorksheets, worksheet, mappingTemp, 1);

            Dictionary<string, string> lstRequiredWorksheets = getRequiredWorksheetsForImport(workbook, ref lstWorksheets, ref importInfo);

            HttpRuntime.Cache.Add("SpreadsheetImport" + AccountID, importInfo, null, System.Web.Caching.Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes((int)Caching.CacheTimeSpans.Medium), System.Web.Caching.CacheItemPriority.Default, null);

            ImportProcessData procData = new ImportProcessData();
            workbook.Close();
            app.Workbooks.Close();

            stream.Close();
            stream.Dispose();
            procData.lstRequireWorksheets = lstRequiredWorksheets;
            procData.lstWorksheets = lstWorksheets;
            //procData.workbook = workbook;


            return procData;
            //HttpContext.Current.Session["ImportData" + AccountID] = procData;

        }

        /// <summary>
        /// Delegate used to initiate the worksheet validation and import 
        /// </summary>
        /// <param name="workbook">Workbook object</param>
        /// <param name="importInfo">Reference to the import info class used to update the session</param>
        /// <param name="lstWorksheets">List of all worksheets and their corresponding columns and data</param>
        /// <param name="lstRequiredWorksheets">List of all worksheets required for the import</param>
        public delegate void delProcessImport(ICurrentUser currentUser, object workbook, Dictionary<string, List<Dictionary<string, object>>> lstWorksheets, Dictionary<string, string> lstRequiredWorksheets);

        /// <summary>
        /// Initialise and run the delegate method which will start processing the import
        /// </summary>
        /// <param name="procData">Structure containing all information required for the import</param>
        public void startImport(ICurrentUser currentUser, ImportProcessData procData)
        {
            //cImportInfo importInfo = (cImportInfo)HttpContext.Current.Session["SpreadsheetImport" + AccountID];

            cImportInfo importInfo = (cImportInfo)HttpRuntime.Cache["SpreadsheetImport" + AccountID];

            //HttpContext.Current.Session["SpreadsheetImport" + AccountID] = importInfo;

            delProcessImport process = new delProcessImport(processImport);
            process.BeginInvoke(currentUser, procData.workbook, procData.lstWorksheets, procData.lstRequireWorksheets, new AsyncCallback(importComplete), procData.workbook);
        }


        /// <summary>
        /// Method called from the delegate to start processing the worksheets and validate them. If the spreadsheet is valid then the import will commence.
        /// </summary>
        /// <param name="currentUser">The current user.</param>
        /// <param name="data">The workbook object.</param>
        /// <param name="lstWorksheets">List of all worksheets and their corresponding columns and data.</param>
        /// <param name="lstRequireWorksheets">List of all worksheets required for the import.</param>
        public void processImport(ICurrentUser currentUser, object data, Dictionary<string, List<Dictionary<string, object>>> lstWorksheets, Dictionary<string, string> lstRequireWorksheets)
        {
            IWorkbook workbook = (IWorkbook)data;
            cImportHistory clsHistory = new cImportHistory(AccountID);

            clsLogging.saveLogItem(logID, LogReasonType.None, null, cLoggingValues.filler1 + "Starting Spreadsheet Validation " + DateTime.Now.ToString() + cLoggingValues.space + cLoggingValues.filler1);

            processWorksheets(ref lstWorksheets, workbook, lstRequireWorksheets);

            clsLogging.saveLogItem(logID, LogReasonType.None, null, cLoggingValues.filler1 + "Finished Spreadsheet Validation " + DateTime.Now.ToString() + cLoggingValues.space + cLoggingValues.filler1);

            if (spreadsheetInvalid)
            {
                updateSession(0, "", 0, 0, ImportStatus.Invalid, WorksheetStatus.Invalid);

                //Finish the log so summary lines can be added
                clsLogging.saveLog(logID, LogType.SpreadsheetImport, expectedrecordcount, 0, 0, 0, 0);

                //Save the import history
                clsHistory.saveHistory(new cImportHistoryItem(0, 0, logID, importStartDate, ImportHistoryStatus.Failure, ApplicationType.ExcelImport, 0, DateTime.UtcNow, DateTime.UtcNow));
            }
            else
            {
                updateSession(0, "", 0, 0, ImportStatus.Valid, WorksheetStatus.Valid);

                clsLogging.saveLogItem(logID, LogReasonType.None, null, cLoggingValues.filler1 + "Starting Spreadsheet Import " + DateTime.Now.ToString() + cLoggingValues.space + cLoggingValues.filler1);

                insertImportData(currentUser, ref lstWorksheets);

                clsLogging.saveLogItem(logID, LogReasonType.None, null, cLoggingValues.filler1 + "Finished Spreadsheet Import " + DateTime.Now.ToString() + cLoggingValues.space + cLoggingValues.filler1);

                //Finish the log so summary line counters can be added
                clsLogging.saveLog(logID, LogType.SpreadsheetImport, expectedrecordcount, processedrecordcount, 0, 0, 0);

                #region Get the import status of the run template from the log line processing

                cLog log = clsLogging.getLogFromDatabase(logID, LogReasonType.None);
                ImportHistoryStatus importStatus = new ImportHistoryStatus();

                if (log.numFailedUpdates > 0 && log.numSuccessfulUpdates > 0)
                {
                    importStatus = ImportHistoryStatus.Success_With_Errors;
                }
                else if (log.numFailedUpdates > 0 && log.numSuccessfulUpdates == 0)
                {
                    importStatus = ImportHistoryStatus.Failure;
                }
                else if (log.numFailedUpdates == 0 && log.numSuccessfulUpdates > 0 && log.numWarningUpdates > 0)
                {
                    importStatus = ImportHistoryStatus.Success_With_Warnings;
                }
                else if (log.numSuccessfulUpdates > 0 && log.numWarningUpdates == 0)
                {
                    importStatus = ImportHistoryStatus.Success;
                }

                #endregion

                //Save the import history
                clsHistory.saveHistory(new cImportHistoryItem(0, 0, logID, importStartDate, importStatus, ApplicationType.ExcelImport, 0, DateTime.UtcNow, null));
            }
        }

        /// <summary>
        /// Callback method run when the delegate has finished processing the import
        /// </summary>
        /// <param name="a"></param>
        private void importComplete(IAsyncResult a)
        {
            AsyncResult result = (AsyncResult)a;
            delProcessImport process = (delProcessImport)result.AsyncDelegate;
            try
            {
                //process.EndInvoke(ref importInfo, a);
                //updateSession(0, 0, 0, ImportStatus.Complete, WorksheetStatus.Imported, context);
            }
            catch (Exception ex)
            {
                //updateSession(0, 0, 0, ImportStatus.Failed, WorksheetStatus.Failed, context);
                clsLogging.saveLogItem(logID, LogReasonType.None, null, "Error: Import failed with the following error: " + ex.Message);
            }
        }

        /// <summary>
        /// Reading of the passed in parent element from the XML template to extract the information about the columns and validation criteria required for its corresponding worksheet
        /// </summary>
        /// <param name="element">Parent XML element</param>
        /// <param name="depth">Depth of the child XML elements</param>
        /// <returns>An object containing all relevant mapping information for the reading of the data for the corresponding worksheet</returns>
        private cWorksheetMappingTemplate ReadXMLTemplateWorksheetMapping(XElement element, int depth)
        {
            cWorksheetMappingTemplate mappingTemp = null;

            if (element != null)
            {
                int startRow = 0;
                string worksheetName = "";
                int worksheetIndex = -1;
                List<cWorksheetColumnMappingTemplate> lstMappings = new List<cWorksheetColumnMappingTemplate>();

                int columnRef = 0, maxLength = 0;
                string columnName = "", fieldType = "", linkRef = "", linkMatch = "";
                bool isUnique = false, isMandatory = false;


                cWorksheetColumnMappingTemplate colMappingTemp;

                XAttribute attribute;

                #region Get the attribute values of the primary XML Element

                attribute = element.Attribute(XName.Get("StartRow"));
                startRow = int.Parse(attribute.Value);

                attribute = element.Attribute(XName.Get("SheetName"));
                worksheetName = attribute.Value;

                attribute = element.Attribute(XName.Get("SheetIndex"));
                worksheetIndex = int.Parse(attribute.Value);

                #endregion

                #region Get the attribute values of the child XML elements for the Spreadsheet columns

                if (element.HasElements)
                {
                    foreach (var child in element.Elements())
                    {
                        //Column Name
                        attribute = child.Attribute(XName.Get("Name"));
                        columnName = attribute.Value;

                        //Column Reference
                        attribute = child.Attribute(XName.Get("ColumnRef"));
                        columnRef = int.Parse(attribute.Value);

                        //Field Type
                        attribute = child.Attribute(XName.Get("FieldType"));
                        fieldType = attribute.Value;

                        //Maximum Length for the data
                        attribute = child.Attribute(XName.Get("MaxLength"));

                        if (attribute.Value == "")
                        {
                            maxLength = 0;
                        }
                        else
                        {
                            maxLength = int.Parse(attribute.Value);
                        }

                        //Is the column value unique
                        attribute = child.Attribute(XName.Get("IsUnique"));

                        if (attribute.Value == "")
                        {
                            isUnique = false;
                        }
                        else
                        {
                            isUnique = bool.Parse(attribute.Value);
                        }

                        //Is the column mandatory
                        attribute = child.Attribute(XName.Get("IsMandatory"));

                        if (attribute.Value == "")
                        {
                            isMandatory = false;
                        }
                        else
                        {
                            isMandatory = bool.Parse(attribute.Value);
                        }

                        //The value link to get the value for the import based on the linkMatch value e.g. countryid from countryname
                        attribute = child.Attribute(XName.Get("LinkRef"));
                        linkRef = attribute.Value;

                        //The link match value used to get the value from the link ref
                        attribute = child.Attribute(XName.Get("LinkMatch"));
                        linkMatch = attribute.Value;


                        colMappingTemp = new cWorksheetColumnMappingTemplate(columnName, columnRef, fieldType, maxLength, isUnique, isMandatory, linkRef, linkMatch);

                        lstMappings.Add(colMappingTemp);
                    }

                    mappingTemp = new cWorksheetMappingTemplate(worksheetIndex, worksheetName, startRow, lstMappings);

                }

                #endregion
            }
            return mappingTemp;
        }

        /// <summary>
        /// Validate the worksheet for the import using the mapping object
        /// </summary>
        /// <param name="importInfo">Reference to the import info class used to update the session</param>
        /// <param name="lstWorksheets">Reference to the list of all worksheets and their corresponding columns and data, so that each validated one can be added to the collection</param>
        /// <param name="worksheet">Worksheet object</param>
        /// <param name="mappingTemp">Object containing the mapping information from the XML template</param>
        /// <param name="sheetID">Sheet ID required to update the session</param>
        /// <returns>The actual row number with data on in the spreadsheet</returns>
        public int validateExcelWorksheet(ref Dictionary<string, List<Dictionary<string, object>>> lstWorksheets, IWorksheet worksheet, cWorksheetMappingTemplate mappingTemp, int sheetID)
        {
            clsLogging.saveLogItem(logID, LogReasonType.None, null, cLoggingValues.filler1 + " Validating worksheet " + worksheet.Name + ". " + DateTime.Now.ToString() + cLoggingValues.space + cLoggingValues.filler1);
            //Set the start date of the import
            importStartDate = DateTime.Now;

            bool worksheetValid = true;
            int rowCount = worksheet.Rows.GetLength(0);
            object cellVal;
            List<Dictionary<string, object>> worksheetData = new List<Dictionary<string, object>>();
            Dictionary<string, object> rowData;
            List<object> uniqueColData = new List<object>();
            List<object> columnVals; //List stores the values of the row columns. If it is empty this is the last row with data and the worksheet read is aborted.
            int actualRows = 0;
            bool validate = true;

            for (int row = mappingTemp.startRow; row <= rowCount; row++)
            {
                rowData = new Dictionary<string, object>();

                columnVals = new List<object>();
                actualRows++;

                //Need to check if the row has data
                foreach (cWorksheetColumnMappingTemplate column in mappingTemp.mappings)
                {
                    cellVal = worksheet.Range[row, column.columnRef].Text;

                    if (cellVal != null)
                    {
                        if (cellVal.ToString() != "")
                        {
                            columnVals.Add(cellVal);
                        }
                    }
                }

                if (columnVals.Count == 0)
                {
                    //This is the last row with data so the worksheet read is aborted
                    validate = false;
                    break;
                }

                if (validate)
                {
                    foreach (cWorksheetColumnMappingTemplate column in mappingTemp.mappings)
                    {

                        cellVal = worksheet.Range[row, column.columnRef].HasFormula ? worksheet.Range[row, column.columnRef].FormulaStringValue : worksheet.Range[row, column.columnRef].Text;
                        //cellVal = worksheet.Rows[row].Cells[column.columnRef].Text;

                        if (cellVal == null)
                        {
                            cellVal = worksheet.Range[row, column.columnRef].HasNumber ? worksheet.Range[row, column.columnRef].Value : "";

                            if (cellVal.ToString() == "")
                            {
                                cellVal = worksheet.Range[row, column.columnRef].Value;
                            }
                        }

                        //Check to see if the cell value has to unique
                        if (column.isUnique)
                        {
                            if (uniqueColData.Contains(cellVal))
                            {
                                clsLogging.saveLogItem(logID, LogReasonType.UniqueField, null, "On Worksheet " + worksheet.Name + " row " + row + " column " + column.name);
                                spreadsheetInvalid = true;
                                worksheetValid = false;
                            }

                            uniqueColData.Add(cellVal);
                        }

                        //Check to see if the cell value is mandatory
                        if (column.isMandatory)
                        {
                            if (cellVal.ToString() == "")
                            {
                                clsLogging.saveLogItem(logID, LogReasonType.MandatoryField, null, "On Worksheet " + worksheet.Name + " row " + row + " column " + column.name);
                                spreadsheetInvalid = true;
                                worksheetValid = false;
                            }
                        }

                        //Check to see if the cell value exceeds the required length
                        if (column.maxLength > 0 && cellVal.ToString().Length > column.maxLength)
                        {
                            clsLogging.saveLogItem(logID, LogReasonType.MaxLengthExceeded, null, "On Worksheet " + worksheet.Name + " row " + row + " column " + column.name);
                            spreadsheetInvalid = true;
                            worksheetValid = false;
                        }

                        if (!spreadsheetInvalid)
                        {
                            rowData.Add(column.name, cellVal);
                        }

                    }

                    //If there is a validation error then dont add any more sheet data
                    if (!spreadsheetInvalid)
                    {
                        worksheetData.Add(rowData);
                    }
                }
            }

            SortedList<int, ImportStatusValues> lstSheetStatus;

            if (mappingTemp.worksheetName != "Instructions")
            {
                cImportInfo importInfo = (cImportInfo)HttpRuntime.Cache["SpreadsheetImport" + AccountID];
                lstSheetStatus = importInfo.worksheets;
            }

            if (worksheetValid)
            {
                if (mappingTemp.worksheetName != "Instructions")
                {
                    //Set The status of the session for the import

                    updateSession(sheetID, mappingTemp.worksheetName, 0, 0, ImportStatus.Validating, WorksheetStatus.Valid);
                }
                clsLogging.saveLogItem(logID, LogReasonType.None, null, cLoggingValues.filler1 + " Worksheet " + worksheet.Name + " has been validated successfully. " + cLoggingValues.filler1);
            }
            else
            {
                if (mappingTemp.worksheetName != "Instructions")
                {
                    //Set The status of the session for the import
                    updateSession(sheetID, mappingTemp.worksheetName, 0, 0, ImportStatus.Validating, WorksheetStatus.Invalid);
                }

                clsLogging.saveLogItem(logID, LogReasonType.None, null, cLoggingValues.filler1 + " Worksheet " + worksheet.Name + " has failed validation, please check above errors. " + cLoggingValues.filler1);
            }

            lstWorksheets.Add(mappingTemp.worksheetName, worksheetData);

            return actualRows;
        }

        /// <summary>
        /// Get all the required worksheets for the import. This is extracted from the Instructions worksheet where the user has specified which worksheets are required for the import.
        /// </summary>
        /// <param name="workbook">Workbook object</param>
        /// <param name="lstWorksheets">Reference to the list of all worksheets and their corresponding columns and data, the Instructions worksheet will be added to the collection</param>
        /// <param name="importInfo">Reference to the import info class used to update the session</param>
        /// <returns>A collection of worksheet names and whether they are required</returns>
        private Dictionary<string, string> getRequiredWorksheetsForImport(IWorkbook workbook, ref Dictionary<string, List<Dictionary<string, object>>> lstWorksheets, ref cImportInfo importInfo)
        {
            //Get the list of worksheets that are being used for this import. This is the first worksheet in the worksheets collection
            Dictionary<string, string> lstRequiredSheets = new Dictionary<string, string>();


            string sheetName = "";
            string sheetRequired = "";

            foreach (List<Dictionary<string, object>> lst in lstWorksheets.Values)
            {
                foreach (Dictionary<string, object> obj in lst)
                {
                    sheetName = obj["Section"].ToString();
                    sheetRequired = obj["Required"].ToString();

                    //Company Details does not need to be Imported
                    if (sheetName != "Company Details")
                    {
                        lstRequiredSheets.Add(sheetName, sheetRequired);
                    }
                }
            }

            //Set up the worksheets to update the progress on.
            SortedList<int, ImportStatusValues> lstSheetStatus = new SortedList<int, ImportStatusValues>();
            int indexOfSheet = 0;

            ImportStatusValues statusVals = new ImportStatusValues();



            foreach (KeyValuePair<string, string> kp in lstRequiredSheets)
            {
                if (kp.Value.ToLower() == "yes")
                {
                    indexOfSheet++;

                    statusVals.sheetName = kp.Key;
                    statusVals.numOfRows = 0;
                    statusVals.processedRows = 0;
                    statusVals.status = WorksheetStatus.Validating;

                    lstSheetStatus.Add(indexOfSheet, statusVals);
                }
            }

            importInfo.worksheets = lstSheetStatus;

            return lstRequiredSheets;

        }

        /// <summary>
        /// Method to process through all the worksheets and validate them if they are required for the import
        /// </summary>
        /// <param name="importInfo">Reference to the import info class used to update the session</param>
        /// <param name="lstWorksheets">Reference to the list of all worksheets and their corresponding columns and data, the Instructions worksheet will be removed from the collection as it is no longer required when all sheets have been validated</param>
        /// <param name="workbook">Workbook object</param>
        /// <param name="lstRequiredSheets">List of worksheets for the import</param>
        public void processWorksheets(ref Dictionary<string, List<Dictionary<string, object>>> lstWorksheets, IWorkbook workbook, Dictionary<string, string> lstRequiredSheets)
        {
            XName name;
            cWorksheetMappingTemplate mappingTemp;
            IWorksheet worksheet;
            ImportStatusValues statusVals;
            int sheetID = 1; //Start at the first element after the instructions sheet";

            foreach (KeyValuePair<string, string> kp in lstRequiredSheets)
            {

                if (kp.Value.ToLower() == "yes")
                {
                    statusVals = new ImportStatusValues();

                    string trimmedName = kp.Key.Replace(" ", "");
                    name = XName.Get(trimmedName);
                    //Put path in app settings in web config
                    mappingTemp = ReadXMLTemplateWorksheetMapping(XDocument.Load(ConfigurationManager.AppSettings["ImplementationSpreadsheetXMLTemplatePath"] + "Excel_Import_Template_Mappings.xml").Root.Element(name), 0);

                    if (mappingTemp != null)
                    {
                        worksheet = workbook.Worksheets[mappingTemp.worksheetName];

                        expectedrecordcount += validateExcelWorksheet(ref lstWorksheets, worksheet, mappingTemp, sheetID);
                    }
                    else
                    {
                        clsLogging.saveLogItem(logID, LogReasonType.Warning, null, "Warning: The worksheet " + kp.Key + " is not a worksheet mapped for the import.");
                    }
                    sheetID++;
                }
            }

            lstWorksheets.Remove("Instructions");

        }

        /// <summary>
        /// Method to import all data from the spreadsheet into expenses now all worksheets are valid.
        /// </summary>
        /// <param name="currentUser">The current user.</param>
        /// <param name="lstWorksheets">Reference to the list of all worksheets and their corresponding columns and data.</param>
        private void insertImportData(ICurrentUser currentUser, ref Dictionary<string, List<Dictionary<string, object>>> lstWorksheets)
        {
            string worksheetName = "";

            cImportInfo importInfo = (cImportInfo)HttpRuntime.Cache["SpreadsheetImport" + AccountID];

            SortedList<int, ImportStatusValues> lstSheetStatus = importInfo.worksheets;

            for (int i = 1; i <= lstSheetStatus.Count; i++)
            {
                ImportStatusValues statusVals = lstSheetStatus[i];

                statusVals.status = WorksheetStatus.Importing;

                lstSheetStatus[i] = statusVals;
            }

            importInfo.worksheets = lstSheetStatus;
            importInfo.importStatus = ImportStatus.Valid;

            int indexofSheet = 1;
            int processedRows = 0;
            processedrecordcount = 0;

            foreach (KeyValuePair<string, List<Dictionary<string, object>>> kp in lstWorksheets)
            {
                worksheetName = kp.Key;
                processedRows = 0;

                clsLogging.saveLogItem(logID, LogReasonType.None, null, cLoggingValues.filler1 + " Saving " + worksheetName + ". " + cLoggingValues.filler1);

                switch (worksheetName)
                {
                    case "Countries":
                        foreach (Dictionary<string, object> obj in kp.Value)
                        {
                            processedrecordcount++;
                            processedRows++;
                            saveCountry(obj);
                            updateSession(indexofSheet, worksheetName, kp.Value.Count, processedRows, ImportStatus.Importing, WorksheetStatus.Importing);
                        }
                        break;

                    case "Currencies":
                        foreach (Dictionary<string, object> obj in kp.Value)
                        {
                            processedrecordcount++;
                            processedRows++;
                            saveCurrency(obj);
                            updateSession(indexofSheet, worksheetName, kp.Value.Count, processedRows, ImportStatus.Importing, WorksheetStatus.Importing);
                        }
                        break;
                    case "Departments":
                        foreach (Dictionary<string, object> obj in kp.Value)
                        {
                            processedrecordcount++;
                            processedRows++;
                            saveDepartment(obj);
                            updateSession(indexofSheet, worksheetName, kp.Value.Count, processedRows, ImportStatus.Importing, WorksheetStatus.Importing);
                        }
                        break;
                    case "Cost Codes":
                        foreach (Dictionary<string, object> obj in kp.Value)
                        {
                            processedrecordcount++;
                            processedRows++;
                            saveCostcode(obj);
                            updateSession(indexofSheet, worksheetName, kp.Value.Count, processedRows, ImportStatus.Importing, WorksheetStatus.Importing);
                        }
                        break;
                    case "Project Codes":
                        foreach (Dictionary<string, object> obj in kp.Value)
                        {
                            processedrecordcount++;
                            processedRows++;
                            saveProjectcode(obj);
                            updateSession(indexofSheet, worksheetName, kp.Value.Count, processedRows, ImportStatus.Importing, WorksheetStatus.Importing);
                        }
                        break;
                    case "Reasons":
                        foreach (Dictionary<string, object> obj in kp.Value)
                        {
                            processedrecordcount++;
                            processedRows++;
                            saveReason(obj);
                            updateSession(indexofSheet, worksheetName, kp.Value.Count, processedRows, ImportStatus.Importing, WorksheetStatus.Importing);
                        }
                        break;
                    case "P11d Categories":
                        foreach (Dictionary<string, object> obj in kp.Value)
                        {
                            processedrecordcount++;
                            processedRows++;
                            saveP11DCat(obj);
                            updateSession(indexofSheet, worksheetName, kp.Value.Count, processedRows, ImportStatus.Importing, WorksheetStatus.Importing);
                        }
                        break;
                    case "Item Roles":
                        foreach (Dictionary<string, object> obj in kp.Value)
                        {
                            processedrecordcount++;
                            processedRows++;
                            saveItemRole(obj);
                            updateSession(indexofSheet, worksheetName, kp.Value.Count, processedRows, ImportStatus.Importing, WorksheetStatus.Importing);
                        }
                        break;
                    case "Expense Categories":
                        foreach (Dictionary<string, object> obj in kp.Value)
                        {
                            processedrecordcount++;
                            processedRows++;
                            saveExpenseCategory(obj);
                            updateSession(indexofSheet, worksheetName, kp.Value.Count, processedRows, ImportStatus.Importing, WorksheetStatus.Importing);
                        }
                        break;
                    case "Expense Items":
                        foreach (Dictionary<string, object> obj in kp.Value)
                        {
                            processedrecordcount++;
                            processedRows++;
                            saveSubcat(obj);
                            updateSession(indexofSheet, worksheetName, kp.Value.Count, processedRows, ImportStatus.Importing, WorksheetStatus.Importing);
                        }
                        break;
                    case "Vehicle Journey Rates":
                        foreach (Dictionary<string, object> obj in kp.Value)
                        {
                            processedrecordcount++;
                            processedRows++;
                            saveMileageCats(currentUser, obj);
                            updateSession(indexofSheet, worksheetName, kp.Value.Count, processedRows, ImportStatus.Importing, WorksheetStatus.Importing);
                        }
                        break;
                    case "Access Roles":
                        foreach (Dictionary<string, object> obj in kp.Value)
                        {
                            processedrecordcount++;
                            processedRows++;
                            saveRoles(currentUser, obj);
                            updateSession(indexofSheet, worksheetName, kp.Value.Count, processedRows, ImportStatus.Importing, WorksheetStatus.Importing);
                        }
                        break;
                    case "Employee Details":
                        foreach (Dictionary<string, object> obj in kp.Value)
                        {
                            processedrecordcount++;
                            processedRows++;
                            saveEmployee(obj);
                            updateSession(indexofSheet, worksheetName, kp.Value.Count, processedRows, ImportStatus.Importing, WorksheetStatus.Importing);
                        }
                        break;
                    case "Employee Line Managers":
                        foreach (Dictionary<string, object> obj in kp.Value)
                        {
                            processedrecordcount++;
                            processedRows++;
                            saveEmployeeLineManager(obj);
                            updateSession(indexofSheet, worksheetName, kp.Value.Count, processedRows, ImportStatus.Importing, WorksheetStatus.Importing);
                        }
                        break;
                    case "ESR Assignments":
                        foreach (Dictionary<string, object> obj in kp.Value)
                        {
                            processedrecordcount++;
                            processedRows++;
                            saveESRAssignment(obj);
                            updateSession(indexofSheet, worksheetName, kp.Value.Count, processedRows, ImportStatus.Importing, WorksheetStatus.Importing);
                        }
                        break;
                    case "Teams":
                        foreach (Dictionary<string, object> obj in kp.Value)
                        {
                            processedrecordcount++;
                            processedRows++;
                            saveTeam(obj);
                            updateSession(indexofSheet, worksheetName, kp.Value.Count, processedRows, ImportStatus.Importing, WorksheetStatus.Importing);
                        }
                        break;
                    case "Team Allocation":
                        foreach (Dictionary<string, object> obj in kp.Value)
                        {
                            processedrecordcount++;
                            processedRows++;
                            saveTeamAllocation(obj);
                            updateSession(indexofSheet, worksheetName, kp.Value.Count, processedRows, ImportStatus.Importing, WorksheetStatus.Importing);
                        }
                        break;
                    case "Budget Holders":
                        foreach (Dictionary<string, object> obj in kp.Value)
                        {
                            processedrecordcount++;
                            processedRows++;
                            saveBudgetHolder(obj);
                            updateSession(indexofSheet, worksheetName, kp.Value.Count, processedRows, ImportStatus.Importing, WorksheetStatus.Importing);
                        }
                        break;
                    case "Signoff Groups":
                        foreach (Dictionary<string, object> obj in kp.Value)
                        {
                            processedrecordcount++;
                            processedRows++;
                            saveSignoff(currentUser, obj);
                            updateSession(indexofSheet, worksheetName, kp.Value.Count, processedRows, ImportStatus.Importing, WorksheetStatus.Importing);
                        }
                        break;
                    case "Employee Approval":
                        foreach (Dictionary<string, object> obj in kp.Value)
                        {
                            processedrecordcount++;
                            processedRows++;
                            saveEmployeeSignoff(obj);
                            updateSession(indexofSheet, worksheetName, kp.Value.Count, processedRows, ImportStatus.Importing, WorksheetStatus.Importing);
                        }
                        break;
                    case "Employee Code Allocation":
                        foreach (Dictionary<string, object> obj in kp.Value)
                        {
                            processedrecordcount++;
                            processedRows++;
                            saveEmployeeCodeAllocation(obj);
                            updateSession(indexofSheet, worksheetName, kp.Value.Count, processedRows, ImportStatus.Importing, WorksheetStatus.Importing);
                        }
                        break;
                    case "Employee Credit Cards":
                        foreach (Dictionary<string, object> obj in kp.Value)
                        {
                            processedrecordcount++;
                            processedRows++;
                            saveEmployeeCreditCard(obj);
                            updateSession(indexofSheet, worksheetName, kp.Value.Count, processedRows, ImportStatus.Importing, WorksheetStatus.Importing);
                        }
                        break;
                    case "Employee Cars":
                        foreach (Dictionary<string, object> obj in kp.Value)
                        {
                            processedrecordcount++;
                            processedRows++;
                            saveCar(obj);
                            updateSession(indexofSheet, worksheetName, kp.Value.Count, processedRows, ImportStatus.Importing, WorksheetStatus.Importing);
                        }
                        break;
                    case "Pool Cars":
                        foreach (Dictionary<string, object> obj in kp.Value)
                        {
                            processedrecordcount++;
                            processedRows++;
                            saveCar(obj);
                            updateSession(indexofSheet, worksheetName, kp.Value.Count, processedRows, ImportStatus.Importing, WorksheetStatus.Importing);
                        }
                        break;
                    case "Pool Car Allocation":
                        foreach (Dictionary<string, object> obj in kp.Value)
                        {
                            processedrecordcount++;
                            processedRows++;
                            savePoolCarUser(obj);
                            updateSession(indexofSheet, worksheetName, kp.Value.Count, processedRows, ImportStatus.Importing, WorksheetStatus.Importing);
                        }
                        break;
                    default:
                        break;
                }

                updateSession(indexofSheet, worksheetName, 0, processedRows, ImportStatus.Importing, WorksheetStatus.Imported);
                indexofSheet++;
            }

            updateSession(indexofSheet - 1, "", 0, 100, ImportStatus.Complete, WorksheetStatus.Imported);
        }

        private void saveCountry(Dictionary<string, object> lstCountries)
        {
            cElements clsElements = new cElements();
            cElement element = clsElements.GetElementByName("Countries");

            // ONLY WORKS FOR A SINGLE SUBACCOUNT AT PRESENT.
            cAccountSubAccounts subaccs = new cAccountSubAccounts(AccountID);
            int subAccountID = subaccs.getFirstSubAccount().SubAccountID;

            cCountries clsCountries = new cCountries(AccountID, subAccountID);
            cGlobalCountries clsGlobalCountries = new cGlobalCountries();
            cCountry country = null;

            string countryRequired = lstCountries["Do you incur expenses from this country?"].ToString();

            if (countryRequired.ToLower() == "yes")
            {
                string countryName = lstCountries["Country"].ToString();

                cGlobalCountry globCountry = clsGlobalCountries.getCountryByName(countryName);

                if (globCountry != null)
                {
                    if (exists("Countries", "globalcountryid", globCountry.GlobalCountryId.ToString()))
                    {
                        country = clsCountries.getCountryByGlobalCountryId(globCountry.GlobalCountryId);
                        clsCountries.saveCountry(country);

                        clsLogging.saveLogItem(logID, LogReasonType.SuccessUpdate, element, "Updated Country " + countryName);
                    }
                    else
                    {
                        country = new cCountry(0, globCountry.GlobalCountryId, false, new Dictionary<int, ForeignVatRate>(), DateTime.UtcNow, employeeid);
                        clsCountries.saveCountry(country);
                        clsLogging.saveLogItem(logID, LogReasonType.SuccessAdd, element, "Added Country " + countryName);
                    }
                }
                else
                {
                    clsLogging.saveLogItem(logID, LogReasonType.Warning, element, "Country " + countryName + " is invalid");
                }


            }
        }

        private void saveCurrency(Dictionary<string, object> lstCurrencies)
        {
            cElements clsElements = new cElements();
            cElement element = clsElements.GetElementByName("Currencies");

            // ONLY WORKS FOR A SINGLE SUBACCOUNT AT PRESENT.
            cAccountSubAccounts subaccs = new cAccountSubAccounts(AccountID);
            int subAccountID = subaccs.getFirstSubAccount().SubAccountID;

            cCurrencies clsCurrencies = new cCurrencies(AccountID, subAccountID);
            cGlobalCurrencies clsGlobalCurrencies = new cGlobalCurrencies();

            string currencyRequired = lstCurrencies["Do you incur expenses using this currency?"].ToString();

            if (currencyRequired.ToLower() == "yes")
            {
                string currencyName = lstCurrencies["Currency"].ToString();

                cGlobalCurrency globCurrency = clsGlobalCurrencies.getGlobalCurrencyByLabel(currencyName);
                cCurrency currency = null;

                if (globCurrency != null)
                {
                    cStaticCurrencies clsStaticCurrs = new cStaticCurrencies(AccountID, subAccountID);

                    if (exists("Currencies", "globalcurrencyid", globCurrency.globalcurrencyid.ToString()))
                    {
                        currency = clsCurrencies.getCurrencyByGlobalCurrencyId(globCurrency.globalcurrencyid);

                        clsLogging.saveLogItem(logID, LogReasonType.Warning, element, "Currency " + currencyName + " already exists and has not been added.");
                    }
                    else
                    {
                        currency = new cCurrency(AccountID, 0, globCurrency.globalcurrencyid, 0, 0, false, DateTime.UtcNow, employeeid, null, null);
                        clsStaticCurrs.saveCurrency(currency);
                        clsLogging.saveLogItem(logID, LogReasonType.SuccessAdd, element, "Added Currency " + currencyName);
                    }
                }
                else
                {
                    clsLogging.saveLogItem(logID, LogReasonType.Warning, element, "Warning: Currency " + currencyName + " is invalid");
                }
            }
        }

        private void saveDepartment(Dictionary<string, object> lstDepartments)
        {
            cElements clsElements = new cElements();
            cElement element = clsElements.GetElementByName("Departments");

            cDepartments clsDepartments = new cDepartments(AccountID);
            string department = lstDepartments["Department Code"].ToString();
            string departmentDesc = lstDepartments["Department Code Description"].ToString();

            if (exists("Departments", "department", department))
            {
                cDepartment reqDepartment = clsDepartments.GetDepartmentByName(department);

                if (reqDepartment != null)
                {
                    clsDepartments.SaveDepartment(new cDepartment(reqDepartment.DepartmentId, department, departmentDesc, reqDepartment.Archived, reqDepartment.CreatedOn, reqDepartment.CreatedBy, DateTime.UtcNow, employeeid, new SortedList<int, object>()));

                    clsLogging.saveLogItem(logID, LogReasonType.SuccessUpdate, element, "Updated Department " + department);
                }
            }
            else
            {
                clsDepartments.SaveDepartment(new cDepartment(0, department, departmentDesc, false, DateTime.UtcNow, employeeid, null, null, new SortedList<int, object>()));
                clsLogging.saveLogItem(logID, LogReasonType.SuccessAdd, element, "Added Department " + department);
            }
        }

        private void saveCostcode(Dictionary<string, object> lstCostcodes)
        {
            cElements clsElements = new cElements();
            cElement element = clsElements.GetElementByName("Cost Codes");

            cCostcodes clsCostcodes = new cCostcodes(AccountID);
            string costcode = lstCostcodes["Cost Code"].ToString();
            string costcodeDesc = lstCostcodes["Cost Code Description"].ToString();
            cCostCode reqCostcode = null;

            if (exists("costcodes", "costcode", costcode))
            {
                reqCostcode = clsCostcodes.GetCostcodeByString(costcode);

                if (reqCostcode != null)
                {
                    clsCostcodes.SaveCostcode(new cCostCode(reqCostcode.CostcodeId, costcode, costcodeDesc, reqCostcode.Archived, reqCostcode.CreatedOn, reqCostcode.CreatedBy, DateTime.UtcNow, employeeid, reqCostcode.UserdefinedFields, reqCostcode.OwnerEmployeeId, reqCostcode.OwnerTeamId, reqCostcode.OwnerBudgetHolderId));

                    clsLogging.saveLogItem(logID, LogReasonType.SuccessUpdate, element, "Updated Cost Code " + costcode);
                }
            }
            else
            {
                reqCostcode = new cCostCode(0, costcode, costcodeDesc, false, DateTime.UtcNow, employeeid, null, null, new SortedList<int, object>(), null, null, null);
                clsCostcodes.SaveCostcode(reqCostcode);
                clsLogging.saveLogItem(logID, LogReasonType.SuccessAdd, element, "Added Cost Code " + costcode);
            }
        }

        private void saveProjectcode(Dictionary<string, object> lstProjectcodes)
        {
            cElements clsElements = new cElements();
            cElement element = clsElements.GetElementByName("Project Codes");

            cProjectCodes clsProjectcodes = new cProjectCodes(AccountID);
            string projectcode = lstProjectcodes["Project Code"].ToString();
            string projectcodeDesc = lstProjectcodes["Project Code Description"].ToString();
            cProjectCode reqProjectcode = null;

            if (exists("project_codes", "projectcode", projectcode))
            {
                reqProjectcode = clsProjectcodes.GetProjectCodeByName(projectcode);

                if (reqProjectcode != null)
                {
                    clsProjectcodes.SaveProjectCode(new cProjectCode(reqProjectcode.projectcodeid, projectcode, projectcodeDesc, false));

                    clsLogging.saveLogItem(logID, LogReasonType.SuccessUpdate, element, "Updated Project Code " + projectcode);
                }
            }
            else
            {
                reqProjectcode = new cProjectCode(0, projectcode, projectcodeDesc, false);
                clsProjectcodes.SaveProjectCode(reqProjectcode);
                clsLogging.saveLogItem(logID, LogReasonType.SuccessAdd, element, "Added Project Code " + projectcode);
            }
        }

        private void saveReason(Dictionary<string, object> lstReasons)
        {
            cElements clsElements = new cElements();
            cElement element = clsElements.GetElementByName("Reasons");

            cReasons clsReasons = new cReasons(AccountID);
            string reason = lstReasons["Reason"].ToString();
            string reasonDesc = lstReasons["Reason Description"].ToString();
            cReason reqReason = null;

            if (exists("reasons", "reason", reason))
            {
                reqReason = clsReasons.getReasonByString(reason);

                if (reqReason != null)
                {
                    clsReasons.saveReason(new cReason(AccountID, reqReason.reasonid, reason, reasonDesc, reqReason.accountcodevat, reqReason.accountcodenovat, reqReason.createdon, reqReason.createdby, DateTime.UtcNow, employeeid, reqReason.Archive));

                    clsLogging.saveLogItem(logID, LogReasonType.SuccessUpdate, element, "Updated Reason " + reason);
                }
            }
            else
            {
                reqReason = new cReason(AccountID, 0, reason, reasonDesc, "", "", DateTime.UtcNow, employeeid, null, null, false);
                clsReasons.saveReason(reqReason);
                clsLogging.saveLogItem(logID, LogReasonType.SuccessAdd, element, "Added Reason " + reason);
            }

        }

        private void saveP11DCat(Dictionary<string, object> lstP11Dcats)
        {
            cElements clsElements = new cElements();
            cElement element = clsElements.GetElementByName("P11D");
            
            string p11DCat = lstP11Dcats["P11d Category Description"].ToString();

            var p11DCategoryId =
                this.GetIdByTableGuidFieldGuidAndValue(new Guid(ReportTable.P11DCategories),
                    new Guid(ReportFields.P11DCategoriesPDName), p11DCat);
            if (p11DCategoryId > 0)
            {
                this.P11DCategoriesRepository.Add(new P11DCategory(p11DCategoryId, p11DCat));
                clsLogging.saveLogItem(logID, LogReasonType.SuccessUpdate, element, "Updated P11D " + p11DCat);
            }
            else
            {
                this.P11DCategoriesRepository.Add(new P11DCategory(0, p11DCat));
                clsLogging.saveLogItem(logID, LogReasonType.SuccessAdd, element, "Added P11D " + p11DCat);
            }
        }

        private void saveItemRole(Dictionary<string, object> lstItemRoles)
        {
            cElements clsElements = new cElements();
            cElement element = clsElements.GetElementByName("ItemRoles");

            cItemRoles clsItemRoles = new cItemRoles(AccountID);
            string itemRole = lstItemRoles["Item Role Description"].ToString();

            if (exists("item_roles", "rolename", itemRole))
            {
                cItemRole reqItemRole = clsItemRoles.getItemRoleByName(itemRole);

                clsItemRoles.updateRole(reqItemRole.itemroleid, itemRole, reqItemRole.description, new List<cRoleSubcat>(), employeeid);
                clsLogging.saveLogItem(logID, LogReasonType.SuccessUpdate, element, "Updated Item Role " + itemRole);
            }
            else
            {
                clsItemRoles.addRole(itemRole, "", new List<cRoleSubcat>(), employeeid);
                clsLogging.saveLogItem(logID, LogReasonType.SuccessAdd, element, "Added Item Role " + itemRole);
            }

        }

        private void saveExpenseCategory(Dictionary<string, object> lstCategories)
        {
            cElements clsElements = new cElements();
            cElement element = clsElements.GetElementByName("ExpenseCategories");

            cCategories clsCategories = new cCategories(AccountID);
            string category = lstCategories["Expense Category Description"].ToString();

            if (exists("categories", "category", category))
            {
                cCategory reqCategory = clsCategories.getCategoryByName(category);

                clsCategories.updateCategory(reqCategory.categoryid, category, reqCategory.description, employeeid);

                clsLogging.saveLogItem(logID, LogReasonType.SuccessUpdate, element, "Updated Category " + category);
            }
            else
            {
                clsCategories.addCategory(category, "", employeeid);
                clsLogging.saveLogItem(logID, LogReasonType.SuccessAdd, element, "Added Category " + category);
            }
        }

        private void saveSubcat(Dictionary<string, object> lstSubcats)
        {
            cElements clsElements = new cElements();
            cElement element = clsElements.GetElementByName("ExpenseItems");

            cSubcats clsSubcats = new cSubcats(AccountID);
            cCategories clsCategories = new cCategories(AccountID);
            cItemRoles clsItemRoles = new cItemRoles(AccountID);

            string masterCat = lstSubcats["Master Category"].ToString();
            string expenseItem = lstSubcats["Expense Items"].ToString();
            string shortSubcat = lstSubcats["Abbreviation"].ToString();
            string itemRole = lstSubcats["Expense Item Role"].ToString();
            string accountCode = lstSubcats["Account Code"].ToString();
            string altAccountCode = lstSubcats["Alternate Account Code"].ToString();
            string reimbursable = lstSubcats["Reimbursable"].ToString();
            string applicable = lstSubcats["Applicable"].ToString();
            string rate = lstSubcats["Rate"].ToString();
            string percentReclaimed = lstSubcats["Percentage Reclaimed"].ToString();
            string comment = lstSubcats["Comments to be shown to claimants"].ToString();
            string p11D = lstSubcats["P11d Category"].ToString();
            string calculationType = lstSubcats["Calculation Type"].ToString();
            string totalEntAs = lstSubcats["Total entered as"].ToString();
            string allowance = lstSubcats["This expense is an allowance"].ToString();
            string allowanceAmount = lstSubcats["Allowance Amount"].ToString();

            cSubcat newSubcat;
            
            cItemRole reqItemRole = clsItemRoles.getItemRoleByName(itemRole);
            cCategory reqCategory = clsCategories.getCategoryByName(masterCat);

            if (reqItemRole == null)
            {
                clsLogging.saveLogItem(logID, LogReasonType.Error, element, "The item Role " + itemRole + " referenced is not a valid item role.");
                return;
            }

            if (reqCategory == null)
            {
                clsLogging.saveLogItem(logID, LogReasonType.Error, element, "The expense category " + masterCat + " referenced is not a valid expense category.");
                return;
            }

            int categoryid = reqCategory.categoryid;
            int itemroleid = reqItemRole.itemroleid;
            int p11Did = this.GetIdByTableGuidFieldGuidAndValue(new Guid(ReportTable.P11DCategories),
                new Guid(ReportFields.P11DCategoriesPDName), p11D);

            bool isReimbursable;
            //bool isAllowance;
            bool isVatApplicable;
            bool isNet;
            CalculationType calculation = CalculationType.NormalItem;


            #region Role subcats

            List<cRoleSubcat> rolesubs = new List<cRoleSubcat>();

            rolesubs.Add(new cRoleSubcat(0, itemroleid, 0, 0, 0, false));

            #endregion

            #region Vat Rates

            List<cSubcatVatRate> lstVatRate = new List<cSubcatVatRate>();

            string sVatAmount = rate.Replace("%", "");
            double vatamount = 0;

            double.TryParse(sVatAmount, out vatamount);

            string sVatPercent = percentReclaimed.Replace("%", "");
            byte vatPercent = 0;

            byte.TryParse(sVatPercent, out vatPercent);

            lstVatRate.Add(new cSubcatVatRate(0, 0, vatamount, false, null, null, vatPercent, DateRangeType.Any, new DateTime(1900, 01, 01), null));

            #endregion

            if (reimbursable.ToLower() == "yes")
            {
                isReimbursable = true;
            }
            else
            {
                isReimbursable = false;
            }

            if (applicable.ToLower() == "yes")
            {
                isVatApplicable = true;

            }
            else
            {
                isVatApplicable = false;
            }

            //if (allowance.ToLower() == "yes")
            //{
            //    isAllowance = true;
            //}
            //else
            //{
            //    isAllowance = false;
            //}

            if (totalEntAs.ToLower() == "net")
            {
                isNet = true;
            }
            else
            {
                isNet = false;
            }

            #region Set Calculation Type

            switch (calculationType)
            {
                case "Normal Item":

                    calculation = CalculationType.NormalItem;
                    break;

                case "Meal":

                    calculation = CalculationType.Meal;
                    break;

                case "Pence Per Mile":

                    calculation = CalculationType.PencePerMile;
                    break;

                case "Pence Per Mile Receipt":

                    calculation = CalculationType.PencePerMileReceipt;
                    break;

                case "Fuel Receipt":

                    calculation = CalculationType.FuelReceipt;
                    break;

                case "Daily Allowance":

                    calculation = CalculationType.DailyAllowance;
                    break;
                default:
                    break;
            }

            #endregion

            decimal dAllowanceAmount = 0;

            decimal.TryParse(allowanceAmount, out dAllowanceAmount);

            if (exists("subcats", "subcat", expenseItem))
            {
                cSubcat reqSubcat = clsSubcats.GetSubcatByString(expenseItem);

                if (reqSubcat != null)
                {
                    newSubcat = new cSubcat(reqSubcat.subcatid, categoryid, expenseItem, reqSubcat.description, reqSubcat.mileageapp, reqSubcat.staffapp, reqSubcat.othersapp, reqSubcat.tipapp, reqSubcat.pmilesapp, reqSubcat.bmilesapp, dAllowanceAmount, accountCode, reqSubcat.attendeesapp, isNet, p11Did, reqSubcat.eventinhomeapp, reqSubcat.receiptapp, calculation, reqSubcat.passengersapp, reqSubcat.nopassengersapp, reqSubcat.passengernamesapp, comment, reqSubcat.splitentertainment, reqSubcat.entertainmentid, isReimbursable, reqSubcat.nonightsapp, reqSubcat.attendeesapp, reqSubcat.nodirectorsapp, reqSubcat.hotelapp, reqSubcat.noroomsapp, reqSubcat.hotelmand, isVatApplicable, reqSubcat.vatnumbermand, reqSubcat.nopersonalguestsapp, reqSubcat.noremoteworkersapp, altAccountCode, reqSubcat.splitpersonal, reqSubcat.splitremote, reqSubcat.personalid, reqSubcat.remoteid, reqSubcat.reasonapp, reqSubcat.otherdetailsapp, reqSubcat.userdefined, reqSubcat.createdon, reqSubcat.createdby, DateTime.UtcNow, employeeid, shortSubcat, reqSubcat.fromapp, reqSubcat.toapp, reqSubcat.countries, reqSubcat.allowances, reqSubcat.associatedudfs, reqSubcat.subcatsplit, reqSubcat.companyapp, lstVatRate, reqSubcat.EnableHomeToLocationMileage, reqSubcat.HomeToLocationType, reqSubcat.MileageCategory, reqSubcat.IsRelocationMileage, reqSubcat.reimbursableSubcatID, false, reqSubcat.HomeToOfficeAlwaysZero, reqSubcat.HomeToOfficeFixedMiles, reqSubcat.PublicTransportRate);

                    clsSubcats.SaveSubcat(newSubcat);
                }

                clsLogging.saveLogItem(logID, LogReasonType.SuccessUpdate, element, "Updated Expense Item " + expenseItem);
            }
            else
            {
                newSubcat = new cSubcat(0, categoryid, expenseItem, "", false, false, false, false, false, false, dAllowanceAmount, accountCode, false, false, p11Did, false, false, calculation, false, false, false, comment, false, 0, isReimbursable, false, false, false, false, false, false, false, false, false, false, altAccountCode, false, false, 0, 0, false, false, new SortedList<int, object>(), DateTime.UtcNow, employeeid, null, null, shortSubcat, false, false, new List<cCountrySubcat>(), new List<int>(), new List<int>(), new List<int>(), false, lstVatRate, false, HomeToLocationType.None, null, false, 0, false, false, 0, null);
                clsSubcats.SaveSubcat(newSubcat);

                clsLogging.saveLogItem(logID, LogReasonType.SuccessAdd, element, "Added Expense Item " + expenseItem);
            }
        }

        private void saveMileageCats(ICurrentUser currentUser, Dictionary<string, object> lstMileageCats)
        {
            cElements clsElements = new cElements();
            cElement element = clsElements.GetElementByName("VehicleJourneyRateCategories");

            cMileagecats clsMileageCats = new cMileagecats(AccountID);

            var financialYear = FinancialYear.GetPrimary(AccountID);

            string mileageCat = lstMileageCats["Category Name"].ToString();

            int rangeValue1;
            int.TryParse(lstMileageCats["Threshold (Annual)"].ToString(), out rangeValue1);

            decimal petrolBeforeRate = 0;
            decimal.TryParse(lstMileageCats["Before Threshold Petrol"].ToString(), out petrolBeforeRate);

            decimal petrolAfterRate = 0;
            decimal.TryParse(lstMileageCats["After Threshold Petrol"].ToString(), out petrolAfterRate);

            decimal amountForVATPetrol = 0;
            decimal.TryParse(lstMileageCats["Amount for VAT Petrol"].ToString(), out amountForVATPetrol);

            decimal dieselBeforeRate = 0;
            decimal.TryParse(lstMileageCats["Before Threshold Diesel"].ToString(), out dieselBeforeRate);

            decimal dieselAfterRate = 0;
            decimal.TryParse(lstMileageCats["After Threshold Diesel"].ToString(), out dieselAfterRate);

            decimal amountForVATDiesel = 0;
            decimal.TryParse(lstMileageCats["Amount for VAT Diesel"].ToString(), out amountForVATDiesel);

            decimal lpgBeforeRate = 0;
            decimal.TryParse(lstMileageCats["Before Threshold LPG"].ToString(), out lpgBeforeRate);

            decimal lpgAfterRate = 0;
            decimal.TryParse(lstMileageCats["After Threshold LPG"].ToString(), out lpgAfterRate);

            decimal amountForVATLpg = 0;
            decimal.TryParse(lstMileageCats["Amount for VAT LPG"].ToString(), out amountForVATLpg);

            decimal passenger1 = 0;
            decimal.TryParse(lstMileageCats["Passenger 1"].ToString(), out passenger1);

            decimal otherPassengers = 0;
            decimal.TryParse(lstMileageCats["Other Passengers"].ToString(), out otherPassengers);

            cAccountSubAccounts clsSubAccounts = new cAccountSubAccounts(AccountID);
            cAccountProperties reqProperties = clsSubAccounts.getFirstSubAccount().SubAccountProperties.Clone();
            int mileageCatID = 0;

            var engines = new List<VehicleEngineType>(VehicleEngineType.GetAll(currentUser));

            if (exists("mileage_categories", "carsize", mileageCat))
            {
                cMileageCat reqMileageCat = clsMileageCats.getMileageCatByName(mileageCat);
                clsMileageCats.saveVehicleJourneyRate(new cMileageCat(reqMileageCat.mileageid, mileageCat, reqMileageCat.comment, reqMileageCat.thresholdType, reqMileageCat.calcmilestotal, reqMileageCat.dateRanges, reqMileageCat.mileUom, reqMileageCat.catvalid, reqMileageCat.catvalidcomment, reqMileageCat.currencyid, reqMileageCat.createdon, reqMileageCat.createdby, DateTime.UtcNow, employeeid, string.Empty, 0, 0, financialYear.FinancialYearID));
                mileageCatID = reqMileageCat.mileageid;
                clsLogging.saveLogItem(logID, LogReasonType.SuccessUpdate, element, "Updated Vehicle Journey Rate Category " + mileageCat);
                //Make the sure the date range type is 'Any' or the wrong date range will get updated 
                if (reqMileageCat.dateRanges[0].daterangetype == DateRangeType.Any)
                {
                    cMileageDaterange range = reqMileageCat.dateRanges[0];

                    if (range != null)
                    {
                        clsMileageCats.saveDateRange(new cMileageDaterange(range.mileagedateid, reqMileageCat.mileageid, range.dateValue1, range.dateValue2, range.thresholds, range.daterangetype, range.createdon, range.createdby, DateTime.UtcNow, employeeid), reqMileageCat.mileageid);

                        foreach (cMileageThreshold clsThreshold in range.thresholds)
                        {
                            var threshold = new cMileageThreshold(clsThreshold.MileageThresholdId, range.mileagedateid, rangeValue1, clsThreshold.RangeValue2, clsThreshold.RangeType, passenger1, otherPassengers, clsThreshold.CreatedOn, clsThreshold.CreatedBy, DateTime.UtcNow, employeeid, clsThreshold.HeavyBulkyEquipment);

                            switch (clsThreshold.RangeType)
                            {
                                case RangeType.LessThan:
                                    //Add Before rate
                                    var beforeThresholdId = clsMileageCats.saveThreshold(mileageCatID, threshold);
                                    this.SaveMileageThresholdRates(currentUser, engines, beforeThresholdId, petrolBeforeRate, dieselBeforeRate, lpgBeforeRate, amountForVATPetrol, amountForVATDiesel, amountForVATLpg);
                                    break;

                                case RangeType.GreaterThanOrEqualTo:
                                    //Add After rate
                                    var afterThresholdId = clsMileageCats.saveThreshold(mileageCatID, threshold);
                                    this.SaveMileageThresholdRates(currentUser, engines, afterThresholdId, petrolAfterRate, dieselAfterRate, lpgAfterRate, amountForVATPetrol, amountForVATDiesel, amountForVATLpg);
                                    break;
                            }
                        }
                    }
                }
            }
            else
            {
                mileageCatID = clsMileageCats.saveVehicleJourneyRate(new cMileageCat(0, mileageCat, "", ThresholdType.Annual, false, new List<cMileageDaterange>(), MileageUOM.Mile, false, "", (int)reqProperties.BaseCurrency, DateTime.UtcNow, employeeid, null, null, string.Empty, 0, 0, financialYear.FinancialYearID));
                int mileageDateID = clsMileageCats.saveDateRange(new cMileageDaterange(0, mileageCatID, null, null, new List<cMileageThreshold>(), DateRangeType.Any, DateTime.UtcNow, employeeid, null, null), mileageCatID);

                //Add Before rate
                var beforeThresholdId = clsMileageCats.saveThreshold(mileageCatID, new cMileageThreshold(0, mileageDateID, rangeValue1, 0, RangeType.LessThan, passenger1, otherPassengers, DateTime.UtcNow, employeeid, null, null, 0));
                this.SaveMileageThresholdRates(currentUser, engines, beforeThresholdId, petrolBeforeRate, dieselBeforeRate, lpgBeforeRate, amountForVATPetrol, amountForVATDiesel, amountForVATLpg);

                //Add After rate
                var afterThresholdId = clsMileageCats.saveThreshold(mileageCatID, new cMileageThreshold(0, mileageDateID, rangeValue1, 0, RangeType.GreaterThanOrEqualTo, passenger1, otherPassengers, DateTime.UtcNow, employeeid, null, null, 0));
                this.SaveMileageThresholdRates(currentUser, engines, afterThresholdId, petrolAfterRate, dieselAfterRate, lpgAfterRate, amountForVATPetrol, amountForVATDiesel, amountForVATLpg);

                clsLogging.saveLogItem(logID, LogReasonType.SuccessAdd, element, "Added Vehicle Journey Rate Category " + mileageCat);
            }
        }

        private void SaveMileageThresholdRates(ICurrentUser currentUser, List<VehicleEngineType> engines, int mileageThresholdId, decimal petrolRatePerUnit, decimal dieselRatePerUnit, decimal lpgRatePerUnit, decimal petrolAmountForVat, decimal dieselAmountForVat, decimal lpgAmountForVat)
        {
            VehicleJourneyRateThresholdRate.CreateOrUpdate(currentUser, currentUser.Account.IsNHSCustomer, engines,
                "Petrol", "Petrol", mileageThresholdId, petrolRatePerUnit, petrolAmountForVat);

            VehicleJourneyRateThresholdRate.CreateOrUpdate(currentUser, currentUser.Account.IsNHSCustomer, engines,
                "Diesel", "Diesel", mileageThresholdId, dieselRatePerUnit, dieselAmountForVat);

            VehicleJourneyRateThresholdRate.CreateOrUpdate(currentUser, currentUser.Account.IsNHSCustomer, engines,
                "LPG", "Liquid Petroleum Gas", mileageThresholdId, lpgRatePerUnit, lpgAmountForVat);

        }

        private void saveRoles(ICurrentUserBase currentUser, Dictionary<string, object> lstRoles)
        {
            cElements clsElements = new cElements();
            cElement element = clsElements.GetElementByName("AccessRoles");

            int? delID = null;

            if (currentUser != null)
            {
                if (currentUser.isDelegate == true)
                {
                    delID = currentUser.Delegate.EmployeeID;
                }
            }

            cAccessRoles clsAccessRoles = new cAccessRoles(AccountID, cAccounts.getConnectionString(AccountID));

            string roleName = lstRoles["Access Role Description"].ToString();
            string roleType = lstRoles["Access Role Type"].ToString();

            if (!exists("accessRoles", "roleName", roleName))
            {
                //    cAccessRole role = clsAccessRoles.GetAccessRoleByName(roleName);

                //    if (role != null)
                //    {
                //        clsAccessRoles.SaveAccessRole(employeeid, role.RoleID, roleName, role.Description, (Int16)role.AccessLevel, new object[,] { { null }, { null } }, role.ExpenseClaimMaximumAmount, role.ExpenseClaimMinimumAmount, role.CanEditCostCode, role.CanEditDepartment, role.CanEditProjectCode, new object[0], delID);
                //        clsLogging.saveLogItem(logID, LogReasonType.Success, "Updated Access Role " + roleName);
                //    }

                //}
                //else
                //{
                switch (roleType)
                {
                    case "Claimant":
                        clsAccessRoles.SaveAccessRole(employeeid, 0, roleName, "", (Int32)AccessRoleLevel.SelectedRoles, new object[,] { { null }, { null } }, null, null, false, false, false, false, new object[0], delID, null, true, false, false);
                        break;

                    case "Line Manager":
                        clsAccessRoles.SaveAccessRole(employeeid, 0, roleName, "", (Int32)AccessRoleLevel.EmployeesResponsibleFor, new object[,] { { null }, { null } }, null, null, false, false, false, false, new object[0], delID, null, true, false, false);
                        break;

                    case "Administrator":
                        clsAccessRoles.SaveAccessRole(employeeid, 0, roleName, "", (Int32)AccessRoleLevel.AllData, new object[,] { { null }, { null } }, null, null, true, true, true, false, new object[0], delID, null, true, false, false);
                        break;
                    default:
                        break;
                }
                clsLogging.saveLogItem(logID, LogReasonType.SuccessAdd, element, "Added Access Role " + roleName);
            }
        }

        private void saveEmployee(Dictionary<string, object> lstEmployee)
        {
            cElements clsElements = new cElements();
            cElement element = clsElements.GetElementByName("Employees");

            // WHEN MULTIPLE SUB-ACCOUNTS SUPPORTED, THIS WILL NEED PULLING FROM THE IMPORT SPREADSHEET
            cAccountSubAccounts subaccounts = new cAccountSubAccounts(AccountID);
            int subAccountID = subaccounts.getFirstSubAccount().SubAccountID;

            cEmployees clsEmployees = new cEmployees(AccountID);
            cCountries clsCountries = new cCountries(AccountID, subAccountID);
            cGlobalCountries clsGlobalCountries = new cGlobalCountries();
            cCurrencies clsCurrencies = new cCurrencies(AccountID, subAccountID);
            cGlobalCurrencies clsGlobalCurrencies = new cGlobalCurrencies();

            string username = lstEmployee["Username"].ToString();
            string title = lstEmployee["Title"].ToString();
            string firstName = lstEmployee["First Name"].ToString();
            string middleNames = lstEmployee["Middle Names"].ToString();
            string surname = lstEmployee["Surname"].ToString();
            string maidenName = lstEmployee["Maiden Name"].ToString();
            string position = lstEmployee["Position"].ToString();
            string emailAddress = lstEmployee["Email Address"].ToString();
            string phoneNumber = lstEmployee["Phone Number"].ToString();
            string mobileNumber = lstEmployee["Mobile Number"].ToString();
            string pagerNumber = lstEmployee["Pager Number"].ToString();
            string payrollNumber = lstEmployee["Payroll Number"].ToString();

            #region Employee Access Roles

            List<int> curAccessRoles = new List<int>();
            Dictionary<int, List<int>> lstAccessRoles = new Dictionary<int, List<int>>();
            cAccessRoles clsAccessRoles = new cAccessRoles(AccountID, cAccounts.getConnectionString(AccountID));
            cAccessRole reqAccessRole;

            reqAccessRole = clsAccessRoles.GetAccessRoleByName(lstEmployee["Access Role 1"].ToString());

            if (reqAccessRole != null)
            {
                curAccessRoles.Add(reqAccessRole.RoleID);
            }

            reqAccessRole = clsAccessRoles.GetAccessRoleByName(lstEmployee["Access Role 2"].ToString());

            if (reqAccessRole != null)
            {
                curAccessRoles.Add(reqAccessRole.RoleID);
            }

            reqAccessRole = clsAccessRoles.GetAccessRoleByName(lstEmployee["Access Role 3"].ToString());

            if (reqAccessRole != null)
            {
                curAccessRoles.Add(reqAccessRole.RoleID);
            }

            lstAccessRoles.Add(subAccountID, curAccessRoles);
            #endregion

            #region Employee Item roles

            cItemRoles clsItemRoles = new cItemRoles(AccountID);
            cItemRole reqItemRole;
            List<EmployeeItemRole> lstItemRoles = new List<EmployeeItemRole>();

            reqItemRole = clsItemRoles.getItemRoleByName(lstEmployee["Item Role 1"].ToString());

            if (reqItemRole != null)
            {
                lstItemRoles.Add(new EmployeeItemRole(reqItemRole.itemroleid));
            }

            reqItemRole = clsItemRoles.getItemRoleByName(lstEmployee["Item Role 2"].ToString());

            if (reqItemRole != null)
            {
                lstItemRoles.Add(new EmployeeItemRole(reqItemRole.itemroleid));
            }

            reqItemRole = clsItemRoles.getItemRoleByName(lstEmployee["Item Role 3"].ToString());

            if (reqItemRole != null)
            {
                lstItemRoles.Add(new EmployeeItemRole(reqItemRole.itemroleid));
            }



            #endregion

            #region Employee Home and Office Locations

            CurrentUser currentUser = new CurrentUser(nAccountID, nEmployeeID, 0, Modules.expenses, subAccountID);
            Address homeAddress = null;
            // todo check this is the correct field for AddressName?
            string homeLocation = lstEmployee["Home Location"].ToString();
            string homeLine1 = lstEmployee["Home Line 1"].ToString();
            string homeCity = lstEmployee["Home City"].ToString();
            string homePostcode = lstEmployee["Home Postcode"].ToString();
            string homeCountry = lstEmployee["Home Country"].ToString();

            int countryID = 0;
            cGlobalCountry globCountry = clsGlobalCountries.getCountryByName(homeCountry);

            if (globCountry != null)
            {
                countryID = globCountry.GlobalCountryId;

                homeAddress = Address.SaveOrGetDuplicate(currentUser, 0, homeLocation, homeLine1, string.Empty, string.Empty, homeCity, string.Empty, countryID, homePostcode, string.Empty, string.Empty, string.Empty, 0, false, Address.AddressCreationMethod.ImplementationImportRoutine);
            }

            Address officeAddress = null;
            string officeLocation = lstEmployee["Office Location"].ToString();
            string officeLine1 = lstEmployee["Office Line 1"].ToString();
            string officeCity = lstEmployee["Office City"].ToString();
            string officePostcode = lstEmployee["Office Postcode"].ToString();
            string officeCountry = lstEmployee["Office Country"].ToString();

            globCountry = clsGlobalCountries.getCountryByName(officeCountry);

            if (globCountry != null)
            {
                officeAddress = Address.SaveOrGetDuplicate(currentUser, 0, officeLocation, officeLine1, string.Empty, string.Empty, officeCity, string.Empty, globCountry.GlobalCountryId, officePostcode, string.Empty, string.Empty, string.Empty, 0, false, Address.AddressCreationMethod.ImplementationImportRoutine);
            }

            #endregion

            string homeFaxNo = lstEmployee["Home Fax Number"].ToString();
            string homeEmail = lstEmployee["Home Email Address"].ToString();

            #region Get Primary country ID

            string primaryCountry = lstEmployee["Primary Country"].ToString();
            int primaryCountryID = 0;

            globCountry = clsGlobalCountries.getCountryByName(primaryCountry);

            if (globCountry != null)
            {
                cCountry country = clsCountries.getCountryByGlobalCountryId(globCountry.GlobalCountryId);

                if (country != null)
                {
                    primaryCountryID = country.CountryId;
                }
            }

            #endregion

            #region Get Primary Currency ID

            string primaryCurrency = lstEmployee["Primary Currency"].ToString();

            int primaryCurrencyID = 0;

            cGlobalCurrency globCurrency = clsGlobalCurrencies.getGlobalCurrencyByLabel(primaryCurrency);

            if (globCurrency != null)
            {
                cCurrency currency = clsCurrencies.getCurrencyByGlobalCurrencyId(globCurrency.globalcurrencyid);

                if (currency != null)
                {
                    primaryCurrencyID = currency.currencyid;
                }
            }

            #endregion

            string gender = lstEmployee["Gender"].ToString();

            string strDateOfBirth = lstEmployee["Date of Birth"].ToString();
            DateTime dateOfBirth = new DateTime(1900, 01, 01);

            if (strDateOfBirth != "")
            {
                DateTime.TryParse(strDateOfBirth, out dateOfBirth);
            }

            string natInsNum = lstEmployee["National Insurance Number"].ToString();

            string strHireDate = lstEmployee["Hire Date"].ToString();
            DateTime hireDate = new DateTime(1900, 01, 01);

            if (strHireDate != "")
            {
                DateTime.TryParse(strHireDate, out hireDate);
            }

            string strTermDate = lstEmployee["Termination Date"].ToString();
            DateTime termDate = new DateTime(1900, 01, 01);

            if (strTermDate != "")
            {
                DateTime.TryParse(strTermDate, out termDate);
            }

            string accountName = lstEmployee["Account Holder Name"].ToString();
            string accountNum = lstEmployee["Account Number"].ToString();
            string accountType = lstEmployee["Account Type"].ToString();
            string sortCode = lstEmployee["Sort Code"].ToString();
            string accountRef = lstEmployee["Account Reference"].ToString();

            int empID;

            if (exists("employees", "username", username))
            {
                empID = clsEmployees.getEmployeeidByUsername(AccountID, username);
                Employee emp = clsEmployees.GetEmployeeById(empID);

                if (emp != null)
                {
                    clsEmployees.SaveEmployee(new Employee(AccountID, empID, username, emp.Password, emailAddress, title, firstName, middleNames, maidenName, surname, emp.Active, emp.Verified, emp.Archived, emp.Locked, emp.LogonCount, emp.LogonRetryCount, emp.CreatedOn, emp.CreatedBy, DateTime.UtcNow, employeeid, new BankAccount(accountName, accountNum, accountType, sortCode, accountRef), emp.SignOffGroupID, emp.TelephoneExtensionNumber, mobileNumber, pagerNumber, homeFaxNo, homeEmail, emp.LineManager, emp.AdvancesSignOffGroup, emp.PreferredName, gender, dateOfBirth, hireDate, termDate, payrollNumber, position, emp.TelephoneNumber, emp.Creditor, emp.CreationMethod, PasswordEncryptionMethod.RijndaelManaged, emp.FirstLogon, emp.AdminOverride, emp.DefaultSubAccount, primaryCurrencyID, primaryCountryID, emp.CreditCardSignOffGroup, emp.PurchaseCardSignOffGroup, emp.HasCustomisedAddItems, emp.LocaleID, emp.NhsTrustID, natInsNum, emp.EmployeeNumber, emp.NhsUniqueID, emp.EsrPersonID, emp.EsrEffectiveStartDate, emp.EsrEffectiveEndDate, emp.CurrentClaimNumber, emp.LastChange, emp.CurrentReferenceNumber, emp.MileageTotal, emp.MileageTotalDate, false, null), emp.GetCostBreakdown().ToArray(), emp.GetEmailNotificationList().EmailNotificationIDs, clsEmployees.GetUserDefinedFields(empID));
                    clsLogging.saveLogItem(logID, LogReasonType.SuccessUpdate, element, "Updated Employee " + username);
                }
            }
            else
            {
                Employee tmpEmployee = new Employee(AccountID, 0, username, string.Empty, emailAddress, title, firstName, middleNames, maidenName, surname, true, true, false, false, 0, 0, DateTime.UtcNow, employeeid, null, null, new BankAccount(accountName, accountNum, accountType, sortCode, accountRef), 0, string.Empty, mobileNumber, pagerNumber, homeFaxNo, homeEmail, 0, 0, string.Empty, gender, dateOfBirth, hireDate, termDate, payrollNumber, position, string.Empty, string.Empty, CreationMethod.ImplementationImport, PasswordEncryptionMethod.RijndaelManaged, true, false, subAccountID, primaryCurrencyID, primaryCountryID, 0, 0, false, null, null, natInsNum, string.Empty, string.Empty, null, null, null, 1, DateTime.UtcNow, 1, 0, null, false);
                empID = clsEmployees.SaveEmployee(tmpEmployee, new cDepCostItem[0], new List<int>(), new SortedList<int, object>());
                clsLogging.saveLogItem(logID, LogReasonType.SuccessAdd, element, "Added Employee " + username);

                if (empID > 0)
                {
                    Employee employee = clsEmployees.GetEmployeeById(empID);

                    //Add the access and item roles
                    foreach (KeyValuePair<int, List<int>> subaccRoles in lstAccessRoles)
                    {
                        int subaccountId = subaccRoles.Key;
                        List<int> curRoles = subaccRoles.Value;


                        employee.GetAccessRoles().Add(curRoles, subaccountId, null);
                    }

                    employee.GetItemRoles().Add(lstItemRoles, null);
                }
            }

            #region Add the employee home and office locations

            if (empID > 0)
            {
                Employee employee = null;
                if (homeAddress != null && homeAddress.Identifier > 0)
                {
                    if (!checkEmployeeLocationexists("employeeHomeAddresses", "AddressId", homeAddress.Identifier.ToString(CultureInfo.InvariantCulture), empID))
                    {
                        employee = clsEmployees.GetEmployeeById(employeeid);
                        employee.GetHomeAddresses().Add(new cEmployeeHomeLocation(0, empID, homeAddress.Identifier, new DateTime(1990, 01, 01), null, DateTime.UtcNow, employeeid, null, null), null);
                    }
                }

                if (officeAddress != null && officeAddress.Identifier > 0)
                {
                    if (!checkEmployeeLocationexists("EmployeeWorkAddresses", "AddressId", officeAddress.Identifier.ToString(CultureInfo.InvariantCulture), empID))
                    {
                        if (employee == null)
                        {
                            employee = clsEmployees.GetEmployeeById(employeeid);
                        }

                        employee.GetWorkAddresses().Add(new cEmployeeWorkLocation(0, empID, officeAddress.Identifier, new DateTime(1990, 01, 01), null, true, false, DateTime.UtcNow, employeeid, null, null, null, false), null);
                    }
                }
            }

            #endregion

            #region Add employee password key

            string strSendOnDate = lstEmployee["Password Key Send on Date"].ToString();
            DateTime tempSendOnDate = new DateTime(1900, 01, 01);
            DateTime? sendOnDate = null;

            if (strSendOnDate != "")
            {
                bool isValid = DateTime.TryParse(strSendOnDate, out tempSendOnDate);

                if (isValid)
                {
                    sendOnDate = tempSendOnDate;
                }
            }

            if (empID != -1)
            {
                clsEmployees.AddPasswordKey(empID, cEmployees.PasswordKeyType.AdminRequest, sendOnDate);
            }


            #endregion
        }

        private void saveEmployeeLineManager(Dictionary<string, object> lstEmployeeLineManager)
        {
            cElements clsElements = new cElements();
            cElement element = clsElements.GetElementByName("EmployeeLineManager");

            cEmployees clsEmployees = new cEmployees(AccountID);

            string username = lstEmployeeLineManager["Username"].ToString();
            string strLinemanager = lstEmployeeLineManager["Line Manager"].ToString();

            int empID = clsEmployees.getEmployeeidByUsername(AccountID, username);
            int linemanager = clsEmployees.getEmployeeidByUsername(AccountID, strLinemanager);

            Employee employee = clsEmployees.GetEmployeeById(empID);
            employee.LineManager = linemanager;
            employee.Save(null);
            clsLogging.saveLogItem(logID, LogReasonType.SuccessUpdate, element, "Updated Line Manager for Employee " + username);
        }

        private void saveESRAssignment(Dictionary<string, object> lstESRAssignment)
        {
            cElements clsElements = new cElements();
            cElement element = clsElements.GetElementByName("ESRAssignment");

            cEmployees clsEmployees = new cEmployees(AccountID);

            string username = lstESRAssignment["Username"].ToString();
            string AssignmentNumber = lstESRAssignment["ESR Assignment Number"].ToString();

            string strPrimary = lstESRAssignment["Primary"].ToString();

            bool isPrimary = false;

            isPrimary = strPrimary.ToLower() == "yes";

            int empID = clsEmployees.getEmployeeidByUsername(AccountID, username);

            cESRAssignments clsAssignments = new cESRAssignments(AccountID, empID);

            if (exists("esr_Assignments", "AssignmentNumber", AssignmentNumber))
            {
                cESRAssignment assignment = clsAssignments.getAssignmentByAssignmentNumber(AssignmentNumber);

                if (assignment != null)
                {
                    clsAssignments.saveESRAssignment(new cESRAssignment(assignment.assignmentid, 0, AssignmentNumber, assignment.earliestassignmentstartdate, assignment.finalassignmentenddate, assignment.assignmentstatus, assignment.payrollpaytype, assignment.payrollname, assignment.payrollperiodtype, assignment.assignmentaddress1, assignment.assignmentaddress2, assignment.assignmentaddresstown, assignment.assignmentaddresscounty, assignment.assignmentaddresspostcode, assignment.assignmentaddresscountry, assignment.supervisorflag, assignment.supervisorassignmentnumber, assignment.supervisoremployementnumber, assignment.supervisorfullname, assignment.accrualplan, assignment.employeecategory, assignment.assignmentcategory, assignment.primaryassignment, assignment.esrprimaryassignmentstring, assignment.normalhours, assignment.normalhoursfrequency, assignment.gradecontracthours, assignment.noofsessions, assignment.sessionsfrequency, assignment.workpatterndetails, assignment.workpatternstartday, assignment.flexibleworkingpattern, assignment.availabilityschedule, assignment.organisation, assignment.legalentity, assignment.positionname, assignment.jobrole, assignment.occupationcode, assignment.assignmentlocation, assignment.grade, assignment.jobname, assignment.group, assignment.tandaflag, assignment.nightworkeroptout, assignment.projectedhiredate, assignment.vacancyid, assignment.esrLocationId, assignment.active, assignment.SignOffOwner, assignment.CreatedOn, assignment.CreatedBy, DateTime.Now, employeeid));
                    clsLogging.saveLogItem(logID, LogReasonType.SuccessUpdate, element, "Updated ESR Assignment " + AssignmentNumber);
                }
            }
            else
            {
                clsAssignments.saveESRAssignment(new cESRAssignment(0, 0, AssignmentNumber, DateTime.Now, null, ESRAssignmentStatus.NotSpecified, "", "", "", "", "", "", "", "", "", false, "", "", "", "", "", "", isPrimary, (isPrimary ? "YES" : "NO"), 0, "", 0, 0, "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", null, null, null, true, null, DateTime.Now, employeeid, null, null));
                clsLogging.saveLogItem(logID, LogReasonType.SuccessAdd, element, "Added ESR Assignment " + AssignmentNumber);
            }
        }

        private void saveTeam(Dictionary<string, object> lstTeam)
        {
            cElements clsElements = new cElements();
            cElement element = clsElements.GetElementByName("Teams");

            cTeams clsTeams = new cTeams(AccountID);

            string teamName = lstTeam["Team Name"].ToString();

            if (exists("teams", "teamname", teamName))
            {
                cTeam team = clsTeams.getTeamByName(teamName);

                int teamLeadID = 0;

                if (team.teamLeaderId != null)
                {
                    teamLeadID = (int)team.teamLeaderId;
                }

                clsTeams.SaveTeam(team.teamid, teamName, team.description, teamLeadID, employeeid);
                clsLogging.saveLogItem(logID, LogReasonType.SuccessUpdate, element, "Updated Team " + teamName);
            }
            else
            {
                clsTeams.SaveTeam(0, teamName, "", 0, employeeid);
                clsLogging.saveLogItem(logID, LogReasonType.SuccessAdd, element, "Added Team " + teamName);
            }
        }

        private void saveTeamAllocation(Dictionary<string, object> lstTeamAllocation)
        {
            cElements clsElements = new cElements();
            cElement element = clsElements.GetElementByName("Teams");

            cTeams clsTeams = new cTeams(AccountID);
            cEmployees clsEmployees = new cEmployees(AccountID);

            string teamName = lstTeamAllocation["Team Name"].ToString();
            string username = lstTeamAllocation["Username"].ToString();

            int empID = clsEmployees.getEmployeeidByUsername(AccountID, username);

            cTeam team = clsTeams.getTeamByName(teamName);

            if (team != null)
            {
                if (empID > 0)
                {
                    if (!team.teammembers.Contains(empID))
                    {
                        clsTeams.AddTeamMember(empID, team.teamid);
                        clsLogging.saveLogItem(logID, LogReasonType.SuccessAdd, element, "Added Team Member " + username + " to Team " + teamName);
                    }
                }
            }

        }

        private void saveBudgetHolder(Dictionary<string, object> lstBudgetHolders)
        {
            cElements clsElements = new cElements();
            cElement element = clsElements.GetElementByName("BudgetHolders");

            cBudgetholders clsBudget = new cBudgetholders(AccountID);
            string budgetHolder = lstBudgetHolders["Budget Holder name"].ToString();
            string username = lstBudgetHolders["Username"].ToString();


            if (exists("budgetholders", "budgetholder", budgetHolder))
            {
                cBudgetHolder budget = clsBudget.getBudgetHolderByName(budgetHolder);
                clsBudget.saveBudgetHolder(new cBudgetHolder(budget.budgetholderid, budgetHolder, budget.description, employeeid, null, null, null, null));

                clsLogging.saveLogItem(logID, LogReasonType.SuccessUpdate, element, "Updated Budget Holder " + budgetHolder);
            }
            else
            {
                clsBudget.saveBudgetHolder(new cBudgetHolder(0, budgetHolder, "", employeeid, null, null, null, null));
                clsLogging.saveLogItem(logID, LogReasonType.SuccessAdd, element, "Added Budget Holder " + budgetHolder);
            }

        }

        private void saveSignoff(ICurrentUser user, Dictionary<string, object> lstSignoffs)
        {
            cElements clsElements = new cElements();
            cElement element = clsElements.GetElementByName("SignOffGroups");

            cGroups clsGroups = new cGroups(AccountID);
            cEmployees clsEmployees = new cEmployees(AccountID);

            string signoffName = lstSignoffs["Signoff Group Name"].ToString();
            bool oneClickAuth = false;
            string oneClickStr = lstSignoffs["One Click Authorisation"].ToString();
            if (oneClickStr.ToLower() == "yes")
            {
                oneClickAuth = true;
            }

            int groupid = 0;
            cGroup group = null;

            if (exists("groups", "groupname", signoffName))
            {
                group = clsGroups.getGroupByName(signoffName);
                clsLogging.saveLogItem(logID, LogReasonType.SuccessUpdate, element, "Updated Signoff group " + signoffName);
            }
            else
            {
                groupid = clsGroups.SaveGroup(0, signoffName, "", oneClickAuth, user, 0);
                clsLogging.saveLogItem(logID, LogReasonType.SuccessAdd, element, "Added Signoff group " + signoffName);
            }

            if (group == null)
            {
                clsGroups = new cGroups(AccountID);
                group = clsGroups.GetGroupById(groupid);
            }

            byte signoffStage = 0;
            byte.TryParse(lstSignoffs["Sign-off Stage"].ToString(), out signoffStage);

            cStage stage = null;

            #region Get signofftype and the related ID for the approver

            string strSignoffType = lstSignoffs["Sign-off Type"].ToString();
            SignoffType signofftype = SignoffType.Employee;

            string strApprover = lstSignoffs["Approver"].ToString();
            int approver = 0;

            switch (strSignoffType)
            {
                case "Budget Holder":
                    cBudgetholders clsBudgetHolders = new cBudgetholders(AccountID);
                    signofftype = SignoffType.BudgetHolder;

                    cBudgetHolder holder = clsBudgetHolders.getBudgetHolderByName(strApprover);

                    if (holder != null)
                    {
                        approver = holder.employeeid;
                    }
                    break;
                case "Employee":
                    signofftype = SignoffType.Employee;
                    approver = clsEmployees.getEmployeeidByUsername(AccountID, strApprover);
                    break;
                case "Team":
                    cTeams clsTeams = new cTeams(AccountID);
                    signofftype = SignoffType.Team;
                    cTeam team = clsTeams.getTeamByName(strApprover);

                    if (team != null)
                    {
                        approver = team.teamid;
                    }
                    break;
                case "Line Manager":
                    signofftype = SignoffType.LineManager;
                    break;

                case "Cost Code Owner":
                    signofftype = SignoffType.CostCodeOwner;
                    break;
                case "Assignment Supervisor":
                    signofftype = SignoffType.AssignmentSignOffOwner;
                    break;
            }

            #endregion

            int extraApprovalLevels = 0;
            if (lstSignoffs.ContainsKey("Extra Approval Matrix Levels"))
            {
                int.TryParse(lstSignoffs["Extra Approval Matrix Levels"].ToString(), out extraApprovalLevels);
            }

            bool approveHigherLevelsOnly = false;
            if (lstSignoffs.ContainsKey("Approve Higher Levels Only"))
            {
                bool.TryParse(lstSignoffs["Approve Higher Levels Only"].ToString(), out approveHigherLevelsOnly);
            }

            bool nhsAssignmentSupervisorApprovesWhenMissingCostCodeOwner = false;
            if (lstSignoffs.ContainsKey("Nhs Assignment Supervisor Approves When Missing Cost Code Owner"))
            {
                bool.TryParse(lstSignoffs["Nhs Assignment Supervisor Approves When Missing Cost Code Owner"].ToString(), out nhsAssignmentSupervisorApprovesWhenMissingCostCodeOwner);
            }

            #region Get the rule type for the stage

            string strRule = lstSignoffs["Rule"].ToString();
            StageInclusionType incType;
            int include = 0;

            switch (strRule)
            {
                case "Always":
                    incType = StageInclusionType.Always;
                    include = 1;
                    break;

                case "Value Below X":
                    incType = StageInclusionType.ClaimTotalBelow;
                    include = 5;
                    break;

                case "Value Exceeeds X":
                    incType = StageInclusionType.ClaimTotalExceeds;
                    include = 2;
                    break;

                case "Item Exceeds Limit":
                    incType = StageInclusionType.ExpenseItemExceeds;
                    include = 3;
                    break;

                case "Claim Includes Cost Code":
                    incType = StageInclusionType.IncludesCostCode;
                    include = 4;
                    break;

                case "Claim Includes Expense Item":
                    incType = StageInclusionType.IncludesExpenseItem;
                    include = 6;
                    break;

                case "Item Older Than X Days":
                    incType = StageInclusionType.OlderThanDays;
                    include = 7;
                    break;

                case "Claim Includes Department":
                    incType = StageInclusionType.IncludesDepartment;
                    include = 8;
                    break;
                case "Validation Failed Twice":
                    incType = StageInclusionType.ValidationFailedTwice;
                    include = 9;
                    break;
                default:
                    incType = StageInclusionType.None;
                    break;
            }

            #endregion

            decimal ruleValue = 0;
            decimal.TryParse(lstSignoffs["Rule Value"].ToString(), out ruleValue);

            #region Get the involvement of the approver for the stage

            string strInvolvement = lstSignoffs["Involvement"].ToString();
            int involvement = 0;

            switch (strInvolvement)
            {
                case "Just notify user of claim":
                    involvement = 1;
                    break;

                case "User is to check claim":
                    involvement = 2;
                    break;

                default:
                    break;
            }

            #endregion

            #region Get the holiday status

            string strOnHoliday = lstSignoffs["If Approver on holiday"].ToString();
            int onHoliday = 0;

            switch (strOnHoliday)
            {
                case "Take No Action":
                    onHoliday = 1;
                    break;

                case "Skip Stage":
                    onHoliday = 2;
                    break;

                case "Assign to Someone Else":
                    onHoliday = 3;
                    break;

                default:
                    break;
            }

            #endregion

            #region Get the holiday type

            string strHolidayType = lstSignoffs["Assign to whom"].ToString();
            SignoffType holidayType = 0;

            switch (strHolidayType)
            {
                case "Budget Holder":
                    holidayType = SignoffType.BudgetHolder;
                    break;

                case "Employee":
                    holidayType = SignoffType.Employee;
                    break;

                case "Team":
                    holidayType = SignoffType.Team;
                    break;

                case "Line Manager":
                    holidayType = SignoffType.LineManager;
                    break;

                case "Cost Code Owner":
                    holidayType = SignoffType.CostCodeOwner;
                    break;
                case "Assignment Supervisor":
                    holidayType = SignoffType.AssignmentSignOffOwner;
                    break;
            }

            #endregion

            string holidayValue = lstSignoffs["Holiday Value"].ToString();
            int holidayEmp = 0;

            if (holidayValue != "")
            {
                holidayEmp = clsEmployees.getEmployeeidByUsername(AccountID, holidayValue);
            }

            string strEmailToApprover = lstSignoffs["Email to Approver (Y/N)"].ToString();

            bool emailToApprover = strEmailToApprover.ToLower() == "yes";
            string strEmailToClaimant = lstSignoffs["Email to Claimant (Y/N)"].ToString();

            bool emailToClaimant = strEmailToClaimant.ToLower() == "yes";
            string strApproverDeclaration = lstSignoffs["Approver Declaration (Y/N)"].ToString();

            bool approverDeclaration = strApproverDeclaration.ToLower() == "yes";

            bool stageExists = false;

            if (signoffStage > 0)
            {
                foreach (cStage reqStage in group.stages.Values)
                {
                    if (reqStage.stage == signoffStage)
                    {
                        stageExists = true;
                        break;
                    }
                }

                if (stageExists)
                {
                    stage = group.stages[signoffStage];
                    clsGroups.updateStage(stage.signoffid, signofftype, approver, include, ruleValue, involvement, onHoliday, holidayType, holidayEmp, stage.includeid, emailToClaimant, stage.singlesignoff, emailToApprover, approverDeclaration, employeeid, extraApprovalLevels, approveHigherLevelsOnly, approverDeclaration, nhsAssignmentSupervisorApprovesWhenMissingCostCodeOwner, false, null, null);
                    clsLogging.saveLogItem(logID, LogReasonType.SuccessUpdate, element, "Updated Signoff Stage " + signoffStage + " in signoff group " + signoffName);
                }
                else
                {
                    clsGroups.addStage(group.groupid, signofftype, approver, include, ruleValue, involvement, onHoliday, holidayType, holidayEmp, 0, emailToClaimant, false, emailToApprover, approverDeclaration, employeeid, 0, false, extraApprovalLevels, approveHigherLevelsOnly, approverDeclaration, false, false, false, null, null);
                    clsLogging.saveLogItem(logID, LogReasonType.SuccessAdd, element, "Added Signoff Stage " + signoffStage + " in signoff group " + signoffName);
                }
            }
        }

        public void saveEmployeeSignoff(Dictionary<string, object> lstEmployeeSignoffs)
        {
            cElements clsElements = new cElements();
            cElement element = clsElements.GetElementByName("SignOffGroups");

            cEmployees clsEmployees = new cEmployees(AccountID);

            cGroups clsGroups = new cGroups(AccountID);
            cGroup group = null;

            string username = lstEmployeeSignoffs["Username"].ToString();
            int empID = clsEmployees.getEmployeeidByUsername(AccountID, username);

            Employee employee = clsEmployees.GetEmployeeById(empID);

            string signoffName = lstEmployeeSignoffs["Signoff Group Name"].ToString();
            int groupid = 0;
            group = clsGroups.getGroupByName(signoffName);

            if (group != null)
            {
                groupid = group.groupid;
            }

            string signoffNameCC = lstEmployeeSignoffs["Signoff Group Name (Credit Cards)"].ToString();
            int groupCCid = 0;
            group = clsGroups.getGroupByName(signoffNameCC);

            if (group != null)
            {
                groupCCid = group.groupid;
            }

            string signoffNamePC = lstEmployeeSignoffs["Signoff Group Name (Purchase Cards)"].ToString();
            int groupPCid = 0;
            group = clsGroups.getGroupByName(signoffNamePC);

            if (group != null)
            {
                groupPCid = group.groupid;
            }

            string signoffNameAdv = lstEmployeeSignoffs["Signoff Group Name (Advances)"].ToString();
            int groupAdvid = 0;
            group = clsGroups.getGroupByName(signoffNameAdv);

            if (group != null)
            {
                groupAdvid = group.groupid;
            }

            employee.SignOffGroupID = groupid;
            employee.CreditCardSignOffGroup = groupCCid;
            employee.PurchaseCardSignOffGroup = groupPCid;
            employee.AdvancesSignOffGroup = groupAdvid;
            employee.Save(null);
            clsLogging.saveLogItem(logID, LogReasonType.SuccessUpdate, element, "Updated Signoffs for employee " + username);
        }

        public void saveEmployeeCodeAllocation(Dictionary<string, object> lstEmployeeCodeAllocation)
        {
            cElements clsElements = new cElements();
            cElement element = clsElements.GetElementByName("CostCodeBreakdown");

            cEmployees clsEmployees = new cEmployees(AccountID);
            cCostcodes clsCostcodes = new cCostcodes(AccountID);
            cDepartments clsDepartments = new cDepartments(AccountID);
            cProjectCodes clsProjectcodes = new cProjectCodes(AccountID);

            string username = lstEmployeeCodeAllocation["Username"].ToString();

            int empID = clsEmployees.getEmployeeidByUsername(AccountID, username);

            if (empID > 0)
            {
                Employee emp = clsEmployees.GetEmployeeById(empID);

                string department = lstEmployeeCodeAllocation["Department"].ToString();
                cDepartment dep = clsDepartments.GetDepartmentByName(department) ?? clsDepartments.GetDepartmentByDescription(department);

                //If name does not match check the description

                int departmentid = 0;

                if (dep != null)
                {
                    departmentid = dep.DepartmentId;
                }

                string costcode = lstEmployeeCodeAllocation["Cost Code"].ToString();
                cCostCode cost = clsCostcodes.GetCostcodeByString(costcode) ?? clsCostcodes.GetCostcodeByDescription(costcode);

                int costcodeid = 0;

                if (cost != null)
                {
                    costcodeid = cost.CostcodeId;
                }

                string projectcode = lstEmployeeCodeAllocation["Project Code"].ToString();
                cProjectCode proj = clsProjectcodes.GetProjectCodeByName(projectcode) ?? clsProjectcodes.GetProjectCodeByDesc(projectcode);

                int projectcodeid = 0;

                if (proj != null)
                {
                    projectcodeid = proj.projectcodeid;
                }

                string strPercent = lstEmployeeCodeAllocation["Percentage"].ToString();
                int percent = 0;

                int.TryParse(strPercent, out percent);
                bool removeBreakdown = false;

                //Check existence of the breakdown item
                cDepCostItem[] lstCostCodes = emp.GetCostBreakdown().ToArray();
                foreach (cDepCostItem depItem in lstCostCodes)
                {
                    if (depItem.percentused == 100 && percent != 100)
                    {
                        removeBreakdown = true;
                        break;
                    }
                }

                if (percent == 100)
                {
                    removeBreakdown = true;
                }

                if (removeBreakdown)
                {
                    emp.GetCostBreakdown().Clear();
                }

                if (costcodeid == 0 && departmentid == 0 && projectcodeid == 0)
                {
                }
                else
                {

                    cDepCostItem costBreakdown = new cDepCostItem
                    {
                        costcodeid = costcodeid,
                        departmentid = departmentid,
                        percentused = percent,
                        projectcodeid = projectcodeid
                    };

                    emp.GetCostBreakdown().Add(false, new[] { costBreakdown });

                    clsLogging.saveLogItem(logID, LogReasonType.SuccessAdd, element, "Added code allocation for employee " + username);
                }
            }
        }

        public void saveEmployeeCreditCard(Dictionary<string, object> lstEmployeeCreditCard)
        {
            cElements clsElements = new cElements();
            cElement element = clsElements.GetElementByName("EmployeeCreditCards");

            cEmployees clsEmployees = new cEmployees(AccountID);
            cEmployeeCorporateCards clsEmployeeCards = new cEmployeeCorporateCards(AccountID);
            CardProviders clsCardProviders = new CardProviders();

            string username = lstEmployeeCreditCard["Username"].ToString();

            int empID = clsEmployees.getEmployeeidByUsername(AccountID, username);

            string cardType = lstEmployeeCreditCard["Card Type"].ToString();
            string strCardProvider = lstEmployeeCreditCard["Card Provider"].ToString();
            cCardProvider cardProvider = clsCardProviders.getProviderByName(strCardProvider);

            string cardNumber = lstEmployeeCreditCard["Card Number"].ToString();

            SortedList<int, cEmployeeCorporateCard> lstCards = clsEmployeeCards.GetEmployeeCorporateCards(empID);
            bool cardExists = false;

            if (empID > 0)
            {
                if (cardProvider != null)
                {
                    cEmployeeCorporateCard empCorpCard;
                    if (lstCards != null)
                    {
                        foreach (cEmployeeCorporateCard card in lstCards.Values)
                        {
                            if (card.cardprovider == cardProvider && card.cardnumber == cardNumber)
                            {
                                cardExists = true;
                                empCorpCard = new cEmployeeCorporateCard(card.corporatecardid, empID, cardProvider, cardNumber, card.active, card.CreatedOn, card.CreatedBy, DateTime.UtcNow, employeeid);
                                clsEmployeeCards.SaveCorporateCard(empCorpCard);
                                clsLogging.saveLogItem(logID, LogReasonType.SuccessUpdate, element, "Updated corporate card for employee " + username);
                                break;
                            }
                        }
                    }

                    if (!cardExists)
                    {
                        empCorpCard = new cEmployeeCorporateCard(0, empID, cardProvider, cardNumber, true, DateTime.UtcNow, employeeid, DateTime.UtcNow, employeeid);
                        clsEmployeeCards.SaveCorporateCard(empCorpCard);
                        clsLogging.saveLogItem(logID, LogReasonType.SuccessAdd, element, "Added corporate card for employee " + username);
                    }
                }
                else
                {
                    clsLogging.saveLogItem(logID, LogReasonType.Warning, element, "The corporate card with provider " + strCardProvider + " is not a valid provider.");
                }
            }

        }

        public void saveCar(Dictionary<string, object> lstCar)
        {
            cElements clsElements = new cElements();
            cElement element = clsElements.GetElementByName("Cars");

            cEmployees clsEmployees = new cEmployees(AccountID);
            cMileagecats clsMileageCats = new cMileagecats(AccountID);

            string username = "";

            if (lstCar.ContainsKey("Username"))
            {
                username = lstCar["Username"].ToString();
            }

            int empID = clsEmployees.getEmployeeidByUsername(AccountID, username);

            string carMake = lstCar["Car Make"].ToString();
            string carModel = lstCar["Car Model"].ToString();
            string regNo = lstCar["Registration No"].ToString();

            string strMileageCat = lstCar["Vehicle Journey Rate Category"].ToString();
            cMileageCat mileageCat = clsMileageCats.getMileageCatByName(strMileageCat);

            string strEngineType = lstCar["Petrol / Diesel / LPG / Hybrid"].ToString();
            byte engineType = 0;

            switch (strEngineType)
            {
                case "Petrol":
                    engineType = 1;
                    break;

                case "Diesel":
                    engineType = 2;
                    break;

                case "LPG":
                    engineType = 3;
                    break;

                case "Hybrid":
                    engineType = 4;
                    break;

                default:
                    break;
            }

            if (engineType == 0)
            {
                clsLogging.saveLogItem(logID, LogReasonType.Warning, element, "Engine type not set for car " + carMake + " " + carModel + " with reg no " + regNo);
            }

            string strEngineSize = lstCar["Engine Size (CC)"].ToString();
            int engineSize = 0;

            int.TryParse(strEngineSize, out engineSize);

            string strOdometerReadingReq = "";
            string strStartOdReading = "";

            int startOdometerReading = 0;
            List<cCar> lstCars;
            var clsEmployeeCars = new cEmployeeCars(AccountID, empID);
            byte vehicleTypeId = 0;

            //If empID is 0 then a pool car is being added     
            if (empID > 0)
            {
                strOdometerReadingReq = lstCar["Odometer Reading Required?"].ToString();
                strStartOdReading = lstCar["Start Odometer Reading"].ToString();
                int.TryParse(strStartOdReading, out startOdometerReading);

                string strVehicleType = lstCar["Vehicle Type"].ToString();

                if (strVehicleType.Equals(CarTypes.VehicleType.Bicycle.ToString())) vehicleTypeId = (byte)CarTypes.VehicleType.Bicycle;
                else if (strVehicleType.Equals(CarTypes.VehicleType.Car.ToString())) vehicleTypeId = (byte)CarTypes.VehicleType.Car;
                else if (strVehicleType.Equals(CarTypes.VehicleType.Moped.ToString())) vehicleTypeId = (byte)CarTypes.VehicleType.Moped;
                else if (strVehicleType.Equals(CarTypes.VehicleType.Motorcycle.ToString())) vehicleTypeId = (byte)CarTypes.VehicleType.Motorcycle;

                else if (strVehicleType.Equals(CarTypes.VehicleType.LGV.ToString())) vehicleTypeId = (byte)CarTypes.VehicleType.LGV;
                else if (strVehicleType.Equals(CarTypes.VehicleType.HGV.ToString())) vehicleTypeId = (byte)CarTypes.VehicleType.HGV;
                else if (strVehicleType.Equals(CarTypes.VehicleType.Minibus.ToString())) vehicleTypeId = (byte)CarTypes.VehicleType.Minibus;
                else if (strVehicleType.Equals(CarTypes.VehicleType.Bus.ToString())) vehicleTypeId = (byte)CarTypes.VehicleType.Bus;

                if (vehicleTypeId == 0)
                {
                    clsLogging.saveLogItem(logID, LogReasonType.Warning, element, "Vehicle type not set " + carMake + " " + carModel + " with reg no " + regNo);
                }

                lstCars = clsEmployeeCars.Cars;
            }
            else
            {
                var poolCars = new cPoolCars(AccountID);
                lstCars = poolCars.Cars;
            }

            var lstMileageCats = new List<int>();
            bool carExists = false;

            foreach (cCar car in lstCars.Where(car => String.Equals(car.registration, regNo, StringComparison.CurrentCultureIgnoreCase)))
            {
                carExists = true;
                lstMileageCats = car.mileagecats;

                if (mileageCat != null)
                {
                    if (!lstMileageCats.Contains(mileageCat.mileageid))
                    {
                        lstMileageCats.Add(mileageCat.mileageid);
                    }
                }
                clsEmployeeCars.SaveCar(new cCar(AccountID, empID, car.carid, carMake, carModel, regNo, car.startdate, car.enddate, car.active, lstMileageCats, engineType, startOdometerReading, car.fuelcard, car.endodometer, car.defaultuom, engineSize, car.createdon, car.createdby, DateTime.UtcNow, employeeid, car.Approved, car.ExemptFromHomeToOffice, vehicleTypeId, car.TaxExpiry, car.IsTaxValid, car.MotExpiry, car.IsMotValid), false);
                clsLogging.saveLogItem(logID, LogReasonType.SuccessUpdate, element, "Updated car " + carMake + " " + carModel + " with reg no " + regNo);
                break;
            }

            if (!carExists)
            {
                if (mileageCat != null)
                {
                    lstMileageCats.Add(mileageCat.mileageid);
                    clsEmployeeCars.SaveCar(new cCar(AccountID, empID, 0, carMake, carModel, regNo, null, null, true, lstMileageCats, engineType, startOdometerReading, false, 0, MileageUOM.Mile, engineSize, DateTime.UtcNow, employeeid, null, null, true, false, vehicleTypeId, null, false, null, false), false);
                    clsLogging.saveLogItem(logID, LogReasonType.SuccessAdd, element, "Added car " + carMake + " " + carModel + " with reg no " + regNo);
                }
            }

            if (empID > 0)
            {
                clsEmployees.Cache.Delete(AccountID, cEmployeeCars.CacheKey, empID.ToString());
            }
            else
            {
                clsEmployees.Cache.Delete(AccountID, cPoolCars.CacheKey, string.Empty);
            }

        }

        public void savePoolCarUser(Dictionary<string, object> lstPoolCarUsers)
        {
            cElements clsElements = new cElements();
            cElement element = clsElements.GetElementByName("PoolCars");

            cEmployees clsEmployees = new cEmployees(AccountID);

            string username = lstPoolCarUsers["Username"].ToString();

            int empID = clsEmployees.getEmployeeidByUsername(AccountID, username);

            string regNo = lstPoolCarUsers["Pool Car Registration Number"].ToString();

            cPoolCars clsPoolCars = new cPoolCars(AccountID);
            List<cCar> lstPoolCars = clsPoolCars.Cars;

            int carid = 0;

            List<int> lstUserPoolCars;
            cCar reqCar = null;
            bool carExists = false;

            foreach (cCar car in lstPoolCars)
            {
                //Check the pool car exists by the registration No
                if (car.registration == regNo)
                {
                    carExists = true;
                    lstUserPoolCars = clsPoolCars.GetUsersPerPoolCar(car.carid);

                    //Check if the user is assigned to the pool car already
                    if (!lstUserPoolCars.Contains(empID))
                    {
                        reqCar = car;
                        break;
                    }
                }
            }

            if (reqCar != null)
            {
                if (empID > 0)
                {
                    clsPoolCars.AddPoolCarUser(reqCar.carid, empID);
                    clsLogging.saveLogItem(logID, LogReasonType.SuccessAdd, element, "Added employee " + username + " to pool car " + reqCar.make + " " + reqCar.model + " with reg no " + regNo);
                }
            }
            else
            {
                if (!carExists)
                {
                    clsLogging.saveLogItem(logID, LogReasonType.Warning, element, "Pool car with reg no " + regNo + " does not exist.");
                }
                else
                {
                    clsLogging.saveLogItem(logID, LogReasonType.Warning, element, "Employee " + username + " is already assigned to Pool car with reg no " + regNo);
                }
            }
        }

        /// <summary>
        /// Method to decide whether the item from the import will be added as new or updated
        /// </summary>
        /// <param name="tableName">Database table name for the data to be inserted</param>
        /// <param name="fieldName">Name of the field to check the existence on</param>
        /// <param name="value">Value of the field for the existence check</param>
        /// <returns>boolean value to see if existence is true or false</returns>
        public bool exists(string tableName, string fieldName, string value)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(AccountID));

            string strsql = "SELECT count(" + fieldName + ") FROM " + tableName + " WHERE " + fieldName + " = @value";
            expdata.sqlexecute.Parameters.AddWithValue("@value", value);

            int count = expdata.getcount(strsql);

            if (count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool checkEmployeeLocationexists(string tableName, string fieldName, string value, int empID)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(AccountID));

            string strsql = "SELECT count(" + fieldName + ") FROM " + tableName + " WHERE " + fieldName + " = @value and employeeID = @employeeID";
            expdata.sqlexecute.Parameters.AddWithValue("@value", value);
            expdata.sqlexecute.Parameters.AddWithValue("@employeeID", empID);

            int count = expdata.getcount(strsql);

            if (count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void updateSession(int indexOfSheet, string currentSheetName, int noOfRows, int rowsProcessed, ImportStatus importStatus, WorksheetStatus sheetStatus)
        {
            cImportInfo importInfo = (cImportInfo)HttpRuntime.Cache["SpreadsheetImport" + AccountID];
            importInfo.currentSheet = indexOfSheet;
            importInfo.currentSheetName = currentSheetName;
            importInfo.importStatus = importStatus;

            if (indexOfSheet > 0)
            {
                SortedList<int, ImportStatusValues> lstSheetStatus = importInfo.worksheets;

                ImportStatusValues statusVals = lstSheetStatus[indexOfSheet];

                statusVals.numOfRows = noOfRows;
                if (noOfRows > 0)
                {
                    decimal calc = Math.Round(decimal.Divide(rowsProcessed, noOfRows) * 100, 0, MidpointRounding.AwayFromZero);
                    statusVals.processedRows = Convert.ToInt32(calc);
                }
                else
                {
                    statusVals.processedRows = 100;
                }

                statusVals.status = sheetStatus;

                lstSheetStatus[indexOfSheet] = statusVals;

                importInfo.worksheets = lstSheetStatus;

                HttpRuntime.Cache["SpreadsheetImport" + AccountID] = importInfo;
            }

        }

        /// <summary>
        /// Gets an id by the table, a field and  value for that field
        /// </summary>
        /// <param name="tableGuid">Guid of the table</param>
        /// <param name="fieldGuid">Guid of the field</param>
        /// <param name="value">Value to search by on the field</param>
        /// <returns>The id of the field</returns>
        public int GetIdByTableGuidFieldGuidAndValue(Guid tableGuid, Guid fieldGuid, string value)
        {
            int id = 0;
            var fields = new cFields(this.AccountID);
            var field = fields.GetFieldByID(fieldGuid);
            var tables = new cTables(this.AccountID);
            var table = tables.GetTableByID(tableGuid);
            var keyField = table.GetPrimaryKey();
            using (var connection = new DatabaseConnection(cAccounts.getConnectionString(this.AccountID)))
            {
                connection.sqlexecute.Parameters.AddWithValue("@value", value);
                var sql =
                    $"SELECT DISTINCT [{keyField.FieldName}] FROM [{table.TableName}] WHERE [{table.TableName}].[{field.FieldName}] = @value";
                id = connection.ExecuteScalar<int>(sql);
            }
            return id;
        }
    }
}

