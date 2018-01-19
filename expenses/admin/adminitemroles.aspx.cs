using System;
using System.Web;
using System.Web.Services;
using System.Web.UI.WebControls;

using Infragistics.WebUI.UltraWebGrid;

using SpendManagementLibrary;
using SpendManagementLibrary.Employees;

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
        }
    }

    [WebMethod(EnableSession = true)]
    public static int deleteRole(int accountid, int itemroleid)
    {
        cItemRoles clsroles = new cItemRoles(accountid);
        return clsroles.deleteRole(itemroleid);
    }

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        gridroles.InitializeDataSource += new Infragistics.WebUI.UltraWebGrid.InitializeDataSourceEventHandler(gridroles_InitializeDataSource);
    }

    void gridroles_InitializeDataSource(object sender, Infragistics.WebUI.UltraWebGrid.UltraGridEventArgs e)
    {
        cItemRoles clsitemroles = new cItemRoles((int)ViewState["accountid"]);
        gridroles.DataSource = clsitemroles.getGrid();
    }
    protected void gridroles_InitializeLayout(object sender, Infragistics.WebUI.UltraWebGrid.LayoutEventArgs e)
    {
        #region Sorting
        cEmployees clsemployees = new cEmployees((int)ViewState["accountid"]);
        Employee reqemp = clsemployees.GetEmployeeById((int)ViewState["employeeid"]);
        cGridSort sortorder = reqemp.GetGridSortOrders().GetBy(Grid.ItemRoles);
        if (sortorder != null)
        {
            if (e.Layout.Bands[0].SortedColumns.Count == 0)
            {
                if (e.Layout.Bands[0].Columns.FromKey(sortorder.columnname) != null)
                {
                    e.Layout.Bands[0].Columns.FromKey(sortorder.columnname).SortIndicator = (SortIndicator)sortorder.sortorder;
                    e.Layout.Bands[0].SortedColumns.Add(sortorder.columnname);
                }
            }
        }
        #endregion
        e.Layout.Bands[0].Columns.FromKey("itemroleid").Hidden = true;
        e.Layout.Bands[0].Columns.FromKey("rolename").Header.Caption = "Role Name";
        e.Layout.Bands[0].Columns.FromKey("description").Header.Caption = "Description";
        if (e.Layout.Bands[0].Columns.FromKey("edit") == null)
        {
            e.Layout.Bands[0].Columns.Insert(0, new Infragistics.WebUI.UltraWebGrid.UltraGridColumn("delete", "<img alt=\"Delete\" src=\"../icons/delete2_blue.gif\">", Infragistics.WebUI.UltraWebGrid.ColumnType.HyperLink, ""));
            e.Layout.Bands[0].Columns.FromKey("delete").Width = Unit.Pixel(25);
            e.Layout.Bands[0].Columns.Insert(0, new Infragistics.WebUI.UltraWebGrid.UltraGridColumn("edit", "<img alt=\"Edit\" src=\"../icons/edit_blue.gif\">", Infragistics.WebUI.UltraWebGrid.ColumnType.HyperLink, ""));
            e.Layout.Bands[0].Columns.FromKey("edit").Width = Unit.Pixel(25);
        }
    }

    protected void gridroles_InitializeRow(object sender, Infragistics.WebUI.UltraWebGrid.RowEventArgs e)
    {
        e.Row.Cells.FromKey("edit").Value = "<a href=\"aeitemrole.aspx?action=2&itemroleid=" + e.Row.Cells.FromKey("itemroleid").Value + "\"><img alt=\"Edit\" src=\"../icons/edit.gif\"></a>";
        e.Row.Cells.FromKey("Delete").Value = "<a href=\"javascript:deleteRole(" + e.Row.Cells.FromKey("itemroleid").Value + ");\"><img alt=\"Delete\" src=\"../icons/delete2.gif\"></a>";
    }

    protected void gridroles_SortColumn(object sender, Infragistics.WebUI.UltraWebGrid.SortColumnEventArgs e)
    {
        UltraWebGrid grid = (UltraWebGrid)sender;
        byte direction = (byte)grid.Columns[e.ColumnNo].SortIndicator;
        CurrentUser currentUser = cMisc.GetCurrentUser();
        currentUser.Employee.GetGridSortOrders().Add(Grid.ItemRoles, grid.Columns[e.ColumnNo].Key, direction);
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
