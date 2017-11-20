using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Web.Services;
using SpendManagementLibrary;
using Spend_Management;

public partial class admin_reassigncontractsupplier : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!this.IsPostBack)
        {
            CurrentUser curUser = cMisc.GetCurrentUser();
            curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.ContractSupplierReassignment, false, true);

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
            string supplierStr = accProperties.SupplierPrimaryTitle;
            Title = "Reassign Contract " + supplierStr;
            Master.PageSubTitle = Title;

            lblCurSupplier.Text = "Current " + supplierStr;
            lblNewSupplier.Text = "New " + supplierStr;
            hiddenSourceId.Attributes.Add("style", "display: none;");
            hiddenTargetSupplier.Attributes.Add("style", "display: none;");

            //Page.SetFocus(txtContract);
        }
        this.divComment.Visible = !string.IsNullOrEmpty(this.lblMessage.Text);
    }

    protected void cmdCancel_Click(object sender, ImageClickEventArgs e)
    {
        Response.Redirect("~/MenuMain.aspx?menusection=management", true);
    }

    protected void cmdUpdate_Click(object sender, ImageClickEventArgs e)
    {
        int newSupplierid = 0;
        string contractKey = txtContract.Text.Substring(0, txtContract.Text.IndexOf('~') - 1).Trim();
        int curSupplierid = 0;
        int contractId = 0;
        string curSupplierName = "";
        string res = "";

        cFWDBConnection db = new cFWDBConnection();
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

		string titleStr = accProperties.SupplierPrimaryTitle;

        try
        {
            db.DBOpen(fws, false);
            db.FWDb("R2", "contract_details", "ContractKey", contractKey, "", "", "", "", "", "", "", "", "", "");
            if (db.FWDb2Flag)
            {
                curSupplierid = int.Parse(db.FWDbFindVal("supplierId", 2));
                contractId = int.Parse(db.FWDbFindVal("ContractId", 2));
            }

            db.FWDb("R2", "supplier_details", "supplierId", curSupplierid, "", "", "", "", "", "", "", "", "", "");
            if (db.FWDb2Flag)
            {
                curSupplierName = db.FWDbFindVal("Vendor Name", 2);
            }

            db.FWDb("R3", "supplier_details", "supplierName", txtNewSupplier.Text.Trim(), "subAccountId", curUser.CurrentSubAccountId, "", "", "", "", "", "", "", "");
            if (db.FWDb3Flag)
            {
                newSupplierid = int.Parse(db.FWDbFindVal("supplierId", 3));
            }

            if (newSupplierid != 0 && contractId != 0 && curSupplierid != 0)
            {
                // allow reassignment to take place
                db.SetFieldValue("supplierId", newSupplierid, "N", true);
                db.SetFieldValue("ContractKey", fws.KeyPrefix + '/' + contractId.ToString() + '/' + newSupplierid.ToString(), "S", false);
                db.FWDb("A", "contract_details", "ContractId", contractId, "", "", "", "", "", "", "", "", "", "");

                // move any variations across to new supplier also for the contract
                db.AddDBParam("conId", contractId, true);
                db.AddDBParam("vendorId", newSupplierid, false);
                db.ExecuteSQL("UPDATE contract_details SET [supplierId] = @vendorId WHERE [ContractId] IN (SELECT [VariationContractId] FROM link_variations WHERE [PrimaryContractId] = @conId)");

                // ensure that supplier product assocation exists for each product and target contract supplier
                string sql = "SELECT [ProductId] FROM contract_productdetails WHERE [ContractId] = @conId OR [ContractId] IN (SELECT [VariationContractId] FROM link_variations WHERE [PrimaryContractId] = @conId)";
                db.AddDBParam("conId", contractId, true);
                db.RunSQL(sql,  db.glDBWorkA, false, "", false);

                foreach (DataRow drow in db.glDBWorkA.Tables[0].Rows)
                {
                    db.FWDb("R2", "product_suppliers", "ProductId", drow["ProductId"], "supplierId", newSupplierid, "", "", "", "", "", "", "", "");
                    if (!db.FWDb2Flag)
                    {
                        db.SetFieldValue("ProductId", drow["ProductId"], "N", true);
                        db.SetFieldValue("supplierId", newSupplierid, "N", false);
                        db.FWDb("W", "product_suppliers", "", "", "", "", "", "", "", "", "", "", "", "");
                    }
                }

                cFWAuditLog ALog = new cFWAuditLog(fws, SpendManagementElement.ContractSupplierReassignment, curUser.CurrentSubAccountId);
                cAuditRecord ARec = new cAuditRecord();
                ARec.Action = cFWAuditLog.AUDIT_UPDATE;
                ARec.ContractNumber = contractKey;
                ARec.DataElementDesc = "ADMIN UPDATE";
                ARec.ElementDesc = "ASSIGN CONTRACT NEW " + titleStr.ToUpper();
                ARec.PreVal = curSupplierName;
                ARec.PostVal = txtNewSupplier.Text.Trim();
                ALog.AddAuditRec( ARec, true);
                ALog.CommitAuditLog(curUser.Employee, contractId);
            }

            db.DBClose();

            res = "Assignment completed successfully";
        }
        catch (Exception ex)
        {
            res = "An error occurred attemting reassignment\n\n" + ex.Message;
        }

        lblMessage.Text = res;

    }

    [WebMethod()]
    static public string GetSupplierFromContract(string contractStr)
    {
        HttpApplication appinfo = (HttpApplication)HttpContext.Current.ApplicationInstance;
        cFWDBConnection db = new cFWDBConnection();
        string supplierDesc = "";
		CurrentUser curUser = cMisc.GetCurrentUser();
		        
        string contractKey = contractStr.Substring(0, contractStr.IndexOf('~') - 1).Trim();
        int supplierId = int.Parse(contractKey.Substring(contractKey.LastIndexOf('/') + 1));
		cSuppliers suppliers = new cSuppliers(curUser.AccountID, curUser.CurrentSubAccountId);

		if (suppliers.getSupplierById(supplierId) != null)
		{
			supplierDesc = suppliers.getSupplierById(supplierId).SupplierName;
		}
        
        return supplierId.ToString() + '~' + supplierDesc.Trim();
    }

    [WebMethod()]
    static public string ValidateSupplier(string supplierStr)
    {
        string retVal = "0";
        HttpApplication appinfo = (HttpApplication)HttpContext.Current.ApplicationInstance;
		CurrentUser curUser = cMisc.GetCurrentUser();
		cAccountSubAccounts subaccs = new cAccountSubAccounts(curUser.AccountID);
		cFWSettings fws = cMigration.ConvertToFWSettings(curUser.Account, subaccs.getSubAccountsCollection(), curUser.CurrentSubAccountId);

        cFWDBConnection db = new cFWDBConnection();
        db.DBOpen(fws, false);

		db.FWDb("R", "supplier_details", "supplierName", supplierStr.Trim(), "subAccountId", curUser.CurrentSubAccountId, "", "", "", "", "", "", "", "");
        if (db.FWDbFlag)
        {
            retVal = "1";
        }
        db.DBClose();

        return retVal;
    }
}
