namespace SpendManagementLibrary.Enumerators
{
    /// <summary>
    /// Represents the permission results for an expense item
    /// </summary>
    public enum ExpenseItemPermissionResult
    {
        /// <summary>
        /// Expense item has passed validation
        /// </summary>
        Pass = 0,

        /// <summary>
        /// The employee does not own the claim
        /// </summary>
        EmployeeDoesNotOwnClaim = 1,

        /// <summary>
        /// The expense item belongs to an claim that has been submitted
        /// </summary>
        ClaimHasBeenSubmitted = 2, 

        /// <summary>
        /// The expense item belongs to an claim that has been approved
        /// </summary>
        ClaimHasBeenApproved = 3,

        /// <summary>
        ///Expense item has been edited after Pay Before Validate       
        /// </summary>
        ExpenseItemHasBeenEdited = 4,

        /// <summary>
        /// Expense item cannot be edited by approver without a reason for amendment    
        /// </summary>
        NoReasonForAmendmentProvidedByApprover = 5
    }
}
