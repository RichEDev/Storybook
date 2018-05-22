namespace BusinessLogic.Validator
{
    using System;

    using BusinessLogic.Accounts;
    using BusinessLogic.Employees.AccessRoles;
    using BusinessLogic.Identity;

    /// <summary>
    /// A class to validate that the given <see cref="UserIdentity"/> is a claimant.
    /// </summary>
    [Serializable]
    public class ClaimantValidator : IValidator
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
        public bool Valid(IAccount account, UserIdentity userIdentity, IEmployeeCombinedAccessRoles employeeCombinedAccessRoles)
        {
            return true;
        }
    }
}