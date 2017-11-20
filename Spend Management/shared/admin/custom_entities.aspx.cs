using System;
using System.Web;

namespace Spend_Management
{
    /// <summary>
    /// Custom Entity Administration Page
    /// </summary>
    public partial class custom_entities : System.Web.UI.Page
    {
        /// <summary>
        /// Page_Load event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            Title = "GreenLights";
            Master.PageSubTitle = "GreenLights";

            Master.UseDynamicCSS = true;

            // Custom Entity Administration is only currently available to 'adminonly' employees
            CurrentUser user = cMisc.GetCurrentUser();
            if (user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.CustomEntities, true) == false)
            {
                Response.Redirect(
                    "~/shared/restricted.aspx?reason=Current%20access%20role%20does%20not%20permit%20you%20to%20view%20this%20page.",
                    true);
            }

            if(IsPostBack == false)
            {
                divNewCustomEntity.Visible = user.CheckAccessRole(AccessRoleType.Add, SpendManagementElement.CustomEntities, true, false);

                string[] gridData = cCustomEntities.createEntityGrid();
                litgrid.Text = gridData[1];

                // set the sel.grid javascript variables
                Page.ClientScript.RegisterStartupScript(GetType(), "CEAdminVars", cGridNew.generateJS_init("CEAdminVars", new System.Collections.Generic.List<string>() {gridData[0]}, user.CurrentActiveModule), true);
            }
        }

        /// <summary>
        /// Close button event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnClose_Click(object sender, EventArgs e)
        {
            string sPreviousURL = (SiteMap.CurrentNode == null) ? "~/home.aspx" : SiteMap.CurrentNode.ParentNode.Url;

            Response.Redirect(sPreviousURL, true);
        }
    }
}
