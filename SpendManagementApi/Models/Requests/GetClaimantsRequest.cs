namespace SpendManagementApi.Models.Requests
{
    using SpendManagementApi.Models.Common;
    using System.ComponentModel.DataAnnotations;
    using SpendManagementApi.Interfaces;

    /// <summary>
    /// Get claimants advances
    /// </summary>
    public class GetClaimantsRequest : ApiRequest, IEmployeeData
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
    }
}