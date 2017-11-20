using System;

namespace SpendManagementLibrary.Claims
{
    /// <summary>
    /// claim details of an apporver to remind him about the claim details
    /// </summary>
    public class ApproverClaim
    {
        /// <summary>
        /// approver id of the claim
        /// </summary>
        public int ApproverId { get; }

        /// <summary>
        /// Claim Id of the claim
        /// </summary>
        public int ClaimId { get; }

        /// <summary>
        /// Stage which the claim is 
        /// </summary>
        public int Stage { get; }

        /// <summary>
        /// claimant name
        /// </summary>
        public string Claimant { get; }

        /// <summary>
        /// name of the claim
        /// </summary>
        public string ClaimName { get; }

        /// <summary>
        /// total of expense items in the claim
        /// </summary>
        public decimal Total { get; }

        /// <summary>
        /// the date when claim was submitted
        /// </summary>
        public DateTime ClaimSubmittedOn { get; }

        /// <summary>
        /// currency symbol for claim total
        /// </summary>
        public string CurrencySymbol { get; }

        /// <summary>
        /// </summary>
        /// <param name="approverId">approver id of the claim</param>
        /// <param name="claimId">Id of claim</param>
        /// <param name="stage">stage of claim</param>
        /// <param name="claimant">name of claimant</param>
        /// <param name="claimName">name of claim</param>
        /// <param name="total">claim total</param>
        /// <param name="claimSubmittedOn">calim submittal date</param>
        /// <param name="currencySymbol">curency symbol for the claim total</param>
        public ApproverClaim(int approverId, int claimId, int stage, string claimant, string claimName, decimal total, DateTime claimSubmittedOn, string currencySymbol)
        {
            this.ApproverId = approverId;
            this.ClaimId = claimId;
            this.Stage = stage;
            this.Claimant = claimant;
            this.ClaimName = claimName;
            this.ClaimSubmittedOn = claimSubmittedOn;
            this.Total = total;
            this.CurrencySymbol = currencySymbol;
        }
    }
}
