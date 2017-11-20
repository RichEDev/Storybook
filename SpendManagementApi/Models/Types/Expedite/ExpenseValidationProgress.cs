namespace SpendManagementApi.Models.Types.Expedite
{
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Defines the point in the validation process at which an expense item sits.
    /// </summary>
    public enum ExpenseValidationProgress
    {
        /// <summary>
        /// The account has not subscribed to the validation service.
        /// </summary>
        [Display(Name = "Validation service unavailable")]
        ValidationServiceDisabled = -20,

        /// <summary>
        /// Unable to validate because the option on the subcat is turned off.
        /// </summary>
        [Display(Name = "Validation disabled on expense template")]
        SubcatValidationDisabled = -10,

        /// <summary>
        /// Validation not required as there are is no Validation SignoffGroup for this user.
        /// </summary>
        [Display(Name = "No validation required - no validation signoff group stage")]
        StageNotInSignoffGroup = -9,

        /// <summary>
        /// Validation not required as there are no Criteria.
        /// </summary>
        [Display(Name = "No validation required - no criteria")]
        NotRequired = -6,

        /// <summary>
        /// Validation not required as there are no Criteria.
        /// </summary>
        [Display(Name = "No receipts to validate")]
        NoReceipts = -4,

        /// <summary>
        /// Validation required.
        /// </summary>
        [Display(Name = "Validation required")]
        Required = 0,

        /// <summary>
        /// Validation currently in progress.
        /// </summary>
        [Display(Name = "Validation in progress")]
        InProgress = 10,

        /// <summary>
        /// Validation currently waiting for claimant.
        /// </summary>
        [Display(Name = "Returned to claimant")]
        WaitingForClaimant = 15,

        /// <summary>
        /// Validation completed, with one or more validation checks failing.
        /// </summary>
        [Display(Name = "Validation Completed: Failed")]
        CompletedFailed = 20,

        /// <summary>
        /// Validation completed, with one or more validation checks in an unknown state.
        /// </summary>
        [Display(Name = "Validation Completed: Warning")]
        CompletedWarning = 21,

        /// <summary>
        /// Validation completed, with all validation checks passing.
        /// </summary>
        [Display(Name = "Validation Completed: Passed")]
        CompletedPassed = 22,

        /// <summary>
        /// Validation completed, with one or more validation checks failing, but subsequently invalidated.
        /// </summary>
        [Display(Name = "Validation Invalidated. Formerly 'Completed: Failed'")]
        InvalidatedFailed = 30,

        /// <summary>
        /// Validation completed, with one or more validation checks in an unknown state, but subsequently invalidated.
        /// </summary>
        [Display(Name = "Validation Invalidated. Formerly 'Completed: Warning'")]
        InvalidatedWarning = 31,

        /// <summary>
        /// Validation completed, with all validation checks passing, but subsequently invalidated.
        /// </summary>
        [Display(Name = "Validation Invalidated. Formerly 'Completed: Passed'")]
        InvalidatedPassed = 32
    }
}