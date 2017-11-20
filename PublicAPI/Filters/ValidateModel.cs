namespace PublicAPI.Filters
{
    using System.Net;
    using System.Net.Http;
    using System.Web.Http.Controllers;
    using System.Web.Http.Filters;

    /// <summary>
    /// Validates a request to a controller action is correct; If not it will return a 400 Bad Request with validation failures.
    /// </summary>
    public class ValidateModelAttribute  : ActionFilterAttribute
    {
        /// <summary>
        /// Occurs before the action method is invoked. Search web api filters for more information.
        /// </summary>
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            if (actionContext.ModelState.IsValid == false)
            {
                actionContext.Response = actionContext.Request.CreateErrorResponse(HttpStatusCode.BadRequest, actionContext.ModelState);
            }
        }
    }
}