namespace SpendManagementApi.Models.Types
{
    using System;

    /// <summary>
    /// A class which holds basic details of an ESR Assignment
    /// </summary>
    public class ESRAssignmentBasic
    {
        /// <summary>
        /// Gets or sets the assignment id.
        /// </summary>
        public long AssignmentId { get; set; }

        /// <summary>
        /// Gets or sets the assignment number.
        /// </summary>
        public string AssignmentNumber { get; set; }

        /// <summary>
        /// Gets or sets the assignment text.
        /// </summary>
        public string AssignmentText { get; set; }

        /// <summary>
        /// Gets or sets the earliest assignment start date.
        /// </summary>
        public DateTime EarliestAssignmentStartDate { get; set; }

        /// <summary>
        /// Gets or sets the effective start date.
        /// </summary>
        public DateTime? EffectiveStartDate { get; set; }

        /// <summary>
        /// Gets or sets the effective end date.
        /// </summary>
        public DateTime? EffectiveEndDate { get; set; }

        /// <summary>
        /// Gets or sets if the assignment is active.
        /// </summary>
        public bool Active { get; set; }
    }
}