using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Xml.Linq;
using expenses.Old_App_Code.admin;
using System.Web.Services;
using expenses.Old_App_Code;

using Spend_Management;
using SpendManagementLibrary;

namespace expenses.admin
{
    public partial class statements : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Title = "Corporate Card Statements";
            Master.title = Title;

            if (IsPostBack == false)
            {
                CurrentUser user = cMisc.GetCurrentUser();
                user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.CorporateCards, true, true);

                ViewState["accountid"] = user.AccountID;
                ViewState["employeeid"] = user.EmployeeID;

                cAccounts clsAccounts = new cAccounts();
                cAccount reqAccount = clsAccounts.GetAccountByID(user.AccountID);

                if (reqAccount.CorporateCardsEnabled == false)
                {
                    Response.Redirect("~/home.aspx", true);
                }
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            gridstatements.InitializeDataSource += new Infragistics.WebUI.UltraWebGrid.InitializeDataSourceEventHandler(gridstatements_InitializeDataSource);
        }

        void gridstatements_InitializeDataSource(object sender, Infragistics.WebUI.UltraWebGrid.UltraGridEventArgs e)
        {
            cCardStatements clsstatements = new cCardStatements((int)ViewState["accountid"]);
            gridstatements.DataSource = clsstatements.getGrid();
        }

        protected void gridstatements_InitializeLayout(object sender, Infragistics.WebUI.UltraWebGrid.LayoutEventArgs e)
        {
            e.Layout.Bands[0].Columns.FromKey("statementid").Hidden = true;
            e.Layout.Bands[0].Columns.FromKey("name").Header.Caption = "Name";
            e.Layout.Bands[0].Columns.FromKey("cardprovider").Header.Caption = "Provider";
            e.Layout.Bands[0].Columns.FromKey("statementdate").Header.Caption = "Statement Date";
            e.Layout.Bands[0].Columns.FromKey("createdon").Header.Caption = "Upload Date";
            e.Layout.Bands[0].Columns.FromKey("item_count").Header.Caption = "Item Count";
            e.Layout.Bands[0].Columns.FromKey("unallocated_cards").Header.Caption = "Unallocated Cards";
            e.Layout.Bands[0].Columns.FromKey("statementdate").Format = "dd/MM/yyyy";
            if (e.Layout.Bands[0].Columns.FromKey("edit") == null)
            {
                e.Layout.Bands[0].Columns.Insert(0, new Infragistics.WebUI.UltraWebGrid.UltraGridColumn("delete", "<img alt=\"Delete\" src=\"../icons/delete2_blue.gif\">", Infragistics.WebUI.UltraWebGrid.ColumnType.HyperLink, ""));
                e.Layout.Bands[0].Columns.FromKey("delete").Width = Unit.Pixel(15);
                e.Layout.Bands[0].Columns.Insert(0, new Infragistics.WebUI.UltraWebGrid.UltraGridColumn("edit", "<img alt=\"Edit\" src=\"../icons/edit_blue.gif\">", Infragistics.WebUI.UltraWebGrid.ColumnType.HyperLink, ""));
                e.Layout.Bands[0].Columns.FromKey("edit").Width = Unit.Pixel(15);
            }
        }

        protected void gridstatements_InitializeRow(object sender, Infragistics.WebUI.UltraWebGrid.RowEventArgs e)
        {
            e.Row.Cells.FromKey("edit").Value = "<a href=\"editstatement.aspx?statementid=" + e.Row.Cells.FromKey("statementid").Value + "\"><img alt=\"Edit\" src=\"../icons/edit.gif\"></a>";
            e.Row.Cells.FromKey("delete").Value = "<a href=\"javascript:deleteStatement(" + e.Row.Cells.FromKey("statementid").Value + ");\"><img alt=\"Delete\" src=\"../icons/delete2.gif\"></a>";
            if ((int)e.Row.Cells.FromKey("unallocated_cards").Value > 0)
            {
                e.Row.Cells.FromKey("unallocated_cards").Value = "<a href=\"unallocated_cards.aspx?statementid=" + e.Row.Cells.FromKey("statementid").Value + "\">" + e.Row.Cells.FromKey("unallocated_cards").Value + "</a>";
            }
            e.Row.Cells.FromKey("name").Value = "<a href=\"transaction_viewer.aspx?statementid=" + e.Row.Cells.FromKey("statementid").Value + "\">" + e.Row.Cells.FromKey("name").Value + "</a>";
        }

        [WebMethod(EnableSession = true)]
        public static bool deleteStatement(int accountid, int statementid)
        {
            cCardStatements clsstatements = new cCardStatements(accountid);
            return clsstatements.deleteStatement(statementid);
        }
    }
}
