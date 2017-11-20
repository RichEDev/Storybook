namespace Auto_Tests.Coded_UI_Tests.Spend_Management.Employee_Management.Employees.ESRAssignments
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// The esr assignments.
    /// </summary>
    public class ESRAssignments
    {

        /// <summary>
        /// Initialises a new instance of the <see cref="ESRAssignments"/> class.
        /// </summary>
        public ESRAssignments()
        {
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="ESRAssignments"/> class.
        /// </summary>
        /// <param name="employeeiId">
        /// The employeei id.
        /// </param>
        /// <param name="assignmentId">
        /// The assignment id.
        /// </param>
        /// <param name="assignmentNumber">
        /// The assignment number.
        /// </param>
        /// <param name="active">
        /// The active.
        /// </param>
        /// <param name="primaryAssignment">
        /// The primary assignment.
        /// </param>
        /// <param name="earliestAssignmentStartDate">
        /// The earliest assignment start date.
        /// </param>
        /// <param name="finalAssignmentEndDate">
        /// The final assignment end date.
        /// </param>
        public ESRAssignments(int employeeiId, int assignmentId, string assignmentNumber, bool active, bool primaryAssignment, DateTime earliestAssignmentStartDate, DateTime finalAssignmentEndDate)
        {
            this.EmployeeId = employeeiId;
            this.AssignmentId = assignmentId;
            this.AssignmentNumber = assignmentNumber;
            this.Active = active;
            this.PrimaryAssignment = primaryAssignment;
            this.EarliestAssignmentStartDate = earliestAssignmentStartDate;
            this.FinalAssignmentEndDate = finalAssignmentEndDate;
        }

        /// <summary>
        /// Gets or sets the employee id.
        /// </summary>
        internal int EmployeeId { get; set; }

        /// <summary>
        /// Gets or sets the assignment id.
        /// </summary>
        internal int AssignmentId { get; set; }

        /// <summary>
        /// Gets or sets the assignment number.
        /// </summary>
        internal string AssignmentNumber { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether active.
        /// </summary>
        internal bool Active { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether primary assignment.
        /// </summary>
        internal bool PrimaryAssignment { get; set; }

        /// <summary>
        /// Gets or sets the earliest assignment start date.
        /// </summary>
        internal DateTime EarliestAssignmentStartDate { get; set; }

        /// <summary>
        /// Gets or sets the final assignment end date.
        /// </summary>
        internal DateTime? FinalAssignmentEndDate { get; set; }

        /// <summary>
        /// Gets or sets the effective start date.
        /// </summary>
        internal DateTime? EffectiveStartDate { get; set; }

        /// <summary>
        /// Gets or sets the effective end date.
        /// </summary>
        internal DateTime? EffectiveEndDate { get; set; }

        /// <summary>
        /// Gets the assignment grid values.
        /// </summary>
        internal List<string> EsrAssignmentGridValues
        {
            get
            {
                return new List<string>
                              {
                                  this.AssignmentNumber,
                                  this.EarliestAssignmentStartDate.ToShortDateString(),
                                  this.Active.ToString(),
                                  this.PrimaryAssignment.ToString(),
                                  this.EffectiveStartDate.ToString(),
                                  this.EffectiveEndDate.ToString()
                              };
            }
        }
    }
}
