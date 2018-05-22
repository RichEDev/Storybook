namespace BusinessLogic.Validator
{
    using BusinessLogic.Accounts;
    using BusinessLogic.Employees.AccessRoles;
    using BusinessLogic.Identity;

    /// <summary>
    /// An interface to determine if a object is valid based on the account and indentity.
    /// </summary>
    public interface IValidator
    {
        /// <summary>
        /// Is this object valid, based on the given <see cref="IAccount"/> and <see cref="UserIdentity"/>.
        /// </summary>
        /// <param name="account">
        /// An instance of <see cref="IAccount"/>.
        /// </param>
        /// <param name="userIdentity">
        /// An instance of <see cref="UserIdentity"/>.
        /// </param>
        /// <param name="employeeCombinedAccessRoles">
        /// An instance of <see cref="IEmployeeCombinedAccessRoles"/>.
        /// </param>
        /// <returns>
        /// <see cref="bool"/> true if this object is valid..
        /// </returns>
        bool Valid(IAccount account, UserIdentity userIdentity, IEmployeeCombinedAccessRoles employeeCombinedAccessRoles);
    }
}