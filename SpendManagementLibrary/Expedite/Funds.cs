namespace SpendManagementLibrary.Expedite
{
   using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data;
    using System.IO;
    using System.Linq;
    using Enumerators.Expedite;
    using Helpers;
    using Interfaces.Expedite;
    public class Funds: IManageFunds
    {

        #region Private Constants / Column Names / Param Keys / SQL

        private string _accountConnectionString;
        private int _accountId;
     


        private const string ParamAccountId = "@accountId";
        private const string ParamTopupAmount = "@transAmount";
        private const string ParamTransType = "@transType";
        private const string ParamTotalFund = "@TotalFund";

        private const string StoredProcGetFundAvailable = "GetFundAvailable";
        private const string StoredProcGetFundLimit = "GetFundLimit";
        private const string StoredProcUpdateFundLimit = "UpdateFundLimit";
        private const string StoredProcAddFundTransaction = "AddFundTransaction";
        private const string StoredProcGetCustomerFundDetailsForExpediteEmail = "GetCustomerFundDetailsForExpediteEmail";
        #endregion


         #region Constructor

        /// <summary>
        /// Creates a new ExpenseValidationManager, with the specified account id.
        /// </summary>
        /// <param name="accountId">A valid account id.</param>
        public Funds(int accountId)
        {
            AccountId = accountId;

        }

        #endregion Constructor

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

        /// <summary>
        /// GetFundAvailable get the fund available for the selected expedite client
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns></returns>
        public Fund GetFundAvailable(int accountId)
        {
            // pass the accountIdS as argument.
            Fund fundAvailable = new Fund();            
            DataSet dsfund = RunSingleStoredProcedure(StoredProcGetFundAvailable, accountId);
            if (dsfund.Tables.Count > 0 && dsfund.Tables[0].Rows.Count > 0)
            {
                DataTable dtFund = new DataTable();
                dtFund = dsfund.Tables[0];
                fundAvailable.AvailableFund = (decimal)dtFund.Rows[0]["AvailableFund"];
            }
            return fundAvailable;
        }

        /// <summary>
        ///  Gets the fund limit for the selected expedite client
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns>Fund limit</returns>
        public Fund GetFundLimit(int accountId)
        {
            DataSet fund = new DataSet();
            Fund fundLimit = new Fund();
            DatabaseConnection connectDatabase = new DatabaseConnection(GlobalVariables.MetabaseConnectionString);
            connectDatabase.sqlexecute.Parameters.AddWithValue("@accountId",accountId);
             fund = connectDatabase.GetProcDataSet(StoredProcGetFundLimit);
            if (fund.Tables.Count > 0 && fund.Tables[0].Rows.Count > 0)
            {
                DataTable fundLimitData = new DataTable();
                fundLimitData = fund.Tables[0];
                var fundvalue= Convert.ToString(fundLimitData.Rows[0]["FundLimit"]);
                if (string.IsNullOrEmpty(fundvalue))
                {
                    fundLimit.FundLimit = 0;
                    return fundLimit;
                }
                fundLimit.FundLimit = Convert.ToDecimal(fundvalue);
            }
            return fundLimit;
        }

        /// <summary>
        /// UpdateFundLimit updates the fund limit for the selected expedite client
        /// </summary>
        /// <param name="accountId">Accountid</param>
        /// <param name="amount">Amount to update</param>
        /// <returns>Fund limit</returns>
        public Fund UpdateFundLimit(int accountId,decimal amount)
        {
            DataSet fund = new DataSet();
            Fund fundLimit = new Fund();
            DatabaseConnection connectDatabase = new DatabaseConnection(GlobalVariables.MetabaseConnectionString);
            connectDatabase.sqlexecute.Parameters.AddWithValue("@accountId", accountId);
            connectDatabase.sqlexecute.Parameters.AddWithValue("@amount", amount);
            fund = connectDatabase.GetProcDataSet(StoredProcUpdateFundLimit);
            if (fund.Tables.Count > 0 && fund.Tables[0].Rows.Count > 0)
            {
                DataTable fundLimitData = new DataTable();
                fundLimitData = fund.Tables[0];
                fundLimit.FundLimit = (decimal)fundLimitData.Rows[0]["FundLimit"];
            }
            return fundLimit;
        }

        /// <summary>
        /// Method call the db access layer method to execute the procedure and returns data set
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="addWithValueArgs"></param>
        /// <param name="connectToMetabase"></param>
        /// <returns></returns>
        private DataSet RunSingleStoredProcedure(string sql,int accountId, bool connectToMetabase = false)
        {
            const string returnValueKey = "returnvalue";
            DataSet  result = new DataSet();
            _accountConnectionString = cAccounts.getConnectionString(accountId);
            using (var connection = new DatabaseConnection(connectToMetabase ? GlobalVariables.MetabaseConnectionString : _accountConnectionString))
            {              
                connection.AddReturn(returnValueKey, SqlDbType.Int);
                result = connection.GetProcDataSet(sql);
                if (result == null)
                {
                    throw new InvalidDataException("Item not found.");
                }

                connection.ClearParameters();
            }

            return result;
        }


       
        /// <summary>
        /// Adds a receipt to the account you initialised this class with.
        /// Linkages will be ignored. Please use the link methods.
        /// </summary>
        /// <param name="receipt">The receipt to add.</param>
        /// <param name="data">The actual binary data for the receipt.</param>
        /// <returns>The added receipt.</returns>
        public Fund AddFundTransaction(Fund fund)
        {   
            var args = new List<KeyValuePair<string, object>>
            {  
                new KeyValuePair<string, object>(ParamTopupAmount, fund.FundTopup),
                new KeyValuePair<string, object>(ParamTransType, fund.TransType),
                 new KeyValuePair<string, object>(ParamTotalFund, fund.AvailableFund) 
            };
            const string returnValueKey = "returnvalue";
            _accountConnectionString = cAccounts.getConnectionString(fund.AccountId);
            using (var connection = new DatabaseConnection(_accountConnectionString))
            {
                if (args != null)
                {
                    args.ForEach(x => connection.AddWithValue(x.Key, x.Value));
                }
                connection.AddReturn(returnValueKey, SqlDbType.Int);
                connection.ExecuteProc(StoredProcAddFundTransaction);
                fund.Id = connection.GetReturnValue<int>(returnValueKey);

                if (fund.Id < 0)
                {
                    throw new InvalidDataException("Item not saved.");
                }
                connection.ClearParameters();
            }          
            
            return fund; 
        }


        /// <summary>
        /// Gets the fund related details of an account, which is cusomized for sending emails. 
        /// </summary>
        /// <returns>list of customer details</returns>
        public static List<CustomerEmailDetails> GetFundDetailsForExpediteEmail()
        {

            List<CustomerEmailDetails> customersDetails;
            var expediteCustomers = new cAccounts().GetAccountsWithPaymentServiceEnabled();
            if (expediteCustomers != null && expediteCustomers.Count != 0)
            {
                customersDetails = new List<CustomerEmailDetails>();
                foreach (cAccount customer in expediteCustomers)
                {
                    DatabaseConnection connectDatabase = new DatabaseConnection(GlobalVariables.MetabaseConnectionString);
                    connectDatabase.sqlexecute.Parameters.AddWithValue("@accountId", customer.accountid);
                    var drCustomerFundInfo = connectDatabase.GetProcDataSet(StoredProcGetCustomerFundDetailsForExpediteEmail);
                    if (drCustomerFundInfo != null && drCustomerFundInfo.Tables.Count > 0 && drCustomerFundInfo.Tables[0].Rows.Count>0)
                    {
                        customersDetails.Add(CustomerEmailDetails.LoadFromDataRow(drCustomerFundInfo.Tables[0].Rows[0]));
                    }
                }
                return customersDetails;
            }
            else
            {
                return null;
            }

        }
    }
}
