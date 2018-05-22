namespace BusinessLogic.Employees.AccessRoles
{
    /// <summary>
    /// Defines members to obtain instances of a <see cref="IEmployeeAccessScope"/>.
    /// </summary>
    public interface IEmployeeCombinedAccessRoles
    {
        /// <summary>
        /// Get an instance of <see cref="IEmployeeAccessScope"/> based on the <paramref name="employeeId"/> and <paramref name="subAccountId"/>.
        /// </summary>
        /// <param name="employeeId">The employee id.</param>
        /// <param name="subAccountId">The sub account id.</param>
        /// <returns>The <see cref="IEmployeeAccessScope"/> for the <paramref name="employeeId"/> on the <paramref name="subAccountId"/>.</returns>
        IEmployeeAccessScope Get(int employeeId, int subAccountId);

        /// <summary>
        /// Gets an instance of <see cref="IEmployeeAccessScope"/> based on the <paramref name="compositeId"/>.
        /// </summary>
        /// <param name="compositeId">The composite id.</param>
        /// <returns>
        /// <returns>The <see cref="IEmployeeAccessScope"/> for the <paramref name="compositeId"/>.</returns>
        /// </returns>
        IEmployeeAccessScope Get(string compositeId);
    }
}
