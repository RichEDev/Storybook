#region Using Directives

using System;
using System.Configuration;
using System.Text;
using System.Web.UI;

using Spend_Management;

using SpendManagementLibrary;

#endregion


internal delegate byte[] exportDelegate(cReportRequest request);


public partial class reports_exportreport : Page
{
    #region Fields


    public int requestnum;

    #endregion

    #region Public Methods and Operators

    public void displayExport(ExportType exporttype, cReportRequest clsrequest, byte[] report)
    {
        switch (clsrequest.report.exportoptions.application)
        {
            case FinancialApplication.CustomReport:
                switch (exporttype)
                {
                    case ExportType.Excel:
                    case ExportType.Pivot:
                        this.Response.AppendHeader(
                            "content-disposition", string.Format("attachment; filename={0}.xls", clsrequest.report.reportname));
                        this.Response.ContentType = "application/vnd.ms-excel";

                        break;
                    case ExportType.CSV:
                        this.Response.AppendHeader(
                            "content-disposition", string.Format("attachment; filename={0}.csv", clsrequest.report.reportname));
                        this.Response.ContentType = "text/csv";
                        break;
                    case ExportType.FlatFile:
                        this.Response.AppendHeader(
                            "content-disposition", string.Format("attachment; filename={0}.txt", clsrequest.report.reportname));
                        this.Response.ContentType = "text/plain";
                        break;
                }

                break;
            case FinancialApplication.ESR:
                this.Response.ContentType = "text/csv";
                break;
        }

        CurrentUser user = cMisc.GetCurrentUser();
        var clsAudit = new cAuditLog(user.AccountID, user.EmployeeID);

        clsAudit.addRecord(
            SpendManagementElement.Reports,
            clsrequest.report.exportoptions.isfinancialexport
                ? string.Format(
                    "Financial Export of type '{0} with the name {1}' ID {2} started exporting by employee {3}",
                    exporttype,
                    clsrequest.report.reportname,
                    clsrequest.report.reportid,
                    clsrequest.employeeid)
                : string.Format(
                    "Export report of type '{0} with the name {1}' ID {2} started exporting by employee {3}",
                    exporttype,
                    clsrequest.report.reportname,
                    clsrequest.report.reportid,
                    clsrequest.employeeid),
            0);

        this.Response.BinaryWrite(report);
        this.Response.Flush();
    }

    #endregion

    #region Methods

