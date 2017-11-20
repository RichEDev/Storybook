
namespace SpendManagementLibrary.Expedite
{
    using System;
    using System.Activities.Expressions;
    using System.Collections.Generic;
    using System.Data;
    using System.IO;
    using SpendManagementLibrary;
    using SpendManagementLibrary.Helpers;
    using SpendManagementLibrary.Interfaces.Expedite;

    /// <summary>
    /// Class contains Payment service Data access methods
    /// </summary>
    public class PaymentService : IManagePayment
    {
      
        /// <summary>
        /// Declare private variables for parameters and stored procedures used in this page
        /// </summary>
        #region Private Constants / Column Names / Param Keys / SQL   
        private const string ParamExpeditePaymentProcessStatus= "@expeditePaymentProcessStatus";
        private const string ParamEmployeeId = "@EmployeeId";
        private const string ParamFinancialexportid = "@financialexportid";
        private const string ParamFund = "@fund";
        private const string ParamAvailableFund = "@AvailableFund";
        private const string ParamIsUpdated = "@isUpdated";
        private const string ParamFinanceExports = "@financeExports";
        private const string StoredProcGetFinanceExportForExpedite = "GetFinanceExportsforExpedite";
        private const string StoredProcGetPaymentHistory = "GetPaymentHistoryByEmployeeId";
        private const string StoredProcUpdatePaymentProcessStatus = "UpdateExpeditePaymentStatus";
        private const string StoredProcUpdateExpeditePaymentExecutedStatus = "UpdateExpeditePaymentExecutedStatus";
        private const string StoredProcGetClaimIdsForFinancialExport = "GetClaimIdsForFinancialExport";
        private string _accountConnectionString;
        private int _accountId;
        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new PaymentService, with the specified account id.
        /// </summary>
        /// <param name="accountId">A valid account id.</param>
        public PaymentService(int accountId)
        {
            AccountId = accountId;
        }

        public PaymentService()
        {
        }

        #endregion Constructor

        #region Property class
        /// <summary>
        /// AccountId Property . get the corresponding connection string
        /// </summary>
        public int AccountId
        {
            get { return _accountId; }
            set
            {
                if (value < 1)
                {
                    throw new ArgumentException();
                }
                _accountId = value;
                _accountConnectionString = cAccounts.getConnectionString(_accountId);
            }
        }
        #endregion

        #region public methods
        /// <summary>
        /// GetFinanceExportForExpedite get the finance export for the account selected . AccountId=0 retrieves data for all customers
        /// </summary>
        /// <param name="accountId">accountId</param>
        /// <param name="expeditePaymentProcessStatus">Payment process status ,0->not downloaded, 1->downloaded , 2->Executed</param>
        /// <returns>method returns list of Payment</returns>
        public List<Payment> GetFinanceExportForExpedite(int accountId , int expeditePaymentProcessStatus)
        {
            
            var returnPaymentList = new List<Payment>();
            List<cAccount> accountList = new List<cAccount>();
            cAccount account = new cAccount(); 
          
            if (accountId == null || accountId==0)
            {
                accountList = new cAccounts().GetAccountsWithPaymentServiceEnabled();
            }
            if (accountId > 0)
            {
                account = (new cAccounts().GetAccountsWithPaymentServiceEnabled()).Find(x => x.accountid == accountId);
                accountList.Add(account);
            }          
            Payment payment = new Payment();
            foreach (cAccount tmpAccount in accountList)
            {
                    Funds fundAmount= new Funds(tmpAccount.accountid);          
                var expdata = new DBConnection(cAccounts.getConnectionString(tmpAccount.accountid));
                expdata.sqlexecute.Parameters.Clear();
                expdata.sqlexecute.Parameters.AddWithValue(ParamExpeditePaymentProcessStatus, expeditePaymentProcessStatus);
                Fund fund=new Fund();
                fund= fundAmount.GetFundAvailable(tmpAccount.accountid);
                using (var reader = expdata.GetProcReader(StoredProcGetFinanceExportForExpedite))
                {
                    var financialExportList = new List<FinancialExport>();
                    int financialExportIdOrdial = reader.GetOrdinal("financialexportid");
                    int reportNameOrdial = reader.GetOrdinal("reportname");
                    int amountOrdial = reader.GetOrdinal("Amount");
                    while (reader.Read())
                    {
                        int financialExportId = reader.GetInt32(financialExportIdOrdial);
                        string reportName = reader.GetString(reportNameOrdial);
                        decimal amount = reader.GetDecimal(amountOrdial);
                        financialExportList.Add(new FinancialExport(financialExportId, reportName, expeditePaymentProcessStatus, amount));                        
                    }
                    payment = new Payment(tmpAccount.accountid, financialExportList, fund.AvailableFund);
                    returnPaymentList.Add(payment);
                    expdata.sqlexecute.Parameters.Clear();
                    reader.Close();
                }
            }
            return returnPaymentList;
        }

