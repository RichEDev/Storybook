namespace SpendManagementApi.Models.Responses
{
    using System.Collections.Generic;
    using Common;
    using Types.Employees;

    public class GetEmployeesResponse : GetApiResponse<Employee>
    {
    }

    public class FindEmployeesResponse : GetEmployeesResponse
    {

    }

    public class EmployeeResponse : ApiResponse<Employee>
    {
    }

    /// <summary>
    /// Represents the result of acivating a list of employees.
    /// </summary>
    public class EmployeeActivateResponse : ApiResponse
    {
        /// <summary>
        /// A list of employees who were activated.
        /// </summary>
        public List<int> ActivatedEmployees { get; set; }

        /// <summary>
        /// A list of employees who were already activated.
        /// </summary>
        public List<int> AlreadyActivatedEmployees { get; set; }
    }
}