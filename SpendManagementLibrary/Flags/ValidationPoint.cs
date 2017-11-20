namespace SpendManagementLibrary.Flags
{
    /// <summary>
    /// When items are being checked for flags where is it happening.
    /// </summary>
    public enum ValidationPoint
    {
        /// <summary>
        /// On the add expense page.
        /// </summary>
        AddExpense,

        /// <summary>
        /// When the claimant is submitting the claim.
        /// </summary>
        SubmitClaim
    }
}
