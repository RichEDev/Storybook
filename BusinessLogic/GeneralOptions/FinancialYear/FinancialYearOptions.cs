namespace BusinessLogic.GeneralOptions.FinancialYear
{
    /// <summary>
    /// Defines a <see cref="FinancialYearOptions"/> and it's members
    /// </summary>
    public class FinancialYearOptions : IFinancialYearOptions
    {
        /// <summary>
        /// Gets or sets the financial year starts
        /// </summary>
        public string FYStarts { get; set; }

        /// <summary>
        /// Gets or sets the financial year ends
        /// </summary>
        public string FYEnds { get; set; }
    }
}
