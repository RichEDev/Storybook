namespace PublicAPI.Bootstrap
{
    using System;
    using System.Linq;
    using System.Security.Claims;
    using System.Web;
    using BusinessLogic.Identity;

    /// <summary>
    /// Defines how to get an instance of <see cref="UserIdentity"/> for websites.
    /// </summary>
    public class WebIdentityContext : IIdentityContextProvider
    {
        /// <summary>
        /// Gets the current users <see cref="UserIdentity"/>.
        /// </summary>
        /// <returns>An instance of <see cref="UserIdentity"/> based on <code>HttContext.Current.User.Identity.Name</code>.</returns>
        public UserIdentity Get()
        {
            int delegateId = 0;

            ClaimsIdentity claimsIdentity = HttpContext.Current.User.Identity as ClaimsIdentity;

            string identityName = claimsIdentity.Name;

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

            if (nameSplit.Length == 3)
            {
                delegateId = int.Parse(nameSplit[2]);
            }

            UserIdentity userIdentity = null;
            
            if (accountId > 0 && employeeId > 0)
            {
                var subAccountClaim = claimsIdentity.Claims.FirstOrDefault(claim => claim.Type == "SubAccountId");
                userIdentity = new UserIdentity(accountId, employeeId, delegateId);

                if (subAccountClaim != null)
                {
                    userIdentity.SubAccountId = Convert.ToInt32(subAccountClaim.Value);
                }
            }

            return userIdentity;
        }
    }
}