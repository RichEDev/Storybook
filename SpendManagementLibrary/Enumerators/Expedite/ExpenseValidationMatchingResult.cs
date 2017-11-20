namespace SpendManagementLibrary.Enumerators.Expedite
{
    /// <summary>
    /// The result of matching a receipt.
    /// </summary>
    public enum ExpenseValidationMatchingResult
    {
        /// <summary>
        /// Found and matched.
        /// </summary>
        FoundAndMatched = 1,

        /// <summary>
        /// Found and NOT matched.
        /// </summary>
        FoundNotMatched = 2,

        /// <summary>
        /// Found but unreadable.
        /// </summary>
        FoundUnreadable = 3,

        /// <summary>
        /// Not found.
        /// </summary>
        NotFound = 4
    }
}
