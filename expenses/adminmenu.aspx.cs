using System;
using System.Collections.Generic;
using System.Linq;

using BusinessLogic;
using BusinessLogic.DataConnections;
using BusinessLogic.GeneralOptions;
using BusinessLogic.Modules;
using BusinessLogic.ProductModules;

using SpendManagementLibrary.Enumerators;
using Spend_Management;

using SpendManagementLibrary;

/// <summary>
/// The adminmenu.
/// </summary>
public partial class adminmenu : System.Web.UI.Page
{
    /// <summary>
    /// An instance of <see cref="IDataFactory{IGeneralOptions,Int32}"/> to get a <see cref="IGeneralOptions"/>
    /// </summary>
    [Dependency]
    public IDataFactory<IGeneralOptions, int> GeneralOptionsFactory { get; set; }

    /// <summary>
    /// An instance of <see cref="IDataFactory{IProductModule,Modules}"/> to get a <see cref="IProductModule"/>
    /// </summary>
    [Dependency]
    public IDataFactory<IProductModule, Modules> ProductModuleFactory { get; set; }

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
            this.Title = "Administrative Settings";
            this.Master.Title = this.Title;
            CurrentUser user = cMisc.GetCurrentUser();
            this.ViewState["accountid"] = user.AccountID;
            this.ViewState["employeeid"] = user.EmployeeID;

            var module = this.ProductModuleFactory[user.CurrentActiveModule];

            var generalOptions = this.GeneralOptionsFactory[user.CurrentSubAccountId].WithDelegate();
            var clsentities = new cCustomEntities(user);
            bool delegateUser = this.isDelegate;

            bool setupMenuAccess = true;
            bool userManagementMenuAccess = true;
            bool systemOptionsMenuAccess = true;
            bool designMenuAccess = !(delegateUser && !generalOptions.Delegate.DelQEDesign);

            if (delegateUser && !generalOptions.Delegate.DelSetup)
            {
                setupMenuAccess = false;
            }

            if (delegateUser && !generalOptions.Delegate.DelEmployeeAdmin)
            {
                userManagementMenuAccess = false;
            }

            if (delegateUser && !generalOptions.Delegate.DelAuditLog)
            {
                systemOptionsMenuAccess = false;
            }

            var usingExpenses = user.CurrentActiveModule == Modules.Expenses;
            string menuText;

            #region custom entities for base information check

            List<cCustomEntityView> menuItems = clsentities.getViewsByMenuId(3);
            bool showMenuForCustomEntities = (from view in menuItems let entity = clsentities.getEntityById(view.entityid) select view).Any(view => user.CheckAccessRole(AccessRoleType.View, CustomEntityElementType.View, view.entityid, view.viewid, false));

            #endregion custom entities for base information check

            if ((!delegateUser && this.CanAccessBaseInfo(user)) || (delegateUser && (setupMenuAccess || this.CanAccessBaseInfo(user))) || showMenuForCustomEntities)
            {
                menuText = usingExpenses
                               ? "Set-up and alter basic category information such as Reasons, Expense Items, Vehicle Journey Rate Categories and Departments."
                               : "Set-up and alter basic category information such as Reasons, Currencies, Cost Codes and Departments.";

                this.Master.AddMenuItem(
                    "data_preferences",
                    48,
                    "Base Information",
                    menuText,
                    "categorymenu.aspx");
            }

            #region custom entities for tailoring check

            menuItems = clsentities.getViewsByMenuId(4);
            showMenuForCustomEntities = (from view in menuItems let entity = clsentities.getEntityById(view.entityid) select view).Any(view => user.CheckAccessRole(AccessRoleType.View, CustomEntityElementType.View, view.entityid, view.viewid, false));

            #endregion custom entities for tailoring check

            if ((delegateUser && (setupMenuAccess || designMenuAccess || this.CanAccessTailoring(user))) || (!delegateUser && this.CanAccessTailoring(user)) || showMenuForCustomEntities)
            {
                this.Master.AddMenuItem(
                    "thread",
                    48,
                    "Tailoring",
                    string.Format("Customise the look and feel of {0} and alter basic settings.", module.BrandName),
                    "tailoringmenu.aspx");
            }

            #region custom entities for policy check

            menuItems = clsentities.getViewsByMenuId(5);
            showMenuForCustomEntities = (from view in menuItems let entity = clsentities.getEntityById(view.entityid) select view).Any(view => user.CheckAccessRole(AccessRoleType.View, CustomEntityElementType.View, view.entityid, view.viewid, false));

            #endregion custom entities for policy check

