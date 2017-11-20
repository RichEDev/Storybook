

namespace Spend_Management.Expedite
{
    using System;
    using System.Configuration;
    using Spend_Management;
    using SpendManagementLibrary.Expedite;
    using System.Collections.Generic;
    using SM = SpendManagementLibrary.Expedite;
    using SpendManagementLibrary;
    using System.Timers;
    using System.Threading;
    using System.Linq;

    /// <summary>
    /// PaymentService class includes implementaion for existing finance report export access
    /// this class is used by Expedite Payment service related methods/api end points 
    /// </summary>
    public class PaymentService
    {
        internal delegate byte[] exportDelegate(cReportRequest request);
        public int requestnum;
        private static System.Timers.Timer ReportProcessStatusChecker;
        private ReportRequestStatus ReportProcessStatus;
        FinancialExport finExport = new FinancialExport();
        SM.PaymentService paymentService = new SM.PaymentService();
        List<Payment> listPayment = new List<Payment>();
        int financialexportid;
        decimal amountBeforeExport;
        AutoResetEvent reportProcessComplete = new AutoResetEvent(false);

        #region Methods

        /// <summary>
        /// ExtractFinancialExportFromReportService methos create report request , send report request to report service and returns the report data
        /// </summary>
        /// <param name="accountId">Account id of the expedite client</param>
        /// <returns>List of Payment class containing financial Export Download</returns> 
        public List<Payment> ExtractFinancialExportFromReportService(int accountId, int financialExportId)
        {

            ReportProcessStatusChecker = new System.Timers.Timer();
            if (ConfigurationManager.AppSettings["ReportsServicePath"] != null)
            {
                IReports clsreports =
                    (IReports)
                        Activator.GetObject(
                            typeof(IReports), ConfigurationManager.AppSettings["ReportsServicePath"] + "/reports.rem");
                ExportType exporttype;
                CurrentUser user = cMisc.GetCurrentUser();
                Guid reportId = Guid.Empty;
                int exporthistoryid = 0;
                bool claimants = false;
                cReportRequest clsrequest;
                cExportOptions exportOptions = null;
                FinancialApplication application;
                cFinancialExport export = null;
                financialexportid = financialExportId;
                if (financialexportid > 0)
                {
                    var clsexports = new Spend_Management.cFinancialExports(accountId);
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
                    if (exportOptions.financialexport.NHSTrustID > 0)
                    {
                        var clsTrusts = new cESRTrusts(accountId);
                        exportOptions.ExportFileName = clsTrusts.CreateESRInboundFilename(
                            exportOptions.financialexport.NHSTrustID);
                    }
                }
                else
                {
                    application = FinancialApplication.CustomReport;
                }
                cReport rpt = null;
                if (export != null)
                {
                    rpt = clsreports.getReportById(user.AccountID, export.reportid);
                    finExport.ReportName = rpt.reportname;
                }
                if (rpt != null)
                {
                    rpt.exportoptions = exportOptions;
                    if (!rpt.SubAccountID.HasValue)
                    {
                        rpt.SubAccountID = user.CurrentSubAccountId;
                    }
                }
                clsrequest = new cReportRequest(
                    accountId,
                    user.CurrentSubAccountId,
                    this.requestnum,
                    (cReport)rpt,
                    (ExportType)export.exporttype,
                    exportOptions,
                    claimants,
                    user.EmployeeID,
                    user.HighestAccessLevel);

                if (exportOptions != null && exportOptions.footerreport != null)
                {
                    exportOptions.footerreport.isFooter = true;
                    exportOptions.footerreport.exportoptions = clsreports.getExportOptions(
                        user.AccountID, user.EmployeeID, exportOptions.footerreport.reportid);
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
                if (exportOptions == null)
                {
                    exportOptions = clsrequest.report.exportoptions;
                }
                clsrequest.report.exportoptions = exportOptions;
                cReportRequest exportrequest = null;
                switch (application)
                {
                    case FinancialApplication.CustomReport:
                        exportrequest = new cReportRequest(
                            accountId,
                            clsrequest.SubAccountId,
                            clsrequest.requestnum,
                            clsrequest.report,
                            (ExportType)export.exporttype,
                            exportOptions,
                            claimants,
                            user.EmployeeID,
                            ReportRunFrom.PrimaryServer,
                            user.HighestAccessLevel);
                        break;
                    case FinancialApplication.ESR:
                        exportrequest = new cReportRequest(
                            accountId,
                            clsrequest.SubAccountId,
                            clsrequest.requestnum,
                            clsrequest.report,
                            ExportType.CSV,
                            exportOptions,
                            false,
                            user.EmployeeID,
                            ReportRunFrom.PrimaryServer,
                            user.HighestAccessLevel);
                        break;
                }
                if (financialexportid > 0)
                {
                    exportrequest.reportRunFrom = ReportRunFrom.PrimaryServer;
                }

                if (exportrequest.AccessLevel == AccessRoleLevel.SelectedRoles &&
                    cReport.canFilterByRole(exportrequest.report.basetable.TableID))
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
                    exportrequest.AccessLevelRoles = reportRoles.ToArray();
                }
                amountBeforeExport = paymentService.GetAmountExportedByFinancialExport(accountId, financialExportId);
                ReportProcessStatusChecker.Interval = 500;
                ReportProcessStatusChecker.AutoReset = false;
                ReportProcessStatusChecker.Elapsed += (sender, e) => ReportProcessStatusMonitor(sender, e, exportrequest);
                bool status = clsreports.createReport(exportrequest);
                if (status)
                {
                    ReportProcessStatusChecker.Start();
                }
                else
                {
                    RemoveRequestFromQueue(exportrequest);
                }
                reportProcessComplete.WaitOne();
            }
            return listPayment;
        }

