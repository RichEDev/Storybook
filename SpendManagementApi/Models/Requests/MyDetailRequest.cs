
namespace SpendManagementApi.Models.Requests
{
    using System.ComponentModel.DataAnnotations;

    using SpendManagementApi.Models.Types.MyDetails;

    /// <summary>
    /// The my detail request.
    /// </summary>
    public class MyDetailRequest
    {
        /// <summary>
        /// Gets or sets the employee id.
        /// </summary>
        public int EmployeeId { get; set; }

        /// <summary>
        /// Gets or sets the title of the employee. Mr, Ms etc.
        /// </summary>
        [Required]
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the forename of the employee.
        /// </summary>
        [Required]
        public string Forename { get; set; }

        /// <summary>
        /// Gets or sets the surname of the employee.
        /// </summary>
        [Required]
        public string Surname { get; set; }


        /// <summary>
        /// Gets or sets the telephone extension number for this employee.
        /// </summary>
        public string TelephoneExtensionNumber { get; set; }

        /// <summary>
        /// Gets or sets the telephone number for this employee.
        /// </summary>
        public string TelephoneNumber { get; set; }

        /// <summary>
        /// Gets or sets the mobilr number for this employee.
        /// </summary>
        public string MobileNumber { get; set; }

        /// <summary>
        /// Gets or sets the pager number for this employee.
        /// </summary>
        public string PagerNumber { get; set; }

        /// <summary>
        /// Gets or sets the email address for this employee.
        /// </summary>
        public string EmailAddress { get; set; }

        /// <summary>
        /// Gets or sets the personal email address for this employee.
        /// </summary>
        public string PersonalEmailAddress { get; set; }
    }
}