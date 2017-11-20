namespace Spend_Management.shared
{
    using System;
    using SpendManagementLibrary;

    public partial class SystemHealth : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack)
            {
                return;
            }
            this.Title = @"System Health";
            CurrentUser currentUser = cMisc.GetCurrentUser();
            if (!currentUser.Employee.AdminOverride)
            {
                Response.Redirect("~/shared/restricted.aspx?reason=Current%20access%20role%20does%20not%20permit%20you%20to%20view%20this%20page.");
            }

            this.iconPath.InnerText = string.Format("{0}/icons/16/plain/", GlobalVariables.StaticContentLibrary);

            this.StaticPath = GlobalVariables.StaticContentLibrary;
            this.Colours = new cColours(currentUser.AccountID, currentUser.CurrentSubAccountId, currentUser.CurrentActiveModule);
        }

        /// <summary>
        /// Gets the static path.
        /// </summary>
        public string StaticPath { get; set; }

        /// <summary>
        /// Gets the Colours to use for this account.
        /// </summary>
        public cColours Colours { get; set; }
    }
}