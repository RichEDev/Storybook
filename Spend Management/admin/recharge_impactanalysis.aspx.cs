using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Services;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using SpendManagementLibrary;
using Spend_Management;

public partial class admin_recharge_impactanalysis : System.Web.UI.Page
{
    //private cRechargeClient Client
    //{
    //    get { return (cRechargeClient)ViewState["IAClient"]; }
    //    set { ViewState["IAClient"] = value; }
    //}

    protected void Page_Load(object sender, EventArgs e)
    {
    //    if (!this.IsPostBack)
    //    {
    //        CurrentUser curUser = cMisc.GetCurrentUser();
    //        cAccountSubAccounts subaccs = new cAccountSubAccounts(curUser.AccountID);
    //        cAccountProperties properties = curUser.CurrentSubAccountId >= 0 ? subaccs.getSubAccountById(curUser.CurrentSubAccountId).SubAccountProperties : subaccs.getFirstSubAccount().SubAccountProperties;

    //        //Master.isSubFolder = true;
    //        Title = "Recharge Impact Analysis";
    //        //Master.title = Title;

    //        cRechargeSettings rscoll = new cRechargeSettings(curUser.AccountID, curUser.CurrentSubAccountId, cAccounts.getConnectionString(curUser.AccountID));
    //        cRechargeSetting rs = rscoll.getSettings;
			
    //        lblRechargeClient.Text = rs.ReferenceAs;
    //        lblEndDate.Text = "Support End Date";

    //        //Page.SetFocus(txtRechargeClient);
    //    }
    }

    //private string GetAnalysisSQL(bool asSummary)
    //{
    //    System.Text.StringBuilder sql = new System.Text.StringBuilder();

    //    sql.Append("SELECT ");

    //    if (asSummary)
    //    {
    //        sql.Append("codes_contractcategory.[Category Description] AS [Category], ");
    //        sql.Append("COUNT(recharge_associations.[Contract-Product Id]) AS [Item Count], ");
    //        sql.Append("ROUND(SUM(dbo.CalcRechargeValue(getdate(),recharge_associations.[Recharge Id])),2) AS [Recharge Value] ");
    //    }
    //    else
    //    {
    //        sql.Append("recharge_associations.[Contract-Product Id] ");
    //    }

    //    sql.Append("FROM recharge_associations ");
    //    if (asSummary)
    //    {
    //        sql.Append("INNER JOIN contract_productdetails ON contract_productdetails.[Contract-Product Id] = recharge_associations.[Contract-Product Id] ");
    //        sql.Append("INNER JOIN contract_details ON contract_details.[Contract Id] = contract_productdetails.[Contract Id] ");
    //        sql.Append("INNER JOIN codes_contractcategory ON codes_contractcategory.[Category Id] = contract_details.[Category Id] ");
    //    }
    //    sql.Append("WHERE recharge_associations.[Recharge Entity Id] = @Client_Id ");
    //    if (asSummary)
    //    {
    //        sql.Append("GROUP BY [Category Description]");
    //    }

    //    return sql.ToString();
    //}


    protected void cmdOK_Click(object sender, ImageClickEventArgs e)
    {
    //    cFWDBConnection db = new cFWDBConnection();
    //    CurrentUser curUser = cMisc.GetCurrentUser();
    //    cAccountSubAccounts subaccs = new cAccountSubAccounts(curUser.AccountID);
    //    cAccountProperties properties = curUser.CurrentSubAccountId >= 0 ? subaccs.getSubAccountById(curUser.CurrentSubAccountId).SubAccountProperties : subaccs.getFirstSubAccount().SubAccountProperties;
    //    cFWSettings fws = cMigration.ConvertToFWSettings(curUser.Account, subaccs.getSubAccountsCollection(), curUser.CurrentSubAccountId);

    //    cRechargeClientList rclients = new cRechargeClientList(curUser.AccountID, curUser.CurrentSubAccountId, cAccounts.getConnectionString(curUser.AccountID));
    //    cRechargeClient rclient = rclients.FindClientByName(txtRechargeClient.Text.Trim());

    //    if (rclient != null)
    //    {
    //        panelResults.Visible = true;

    //        db.DBOpen(fws, false);

    //        db.AddDBParam("Client_Id", rclient.EntityId, true);

    //        db.RunSQL(GetAnalysisSQL(true),  db.glDBWorkA, false, "", false);

    //        igResultsGrid.DataSource = db.glDBWorkA;
    //        igResultsGrid.DataBind();

    //        db.DBClose();
    //    }
    }

