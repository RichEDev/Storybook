namespace SpendManagementLibrary.Receipts
{
    using System;

    /// <summary>
    /// Defines a <see cref="ProcessedReceipt"/> and all it members
    /// </summary>
    public class ProcessedReceipt
    {
        /// <summary>
        /// Id of the <see cref="ProcessedReceipt"/>
        /// </summary>
        public int ProcessedReceiptId { get; set; }

        /// <summary>
        /// Account id the <see cref="ProcessedReceipt"/> is from
        /// </summary>
        public int AccountId { get; set; }

        /// <summary>
        /// Wallet receipt id the <see cref="ProcessedReceipt"/> relates to
        /// </summary>
        public int WalletReceiptId { get; set; }

        /// <summary>
        /// Status of the <see cref="ProcessedReceipt"/>
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// Total of the <see cref="ProcessedReceipt"/>
        /// </summary>
        public decimal? Total { get; set; }

        /// <summary>
        /// Date of the <see cref="ProcessedReceipt"/>
        /// </summary>
        public DateTime? Date { get; set; }

        /// <summary>
        /// Merchant of the <see cref="ProcessedReceipt"/>
        /// </summary>
        public string Merchant { get; set; }

        /// <summary>
        /// Receipt data of the <see cref="ProcessedReceipt"/>
        /// </summary>
        public string ReceiptData { get; set; }
        
        /// <summary>
        /// File extension of the <see cref="ProcessedReceipt"/>
        /// </summary>
        public string FileExtension { get; set; }
    }
}
