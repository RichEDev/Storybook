namespace BusinessLogic.Identity
{
    using System.Security.Principal;

    /// <summary>
    /// Defines a user and their current identity.
    /// </summary>
    public class UserIdentity : IIdentity
    {
        /// <inheritdoc />
        public string Name { get; }

        /// <inheritdoc />
        public string AuthenticationType { get; internal set; }

        /// <inheritdoc />
        public bool IsAuthenticated { get; internal set; }

        /// <summary>
        /// Gets the unique employee identifier for the current user -OR- the unique employee identifier used for impersonating another user.
        /// </summary>
        /// <remarks>If <see cref="DelegateId"/> is set <see cref="EmployeeId"/> will be set to the impersonated employee identifier.</remarks>
        public int EmployeeId { get; }

        /// <summary>
        /// Gets the unique account identifier for the current user.
        /// </summary>
        public int AccountId { get; }

        /// <summary>
        /// Gets the unique employee identifier for the current user if impersonating another user.
        /// </summary>
        /// <remarks>This represents the users real employee identifier if set.</remarks>
        public int? DelegateId { get; }

        /// <summary>
        /// Gets the current sub account identifier for the user.
        /// </summary>
        public int SubAccountId { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserIdentity"/> class.
        /// </summary>
        /// <param name="accountId">The unique account identifier for the current user.</param>
        /// <param name="employeeId">The unique employee identifier for the current user -OR- the unique employee identifier used for impersonating another user.</param>
        public UserIdentity(int accountId, int employeeId) : this(accountId, employeeId, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserIdentity"/> class.
        /// </summary>
        /// <param name="accountId">The unique account identifier for the current user.</param>
        /// <param name="employeeId">The unique employee identifier for the current user -OR- the unique employee identifier used for impersonating another user.</param>
        /// <param name="delegateId">The unique employee identifier for the current user if impersonating another user.</param>
        public UserIdentity(int accountId, int employeeId, int? delegateId)
        {
            this.AccountId = accountId;
            this.EmployeeId = employeeId;
            this.DelegateId = delegateId;

            this.Name = $"{this.AccountId},{this.EmployeeId}";

            if (this.DelegateId.HasValue)
            {
                this.Name += $",{this.DelegateId.Value}";
            }

            if (this.AccountId > 0 && this.EmployeeId > 0)
            {
                this.IsAuthenticated = true;
            }
        }
    }
}
