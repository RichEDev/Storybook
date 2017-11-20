#region Using Directives

using System;
using System.Collections.Generic;
using System.Web.UI;

using Spend_Management;
using SpendManagementLibrary;

using Spend_Management.shared.code.GreenLight;

#endregion

/// <summary>
/// The policymenu.
/// </summary>
public partial class policymenu : Page
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
            CurrentUser user = cMisc.GetCurrentUser();
            var clsModules = new cModules();
            var usingExpenses = user.CurrentActiveModule == Modules.expenses;
            cModule module = clsModules.GetModuleByID((int)user.CurrentActiveModule);
            this.Title = "Policy Information";
            this.Master.Title = this.Title;

            if (user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.CompanyPolicy, true))
            {
                this.Master.AddMenuItem(
                    "lock_view",
                    48,
                    "Company Policy",
                    "Upload an HTML, PDF or plain text version of your company policy.",
                    "admin/adminpolicy.aspx");
            }

            if (user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.BroadcastMessages, true))
            {
                this.Master.AddMenuItem(
                    "satellite_dish",
                    48,
                    "Broadcast Messages",
                    string.Format("Enter a message that all employees will see when they logon to {0}.", module.BrandNameHTML),
                    "admin/broadcastmessages.aspx");
            }

            if (usingExpenses)
            {
                if (user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.FlagsAndLimits, true))
                {
                    this.Master.AddMenuItem("signal_flag_red", 48, "Flag Management", "Flags can be configured to warn of exceptions to your policy and to prevent items which breach policy from being claimed.", "expenses/admin/flags.aspx");
                }
            }

            this.Master.AddCustomEntityViewMenuIcons(user, 5);
        }
    }

    #endregion
}