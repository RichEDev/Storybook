namespace SpendManagementApi.Models.Requests
{
    using Common;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Handles the CheckAndPayActionRequest (approve, unapprove) request
    /// </summary>
    public class CheckAndPayActionRequest : ApiRequest
    {
        /// <summary>
        /// The claimId
        /// </summary>
        [Required]
        public int claimId;

        /// <summary>
        /// The list of expense ids
        /// </summary>
        [Required]
        public List<int> expenseIds;
    }
}