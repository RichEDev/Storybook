namespace BusinessLogic.GeneralOptions.FinancialYear
{
    /// <summary>
    /// Defines a <see cref="IFinancialYearOptions"/> and it's members
    /// </summary>
    public interface IFinancialYearOptions
    {
        /// <summary>
        /// Gets or sets the financial year starts
        /// </summary>
        string FYStarts { get; set; }

        /// <summary>
        /// Gets or sets the financial year ends
        /// </summary>
        string FYEnds { get; set; }
    }
}
