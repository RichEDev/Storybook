namespace SpendManagementLibrary.Flags
{
    /// <summary>
    /// Whether a flag can be validated at any point or needs the expense saving to the database first.
    /// </summary>
    public enum ValidationType
    {
        /// <summary>
        /// Can be validated at any time.
        /// </summary>
        Any,

        /// <summary>
        /// Expense does not require saving before validation for flags.
        /// </summary>
        DoesNotRequireSave,

        /// <summary>
        /// Expense requires saving to database before validation for flags.
        /// </summary>
        RequiresSave
    }
}