    protected void igResultsGrid_InitializeLayout(object sender, Infragistics.WebUI.UltraWebGrid.LayoutEventArgs e)
    {
    //    e.Layout.Bands[0].Columns.FromKey("Category").Width = Unit.Pixel(200);
    //    e.Layout.Bands[0].Columns.FromKey("Recharge Value").Width = Unit.Pixel(100);
    //    e.Layout.Bands[0].Columns.FromKey("Item Count").Width = Unit.Pixel(100);

    //    e.Layout.Bands[0].Columns.FromKey("Recharge Value").CellStyle.HorizontalAlign = HorizontalAlign.Right;
    //    e.Layout.Bands[0].Columns.FromKey("Item Count").CellStyle.HorizontalAlign = HorizontalAlign.Center;
    //    e.Layout.ViewType = Infragistics.WebUI.UltraWebGrid.ViewType.Flat;
    }

    protected void igResultsGrid_InitializeRow(object sender, Infragistics.WebUI.UltraWebGrid.RowEventArgs e)
    {

    }

    protected void cmdCancel_Click(object sender, ImageClickEventArgs e)
    {
    //    Client = null;
    //    panelResults.Visible = false;
    }

    protected void cmdClose_Click(object sender, ImageClickEventArgs e)
    {
    //    Response.Redirect("../MenuMain.aspx?menusection=management", true);
    }

    protected void cmdUpdate_Click(object sender, ImageClickEventArgs e)
    {
    //    CurrentUser curUser = cMisc.GetCurrentUser();
    //    cAccountSubAccounts subaccs = new cAccountSubAccounts(curUser.AccountID);
    //    DateTime sed = DateTime.Parse(txtEndDate.Text);
    //    cAccountProperties accProperties;
    //    if (curUser.CurrentSubAccountId >= 0)
    //    {
    //        accProperties = subaccs.getSubAccountById(curUser.CurrentSubAccountId).SubAccountProperties;
    //    }
    //    else
    //    {
    //        accProperties = subaccs.getFirstSubAccount().SubAccountProperties;
    //    }
    //    cRechargeSettings rscoll = new cRechargeSettings(curUser.AccountID, curUser.CurrentSubAccountId, cAccounts.getConnectionString(curUser.AccountID));
    //    cRechargeSetting rs = rscoll.getSettings;

    //    cRechargeClientList rclients = new cRechargeClientList(curUser.AccountID, curUser.CurrentSubAccountId, cAccounts.getConnectionString(curUser.AccountID));
    //    cRechargeClient rclient = rclients.FindClientByName(txtRechargeClient.Text.Trim());
        
    //    try
    //    {
    //        cAuditLog ALog = new cAuditLog(curUser.AccountID, curUser.EmployeeID);
    //        //ALog.editRecord(rclient.EntityId, "End Date", SpendManagementElement.RechargeClients,, "Recharge Client Termination Success", "", rclient.ClientName);
            
    //        // RUN IMPACT ANALYSIS UPDATE AS A BACKGROUND TASK
    //        IAsyncResult syncres;
    //        cBackgroundTasks bt = new cBackgroundTasks(curUser);

    //        cBackgroundTasks.TerminateClient tc = new cBackgroundTasks.TerminateClient(bt.SetClientTermination);
    //        syncres = tc.BeginInvoke(rclient.EntityId, sed, new AsyncCallback(TCCallBackMethod), tc);
            
    //        lblStatus.Text = rs.ReferenceAs + " closure action started as a background task.";
    //    }
    //    catch 
    //    {
    //        lblStatus.Text = "A problem has occurred trying to perform action requested.";
    //        cAuditLog ALog = new cAuditLog(curUser.AccountID, curUser.EmployeeID);
    //        //ALog.editRecord("Recharge Clients", "End Date", "Recharge Client Termination Failed", "", rclient.ClientName);
    //    }

    //    panelResults.Visible = false;
    //    txtRechargeClient.Text = "";
    }

    //private void TCCallBackMethod(IAsyncResult res)
    //{
    //    cBackgroundTasks.TerminateClient tc = (cBackgroundTasks.TerminateClient)res.AsyncState;

    //    bool success = tc.EndInvoke(res);

    //    CurrentUser curUser = cMisc.GetCurrentUser();

    //    cAuditLog ALog = new cAuditLog(curUser.AccountID, curUser.EmployeeID);
    //    //ALog.editRecord("Recharge Client Termination","","Archive Client", "", success ? "Action Complete" : "Action Failed");
    //}
}
