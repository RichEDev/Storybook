using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Services;
using System.Text;

namespace Spend_Management
{
    /// <summary>
    /// adminAudiences page
    /// </summary>
    public partial class adminAudiences : System.Web.UI.Page
    {
        /// <summary>
        /// Page Load event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack == false)
            {
                Title = "Audiences";
                Master.title = Title;

                CurrentUser currentUser = cMisc.GetCurrentUser();
                currentUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.Audiences, true, true);

                svcAudiences service = new svcAudiences();
                
                // audience element not yet mapped to access roles
                //currentUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.Audiences, true, true);
                string[] gridData = service.CreateAudiencesGrid();
                litGrid.Text = gridData[1];

                Page.ClientScript.RegisterStartupScript(this.GetType(), "AudienceGridVars", cGridNew.generateJS_init("AudienceGridVars", new List<string>() { gridData[0] }, currentUser.CurrentActiveModule), true);
            }
        }

        /// <summary>
        /// Close button event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnClose_Click(object sender, ImageClickEventArgs e)
        {
            string sPreviousURL = (SiteMap.CurrentNode == null) ? "~/home.aspx" : SiteMap.CurrentNode.ParentNode.Url;

            Response.Redirect(sPreviousURL, true);
        }
    }
}
