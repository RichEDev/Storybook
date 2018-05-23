namespace Spend_Management.shared.reports
{
    using System;
    using System.Collections.Generic;
    using System.Web.Script.Serialization;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    using SpendManagementHelpers.TreeControl;
    using SpendManagementLibrary;
    using SpendManagementLibrary.Helpers;
    using SpendManagementLibrary.Logic_Classes.Fields;
    using SpendManagementLibrary.Logic_Classes.Tables;

    using Spend_Management.shared.code.Helpers;
    using Spend_Management.shared.webServices;
    using System.Text;

    using BusinessLogic.Modules;

    public partial class aeReport : System.Web.UI.Page
    {
        /// <summary>
        /// Gets or sets the default tab to show on page load.  Normally zero (General) but when editing
        /// via report viewer 1 which is Columns.
        /// </summary>
        public int DefaultTab { get; set; }

        /// <summary>
        /// Gets or sets the colour for the chart preview border and table header.
        /// </summary>
        public string BorderColour { get; set; }

        /// <summary>
        /// Gets or sets the colour for the text.
        /// </summary>
        public string TextColour { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            // Force IE out of compat mode
            this.Response.AddHeader("X-UA-Compatible", "IE=edge");

            this.Master.enablenavigation = false;
            this.Master.UseDynamicCSS = true;
            this.Master.ShowPageOptions = false;
            

            if (this.IsPostBack == false)
            {
                var user = cMisc.GetCurrentUser();
                cColours clscolours = new cColours(user.AccountID, user.CurrentSubAccountId, user.CurrentActiveModule);
                this.BorderColour = clscolours.tableHeaderBGColour;
                this.TextColour = clscolours.tableHeaderTxtColour;

                user.CheckAccessRole(AccessRoleType.Add, SpendManagementElement.Reports, true, true);

                this.smProxy.Scripts.Add(new ScriptReference(GlobalVariables.StaticContentLibrary + "/js/jQuery/shortcut.js"));
                this.smProxy.Scripts.Add(new ScriptReference(GlobalVariables.StaticContentLibrary + "/js/jQuery/jquery-ui.datepicker-en-gb.js"));
                this.smProxy.Scripts.Add(new ScriptReference(GlobalVariables.StaticContentLibrary + "/js/jQuery/jquery.ui.timepicker-0.3.2.js"));
                this.smProxy.Scripts.Add(new ScriptReference(GlobalVariables.StaticContentLibrary + "/js/jQuery/jquery.ui.multiselect.js"));
                this.Master.scriptman.Scripts.Add(new ScriptReference("SyncFusion", string.Empty));

                Guid reportId;
                string startUpScript = string.Empty;
                int requestnum = 0;
                Guid preselectedReportBase = Guid.Empty;
                cAccountProperties accountProperties = new cAccountSubAccounts(user.AccountID).getSubAccountById(user.CurrentSubAccountId).SubAccountProperties;
                var tables = new SubAccountTables(new cTables(user.AccountID), new TableRelabler(accountProperties));
                var customEntities = new cCustomEntities(user);
                var listItemFactory = new ListItemFactory(user, customEntities);
                this.cmbreporton.Items.AddRange(listItemFactory.CreateList(tables.GetItemsForReportDropDown(user.CurrentActiveModule)).ToArray());

                if (this.cmbreporton.Items.FindByValue(preselectedReportBase.ToString()) != null)
                {
                    this.cmbreporton.Items.FindByValue(preselectedReportBase.ToString()).Selected = true;
                }

                var clsfolders = new cReportFolders(user.AccountID);
                this.cmbfolders.Items.AddRange(clsfolders.CreateDropDown());
                this.chkLimitReport.Attributes.Add("onclick", "SEL.Reports.Report.LimitReportChanged(this.checked);");
                this.txtLimitReport.Style.Add(HtmlTextWriterStyle.Display, "none");
                this.lbRowLimit.Style.Add(HtmlTextWriterStyle.Display, "none");
                this.chkLimitReport.Checked = false;
                if (Request.QueryString["reportid"] != null || Request.QueryString["requestnum"] != null)
                {
                    user.CheckAccessRole(AccessRoleType.Edit, SpendManagementElement.Reports, true, true);
                    
                    Guid.TryParse(Request.QueryString["reportid"], out reportId);
                    cReport currentReport;
                    if (reportId != Guid.Empty)
                    {
                        var reports = new cReports(user.AccountID, user.CurrentSubAccountId);
                        currentReport = reports.getReportById(reportId);
                    }
                    else
                    {
                        requestnum = int.Parse(Request.QueryString["requestnum"]);
                        var request = (cReportRequest)Session["request" + requestnum];
                        this.ViewState["requestnum"] = requestnum;
                        currentReport = request.report;
                        this.txtreport.ReadOnly = true;
                        this.txtdescription.ReadOnly = true;
                        this.cmbfolders.Enabled = false;
                        reportId = request.report.reportid;
                    }

                    this.Master.title = $"Report: {currentReport.reportname}";
                    this.cmbreporton.Enabled = false;
                    this.txtreport.Text = currentReport.reportname;
                    this.txtdescription.Text = currentReport.description;
                    this.chkclaimant.Checked = currentReport.claimantreport;
                    if (this.cmbfolders.Items.FindByValue(currentReport.FolderID.ToString()) != null)
                    {
                        this.cmbfolders.Items.FindByValue(currentReport.FolderID.ToString()).Selected = true;
                    }

                    if (this.cmbreporton.Items.FindByValue(currentReport.basetable.TableID.ToString()) != null)
                    {
                        var currentItem =
                            this.cmbreporton.Items.FindByValue(currentReport.basetable.TableID.ToString());
                        this.cmbreporton.Items.Clear();
                        this.cmbreporton.Items.Add(currentItem);
                        this.cmbreporton.Items.FindByValue(currentReport.basetable.TableID.ToString()).Selected = true;
                    }

                    if (currentReport.Limit > 0)
                    {
                        this.txtLimitReport.Text = currentReport.Limit.ToString();
                        this.txtLimitReport.Style.Add(HtmlTextWriterStyle.Display, string.Empty);
                        this.lbRowLimit.Style.Add(HtmlTextWriterStyle.Display, string.Empty);
                        this.chkLimitReport.Checked = true;
                    }

                    this.GenerateSelectedItems(currentReport);
                    this.GenerateCriteriaItems(currentReport, user);
                    this.PopulateChartDetails(currentReport, user);
                }
                else
                {
                    this.Master.title = "New Report";
                    reportId = Guid.Empty;
                    switch (user.CurrentActiveModule)
                    {
                        case Modules.SpendManagement:
                        case Modules.SmartDiligence:
                        case Modules.Contracts:
                            // Set default report base to contract details
                            this.SetDefaultSelectedItem(new Guid(ReportTable.ContractDetails));
                            break;
                        case Modules.Expenses:
                            // Set default report base to expenses
                            this.SetDefaultSelectedItem(new Guid(ReportTable.SavedExpenses));
                            break;
                    }

                    this.PopulateDefaultChartDetails(new cReport());
                }
                
                this.Master.PageSubTitle = "Report Details";

                var currencySymbol = "£";
                if (accountProperties.BaseCurrency.HasValue)
                {
                    cCurrency currency = new cCurrencies(user.AccountID, user.CurrentSubAccountId).getCurrencyById(accountProperties.BaseCurrency.Value);
                    currencySymbol = new cGlobalCurrencies().getGlobalCurrencyById(currency.globalcurrencyid).symbol;
                }
                this.divCurrencySymbol.Value = currencySymbol;

                ClientScript.RegisterStartupScript(this.GetType(), "variables", "SEL.Reports.ReportID = '" + reportId.ToString() + "';\nSEL.Reports.RequestNumber = '" + requestnum + "'; \nSEL.Reports.Columns.Refresh();SEL.Reports.GroupReportSource();$('.SelectedType').trigger('onclick');$('.showLegend').trigger('onclick');$('.chkLimitReport').trigger('onclick');", true);
                ClientScript.RegisterStartupScript(this.GetType(), "Startup", startUpScript, true);

                if (requestnum > 0)
                {
                    this.DefaultTab = 1;
                }

                var panel = new Panel();
                var filterRuntime = new CheckBox();
                TreeControls.CreateCriteriaModalPopup(this.divFilter.Controls, GlobalVariables.StaticContentLibrary, ref panel, ref filterRuntime, domain: "SEL.Reports", filterValidationGroup: "vgFilter", renderReportOptions:true, includeModal: false);
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "filterDomIDS", "$(document).ready(function () {" + TreeControls.GeneratePanelDomIDsForFilterModal(true, true, panel, null, null, filterRuntime, "SEL.Reports", includeModal: false) + "});", true);
            }
        }

        private void GenerateCriteriaItems(cReport currentReport, CurrentUser user)
        {
            var filterData = new JavascriptTreeData();
            var lstNodes = new List<JavascriptTreeData.JavascriptTreeNode>();
            var fields = new cFields(user.AccountID);

            // g for Group - table join, k for node linK - foreign key join, n for node - field
            var duplicateFilters = new List<string>();

            if (currentReport != null)
            {
                foreach (cReportCriterion filter in currentReport.criteria)
                {
                    if (filter.field != null && filter.field.FieldName.ToLower() != "subaccountid")
                    {
                        var node = TreeViewNodes.CreateCustomEntityFilterJavascriptNode(filter, duplicateFilters, user, fields);
                        lstNodes.Add(node);
                    }
                }
            }

            filterData.data = lstNodes;

            divCriteriaValue.Value = filterData.ToString();
            
        }

        private void GenerateSelectedItems(cReport currentReport)
        {
            var lstNodes = new List<JavascriptTreeData.JavascriptTreeNode>();
            var reportService = new svcReports();
            if (currentReport != null)
            {
                foreach (cReportColumn column in currentReport.columns)
                {
                    if (column is cStandardColumn && !column.system)
                    {
                        var standard = (cStandardColumn)column;
                        if (standard.field != null)
                        {
                            var node = TreeViewNodes.CreateReportCriteriaJavascriptNode(standard, currentReport.basetable.TableID);

                            lstNodes.Add(node);
                        }
                    }

                    if (column is cCalculatedColumn)
                    {
                        var calculated = (cCalculatedColumn)column;
                        var node = reportService.FormatCalculatedField(
                            calculated.reportcolumnid.ToString(),
                            "W",
                            calculated.columnname,
                            calculated.formattedFormula);
                        node.attr.columnid = calculated.reportcolumnid.ToString();
                        lstNodes.Add(node);
                    }

                    if (column is cStaticColumn)
                    {
                        var staticColumn = (cStaticColumn)column;
                        var literal = staticColumn.literalvalue;
                        var runtime = staticColumn.runtime;
                        if (runtime)
                        {
                            literal = "<EnterAtRunTime>";
                            runtime = false;
                        }
                        var node = reportService.FormatCalculatedField(
                            staticColumn.reportcolumnid.ToString(),
                            "Z",
                            staticColumn.literalname,
                            literal,
                            runtime);
                        node.attr.columnid = staticColumn.reportcolumnid.ToString();
                        lstNodes.Add(node);
                    }
                }
            }

            divItemvalue.Value = new JavaScriptSerializer().Serialize(lstNodes);
        }

        private void PopulateChartDetails(cReport report, ICurrentUserBase user)
        {
          this.PopulateDefaultChartDetails(report);
            var chart = ReportChart.Get(report.reportid, user);
            if (chart.Reportid == report.reportid && chart.DisplayType != Chart.ChartType.Area)
            {
                this.imgBarChart.CssClass = "Type";
                switch (chart.DisplayType)
                {
                    case Chart.ChartType.Bar:
                        this.imgBarChart.CssClass = "SelectedType";
                        break;
                    case Chart.ChartType.Column:
                        this.imgColumnChart.CssClass = "SelectedType";
                        break;
                    case Chart.ChartType.Donut:
                        this.imgDonutChart.CssClass = "SelectedType";
                        break;
                    case Chart.ChartType.Dot:
                        this.imgDotChart.CssClass = "SelectedType";
                        break;
                    case Chart.ChartType.Line:
                        this.imgLineChart.CssClass = "SelectedType";
                        break;
                    case Chart.ChartType.Pie:
                        this.imgPieChart.CssClass = "SelectedType";
                        break;
                    case Chart.ChartType.Funnel:
                        this.imgFunnelChart.CssClass = "SelectedType";
                        break;
                }
                this.txtChartTitle.Text = chart.ChartTitle;
                this.chkShowLegend.Checked = chart.ShowLegend;
                if (chart.ShowLegend)
                {
                    this.lblLegendPosition.Style["display"] = string.Empty;
                    this.ddlLegendPosition.Style["display"] = string.Empty;
                    this.imgTooltipLegendPosition.Style["display"] = string.Empty;
                    this.ddlLegendPosition.SelectedValue = ((byte)chart.LegendPosition).ToString();
                }
                else
                {
                    this.lblLegendPosition.Style["display"] = "none";
                    this.ddlLegendPosition.Style["display"] = "none";
                    this.imgTooltipLegendPosition.Style["display"] = "none";
                }

                this.ddlXAxis.SelectedValue = chart.XAxis.ToString();
                this.ddlYAxis.SelectedValue = chart.YAxis.ToString();
                this.ddlGroupBy.SelectedValue = chart.GroupBy.ToString();

                this.ddlChartTitleFont.SelectedValue = chart.ChartTitleFont.ToString();
                this.txtChartTitleColour.Text = chart.ChartTitleColour;
                this.ddlTextFont.SelectedValue = chart.TextFont.ToString();
                this.txtTextFontColour.Text = chart.TextFontColour;
                this.txtTextBackgroundColour.Text = chart.TextBackgroundColour;
                this.chkShowValues.Checked = chart.ShowValues;
                this.chkShowPercent.Checked = chart.ShowPercent;
                this.chkShowLabels.Checked = chart.ShowLabels;
                this.ddlChartSize.SelectedValue = ((byte)chart.Size).ToString();
                this.ddlCombineOthers.SelectedValue = chart.CombineOthersPercentage.ToString();
                this.ddlShowChart.SelectedValue = ((byte)report.ShowChart).ToString();
            }
        }

        private void PopulateDefaultChartDetails(cReport report)
        {
            // setup chart
            this.imgAreaChart.Visible = false;
            this.imgAreaChart.Attributes.Add("onclick", "SEL.Reports.Chart.SelectedType(this, 'area');");
            this.imgBarChart.Attributes.Add("onclick", "SEL.Reports.Chart.SelectedType(this, 'bar');");
            this.imgColumnChart.Attributes.Add("onclick", "SEL.Reports.Chart.SelectedType(this, 'column');");
            this.imgDonutChart.Attributes.Add("onclick", "SEL.Reports.Chart.SelectedType(this, 'donut');");
            this.imgDotChart.Attributes.Add("onclick", "SEL.Reports.Chart.SelectedType(this, 'dot');");
            this.imgLineChart.Attributes.Add("onclick", "SEL.Reports.Chart.SelectedType(this, 'line');");
            this.imgPieChart.Attributes.Add("onclick", "SEL.Reports.Chart.SelectedType(this, 'pie');");
            this.imgFunnelChart.Attributes.Add("onclick", "SEL.Reports.Chart.SelectedType(this, 'funnel');");
            this.chkShowLegend.Attributes.Add("onclick", "SEL.Reports.Chart.ShowLegendChanged(this.checked);");
            this.ddlChartTitleFont.Items.AddRange(PopulateFontSizes(12));
            this.txtChartTitle.Text = txtreport.Text;
            this.ddlTextFont.Items.AddRange(PopulateFontSizes(10));
            this.txtChartTitleColour.Text = "000000";
            this.txtTextFontColour.Text = "000000";
            this.txtTextBackgroundColour.Text = "FFFFFF";
            this.ddlGroupBy.Items.Add(new ListItem("[None]", "-1"));
            this.ddlXAxis.Items.Add(new ListItem("[None]", "-1"));
            this.ddlYAxis.Items.Add(new ListItem("[None]", "-1"));

            this.ddlChartSize.Items.Add(new ListItem("XXS", "4"));
            this.ddlChartSize.Items.Add(new ListItem("XS", "5"));
            this.ddlChartSize.Items.Add(new ListItem("S", "6"));
            this.ddlChartSize.Items.Add(new ListItem("M", "7"));
            this.ddlChartSize.Items.Add(new ListItem("L", "8"));
            this.ddlChartSize.Items.Add(new ListItem("XL", "9"));
            this.ddlChartSize.Items.Add(new ListItem("XXL", "10"));

            this.ddlChartSize.SelectedIndex = 3;

            this.ddlLegendPosition.Items.Add(new ListItem("Top Left", "1"));
            this.ddlLegendPosition.Items.Add(new ListItem("Top Right", "2"));
            this.ddlLegendPosition.Items.Add(new ListItem("Bottom Left", "3"));
            this.ddlLegendPosition.Items.Add(new ListItem("Bottom Right", "4"));
            this.ddlLegendPosition.SelectedIndex = 0;
            this.lblLegendPosition.Style["display"] = "none";
            this.ddlLegendPosition.Style["display"] = "none";
            this.imgTooltipLegendPosition.Style["display"] = "none";
            this.ddlCombineOthers.Items.Add(new ListItem("[None]", "0"));
            for (int i = 5; i < 50; i = i + 5)
            {
                this.ddlCombineOthers.Items.Add(new ListItem(string.Format("{0} %", i), i.ToString()));
            }

            this.ddlShowChart.Items.Add(new ListItem("Chart and Data", "0"));
            this.ddlShowChart.Items.Add(new ListItem("Data only", "1"));
            this.ddlShowChart.Items.Add(new ListItem("Chart only", "2"));
            var reports = new svcReports();
            var selectedNodes =  report.reportid == Guid.Empty ? null : reports.GetSelectedNodes(report.reportid.ToString(), 0);
            var order = 0;
            if (selectedNodes?.data != null)
            {
                foreach (JavascriptTreeData.JavascriptTreeNode treeNode in selectedNodes.data)
                {
                    this.ddlXAxis.Items.Add(new ListItem(treeNode.data, order.ToString()));
                    if ((bool)treeNode.metadata["GroupBy"] == true)
                    {
                        this.ddlGroupBy.Items.Add(new ListItem(treeNode.data, order.ToString()));
                    }

                    if ((bool)treeNode.metadata["Count"] || (bool)treeNode.metadata["Sum"] || (bool)treeNode.metadata["Average"] || (bool)treeNode.metadata["Max"] || (bool)treeNode.metadata["Min"])
                    {
                        this.ddlYAxis.Items.Add(new ListItem(treeNode.data, order.ToString()));
                    }

                    order++;
                }
            }
        }

        private static ListItem[] PopulateFontSizes(int defaultSize)
        {
            var result = new List<ListItem>();
            for (var i = 20; i > 0; i--)
            {
                var newItem = new ListItem(i + "pt", i.ToString());
                if (i == defaultSize)
                {
                    newItem.Selected = true;
                }

                result.Add(newItem);
            }

            return result.ToArray();
        }

        private void SetDefaultSelectedItem(Guid tableId)
        {
            if (this.cmbreporton.Items.FindByValue(tableId.ToString()) != null)
            {
                this.cmbreporton.Items.FindByValue(tableId.ToString()).Selected = true;
            }
        }
    }
}