using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpendManagementLibrary
{
    [Serializable()]
    public class cAutomatedTransfer
    {
        private int nAccountID;
        private int nTrustID;
        private AutomatedTransferType eTransferType;
        private int nFinancialExportID;
        private int nExportHistoryID;

        #region properties
        public int accountID
        {
            get { return nAccountID; }
            set { nAccountID = value; }
        }
        public int trustID
        {
            get { return nTrustID; }
            set { nTrustID = value; }
        }
        public AutomatedTransferType TransferType
        {
            get { return eTransferType; }
            set { eTransferType = value; }
        }
        public int financialExportID
        {
            get { return nFinancialExportID; }
            set { nFinancialExportID = value; }
        }
        public int exportHistoryID
        {
            get { return nExportHistoryID; }
            set { nExportHistoryID = value; }
        }
        #endregion properties

        public cAutomatedTransfer(int accountid, int trustid, AutomatedTransferType transfertype, int financialexportid, int exporthistoryid)
        {
            nAccountID = accountid;
            nTrustID = trustid;
            eTransferType = transfertype;
            nFinancialExportID = financialexportid;
            nExportHistoryID = exporthistoryid;
        }

        public cAutomatedTransfer()
        {

        }
    }

    [Serializable()]
    public enum AutomatedTransferType
    {
        None = 0,
        ESRInbound = 1,
        ESROutbound
    }
}
