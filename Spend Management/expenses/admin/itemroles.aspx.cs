using System;
using System.Collections.Generic;
using SpendManagementLibrary;
using Spend_Management;

public partial class itemroles : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (this.IsPostBack == false)
        {
            this.Title = "Item Roles";
            
            CurrentUser user = cMisc.GetCurrentUser();
            user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.ItemRoles, true, true);
            string[] gridData = this.CreateGrid(user);
            this.litgrid.Text = gridData[1];

            // set the sel.grid javascript variables
            this.Page.ClientScript.RegisterStartupScript(this.GetType(), "ItemRolesGridVars", cGridNew.generateJS_init("ItemRolesGridVars", new List<string>() { gridData[0] }, user.CurrentActiveModule), true);
        }
    }

    /// <summary>
    /// Creates the item roles grid
    /// </summary>
    /// <param name="user">The instance of the user creating the grid</param>
    /// <returns>The html of the grid</returns>
    private string[] CreateGrid(CurrentUser user)
    {
        cTables clstables = new cTables(user.AccountID);
        cFields clsfields = new cFields(user.AccountID);
        List<cNewGridColumn> columns = new List<cNewGridColumn>
        {
            new cFieldColumn(clsfields.GetFieldByID(new Guid("F3016E05-1832-49D1-9D33-79ED893B4366"))), //roleid
            new cFieldColumn(clsfields.GetFieldByID(new Guid("54825039-9125-4705-B2D4-EB340D1D30DE"))), //rolename
            new cFieldColumn(clsfields.GetFieldByID(new Guid("DCC4C3E7-1ED8-40B9-94BC-F5C52897FD86"))) //description
        };
        
        cGridNew clsgrid = new cGridNew(user.AccountID, user.EmployeeID, "gridItemRoles", clstables.GetTableByID(new Guid("DB7D42FD-E1FA-4A42-84B4-E8B95C751BDA")), columns); // item_roles
        clsgrid.getColumnByName("itemroleid").hidden = true;
        clsgrid.KeyField = "itemroleid";
        clsgrid.enabledeleting = user.CheckAccessRole(AccessRoleType.Delete, SpendManagementElement.ItemRoles, true, false);
        clsgrid.deletelink = "javascript:SEL.ItemRoles.ItemRole.Delete({itemroleid});";
        clsgrid.enableupdating = user.CheckAccessRole(AccessRoleType.Edit, SpendManagementElement.ItemRoles, true, false);
        clsgrid.editlink = "aeitemrole.aspx?id={itemroleid}";
        return clsgrid.generateGrid();
    }    
}
