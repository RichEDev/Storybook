using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Spend_Management;
using SpendManagementLibrary;

namespace SpendManagementUnitTests.Global_Objects
{
    public class cImportHistoryObject
    {
        /// <summary>
        /// Create a global import history variable in the database
        /// </summary>
        /// <returns></returns>
        public static cImportHistoryItem CreateImportHistory()
        {
            cImportHistory clsHistory = new cImportHistory(cGlobalVariables.AccountID);
            cLogging clsLogging = new cLogging(cGlobalVariables.AccountID);
            cImportTemplate template = cImportTemplateObject.CreateImportTemplate();
            int logID = clsLogging.saveLog(0, LogType.ESROutboundImport, 4, 4);
            int tempImportHistoryID = clsHistory.saveHistory(new cImportHistoryItem(0, template.TemplateID, logID, DateTime.Now, ImportHistoryStatus.Success, ApplicationType.ESROutboundImport, 0, DateTime.UtcNow, null));
            cGlobalVariables.HistoryID = tempImportHistoryID;
            cImportHistoryItem importHistory = clsHistory.getHistoryById(tempImportHistoryID);
            return importHistory;
        }

        /// <summary>
        /// Delete the import history from the database
        /// </summary>
        public static void DeleteImportHistory()
        {
            cImportHistory clsHistory = new cImportHistory(cGlobalVariables.AccountID);
            cLogging clsLogging = new cLogging(cGlobalVariables.AccountID);
            cImportHistoryItem item = clsHistory.getHistoryById(cGlobalVariables.HistoryID);

            if (item != null)
            {
                clsLogging.deleteLog(item.LogId);
            }

            cImportTemplateObject.DeleteImportTemplate();
            clsHistory.deleteHistory(cGlobalVariables.HistoryID);
        }
    }
}
