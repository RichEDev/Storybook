namespace SELFileTransferService
{
    using System;
    using System.Collections.Generic;
    using SpendManagementLibrary;
    using SpendManagementLibrary.Helpers;
    using System.Configuration;
    using SpendManagementLibrary.ESRTransferServiceClasses;
    using System.Threading;
    using Common.Logging;

    /// <summary>
    /// A class to process Inbound files from the expenses website into the ESRFileTransfer database
    /// </summary>
    public class cInboundTransfers
    {
        /// <summary>
        /// An instance of <see cref="Remoting"/>
        /// </summary>
        private readonly Remoting _remoting;

        /// <summary>
        /// An instance of <see cref="ILog"/> for logging <see cref="ICache{T,TK}"/> diagnostics and information.
        /// </summary>
        private static readonly ILog Log = new LogFactory<cInboundTransfers>().GetLogger();

        /// <summary>
        /// Create a new instance of <see cref="cInboundTransfers"/>
        /// </summary>
        /// <param name="remoting"></param>
        public cInboundTransfers(Remoting remoting)
        {
            this._remoting = remoting;
        }

        private static bool bInboundTransfersProcessing;

        #region Properties

        /// <summary>
        /// Flag to set so only one Inbound transfer occurs at once
        /// </summary>
        public static bool InboundTransfersProcessing
        {
            get { return bInboundTransfersProcessing; }
            set { bInboundTransfersProcessing = value; }
        }

        #endregion

        /// <summary>
        /// Check to see if there are any new inbound files ready for going up to ESR
        /// </summary>
        public void CheckForInboundFiles(IESR esr)
        {
            //Check to see if the inbound transfers are already processing
            if (InboundTransfersProcessing)
            {
                return;
            }

            Log.Info("Checking for inbound files.");

            SELFileTransferService clsService = new SELFileTransferService();

            try
            {
                //Set to true so others dont run
                InboundTransfersProcessing = true;

                cESRTransferLogging clsTransferLogging = new cESRTransferLogging(ConfigurationManager.ConnectionStrings["ESRFileTransfer"].ConnectionString);
                cNHSTrusts clsTrusts = new cNHSTrusts(ConfigurationManager.ConnectionStrings["ESRFileTransfer"].ConnectionString);
                List<cAutomatedTransfer> lstTransfers = esr.GetReadyTransfers();

                if (Log.IsInfoEnabled)
                {
                    Log.Info($"{lstTransfers.Count} transfers found waiting");
                }

                byte[] FileData = new byte[0];
                string FileName = "";
                int NHSTrustID = 0;
                ReportRequestStatus status = ReportRequestStatus.Queued;

                foreach (cAutomatedTransfer transfer in lstTransfers)
                {
                    if (transfer.TransferType == AutomatedTransferType.ESRInbound)
                    {
                        try
                        {
                            NHSTrustID = clsTrusts.GetTrustIDByExpTrustIDAndAccountID(transfer.trustID, transfer.accountID);
                            cReportRequest expRequest = esr.startInboundExport(transfer.accountID, transfer.financialExportID, transfer.exportHistoryID);

                            //Add the processing transfer to the static collection. Due to the possibility of the service being called multiple
                            //times whilst an inbound file is being transfered the collection needs to be locked when being accessed due to the possibility
                            //of multiple threads. 

                            FileName = expRequest.report.exportoptions.ExportFileName;

                            if (Log.IsInfoEnabled)
                            {
                                Log.Info($"File name set ({FileName})");
                            }

                            status = ReportRequestStatus.Queued;

                            while (status == ReportRequestStatus.BeingProcessed || status == ReportRequestStatus.Queued)
                            {
                                object[] data = esr.getExportProgress(expRequest);
                                if (Log.IsInfoEnabled)
                                {
                                    Log.Info($"{data[0]} {data[1]} {data[2]} {data[3]}");
                                }
                                status = (ReportRequestStatus)Enum.Parse(typeof(ReportRequestStatus), data[0].ToString(), true);

                                Log.Info("Processing report ");

                                Thread.Sleep(1000);
                            }

                            switch (status)
                            {
                                case ReportRequestStatus.Complete:
                                    Log.Info("Inbound report creation completed.");
                                    //Get the file data from the web service
                                    FileData = esr.GetExportData(expRequest);
                                    this.SaveInboundFile(transfer, FileName, FileData, FinancialExportStatus.CollectedForUploadToESR);
                                    esr.UpdateTransferStatus(transfer.accountID, transfer.exportHistoryID, FinancialExportStatus.CollectedForUploadToESR);

                                    //Log the successful Inbound file processing
                                    clsTransferLogging.SaveLogItem(new cESRTransferLogItem(0, TransferLogItemType.InboundFileProcessedSuccessfully, AutomatedTransferType.ESRInbound, NHSTrustID, FileName, false, "", null, null));
                                    break;

                                case ReportRequestStatus.Failed:
                                    FileData = new byte[0];
                                    Log.Info("Inbound report creation failed.");
                                    esr.UpdateTransferStatus(transfer.accountID, transfer.exportHistoryID, FinancialExportStatus.ExportFailed);

                                    //Log the failure of the Inbound file processing
                                    clsTransferLogging.SaveLogItem(new cESRTransferLogItem(0, TransferLogItemType.InboundFileProcessingFailed, AutomatedTransferType.ESRInbound, NHSTrustID, FileName, false, "", null, null));

                                    break;
                            }
                        }
                        catch (Exception ex)
                        {
                            Log.Warn(ex.Message + "\n" + ex.StackTrace);
                            esr.UpdateTransferStatus(transfer.accountID, transfer.exportHistoryID, FinancialExportStatus.ExportFailed);
                            //Log the failure of the Inbound file processing
                            clsTransferLogging.SaveLogItem(new cESRTransferLogItem(0, TransferLogItemType.InboundFileProcessingFailed, AutomatedTransferType.ESRInbound, NHSTrustID, FileName, false, "", null, null));
                            //Send Error Email
                            clsService.SendErrorEmail(ex);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Warn(ex.Message + "\n" + ex.StackTrace);
                //This catch is required as there is a finally to set the flag back to false
                clsService.SendErrorEmail(ex);
            }
            finally
            {
                //Allow the next inbound transfer to process
                InboundTransfersProcessing = false;
            }
        }

        /// <summary>
        /// Save the inbound file record with the byte array to the database
        /// </summary>
        /// <param name="transfer"></param>
        /// <param name="FileName"></param>
        /// <param name="FileData"></param>
        /// <param name="status"></param>
        public void SaveInboundFile(cAutomatedTransfer transfer, string FileName, byte[] FileData, FinancialExportStatus status)
        {
            if (Log.IsDebugEnabled)
            {
                Log.Debug($"SaveInboundFile -  {FileName}");
            }

            var connectionString = ConfigurationManager.ConnectionStrings["ESRFileTransfer"].ConnectionString;
            using (var smData = new DatabaseConnection(connectionString))
            {
                cNHSTrusts clsTrusts = new cNHSTrusts(connectionString);
                int nhsTrustId = clsTrusts.GetTrustIDByExpTrustIDAndAccountID(transfer.trustID, transfer.accountID);

                smData.sqlexecute.Parameters.AddWithValue("@FileData", FileData);
                smData.sqlexecute.Parameters.AddWithValue("@FileName", FileName);
                smData.sqlexecute.Parameters.AddWithValue("@NHSTrustID", nhsTrustId);
                smData.sqlexecute.Parameters.AddWithValue("@FinancialExportID", transfer.financialExportID);
                smData.sqlexecute.Parameters.AddWithValue("@Status", status);
                smData.sqlexecute.Parameters.AddWithValue("@ExportHistoryID", transfer.exportHistoryID);

                smData.ExecuteProc("saveInboundFileData");
                smData.sqlexecute.Parameters.Clear();
            }
            Log.Debug("SaveInboundFileComplete");
        }

        /// <summary>
        /// Update the web application's inbound file status 
        /// </summary>
        /// <param name="esr">An instance of <see cref="IESR"/>used for processing on the website.</param>
        public void GetInboundUpdatedStatuses(IESR esr)
        {
            Log.Debug("GetInboundUpdatedStatuses");
            using (var smData = new DatabaseConnection(ConfigurationManager.ConnectionStrings["ESRFileTransfer"].ConnectionString))
            {
                cNHSTrusts clsTrusts = new cNHSTrusts(ConfigurationManager.ConnectionStrings["ESRFileTransfer"].ConnectionString);

                string strSQL = "SELECT DataID, ExportHistoryID, NHSTrustID, Status FROM InboundFileData WHERE Status = @status1 OR Status = @status2";
                smData.sqlexecute.Parameters.AddWithValue("@status1", FinancialExportStatus.UploadToESRSucceeded);
                smData.sqlexecute.Parameters.AddWithValue("@status2", FinancialExportStatus.UploadToESRFailed);
                using (var reader = smData.GetReader(strSQL))
                {
                    while (reader.Read())
                    {
                        var dataId = reader.GetInt32(reader.GetOrdinal("DataID"));
                        var exportHistoryId = reader.GetInt32(reader.GetOrdinal("ExportHistoryID"));
                        var nhsTrustId = reader.GetInt32(reader.GetOrdinal("NHSTrustID"));
                        var status = (FinancialExportStatus)reader.GetByte(reader.GetOrdinal("Status"));

                        var trust = clsTrusts.GetESRTrustByID(nhsTrustId);
                        if (trust != null && trust.Archived == false)
                        {
                            //Web service call to update status. Get accountid from trust for web service method
                            esr.UpdateTransferStatus(trust.AccountID, exportHistoryId, status); 

                            this.UpdateInboundFileStatus(FinancialExportStatus.FileTransferProcessComplete, dataId);

                        }
                    }

                    reader.Close();
                }
                smData.sqlexecute.Parameters.Clear();
            }

            Log.Debug("GetInboundUpdatedStatuses - Complete");
        }

        /// <summary>
        /// Update the status of an Inbound file 
        /// </summary>
        /// <param name="status"></param>
        /// <param name="DataID"></param>
        public void UpdateInboundFileStatus(FinancialExportStatus status, int DataID)
        {
            if (Log.IsDebugEnabled)
            {
                Log.Debug($"UpdateInboundFileStatus - {status}");
            }

            using (var smData = new DatabaseConnection(ConfigurationManager.ConnectionStrings["ESRFileTransfer"].ConnectionString))
            {
                smData.sqlexecute.Parameters.AddWithValue("@DataID", DataID);
                smData.sqlexecute.Parameters.AddWithValue("@Status", status);

                smData.ExecuteProc("dbo.UpdateInboundFileStatus");
                smData.sqlexecute.Parameters.Clear();
            }
        }
    }
}
