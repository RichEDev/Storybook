using SpendManagementApi.Common.Enums;

namespace SpendManagementApi.Models.Types
{
    /// <summary>
    /// The result of Saving or Editing a Claim
    /// </summary>
    public class SaveUpdateClaimResult : BaseExternalType
    {
        /// <summary>
        /// The id of the claim.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The outcome of the action
        /// </summary>
        public SaveEditClaimOutcome SaveEditClaimOutcome { get; set; }
    }
}