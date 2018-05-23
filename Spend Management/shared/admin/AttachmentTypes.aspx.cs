using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;

namespace Spend_Management
{
    using BusinessLogic.Modules;

    public partial class AttachmentTypes : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();
            currentUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.AttachmentMimeTypes, true, true);

            if(!currentUser.CheckAccessRole(AccessRoleType.Add, SpendManagementElement.AttachmentMimeTypes, true))
            {
                lnkAddAttachmentType.Visible = false;
            }

            Title = "Attachment Types";
            Master.title = Title;

            switch (currentUser.CurrentActiveModule)
            {
                case Modules.Contracts:
                    Master.helpid = 1026;
                    break;
                default:
                    Master.helpid = 0;
                    break;
            }

            svcAttachmentTypes attachTypes = new svcAttachmentTypes();

            string[] gridData = attachTypes.CreateAttachmentTypeGrid();
            litGrid.Text = gridData[1];

            // set the sel.grid javascript variables
            Page.ClientScript.RegisterStartupScript(this.GetType(), "AttTypesGridVars", cGridNew.generateJS_init("AttTypesGridVars", new List<string>() { gridData[0] }, currentUser.CurrentActiveModule), true);
        }

        /// <summary>
        /// Close button event function
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void cmdClose_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            string sPreviousURL = (SiteMap.CurrentNode == null) ? "~/home.aspx" : SiteMap.CurrentNode.ParentNode.Url;

            Response.Redirect(sPreviousURL, true);
        }

    }
}
