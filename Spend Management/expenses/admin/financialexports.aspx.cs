using System;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Services;
using SpendManagementLibrary;
using System.Collections.Generic;


namespace Spend_Management
{
    public partial class financialexports : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Title = "Financial Exports";
            Master.title = Title;
            Master.PageSubTitle = "Financial Export Details";
            Master.UseDynamicCSS = true;
           
            Session["ReportViewer"] = false;

            if (IsPostBack == false)
            {
                CurrentUser currentUser = cMisc.GetCurrentUser();
                currentUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.FinancialExports, true, true);

                cAccount account = new cAccounts().GetAccountsWithPaymentServiceEnabled().Find(x => x.accountid == currentUser.AccountID);
                if (account == null)
                {
                    expeditePaymentRow.Visible = false;
                }
                chkExpeditePayment.Enabled = currentUser.CheckAccessRole(AccessRoleType.Edit, SpendManagementElement.ExpeditePaymentReport, true, false);
                ViewState["accountid"] = currentUser.AccountID;
                ViewState["employeeid"] = currentUser.EmployeeID;

                string[] gridData = CreateFinancialExportsGrid("");
                litGrid.Text = gridData[1];

                // set the sel.grid javascript variables
                Page.ClientScript.RegisterStartupScript(this.GetType(), "finexpGridVars", cGridNew.generateJS_init("finexpGridVars", new System.Collections.Generic.List<string>() { gridData[0] }, currentUser.CurrentActiveModule), true);

                // populate reports dropdown list
                cReports clsreports = new cReports(currentUser.AccountID,currentUser.CurrentSubAccountId);
                
                SortedList<string, string> lstReports = clsreports.getFinancialExportableList();
                foreach (KeyValuePair<string, string> reportInfo in lstReports)
                {
                    ddlReport.Items.Add(new ListItem(reportInfo.Key, reportInfo.Value));
                }

                //populate trusts dropdown list
                cESRTrusts clsTrusts = new cESRTrusts(currentUser.AccountID);
                clsTrusts.CreateDropDownList(ref ddlNHSTrust);
            }
        }

        [WebMethod(EnableSession = true)]
        public static string[] CreateFinancialExportsGrid(string contextKey)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();

            string strSQL = "SELECT financial_exports.financialexportid, financial_exports.applicationtype, reports.reportname, reports.description, employees.username, financial_exports.lastexportdate, financial_exports.curexportnum, financial_exports.exporttype, financial_exports.NHSTrustID,financial_exports.ExpeditePaymentReport FROM dbo.financial_exports";
            cGridNew gridFinancialExports = new cGridNew(currentUser.AccountID, currentUser.EmployeeID, "financialExportsGrid", strSQL);
            cFields clsFields = new cFields(currentUser.AccountID);

            gridFinancialExports.KeyField = "financialexportid";
            gridFinancialExports.getColumnByName("financialexportid").hidden = true;
            gridFinancialExports.getColumnByName("NHSTrustID").hidden = true;
            gridFinancialExports.getColumnByName("ExpeditePaymentReport").hidden = true;

            gridFinancialExports.EmptyText = "No financial exports have been configured";
            gridFinancialExports.enabledeleting = true;
            gridFinancialExports.enableupdating = true;            
            gridFinancialExports.editlink = "javascript:EditFinancialExport({financialexportid});";
            gridFinancialExports.deletelink = "javascript:DeleteFinancialExport({financialexportid});";

            gridFinancialExports.addEventColumn("export", "Export", "javascript:exportReport({financialexportid},'{ExpeditePaymentReport}');", "Export this Financial Report");

            gridFinancialExports.addEventColumn("schedule", "Schedule", "javascript:checkTrustDetailsExist({financialexportid},{applicationtype},'{NHSTrustID}');", "View or Create a Schedule");

            gridFinancialExports.addEventColumn("history", "/shared/images/icons/history2.png", "exporthistory.aspx?financialexportid={financialexportid}&apptype={applicationtype}", "History", "View export history");


            ((cFieldColumn)gridFinancialExports.getColumnByName("applicationtype")).addValueListItem((byte)1, "Custom Report");
            ((cFieldColumn)gridFinancialExports.getColumnByName("applicationtype")).addValueListItem((byte)2, "ESR");
            ((cFieldColumn)gridFinancialExports.getColumnByName("exporttype")).addValueListItem((byte)1, "Report Viewer");
            ((cFieldColumn)gridFinancialExports.getColumnByName("exporttype")).addValueListItem((byte)2, "Excel");
            ((cFieldColumn)gridFinancialExports.getColumnByName("exporttype")).addValueListItem((byte)3, "CSV");
            ((cFieldColumn)gridFinancialExports.getColumnByName("exporttype")).addValueListItem((byte)4, "Flat File");
            ((cFieldColumn)gridFinancialExports.getColumnByName("exporttype")).addValueListItem((byte)5, "Pivot");

            return gridFinancialExports.generateGrid();
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
