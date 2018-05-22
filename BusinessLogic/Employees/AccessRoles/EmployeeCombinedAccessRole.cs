namespace BusinessLogic.Employees.AccessRoles
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using BusinessLogic.AccessRoles;
    using BusinessLogic.AccessRoles.ApplicationAccess;
    using BusinessLogic.AccessRoles.ReportsAccess;
    using BusinessLogic.AccessRoles.Scopes;

    /// <inheritdoc />
    [Serializable]
    public class EmployeeCombinedAccessRole : IEmployeeAccessScope
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EmployeeCombinedAccessRole"/> class.
        /// </summary>
        /// <param name="assignedAccessRoles">A collection of assigned <see cref="IAccessRole"/> objects to use for 
        /// extracting the highest permissions a user should be granted.
        /// </param>
        public EmployeeCombinedAccessRole(IEnumerable<IAccessRole> assignedAccessRoles)
        {
            Guard.ThrowIfNull(assignedAccessRoles, nameof(assignedAccessRoles));

            this.ReportsAccess = this.GetHighestReportAccess(assignedAccessRoles);
            this.Scopes = this.GetHighestAccessScopeCollection(assignedAccessRoles);
            this.ApplicationScopes = this.GetHighestApplicationScopeCollection(assignedAccessRoles);
            this.Id = this.BuildCompositeKey(assignedAccessRoles);
        }

        /// <summary>
        /// Gets the <see cref="ApplicationScopeCollection"/> specifying what applications this <see cref="AccessRole"/> grants access to.
        /// </summary>
        public ApplicationScopeCollection ApplicationScopes { get; }

        /// <summary>
        /// Gets the composite key made up of access role Id's
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets the <see cref="IReportsAccess" /> for this <see cref="IAccessRole"/>.
        /// </summary>
        public IReportsAccess ReportsAccess { get; }

        /// <summary>
        /// Gets the <see cref="AccessScopeCollection"/> specifying what access this <see cref="AccessRole"/> grants.
        /// </summary>
        public AccessScopeCollection Scopes { get; }

        /// <summary>
        /// Builds a composite key from the collection of <see cref="IAccessRole"/> objects.
        /// </summary>
        /// <param name="accessRoles">The access roles.</param>
        /// <returns>The <see cref="string"/> composite key.</returns>
        private string BuildCompositeKey(IEnumerable<IAccessRole> accessRoles)
        {
            StringBuilder sb = new StringBuilder();

            accessRoles.ToList().Sort((x, y) => x.Id.CompareTo(y.Id));

            foreach (IAccessRole accessRole in accessRoles)
            {
                sb.Append(accessRole.Id + ",");
            }

            // Remove trailing comma
            if (sb.Length > 0)
            {
                sb.Remove(sb.Length - 1, 1);
            }

            return sb.ToString();
        }

        /// <summary>
        /// The get highest report access permissions from the <paramref name="accessRoles"/> collection.
        /// </summary>
        /// <param name="accessRoles">The access roles.</param>
        /// <returns>The highest permissions for each element and returns a <see cref="AccessScopeCollection"/>.</returns>
        private AccessScopeCollection GetHighestAccessScopeCollection(IEnumerable<IAccessRole> accessRoles)
        {
            AccessScopeCollection highestAccessScopeCollection = new AccessScopeCollection();

            foreach (IAccessRole accessRole in accessRoles)
            {
                foreach (IAccessScope accessRoleScope in accessRole.AccessScopes)
                {
                    if (highestAccessScopeCollection[accessRoleScope.Element] == null)
                    {
                        highestAccessScopeCollection.Add(accessRoleScope);

                        // no point continuing since we are taking this accessRoleScope as is.
                        continue;
                    }

                    // Check if the add/edit/delete/view from this iteration is higher than the currently completed highest.
                    if (this.IsHigherScope(accessRoleScope.Add, highestAccessScopeCollection[accessRoleScope.Element].Add))
                    {
                        highestAccessScopeCollection[accessRoleScope.Element].Add = true;
                    }

                    if (this.IsHigherScope(accessRoleScope.Edit, highestAccessScopeCollection[accessRoleScope.Element].Edit))
                    {
                        highestAccessScopeCollection[accessRoleScope.Element].Edit = true;
                    }

                    if (this.IsHigherScope(accessRoleScope.Delete, highestAccessScopeCollection[accessRoleScope.Element].Delete))
                    {
                        highestAccessScopeCollection[accessRoleScope.Element].Delete = true;
                    }

                    if (this.IsHigherScope(accessRoleScope.View, highestAccessScopeCollection[accessRoleScope.Element].View))
                    {
                        highestAccessScopeCollection[accessRoleScope.Element].View = true;
                    }
                }
            }

            return highestAccessScopeCollection;
        }

        /// <summary>
        /// The get highest report access permissions from the <paramref name="accessRoles"/> collection.
        /// </summary>
        /// <param name="accessRoles">The access roles.</param>
        /// <returns>The highest permissions for each element and returns a <see cref="ApplicationScopeCollection"/>.</returns>
        private ApplicationScopeCollection GetHighestApplicationScopeCollection(IEnumerable<IAccessRole> accessRoles)
        {
            ApplicationScopeCollection highestApplicationScopes = new ApplicationScopeCollection();

            foreach (IAccessRole accessRole in accessRoles)
            {
                foreach (IApplicationAccessScope applicationScope in accessRole.ApplicationScopes)
                {
                    if (highestApplicationScopes.Contains(applicationScope.GetType()) == false)
                    {
                        highestApplicationScopes.Add(applicationScope);
                    }
                }
            }

            return highestApplicationScopes;
        }

        /// <summary>
        /// The get highest report access permissions from the <paramref name="accessRoles"/> collection.
        /// </summary>
        /// <param name="accessRoles">The access roles.</param>
        /// <returns>The highest permission <see cref="IReportsAccess"/>.</returns>
        private IReportsAccess GetHighestReportAccess(IEnumerable<IAccessRole> accessRoles)
        {
            IReportsAccess reportsAccess = null;

            foreach (IAccessRole accessRole in accessRoles)
            {
                if (reportsAccess == null)
                {
                    reportsAccess = accessRole.ReportsAccess;
                    continue;
                }

                if (accessRole.ReportsAccess.Level > reportsAccess.Level)
                {
                    reportsAccess = accessRole.ReportsAccess;
                }
            }

            return reportsAccess;
        }

        /// <summary>
        /// Checks if <paramref name="iterationValue"/> grants higher access than <paramref name="existingValue"/>.
        /// </summary>
        /// <param name="existingValue">The current known highest access.</param>
        /// <param name="iterationValue">The access to compare.</param>
        /// <returns>True if <paramref name="iterationValue"/> grants higher access than <paramref name="existingValue"/>; otherwise, false.</returns>
        private bool IsHigherScope(bool existingValue, bool iterationValue)
        {
            return existingValue && iterationValue == false;
        }
    }
}
