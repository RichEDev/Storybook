namespace SpendManagementApi.Attributes.Mobile
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Web.Http;
    using System.Web.Http.Controllers;
    using System.Web.Http.Filters;

    using SpendManagementApi.Common;
    using SpendManagementApi.Controllers.Mobile;

    using SpendManagementLibrary.Mobile;

    /// <summary>
    /// This attribute causes the method to validate the pairing key and serial key in the request header.
    /// </summary>
    public class MobileAuthAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// Occurs before the action method is invoked.
        /// </summary>
        /// <param name="actionContext">The action context.</param>
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            var controller = actionContext.ControllerContext.Controller as BaseMobileApiController;

            if (controller == null)
            {
                throw new NotImplementedException("The controller containing the action this is decorating is not a BaseMobileApiController.");
            }

            string key = Common.Mobile.HttpRequestMessageExtensions.GetHeader(actionContext.Request, "PairingKeySerialKey");

            if (key == null)
            {
                throw new HttpResponseException(actionContext.Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Invalid header"));
            }

            var headerArray = key.Split(char.Parse("|"));

            var pksk = new PairingKeySerialKey(new PairingKey(headerArray[0]), headerArray[1]);

            var serviceResultMessage = Authenticator.Authenticate(pksk.PairingKey.Pairingkey, pksk.SerialKey);

            controller.Initialise(serviceResultMessage, pksk);
        }
    }
}