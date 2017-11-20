using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Script.Services;
using System.Text;
using SpendManagementLibrary;
using System.Configuration;
using SpendManagementLibrary.ESRTransferServiceClasses;
using System.Collections;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Http;
using System.Web.UI.WebControls;

namespace Spend_Management
{
    /// <summary>
    /// Summary description for svcLogging
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]
    public class svcLogging : System.Web.Services.WebService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="logID"></param>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public string getLogData(int logID, int nLogViewType, int nLogElementType)
        {
            CurrentUser user = cMisc.GetCurrentUser();

            cLogging clsLogging = new cLogging(user.AccountID);
            
            LogViewType eLogViewType = (LogViewType)nLogViewType;
            cElements clsElements = new cElements();
            cElement element = clsElements.GetElementByID(nLogElementType);

            string logInfo = clsLogging.generateLogInfo(logID, eLogViewType, element);

            return logInfo;
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public ListItem[] GetLogElementOptions(int logID)
        {
            CurrentUser user = cMisc.GetCurrentUser();

            cLogging clsLogging = new cLogging(user.AccountID);

            List<ListItem> logElementOptions = clsLogging.GenerateLogElementOptions(logID);

            return logElementOptions.ToArray();
        }

        /// <summary>
        /// getHistoryGrid: returns a grid containing import history
        /// </summary>
        /// <param name="accountid">Customer Account Id</param>
        /// <param name="employeeid">Current Employee Id</param>
        /// <param name="filterType">Filter history by (-1 displays all)</param>
        /// <returns>HTML string</returns>
        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public string[] getHistoryGrid(int filterType)
        {
            CurrentUser user = cMisc.GetCurrentUser();
            cImportHistory ihistory = new cImportHistory(user.AccountID);

            cGridNew grid = new cGridNew(user.AccountID, user.EmployeeID, "historygrid", ihistory.getHistoryGridSQL);
            grid.SortedColumn = grid.getColumnByName("importedDate");
            grid.SortDirection = SpendManagementLibrary.SortDirection.Descending;
            grid.KeyField = "historyId";
            grid.EmptyText = "There are currently no import history records to display";
            grid.enabledeleting = user.CheckAccessRole(AccessRoleType.Delete, SpendManagementElement.ImportHistory, true);

            grid.addEventColumn("viewlog", "/shared/images/icons/16/plain/zoom_in.png", "javascript:showLogModal({logId}, {applicationType});", "View Log", "View the import log for file");
            grid.addTwoStateEventColumn("download", (cFieldColumn)grid.getColumnByName("applicationtype"), null, 0, "", "", "", "", "/shared/images/icons/16/plain/export1.png", "javascript:window.open('/shared/getDocument.axd?docType=" + (byte)DocumentType.ESROutbound + "&dataid={dataId}');", "Download File", "Download File", false);
            grid.addTwoStateEventColumn("rerunimport", (cFieldColumn)grid.getColumnByName("applicationtype"), null, 1, "/shared/images/icons/16/plain/replace2.png", "javascript:rerunImport({dataId}, {applicationType});", "Re-Run", "Re-import file", "", "", "", "", false);

            grid.deletelink = "javascript:deleteImportHistory({historyId});";
            grid.getColumnByName("historyId").hidden = true;
            grid.getColumnByName("logId").hidden = true;
            grid.getColumnByName("importId").hidden = true;
            grid.getColumnByName("dataId").hidden = true;

            ((cFieldColumn)grid.getColumnByName("importStatus")).addValueListItem((int)ImportHistoryStatus.Success, "Success");
            ((cFieldColumn)grid.getColumnByName("importStatus")).addValueListItem((int)ImportHistoryStatus.Failure, "Failure");
            ((cFieldColumn)grid.getColumnByName("importStatus")).addValueListItem((int)ImportHistoryStatus.Success_With_Errors, "Success with Errors");
            ((cFieldColumn)grid.getColumnByName("importStatus")).addValueListItem((int)ImportHistoryStatus.Success_With_Warnings, "Success with Warnings");
            ((cFieldColumn)grid.getColumnByName("importStatus")).addValueListItem((int)ImportHistoryStatus.InProgress, "Import in Progress");

            ((cFieldColumn)grid.getColumnByName("applicationType")).addValueListItem((int)ApplicationType.ESROutboundImport, "ESR Outbound V1.0");
            ((cFieldColumn)grid.getColumnByName("applicationType")).addValueListItem((int)ApplicationType.ExcelImport, "Excel Import");
            ((cFieldColumn)grid.getColumnByName("applicationType")).addValueListItem((int)ApplicationType.EsrOutboundImportV2, "ESR Outbound V2.0");

            if (filterType > -1)
            {
                cFields clsFields = new cFields(user.AccountID);
                cField applicationType = clsFields.GetFieldByID(new Guid("B0829007-6AAD-47C2-A585-12C5CA8B5FD3"));

                grid.addFilter(applicationType, ConditionType.Equals, new object[] { filterType }, null, ConditionJoiner.None);
            }

            List<string> retVals = new List<string>();
            retVals.Add(grid.GridID);
            retVals.AddRange(grid.generateGrid());
            return retVals.ToArray();
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public void rerunImport(int DataID)
        {
            CurrentUser user = cMisc.GetCurrentUser();

            BinaryClientFormatterSinkProvider clientProvider = new BinaryClientFormatterSinkProvider();
            //BinaryServerFormatterSinkProvider serverProvider = new BinaryServerFormatterSinkProvider();
            //serverProvider.TypeFilterLevel = System.Runtime.Serialization.Formatters.TypeFilterLevel.Full;

            int chanCount = 0;
            IDictionary props = new Hashtable();
            props["port"] = 0;
            props["typeFilterLevel"] = System.Runtime.Serialization.Formatters.TypeFilterLevel.Full;
            HttpChannel chan = new HttpChannel(props, clientProvider, null);

            foreach (IChannel c in ChannelServices.RegisteredChannels)
            {
                if (c.ChannelName == chan.ChannelName)
                {
                    chanCount++;
                }
            }

            if (chanCount == 0)
            {
                ChannelServices.RegisterChannel(chan, false);
            }


            IESRTransfer clsESR = (IESRTransfer)Activator.GetObject(typeof(IESRTransfer), ConfigurationManager.AppSettings["ESRTransferServicePath"] + "/ESRTransfer.rem");

            cESRFileInfo[] lstData = clsESR.getOutboundData(DataID);
            byte[] data = lstData[0].FileData;
            cImportTemplates clsImportTemps = new cImportTemplates(user.AccountID);
            int TemplateID = clsImportTemps.checkExistenceOfESRAutomatedTemplate(lstData[0].TrustID);

            if (TemplateID > 0)
            {
                clsImportTemps.processESROutboundImport(TemplateID, DataID, data);
            }
        }

        /// <summary>
        /// Resubmit a file via the ESR hub.
        /// </summary>
        /// <param name="fileId">
        /// The file id.
        /// </param>
        /// <param name="fileName">
        /// The file name.
        /// </param>
        /// <returns>
        /// The <see cref="ApiResult"/>.
        /// </returns>
        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public string ResubmitFile(int fileId, string fileName)
        {
            string retMessage = string.Empty;

            try
            {
                var user = cMisc.GetCurrentUser();
                var logging = new cLogging(user.AccountID);
                fileName = logging.GetDataItemFileName(fileId);
            
                string vpd;
                int currentOutboundSequence;
                char typeCode;
                ParseOutboundFilename(fileName, out typeCode, out currentOutboundSequence, out vpd);
            
                cESRTrusts trusts = new cESRTrusts(user.AccountID);
                cESRTrust tmpTrust = trusts.GetESRTrustByVPD(vpd);

                if (tmpTrust.CurrentOutboundSequenceNumber != currentOutboundSequence)
                {
                    return "File not submitted for reprocessing. File is not the last sequence number processed. Contact your administrator to permit processing of this file.";
                }

                var esrClient = new EsrNhsHub.EsrNhsHubCommandsClient("EsrRouterToHubEndpoint");
                var result = esrClient.FlagFileForNewTransfer(fileId, fileName);

                retMessage = result ? "File submitted for processing." : "File cannot be re-submitted.";
            }
            catch (Exception ex)
            {
                retMessage = string.Format("An error occurred while requesting file reprocess.\n{0}", ex.Message);
            }

            return retMessage;
        }

        /// <summary>
        /// Extracts the sequence number and the extract type (Full or Change) from the filename
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="typeCode"></param>
        /// <param name="fileSequenceId"></param>
        /// <param name="trustVpd"></param>
        private void ParseOutboundFilename(string fileName, out char typeCode, out int fileSequenceId, out string trustVpd)
        {
            // GO_VPD_EXP_GOF_20110826_00001234.DAT
            // Where:
            // GO		Denotes that the file is a Generic Outbound file
            // VPD		Trust Virtual Private Database number
            // EXP		Sub-file type (e.g. EXP - Expenses)
            // GO 		GO Outbound format/version (or if you like sub-file type)
            // F/C 		Extract Type Code (F – Full, C – Changes)
            // 20110826 File Extract Date (e.g. YYYYMMDD)
            // 00001234	Unique ID (usually a sequentially incremented number)
            // DAT 		Constant ‘DAT’ extension denoting data file
            fileName = fileName.Replace(".DAT", string.Empty);
            string[] filenameParts = fileName.Split('_');

            trustVpd = filenameParts[1];
            typeCode = Convert.ToChar(filenameParts[3].Substring(2, 1));

            int.TryParse(filenameParts[5], out fileSequenceId);
        }
 
        /// <summary>
        /// deleteImportHistory
        /// </summary>
        /// <param name="historyid">ID of the history item to delete</param>
        /// <returns>ID of the history item deleted so can remove row from grid</returns>
        [WebMethod()]
        public int deleteImportHistory(int historyid)
        {
            CurrentUser user = cMisc.GetCurrentUser();
            cImportHistory ihistory = new cImportHistory(user.AccountID);

            ihistory.deleteHistory(historyid);

            return historyid;
        }
    }
}
