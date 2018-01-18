using BusinessLogic.Identity;

namespace SqlDataAccess.Tests
{
    /// <summary>
    /// Helper to create mock'd <see cref="UserIdentity"/>
    /// </summary>
    public class IdentityContextStub : IIdentityContextProvider
    {
        /// <summary>
        /// Mock account id
        /// </summary>
        private int AccountId { get;}

        /// <summary>
        /// Mock employee id
        /// </summary>
        private int EmployeeId { get;}

        /// <summary>
        /// Constructor for the <see cref="IdentityContextStub"/>
        /// </summary>
        /// <param name="accountId">Mock account id</param>
        /// <param name="employeeId">Mock employee id</param>
        public IdentityContextStub(int accountId, int employeeId)
        {
            this.AccountId = accountId;
            this.EmployeeId = employeeId;
        }

        /// <summary>
        /// Get a UserIdentity
        /// </summary>
        /// <returns>A <see cref="UserIdentity"/></returns>
        public UserIdentity Get()
        {
            return new UserIdentity(this.AccountId, this.EmployeeId);
        }
    }
}
