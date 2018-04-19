namespace BusinessLogic.GeneralOptions.Framework.Invoices
{
    /// <summary>
    /// Defines a <see cref="InvoicesOptions"/> and it's members
    /// </summary>
    public class InvoicesOptions : IInvoicesOptions
    {
        /// <summary>
        /// Gets or sets the keep invoice forecasts
        /// </summary>
        public bool KeepInvoiceForecasts { get; set; }

        /// <summary>
        /// Gets or set the PO number generate
        /// </summary>
        public bool PONumberGenerate { get; set; }

        /// <summary>
        /// Get or sets the PO number sequence
        /// </summary>
        public int PONumberSequence { get; set; }

        /// <summary>
        /// Gets or sets the PO number format
        /// </summary>
        public string PONumberFormat { get; set; }
    }
}
