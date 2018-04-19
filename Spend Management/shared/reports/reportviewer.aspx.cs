using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using BusinessLogic;
using BusinessLogic.DataConnections;
using BusinessLogic.GeneralOptions;

using Infragistics.WebUI.CalcEngine;
using Infragistics.WebUI.UltraWebGrid;

using SpendManagementLibrary.Definitions.JoinVia;
using SpendManagementLibrary.Helpers;
using SpendManagementLibrary;
using SpendManagementLibrary.Logic_Classes.Fields;

using Spend_Management;
using Spend_Management.shared.webServices;

public partial class reports_reportviewer : System.Web.UI.Page
{
    private cReport reqReport;
    public int requestnum;
    public string reportName;
    int nTotalColumnId;
    decimal dCurGrandTotal;
    private CustomEntityImageData _customEntityImageData;

    /// <summary>
    /// An instance of <see cref="IDataFactory{IGeneralOptions,Int32}"/> to get a <see cref="IGeneralOptions"/>
    /// </summary>
    [Dependency]
    public IDataFactory<IGeneralOptions, int> GeneralOptionsFactory { get; set; }

    /// <summary>
    /// Gets an instance of <see cref="CustomEntityImageData"/>
    /// </summary>
    public CustomEntityImageData CustomEntityImageData
    {
        get
        {
            if (this._customEntityImageData != null)
            {
                return this._customEntityImageData;              
            }

            CurrentUser user = cMisc.GetCurrentUser();
            this._customEntityImageData = new CustomEntityImageData(user.AccountID);

            return this._customEntityImageData;
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        Session["ReportViewer"] = true;  
            
        if (IsPostBack == false)
        {
            CurrentUser user = cMisc.GetCurrentUser();
            ViewState["accountid"] = user.AccountID;
            ViewState["employeeid"] = user.EmployeeID;
            ViewState["subaccountid"] = user.CurrentSubAccountId;

            Guid reportid = new Guid(Request.QueryString["reportid"]);
            ViewState["reportid"] = reportid;
            ViewState["reportArea"] = ReportArea.Custom;

            bool claimants = user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.Reports, false) == false;

            IReports clsreports = (IReports)Activator.GetObject(typeof(IReports), ConfigurationManager.AppSettings["ReportsServicePath"] + "/reports.rem");
            cReport rpt = clsreports.getReportById(user.AccountID, reportid);
            reqReport = rpt;
            Title = rpt.reportname;
            string convertedReportName = rpt.reportname.Replace("'", "");
            reportName = "Copy of " + convertedReportName;
            Master.PageSubTitle = Title;
            Master.title = "View Report";

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

                switch(item)
                {
                    case 1: //drilldown
                        int columnnum = int.Parse(Request.Form["column"]);

                        requestnum = int.Parse(Request.QueryString["requestnum"]);
                        cReportRequest drillrequest = (cReportRequest) Session["request" + requestnum];
                        Guid drilldownreportid = new Guid(Request.QueryString["drilldownreportid"].ToString());
                        cReport drilldownrpt;


                        cReportColumn column;
                        if(reportid != drilldownreportid || drillrequest.report.getReportType() == ReportType.Summary) //copy criteria from orignal report
                        {
                            if(drillrequest.report.getReportType() == ReportType.Summary)
                            {
                                cExportOptions drilloptions = clsreports.getExportOptions(user.AccountID, user.EmployeeID, drillrequest.report.reportid);
                                cMisc clsmisc = new cMisc(user.AccountID);

                                var generalOptions = this.GeneralOptionsFactory[user.CurrentSubAccountId].WithReport();

                                if(drilloptions.drilldownreport != Guid.Empty)
                                {
                                    drilldownreportid = drilloptions.drilldownreport;
                                }
                                else if(generalOptions.Report.DrilldownReport != Guid.Empty)
                                {
                                    drilldownreportid = (Guid) generalOptions.Report.DrilldownReport;
                                }
                            }

                            cReport currentRpt = drillrequest.report;
                            column = (cReportColumn) currentRpt.columns[columnnum - 1];
                            drilldownrpt = clsreports.getReportById(user.AccountID, drilldownreportid);
                            drilldownrpt.swapCriteria(currentRpt.criteria);
                        }
                        else
                        {
                            drilldownrpt = (cReport) drillrequest.report.Clone();
                            column = (cReportColumn) drilldownrpt.columns[columnnum - 1];
                        }

                        if((drilldownrpt.basetable.TableID == new Guid("0efa50b5-da7b-49c7-a9aa-1017d5f741d0") || drilldownrpt.basetable.TableID == new Guid("d70d9e5f-37e2-4025-9492-3bcf6aa746a8")) && drilldownrpt.getReportType() == ReportType.Item) //add claimid for view claim link
                        {
                            col = new cStandardColumn(new Guid(), drilldownrpt.reportid, ReportColumnType.Standard, ColumnSort.None, drilldownrpt.columns.Count + 1, clsfields.GetFieldByID(new Guid("e3af2b67-a613-437e-aabf-6853c4553977")), false, false, false, false, false, false, true);
                            col.system = true;
                            drilldownrpt.columns.Add(col);

                            if(drilldownrpt.basetable.TableID == new Guid("d70d9e5f-37e2-4025-9492-3bcf6aa746a8"))
                            {
                                col = new cStandardColumn(new Guid(), drilldownrpt.reportid, ReportColumnType.Standard, ColumnSort.None, drilldownrpt.columns.Count + 1, clsfields.GetFieldByID(new Guid("a528de93-3037-46f6-974c-a76bd0c8642a")), false, false, false, false, false, false, true);
                                col.system = true;
                                drilldownrpt.columns.Add(col);
                            }
                        }

                        if((drilldownrpt.basetable.TableID.ToString().ToUpper() == ReportTable.ContractDetails || drilldownrpt.basetable.TableID.ToString().ToUpper() == ReportTable.ContractProductDetails) && drilldownrpt.getReportType() == ReportType.Item) //add contractid for view contract link
                        {
                            col = new cStandardColumn(new Guid(), drilldownrpt.reportid, ReportColumnType.Standard, ColumnSort.None, drilldownrpt.columns.Count + 1, clsfields.GetFieldByID(new Guid(ReportKeyFields.ContractDetails_ContractId)), false, false, false, false, false, false, true);
                            col.system = true;

                            drilldownrpt.columns.Add(col);

                            foreach(cReportColumn repCol in drilldownrpt.columns)
                            {
                                if(repCol.columntype != ReportColumnType.Standard) continue;

                                tmpCol = (cStandardColumn) repCol;
                                if(tmpCol.field.TableID.ToString() == ReportTable.ContractProductDetails)
                                {
                                    cReportColumn CPcol = new cStandardColumn(new Guid(), drilldownrpt.reportid, ReportColumnType.Standard, ColumnSort.None, drilldownrpt.columns.Count + 1, clsfields.GetFieldByID(new Guid(ReportKeyFields.ContractProducts_ConProdId)), false, false, false, false, false, false, true);
                                    CPcol.system = true;
                                    drilldownrpt.columns.Add(CPcol);
                                    break;
                                }
                            }
                        }
                        // if supplier report, put View Supplier link column on the report
                        if((drilldownrpt.basetable.TableID.ToString().ToUpper() == ReportTable.SupplierDetails || drilldownrpt.basetable.TableID.ToString().ToUpper() == ReportTable.SupplierContacts) && drilldownrpt.getReportType() == ReportType.Item)
                        {
                            col = new cStandardColumn(new Guid(), drilldownrpt.reportid, ReportColumnType.Standard, ColumnSort.None, drilldownrpt.columns.Count + 1, clsfields.GetFieldByID(new Guid(ReportKeyFields.SupplierDetails_SupplierId)), false, false, false, false, false, false, true);
                            col.system = true;
                            drilldownrpt.columns.Add(col);
                        }

                        // if a task report, add a view task link column on the report
                        if(drilldownrpt.basetable.TableID.ToString().ToUpper() == ReportTable.Tasks && drilldownrpt.getReportType() == ReportType.Item)
                        {
                            col = new cStandardColumn(new Guid(), rpt.reportid, ReportColumnType.Standard, ColumnSort.None, drilldownrpt.columns.Count + 1, clsfields.GetFieldByID(new Guid(ReportKeyFields.Tasks_TaskId)), false, false, false, false, false, false, true);
                            col.system = true;
                            drilldownrpt.columns.Add(col);
                        }

                        object value = Request.Form["value"];

                        ConditionType contype = ConditionType.Equals;

                        if(column.columntype == ReportColumnType.Standard)
                        {
                            object[] values = new object[1];

                            cStandardColumn standardcol = (cStandardColumn) column;
                            if(value.ToString() != "null" && !string.IsNullOrWhiteSpace(value.ToString()))
                            {
                                switch(standardcol.field.FieldType)
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
                                        string sql = "select [" + standardcol.field.GetRelatedTable().GetPrimaryKey().FieldName + "] from [" + standardcol.field.GetRelatedTable().TableName + "] where [" + standardcol.field.GetRelatedTable().GetKeyField().FieldName + "] = @strVal";
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
                                        if(standardcol.field.ValueList)
                                        {
                                            foreach(KeyValuePair<object, string> kvp in standardcol.field.ListItems)
                                            {
                                                if(kvp.Value == (string) value)
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
                            cReportCriterion criteria = new cReportCriterion(Guid.Empty, drilldownrpt.reportid, standardcol.field, contype, values, new object[1], ConditionJoiner.And, order, false, 0);
                            criteria.changeToDrilldown();
                            drilldownrpt.addCriteria(criteria);

                            //create new request
                            if(Session["currentrequestnum"] == null)
                            {
                                Session["currentrequestnum"] = 0;
                            }


                            int currentrequestnum = (int) Session["currentrequestnum"];
                            requestnum = currentrequestnum + 1;
                            Session["currentrequestnum"] = requestnum;


                            cExportOptions clsoptions = clsreports.getExportOptions(user.AccountID, user.EmployeeID, reportid);
                            List<int> reportRoles = new List<int>();
                            AccessRoleLevel roleLevel = user.HighestAccessLevel;

                            if(cReport.canFilterByRole(drillrequest.report.basetable.TableID))
                            {
                                if(roleLevel == AccessRoleLevel.SelectedRoles)
                                {
                                    // get the roles that can be reported on. If > 1 role with SelectedRoles, then need to merge
                                    cAccessRoles roles = new cAccessRoles(user.AccountID, cAccounts.getConnectionString(user.AccountID));
                                    List<int> lstAccessRoles = user.Employee.GetAccessRoles().GetBy(user.CurrentSubAccountId);

                                    foreach(int emp_arId in lstAccessRoles)
                                    {
                                        cAccessRole empRole = roles.GetAccessRoleByID(emp_arId);
                                        foreach(int arId in empRole.AccessRoleLinks)
                                        {
                                            if(!reportRoles.Contains(arId))
                                            {
                                                reportRoles.Add(arId);
                                            }
                                        }
                                    }
                                }
                            }

                            clsrequest = new cReportRequest((int) ViewState["accountid"], user.CurrentSubAccountId, requestnum, (cReport) drilldownrpt, ExportType.Viewer, clsoptions, claimants, user.EmployeeID, roleLevel);

                            clsrequest.AccessLevelRoles = reportRoles.ToArray();

                            Session["request" + requestnum] = clsrequest;
                            Response.Write(requestnum);
                            Response.Flush();
                            Response.End();
                        }
                        break;
                }
            }

            if (!rpt.SubAccountID.HasValue)
            {
                rpt.SubAccountID = user.CurrentSubAccountId;
            }

            Session["rpt" + rpt.reportid] = rpt;
            ViewState["reportid"] = rpt.reportid;

            gridreport.CalcManager = calcman;

            if (Request.QueryString["requestnum"] != null)
            {
                if (Request.QueryString["requestnum"].IndexOf(",") != -1)
                {
                    string[] arrrequest = Request.QueryString["requestnum"].Split(',');
                    requestnum = int.Parse(arrrequest[0].Trim());
                }
                else
                {
                    requestnum = int.Parse(Request.QueryString["requestnum"]);
                }
                clsrequest = (cReportRequest)Session["request" + requestnum];
                clsrequest.report = this.AddMissingColumns(clsrequest.report, user, clsfields);
            }
            else
            {
                if (Session["currentrequestnum"] == null)
                {
                    Session["currentrequestnum"] = 0;
                }


                int currentrequestnum = (int)Session["currentrequestnum"];
                requestnum = currentrequestnum + 1;
                Session["currentrequestnum"] = requestnum;
                Session["runtimeUpdate"] = false;
                cExportOptions clsoptions = clsreports.getExportOptions(user.AccountID, user.EmployeeID, reportid);
                AccessRoleLevel roleLevel = user.HighestAccessLevel;
                clsrequest = new cReportRequest((int)ViewState["accountid"], user.CurrentSubAccountId, requestnum, (cReport)Session["rpt" + ViewState["reportid"]], ExportType.Viewer, clsoptions, claimants, user.EmployeeID, roleLevel);

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
                Session["request" + requestnum] = clsrequest;
                if (rpt.hasRuntimeCriteria())
                {
                    Response.Redirect("enteratruntime.aspx?requestnum=" + requestnum, true);
                }
            }

            ViewState["requestnum"] = requestnum;

            litcriteria.Text = displayCriteria(clsrequest.report.criteria);

            cAuditLog clsaudit = new cAuditLog();
            clsaudit.addRecord(SpendManagementElement.Reports, "Report Ran:" + rpt.reportname, 0);
            ClientScript.RegisterClientScriptBlock(typeof(System.String), "reports", "var reportid = '" + rpt.reportid + "';\n var requestNum = " + requestnum + ";\nvar employeeid = " + user.EmployeeID + ";\n var accountid = " + user.AccountID + "; self.focus();", true);


            #region toolbar

            if (string.IsNullOrEmpty(reqReport.StaticReportSQL))
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
            if (string.IsNullOrEmpty(this.reqReport.StaticReportSQL))
            {
                this.litoptions3.Text += "<A href=\"javascript:exportOptions();\" class=\"submenuitem\">Export Options</A>";
                this.litoptions3.Text += "<a href=\"javascript:showDrillDownReports();\" class=\"submenuitem\">Drilldown Report</a>";
            }

            this.litoptions3.Text += string.Format("<input type=hidden name=drilldownreportid id=drilldownreportid value=\"{0}\" />", rpt.reportid);
            this.litoptions3.Text += string.Format("<input type=hidden name=basetable id=basetable value=\"{0}\" />", rpt.basetable.TableID);

            #endregion
        }
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
            col = new cStandardColumn(new Guid(), rpt.reportid, ReportColumnType.Standard, ColumnSort.None, rpt.columns.Count + 1, clsfields.GetFieldByID(new Guid(ReportKeyFields.ClaimsClaimId)), false, false, false, false, false, false, true);
            col.system = true;
            if (rpt.UseJoinVia && rpt.basetable.TableID != new Guid(ReportTable.Claims))
            {
                var joinVias = new JoinVias(user);
                var joinViaList = new SortedList<int, JoinViaPart>();
                var joinPart = new JoinViaPart(new Guid(ReportKeyFields.SavedexpensesClaimId),
                    JoinViaPart.IDType.Field);
                joinViaList.Add(0, joinPart);
                var joinVia = new JoinVia(0, "ClaimID", Guid.Empty, joinViaList);
                var joinViaID = joinVias.SaveJoinVia(joinVia);
                col.JoinVia = joinVias.GetJoinViaByID(joinViaID);
            }

            rpt.columns.Add(col);

            if (rpt.basetable.TableID == new Guid(ReportTable.SavedExpenses))
            {
                col = new cStandardColumn(new Guid(), rpt.reportid, ReportColumnType.Standard, ColumnSort.None, rpt.columns.Count + 1, clsfields.GetFieldByID(new Guid(ReportKeyFields.SavedexpensesExpenseId)), false, false, false, false, false, false, true);
                col.system = true;
                rpt.columns.Add(col);
            }
        }

        if ((rpt.basetable.TableID.ToString().ToUpper() == ReportTable.ContractDetails || rpt.basetable.TableID.ToString().ToUpper() == ReportTable.ContractProductDetails) && rpt.getReportType() == ReportType.Item) //add contractid for view contract link
        {
            col = new cStandardColumn(new Guid(), rpt.reportid, ReportColumnType.Standard, ColumnSort.None, rpt.columns.Count + 1, clsfields.GetFieldByID(new Guid(ReportKeyFields.ContractDetails_ContractId)), false, false, false, false, false, false, true);
            col.system = true;

            rpt.columns.Add(col);

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
            col = new cStandardColumn(new Guid(), rpt.reportid, ReportColumnType.Standard, ColumnSort.None, rpt.columns.Count + 1, clsfields.GetFieldByID(new Guid(ReportKeyFields.SupplierDetails_SupplierId)), false, false, false, false, false, false, true);
            col.system = true;
            rpt.columns.Add(col);
        }

        // if a task report, add a view task link column on the report
        if (rpt.basetable.TableID.ToString().ToUpper() == ReportTable.Tasks && rpt.getReportType() == ReportType.Item)
        {
            col = new cStandardColumn(new Guid(), rpt.reportid, ReportColumnType.Standard, ColumnSort.None, rpt.columns.Count + 1, clsfields.GetFieldByID(new Guid(ReportKeyFields.Tasks_TaskId)), false, false, false, false, false, false, true);
            col.system = true;
            rpt.columns.Add(col);
        }

        return rpt;
    }

    public void getGrid()
    {
        var clstext = new cText();
        var clsexcel = new cExcel();
        var clsRow = new cRowFunction();
        var clsColumn = new cColumnFunction();
        var column = new ColFunction();
        var clsAddress = new cAddressFunction();
        var clsReplaceText = new cReplaceTextFunction();

        this.calcman.RegisterUserDefinedFunction((UltraCalcFunction)clstext);
        this.calcman.RegisterUserDefinedFunction(clstext);
        this.calcman.RegisterUserDefinedFunction(clsexcel);
        this.calcman.RegisterUserDefinedFunction(clsRow);
        this.calcman.RegisterUserDefinedFunction(clsColumn);
        this.calcman.RegisterUserDefinedFunction(column);
        this.calcman.RegisterUserDefinedFunction(clsAddress);
        this.calcman.RegisterUserDefinedFunction(clsReplaceText);

        var clsreports = (IReports)Activator.GetObject(typeof(IReports), ConfigurationManager.AppSettings["ReportsServicePath"] + "/reports.rem");
        var clsrequest = (cReportRequest)Session["request" + ViewState["requestnum"]];
        int count = 0;
        bool joinsOk = true;
        CurrentUser user = cMisc.GetCurrentUser();

        this.divExcel.Style.Add("display", string.Empty);
        this.divPrinterFriendly.Style.Add("display", string.Empty);
        this.divPivot.Style.Add("display", string.Empty);

        try
        {
            count = clsreports.getReportCount(clsrequest);
        }
        catch (KeyNotFoundException ex)
        {
            this.gridreport.Visible = false;
            this.lblcountmsg.Text = "A required join could not be found.";
            this.lblcountmsg.Visible = true;
            this.reportStatusInformation.Visible = false;
            joinsOk = false;
        }

        if (count == 0 && joinsOk)
        {
            this.gridreport.Visible = false;
            this.reportStatusInformation.Visible = false;
            this.lblcountmsg.Text = "No data to display.";
            this.lblcountmsg.Style.Add("color", "#000");
            this.lblcountmsg.Visible = true;
        }
        else if (count > 65536 && joinsOk)
        {
            this.gridreport.Visible = false;
            this.lblcountmsg.Text = string.Format("The report you requested exceeds 65,536 records so it is too large to view on a web page, or be exported to excel. We recommend that you <a href=\"javascript:exportReport(3,'{0}');\">export it as a CSV file</a> instead. Please be patient as this could take a long time to generate.", this.reqReport.reportid);
            this.lblcountmsg.Visible = true;
            this.divExcel.Style.Add("display", "none");
            this.divPivot.Style.Add("display", "none");
            this.divPrinterFriendly.Style.Add("display", "none");
        }
        else if (count > 10000 && joinsOk)
        {
            this.gridreport.Visible = false;
            this.lblcountmsg.Text = string.Format("The report you requested exceeds 10,000 records so it is too large to view on a web page, we recommend that you <a href=\"javascript:exportReport(2,'{0}');\">export it as an Excel file</a> instead.", this.reqReport.reportid);
            this.lblcountmsg.Visible = true;
            this.divPrinterFriendly.Style.Add("display", "none");
        }
        else
        {
            var functions = new ReportFunctions();
            DataSet ds;

            if (this.Session[string.Format("{0}_{1}_{2}", this.ViewState["requestnum"], user.EmployeeID, user.AccountID)] != null)
            {
                if ((bool)Session["runtimeUpdate"])
                {
                    ds = clsreports.refreshReportData(clsrequest);
                    this.Session["runtimeUpdate"] = false;
                    this.Session[string.Format("{0}_{1}_{2}", this.ViewState["requestnum"], user.EmployeeID, user.AccountID)] = ds;
                    this.gridreport.DataSource = ds;
                    this.gridreport.Visible = true;
                    this.reportStatusInformation.Visible = false;
                    Page.Title = clsrequest.report.reportname;
                    this.displayChart(ds, clsrequest, user);
                }
                else
                {
                    ds = (DataSet)Session[string.Format("{0}_{1}_{2}", this.ViewState["requestnum"], user.EmployeeID, user.AccountID)];
                    this.gridreport.DataSource = ds;
                    this.gridreport.Visible = true;
                    this.reportStatusInformation.Visible = false;
                    Page.Title = clsrequest.report.reportname;
                }
            }
            else if (functions.getReportProgress(clsrequest.requestnum) != null && functions.getReportProgress(clsrequest.requestnum)[0].ToString() == "Complete")
            {
                if (this.Session[string.Format("{0}_{1}_{2}", this.ViewState["requestnum"], user.EmployeeID, user.AccountID)] == null)
                {
                    try
                    {
                        ds = (DataSet)functions.getReportData(clsrequest.requestnum);
                        this.Session[string.Format("{0}_{1}_{2}", this.ViewState["requestnum"], user.EmployeeID, user.AccountID)] = ds;
                    }
                    catch (Exception ex)
                    {
                        ds = new DataSet();
                        string message = ex.Message;
                        string methodInfo = "reportViewer:getReportprogress";
                        if (ex.InnerException != null)
                        {
                            message = string.Format("{0}{1}{1}{2}", ex.Message, Environment.NewLine, ex.InnerException.Message);
                        }

                        cEventlog.LogEntry(
                            string.Format(
                                "ReportEngine : reportviewer : {0} : {1} : {2}{3}{4}{5}",
                                "getReportprogress",
                                methodInfo,
                                Environment.NewLine,
                                message,
                                Environment.NewLine,
                                ex.StackTrace));
                    }

                    var clsIReports = (IReports)Activator.GetObject(typeof(IReports), ConfigurationManager.AppSettings["ReportsServicePath"] + "/reports.rem");
                    clsIReports.cancelRequest(clsrequest);
                }
                else
                {
                    ds = (DataSet)Session[string.Format("{0}_{1}_{2}", this.ViewState["requestnum"], user.EmployeeID, user.AccountID)];
                }
          
                this.gridreport.Visible = true;
                this.displayChart(ds, clsrequest, user);

                this.gridreport.DataSource = clsrequest.report.ShowChart == ShowChartFlag.OnlyChart ? new DataSet() : ds;
                
                this.reportStatusInformation.Visible = false;
                Page.Title = clsrequest.report.reportname;
            }
            else
            {
                var clsAudit = new cAuditLog(user.AccountID, user.EmployeeID);
                clsAudit.addRecord(SpendManagementElement.Reports, string.Format("Report {0} ID {1} was started by employee {2}", clsrequest.report.reportname, clsrequest.report.reportid, clsrequest.employeeid), 0);
                clsreports.createReport(clsrequest);
                this.gridreport.Visible = false;
            }
        }
    }

    public void displayChart(DataSet ds, cReportRequest clsrequest, CurrentUser user)
    {
        if (ds.Tables.Count > 1)
        {
            if (clsrequest.report.ShowChart != ShowChartFlag.Never)
            {
                var sw = new StringWriter();
                var h = new HtmlTextWriter(sw);
                var imgChart = new HtmlImage
                                   {
                                       Alt = "Chart",
                                       Src = svcReports.GetImagePath(user) + ds.Tables[1].Rows[0].ItemArray[0].ToString(),
                                       ID = "imgChart"
                                   };

                imgChart.Style.Add(HtmlTextWriterStyle.MarginLeft, "25%");
                imgChart.Style.Add(HtmlTextWriterStyle.MarginTop, "2px");
                imgChart.Style.Add(HtmlTextWriterStyle.MarginBottom, "2px");
                imgChart.Style.Add(HtmlTextWriterStyle.BorderStyle, "solid");
                imgChart.Style.Add(HtmlTextWriterStyle.BorderWidth, "1px");

                imgChart.RenderControl(h);
                this.litChart.InnerHtml = sw.GetStringBuilder().ToString();
                this.litChart.Visible = true;
            }

            if (clsrequest.report.ShowChart == ShowChartFlag.OnlyChart)
            {
                gridreport.Visible = false;
            }
        }
    }
    public string displayCriteria(ArrayList criteria)
    {
        cReports clsreports = new cReports((int)ViewState["accountid"], (int)ViewState["subaccountid"]);
        cAccountSubAccounts clsSubAccounts = new cAccountSubAccounts((int)ViewState["accountid"]);
        cAccountProperties accountProperties = clsSubAccounts.getSubAccountById((int)ViewState["subaccountid"]).SubAccountProperties;

        var relabeler = new FieldRelabler(accountProperties);
        System.Text.StringBuilder output = new System.Text.StringBuilder();
        string rowclass = "row1";
        output.Append("<table class=\"datatbl\">");
        foreach (cReportCriterion criterion in criteria)
        {
            string fieldDescription = relabeler.Relabel(criterion.field).Description;

            output.Append("<tr>");
            output.Append("<td class=\"" + rowclass + "\">" + fieldDescription + "</td>");
            output.Append("<td class=\"" + rowclass + "\">");
         
            switch (criterion.condition)
            {
                case ConditionType.Between:
                    output.Append("Between");
                    break;
                case ConditionType.ContainsData:
                    output.Append("Contains Data");
                    break;
                case ConditionType.DoesNotContainData:
                    output.Append("Does Not Contain Data");
                    break;
                case ConditionType.DoesNotEqual:
                    output.Append("Not Equal To");
                    break;
                case ConditionType.Equals:
                    output.Append("Equal To");
                    break;
                case ConditionType.On:
                    output.Append("On");
                    break;
                case ConditionType.GreaterThan:
                    output.Append("Greater Than");
                    break;
                case ConditionType.After:
                    output.Append("After");
                    break;
                case ConditionType.GreaterThanEqualTo:
                    output.Append("Greater Than Or Equal To");
                    break;
                case ConditionType.OnOrAfter:
                    output.Append("On Or After");
                    break;
                case ConditionType.Last7Days:
                    output.Append("in the last 7 days");
                    break;
                case ConditionType.LastFinancialYear:
                    output.Append("in the last financial year");
                    break;
                case ConditionType.LastMonth:
                    output.Append("in the last month");
                    break;
                case ConditionType.LastWeek:
                    output.Append("in the last week");
                    break;
                case ConditionType.LastXDays:
                    output.Append("in the last " + criterion.value1[0] + " days");
                    break;
                case ConditionType.LastXMonths:
                    output.Append("in the last " + criterion.value1[0] + " months");
                    break;
                case ConditionType.LastXWeeks:
                    output.Append("in the last " + criterion.value1[0] + " weeks");
                    break;
                case ConditionType.LastXYears:
                    output.Append("in the last " + criterion.value1[0] + " years");
                    break;
                case ConditionType.LastYear:
                    output.Append("last year");
                    break;
                case ConditionType.LessThan:
                    output.Append("Less Than");
                    break;
                case ConditionType.Before:
                    output.Append("Before");
                    break;
                case ConditionType.OnOrBefore:
                    output.Append("On Or Before");
                    break;
                case ConditionType.LessThanEqualTo:
                    output.Append("Less Than Or Equal To");
                    break;
                case ConditionType.Like:
                    output.Append("Like");
                    break;
                case ConditionType.Next7Days:
                    output.Append("in the next 7 days");
                    break;
                case ConditionType.NextFinancialYear:
                    output.Append("next financial year");
                    break;
                case ConditionType.NextMonth:
                    output.Append("in the next month");
                    break;
                case ConditionType.NextWeek:
                    output.Append("in the next week");
                    break;
                case ConditionType.NextXDays:
                    output.Append("in the next " + criterion.value1[0] + " days");
                    break;
                case ConditionType.NextXMonths:
                    output.Append("in the next " + criterion.value1[0] + " months");
                    break;
                case ConditionType.NextXWeeks:
                    output.Append("in the next " + criterion.value1[0] + " weeks");
                    break;
                case ConditionType.NextXYears:
                    output.Append("in the next " + criterion.value1[0] + " years");
                    break;
                case ConditionType.NextYear:
                    output.Append("next year");
                    break;
                case ConditionType.NextTaxYear:
                    output.Append("next tax year");
                    break;
                case ConditionType.LastTaxYear:
                    output.Append("last tax year");
                    break;
                case ConditionType.ThisTaxYear:
                    output.Append("this tax year");
                    break;
                case ConditionType.ThisFinancialYear:
                    output.Append("this financial year");
                    break;
                case ConditionType.ThisMonth:
                    output.Append("this month");
                    break;
                case ConditionType.ThisWeek:
                    output.Append("this week");
                    break;
                case ConditionType.ThisYear:
                    output.Append("this year");
                    break;
                case ConditionType.Today:
                    output.Append("today");
                    break;
                case ConditionType.Tomorrow:
                    output.Append("tomorrow");
                    break;
                case ConditionType.Yesterday:
                    output.Append("yesterday");
                    break;
            }

            output.Append("</td>");
            output.Append("<td class=\"" + rowclass + "\">");
            switch (criterion.field.FieldType)
            {
                case "S":
                case "FS":
                case "LT":
                case "R":
                    if ((criterion.field.GenList) && criterion.drilldown == false && (criterion.condition == ConditionType.Equals || criterion.condition == ConditionType.DoesNotEqual))
                    {
                        output.Append(clsreports.getReportListValues(criterion));
                    }
                    else
                    {
                        output.Append(criterion.value1[0]);
                    }
                    break;
                case "C":
                case "A":
                    output.Append(criterion.value1[0]);
                    if (criterion.condition == ConditionType.Between)
                    {
                        output.Append("&nbsp;and&nbsp;" + criterion.value2[0]);
                    }
                    break;
                case "N":
                    if (criterion.field.ValueList || (criterion.field.FieldSource != cField.FieldSourceType.Metabase && criterion.field.GetRelatedTable() != null && criterion.field.IsForeignKey))
                    {
                        if (criterion.value1[0] != null && criterion.value1[0].ToString() != "")
                        {
                            Dictionary<object, object> values = null;
                            if (!criterion.field.ValueList)
                            {
                                values = cReports.getRelationshipValues(cMisc.GetCurrentUser(), criterion.field.FieldID);
                            }
                            object[] vlistitems = criterion.value1[0].ToString().Split(',');
                            System.Text.StringBuilder temp = new System.Text.StringBuilder();
                            for (int x = 0; x < vlistitems.GetLength(0); x++)
                            {
                                if (criterion.field.ValueList)
                                {
                                    foreach(KeyValuePair<object, string> kvp in criterion.field.ListItems)
                                    {
                                        if(vlistitems[x].ToString() == kvp.Key.ToString())
                                        {
                                            temp.Append(kvp.Value + ", ");
                                        }
                                    }
                                }
                                else
                                {
                                    // must be n:1 CE or UDF
                                    foreach(KeyValuePair<object, object> kvp in values)
                                    {
                                        if (vlistitems[x].ToString() == kvp.Key.ToString())
                                        {
                                            temp.Append(kvp.Value + ", ");
                                        }
                                    }
                                }
                            }
                            if (temp.Length > 0)
                            {
                                temp = temp.Remove(temp.Length - 2, 2);
                            }
                            output.Append(temp.ToString());
                        }
                    }
                    else
                    {
                        output.Append(criterion.value1[0]);
                        if (criterion.condition == ConditionType.Between)
                        {
                            output.Append("&nbsp;and&nbsp;" + criterion.value2[0]);
                        }
                    }
                    break;
                case "D":
                    if (criterion.condition == ConditionType.After || criterion.condition == ConditionType.Before || criterion.condition == ConditionType.On || criterion.condition == ConditionType.OnOrAfter || criterion.condition == ConditionType.OnOrBefore || criterion.condition == ConditionType.NotOn || criterion.condition == ConditionType.Between)
                    {
                        output.Append(((DateTime)criterion.value1[0]).ToShortDateString());
                        if (criterion.condition == ConditionType.Between)
                        {
                            output.Append("&nbsp;and&nbsp;" + ((DateTime)criterion.value2[0]).ToShortDateString());
                        }
                    }
                    break;
                case "DT":
                    if (criterion.condition == ConditionType.After || criterion.condition == ConditionType.Before || criterion.condition == ConditionType.Equals || criterion.condition == ConditionType.OnOrAfter || criterion.condition == ConditionType.OnOrBefore || criterion.condition == ConditionType.DoesNotEqual || criterion.condition == ConditionType.Between)
                    {
                        string datetime = "";
                        if (criterion.value1 != null)
                        {
                            if (criterion.value1.GetLength(0) > 0)
                            {
                                if (criterion.value1[0] != null)
                                {
                                    datetime = criterion.value1[0].ToString();
                                }
                            }
                        }
                        output.Append(datetime);
                        if (criterion.condition == ConditionType.Between)
                        {
                            datetime = "";
                            if (criterion.value2 != null)
                            {
                                if (criterion.value2.GetLength(0) > 0)
                                {
                                    if (criterion.value2[0] != null)
                                    {
                                        datetime = criterion.value2[0].ToString();
                                        int timepos = 0;
                                    }
                                }
                            }
                            output.Append("&nbsp;and&nbsp;" + datetime);
                        }
                    }
                    break;
                case "T":
                    if (criterion.condition == ConditionType.After || criterion.condition == ConditionType.Before || criterion.condition == ConditionType.Equals || criterion.condition == ConditionType.OnOrAfter || criterion.condition == ConditionType.OnOrBefore || criterion.condition == ConditionType.DoesNotEqual || criterion.condition == ConditionType.Between)
                    {
                        string timepart = "";
                        if (criterion.value1 != null && criterion.value1[0] != null)
                        {
                            timepart = DateTime.Parse(criterion.value1[0].ToString()).ToShortTimeString();
                        }
                        output.Append(timepart);
                        if (criterion.condition == ConditionType.Between)
                        {
                            timepart = "";
                            if (criterion.value2 != null && criterion.value2[0] != null)
                            {
                                timepart = DateTime.Parse(criterion.value2[0].ToString()).ToShortTimeString();
                            }
                            output.Append("&nbsp;and&nbsp;" + timepart);
                        }
                    }
                    break;
                case "X":
                    var byteValue = criterion.value1[0];
                    if ((byteValue is byte && (byte)criterion.value1[0] == 1) 
                        || ((byteValue is string && criterion.value1[0].ToString() == "1")))
                    {
                        output.Append("Yes");
                    }
                    else
                    {
                        output.Append("No");
                    }
                    break;
                case "Y":
                    output.Append(criterion.value1[0]);
                    break;
            }
            output.Append("</td>");
            output.Append("</tr>");
            if (rowclass == "row1")
            {
                rowclass = "row2";
            }
            else
            {
                rowclass = "row1";
            }
        }
        output.Append("</table>");
        return output.ToString();
    }


    private void formatAggColumn()
    {

    }
    protected void gridreport_InitializeLayout(object sender, LayoutEventArgs e)
    {
        CurrentUser currentUser = cMisc.GetCurrentUser();

        cReportRequest clsrequest = (cReportRequest)Session["request" + ViewState["requestnum"]];
        cReport rpt = clsrequest.report;
        reqReport = rpt;

        var generalOptions = this.GeneralOptionsFactory[cMisc.GetCurrentUser().CurrentSubAccountId].WithCurrency();

        cAccountSubAccounts clsSubAccounts = new cAccountSubAccounts((int)ViewState["accountid"]);
        cAccountProperties accountProperties = clsSubAccounts.getSubAccountById((int)ViewState["subaccountid"]).SubAccountProperties;

        if (string.IsNullOrEmpty(reqReport.StaticReportSQL))
        {
            e.Layout.GroupByBox.Hidden = false;
            e.Layout.ViewType = Infragistics.WebUI.UltraWebGrid.ViewType.OutlookGroupBy;
            cCurrencies clscurrencies = new cCurrencies((int)ViewState["accountid"], (int)ViewState["subaccountid"]);
            
            var relabeler = new FieldRelabler(accountProperties);
            cGlobalCurrencies clsglobalcurrencies = new cGlobalCurrencies();
            cCurrency currency = clscurrencies.getCurrencyById((int) generalOptions.Currency.BaseCurrency);
            string symbol = "";
            if (currency != null)
            {
                symbol = clsglobalcurrencies.getGlobalCurrencyById(currency.globalcurrencyid).symbol;
            }

            foreach(cReportColumn column in rpt.columns)
            {
                switch (column.columntype)
                {
                    case ReportColumnType.Standard:
                        cStandardColumn standard = (cStandardColumn)column;
                        
                        var fieldDescription = relabeler.Relabel(standard.field).Description;

                        if (standard.field.FieldID == new Guid("e3af2b67-a613-437e-aabf-6853c4553977"))
                        {
                            ViewState["claimidcol"] = standard.order;
                        }
                        if (standard.field.FieldID == new Guid("a528de93-3037-46f6-974c-a76bd0c8642a"))
                        {
                            ViewState["expenseidcol"] = standard.order;
                        }
                        if (standard.field.FieldID.ToString().ToUpper() == ReportKeyFields.ContractDetails_ContractId)
                        {
                            ViewState["contractidcol"] = standard.order;
                        }

                        if (standard.field.FieldID.ToString().ToUpper() == ReportKeyFields.ContractProducts_ConProdId)
                        {
                            ViewState["contractproductidcol"] = standard.order;
                        }

                        if (standard.field.FieldID.ToString().ToUpper() == ReportKeyFields.SupplierDetails_SupplierId)
                        {
                            ViewState["supplieridcol"] = standard.order;
                        }

                        if (standard.field.FieldID.ToString().ToUpper() == ReportKeyFields.Tasks_TaskId)
                        {
                            ViewState["taskidcol"] = standard.order;
                        }
                        if (standard.funcavg || standard.funccount || standard.funcmax || standard.funcmin || standard.funcsum)
                        {
                            var currentColumn = e.Layout.Bands[0].Columns.FromKey(column.columnid.ToString());
                            if (standard.funcavg)
                            {
                                if (currentColumn == null)
                                {
                                    currentColumn =
                                        e.Layout.Bands[0].Columns.FromKey(column.columnid.ToString() + "_AVG");
                                }
                                currentColumn.Hidden = column.hidden;
                                currentColumn.Header.Caption = "AVG of " + fieldDescription;
                                this.SetColumnFormat(standard, currentColumn, symbol);

                            }
                            if (standard.funccount)
                            {
                                if (currentColumn == null)
                                {
                                    currentColumn =
                                        e.Layout.Bands[0].Columns.FromKey(column.columnid.ToString() + "_COUNT");
                                }
                                currentColumn.Hidden = column.hidden;
                                currentColumn.Header.Caption = "COUNT of " + fieldDescription;
                            }
                            if (standard.funcmax)
                            {
                                if (currentColumn == null)
                                {
                                    currentColumn =
                                        e.Layout.Bands[0].Columns.FromKey(column.columnid.ToString() + "_MAX");
                                }
                                currentColumn.Hidden = column.hidden;
                                currentColumn.Header.Caption = "MAX of " + fieldDescription;
                                this.SetColumnFormat(standard, currentColumn, symbol);
                            }
                            if (standard.funcmin)
                            {
                                if (currentColumn == null)
                                {
                                    currentColumn =
                                        e.Layout.Bands[0].Columns.FromKey(column.columnid.ToString() + "_MIN");
                                }
                                currentColumn.Hidden = column.hidden;
                                currentColumn.Header.Caption = "MIN of " + fieldDescription;
                                this.SetColumnFormat(standard, currentColumn, symbol);
                            }
                            if (standard.funcsum)
                            {
                                if (currentColumn == null)
                                {
                                    currentColumn =
                                        e.Layout.Bands[0].Columns.FromKey(column.columnid.ToString() + "_SUM");
                                }
                                switch (currentUser.CurrentActiveModule)
                                {
                                    case Modules.contracts:
                                        currentColumn.Footer.Style.BackColor = System.Drawing.ColorTranslator.FromHtml("#00a0af");
                                        currentColumn.Footer.Style.BorderDetails.ColorTop = System.Drawing.ColorTranslator.FromHtml("#00a0af");
                                        currentColumn.Footer.Style.BorderDetails.ColorBottom = System.Drawing.ColorTranslator.FromHtml("#00a0af");
                                        break;
                                    case Modules.expenses:
                                        currentColumn.Footer.Style.BackColor = System.Drawing.ColorTranslator.FromHtml("#19A2E6");
                                        currentColumn.Footer.Style.BorderDetails.ColorTop = System.Drawing.ColorTranslator.FromHtml("#19A2E6");
                                        currentColumn.Footer.Style.BorderDetails.ColorBottom = System.Drawing.ColorTranslator.FromHtml("#19A2E6");
                                        break;
                                    default:
                                        currentColumn.Footer.Style.BackColor = System.Drawing.ColorTranslator.FromHtml("#003768");
                                        currentColumn.Footer.Style.BorderDetails.ColorTop = System.Drawing.ColorTranslator.FromHtml("#003768");
                                        currentColumn.Footer.Style.BorderDetails.ColorBottom = System.Drawing.ColorTranslator.FromHtml("#003768");
                                        break;
                                }

                                currentColumn.Footer.Style.Font.Names = new string[] { "Arial", "Sans-serif" };
                                currentColumn.Footer.Style.ForeColor = System.Drawing.Color.White;
                                currentColumn.Footer.Style.Padding.Bottom = Unit.Pixel(3);
                                currentColumn.Footer.Style.Padding.Left = Unit.Pixel(3);
                                currentColumn.Footer.Style.Padding.Top = Unit.Pixel(3);
                                currentColumn.Footer.Style.Padding.Right = Unit.Pixel(3);
                                currentColumn.Footer.Style.HorizontalAlign = HorizontalAlign.Center;
                                currentColumn.Footer.Style.BorderDetails.WidthTop = Unit.Pixel(1);
                                currentColumn.Footer.Style.BorderDetails.StyleTop = BorderStyle.Solid;
                                currentColumn.Footer.Style.BorderDetails.WidthBottom = Unit.Pixel(2);
                                currentColumn.Footer.Style.BorderDetails.StyleBottom = BorderStyle.Solid;
                                currentColumn.Hidden = column.hidden;
                                currentColumn.Header.Caption = "SUM of " + fieldDescription;
                                currentColumn.FooterTotal = SummaryInfo.Sum;
                                currentColumn.FooterStyle.HorizontalAlign = HorizontalAlign.Right;
                                currentColumn.CellStyle.HorizontalAlign = HorizontalAlign.Right;

                                this.SetColumnFormat(standard, currentColumn, symbol);
                            }

                            if (standard.groupby)
                            {
                                currentColumn.IsGroupByColumn = true;
                            }
                        }
                        else
                        {
                            if (standard.groupby)
                            {
                                e.Layout.Bands[0].Columns.FromKey(column.columnid.ToString()).IsGroupByColumn = true;
                            }
                            e.Layout.Bands[0].Columns.FromKey(column.columnid.ToString()).Hidden = column.hidden;
                            e.Layout.Bands[0].Columns.FromKey(standard.columnid.ToString()).Header.Caption = fieldDescription;
                            e.Layout.Bands[0].Columns.FromKey(standard.columnid.ToString()).Header.Style.HorizontalAlign = HorizontalAlign.Center;
                            e.Layout.Bands[0].Columns.FromKey(standard.columnid.ToString()).Type = ColumnType.Button;

                            switch (standard.field.FieldType)
                            {
                                case "D":
                                    e.Layout.Bands[0].Columns.FromKey(standard.columnid.ToString()).Format = "dd/MM/yyyy";
                                    break;
                                case "DT":
                                    e.Layout.Bands[0].Columns.FromKey(standard.columnid.ToString()).Format = "dd/MM/yyyy HH:mm:ss";
                                    break;
                                case "Y":
                                    e.Layout.Bands[0].Columns.FromKey(standard.columnid.ToString()).CellStyle.HorizontalAlign = HorizontalAlign.Center;
                                    break;
                                case "T":
                                    e.Layout.Bands[0].Columns.FromKey(standard.columnid.ToString()).Format = "HH:mm";
                                    break;
                                case "X":
                                    e.Layout.Bands[0].Columns.FromKey(standard.columnid.ToString()).Type = ColumnType.CheckBox;
                                    e.Layout.Bands[0].Columns.FromKey(standard.columnid.ToString()).CellStyle.HorizontalAlign = HorizontalAlign.Center;
                                    break;
                                case "N":
                                    if (standard.field.FieldSource != cField.FieldSourceType.Metabase && standard.field.IsForeignKey && standard.field.RelatedTableID != Guid.Empty)
                                    {
                                        // relationship field from CE or UDF
                                        e.Layout.Bands[0].Columns.FromKey(standard.columnid.ToString()).ValueList = cReports.getRelationshipValuesAsValueList(currentUser, standard.field.FieldID);
                                        e.Layout.Bands[0].Columns.FromKey(standard.columnid.ToString()).Type = ColumnType.DropDownList;
                                        e.Layout.Bands[0].Columns.FromKey(standard.columnid.ToString()).ValueList.DisplayStyle = ValueListDisplayStyle.DisplayText;
                                    }
                                    else
                                    {
                                        if(standard.field.CanTotal)
                                        {                                            
                                            switch (currentUser.CurrentActiveModule)
                                            {
                                                case Modules.contracts:
                                                    e.Layout.Bands[0].Columns.FromKey(standard.columnid.ToString()).Footer.Style.BackColor = System.Drawing.ColorTranslator.FromHtml("#00a0af");
                                                    e.Layout.Bands[0].Columns.FromKey(standard.columnid.ToString()).Footer.Style.BorderDetails.ColorTop = System.Drawing.ColorTranslator.FromHtml("#00a0af");
                                                    e.Layout.Bands[0].Columns.FromKey(standard.columnid.ToString()).Footer.Style.BorderDetails.ColorBottom = System.Drawing.ColorTranslator.FromHtml("#00a0af");
                                                    break;
                                                case Modules.expenses:
                                                    e.Layout.Bands[0].Columns.FromKey(standard.columnid.ToString()).Footer.Style.BackColor = System.Drawing.ColorTranslator.FromHtml("#19A2E6");
                                                    e.Layout.Bands[0].Columns.FromKey(standard.columnid.ToString()).Footer.Style.BorderDetails.ColorTop = System.Drawing.ColorTranslator.FromHtml("#19A2E6");
                                                    e.Layout.Bands[0].Columns.FromKey(standard.columnid.ToString()).Footer.Style.BorderDetails.ColorBottom = System.Drawing.ColorTranslator.FromHtml("#19A2E6");
                                                    break;
                                                default:
                                                    e.Layout.Bands[0].Columns.FromKey(standard.columnid.ToString()).Footer.Style.BackColor = System.Drawing.ColorTranslator.FromHtml("#003768");
                                                    e.Layout.Bands[0].Columns.FromKey(standard.columnid.ToString()).Footer.Style.BorderDetails.ColorTop = System.Drawing.ColorTranslator.FromHtml("#003768");
                                                    e.Layout.Bands[0].Columns.FromKey(standard.columnid.ToString()).Footer.Style.BorderDetails.ColorBottom = System.Drawing.ColorTranslator.FromHtml("#003768");
                                                    break;
                                            }
                                            e.Layout.Bands[0].Columns.FromKey(standard.columnid.ToString()).Footer.Style.Font.Names = new string[] {"Arial", "Sans-serif"};
                                            e.Layout.Bands[0].Columns.FromKey(standard.columnid.ToString()).Footer.Style.ForeColor = System.Drawing.Color.White;
                                            e.Layout.Bands[0].Columns.FromKey(standard.columnid.ToString()).Footer.Style.Padding.Bottom = Unit.Pixel(3);
                                            e.Layout.Bands[0].Columns.FromKey(standard.columnid.ToString()).Footer.Style.Padding.Left = Unit.Pixel(3);
                                            e.Layout.Bands[0].Columns.FromKey(standard.columnid.ToString()).Footer.Style.Padding.Top = Unit.Pixel(3);
                                            e.Layout.Bands[0].Columns.FromKey(standard.columnid.ToString()).Footer.Style.Padding.Right = Unit.Pixel(3);
                                            e.Layout.Bands[0].Columns.FromKey(standard.columnid.ToString()).Footer.Style.HorizontalAlign = HorizontalAlign.Center;
                                            e.Layout.Bands[0].Columns.FromKey(standard.columnid.ToString()).Footer.Style.BorderDetails.ColorTop = System.Drawing.ColorTranslator.FromHtml("#003768");
                                            e.Layout.Bands[0].Columns.FromKey(standard.columnid.ToString()).Footer.Style.BorderDetails.WidthTop = Unit.Pixel(1);
                                            e.Layout.Bands[0].Columns.FromKey(standard.columnid.ToString()).Footer.Style.BorderDetails.StyleTop = BorderStyle.Solid;
                                            e.Layout.Bands[0].Columns.FromKey(standard.columnid.ToString()).Footer.Style.BorderDetails.ColorBottom = System.Drawing.ColorTranslator.FromHtml("#003768");
                                            e.Layout.Bands[0].Columns.FromKey(standard.columnid.ToString()).Footer.Style.BorderDetails.WidthBottom = Unit.Pixel(2);
                                            e.Layout.Bands[0].Columns.FromKey(standard.columnid.ToString()).Footer.Style.BorderDetails.StyleBottom = BorderStyle.Solid;
                                            e.Layout.Bands[0].Columns.FromKey(standard.columnid.ToString()).FooterTotal = SummaryInfo.Sum;
                                            e.Layout.Bands[0].Columns.FromKey(standard.columnid.ToString()).FooterStyle.HorizontalAlign = HorizontalAlign.Right;
                                        }
                                        e.Layout.Bands[0].Columns.FromKey(standard.columnid.ToString()).CellStyle.HorizontalAlign = HorizontalAlign.Right;
                                    }
                                    break;
                                case "C":
                                case "FC":
                                case "A":
                                    if (standard.field.CanTotal)
                                    {                                        
                                        switch (currentUser.CurrentActiveModule)
                                        {
                                            case Modules.contracts:
                                                e.Layout.Bands[0].Columns.FromKey(standard.columnid.ToString()).Footer.Style.BackColor = System.Drawing.ColorTranslator.FromHtml("#00a0af");
                                                e.Layout.Bands[0].Columns.FromKey(standard.columnid.ToString()).Footer.Style.BorderDetails.ColorTop = System.Drawing.ColorTranslator.FromHtml("#00a0af");
                                                e.Layout.Bands[0].Columns.FromKey(standard.columnid.ToString()).Footer.Style.BorderDetails.ColorBottom = System.Drawing.ColorTranslator.FromHtml("#00a0af");
                                                break;
                                            case Modules.expenses:
                                                e.Layout.Bands[0].Columns.FromKey(standard.columnid.ToString()).Footer.Style.BackColor = System.Drawing.ColorTranslator.FromHtml("#19A2E6");
                                                e.Layout.Bands[0].Columns.FromKey(standard.columnid.ToString()).Footer.Style.BorderDetails.ColorTop = System.Drawing.ColorTranslator.FromHtml("#19A2E6");
                                                e.Layout.Bands[0].Columns.FromKey(standard.columnid.ToString()).Footer.Style.BorderDetails.ColorBottom = System.Drawing.ColorTranslator.FromHtml("#19A2E6");
                                                break;
                                            default:
                                                e.Layout.Bands[0].Columns.FromKey(standard.columnid.ToString()).Footer.Style.BackColor = System.Drawing.ColorTranslator.FromHtml("#003768");
                                                e.Layout.Bands[0].Columns.FromKey(standard.columnid.ToString()).Footer.Style.BorderDetails.ColorTop = System.Drawing.ColorTranslator.FromHtml("#003768");
                                                e.Layout.Bands[0].Columns.FromKey(standard.columnid.ToString()).Footer.Style.BorderDetails.ColorBottom = System.Drawing.ColorTranslator.FromHtml("#003768");
                                                break;
                                        }                                        
                                        e.Layout.Bands[0].Columns.FromKey(standard.columnid.ToString()).Footer.Style.Font.Names = new string[] { "Arial", "Sans-serif" };
                                        e.Layout.Bands[0].Columns.FromKey(standard.columnid.ToString()).Footer.Style.ForeColor = System.Drawing.Color.White;
                                        e.Layout.Bands[0].Columns.FromKey(standard.columnid.ToString()).Footer.Style.Padding.Bottom = Unit.Pixel(3);
                                        e.Layout.Bands[0].Columns.FromKey(standard.columnid.ToString()).Footer.Style.Padding.Left = Unit.Pixel(3);
                                        e.Layout.Bands[0].Columns.FromKey(standard.columnid.ToString()).Footer.Style.Padding.Top = Unit.Pixel(3);
                                        e.Layout.Bands[0].Columns.FromKey(standard.columnid.ToString()).Footer.Style.Padding.Right = Unit.Pixel(3);
                                        e.Layout.Bands[0].Columns.FromKey(standard.columnid.ToString()).Footer.Style.HorizontalAlign = HorizontalAlign.Center;
                                        e.Layout.Bands[0].Columns.FromKey(standard.columnid.ToString()).Footer.Style.BorderDetails.ColorTop = System.Drawing.ColorTranslator.FromHtml("#003768");
                                        e.Layout.Bands[0].Columns.FromKey(standard.columnid.ToString()).Footer.Style.BorderDetails.WidthTop = Unit.Pixel(1);
                                        e.Layout.Bands[0].Columns.FromKey(standard.columnid.ToString()).Footer.Style.BorderDetails.StyleTop = BorderStyle.Solid;
                                        e.Layout.Bands[0].Columns.FromKey(standard.columnid.ToString()).Footer.Style.BorderDetails.ColorBottom = System.Drawing.ColorTranslator.FromHtml("#003768");
                                        e.Layout.Bands[0].Columns.FromKey(standard.columnid.ToString()).Footer.Style.BorderDetails.WidthBottom = Unit.Pixel(2);
                                        e.Layout.Bands[0].Columns.FromKey(standard.columnid.ToString()).Footer.Style.BorderDetails.StyleBottom = BorderStyle.Solid;
                                        e.Layout.Bands[0].Columns.FromKey(standard.columnid.ToString()).FooterTotal = SummaryInfo.Sum;
                                        e.Layout.Bands[0].Columns.FromKey(standard.columnid.ToString()).FooterStyle.HorizontalAlign = HorizontalAlign.Right;
                                    }
                                    e.Layout.Bands[0].Columns.FromKey(standard.columnid.ToString()).Format = symbol + getPrecisonFormat(standard.field.FieldName);
                                    e.Layout.Bands[0].Columns.FromKey(standard.columnid.ToString()).CellStyle.HorizontalAlign = HorizontalAlign.Right;
                                    break;
                                case "M":
                                case "FD":
                                case "F":
                                    if (standard.field.CanTotal)
                                    {                                      
                                        switch (currentUser.CurrentActiveModule)
                                        {
                                            case Modules.contracts:
                                                e.Layout.Bands[0].Columns.FromKey(standard.columnid.ToString()).Footer.Style.BackColor = System.Drawing.ColorTranslator.FromHtml("#00a0af");
                                                e.Layout.Bands[0].Columns.FromKey(standard.columnid.ToString()).Footer.Style.BorderDetails.ColorTop = System.Drawing.ColorTranslator.FromHtml("#00a0af");
                                                e.Layout.Bands[0].Columns.FromKey(standard.columnid.ToString()).Footer.Style.BorderDetails.ColorBottom = System.Drawing.ColorTranslator.FromHtml("#00a0af");
                                                break;
                                            case Modules.expenses:
                                                e.Layout.Bands[0].Columns.FromKey(standard.columnid.ToString()).Footer.Style.BackColor = System.Drawing.ColorTranslator.FromHtml("#19A2E6");
                                                e.Layout.Bands[0].Columns.FromKey(standard.columnid.ToString()).Footer.Style.BorderDetails.ColorTop = System.Drawing.ColorTranslator.FromHtml("#19A2E6");
                                                e.Layout.Bands[0].Columns.FromKey(standard.columnid.ToString()).Footer.Style.BorderDetails.ColorBottom = System.Drawing.ColorTranslator.FromHtml("#19A2E6");
                                                break;
                                            default:
                                                e.Layout.Bands[0].Columns.FromKey(standard.columnid.ToString()).Footer.Style.BackColor = System.Drawing.ColorTranslator.FromHtml("#003768");
                                                e.Layout.Bands[0].Columns.FromKey(standard.columnid.ToString()).Footer.Style.BorderDetails.ColorTop = System.Drawing.ColorTranslator.FromHtml("#003768");
                                                e.Layout.Bands[0].Columns.FromKey(standard.columnid.ToString()).Footer.Style.BorderDetails.ColorBottom = System.Drawing.ColorTranslator.FromHtml("#003768");
                                                break;
                                        }
                                        e.Layout.Bands[0].Columns.FromKey(standard.columnid.ToString()).Footer.Style.Font.Names = new string[] { "Arial", "Sans-serif" };
                                        e.Layout.Bands[0].Columns.FromKey(standard.columnid.ToString()).Footer.Style.ForeColor = System.Drawing.Color.White;
                                        e.Layout.Bands[0].Columns.FromKey(standard.columnid.ToString()).Footer.Style.Padding.Bottom = Unit.Pixel(3);
                                        e.Layout.Bands[0].Columns.FromKey(standard.columnid.ToString()).Footer.Style.Padding.Left = Unit.Pixel(3);
                                        e.Layout.Bands[0].Columns.FromKey(standard.columnid.ToString()).Footer.Style.Padding.Top = Unit.Pixel(3);
                                        e.Layout.Bands[0].Columns.FromKey(standard.columnid.ToString()).Footer.Style.Padding.Right = Unit.Pixel(3);
                                        e.Layout.Bands[0].Columns.FromKey(standard.columnid.ToString()).Footer.Style.HorizontalAlign = HorizontalAlign.Center;
                                        e.Layout.Bands[0].Columns.FromKey(standard.columnid.ToString()).Footer.Style.BorderDetails.ColorTop = System.Drawing.ColorTranslator.FromHtml("#003768");
                                        e.Layout.Bands[0].Columns.FromKey(standard.columnid.ToString()).Footer.Style.BorderDetails.WidthTop = Unit.Pixel(1);
                                        e.Layout.Bands[0].Columns.FromKey(standard.columnid.ToString()).Footer.Style.BorderDetails.StyleTop = BorderStyle.Solid;
                                        e.Layout.Bands[0].Columns.FromKey(standard.columnid.ToString()).Footer.Style.BorderDetails.ColorBottom = System.Drawing.ColorTranslator.FromHtml("#003768");
                                        e.Layout.Bands[0].Columns.FromKey(standard.columnid.ToString()).Footer.Style.BorderDetails.WidthBottom = Unit.Pixel(2);
                                        e.Layout.Bands[0].Columns.FromKey(standard.columnid.ToString()).Footer.Style.BorderDetails.StyleBottom = BorderStyle.Solid;
                                        e.Layout.Bands[0].Columns.FromKey(standard.columnid.ToString()).FooterTotal = SummaryInfo.Sum;
                                        e.Layout.Bands[0].Columns.FromKey(standard.columnid.ToString()).FooterStyle.HorizontalAlign = HorizontalAlign.Right;
                                    }

                                    e.Layout.Bands[0].Columns.FromKey(standard.columnid.ToString()).Format = getPrecisonFormat(standard.field.FieldName);
                                    e.Layout.Bands[0].Columns.FromKey(standard.columnid.ToString()).CellStyle.HorizontalAlign = HorizontalAlign.Right;
                                    break;
                                case "CL":
                                    // GreenLight currency field
                                    e.Layout.Bands[0].Columns.FromKey(standard.columnid.ToString()).ValueList = clscurrencies.CreateVList();
                                    e.Layout.Bands[0].Columns.FromKey(standard.columnid.ToString()).Type = ColumnType.DropDownList;
                                    break;
                                default:
                                    break;
                            }
                        }
                        break;
                    case ReportColumnType.Static:
                        cStaticColumn staticcol = (cStaticColumn)column;
                        e.Layout.Bands[0].Columns.FromKey(column.columnid.ToString()).Hidden = column.hidden;
                        e.Layout.Bands[0].Columns.FromKey(staticcol.columnid.ToString()).Header.Caption = staticcol.literalname;
                        e.Layout.Bands[0].Columns.FromKey(staticcol.columnid.ToString()).Header.Style.HorizontalAlign = HorizontalAlign.Center;
                        break;
                    case ReportColumnType.Calculated:
                        e.Layout.Bands[0].Columns.FromKey(column.columnid.ToString()).Hidden = column.hidden;
                        cCalculatedColumn calculatedcol = (cCalculatedColumn)column;
                        e.Layout.Bands[0].Columns.FromKey(calculatedcol.columnid.ToString()).Header.Caption = calculatedcol.columnname;
                        e.Layout.Bands[0].Columns.FromKey(calculatedcol.columnid.ToString()).Header.Style.HorizontalAlign = HorizontalAlign.Center;
                        e.Layout.Bands[0].Columns.FromKey(calculatedcol.columnid.ToString()).Formula = convertFormula(calculatedcol.formula);
                        e.Layout.Bands[0].Columns.FromKey(calculatedcol.columnid.ToString()).DataType = "string";
                        break;
                }
            }
            //add column for view report if necessary
            if ((rpt.basetable.TableID == new Guid("d70d9e5f-37e2-4025-9492-3bcf6aa746a8") || rpt.basetable.TableID == new Guid("0efa50b5-da7b-49c7-a9aa-1017d5f741d0")) && rpt.getReportType() == ReportType.Item)
            {
                e.Layout.Bands[0].Columns.Insert(0, "viewclaim");
                e.Layout.Bands[0].Columns.FromKey("viewclaim").Header.Caption = "View Claim";
                e.Layout.Bands[0].Columns.FromKey("viewclaim").Type = ColumnType.NotSet;

            }

            if (rpt.basetable.TableID.ToString().ToUpper() == ReportTable.ContractDetails && rpt.getReportType() == ReportType.Item)
            {
                e.Layout.Bands[0].Columns.Insert(0, "viewcontract");
                e.Layout.Bands[0].Columns.FromKey("viewcontract").Header.Caption = "View Contract";
                e.Layout.Bands[0].Columns.FromKey("viewcontract").Type = ColumnType.NotSet;
            }


            if (rpt.basetable.TableID.ToString().ToUpper() == ReportTable.SupplierDetails && rpt.getReportType() == ReportType.Item)
            {
                e.Layout.Bands[0].Columns.Insert(0, "viewsupplier");
                e.Layout.Bands[0].Columns.FromKey("viewsupplier").Header.Caption = "View " + accountProperties.SupplierPrimaryTitle;
                e.Layout.Bands[0].Columns.FromKey("viewsupplier").Type = ColumnType.NotSet;
            }

            if (rpt.basetable.TableID.ToString().ToUpper() == ReportTable.Tasks && rpt.getReportType() == ReportType.Item)
            {
                e.Layout.Bands[0].Columns.Insert(0, "viewtask");
                e.Layout.Bands[0].Columns.FromKey("viewtask").Header.Caption = "View Task";
                e.Layout.Bands[0].Columns.FromKey("viewtask").Type = ColumnType.NotSet;
            }
        }

        doGroupTotals();

    }

    private void SetColumnFormat(cStandardColumn standard, UltraGridColumn currentColumn, string symbol)
    {
        switch (standard.field.FieldType)
        {
            case "C":
            case "FC":
            case "A":
                currentColumn.Format = symbol + this.getPrecisonFormat(standard.field.FieldName);
                break;
            case "M":
            case "FD":
            case "F":
            case "N":
                currentColumn.Format = this.getPrecisonFormat(standard.field.FieldName);
                break;
            case "D":
                currentColumn.Format = "dd/MM/yyyy";
                break;
            case "DT":
                currentColumn.Format = "dd/MM/yyyy HH:mm:ss";
                break;
            case "T":
                currentColumn.Format = "HH:mm";
                break;
        }
    }

    public string convertFormula(string formula)
    {
        int start = 0;

        // get rid of quoted (i.e. "[ ]") formula fields
        formula = formula.Replace("\"[", "[");
        formula = formula.Replace("]\"", "]");

        while (start < formula.Length)
        {
            int startIndex = formula.IndexOf('[', start);
            if (startIndex == -1)
            {
                break;
            }
            int endIndex = formula.IndexOf(']', startIndex);
            if (endIndex == -1)
            {
                break;
            }
            string column = formula.Substring(startIndex + 1, endIndex - startIndex - 1);
            //replace with index
            for (int i = 0; i < gridreport.Columns.Count; i++)
            {
                if(gridreport.Columns[i].Header.Caption != column) continue;

                int pos = i + 1;
                if (gridreport.Columns.FromKey("viewclaim") != null)
                {
                    pos--;
                }
                if (column.IndexOf("SUM") != -1)
                {
                    formula = formula.Replace(column, pos.ToString() + "_SUM");
                }
                else if (column.IndexOf("COUNT") != -1)
                {
                    formula = formula.Replace(column, pos.ToString() + "_COUNT");
                }
                else if (column.IndexOf("MAX") != -1)
                {
                    formula = formula.Replace(column, pos.ToString() + "_MAX");
                }
                else if (column.IndexOf("MIN") != -1)
                {
                    formula = formula.Replace(column, pos.ToString() + "_MIN");
                }
                else if (column.IndexOf("COUNT") != -1)
                {
                    formula = formula.Replace(column, pos.ToString() + "_COUNT");
                }
                else
                {
                    formula = formula.Replace(column, pos.ToString());
                }
                break;
            }
            endIndex = formula.IndexOf(']', startIndex);
            start = endIndex;
        }
        return formula;
    }

    public int TotalColumnID
    {
        get { return nTotalColumnId; }
        set { nTotalColumnId = value; }
    }
    public decimal GrandTotal
    {
        get { return dCurGrandTotal; }
        set { dCurGrandTotal = value; }
    }
    public void doGroupTotals()
    {
        int colnum = 0;
        int bestIdx = 999;
        
        string[] priority = new string[] { "Contract Value inc Variations", "Contract Value excl Variations", "Annual Contract Value", "Total Annual Cost", "Next Period Annual Cost", "Next Period Annual Cost+1", "Next Period Annual Cost+2", "Next Period Annual Cost+3", "Total Invoice Amount", "Contract Product Value", "Projected Saving" };
        string[] priorityID = new string[] { "53EA9A85-5F91-468A-A93A-B6B46ACC91B2", "F0A8B9AC-440A-4A44-8D34-72ABE2AB18F2", "8F9DAD4B-4309-462B-BEA3-E68EB9FEBE1F", "2B3932A7-3585-4EBA-8877-33911E304E15", "04983186-6FA2-4309-B6B0-D3C47B214D75", "3A29AB1A-E1A2-44F7-B3FE-70C7FE413F3F", "A15840C4-E068-4470-8133-E16D8BE1D559", "263D3C02-A425-4CAA-96DC-81DF24DC1211", "C2C5417F-4DD5-4EA5-A0F8-57FFC4EF5902", "1D004A16-D630-4C63-A6BF-8ABCE5D49331", "FA4ED990-AD57-4E8C-BFCB-CC0B0430C13F" }; //{339, 344, 85, 137, 379, 142, 143, 144, 145, 161, 132, 141}

        string tmpStr = "<table style=\"display: inline;\"><tr><td width=\"400px\">[value]</td><td width=\"100px\">Items: <b>[count]</b></td></tr></table>";
        //tmpStr = "[value]&#09;([count])"; // "[caption]  :  <b> [value] </b>"
        cReportRequest clsrequest = (cReportRequest)Session["request" + ViewState["requestnum"]];
        foreach (cReportColumn col in clsrequest.report.columns)
        {
            if(col.columntype != ReportColumnType.Standard) continue;

            cStandardColumn standardcol = (cStandardColumn)col;
            int arrIdx;
            for (arrIdx = priorityID.GetLength(0)-1; arrIdx>=0;arrIdx--)
            {
                if(standardcol.field.FieldID.ToString().ToUpper() != priorityID[arrIdx]) continue;

                if(arrIdx >= bestIdx) continue;

                colnum = col.columnid;
                bestIdx = arrIdx;
                break;
            }
        }

        if (bestIdx != 999)
        {
            //if (gridreport.Bands[0].Columns.FromKey("viewcontract") != null)
            //{
            //    colnum +=1;
            //}
            tmpStr = "<table style=\"display: inline;\"><tr><td width=\"400px\">[value]</td><td width=\"150px\">Group Total :<b> [sum:" + gridreport.DisplayLayout.Bands[0].Columns[colnum].Key.ToString() + "]</b></td><td width=\"100px\">Items: <b>[count]</b></td></tr></table>";
            TotalColumnID = colnum;
        }
        else
        {
            TotalColumnID = -1;
        }

        gridreport.DisplayLayout.GroupByRowDescriptionMaskDefault = tmpStr;
    }

    protected void gridreport_GroupColumn(object sender, ColumnEventArgs e)
    {
        //tmpStr = "[caption]  :  <b> [value] </b>";
        string tmpStr = "<table style=\"display: inline;\"><tr><td width=\"400px\">[value]</td><td width=\"100px\">Items: <b>[count]</b></td></tr></table>";

        doGroupTotals();

        cReportRequest clsrequest = (cReportRequest)Session["request" + ViewState["requestnum"]];
        for (int i = 0; i < clsrequest.report.columns.Count; i++)
        {
            cReportColumn col = (cReportColumn)clsrequest.report.columns[i];
            if(col.columntype != ReportColumnType.Standard) continue;

            cStandardColumn standardcol = (cStandardColumn)col;
            if(standardcol.field.Description != "Total") continue;

            int colnum = i;
            if (gridreport.Bands[0].Columns.FromKey("viewclaim") != null)
            {
                colnum++;
            }
            tmpStr = "<table style=\"display: inline;\"><tr><td width=\"400px\">[caption] : <b>[value]</b></td><td width=\"150px\">Total: <b> [sum:" + e.Column.Band.Columns[colnum].Key.ToString() + "] </b></td><td width=\"100px\">Items: <b>[count]</b></td></tr></table>";
            //tmpStr = "[caption]  :  <b> [value] </b> :     Total: <b> [sum:" + e.Column.Band.Columns[colnum].Key.ToString() + "] </b> Items: <b>[count]</b>";
            break;
        }

        gridreport.DisplayLayout.GroupByRowDescriptionMaskDefault = tmpStr;
    }


    protected void calcman_FormulaSyntaxError(object sender, Infragistics.WebUI.UltraWebCalcManager.FormulaSyntaxErrorEventArgs e)
    {

    }
    protected void calcman_FormulaCalculationError(object sender, Infragistics.WebUI.CalcEngine.FormulaCalculationErrorEventArgs e)
    {

    }
    protected void calcman_FormulaReferenceError(object sender, Infragistics.WebUI.CalcEngine.FormulaCalculationErrorEventArgs e)
    {

    }
    protected void calcman_FormulaCircularityError(object sender, Infragistics.WebUI.UltraWebCalcManager.FormulaCircularityErrorEventArgs e)
    {

    }
    protected void toolbar_ButtonClicked(object sender, Infragistics.WebUI.UltraWebToolbar.ButtonEvent be)
    {

    }

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        gridreport.InitializeDataSource += new InitializeDataSourceEventHandler(gridreport_InitializeDataSource);
        gridreport.InitializeRow += new InitializeRowEventHandler(gridreport_InitializeRow);
    }


