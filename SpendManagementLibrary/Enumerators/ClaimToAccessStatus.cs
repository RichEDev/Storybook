namespace SpendManagementLibrary.Enumerators
{
    /// <summary>
    /// Access status of claim whether it is accessible for requested user
    /// </summary>
    public enum ClaimToAccessStatus
    {
        /// <summary>
        /// The claim not found status.
        /// </summary>
        ClaimNotFound = 0,

        /// <summary>
        /// The insufficient access of the claim.
        /// </summary>
        InSufficientAccess = 1,

        /// <summary>
        /// Can access a claim successfully.
        /// </summary>
        Success = 2
    }
}
