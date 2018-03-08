namespace SpendManagementApi.Models.Types
{
    using System;

    /// <summary>
    /// Defines a <see cref="ProcessedReceipt"/> and it's members
    /// </summary>
    public class ProcessedReceipt : BaseExternalType
    {
        /// <summary>
        /// The id of the <see cref="ProcessedReceipt"/>
        /// </summary>
        public int ProcessedReceiptId { get; set; }

        /// <summary>
        /// The account id the <see cref="ProcessedReceipt"/> belongs to
        /// </summary>
        public int AccountId { get; set; }

        /// <summary>
        /// The id of the <see cref="WalletReceipt"/> the <see cref="ProcessedReceipt"/> relates to
        /// </summary>
        public int WalletReceiptId { get; set; }

        /// <summary>
        /// The status fo the <see cref="WalletReceipt"/>
        /// </summary>
        public int Status { get; set; }

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
        /// The receipt data of the <see cref="WalletReceipt"/>
        /// </summary>
        public string ReceiptData { get; set; }

        /// <summary>
        /// The file extension of the <see cref="WalletReceipt"/>
        /// </summary>
        public string FileExtension { get; set; }

        /// <summary>
        /// Converts from the DAL type to the API type.
        /// </summary>
        /// <returns>This, the API type.</returns>
        public ProcessedReceipt From(SpendManagementLibrary.Receipts.ProcessedReceipt original)
        {
            this.AccountId = original.AccountId;
            this.WalletReceiptId = original.WalletReceiptId;
            this.FileExtension = original.FileExtension;
            this.ReceiptData = original.ReceiptData;
            this.Total = original.Total;
            this.Date = original.Date;
            this.Status = original.Status;

            return this;
        }

        /// <summary>
        /// Converts from the API type to the DAL type.
        /// </summary>
        /// <returns>The DAL type.</returns>
        public SpendManagementLibrary.Receipts.ProcessedReceipt To(ProcessedReceipt actionContext)
        {
            var receipt =
                new SpendManagementLibrary.Receipts.ProcessedReceipt()
                {
                    Date = this.Date,
                    Total = this.Total,
                    Status = this.Status,
                    WalletReceiptId = this.WalletReceiptId,
                    FileExtension = this.FileExtension,
                    ReceiptData = this.ReceiptData
                };

            return receipt;
        }
    }
}
