using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SpendManagementLibrary;

namespace Spend_Management
{
    public class cAutomatedTransfers
    {
        private SortedList<int,List<cAutomatedTransfer>> lstTransfers;

        #region properties

        public SortedList<int, List<cAutomatedTransfer>> Transfers
        {
            get { return lstTransfers; }
        }
        #endregion properties

        public cAutomatedTransfers()
        {
            lstTransfers = new SortedList<int,List<cAutomatedTransfer>>();
            List<cAutomatedTransfer> lstTransfer;
            DBConnection transferData;
            foreach (cAccount acc in cAccounts.CachedAccounts.Values)
            {
                lstTransfer = new List<cAutomatedTransfer>();
                if (!acc.archived && acc.IsNHSCustomer)
                {
                    //string strSQL = "SELECT exporthistoryid, financialexportid FROM dbo.exporthistory WHERE (exportStatus = @exportStatus)";

                    string strSQL = "SELECT eh.exporthistoryid, eh.financialexportid, eh.exportStatus, fe.NHSTrustID FROM dbo.exporthistory AS eh INNER JOIN dbo.financial_exports AS fe ON fe.financialexportid = eh.financialexportid WHERE (eh.exportStatus = @exportStatus)";

                    transferData = new DBConnection(cAccounts.getConnectionString(acc.accountid));
                    transferData.sqlexecute.Parameters.AddWithValue("@exportStatus", (byte)FinancialExportStatus.AwaitingESRInboundTransfer);
                    int exportHistoryID = 0;
                    int financialExportID = 0;
                    AutomatedTransferType transferType = AutomatedTransferType.ESRInbound;
                    int trustID = 0;

                    using (System.Data.SqlClient.SqlDataReader reader = transferData.GetReader(strSQL))
                    {
                        while (reader.Read())
                        {
                            financialExportID = reader.GetInt32(reader.GetOrdinal("financialexportid"));
                            trustID = reader.GetInt32(reader.GetOrdinal("NHSTrustID"));
                            exportHistoryID = reader.GetInt32(reader.GetOrdinal("exporthistoryid"));
                            lstTransfer.Add(new cAutomatedTransfer(acc.accountid, trustID, transferType, financialExportID, exportHistoryID));
                        }
                        reader.Close();
                    }

                    transferData.sqlexecute.Parameters.Clear();

                    if (lstTransfer.Count > 0)
                    {
                        lstTransfers.Add(acc.accountid,lstTransfer);
                    }
                }
            }
        }
    }
}
