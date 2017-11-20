namespace expenses
{
    using System;
    using System.Web.UI;
    using Spend_Management;

    public partial class EnvelopeManagement : Page, IRequireCurrentUser
    {
        /// <summary>
        /// The Current User
        /// </summary>
        public ICurrentUser CurrentUser { get; set; }

        /// <summary>
        /// The AccountId that this page is manipulating envelopes for.
        /// </summary>
        public int AccountId;
        
        protected void Page_Load(object sender, EventArgs e)
        {
            // Force IE out of compat mode
            Response.AddHeader("X-UA-Compatible", "IE=edge");

            Title = @"Envelope Management";
            Master.title = Title;
            Master.helpid = 666;
            Master.enablenavigation = false;
            Master.UseDynamicCSS = true;

            // check rights
            CurrentUser = cMisc.GetCurrentUser();

            if (!CurrentUser.Account.ReceiptServiceEnabled || !CurrentUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.EnvelopeManagement, true))
            {
                Response.Redirect(ErrorHandlerWeb.InsufficientAccess, true);
            }

            ClaimSelector.Selectable = true;
            AccountId = CurrentUser.AccountID;
        }

        #region Web Form Designer generated code
        override protected void OnInit(EventArgs e)
        {
            //
            // CODEGEN: This call is required by the ASP.NET Web Form Designer.
            //
            InitializeComponent();
            base.OnInit(e);
        }

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
        }

        #endregion
    }
}
