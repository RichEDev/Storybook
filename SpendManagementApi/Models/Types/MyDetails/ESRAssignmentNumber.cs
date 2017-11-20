using System;

namespace SpendManagementApi.Models.Types.MyDetails
{
    /// <summary>
    /// The esr assignments.
    /// </summary>
    public class ESRAssignmentNumber
    {
        /// <summary>
        /// Gets the assignment number.
        /// </summary>
        public string AssignmentNumber { get; set; }

        /// <summary>
        /// Gets the assignment start date.
        /// </summary>
        public DateTime Assignmentstartdate { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether active.
        /// </summary>
        public bool Active { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether primary assignment.
        /// </summary>
        public string PrimaryAssignment { get; set; }

        /// <summary>
        /// Gets or sets the effective start date.
        /// </summary>
        public DateTime EffectiveStartDate { get; set; }

        /// <summary>
        /// Gets or sets the effective end date.
        /// </summary>
        public DateTime EffectiveEndDate { get; set; }
    }
}