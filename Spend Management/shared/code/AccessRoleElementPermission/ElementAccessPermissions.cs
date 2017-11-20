namespace SpendManagementLibrary.Employees.ElementAccess
{

    using IAccessRoleElementPermissions = Spend_Management.shared.code.AccessRoleElementPermission.Interfaces.IAccessRoleElementPermissions;

    /// <summary>
    /// A class to hold the access permissions for a spendmanagement element
    /// </summary>
    public class ElementAccessPermissions : IAccessRoleElementPermissions
    {
      
        /// <summary>
        /// Gets or sets a value indicating whether the user can view the element.
        /// </summary>
        public bool CanView { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the user can add the element.
        /// </summary>
        public bool CanAdd { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the user can edit the element.
        /// </summary>
        public bool CanEdit { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the user can delete the element.
        /// </summary>
        public bool CanDelete { get; set; }
    }
}
