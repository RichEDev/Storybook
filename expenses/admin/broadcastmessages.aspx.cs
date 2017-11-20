#region Using Directives

using System;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using Infragistics.WebUI.UltraWebGrid;
using Spend_Management;
using SpendManagementLibrary;

#endregion

/// <summary>
/// The admin_broadcastmessages.
/// </summary>
public partial class admin_broadcastmessages : Page
{
    #region Public Methods and Operators

    /// <summary>
    /// The delete broadcast.
    /// </summary>
    /// <param name="accountid">
    /// The accountid.
    /// </param>
    /// <param name="broadcastid">
    /// The broadcastid.
    /// </param>
    [WebMethod(EnableSession = true)]
    public static void deleteBroadcast(int accountid, int broadcastid)
    {
        var clsmessages = new cBroadcastMessages(accountid);
        clsmessages.deleteBroadcastMessage(broadcastid);
    }

    #endregion

    #region Methods

    /// <summary>
    /// The on init.
    /// </summary>
    /// <param name="e">
    /// The e.
    /// </param>
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        this.gridbroadcast.InitializeDataSource += this.gridbroadcast_InitializeDataSource;
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
        if (this.IsPostBack == false)
        {
            this.Title = "Broadcast Messages";
            this.Master.title = this.Title;
            this.Master.helpid = 1026;

            CurrentUser user = cMisc.GetCurrentUser();
            user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.BroadcastMessages, true, true);
            this.ViewState["accountid"] = user.AccountID;
            this.ViewState["employeeid"] = user.EmployeeID;
        }
    }

    /// <summary>
    /// Close button event function
    /// </summary>
    /// <param name="sender">
    /// </param>
    /// <param name="e">
    /// </param>
    protected void cmdClose_Click(object sender, ImageClickEventArgs e)
    {
        string previousUrl = (SiteMap.CurrentNode == null) ? "~/home.aspx" : SiteMap.CurrentNode.ParentNode.Url;

        this.Response.Redirect(previousUrl, true);
    }

    /// <summary>
    /// The gridbroadcast_ initialize layout.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void gridbroadcast_InitializeLayout(object sender, LayoutEventArgs e)
    {
        e.Layout.Bands[0].Columns.FromKey("title").Header.Caption = "Title";
        e.Layout.Bands[0].Columns.FromKey("startdate").Header.Caption = "Start Date";
        e.Layout.Bands[0].Columns.FromKey("enddate").Header.Caption = "End Date";
        e.Layout.Bands[0].Columns.FromKey("startdate").Format = "dd/MM/yyyy";
        e.Layout.Bands[0].Columns.FromKey("enddate").Format = "dd/MM/yyyy";
        e.Layout.Bands[0].Columns.FromKey("expirewhenread").Header.Caption = "Expire When Read";
        e.Layout.Bands[0].Columns.FromKey("location").Header.Caption = "Location";
        e.Layout.Bands[0].Columns.FromKey("location").Type = ColumnType.DropDownList;
        e.Layout.Bands[0].Columns.FromKey("location").ValueList.ValueListItems.Add((byte)1, "Home Page");
        e.Layout.Bands[0].Columns.FromKey("location").ValueList.ValueListItems.Add((byte)2, "Submit Claim");
        e.Layout.Bands[0].Columns.FromKey("broadcastid").Hidden = true;
        if (e.Layout.Bands[0].Columns.FromKey("edit") == null)
        {
            e.Layout.Bands[0].Columns.Insert(
                0, 
                new UltraGridColumn(
                    "delete", "<img alt=\"Delete\" src=\"../icons/delete2_blue.gif\">", ColumnType.HyperLink, string.Empty));
            e.Layout.Bands[0].Columns.FromKey("delete").Width = Unit.Pixel(15);
            e.Layout.Bands[0].Columns.Insert(
                0, 
                new UltraGridColumn(
                    "edit", "<img alt=\"Edit\" src=\"../icons/edit_blue.gif\">", ColumnType.HyperLink, string.Empty));
            e.Layout.Bands[0].Columns.FromKey("edit").Width = Unit.Pixel(15);
        }
    }

    /// <summary>
    /// The gridbroadcast_ initialize row.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void gridbroadcast_InitializeRow(object sender, RowEventArgs e)
    {
        e.Row.Cells.FromKey("edit").Value = "<a href=\"aebroadcastmessage.aspx?broadcastid="
                                            + e.Row.Cells.FromKey("broadcastid").Value
                                            + "\"><img alt=\"Edit\" src=\"../icons/edit.gif\"></a>";
        e.Row.Cells.FromKey("delete").Value = "<a href=\"javascript:deleteBroadcast("
                                              + e.Row.Cells.FromKey("broadcastid").Value
                                              + ");\"><img alt=\"Delete\" src=\"../icons/delete2.gif\"></a>";
    }

    /// <summary>
    /// The gridbroadcast_ initialize data source.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    private void gridbroadcast_InitializeDataSource(object sender, UltraGridEventArgs e)
    {
        CurrentUser user = cMisc.GetCurrentUser();
        var clsmessages = new cBroadcastMessages((int)this.ViewState["accountid"]);
        this.gridbroadcast.DataSource = clsmessages.getGrid(user.CurrentActiveModule);
    }

    #endregion
}