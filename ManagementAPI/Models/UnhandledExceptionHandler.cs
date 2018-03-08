namespace ManagementAPI.Models
{
    using System.Web.Http.ExceptionHandling;

    public class UnhandledExceptionHandler : ExceptionHandler
    {       
        public override void Handle(ExceptionHandlerContext context)
        {
            context.Result = new TextPlainErrorResult
            {
                Request = context.ExceptionContext.Request,
                Content = "Oops, something bad happened",
                Message = context.Exception.Message
            };
        }
    }
}