namespace SpendManagementApi.Models.Responses
{
    using SpendManagementApi.Common.Enums;
    using SpendManagementApi.Models.Types; 

    /// <summary>
    /// Represent claim submission result with submit rejection reason.
    /// </summary>
    public class ClaimSubmissionResult
    {
       /// <summary>
        /// Gets or sets the claim id.
        /// </summary>
        public int ClaimId { get; set; }

        /// <summary>
        /// Gets or sets the status of the claim.
        /// </summary>
        public ClaimStatus ClaimStatus { get; set; }

        /// <summary>
        /// Gets or sets the claim submission result with data whether it can be submitted or not.
        /// </summary>
        public SubmitClaimResult SubmitClaimResult { get; set; }

        /// <summary>
        /// Gets or sets the claim submit rejection reason.
        /// </summary>
        public string ClaimSubmitRejectionReason { get; set; }
    }
}