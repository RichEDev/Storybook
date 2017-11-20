namespace Spend_Management.expenses
{
    #region Using Directives

    using System;
    using System.Web.UI;

    #endregion

    /// <summary>
    /// The claim selector.
    /// </summary>
    public partial class ClaimSelector : Page, IRequireCurrentUser
    {
        #region Properties

        /// <summary>
        /// The Current User
        /// </summary>
        public ICurrentUser CurrentUser { get; set; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// The load event for the page.
        /// </summary>
        /// <param name="sender">
        /// The sender object.
        /// </param>
        /// <param name="e">
        /// The event argument e
        /// </param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack)
            {
                return;
            }

            Title = @"Claim Viewer";
            Master.PageSubTitle = "Claim Viewer";
            Master.enablenavigation = true;
            Master.UseDynamicCSS = true;

            CurrentUser = cMisc.GetCurrentUser();

            if (CurrentUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.ClaimViewer, true) == false)
            {
                Response.Redirect(ErrorHandlerWeb.InsufficientAccess, true);
            }
        }

        #endregion
    }
}