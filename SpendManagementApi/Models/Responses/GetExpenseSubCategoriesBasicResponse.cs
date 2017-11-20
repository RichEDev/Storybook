namespace SpendManagementApi.Models.Responses
{
    using System.Collections.Generic;

    using SpendManagementApi.Models.Common;
    using SpendManagementApi.Models.Types;

    /// <summary>
    /// The expense sub categories mileage basic response.
    /// </summary>
    public class ExpenseSubCategoriesMileageBasicResponse : ApiResponse
    {
        /// <summary>
        /// Gets or sets the list of <see cref="ExpenseSubCategoriesMileageBasic">ExpenseSubCategoriesMileageBasic</see>
        /// </summary>
        public List<ExpenseSubCategoriesMileageBasic> List { get; set; }
    }
}