            if ((delegateUser && (setupMenuAccess || this.CanAccessPolicyInfo(user))) || (!delegateUser && this.CanAccessPolicyInfo(user)) || showMenuForCustomEntities)
            {
                menuText = usingExpenses
                    ? "Upload and display information about your Travel and Expense Company policy. Enforce policy by highlighting any exceptions that occur."
                    : "Upload and display information about your company policy.";

                this.Master.AddMenuItem(
                    "lock_preferences",
                    48,
                    "Policy Information",
                    menuText,
                    "policymenu.aspx");
            }

            #region custom entities for user management check

            menuItems = clsentities.getViewsByMenuId(6);
            showMenuForCustomEntities = (from view in menuItems let entity = clsentities.getEntityById(view.entityid) select view).Any(view => user.CheckAccessRole(AccessRoleType.View, CustomEntityElementType.View, view.entityid, view.viewid, false));

            #endregion custom entities for user management check

            if ((delegateUser && (userManagementMenuAccess || this.CanAccessUserManagement(user))) || (!delegateUser && this.CanAccessUserManagement(user)) || showMenuForCustomEntities)
            {
                menuText = usingExpenses
                    ? "Alter employees, roles, groups, teams and budget holders."
                    : "Alter employees, roles and teams.";

                this.Master.AddMenuItem(
                    "user_preferences",
                    48,
                    "User Management",
                    menuText,
                    "usermanagementmenu.aspx");
            }

            if (this.CanAccessImportsExports(user))
            {
                this.Master.AddMenuItem(
                    "objects_exchange",
                    48,
                    "Imports / Exports",
                    string.Format("Import and export data from {0}", module.BrandName),
                    "exportmenu.aspx");
            }

            #region custom entities for system options check

            menuItems = clsentities.getViewsByMenuId(6);
            showMenuForCustomEntities = (from view in menuItems let entity = clsentities.getEntityById(view.entityid) select view).Any(view => user.CheckAccessRole(AccessRoleType.View, CustomEntityElementType.View, view.entityid, view.viewid, false));

            #endregion custom entities for system options check

            if ((delegateUser && (systemOptionsMenuAccess || this.CanAccessSystemOptions(user))) || (!delegateUser && this.CanAccessSystemOptions(user)) || showMenuForCustomEntities)
            {
                this.Master.AddMenuItem(
                    "preferences",
                    48,
                    "System Options",
                    "Application configuration area, including audit log, attachment types and IP address filtering.",
                    "~/shared/menu.aspx?area=systemoptions");
            }

            if (this.CanAccessGreenLight(user))
            {
                this.Master.AddMenuItem(
                    "trafficlight_green",
                    48,
                    "GreenLight Management",
                    "Create and configure GreenLights on the system.",
                    "GreenLightAdminMenu.aspx");
            }

            if (this.CanAccessHelpAndSupport(user))
            {
                this.Master.AddMenuItem(
                    "help2",
                    48,
                    "Help &amp; Support Management",
                    "Respond to support tickets, customise the help &amp; support information that is presented to users and administer custom knowledge articles.",
                    "~/shared/menu.aspx?area=helpandsupport"
                );

            }

