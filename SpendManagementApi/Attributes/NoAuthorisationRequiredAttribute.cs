namespace SpendManagementApi.Attributes
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Web.Http;
    using System.Web.Http.Controllers;
    using System.Web.Http.Filters;

    using SpendManagementApi.Controllers;
    using SpendManagementApi.Utilities;

    /// <summary>
    /// The no authorisation required attribute. Used for endpoints that don't require an authenitication token.
    /// Can only be using in conjunction with those endpoints that also implement the <see cref="InternalSelenityMethodAttribute"/> as well.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class NoAuthorisationRequiredAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// The on action executing.
        /// </summary>
        /// <param name="actionContext">/
        /// The action context.
        /// </param>
        /// <exception cref="NotImplementedException">
        /// </exception>
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            if (!IPValidator.IsSelenity(actionContext.Request))
            {
                throw new HttpResponseException(actionContext.Request.CreateErrorResponse(HttpStatusCode.Forbidden, "The NoAuthorisationRequiredAttribute can only be used in conjunction with the InternalSelenityMethodAttribute."));
            }

            // attempt to get the controller as a BaseApiController
            var controller = actionContext.ControllerContext.Controller as BaseApiController;

            // throw if the controller is not a BaseApiController.
            if (controller == null)
            {
                throw new NotImplementedException("The controller containing the action this is decorating is not a BaseAPIController.");
            }

            controller.Initialise(); 
        }
    }
}