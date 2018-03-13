namespace SpendManagementApi.Models.Types
{
    using System;

    using BusinessLogic.Receipts;

    using SpendManagementApi.Interfaces;

    /// <summary>
    /// Defines a basic <see cref="WalletReceipt"/> and its members.
    /// </summary>
    public class WalletReceipt : BaseExternalType
    {
        /// <summary>
        /// Gets or sets the id for this <see cref="WalletReceipt"/>.
        /// </summary>
        public int WalletReceiptId { get; set; }

        /// <summary>
        /// Gets or sets the data for this <see cref="WalletReceipt"/>.
        /// </summary>
        public string ReceiptData { get; set; }

        /// <summary>
        /// Gets or sets the status for this <see cref="WalletReceipt"/>.
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// Gets or sets the total for this <see cref="WalletReceipt"/>.
        /// </summary>
        public decimal? Total { get; set; }

        /// <summary>
        /// Gets or sets the date for this <see cref="WalletReceipt"/>.
        /// </summary>
        public DateTime? Date { get; set; }

        /// <summary>
        /// Gets or sets the merchant for this <see cref="WalletReceipt"/>.
        /// </summary>
        public string Merchant { get; set; }

        /// <summary>
        /// Gets or sets the file extension for this <see cref="WalletReceipt"/>.
        /// </summary>
        public string Extension { get; set; }

        /// <summary>
        /// Gets or sets the created on for this <see cref="WalletReceipt"/>.
        /// </summary>
        public DateTime CreatedOn { get; set; }

        /// <summary>
        /// Gets or sets the created by for this <see cref="WalletReceipt"/>.
        /// </summary>
        public int CreatedById { get; set; }

        /// <summary>
        /// Converts from the DAL type to the API type.
        /// </summary>
        /// <returns>This, the API type.</returns>
        public WalletReceipt From(IWalletReceipt original, IActionContext actionContext)
        {
            this.WalletReceiptId = original.Id;
            this.Extension = original.FileExtension;
            this.ReceiptData = original.ReceiptData;
            this.Total = original.Total;
            this.Date = original.Date;
            this.Merchant = original.Merchant;
            this.Status = original.Status;
            this.CreatedById = original.CreatedBy;
            this.CreatedOn = original.CreatedOn;

            return this;
        }

        /// <summary>
        /// Converts from the API type to the DAL type.
        /// </summary>
        /// <returns>The DAL type.</returns>
        public IWalletReceipt To(IActionContext actionContext)
        {
            var receipt =
                new BusinessLogic.Receipts.WalletReceipt(this.WalletReceiptId, this.Extension, this.ReceiptData)
                {
                    Date = this.Date,
                    Total = this.Total,
                    Merchant = this.Merchant,
                    Status = this.Status,
                    CreatedBy = this.CreatedById,
                    CreatedOn = this.CreatedOn
                };

            return receipt;
        }
    }
}