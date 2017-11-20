namespace SpendManagementApi.Interfaces
{
    using System.ComponentModel.DataAnnotations;

    using SpendManagementApi.Utilities;

    /// <summary>
    /// An interface representing  the common properties to be found in ALL employee instances.
    /// </summary>
    public interface IEmployee
    {
        /// <summary>
        /// The unique Id of this Employee.
        /// </summary>
        [Required, Range(0, int.MaxValue, ErrorMessage = @"Employee ID must be a valid integer.")]
        int Id { get; set; }

        /// <summary>
        /// The title of the employee. Mr, Ms etc.
        /// </summary>
        [Required, MaxLength(30, ErrorMessage = ApiResources.ErrorMaxLength + @"30")]
        string Title { get; set; }

        /// <summary>
        /// The user name of this Employee.
        /// </summary>
        [Required, MaxLength(50, ErrorMessage = ApiResources.ErrorMaxLength + @"50")] 
        string UserName { get; set; }

        /// <summary>
        /// The forename of the employee.
        /// </summary>
        [Required, MaxLength(150, ErrorMessage = ApiResources.ErrorMaxLength + @"150")] 
        string Forename { get; set; }

        /// <summary>
        /// The surname of the employee.
        /// </summary>
        [Required, MaxLength(150, ErrorMessage = ApiResources.ErrorMaxLength + @"150")]
        string Surname { get; set; }
    }
}
