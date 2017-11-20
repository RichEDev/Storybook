namespace SpendManagementLibrary.Expedite
{
    using System.Collections.Generic;
    /// <summary>
    /// Represents an approved  expenses item  which is ready for physical payment
    /// </summary>
    public class Payment
    {
        /// <summary>
        ///Initialises a new instance of the <see cref="Payment"/> class
        /// </summary>
        public Payment()
        {
        }
        /// <summary>
        ///Initialises a new instance of the <see cref="Payment"/> class accepting the accountid and finance export class
        /// </summary>
        /// <param name="accountId">AccountId  of the customer</param>
        /// <param name="financialExport">list of financial export that belongs to customer</param>
        ///  <param name="fundAmount">Available Fund for the customer </param>
        public Payment(int accountId, IList<FinancialExport> financialExport, decimal fundAmount)
        {
            this.AccountId = accountId;
            this.FinancialExport = financialExport;
            this.FundAmount = fundAmount;
        }
        /// <summary>
        ///Initialises a new instance of the <see cref="Payment"/> class accepting the accountid and finance export class
        /// </summary>
        /// <param name="accountId">AccountId  of the customer</param>
        /// <param name="financialExport">list of financial export that belongs to customer</param>
        public Payment(int accountId, IList<FinancialExport> financialExport)
        {
            this.AccountId = accountId;
            this.FinancialExport = financialExport;
        }
        /// <summary>
        /// The AccountId of the account that 
        /// this FinanceExport belongs to
        /// </summary>
        public int AccountId { get; set; }
        /// <summary>
        /// The list of Finance Export for Particular Account
        /// </summary>
        public IList<FinancialExport> FinancialExport { get; set; }
        /// <summary>
        /// The account has Payment Service enabled
        /// </summary>
        public bool PaymentServiceEnabled { get; set; }
        /// <summary>
        /// Fund Amount available for Expedite client
        /// </summary>
        public decimal FundAmount { get; set; }
    }    
}