    protected void Page_Load(object sender, EventArgs e)
    {
        CurrentUser user = cMisc.GetCurrentUser();
        this.ViewState["accountid"] = user.AccountID;
        this.ViewState["employeeid"] = user.EmployeeID;

        if (this.IsPostBack == false)
        {
            ExportType exporttype;

            var clsreports =
                (IReports)
                Activator.GetObject(
                    typeof(IReports), ConfigurationManager.AppSettings["ReportsServicePath"] + "/reports.rem");

            var clsemployees = new cEmployees(user.AccountID);

            int financialexportid = 0;
            if (this.Request.QueryString["financialexportid"] != null)
            {
                financialexportid = int.Parse(this.Request.QueryString["financialexportid"]);
            }

            Guid reportId = Guid.Empty;
            if (this.Request.QueryString["reportid"] != null)
            {
                reportId = new Guid(this.Request.QueryString["reportid"]);
            }

            int exporthistoryid = 0;
            if (this.Request.QueryString["exporthistoryid"] != null)
            {
                exporthistoryid = Convert.ToInt32(this.Request.QueryString["exporthistoryid"]);
            }

            bool claimants = false;

            this.ViewState["reportid"] = reportId;

            if (user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.Reports, true) == false)
            {
                claimants = true;
            }

            if (this.Request.QueryString["exporttype"] != null)
            {
                exporttype = (ExportType)byte.Parse(this.Request.QueryString["exporttype"]);
            }
            else
            {
                exporttype = ExportType.Viewer;
            }

            cReportRequest clsrequest;
            cExportOptions exportOptions = null;
            FinancialApplication application;

            cFinancialExport export = null;
            if (financialexportid > 0)
            {
                var clsexports = new cFinancialExports(user.AccountID);
                export = clsexports.getExportById(financialexportid);
                reportId = export.reportid;
                exportOptions = clsreports.getExportOptions(user.AccountID, user.EmployeeID, reportId, true);
                exporttype = (ExportType)export.exporttype;
                exportOptions.isfinancialexport = true;
                exportOptions.financialexport = export;
                exportOptions.application = export.application;
                exportOptions.exporthistoryid = exporthistoryid;
                exportOptions.PreventNegativePayment = export.PreventNegativeAmountPayable;
                application = export.application;
                this.Session["financialexportid"] = financialexportid;

                if (exportOptions.financialexport.NHSTrustID > 0)
                {
                    var clsTrusts = new cESRTrusts((int)this.ViewState["accountid"]);
                    exportOptions.ExportFileName = clsTrusts.CreateESRInboundFilename(
                        exportOptions.financialexport.NHSTrustID);
                }
            }
            else
            {
                application = FinancialApplication.CustomReport;
            }

            // This is passed in to reset the session variables that are storing reports that have been amended or stored from the report viewer
            // as the request for export has come from a source that requires the report to be in its original form.
            if (this.Request.QueryString["rstSessVars"] != null)
            {
                int resetSessionVariables;
                int.TryParse(this.Request.QueryString["rstSessVars"], out resetSessionVariables);

                if (resetSessionVariables == 1)
                {
                    this.Session["ReportViewer"] = false;
                }
            }

            if (this.Request.QueryString["requestnum"] != null)
            {
                this.requestnum = int.Parse(this.Request.QueryString["requestnum"]);
            }
            else
            {
                // generate a new one
                if (this.Session["currentrequestnum"] == null)
                {
                    this.Session["currentrequestnum"] = 0;
                }

                var currentrequestnum = (int)this.Session["currentrequestnum"];

                cReport rpt = null;

                if ((bool)this.Session["ReportViewer"] == false)
                {
                    this.requestnum = currentrequestnum + 1;
                    this.Session["currentrequestnum"] = this.requestnum;

                    switch (application)
                    {
                        case FinancialApplication.CustomReport:
                            rpt = clsreports.getReportById(user.AccountID, reportId);
                            this.ViewState["reportid"] = reportId;
                            break;
                        case FinancialApplication.ESR:
                            if (export != null)
                            {
                                rpt = clsreports.getReportById(user.AccountID, export.reportid);

                                this.ViewState["reportid"] = export.reportid;
                            }

                            break;
                    }
                }
                else
                {
                    var tempReq = (cReportRequest)this.Session["request" + currentrequestnum];
                    rpt = tempReq.report;
                    this.requestnum = currentrequestnum;
                    this.ViewState["reportid"] = reportId;
                    this.Session["currentrequestnum"] = this.requestnum;
                }

                if (exportOptions == null)
                {
                    if (exporttype == ExportType.Pivot)
                    {
                        // report MUST have headers for a pivot report, regardless of export options
                        exportOptions = new cExportOptions(
                            user.EmployeeID, 
                            reportId, 
                            true, 
                            true, 
                            true, 
                            null, 
                            null, 
                            Guid.Empty, 
                            FinancialApplication.CustomReport, 
                            ",", 
                            false, 
                            true,
                            false);
                    }
                    else
                    {
                        exportOptions = clsreports.getExportOptions(user.AccountID, user.EmployeeID, reportId);
                    }
                }

                if (rpt != null)
                {
                    rpt.exportoptions = exportOptions;

                    if (!rpt.SubAccountID.HasValue)
                    {
                        rpt.SubAccountID = user.CurrentSubAccountId;
                    }

                    this.Session["rpt" + rpt.reportid] = rpt;
                }

                clsrequest = new cReportRequest(
                    (int)this.ViewState["accountid"], 
                    user.CurrentSubAccountId, 
                    this.requestnum, 
                    (cReport)this.Session["rpt" + this.ViewState["reportid"]], 
                    exporttype, 
                    exportOptions, 
                    claimants, 
                    user.EmployeeID, 
                    user.HighestAccessLevel);

                if (exportOptions != null && exportOptions.footerreport != null)
                {
                    exportOptions.footerreport.isFooter = true;

                    exportOptions.footerreport.exportoptions = clsreports.getExportOptions(
                        user.AccountID, user.EmployeeID, exportOptions.footerreport.reportid);

                    // replace criteria with our report
                    exportOptions.footerreport.criteria.Clear();

                    for (int i = 0; i < clsrequest.report.criteria.Count; i++)
                    {
                        exportOptions.footerreport.criteria.Add(clsrequest.report.criteria[i]);
                    }

                    if (financialexportid > 0)
                    {
                        exportOptions.footerreport.exportoptions.isfinancialexport = true;
                        exportOptions.footerreport.exportoptions.financialexport = export;
                        exportOptions.footerreport.exportoptions.application = export.application;
                        exportOptions.footerreport.exportoptions.exporthistoryid = exporthistoryid;
                    }
                }

                this.Session["request" + this.requestnum] = clsrequest;

                if (rpt.hasRuntimeCriteria() && rpt.runtimecriteriaset == false)
                {
                    this.Response.Write(
                        string.Format(
                            "<script type='text/javascript'>window.open('enteratruntime.aspx?requestnum={0}&exporttype={1}&financialexportid={2}&exporthistoryid={3}', 'runtime');</script>", 
                            this.requestnum, 
                            (byte)exporttype, 
                            financialexportid, 
                            exporthistoryid));
                    this.Response.Write("<script type=\"text/javascript\">window.close();</script>");
                    this.Response.End();
                }
            }

            clsrequest = (cReportRequest)this.Session["request" + this.requestnum];
            if (exportOptions == null)
            {
                exportOptions = clsrequest.report.exportoptions ?? clsreports.getExportOptions(user.AccountID, user.EmployeeID, clsrequest.report.reportid);
                clsrequest.report.exportoptions = exportOptions;
            }

            if (this.Request.QueryString["exporthistoryid"] != null)
            {
                exporthistoryid = int.Parse(this.Request.QueryString["exporthistoryid"]);
                exportOptions.exporthistoryid = int.Parse(this.Request.QueryString["exporthistoryid"]);
            }

            clsrequest.report.exportoptions = exportOptions;
                
                // clsrequest.exportoptions; -- THIS DISREGARDS THE EXPORT OPTIONS
            if (exporttype == ExportType.Pivot)
            {
                this.litPivotMessage.Text =
                    "<div style=\"height: 25px; font-style: Arial; font-size: 10px; color: Red; border: 1px dotted Red;\"><strong>NOTE:</strong>&nbsp;File MUST be saved for pivot export to work. Do not use the Open option at the prompt.<p></p></div>";
            }

            switch (application)
            {
                case FinancialApplication.CustomReport:
                    new cReportRequest(
                        (int)this.ViewState["accountid"], 
                        clsrequest.SubAccountId, 
                        clsrequest.requestnum, 
                        clsrequest.report, 
                        exporttype, 
                        exportOptions, 
                        claimants, 
                        user.EmployeeID, 
                        user.HighestAccessLevel);
                    break;
                case FinancialApplication.ESR:
                    new cReportRequest(
                        (int)this.ViewState["accountid"], 
                        clsrequest.SubAccountId, 
                        clsrequest.requestnum, 
                        clsrequest.report, 
                        ExportType.CSV, 
                        exportOptions, 
                        false, 
                        user.EmployeeID, 
                        user.HighestAccessLevel);
                    break;
            }

            this.btnDownload.PostBackUrl = "getExport.aspx?requestnum=" + this.requestnum;

            var output = new StringBuilder();
            output.Append("<script type=\"text/javascript\" language=\"javascript\">\n");
            output.Append("function window_onload()\n");
            output.Append("{\n");
            output.Append(
                "ReportFunctions.exportReport(" + this.requestnum + "," + (byte)exporttype + ",'" + reportId + "',"
                + financialexportid + "," + exporthistoryid + ",exportReportComplete, errorHandling);\n");
            output.Append("self.focus();");
            output.Append("}\n");
            output.Append("</script>");
            this.ClientScript.RegisterStartupScript(this.GetType(), "onload", output.ToString());

            this.Page.Title = string.Format("Exporting {0}", clsrequest.report.reportname);
        }
    }

    #endregion
}