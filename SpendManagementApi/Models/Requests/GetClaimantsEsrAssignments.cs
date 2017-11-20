namespace SpendManagementApi.Models.Requests
{
    using System;
    using System.ComponentModel.DataAnnotations;

    using SpendManagementApi.Attributes.Validation;
    using SpendManagementApi.Interfaces;
    using SpendManagementApi.Models.Common;

    /// <summary>
    /// The request for getting a claimants assignments 
    /// </summary>
    public class GetClaimantsEsrAssignmentsRequest : ApiRequest, IEmployeeData
    {
        /// <summary>
        /// Gets or sets the employee id.
        /// </summary>
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = @"Please enter an EmployeeId greater than {1}")]
        public int EmployeeId { get; set; }

        /// <summary>
        /// Gets or sets the expense id.
        /// </summary>     
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = @"Please enter an ExpenseId greater than {1}")]
        public int ExpenseId { get; set; }

        /// <summary>
        /// Gets or sets the expense date.
        /// </summary>
        [Required]
        [ExpenseDateValidation("ExpenseDate")]
        public DateTime ExpenseDate { get; set; }
    }
}