using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using SpendManagementLibrary;
using System.Configuration;
using System.Threading;
using SpendManagementLibrary.ESRTransferServiceClasses;
using System.Xml.Serialization;
using System.Web.Script.Services;

namespace Spend_Management
{
    /// <summary>
    /// Summary description for svcAutomatedTransfers
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]
    public class svcAutomatedTransfers : System.Web.Services.WebService
    {
        /// <summary>
        /// Gets a list of Financial Export Histories that have been flagged as ready for transfer
        /// </summary>
        /// <returns>The first index is the AccountID, the nested index is the exportHistoryID</returns>
        [WebMethod(EnableSession = true)]
        public List<cAutomatedTransfer> GetReadyTransfers()
        {
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

            return lstTransfers;
        }

        //public void LogEvent(string text)
        //{
        //    System.IO.TextWriter tw = new System.IO.StreamWriter("C:\\output.log", true);

        //    tw.WriteLine(DateTime.Now.ToString() + " -  " + text);

        //    tw.Close();
        //}

        /// <summary>
        /// Gets a list of Trusts with details updated since a specified date
        /// </summary>
        /// <returns>The first index is the AccountID, the nested index is the TrustID</returns>
        [WebMethod(EnableSession = true)]
        public List<TrustUpdateInfo> GetUpdatedTrusts(DateTime since)
        {
            cESRTrusts clsTrusts;
            List<TrustUpdateInfo> lstAccounts = new List<TrustUpdateInfo>();
            List<cESRTrust> lstTrusts;
            TrustUpdateInfo trustInfo;

            foreach (KeyValuePair<int, cAccount> account in cAccounts.CachedAccounts)
            {
                if (!account.Value.archived && account.Value.IsNHSCustomer)
                {
                    clsTrusts = new cESRTrusts(account.Value.accountid);
                    lstTrusts = new List<cESRTrust>();

                    foreach (KeyValuePair<int, cESRTrust> kvp in clsTrusts.Trusts)
                    {
                        if (kvp.Value.CreatedOn > since || (kvp.Value.ModifiedOn.HasValue && kvp.Value.ModifiedOn > since))
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
            return lstAccounts;
        }

        /// <summary>
        /// Get the specified instance of a financial export report
        /// </summary>
        /// <param name="financialReportID">Financial Export Report</param>
        /// <param name="exportHistoryID">Report History Item</param>
        /// <returns>File byte array</returns>
        [WebMethod(EnableSession = true)]
        public byte[] GetExportData(object request)
        {
            IReports clsreports = (IReports)Activator.GetObject(typeof(IReports), ConfigurationManager.AppSettings["ReportsServicePath"] + "/reports.rem");

            byte[] FileData = (byte[])clsreports.getReportData((cReportRequest)request);

            return FileData;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="accountID"></param>
        /// <param name="financialReportID"></param>
        /// <param name="exportHistoryID"></param>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        public object startInboundExport(int accountID, int financialReportID, int exportHistoryID)
        {
            cFinancialExports clsExports = new cFinancialExports(accountID);
            cReportRequest request = clsExports.getExportHistoryData(exportHistoryID, financialReportID);

            return (object)request;
        }

        [WebMethod(EnableSession = true)]
        public object[] getExportProgress(object request)
        {
            IReports clsreports = (IReports)Activator.GetObject(typeof(IReports), ConfigurationManager.AppSettings["ReportsServicePath"] + "/reports.rem");
            object[] data = clsreports.getReportProgress((cReportRequest)request);
            return data;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="accountID"></param>
        /// <param name="lstOutboundFiles"></param>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        public void UploadOutboundFiles(int accountID, cESRFileInfo outboundFileInfo)
        {
            cImportTemplates clsTemplates = new cImportTemplates(accountID);
            int templateID = clsTemplates.checkExistenceOfESRAutomatedTemplate(outboundFileInfo.TrustID);
            
            if (templateID > 0)
            {
                clsTemplates.processESROutboundImport(templateID, outboundFileInfo.DataID, outboundFileInfo.FileData);
            }
            
            return;
        }

        /// <summary>
        /// Update the status on a given export
        /// </summary>
        /// <param name="accountID">Account ID</param>
        /// <param name="exportHistoryID">Financial Export History item</param>
        /// <param name="status"></param>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        [ScriptMethod]
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
        [WebMethod(EnableSession = true)]
        public DateTime GetUpdateServerUTCDateTime()
        {
            return DateTime.UtcNow;
        }
    }
}
