namespace SpendManagementLibrary.Expedite
{
    using System;

    /// <summary>
    /// Represents information about an expense item that needs validating.
    /// </summary>
    public class ValidatableExpenseInfo
    {
        /// <summary>
        /// The Id of the account that holds the expense.
        /// </summary>
        public int AccountId { get; set; }

        /// <summary>
        /// The Id of the claim that contains the expense.
        /// </summary>
        public int ClaimId { get; set; }

        /// <summary>
        /// The Id of the expense itself.
        /// </summary>
        public int ExpenseId { get; set; }

        /// <summary>
        /// The last modified date of the claim.
        /// </summary>
        public DateTime? ModifiedOn { get; set; }
    }
}
