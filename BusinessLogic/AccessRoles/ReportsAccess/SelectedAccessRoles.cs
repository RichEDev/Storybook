namespace BusinessLogic.AccessRoles.ReportsAccess
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary> 
    /// <see cref="SelectedAccessRoles">SelectedAccessRoles</see> defines the SelectedAccessRoles for Report Access
    /// </summary>
    [Serializable]
    public class SelectedAccessRoles : ListWrapper<int>, IReportsAccess
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SelectedAccessRoles"/> class. 
        /// </summary>
        /// <param name="selectedAccessRoles">A collection of linked access roles.</param>
        public SelectedAccessRoles(IEnumerable<int> selectedAccessRoles)
        {
            Guard.ThrowIfNull(selectedAccessRoles, nameof(selectedAccessRoles));

            if (selectedAccessRoles.Any() == false)
            {
                throw new ArgumentException($"{nameof(selectedAccessRoles)} must have at least one entry.");
            }

            this.BackingCollection.AddRange(selectedAccessRoles);
        }

        /// <inheritdoc />
        public ReportingAccess Level => ReportingAccess.SelectedRoles;
    }
}
