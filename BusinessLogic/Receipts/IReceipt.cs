namespace BusinessLogic.Receipts
{
    using System;

    using BusinessLogic.Interfaces;

    /// <summary>
    /// Defines a basic <see cref="IReceipt"/> and its members.
    /// </summary>
    public interface IReceipt : IIdentifier<int>
    {
        /// <summary>
        /// Gets or sets the File Extension for this <see cref="IReceipt"/>.
        /// </summary>
        string FileExtension { get; set; }

        /// <summary>
        /// Gets or sets the data for this <see cref="IReceipt"/>.
        /// </summary>
        string ReceiptData { get; set; }

        /// <summary>
        /// Gets or sets the status for this <see cref="IReceipt"/>.
        /// </summary>
        int Status { get; set; }

        /// <summary>
        /// Gets or sets the created by for this <see cref="IReceipt"/>.
        /// </summary>
        int CreatedBy { get; set; }

        /// <summary>
        /// Gets or sets the created on for this <see cref="IReceipt"/>.
        /// </summary>
        DateTime CreatedOn { get; set; }
    }
}
