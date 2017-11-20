namespace Spend_Management.shared.admin
{
    using System;
    using System.Web;
    using System.Globalization;

    using SpendManagementLibrary;

    /// <summary>
    /// The Session Timeout Manager web page class
    /// </summary>
    public partial class SessionTimeoutManager : System.Web.UI.Page
    {
        /// <summary>
        /// The page load event.
        /// </summary>
        /// <param name="sender">The sender object</param>
        /// <param name="e">The event arguments</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            Master.enablenavigation = false;
            Master.UseDynamicCSS = true;
            Master.title = "Session Timout Manager";
            if (this.IsPostBack != false)
            {
                return;
            }

            CurrentUser user = cMisc.GetCurrentUser();

            if (user.CheckAccessRole(AccessRoleType.Edit, SpendManagementElement.SessionTimeoutManager, true)
                == false)
            {
                this.Response.Redirect(
                    "~/shared/restricted.aspx?reason=Current%20access%20role%20does%20not%20permit%20you%20to%20view%20this%20page.",
                    true);
            }

            var accountSubAccounts = new cAccountSubAccounts(user.AccountID);
            cAccountProperties accountProperties = accountSubAccounts.getSubAccountById(user.CurrentSubAccountId).SubAccountProperties;

            cmbCountdown.SelectedValue = accountProperties.CountdownTimer.ToString(CultureInfo.InvariantCulture);
            cmbIdleTimeout.SelectedValue = accountProperties.IdleTimeout.ToString(CultureInfo.InvariantCulture);
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            CurrentUser user = cMisc.GetCurrentUser();
            var accountSubAccounts = new cAccountSubAccounts(user.AccountID);
            cAccountProperties subAccountProperties = accountSubAccounts.getSubAccountById(user.CurrentSubAccountId).SubAccountProperties.Clone();

            subAccountProperties.IdleTimeout = Convert.ToInt32(cmbIdleTimeout.SelectedValue);
            subAccountProperties.CountdownTimer = Convert.ToInt32(cmbCountdown.SelectedValue);
            accountSubAccounts.SaveAccountProperties(subAccountProperties, user.EmployeeID, user.isDelegate ? user.Delegate.EmployeeID : (int?)null);
            Response.Redirect((SiteMap.CurrentNode == null) ? "~/home.aspx" : SiteMap.CurrentNode.ParentNode.Url, true);
        }
         
        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect((SiteMap.CurrentNode == null) ? "~/home.aspx" : SiteMap.CurrentNode.ParentNode.Url, true);
        }
    }
}