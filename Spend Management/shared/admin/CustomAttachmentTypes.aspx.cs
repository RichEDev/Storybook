using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;

namespace Spend_Management
{
    public partial class CustomAttachmentTypes : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();
            currentUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.CustomMimeHeaders, false, true);

            if (!currentUser.CheckAccessRole(AccessRoleType.Add, SpendManagementElement.CustomMimeHeaders, true))
            {
                lnkAddCustAttachmentType.Visible = false;
            }

            Title = "Custom Attachment Types";
            Master.title = Title;

            switch (currentUser.CurrentActiveModule)
            {
                case Modules.contracts:
                    Master.helpid = 1026;
                    break;
                default:
                    Master.helpid = 0;
                    break;
            }

            cGlobalMimeTypes clsGlobalMimeTypes = new cGlobalMimeTypes(currentUser.AccountID);
            string[] gridData = clsGlobalMimeTypes.GenerateCustomAttachmentGrid(currentUser);
            litCustGrid.Text = gridData[2];

            // set the sel.grid javascript variables
            Page.ClientScript.RegisterStartupScript(this.GetType(), "CustAttTypesGridVars", cGridNew.generateJS_init("CustAttTypesGridVars", new List<string>() { gridData[1] }, currentUser.CurrentActiveModule), true);
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
