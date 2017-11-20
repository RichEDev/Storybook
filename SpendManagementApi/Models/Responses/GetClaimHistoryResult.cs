namespace SpendManagementApi.Models.Responses
{
    using SpendManagementLibrary;
    using System.Collections.Generic;

    /// <summary>
    /// Represents get claim history result with claim history.
    /// </summary>
    public class GetClaimHistoryResult
    {
        /// <summary>
        /// Gets or sets the claim history.
        /// </summary>
        public List<cClaimHistory> ClaimHistory { get; set; }
    }
}