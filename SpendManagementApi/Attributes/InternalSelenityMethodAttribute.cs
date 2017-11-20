namespace SpendManagementApi.Attributes
{
    using System;
    using System.Web.Http.Controllers;
    using System.Web.Http.Filters;
    using System.Net;
    using System.Net.Http;
    using System.Web.Http;
    using Utilities;

    /// <summary>
    /// Allows an action only if the IP is local or the IP is in the whitelist in the config
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
    public class InternalSelenityMethodAttribute : ActionFilterAttribute
    {
        /// <inheritdoc />
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            if (!IPValidator.IsSelenity(actionContext.Request))
            {
                throw new HttpResponseException(actionContext.Request.CreateErrorResponse(HttpStatusCode.Forbidden, "Only Selenity staff may access this endpoint."));
            }

            // if we got this far, allow access to the action...
            base.OnActionExecuting(actionContext);
        }
    }
}
