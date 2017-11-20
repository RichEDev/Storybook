namespace SpendManagementLibrary.Enumerators
{
    /// <summary>
    /// The reasons that a claim may not be un-submitted..
    /// </summary>
    public enum TransactionType
    {
        /// <summary>
        /// Gets all transactions.
        /// </summary>
        All = 0,

        /// <summary>
        /// Get credited transaction
        /// </summary>
        Credit = 1,

        /// <summary>
        /// Get debited transaction
        /// </summary>
        Debit = 2

    }
}