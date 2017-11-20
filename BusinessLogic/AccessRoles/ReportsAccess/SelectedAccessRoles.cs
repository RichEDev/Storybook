namespace BusinessLogic.AccessRoles.ReportsAccess
{
    using System;

    /// <summary> 
    /// <see cref="SelectedAccessRoles">SelectedAccessRoles</see> defines the SelectedAccessRoles for Report Access
    /// </summary>
    public class SelectedAccessRoles : IReportsAccess, ISelectedAccessRoles
    {
        private readonly LinkedAccessRoleCollection _selectedAccessRoles;

        /// <summary>
        /// Initialises an instance of <see cref="SelectedAccessRoles">SelectedAccessRoles</see>
        /// </summary>
        /// <param name="selectedAccessRoles">The <see cref="LinkedAccessRoleCollection">LinkedAccessRoleCollection</see></param>
        public SelectedAccessRoles(LinkedAccessRoleCollection selectedAccessRoles)
        {
            if (selectedAccessRoles == null)
            {
                throw new ArgumentNullException(nameof(selectedAccessRoles));
            }

            if (selectedAccessRoles.Count == 0)
            {
                throw new ArgumentException($"{nameof(selectedAccessRoles)} must have at least one entry.");
            }

            this._selectedAccessRoles = selectedAccessRoles;
        }

        /// <summary>
        /// Returns whether an accessRoleId if it is in the backing collection
        /// </summary>
        /// <param name="accessRoleId">The access role Id to check for</param>
        /// <returns>The result of the lookup</returns>
        public bool Contains(int accessRoleId)
        {
            return this._selectedAccessRoles.Contains(accessRoleId);
        }
    }
}