    void gridreport_InitializeDataSource(object sender, UltraGridEventArgs e)
    {
        getGrid();
    }

    protected void gridreport_InitializeRow(object sender, RowEventArgs e)
    {
        if (string.IsNullOrEmpty(reqReport.StaticReportSQL))
        {
            if (e.Row.Cells.FromKey("viewclaim") != null)
            {
                e.Row.Cells.FromKey("viewclaim").Value = "<a href=\"javascript:viewclaim(" + ViewState["requestnum"] + "," + e.Row.Cells.FromKey(ViewState["claimidcol"].ToString()).Value + ",";
                if (ViewState["expenseidcol"] != null)
                {
                    e.Row.Cells.FromKey("viewclaim").Value += e.Row.Cells.FromKey(ViewState["expenseidcol"].ToString()).Value.ToString();
                }
                else
                {
                    e.Row.Cells.FromKey("viewclaim").Value += "0";
                }
                e.Row.Cells.FromKey("viewclaim").Value += ");\">View Claim</a>";
            }
            if (e.Row.Cells.FromKey("viewcontract") != null)
            {
                e.Row.Cells.FromKey("viewcontract").Value = "<a href=\"javascript:viewcontract(" + ViewState["requestnum"] + ",";
                if (ViewState["contractidcol"] != null)
                {
                    e.Row.Cells.FromKey("viewcontract").Value += e.Row.Cells.FromKey(ViewState["contractidcol"].ToString()).Value.ToString();
                }
                else
                {
                    e.Row.Cells.FromKey("viewcontract").Value += "0";
                }
                e.Row.Cells.FromKey("viewcontract").Value += ");\">View Contract</a>";
            }

            if (e.Row.Cells.FromKey("viewsupplier") != null)
            {
                cAccountSubAccounts clsSubAccounts = new cAccountSubAccounts((int)ViewState["accountid"]);
                cAccountProperties clsProperties = clsSubAccounts.getSubAccountById((int)ViewState["subaccountid"]).SubAccountProperties;

                e.Row.Cells.FromKey("viewsupplier").Value = "<a href=\"javascript:viewsupplier(" + ViewState["requestnum"] + ",";
                if (ViewState["supplieridcol"] != null)
                {
                    e.Row.Cells.FromKey("viewsupplier").Value += e.Row.Cells.FromKey(ViewState["supplieridcol"].ToString()).Value.ToString();
                }
                else
                {
                    e.Row.Cells.FromKey("viewsupplier").Value += "0";
                }
                e.Row.Cells.FromKey("viewsupplier").Value += ");\">View " + clsProperties.SupplierPrimaryTitle + "</a>";
            }

            if (e.Row.Cells.FromKey("viewtask") != null)
            {
                e.Row.Cells.FromKey("viewtask").Value = "<a href=\"javascript:viewtask(" + ViewState["requestnum"] + ",";
                if (ViewState["taskidcol"] != null)
                {
                    e.Row.Cells.FromKey("viewtask").Value += e.Row.Cells.FromKey(ViewState["taskidcol"].ToString()).Value.ToString();
                }
                else
                {
                    e.Row.Cells.FromKey("viewtask").Value += "0";
                }
                e.Row.Cells.FromKey("viewtask").Value += ");\">View Task</a>";
            }

            if (TotalColumnID >= 0)
            {
                UltraGridRow row = e.Row;
                if (row.Cells[TotalColumnID] is decimal)
                {
                    GrandTotal += Convert.ToDecimal(row.Cells[TotalColumnID].Value);
                }
            }
        
            foreach (UltraGridCell cell in e.Row.Cells)
            {
                var c = cell.Text;
                Guid fileGuid;
                if (Guid.TryParse(c, out fileGuid))
                {
                    var images = this.CustomEntityImageData.GetHtmlImageData(fileGuid.ToString());
                    if (images != null)
                    {
                        cell.Text = images.FileName;
                    }
                }
            }
        }
    }