        /// <summary>
        /// GetAmountExportedByFinancialExport get the total amount exported by the financial export .
        /// Total amount that is approved and not yet marked as paid by the expedite
        /// </summary>
        /// <param name="accountId">accountId</param>
        /// <param name="financialExportId">financialExportId</param>
        /// <returns>Amount Payable by the export</returns>
        public decimal GetAmountExportedByFinancialExport(int accountId, int financialExportId)
        {
                var expdata = new DBConnection(cAccounts.getConnectionString(accountId));
                decimal amount = 0;
                expdata.sqlexecute.Parameters.Clear();
                expdata.sqlexecute.Parameters.AddWithValue("@financialexportid", financialExportId);
                using (var reader = expdata.GetProcReader("GetAmountExportedByFinancialExport"))
                {
                    int amountOrdial = reader.GetOrdinal("Amount");
                    while (reader.Read())
                    {
                         amount = reader.GetDecimal(amountOrdial);
                    }
                }
            
            return amount;
        }


        /// <summary>
        /// Get the Payment history of an employee.
        /// </summary>
        /// <param name="employeeId">Employee Id of logged in user</param>
        /// <returns>Data set with History of Payment made to customer</returns>
        public DataSet GetPaymentHistory(int employeeId)
        {
            DataSet datasetPaymentHistory = new DataSet();
            _accountConnectionString = cAccounts.getConnectionString(AccountId);
            using (var connection = new DatabaseConnection(_accountConnectionString))
            {
                connection.AddWithValue(ParamEmployeeId, employeeId);
                //fill the dataset with the result of GetPaymentHistoryByEmployeeId procedure
                datasetPaymentHistory = connection.GetProcDataSet(StoredProcGetPaymentHistory);

                if (datasetPaymentHistory == null)
                {
                    throw new InvalidDataException("Payment History not found.");
                }

                connection.ClearParameters();
            }
            return datasetPaymentHistory;
        }
        
    

        /// <summary>
        /// Update ExpeditePaymentProcessStatus as Downloaded for the list of financial exports      
        /// <param name="financialExport">List of executed financila exports</param>
        /// <param name="accountId">expedite Client Account id</param>
        /// <returns>Processed status</returns>
        /// </summary>
        
        public int UpdateFinanceExportProcessDownloadStatus(List<FinancialExport> financialExport, int accountId)
        {
            int isUpdated = 0;
            isUpdated = UpdateDownLoadStatus(financialExport, accountId);  
            return isUpdated;
        }
              
        /// <summary>
        /// UpdateFinanceExportProcessStatus update the Export Status of the Downloaded finance Exports
        /// </summary>
        /// <param name="financialExport">List of executed financila exports</param>
        /// <param name="accountId">expedite Client Account id</param>
        /// <param name="fundAmount">Fund Amount available for the customer</param>
        /// <returns>Processed status</returns>
        ///  </summary>
        public int UpdateFinanceExportProcessExecutedStatus(List<FinancialExport> financialExport, int accountId, decimal fundAmount)
        {
            int isUpdated = 0;
            isUpdated = UpdateExecutedStatus(financialExport, accountId, fundAmount);
            return isUpdated;
        }

        /// <summary>
        /// Get claim ids associated with each financial export
        /// </summary>   
        /// <param name="financialExportId">id of the export</param>
        /// <param name="accountId">accountid of the customer</param>
        /// <returns>List of claim ids associated with export</returns>
        public List<int> GetClaimByFinancialExportIds(int financialExportId, int accountId)
        {
            DataSet claims = new DataSet();
            List<int> claimIds = new List<int>();
            _accountConnectionString = cAccounts.getConnectionString(accountId);
            using (var connection = new DatabaseConnection(_accountConnectionString))
            {
                connection.AddWithValue(ParamFinancialexportid, financialExportId);
                claims = connection.GetProcDataSet(StoredProcGetClaimIdsForFinancialExport);
                connection.ClearParameters();
            }
            if (claims != null && claims.Tables[0] != null)
            {
                foreach (DataRow row in claims.Tables[0].Rows)
                {
                    claimIds.Add(int.Parse(row.ItemArray.GetValue(0).ToString()));
                }
            }
            return claimIds;
        }
        #endregion

