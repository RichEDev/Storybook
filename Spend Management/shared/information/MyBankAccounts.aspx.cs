namespace Spend_Management.shared.information
{
    using System;
    using System.Web;

    /// <summary>
    /// Class for details of user's bank accounts
    /// </summary>
    public partial class MyBankAccounts : System.Web.UI.Page
    {
        /// <summary>
        /// Main page load method
        /// </summary>
        /// <param name="sender">object</param>
        /// <param name="e">EventArgs</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            Title = @"My Bank Accounts";
            Master.PageSubTitle = "My Bank Accounts";
            Master.UseDynamicCSS = true;

            if (!this.IsPostBack)
            {
                // check access role permits page access
                var user = cMisc.GetCurrentUser();

                // ensure it uses current user
                usrBankAccounts.AccountsEmployeeId = -1;
                usrBankAccounts.RedactGridData = false;
                usrBankAccounts.AllowEdit = user.CheckAccessRole(AccessRoleType.Edit, SpendManagementElement.BankAccounts, true);
                usrBankAccounts.AllowDelete = user.CheckAccessRole(AccessRoleType.Delete, SpendManagementElement.BankAccounts, true);
                lblAddNew.Visible = user.CheckAccessRole(AccessRoleType.Add, SpendManagementElement.BankAccounts, true);
                if (user.isDelegate || !user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.BankAccounts, true))   
                {
                    Response.Redirect("~/shared/restricted.aspx?reason=Current%20access%20role%20does%20not%20permit%20you%20to%20view%20this%20page.", true);
                }
            }
        }

        /// <summary>
        /// Handles the click of the Close button and navigates to be parent breadcrumb in the web sitemap
        /// </summary>
        /// <param name="sender">object</param>
        /// <param name="e">Eventargs</param>
        protected void btnClose_Click(object sender, EventArgs e)
        {
            Response.Redirect((SiteMap.CurrentNode == null) ? "~/home.aspx" : SiteMap.CurrentNode.ParentNode.Url, true);
        }
    }
}