    protected void gridreport_UnGroupColumn(object sender, ColumnEventArgs e)
    {
        gridreport.DataBind();
    }
    protected void gridreport_DataBinding(object sender, EventArgs e)
    {

    }

    [WebMethod(EnableSession = true)]
    public static void updateDrilldownReport(int accountid, int employeeid, Guid reportid, Guid drilldown)
    {

        IReports clsreports = (IReports)Activator.GetObject(typeof(IReports), ConfigurationManager.AppSettings["ReportsServicePath"] + "/reports.rem");
        clsreports.updateDrillDownReport(accountid, employeeid, reportid, drilldown);
    }

    [WebMethod(EnableSession = true)]
    public static Guid saveAs(int accountID, Guid reportID, int employeeID, string reportName, Guid? folderID)
    {
        CurrentUser currentUser = cMisc.GetCurrentUser();
        cReports clsReports = new cReports(currentUser.AccountID, currentUser.CurrentSubAccountId);
        Guid newReportID = clsReports.SaveReportAs(reportID, employeeID, reportName, folderID, currentUser);

        return newReportID;
    }

    protected void calcman_FormatValue(object sender, Infragistics.WebUI.UltraWebCalcManager.FormatValueEventArgs e)
    {

    }
    protected void calcman_ParseValue(object sender, Infragistics.WebUI.UltraWebCalcManager.ParseValueEventArgs e)
    {

    }
    protected void calcman_CalculationsCompleted(object sender, EventArgs e)
    {

    }

