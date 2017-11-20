using SpendManagementApi.Models.Types;
using SpendManagementApi.Models.Types.Employees;

namespace SpendManagementApi.Models.Responses
{
    using Common;

    /// <summary>
    /// Represents the result of tying together an <see cref="Employee">Employee</see> 
    /// and another item, such as an <see cref="AccessRole">AccessRole</see>...
    /// </summary>
    public class EmployeeLinkageResponse : ApiResponse
    {
        /// <summary>
        /// The Id of the employee.
        /// </summary>
        public int EmployeeId { get; set; }
        
        /// <summary>
        /// The Id of the item that has been linked.
        /// </summary>
        public int LinkedItemId { get; set; }
    }
}