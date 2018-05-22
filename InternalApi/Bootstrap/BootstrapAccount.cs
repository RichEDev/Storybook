namespace InternalApi.Bootstrap
{
    using BusinessLogic.Accounts;
    using BusinessLogic.Databases;
    using BusinessLogic.DataConnections;
    using BusinessLogic.Identity;

    using SimpleInjector;

    using Utilities.Cryptography;

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
            // Returning an artificial account as there will not be a real one for InternalAPI.
            return new Account(0, new DatabaseCatalogue(new DatabaseServer(0, string.Empty), "metabaseExpenses", "spenduser", "JT29Nz0PxpDmnGr2zcXooQ==", new ExpensesCryptography()), false);            
        }
    }
}