using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using SpendManagementLibrary;
using Spend_Management;

public partial class admin_recharge_paymentgeneration : System.Web.UI.Page
{
    //private enum SearchType
    //{
    //    Contract = 1,
    //    Supplier = 2
    //}

    protected void Page_Load(object sender, EventArgs e)
    {
    //    CurrentUser curUser = cMisc.GetCurrentUser();
    //    cAccountSubAccounts subaccs = new cAccountSubAccounts(curUser.AccountID);
    //    cAccountProperties accProperties;
    //    if (curUser.CurrentSubAccountId >= 0)
    //    {
    //        accProperties = subaccs.getSubAccountById(curUser.CurrentSubAccountId).SubAccountProperties;
    //    }
    //    else
    //    {
    //        accProperties = subaccs.getFirstSubAccount().SubAccountProperties;
    //    }

    //    lblSupplier.Text = accProperties.SupplierPrimaryTitle;

    //    Title = "Recharge Payment Generation";
    //    Master.PageSubTitle = Title;
    }

    protected void cmdClose_Click(object sender, ImageClickEventArgs e)
    {
    //    Session["genResList"] = null;
    //    Response.Redirect("~/MenuMain.aspx?menusection=recharge", true);
    }

    protected void cmdSupplierSearch_Click(object sender, ImageClickEventArgs e)
    {
    //    CurrentUser curUser = cMisc.GetCurrentUser();
    //    resultPanel.Visible = true;
    //    searchResults.Controls.Add(GetContracts(curUser, txtSupplier.Text, SearchType.Supplier));
    }

    protected void cmdContactSearch_Click(object sender, ImageClickEventArgs e)
    {
    //    CurrentUser curUser = cMisc.GetCurrentUser();
    //    resultPanel.Visible = true;
    //    searchResults.Controls.Add(GetContracts(curUser, txtContract.Text, SearchType.Contract));
    }

    //private Table GetContracts(CurrentUser curUser, string searchTxt, SearchType searchType)
    //{
    //    cmdCancel.Visible = true;
		
    //    cFWDBConnection db = new cFWDBConnection();
    //    cAccountSubAccounts subaccs = new cAccountSubAccounts(curUser.AccountID);
    //    cFWSettings fws = cMigration.ConvertToFWSettings(curUser.Account, subaccs.getSubAccountsCollection(), curUser.CurrentSubAccountId);

    //    StringBuilder sql = new StringBuilder();
    //    sql.Append("SELECT [ContractId], supplier_details.supplierid, ISNULL([ContractNumber],'') AS [ContractNumber],");
    //    sql.Append("[ContractDescription], supplier_details.suppliername, ");
    //    sql.Append("dbo.GetContractProductCount([ContractId]) AS [ConProdCount] ");
    //    sql.Append("FROM contract_details ");
    //    sql.Append("INNER JOIN supplier_details ON contract_details.[supplierId] = supplier_details.supplierid ");
    //    sql.Append("WHERE dbo.CheckContractAccess(@userId, [ContractId]) > 0");
    //    switch (searchType)
    //    {
    //        case SearchType.Contract:
    //            sql.Append("AND [ContractDescription] LIKE '%' + @searchStr + '%' "); // OR [] LIKE @searchStr + '%'
    //            break;
    //        case SearchType.Supplier:
    //            sql.Append("AND suppliername LIKE '%' + @searchStr + '%' "); // OR [] LIKE @searchStr + '%'
    //            break;
    //        default:
    //            break;
    //    }
        
    //    db.DBOpen(fws, false);
    //    db.AddDBParam("userId", curUser.EmployeeID, true);
    //    db.AddDBParam("searchStr", searchTxt, false);

