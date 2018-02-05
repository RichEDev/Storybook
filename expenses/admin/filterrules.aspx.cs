using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;

using expenses.Old_App_Code.admin;
using Infragistics.WebUI.UltraWebGrid;
using System.Web.Services;
using SpendManagementLibrary;
using Spend_Management;

namespace expenses.admin
{
    using System.Collections.Generic;

    using SpendManagementLibrary.Employees;

    public partial class filterrules : System.Web.UI.Page
    {
        private cFilterRules _filterRules;

        private cCostcodes _costCodes;

        protected void Page_Load(object sender, EventArgs e)
        {
            
            Title = "Filter Rules";
            Master.title = Title;
            //Master.helpurl = "/help/AD_CAT_mileage.htm";

            if (IsPostBack == false)
            {
                CurrentUser user = cMisc.GetCurrentUser();
                this.SetFilterRules(user.AccountID);
                user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.FilterRules, true, true);

                ViewState["accountid"] = user.AccountID;
                ViewState["employeeid"] = user.EmployeeID;
                ViewState["filtertype"] = Request.QueryString["FilterType"];

                // filter rules element not mapped yet

                cmbfilter.SelectedIndex = int.Parse(ViewState["filtertype"].ToString());
                ViewState["filtertype"] = (FilterType)cmbfilter.SelectedIndex;

                var filterRules = new cFilterRules(user.AccountID);
                byte selectedValue = Convert.ToByte(this.cmbfilter.Items[this.cmbfilter.SelectedIndex].Value);
                string[] gridData = filterRules.createFilterRuleGrid(user.AccountID, user.EmployeeID, (FilterType)selectedValue);
                litgrid.Text = gridData[1];

                // set the sel.grid javascript variables
                Page.ClientScript.RegisterStartupScript(this.GetType(), "FilterRulesGridVars", cGridNew.generateJS_init("FilterRulesGridVars", new List<string>() { gridData[0] }, user.CurrentActiveModule), true);
            }
        }

        #region Web Form Designer generated code
        override protected void OnInit(EventArgs e)
        {
            //
            // CODEGEN: This call is required by the ASP.NET Web Form Designer.
            //
            InitializeComponent();
            base.OnInit(e);
        }

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {

        }

        #endregion

        protected void cmbfilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            var filterRules = new cFilterRules((int)ViewState["accountid"]);
            byte selectedValue = Convert.ToByte(this.cmbfilter.Items[this.cmbfilter.SelectedIndex].Value);
            string[] gridData = filterRules.createFilterRuleGrid((int)ViewState["accountid"], (int)ViewState["employeeid"], (FilterType)selectedValue);
            litgrid.Text = gridData[1];
        }

        [WebMethod(EnableSession = true)]
        public static void deleteFilterRule(int accountid, int filterid)
        {
            var costCodes = new cCostcodes(accountid);
            var filterRules = new cFilterRules(accountid, costCodes);
            filterRules.DeleteFilterRule(filterid);
        }

        protected void lnkAddFilterRule_Click(object sender, EventArgs e)
        {
            Response.Redirect("aefilterrule.aspx?action=0&FilterType=" + cmbfilter.SelectedIndex);
            
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

        private void SetFilterRules(int accountId)
        {
            if (this._filterRules != null)
            {
                return;
            }

            if (this._costCodes == null)
            {
                this._costCodes = new cCostcodes(accountId);
            }

            this._filterRules = new cFilterRules(accountId, this._costCodes);
        }
    }
}
