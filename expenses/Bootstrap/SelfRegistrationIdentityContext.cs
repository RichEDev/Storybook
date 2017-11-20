namespace expenses.Bootstrap
{
    using BusinessLogic.Identity;

    using System;
    using System.Web;

    /// <summary>
    /// Defines how to get an instance of <see cref="UserIdentity"/> for self registration.
    /// </summary>
    public class SelfRegistrationIdentityContext : IIdentityContextProvider
    {
        /// <summary>
        /// Gets the current users <see cref="UserIdentity"/> for use on self registration.
        /// </summary>
        /// <remarks>Only accountId will ever be set.</remarks>
        /// <returns>An instance of <see cref="UserIdentity"/> for the current user or null if not available.</returns>
        public UserIdentity Get()
        {
            int accountId;

            if (HttpContext.Current == null)
            {
                throw new Exception("Context unavailable.");
            }

            if (HttpContext.Current.Session == null)
            {
                throw new Exception("Session unavailable.");
            }

            if (int.TryParse(HttpContext.Current.Session["selfRegAccountId"].ToString(), out accountId))
            {
                return new UserIdentity(accountId, 0);
            }

            return null;
        }
    }
}