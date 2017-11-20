namespace SpendManagementApi.Models.Requests
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Common;

    /// <summary>
    /// THe request for returning expense items
    /// </summary>
    public class ReturnExpenseItemsRequest : ApiRequest
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

        /// <summary>
        /// The reason for returning the items
        /// </summary>
        [Required]
        public string reason;

    }
}