namespace BusinessLogic.GeneralOptions.Framework.Invoices
{
    /// <summary>
    /// Defines a <see cref="IInvoicesOptions"/> and it's members
    /// </summary>
    public interface IInvoicesOptions
    {
        /// <summary>
        /// Gets or sets the keep invoice forecasts
        /// </summary>
        bool KeepInvoiceForecasts { get; set; }

        /// <summary>
        /// Gets or set the PO number generate
        /// </summary>
        bool PONumberGenerate { get; set; }

        /// <summary>
        /// Get or sets the PO number sequence
        /// </summary>
        int PONumberSequence { get; set; }

        /// <summary>
        /// Gets or sets the PO number format
        /// </summary>
        string PONumberFormat { get; set; }
    }
}
