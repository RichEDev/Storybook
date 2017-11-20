namespace SpendManagementLibrary.Claims
{
    /// <summary>
    /// The result of allocating a claim to a team member for approval 
    /// </summary>
    public class AllocateClaimResult
    {
        /// <summary>
        /// Gets or sets the Id of the claim.
        /// </summary>
        public int ClaimId { get; set; }

        /// <summary>
        /// Gets or sets the name of the claim.
        /// </summary>
        public string ClaimName { get; set; }

        /// <summary>
        /// Gets or sets the result of allocating a claim to a team member for approval.
        /// </summary>
        public bool Success { get; set; }
    }
}
