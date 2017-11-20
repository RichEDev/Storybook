namespace SpendManagementLibrary.Flags
{
    /// <summary>
    /// The flag failure reason.
    /// </summary>
    public enum FlagFailureReason
    {
        /// <summary>
        /// No failure.
        /// </summary>
        None,

        /// <summary>
        /// Flag causing the failure has its action set to block.
        /// </summary>
        Blocked,

        /// <summary>
        /// The claimant must provide a justification.
        /// </summary>
        ClaimantJustificationRequired
    }
}
