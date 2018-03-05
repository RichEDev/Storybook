namespace BusinessLogic.Receipts
{
    using System;

    using BusinessLogic.Interfaces;

    /// <summary>
    /// Defines a basic <see cref="IProcessedReceipt"/> and its members.
    /// </summary>
    public interface IProcessedReceipt : IIdentifier<int>
    {
        /// <summary>
        /// Gets or sets the date for this <see cref="IProcessedReceipt"/>.
        /// </summary>
        DateTime? Date { get; set; }

        /// <summary>
        /// Gets or sets the total for this <see cref="IProcessedReceipt"/>.
        /// </summary>
        decimal? Total { get; set; }

        /// <summary>
        /// Gets or sets the merchant for this <see cref="IProcessedReceipt"/>.
        /// </summary>
        string Merchant { get; set; }
    }
}
