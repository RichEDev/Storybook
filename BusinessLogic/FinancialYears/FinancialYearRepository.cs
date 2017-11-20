namespace BusinessLogic.FinancialYears
{
    using System;
    /// <summary>
    /// An implementation of the <see cref="FinancialYearRepository"/> class
    /// </summary>
    public abstract class FinancialYearRepository
    {
        /// <summary>
        /// Get the financial year [2] [0] = Start date [1] = End Date
        /// </summary>
        /// <returns></returns>
        public abstract DateTime[] GetFinancialYear();
    }
}
