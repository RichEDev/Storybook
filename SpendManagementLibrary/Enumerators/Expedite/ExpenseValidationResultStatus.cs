namespace SpendManagementLibrary.Enumerators.Expedite
{
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// The status of validating an Expense Item or claim line.
    /// </summary>
    public enum ExpenseValidationResultStatus
    {
        /// <summary>
        /// Validation Failed
        /// </summary>
        [Display(Name = "Validation Failed")]
        Fail = -1,

        /// <summary>
        /// Unable to Validate
        /// </summary>
        [Display(Name = "Not Applicable")]
        NotApplicable = 0,

        /// <summary>
        /// Validation Passed
        /// </summary>
        [Display(Name = "Validation Passed")]
        Pass = 1
    }
}
