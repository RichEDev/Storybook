namespace SpendManagementLibrary.Expedite
{
    using System;
    using Enumerators.Expedite;

    /// <summary>
    /// Represents a single unit of validation for an ExpenseItem (claim line / savedexpense).
    /// </summary>
    [Serializable]
    public class ExpenseValidationResult
    {
        /// <summary>
        /// The unique Id of this item in the database.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The Id of the Expense Item (claim line)
        /// that this result pertains to.
        /// </summary>
        public int ExpenseItemId { get; set; }

        /// <summary>
        /// The rule that this is the result for.
        /// </summary>
        public ExpenseValidationCriterion Criterion { get; set; }

        /// <summary>
        /// The Business Reasons status of this ValidationResult.
        /// </summary>
        public ExpenseValidationResultStatus BusinessStatus { get; set; }

        /// <summary>
        /// The VAT status of this ValidationResult.
        /// </summary>
        public ExpenseValidationResultStatus VATStatus { get; set; }

        /// <summary>
        /// Whether the receipt / expense being validated is possibly fraudulent.
        /// </summary>
        public bool PossiblyFraudulent { get; set; }

        /// <summary>
        /// The DateTime at which this validation was performed.
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Any comments that the Expedite operator wishes to offer as 
        /// extra reasons for why the validation status is the way it is.
        /// </summary>
        public string Comments { get; set; }
        
        /// <summary>
        /// Any other data, in XML format.
        /// </summary>
        public string Data { get; set; }

        /// <summary>
        /// The result of the expedite operator matching criteria to a receipt.
        /// Used to determine the actual status of this result.
        /// </summary>
        public ExpenseValidationMatchingResult MatchingResult { get; set; }
    }
}
