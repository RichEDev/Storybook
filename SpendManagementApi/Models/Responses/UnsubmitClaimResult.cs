namespace SpendManagementApi.Models.Responses
{
    using SpendManagementApi.Common.Enum;
    
    /// <summary>
    /// Represents unsubmit claim result with unsubmit reason.
    /// </summary>
    public class UnsubmitClaimResult
    {
        /// <summary>
        /// Gets or sets the unsubmit reason.
        /// </summary>
        public ClaimUnsubmittableReason UnsubmitReason { get; set; }

        /// <summary>
        /// Gets or sets the unsubmit rejection reason.
        /// </summary>
        public string UnsubmitRejectionReason { get; set; }
    }
}