    protected void gridreport_PreRender(object sender, EventArgs e)
    {
        if (GrandTotal > 0)
        {
            WebPanel1.Header.Text += " - Grand Total = " + GrandTotal.ToString("#,###,###.00");
        }
    }

    /// <summary>
    /// Get the format of decimal number values and check if they are userdefined as these can be greater than the default of 2 decimal places 
    /// </summary>
    /// <param name="fieldName"></param>
    /// <returns></returns>
    private string getPrecisonFormat(string fieldName)
    {
        string strFormat = string.Empty;

        if (fieldName.StartsWith("udf"))
        {
            int udfID = 0;
            int.TryParse(fieldName.Substring(3), out udfID);

            if (udfID > 0)
            {
                cUserdefinedFields clsUdfFields = new cUserdefinedFields((int)ViewState["accountid"]);
                cUserDefinedField udf = clsUdfFields.GetUserDefinedById(udfID);

                if (udf != null)
                {
                    if (udf.attribute.GetType() == typeof(cNumberAttribute))
                    {
                        int precision = ((cNumberAttribute)udf.attribute).precision;

                        if (precision > 0)
                        {
                            strFormat = "#,###,##0.";

                            for (int x = 0; x < precision; x++)
                            {
                                strFormat += "0";
                            }
                        }
                        else
                        {
                            strFormat = "#,###,##0";
                        }
                    }
                }
            }
        }

        if (strFormat == string.Empty)
        {
            strFormat = "#,###,##0.00";
        }
        return strFormat;
    }
}
