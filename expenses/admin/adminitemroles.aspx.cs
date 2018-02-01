using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Services;

using SpendManagementLibrary;

using Spend_Management;

public partial class admin_adminitemroles : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (IsPostBack == false)
        {
            Title = "Item Roles";
            Master.title = Title;
            Master.helpid = 1100;
            CurrentUser user = cMisc.GetCurrentUser();
            user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.ItemRoles, true, true);
            ViewState["accountid"] = user.AccountID;
            ViewState["employeeid"] = user.EmployeeID;

            string[] gridData = createGrid(user.AccountID, user.EmployeeID);
            litgrid.Text = gridData[1];

            // set the sel.grid javascript variables
            Page.ClientScript.RegisterStartupScript(this.GetType(), "ItemRolesGridVars", cGridNew.generateJS_init("ItemRolesGridVars", new List<string>() { gridData[0] }, user.CurrentActiveModule), true);
        }
    }

    private string[] createGrid(int accountid, int employeeid)
    {
        cTables clstables = new cTables(accountid);
        cFields clsfields = new cFields(accountid);
        List<cNewGridColumn> columns = new List<cNewGridColumn>();
        columns.Add(new cFieldColumn(clsfields.GetFieldByID(new Guid("F3016E05-1832-49D1-9D33-79ED893B4366"))));
        columns.Add(new cFieldColumn(clsfields.GetFieldByID(new Guid("54825039-9125-4705-B2D4-EB340D1D30DE"))));
        columns.Add(new cFieldColumn(clsfields.GetFieldByID(new Guid("DCC4C3E7-1ED8-40B9-94BC-F5C52897FD86"))));
        cGridNew clsgrid = new cGridNew(accountid, employeeid, "gridItemRoles", clstables.GetTableByID(new Guid("DB7D42FD-E1FA-4A42-84B4-E8B95C751BDA")), columns);
        clsgrid.getColumnByName("itemroleid").hidden = true;
        clsgrid.KeyField = "itemroleid";
        clsgrid.enabledeleting = true;
        clsgrid.deletelink = "javascript:deleteRole({itemroleid});";
        clsgrid.enableupdating = true;
        clsgrid.editlink = "aeitemrole.aspx?action=2&itemroleid={itemroleid}";
        return clsgrid.generateGrid();
    }

    [WebMethod(EnableSession = true)]
    public static int deleteRole(int accountid, int itemroleid)
    {
        cItemRoles clsroles = new cItemRoles(accountid);
        return clsroles.deleteRole(itemroleid);
    }

    /// <summary>
    /// Close button event function
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void cmdClose_Click(object sender, System.Web.UI.ImageClickEventArgs e)
    {
        string sPreviousURL = (SiteMap.CurrentNode == null) ? "~/home.aspx" : SiteMap.CurrentNode.ParentNode.Url;

        Response.Redirect(sPreviousURL, true);
    }
}
