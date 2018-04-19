namespace Spend_Management
{
    using System.Net;
    using System;
    using System.Web;
    using System.Web.UI;

    using BusinessLogic;
    using BusinessLogic.AccountProperties;
    using BusinessLogic.DataConnections;
    using BusinessLogic.Enums;
    using BusinessLogic.GeneralOptions;

    using SpendManagementLibrary;

    public partial class helpInformation : System.Web.UI.Page
    {
        /// <summary>
        /// An instance of <see cref="IDataFactory{IGeneralOptions,Int32}"/> to get a <see cref="IGeneralOptions"/>
        /// </summary>
        [Dependency]
        public IDataFactory<IAccountProperty, AccountPropertyCacheKey> AccountPropertiesFactory { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();
            Master.title = "Help &amp; Support Information";
            Title = "Help &amp; Support Information";
            Master.PageSubTitle = Title;

            Master.enablenavigation = false;

            cAccountSubAccounts clsSubAccounts = new cAccountSubAccounts(currentUser.AccountID);
            cAccountProperties reqProperties = clsSubAccounts.getSubAccountById(currentUser.CurrentSubAccountId).SubAccountProperties;

            cModules clsModules = new cModules();

            if (IsPostBack == false)
            {
                litModuleSectionTitle.Text = clsModules.GetModuleByID((int)currentUser.CurrentActiveModule).BrandNamePlainText;

                #region help information
                txtCustomerHelpContactName.Text = reqProperties.CustomerHelpContactName;
                txtCustomerHelpContactTelephone.Text = reqProperties.CustomerHelpContactTelephone;
                txtCustomerHelpContactFax.Text = reqProperties.CustomerHelpContactFax;
                txtCustomerHelpContactAddress.Text = reqProperties.CustomerHelpContactAddress;
                txtCustomerHelpContactEmailAddress.Text = reqProperties.CustomerHelpContactEmailAddress;
                this.txtCustomerHelpInformation.Text = reqProperties.CustomerHelpInformation;
                #endregion help information
                this.txtCustomerHelpInformation.BasePath = GlobalVariables.StaticContentLibrary + "/ckeditor";
                CKEditorExtensions.Configure(this.txtCustomerHelpInformation, EditorMode.Short);
                //no helpinformation element defined yet
                //currentUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement, true, true);
            }
        }

        /// <summary>
        /// Save Page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSave_Click(object sender, ImageClickEventArgs e)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();

            this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.CustomerHelpContactName.GetDescription(), this.txtCustomerHelpContactName.Text, currentUser.CurrentSubAccountId));

            this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.CustomerHelpContactTelephone.GetDescription(), this.txtCustomerHelpContactTelephone.Text, currentUser.CurrentSubAccountId));

            this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.CustomerHelpContactFax.GetDescription(), this.txtCustomerHelpContactFax.Text, currentUser.CurrentSubAccountId));

            this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.CustomerHelpContactAddress.GetDescription(), this.txtCustomerHelpContactAddress.Text, currentUser.CurrentSubAccountId));

            this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.CustomerHelpContactEmailAddress.GetDescription(), this.txtCustomerHelpContactEmailAddress.Text, currentUser.CurrentSubAccountId));

            this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.CustomerHelpInformation.GetDescription(), WebUtility.HtmlDecode(this.txtCustomerHelpInformation.Text), currentUser.CurrentSubAccountId));

            var accountBase = new cAccountSubAccountsBase(currentUser.AccountID);
            accountBase.InvalidateCache(currentUser.CurrentSubAccountId);

            this.btnCancel_Click(sender, e);
        }

        /// <summary>
        /// Cancel Page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnCancel_Click(object sender, ImageClickEventArgs e)
        {
            string previousUrl = (SiteMap.CurrentNode == null) ? "~/home.aspx" : SiteMap.CurrentNode.ParentNode.Url;

            Response.Redirect(previousUrl, true);
        }
    }
}
