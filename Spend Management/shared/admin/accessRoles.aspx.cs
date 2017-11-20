using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SpendManagementLibrary;
using System.Text;

namespace Spend_Management
{
    /// <summary>
    /// Access roles summary screen
    /// </summary>
    public partial class accessRoles : System.Web.UI.Page
    {
        /// <summary>
        /// Page_Load
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();
            currentUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.AccessRoles, false, true);
            pnlAddNewAccessRole.Visible = currentUser.CheckAccessRole(AccessRoleType.Add, SpendManagementElement.AccessRoles, false);
            Title = "Access Roles";
            Master.title = Title;

            switch (currentUser.CurrentActiveModule)
            {
                case Modules.contracts:
                    Master.helpid = 1149;
                    break;
                default:
                    Master.helpid = 1054;
                    break;
            }

            cAccessRoles clsAccessRoles = new cAccessRoles(currentUser.AccountID, cAccounts.getConnectionString(currentUser.AccountID));

            cTables clsTables = new cTables(currentUser.AccountID);
            cFields clsFields = new cFields(currentUser.AccountID);

            cNewGridColumn gridColumn;

            List<cNewGridColumn> lstColumns = new List<cNewGridColumn>();

            gridColumn = new cFieldColumn(clsFields.GetFieldByID(new Guid("bdce49c3-eaf8-4070-91c6-959da3827db6")));
            lstColumns.Add(gridColumn);
            gridColumn = new cFieldColumn(clsFields.GetFieldByID(new Guid("735a4159-090b-420d-80c4-57987422380c")));
            lstColumns.Add(gridColumn);
            gridColumn = new cFieldColumn(clsFields.GetFieldByID(new Guid("0ed6481d-e3ff-49a1-b95f-f8a0ca0951f4")));
            lstColumns.Add(gridColumn);



            cGridNew clsGridNew = new cGridNew(currentUser.AccountID, currentUser.EmployeeID, "accessRolesGrid", clsTables.GetTableByID(new Guid("12ded231-b220-4acb-a51d-896c52ff8979")), lstColumns);
            clsGridNew.KeyField = "roleID";
            clsGridNew.enabledeleting = currentUser.CheckAccessRole(AccessRoleType.Delete, SpendManagementElement.AccessRoles, true);
            clsGridNew.getColumnByID(new Guid("BDCE49C3-EAF8-4070-91C6-959DA3827DB6")).hidden = true;

            clsGridNew.deletelink = "javascript:deleteAccessRole({roleID});";

            clsGridNew.enableupdating = currentUser.CheckAccessRole(AccessRoleType.Edit, SpendManagementElement.AccessRoles, true);
            clsGridNew.editlink = "aeAccessRole.aspx?accessRoleID={roleID}";

            string[] gridData = clsGridNew.generateGrid();
            litAccessRoles.Text = gridData[1];

            // set the sel.grid javascript variables
            Page.ClientScript.RegisterStartupScript(this.GetType(), "AccessRoleGridVars", cGridNew.generateJS_init("AccessRoleGridVars", new List<string>() { gridData[0] }, currentUser.CurrentActiveModule), true);
        }

        /// <summary>
        /// Close button event function
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void cmdClose_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            CurrentUser user = cMisc.GetCurrentUser();
            switch (user.CurrentActiveModule)
            {
                case Modules.SmartDiligence:
                case Modules.SpendManagement:
                case Modules.contracts:
                    Response.Redirect("~/MenuMain.aspx?menusection=employee", true);
                    break;
                default:
                    Response.Redirect("~/usermanagementmenu.aspx", true);
                    break;
            }
        }
    }
}
