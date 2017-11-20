namespace SpendManagementApi.Models.Types.Employees
{
    using System.ComponentModel.DataAnnotations;

    using SpendManagementApi.Interfaces;

    /// <summary>
    /// The very basic parts to identify an employee.
    /// </summary>
    public class EmployeeBasic : IEmployee
    {
        /// <summary>
        /// The unique Id of this Employee.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The title of the employee. Mr, Ms etc.
        /// </summary>
        [Required]
        public string Title { get; set; }

        /// <summary>
        /// The user name of this Employee.
        /// </summary>
        [Required]
        public string UserName { get; set; }

        /// <summary>
        /// The forename of the employee.
        /// </summary>
        [Required]
        public string Forename { get; set; }

        /// <summary>
        /// The surname of the employee.
        /// </summary>
        [Required]
        public string Surname { get; set; }
    }
}