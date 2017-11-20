namespace SpendManagementLibrary.Flags
{
    /// <summary>
    /// The flag period type.
    /// </summary>
    public enum FlagPeriodType
    {
        /// <summary>
        /// X number of days back from today.
        /// </summary>
        Days = 1,

        /// <summary>
        /// X number of weeks back from today.
        /// </summary>
        Weeks,

        /// <summary>
        /// X number of months back from today.
        /// </summary>
        Months,

        /// <summary>
        /// X number of years back from today.
        /// </summary>
        Years,

        /// <summary>
        /// Looks at a full calendar week(s)
        /// </summary>
        CalendarWeeks,

        /// <summary>
        /// Looks at a full calendar month(s).
        /// </summary>
        CalendarMonths,

        /// <summary>
        /// Looks at a full calendar year(s).
        /// </summary>
        CalendarYears,

        /// <summary>
        /// Looks at a full financial year(s)
        /// </summary>
        FinancialYears
    }
}