    //    Table resTable = new Table();
    //    resTable.CssClass = "datatbl";
    //    TableHeaderRow headerRow = new TableHeaderRow();
    //    TableHeaderCell headerCell = new TableHeaderCell();
    //    headerCell.Text = "Select";
    //    headerRow.Cells.Add(headerCell);
    //    headerCell = new TableHeaderCell();
    //    headerCell.Text = "Contract Number";
    //    headerCell.Width = Unit.Pixel(100);
    //    headerRow.Cells.Add(headerCell);
    //    headerCell = new TableHeaderCell();
    //    headerCell.Text = "Contract Description";
    //    headerCell.Width = Unit.Pixel(200);
    //    headerRow.Cells.Add(headerCell);
    //    headerCell = new TableHeaderCell();
    //    headerCell.Text = lblSupplier.Text;
    //    headerCell.Width = Unit.Pixel(100);
    //    headerRow.Cells.Add(headerCell);
    //    headerCell = new TableHeaderCell();
    //    headerCell.Text = "Contract Product Count";
    //    headerRow.Cells.Add(headerCell);
    //    resTable.Rows.Add(headerRow);

    //    TableCell tcell;
    //    TableRow trow;
    //    Dictionary<int, int> resList = new Dictionary<int, int>();

    //    bool rowalt = false;
    //    string rowClass;
    //    bool hasdata = false;

    //    System.Data.SqlClient.SqlDataReader reader;
    //    reader = db.GetReader(sql.ToString());
    //    while (reader.Read())
    //    {
    //        int conId = reader.GetInt32(reader.GetOrdinal("ContractId"));
    //        int suppId = reader.GetInt32(reader.GetOrdinal("supplierId"));
    //        string conNum = reader.GetString(reader.GetOrdinal("ContractNumber"));
    //        string conDesc = reader.GetString(reader.GetOrdinal("ContractDescription"));
    //        string suppName = reader.GetString(reader.GetOrdinal("suppliername"));
    //        int countVal = reader.GetInt32(reader.GetOrdinal("ConProdCount"));

    //        rowalt = (rowalt ^ true);
    //        if (rowalt)
    //        {
    //            rowClass = "row1";
    //        }
    //        else
    //        {
    //            rowClass = "row2";
    //        }

    //        trow = new TableRow();
    //        trow.CssClass = rowClass;
    //        tcell = new TableCell();
    //        tcell.CssClass = rowClass;
    //        tcell.HorizontalAlign = HorizontalAlign.Center;
    //        CheckBox chk = new CheckBox();
    //        chk.ID = "chk" + conId.ToString();
    //        tcell.Controls.Add(chk);
    //        trow.Cells.Add(tcell);
    //        tcell = new TableCell();
    //        tcell.CssClass = rowClass;
    //        tcell.Text = conNum;
    //        trow.Cells.Add(tcell);
    //        tcell = new TableCell();
    //        tcell.CssClass = rowClass;
    //        tcell.Text = conDesc;
    //        trow.Cells.Add(tcell);
    //        tcell = new TableCell();
    //        tcell.CssClass = rowClass;
    //        tcell.Text = suppName;
    //        trow.Cells.Add(tcell);
    //        tcell = new TableCell();
    //        tcell.CssClass = rowClass;
    //        tcell.HorizontalAlign = HorizontalAlign.Center;
    //        tcell.Text = countVal.ToString();
    //        trow.Cells.Add(tcell);

    //        resTable.Rows.Add(trow);
    //        resList.Add(conId, 0);
    //        hasdata = true;
    //    }

    //    reader.Close();
    //    db.DBClose();

    //    if (!hasdata)
    //    {
    //        trow = new TableRow();
    //        tcell = new TableCell();
    //        tcell.ColumnSpan = 5;
    //        tcell.HorizontalAlign = HorizontalAlign.Center;
    //        tcell.Text = "No matching contracts returned by search";
    //        trow.Cells.Add(tcell);
    //        resTable.Rows.Add(trow);

    //        cmdGenerate.Visible = false;
    //    }
    //    else
    //    {
    //        cmdGenerate.Visible = true;
    //    }
    //    Session["genResList"] = resList;
    //    return resTable;
    //}

    protected void cmdCancel_Click(object sender, ImageClickEventArgs e)
    {
    //    resultPanel.Visible = false;
    //    cmdCancel.Visible = false;
    //    cmdGenerate.Visible = false;
    //    Session["genResList"] = null;
    }

