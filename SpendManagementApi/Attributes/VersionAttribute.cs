namespace SpendManagementApi.Attributes
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Web.Http.Controllers;
    using System.Web.Http.Filters;

    /// <summary>
    /// Allows specification of the version number
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class VersionAttribute : ActionFilterAttribute
    {
        #region Constructor

        /// <summary>
        /// Specifies the version number
        /// </summary>
        /// <param name="version">The version number.</param>
        public VersionAttribute(int version)
        {
        }

        #endregion

        /// <summary>
        /// Checks validity of the action context.
        /// </summary>
        /// <param name="actionContext"></param>
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            if (!actionContext.ModelState.IsValid)
            {
                actionContext.Response = actionContext.Request.CreateErrorResponse(HttpStatusCode.BadRequest, actionContext.ModelState);
            }
        }
    }
}