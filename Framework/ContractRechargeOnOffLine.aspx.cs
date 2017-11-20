using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Spend_Management;
using SpendManagementLibrary;
using FWClasses;

public partial class ContractRechargeOnOffLine : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        bool skipGet = false;
        int contractId = int.Parse(Request.QueryString["cid"]);
        string csvCPList = Request.QueryString["cplist"];
        ViewState["oodateids"] = csvCPList;

        if (!this.IsPostBack)
        {
            CurrentUser curUser = cMisc.getCurrentUser(User.Identity.Name);
            Title = "On-Off Line Dates";
            Master.title = Title;

            SetHeadings(curUser);

            if(Session["OODStart"] == null)
            {
                TimeSpan back = new TimeSpan(-30, 0, 0, 0, 0);
                DateTime tmpStart = DateTime.Today.Add(back);
                txtDisplayStart.Text = tmpStart.ToShortDateString();
            }
            else
            {
                DateTime tmpStart = (DateTime)Session["OODStart"];
                txtDisplayStart.Text = tmpStart.ToShortDateString();
            }
            
            if (Session["OODEnd"] == null)
            {
                TimeSpan ahead = new TimeSpan(30, 0, 0, 0, 0);
                DateTime tmpFinish = DateTime.Today.Add(ahead);
                txtDisplayFinish.Text = tmpFinish.ToShortDateString();
            }
            else
            {
                DateTime tmpFinish = (DateTime)Session["OODEnd"];
                txtDisplayFinish.Text = tmpFinish.ToShortDateString();
            }

            string action = Request.QueryString["action"];
            switch (action)
            {
                case "delete":
                    int delid = int.Parse(Request.QueryString["sdid"]);
                    int delcpid = int.Parse(Request.QueryString["cpid"]);
                    DeleteServiceDate(delcpid, delid);
                    break;

                case "edit":
                    int editid = int.Parse(Request.QueryString["sdid"]);
                    int editcpid = int.Parse(Request.QueryString["cpid"]);
                    EditServiceDate(editcpid, editid);
                    skipGet = true;
                    break;

                default:
                    break;
            }
        }

        if (!skipGet)
        {
            GetDates(csvCPList);
        }
    }

    protected void cmdCancel_Click(object sender, ImageClickEventArgs e)
    {
        dateEntryPanel.Visible = false;
        curDatesPanel.Visible = true;

        GetDates((string)ViewState["oodateids"]);
    }

    protected void cmdUpdate_Click(object sender, ImageClickEventArgs e)
    {
        CurrentUser curUser = cMisc.getCurrentUser(User.Identity.Name);
        cAccountSubAccounts subaccs = new cAccountSubAccounts(curUser.AccountID);
        cFWSettings fws = cMigration.ConvertToFWSettings(curUser.Account, subaccs.getSubAccountsCollection(), curUser.CurrentSubAccountId);
        cAccountProperties fwparams = subaccs.getSubAccountById(curUser.CurrentSubAccountId).SubAccountProperties;
        cEmployees employees = new cEmployees(curUser.AccountID);
        cRechargeCollection rcoll = new cRechargeCollection(curUser.AccountID,curUser.CurrentSubAccountId, curUser.EmployeeID, (int)Session["ActiveContract"], cAccounts.getConnectionString(curUser.AccountID), fwparams);

        cFWDBConnection db = new cFWDBConnection();
        db.DBOpen(fws, false);

        if (Request.QueryString["action"] == "edit")
        {
            int contractId = int.Parse(Request.QueryString["cid"]);
            db.FWDb("R3", "contract_details", "ContractId", contractId, "", "", "", "", "", "", "", "", "", "");

            int SDid = int.Parse(Request.QueryString["sdid"]);
            string csvCPList = Request.QueryString["cplist"];
            string[] cpids = csvCPList.Split(',');

            for (int sdIdx = 0; sdIdx < cpids.Length; sdIdx++)
            {
                ArrayList arcpitems = rcoll.GetRechargeItemsByCPId(int.Parse(cpids[sdIdx]));
                for (int ritemIdx = 0; ritemIdx < arcpitems.Count; ritemIdx++)
                {
                    cRecharge ritem = (cRecharge)arcpitems[ritemIdx];
					SortedList<int,cRechargeServiceDate> sdates = ritem.ServiceDates;
                    cRechargeServiceDate sdate = sdates[SDid];

                    if (sdate != null)
                    {
                        bool firstchange = true;
                        cAuditRecord ARec = new cAuditRecord();
                        ARec.Action = cFWAuditLog.AUDIT_UPDATE;
                        ARec.DataElementDesc = "RECHARGE SERVICE DATE";

                        if (db.FWDb3Flag)
                        {
                            ARec.ContractNumber = db.FWDbFindVal("ContractKey", 3);
                        }
                        else
                        {
                            ARec.ContractNumber = "Unknown";
                        }

                        if (DateTime.Parse(txtOfflineFrom.Text) != sdate.OfflineFrom)
                        {
                            ARec.ElementDesc = "OFFLINE FROM DATE";
                            ARec.PostVal = txtOfflineFrom.Text;
                            ARec.PreVal = sdate.OfflineFrom.ToShortDateString();
                            cFWAuditLog ALog = new cFWAuditLog(fws, SpendManagementElement.RechargeAssociations, curUser.CurrentSubAccountId);
                            ALog.AddAuditRec( ARec, true);
                            ALog.CommitAuditLog(curUser.Employee, ritem.RechargeId);

                            db.SetFieldValue("OfflineFrom", txtOfflineFrom.Text, "D", firstchange);
                            firstchange = false;
                        }

                        if (DateTime.Parse(txtOnlineFrom.Text) != sdate.OnlineFrom)
                        {
                            ARec.ElementDesc = "ONLINE FROM DATE";
                            ARec.PostVal = txtOnlineFrom.Text;
                            ARec.PreVal = sdate.OnlineFrom.ToShortDateString();
                            cFWAuditLog ALog = new cFWAuditLog(fws, SpendManagementElement.RechargeAssociations, curUser.CurrentSubAccountId);
                            ALog.AddAuditRec( ARec, true);
                            ALog.CommitAuditLog(curUser.Employee, ritem.RechargeId);

                            db.SetFieldValue("OnlineFrom", txtOnlineFrom.Text, "D", firstchange);
                            firstchange = false;
                        }

                        if (!firstchange)
                        {
                            db.FWDb("A", "recharge_servicedates", "ServiceDateId", SDid, "", "", "", "", "", "", "", "", "", "");
                            cRechargeServiceDate newdates = new cRechargeServiceDate(SDid, ritem.ContractProductId, DateTime.Parse( txtOfflineFrom.Text),DateTime.Parse( txtOnlineFrom.Text));
							sdates[SDid] = newdates;
                            ritem.ServiceDates = sdates;
                            rcoll.UpdateRechargeItem(ritem);
                        }
                    }
                }
            }
        }
        else
        {
            // must be adding a new date
            int contractId = int.Parse(Request.QueryString["cid"]);
            string csvCPList = Request.QueryString["cplist"];
            string[] cid = csvCPList.Split(',');

            if (cid.Length > 0)
            {
                cAuditRecord ARec = new cAuditRecord();
                ARec.Action = cFWAuditLog.AUDIT_ADD;
                ARec.DataElementDesc = "RECHARGE SERVICE DATE";

                for (int i = 0; i < cid.Length; i++)
                {
                    ArrayList arcpitems = rcoll.GetRechargeItemsByCPId(int.Parse(cid[i]));
                    for (int cpitems = 0; cpitems < arcpitems.Count; cpitems++)
                    {
                        cRecharge ritem = (cRecharge)arcpitems[cpitems];
                        ARec.ElementDesc = ritem.RechargeEntityName;
                        ARec.PreVal = "";
                        ARec.PostVal = txtOfflineFrom.Text;
                        
                        db.SetFieldValue("OnlineFrom", txtOnlineFrom.Text, "D", true);
                        db.SetFieldValue("OfflineFrom", txtOfflineFrom.Text, "D", false);
                        db.SetFieldValue("RechargeId", ritem.RechargeId, "N", false);
                        db.FWDb("W", "recharge_servicedates", "", "", "", "", "", "", "", "", "", "", "", "");

                        ritem.ServiceDates.Add(db.glIdentity, new cRechargeServiceDate(db.glIdentity, ritem.RechargeId, DateTime.Parse(txtOfflineFrom.Text), DateTime.Parse(txtOnlineFrom.Text)));
                        rcoll.UpdateRechargeItem(ritem);

                        cFWAuditLog ALog = new cFWAuditLog(fws, SpendManagementElement.RechargeAssociations, curUser.CurrentSubAccountId);
                        ALog.AddAuditRec( ARec, true);
						ALog.CommitAuditLog(curUser.Employee, ritem.RechargeId);
                    }
                }
            }
        }

        dateEntryPanel.Visible = false;
        curDatesPanel.Visible = true;

        GetDates((string)ViewState["oodateids"]);

        return;
    }

    protected void cmdClose_Click(object sender, ImageClickEventArgs e)
    {
        Response.Redirect("~/ContractRechargeBreakdown.aspx?id=" + Session["ActiveContract"], true);
    }

    protected void lnkAddDates_Click(object sender, EventArgs e)
    {
        dateEntryPanel.Visible = true;
        curDatesPanel.Visible = false;
    }

    private void GetDates(string csvCPList)
    {
        oodData.Controls.Clear();
        CurrentUser curUser = cMisc.getCurrentUser(User.Identity.Name);
        cAccountSubAccounts subaccs = new cAccountSubAccounts(curUser.AccountID);
        cFWSettings fws = cMigration.ConvertToFWSettings(curUser.Account, subaccs.getSubAccountsCollection(), curUser.CurrentSubAccountId);
        cAccountProperties fwparams = subaccs.getSubAccountById(curUser.AccountID).SubAccountProperties;
        cEmployees employees = new cEmployees(curUser.AccountID);
        cRechargeSettings rscoll = new cRechargeSettings(curUser.AccountID, curUser.CurrentSubAccountId, cAccounts.getConnectionString(curUser.AccountID));
        cRechargeSetting rs = rscoll.getSettings;
        cRechargeCollection rcoll = new cRechargeCollection(curUser.AccountID, curUser.CurrentSubAccountId, curUser.EmployeeID, (int)Session["ActiveContract"], cAccounts.getConnectionString(curUser.AccountID), fwparams);
        Table OOD_Table = new Table();
        TableFooterRow THrow = new TableFooterRow();
        TableHeaderCell THcell;

        OOD_Table.CssClass = "datatbl";

        THcell = new TableHeaderCell();
        Image edimg = new Image();
        edimg.ImageUrl = "./icons/edit.gif";
        THcell.Controls.Add(edimg);
        THrow.Cells.Add(THcell);

        THcell = new TableHeaderCell();
        Image delimg = new Image();
        delimg.ImageUrl = "./icons/delete2.gif";
        THcell.Controls.Add(delimg);
        THrow.Cells.Add(THcell);

        THcell = new TableHeaderCell();
        THcell.Width = Unit.Pixel(150);
        THcell.Text = "Contract Product Name";
        THrow.Cells.Add(THcell);

        THcell = new TableHeaderCell();
        THcell.Width = Unit.Pixel(150);
        THcell.Text = rs.ReferenceAs + " Name";
        THrow.Cells.Add(THcell);

        THcell = new TableHeaderCell();
        THcell.Width = Unit.Pixel(100);
        THcell.Text = "Offline from date";
        THrow.Cells.Add(THcell);

        THcell = new TableHeaderCell();
        THcell.Width = Unit.Pixel(100);
        THcell.Text = "Online from date";
        THrow.Cells.Add(THcell);

        OOD_Table.Rows.Add(THrow);

        bool rowalt = false;
        string rowClass = "row1";

        string[] cid = csvCPList.Split(',');

        if (cid.Length > 0)
        {
            for (int i = 0; i < cid.Length; i++)
            {
                ArrayList arcpitems = rcoll.GetRechargeItemsByCPId(int.Parse(cid[i]));
                for (int cpitems = 0; cpitems < arcpitems.Count; cpitems++)
                {
                    cRecharge ritem = (cRecharge)arcpitems[cpitems];
					SortedList<int, cRechargeServiceDate> sdateitems = ritem.ServiceDates;

                    foreach (KeyValuePair<int, cRechargeServiceDate> dateitem in sdateitems)
                    {
                        cRechargeServiceDate rsdate = dateitem.Value;
                        TableRow OODrow = new TableRow();
                        rowalt = (rowalt ^ true);
                        if (rowalt)
                        {
                            rowClass = "row1";
                        }
                        else
                        {
                            rowClass = "row2";
                        }

                        TableCell tcell;
                        tcell = new TableCell();
                        tcell.CssClass = rowClass;
                        if (curUser.CheckAccessRole(AccessRoleType.Edit, SpendManagementElement.RechargeAssociations,false))
                        {
                            Literal litEdit = new Literal();
                            litEdit.Text = "<a onmouseover=\"window.status='Edit this service date';return true;\" onmouseout=\"window.status='Done';\" href=\"ContractRechargeOnOffLine.aspx?action=edit&sdid=" + dateitem.Key.ToString() + "&cpid=" + ritem.ContractProductId.ToString() + "&cid=" + Session["ActiveContract"] + "&cplist=" + ViewState["oodateids"] + "\"><img alt=\"Edit Date\" src=\"" + edimg.ImageUrl + "\" /></a>";
                            tcell.Controls.Add(litEdit);
                        }
                        OODrow.Cells.Add(tcell);

                        tcell = new TableCell();
                        tcell.CssClass = rowClass;
                        if (curUser.CheckAccessRole(AccessRoleType.Delete, SpendManagementElement.RechargeAssociations, false))
                        {
                            Literal litDelete = new Literal();
                            litDelete.Text = "<a onmouseover=\"window.status='Delete this service date';return true;\" onmouseout=\"window.status='Done';\" style=\"cursor: hand;\" onclick=\"javascript:if(confirm('Click OK to confirm deletion')){window.location.href='ContractRechargeOnOffLine.aspx?action=delete&cpid=" + ritem.ContractProductId.ToString() + "&sdid=" + dateitem.Key.ToString() + "&cid=" + Session["ActiveContract"] + "&cplist=" + ViewState["oodateids"] + "';}\"><img alt=\"Delete Date\" src=\"" + delimg.ImageUrl + "\" /></a>";
                            tcell.Controls.Add(litDelete);
                        }
                        OODrow.Cells.Add(tcell);

                        tcell = new TableCell();
                        tcell.CssClass = rowClass;
                        tcell.Text = ritem.ProductName;
                        OODrow.Cells.Add(tcell);

                        tcell = new TableCell();
                        tcell.CssClass = rowClass;
                        tcell.Text = ritem.RechargeEntityName;
                        OODrow.Cells.Add(tcell);

                        tcell = new TableCell();
                        tcell.CssClass = rowClass;
                        tcell.Text = rsdate.OfflineFrom.ToShortDateString();
                        OODrow.Cells.Add(tcell);

                        tcell = new TableCell();
                        tcell.CssClass = rowClass;
                        tcell.Text = rsdate.OnlineFrom.ToShortDateString();
                        OODrow.Cells.Add(tcell);

                        OOD_Table.Rows.Add(OODrow);
                    }
                }
            }
        }
        else
        {
            TableRow trow = new TableRow();
            TableCell tcell = new TableCell();
            tcell.CssClass = "row1";
            tcell.ColumnSpan = 6;
            tcell.HorizontalAlign = HorizontalAlign.Center;
            tcell.Text = "No Service Dates returned";
            trow.Cells.Add(tcell);
            OOD_Table.Rows.Add(trow);
        }

        oodData.Controls.Add(OOD_Table);
    }

    protected void cmdRefresh_Click(object sender, ImageClickEventArgs e)
    {
        Session["OODStart"] = DateTime.Parse(txtDisplayStart.Text);
        Session["OODEnd"] = DateTime.Parse(txtDisplayFinish.Text);
        string csvCPList = Request.QueryString["cplist"];

        GetDates(csvCPList);
    }

    private void SetHeadings(CurrentUser curUser)
    {
        if (!curUser.CheckAccessRole(AccessRoleType.Edit, SpendManagementElement.RechargeAssociations, false))
        {
            cmdUpdate.Visible = false;
            cmdCancel.ImageUrl = "~/buttons/page_close.gif";
        }

        if (curUser.CheckAccessRole(AccessRoleType.Add, SpendManagementElement.RechargeAssociations, false))
        {
            lnkAddDates.Visible = false;
        }

        lblOfflineDate.Text = "Offline from";
        lblOnlineDate.Text = "Online from";
    }

    private void EditServiceDate(int cpid, int editid)
    {
        dateEntryPanel.Visible = true;
        curDatesPanel.Visible = false;
        CurrentUser curUser = cMisc.getCurrentUser(User.Identity.Name);
        cAccountSubAccounts subaccs = new cAccountSubAccounts(curUser.AccountID);
        cFWSettings fws = cMigration.ConvertToFWSettings(curUser.Account, subaccs.getSubAccountsCollection(), curUser.CurrentSubAccountId);
        cAccountProperties fwparams = subaccs.getSubAccountById(curUser.AccountID).SubAccountProperties;
        cEmployees employees = new cEmployees(fws.MetabaseCustomerId);
        cRechargeCollection rcoll = new cRechargeCollection(curUser.AccountID, curUser.CurrentSubAccountId, curUser.EmployeeID, (int)Session["ActiveContract"], cAccounts.getConnectionString(curUser.AccountID), fwparams);
        
        ArrayList raitems = rcoll.GetRechargeItemsByCPId(cpid);
        if (raitems.Count > 0)
        {
            cRecharge ritem = (cRecharge)raitems[0];
            SortedList<int,cRechargeServiceDate> sdates = ritem.ServiceDates;
            cRechargeServiceDate sdate = sdates[editid];
            if (sdate != null)
            {
                txtOfflineFrom.Text = sdate.OfflineFrom.ToShortDateString();
                txtOnlineFrom.Text = sdate.OnlineFrom.ToShortDateString();
            }
        }
        return;
    }

    private void DeleteServiceDate(int CPid, int SDid)
    {
        CurrentUser curUser = cMisc.getCurrentUser(User.Identity.Name);
        cAccountSubAccounts subaccs = new cAccountSubAccounts(curUser.AccountID);
        cFWSettings fws = cMigration.ConvertToFWSettings(curUser.Account, subaccs.getSubAccountsCollection(), curUser.CurrentSubAccountId);
        cAccountProperties fwparams = subaccs.getSubAccountById(curUser.AccountID).SubAccountProperties;
        cEmployees employees = new cEmployees(fws.MetabaseCustomerId);
        cRechargeCollection rcoll = new cRechargeCollection(curUser.AccountID, curUser.CurrentSubAccountId, curUser.EmployeeID, (int)Session["ActiveContract"], cAccounts.getConnectionString(curUser.AccountID), fwparams);

        cFWDBConnection db = new cFWDBConnection();
        db.DBOpen(fws, false);
        db.FWDb("D", "recharge_servicedates", "ServiceDateId", SDid, "", "", "", "", "", "", "", "", "", "");
        db.DBClose();

        ArrayList raitems = rcoll.GetRechargeItemsByCPId(CPid);
        for (int i = 0; i < raitems.Count; i++)
        {
            cRecharge delitem = (cRecharge)raitems[i];
            SortedList<int,cRechargeServiceDate> sdates = delitem.ServiceDates;
            sdates.Remove(SDid);
            delitem.ServiceDates = sdates;
            rcoll.UpdateRechargeItem(delitem);
        }
    }
}
