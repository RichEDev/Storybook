

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
        /// <param name="financialExportId">The ID of the financial export to extract</param>
        /// <param name="reports">An instance of <see cref="IReports"/></param>
        /// <returns>List of Payment class containing financial Export Download</returns> 
        public List<Payment> ExtractFinancialExportFromReportService(int accountId, int financialExportId, IReports reports)
        {

            ReportProcessStatusChecker = new System.Timers.Timer();
            if (reports != null)
            {
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
                    exportOptions = reports.getExportOptions(user.AccountID, user.EmployeeID, reportId, true);
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
                    rpt = reports.getReportById(user.AccountID, export.reportid);
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
                    exportOptions.footerreport.exportoptions = reports.getExportOptions(
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
                ReportProcessStatusChecker.Elapsed += (sender, e) => ReportProcessStatusMonitor(sender, e, exportrequest, reports);
                bool status = reports.createReport(exportrequest);
                if (status)
                {
                    ReportProcessStatusChecker.Start();
                }
                else
                {
                    RemoveRequestFromQueue(exportrequest, reports);
                }
                reportProcessComplete.WaitOne();
            }
            return listPayment;
        }

        /// <summary>
        /// Peridically check the report status, when finished get the data and process.
        /// </summary>
        /// <param name="source">
        /// The event source <see cref="object"/>
        /// </param>
        /// <param name="e">
        /// An instance of <see cref="ElapsedEventArgs"/>
        /// </param>
        /// <param name="exportrequest">
        /// An instance of <see cref="cReportRequest"/> that created the current running report
        /// </param>
        /// <param name="reports">
        /// An instance of <see cref="IReports"/>
        /// </param>
        public void ReportProcessStatusMonitor(object source, ElapsedEventArgs e, cReportRequest exportrequest, IReports reports)
        {
            object[] reportData;
            reportData = reports.getReportProgress(exportrequest);
            if (reportData != null)
            {
                this.ReportProcessStatus = (ReportRequestStatus)Enum.Parse(
                    typeof(ReportRequestStatus),
                    reportData[0].ToString(),
                    true);
            }

            switch (this.ReportProcessStatus)
            {
                case ReportRequestStatus.Queued:
                case ReportRequestStatus.BeingProcessed:
                    ReportProcessStatusChecker.Start();
                    break;
                case ReportRequestStatus.Complete:
                    ReportProcessStatusChecker.Stop();
                    object[] exportData = reports.getReportDataForExpedite(exportrequest);
                    if (exportData != null)
                    {
                        this.finExport.ReportData = (byte[])exportData[0];
                        int exportHistory = Convert.ToInt32(exportData[1]);

                        this.RemoveRequestFromQueue(exportrequest, reports);
                        this.finExport.ExportType = (ExportType)exportrequest.exporttype;
                        this.finExport.Amount = this.paymentService.GetAmountExportedByFinancialExport(exportrequest.accountid, this.financialexportid) - this.amountBeforeExport;
                        this.finExport.Id = this.financialexportid;
                        this.finExport.ExportHistoryId = exportHistory;
                        List<FinancialExport> financialExport = new List<FinancialExport>();
                        financialExport.Add(this.finExport);
                        Payment payment = new Payment(exportrequest.accountid, financialExport);
                        this.listPayment.Add(payment);
                        this.reportProcessComplete.Set();
                    }

                    break;
                case ReportRequestStatus.Failed:
                    ReportProcessStatusChecker.Stop();
                    this.RemoveRequestFromQueue(exportrequest, reports);
                    this.reportProcessComplete.Set();

                    break;
            }
        }

        /// <summary>
        /// RemoveRequestFromQueue method remove the report request from queue 
        /// </summary>
        /// <param name="exportrequest">Export request to report service</param>
        /// <param name="reports">An instance of <see cref="IReports"/></param>
        public void RemoveRequestFromQueue(cReportRequest exportrequest, IReports reports)
        {
            // Remove export request from the queue now it has been processed
            reports.cancelRequest(exportrequest);
        }
        #endregion

    }
}
