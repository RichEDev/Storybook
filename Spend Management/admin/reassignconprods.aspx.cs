using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.Configuration;
using SpendManagementLibrary;
using Spend_Management;

public partial class admin_reassignconprods : System.Web.UI.Page
{
    protected override void OnLoad(EventArgs e)
    {    
        Title = "Reassign Contract Products";
        Master.PageSubTitle = Title;
        Master.enablenavigation = false;
       
        if (!this.IsPostBack)
        {
            CurrentUser curUser = cMisc.GetCurrentUser();
            curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.ContractProductReassignment, false, true);
            lblMessage.Text = "";

            //Page.SetFocus(txtSource);
        }
        this.DisableContainer();
        hiddenSourceId.Attributes.Add("style", "display: none;");
        hiddenTargetId.Attributes.Add("style", "display: none;");

        btnMoveCancel.Attributes.Add("onclick", "window.location.href='../admin/reassignconprods.aspx';");
        base.OnLoad(e);
    }

    protected void cmdOK_Click(object sender, ImageClickEventArgs e)
    {
        GetData(chkState.None);
    }

    private enum chkState
    {
        None = 0,
        All = 1
    }        

    private void GetData(chkState selectState)
    {
        cFWDBConnection db = new cFWDBConnection();
        Table CP_Table = new Table();
        CP_Table.CssClass = "datatbl";
        TableHeaderRow CPHRow = new TableHeaderRow();
        TableHeaderCell CPHCell;
        CurrentUser curUser = cMisc.GetCurrentUser();
		cAccountSubAccounts subaccs = new cAccountSubAccounts(curUser.AccountID);
		cFWSettings fws = cMigration.ConvertToFWSettings(curUser.Account, subaccs.getSubAccountsCollection(), curUser.CurrentSubAccountId);
        
        int curContractId = 0;
        int CP_Count = 0;
        int colCount = 3;

        db.DBOpen(fws, false);

		cAccountProperties accProperties;
        if (curUser.CurrentSubAccountId >= 0)
        {
            accProperties = subaccs.getSubAccountById(curUser.CurrentSubAccountId).SubAccountProperties;
        }
        else
        {
            accProperties=subaccs.getFirstSubAccount().SubAccountProperties;
        }

		bool useCPStatus = accProperties.RechargeSettings.CP_Delete_Action == 1 ? true : false;

        StringBuilder sql = new StringBuilder();
        string UFieldlist = "";
        string ContractKey = "";

        ContractKey = txtSource.Text.Substring(0, txtSource.Text.IndexOf(" ~ "));

        db.FWDb("R3", "contract_details", "contractKey", ContractKey, "", "", "", "", "", "", "", "", "", "");
        if (db.FWDb3Flag)
        {
            // ######Needs new User defined fields implementing###############
            curContractId = int.Parse(db.FWDbFindVal("contractId", 3));
        }

		cCPFieldInfo CPInfo = new cCPFieldInfo(curUser.AccountID, curUser.CurrentSubAccountId, cAccounts.getConnectionString(curUser.AccountID), curUser.EmployeeID, curContractId);
        CP_Count = GetCPCount(db, curContractId);

        sql.Append(GetFilterSQL(CPInfo, CP_Count, useCPStatus, UFieldlist));

        // construct result table
        CPHCell = new TableHeaderCell();
        CPHCell.Text = "";
        CPHCell.Attributes.Add("width", "50px");
        CPHRow.Cells.Add(CPHCell);

        CPHCell = new TableHeaderCell();
        CPHCell.Text = "Product Name";
        CPHRow.Cells.Add(CPHCell);

        if (CPInfo.CPFieldInfoItem.CP_UF1 > 0)
        {
            CPHCell = new TableHeaderCell();
            CPHCell.Text = CPInfo.CPFieldInfoItem.CP_UF1_Title;
            CPHRow.Cells.Add(CPHCell);
            colCount++;
        }

        if (CPInfo.CPFieldInfoItem.CP_UF2 > 0)
        {
            CPHCell = new TableHeaderCell();
            CPHCell.Text = CPInfo.CPFieldInfoItem.CP_UF2_Title;
            CPHRow.Cells.Add(CPHCell);
            colCount++;
        }

        CPHCell = new TableHeaderCell();
        CPHCell.Text = "Active?";
        CPHCell.Attributes.Add("width", "100px");
        CPHRow.Cells.Add(CPHCell);

        CP_Table.Rows.Add(CPHRow);

        db.AddDBParam("CP_Filter", txtCPFilter.Text.Trim(), true);
        if (useCPStatus)
        {
            db.AddDBParam("CPstatus", 0, false);
        }
        db.AddDBParam("conId", curContractId, false);
        db.RunSQL(sql.ToString(),  db.glDBWorkA, false, "", false);

        lblMessage.Text = "Contract Products matched on source contract = " + db.glNumRowsReturned.ToString();

        TableCell CPCell;
        bool rowalt = false;
        string rowClass = "row1";
        int index = 0;

        if (db.glNumRowsReturned > 0)
        {
            TableRow[] CPRow = new TableRow[db.glNumRowsReturned];

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

                CPRow[index] = new TableRow();
                CPCell = new TableCell();
                CPCell.CssClass = rowClass;

                string checkedVal = "";
                switch (selectState)
                {
                    case chkState.All:
                        checkedVal = " checked ";
                        break;
                    default:
                        break;
                }

                Literal litChk = new Literal();
                litChk.Text = "<input id=\"chk" + drow["contractProductId"].ToString() + "\" type=\"checkbox\"" + checkedVal + " value=\"" + drow["contractProductId"].ToString() + "\" name=\"chk\" />";
                CPCell.Controls.Add(litChk);
                CPRow[index].Cells.Add(CPCell);

                CPCell = new TableCell();
                CPCell.CssClass = rowClass;
                CPCell.Text = (string)drow["productName"];
                CPRow[index].Cells.Add(CPCell);

                if (CPInfo.CPFieldInfoItem.CP_UF1 > 0)
                {
                    CPCell = new TableCell();
                    CPCell.CssClass = rowClass;
                    CPCell.Width = Unit.Pixel(150);
                    if (drow["UF" + CPInfo.CPFieldInfoItem.CP_UF1.ToString().Trim()] != null)
                    {
                        string tmpVal = "";

                        switch (CPInfo.CPFieldInfoItem.CP_UF2_Type)
                        {
                            case UserFieldType.Float:
                                tmpVal = drow["UF" + CPInfo.CPFieldInfoItem.CP_UF1.ToString().Trim()].ToString();
                                break;
                            case UserFieldType.Checkbox:
                            case UserFieldType.Number:
                            case UserFieldType.RechargeAcc_Code:
                            case UserFieldType.RechargeClient_Ref:
                            case UserFieldType.Site_Ref:
                            case UserFieldType.StaffName_Ref:
                                tmpVal = drow["UF" + CPInfo.CPFieldInfoItem.CP_UF1.ToString().Trim()].ToString();
                                break;
                            default:
                                tmpVal = (string)drow["UF" + CPInfo.CPFieldInfoItem.CP_UF1.ToString().Trim()];
                                break;
                        }

                        if (tmpVal != "" && tmpVal != "0")
                        {
                            switch (CPInfo.CPFieldInfoItem.CP_UF1_Type)
                            {
                                case UserFieldType.RechargeAcc_Code:
                                case UserFieldType.RechargeClient_Ref:
                                case UserFieldType.Site_Ref:
                                case UserFieldType.StaffName_Ref:
                                    CPCell.Text = (string)CPInfo.CPFieldInfoItem.CP_UF1_Coll[drow["UF" + CPInfo.CPFieldInfoItem.CP_UF1.ToString().Trim()]];
                                    break;
                                case UserFieldType.Checkbox:
                                    CheckBox chk = new CheckBox();
                                    chk.ID = "chkCPUF2";
                                    chk.Enabled = false;
                                    if (tmpVal == "1")
                                    {
                                        chk.Checked = true;
                                    }
                                    CPCell.HorizontalAlign = HorizontalAlign.Center;
                                    CPCell.Controls.Add(chk);
                                    break;
                                default:
                                    CPCell.Text = (string)drow["UF" + CPInfo.CPFieldInfoItem.CP_UF1.ToString().Trim()];
                                    break;
                            }
                        }
                    }
                    else
                    {
                        CPCell.Text = "";
                    }
                    CPRow[index].Cells.Add(CPCell);
                }

                if (CPInfo.CPFieldInfoItem.CP_UF2 > 0)
                {
                    CPCell = new TableCell();
                    CPCell.CssClass = rowClass;
                    CPCell.Width = Unit.Pixel(150);
                    if (drow["UF" + CPInfo.CPFieldInfoItem.CP_UF2.ToString().Trim()] != null)
                    {
                        string tmpVal = "";

                        switch (CPInfo.CPFieldInfoItem.CP_UF2_Type)
                        {
                            case UserFieldType.Float:
                                tmpVal = drow["UF" + CPInfo.CPFieldInfoItem.CP_UF2.ToString().Trim()].ToString();
                                break;
                            case UserFieldType.Checkbox:
                            case UserFieldType.Number:
                            case UserFieldType.RechargeAcc_Code:
                            case UserFieldType.RechargeClient_Ref:
                            case UserFieldType.Site_Ref:
                            case UserFieldType.StaffName_Ref:
                                tmpVal = drow["UF" + CPInfo.CPFieldInfoItem.CP_UF2.ToString().Trim()].ToString();
                                break;
                            default:
                                tmpVal = (string)drow["UF" + CPInfo.CPFieldInfoItem.CP_UF2.ToString().Trim()];
                                break;
                        }

                        if (tmpVal != "" && tmpVal != "0")
                        {
                            switch (CPInfo.CPFieldInfoItem.CP_UF2_Type)
                            {
                                case UserFieldType.RechargeAcc_Code:
                                case UserFieldType.RechargeClient_Ref:
                                case UserFieldType.Site_Ref:
                                case UserFieldType.StaffName_Ref:
                                    CPCell.Text = (string)CPInfo.CPFieldInfoItem.CP_UF2_Coll[drow["UF" + CPInfo.CPFieldInfoItem.CP_UF2.ToString().Trim()]];
                                    break;
                                case UserFieldType.Checkbox:
                                    CheckBox chk = new CheckBox();
                                    chk.ID = "chkCPUF2";
                                    chk.Enabled = false;
                                    if (tmpVal == "1")
                                    {
                                        chk.Checked = true;
                                    }
                                    CPCell.HorizontalAlign = HorizontalAlign.Center;
                                    CPCell.Controls.Add(chk);
                                    break;
                                default:
                                    CPCell.Text = tmpVal;
                                    break;
                            }
                        }
                    }
                    else
                    {
                        CPCell.Text = "";
                    }
                    CPRow[index].Cells.Add(CPCell);
                }

                CPCell = new TableCell();
                CPCell.CssClass = rowClass;
                CPCell.HorizontalAlign = HorizontalAlign.Center;
                CheckBox activechk = new CheckBox();
                activechk.ID = "achk" + drow["contractProductId"];
                activechk.Enabled = false;
                if ((int)drow["archiveStatus"] == 1)
                {
                    activechk.Checked = true;
                }
                else
                {
                    activechk.Checked = false;
                }
                CPCell.Controls.Add(activechk);
                CPRow[index].Cells.Add(CPCell);

                index++;
            }
            CP_Table.Rows.AddRange(CPRow);

            ViewState["CPMove_Data"] = db.glDBWorkA;

            lnkDeselectAll.Visible = true;
            lnkSelectAll.Visible = true;
        }
        else
        {
            TableRow CPRow = new TableRow();
            CPCell = new TableCell();
            CPCell.CssClass = "row1";
            CPCell.ColumnSpan = colCount;
            CPCell.Text = "No contract product information returned";
            CPRow.Cells.Add(CPCell);
            CP_Table.Rows.Add(CPRow);

            btnMove.Visible = false;
            btnMoveCancel.Visible = false;
        }

        resultPanel.Controls.Add(CP_Table);

        btnMove.Visible = true;
        btnMoveCancel.Visible = true;

        db.DBClose();
    }

    private int GetCPCount(cFWDBConnection db, int contractId)
    {
        StringBuilder sql = new StringBuilder();
        sql.Append("SELECT COUNT(*) AS [ConProdCount] ");
        sql.Append(" FROM [contract_productdetails]");
        sql.Append(" WHERE [contract_productdetails].[contractId] = " + contractId.ToString());
        db.RunSQL(sql.ToString(),  db.glDBWorkD, false, "", false);

        return (int)db.GetFieldValue( db.glDBWorkD, "ConProdCount", 0, 0);
    }

    private StringBuilder GetFilterSQL(cCPFieldInfo cpInfo, int CP_Count, bool useCPStatus, string UFieldlist)
    {
        StringBuilder sql = new StringBuilder();
        SortedList moreJoins = new SortedList();
        SortedList UF_DisplayFields = new SortedList();
        string UF1_Key = "";
        string UF2_Key = "";
        string whereAND = "";

        sql.Append("SELECT ");
        if (txtCPFilter.Text.Trim() == "")
        {
            sql.Append("TOP " + (int)cDef.MAX_CP_DISPLAY);
        }
        sql.Append(" [contractProductId],[contractId],[contract_productdetails].[productId] AS [ProdID],ISNULL([currencyId],0) AS [currencyId],[codes_salestax].[description],\n");
        sql.Append("[Quantity],[codes_units].[description],[unitCost],ISNULL([projectedSaving],0) AS [projectedSaving],[productDetails].[ProductName],ISNULL([productValue],0) AS [productValue],ISNULL([pricePaid],0) AS [pricePaid],ISNULL([maintenanceValue],0) AS [maintenanceValue],[maintenancePercent],[archiveStatus]<**UF**><**UFDISPFIELDS**>\n");
        sql.Append("FROM [contract_productdetails] \n");
        sql.Append("LEFT OUTER JOIN [codes_units] ON [contract_productdetails].[unitId] = [codes_units].[unitId]\n");
        sql.Append("LEFT OUTER JOIN [codes_salestax] ON [contract_productdetails].[salesTaxRate] = [codes_salestax].[salesTaxId]\n");
        sql.Append("LEFT OUTER JOIN [productDetails] ON [contract_productdetails].[productId] = [productDetails].[ProductId]\n");
        sql.Append("<**JOINS**>");
        sql.Append("WHERE ");

        if (cpInfo.CPFieldInfoItem.CP_UF1 > 0)
        {
            UF1_Key = "";
            switch (cpInfo.CPFieldInfoItem.CP_UF1_Type)
            {
                case UserFieldType.Site_Ref:
                    UF1_Key = "SITE";
                    moreJoins.Add(UF1_Key, "LEFT JOIN [codes_sites] ON [codes_sites].[site Id] = [contract_productdetails].[UF" + cpInfo.CPFieldInfoItem.CP_UF1.ToString() + "] \n");
                    UF_DisplayFields.Add(UF1_Key, "[codes_sites].[Site Code]");
                    break;
                case UserFieldType.RechargeClient_Ref:
                    UF1_Key = "CLIENT";
                    moreJoins.Add(UF1_Key, "LEFT JOIN [codes_rechargeentity] ON [codes_rechargeentity].[entityId] = [contract_productdetails].[UF" + cpInfo.CPFieldInfoItem.CP_UF1.ToString() + "] \n");
                    UF_DisplayFields.Add(UF1_Key, "[codes_rechargeentity].[Name]");
                    break;
                case UserFieldType.RechargeAcc_Code:
                    UF1_Key = "ACCOUNT";
                    moreJoins.Add(UF1_Key, "LEFT JOIN [codes_accountcodes] ON [codes_accountcodes].[code Id] = [contract_productdetails].[UF" + cpInfo.CPFieldInfoItem.CP_UF1.ToString() + "] \n");
                    UF_DisplayFields.Add(UF1_Key, "[codes_accountcodes].[accountCode]");
                    break;
				default:
					break;
            }
        }

        if (cpInfo.CPFieldInfoItem.CP_UF2 > 0)
        {
            UF2_Key = "";
            switch (cpInfo.CPFieldInfoItem.CP_UF2_Type)
            {
                case UserFieldType.StaffName_Ref:
                    if (!moreJoins.ContainsKey("STAFF"))
                    {
                        UF2_Key = "STAFF";
                        moreJoins.Add(UF2_Key, "LEFT JOIN [staff_details] ON [staff_details].[Staff Id] = [contract_productdetails].[UF" + cpInfo.CPFieldInfoItem.CP_UF2.ToString() + "] \n");
                        UF_DisplayFields.Add(UF2_Key, "[staff_details].[Staff Name]");
                    }
                    break;
                case UserFieldType.Site_Ref:
                    if (!moreJoins.ContainsKey("SITE"))
                    {
                        UF2_Key = "SITE";
                        moreJoins.Add(UF2_Key, "LEFT JOIN [codes_sites] ON [codes_sites].[Site Id] = [contract_productdetails].[UF" + cpInfo.CPFieldInfoItem.CP_UF2.ToString() + "] \n");
                        UF_DisplayFields.Add(UF2_Key, "[codes_sites].[Site Code]");
                    }
                    break;

                case UserFieldType.RechargeClient_Ref:
                    if (!moreJoins.ContainsKey("CLIENT"))
                    {
                        UF2_Key = "CLIENT";
                        moreJoins.Add(UF2_Key, "LEFT JOIN [codes_rechargeentity] ON [codes_rechargeentity].[entityId] = [contract_productdetails].[UF" + cpInfo.CPFieldInfoItem.CP_UF2.ToString() + "] \n");
                        UF_DisplayFields.Add(UF2_Key, "[codes_rechargeentity].[Name]");
                    }
                    break;
                case UserFieldType.RechargeAcc_Code:
                    if (!moreJoins.ContainsKey("ACCOUNT"))
                    {
                        UF2_Key = "ACCOUNT";
                        moreJoins.Add(UF2_Key, "LEFT JOIN [codes_accountcodes] ON [codes_accountcodes].[Code Id] = [contract-productdetails].[UF" + cpInfo.CPFieldInfoItem.CP_UF2.ToString() + "] \n");
                        UF_DisplayFields.Add(UF2_Key, "[codes_accountcodes].[Account Code]");
                    }
                    break;
                default:
                    break;
            }
        }

        if (CP_Count > cDef.MAX_CP_DISPLAY)
        {
            if (txtCPFilter.Text.Trim() != "" && txtCPFilter.Text.Trim() != "*")
            {
                sql.Append(" (");
                sql.Append("[productName] LIKE '%' + @CP_Filter + '%'");

                if (cpInfo.CPFieldInfoItem.CP_UF1 > 0)
                {
                    switch (cpInfo.CPFieldInfoItem.CP_UF1_Type)
                    {
                        case UserFieldType.StaffName_Ref:
                        case UserFieldType.Site_Ref:
                        case UserFieldType.RechargeClient_Ref:
                        case UserFieldType.RechargeAcc_Code:
                            sql.Append(" OR " + UF_DisplayFields[UF1_Key] + " LIKE '%' + @CP_Filter + '%'");
                            break;
                        default:
                            sql.Append(" OR [UF" + cpInfo.CPFieldInfoItem.CP_UF1.ToString().Trim() + "] LIKE '%' + @CP_Filter + '%'");
                            break;
                    }
                }

                if (cpInfo.CPFieldInfoItem.CP_UF2 > 0)
                {
                    switch (cpInfo.CPFieldInfoItem.CP_UF2_Type)
                    {
                        case UserFieldType.RechargeAcc_Code:
                        case UserFieldType.RechargeClient_Ref:
                        case UserFieldType.Site_Ref:
                        case UserFieldType.StaffName_Ref:
                            sql.Append(" OR " + UF_DisplayFields[UF2_Key] + " LIKE '%' + @CP_Filter + '%'");
                            break;
                        default:
                            sql.Append(" OR [UF" + cpInfo.CPFieldInfoItem.CP_UF2.ToString().Trim() + "] LIKE '%' + @CP_Filter + '%'");
                            break;
                    }
                }

                sql.Append(")");
                whereAND = " AND ";
            }
        }

        if (useCPStatus)
        {
            sql.Append(whereAND + "[archiveStatus] = @CPstatus ");
            whereAND = " AND ";
        }

        sql.Append(whereAND + "[contractId] = @conId \n");
        sql.Append("ORDER BY ");

        string comma = "";

        if (cpInfo.CPFieldInfoItem.CP_UF1 > 0)
        {
            switch (cpInfo.CPFieldInfoItem.CP_UF1_Type)
            {
                case UserFieldType.StaffName_Ref:
                case UserFieldType.Site_Ref:
                case UserFieldType.RechargeClient_Ref:
                case UserFieldType.RechargeAcc_Code:
                    sql.Append(UF_DisplayFields[UF1_Key]);
                    break;
                default:
                    sql.Append(" [UF" + cpInfo.CPFieldInfoItem.CP_UF1.ToString().Trim() + "]");
                    break;
            }

            comma = ",";
        }

        if (cpInfo.CPFieldInfoItem.CP_UF2 > 0)
        {
            switch (cpInfo.CPFieldInfoItem.CP_UF2_Type)
            {
                case UserFieldType.StaffName_Ref:
                case UserFieldType.Site_Ref:
                case UserFieldType.RechargeClient_Ref:
                case UserFieldType.RechargeAcc_Code:
                    sql.Append(comma + UF_DisplayFields[UF2_Key]);
                    break;
                default:
                    sql.Append(comma + " [UF" + cpInfo.CPFieldInfoItem.CP_UF2.ToString().Trim() + "]");
                    break;
            }
            comma = ",";
        }
        sql.Append(comma + " [productName]");

        sql.Replace("<**UF**>", UFieldlist);

        StringBuilder joinSQL = new StringBuilder();

        for (int x = 0; x < moreJoins.Count - 1; x++)
        {
            joinSQL.Append(moreJoins.GetByIndex(x));
        }
        sql.Replace("<**JOINS**>", joinSQL.ToString());

        joinSQL = new StringBuilder();
        for (int x = 0; x < UF_DisplayFields.Count - 1; x++)
        {
            joinSQL.Append("," + UF_DisplayFields.GetByIndex(x));
        }

        sql.Replace("<**UFDISPFIELDS**>", joinSQL.ToString());

        return sql;
    }


    protected void lnkSelectAll_Click(object sender, EventArgs e)
    {
        GetData(chkState.All);
    }

    protected void lnkDeselectAll_Click(object sender, EventArgs e)
    {
        GetData(chkState.None);
    }

    protected void cmdGetDataCancel_Click(object sender, ImageClickEventArgs e)
    {
        Response.Redirect("~/MenuMain.aspx?menusection=management", true);
    }

    protected void txtSource_TextChanged(object sender, EventArgs e)
    {
        CurrentUser curUser = cMisc.GetCurrentUser();
        cAccountSubAccounts subaccs = new cAccountSubAccounts(curUser.AccountID);
        cFWSettings fws = cMigration.ConvertToFWSettings(curUser.Account, subaccs.getSubAccountsCollection(), curUser.CurrentSubAccountId);

        // validate that selection is correct
        string selectionstr = txtSource.Text.Trim();
        imgSource.ImageUrl = "~/buttons/delete2.png";
        hiddenSourceId.Text = "0";

        if (selectionstr.IndexOf('~') > 0)
        {
            string key = selectionstr.Substring(0, selectionstr.IndexOf('~'));
            cFWDBConnection db = new cFWDBConnection();
            db.DBOpen(fws, false);
            db.FWDb("R", "contract_details", "contractKey", key.Trim(), "", "", "", "", "", "", "", "", "", "");
            if (db.FWDbFlag)
            {
                imgSource.ImageUrl = "~/buttons/check.png";
                hiddenSourceId.Text = db.FWDbFindVal("contractId", 1);
            }
            
            db.DBClose();
        }

        resultPanel.Controls.Clear();
        
        lblMessage.Text = "";
        this.DisableContainer();
    }

    protected void txtTarget_TextChanged(object sender, EventArgs e)
    {
        CurrentUser curUser = cMisc.GetCurrentUser();
        cAccountSubAccounts subaccs = new cAccountSubAccounts(curUser.AccountID);
        cFWSettings fws = cMigration.ConvertToFWSettings(curUser.Account, subaccs.getSubAccountsCollection(), curUser.CurrentSubAccountId);

        // validate that selection is correct
        imgTarget.ImageUrl = "~/buttons/delete2.png";
        hiddenTargetId.Text = "0";

        string selectionstr = txtTarget.Text.Trim();
        if (selectionstr.IndexOf('~') > 0)
        {
            string key = selectionstr.Substring(0, selectionstr.IndexOf('~'));
            cFWDBConnection db = new cFWDBConnection();
            db.DBOpen(fws, false);
            db.FWDb("R", "contract_details", "contractKey", key.Trim(), "", "", "", "", "", "", "", "", "", "");
            if (db.FWDbFlag)
            {
                imgTarget.ImageUrl = "~/buttons/check.png";
                hiddenTargetId.Text = db.FWDbFindVal("contractId", 1);
            }
            db.DBClose();
        }

        resultPanel.Controls.Clear();
        lblMessage.Text = "";
        this.DisableContainer();
    }

    protected void btnMove_Click(object sender, ImageClickEventArgs e)
    {
        string csvCP = Request.Form["chk"];

        if (csvCP == null)
        {
            GetData(chkState.None);

            lblMessage.Text += "<br /><br /> A Product must be selected";

            return;
        }

        cFWDBConnection db = new cFWDBConnection();
        DataSet dset = (DataSet)ViewState["CPMove_Data"];
        int targetConId = 0;
        int targetSupplierId = 0;
        int srcConId = 0;
        string abortStatus = Request.Form["hiddenAbortStatus"];

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

        if (abortStatus == null || abortStatus == "0")
        {
            lblMessage.Text = "Reassignment aborted";
            return;
        }

        db.DBOpen(fws, false);
        db.FWDb("R3", "contract_details", "ContractKey", txtTarget.Text.Substring(0, txtTarget.Text.IndexOf(" ~ ")), "", "", "", "", "", "", "", "", "", "");
        if (db.FWDb3Flag)
        {
            targetConId = int.Parse(db.FWDbFindVal("ContractId", 3));
            targetSupplierId = int.Parse(db.FWDbFindVal("supplierId", 3));
        }

        db.FWDb("R3", "contract_details", "ContractKey", txtSource.Text.Substring(0, txtSource.Text.IndexOf(" ~ ")), "", "", "", "", "", "", "", "", "", "");
        if (db.FWDb3Flag)
        {
            srcConId = int.Parse(db.FWDbFindVal("ContractId", 3));
        }

        // ensure that supplier product assocation exists for each product and target contract supplier
        string[] cpId = csvCP.Split(',');
        for (int x = 0; x < cpId.Length; x++)
        {
            db.FWDb("R3", "contract_productdetails", "ContractProductId", cpId[x], "", "", "", "", "", "", "", "", "", "");
            if (db.FWDb3Flag)
            {
                db.FWDb("R2", "product_suppliers", "ProductId", db.FWDbFindVal("ProductId", 3), "supplierId", targetSupplierId, "", "", "", "", "", "", "", "");
                if (!db.FWDb2Flag)
                {
                    db.SetFieldValue("ProductId", db.FWDbFindVal("ProductId", 3), "N", true);
                    db.SetFieldValue("supplierId", targetSupplierId, "N", false);
                    db.FWDb("W", "product_suppliers", "", "", "", "", "", "", "", "", "", "", "", "");
                }
            }
        }

        try
        {
            // change contract id of selected contract products
            string sql = "UPDATE [contract_productdetails] SET [ContractId] = @conId WHERE [ContractProductId] IN (" + csvCP.Trim() + ")";
            db.AddDBParam("conId", targetConId, true);
            db.ExecuteSQL(sql);
            
            // should refresh the annual contract value for both contracts
            string strUpdateCPfunction = "EXECUTE dbo.UpdateCPAnnualCost @conId, @acv";
			if (fws.glUseRechargeFunction)
			{
				if (accProperties.AutoUpdateCVRechargeLive)
				{
					strUpdateCPfunction = "EXECUTE dbo.UpdateRechargeCPAnnualCost @conId, 1";
				}
			}

            if (fws.glAutoUpdateCV)
            {
                db.AddDBParam("acv", 1, true);
            }
            else
            {
                db.AddDBParam("acv", 0, true);
            }
            db.AddDBParam("conId", srcConId, false);
            db.ExecuteSQL(strUpdateCPfunction);

            if (fws.glAutoUpdateCV)
            {
                db.AddDBParam("acv", 1, true);
            }
            else
            {
                db.AddDBParam("acv", 0, true);
            }
            db.AddDBParam("conId", targetConId, false);
            db.ExecuteSQL(strUpdateCPfunction);

            cAuditRecord ARec = new cAuditRecord();
            cFWAuditLog ALog = new cFWAuditLog(fws, SpendManagementElement.ContractProducts, curUser.CurrentSubAccountId);
            ARec.Action = cFWAuditLog.AUDIT_UPDATE;
            ARec.ContractNumber = txtSource.Text.Substring(0, txtSource.Text.IndexOf(" ~ "));
            ARec.DataElementDesc = "TRANSFER CONTRACT PRODUCTS";
            ARec.ElementDesc = cpId.Length.ToString() + " products transferred";
			ALog.AddAuditRec(ARec, true);
            ALog.CommitAuditLog(curUser.Employee, targetConId);
            
            db.DBClose();
            lblMessage.Text = cpId.Length.ToString() + " records transferred successfully";

            btnMove.Visible = false;
            btnMoveCancel.Visible = false;

            resultPanel.Controls.Clear();
        }
        catch (Exception ex)
        {
            lblMessage.Text = "Record transfer failed.";
        }
    }
    /// <summary>
    /// Disable controls.
    /// </summary>
    public void DisableContainer()
    {
        this.divMessage.Visible = !string.IsNullOrEmpty(this.lblMessage.Text);
    }
}