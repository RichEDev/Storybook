using System;
using System.Collections.Generic;
using System.Text;

namespace SpendManagementLibrary
{
    [Serializable()]
    public class cFinancialExport
    {
        private int nFinancialExportID;
        private int nAccountid;
        private FinancialApplication faApplication;
        private Guid gReportid;
        private bool bAutomated;
        private int nCreatedBy;
        private DateTime dtCreatedOn;
        private int nCurexportnum;
        private DateTime dtLastexportdate;
        private byte bExporttype;
        private int nNHSTrustID;

        public cFinancialExport(int financialexportid, int accountid, FinancialApplication application, Guid reportid, bool automated, int createdby, DateTime createdon, int curexportnum, DateTime lastexportdate, byte exporttype, int NHSTrustID, bool preventNegativeAmountPayable,bool expeditePaymentReport)
        {
            this.PreventNegativeAmountPayable = preventNegativeAmountPayable;
            this.ExpeditePaymentReport = expeditePaymentReport;
            nFinancialExportID = financialexportid;
            nAccountid = accountid;
            faApplication = application;
            gReportid = reportid;
            bAutomated = automated;
            nCreatedBy = createdby;
            dtCreatedOn = createdon;
            nCurexportnum = curexportnum;
            dtLastexportdate = lastexportdate;
            bExporttype = exporttype;
            nNHSTrustID = NHSTrustID;
        }

        public bool PreventNegativeAmountPayable { get; set; }
        public bool ExpeditePaymentReport { get; set; }
        public void setFinancialExportID(int id)
        {
            nFinancialExportID = id;
        }
        #region properties
        public int financialexportid
        {
            get { return nFinancialExportID; }
        }
        public int accountid
        {
            get { return nAccountid; }
        }
        public FinancialApplication application
        {
            get { return faApplication; }
        }
        public Guid reportid
        {
            get { return gReportid; }
        }
        public bool automated
        {
            get { return bAutomated; }
        }
        public int createdby
        {
            get { return nCreatedBy; }
        }
        public DateTime createdon
        {
            get { return dtCreatedOn; }
        }
        public int curexportnum
        {
            get { return nCurexportnum; }
        }
        public DateTime lastexportdate
        {
            get { return dtLastexportdate; }
        }
        public byte exporttype
        {
            get { return bExporttype; }
        }
        public int NHSTrustID
        {
            get { return nNHSTrustID; }
        }
        #endregion
    }

    [Serializable()]
    public enum FinancialApplication
    {
        CustomReport = 1,
        ESR
    }

    [Serializable()]
    public enum FinancialExportStatus
    {
        None = 0,
        ExportFailed,               // 1
        ExportSucceeded,            // 2
        AwaitingESRInboundTransfer, // 3
        CollectedForUploadToESR,    // 4
        UploadToESRFailed,          // 5
        UploadToESRSucceeded,       // 6
        FileTransferProcessComplete // 7
    }
}
