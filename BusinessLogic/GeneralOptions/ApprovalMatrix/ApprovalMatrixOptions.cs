namespace BusinessLogic.GeneralOptions.ApprovalMatrix
{
    /// <summary>
    /// The approval matrix options.
    /// </summary>
    public class ApprovalMatrixOptions : IApprovalMatrixOptions
    {
        /// <summary>
        /// Gets or sets the number of approvers to remember for claimant in approval matrix claim.
        /// </summary>
        public int NumberOfApproversToRememberForClaimantInApprovalMatrixClaim { get; set; }
    }
}
