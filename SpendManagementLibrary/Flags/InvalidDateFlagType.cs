namespace SpendManagementLibrary.Flags
{
    /// <summary>
    /// The invalid date flag type.
    /// </summary>
    public enum InvalidDateFlagType
    {
        /// <summary>
        /// A set date that any item will be flagged before.
        /// </summary>
        SetDate = 1,

        /// <summary>
        /// The last x months.
        /// </summary>
        LastXMonths
    }
}
