namespace BusinessLogic.Identity
{
    /// <summary>
    /// Defines members to get the current users <see cref="UserIdentity"/>
    /// </summary>
    public interface IIdentityProvider
    {
        /// <summary>
        /// Gets the current users <see cref="UserIdentity"/>.
        /// </summary>
        /// <returns>An instance of <see cref="UserIdentity"/> for the current user or null if not available.</returns>
        UserIdentity GetUserIdentity();
    }
}