using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Spend_Management.shared.reports
{
    using System.Configuration;
    using System.Linq;

    using BusinessLogic;
    using BusinessLogic.DataConnections;
    using BusinessLogic.GeneralOptions;

    using SpendManagementHelpers.TreeControl;

    using SpendManagementLibrary;
    using SpendManagementLibrary.Definitions.JoinVia;

    using Spend_Management.shared.code.Helpers;

    public partial class View : System.Web.UI.Page
    {
        /// <summary>
        /// An instance of <see cref="IDataFactory{TComplexType,TPrimaryKeyDataType}"/> to get a <see cref="IGeneralOptions"/>
        /// </summary>
        [Dependency]
        public IDataFactory<IGeneralOptions, int> GeneralOptionsFactory { get; set; }


        public int requestnumber;
        public string reportName;

        public string reportId;

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
            Session["ReportViewer"] = true;  
            
        if (IsPostBack == false)
        {
            CurrentUser user = cMisc.GetCurrentUser();
            cColours clscolours = new cColours(user.AccountID, user.CurrentSubAccountId, user.CurrentActiveModule);
            this.BorderColour = clscolours.tableHeaderBGColour;
            this.TextColour = clscolours.tableHeaderTxtColour;
            ViewState["accountid"] = user.AccountID;
            ViewState["employeeid"] = user.EmployeeID;
            ViewState["subaccountid"] = user.CurrentSubAccountId;

            Guid reportid = new Guid(Request.QueryString["reportid"]);
            ViewState["reportid"] = reportid;
            this.reportId = reportid.ToString();
            ViewState["reportArea"] = ReportArea.Custom;

            bool claimants = user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.Reports, false) == false;

            IReports clsreports = (IReports)Activator.GetObject(typeof(IReports), ConfigurationManager.AppSettings["ReportsServicePath"] + "/reports.rem");
            cReport rpt = clsreports.getReportById(user.AccountID, reportid);
            Title = rpt.reportname;
            string convertedReportName = rpt.reportname.Replace("'", string.Empty);
            Master.PageSubTitle = string.Empty;
            Master.title = convertedReportName;
            switch (user.CurrentActiveModule)
            {
                case Modules.contracts:
                    Master.helpid = 1027;
                    break;
                default:
                    Master.helpid = 1115;
                    break;
            }

            cReportColumn col;
            cStandardColumn tmpCol;
            cFields clsfields = new cFields((int)ViewState["accountid"]);

            rpt = this.AddMissingColumns(rpt, user, clsfields);
            cReportRequest clsrequest;

            if (Request.QueryString["callback"] != null)
            {
                int item = int.Parse(Request.QueryString["item"]);

                if (item == 1)
                {
                    int columnnum = int.Parse(this.Request.Form["column"]);

                    this.requestnumber = int.Parse(this.Request.QueryString["requestnum"]);
                    cReportRequest drillrequest = (cReportRequest)this.Session["request" + this.requestnumber];
                    Guid drilldownreportid = new Guid(this.Request.QueryString["drilldownreportid"].ToString());
                    cReport drilldownrpt;

                    cReportColumn column;
                    if (reportid != drilldownreportid || drillrequest.report.getReportType() == ReportType.Summary) // copy criteria from orignal report
                    {
                        if (drillrequest.report.getReportType() == ReportType.Summary)
                        {
                            cExportOptions drilloptions = clsreports.getExportOptions(
                                user.AccountID,
                                user.EmployeeID,
                                drillrequest.report.reportid);
                            cMisc clsmisc = new cMisc(user.AccountID);
                            var generalOptions = this.GeneralOptionsFactory[user.CurrentSubAccountId].WithReport();
                            if (drilloptions.drilldownreport != Guid.Empty)
                            {
                                drilldownreportid = drilloptions.drilldownreport;
                            }
                            else if (generalOptions.Report.DrilldownReport != Guid.Empty)
                            {
                                drilldownreportid = generalOptions.Report.DrilldownReport.Value;
                            }
                        }

                        cReport currentRpt = drillrequest.report;
                        column = (cReportColumn)currentRpt.columns[columnnum];
                        drilldownrpt = clsreports.getReportById(user.AccountID, drilldownreportid);
                        drilldownrpt.swapCriteria(currentRpt.criteria);
                    }
                    else
                    {
                        drilldownrpt = (cReport)drillrequest.report.Clone();
                        column = (cReportColumn)drilldownrpt.columns[columnnum];
                    }

                    if ((drilldownrpt.basetable.TableID == new Guid("0efa50b5-da7b-49c7-a9aa-1017d5f741d0")
                         || drilldownrpt.basetable.TableID == new Guid("d70d9e5f-37e2-4025-9492-3bcf6aa746a8"))
                        && drilldownrpt.getReportType() == ReportType.Item) //add claimid for view claim link
                    {
                        col = new cStandardColumn(
                            new Guid(),
                            drilldownrpt.reportid,
                            ReportColumnType.Standard,
                            ColumnSort.None,
                            drilldownrpt.columns.Count + 1,
                            clsfields.GetFieldByID(new Guid("e3af2b67-a613-437e-aabf-6853c4553977")),
                            false,
                            false,
                            false,
                            false,
                            false,
                            false,
                            true);
                        col.system = true;
                        drilldownrpt.columns.Add(col);

                        if (drilldownrpt.basetable.TableID == new Guid("d70d9e5f-37e2-4025-9492-3bcf6aa746a8"))
                        {
                            col = new cStandardColumn(
                                new Guid(),
                                drilldownrpt.reportid,
                                ReportColumnType.Standard,
                                ColumnSort.None,
                                drilldownrpt.columns.Count + 1,
                                clsfields.GetFieldByID(new Guid("a528de93-3037-46f6-974c-a76bd0c8642a")),
                                false,
                                false,
                                false,
                                false,
                                false,
                                false,
                                true);
                            col.system = true;
                            drilldownrpt.columns.Add(col);
                        }
                    }

                    if ((drilldownrpt.basetable.TableID.ToString().ToUpper() == ReportTable.ContractDetails
                         || drilldownrpt.basetable.TableID.ToString().ToUpper() == ReportTable.ContractProductDetails)
                        && drilldownrpt.getReportType() == ReportType.Item) //add contractid for view contract link
                    {
                        col = new cStandardColumn(
                            new Guid(),
                            drilldownrpt.reportid,
                            ReportColumnType.Standard,
                            ColumnSort.None,
                            drilldownrpt.columns.Count + 1,
                            clsfields.GetFieldByID(new Guid(ReportKeyFields.ContractDetails_ContractId)),
                            false,
                            false,
                            false,
                            false,
                            false,
                            false,
                            true);
                        col.system = true;

                        drilldownrpt.columns.Add(col);

                        foreach (cReportColumn repCol in drilldownrpt.columns)
                        {
                            if (repCol.columntype != ReportColumnType.Standard) continue;

                            tmpCol = (cStandardColumn)repCol;
                            if (tmpCol.field.TableID.ToString() == ReportTable.ContractProductDetails)
                            {
                                cReportColumn CPcol = new cStandardColumn(
                                    new Guid(),
                                    drilldownrpt.reportid,
                                    ReportColumnType.Standard,
                                    ColumnSort.None,
                                    drilldownrpt.columns.Count + 1,
                                    clsfields.GetFieldByID(new Guid(ReportKeyFields.ContractProducts_ConProdId)),
                                    false,
                                    false,
                                    false,
                                    false,
                                    false,
                                    false,
                                    true);
                                CPcol.system = true;
                                drilldownrpt.columns.Add(CPcol);
                                break;
                            }
                        }
                    }

                    // if supplier report, put View Supplier link column on the report
                    if ((drilldownrpt.basetable.TableID.ToString().ToUpper() == ReportTable.SupplierDetails
                         || drilldownrpt.basetable.TableID.ToString().ToUpper() == ReportTable.SupplierContacts)
                        && drilldownrpt.getReportType() == ReportType.Item)
                    {
                        col = new cStandardColumn(
                            new Guid(),
                            drilldownrpt.reportid,
                            ReportColumnType.Standard,
                            ColumnSort.None,
                            drilldownrpt.columns.Count + 1,
                            clsfields.GetFieldByID(new Guid(ReportKeyFields.SupplierDetails_SupplierId)),
                            false,
                            false,
                            false,
                            false,
                            false,
                            false,
                            true);
                        col.system = true;
                        drilldownrpt.columns.Add(col);
                    }

                    // if a task report, add a view task link column on the report
                    if (drilldownrpt.basetable.TableID.ToString().ToUpper() == ReportTable.Tasks
                        && drilldownrpt.getReportType() == ReportType.Item)
                    {
                        col = new cStandardColumn(
                            new Guid(),
                            rpt.reportid,
                            ReportColumnType.Standard,
                            ColumnSort.None,
                            drilldownrpt.columns.Count + 1,
                            clsfields.GetFieldByID(new Guid(ReportKeyFields.Tasks_TaskId)),
                            false,
                            false,
                            false,
                            false,
                            false,
                            false,
                            true);
                        col.system = true;
                        drilldownrpt.columns.Add(col);
                    }

                    object value = this.Request.Form["value"];

                    ConditionType contype = ConditionType.Equals;

                    if (column.columntype == ReportColumnType.Standard)
                    {
                        object[] values = new object[1];

                        cStandardColumn standardcol = (cStandardColumn)column;
                        if (value.ToString() != "null" && !string.IsNullOrWhiteSpace(value.ToString()))
                        {
                            switch (standardcol.field.FieldType)
                            {
                                case "S":
                                case "FS":
                                case "LT":
                                case "T":
                                    value = value.ToString();
                                    break;
                                case "R":
                                    // relationship text box, so get the id
                                    DBConnection db = new DBConnection(cAccounts.getConnectionString(user.AccountID));
                                    string sql =
                                        "select [" + standardcol.field.GetRelatedTable().GetPrimaryKey().FieldName
                                                   + "] from [" + standardcol.field.GetRelatedTable().TableName
                                                   + "] where ["
                                                   + standardcol.field.GetRelatedTable().GetKeyField().FieldName
                                                   + "] = @strVal";
                                    db.sqlexecute.Parameters.AddWithValue("@strVal", value.ToString());
                                    value = db.getcount(sql);
                                    break;
                                case "DT":
                                    value = DateTime.Parse(value.ToString());
                                    break;
                                case "D":
                                    value = DateTime.Parse(value.ToString());
                                    contype = ConditionType.On;
                                    break;
                                case "N":
                                case "M":
                                case "FD":
                                case "C":
                                case "A":
                                case "F":
                                    if (standardcol.field.ValueList)
                                    {
                                        foreach (KeyValuePair<object, string> kvp in standardcol.field.ListItems)
                                        {
                                            if (kvp.Value == (string)value)
                                            {
                                                value = kvp.Key;
                                                break;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        value = Convert.ToDecimal(value);
                                    }

                                    break;
                                case "X":
                                    value = Convert.ToBoolean(value);
                                    break;
                                case "Y":
                                    value = value.ToString();
                                    break;
                            }

                            values[0] = value;
                        }
                        else
                        {
                            values[0] = DBNull.Value;
                            contype = ConditionType.DoesNotContainData;
                        }

                        int order = drilldownrpt.criteria.Count + 1;
                        cReportCriterion criteria = new cReportCriterion(
                            Guid.Empty,
                            drilldownrpt.reportid,
                            standardcol.field,
                            contype,
                            values,
                            new object[1],
                            ConditionJoiner.And,
                            order,
                            false,
                            0);
                        criteria.changeToDrilldown();
                        drilldownrpt.addCriteria(criteria);

                        //create new request
                        if (this.Session["currentrequestnum"] == null)
                        {
                            this.Session["currentrequestnum"] = 0;
                        }

                        int currentrequestnum = (int)this.Session["currentrequestnum"];
                        this.requestnumber = currentrequestnum + 1;
                        this.Session["currentrequestnum"] = this.requestnumber;

                        cExportOptions clsoptions = clsreports.getExportOptions(
                            user.AccountID,
                            user.EmployeeID,
                            reportid);
                        List<int> reportRoles = new List<int>();
                        AccessRoleLevel roleLevel = user.HighestAccessLevel;

                        if (cReport.canFilterByRole(drillrequest.report.basetable.TableID))
                        {
                            if (roleLevel == AccessRoleLevel.SelectedRoles)
                            {
                                // get the roles that can be reported on. If > 1 role with SelectedRoles, then need to merge
                                cAccessRoles roles = new cAccessRoles(
                                    user.AccountID,
                                    cAccounts.getConnectionString(user.AccountID));
                                List<int> lstAccessRoles =
                                    user.Employee.GetAccessRoles().GetBy(user.CurrentSubAccountId);

                                foreach (int emp_arId in lstAccessRoles)
                                {
                                    cAccessRole empRole = roles.GetAccessRoleByID(emp_arId);
                                    foreach (int arId in empRole.AccessRoleLinks)
                                    {
                                        if (!reportRoles.Contains(arId))
                                        {
                                            reportRoles.Add(arId);
                                        }
                                    }
                                }
                            }
                        }

                        clsrequest = new cReportRequest(
                            (int)this.ViewState["accountid"],
                            user.CurrentSubAccountId,
                            this.requestnumber,
                            (cReport)drilldownrpt,
                            ExportType.Viewer,
                            clsoptions,
                            claimants,
                            user.EmployeeID,
                            roleLevel);

                        clsrequest.AccessLevelRoles = reportRoles.ToArray();

                        this.Session["request" + this.requestnumber] = clsrequest;
                        this.Response.Write(this.requestnumber);
                        this.Response.Flush();
                        this.Response.End();
                    }
                }
            }

            if (!rpt.SubAccountID.HasValue)
            {
                rpt.SubAccountID = user.CurrentSubAccountId;
            }

            Session["rpt" + rpt.reportid] = rpt;
            ViewState["reportid"] = rpt.reportid;

            if (Request.QueryString["requestnum"] != null)
            {
                if (Request.QueryString["requestnum"].IndexOf(",") != -1)
                {
                    string[] arrrequest = Request.QueryString["requestnum"].Split(',');
                    this.requestnumber = int.Parse(arrrequest[0].Trim());
                }
                else
                {
                    this.requestnumber = int.Parse(Request.QueryString["requestnum"]);
                }
                clsrequest = (cReportRequest)Session["request" + this.requestnumber];
                clsrequest.report = this.AddMissingColumns(clsrequest.report, user, clsfields);
            }
            else
            {
                if (Session["currentrequestnum"] == null)
                {
                    Session["currentrequestnum"] = 0;
                }


                int currentrequestnum = (int)Session["currentrequestnum"];
                this.requestnumber = currentrequestnum + 1;
                Session["currentrequestnum"] = this.requestnumber;
                Session["runtimeUpdate"] = false;
                cExportOptions clsoptions = clsreports.getExportOptions(user.AccountID, user.EmployeeID, reportid);
                AccessRoleLevel roleLevel = user.HighestAccessLevel;
                clsrequest = new cReportRequest((int)ViewState["accountid"], user.CurrentSubAccountId, this.requestnumber, (cReport)Session["rpt" + ViewState["reportid"]], ExportType.Preview, clsoptions, claimants, user.EmployeeID, roleLevel);

                if (roleLevel == AccessRoleLevel.SelectedRoles && cReport.canFilterByRole(clsrequest.report.basetable.TableID))
                {
                    // get the roles that can be reported on. If > 1 role with SelectedRoles, then need to merge
                    cAccessRoles roles = new cAccessRoles(user.AccountID, cAccounts.getConnectionString(user.AccountID));
                    List<int> reportRoles = new List<int>();
                    List<int> lstAccessRoles = user.Employee.GetAccessRoles().GetBy(user.CurrentSubAccountId);

                    foreach (int emp_arId in lstAccessRoles)
                    {
                        cAccessRole empRole = roles.GetAccessRoleByID(emp_arId);
                        foreach (int arId in empRole.AccessRoleLinks)
                        {
                            if (!reportRoles.Contains(arId))
                            {
                                reportRoles.Add(arId);
                            }
                        }
                    }
                    clsrequest.AccessLevelRoles = reportRoles.ToArray();
                }
                Session["request" + this.requestnumber] = clsrequest;
                if (rpt.hasRuntimeCriteria())
                {
                    Response.Redirect("enteratruntime.aspx?requestnum=" + this.requestnumber, true);
                }
            }

            ViewState["requestnum"] = this.requestnumber;

            cAuditLog clsaudit = new cAuditLog();
            clsaudit.addRecord(SpendManagementElement.Reports, "Report Ran:" + rpt.reportname, 0);
            ClientScript.RegisterClientScriptBlock(typeof(System.String), "reports", "var reportid = '" + rpt.reportid + "';\n var requestNum = " + this.requestnumber + ";\nvar employeeid = " + user.EmployeeID + ";\n var accountid = " + user.AccountID + "; self.focus();", true);

            #region toolbar

            if (string.IsNullOrEmpty(clsrequest.report.StaticReportSQL))
            {
                this.litoptions.Text += "<A href=\"javascript:changeColumns();\" class=\"submenuitem\">Change Report</A>";
                this.litoptions.Text += "<a href=\"javascript:saveDialog();\" class=\"submenuitem\">Save As</a>";
                this.txtReportName.Text = string.Format("Copy of {0}", rpt.reportname);

                var clsfolders = new cReportFolders(user.AccountID);
                this.cmbCategory.Items.AddRange(clsfolders.CreateDropDown());
                if (rpt.FolderID != Guid.Empty)
                {
                    if (this.cmbCategory.Items.FindByValue(rpt.FolderID.ToString()) != null)
                    {
                        this.cmbCategory.Items.FindByValue(rpt.FolderID.ToString()).Selected = true;
                    }
                }
                }

            this.hrefExcel.HRef = "javascript:exportReport(2,'" + reportid + "');";
            this.hrefPivot.HRef = "javascript:exportReport(5,'" + reportid + "');";
            this.litoptions2.Text += string.Format("<A href=\"javascript:exportReport(3,'{0}');\" class=\"submenuitem\">Export to CSV</A>", reportid);
            this.litoptions2.Text += string.Format("<A href=\"javascript:exportReport(4,'{0}');\" class=\"submenuitem\">Export to Flat File</A>", reportid);
            if (string.IsNullOrEmpty(clsrequest.report.StaticReportSQL))
            {
                this.litoptions3.Text += "<A href=\"javascript:exportOptions();\" class=\"submenuitem\">Export Options</A>";
                this.litoptions3.Text += "<a href=\"javascript:showDrillDownReports();\" class=\"submenuitem\">Drilldown Report</a>";
            }

            this.litoptions3.Text += string.Format("<input type=hidden name=drilldownreportid id=drilldownreportid value=\"{0}\" />", rpt.reportid);
            this.litoptions3.Text += string.Format("<input type=hidden name=basetable id=basetable value=\"{0}\" />", rpt.basetable.TableID);
            var clsAudit = new cAuditLog(user.AccountID, user.EmployeeID);
            clsAudit.addRecord(SpendManagementElement.Reports, string.Format("Report {0} ID {1} was started by employee {2}", clsrequest.report.reportname, clsrequest.report.reportid, clsrequest.employeeid), 0);
            this.GenerateCriteriaItems(clsrequest.report, user);
            clsreports.createReport(clsrequest);
            #endregion
        }

            var panel = new Panel();
            var filterRuntime = new CheckBox();
            TreeControls.CreateCriteriaModalPopup(this.divFilter.Controls, GlobalVariables.StaticContentLibrary, ref panel, ref filterRuntime, domain: "SEL.Reports", filterValidationGroup: "vgFilter", renderReportOptions:true, includeModal: false);
            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "filterDomIDS", "$(document).ready(function () {" + TreeControls.GeneratePanelDomIDsForFilterModal(true, true, panel, null, null, filterRuntime, "SEL.Reports", includeModal: false) + "});", true);

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

            /// <summary>
    /// Adds missing columns for view claim link, view supplier details etc.
    /// </summary>
    /// <param name="rpt">Current report</param>
    /// <param name="user">Current user</param>
    /// <param name="clsfields">List of fields</param>
    /// <returns></returns>
    private cReport AddMissingColumns(cReport rpt, CurrentUser user, cFields clsfields)
    {
        cReportColumn col;
        cStandardColumn tmpCol;
        if ((rpt.basetable.TableID == new Guid(ReportTable.Claims) || rpt.basetable.TableID == new Guid(ReportTable.SavedExpenses)) && rpt.getReportType() == ReportType.Item) //add claimid for view claim link
        {
            var claimIdField = clsfields.GetFieldByID(new Guid(ReportKeyFields.ClaimsClaimId));

            if (!FieldInReport(rpt, claimIdField))
            {
                col = new cStandardColumn(new Guid(), rpt.reportid, ReportColumnType.Standard, ColumnSort.None, rpt.columns.Count + 1, claimIdField, false, false, false, false, false, false, true);
                col.system = true;    
                if (rpt.UseJoinVia && rpt.basetable.TableID != new Guid(ReportTable.Claims))
                {
                    var joinVias = new JoinVias(user);
                    var joinViaList = new SortedList<int, JoinViaPart>();
                    var joinPart = new JoinViaPart(
                        new Guid(ReportKeyFields.SavedexpensesClaimId),
                        JoinViaPart.IDType.Field);
                    joinViaList.Add(0, joinPart);
                    var joinVia = new JoinVia(0, "ClaimID", Guid.Empty, joinViaList);
                    col.JoinVia = joinVias.GetJoinViaByID(joinVias.SaveJoinVia(joinVia));
                }

                rpt.columns.Add(col);
            }

            if (rpt.basetable.TableID == new Guid(ReportTable.SavedExpenses))
            {
                col = new cStandardColumn(new Guid(), rpt.reportid, ReportColumnType.Standard, ColumnSort.None, rpt.columns.Count + 1, clsfields.GetFieldByID(new Guid(ReportKeyFields.SavedexpensesExpenseId)), false, false, false, false, false, false, true);
                col.system = true;
                rpt.columns.Add(col);
            }
        }

        if ((rpt.basetable.TableID.ToString().ToUpper() == ReportTable.ContractDetails || rpt.basetable.TableID.ToString().ToUpper() == ReportTable.ContractProductDetails) && rpt.getReportType() == ReportType.Item) //add contractid for view contract link
        {
            var contractIdField = clsfields.GetFieldByID(new Guid(ReportKeyFields.ContractDetails_ContractId));
            if (!FieldInReport(rpt, contractIdField))
            {
                col = new cStandardColumn(new Guid(), rpt.reportid, ReportColumnType.Standard, ColumnSort.None, rpt.columns.Count + 1, contractIdField, false, false, false, false, false, false, true);
                col.system = true;

                rpt.columns.Add(col);
            }
            
            foreach (cReportColumn repCol in rpt.columns)
            {
                if (repCol.columntype == ReportColumnType.Standard)
                {
                    tmpCol = (cStandardColumn)repCol;
                    if (tmpCol.field.TableID.ToString() == ReportTable.ContractProductDetails)
                    {
                        cReportColumn CPcol = new cStandardColumn(new Guid(), rpt.reportid, ReportColumnType.Standard, ColumnSort.None, rpt.columns.Count + 1, clsfields.GetFieldByID(new Guid(ReportKeyFields.ContractProducts_ConProdId)), false, false, false, false, false, false, true);
                        CPcol.system = true;
                        rpt.columns.Add(CPcol);
                        break;
                    }
                }
            }
        }
        // if supplier report, put View Supplier link column on the report
        if ((rpt.basetable.TableID.ToString().ToUpper() == ReportTable.SupplierDetails || rpt.basetable.TableID.ToString().ToUpper() == ReportTable.SupplierContacts) && rpt.getReportType() == ReportType.Item)
        {
            var supplierIdField = clsfields.GetFieldByID(new Guid(ReportKeyFields.SupplierDetails_SupplierId));
            if (!FieldInReport(rpt, supplierIdField))
            {
                col = new cStandardColumn(new Guid(), rpt.reportid, ReportColumnType.Standard, ColumnSort.None, rpt.columns.Count + 1, supplierIdField, false, false, false, false, false, false, true);
                col.system = true;
                rpt.columns.Add(col);    
            }
        }

        // if a task report, add a view task link column on the report
        if (rpt.basetable.TableID.ToString().ToUpper() == ReportTable.Tasks && rpt.getReportType() == ReportType.Item)
        {
            var taskIdField = clsfields.GetFieldByID(new Guid(ReportKeyFields.Tasks_TaskId));
            if (!FieldInReport(rpt, taskIdField))
            {
                col = new cStandardColumn(new Guid(), rpt.reportid, ReportColumnType.Standard, ColumnSort.None, rpt.columns.Count + 1, taskIdField, false, false, false, false, false, false, true);
                col.system = true;
                rpt.columns.Add(col);    
            }
        }

        return rpt;
    }

        private static bool FieldInReport(cReport rpt, cField claimIdField)
        {
            var found = false;
            foreach (cReportColumn column in rpt.columns)
            {
                if (column is cStandardColumn && ((cStandardColumn)column).field.FieldID == claimIdField.FieldID)
                {
                    found = true;
                    break;
                }
            }

            return found;
        }
    }
}