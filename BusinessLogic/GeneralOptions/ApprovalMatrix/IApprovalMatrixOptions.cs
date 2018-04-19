namespace BusinessLogic.GeneralOptions.ApprovalMatrix
{
    /// <summary>
    /// Defines a <see cref="IApprovalMatrixOptions"/> and it's members
    /// </summary>
    public interface IApprovalMatrixOptions
    {
        /// <summary>
        /// Gets or sets the number of approvers to remember for claimant in approval matrix claim.
        /// </summary>
        int NumberOfApproversToRememberForClaimantInApprovalMatrixClaim { get; set; }
    }
}
