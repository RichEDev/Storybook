namespace Spend_Management
{
    using System;
    using System.Collections.Generic;
    using SpendManagementLibrary;
    using SpendManagementLibrary.ESRTransferServiceClasses;
    using System.Configuration;
    using BusinessLogic.Cache;
    using Common.Logging;

    public class cESRSvc : MarshalByRefObject, IESR
    {

        /// <summary>
        /// An instance of <see cref="ILog"/> for logging <see cref="ICache{T,TK}"/> diagnostics and information.
        /// </summary>
        private static readonly ILog Log = new LogFactory<cESRSvc>().GetLogger();

        public cESRTrust GetESRTrustByVPD(string trustVPD)
        {
            cESRTrust trust = null;

            foreach (cAccount acc in cAccounts.CachedAccounts.Values)
            {
                if (!acc.archived)
                {
                    cESRTrusts clsESRTrusts = new cESRTrusts(acc.accountid);

                    trust = clsESRTrusts.GetESRTrustByVPD(trustVPD);

                    if (trust != null)
                    {
                        break;
                    }
                }
            }

            return trust;
        }
        public override object InitializeLifetimeService()
        {
            return null;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="accountid"></param>
        /// <param name="isHeader"></param>
        /// <param name="trustID">TrustID</param>
        /// <returns></returns>
        [Obsolete("Taken out as the ESR File name is now attcahed to the export options on a cReportRequest", true)]
        public string getESRInboundFilename(int accountid, int trustID)
        {
            cESRTrusts trustDetails = new cESRTrusts(accountid);
            return trustDetails.CreateESRInboundFilename(trustID);
        }

        public List<cESRElement> getESRElementsBySubcatID(int AccountID, int NHSTrustID, int SubcatID)
        {
            cESRElementMappings clsESRElements = new cESRElementMappings(AccountID, NHSTrustID);
            return clsESRElements.getESRElementsBySubcatID(SubcatID);
        }

        public SortedList<int, cGlobalESRElement> getListOfGlobalElements(int AccountID, int NHSTrustID)
        {
            cESRElementMappings clsESRElements = new cESRElementMappings(AccountID, NHSTrustID);
            return clsESRElements.lstGlobalElements();
        }
        

        public cESRTrust GetESRTrustByID(int AccountID, int TrustID)
        {
            cESRTrusts clsTrusts = new cESRTrusts(AccountID);
            return clsTrusts.GetESRTrustByID(TrustID);
        }

        public int checkExistenceOfESRAutomatedTemplate(int AccountID, int NHSTrustID)
        {
            cImportTemplates clsImportTemps = new cImportTemplates(AccountID);
            return clsImportTemps.checkExistenceOfESRAutomatedTemplate(NHSTrustID);
        }

        public void processESROutboundImport(int AccountID, int TemplateID, byte[] data)
        {
            
        }

        public List<cAutomatedTransfer> GetReadyTransfers()
        {
            Log.Debug("GetReadyTransfers");
            cAutomatedTransfers clsTransfers = new cAutomatedTransfers();
            List<cAutomatedTransfer> lstTransfers = new List<cAutomatedTransfer>(); ;

            if (clsTransfers.Transfers != null)
            {
                foreach (List<cAutomatedTransfer> lstTrans in clsTransfers.Transfers.Values)
                {

                    foreach (cAutomatedTransfer val in lstTrans)
                    {
                        lstTransfers.Add(val);
                    }
                }
            }

            if (Log.IsDebugEnabled)
            {
                Log.Debug($"GetReadyTransfers complete - {lstTransfers.Count} files found.");
            }

            return lstTransfers;
        }

        /// <summary>
        /// Gets a list of Trusts with details updated since a specified date
        /// </summary>
        /// <returns>The first index is the AccountID, the nested index is the TrustID</returns>
        public List<TrustUpdateInfo> GetUpdatedTrusts(DateTime since)
        {
            List<TrustUpdateInfo> lstAccounts = new List<TrustUpdateInfo>();
            try
            {
                cESRTrusts clsTrusts;

                List<cESRTrust> lstTrusts;
                TrustUpdateInfo trustInfo;

                foreach (KeyValuePair<int, cAccount> account in cAccounts.CachedAccounts)
                {
                    try
                    {
                        if (!account.Value.archived && account.Value.IsNHSCustomer)
                        {
                            clsTrusts = new cESRTrusts(account.Value.accountid);
                            lstTrusts = new List<cESRTrust>();

                            foreach (KeyValuePair<int, cESRTrust> kvp in clsTrusts.Trusts)
                            {
                                if (kvp.Value.EsrInterfaceVersionNumber == 1 && kvp.Value.CreatedOn > since || (kvp.Value.ModifiedOn.HasValue && kvp.Value.ModifiedOn > since))
                                {
                                    lstTrusts.Add(kvp.Value);
                                }
                            }
                            trustInfo = new TrustUpdateInfo();
                            trustInfo.AccountID = account.Value.accountid;
                            trustInfo.lstTrusts = lstTrusts;
                            lstAccounts.Add(trustInfo);
                            
                        }
                    }
                    catch (Exception ex)
                    {
                        cEventlog.LogEntry("AccountID: " + account.Value.accountid + " " + ex.Message + " " + ex.StackTrace);
                    }
                }
            }
            catch (Exception ex)
            {
                cEventlog.LogEntry(ex.Message + " " + ex.StackTrace);
            }
            
            return lstAccounts;
        }

        /// <summary>
        /// Get the specified instance of a financial export report
        /// </summary>
        /// <param name="financialReportID">Financial Export Report</param>
        /// <param name="exportHistoryID">Report History Item</param>
        /// <returns>File byte array</returns>
        public byte[] GetExportData(cReportRequest request)
        {
            IReports clsreports = (IReports)Activator.GetObject(typeof(IReports), ConfigurationManager.AppSettings["ReportsServicePath"] + "/reports.rem");

            byte[] FileData = (byte[])clsreports.getReportData(request);

            return FileData;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="accountID"></param>
        /// <param name="financialReportID"></param>
        /// <param name="exportHistoryID"></param>
        /// <returns></returns>
        public cReportRequest startInboundExport(int accountID, int financialReportID, int exportHistoryID)
        {
            cFinancialExports clsExports = new cFinancialExports(accountID);
            cReportRequest request = clsExports.getExportHistoryData(exportHistoryID, financialReportID);

            return request;
        }


        public object[] getExportProgress(cReportRequest request)
        {
            IReports clsreports = (IReports)Activator.GetObject(typeof(IReports), ConfigurationManager.AppSettings["ReportsServicePath"] + "/reports.rem");
            object[] data = clsreports.getReportProgress(request);
            return data;
        }

        /// <summary>
        /// Upload outbound files to the ESR service.
        /// </summary>
        /// <param name="accountId">
        /// The account Id.
        /// </param>
        /// <param name="outboundFileInfo">
        /// </param>
        /// <param name="expTrustId">
        /// </param>
        /// <returns>
        /// </returns>
        public OutboundStatus UploadOutboundFiles(int accountId, cESRFileInfo outboundFileInfo, int expTrustId)
        {
            OutboundStatus status = OutboundStatus.None;

            try
            {
                if (Log.IsInfoEnabled)
                {
                    Log.Info($"Upload Outbound Files - Starting for Trust ID - {outboundFileInfo.TrustID} Filename - {outboundFileInfo.fileName}");
                }
                cESRTrusts trusts = new cESRTrusts(accountId);
                cESRTrust trust = trusts.GetESRTrustByID(expTrustId);
                if (trust.Archived )
                {
                    if (Log.IsInfoEnabled)
                    {
                        Log.Info($"Upload Outbound Files - Failed for Trust ID - {outboundFileInfo.TrustID} Filename - {outboundFileInfo.fileName} - Trust is archived.");
                    }
                    return OutboundStatus.Failed;
                }

                if (trust.EsrInterfaceVersionNumber != Convert.ToByte(1))
                {
                    if (Log.IsInfoEnabled)
                    {
                        Log.Info($"Upload Outbound Files - Failed for Trust ID - {outboundFileInfo.TrustID} Filename - {outboundFileInfo.fileName} - Esr Interface Version Number does not match.");
                    }
                    return OutboundStatus.Failed;
                }


                cImportTemplates clsTemplates = new cImportTemplates(accountId);
                int templateID = clsTemplates.checkExistenceOfESRAutomatedTemplate(expTrustId);

                if (templateID > 0)
                {
                    status = clsTemplates.processESROutboundImport(templateID, outboundFileInfo.DataID, outboundFileInfo.FileData);
                }
                else
                {
                    if (Log.IsInfoEnabled)
                    {
                        Log.Info($"Upload Outbound Files - Failed for Trust ID - {outboundFileInfo.TrustID} Filename - {outboundFileInfo.fileName} - Template ID {templateID} was not found.");
                    }
                }
            }
            catch(Exception ex)
            {
                Log.Warn($"Upload Outbound Files failed - {ex.Message} - {ex.StackTrace}");
                status = OutboundStatus.Failed;
                ErrorHandlerWeb clsErrors = new ErrorHandlerWeb();
                clsErrors.SendAutomatedProcessError(ex);
            }

            return status;
        }

        /// <summary>
        /// Update the status on a given export
        /// </summary>
        /// <param name="accountID">Account ID</param>
        /// <param name="exportHistoryID">Financial Export History item</param>
        /// <param name="status"></param>
        /// <returns></returns>
        public int UpdateTransferStatus(int accountID, int exportHistoryID, FinancialExportStatus status)
        {
            cFinancialExports clsFinancialExports = new cFinancialExports(accountID);
            int ret = clsFinancialExports.UpdateExportHistoryStatus(exportHistoryID, status);
            return ret;
        }

        /// <summary>
        /// Gets the UTC time on this server for sync
        /// </summary>
        /// <returns>UTC DateTime</returns>
        public DateTime GetUpdateServerUTCDateTime()
        {
            return DateTime.UtcNow;
        }
    }
}
