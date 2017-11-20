namespace SpendManagementApi.Models.Responses.Expedite
{
    using System.Collections.Generic;

    /// <summary>
    /// The expense validation results.
    /// </summary>
    public class ExpenseValidationResults
    {
        /// <summary>
        /// Gets or sets a list of validation results for the validation areas.
        /// </summary>
        public List<ExpenseValidationArea> ValidationResults { get; set; }
    }
}