        /// <summary>
        /// ReportProcessStatusMonitor Timer event which monitor the report process status if the report is processed successfully.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        /// <param name="exportrequest"></param>
        public void ReportProcessStatusMonitor(object source, ElapsedEventArgs e, cReportRequest exportrequest)
        {
            object[] reportData;
            var clsreports = (IReports)Activator.GetObject(typeof(IReports), ConfigurationManager.AppSettings["ReportsServicePath"] + "/reports.rem");
            reportData = clsreports.getReportProgress(exportrequest);
            if (reportData != null)
                ReportProcessStatus = (ReportRequestStatus)Enum.Parse(typeof(ReportRequestStatus), reportData[0].ToString(), true);
            if (ReportProcessStatus == ReportRequestStatus.BeingProcessed)
            {
                ReportProcessStatusChecker.Start();
            }
            else if (ReportProcessStatus == ReportRequestStatus.Complete)
            {
                ReportProcessStatusChecker.Stop();
                object[] exportData = clsreports.getReportDataForExpedite(exportrequest);
                if (exportData != null)
                {
                    finExport.ReportData = (byte[])exportData[0];
                    int exportHistory = Convert.ToInt32(exportData[1]);

                    RemoveRequestFromQueue(exportrequest);
                    finExport.ExportType = (ExportType)exportrequest.exporttype;
                    finExport.Amount = paymentService.GetAmountExportedByFinancialExport(exportrequest.accountid, financialexportid) - amountBeforeExport;
                    finExport.Id = financialexportid;
                    finExport.ExportHistoryId = exportHistory;
                    List<FinancialExport> financialExport = new List<FinancialExport>();
                    financialExport.Add(finExport);
                    Payment payment = new Payment(exportrequest.accountid, financialExport);
                    listPayment.Add(payment);
                    reportProcessComplete.Set();
                }
            }
            else if (ReportProcessStatus == ReportRequestStatus.Failed)
            {
                ReportProcessStatusChecker.Stop();
                RemoveRequestFromQueue(exportrequest);
                reportProcessComplete.Set();
            }
        }
        /// <summary>
        /// RemoveRequestFromQueue method remove the report request from queue 
        /// </summary>
        /// <param name="exportrequest">Export request to report service</param>
        public void RemoveRequestFromQueue(cReportRequest exportrequest)
        {
            // Remove export request from the queue now it has been processed
            var clsreports = (IReports)Activator.GetObject(typeof(IReports), ConfigurationManager.AppSettings["ReportsServicePath"] + "/reports.rem");
            clsreports.cancelRequest(exportrequest);
        }
        #endregion

    }
}
