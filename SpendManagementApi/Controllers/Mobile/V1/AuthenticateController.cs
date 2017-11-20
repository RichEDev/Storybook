namespace SpendManagementApi.Controllers.Mobile.V1
{
    using System.Web.Http;
    using System.Web.Http.Description;

    using SpendManagementApi.Attributes;
    using SpendManagementApi.Attributes.Mobile;

    using SpendManagementLibrary.Mobile;

    /// <summary>
    /// The controller that handles authentication.
    /// </summary>
    [Version(1)]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class AuthenticateV1Controller : BaseMobileApiController
    {
        /// <summary>
        /// Authenticates a pairing key and serial key combination to ensure it is still valid - used every time the app is launched
        /// </summary>
        /// <returns>A <see cref="ServiceResultMessage"/> pre populated with valid values.</returns>
        [HttpGet]
        [MobileAuth]
        [Route("mobile/authenticate")]
        public ServiceResultMessage Authenticate()
        {
            return this.ServiceResultMessage;
        }
    }
}