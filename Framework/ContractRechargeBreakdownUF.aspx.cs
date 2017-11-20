using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using FWClasses;
using SpendManagementLibrary;
using Spend_Management;

public partial class ContractRechargeBreakdownUF : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        Title = "Recharge Template User Fields";
        Master.title = "Recharge Template User Fields";
        
        returnURL.Value = Server.UrlDecode(Request.QueryString["returl"]);
        int cpId = int.Parse(Request.QueryString["cpid"]);
        int rechargeId = int.Parse(Request.QueryString["rid"]);
        int contractId = int.Parse(Request.QueryString["cid"]);

        PopulateUFields(cpId, rechargeId);
    }

    private void PopulateUFields(int cpId, int rechargeId)
    {
        CurrentUser curUser = cMisc.getCurrentUser(User.Identity.Name);
        	
		cUserdefinedFields userfields = new cUserdefinedFields(curUser.AccountID);
		cTables tables = new cTables(curUser.AccountID);
		cTable raTable = tables.GetTableByName("recharge_associations");
        System.Text.StringBuilder jscript = new System.Text.StringBuilder();

        userfields.createFieldPanel(ref phRAUFields, raTable, "rechargebreakdownUFields", out jscript);

		if (rechargeId > 0)
		{
			ViewState["record"] = cUFInterim.GetUFRecordValues(curUser.AccountID, rechargeId, userfields.GetFieldsByTable(raTable));
            userfields.populateRecordDetails(ref phRAUFields, raTable.UserdefinedTable, (SortedList<int, object>)ViewState["record"]);
		}
		else
		{
			ViewState["record"] = null;
		}

		//cUserFieldGroupingCollection ufgrps = new cUserFieldGroupingCollection(curUser.UserFWS, curUser.currentUser.userInfo, AppAreas.RECHARGE_GROUPING, cpId, employees);
		//cUserDefinedFields ufields = new cUserDefinedFields(curUser.UserFWS, curUser.currentUser.userInfo, employees);

		//db.DBOpen(curUser.UserFWS, false);

		//int categoryId = getContractCategory(db, cpId);
		//ArrayList arrGrps = ufgrps.GetFieldGroupsForCategoryId(categoryId);

		//if (arrGrps.Count > 0)
		//{
		//    for (int grpIdx = 0; grpIdx < arrGrps.Count; grpIdx++)
		//    {
		//        cUserFieldGrouping ufieldgrp = (cUserFieldGrouping)arrGrps[grpIdx];
		//        Literal litHeader = new Literal();
		//        litHeader.Text = "<div class=\"inputpanel\"><div class=\"inputpaneltitle\">" + ufieldgrp.GroupingDescription + "</div>";
		//        UFPanel.Controls.Add(litHeader);
		//        UFPanel.Controls.Add(ufields.GetUserFieldDisplayTable(AppAreas.RECHARGE_GROUPING, cpId, ufieldgrp.GroupingId));
		//        Literal litCloseHeader = new Literal();
		//        litCloseHeader.Text = "</div>";
		//        UFPanel.Controls.Add(litCloseHeader);
		//    }
		//}

		//db.DBClose();

        return;
    }

    private int getContractCategory(cFWDBConnection db, int cpId)
    {
        int conCategory = 0;

        string sql = "SELECT contract_details.[CategoryId] FROM contract_productdetails INNER JOIN contract_details ON contract_productdetails.[ContractId] = contract_details.[ContractId] WHERE [ContractProductId] = @cpId";
        db.AddDBParam("cpId", cpId, true);
        db.RunSQL(sql, db.glDBWorkA, false, "", false);

        conCategory = (int)db.GetFieldValue(db.glDBWorkA, "CategoryId", 0, 0);
        return conCategory;
    }

    protected void cmdCancel_Click(object sender, ImageClickEventArgs e)
    {
        Response.Redirect("~" + returnURL.Value,true);
    }

    protected void cmdUpdate_Click(object sender, ImageClickEventArgs e)
    {
        cFWDBConnection db = new cFWDBConnection();
		CurrentUser curUser = cMisc.getCurrentUser(User.Identity.Name);
        cAccountSubAccounts subaccs = new cAccountSubAccounts(curUser.AccountID);
        cFWSettings fws = cMigration.ConvertToFWSettings(curUser.Account, subaccs.getSubAccountsCollection(), curUser.CurrentSubAccountId);
		cEmployees emps = new cEmployees(curUser.AccountID);
        int rechargeId = int.Parse(Request.QueryString["rid"]);
        int contractId = int.Parse(Request.QueryString["cid"]);
        int cpId = int.Parse(Request.QueryString["cpid"]);

		cUserdefinedFields userfields = new cUserdefinedFields(curUser.AccountID);
		cTables tables = new cTables(curUser.AccountID);
		cTable raTable = tables.GetTableByName("recharge_associations");

        SortedList<int, object> newVals = userfields.getItemsFromPanel(ref phRAUFields, raTable.UserdefinedTable);

		//cUserFieldGroupingCollection ufgrps = new cUserFieldGroupingCollection(curUser.UserFWS, curUser.currentUser.userInfo, AppAreas.RECHARGE_GROUPING, cpId, emps);
		//cUserDefinedFields ufields = new cUserDefinedFields(curUser.UserFWS, curUser.currentUser.userInfo, emps);

		db.DBOpen(fws, false);
		//int categoryId = getContractCategory(db, cpId);

        cRechargeCollection templates = new cRechargeCollection(curUser.AccountID, curUser.CurrentSubAccountId, curUser.EmployeeID, contractId, cAccounts.getConnectionString(curUser.AccountID), subaccs.getSubAccountById(curUser.CurrentSubAccountId).SubAccountProperties);
		cRecharge rItem = null;
		if (rechargeId > 0)
		{
			rItem = templates.GetRechargeItemById(rechargeId);
		}
        cFWAuditLog ALog = new cFWAuditLog(fws, SpendManagementElement.RechargeAssociations, curUser.CurrentSubAccountId);

		bool firstchange = true;

		foreach (KeyValuePair<int, object> ufVal in newVals)
		{
			//ArrayList arrGrps = ufgrps.GetFieldGroupsForCategoryId(categoryId);

			//if (arrGrps.Count > 0)
			//{
			//    for (int grpIdx = 0; grpIdx < arrGrps.Count; grpIdx++)
			//    {
			//        cUserFieldGrouping ufieldgrp = (cUserFieldGrouping)arrGrps[grpIdx];

			//        foreach (KeyValuePair<int,cUserField> x in ufieldgrp.UserFields)
			//        {
			cAuditRecord ARec = new cAuditRecord();
			ARec.Action = cFWAuditLog.AUDIT_UPDATE;
			ARec.DataElementDesc = "RECHARGE ADDITIONAL INFO";
			//            cUserField ufield = (cUserField)x.Value;

			cUserDefinedField ufield = userfields.GetUserDefinedById(ufVal.Key);
			string fieldid = "UF" + ufield.userdefineid.ToString();

			ARec.ElementDesc = userfields.GetUserDefinedById(ufVal.Key).label; //ufield.FieldName.ToUpper();

			switch (ufield.fieldtype)
			{
				case FieldType.AutoCompleteTextbox:
				case FieldType.Hyperlink:
				case FieldType.LargeText:
				case FieldType.RelationshipTextbox:
				case FieldType.Text:
					if (rItem == null)
					{
						db.SetFieldValue(fieldid, (string)ufVal.Value, "S", firstchange);
						firstchange = false;

						ARec.PreVal = "";
						ARec.PostVal = (string)ufVal.Value;
						ALog.AddAuditRec(ARec, firstchange);
					}
					else
					{
						db.SetFieldValue(fieldid, (string)ufVal.Value, "S", firstchange);
						firstchange = false;

						ARec.PreVal = rItem.GetUserFieldValue(fieldid);
						ARec.PostVal = (string)ufVal.Value;
						ALog.AddAuditRec(ARec, firstchange);
					}
					break;

				case FieldType.Currency:
				case FieldType.Integer:
				case FieldType.Number:
				case FieldType.Relationship:
					if (rItem == null)
					{
						db.SetFieldValue(fieldid, (decimal)ufVal.Value, "N", firstchange);
						firstchange = false;

						ARec.PreVal = "";
						ARec.PostVal = ((decimal)ufVal.Value).ToString();
						ALog.AddAuditRec(ARec, firstchange);
					}
					else
					{
						db.SetFieldValue(fieldid, (decimal)ufVal.Value, "N", firstchange);
						firstchange = false;

						ARec.PreVal = rItem.GetUserFieldValue(fieldid);
						ARec.PostVal = ((decimal)ufVal.Value).ToString();
						ALog.AddAuditRec(ARec, firstchange);
					}
					break;

				case FieldType.TickBox:
				case FieldType.RunWorkflow:
					if (rItem == null)
					{
						db.SetFieldValue(fieldid, (bool)ufVal.Value, "B", firstchange);
						firstchange = false;

						ARec.PreVal = "";
						ARec.PostVal = ((bool)ufVal.Value).ToString();
						ALog.AddAuditRec(ARec, firstchange);
					}
					else
					{
						db.SetFieldValue(fieldid, (bool)ufVal.Value, "B", firstchange);
						firstchange = false;

						if (rItem.GetUserFieldValue(fieldid) == "1")
						{
							ARec.PreVal = "CHECKED";
						}
						else
						{
							ARec.PreVal = "UNCHECKED";
						}
						ARec.PostVal = ((bool)ufVal.Value).ToString();
						ALog.AddAuditRec(ARec, firstchange);
					}
					break;

				case FieldType.DateTime:
					if (rItem == null)
					{
						db.SetFieldValue(fieldid, (DateTime)ufVal.Value, "D", firstchange);
						firstchange = false;

						ARec.PreVal = "";
						ARec.PostVal = ((DateTime)ufVal.Value).ToShortDateString();
						ALog.AddAuditRec(ARec, firstchange);
					}
					else
					{
						db.SetFieldValue(fieldid, (DateTime)ufVal.Value, "D", firstchange);
						firstchange = false;
						ARec.PreVal = rItem.GetUserFieldValue(fieldid);
						ARec.PostVal = ((DateTime)ufVal.Value).ToShortDateString();
						ALog.AddAuditRec(ARec, firstchange);
					}
					break;
				default:
					break;
			}

			//                case UserFieldType.Character:
			//                case UserFieldType.Text:
			//                case UserFieldType.Hyperlink:
			//                    TextBox txt = (TextBox)UFPanel.FindControl(fieldid);
			//                    if (txt != null)
			//                    {
			//                        if (txt.Text != rItem.GetUserFieldValue(fieldid))
			//                        {
			//                            ARec.PreVal = rItem.GetUserFieldValue(fieldid);
			//                            ARec.PostVal = txt.Text;
			//                            ALog.AddAuditRec( ARec, firstchange);

			//                            db.SetFieldValue(fieldid, txt.Text, "S", firstchange);
			//                            rItem.UpdateUserFieldValue(fieldid, txt.Text);
			//                            firstchange = false;
			//                        }
			//                    }
			//                    break;
			//                case UserFieldType.DDList:
			//                    DropDownList ddlUF = (DropDownList)UFPanel.FindControl(fieldid);
			//                    if (ddlUF != null)
			//                    {
			//                        if (ddlUF.SelectedItem.Text != rItem.GetUserFieldValue(fieldid))
			//                        {
			//                            ARec.PreVal = rItem.GetUserFieldValue(fieldid);
			//                            ARec.PostVal = ddlUF.SelectedItem.Text;
			//                            ALog.AddAuditRec( ARec, firstchange);

			//                            db.SetFieldValue(fieldid, ddlUF.SelectedItem.Text, "S", firstchange);
			//                            rItem.UpdateUserFieldValue(fieldid, ddlUF.SelectedItem.Text);
			//                            firstchange = false;
			//                        }
			//                    }
			//                    break;
			//                case UserFieldType.DateField:
			//                    TextBox date = (TextBox)UFPanel.FindControl("UF" + ufield.FieldId.ToString());
			//                    if (date != null)
			//                    {
			//                        if (date.Text != rItem.GetUserFieldValue(fieldid))
			//                        {
			//                            ARec.PreVal = rItem.GetUserFieldValue(fieldid);
			//                            ARec.PostVal = date.Text;
			//                            ALog.AddAuditRec( ARec, firstchange);

			//                            db.SetFieldValue(fieldid, date.Text, "D", firstchange);
			//                            rItem.UpdateUserFieldValue(fieldid, date.Text);
			//                            firstchange = false;
			//                        }
			//                    }
			//                    break;
			//                case UserFieldType.Float:
			//                case UserFieldType.Number:
			//                    TextBox num = (TextBox)UFPanel.FindControl("UF" + ufield.FieldId.ToString());
			//                    if (num != null)
			//                    {
			//                        if (num.Text != rItem.GetUserFieldValue(fieldid))
			//                        {
			//                            ARec.PreVal = rItem.GetUserFieldValue(fieldid);
			//                            ARec.PostVal = num.Text;
			//                            ALog.AddAuditRec( ARec, firstchange);

			//                            db.SetFieldValue(fieldid, num.Text, "N", firstchange);
			//                            rItem.UpdateUserFieldValue(fieldid, num.Text);
			//                            firstchange = false;
			//                        }
			//                    }
			//                    break;
			//                case UserFieldType.RechargeAcc_Code:
			//                    cRechargeAccountCodes codes = new cRechargeAccountCodes(curUser.currentUser.userInfo, curUser.UserFWS);

			//                    if (rItem.GetUserFieldValue(fieldid) == "0" || rItem.GetUserFieldValue(fieldid) == "")
			//                    {
			//                        ARec.PreVal = "";
			//                    }
			//                    else
			//                    {
			//                        ARec.PreVal = codes.GetCodeById(int.Parse(rItem.GetUserFieldValue(fieldid))).AccountCode;
			//                    }

			//                    if (codes.Count > cDef.UF_MAXCOUNT)
			//                    {
			//                        // will use lookup, not ddlist
			//                        HiddenField txtcode = (HiddenField)UFPanel.FindControl("UF" + ufield.FieldId.ToString());
			//                        if (txtcode != null)
			//                        {
			//                            if (txtcode.Value != rItem.GetUserFieldValue(fieldid))
			//                            {
			//                                if (txtcode.Value == "0" || txtcode.Value == "")
			//                                {
			//                                    ARec.PostVal = "";
			//                                    ALog.AddAuditRec( ARec, firstchange);
			//                                    db.SetFieldValue(fieldid, 0, "N", firstchange);
			//                                    rItem.UpdateUserFieldValue(fieldid, "0");
			//                                    firstchange = false;
			//                                }
			//                                else
			//                                {
			//                                    ARec.PostVal = codes.GetCodeById(int.Parse(txtcode.Value)).AccountCode;
			//                                    ALog.AddAuditRec( ARec, firstchange);
			//                                    db.SetFieldValue(fieldid, txtcode.Value, "N", firstchange);
			//                                    rItem.UpdateUserFieldValue(fieldid, txtcode.Value);
			//                                    firstchange = false;
			//                                }
			//                            }
			//                        }
			//                    }
			//                    else
			//                    {
			//                        DropDownList ddlacc = (DropDownList)UFPanel.FindControl("UF" + ufield.FieldId.ToString());
			//                        if (ddlacc != null)
			//                        {
			//                            if (ddlacc.SelectedItem.Value != rItem.GetUserFieldValue(fieldid))
			//                            {
			//                                ARec.PostVal = ddlacc.SelectedItem.Text;
			//                                ALog.AddAuditRec( ARec, firstchange);
			//                                db.SetFieldValue(fieldid, ddlacc.SelectedItem.Value, "N", firstchange);
			//                                rItem.UpdateUserFieldValue(fieldid, ddlacc.SelectedItem.Value);
			//                                firstchange = false;
			//                            }
			//                        }
			//                    }
			//                    break;
			//                case UserFieldType.RechargeClient_Ref:
			//                    cRechargeClientList clients = new cRechargeClientList(curUser.currentUser.userInfo, curUser.UserFWS);

			//                    if (rItem.GetUserFieldValue(fieldid) == "0" || rItem.GetUserFieldValue(fieldid) == "")
			//                    {
			//                        ARec.PreVal = "";
			//                    }
			//                    else
			//                    {
			//                        ARec.PreVal = clients.GetClientById(int.Parse(rItem.GetUserFieldValue(fieldid))).ClientName;
			//                    }

			//                    if (clients.Count > cDef.UF_MAXCOUNT)
			//                    {
			//                        // used lookup and not ddlist
			//                        HiddenField txtcode = (HiddenField)UFPanel.FindControl("UF" + ufield.FieldId.ToString());
			//                        if (txtcode != null)
			//                        {
			//                            if (txtcode.Value != rItem.GetUserFieldValue(fieldid))
			//                            {
			//                                if (txtcode.Value == "0" || txtcode.Value == "")
			//                                {
			//                                    ARec.PostVal = "";
			//                                    ALog.AddAuditRec( ARec, firstchange);
			//                                    db.SetFieldValue(fieldid, 0, "N", firstchange);
			//                                    rItem.UpdateUserFieldValue(fieldid, "0");
			//                                    firstchange = false;
			//                                }
			//                                else
			//                                {
			//                                    ARec.PostVal = clients.GetClientById(int.Parse(txtcode.Value)).ClientName;
			//                                    ALog.AddAuditRec( ARec, firstchange);
			//                                    db.SetFieldValue(fieldid, txtcode.Value, "N", firstchange);
			//                                    rItem.UpdateUserFieldValue(fieldid, txtcode.Value);
			//                                    firstchange = false;
			//                                }
			//                            }
			//                        }
			//                    }
			//                    else
			//                    {
			//                        DropDownList ddlclient = (DropDownList)UFPanel.FindControl("UF" + ufield.FieldId.ToString());
			//                        if (ddlclient != null)
			//                        {
			//                            if (ddlclient.SelectedItem.Value != rItem.GetUserFieldValue(fieldid))
			//                            {
			//                                ARec.PostVal = ddlclient.SelectedItem.Text;
			//                                ALog.AddAuditRec( ARec, firstchange);
			//                                db.SetFieldValue(fieldid, ddlclient.SelectedItem.Value, "N", firstchange);
			//                                rItem.UpdateUserFieldValue(fieldid, ddlclient.SelectedItem.Value);
			//                                firstchange = false;
			//                            }
			//                        }
			//                    }
			//                    break;
			//                case UserFieldType.Site_Ref:
			//                    cSites sites = new cSites(curUser.UserFWS, curUser.currentUser.userInfo);

			//                    if (rItem.GetUserFieldValue(fieldid) == "0" || rItem.GetUserFieldValue(fieldid) == "")
			//                    {
			//                        ARec.PreVal = "";
			//                    }
			//                    else
			//                    {
			//                        ARec.PreVal = sites.GetSiteById(int.Parse(rItem.GetUserFieldValue(fieldid))).SiteCode;
			//                    }

			//                    if (sites.Count > cDef.UF_MAXCOUNT)
			//                    {
			//                        // used lookup not ddlist
			//                        HiddenField txtcode = (HiddenField)UFPanel.FindControl("UF" + ufield.FieldId.ToString());
			//                        if (txtcode != null)
			//                        {
			//                            if (txtcode.Value != rItem.GetUserFieldValue(fieldid))
			//                            {
			//                                if (txtcode.Value == "0" || txtcode.Value == "")
			//                                {
			//                                    ARec.PostVal = "";
			//                                    ALog.AddAuditRec( ARec, firstchange);
			//                                    db.SetFieldValue(fieldid, 0, "N", firstchange);
			//                                    rItem.UpdateUserFieldValue(fieldid, "0");
			//                                    firstchange = false;
			//                                }
			//                                else
			//                                {
			//                                    ARec.PostVal = sites.GetSiteById(int.Parse(txtcode.Value)).SiteCode;
			//                                    ALog.AddAuditRec( ARec, firstchange);
			//                                    db.SetFieldValue(fieldid, txtcode.Value, "N", firstchange);
			//                                    rItem.UpdateUserFieldValue(fieldid, txtcode.Value);
			//                                    firstchange = false;
			//                                }
			//                            }
			//                        }
			//                    }
			//                    else
			//                    {
			//                        DropDownList ddlsite = (DropDownList)UFPanel.FindControl("UF" + ufield.FieldId.ToString());
			//                        if (ddlsite != null)
			//                        {
			//                            if (ddlsite.SelectedItem.Value != rItem.GetUserFieldValue(fieldid))
			//                            {
			//                                ARec.PostVal = ddlsite.SelectedItem.Text;
			//                                ALog.AddAuditRec( ARec, firstchange);
			//                                db.SetFieldValue(fieldid, ddlsite.SelectedItem.Value, "N", firstchange);
			//                                rItem.UpdateUserFieldValue(fieldid, ddlsite.SelectedItem.Value);
			//                                firstchange = false;
			//                            }
			//                        }
			//                    }
			//                    break;
			//                case UserFieldType.StaffName_Ref:
			//                    if (rItem.GetUserFieldValue(fieldid) == "0" || rItem.GetUserFieldValue(fieldid) == "")
			//                    {
			//                        ARec.PreVal = "";
			//                    }
			//                    else
			//                    {
			//                        ARec.PreVal = emps.GetEmployeeById(int.Parse(rItem.GetUserFieldValue(fieldid))).EmployeeName;
			//                    }

			//                    if (emps.Count > cDef.UF_MAXCOUNT)
			//                    {
			//                        // used lookup, not ddlist
			//                        HiddenField txtcode = (HiddenField)UFPanel.FindControl("UF" + ufield.FieldId.ToString());
			//                        if (txtcode != null)
			//                        {
			//                            if (txtcode.Value != rItem.GetUserFieldValue(fieldid))
			//                            {
			//                                if (txtcode.Value == "0" || txtcode.Value == "")
			//                                {
			//                                    ARec.PostVal = "";
			//                                    ALog.AddAuditRec( ARec, firstchange);
			//                                    db.SetFieldValue(fieldid, 0, "N", firstchange);
			//                                    rItem.UpdateUserFieldValue(fieldid, "0");
			//                                    firstchange = false;
			//                                }
			//                                else
			//                                {
			//                                    ARec.PostVal = emps.GetEmployeeById(int.Parse(txtcode.Value)).EmployeeName;
			//                                    ALog.AddAuditRec( ARec, firstchange);
			//                                    db.SetFieldValue(fieldid, txtcode.Value, "N", firstchange);
			//                                    rItem.UpdateUserFieldValue(fieldid, txtcode.Value);
			//                                    firstchange = false;
			//                                }
			//                            }
			//                        }
			//                    }
			//                    else
			//                    {
			//                        DropDownList ddlemp = (DropDownList)UFPanel.FindControl("UF" + ufield.FieldId.ToString());
			//                        if (ddlemp != null)
			//                        {
			//                            if (ddlemp.SelectedItem.Value != rItem.GetUserFieldValue(fieldid))
			//                            {
			//                                ARec.PostVal = ddlemp.SelectedItem.Text;
			//                                ALog.AddAuditRec( ARec, firstchange);
			//                                db.SetFieldValue(fieldid, ddlemp.SelectedItem.Value, "N", firstchange);
			//                                rItem.UpdateUserFieldValue(fieldid, ddlemp.SelectedItem.Value);
			//                                firstchange = false;
			//                            }
			//                        }
			//                    }
			//                    break;
			//                case UserFieldType.Checkbox:
			//                    CheckBox chk = (CheckBox)UFPanel.FindControl("UF" + ufield.FieldId.ToString());

			//                    if (chk != null)
			//                    {
			//                        if (chk.Checked != bool.Parse(rItem.GetUserFieldValue(fieldid)))
			//                        {
			//                            if (chk.Checked)
			//                            {
			//                                ARec.PostVal = "CHECKED";
			//                                ARec.PreVal = "UNCHECKED";
			//                                ALog.AddAuditRec( ARec, firstchange);

			//                                db.SetFieldValue(fieldid, true, "B", firstchange);
			//                                rItem.UpdateUserFieldValue(fieldid, "1");
			//                                firstchange = false;
			//                            }
			//                            else
			//                            {
			//                                ARec.PreVal = "CHECKED";
			//                                ARec.PostVal = "UNCHECKED";
			//                                ALog.AddAuditRec( ARec, firstchange);

			//                                db.SetFieldValue(fieldid, false, "B", firstchange);
			//                                rItem.UpdateUserFieldValue(fieldid, "0");
			//                                firstchange = false;
			//                            }
			//                        }
			//                    }
			//                    break;
			//                default:
			//                    break;
			//            }
			//        }
			//}

			if (!firstchange)
			{
				ALog.CommitAuditLog(curUser.Employee, rechargeId);

				db.FWDb("A", "recharge_associations", "Recharge Id", rechargeId, "", "", "", "", "", "", "", "", "", "");
			}
		}

        db.DBClose();

        Response.Redirect(returnURL.Value, true);
    }
}
