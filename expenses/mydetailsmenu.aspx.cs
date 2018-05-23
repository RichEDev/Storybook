using System;
using System.Web.UI;

using BusinessLogic;
using BusinessLogic.DataConnections;
using BusinessLogic.GeneralOptions;
using BusinessLogic.Modules;
using BusinessLogic.ProductModules;

using Spend_Management;
using Spend_Management.shared.code;

using SpendManagementLibrary;

/// <summary>
/// The mydetailsmenu.
/// </summary>
public partial class mydetailsmenu : Page
{
    /// <summary>
    /// An instance of <see cref="IDataFactory{IGeneralOptions,Int32}"/> to get a <see cref="IGeneralOptions"/>
    /// </summary>
    [Dependency]
    public IDataFactory<IGeneralOptions, int> GeneralOptionsFactory { get; set; }

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
        if (string.IsNullOrEmpty(this.Request.QueryString["message"]) == false
            && this.Request.QueryString["message"] == "vehicle added")
        {
            this.litMenuHeader.Text =
                "<div class=\"errortext\" style=\"text-align: center\">Your vehicle has been added</div>";
        }

        if (this.IsPostBack == false)
        {
            var user = cMisc.GetCurrentUser();
            this.ViewState["accountid"] = user.AccountID;
            this.ViewState["employeeid"] = user.EmployeeID;

            var usingExpenses = user.CurrentActiveModule == Modules.Expenses;
            
            var generalOptions = this.GeneralOptionsFactory[user.CurrentSubAccountId].WithCar().WithMobile();

            var module = this.ProductModuleFactory[user.CurrentActiveModule];

            string brandName = (module != null) ? module.BrandNameHtml : "Expenses";

            this.Title = "My Details";
            this.Master.Title = this.Title;

            var menuText = usingExpenses
                ? "Update your name, address and other basic details. View details of your vehicle(s) and mileage. Examine the stages your claim has to go through to be approved."
                : "Update your name, address and other basic details.";

            this.Master.AddMenuItem(
                "user_view",
                48,
                "Change My Details",
                menuText, 
                "shared/information/mydetails.aspx");

            this.Master.AddMenuItem(
                "user_add",
                48,
                "Delegates",
                "Assign delegates that can peform actions on your behalf.", 
                "information/delegates.aspx");

            cAccount account = new cAccounts().GetAccountsWithPaymentServiceEnabled().Find(x => x.accountid == user.AccountID);

            if (account != null)
            {
                this.Master.AddMenuItem(
                    "wallet_open",
                    48,
                    "Payment History",
                    "View the History of Payments",
                    "shared/information/PaymentHistory.aspx"
                     );
            }
            
            if (generalOptions.Car.AllowUsersToAddCars)
            {
                this.Master.AddMenuItem(
                    "car_compact_green",
                    48,
                    "Add Vehicle",
                    string.Format("Add your vehicle on to {0} for an administrator to approve.", brandName),
                    "expenses/information/mycars.aspx");
            }

            if (user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.Tasks, true))
            {
                this.Master.AddMenuItem(
                    "calendar_preferences",
                    48,
                    "My Tasks",
                    "View tasks currently assigned either directly to you or to a team, of which you are a member.",
                    "shared/tasks/MyTasks.aspx");
            }

            if (usingExpenses && generalOptions.Mobile.UseMobileDevices)
            {
                if (user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.MobileDevices, true))
                {
                    this.Master.AddMenuItem(
                        "user_mobilephone",
                        48,
                        "My Mobile Devices",
                        "View and manage mobile devices associated to your account.",
                        "shared/information/MyMobileDevices.aspx",
                        "png");
                }
            }
            // Show Add Bank Account menu item //
            if (user.isDelegate == false && user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.BankAccounts, true))
            {
                this.Master.AddMenuItem(
                    "user_bankaccount",
                    48,
                    "My Bank Accounts",
                    "Manage your personal Bank Accounts within Expenses. Add multiple Bank Accounts so that you can specify where reimbursements are paid.",
                    "shared/information/MyBankAccounts.aspx");
            }
            int menuId = new CustomMenuStructure(user.AccountID).GetCustomMenuIdByName(this.Title, true);
            this.Master.AddCustomEntityViewMenuIcons(user, menuId);
        }
    }

    #endregion
}
