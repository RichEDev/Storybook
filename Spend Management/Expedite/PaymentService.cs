

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
                ExportType exportType;        
                Guid reportId = Guid.Empty;
                int exporthistoryid = 0;
                bool claimants = false;
                cExportOptions exportOptions = null;
                FinancialApplication application;
                cFinancialExport export = null;
                var employees = new cEmployees(accountId);
              
                financialexportid = financialExportId;

                if (financialexportid > 0)
                {
                    var clsexports = new Spend_Management.cFinancialExports(accountId);
                    export = clsexports.getExportById(financialexportid);
                    reportId = export.reportid;
                    exportOptions = reports.getExportOptions(accountId, 0, reportId, true);
                    exportType = (ExportType)export.exporttype;
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
                    rpt = reports.getReportById(accountId, export.reportid);
                    finExport.ReportName = rpt.reportname;
                }
                if (rpt != null)
                {
                    rpt.exportoptions = exportOptions;
                    if (!rpt.SubAccountID.HasValue)
                    {
                        rpt.SubAccountID = 1;
                    }
                }

                var accessRoleLevel = AccessRoleLevel.AllData;

                var reportRequest = new cReportRequest(
                    accountId,
                    1,
                    this.requestnum,
                    (cReport)rpt,
                    (ExportType)export.exporttype,
                    exportOptions,
                    claimants,
                    0, accessRoleLevel);

                if (exportOptions != null && exportOptions.footerreport != null)
                {
                    exportOptions.footerreport.isFooter = true;
                    exportOptions.footerreport.exportoptions = reports.getExportOptions(
                      accountId, 0, exportOptions.footerreport.reportid);
                    exportOptions.footerreport.criteria.Clear();
                    for (int i = 0; i < reportRequest.report.criteria.Count; i++)
                    {
                        exportOptions.footerreport.criteria.Add(reportRequest.report.criteria[i]);
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
                    exportOptions = reportRequest.report.exportoptions;
                }
                reportRequest.report.exportoptions = exportOptions;

                cReportRequest exportRequest = null;

                switch (application)
                {
                    case FinancialApplication.CustomReport:
                      
                        exportRequest = new cReportRequest(
                            accountId,
                            reportRequest.SubAccountId,
                            reportRequest.requestnum,
                            reportRequest.report,
                            (ExportType)export.exporttype,
                            exportOptions,
                            claimants,
                            0,
                            ReportRunFrom.PrimaryServer, accessRoleLevel
                            );
                        break;
                    case FinancialApplication.ESR:
                        exportRequest = new cReportRequest(
                            accountId,
                            reportRequest.SubAccountId,
                            reportRequest.requestnum,
                            reportRequest.report,
                            ExportType.CSV,
                            exportOptions,
                            false,
                            0,
                            ReportRunFrom.PrimaryServer,
                            accessRoleLevel);
                        break;
                }
                if (financialexportid > 0)
                {
                    exportRequest.reportRunFrom = ReportRunFrom.PrimaryServer;
                }
      
                amountBeforeExport = paymentService.GetAmountExportedByFinancialExport(accountId, financialExportId);
                ReportProcessStatusChecker.Interval = 500;
                ReportProcessStatusChecker.AutoReset = false;
                ReportProcessStatusChecker.Elapsed += (sender, e) => ReportProcessStatusMonitor(sender, e, exportRequest, reports);
                bool status = reports.createReport(exportRequest);
                if (status)
                {
                    ReportProcessStatusChecker.Start();
                }
                else
                {
                    RemoveRequestFromQueue(exportRequest, reports);
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
