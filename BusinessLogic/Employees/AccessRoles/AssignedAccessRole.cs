namespace BusinessLogic.Employees.AccessRoles
{
    using System;

    /// <summary>
    /// Denotes an access role, employee and sub-account linked object.
    /// </summary>
    [Serializable]
    public class AssignedAccessRole : IAssignedAccessRole
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AssignedAccessRole"/> class.
        /// </summary>
        /// <param name="employeeId">The employee id this <see cref="IAssignedAccessRole"/> is assigned to.</param>
        /// <param name="accessRoleId">The access role id this <see cref="IAssignedAccessRole"/> is assigned to.</param>
        /// <param name="subAccountId">The sub account id this <see cref="IAssignedAccessRole"/> is assigned to.</param>
        public AssignedAccessRole(int employeeId, int accessRoleId, int subAccountId)
        {
            this.EmployeeId = employeeId;
            this.AccessRoleId = accessRoleId;
            this.SubAccountId = subAccountId;
        }

        /// <inheritdoc />
        public int AccessRoleId { get; }

        /// <inheritdoc />
        public int EmployeeId { get; }

        /// <inheritdoc />
        public int SubAccountId { get; }

        /// <inheritdoc />
        public string Id
        {
            get => $"{this.EmployeeId},{this.AccessRoleId},{this.SubAccountId}";
            set => throw new NotImplementedException();
        }
    }
}
