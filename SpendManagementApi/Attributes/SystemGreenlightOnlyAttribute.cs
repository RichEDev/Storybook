namespace SpendManagementApi.Attributes
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Web.Http;
    using System.Web.Http.Controllers;
    using System.Web.Http.Filters;
    using SpendManagementApi.Utilities;

    /// <summary>
    /// Authorises and Authenticates an ActionMethod, using the request headers,
    /// coupled with Spend Management's Access Roles.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
    [SystemGreenlightOnly]
    public class SystemGreenlightOnlyAttribute : ActionFilterAttribute
    {
         /// <summary>
         /// Only valid ip addresses should be allowed to access the system greenlight actions.
        /// </summary>
        /// <param name="actionContext"> Action request</param>
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            if (!IPValidator.IsSystemGreenlightOnly(actionContext.Request))
            {
                throw new HttpResponseException(actionContext.Request.CreateErrorResponse(HttpStatusCode.Forbidden, "You do not have the permissions to perform this action."));
            }

            // if we got this far, allow access to the action...
            base.OnActionExecuting(actionContext);
        }
    }
}