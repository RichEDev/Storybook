namespace SpendManagementApi.Models.Types.Employees
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using Spend_Management;
    using Interfaces;
    using Repositories;
    using Utilities;

    /// <summary>
    /// Represents all of the <see cref="AccessRole">AccessRoles</see> that apply to an Employee.
    /// The access roles are represented by ids, to keep the objects simpler.
    /// </summary>
    public class EmployeePermissions : BaseExternalType, IRequiresValidation, IEquatable<EmployeePermissions>
    {
        /// <summary>
        /// The Default Sub Account Id of the employee. Leave this as null if you don't know what a sub account is 
        /// or you don't have any sub accounts set up for your organisation.
        /// </summary>
        public int? DefaultSubAccountId { get; set; }

        /// <summary>
        /// The list of associated AccessRoles for this Employee.
        /// <strong>Do not try to modify this user's AccessRoles by changing this list.</strong>
        /// Instead use the <see cref="AccessRole">AccessRoles</see> resource.
        /// </summary>
        public List<int> AccessRoles { get; set; }


        /// <summary>
        /// Validates this EmployeePermissions object.
        /// </summary>
        /// <param name="actionContext">The action context.</param>
        /// <exception cref="InvalidDataException"></exception>
        public void Validate(IActionContext actionContext)
        {
            // make sure that if the sub account is not set, set it to its default.
            if (DefaultSubAccountId == null)
            {
                DefaultSubAccountId = actionContext.SubAccountId;
            }

            if (DefaultSubAccountId <= 0)
            {
                throw new InvalidDataException(ApiResources.ApiErrorSubAccount);
            }
        }

        internal static EmployeePermissions Merge(EmployeePermissions permissionsToUpdate, EmployeePermissions existing)
        {
            return permissionsToUpdate ?? (new EmployeePermissions
            {
                AccessRoles = existing.AccessRoles,
                DefaultSubAccountId = existing.DefaultSubAccountId
            });
        }

        public bool Equals(EmployeePermissions other)
        {
            return other != null && (AccessRoles.SequenceEqual(other.AccessRoles) && DefaultSubAccountId.Equals(other.DefaultSubAccountId));
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as EmployeePermissions);
        }
    }

    internal static class EmployeePermissionsConversion
    {
        internal static TResult Cast<TResult>(this SpendManagementLibrary.Employees.Employee employee, 
            AccessRoleRepository accessRoleRepository,
            cAccountSubAccounts accountSubAccounts)
            where TResult : EmployeePermissions, new()
        {
            return new TResult
                       {
                           AccessRoles = employee.GetAccessRoles().GetBy(employee.DefaultSubAccount),
                           DefaultSubAccountId = employee.DefaultSubAccount
                       };
        }
    }
}