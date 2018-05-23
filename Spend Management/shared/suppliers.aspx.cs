using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SpendManagementLibrary;
using System.Text;


namespace Spend_Management
{
    using BusinessLogic.Modules;

    public partial class suppliers : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            CurrentUser curUser = cMisc.GetCurrentUser();

            switch (curUser.CurrentActiveModule)
            {
                case Modules.Contracts:
                    Master.helpid = 1058;
                    break;
                default:
                    Master.helpid = 0;
                    break;
            }

            if (Session["SM_allowAdd"] != null && Session["SM_allowAmend"] != null && Session["SM_allowDelete"] != null)
            {
                if (Convert.ToBoolean(Session["SM_allowAdd"]) != true && Convert.ToBoolean(Session["SM_allowAmend"]) != true && Convert.ToBoolean(Session["SM_allowDelete"]) != true)
                {
                    System.Web.HttpContext.Current.Response.Redirect("~/shared/restricted.aspx?reason=Current%20access%20role%20does%20not%20permit%20you%20to%20view%20this%20page.", true);
                }
            }
            else
            {
                curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.SupplierDetails, true, true);
            }

            hypNewSupplier.Visible = curUser.CheckAccessRole(AccessRoleType.Add, SpendManagementElement.SupplierDetails, true);

            cAccountSubAccounts clsSubAccounts = new cAccountSubAccounts(curUser.AccountID);
            cAccountProperties clsProperties = clsSubAccounts.getSubAccountById(curUser.CurrentSubAccountId).SubAccountProperties;

            hypNewSupplier.Text = "New " + clsProperties.SupplierPrimaryTitle;

            string pageTitle;
            if (clsProperties.SupplierPrimaryTitle.Substring(clsProperties.SupplierPrimaryTitle.Length - 1) == "s")
            {
                pageTitle = clsProperties.SupplierPrimaryTitle;
            }
            else
            {
                pageTitle = clsProperties.SupplierPrimaryTitle + "s";
            }

            Title = pageTitle;
            Master.PageSubTitle = pageTitle;

            if (!IsPostBack)
            {
                GetSupplierGrid(curUser.AccountID, curUser.EmployeeID, curUser.CurrentSubAccountId, curUser.CheckAccessRole(AccessRoleType.Delete, SpendManagementElement.SupplierDetails, true), curUser.CurrentActiveModule);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="accountid"></param>
        /// <param name="employeeid"></param>
        /// <param name="subaccountid"></param>
        /// <param name="enabledeleting"></param>
        /// <param name="activeModule"></param>
		private void GetSupplierGrid(int accountid, int employeeid, int subaccountid, bool enabledeleting, Modules activeModule)
		{
			cSuppliers suppliers = new cSuppliers(accountid, employeeid);

            Literal litGrid = new Literal();

            string[] gridData = suppliers.GetSuppliersGrid(accountid, employeeid, subaccountid, enabledeleting);
            litGrid.Text = gridData[1];
            			
			phGrid.Controls.Add(litGrid);

            // set the sel.grid javascript variables
            Page.ClientScript.RegisterStartupScript(this.GetType(), "SupplierGridVars", cGridNew.generateJS_init("SupplierGridVars", new List<string> { gridData[0] }, activeModule), true);
		}


        /// <summary>
        /// Close button event function
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void cmdClose_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            string sPreviousURL = (SiteMap.CurrentNode == null) ? "~/home.aspx" : SiteMap.CurrentNode.ParentNode.Url;

            Response.Redirect(sPreviousURL, true);
        }
    }
}
