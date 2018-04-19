﻿namespace WebBootstrap
{
    using System.Web;
    using BusinessLogic.Identity;

    /// <summary>
    /// Defines how to get an instance of <see cref="UserIdentity"/> for websites.
    /// </summary>
    public class LogonIdentityContext : IIdentityContextProvider
    {
        /// <summary>
        /// Gets the current users <see cref="UserIdentity"/>.
        /// </summary>
        /// <returns>An instance of <see cref="UserIdentity"/> based on <code>HttContext.Current.User.Identity.Name</code>.</returns>
        public UserIdentity Get()
        {
            string identityName = HttpContext.Current.User.Identity.Name;

            if (string.IsNullOrWhiteSpace(identityName))
            {
                return null;
            }

            string[] nameSplit = identityName.Split(',');

            if (nameSplit.Length < 2)
            {
                return null;
            }

            var accountId = int.Parse(nameSplit[0]);
            var employeeId = int.Parse(nameSplit[1]);

            return accountId > 0 ? new UserIdentity(accountId, employeeId) : null;
        }
    }
}