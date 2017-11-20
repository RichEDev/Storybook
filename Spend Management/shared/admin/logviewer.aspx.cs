using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using SpendManagementLibrary;

namespace Spend_Management
{
    public partial class logviewer : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            CurrentUser user = cMisc.GetCurrentUser();

            cLogging clsLogging = new cLogging(user.AccountID);
            int logID = 0;
            LogViewType logView = LogViewType.All;

            cElements clsElements = new cElements();
            cElement element = null;

            if (Request.QueryString["logid"] != null)
            {
                logID = int.Parse(Request.QueryString["logid"]);
            }
            if (Request.QueryString["viewtype"] != null)
            {
                logView = (LogViewType)int.Parse(Request.QueryString["viewtype"]);
            }
            if (Request.QueryString["elementtype"] != null)
            {
                element = clsElements.GetElementByID(int.Parse(Request.QueryString["elementtype"]));
            }
            


            string logInfo = clsLogging.generateLogInfo(logID, logView, element);

            logDiv.InnerHtml = logInfo;
        }
    }
}
