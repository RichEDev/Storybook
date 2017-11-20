namespace SpendManagementApi.Models.Types
{
    using System.ComponentModel.DataAnnotations;

    using SpendManagementApi.Interfaces;

    /// <summary>
    /// An access role in it's most basic format
    /// </summary>
    [MetadataType(typeof(IAccessRole))]
    public class AccessRoleBasic : IAccessRole
    {
        /// <summary>
        /// RoleID of the access role.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Label of the access role.
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// Description for the access role.
        /// </summary>
        public string Description { get; set; }
    }
} 