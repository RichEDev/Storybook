namespace Spend_Management.shared.admin
{
    using System;
    using System.Web;
    using System.Web.UI;

    /// <summary>
    /// Level based matrix summary
    /// </summary>
    public partial class ApprovalMatrices : Page
    {
        /// <summary>
        /// The page_ load event.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void Page_Load(object sender, EventArgs e)
        {
            this.Master.Page.Title = "Approval Matrices";
            this.Master.PageSubTitle = "Approval Matrices";
            this.Master.UseDynamicCSS = true;

            var user = cMisc.GetCurrentUser();

            if (user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.ApprovalMatrix, true) == false)
            {
                return;
            }

            lnkNewMatrix.Visible = user.CheckAccessRole(AccessRoleType.Add, SpendManagementElement.ApprovalMatrix, true);

            string[] gridData = new code.ApprovalMatrix.ApprovalMatrices(user.AccountID).GetMatrixGrid();
            this.litgrid.Text = gridData[1];


            // set the sel.grid javascript variables
            Page.ClientScript.RegisterStartupScript(this.GetType(), "vars", cGridNew.generateJS_init("vars", new System.Collections.Generic.List<string>() { gridData[0] }, user.CurrentActiveModule), true);
        }

        /// <summary>
        /// The command button close_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The event.
        /// </param>
        protected void btnCloseClick(object sender, EventArgs e)
        {
            string previousUrl = (SiteMap.CurrentNode == null) ? "~/home.aspx" : SiteMap.CurrentNode.ParentNode.Url;

            Response.Redirect(previousUrl, true);
        }
    }
}