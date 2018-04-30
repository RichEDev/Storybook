namespace WebBootstrap
{
    using System.Web;

    using BusinessLogic.Identity;

    /// <summary>
    /// Bootstrap the <see cref="IIdentityContextProvider"/>
    /// </summary>
    public static class IdentityFactory
    {
        /// <summary>
        /// Determines the type of <see cref="IIdentityContextProvider"/> to be bootstrapped
        /// </summary>
        /// <param name="context">The <see cref="HttpContext"/> of the current request</param>
        /// <returns>The concrete type of the <see cref="IIdentityContextProvider"/> to be registered</returns>
        public static IIdentityContextProvider Create(HttpContext context)
        {
            if (context.Request.Path.ToLower().Contains("logon.aspx") || context.Request.Path.ToLower().Contains("sso.aspx") || context.Request.Path.ToLower().Contains("svclogon.asmx") || context.Request.Path.ToLower().Contains("register.aspx"))
            {
                // Create logon identity
                return new LogonIdentityContext();
            }

            // Create web identity
            return new WebIdentityContext();
        }
    }
}