    protected void cmdGenerate_Click(object sender, ImageClickEventArgs e)
    {
    //    CurrentUser curUser = cMisc.GetCurrentUser();
    //    cAccountSubAccounts subaccs = new cAccountSubAccounts(curUser.AccountID);
    //    cFWSettings fws = cMigration.ConvertToFWSettings(curUser.Account, subaccs.getSubAccountsCollection(), curUser.CurrentSubAccountId);

    //    cAccountProperties accProperties;
    //    if (curUser.CurrentSubAccountId >= 0)
    //    {
    //        accProperties = subaccs.getSubAccountById(curUser.CurrentSubAccountId).SubAccountProperties;
    //    }
    //    else
    //    {
    //        accProperties = subaccs.getFirstSubAccount().SubAccountProperties;
    //    }

    //    Dictionary<int, int> resList = (Dictionary<int, int>)Session["genResList"];

    //    foreach (KeyValuePair<int, int> i in resList)
    //    {
    //        int conId = (int)i.Value;

    //        CheckBox chk = (CheckBox)searchResults.FindControl("chk" + conId.ToString());
    //        if (chk != null)
    //        {
    //            if (chk.Checked)
    //            {
    //                // generate payments for this contract
    //                DateTime from = DateTime.Parse(dateFrom.Text.Trim());
    //                DateTime to = DateTime.Parse(dateTo.Text.Trim());

    //                cRechargeSettings rscoll = new cRechargeSettings(curUser.AccountID, curUser.CurrentSubAccountId, cAccounts.getConnectionString(curUser.AccountID));
    //                cRechargeSetting rs = rscoll.getSettings;

    //                int rcPeriodMths = 0;

    //                switch (rs.RechargePeriod)
    //                {
    //                    case 0: // monthly
    //                        rcPeriodMths = 1;
    //                        break;
    //                    case 1: // quarterly
    //                        rcPeriodMths = 3;
    //                        break;
    //                    case 2: // 6 monthly
    //                        rcPeriodMths = 6;
    //                        break;
    //                    default:
    //                        break;
    //                }

    //                DateTime curPeriodDate;
    //                StringBuilder sql = new StringBuilder();
    //                cEmployees employees = new cEmployees(fws.MetabaseCustomerId);
    //                cRechargeCollection rItems = new cRechargeCollection(curUser.AccountID, curUser.CurrentSubAccountId, curUser.EmployeeID, conId, cAccounts.getConnectionString(curUser.AccountID), accProperties);
    //                cBackgroundTasks btasks = new cBackgroundTasks(curUser);

    //                curPeriodDate = DateTime.Parse("01/" + from.Month.ToString() + "/" + from.Year.ToString() + " 00:00:00");

    //                if (rItems.Count > 0)
    //                {
    //                    Cache["RCG_" + conId.ToString()] = 1;

    //                    IAsyncResult syncres;
    //                    cBackgroundTasks.GeneratePaymentsForContract rg = new cBackgroundTasks.GeneratePaymentsForContract(btasks.GenerateRechargePaymentsForContract);
    //                    syncres = rg.BeginInvoke(conId, curPeriodDate, to, rcPeriodMths, new AsyncCallback(CallBackMethod), rg);
    //                }
    //            }
    //        }
    //    }
    //    litMessage.Text = "<img alt=\"Alert!\" src=\".././images/information.gif\" />&nbsp;Payment Generation for selected contracts successfully started as as a background task";

    //    resultPanel.Visible = false;
    //    cmdCancel.Visible = false;
    //    cmdGenerate.Visible = false;
    }

    //private void CallBackMethod(IAsyncResult result)
    //{
    //    cBackgroundTasks.GeneratePaymentsForContract rg = (cBackgroundTasks.GeneratePaymentsForContract)result.AsyncState;
        
    //    int conId = rg.EndInvoke(result);

    //    if (Cache["RCG_" + conId.ToString()] != null)
    //    {
    //        Cache.Remove("RCG_" + conId.ToString());
    //    }
    //}
}
