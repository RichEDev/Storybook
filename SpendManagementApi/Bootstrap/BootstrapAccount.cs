namespace SpendManagementApi.Bootstrap
{
    using BusinessLogic.Accounts;
    using BusinessLogic.DataConnections;
    using BusinessLogic.Identity;
    using SimpleInjector;

    /// <summary>
    /// Bootstrapper for obtaining the <see cref="IAccount"/> for the <see cref="UserIdentity"/> and this request.
    /// </summary>
    internal class BootstrapAccount
    {
        /// <summary>
        /// Creates an instance of <see cref="IAccount"/> for the <see cref="UserIdentity"/> and this request.
        /// </summary>
        /// <param name="container">An instance of the DI container to get the current user from.</param>
        /// <returns>an instance of <see cref="IAccount"/> for the <see cref="UserIdentity"/> and this request or null if the <see cref="UserIdentity"/> is in an invalid state.</returns>
        public static IAccount CreateNew(Container container)
        {
            UserIdentity user = container.GetInstance<IIdentityProvider>().GetUserIdentity();

            if (user.AccountId != 0)
            {
                IDataFactory<IAccount, int> accountFactory = container.GetInstance<IDataFactory<IAccount, int>>();
                return accountFactory[user.AccountId];
            }

            return null;
        }
    }
}