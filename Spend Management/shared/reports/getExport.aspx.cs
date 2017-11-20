namespace Spend_Management
{
    using System;
    using System.Configuration;
    using System.IO;

    using SpendManagementLibrary;

    public partial class getExport : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack == false)
            {

                try
                {
                    var rptfunctions = new ReportFunctions();

                    byte[] report;

                    var exporttype = (ExportType)this.Session["exporttype"];
                    var exportrequest = (cReportRequest)this.Session["request" + this.Session["exportrequestnum"]];

                    string reportFilePath = ConfigurationManager.AppSettings["ReportsOutputFilePath"];
                    if (string.IsNullOrEmpty(reportFilePath))
                    {
                        var ex = new ArgumentNullException(
                            reportFilePath, @"ReportsOutputFilePath key is null or empty in configuration file.");
                        throw ex;
                    }

                    if (!reportFilePath.EndsWith("\\"))
                    {
                        reportFilePath += "\\";
                    }

                    reportFilePath = string.Format("{0}{1}.sel", reportFilePath, exportrequest.ReportFileGuid);
                    if (!File.Exists(reportFilePath))
                    {
                        report = (byte[])rptfunctions.getReportData(exportrequest.requestnum);
                        reportFilePath = string.Empty;
                    }
                    else
                    {
                        report = File.ReadAllBytes(reportFilePath);
                    }

                    if (report == null || report.Length == 0)
                    {
                        var ex = (Exception)rptfunctions.GetReportError(exportrequest.requestnum);
                        string message = ex == null
                                             ? "There are no entries to export from this report."
                                             : "There has been a problem exporting this report.";

                        Response.Write(
                            string.Format(
                                "<html><head><title>{0}</title></head><body><div style=\"padding: 5px;\"><p style=\"font-family: arial; font-size: 12px;\">{2}</p><p><img src=\"{1}/shared/images/buttons/cancel_up.gif\" onclick=\"javascript:window.close();\" alt=\"Cancel\" /></p></div>",
                                exportrequest.report.reportname,
                                cMisc.Path,
                                message));

                        if (ex != null)
                        {
                            Response.Write(
                                string.Format(
                                    "<div style=\"display:none\">{0}<br />{1}</div>", ex.Message, ex.StackTrace));
                        }

                        Response.Write("</body></html>");
                        this.removeRequestFromQueue(exportrequest);
                        Response.End();
                    }

                    if (report != null && report.Length == 0)
                    {
                        this.removeRequestFromQueue(exportrequest);
                        Response.End();
                    }

                    this.displayExport(exporttype, exportrequest, report, reportFilePath);
                    Cache.Remove("exportdata" + User.Identity.Name);
                    Cache.Remove("exporttype" + User.Identity.Name);
                    this.removeRequestFromQueue(exportrequest);
                    Response.End();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.Print(ex.Message);
                }
            }       
        }

        public void displayExport(ExportType exporttype, cReportRequest clsrequest, byte[] report, string fileName)
        {
            CurrentUser user = cMisc.GetCurrentUser();

                switch (clsrequest.report.exportoptions.application)
                {
                    case FinancialApplication.CustomReport:
                        switch (exporttype)
                        {
                            case ExportType.Excel:
                            case ExportType.Pivot:
                                Response.AppendHeader("content-disposition", string.Format("attachment; filename={0}.xls", clsrequest.report.reportname));
                                Response.ContentType = "application/vnd.ms-excel";

                                break;
                            case ExportType.CSV:
                                Response.AppendHeader("content-disposition", string.Format("attachment; filename={0}.csv", clsrequest.report.reportname));
                                Response.ContentType = "text/csv";
                                break;
                            case ExportType.FlatFile:
                                Response.AppendHeader("content-disposition", string.Format("attachment; filename={0}.txt", clsrequest.report.reportname));
                                Response.ContentType = "text/plain";
                                break;
                        }

                        break;
                    case FinancialApplication.ESR:
                        Response.AppendHeader("content-disposition", string.Format("attachment; filename={0}", clsrequest.report.exportoptions.ExportFileName));
                        Response.ContentType = "text/csv";
                        break;
                }

                var clsAudit = new cAuditLog(user.AccountID, user.EmployeeID);

                if (clsrequest.report.exportoptions != null && clsrequest.report.exportoptions.isfinancialexport)
                {
                    clsAudit.addRecord(SpendManagementElement.Reports, string.Format("Financial Export of type '{0} with the name {1}' ID {2} started exporting by employee {3}", exporttype, clsrequest.report.reportname, clsrequest.report.reportid, clsrequest.employeeid), 0);
                }
                else
                {
                    clsAudit.addRecord(SpendManagementElement.Reports, string.Format("Export report of type '{0} with the name {1}' ID {2} started exporting by employee {3}", exporttype, clsrequest.report.reportname, clsrequest.report.reportid, clsrequest.employeeid), 0);
                }

                // put a try around this as when prompted with Open/Save/Cancel on export excel, clicking Cancel gives error on the Flush()
                try
                {
                    if (Response.IsClientConnected)
                    {
                        if (string.IsNullOrEmpty(fileName))
                        {
                            Response.BinaryWrite(report);
                        }
                        else
                        {
                            Response.TransmitFile(fileName);
                        }

                        Response.Flush();    
                    }
                }
                catch
                {
                }
        }

        public void removeRequestFromQueue(cReportRequest req)
        {
            // Remove export request from the queue now it has been processed
            var clsreports = (IReports)Activator.GetObject(typeof(IReports), ConfigurationManager.AppSettings["ReportsServicePath"] + "/reports.rem");
            clsreports.cancelRequest(req);
        }
    }
}
