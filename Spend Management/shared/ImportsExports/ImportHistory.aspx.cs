using System;
using System.Web.Services;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SpendManagementLibrary;
using System.Text;

namespace Spend_Management
{
    /// <summary>
    /// Import History Screen
    /// </summary>
    public partial class ImportHistory : System.Web.UI.Page
    {
        /// <summary>
        /// ClientID of the modal popup used by importexport.js routines
        /// </summary>
        public string logModalID;
        /// <summary>
        /// ClientID of the drop down list for filtering the grid
        /// </summary>
        public string ddlFilterID;

        /// <summary>
        /// Primary Page Load
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                Master.title = "Imports / Exports";
                Title = "Import History";
                Master.PageSubTitle = Title;

                CurrentUser curUser = cMisc.GetCurrentUser();
                curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.ImportHistory, true, true);

                string[] gridData = getHistoryGrid();
                Literal litGrid = new Literal();
                litGrid.Text = gridData[2];
                phHistoryGrid.Controls.Add(litGrid);

                // set the sel.grid javascript variables
                Page.ClientScript.RegisterStartupScript(this.GetType(), "ImpHistoryGridVars", cGridNew.generateJS_init("ImpHistoryGridVars", new List<string>() { gridData[1] }, curUser.CurrentActiveModule), true);

                var accProperties = new cAccountProperties();
                this.lstFilter.Items.Add(new ListItem("ESR Outbound Import V1.0", ((int)ApplicationType.ESROutboundImport).ToString()));
                this.lstFilter.Items.Add(new ListItem("ESR Outbound Import V2.0", ((int)ApplicationType.EsrOutboundImportV2).ToString()));
                this.lstFilter.Items.Add(new ListItem("Excel Import", ((int)ApplicationType.ExcelImport).ToString()));
                this.lstFilter.Attributes.Add("onchange", "javascript:refreshHistoryGrid();");

                ClientScriptManager mgr = this.ClientScript;
                mgr.RegisterStartupScript(
                    typeof(string), 
                    "accInfo", 
                    "var accountID = " + curUser.AccountID + "; var employeeID = " + curUser.EmployeeID + ";", 
                    true);

                this.ddlViewType.Items.Add(new ListItem(LogViewType.All.ToString(), ((int)LogViewType.All).ToString()));
                this.ddlViewType.Items.Add(new ListItem(LogViewType.Successes.ToString(), ((int)LogViewType.Successes).ToString()));
                this.ddlViewType.Items.Add(new ListItem(LogViewType.Added.ToString(), ((int)LogViewType.Added).ToString()));
                this.ddlViewType.Items.Add(new ListItem(LogViewType.Updated.ToString(), ((int)LogViewType.Updated).ToString()));
                this.ddlViewType.Items.Add(new ListItem(LogViewType.Failures.ToString(), ((int)LogViewType.Failures).ToString()));
                this.ddlViewType.Items.Add(new ListItem(LogViewType.Warnings.ToString(), ((int)LogViewType.Warnings).ToString()));
                this.ddlViewType.Items.Add(new ListItem(LogViewType.Other.ToString(), ((int)LogViewType.Other).ToString()));
                this.ddlViewType.Items.Add(new ListItem(LogViewType.Summary.ToString(), ((int)LogViewType.Summary).ToString()));

                this.ddlElementType.Items.Add(new ListItem("All", "0"));
            }

            this.logModalID = this.modLog.ClientID;
            this.ddlFilterID = this.lstFilter.ClientID;
            return;
        }

        /// <summary>
        /// Gets the HTML grid of import history
        /// </summary>
        /// <returns>HTML structured data</returns>
        private string[] getHistoryGrid()
        {
            svcLogging ws = new svcLogging();
            return ws.getHistoryGrid(Convert.ToInt32(lstFilter.SelectedValue));
        }

        /// <summary>
        /// Action the Close button click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void cmdClose_Click(object sender, ImageClickEventArgs e)
        {
            Response.Redirect("~/exportmenu.aspx", true);
        }
    }
}
