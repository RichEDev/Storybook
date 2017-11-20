using System;
using System.Collections;
using System.Text;
using System.Data;
using SpendManagementLibrary;
using System.Collections.Generic;

namespace SpendManagementLibrary
{
    using SpendManagementLibrary.Definitions;

    public interface IReports
    {
        Exception GetReportError(cReportRequest request);
        
        
        
        
        bool createReport(cReportRequest request);
        DataSet createSynchronousReport(cReportRequest request);
        bool test();
        cReport getReportById(int accountid, Guid reportid);
        byte[] exportReport(cReportRequest request);
        
        
        void updateExportOptions(int accountid, cExportOptions options);
        cExportOptions getExportOptions(int accountid, int employeeid, Guid reportid, bool isFinancialExport = false);
        
        System.Data.DataSet getHistoryGrid(int accountid, Guid reportid);
        
        void updateDrillDownReport(int accountid, int employeeid, Guid reportid, Guid drilldown);

        DataSet refreshReportData(cReportRequest request);
        DataSet GetChartData(cReportRequest request, DataSet chartData);



        int getReportCount(cReportRequest request);
        void cancelRequest(cReportRequest request);
        object[] getReportProgress(cReportRequest request);
        object getReportData(cReportRequest request);
        
        List<ReportRequestInformation> GetCurrentRequests();
        List<ReportThreadInformation> GetCurrentThreads();
        
        /// <summary>
        /// Get Report data for expedite payment service
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        object[] getReportDataForExpedite(cReportRequest request);
    }

	

	
}
