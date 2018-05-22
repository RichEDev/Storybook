namespace BusinessLogic.Employees.AccessRoles
{
    using BusinessLogic.Interfaces;

    /// <summary>
    /// Defines members required for a <see cref="IAssignedAccessRole"/>.
    /// </summary>
    public interface IAssignedAccessRole : IIdentifier<string>
    {
        /// <summary>
        /// Gets the employee id this <see cref="IAssignedAccessRole"/> is assigned to.
        /// </summary>
        int EmployeeId { get; }

        /// <summary>
        /// Gets the access role id this <see cref="IAssignedAccessRole"/> is assigned to.
        /// </summary>
        int AccessRoleId { get; }

        /// <summary>
        /// Gets the sub account id this <see cref="IAssignedAccessRole"/> is assigned to.
        /// </summary>
        int SubAccountId { get; }
    }
}
