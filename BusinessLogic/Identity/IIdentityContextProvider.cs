namespace BusinessLogic.Identity
{
    /// <summary>
    /// Defines how to get the current <see cref="UserIdentity"/> under the context of an environment.
    /// </summary>
    public interface IIdentityContextProvider
    {
        /// <summary>
        /// Gets the current users <see cref="UserIdentity"/>.
        /// </summary>
        /// <returns>An instance of <see cref="UserIdentity"/> for the current user or null if not available.</returns>
        UserIdentity Get();
    }
}
