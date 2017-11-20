using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Xml.Linq;

namespace Spend_Management
{
    /// <summary>
    /// Completes actions from the workflow
    /// </summary>
    public partial class workflowsActionsPage : System.Web.UI.Page
    {
        /// <summary>
        /// Page_Load
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            int? nWorkflowID = null;
            int? nEntityID = null;
            CurrentUser currentUser = cMisc.GetCurrentUser();
            Title = "Workflow Actions";
            Master.PageSubTitle = Title;

            if (Request.QueryString["workflowID"] != null && Request.QueryString["workflowID"] != "")
            {
                nWorkflowID = Convert.ToInt32(Request.QueryString["workflowID"]);
            }

            if (Request.QueryString["entityID"] != null && Request.QueryString["entityID"] != "")
            {
                nEntityID = Convert.ToInt32(Request.QueryString["entityID"]);
            }

            if (nEntityID.HasValue && nWorkflowID.HasValue)
            {
                wfUC.EntityID = nEntityID;
                wfUC.WorkflowID = nWorkflowID;
                cWorkflows clsWorkflows = new cWorkflows(currentUser);
            }
        }
    }
}
