namespace SpendManagementApi.Bootstrap
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

        public WebFeatureFlagContext(IIdentityProvider identityProvider)
        {
            this._identityProvider = identityProvider;
        }
    }
}