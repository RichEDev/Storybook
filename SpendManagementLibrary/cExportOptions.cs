using System;
using System.Collections.Generic;

namespace SpendManagementLibrary
{
    [Serializable()]
    public class cExportOptions
    {
        public bool PreventNegativePayment { get; set; }

        private int nEmployeeid;
        private Guid gReportid;
        private bool bShowHeadersExcel;
        private bool bShowHeadersCSV;
        private bool bShowHeadersFlatfile;
        private SortedList<Guid, int> lstFlatfile;
        private bool bFinancialExport;
        private int nExporthistoryid;
        private cReport clsfooterreport;
        private Guid gDrilldownreport;
        private FinancialApplication faApplication;
        private cFinancialExport clsFinancialExport;
        private string sExportFilename;
        private string sDelimiter;
        private bool bExportAsESR;
        private bool bRemoveCarriageReturns;
        private bool bEncloseInSpeechMarks;
        private bool bPrimaryKeyInReport = false;
        private int nPrimaryKeyIndex = -1;

        public cExportOptions(int employeeid, Guid reportid, bool showheadersexcel, bool showheaderscsv, bool showheadersflatfile, SortedList<Guid, int> flatfile, cReport footerrpt, Guid drilldownreport, FinancialApplication application, string delimiter, bool removeCarriageReturns, bool encloseinspeechmarks, bool preventNegativePayment)
        {
            this.PreventNegativePayment = preventNegativePayment;
            nEmployeeid = employeeid;
            gReportid = reportid;
            bShowHeadersExcel = showheadersexcel;
            bShowHeadersCSV = showheaderscsv;
            bShowHeadersFlatfile = showheadersflatfile;
            lstFlatfile = flatfile;
            clsfooterreport = footerrpt;
            gDrilldownreport = drilldownreport;
            faApplication = application;
            sDelimiter = delimiter;
            bRemoveCarriageReturns = removeCarriageReturns;
            bEncloseInSpeechMarks = encloseinspeechmarks;
        }

        public cExportOptions()
        {
        }

        #region properties
        public int employeeid
        {
            get { return nEmployeeid; }
            set { nEmployeeid = value; }
        }
        public Guid reportid
        {
            get { return gReportid; }
            set { gReportid = value; }
        }
        public bool showheadersexcel
        {
            get { return bShowHeadersExcel; }
            set { bShowHeadersExcel = value; }
        }
        public bool showheaderscsv
        {
            get { return bShowHeadersCSV; }
            set { bShowHeadersCSV = value; }
        }
        public bool showheadersflatfile
        {
            get { return bShowHeadersFlatfile; }
            set { bShowHeadersFlatfile = value; }
        }
        public SortedList<Guid, int> flatfile
        {
            get { return lstFlatfile; }
            set { lstFlatfile = value; }
        }
        public bool isfinancialexport
        {
            get { return bFinancialExport; }
            set { bFinancialExport = value; }
        }
        public cFinancialExport financialexport
        {
            get { return clsFinancialExport; }
            set { clsFinancialExport = value; }
        }
        public int exporthistoryid
        {
            get { return nExporthistoryid; }
            set { nExporthistoryid = value; }
        }
        public cReport footerreport
        {
            get { return clsfooterreport; }
            set { clsfooterreport = value; }
        }
        public Guid drilldownreport
        {
            get { return gDrilldownreport; }
            set { gDrilldownreport = value; }
        }
        public FinancialApplication application
        {
            get { return faApplication; }
            set { faApplication = value; }
        }

        /// <summary>
        /// Used to set the export filename 
        /// </summary>
        public string ExportFileName
        {
            get { return sExportFilename; }
            set { sExportFilename = value; }
        }

        /// <summary>
        /// Gets or sets the delimiter to use when using exportDelimited
        /// </summary>
        public string Delimiter
        {
            get {
                if (sDelimiter == "\\t")
                {
                    return "\t";
                }
                else
                {
                    if (sDelimiter == "")
                    {
                        return ",";
                    }
                    else
                    {
                        return sDelimiter;
                    }
                }
            }
            set { sDelimiter = value; }
        }
        /// <summary>
        /// Gets whether to remove any user entered carriage returns from string fields when exporting
        /// </summary>
        public bool RemoveCarriageReturns
        {
            get { return bRemoveCarriageReturns; }
        }

        /// <summary>
        /// Gets or sets if the report should be exported with file headers + specific file name
        /// </summary>
        public bool ExportAsESR
        {
            get { return bExportAsESR; }
            set { bExportAsESR = value; }
        }
        /// <summary>
        /// Gets whether to surround string fields in speech marks when exporting
        /// </summary>
        public bool EncloseInSpeechMarks
        {
            get { return bEncloseInSpeechMarks; }
        }

        /// <summary>
        /// Gets or Sets whether the report contains the primary key as a column - for use with export history
        /// </summary>
        public bool PrimaryKeyInReport
        {
            get { return bPrimaryKeyInReport; }
            set { bPrimaryKeyInReport = value; }
        }

        /// <summary>
        /// Gets or Sets the position index of the primary key column in the report - for use with export history
        /// </summary>
        public int PrimaryKeyIndex
        {
            get { return nPrimaryKeyIndex; }
            set { nPrimaryKeyIndex = value; }
        }
        #endregion
    }
}