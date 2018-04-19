namespace ConsoleBootstrap
{
    using BusinessLogic.Identity;

    /// <summary>
    /// Defines how to get an instance of <see cref="UserIdentity"/>.
    /// </summary>
    public class ConsoleIdentityContext : IIdentityContextProvider
    {
        /// <summary>
        /// Gets the current users <see cref="UserIdentity"/>.
        /// </summary>
        /// <returns>An instance of <see cref="UserIdentity"/>.</returns>
        public UserIdentity Get()
        {
            return new UserIdentity(Bootstrapper.AccountId, 0);
        }
    }
}