namespace SELFileTransferService
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using SpendManagementLibrary;
    using SpendManagementLibrary.ESRTransferServiceClasses;
    using SpendManagementLibrary.Helpers;
    using Common.Logging;

    public class cOutboundTransfers
    {
        private readonly Remoting _remoting;
        private static bool bOutboundImportInProgress;

        /// <summary>
        /// An instance of <see cref="ILog"/> for logging <see cref="ICache{T,TK}"/> diagnostics and information.
        /// </summary>
        private static readonly ILog Log = new LogFactory<cOutboundTransfers>().GetLogger();

        #region Properties

        /// <summary>
        /// Flag to say whether an Outbound import is in progress
        /// </summary>
        public static bool OutboundImportInProgress
        {
            get { return bOutboundImportInProgress; }
            set { bOutboundImportInProgress = value; }
        }

        #endregion

        public cOutboundTransfers(Remoting remoting)
        {
            this._remoting = remoting;
        }

        /// <summary>
        /// Process any new outbound import files into the Web Application
        /// </summary>
        public void ProcessOutboundFiles(IESR esr, SELFileTransferService clsService)
        {
            if (!OutboundImportInProgress)
            {
                Log.Debug("Process Outbound Files starting");
                try
                {
                    OutboundImportInProgress = true;
                    cNHSTrusts clsTrusts = new cNHSTrusts(ConfigurationManager.ConnectionStrings["ESRFileTransfer"].ConnectionString);
                    cESRTransferLogging clsTransferLogging = new cESRTransferLogging(ConfigurationManager.ConnectionStrings["ESRFileTransfer"].ConnectionString);
                    List<cESRFileInfo> lstData = this.getOutboundFiles(0);
                    cESRTrust trust = null;

                    if (lstData.Count > 0)
                    {
                        foreach (cESRFileInfo val in lstData)
                        {
                            try
                            {
                                trust = clsTrusts.GetESRTrustByID(val.TrustID);
                                if (trust != null)
                                {
                                    if (Log.IsDebugEnabled)
                                    {
                                        Log.Debug($"GetESRTrustByID for ID '{val.TrustID}' returned {trust.TrustName}");
                                    }
                                    //Log the start of the import
                                    clsTransferLogging.SaveLogItem(new cESRTransferLogItem(0, TransferLogItemType.OutboundFileImportStarted, AutomatedTransferType.ESROutbound, trust.TrustID, val.fileName, false, "", null, null));
                                    
                                    //Upload outbound file to expenses2009
                                    var status = esr.UploadOutboundFiles(trust.AccountID, val, trust.expTrustID); 

                                    switch (status)
                                    {
                                        case  OutboundStatus.ValidationFailed:
                                            clsTransferLogging.SaveLogItem(new cESRTransferLogItem(0, TransferLogItemType.OutboundFileValidationFailed, AutomatedTransferType.ESROutbound, trust.TrustID, val.fileName, false, "", null, null));
                                            break;
                                        case OutboundStatus.Complete:
                                            clsTransferLogging.SaveLogItem(new cESRTransferLogItem(0, TransferLogItemType.OutboundFileImportedSuccessfully, AutomatedTransferType.ESROutbound, trust.TrustID, val.fileName, false, "", null, null));
                                            break;
                                        case OutboundStatus.Failed:
                                            clsTransferLogging.SaveLogItem(new cESRTransferLogItem(0, TransferLogItemType.OutboundFileImportFailed, AutomatedTransferType.ESROutbound, trust.TrustID, val.fileName, false, "", null, null));
                                            break;
                                        default:
                                            break;
                                    }
                                    
                                    this.UpdateOutboundFileStatus(val.DataID, status);
                                }
                                else
                                {
                                    if (Log.IsDebugEnabled)
                                    {
                                        Log.Debug($"GetESRTrustByID for ID '{val.TrustID}' returned an empty object.");
                                    }
                                }
                            }
                            catch(Exception ex)
                            {
                                //Log the failure of the import
                                clsTransferLogging.SaveLogItem(new cESRTransferLogItem(0, TransferLogItemType.OutboundFileImportFailed, AutomatedTransferType.ESROutbound, trust == null ? 0 : trust.TrustID, val.fileName, false, "", null, null));
                                this.UpdateOutboundFileStatus(val.DataID, OutboundStatus.Failed);

                                //Send the error email
                                clsService.SendErrorEmail(ex);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    //Catch required because of the finally setting the outbound flag back to false
                    clsService.SendErrorEmail(e);
                }
                finally
                {
                    OutboundImportInProgress = false;
                    Log.Debug("Process Outbound Files - Complete.");
                }

            }
        }

        /// <summary>
        /// Get any outbound files that are awaiting to be imported into the Web application or require to be rerun
        /// </summary>
        /// <param name="DataID">The ID of the outbound data file</param>
        /// <returns></returns>
        public List<cESRFileInfo> getOutboundFiles(int DataID)
        {
            List<cESRFileInfo> lstData = new List<cESRFileInfo>();

            try
            {
                Log.Debug("getOutboundFiles - Starting");
                byte[] FileData;
                int TrustID;
                string fileName;

                DBConnection smData =
                    new DBConnection(ConfigurationManager.ConnectionStrings["ESRFileTransfer"].ConnectionString);
                string strSQL;

                if (DataID > 0)
                {
                    // If a DataID is greater than 0 then it is a rerun request from the expenses application
                    strSQL =
                        "SELECT DataID, NHSTrustID, FileData, FileName FROM OutboundFileData WHERE DataID = @DataID";
                    smData.sqlexecute.Parameters.AddWithValue("@DataID", DataID);
                }
                else
                {
                    // This is a first time upload where the status is changed to awaiting Import
                    strSQL =
                        "SELECT DataID, NHSTrustID, FileData, FileName FROM OutboundFileData WHERE status = @status";
                    smData.sqlexecute.Parameters.AddWithValue("@status", (byte) OutboundStatus.AwaitingImport);
                }

                using (var reader = smData.GetReader(strSQL))
                {
                    smData.sqlexecute.Parameters.Clear();

                    while (reader.Read())
                    {
                        DataID = reader.GetInt32(reader.GetOrdinal("DataID"));
                        TrustID = reader.GetInt32(reader.GetOrdinal("NHSTrustID"));
                        if (reader.IsDBNull(reader.GetOrdinal("FileData")))
                        {
                            FileData = new byte[0];
                        }
                        else
                        {
                            FileData = (byte[]) reader.GetSqlBinary(reader.GetOrdinal("FileData"));
                        }
                        fileName = reader.GetString(reader.GetOrdinal("FileName"));

                        if (FileData.Length > 0)
                        {
                            lstData.Add(new cESRFileInfo(DataID, TrustID, fileName, FileData));
                        }
                    }

                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                Log.Warn($"getOutboundFiles has failed - {ex.Message} - {ex.StackTrace}");
                // This is required as this method is directly accessed from the web application and wont be in the scope of the Try Catch block
                // on the service timer
                SELFileTransferService clsService = new SELFileTransferService();
                clsService.SendErrorEmail(ex);
            }

            return lstData;
        }

        /// <summary>
        /// Update the status of an Outbound import
        /// </summary>
        /// <param name="dataID"></param>
        /// <param name="status"></param>
        public void UpdateOutboundFileStatus(int dataID, OutboundStatus status)
        {
            if (Log.IsDebugEnabled)
            {
                Log.Debug($"UpdateOutboundFileStatus to {status} for data  ID {dataID}");
            }
            using (var smData = new DatabaseConnection(ConfigurationManager.ConnectionStrings["ESRFileTransfer"].ConnectionString))
            {
                smData.sqlexecute.Parameters.AddWithValue("@DataID", dataID);
                smData.sqlexecute.Parameters.AddWithValue("@Status", status);

                smData.ExecuteProc("dbo.UpdateOutboundFileStatus");
                smData.sqlexecute.Parameters.Clear();
            }
        }
    }
}
