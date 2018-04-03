#region Using Directives

using System;
using System.Collections.Generic;
using System.Web.UI;

using Spend_Management;
using SpendManagementLibrary;

using Spend_Management.shared.code.GreenLight;

#endregion

/// <summary>
/// The usermanagementmenu.
/// </summary>
public partial class usermanagementmenu : Page
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
            this.Title = "User Management";
            this.Master.Title = this.Title;

            var user = cMisc.GetCurrentUser();
            var clsModules = new cModules();
            var module = clsModules.GetModuleByID((int)user.CurrentActiveModule);
            var usingExpenses = user.CurrentActiveModule == Modules.expenses;

            this.ViewState["accountid"] = user.AccountID;

            if (user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.AccessRoles, true))
            {
                this.Master.AddMenuItem(
                    "data_lock",
                    48,
                    "Access Roles",
                    string.Format("Add, edit or delete Access Roles. An employee’s Access Role determines the areas within {0} which can be accessed and the data that can be viewed through Reports. Build a finer security policy by creating Access Roles limiting access to specific functions.", module.BrandNameHTML),
                    "shared/admin/accessRoles.aspx");
            }

            if (usingExpenses && user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.ItemRoles, true))
            {
                this.Master.AddMenuItem(
                    "data_preferences",
                    48,
                    "Item Roles",
                    "Add, edit or delete Item Roles. An employee’s Item Role determines the expenses items they are authorised to claim for. Build multiple Item Role types and add to an employee record when required, such as relocation expenses, to enable temporary authorisation of these items.",
                    "expenses/admin/itemroles.aspx");
            }

            if (usingExpenses && user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.SignOffGroups, true))
            {
                this.Master.AddMenuItem(
                    "branch",
                    48,
                    "Signoff Groups",
                    "A Group determines the stages a claim has to go through in order to be approved. Add, edit or delete groups. Modify the individual stages of an existing group.",
                    "expenses/admin/SignoffGroups.aspx");
            }

            if (user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.Employees, true))
            {
                this.Master.AddMenuItem(
                    "user",
                    48,
                    "Employees",
                    "Add, edit or delete employees. Archive or un-archive employees. Reset a password for an employee.",
                    "shared/admin/selectemployee.aspx");
            }

            if (user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.Teams, true))
            {
                this.Master.AddMenuItem(
                    "users2",
                    48,
                    "Teams",
                    "A team is a group of related employees. Add, edit or delete a team. Modify the individual members of a team. A team can be linked to a particular stage in a group.",
                    "~/shared/admin/adminteams.aspx");
            }

            if (user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.BudgetHolders, true))
            {

                var menuText = usingExpenses
                               ? "Create a budget and assign an employee assigned to be responsible for this budget. A budget holder can be linked to a particular stage in a group."
                               : "Create a budget and assign an employee assigned to be responsible for this budget. A budget holder can be linked to a particular stage in a workflow.";

                this.Master.AddMenuItem(
                    "hand_money",
                    48,
                    "Budget Holders",
                    menuText,
                    "~/shared/admin/adminbudget.aspx");
            }

            if (user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.Audiences, true))
            {
                this.Master.AddMenuItem(
                    "data_find",
                    48,
                    "Audiences",
                    "Add, edit or delete audiences. Users can be grouped into audiences, which are then attached to entities to make them invisible to anyone outside of those audiences.",
                    "shared/admin/adminAudiences.aspx");
            } 
            if (user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.ApprovalMatrix, true))
            {
                this.Master.AddMenuItem(
                    "table_preferences",
                    48,
                    "Approval Matrices",
                    "An approval matrix is a graduated series of approvers that can approve increasing claim values for an individual signoff group stage. When a claim reaches an approval matrix stage, the approvers that are capable of signing off that claim value will be used.",
                    "shared/admin/ApprovalMatrices.aspx");
            }

            if (user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.AuthoriserLevel, true))
            {
                this.Master.AddMenuItem(
                    "users3_preferences",
                    48,
                    "Authoriser Levels",
                    "Create Authoriser Levels to control the amount that a user is permitted to approve per employee, each month.",
                    "shared/admin/AuthoriserLevel.aspx");
            }
            this.Master.AddCustomEntityViewMenuIcons(user, 6);
        }
    }
    #endregion
}