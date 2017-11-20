namespace EsrFileProcessingService
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using ApiLibrary.ApiObjects.Base;
    using ApiLibrary.ApiObjects.Enum;
    using ApiLibrary.ApiObjects.ESR;
    using ApiLibrary.DataObjects.Base;
    using ApiLibrary.DataObjects.ESR;
    using ApiLibrary.DataObjects.Spend_Management;

    using EsrFileProcessingService.ApiService;
    using EsrFileProcessingService.EsrNhsHub;

    using Action = ApiLibrary.DataObjects.Base.Action;

    /// <summary>
    /// ESR Record Processor class
    /// </summary>
    public class EsrRecordProcessor
    {
        #region private variables

        /// <summary>
        /// The message.
        /// </summary>
        private const string Message = "<span style=\"color: {0};\">Line {1}: {2}: Record ID {3} processed. Processing was {4}. {5}</span>";

        /// <summary>
        /// Logger to writing log, diagnostics etc.
        /// </summary>
        private readonly Log logger;

        /// <summary>
        /// The log API.
        /// </summary>
        private readonly ImportLogApi logApi;

        /// <summary>
        /// The history API.
        /// </summary>
        private readonly ImportHistoryApi historyApi;

        /// <summary>
        /// Connection string to the 
        /// </summary>
        private readonly string loggerConnString;

        /// <summary>
        /// The import histories.
        /// </summary>
        private List<ImportHistory> importHistories;

        /// <summary>
        /// Current Header Record
        /// </summary>
        private EsrHeaderRecord headerRecord;

        /// <summary>
        /// Current account ID for VPD file being processed
        /// </summary>
        private int nAccountId;

        /// <summary>
        /// The log record.
        /// </summary>
        private ImportLog logRecord;

        /// <summary>
        /// The file id.
        /// </summary>
        private int fileId;

        /// <summary>
        /// Current trust record being processed
        /// </summary>
        private EsrTrust currentTrust;
        #endregion private variables

        /// <summary>
        /// Initialises a new instance of the <see cref="EsrRecordProcessor"/> class. 
        /// Primary constructor
        /// </summary>
        public EsrRecordProcessor()
        {
            this.logger = new Log();
            this.loggerConnString = ConfigurationManager.ConnectionStrings["ApiLog"].ConnectionString;
            this.logApi = new ImportLogApi();
            this.historyApi = new ImportHistoryApi();
        }

        /// <summary>
        /// Validates and processes ESR outbound file rows
        /// </summary>
        /// <param name="fileObject">The name of the file from which the rows were taken and the file rows</param>
        public void ProcessEsrFile(FileHeadersAndRows fileObject)
        {
            this.fileId = fileObject.FileId;
            string fileName = fileObject.FileName;
            List<string> fileRows = fileObject.FileRows;
            var esrNhsHubCommandsClient = new EsrNhsHubCommandsClient("EsrRouterToHubEndpoint");
            bool sentProcessingStatus = false;

            var validateResult = this.ValidateFile(fileRows);

            if (this.logRecord == null)
            {
                this.logRecord = new ImportLog { Action = Action.Create, logType = 2, logName = fileName, expectedLines = fileRows.Count };
                this.logRecord = this.UpdateLog();
            }

            this.importHistories = this.historyApi.Send(
                this.nAccountId,
                "",
                new List<ImportHistory>
                    {
                        new ImportHistory
                            {
                                Action = Action.Create,
                                applicationType = 2,
                                logId = this.logRecord.logID,
                                importedDate = DateTime.Now,
                                dataId = this.fileId,
                                importStatus = 4 //// In Progress
                            }
                    });

            if (validateResult != EsrHubStatus.EsrHubTransferStatus.Success)
            {
                this.logger.Write(
                    "expenses",
                    this.headerRecord != null ? this.headerRecord.VpdNumber : 0,
                    this.nAccountId,
                    LogRecord.LogItemTypes.OutboundFileValidationFailed,
                    LogRecord.TransferTypes.EsrOutbound,
                    this.logRecord == null ? 0 : this.logRecord.logID,
                    this.headerRecord != null ? this.headerRecord.Filename : fileName,
                    LogRecord.LogReasonType.Error,
                    "ESR Outbound File Validation Failed. Processing Aborted.",
                    "ESRRecordProcessor : ProcessEsrFile()");

                try
                {
                    sentProcessingStatus = esrNhsHubCommandsClient.FailedToProcessFile(this.fileId, fileName, validateResult);
                }
                finally
                {
                    if (!sentProcessingStatus)
                    {
                        this.logger.Write("expenses", this.headerRecord != null ? this.headerRecord.VpdNumber : 0, this.nAccountId, LogRecord.LogItemTypes.OutboundFileValidationFailed, LogRecord.TransferTypes.EsrOutbound, this.logRecord == null ? 0 : this.logRecord.logID, fileName, LogRecord.LogReasonType.Error, "ESR Outbound File Finished. Processing File Command Relay Failed.", "ESRRecordProcessor : ProcessEsrFile()");
                    }
                }

                this.WriteHistory(1); // Status 1 = Failure

                return;
            }

            this.logger.Write(
                 "expenses",
                 this.headerRecord.VpdNumber,
                 this.nAccountId,
                 LogRecord.LogItemTypes.OutboundFileProgressMessage,
                 LogRecord.TransferTypes.EsrOutbound,
                 this.logRecord.logID,
                 this.headerRecord.Filename,
                 LogRecord.LogReasonType.None,
                 string.Format("ESR Outbound File Validated. Record Processing Starting on file {0}", this.headerRecord.Filename),
                 "ESRRecordProcessor : ProcessEsrFile()");

            this.ProcessRecords(fileRows);

            this.logger.Write(
                "expenses",
                this.headerRecord.VpdNumber,
                this.nAccountId,
                LogRecord.LogItemTypes.OutboundFileProgressMessage,
                LogRecord.TransferTypes.EsrOutbound,
                this.logRecord.logID,
                this.headerRecord.Filename,
                LogRecord.LogReasonType.None,
                string.Format("ESR Outbound File Record Processing Finished on file {0}", this.headerRecord.Filename),
                "ESRRecordProcessor : ProcessEsrFile()");

            try
            {
                sentProcessingStatus = esrNhsHubCommandsClient.SuccessfullyProcessedFile(this.fileId, fileName);
            }
            finally
            {
                if (!sentProcessingStatus)
                {
                    this.logger.Write(
                        "expenses",
                        this.headerRecord.VpdNumber,
                        this.nAccountId,
                        LogRecord.LogItemTypes.OutboundFileProgressMessage,
                        LogRecord.TransferTypes.EsrOutbound,
                        this.logRecord.logID,
                        fileName,
                        LogRecord.LogReasonType.Error,
                        "ESR Outbound File Finished Processing. File Command Relay Failed.",
                        "ESRRecordProcessor : ProcessEsrFile()");
                }
            }

            this.UpdateFileSequence();

            this.WriteHistory(0); // Status 0 = Success
        }

        private void WriteHistory(int importHistoryStatus)
        {
            var updated = false;
            foreach (ImportHistory history in this.importHistories.Where(history => history.importStatus == 4))
            {
                history.importStatus = importHistoryStatus;
                updated = true;
                history.Action = Action.Update;
            }

            if (updated)
            {
                this.historyApi.Send(this.nAccountId, string.Empty, this.importHistories);
            }
        }

        private void UpdateFileSequence()
        {
            ApiService.ApiRpcClient api = new ApiService.ApiRpcClient();

            if (currentTrust != null)
            {
                currentTrust.Action = Action.Update;
                currentTrust.currentOutboundSequence = headerRecord.UniqueFileSequenceId;

                if (!api.UpdateEsrTrust(currentTrust))
                {
                    this.logger.Write(
                        "expenses",
                        headerRecord.VpdNumber,
                        this.nAccountId,
                        LogRecord.LogItemTypes.OutboundFileProgressMessage,
                        LogRecord.TransferTypes.EsrOutbound,
                        this.logRecord.logID,
                        headerRecord.Filename,
                        LogRecord.LogReasonType.Error,
                        string.Format(
                            "ESR Outbound File Finished Processing. Failed to update the current outbound file sequence on Trust ID {0} (VPD: {1}) to {2}",
                            currentTrust.trustID,
                            currentTrust.trustVPD,
                            currentTrust.currentOutboundSequence),
                        "ESRRecordProcessor : UpdateFileSequence()");
                }
            }
        }

        /// <summary>
        /// Update the Import Log.
        /// </summary>
        /// <returns>
        /// The <see cref="ImportLog"/>.
        /// </returns>
        private ImportLog UpdateLog()
        {
            var LogResult = logApi.Send(nAccountId, "", new List<ImportLog> { this.logRecord });
            return LogResult.Count == 1 ? LogResult[0] : new ImportLog();
        }

        /// <summary>
        /// Validates the file structure and number of records
        /// </summary>
        /// <param name="fileRows">Inidividual rows from the file</param>
        /// <returns>TRUE if file is valid in structure, otherwise FALSE</returns>
        private EsrHubStatus.EsrHubTransferStatus ValidateFile(List<string> fileRows)
        {
            headerRecord = EsrHeaderRecord.ParseEsrHeaderRecord(fileRows.First());
            EsrTrailerRecord trailerRecord = EsrTrailerRecord.ParseEsrTrailerRecord(fileRows.Last());

            if (headerRecord != null)
            {
                currentTrust = GetVpdAccountId(headerRecord.VpdNumber);
                nAccountId = currentTrust.AccountId;

                if (nAccountId == 0)
                {
                    this.logger.Write(
                       "expenses",
                       headerRecord.VpdNumber,
                       nAccountId,
                       LogRecord.LogItemTypes.OutboundFileValidationFailed,
                       LogRecord.TransferTypes.EsrOutbound,
                       this.logRecord == null ? 0 : this.logRecord.logID,
                       headerRecord.Filename,
                       LogRecord.LogReasonType.Error,
                       string.Format("Failed to find customer account matching VPD number {0} for file {1}.  The VPD may be missing or archived.", this.headerRecord.VpdNumber, this.headerRecord.Filename),
                       "EsrRecordProcessor : ValidateFile()");

                    return EsrHubStatus.EsrHubTransferStatus.NhsVpdNotFound;
                }

                if (this.logRecord == null)
                {
                    this.logRecord = new ImportLog { Action = Action.Create, logType = 2, logName = headerRecord.Filename, expectedLines = fileRows.Count };
                    this.logRecord = this.UpdateLog();
                }

                this.logger.Write(
                "expenses",
                0,
                this.nAccountId,
                LogRecord.LogItemTypes.OutboundFileImportStarted,
                LogRecord.TransferTypes.EsrOutbound,
                this.logRecord == null ? 0 : this.logRecord.logID,
                headerRecord.Filename,
                LogRecord.LogReasonType.None,
                string.Format("ESR Outbound File Processing Starting for file {0}", headerRecord.Filename),
                "ESRRecordProcessor : ProcessEsrFile()");

                if (currentTrust.currentOutboundSequence.HasValue && ((headerRecord.UniqueFileSequenceId != currentTrust.currentOutboundSequence) && (headerRecord.UniqueFileSequenceId != (currentTrust.currentOutboundSequence.Value + 1))))
                {
                    this.logger.Write(
                        "expenses",
                        headerRecord.VpdNumber,
                        nAccountId,
                        LogRecord.LogItemTypes.OutboundFileValidationFailed,
                        LogRecord.TransferTypes.EsrOutbound,
                        this.logRecord == null ? 0 : this.logRecord.logID,
                        headerRecord.Filename,
                        LogRecord.LogReasonType.Error,
                        string.Format(
                            "File sequence number {0} is invalid. Expected file sequence number {1} for VPD number {2}.",
                            headerRecord.UniqueFileSequenceId,
                            currentTrust.currentOutboundSequence + 1,
                            headerRecord.VpdNumber),
                        "EsrRecordProcessor : ValidateFile()");

                    return EsrHubStatus.EsrHubTransferStatus.ValidationWrongSequenceNumber;
                }

                //if (headerRecord.InterfaceVersion != "02")
                //{
                //    // Not an ESR v2 file, so don't proceed with processing
                //    this.logger.Write(
                //        "expenses",
                //        headerRecord.VpdNumber,
                //        nAccountId,
                //        LogRecord.LogItemTypes.OutboundFileValidationFailed,
                //        LogRecord.TransferTypes.EsrOutbound,
                //        this.logRecord == null ? 0 : this.logRecord.logID,
                //        headerRecord.Filename,
                //        LogRecord.LogReasonType.Error,
                //        "Incorrect Interface Version in Header Record",
                //        "EsrRecordProcessor : ValidateFile()");

                //    return false;
                //}
            }
            else
            {
                this.logger.Write(
                    "expenses",
                    0,
                    nAccountId,
                    LogRecord.LogItemTypes.OutboundFileValidationFailed,
                    LogRecord.TransferTypes.EsrOutbound,
                    this.logRecord == null ? 0 : this.logRecord.logID,
                    string.Empty,
                    LogRecord.LogReasonType.Error,
                    "First record in supplied file is invalid Header Record",
                    "EsrRecordProcessor : ValidateFile()");

                return EsrHubStatus.EsrHubTransferStatus.ValidationFailedNoHeader;
            }

            if (trailerRecord != null)
            {
                if (trailerRecord.NumberOfRecords != fileRows.Count)
                {
                    // number of records in the file does not match the value specified in the trailer record
                    this.logger.Write(
                        "expenses",
                        headerRecord.VpdNumber,
                        nAccountId,
                        LogRecord.LogItemTypes.OutboundFileValidationFailed,
                        LogRecord.TransferTypes.EsrOutbound,
                        this.logRecord == null ? 0 : this.logRecord.logID,
                        headerRecord.Filename,
                        LogRecord.LogReasonType.Error,
                        "Number of records in supplied file does not match expected count detailed in the Trailer Record",
                        "EsrRecordProcessor : ValidateFile()");

                    return EsrHubStatus.EsrHubTransferStatus.ValidationFailedRecordCount;
                }
            }
            else
            {
                // Last record must be a trailer record. File is invalid
                this.logger.Write(
                    "expenses",
                    headerRecord.VpdNumber,
                    nAccountId,
                    LogRecord.LogItemTypes.OutboundFileValidationFailed,
                    LogRecord.TransferTypes.EsrOutbound,
                    this.logRecord == null ? 0 : this.logRecord.logID,
                    headerRecord.Filename,
                    LogRecord.LogReasonType.Error,
                    "Last record in supplied file is invalid Trailer Record",
                    "EsrRecordProcessor : ValidateFile()");

                return EsrHubStatus.EsrHubTransferStatus.ValidationFailedNoFooter;
            }

            return EsrHubStatus.EsrHubTransferStatus.Success;
        }

        /// <summary>
        /// Used to perform a reduced, simple (and quick) validation of an ESR v2 file contents
        /// </summary>
        /// <param name="fileRows">The rows extracted from an ESR file</param>
        /// <param name="returnMessage">A return message indicating the reason for pass/fail</param>
        /// <returns></returns>
        public static bool ValidateFileTransfer(ref List<string> fileRows, out string returnMessage)
        {
            if (fileRows.Count < 3)
            {
                returnMessage = "Invalid file. Minimum number of rows (i.e. 3) are not present.";
                return false;
            }

            string firstRow = fileRows.First();

            if (firstRow.Length < 4)
            {
                returnMessage = "Invalid file. The file does not contain enough characters to find a delimiter.";
                return false;
            }

            if (firstRow.Substring(3, 1) != "~")
            {
                returnMessage = "Invalid file. An incorrect delimiter is being used.";
                return false;
            }

            if (firstRow.Length < 3 || firstRow.Substring(0, 3) != "HDR")
            {
                returnMessage = "Invalid file. The first row must be a HDR row.";
                return false;
            }

            string lastRow = fileRows.Last(x => !string.IsNullOrWhiteSpace(x));

            if (lastRow.Length < 3 || lastRow.Substring(0, 3) != "TRL")
            {
                returnMessage = "Invalid file. The last row must be a TRL row.";
                return false;
            }

            int numRows;
            if (!int.TryParse(lastRow.Substring(4), out numRows))
            {
                returnMessage = "Invalid file. A rowcount cannot be found in the TRL row.";
                return false;
            }

            if (numRows != fileRows.Count(s => !string.IsNullOrWhiteSpace(s)))
            {
                returnMessage = "Invalid file. The rowcount from the last row does not match the number of rows found in the stream.";
                return false;
            }

            returnMessage = "Valid file. Simple file validation passed.";
            return true;
        }

        /// <summary>
        /// Get the database Account ID for a VPD number
        /// </summary>
        /// <param name="vpdNumber">VPD number of obtain account id for</param>
        /// <returns>Account Id for the supplied VPD number</returns>
        private EsrTrust GetVpdAccountId(int vpdNumber)
        {
            ApiService.ApiRpcClient api = new ApiService.ApiRpcClient();
            return api.GetAccountFromVpd(vpdNumber.ToString());
        }

        /// <summary>
        /// Processes the individual records according to their type
        /// </summary>
        /// <param name="fileRows">
        /// Inidividual rows from the file
        /// </param>
        private void ProcessRecords(IEnumerable<string> fileRows)
        {
            List<EsrLocationRecord> locationRecords = new List<EsrLocationRecord>();
            List<EsrOrganisationRecord> organisationRecords = new List<EsrOrganisationRecord>();
            List<EsrPositionRecord> positionRecords = new List<EsrPositionRecord>();
            List<EsrPersonRecord> personRecords = new List<EsrPersonRecord>();
            EsrPersonRecord currentPersonRecord = null;
            long currentBatchEsrPersonId = 0;
            var startTime = DateTime.Now.AddMinutes(1);

            List<object> processedRecords = new List<object>();
            ApiRecordType currentRecordType = ApiRecordType.EsrLocationRecord;

            int rowIndex = 0;

            foreach (string row in fileRows)
            {
                string recordType = row.Substring(0, 3);
                rowIndex++;

                if (startTime < DateTime.Now)
                {
                    this.OutputImportStatus(rowIndex);
                    startTime = DateTime.Now.AddMinutes(1);
                }

                if (rowIndex > 4850)
                {
                    var a = 1;
                }

                switch (recordType)
                {
                    case EsrRecordTypes.EsrLocationUpdateRecordType:
                    case EsrRecordTypes.EsrLocationDeleteRecordType:
                        this.ProcessLocationRecord(row, ref locationRecords, ref currentRecordType, ref processedRecords, rowIndex);
                        break;

                    case EsrRecordTypes.EsrOrganisationUpdateRecordType:
                    case EsrRecordTypes.EsrOrganisationDeleteRecordType:
                        if (currentRecordType != ApiRecordType.EsrOrganisationRecord && locationRecords.Count > 0)
                        {
                            this.LogRecordBatchSend("Location", locationRecords.Count);

                            // moved on from EsrLocationRecords, so flush
                            processedRecords.AddRange(this.SendLocationRecords(ref locationRecords));
                        }

                        this.ProcessOrganisationRecord(row, ref organisationRecords, ref currentRecordType, ref processedRecords, rowIndex);
                        break;

                    case EsrRecordTypes.EsrPositionUpdateRecordType:
                    case EsrRecordTypes.EsrPositionDeleteType:
                        if (currentRecordType != ApiRecordType.EsrPositionRecord && organisationRecords.Count > 0)
                        {
                            this.LogRecordBatchSend("Organisation", organisationRecords.Count);

                            // move on from EsrOrganisationRecords, so flush
                            processedRecords.AddRange(this.SendOrganisationRecords(ref organisationRecords));
                        }

                        this.ProcessPositionRecord(row, ref positionRecords, ref currentRecordType, ref processedRecords, rowIndex);
                        break;

                    case EsrRecordTypes.EsrPersonUpdateRecordType:
                    case EsrRecordTypes.EsrPersonDeleteRecordType:
                        if (currentRecordType != ApiRecordType.EsrPositionRecord && positionRecords.Count > 0)
                        {
                            this.LogRecordBatchSend("Position", positionRecords.Count);

                            // move on from EsrPositionRecords, so flush
                            processedRecords.AddRange(this.SendPositionRecords(ref positionRecords));
                        }

                        this.ProcessPersonRecord(row, ref currentPersonRecord, ref personRecords, ref currentRecordType, ref processedRecords, ref currentBatchEsrPersonId, rowIndex);
                        break;

                    case EsrRecordTypes.EsrAssignmentDeleteRecordType:
                        this.DeleteAssignment(row, rowIndex, ref processedRecords);
                        break;
                    case EsrRecordTypes.EsrAssignmentUpdateRecordType:
                        this.ProcessAssignmentRecord(row, ref currentPersonRecord, rowIndex);
                        break;
                    case EsrRecordTypes.EsrPhoneDeleteRecordType:
                        this.DeletePhone(row, rowIndex, ref processedRecords);
                        break;
                    case EsrRecordTypes.EsrPhoneUpdateRecordType:
                        this.ProcessPhoneRecord(row, ref currentPersonRecord, rowIndex);
                        break;
                    case EsrRecordTypes.EsrAddressDeleteRecordType:
                        this.DeleteAddress(row, rowIndex, ref processedRecords);
                        break;
                    case EsrRecordTypes.EsrAddressUpdateRecordType:
                        this.ProcessAddressRecord(row, ref currentPersonRecord, rowIndex);
                        break;
                    case EsrRecordTypes.EsrVehicleDeleteRecordType:
                        this.DeleteVehicle(row, rowIndex, ref processedRecords);
                        break;
                    case EsrRecordTypes.EsrVehicleUpdateRecordType:
                        this.ProcessVehicleRecord(row, ref currentPersonRecord, rowIndex);
                        break;
                    case EsrRecordTypes.EsrAssignmentCostingDeleteRecordType: 
                        this.DeleteAssignmentCosting(row, rowIndex, ref processedRecords);
                        break;
                    case EsrRecordTypes.EsrAssignmentCostingUpdateRecordType:
                        this.ProcessAssignmentCostingRecord(row, ref currentPersonRecord, rowIndex);
                        break;

                    case EsrRecordTypes.EsrTrailerRecordType:
                        if (currentPersonRecord != null)
                        {
                            this.ProcessPersonRecord(row, ref currentPersonRecord, ref personRecords, ref currentRecordType, ref processedRecords, ref currentBatchEsrPersonId, rowIndex, true);
                        }

                        rowIndex++;
                        break;
                    case EsrRecordTypes.EsrHeaderRecordType:
                        rowIndex++;
                        continue;
                }
            }

            this.ProcessRemainingRecords(ref locationRecords, ref organisationRecords, ref positionRecords, ref personRecords, ref processedRecords);

            this.CheckForRetrospectiveUpdates(ref processedRecords);

            this.logger.Write(
                "expenses",
                headerRecord.VpdNumber,
                this.nAccountId,
                LogRecord.LogItemTypes.OutboundFileProgressMessage,
                LogRecord.TransferTypes.EsrOutbound,
                this.logRecord.logID,
                headerRecord.Filename,
                LogRecord.LogReasonType.None,
                string.Format("Processed {0} Records. Writing Record Processing Log", processedRecords.Count),
                "ESRRecordProcessor : ProcessRecords()");
            this.logRecord.processedLines = rowIndex - 2;
            this.LogRecordProcessingStatus(processedRecords);
        }

        /// <summary>
        /// The delete assignment costing.
        /// </summary>
        /// <param name="row">
        /// The row.
        /// </param>
        /// <param name="rowIndex">
        /// The row index.
        /// </param>
        /// <param name="processedRecords">
        /// The processed records.
        /// </param>
        private void DeleteAssignmentCosting(string row, int rowIndex, ref List<object> processedRecords)
        {
            var assignmentCostingRecord = EsrAssignmentCostingRecord.ParseEsrAssignmentCostingRecord(row);

            if (assignmentCostingRecord == null)
            {
                this.LogRecordParseFailure(row, rowIndex);
                return;
            }

            this.ProcessDelete(assignmentCostingRecord, ref processedRecords);
        }

        /// <summary>
        /// The delete address.
        /// </summary>
        /// <param name="row">
        /// The row.
        /// </param>
        /// <param name="rowIndex">
        /// The row index.
        /// </param>
        /// <param name="processedRecords">
        /// The processed records.
        /// </param>
        private void DeleteAddress(string row, int rowIndex, ref List<object> processedRecords)
        {
            var addressRecord = EsrAddressRecord.ParseEsrAddressRecord(row);

            if (addressRecord == null)
            {
                this.LogRecordParseFailure(row, rowIndex);
                return;
            }

            this.ProcessDelete(addressRecord, ref processedRecords);
        }

        /// <summary>
        /// The delete assignment.
        /// </summary>
        /// <param name="row">
        /// The row.
        /// </param>
        /// <param name="rowIndex">
        /// The row index.
        /// </param>
        /// <param name="processedRecords">
        /// The processed records.
        /// </param>
        private void DeleteAssignment(string row, int rowIndex, ref List<object> processedRecords)
        {
            var assignmentRecord = EsrAssignmentRecord.ParseEsrAssignmentRecord(row);

            if (assignmentRecord == null)
            {
                this.LogRecordParseFailure(row, rowIndex);
                return;
            }

            this.ProcessDelete(assignmentRecord, ref processedRecords);
        }

        /// <summary>
        /// Delete phone.
        /// </summary>
        /// <param name="row">
        /// The row.
        /// </param>
        /// <param name="rowIndex">
        /// The row index.
        /// </param>
        /// <param name="processedRecords">
        /// The processed records.
        /// </param>
        private void DeletePhone(string row, int rowIndex, ref List<object> processedRecords)
        {
            var phoneRecord = EsrPhoneRecord.ParseEsrPhoneRecord(row);

            if (phoneRecord == null)
            {
                this.LogRecordParseFailure(row, rowIndex);
                return;
            }

            this.ProcessDelete(phoneRecord, ref processedRecords);
        }

        /// <summary>
        /// Delete vehicle.
        /// </summary>
        /// <param name="row">
        /// The row.
        /// </param>
        /// <param name="rowIndex">
        /// The row index.
        /// </param>
        /// <param name="processedRecords">
        /// The processed records.
        /// </param>
        private void DeleteVehicle(string row, int rowIndex, ref List<object> processedRecords)
        {
            var vehicleRecord = EsrVehicleRecord.ParseEsrVehicleRecord(row);

            if (vehicleRecord == null)
            {
                this.LogRecordParseFailure(row, rowIndex);
                return;
            }

            this.ProcessDelete(vehicleRecord, ref processedRecords);
        }

        /// <summary>
        /// The process delete.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        /// <param name="processedRecords">
        /// The processed records.
        /// </param>
        private void ProcessDelete(DataClassBase entity, ref List<object> processedRecords)
        {
            var dataClass = entity as DataClassBase;
            var result = this.DeleteEntity(ref dataClass);
            if (result != null)
            {
                entity.ActionResult = result.ActionResult;
                processedRecords.Add(entity);
            }
        }

        /// <summary>
        /// Output the import status.
        /// </summary>
        /// <param name="rowIndex">
        /// The row index.
        /// </param>
        private void OutputImportStatus(int rowIndex)
        {
            decimal percentage = ((decimal)rowIndex / (decimal)this.logRecord.expectedLines) * 100;

            this.logger.Write(
                "expenses",
                this.headerRecord.VpdNumber,
                this.nAccountId,
                LogRecord.LogItemTypes.OutboundFileProgressMessage,
                LogRecord.TransferTypes.EsrOutbound,
                this.logRecord.logID,
                this.headerRecord.Filename,
                LogRecord.LogReasonType.None,
                string.Format("Processed {0}%.", decimal.Round(percentage, 2)),
                "ESRRecordProcessor : ProcessRecords()");
        }

        /// <summary>
        /// Reissue any person or organisation records where supervisor/parent organisation could not be updated as record not found
        /// </summary>
        /// <param name="processedRecords"></param>
        private void CheckForRetrospectiveUpdates(ref List<object> processedRecords)
        {
            List<object> reprocessedRecs = new List<object>();
            List<EsrOrganisationRecord> orgRecs = new List<EsrOrganisationRecord>();
            List<EsrPersonRecord> perRecs = new List<EsrPersonRecord>();
            int recIdx = 0;

            foreach (object record in processedRecords)
            {
                recIdx++;

                if (record.GetType() == typeof(EsrOrganisationRecord))
                {
                    EsrOrganisationRecord organisationRecord = (EsrOrganisationRecord)record; // DataClassBase.GetRecordFromDataClass(record, typeof(EsrOrganisationRecord)) as EsrOrganisationRecord;

                    if (organisationRecord.ActionResult.Result != ApiActionResult.ForeignKeyFail || !organisationRecord.ActionResult.LookupUpdateFailure || organisationRecord.Action == Action.NoAction || organisationRecord.Action == Action.Delete)
                    {
                        continue;
                    }

                    this.logger.WriteDebug(
                        "expenses",
                        this.headerRecord.VpdNumber,
                        this.nAccountId,
                        LogRecord.LogItemTypes.OutboundFileProgressMessage,
                        LogRecord.TransferTypes.EsrOutbound,
                        this.logRecord.logID,
                        this.headerRecord.Filename,
                        LogRecord.LogReasonType.Warning,
                        string.Format("Re-issuing Organisation Record {0} to update parent organisation {1}", organisationRecord.ESROrganisationId, organisationRecord.SafeParentOrganisationId),
                        "EsrFileProcessor : CheckForRetrospectiveOrganisationUpdates()");

                    orgRecs.Add(EsrOrganisationRecord.ResetOrganisationRecord(organisationRecord, recIdx));

                    if (orgRecs.Count >= EsrFile.RecordBatchSize)
                    {
                        reprocessedRecs.AddRange(this.SendOrganisationRecords(ref orgRecs));
                        orgRecs.Clear();
                    }
                }

                if (record.GetType() != typeof(EsrPersonRecord))
                {
                    continue;
                }

                EsrPersonRecord personRecord = (EsrPersonRecord)record; // DataClassBase.GetRecordFromDataClass(record, typeof(EsrPersonRecord)) as EsrPersonRecord;

                if ((personRecord.ActionResult.Result != ApiActionResult.PartialSuccess && !personRecord.ActionResult.LookupUpdateFailure) || personRecord.Action == Action.NoAction || personRecord.Action == Action.Delete)
                {
                    continue;
                }

                this.logger.WriteDebug(
                    "expenses",
                    this.headerRecord.VpdNumber,
                    this.nAccountId,
                    LogRecord.LogItemTypes.OutboundFileProgressMessage,
                    LogRecord.TransferTypes.EsrOutbound,
                    this.logRecord.logID,
                    this.headerRecord.Filename,
                    LogRecord.LogReasonType.Warning,
                    string.Format("Re-issuing Person Record {0} to update supervisor assignment", personRecord.ESRPersonId),
                    "EsrFileProcessor : CheckForRetrospectiveUpdates()");

                // Update supervisor fields with safe values (these will have been nullified if lookup failed)
                personRecord.RecordPositionIndex = recIdx;
                this.ResetSupervisorFields(ref personRecord);

                perRecs.Add(personRecord);

                if (perRecs.Count >= EsrFile.RecordBatchSize)
                {
                    reprocessedRecs.AddRange(this.SendPersonRecords(ref perRecs));
                    perRecs.Clear();
                }
            }

            if (perRecs.Count >= 0)
            {
                reprocessedRecs.AddRange(this.SendPersonRecords(ref perRecs));
                perRecs.Clear();
            }

            if (orgRecs.Count >= 0)
            {
                reprocessedRecs.AddRange(this.SendOrganisationRecords(ref orgRecs));
                orgRecs.Clear();
            }

            // overwrite processed log entries with response from reissue
            foreach (object rec in reprocessedRecs)
            {
                if (rec.GetType() == typeof(EsrOrganisationRecord))
                {
                    if (((EsrOrganisationRecord)rec).RecordPositionIndex > 0)
                    {
                        processedRecords[((EsrOrganisationRecord)rec).RecordPositionIndex - 1] = rec;
                    }
                }

                if (rec.GetType() == typeof(EsrPersonRecord))
                {
                    if (((EsrPersonRecord)rec).RecordPositionIndex > 0)
                    {
                        processedRecords[((EsrPersonRecord)rec).RecordPositionIndex - 1] = rec;
                    }
                }
            }
        }

        /// <summary>
        /// Resets Supervisor field values if ready for re-issue
        /// </summary>
        /// <param name="personRecord">Person record object</param>
        private void ResetSupervisorFields(ref EsrPersonRecord personRecord)
        {
            var assignments = new List<EsrAssignment>();
            foreach (EsrAssignment assignmentRecord in personRecord.EsrAssignments)
            {
                if (assignmentRecord.ActionResult.Result == ApiActionResult.ForeignKeyFail && assignmentRecord.ActionResult.LookupUpdateFailure)
                {
                    assignments.Add(EsrAssignmentRecord.ResetAssignmentRecord(assignmentRecord));
                }
                else
                {
                    assignments.Add(assignmentRecord);
                }
            }

            personRecord.EsrAssignments = assignments;
            personRecord.ActionResult = new ApiResult();
        }

        /// <summary>
        /// Check for any remaining records that require processing
        /// </summary>
        /// <param name="locationRecords"></param>
        /// <param name="organisationRecords"></param>
        /// <param name="positionRecords"></param>
        /// <param name="personRecords"></param>
        /// <param name="processedRecords"></param>
        private void ProcessRemainingRecords(ref List<EsrLocationRecord> locationRecords, ref List<EsrOrganisationRecord> organisationRecords, ref List<EsrPositionRecord> positionRecords, ref List<EsrPersonRecord> personRecords, ref List<object> processedRecords)
        {
            if (locationRecords.Count > 0)
            {
                this.LogRecordBatchSend("Location", locationRecords.Count);

                processedRecords.AddRange(this.SendLocationRecords(ref locationRecords));
            }

            if (organisationRecords.Count > 0)
            {
                this.LogRecordBatchSend("Organisation", organisationRecords.Count);

                processedRecords.AddRange(this.SendOrganisationRecords(ref organisationRecords));
            }

            if (positionRecords.Count > 0)
            {
                this.LogRecordBatchSend("Position", positionRecords.Count);

                processedRecords.AddRange(this.SendPositionRecords(ref positionRecords));
            }

            if (personRecords.Count > 0)
            {
                this.LogRecordBatchSend("Person", personRecords.Count);

                processedRecords.AddRange(this.SendPersonRecords(ref personRecords));
            }
        }

        /// <summary>
        /// Processes an individual Assignment Costing record
        /// </summary>
        /// <param name="row"></param>
        /// <param name="currentPersonRecord"></param>
        /// <param name="rowIndex"></param>
        private void ProcessAssignmentCostingRecord(string row, ref EsrPersonRecord currentPersonRecord, int rowIndex)
        {
            EsrAssignmentCostingRecord costingRecord = EsrAssignmentCostingRecord.ParseEsrAssignmentCostingRecord(row);

            if (costingRecord == null)
            {
                this.LogRecordParseFailure(row, rowIndex);
                return;
            }

            if (currentPersonRecord != null)
            {
                this.logger.WriteDebug(
                    "expenses",
                    headerRecord.VpdNumber,
                    this.nAccountId,
                    LogRecord.LogItemTypes.OutboundFileProgressMessage,
                    LogRecord.TransferTypes.EsrOutbound,
                    this.logRecord.logID,
                    headerRecord.Filename,
                    LogRecord.LogReasonType.None,
                    string.Format(
                        "Adding ESR Assignment Costing record (Costing Allocation Id: {0}) to Person Id {1}",
                        costingRecord.ESRCostingAllocationId,
                        currentPersonRecord.ESRPersonId),
                    "ESRRecordProcessor : ProcessRecords()");

                currentPersonRecord.AddAssignmentCostingRecord(costingRecord);
            }
        }

        /// <summary>
        /// Processes an individual Vehicle record
        /// </summary>
        /// <param name="row">
        /// </param>
        /// <param name="currentPersonRecord">
        /// </param>
        /// <param name="rowIndex">
        /// </param>
        /// <param name="personRecords">
        /// The person Records.
        /// </param>
        /// <param name="processedRecords">
        /// The processed Records.
        /// </param>
        private void ProcessVehicleRecord(string row, ref EsrPersonRecord currentPersonRecord, int rowIndex)
        {
            EsrVehicleRecord vehicleRecord = EsrVehicleRecord.ParseEsrVehicleRecord(row);

            if (vehicleRecord == null)
            {
                this.LogRecordParseFailure(row, rowIndex);
                return;
            }

            if (currentPersonRecord != null)
            {
                this.logger.WriteDebug(
                    "expenses",
                    headerRecord.VpdNumber,
                    this.nAccountId,
                    LogRecord.LogItemTypes.OutboundFileProgressMessage,
                    LogRecord.TransferTypes.EsrOutbound,
                    this.logRecord.logID,
                    headerRecord.Filename,
                    LogRecord.LogReasonType.None,
                    string.Format(
                        "Adding ESR Vehicle record (Vehicle Allocation Id: {0}) to Person Id {1}",
                        vehicleRecord.ESRVehicleAllocationId,
                        currentPersonRecord.ESRPersonId),
                    "ESRRecordProcessor : ProcessRecords()");

                currentPersonRecord.AddVehicleRecord(vehicleRecord);
            }
        }

        /// <summary>
        /// Processes an individual Address record
        /// </summary>
        /// <param name="row"></param>
        /// <param name="currentPersonRecord"></param>
        /// <param name="rowIndex"></param>
        private void ProcessAddressRecord(string row, ref EsrPersonRecord currentPersonRecord, int rowIndex)
        {
            EsrAddressRecord adrRecord = EsrAddressRecord.ParseEsrAddressRecord(row);

            if (adrRecord == null)
            {
                this.LogRecordParseFailure(row, rowIndex);
                return;
            }

            if (currentPersonRecord != null)
            {
                this.logger.WriteDebug(
                    "expenses",
                    headerRecord.VpdNumber,
                    this.nAccountId,
                    LogRecord.LogItemTypes.OutboundFileProgressMessage,
                    LogRecord.TransferTypes.EsrOutbound,
                    this.logRecord.logID,
                    headerRecord.Filename,
                    LogRecord.LogReasonType.None,
                    string.Format(
                        "Adding ESR Address record (Address Id: {0}) to Person Id {1}",
                        adrRecord.ESRAddressId,
                        currentPersonRecord.ESRPersonId),
                    "ESRRecordProcessor : ProcessRecords()");

                currentPersonRecord.AddAddressRecord(adrRecord);
            }
        }

        /// <summary>
        /// Processes an individual Phone record
        /// </summary>
        /// <param name="row">
        /// </param>
        /// <param name="currentPersonRecord">
        /// </param>
        /// <param name="rowIndex">
        /// </param>
        /// <param name="personRecords">
        /// The person Records.
        /// </param>
        /// <param name="processedRecords">
        /// The processed Records.
        /// </param>
        private void ProcessPhoneRecord(string row, ref EsrPersonRecord currentPersonRecord, int rowIndex)
        {
            EsrPhoneRecord phoneRec = EsrPhoneRecord.ParseEsrPhoneRecord(row);

            if (phoneRec == null)
            {
                this.LogRecordParseFailure(row, rowIndex);
                return;
            }

            if (currentPersonRecord != null)
            {
                this.logger.WriteDebug(
                    "expenses",
                    headerRecord.VpdNumber,
                    this.nAccountId,
                    LogRecord.LogItemTypes.OutboundFileProgressMessage,
                    LogRecord.TransferTypes.EsrOutbound,
                    this.logRecord.logID,
                    headerRecord.Filename,
                    LogRecord.LogReasonType.None,
                    string.Format(
                        "Adding ESR Phone record (Phone Id: {0}) to Person Id {1}",
                        phoneRec.ESRPhoneId,
                        currentPersonRecord.ESRPersonId),
                    "ESRRecordProcessor : ProcessRecords()");

                currentPersonRecord.AddPhoneRecord(phoneRec);
            }
        }

        /// <summary>
        /// Processes an individual Assignment record
        /// </summary>
        /// <param name="row">
        /// </param>
        /// <param name="currentPersonRecord">
        /// </param>
        /// <param name="rowIndex">
        /// </param>
        /// <param name="personRecords">
        /// The person Records.
        /// </param>
        /// <param name="processedRecords">
        /// The processed Records.
        /// </param>
        private void ProcessAssignmentRecord(string row, ref EsrPersonRecord currentPersonRecord, int rowIndex)
        {
            EsrAssignmentRecord asgRec = EsrAssignmentRecord.ParseEsrAssignmentRecord(row);

            if (asgRec == null)
            {
                this.LogRecordParseFailure(row, rowIndex);
                return;
            }

            if (currentPersonRecord != null)
            {
                this.logger.WriteDebug(
                    "expenses",
                    headerRecord.VpdNumber,
                    this.nAccountId,
                    LogRecord.LogItemTypes.OutboundFileProgressMessage,
                    LogRecord.TransferTypes.EsrOutbound,
                    this.logRecord.logID,
                    headerRecord.Filename,
                    LogRecord.LogReasonType.None,
                    string.Format(
                        "Adding ESR Assignment record (Assignment Id: {0}) to Person Id {1}",
                        asgRec.AssignmentID,
                        currentPersonRecord.ESRPersonId),
                    "ESRRecordProcessor : ProcessRecords()");

                currentPersonRecord.AddAssignmentRecord(asgRec);
            }
        }

        /// <summary>
        /// Processes an individual Person record
        /// </summary>
        /// <param name="row"></param>
        /// <param name="currentPersonRecord"></param>
        /// <param name="personRecords"></param>
        /// <param name="currentRecordType"></param>
        /// <param name="processedRecords"></param>
        /// <param name="currentBatchEsrPersonId"></param>
        /// <param name="rowIndex"></param>
        /// <param name="isFileEnd"></param>
        private void ProcessPersonRecord(string row, ref EsrPersonRecord currentPersonRecord, ref List<EsrPersonRecord> personRecords, ref ApiRecordType currentRecordType, ref List<object> processedRecords, ref long currentBatchEsrPersonId, int rowIndex, bool isFileEnd = false)
        {
            EsrPersonRecord tmpPerson = EsrPersonRecord.ParseEsrPersonRecord(row);

            if (currentBatchEsrPersonId > 0 && ((tmpPerson != null && tmpPerson.ESRPersonId != currentBatchEsrPersonId) || tmpPerson == null) || isFileEnd)
            {
                // new person record, so save existing
                personRecords.Add(currentPersonRecord);
                currentRecordType = ApiRecordType.EsrPersonRecord;

                if (personRecords.Count >= EsrFile.RecordBatchSize)
                {
                    this.LogRecordBatchSend("Person");

                    processedRecords.AddRange(this.SendPersonRecords(ref personRecords));
                }
            }

            if (tmpPerson == null && !isFileEnd)
            {
                this.LogRecordParseFailure(row, rowIndex);
            }

            currentPersonRecord = tmpPerson;
            currentBatchEsrPersonId = currentPersonRecord != null ? currentPersonRecord.ESRPersonId : 0;
        }

        /// <summary>
        /// Processes an individual Position record
        /// </summary>
        /// <param name="row"></param>
        /// <param name="positionRecords"></param>
        /// <param name="currentRecordType"></param>
        /// <param name="processedRecords"></param>
        /// <param name="rowIndex"></param>
        private void ProcessPositionRecord(string row, ref List<EsrPositionRecord> positionRecords, ref ApiRecordType currentRecordType, ref List<object> processedRecords, int rowIndex)
        {
            EsrPositionRecord positionRecord = EsrPositionRecord.ParseEsrPositionRecord(row);

            if (positionRecord == null)
            {
                this.LogRecordParseFailure(row, rowIndex);
                return;
            }

            positionRecords.Add(positionRecord);
            currentRecordType = ApiRecordType.EsrPositionRecord;

            if (positionRecords.Count >= EsrFile.RecordBatchSize)
            {
                this.LogRecordBatchSend("Position");

                processedRecords.AddRange(this.SendPositionRecords(ref positionRecords));
            }
        }

        /// <summary>
        /// Processes an individual Organisation record
        /// </summary>
        /// <param name="row"></param>
        /// <param name="organisationRecords"></param>
        /// <param name="currentRecordType"></param>
        /// <param name="processedRecords"></param>
        /// <param name="rowIndex"></param>
        private void ProcessOrganisationRecord(string row, ref List<EsrOrganisationRecord> organisationRecords, ref ApiRecordType currentRecordType, ref List<object> processedRecords, int rowIndex)
        {
            EsrOrganisationRecord organisationRecord = EsrOrganisationRecord.ParseEsrOrganisationRecord(row);

            if (organisationRecord == null)
            {
                this.LogRecordParseFailure(row, rowIndex);
                return;
            }

            organisationRecords.Add(organisationRecord);
            currentRecordType = ApiRecordType.EsrOrganisationRecord;

            if (organisationRecords.Count >= EsrFile.RecordBatchSize)
            {
                this.LogRecordBatchSend("Organisation");

                processedRecords.AddRange(this.SendOrganisationRecords(ref organisationRecords));
            }
        }

        /// <summary>
        /// Processes an individual Location record
        /// </summary>
        /// <param name="row"></param>
        /// <param name="locationRecords"></param>
        /// <param name="currentRecordType"></param>
        /// <param name="processedRecords"></param>
        /// <param name="rowIndex"></param>
        private void ProcessLocationRecord(string row, ref List<EsrLocationRecord> locationRecords, ref ApiRecordType currentRecordType, ref List<object> processedRecords, int rowIndex)
        {
            EsrLocationRecord locationRecord = EsrLocationRecord.ParseEsrLocationRecord(row);

            if (locationRecord == null)
            {
                this.LogRecordParseFailure(row, rowIndex);
                return;
            }

            locationRecords.Add(locationRecord);
            currentRecordType = ApiRecordType.EsrLocationRecord;

            if (locationRecords.Count >= EsrFile.RecordBatchSize)
            {
                this.LogRecordBatchSend("Location");

                processedRecords.AddRange(this.SendLocationRecords(ref locationRecords));
            }
        }

        /// <summary>
        /// Log message to API Log that batch of records issued to API for processing
        /// </summary>
        /// <param name="recordTypeName">Text name of record type</param>
        /// <param name="recordCount">The number of records being written</param>
        private void LogRecordBatchSend(string recordTypeName, int recordCount = 0)
        {
            string message = recordCount == 0 ? string.Format("Issuing batch of {0} Records to API for processing", recordTypeName) : string.Format("Issuing remaining batch of {0} {1} Records to API for processing", recordCount, recordTypeName);

            this.logger.WriteDebug(
                                "expenses",
                                headerRecord.VpdNumber,
                                this.nAccountId,
                                LogRecord.LogItemTypes.OutboundFileProgressMessage,
                                LogRecord.TransferTypes.EsrOutbound,
                                this.logRecord.logID,
                                headerRecord.Filename,
                                LogRecord.LogReasonType.None,
                                message,
                                "ESRRecordProcessor : ProcessRecords()");
        }

        /// <summary>
        /// Logs record processing status of records
        /// </summary>
        /// <param name="processedRecords">
        /// Record statuses to be logged
        /// </param>
        private void LogRecordProcessingStatus(IEnumerable<object> processedRecords)
        {
            int rowIdx = 1;
            int importStatus = 0;

            foreach (var record in processedRecords)
            {
                DataClassBase baseObject = (DataClassBase)record;

                string resultMessage;
                string resultStatus = string.Empty;

                switch (baseObject.ClassName())
                {
                    case "EsrLocationRecord":
                        EsrLocationRecord locationRecord = DataClassBase.GetRecordFromDataClass(baseObject, typeof(EsrLocationRecord)) as EsrLocationRecord;

                        resultStatus = GetApiResultStatus(locationRecord.ActionResult.Result);
                        resultMessage = string.IsNullOrEmpty(locationRecord.ActionResult.Message) ? string.Empty : string.Format("Message: {0}", locationRecord.ActionResult.Message);

                        this.logger.Write(
                            "expenses",
                            headerRecord.VpdNumber,
                            this.nAccountId,
                            LogRecord.LogItemTypes.OutboundFileProgressMessage,
                            LogRecord.TransferTypes.EsrOutbound,
                            this.logRecord.logID,
                            headerRecord.Filename,
                            this.GetLogRecordType(locationRecord.ActionResult.Result),
                            this.FormatMessage(Message, rowIdx++, "ESR Location", locationRecord.ESRLocationId, resultStatus, resultMessage, locationRecord.ActionResult.Result),
                            "ESRRecordProcessor : LogRecordProcessingStatus()");
                        break;

                    case "EsrOrganisationRecord":
                        EsrOrganisationRecord organisationRecord = DataClassBase.GetRecordFromDataClass(baseObject, typeof(EsrOrganisationRecord)) as EsrOrganisationRecord;

                        resultStatus = GetApiResultStatus(organisationRecord.ActionResult.Result);
                        resultMessage = string.IsNullOrEmpty(organisationRecord.ActionResult.Message) ? string.Empty : string.Format("Message: {0}", organisationRecord.ActionResult.Message);

                        this.logger.Write(
                            "expenses",
                            headerRecord.VpdNumber,
                            this.nAccountId,
                            LogRecord.LogItemTypes.OutboundFileProgressMessage,
                            LogRecord.TransferTypes.EsrOutbound,
                            this.logRecord.logID,
                            headerRecord.Filename,
                            this.GetLogRecordType(organisationRecord.ActionResult.Result),
                            this.FormatMessage(Message, rowIdx++, "ESR Organisation", organisationRecord.ESROrganisationId, resultStatus, resultMessage, organisationRecord.ActionResult.Result),
                            "ESRRecordProcessor : LogRecordProcessingStatus()");
                        break;

                    case "EsrPositionRecord":
                        EsrPositionRecord positionRecord = DataClassBase.GetRecordFromDataClass(baseObject, typeof(EsrPositionRecord)) as EsrPositionRecord;

                        resultStatus = GetApiResultStatus(positionRecord.ActionResult.Result);
                        resultMessage = string.IsNullOrEmpty(positionRecord.ActionResult.Message) ? string.Empty : string.Format("Message: {0}", positionRecord.ActionResult.Message);

                        this.logger.Write(
                            "expenses",
                            headerRecord.VpdNumber,
                            this.nAccountId,
                            LogRecord.LogItemTypes.OutboundFileProgressMessage,
                            LogRecord.TransferTypes.EsrOutbound,
                            this.logRecord.logID,
                            headerRecord.Filename,
                            this.GetLogRecordType(positionRecord.ActionResult.Result),
                            this.FormatMessage(Message, rowIdx++, "ESR Position", positionRecord.ESRPositionId, resultStatus, resultMessage, positionRecord.ActionResult.Result),
                            "ESRRecordProcessor : LogRecordProcessingStatus()");
                        break;

                    case "EsrPersonRecord":
                        var personRecord = DataClassBase.GetRecordFromDataClass(baseObject, typeof(EsrPersonRecord)) as EsrPersonRecord;

                        if (personRecord != null)
                        {
                            resultStatus = GetApiResultStatus(personRecord.ActionResult.Result);
                            resultMessage = string.IsNullOrEmpty(personRecord.ActionResult.Message) ? string.Empty : string.Format("Message: {0}", personRecord.ActionResult.Message);

                            this.logger.Write(
                                "expenses",
                                this.headerRecord.VpdNumber,
                                this.nAccountId,
                                LogRecord.LogItemTypes.OutboundFileProgressMessage,
                                LogRecord.TransferTypes.EsrOutbound,
                                this.logRecord.logID,
                                this.headerRecord.Filename,
                                this.GetLogRecordType(personRecord.ActionResult.Result),
                                this.FormatMessage(Message, rowIdx++, "ESR Person", personRecord.ESRPersonId, resultStatus, resultMessage, personRecord.ActionResult.Result),
                                "ESRRecordProcessor : LogRecordProcessingStatus()");

                            if (personRecord.ActionResult.Result != ApiActionResult.Failure)
                            {
                                // need to log the child record statuses
                                if (personRecord.EsrAssignments != null)
                                {
                                    foreach (EsrAssignment esrAssignment in personRecord.EsrAssignments)
                                    {
                                        resultStatus = GetApiResultStatus(esrAssignment.ActionResult.Result);
                                        resultMessage = string.IsNullOrEmpty(esrAssignment.ActionResult.Message) ? string.Empty : string.Format("Message: {0}", esrAssignment.ActionResult.Message);

                                        this.logger.Write(
                                            "expenses",
                                            this.headerRecord.VpdNumber,
                                            this.nAccountId,
                                            LogRecord.LogItemTypes.OutboundFileProgressMessage,
                                            LogRecord.TransferTypes.EsrOutbound,
                                            this.logRecord.logID,
                                            this.headerRecord.Filename,
                                            this.GetLogRecordType(esrAssignment.ActionResult.Result),
                                            this.FormatMessage(Message, rowIdx++, "ESR Assignment", esrAssignment.AssignmentID, resultStatus, resultMessage, esrAssignment.ActionResult.Result),
                                            "ESRRecordProcessor : LogRecordProcessingStatus()");
                                    }
                                }

                                if (personRecord.Phones != null)
                                {
                                    foreach (EsrPhone esrPhone in personRecord.Phones)
                                    {
                                        resultStatus = GetApiResultStatus(esrPhone.ActionResult.Result);
                                        resultMessage = string.IsNullOrEmpty(esrPhone.ActionResult.Message) ? string.Empty : string.Format("Message: {0}", esrPhone.ActionResult.Message);

                                        this.logger.Write(
                                            "expenses",
                                            this.headerRecord.VpdNumber,
                                            this.nAccountId,
                                            LogRecord.LogItemTypes.OutboundFileProgressMessage,
                                            LogRecord.TransferTypes.EsrOutbound,
                                            this.logRecord.logID,
                                            this.headerRecord.Filename,
                                            this.GetLogRecordType(esrPhone.ActionResult.Result),
                                            this.FormatMessage(Message, rowIdx++, "ESR Phone", esrPhone.ESRPhoneId, resultStatus, resultMessage, esrPhone.ActionResult.Result),
                                            "ESRRecordProcessor : LogRecordProcessingStatus()");
                                    }
                                }

                                if (personRecord.Addresses != null)
                                {
                                    foreach (EsrAddress esrAddress in personRecord.Addresses)
                                    {
                                        resultStatus = GetApiResultStatus(esrAddress.ActionResult.Result);
                                        resultMessage = string.IsNullOrEmpty(esrAddress.ActionResult.Message) ? string.Empty : string.Format("Message: {0}", esrAddress.ActionResult.Message);

                                        this.logger.Write(
                                            "expenses",
                                            this.headerRecord.VpdNumber,
                                            this.nAccountId,
                                            LogRecord.LogItemTypes.OutboundFileProgressMessage,
                                            LogRecord.TransferTypes.EsrOutbound,
                                            this.logRecord.logID,
                                            this.headerRecord.Filename,
                                            this.GetLogRecordType(esrAddress.ActionResult.Result),
                                            this.FormatMessage(Message, rowIdx++, "ESR Address", esrAddress.ESRAddressId, resultStatus, resultMessage, esrAddress.ActionResult.Result),
                                            "ESRRecordProcessor : LogRecordProcessingStatus()");
                                    }
                                }

                                if (personRecord.Vehicles != null)
                                {
                                    foreach (EsrVehicle esrVehicle in personRecord.Vehicles)
                                    {
                                        resultStatus = GetApiResultStatus(esrVehicle.ActionResult.Result);
                                        resultMessage = string.IsNullOrEmpty(esrVehicle.ActionResult.Message) ? string.Empty : string.Format("Message: {0}", esrVehicle.ActionResult.Message);

                                        this.logger.Write(
                                            "expenses",
                                            this.headerRecord.VpdNumber,
                                            this.nAccountId,
                                            LogRecord.LogItemTypes.OutboundFileProgressMessage,
                                            LogRecord.TransferTypes.EsrOutbound,
                                            this.logRecord.logID,
                                            this.headerRecord.Filename,
                                            this.GetLogRecordType(esrVehicle.ActionResult.Result),
                                            this.FormatMessage(Message, rowIdx++, "ESR Vehicle", esrVehicle.ESRVehicleAllocationId, resultStatus, resultMessage, esrVehicle.ActionResult.Result),
                                            "ESRRecordProcessor : LogRecordProcessingStatus()");
                                    }
                                }

                                if (personRecord.AssignmentCostings != null)
                                {
                                    foreach (EsrAssignmentCostingRecord esrAssignmentCostingRecord in personRecord.AssignmentCostings)
                                    {
                                        resultStatus = GetApiResultStatus(esrAssignmentCostingRecord.ActionResult.Result);
                                        resultMessage = string.IsNullOrEmpty(esrAssignmentCostingRecord.ActionResult.Message) ? string.Empty : string.Format("Message: {0}", esrAssignmentCostingRecord.ActionResult.Message);
                                        this.logger.Write(
                                            "expenses",
                                            this.headerRecord.VpdNumber,
                                            this.nAccountId,
                                            LogRecord.LogItemTypes.OutboundFileProgressMessage,
                                            LogRecord.TransferTypes.EsrOutbound,
                                            this.logRecord.logID,
                                            this.headerRecord.Filename,
                                            this.GetLogRecordType(esrAssignmentCostingRecord.ActionResult.Result),
                                            this.FormatMessage(Message, rowIdx++, "ESR Assignment Costing", esrAssignmentCostingRecord.ESRCostingAllocationId, resultStatus, resultMessage, esrAssignmentCostingRecord.ActionResult.Result),
                                            "ESRRecordProcessor : LogRecordProcessingStatus()");
                                    }
                                }
                            }
                        }
                        break;
                    case "EsrVehicleRecord":
                    case "EsrVehicle":
                    case "EsrPhone":
                    case "EsrPhoneRecord":
                    case "EsrAddressRecord":
                    case "EsrAddress":
                    case "EsrAssignmentRecord":
                    case "EsrAssignment":
                    case "EsrAssignmentCostingRecord":
                    case "EsrAssignmentCostings":
                        resultStatus = this.CreateLogEntryForObject(baseObject, rowIdx);
                        rowIdx++;
                        break;
                }

                if (resultStatus != null)
                {
                    importStatus = this.UpdateImportStatus(resultStatus, importStatus);
                }
            }


            this.logRecord = this.UpdateLog();
            foreach (ImportHistory history in importHistories)
            {
                history.Action = Action.Update;
                history.importStatus = importStatus;
            }

            importHistories = historyApi.Send(this.nAccountId, "", importHistories);
        }

        /// <summary>
        /// The create log entry for object.
        /// </summary>
        /// <param name="baseObject">
        /// The base object.
        /// </param>
        /// <param name="rowIdx">
        /// The row index.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string CreateLogEntryForObject(DataClassBase baseObject, int rowIdx)
        {
            var resultStatus = GetApiResultStatus(baseObject.ActionResult.Result);
            var resultMessage = string.IsNullOrEmpty(baseObject.ActionResult.Message) ? string.Empty : string.Format("Message: {0}", baseObject.ActionResult.Message);
            var keyValue = baseObject.KeyValue;
            if (baseObject.ClassName() == "EsrAssignmentRecord")
            {
                keyValue = ((EsrAssignmentRecord)baseObject).AssignmentID;
            }

            this.logger.Write(
                "expenses",
                this.headerRecord.VpdNumber,
                this.nAccountId,
                LogRecord.LogItemTypes.OutboundFileProgressMessage,
                LogRecord.TransferTypes.EsrOutbound,
                this.logRecord.logID,
                this.headerRecord.Filename,
                this.GetLogRecordType(baseObject.ActionResult.Result),
                this.FormatMessage(Message, rowIdx, Utilities.StringManipulation.Spacing.AddSpaceBeforeCapitals(baseObject.ClassName()), keyValue, resultStatus, resultMessage, baseObject.ActionResult.Result),
                "ESRRecordProcessor : LogRecordProcessingStatus()");
            return resultStatus;
        }

        /// <summary>
        /// The get log record type.
        /// </summary>
        /// <param name="actionResult">
        /// The action result.
        /// </param>
        /// <returns>
        /// The <see cref="LogRecord.LogReasonType"/>.
        /// </returns>
        private LogRecord.LogReasonType GetLogRecordType(ApiActionResult actionResult)
        {
            var result = LogRecord.LogReasonType.None;
            switch (actionResult)
            {
                case ApiActionResult.Deleted:
                    result = LogRecord.LogReasonType.SuccessDelete;
                    this.logRecord.successfulLines++;
                    break;
                case ApiActionResult.Failure:
                    result = LogRecord.LogReasonType.Error;
                    this.logRecord.failedLines++;
                    break;
                case ApiActionResult.ForeignKeyFail:
                    result = LogRecord.LogReasonType.Warning;
                    this.logRecord.warningLines++;
                    break;
                case ApiActionResult.NoAction:
                    result = LogRecord.LogReasonType.None;
                    break;
                case ApiActionResult.PartialSuccess:
                    result = LogRecord.LogReasonType.Warning;
                    this.logRecord.warningLines++;
                    break;
                case ApiActionResult.Success:
                    result = LogRecord.LogReasonType.Success;
                    this.logRecord.successfulLines++;
                    break;
                case ApiActionResult.ValidationFailed:
                    result = LogRecord.LogReasonType.Warning;
                    this.logRecord.failedLines++;
                    break;
            }

            return result;
        }

        /// <summary>
        /// The format message.
        /// </summary>
        /// <param name="messageString">
        /// The line record id processed processing was.
        /// </param>
        /// <param name="lineNumber">
        /// The i.
        /// </param>
        /// <param name="entityName">
        /// The entity name.
        /// </param>
        /// <param name="entityId">
        /// The entity id.
        /// </param>
        /// <param name="resultStatus">
        /// The result status.
        /// </param>
        /// <param name="resultMessage">
        /// The result message.
        /// </param>
        /// <param name="actionResult">
        /// The action Result.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string FormatMessage(string messageString, int lineNumber, string entityName, int entityId, string resultStatus, string resultMessage, ApiActionResult actionResult)
        {
            var result = string.Format(messageString, this.GetResultColour(actionResult), lineNumber, entityName, entityId, resultStatus, resultMessage);
            return result;
        }

        /// <summary>
        /// The format message.
        /// </summary>
        /// <param name="messageString">
        /// The line record id processed processing was.
        /// </param>
        /// <param name="lineNumber">
        /// The i.
        /// </param>
        /// <param name="entityName">
        /// The entity name.
        /// </param>
        /// <param name="entityId">
        /// The entity id.
        /// </param>
        /// <param name="resultStatus">
        /// The result status.
        /// </param>
        /// <param name="resultMessage">
        /// The result message.
        /// </param>
        /// <param name="actionResult">
        /// The action Result.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string FormatMessage(string messageString, int lineNumber, string entityName, long entityId, string resultStatus, string resultMessage, ApiActionResult actionResult)
        {
            var result = string.Format(messageString, this.GetResultColour(actionResult), lineNumber, entityName, entityId, resultStatus, resultMessage);
            return result;
        }

        /// <summary>
        /// Set the result colour.
        /// </summary>
        /// <param name="result">
        /// The result.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string GetResultColour(ApiActionResult result)
        {
            switch (result)
            {
                case ApiActionResult.Deleted:
                    return "Green";
                case ApiActionResult.Success:
                    return "Green";
                case ApiActionResult.Failure:
                    return "Red";
                case ApiActionResult.ForeignKeyFail:
                    return "maroon";
                case ApiActionResult.NoAction:
                    return "Red";
                case ApiActionResult.PartialSuccess:
                    return "maroon";
                case ApiActionResult.ValidationFailed:
                    return "Red";
            }

            return string.Empty;
        }

        /// <summary>
        /// Update import status.
        /// </summary>
        /// <param name="resultStatus">
        /// The result status.
        /// </param>
        /// <param name="importStatus">
        /// The import status.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        private int UpdateImportStatus(string resultStatus, int importStatus)
        {
            int newStatus = -1;
            switch (resultStatus)
            {
                case "No Action":
                    newStatus = 3;
                    break;
                case "Deleted":
                    newStatus = 0;
                    break;
                case "Failure":
                    newStatus = 1;
                    break;
                case "Partial Success":
                    newStatus = 2;
                    break;
                case "Success":
                    newStatus = 0;
                    break;
                case "Validation Failed":
                    newStatus = 1;
                    break;
            }
            if (newStatus > importStatus)
            {
                return newStatus;
            }

            return importStatus;
        }

        /// <summary>
        /// Send Location record to Api for processing
        /// </summary>
        /// <param name="recs">Collection of ESR Location Records</param>
        private IEnumerable<EsrLocationRecord> SendLocationRecords(ref List<EsrLocationRecord> recs)
        {
            List<EsrLocationRecord> outputRecs = new List<EsrLocationRecord>();

            if (recs.Count == 0)
            {
                return outputRecs;
            }

            try
            {
                LocationApi locApi = new LocationApi();
                outputRecs.AddRange(locApi.Send(nAccountId, headerRecord.VpdNumber.ToString(), recs));
                recs.Clear();
            }
            catch (Exception ex)
            {
                this.logger.Write(
                    "expenses",
                    headerRecord.VpdNumber,
                    nAccountId,
                    LogRecord.LogItemTypes.OutboundFileValidationFailed,
                    LogRecord.TransferTypes.EsrOutbound,
                    this.logRecord.logID,
                    headerRecord.Filename,
                    LogRecord.LogReasonType.Error,
                    string.Format("Error Processing LocationRecords : {0}", ex.Message),
                    ex.Source);

                this.logger.WriteDebug(
                    "expenses",
                    headerRecord.VpdNumber,
                    nAccountId,
                    LogRecord.LogItemTypes.OutboundFileValidationFailed,
                    LogRecord.TransferTypes.EsrOutbound,
                    this.logRecord.logID,
                    headerRecord.Filename,
                    LogRecord.LogReasonType.Error,
                    ex.StackTrace,
                    ex.Source);
            }

            return outputRecs;
        }

        /// <summary>
        /// Send Organisation record to Api for processing
        /// </summary>
        /// <param name="recs">Collection of ESR Organisation Records</param>
        private IEnumerable<EsrOrganisationRecord> SendOrganisationRecords(ref List<EsrOrganisationRecord> recs)
        {
            List<EsrOrganisationRecord> outputRecs = new List<EsrOrganisationRecord>();
            if (recs.Count == 0)
            {
                return outputRecs;
            }

            try
            {
                OrganisationApi orgApi = new OrganisationApi();
                outputRecs.AddRange(orgApi.Send(nAccountId, headerRecord.VpdNumber.ToString(), recs));
                recs.Clear();
            }
            catch (Exception ex)
            {
                this.logger.Write(
                    "expenses",
                    headerRecord.VpdNumber,
                    nAccountId,
                    LogRecord.LogItemTypes.OutboundFileValidationFailed,
                    LogRecord.TransferTypes.EsrOutbound,
                    this.logRecord.logID,
                    headerRecord.Filename,
                    LogRecord.LogReasonType.Error,
                    string.Format("Error Processing OrganisationRecords : {0}", ex.Message),
                    ex.Source);

                this.logger.WriteDebug(
                    "expenses",
                    headerRecord.VpdNumber,
                    nAccountId,
                    LogRecord.LogItemTypes.OutboundFileValidationFailed,
                    LogRecord.TransferTypes.EsrOutbound,
                    this.logRecord.logID,
                    headerRecord.Filename,
                    LogRecord.LogReasonType.Error,
                    ex.StackTrace,
                    ex.Source);
            }

            return outputRecs;
        }

        /// <summary>
        /// Send Position record to Api for processing
        /// </summary>
        /// <param name="recs">Collection of ESR Position Records</param>
        private IEnumerable<EsrPositionRecord> SendPositionRecords(ref List<EsrPositionRecord> recs)
        {
            List<EsrPositionRecord> outputRecs = new List<EsrPositionRecord>();
            if (recs.Count == 0)
            {
                return outputRecs;
            }

            try
            {
                PositionApi posApi = new PositionApi();
                outputRecs.AddRange(posApi.Send(nAccountId, headerRecord.VpdNumber.ToString(), recs));
                recs.Clear();
            }
            catch (Exception ex)
            {
                this.logger.Write(
                    "expenses",
                    headerRecord.VpdNumber,
                    nAccountId,
                    LogRecord.LogItemTypes.OutboundFileValidationFailed,
                    LogRecord.TransferTypes.EsrOutbound,
                    this.logRecord.logID,
                    headerRecord.Filename,
                    LogRecord.LogReasonType.Error,
                    string.Format("Error Processing PositionRecords : {0}", ex.Message),
                    ex.Source);

                this.logger.WriteDebug(
                    "expenses",
                    headerRecord.VpdNumber,
                    nAccountId,
                    LogRecord.LogItemTypes.OutboundFileValidationFailed,
                    LogRecord.TransferTypes.EsrOutbound,
                    this.logRecord.logID,
                    headerRecord.Filename,
                    LogRecord.LogReasonType.Error,
                    ex.StackTrace,
                    ex.Source);
            }

            return outputRecs;
        }

        /// <summary>
        /// Send Person record to Api for processing
        /// </summary>
        /// <param name="recs">Collection of ESR Person Records</param>
        private IEnumerable<EsrPersonRecord> SendPersonRecords(ref List<EsrPersonRecord> recs)
        {
            List<EsrPersonRecord> outputRecs = new List<EsrPersonRecord>();

            if (recs.Count == 0)
            {
                return outputRecs;
            }

            try
            {
                PersonsApi personsApi = new PersonsApi();
                outputRecs.AddRange(personsApi.Send(nAccountId, headerRecord.VpdNumber.ToString(), recs));
                recs.Clear();
            }
            catch (Exception ex)
            {
                this.logger.Write(
                    "expenses",
                    headerRecord.VpdNumber,
                    nAccountId,
                    LogRecord.LogItemTypes.OutboundFileValidationFailed,
                    LogRecord.TransferTypes.EsrOutbound,
                    this.logRecord.logID,
                    headerRecord.Filename,
                    LogRecord.LogReasonType.Error,
                    string.Format("Error Processing PersonRecords : {0}", ex.Message),
                    ex.Source);

                this.logger.WriteDebug(
                    "expenses",
                    headerRecord.VpdNumber,
                    nAccountId,
                    LogRecord.LogItemTypes.OutboundFileValidationFailed,
                    LogRecord.TransferTypes.EsrOutbound,
                    this.logRecord.logID,
                    headerRecord.Filename,
                    LogRecord.LogReasonType.Error,
                    ex.StackTrace,
                    ex.Source);
                recs.Clear();
            }

            return outputRecs;
        }

        /// <summary>
        /// Delete entity.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        /// <returns>
        /// The <see cref="DataClassBase"/>.
        /// </returns>
        private DataClassBase DeleteEntity(ref DataClassBase entity)
        {
            var outputRecs = new DataClassBase();

            if (entity == null)
            {
                return outputRecs;
            }

            try
            {
                var baseApi = new DataClassBaseApi();
                outputRecs = baseApi.Delete(this.nAccountId, entity);
            }
            catch (Exception ex)
            {
                this.logger.Write(
                    "expenses",
                    headerRecord.VpdNumber,
                    nAccountId,
                    LogRecord.LogItemTypes.OutboundFileValidationFailed,
                    LogRecord.TransferTypes.EsrOutbound,
                    this.logRecord.logID,
                    headerRecord.Filename,
                    LogRecord.LogReasonType.Error,
                    string.Format("Error Processing PersonRecords : {0}", ex.Message),
                    ex.Source);

                this.logger.WriteDebug(
                    "expenses",
                    headerRecord.VpdNumber,
                    nAccountId,
                    LogRecord.LogItemTypes.OutboundFileValidationFailed,
                    LogRecord.TransferTypes.EsrOutbound,
                    this.logRecord.logID,
                    headerRecord.Filename,
                    LogRecord.LogReasonType.Error,
                    ex.StackTrace,
                    ex.Source);
            }

            return outputRecs;
        }

        /// <summary>
        /// Converts the API Result Status to a string expression
        /// </summary>
        /// <param name="result">Text readable form of Result status</param>
        /// <returns></returns>
        private static string GetApiResultStatus(ApiActionResult result)
        {
            string retStr = string.Empty;

            switch (result)
            {
                case ApiActionResult.NoAction:
                    retStr = "No Action";
                    break;
                case ApiActionResult.Deleted:
                    retStr = "Deleted";
                    break;
                case ApiActionResult.Failure:
                    retStr = "Failure";
                    break;
                case ApiActionResult.PartialSuccess:
                    retStr = "Partial Success";
                    break;
                case ApiActionResult.Success:
                    retStr = "Success";
                    break;
                case ApiActionResult.ValidationFailed:
                    retStr = "Validation Failed";
                    break;
            }

            return retStr;
        }

        /// <summary>
        /// Write to the API Log that the parsing of a data row in the file failed
        /// </summary>
        /// <param name="dataRow">Record row from the file that failed</param>
        /// <param name="rowIdx">Line Number of the row in the file</param>
        private void LogRecordParseFailure(string dataRow, int rowIdx)
        {
            this.logger.Write(
                "expenses",
                headerRecord.VpdNumber,
                this.nAccountId,
                LogRecord.LogItemTypes.OutboundFileProgressMessage,
                LogRecord.TransferTypes.EsrOutbound,
                this.logRecord.logID,
                headerRecord.Filename,
                LogRecord.LogReasonType.Error,
                string.Format("Record could not be parsed at Line {0}.\nRecord: {1}", rowIdx, dataRow),
                "ESRRecordProcessor : ProcessRecords()");
        }
    }
}