        #region private methods
        /// <summary>
        /// UpdateExecutedStatus update the executed status of financial exports
        /// </summary>
        /// <param name="financialExport">financial Exports which are mareked as executed</param>
        /// <param name="accountId">accountid of the customer</param>
        /// <param name="fundAmount"> Fund Amount available for the account</param>
        /// <returns></returns>
        private int UpdateExecutedStatus(List<FinancialExport> financialExport, int accountId, decimal fundAmount)
        {
            int isUpdated = 0;
            decimal amountDebit = 0;
            try
            {
                DataTable inputTable = CreateFinanceExportDataTable();
                foreach (FinancialExport finExport in financialExport)
                {
                    inputTable = PopulateRowFromFinancialExport(inputTable, finExport);
                    amountDebit = amountDebit + Convert.ToDecimal(finExport.Amount);
                }
                _accountConnectionString = cAccounts.getConnectionString(accountId);
                using (var connection = new DatabaseConnection(_accountConnectionString))
                {
                    connection.AddWithValue(ParamFinanceExports, inputTable);
                    connection.AddWithValue(ParamFund, amountDebit);
                    connection.AddWithValue(ParamAvailableFund, (Convert.ToDouble(fundAmount) - Convert.ToDouble(amountDebit)));
                    connection.AddReturn(ParamIsUpdated);
                    connection.ExecuteProc(StoredProcUpdateExpeditePaymentExecutedStatus);
                    isUpdated = connection.GetReturnValue<int>(ParamIsUpdated);
                    connection.ClearParameters();
                }
            }
            catch
            {
                isUpdated = 0;
            }
            return isUpdated;
        }
        
        /// <summary>
        /// Creates a data table for the User Defined Table Type "EnvelopeBatch".
        /// </summary>      
        /// <returns>A new DataTable instance.</returns>
        private static DataTable CreateFinanceExportDataTable()
        {
            var financeExport = new DataTable("Int_TinyInt_Int");
            financeExport.Columns.Add("c1", typeof(int));
            financeExport.Columns.Add("c2", typeof(int));
            financeExport.Columns.Add("c3", typeof(int));
            return financeExport;
        }
        
        /// <summary>
        /// PopulateRowFromFinancialExport create rows for financial export input table
        /// </summary>
        /// <param name="inputTable"></param>
        /// <param name="financialExport"></param>
        /// <returns>Datatable with financial export ids and status</returns>
        private DataTable PopulateRowFromFinancialExport(DataTable inputTable, FinancialExport financialExport)
        {           
            var row = new List<object>
            {
                financialExport.Id,
                financialExport.ExpeditePaymentStatus,
                financialExport.ExportHistoryId,
            };          
            inputTable.Rows.Add(row.ToArray());
            return inputTable;
        }

        /// <summary>
        /// UpdateDownLoadStatus Update the downloaded status of the financial export
        /// </summary>
        /// <param name="financialExport"></param>
        /// <param name="accountId"></param>
        /// <returns>processes status</returns>
        private int UpdateDownLoadStatus(List<FinancialExport> financialExport, int accountId)
        {
            int isUpdated = 0;
            try
            {
                if (financialExport.Count > 0)
                {
                    DataTable inputTable = CreateFinanceExportDataTable();
                    foreach (FinancialExport finExport in financialExport)
                    {
                        inputTable = PopulateRowFromFinancialExport(inputTable, finExport);
                    }
                    _accountConnectionString = cAccounts.getConnectionString(accountId);
                    using (var connection = new DatabaseConnection(_accountConnectionString))
                    {
                        connection.AddWithValue(ParamFinanceExports, inputTable);
                        connection.AddReturn(ParamIsUpdated);
                        connection.ExecuteProc(StoredProcUpdatePaymentProcessStatus);
                        isUpdated = connection.GetReturnValue<int>(ParamIsUpdated);
                        connection.ClearParameters();
                    }
                }
            }
            catch(Exception ex)
            {
                cEventlog.LogEntry("Failed to update status\n\n" + ex.Message);
            }
            return isUpdated;
        }
        
        #endregion
    }
}
  
