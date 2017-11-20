namespace SpendManagementApi.Models.Responses
{
    using System.Collections.Generic;

    using SpendManagementApi.Models.Common;
    using SpendManagementApi.Models.Types;

    /// <summary>
    /// The claim expense overview response.
    /// </summary>
    public class ClaimExpenseOverviewResponse : ApiResponse
    {
        /// <summary>
        /// Gets or sets the list of <see cref="ClaimExpenseOverview">ClaimExpenseOverview</see>
        /// </summary>
        public List<ClaimExpenseOverview> List { get; set; }
      
        
    }
}