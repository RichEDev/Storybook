namespace WebBootstrap
{
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

            int? delegateId = null;

            if (nameSplit.Length == 3)
            {
                delegateId = int.Parse(nameSplit[2]);
            }

            if (HttpContext.Current != null && HttpContext.Current.Session != null && delegateId == null)
            {
                if (HttpContext.Current.Session["myid"] != null)
                {
                    delegateId = (int)HttpContext.Current.Session["myid"];
                }
            }

            var accountId = int.Parse(nameSplit[0]);
            var employeeId = int.Parse(nameSplit[1]);

            if (accountId > 0 && employeeId > 0)
            {
                return new UserIdentity(accountId, employeeId, delegateId);
            }

            return null;
        }
    }
}