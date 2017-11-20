namespace PublicAPI.Controllers
{
    using Security.Filters;
    using System.Net;
    using System.Web.Http;
    using Security;

    public class TokenController : ApiController
    {
        [AllowAnonymous]
        public string Post([FromBody]TokenRequest tokenRequest)
        {
            if (tokenRequest != null && tokenRequest.AccountId > 0 && tokenRequest.EmployeeId > 0)
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
