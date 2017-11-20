using SpendManagementLibrary.Enumerators;
using SpendManagementLibrary.HelpAndSupport;
using Spend_Management.shared.code;

namespace expenses
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Linq;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    using Spend_Management.shared.code.GreenLight;

    using expenses.information;
    using Spend_Management;
    using SpendManagementLibrary;
    using SpendManagementLibrary.Helpers;
    using SpendManagementLibrary.Holidays;

    /// <summary>
    /// Home page for expenses.
    /// </summary>
    public partial class home : Page
    {
        /// <summary>
        /// The litoptions.
        /// </summary>
        protected Literal Litoptions;

        /// <summary>
        /// Gets a value indicating whether is delegate.
        /// </summary>
        public bool IsDelegate
        {
            get
            {
                if (this.Session["myid"] != null)
                {
                    return (int)Session["delegatetype"] == 1;
                }

                return false;
            }
        }

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
            var user = cMisc.GetCurrentUser();
            var clsModules = new cModules();
            var module = clsModules.GetModuleByID((int)user.CurrentActiveModule);

            var usingExpenses = user.CurrentActiveModule == Modules.expenses;

            cAccountProperties accountProperties = new cAccountSubAccounts(user.AccountID).getSubAccountById(user.CurrentSubAccountId).SubAccountProperties;

            this.Title = string.Format("Welcome to {0}", module.BrandNamePlainText);
            this.Master.Title = string.Format("Welcome to {0}", module.BrandNameHTML);

            if (this.IsPostBack == false)
            {
                this.ViewState["accountid"] = user.AccountID;

                var clsmisc = new cMisc(user.AccountID);
                cGlobalProperties clsproperties = clsmisc.GetGlobalProperties(user.AccountID);
                var clsemployees = new cEmployees(user.AccountID);

                bool delegateUser = AccessRoleCheck.IsDelegate;

                if (usingExpenses)
                {
                    this.Master.AddMenuItem("add_new_expense", 48, "Add New Expenses", "Add new entries to current claims for any expenses you have incurred. If a current claim does not exist one will be created.", "aeexpense.aspx");

                    var clsforms = new cQeForms(user.AccountID);
                    if (clsforms.count != 0 && user.Account.QuickEntryFormsEnabled)
                    {
                        this.Master.AddMenuItem("quick_entry_form", 48, "Quick Entry Forms", "An alternative to Add New Expenses, allowing you to add many items concurrently.", "qelst.aspx");
                    }

                    this.Master.AddMenuItem("my_claims", 48, "My Claims", "Create new claims, edit or delete existing ones. View current, submitted and previous claims. Submit finished claims into the approval process.", "claimsmenu.aspx");
                    if (user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.ClaimViewer, true))
                    {
                        this.Master.AddMenuItem("claim_viewer", 48, "Claim Viewer", "View other user's claims.", "expenses/claimSelector.aspx");
                    }

                    var clscards = new cEmployeeCorporateCards(user.AccountID);
                    if (clscards.HasCreditCard(user.EmployeeID) || clscards.HasPurchaseCard(user.EmployeeID))
                    {
                        this.Master.AddMenuItem("credit_cards", 48, "Reconcile Corporate Cards", "Reconcile transactions you have made on corporate credit or purchase cards.", "expenses/claimsummary.aspx?claimtype=1");
                    }
                }

                string menuText = usingExpenses
                                      ? "Update your basic details such as name and address. View details of your current car and vehicle journey rate. View the steps required to approve a claim you submit. Assign delegates to manage your account."
                                      : "Update your basic details such as name and address. Assign delegates to manage your account.";

                this.Master.AddMenuItem(
                        "my_details",
                        48,
                        "My Details",
                        menuText,
                        "mydetailsmenu.aspx");

                this.Master.AddMenuItem("help", 48, "Help &amp; Support", "Help &amp; Support is an online service for education, guidance and support that enables you to find the best answers for your " + module.BrandNameHTML + " questions.", "shared/helpAndSupport.aspx");

                if (usingExpenses)
                {
                    if (user.Employee.AdvancesSignOffGroup != 0 && user.Account.AdvancesEnabled)
                    {
                        this.Master.AddMenuItem("advances", 48, "My Advances", "Request new advances and see the status of approved ones.", "information/myadvances.aspx");
                    }
                }

                menuText = usingExpenses
                    ? "View a copy of your travel and expense company policy."
                    : "View a copy of your company policy.";

                if (user.CurrentActiveModule != Modules.CorporateDiligence)
                {
                    this.Master.AddMenuItem("policy", 48, "View My Company Policy", menuText, "showpolicy('./policy.aspx');");
                }

                if (usingExpenses)
                {
                    this.Master.AddMenuItem("claimable_item", 48, "View My Claimable Items", "Examine the expense items you are allowed to claim and view any associated limits.", "information/selectmultiple.aspx");

                    if (user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.CheckAndPay, true))
                    {
                        var clsclaims = new cClaims(user.AccountID);
                        int claimcount = clsclaims.getClaimsToCheckCount(user.EmployeeID, false, user.isDelegate ? user.Delegate.EmployeeID : (int?)null);
                        this.Master.AddMenuItem("check_and_pay", 48, "Check & Pay Expenses (" + claimcount + ")", "View and approve any claims you are responsible for checking.", "expenses/checkpaylist.aspx");
                    }
                }

                var claimantreports = (bool?)Cache["claimantreports" + user.AccountID];
                if (claimantreports == null)
                {
                    try
                    {
                        var data = new DBConnection(cAccounts.getConnectionString(user.AccountID));
                        const string Sql = "SELECT COUNT([reportID]) FROM [dbo].[reports] WHERE [forclaimants] = 1";
                        int cnt = data.getcount(Sql);
                        claimantreports = cnt > 0;

                        if (GlobalVariables.GetAppSettingAsBoolean("EnableBrokers"))
                        {
                            Cache.Insert("claimantreports" + user.AccountID, claimantreports, null, System.Web.Caching.Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes((int)Caching.CacheTimeSpans.VeryShort), System.Web.Caching.CacheItemPriority.Default, null);
                        }
                    }
                    catch 
                    {
                        claimantreports = false;
                    }
                }

                bool reportsAccess = user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.Reports, true);
                if (claimantreports.Value || reportsAccess)
                {
                    this.Master.AddMenuItem(
                        "reports",
                        48,
                        "Reports",
                        "Create new reports, edit, delete or view existing ones. Export data to Excel, CSV, flat file or create pivot tables.",
                        "shared/reports/rptlist.aspx" + (claimantreports.Value && !reportsAccess ? "?claimants=1" : string.Empty));
                }

                if (accountProperties.EnableInternalSupportTickets && user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.SupportTickets, true))
                {
                    this.Master.AddMenuItem(
                        "supprt_ticket",
                        48,
                        string.Format("Support Tickets ({0})", SupportTicketInternal.GetCountForAdministrator(user)),
                        "Respond to support tickets raised by other users.",
                        "shared/admin/adminHelpAndSupportTickets.aspx");
                }

                if (AccessRoleCheck.CanAccessAdminSettings(user))
                 {
                    menuText = usingExpenses
                        ? "Manage basic information such as Expense Categories, Addresses and Reasons. Manage new employees and customise the way {0} functions."
                        : "Manage basic information such as Cost Codes, Currencies and Addresses. Manage new employees and customise the way {0} functions.";

                    this.Master.AddMenuItem(
                        "administrative_settings",
                        48,
                        "Administrative Settings",
                        string.Format(menuText, module.BrandNameHTML),
                        "adminmenu.aspx");
                }

                if (user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.LogonMessages, true))
                {
                    if (usingExpenses)
                    {
                        this.Master.AddMenuItem(
                            "flatscreen_tv",
                            48,
                            "Marketing Information",
                            string.Format("Manage background images, icons and text for the marketing panel on the logon page."),
                            "shared/admin/LogonMessages.aspx");
                    }
                }

                if ((usingExpenses && clsproperties.showreviews && user.Account.HotelReviewsEnabled) || (clsproperties.searchemployees && user.Account.EmployeeSearchEnabled))
                {
                    menuText = usingExpenses
                        ? "Search for other claimants who use {0} or search for and view hotel reviews."
                        : "Search for other employees who use {0}.";

                    this.Master.AddMenuItem(
                        "search",
                        48,
                        "Search",
                        string.Format(menuText, module.BrandNameHTML),
                        "searchmenu.aspx");
                }

                if ((!delegateUser && (user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.EmployeeExpenses, false) || clsemployees.isProxy(user.Employee.EmployeeID)))
                    || (delegateUser && (clsproperties.delemployeeaccounts || clsemployees.isProxy(user.Employee.EmployeeID))))
                {
                    if (this.Session["myid"] == null)
                    {
                        this.Master.AddMenuItem(
                            "delegate_logon",
                            48,
                            "Delegate Logon",
                            "Log on to another user's account as a delegate. Perform actions on their behalf.",
                            "shared/admin/emplogon.aspx");
                    }
                }

                if (usingExpenses)
                {
                    if (user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.Advances, true) && user.Account.AdvancesEnabled)
                    {
                        this.Master.AddMenuItem("cashier_approve", 48, "Advances", "Authorise and check the status of any cash advances.", "admin/adminfloats.aspx");
                    }

                    if (user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.CheckAndPay, true))
                    {
                        this.Master.AddMenuItem("holiday", 48, "Holidays", "Enter your upcoming holidays here so that any claims you are responsible for checking can be dealt with accordingly.", "information/holidays.aspx");
                    }
                }

                if (usingExpenses)
                {
                    if (accountProperties.UseMobileDevices)
                    {
                        if (user.Account.IsNHSCustomer == true)
                        {
                            this.Master.AddMenuItem("mobile", 48, "NHS Expenses Mobile App",
                                "Download the new app that allows you to manage your NHS Expenses without the need to power up your computer.",
                                "http://www.software-europe.com/nhs-app-download",
                                "png", "_blank", true);

                        }
                        else
                        {
                            this.Master.AddMenuItem("mobile", 48, "Expenses Mobile App",
                                "Download the new app that allows you to manage your expenses without the need to power up your computer.",
                                "http://www.software-europe.com/app-download",
                                "png", "_blank", true);
                        }
                    }
                    else if (user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.GeneralOptions, true))
                    {
                        if (user.Account.IsNHSCustomer == true)
                        {
                            this.Master.AddMenuItem("mobile", 48, "Enable the NHS Expenses Mobile app for your claimants",
                                "Find out how to save time and money by enabling NHS Expenses Mobile for your claimants",
                                "http://knowledge.software-europe.com/articles/Walkthrough_Guide/Enable-Expenses-Mobile-within-your-organisation/",
                                "png", "_blank", true);

                        }
                        else
                        {
                            this.Master.AddMenuItem("mobile", 48, "Enable the Expenses Mobile App for your claimants",
                                "Find out how to save time and money by enabling Expenses Mobile for your claimants",
                                "http://knowledge.software-europe.com/articles/Walkthrough_Guide/Enable-Expenses-Mobile-within-your-organisation/",
                                "png", "_blank", true);
                        }

                    }

                    this.Master.AddMenuItem(
                        "trafficlight_green",
                        48,
                        "GreenLight",
                        "GreenLight means you can now take any paper based approval and put it online, click here to find out more.",
                        GlobalVariables.GreenLightInfoPage,
                        "png",
                        "_blank",
                        true);
                }
                this.Master.AddCustomEntityViewMenuIcons(user, 1);

                this.Master.AddMenuItem("exit", 48, "Log Out", "Log out of " + module.BrandNameHTML + " and close this window.", "shared/process.aspx?process=1");

                if (Request.QueryString["action"] != null)
                {
                    int action = int.Parse(Request.QueryString["action"]);
                    if (action == 4)
                    {
                        clsemployees.clearNotes(user.Employee.EmployeeID);
                    }
                }
            }
        }

        /// <summary>
        /// The on init.
        /// </summary>
        /// <param name="e">
        /// The e.
        /// </param>
        [SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1206:DeclarationKeywordsMustFollowOrder", Justification = "Reviewed. Suppression is OK here.")]
        override protected void OnInit(EventArgs e)
        {
            this.InitializeComponent();
            base.OnInit(e);
        }

        /// <summary>
        /// The initialize component.
        /// </summary>
        private void InitializeComponent()
        {
        }
    }
}
