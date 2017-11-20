namespace SpendManagementLibrary.Claims
{
    using System.Collections.Generic;

    /// <summary>
    /// Represents claim submission approver.
    /// </summary>
    public class ClaimSubmissionApprover
    {
        /// <summary>
        /// Gets or sets recent approvers.
        /// </summary>
        public SortedList<string, string> RecentApprovers { get; set; }

        /// <summary>
        /// Gets or sets all approvers.
        /// </summary>
        public SortedList<string, string> AllApprovers { get; set; }

        /// <summary>
        /// Gets or sets the last approverId of the employee's previous claim.
        /// </summary>
        public int LastApproverId { get; set; }

        /// <summary>
        /// Gets or sets the last approver of the employee's previous claim.
        /// </summary>
        public string LastApprover { get; set; }
    }
}
