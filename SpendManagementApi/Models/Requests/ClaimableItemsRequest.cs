namespace SpendManagementApi.Models.Requests
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Defines a request that update the subcategories and will appear when user adds a new expense.
    /// </summary>
    public class ClaimableItemsRequest
    {
        /// <summary>
        /// Gets or sets the list of sub category Ids.
        /// </summary>
        [Required]
        public List<int> SubCatIds { get; set; }
    }
}