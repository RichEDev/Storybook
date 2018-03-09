namespace PublicAPI.Controllers
{
    using System.Configuration;

    using Security.Filters;
    using System.Net;
    using System.Web.Http;
    using Security;

    public class TokenController : ApiController
    {
        private static string SecretKey;

        public TokenController()
        {
            if (string.IsNullOrWhiteSpace(SecretKey))
            {
                SecretKey = ConfigurationManager.AppSettings["logonSecretKey"];
            }
        }

        [AllowAnonymous]
        public string Post([FromBody]TokenRequest tokenRequest)
        {
            if (tokenRequest != null && tokenRequest.AccountId > 0 && tokenRequest.EmployeeId > 0 && SecretKey == tokenRequest.SecretKey)
            {
                string token = JwtManager.GenerateToken(tokenRequest);

                JwtAuthenticationAttribute authentication = new JwtAuthenticationAttribute();
                this.RequestContext.Principal = authentication.AuthenticateJwtToken(token).Result;

                return token;
            }

            // if the logon fails 
            throw new HttpResponseException(HttpStatusCode.Unauthorized);
        }
    }
}