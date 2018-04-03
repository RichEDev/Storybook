using System;
using System.Collections.Generic;
using SpendManagementLibrary;
using Spend_Management;
using System.Text;

public partial class admin_aeitemrole : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        this.Response.Cache.SetCacheability(System.Web.HttpCacheability.NoCache);
        this.Response.Cache.SetExpires(DateTime.Now.AddSeconds(-60));
        this.Response.Cache.SetNoStore();

        this.Title = "Add / Edit Item Role";
        this.Master.title = this.Title;
        this.Master.showdummymenu = true;

        if (!this.IsPostBack)
        {
            this.Master.enablenavigation = false;
            this.Master.ShowSubMenus = false;
            CurrentUser user = cMisc.GetCurrentUser();
            user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.ItemRoles, true, true);

            int itemroleid = 0;

            if (this.Request.QueryString["id"] != null)
            {
                int.TryParse(this.Request.QueryString["id"], out itemroleid);
            }

            StringBuilder js = new StringBuilder();

            js.Append("SEL.ItemRoles.ExpenseItems.SelectedItems = [];\n");

            if (itemroleid > 0)
            {
                ItemRoles clsroles = new ItemRoles(user.AccountID);
                ItemRole reqrole = clsroles.GetItemRoleById(itemroleid);
                this.txtrolename.Text = reqrole.Rolename;
                this.txtdescription.Text = reqrole.Description;
                
                foreach (RoleSubcat roleSubcat in reqrole.Items.Values)
                {
                    js.Append("SEL.ItemRoles.ExpenseItems.SelectedItems.push(" + roleSubcat.SubcatId + ");\n");
                }
            }

            
            js.Append("SEL.ItemRoles.IDs.itemRoleId = " + itemroleid + ";\n");
            js.Append("SEL.ItemRoles.SetupAssociatedExpenseItemDialog();\n");
            js.Append("SEL.ItemRoles.ExpenseItems.GetExpenseItems();\n");
            js.Insert(0, "$(document).ready(function() {\n");
            js.Append("});\n");
            this.ClientScript.RegisterStartupScript(this.GetType(), "js", js.ToString(), true);
            string[] gridData = this.CreateAssociatedExpenseItemGrid(user.AccountID, user.EmployeeID, itemroleid);
            this.litAssociatedExpenseItems.Text = gridData[1];

            // set the sel.grid javascript variables
            this.Page.ClientScript.RegisterStartupScript(this.GetType(), "ItemRolesGridVars", cGridNew.generateJS_init("ItemRolesGridVars", new List<string>() { gridData[0] }, user.CurrentActiveModule), true);
        }
        
    }

    /// <summary>
    /// Creates the item roles grid
    /// </summary>
    /// <param name="accountId">The account id of logged in user</param>
    /// <param name="employeeId">The employee id of logged in user</param>
    /// <param name="roleId">The id of the item role</param>
    /// <returns>The html of the grid</returns>
    private string[] CreateAssociatedExpenseItemGrid(int accountId, int employeeId, int roleId)
    {
        cTables clstables = new cTables(accountId);
        cFields clsfields = new cFields(accountId);
        List<cNewGridColumn> columns = new List<cNewGridColumn>
        {
            new cFieldColumn(clsfields.GetFieldByID(new Guid("4F800539-D9E8-4EE6-8CBF-EF41DFCD72B7"))), //rolesubcat
            new cFieldColumn(clsfields.GetFieldByID(new Guid("6EB08C2F-6FB5-49D0-B2B1-EB4EDAA586F3"))), // subcatId
            new cFieldColumn(clsfields.GetFieldByID(new Guid("ABFE0BB2-E6AC-40D0-88CE-C5F7B043924D"))), // subcat
            new cFieldColumn(clsfields.GetFieldByID(new Guid("A3BA1781-2C99-48DC-BC8C-126EF90CA55B"))), // receipt maximum
            new cFieldColumn(clsfields.GetFieldByID(new Guid("10A310FA-D34A-4568-B573-07A91F9AA765"))), // maximum
            new cFieldColumn(clsfields.GetFieldByID(new Guid("D2702AE8-609A-45AB-BF01-C58210EF1720"))) // isadditem
        };

        cGridNew clsgrid = new cGridNew(accountId, employeeId, "gridAssociatedExpenseItems", clstables.GetTableByID(new Guid("0123E0C5-5E68-4911-A062-9A6967D33BEB")), columns); // rolesubcats
        clsgrid.addFilter(clsfields.GetFieldByID(new Guid("01B16558-79DF-44D9-914F-AD9092B4D5D2")), ConditionType.Equals, new object[] { roleId }, null, ConditionJoiner.None);
        clsgrid.getColumnByName("rolesubcatid").hidden = true;
        clsgrid.getColumnByName("subcatid").hidden = true;
        clsgrid.KeyField = "rolesubcatid";
        clsgrid.enabledeleting = true;
        clsgrid.deletelink = "javascript:SEL.ItemRoles.AssociatedExpenseItems.Delete({rolesubcatid},{subcatid});";
        clsgrid.enableupdating = true;
        clsgrid.editlink = "javascript:SEL.ItemRoles.AssociatedExpenseItems.Edit(SEL.ItemRoles.IDs.itemRoleId,{rolesubcatid});";

        cAccountSubAccounts subaccs = new cAccountSubAccounts(accountId);
        cAccountSubAccount subAcc = subaccs.getFirstSubAccount();
        cAccountProperties properties = subAcc?.SubAccountProperties;
        if (properties?.BaseCurrency != null)
        {
            cCurrencies currencies = new cCurrencies(accountId, subAcc.SubAccountID);
            cCurrency currency = currencies.getCurrencyById(properties.BaseCurrency.Value);
            cGlobalCurrencies globalCurrencies = new cGlobalCurrencies();
            cGlobalCurrency globalCurrency = globalCurrencies.getGlobalCurrencyById(currency.globalcurrencyid);
            clsgrid.DefaultCurrencySymbol = globalCurrency.symbol;
            cGridFormat currencyFormat = new cGridFormat {Symbol = globalCurrency.symbol};
            clsgrid.getColumnByName("maximum").Format = currencyFormat;
            clsgrid.getColumnByName("receiptmaximum").Format = currencyFormat;

        }
        return clsgrid.generateGrid();
    }
    
}
