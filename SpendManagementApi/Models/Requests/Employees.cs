namespace SpendManagementApi.Models.Requests
{
    using System.Collections.Generic;

    using Common;

    /// <summary>
    /// Facilitates the finding of Employees, by providing a few optional search / filter parameters.
    /// </summary>
    public class FindEmployeeRequest : FindRequest
    {
        /// <summary>
        /// Find users by Title.
        /// </summary>
        public string Title { get; set; }
        
        /// <summary>
        /// Find users by Username.
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Find users by first name.
        /// </summary>
        public string Forename { get; set; }

        /// <summary>
        /// Find users by last name.
        /// </summary>
        public string Surname { get; set; }
    }

    /// <summary>
    /// Accepts a list of employee ids to be activated
    /// </summary>
    public class ActivateEmployeesRequest
    {
        /// <summary>
        /// List of Employee Ids.
        /// </summary>
        public List<int> EmployeeIds { get; set; }

    }
}