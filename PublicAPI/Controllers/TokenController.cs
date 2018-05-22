namespace PublicAPI.Controllers
{
    using System;
    using System.Configuration;
    using System.Net;
    using System.Security.Claims;
    using System.Web.Http;

    using BusinessLogic.Employees.AccessRoles;
    using BusinessLogic.Identity;

    using Security;

    /// <summary>
    /// Controller for authentication.
    /// </summary>
    public class TokenController : ApiController
    {
        /// <summary>
        /// The secret key used to verify the request is coming from a trusted source.
        /// </summary>
        private static string SecretKey;

        /// <summary>
        /// The combined access role factory for this request.
        /// </summary>
        private readonly Lazy<IEmployeeCombinedAccessRoles> _combinedAccessRoleFactory = new Lazy<IEmployeeCombinedAccessRoles>(() => WebApiApplication.container.GetInstance<IEmployeeCombinedAccessRoles>());

        /// <summary>
        /// Initializes a new instance of the <see cref="TokenController"/> class.
        /// </summary>
        public TokenController()
        {
            if (string.IsNullOrWhiteSpace(SecretKey))
            {
                SecretKey = ConfigurationManager.AppSettings["logonSecretKey"];
            }
        }

        /// <summary>
        /// Controller action to authenticate and obtain a JWT token.
        /// </summary>
        /// <remarks>
        /// POST: <a href="https://api.hostname/Token">https://api.hostname/Token</a>
        ///  Body: <see cref="TokenRequest"/>
        /// </remarks>
        /// <param name="tokenRequest">The <see cref="TokenRequest"/> to use for generating the JWT Token.</param>
        /// <returns>A JWT Token.</returns>
        [AllowAnonymous]
        public string Post([FromBody]TokenRequest tokenRequest)
        {
            if (tokenRequest != null && tokenRequest.AccountId > 0 && tokenRequest.EmployeeId > 0 && SecretKey == tokenRequest.SecretKey)
            {
                this.RequestContext.Principal = new ClaimsPrincipal(new UserIdentity(tokenRequest.AccountId, tokenRequest.EmployeeId));
                string token = JwtManager.GenerateToken(tokenRequest, this._combinedAccessRoleFactory.Value);

                return token;
            }

            // If the logon fails throw an unauthorized exception (HTTP 401).
            throw new HttpResponseException(HttpStatusCode.Unauthorized);
        }
    }
}