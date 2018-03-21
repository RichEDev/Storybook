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

using SpendManagementLibrary;
using Spend_Management;

namespace expenses.admin
{
    public partial class settledadvances : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //Stops user accessing directly via url if no access
            if (!(cMisc.GetCurrentUser().CheckAccessRole(AccessRoleType.View, SpendManagementElement.Advances, true) && cMisc.GetCurrentUser().Account.AdvancesEnabled))
            {
                Response.Redirect("http://" + Request.Url.Host + "/restricted.aspx");
            }

            Title = "Settled Advances";
            Master.title = Title;
            if (!IsPostBack)
            {
                CurrentUser user = cMisc.GetCurrentUser();
                user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.Advances, true, true);
                ViewState["accountid"] = user.AccountID;
                ViewState["employeeid"] = user.EmployeeID;
                ViewState["subAccountID"] = user.CurrentSubAccountId;

                cFloats floats = new cFloats((int)ViewState["accountid"]);
                floats.AuditViewAdvances($"Settled Advances", user);
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            gridadvances.InitializeDataSource += new Infragistics.WebUI.UltraWebGrid.InitializeDataSourceEventHandler(gridadvances_InitializeDataSource);
            
        }

        void gridadvances_InitializeDataSource(object sender, Infragistics.WebUI.UltraWebGrid.UltraGridEventArgs e)
        {
            cFloats clsfloats = new cFloats((int)ViewState["accountid"]);
            gridadvances.DataSource = clsfloats.getSettledGrid(true, (int)ViewState["employeeid"], true);
        }
        protected void gridadvances_InitializeLayout(object sender, Infragistics.WebUI.UltraWebGrid.LayoutEventArgs e)
        {
            cEmployees clsemployees = new cEmployees((int)ViewState["accountid"]);
            cCurrencies clscurrencies = new cCurrencies((int)ViewState["accountid"], (int)ViewState["subAccountID"]);
            e.Layout.Bands[0].Columns.FromKey("floatid").Hidden = true;
            e.Layout.Bands[0].Columns.FromKey("basecurrency").Hidden = true;
            e.Layout.Bands[0].Columns.FromKey("employeeid").ValueList = clsemployees.CreateVList((int)ViewState["accountid"]);
            e.Layout.Bands[0].Columns.FromKey("employeeid").Type = Infragistics.WebUI.UltraWebGrid.ColumnType.DropDownList;
            e.Layout.Bands[0].Columns.FromKey("employeeid").Header.Caption = "Employee";
            e.Layout.Bands[0].Columns.FromKey("name").Header.Caption = "Advance Name";
            e.Layout.Bands[0].Columns.FromKey("reason").Header.Caption = "Reason for Advance";
            e.Layout.Bands[0].Columns.FromKey("originalcurrency").Header.Caption = "Currency";
            e.Layout.Bands[0].Columns.FromKey("originalcurrency").Type = Infragistics.WebUI.UltraWebGrid.ColumnType.DropDownList;
            e.Layout.Bands[0].Columns.FromKey("originalcurrency").ValueList = clscurrencies.CreateVList();
            
            e.Layout.Bands[0].Columns.FromKey("floatamount").Header.Caption = "Amount Required";
            
            e.Layout.Bands[0].Columns.FromKey("floatused").Header.Caption = "Amount Used";
            e.Layout.Bands[0].Columns.FromKey("floatavailable").Header.Caption = "Amount Available";
            e.Layout.Bands[0].Columns.FromKey("exchangerate").Header.Caption = "Exchange Rate";
            e.Layout.Bands[0].Columns.FromKey("Total Prior To Convert").Header.Caption = "Foreign Amount";
            e.Layout.Bands[0].Columns.FromKey("settled").Hidden = true;

            e.Layout.Bands[0].Columns.FromKey("floatamount").Header.Caption = "Amount Issued";
                e.Layout.Bands[0].Columns.FromKey("requiredby").Hidden = true;
                e.Layout.Bands[0].Columns.FromKey("stage").Hidden = true;
                e.Layout.Bands[0].Columns.FromKey("rejected").Hidden = true;
                e.Layout.Bands[0].Columns.FromKey("rejectreason").Hidden = true;
                e.Layout.Bands[0].Columns.FromKey("disputed").Hidden = true;
                e.Layout.Bands[0].Columns.FromKey("dispute").Hidden = true;
                e.Layout.Bands[0].Columns.FromKey("paid").Hidden = true;

                e.Layout.Bands[0].Columns.FromKey("requiredby").Header.Caption = "Required By";
                e.Layout.Bands[0].Columns.FromKey("requiredby").Format = "dd/MM/yyyy";
            e.Layout.Bands[0].Columns.FromKey("approver").Hidden = true;
            e.Layout.Bands[0].Columns.FromKey("approved").Hidden = true;

            e.Layout.Bands[0].Columns.FromKey("rejected").Header.Caption = "Rejected";
            e.Layout.Bands[0].Columns.FromKey("rejectreason").Header.Caption = "Reason for Rejection";

            e.Layout.Bands[0].Columns.FromKey("disputed").Header.Caption = "Corrected / Disputed";
            e.Layout.Bands[0].Columns.FromKey("dispute").Hidden = true;
            e.Layout.Bands[0].Columns.FromKey("paid").Header.Caption = "Paid";

            e.Layout.Bands[0].Columns.FromKey("stage").Header.Caption = "Stage";
            e.Layout.Bands[0].Columns.FromKey("issuenum").Header.Caption = "Issue Num";
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
