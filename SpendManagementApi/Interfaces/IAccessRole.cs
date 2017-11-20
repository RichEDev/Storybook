namespace SpendManagementApi.Interfaces
{
    using System.ComponentModel.DataAnnotations;
    using Utilities;

    /// <summary>
    /// An interface representing the common properties that are present in ALL access role instances.
    /// </summary>
    public interface IAccessRole
    {
        /// <summary>
        /// RoleID of the access role.
        /// </summary>
        [Required, Range(0, int.MaxValue, ErrorMessage = @"Access Role ID must be a valid integer.")]
        int Id { get; set; }

        /// <summary>
        /// Label of the access role.
        /// </summary>
        [Required, MaxLength(100, ErrorMessage = ApiResources.ErrorMaxLength + @"100")]
        string Label { get; set; }
        /// <summary>
        /// Description for the access role.
        /// </summary>
        [MaxLength(4000, ErrorMessage = ApiResources.ErrorMaxLength + @"4000")]
        string Description { get; set; }
    }
}
