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

public partial class admin_oneoffrechargecosts : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!this.IsPostBack)
        {
            Title = "Recharge One-off Charge";
            Master.PageSubTitle = Title;

            CurrentUser curUser = cMisc.GetCurrentUser();
            curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.RechargeOneTimeCharges, false, true);

            cRechargeSettings rscoll = new cRechargeSettings(curUser.AccountID, curUser.CurrentSubAccountId, cAccounts.getConnectionString(curUser.AccountID));
            cRechargeSetting rs = rscoll.getSettings;

            lblClient.Text = rs.ReferenceAs;
            reqClient.ErrorMessage = rs.ReferenceAs + " field is mandatory";

            DateTime curDate = DateTime.Today;
            if (ViewState["OOCStartDate"] == null)
            {
                txtStartDate.Text = curDate.AddMonths(-1).ToShortDateString();
                ViewState["OOCStartDate"] = txtStartDate.Text;
            }
            else
            {
                txtStartDate.Text = (string)ViewState["OOCStartDate"];
            }
            if (ViewState["OOCEndDate"] == null)
            {
                txtEndDate.Text = curDate.AddMonths(1).ToShortDateString();
                ViewState["OOCEndDate"] = txtEndDate.Text;
            }
            else
            {
                txtEndDate.Text = (string)ViewState["OOCEndDate"];
            }

            Page.SetFocus(txtContract);
        }

        GetOOCTable((string)ViewState["OOCStartDate"], (string)ViewState["OOCEndDate"]);
    }

    protected void cmdCancel_Click(object sender, ImageClickEventArgs e)
    {
        if (ViewState["OOCReturn"] == null)
        {
            Response.Redirect("../MenuMain.aspx?menusection=management", true);
        }
        else
        {
            Response.Redirect("../ContractRechargeBreakdown.aspx?id=" + (string)Session["ActiveContract"], true);
        }
    }

    protected void cmdUpdate_Click(object sender, ImageClickEventArgs e)
    {
        CurrentUser curUser = cMisc.GetCurrentUser();
        cAccountSubAccounts subaccs = new cAccountSubAccounts(curUser.AccountID);

        cFWSettings fws = cMigration.ConvertToFWSettings(curUser.Account, subaccs.getSubAccountsCollection(), curUser.CurrentSubAccountId);

        string contractKey = txtContract.Text.Substring(0, txtContract.Text.IndexOf('~'));
        string tmpKey = contractKey.Substring(contractKey.IndexOf('/') + 1, contractKey.LastIndexOf('/') - (contractKey.IndexOf('/') + 1));
        int contractId = int.Parse(tmpKey);
        int supplierId = int.Parse(contractKey.Substring(contractKey.LastIndexOf('/') + 1).Trim());
        int entityId = 0;

        cFWDBConnection db = new cFWDBConnection();

        db.DBOpen(fws, false);

        cRechargeClientList clients = new cRechargeClientList(fws.MetabaseCustomerId, curUser.CurrentSubAccountId, fws.getConnectionString);
        cRechargeClient curclient = clients.FindClientByName(txtClient.Text);
        if (curclient.EntityId == -1)
        {
            lblMessage.Text = "ERROR! - Unable to find " + lblClient.Text;
            db.DBClose();
        }
        else
        {
            entityId = curclient.EntityId;

            try
            {
                db.SetFieldValue("ContractId", contractId, "N", true);
                db.SetFieldValue("ChargeDate", txtDate.Text, "D", false);
                db.SetFieldValue("RechargeEntityId", entityId, "N", false);
                db.SetFieldValue("ChargeAmount", double.Parse(txtCost.Text.Trim()), "N", false);
                db.FWDb("W", "contract_productdetails_oneoffcharge", "", "", "", "", "", "", "", "", "", "", "", "");

                cAuditRecord ARec = new cAuditRecord();
                ARec.Action = cFWAuditLog.AUDIT_ADD;
                ARec.ContractNumber = contractKey;
                ARec.DataElementDesc = "RECHARGE COST";
                ARec.ElementDesc = "ONE-OFF COST";
                ARec.PreVal = "";
                ARec.PostVal = txtCost.Text;
                cFWAuditLog ALog = new cFWAuditLog(fws, SpendManagementElement.RechargeOneTimeCharges, curUser.CurrentSubAccountId);
                ALog.AddAuditRec(ARec, true);
                ALog.CommitAuditLog(curUser.Employee, contractId);

                db.DBClose();

                lblMessage.Text = "Charge Added successfully";

                panelCostList.Controls.Clear();
                GetOOCTable((string)ViewState["OOCStartDate"], (string)ViewState["OOCEndDate"]);
            }
            catch (Exception ex)
            {
                lblMessage.Text = "An error occurred trying to save entry\n" + ex.Message;
            }
        }
    }

    private Image GetEditImg(int editid)
    {
        Image editimg = new Image();
        editimg.ImageUrl = "../icons/edit.gif";
        if (editid != 0)
        {
            editimg.Attributes.Add("onmouseover", "window.status='Edit charge';return true;");
            editimg.Attributes.Add("onmouseout", "window.status='Done';");
            editimg.Attributes.Add("onclick", "javascript:EditCharge(" + editid.ToString() + ");");
        }
        return editimg;
    }

    private Image GetDeleteImg(int deleteid)
    {
        Image deleteimg = new Image();
        deleteimg.ImageUrl = "../icons/delete2.gif";
        if (deleteid != 0)
        {
            deleteimg.Attributes.Add("onmouseover", "window.status='Delete this charge';return true;");
            deleteimg.Attributes.Add("onmouseout", "window.status='Done';");
            deleteimg.Attributes.Add("onclick", "javascript:DeleteCharge(" + deleteid.ToString() + ");");
        }
        return deleteimg;
    }

    private void GetOOCTable(string StartDate, string EndDate)
    {
		CurrentUser curUser = cMisc.GetCurrentUser();
		cAccountSubAccounts subaccs = new cAccountSubAccounts(curUser.AccountID);
		cFWSettings fws = cMigration.ConvertToFWSettings(curUser.Account, subaccs.getSubAccountsCollection(), curUser.CurrentSubAccountId);
		cAccountProperties accProperties;
        if (curUser.CurrentSubAccountId >= 0)
        {
            accProperties = subaccs.getSubAccountById(curUser.CurrentSubAccountId).SubAccountProperties;
        }
        else
        {
            accProperties = subaccs.getFirstSubAccount().SubAccountProperties;
        }

		cRechargeSettings rscoll = new cRechargeSettings(curUser.AccountID, curUser.CurrentSubAccountId, cAccounts.getConnectionString(curUser.AccountID));
		cRechargeSetting rs = rscoll.getSettings;

        Table OOCTable = new Table();
        OOCTable.CssClass = "datatbl";
        TableRow OOCRow;
        TableCell OOCCell;

        // define table header
        TableHeaderRow OOCHRow = new TableHeaderRow();
        TableHeaderCell OOCHCell;

        if(curUser.CheckAccessRole(AccessRoleType.Edit, SpendManagementElement.RechargeOneTimeCharges, false))
        {
            OOCHCell = new TableHeaderCell();
            OOCHCell.Controls.Add(GetEditImg(0));
            OOCHRow.Cells.Add(OOCHCell);
        }
		if (curUser.CheckAccessRole(AccessRoleType.Delete, SpendManagementElement.RechargeOneTimeCharges, false))
        {
            OOCHCell = new TableHeaderCell();
            OOCHCell.Controls.Add(GetDeleteImg(0));
            OOCHRow.Cells.Add(OOCHCell);
        }
        OOCHCell = new TableHeaderCell();
        OOCHCell.Text = "Supplier";
        OOCHRow.Cells.Add(OOCHCell);
        OOCHCell = new TableHeaderCell();
        OOCHCell.Text = "Contract";
        OOCHRow.Cells.Add(OOCHCell);
        OOCHCell = new TableHeaderCell();
        OOCHCell.Text = rs.ReferenceAs;
        OOCHRow.Cells.Add(OOCHCell);
        OOCHCell = new TableHeaderCell();
        OOCHCell.Text = "Date of Charge";
        OOCHRow.Cells.Add(OOCHCell);
        OOCHCell = new TableHeaderCell();
        OOCHCell.Text = "Charge";
        OOCHRow.Cells.Add(OOCHCell);
        OOCTable.Rows.Add(OOCHRow);

        System.Text.StringBuilder sql = new System.Text.StringBuilder();
        sql.Append("SELECT contract_productdetails_oneoffcharge.*, contract_details.[ContractCurrency], contract_details.[ContractDescription], supplier_details.suppliername FROM contract_productdetails_oneoffcharge ");
        sql.Append("INNER JOIN contract_details ON contract_productdetails_oneoffcharge.[ContractId] = contract_details.[ContractId] ");
        sql.Append("INNER JOIN supplier_details ON supplier_details.supplierid = contract_details.[supplierId] ");
        sql.Append("WHERE [ChargeDate] BETWEEN @startdate AND @enddate ORDER BY [ChargeDate] ASC");

        cFWDBConnection db = new cFWDBConnection();
        db.DBOpen(fws, false);
        db.AddDBParam("startdate", DateTime.Parse(txtStartDate.Text), true);
        db.AddDBParam("enddate", DateTime.Parse(txtEndDate.Text), false);
        db.RunSQL(sql.ToString(), db.glDBWorkA, false, "", false);

        cCurrencies currencies = new cCurrencies(curUser.AccountID, curUser.CurrentSubAccountId);
		cRechargeClientList clients = new cRechargeClientList(curUser.AccountID, curUser.CurrentSubAccountId, cAccounts.getConnectionString(curUser.AccountID));
        bool rowalt = false;
        string rowClass = "row1";

        if (db.glNumRowsReturned > 0)
        {
            foreach (DataRow drow in db.glDBWorkA.Tables[0].Rows)
            {
                rowalt = rowalt ^ true;
                if (rowalt)
                {
                    rowClass = "row1";
                }
                else
                {
                    rowClass = "row2";
                }
                OOCRow = new TableRow();
                OOCRow.CssClass = rowClass;
                if (curUser.CheckAccessRole(AccessRoleType.Edit, SpendManagementElement.RechargeOneTimeCharges,false))
                {
                    OOCCell = new TableCell();
                    OOCCell.CssClass = rowClass;
                    OOCCell.Controls.Add(GetEditImg((int)drow["Charge Id"]));
                    OOCRow.Cells.Add(OOCCell);
                }
				if (curUser.CheckAccessRole(AccessRoleType.Delete, SpendManagementElement.RechargeOneTimeCharges, false))
                {
                    OOCCell = new TableCell();
                    OOCCell.CssClass = rowClass;
                    OOCCell.Controls.Add(GetDeleteImg((int)drow["ChargeId"]));
                    OOCRow.Cells.Add(OOCCell);
                }
                OOCCell = new TableCell();
                OOCCell.CssClass = rowClass;
                OOCCell.Text = (string)drow["suppliername"];
                OOCRow.Cells.Add(OOCCell);
                OOCCell = new TableCell();
                OOCCell.CssClass = rowClass;
                OOCCell.Text = (string)drow["ContractDescription"];
                OOCRow.Cells.Add(OOCCell);
                OOCCell = new TableCell();
                OOCCell.CssClass = rowClass;
                cRechargeClient curclient = clients.GetClientById((int)drow["RechargeEntityId"]);
                OOCCell.Text = curclient.ClientName;
                OOCRow.Cells.Add(OOCCell);
                OOCCell = new TableCell();
                OOCCell.CssClass = rowClass;
                DateTime tmpDate = (DateTime)drow["ChargeDate"];
                OOCCell.Text = tmpDate.ToShortDateString();
                OOCRow.Cells.Add(OOCCell);
                OOCCell = new TableCell();
                OOCCell.CssClass = rowClass;
                OOCCell.Text = currencies.FormatCurrency((double)drow["ChargeAmount"], currencies.getCurrencyById((int)drow["ContractCurrency"]), false).ToString();
                OOCRow.Cells.Add(OOCCell);

                OOCTable.Rows.Add(OOCRow);
            }
        }
        else
        {
            OOCRow = new TableRow();
            OOCRow.CssClass = "row1";
            OOCCell = new TableCell();
            int cols = 5;
			if (curUser.CheckAccessRole(AccessRoleType.Edit, SpendManagementElement.RechargeOneTimeCharges, false)) { cols++; }
			if (curUser.CheckAccessRole(AccessRoleType.Delete, SpendManagementElement.RechargeOneTimeCharges, false)) { cols++; }
            OOCCell.ColumnSpan = cols;
            OOCCell.HorizontalAlign = HorizontalAlign.Center;
            OOCCell.Text = "No charges to display";
            OOCRow.Cells.Add(OOCCell);
            OOCTable.Rows.Add(OOCRow);
        }
        db.DBClose();

        panelCostList.Controls.Add(OOCTable);
    }

    protected void cmdRefresh_Click(object sender, ImageClickEventArgs e)
    {
        panelCostList.Controls.Clear();
        ViewState["OOCStartDate"] = txtStartDate.Text;
        ViewState["OOCEndDate"] = txtEndDate.Text;
        GetOOCTable((string)txtStartDate.Text, (string)txtEndDate.Text);
    }

    [WebMethod()]
    public static string DeleteOOC(int deleteid)
    {
        try
        {
            HttpApplication appinfo = (HttpApplication)HttpContext.Current.ApplicationInstance;
            cFWDBConnection db = new cFWDBConnection();
            CurrentUser curUser = cMisc.GetCurrentUser();
            cAccountSubAccounts subaccs = new cAccountSubAccounts(curUser.AccountID);
            cFWSettings fws = cMigration.ConvertToFWSettings(curUser.Account, subaccs.getSubAccountsCollection(), curUser.CurrentSubAccountId);

            cRechargeClientList clients = new cRechargeClientList(curUser.AccountID, curUser.CurrentSubAccountId, cAccounts.getConnectionString(curUser.AccountID));

            db.DBOpen(fws, false);
            db.FWDb("R2", "contract_productdetails_oneoffcharge", "ChargeId", deleteid, "", "", "", "", "", "", "", "", "", "");
            if (db.FWDb2Flag)
            {
                db.FWDb("R3", "contract_details", "ContractId", db.FWDbFindVal("ContractId", 2), "", "", "", "", "", "", "", "", "", "");
                cFWAuditLog ALog = new cFWAuditLog(fws, SpendManagementElement.RechargeOneTimeCharges, curUser.CurrentSubAccountId);
                cAuditRecord ARec = new cAuditRecord();
                ARec.Action = cFWAuditLog.AUDIT_DEL;
                ARec.DataElementDesc = "ONE OFF CHARGE";
                if (db.FWDb3Flag)
                {
                    ARec.ContractNumber = db.FWDbFindVal("Contract Key", 3);
                }
                ARec.ElementDesc = clients.GetClientById(int.Parse(db.FWDbFindVal("RechargeEntityId", 2))).ClientName;
                ALog.AddAuditRec(ARec, true);
                ALog.CommitAuditLog(curUser.Employee, deleteid);

                db.FWDb("D", "contract_productdetails_oneoffcharge", "ChargeId", deleteid, "", "", "", "", "", "", "", "", "", "");
            }

            db.DBClose();
            return "OK";
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }

    [WebMethod()]
    public static string EditOOC(int editid)
    {
        try
        {
            return "OK";
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }
}
