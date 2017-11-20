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
                //user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.FilterRules, true, true);

                cmbfilter.SelectedIndex = int.Parse(ViewState["filtertype"].ToString());
                ViewState["filtertype"] = (FilterType)cmbfilter.SelectedIndex;
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

        protected override void OnInitComplete(EventArgs e)
        {
            base.OnInitComplete(e);
            gridfilterrules.InitializeDataSource += new Infragistics.WebUI.UltraWebGrid.InitializeDataSourceEventHandler(gridfilterrules_InitializeDataSource);
        }

        private void getGrid(FilterType filtertype)
        {
            this.SetFilterRules((int)ViewState["accountid"]);
            gridfilterrules.DataSource = _filterRules.getFilterRuleGrid(filtertype);
            gridfilterrules.DataBind();
        }

        void gridfilterrules_InitializeDataSource(object sender, Infragistics.WebUI.UltraWebGrid.UltraGridEventArgs e)
        {
            getGrid((FilterType)ViewState["filtertype"]);
        }

        protected void gridfilterrules_InitializeRow(object sender, Infragistics.WebUI.UltraWebGrid.RowEventArgs e)
        {
            e.Row.Cells.FromKey("edit").Value = "<a href=\"aefilterrule.aspx?action=2&filterid=" + e.Row.Cells.FromKey("filterid").Value + "\"><img alt=\"Edit\" src=\"../icons/edit.gif\"></a>";
            e.Row.Cells.FromKey("delete").Value = "<a href=\"javascript:deleteFilterRule(" + (int)ViewState["accountid"] + "," + e.Row.Cells.FromKey("filterid").Value + ");\"><img alt=\"Delete\" src=\"../icons/delete2.gif\"></a>";
        }

        protected void gridfilterrules_InitializeLayout(object sender, Infragistics.WebUI.UltraWebGrid.LayoutEventArgs e)
        {
            #region Sorting
            cEmployees clsemployees = new cEmployees((int)ViewState["accountid"]);
            Employee reqemp = clsemployees.GetEmployeeById((int)ViewState["employeeid"]);
            cGridSort sortorder = reqemp.GetGridSortOrders().GetBy(Grid.FilterRule);
            if (sortorder != null)
            {
                if (e.Layout.Bands[0].SortedColumns.Count == 0)
                {
                    if (e.Layout.Bands[0].Columns.FromKey(sortorder.columnname) != null)
                    {
                        e.Layout.Bands[0].Columns.FromKey(sortorder.columnname).SortIndicator = (SortIndicator)sortorder.sortorder;
                        e.Layout.Bands[0].SortedColumns.Add(sortorder.columnname);
                    }
                }
            }
            #endregion

            e.Layout.Bands[0].Columns.FromKey("filterid").Hidden = true;
            e.Layout.Bands[0].Columns.FromKey("parent").Header.Caption = "Parent";
            e.Layout.Bands[0].Columns.FromKey("child").Header.Caption = "Child";
            e.Layout.Bands[0].Columns.FromKey("enabled").Header.Caption = "Enabled";

            if (e.Layout.Bands[0].Columns.FromKey("edit") == null)
            {
                e.Layout.Bands[0].Columns.Insert(0, new Infragistics.WebUI.UltraWebGrid.UltraGridColumn("delete", "<img alt=\"Delete\" src=\"../icons/delete2_blue.gif\">", Infragistics.WebUI.UltraWebGrid.ColumnType.HyperLink, ""));
                e.Layout.Bands[0].Columns.FromKey("delete").Width = Unit.Pixel(15);
                e.Layout.Bands[0].Columns.Insert(0, new Infragistics.WebUI.UltraWebGrid.UltraGridColumn("edit", "<img alt=\"Edit\" src=\"../icons/edit_blue.gif\">", Infragistics.WebUI.UltraWebGrid.ColumnType.HyperLink, ""));
                e.Layout.Bands[0].Columns.FromKey("edit").Width = Unit.Pixel(15);
            }
        }

        protected void cmbfilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            getGrid((FilterType)cmbfilter.SelectedIndex);
            
        }

        protected void gridfilterrules_SortColumn(object sender, Infragistics.WebUI.UltraWebGrid.SortColumnEventArgs e)
        {
            UltraWebGrid grid = (UltraWebGrid)sender;
            byte direction = (byte)grid.Columns[e.ColumnNo].SortIndicator;
            CurrentUser currentUser = cMisc.GetCurrentUser();
            currentUser.Employee.GetGridSortOrders().Add(Grid.FilterRule, grid.Columns[e.ColumnNo].Key, direction);
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
