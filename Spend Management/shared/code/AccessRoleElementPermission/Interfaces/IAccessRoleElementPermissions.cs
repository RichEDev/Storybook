namespace Spend_Management.shared.code.AccessRoleElementPermission.Interfaces
{
    /// <summary>
    /// The AccessRoleElementPermissions interface.
    /// </summary>
    public interface IAccessRoleElementPermissions
    {
        /// <summary>
        /// Gets or sets a value indicating whether the user can view the element.
        /// </summary>
        bool CanView { get;set; }

        /// <summary>
        /// Gets or sets a value indicating whether the user can add the element.
        /// </summary>
        bool CanAdd { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the user can edit the element.
        /// </summary>
        bool CanEdit { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the user can delete the element.
        /// </summary>
        bool CanDelete { get; set; }
    }
}
