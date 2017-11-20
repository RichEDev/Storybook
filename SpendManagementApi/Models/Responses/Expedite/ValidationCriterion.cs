namespace SpendManagementApi.Models.Responses.Expedite
{
    using SpendManagementApi.Models.Types.Expedite;

    /// <summary>
    /// The validation criterion.
    /// </summary>
    public class ValidationCriterion
    {
        /// <summary>
        /// Gets or sets the criterion text.
        /// </summary>
        public string Criterion { get; set; }

        /// <summary>
        /// Gets or sets any validator's comment.
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// Gets or sets the expense validation status for the criterion.
        /// </summary>
        public ExpenseValidationStatus ExpenseValidationStatus { get; set; }
    }
}