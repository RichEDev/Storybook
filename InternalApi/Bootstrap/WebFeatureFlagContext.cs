namespace InternalApi.Bootstrap
{
    using BusinessLogic.Identity;
    using SEL.FeatureFlags.Context;

    /// <inheritdoc />
    public class WebFeatureFlagContext : IFeatureFlagsContextProvider
    {
        private readonly IIdentityProvider _identityProvider;

        /// <inheritdoc />
        public IRequestContext Get()
        {
            UserIdentity userIdentity = this._identityProvider.GetUserIdentity();
            IRequestContext requestContext = new RequestContext(userIdentity?.EmployeeId, string.Empty, string.Empty);

            return requestContext;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WebFeatureFlagContext"/> class.
        /// </summary>
        /// <param name="identityProvider">
        /// Initializes the value of <see cref="identityProvider"/>.
        /// </param>
        public WebFeatureFlagContext(IIdentityProvider identityProvider)
        {
            this._identityProvider = identityProvider;
        }
    }
}