using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Script.Services;
using System.Text;

namespace Spend_Management
{
    /// <summary>
    /// Summary description for importSvc
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [ScriptService]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class importSvc : System.Web.Services.WebService
    {

        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public string HelloWorld()
        {
            return "Hello World";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public object[] getImportProgress()
        {
            CurrentUser user = cMisc.GetCurrentUser();
            cImportInfo importInfo = (cImportInfo)HttpRuntime.Cache["SpreadsheetImport" + user.AccountID];
            
            //(cImportInfo)Session["SpreadsheetImport" + user.AccountID];

            if (importInfo != null)
            {
                SortedList<int, ImportStatusValues> lstSheetStatus = importInfo.worksheets;

                object[] statusInfo = new object[lstSheetStatus.Count];
                object[] statusData;

                int i = 0;


                foreach (KeyValuePair<int, ImportStatusValues> kp in lstSheetStatus)
                {
                    statusData = new object[8];
                    statusData[0] = importInfo.currentSheet;
                    statusData[1] = importInfo.currentSheetName;
                    statusData[2] = importInfo.importStatus.ToString();
                    statusData[3] = kp.Key;
                    statusData[4] = kp.Value.numOfRows;
                    statusData[5] = kp.Value.processedRows;
                    statusData[6] = kp.Value.sheetName;
                    statusData[7] = kp.Value.status.ToString();

                    statusInfo[i] = statusData;
                    i++;
                }

                return statusInfo;
            }

            return null;
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public void ClearImportFromSession()
        {
            CurrentUser user = cMisc.GetCurrentUser();

            HttpRuntime.Cache.Remove("SpreadsheetImport" + user.AccountID);
        }

        
    }
}
