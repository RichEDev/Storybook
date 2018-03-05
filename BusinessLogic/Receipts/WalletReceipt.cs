namespace BusinessLogic.Receipts
{
    using System;

    /// <summary>
    /// Defines a basic <see cref="WalletReceipt"/> and its members.
    /// </summary>
    [Serializable]
    public class WalletReceipt : IWalletReceipt
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WalletReceipt"/> class.
        /// </summary>
        /// <param name="id">The id of the <see cref="WalletReceipt"/></param>
        /// <param name="fileExtension">The file extension of the <see cref="WalletReceipt"/></param>
        /// <param name="receiptData">The receipt data for this <see cref="WalletReceipt"/></param>
        public WalletReceipt(int id, string fileExtension, string receiptData)
        {
            this.Id = id;
            this.FileExtension = fileExtension;
            this.ReceiptData = receiptData;
        }

        /// <summary>
        /// Gets or sets the id for this <see cref="WalletReceipt"/>.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the file extension for this <see cref="WalletReceipt"/>.
        /// </summary>
        public string FileExtension { get; set; }

        /// <summary>
        /// Gets or sets the data for this <see cref="WalletReceipt"/>.
        /// </summary>
        public string ReceiptData { get; set; }

        /// <summary>
        /// Gets or sets the status for this <see cref="WalletReceipt"/>.
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// Gets or sets the date for this <see cref="WalletReceipt"/>.
        /// </summary>
        public DateTime? Date { get; set; }

        /// <summary>
        /// Gets or sets the total for this <see cref="WalletReceipt"/>.
        /// </summary>
        public decimal? Total { get; set; }

        /// <summary>
        /// Gets or sets the merchant for this <see cref="WalletReceipt"/>.
        /// </summary>
        public string Merchant { get; set; }

        /// <summary>
        /// Gets or sets the created by for this <see cref="WalletReceipt"/>.
        /// </summary>
        public int CreatedBy { get; set; }

        /// <summary>
        /// Gets or sets the created on for this <see cref="WalletReceipt"/>.
        /// </summary>
        public DateTime CreatedOn { get; set; }
    }
}
