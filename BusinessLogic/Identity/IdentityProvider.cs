namespace BusinessLogic.Identity
{
    /// <summary>
    /// Defines members to get the current users <see cref="UserIdentity"/>
    /// </summary>
    public class IdentityProvider : IIdentityProvider
    {
        private readonly IIdentityContextProvider _identityContextProvider;

        /// <summary>
        /// Initializes a new instance of the<see cref="IdentityProvider"/> class.
        /// </summary>
        /// <param name="identityContextProvider">The <see cref="IIdentityContextProvider"/> to use when creating a new <see cref="UserIdentity"/>.</param>
        public IdentityProvider(IIdentityContextProvider identityContextProvider)
        {
            Guard.ThrowIfNull(identityContextProvider, nameof(identityContextProvider));

            this._identityContextProvider = identityContextProvider;
        }

        /// <summary>
        /// Gets the current users <see cref="UserIdentity"/>.
        /// </summary>
        /// <returns>An instance of <see cref="UserIdentity"/> for the current user or null if not available.</returns>
        public UserIdentity GetUserIdentity()
        {
            UserIdentity identity = this._identityContextProvider.Get();

            if (identity != null && identity.AccountId > 0 && identity.EmployeeId > 0)
            {
                identity.IsAuthenticated = true;
            }

            return identity;
        }
    }
}
