using SpendManagementLibrary.HelpAndSupport;

namespace Spend_Management
{
    #region Using Directives

    using System;
    using System.Collections.Generic;
    using System.Web.UI;

    using BusinessLogic;
    using BusinessLogic.DataConnections;
    using BusinessLogic.Modules;
    using BusinessLogic.ProductModules;

    using SpendManagementLibrary;

    using Spend_Management.shared.code.GreenLight;

    #endregion

    /// <summary>
    /// The menu.
    /// </summary>
    public partial class menu : Page
    {
        /// <summary>
        /// An instance of <see cref="IDataFactory{TComplexType,TPrimaryKeyDataType}"/> to get a <see cref="IProductModule"/>
        /// </summary>
        [Dependency]
        public IDataFactory<IProductModule, Modules> ProductModuleFactory { get; set; }

        #region Methods

        /// <summary>
        /// The page_ load.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void Page_Load(object sender, EventArgs e)
        {
            this.Master.UseDynamicCSS = true;
            CurrentUser user = cMisc.GetCurrentUser();
            this.ViewState["accountid"] = user.AccountID;
            this.ViewState["employeeid"] = user.EmployeeID;

            var module = this.ProductModuleFactory[user.CurrentActiveModule];

            string menuPage = string.Empty;

            if (this.Request.QueryString["area"] != null)
            {
                menuPage = this.Request.QueryString["area"];
            }

            switch (menuPage)
            {
                case "systemoptions":
                    this.Title = "System Options";

                    if (user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.AuditLog, true))
                    {
                        this.Master.addMenuItem(
                            "policeman_bobby",
                            48,
                            "Audit Log",
                            string.Format("Every action that occurs in {0} is logged. Search and view logged actions.", module.BrandName),
                            "~/shared/admin/auditlog.aspx");
                    }

                    if (user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.IPAdressFiltering, true))
                    {
                        this.Master.addMenuItem(
                            "key_preferences",
                            48,
                            "IP Address Filtering",
                            string.Format("Specify which IP Addresses should be allowed to access {0}.", module.BrandName),
                            "~/shared/admin/adminIPfilters.aspx");
                    }

                    if (user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.AttachmentMimeTypes, true))
                    {
                        this.Master.addMenuItem(
                            "document_ok",
                            48,
                            "Attachment Types",
                            "Define and manage the file attachment types permitted to be uploaded, including the associated MIME content type definition required by the browser.",
                            "~/shared/admin/AttachmentTypes.aspx",
                            "png");
                    }

                    if (user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.CustomMimeHeaders, true))
                    {
                        this.Master.addMenuItem(
                            "document_edit",
                            48,
                            "Custom Attachment Types",
                            "Add and edit custom file attachment types, including the associated MIME content type definition required by the browser.",
                            "~/shared/admin/CustomAttachmentTypes.aspx",
                            "png");
                    }

                    if (user.Account.HasLicensedElement(SpendManagementElement.SingleSignOn) && user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.SingleSignOn, true))
                    {
                        this.Master.addMenuItem(
                            "application_certificate",
                            48,
                            "Single Sign-on",
                            "Upload encryption certificates and configure SSO attributes and SAML responses. Download the Selenity Limited public certificate.",
                            "~/shared/admin/sso.aspx",
                            "png");
                    }

                    this.Master.AddCustomEntityViewMenuIcons(user, 7);

                    break;
                case "helpandsupport":

                    this.Title = "Help &amp; Support Management";

                    cAccountProperties accountProperties = new cAccountSubAccounts(user.AccountID).getSubAccountById(user.CurrentSubAccountId).SubAccountProperties;
                    if (accountProperties.EnableInternalSupportTickets && user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.SupportTickets, true))
                    {
                        this.Master.addMenuItem(
                            "lifebelt",
                            48,
                            string.Format("Support Tickets ({0})", SupportTicketInternal.GetCountForAdministrator(user)),
                            "Respond to support tickets raised by other users.",
                            "~/shared/admin/adminHelpAndSupportTickets.aspx");
                    }

                    if (user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.CompanyHelpAndSupportInformation, true))
                    {
                        this.Master.addMenuItem(
                            "books_blue",
                            48,
                            "Help &amp; Support Information",
                            "Customise the information presented to users on the Help &amp; Support page.",
                            "~/shared/admin/helpInformation.aspx");
                    }

                    if (user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.FAQS, true))
                    {
                            this.Master.addMenuItem(
                            "question_and_answer",
                            48,
                            "Knowledge Articles",
                            "Define Frequently Asked Questions, about your company policy or how to use expense items, that claimants can view in the Help &amp; Support section.",
                            "~/shared/admin/KnowledgeCustomArticles.aspx");
                    }

                    break;
            }

            this.Master.title = this.Title;

        }

        #endregion
    }
}