namespace SpendManagementLibrary.Employees
{
    using System;

    /// <summary>
    /// The employee item role.
    /// </summary>
    public class EmployeeItemRole
    {
        /// <summary>
        /// The item role id.
        /// </summary>
        public int ItemRoleId;

        /// <summary>
        /// The start date for the current item role for the employee
        /// </summary>
        public DateTime? StartDate;

        /// <summary>
        /// The end date for the current item role for the employee
        /// </summary>
        public DateTime? EndDate;

        /// <summary>
        /// The item role name.
        /// </summary>
        public string ItemRoleName;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmployeeItemRole"/> class.
        /// </summary>
        public EmployeeItemRole()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EmployeeItemRole"/> class.
        /// </summary>
        /// <param name="itemRoleId">
        /// The item role id that is set agaisnst the employee
        /// </param>
        /// <param name="startDate">
        /// The start date for the current item role for the employee
        /// </param>
        /// <param name="endDate">
        /// The end date of the item for the current item role for the employee
        /// </param>
        public EmployeeItemRole(int itemRoleId, DateTime? startDate = null, DateTime? endDate = null)
        {
            this.ItemRoleId = itemRoleId;
            this.StartDate = startDate;
            this.EndDate = endDate;
        }
    }
}