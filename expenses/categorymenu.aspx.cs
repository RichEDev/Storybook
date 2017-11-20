using System;
using System.Web.UI;

using Spend_Management;

/// <summary>
/// The categorymenu.
/// </summary>
public partial class categorymenu : Page
{
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
            var user = cMisc.GetCurrentUser();
            var usingExpenses = user.CurrentActiveModule == Modules.expenses;

            this.Title = "Base Information";
            this.Master.Title = this.Title;
            string menuText;

            if (user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.CostCodes, true))
            {
                menuText = usingExpenses
                    ? "A list of your cost codes. An employee or expense item can be assigned to a given cost code."
                    : "A list of your cost codes. An employee can be assigned to a given cost code.";

                this.Master.AddMenuItem(
                    "safe_open_full",
                    48,
                    "Cost Codes",
                    menuText,
                    "shared/admin/admincostcodes.aspx");
            }

            if (user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.Currencies, true))
            {
                menuText = usingExpenses
                    ? "A list of currencies and their corresponding exchange rates. The currencies listed here will be available for the claimant to convert from whilst adding an expense."
                    : "A list of currencies and their corresponding exchange rates.";

                this.Master.AddMenuItem(
                    "currency_pound",
                    48,
                    "Currencies",
                    menuText,
                    "shared/admin/admincurrencies.aspx");
            }

            if (user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.Countries, true))
            {
                menuText = usingExpenses
                    ? "A list of countries and their corresponding VAT rates for each expense item. The countries listed here will be available for the claimant to assign a new expense item to."
                    : "A list of countries and their corresponding VAT rates.";

                this.Master.AddMenuItem(
                    "earth_view",
                    48,
                    "Countries",
                    menuText,
                    "shared/admin/admincountries.aspx");
            }

            if (user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.Organisations, true))
            {
                this.Master.AddMenuItem(
                    "office_building",
                    48,
                    "Organisations",
                    "A list of organisations.",
                    "shared/admin/organisations.aspx");
            }

            if (user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.Addresses, true))
            {
                this.Master.AddMenuItem(
                    "earth_location",
                    48,
                    "Addresses",
                    "A list of addresses and their distances to other addresses.",
                    "shared/admin/addresses.aspx");
            }

            if (usingExpenses && user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.ExpenseItems, true))
            {
                this.Master.AddMenuItem(
                    "cubes",
                    48,
                    "Expense Items",
                    "The expense items claimed for by a claimant. Tailor the information a claimant is required to enter, VAT and other calculations.",
                    "expenses/admin/adminsubcats.aspx");
            }

            if (user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.Reasons, true))
            {
                menuText = usingExpenses
                    ? "A list of reasons why a claimant is making a claim for an expense."
                    : "A list of reasons.";

                this.Master.AddMenuItem(
                    "note",
                    48,
                    "Reasons",
                    menuText,
                    "expenses/admin/adminreasons.aspx");
            }

            if (user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.ProjectCodes, true))
            {
                menuText = usingExpenses
                    ? "A list of your project codes. A claimant can assign an expense to a given project code."
                    : "A list of your project codes.";

                this.Master.AddMenuItem(
                    "components",
                    48,
                    "Project Codes",
                    menuText,
                    "shared/admin/adminprojectcodes.aspx");
            }

            if (usingExpenses && user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.P11D, true))
            {
                this.Master.AddMenuItem(
                    "elements",
                    48,
                    "P11D Categories",
                    "A list of the P11D categories used by your company. An expense item can be assigned to a P11d category for reporting purposes.",
                    "admin/adminp11d.aspx");
            }

            if (user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.Departments, true))
            {
                menuText = usingExpenses
                    ? "A list of your departments. An employee or expense item can be assigned to a given department."
                    : "A list of your departments.";

                this.Master.AddMenuItem(
                    "branch_rotated",
                    48,
                    "Departments",
                    menuText,
                    "shared/admin/admindepartments.aspx");
            }

            if (usingExpenses && user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.VehicleJourneyRateCategories, true))
            {
                this.Master.AddMenuItem(
                    "car_compact_blue",
                    48,
                    "Vehicle Journey Rate Categories",
                    "A list of vehicle journey rate categories and their corresponding date ranges and thresholds with rates for different fuel types.",
                    "expenses/admin/adminmileage.aspx");
            }

            if (usingExpenses && user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.VehicleEngineType, true))
            {
                this.Master.AddMenuItem(
                    "fuel_truck",
                    48,
                    "Vehicle Engine Types",
                    "A list of vehicle engine types. These engine types are used for vehicles and in vehicle journey rate categories.",
                    "expenses/admin/adminVehicleEngineTypes.aspx");
            }

            if (usingExpenses && user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.Allowances, true))
            {
                this.Master.AddMenuItem(
                    "money2",
                    48,
                    "Allowances",
                    "A list of allowances that claimants can make a claim for. The amount that can be claimed differs depending on the parameters set for an allowance.",
                    "expenses/admin/adminallowances.aspx");
            }

            if (usingExpenses && user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.ExpenseCategories, true))
            {
                this.Master.AddMenuItem(
                    "index",
                    48,
                    "Expense Categories",
                    "A list of categories that each expense item will be assigned to.",
                    "admin/admincategories.aspx");
            }

            if (usingExpenses && user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.PoolCars, true))
            {
                this.Master.AddMenuItem(
                    "car_sedan_green",
                    48,
                    "Pool Vehicles",
                    "A list of company vehicles that can be used by employees assigned to them.",
                    "~/shared/admin/poolcars.aspx");
            }

            if (user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.Workflows, true))
            {
                this.Master.AddMenuItem(
                    "office_building",
                    48,
                    "Workflows",
                    "Manage your workflows",
                    "~/shared/admin/workflows.aspx");
            }

            this.Master.AddCustomEntityViewMenuIcons(user, 3);
        }
    }
}