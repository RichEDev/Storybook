namespace ManagementAPI.Filters
{
    using System.Net;
    using System.Net.Http;
    using System.Web.Http.Filters;

    public class ManagementToolExceptionFilterAttribute : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext context)
        {
            var response = new HttpResponseMessage();

            var message = context.Exception.GetType().ToString() + ": " + context.Exception.Message.ToString();
            response.Content = new StringContent(message);
            response.StatusCode = HttpStatusCode.InternalServerError;

            context.Response = response;
        }
    }
}