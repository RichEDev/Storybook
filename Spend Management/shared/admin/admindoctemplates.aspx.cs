using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Script;
using System.Web.Script.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using SpendManagementLibrary;
using Spend_Management;

namespace Spend_Management
{
    using Spend_Management.shared.code;

    public partial class admindoctemplates : System.Web.UI.Page
    {
        /// <summary>
        /// Panel where document template grid is displayed
        /// </summary>
        public Panel currentDocsPanel;
        
        /// <summary>
        /// Page Load event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();
            currentUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.DocumentTemplates, true, true);

            if (!this.IsPostBack)
            {
                RedirectIfNoMergeProjects(currentUser);
                Title = "Document Templates";
                Master.PageSubTitle = Title;

                lnkNew.Visible = currentUser.CheckAccessRole(AccessRoleType.Add, SpendManagementElement.DocumentTemplates, true);
                getCurrentDocs();

                if (Request.QueryString["ret"] != null)
                {
                    string msg = (string)Request.QueryString["ret"];
                    divMessage.InnerHtml = "<div id='divMessage' runat='server' class=\"errorModalSubject\">" + "Status Message" + "</div><br /><div class=\"errorModalBody\">" + msg + "</div>";
                    mdlMessage.Show();
                }
            }
        }

        /// <summary>
        /// New link clicked under page options
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lnkNew_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/shared/admin/aedocumenttemplate.aspx", true);
        }

        /// <summary>
        /// Populates the document template list to the screen
        /// </summary>
        private void getCurrentDocs()
        {
            var documentMerge = new svcDocumentTemplates();
            var gridData = documentMerge.GetDocumentTemplateGrid();
            litDocTemplates.Text = gridData[1];
            Page.ClientScript.RegisterStartupScript(this.GetType(), "SignOffGroupsVars", cGridNew.generateJS_init("SignOffGroupsVars", new List<string>() { gridData[0] }, cMisc.GetCurrentUser().CurrentActiveModule), true);
            return;
        }

        /// <summary>
        /// Close button event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnClose_Click(object sender, ImageClickEventArgs e)
        {
            Response.Redirect((SiteMap.CurrentNode == null) ? "~/home.aspx" : SiteMap.CurrentNode.ParentNode.Url, true);
        }

        /// <summary>
        /// Redirect method if no merge configurations exist
        /// </summary>
        /// <param name="user"></param>
        private void RedirectIfNoMergeProjects(CurrentUser user)
        {
            var dmps = new DocumentMergeProject(user);
            if (dmps.Projects.Count == 0)
                Response.Redirect("~/shared/admin/admindocmergeprojects.aspx?nomerge=true");
        }
    }
}
