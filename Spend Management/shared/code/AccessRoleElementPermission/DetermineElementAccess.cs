namespace SpendManagementLibrary.Employees.ElementAccess
{
    using Spend_Management.shared.code.AccessRoleElementPermission.Interfaces;

    /// <summary>
    /// Determines the element access permissions for a user
    /// </summary>
    public static class DetermineElementAccess
    {
        /// <summary>
        /// Sets the element access permissions for a user
        /// </summary>
        /// <param name="accessRolePermissions">
        /// An instance of <see cref="IAccessRoleElementPermissions"/>
        /// </param>
        /// <param name="user">
        /// An instance of <see cref="BusinessLogic.CurrentUser.ICurrentUser"/>
        /// </param>
        /// <param name="element">
        /// The <see cref="SpendManagementElement"/> to check
        /// </param>
        /// <param name="checkIfDelegate">
        /// If the check needs to consider the delegate
        /// </param>
        /// <returns>
        /// An instance of <see cref="IAccessRoleElementPermissions"/> with the permissions set.
        /// </returns>
        public static IAccessRoleElementPermissions SetElementPermissions(IAccessRoleElementPermissions accessRolePermissions, Spend_Management.ICurrentUser user, SpendManagementElement element, bool checkIfDelegate = true )
        {          
            accessRolePermissions.CanView = user.CheckAccessRole(AccessRoleType.View, element, checkIfDelegate);
            accessRolePermissions.CanEdit = user.CheckAccessRole(AccessRoleType.Edit, element, checkIfDelegate);         
            accessRolePermissions.CanAdd = user.CheckAccessRole(AccessRoleType.Add, element, checkIfDelegate);
            accessRolePermissions.CanDelete = user.CheckAccessRole(AccessRoleType.Delete, element, checkIfDelegate);

            return accessRolePermissions;
        }
    }
}
