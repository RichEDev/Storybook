namespace SpendManagementApi.Models.Responses.Expedite
{
    using System.Collections.Generic;

    using SpendManagementApi.Models.Types.Expedite;

    /// <summary>
    /// The expense validation area. i.e. Business, VAT, Fraud
    /// </summary>
    public class ExpenseValidationArea
    {
        /// <summary>
        /// Gets or sets whether the validation area is invalidated
        /// </summary>
        public bool Invalidated { get; set; }

        /// <summary>
        /// Gets or sets the header text.
        /// </summary>
        public string HeaderText { get; set; }

        /// <summary>
        /// Gets or sets the expense validation status for the area.
        /// </summary>
        public ExpenseValidationStatus ExpenseValidationStatus { get; set; }

        /// <summary>
        /// Gets or sets the validation criterion results for the area.
        /// </summary>
        public List<ValidationCriterion> ValidationCriterionResults { get; set; }
    }
}