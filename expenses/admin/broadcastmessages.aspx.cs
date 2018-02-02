#region Using Directives

using System;
using System.Collections.Generic;
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

            var clsmessages = new cBroadcastMessages(user.AccountID);
            string[] gridData = clsmessages.CreateGrid(user.AccountID, user.EmployeeID);
            litgrid.Text = gridData[1];

            // set the sel.grid javascript variables
            Page.ClientScript.RegisterStartupScript(this.GetType(), "BroadcastMessagesGridVars", cGridNew.generateJS_init("CatsGridVars", new List<string>() { gridData[0] }, user.CurrentActiveModule), true);
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

    #endregion
}