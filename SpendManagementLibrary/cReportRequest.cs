using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using SpendManagementLibrary;

namespace SpendManagementLibrary
{
    [Serializable()]
    public class cReportRequest
    {
        private int nAccountid;
        private int nRequestNum;
        private cReport clsreport;
        ExportType etExporttype;
        private bool bClaimantreport;
        private int nEmployeeid;
        private int nProcessedRows = 0;
        private int nRowCount = 0;
        private object oReportData;
        ReportRequestStatus eStatus;
        DateTime dtCompletionTime;
        ReportRunFrom eReportRunFrom;
        private int nSubAccountId;
        private AccessRoleLevel eAccessRoleLevel;
        protected int[] arrAccessLevelRoles;
        private Guid gSchedulerRequestID;

        /// <summary>
        /// Gets or sets the report output file path for exports.
        /// </summary>
        public Guid ReportFileGuid { get; set; }

        public Exception Exception { get; set; }

        public cReportRequest(int accountid, int subaccountid, int requestnum, cReport report, ExportType exporttype, cExportOptions exportoptions, bool claimantreport, int employeeid, AccessRoleLevel accesslevel)
        {
            nAccountid = accountid;
            nRequestNum = requestnum;
            clsreport = report;
            etExporttype = exporttype;
            bClaimantreport = claimantreport;
            nEmployeeid = employeeid;
            eReportRunFrom = ReportRunFrom.ReportsServer;
            nSubAccountId = subaccountid;
            eAccessRoleLevel = accesslevel;
            gSchedulerRequestID = Guid.Empty;
            ReportFileGuid = Guid.NewGuid();
        }


        public cReportRequest(int accountid, int subaccountid, int requestnum, cReport report, ExportType exporttype, cExportOptions exportoptions, bool claimantreport, int employeeid, ReportRunFrom reportRunFrom, AccessRoleLevel accesslevel)
        {
            nAccountid = accountid;
            nRequestNum = requestnum;
            clsreport = report;
            etExporttype = exporttype;
            bClaimantreport = claimantreport;
            nEmployeeid = employeeid;
            eReportRunFrom = reportRunFrom;
            nSubAccountId = subaccountid;
            eAccessRoleLevel = accesslevel;
            gSchedulerRequestID = Guid.Empty;
            ReportFileGuid = Guid.NewGuid();
        }

        public cReportRequest()
        {
        }



        #region properties

        public ReportRunFrom reportRunFrom
        {
            get { return eReportRunFrom; }
            set { eReportRunFrom = value; }
        }

        public int accountid
        {
            get { return nAccountid; }
            set { nAccountid = value; }
        }
        public int requestnum
        {
            get { return nRequestNum; }
            set { nRequestNum = value; }
        }
        public cReport report
        {
            get { return clsreport; }
            set { clsreport = value; }
        }
        public ExportType exporttype
        {
            get { return etExporttype; }
            set { etExporttype = value; }
        }
        public bool claimantreport
        {
            get { return bClaimantreport; }
            set { claimantreport = value; }
        }
        public int employeeid
        {
            get { return nEmployeeid; }
            set { nEmployeeid = value; }
        }
        public int RowCount
        {
            get { return nRowCount; }
            set { nRowCount = value; }
        }
        public int ProcessedRows
        {
            get { return nProcessedRows; }
            set { nProcessedRows = value; }
        }
        public int PercentageProcessed
        {
            get
            {
                if (RowCount == 0)
                {
                    return 0;
                }
                else
                {
                    return (int)(((decimal)ProcessedRows / (decimal)RowCount) * 100);
                }
            }
            set { RowCount = value; }
        }

        public object ReportData
        {
            get { return oReportData; }
            set { oReportData = value; }
        }
        public ReportRequestStatus Status
        {
            get { return eStatus; }
            set { eStatus = value; }
        }
        public DateTime CompletionTime
        {
            get { return dtCompletionTime; }
            set { dtCompletionTime = value; }
        }
        public int SubAccountId
        {
            get { return nSubAccountId; }
        }
        public AccessRoleLevel AccessLevel
        {
            get { return eAccessRoleLevel; }
        }
        public int[] AccessLevelRoles
        {
            get { return arrAccessLevelRoles; }
            set { arrAccessLevelRoles = value; }
        }
        public Guid SchedulerRequestID
        {
            get { return gSchedulerRequestID; }
            set { gSchedulerRequestID = value; }
        }

        /// <summary>
        /// Gets or sets the <see cref="ReportChart"/> for this request.  If null, the system will get from the database.
        /// </summary>
        public ReportChart ReportChart { get; set; }
        #endregion
    }



    [Serializable()]
    public enum ReportRunFrom
    {
        PrimaryServer,
        ReportsServer
    }

    [Serializable()]
    public enum ReportRequestStatus
    {
        BeingProcessed,
        Complete,
        Failed,
        Queued
    }
}