            // Check for access to View Fund Details //
            if (user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.ViewFunds, true) && user.Account.PaymentServiceEnabled)
            {
                this.Master.AddMenuItem(
                    "currency_pound",
                    48,
                    "View Funds",
                    "Search and view funds details.",
                    "~/shared/admin/viewfundsdetails.aspx"
                );
            }

            if (user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.DocumentConfigurations, false) || user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.DocumentTemplates, false))
            {
                Master.AddMenuItem("eye", 48, "Torch",
                    "Create and modify templates for generating customised mail merge report documents.",
                    "TorchMenu.aspx", "png");
            }

            if (user.Account.ReceiptServiceEnabled && user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.EnvelopeManagement, true))
            {
                Master.AddMenuItem("airmail", 48, "Envelope Management", "Assign account-level envelopes to users' claims.", "~/EnvelopeManagement.aspx");
            }


            this.Master.AddCustomEntityViewMenuIcons(user, 2);
        }
    }

    /// <summary>
    /// The can access base info.
    /// </summary>
    /// <param name="cu">
    /// The cu.
    /// </param>
    /// <returns>
    /// The <see cref="bool"/>.
    /// </returns>
    private bool CanAccessBaseInfo(ICurrentUser cu)
    {
        return cu.CheckAccessRole(AccessRoleType.View, SpendManagementElement.CostCodes, true) ||
               cu.CheckAccessRole(AccessRoleType.View, SpendManagementElement.Currencies, true) ||
               cu.CheckAccessRole(AccessRoleType.View, SpendManagementElement.Countries, true) ||
               cu.CheckAccessRole(AccessRoleType.View, SpendManagementElement.Organisations, true) ||
               cu.CheckAccessRole(AccessRoleType.View, SpendManagementElement.Addresses, true) ||
               cu.CheckAccessRole(AccessRoleType.View, SpendManagementElement.ExpenseItems, true) ||
               cu.CheckAccessRole(AccessRoleType.View, SpendManagementElement.Reasons, true) ||
               cu.CheckAccessRole(AccessRoleType.View, SpendManagementElement.ProjectCodes, true) ||
               cu.CheckAccessRole(AccessRoleType.View, SpendManagementElement.P11D, true) ||
               cu.CheckAccessRole(AccessRoleType.View, SpendManagementElement.Departments, true) ||
               cu.CheckAccessRole(AccessRoleType.View, SpendManagementElement.VehicleJourneyRateCategories, true) ||
               cu.CheckAccessRole(AccessRoleType.View, SpendManagementElement.VehicleEngineType, true) ||
               cu.CheckAccessRole(AccessRoleType.View, SpendManagementElement.Allowances, true) ||
               cu.CheckAccessRole(AccessRoleType.View, SpendManagementElement.ExpenseCategories, true) ||
               cu.CheckAccessRole(AccessRoleType.View, SpendManagementElement.PoolCars, true) ||
               cu.CheckAccessRole(AccessRoleType.View, SpendManagementElement.Workflows, true);
    }

    /// <summary>
    /// The can access tailoring.
    /// </summary>
    /// <param name="cu">
    /// The cu.
    /// </param>
    /// <returns>
    /// The <see cref="bool"/>.
    /// </returns>
    private bool CanAccessTailoring(ICurrentUser cu)
    {
        return cu.CheckAccessRole(AccessRoleType.View, SpendManagementElement.PrintOut, true) ||
               cu.CheckAccessRole(AccessRoleType.View, SpendManagementElement.DefaultView, true) ||
               cu.CheckAccessRole(AccessRoleType.View, SpendManagementElement.DefaultPrintView, true) ||
               cu.CheckAccessRole(AccessRoleType.View, SpendManagementElement.Colours, true) ||
               cu.CheckAccessRole(AccessRoleType.View, SpendManagementElement.CompanyLogo, true) ||
               cu.CheckAccessRole(AccessRoleType.View, SpendManagementElement.Emails, true) ||
               cu.CheckAccessRole(AccessRoleType.View, SpendManagementElement.EmailSuffixes, true) ||
               cu.CheckAccessRole(AccessRoleType.View, SpendManagementElement.CompanyDetails, true) ||
               cu.CheckAccessRole(AccessRoleType.View, SpendManagementElement.GeneralOptions, true) ||
               cu.CheckAccessRole(AccessRoleType.View, SpendManagementElement.UserDefinedFields, true) ||
               cu.CheckAccessRole(AccessRoleType.View, SpendManagementElement.UserdefinedGroupings, true) ||
               cu.CheckAccessRole(AccessRoleType.View, SpendManagementElement.QuickEntryForms, true) ||
               cu.CheckAccessRole(AccessRoleType.View, SpendManagementElement.FilterRules, true) ||
               cu.CheckAccessRole(AccessRoleType.View, SpendManagementElement.Tooltips, true) ||
               cu.CheckAccessRole(AccessRoleType.View, SpendManagementElement.FAQS, true) ||
               cu.CheckAccessRole(AccessRoleType.View, SpendManagementElement.CompanyHelpAndSupportInformation, true);
    }

    /// <summary>
    /// The can access policy info.
    /// </summary>
    /// <param name="cu">
    /// The cu.
    /// </param>
    /// <returns>
    /// The <see cref="bool"/>.
    /// </returns>
    private bool CanAccessPolicyInfo(ICurrentUser cu)
    {
        return cu.CheckAccessRole(AccessRoleType.View, SpendManagementElement.CompanyPolicy, true) ||
               cu.CheckAccessRole(AccessRoleType.View, SpendManagementElement.BroadcastMessages, true) ||
               cu.CheckAccessRole(AccessRoleType.View, SpendManagementElement.FlagsAndLimits, true);
    }

    /// <summary>
    /// The can access user management.
    /// </summary>
    /// <param name="cu">
    /// The cu.
    /// </param>
    /// <returns>
    /// The <see cref="bool"/>.
    /// </returns>
    private bool CanAccessUserManagement(ICurrentUser cu)
    {
        return cu.CheckAccessRole(AccessRoleType.View, SpendManagementElement.AccessRoles, true) ||
               cu.CheckAccessRole(AccessRoleType.View, SpendManagementElement.ItemRoles, true) ||
               cu.CheckAccessRole(AccessRoleType.View, SpendManagementElement.SignOffGroups, true) ||
               cu.CheckAccessRole(AccessRoleType.View, SpendManagementElement.Employees, true) ||
               cu.CheckAccessRole(AccessRoleType.View, SpendManagementElement.Teams, true) ||
               cu.CheckAccessRole(AccessRoleType.View, SpendManagementElement.BudgetHolders, true) ||
               cu.CheckAccessRole(AccessRoleType.View, SpendManagementElement.Audiences, true);
    }

    /// <summary>
    /// The can access system options.
    /// </summary>
    /// <param name="cu">
    /// The cu.
    /// </param>
    /// <returns>
    /// The <see cref="bool"/>.
    /// </returns>
    private bool CanAccessSystemOptions(ICurrentUser cu)
    {
        return cu.CheckAccessRole(AccessRoleType.View, SpendManagementElement.AuditLog, true) ||
               cu.CheckAccessRole(AccessRoleType.View, SpendManagementElement.AttachmentMimeTypes, true) ||
               cu.CheckAccessRole(AccessRoleType.View, SpendManagementElement.CustomMimeHeaders, true) ||
               cu.CheckAccessRole(AccessRoleType.View, SpendManagementElement.IPAdressFiltering, true) ||
               (
                    cu.Account.HasLicensedElement(SpendManagementElement.SingleSignOn) &&
                    cu.CheckAccessRole(AccessRoleType.View, SpendManagementElement.SingleSignOn, true)
               );
    }

    /// <summary>
    /// The can access imports exports.
    /// </summary>
    /// <param name="cu">
    /// The cu.
    /// </param>
    /// <returns>
    /// The <see cref="bool"/>.
    /// </returns>
    private bool CanAccessImportsExports(ICurrentUser cu)
    {
        if (cu.CurrentActiveModule != Modules.Greenlight && cu.CurrentActiveModule != Modules.GreenlightWorkforce)
        {
            return cu.CheckAccessRole(AccessRoleType.View, SpendManagementElement.CorporateCards, true) ||
                   cu.CheckAccessRole(AccessRoleType.View, SpendManagementElement.FinancialExports, true) ||
                   cu.CheckAccessRole(AccessRoleType.View, SpendManagementElement.ImportDataWizard, true) ||
                   cu.CheckAccessRole(AccessRoleType.View, SpendManagementElement.ESRTrustDetails, true) ||
                   cu.CheckAccessRole(AccessRoleType.View, SpendManagementElement.ImportHistory, true) ||
                   cu.CheckAccessRole(AccessRoleType.View, SpendManagementElement.ImportTemplates, true);
        }

        return cu.CheckAccessRole(AccessRoleType.View, SpendManagementElement.FinancialExports, true)
                || cu.CheckAccessRole(AccessRoleType.View, SpendManagementElement.ImportDataWizard, true)
                ||
                (cu.Account.IsNHSCustomer
                 &&
                 (cu.CheckAccessRole(AccessRoleType.View, SpendManagementElement.ImportHistory, true)
                  || cu.CheckAccessRole(AccessRoleType.View, SpendManagementElement.ESRTrustDetails, true)
                  || cu.CheckAccessRole(AccessRoleType.View, SpendManagementElement.ImportTemplates, true)));
    }

    /// <summary>
    /// Check user can access green light.
    /// </summary>
    /// <param name="user">
    /// The user.
    /// </param>
    /// <returns>
    /// The <see cref="bool"/>.
    /// </returns>
    private bool CanAccessGreenLight(ICurrentUser user)
    {
        return user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.CustomEntities, true)
               || user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.GreenLightMenu, true);
    }

    /// <summary>
    /// Check user can access help and support management.
    /// </summary>
    /// <param name="user">
    /// The user.
    /// </param>
    /// <returns>
    /// The <see cref="bool"/>.
    /// </returns>
    private bool CanAccessHelpAndSupport(ICurrentUser user)
    {
        return user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.FAQS, true)
               || user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.CompanyHelpAndSupportInformation, true)
               || user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.SupportTickets, true);
    }

    /// <summary>
    /// Gets a value indicating whether is delegate.
    /// </summary>
    public bool isDelegate
    {
        get
        {
            if (this.Session["myid"] != null)
            {
                return (int)this.Session["delegatetype"] == 1;
            }

            return false;
        }
    }
}
