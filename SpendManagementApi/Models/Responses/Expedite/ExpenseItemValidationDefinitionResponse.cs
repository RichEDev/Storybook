namespace SpendManagementApi.Models.Responses.Expedite
{
    using SpendManagementApi.Models.Common;

    /// <summary>
    /// The expense item validation definition response.
    /// </summary>
    public class ExpenseItemValidationResultsResponse : ApiResponse
    {
        /// <summary>
        /// Gets or sets the expense validation results.
        /// </summary>
        public ExpenseValidationResults Results { get; set; }
    }
}