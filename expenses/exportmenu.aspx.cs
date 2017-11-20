#region Using Directives

using System;
using System.Web.UI;

using SpendManagementLibrary;

using Spend_Management;

#endregion

/// <summary>
/// The exportmenu.
/// </summary>
public partial class exportmenu : Page
{
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
            this.Title = "Imports / Exports";
            this.Master.Title = this.Title;
            CurrentUser user = cMisc.GetCurrentUser();
            var usingExpenses = user.CurrentActiveModule == Modules.expenses;

            this.ViewState["accountid"] = user.AccountID;
            this.ViewState["employeeid"] = user.EmployeeID;
            this.ViewState["accountid"] = user.AccountID;

            if (usingExpenses && user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.CorporateCards, true))
            {
                this.Master.AddMenuItem(
                    "credit_cards",
                    48,
                    "Corporate Card Imports",
                    "Import electronic credit card statements and automatically distribute them to the associated claimants for reconciliation.",
                    "admin/statements.aspx");
            }

            if (user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.FinancialExports, true))
            {
                this.Master.AddMenuItem(
                    "export2",
                    48,
                    "Financial Exports",
                    "Export report data to be used in other software packages. Data exported using this option can only be exported once.",
                    "expenses/admin/financialexports.aspx");
            }

            if (user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.ImportDataWizard, true))
            {
                this.Master.AddMenuItem(
                    "import2", 48, "Import Data Wizard", "Import data.", "shared/importsexports/importdatawizard.aspx");
            }

            if (user.Account.IsNHSCustomer)
            {
                if (user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.ImportHistory, true))
                {
                    this.Master.AddMenuItem(
                        "import2",
                        48,
                        "Import History",
                        "View previously executed imports.",
                        "shared/importsexports/importhistory.aspx");
                }

                if (user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.ImportTemplates, true))
                {
                    this.Master.AddMenuItem(
                        "import2",
                        48,
                        "Import Templates",
                        "Manage import template mappings.",
                        "shared/importsexports/importTemplates.aspx");
                }

                if (user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.ESRTrustDetails, true))
                {
                    this.Master.AddMenuItem(
                        "table2_selection_row",
                        48,
                        "NHS Trust Details",
                        "Add/Edit NHS Trust details and ESR Inbound file details.",
                        "expenses/nhs/trusts.aspx");
                }
            }
        }
    }

    #endregion
}