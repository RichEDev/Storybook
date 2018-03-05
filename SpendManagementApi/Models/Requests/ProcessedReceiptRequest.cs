namespace SpendManagementApi.Models.Requests
{
    using System;

    using SpendManagementApi.Models.Types;

    using receipt = SpendManagementLibrary.Receipts.ProcessedReceipt;

    /// <summary>
    /// Defines a <see cref="ProcessedReceiptRequest"/> and all its members
    /// </summary>
    public class ProcessedReceiptRequest
    {
        /// <summary>
        /// Id of the <see cref="ProcessedReceiptRequest"/>
        /// </summary>
        public int ProcessedReceiptId { get; set; }

        /// <summary>
        /// The account Id the <see cref="WalletReceipt"/> belongs too
        /// </summary>
        public int AccountId { get; set; }

        /// <summary>
        /// The Id of the <see cref="WalletReceipt"/>
        /// </summary>
        public int WalletReceiptId { get; set; }

        /// <summary>
        /// The total found on the <see cref="WalletReceipt"/>
        /// </summary>
        public decimal? Total { get; set; }

        /// <summary>
        /// The date found on the <see cref="WalletReceipt"/>
        /// </summary>
        public DateTime? Date { get; set; }

        /// <summary>
        /// The merchant found on the <see cref="WalletReceipt"/>
        /// </summary>
        public string Merchant { get; set; }

        /// <summary>
        /// Converts from the API type to the DAL type.
        /// </summary>
        /// <returns>The DAL type.</returns>
        public receipt To(ProcessedReceiptRequest actionContext)
        {
            var receipt =
                new receipt()
                {
                    Date = this.Date,
                    Total = this.Total,
                    ProcessedReceiptId = this.ProcessedReceiptId,
                    WalletReceiptId = this.WalletReceiptId,
                    Merchant = this.Merchant,
                    AccountId = this.AccountId
                };

            return receipt;
        }
    }
}