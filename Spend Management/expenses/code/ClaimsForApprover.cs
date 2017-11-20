namespace Spend_Management.expenses.code
{
    /// <summary>
    /// The claims for approver along with the claimant id
    /// </summary>
    public class ClaimsForApprover
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ClaimsForApprover"/> class.
        /// </summary>
        /// <param name="approverId">
        /// The approver id.
        /// </param>
        /// <param name="claimId">
        /// The claim id.
        /// </param>
        /// <param name="claimantId">
        /// The claimant id.
        /// </param>
        public ClaimsForApprover(int approverId, int claimId, int claimantId)
        {
            this.ApproverId = approverId;
            this.ClaimId = claimId;
            this.ClaimantId = claimantId;
        }

        /// <summary>
        /// Gets or sets the approver id.
        /// </summary>
        public int ApproverId { get; set; }

        /// <summary>
        /// Gets or sets the claimant id.
        /// </summary>
        public int ClaimantId { get; set; }

        /// <summary>
        /// Gets or sets the claim id.
        /// </summary>
        public int ClaimId { get; set; }

    }
}