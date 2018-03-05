namespace PublicAPI.DTO
{
    using System;

    /// <summary>
    /// Defines a <see cref="WalletReceiptDto"/> and it's members
    /// </summary>
    public class WalletReceiptDto
    {

        /// <summary>
        /// Gets or sets the id for this <see cref="WalletReceiptDto"/>.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the file extension for this <see cref="WalletReceiptDto"/>.
        /// </summary>
        public string FileExtension { get; set; }

        /// <summary>
        /// Gets or sets the data for this <see cref="WalletReceiptDto"/>.
        /// </summary>
        public string ReceiptData { get; set; }

        /// <summary>
        /// Gets or sets the status for this <see cref="WalletReceiptDto"/>.
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// Gets or sets the date for this <see cref="WalletReceiptDto"/>.
        /// </summary>
        public DateTime? Date { get; set; }

        /// <summary>
        /// Gets or sets the total for this <see cref="WalletReceiptDto"/>.
        /// </summary>
        public decimal? Total { get; set; }

        /// <summary>
        /// Gets or sets the merchant for this <see cref="WalletReceiptDto"/>.
        /// </summary>
        public string Merchant { get; set; }

        /// <summary>
        /// Gets or sets the created by for this <see cref="WalletReceiptDto"/>.
        /// </summary>
        public int CreatedBy { get; set; }

        /// <summary>
        /// Gets or sets the created on for this <see cref="WalletReceiptDto"/>.
        /// </summary>
        public DateTime CreatedOn { get; set; }
    }
}