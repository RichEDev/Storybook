#region Using Directives

using System;
using System.Collections.Generic;
using System.Web.UI;

using BusinessLogic;
using BusinessLogic.DataConnections;
using BusinessLogic.Modules;
using BusinessLogic.ProductModules;

using Spend_Management;
using SpendManagementLibrary;

using Spend_Management.shared.code.GreenLight;

#endregion

/// <summary>
/// The reports_tailoringmenu.
/// </summary>
public partial class reports_tailoringmenu : Page
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
        if (this.IsPostBack == false)
        {
            CurrentUser user = cMisc.GetCurrentUser();
            var usingExpenses = user.CurrentActiveModule == Modules.Expenses;

            var module = this.ProductModuleFactory[user.CurrentActiveModule];

            this.ViewState["accountid"] = user.AccountID;
            this.ViewState["employeeid"] = user.EmployeeID;

            var clsAccounts = new cAccounts();
            cAccount reqAccount = clsAccounts.GetAccountByID(user.AccountID);

            this.Title = "Tailoring";
            this.Master.Title = this.Title;

            if (usingExpenses)
            {
                if (user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.PrintOut, true))
                {
                    this.Master.AddMenuItem(
                        "printer2",
                        48,
                        "Print-Out",
                        "Specify information about a claimant that you would like to appear on a print-out of their claims.",
                        "admin/printout.aspx");
                }

                if (user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.DefaultView, true))
                {
                    this.Master.AddMenuItem(
                        "monitor_preferences",
                        48,
                        "Default View",
                        "Set the default view for a current, submitted or previous claim that claimants will see.",
                        "setupview.aspx?viewid=-1");
                }

                if (user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.DefaultPrintView, true))
                {
                    this.Master.AddMenuItem(
                        "printer_preferences",
                        48,
                        "Default Print View",
                        "Specify the view for a current, submitted or previous claim on a print-out claim. This can not be altered by a claimant.",
                        "setupview.aspx?viewid=-2");
                }
            }

            if (user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.Colours, true))
            {
                this.Master.AddMenuItem(
                    "palette",
                    48,
                    "Colours",
                    string.Format("Modify the look of {0} by altering the colour of various sections.", module.BrandNameHtml),
                    "shared/admin/colours.aspx");
            }

            if (user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.CompanyLogo, true))
            {
                this.Master.AddMenuItem(
                    "photo_landscape2",
                    48,
                    "Company Logo",
                    "Upload your company logo to appear in the top left hand corner of the window.",
                    "admin/companylogo.aspx");
            }

            if (user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.Emails, true))
            {
                this.Master.AddMenuItem(
                    "mail_server",
                    48,
                    "Notification Templates",
					"Create and modify email templates, broadcast messages and mobile notifications.",
					"shared/admin/adminnotificationtemplates.aspx");
            }

            if (user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.EmailSuffixes, true))
            {
                this.Master.AddMenuItem(
                    "symbol_at",
                    48,
                    "E-mail Suffixes",
                    "Manage email suffixes which may use the self registration form.",
                    "~/expenses/admin/emailsuffixes.aspx");
            }

            if (user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.CompanyDetails, true))
            {
                this.Master.AddMenuItem(
                    "office_building",
                    48,
                    "Company Details",
                    "Specify basic company information.",
                    "expenses/admin/companydetails.aspx");
            }

            if (user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.GeneralOptions, true))
            {
                this.Master.AddMenuItem(
                    "gear_preferences",
                    48,
                    "General Options",
                    string.Format("Enable or disable various features within {0}.", module.BrandNameHtml),
                    "~/shared/admin/accountOptions.aspx"); // admin/generaloptions.aspx");
            }

            if (user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.UserDefinedFields, true))
            {
                this.Master.AddMenuItem(
                    "data_edit",
                    48,
                    "User Defined Fields",
                    string.Format("Add, edit or delete User Defined Fields here. These fields are used to store additional information that {0} does not record by default.", module.BrandNameHtml),
                    "shared/admin/adminuserdefined.aspx");

                this.Master.AddMenuItem(
                    "sort_ascending",
                    48,
                    "Userdefined Ordering",
                    "Modify the order of userdefined groups/fields.",
                    "shared/admin/userdefinedOrdering.aspx");
            }

            if (user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.UserdefinedGroupings, true))
            {
                this.Master.AddMenuItem(
                    "elements_selection",
                    48,
                    "Userdefined Groupings",
                    "Define and maintain the user defined field groupings.",
                    "shared/admin/userdefinedFieldGroupings.aspx");
            }

            if (usingExpenses)
            {
                if (reqAccount.QuickEntryFormsEnabled
                    && user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.QuickEntryForms, true))
                {
                    this.Master.AddMenuItem(
                        "table2_add",
                        48,
                        "Quick Entry Form Design",
                        "Add, edit and delete Quick Entry Forms that are available for use by claimants.",
                        "admin/adminqeforms.aspx");
                }
            }

            if (usingExpenses)
            {
                if (user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.FilterRules, true))
                {
                    this.Master.AddMenuItem(
                        "scroll_ok",
                        48,
                        "Filter Rules",
                        "Add and edit filtering rules for Costcodes, Departments, Addresses, Project Codes and Reasons.",
                        "admin/filterrules.aspx?FilterType=0");
                }
            }

            if (user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.Tooltips, true))
            {
                this.Master.AddMenuItem(
                    "help2",
                    48,
                    "Tooltips",
                    "Customise the text that is displayed in a tooltip.",
                    "shared/admin/tooltips/tooltips.aspx");
            }

            this.Master.AddCustomEntityViewMenuIcons(user, 4);
        }
    }

    #endregion
}
