using System.Configuration;
using System.Linq.Expressions;
using System.Web.Http;
using SpendManagementApi.Common;

namespace SpendManagementApi.Attributes
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Net;
    using System.Net.Http;
    using System.Web.Http.Filters;
    using Models.Common;

    using SpendManagementLibrary;

    using Utilities;

    /// <summary>
    /// Handles all common errors that the API will throw.
    /// These basically include ApiException, InvalidDataException, and Exception.
    /// </summary>
    public class HandleApiError : ExceptionFilterAttribute
    {
        /// <summary>
        /// Raises the exception event.
        /// </summary>
        /// <param name="actionExecutedContext">The context for the action.</param>
        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            var passFullError = ConfigurationManager.AppSettings["PassFullErrorInStackTrace"] == "1";

            if (actionExecutedContext.Exception is HttpResponseException)
            {
                throw actionExecutedContext.Exception;
            }
            if (actionExecutedContext.Exception is ApiException)
            {
                CreateAppropriateErrorResponse<ApiException>(actionExecutedContext, HttpStatusCode.BadRequest, e => e.Code + ":  " + e.Message, passFullError);
            }
            else if (actionExecutedContext.Exception is ArgumentOutOfRangeException)
            {
                CreateAppropriateErrorResponse<ArgumentOutOfRangeException>(actionExecutedContext, HttpStatusCode.BadRequest, e => e.Message + ":  " + e.Data, passFullError);
            }
            else if (actionExecutedContext.Exception is ArgumentException)
            {
                CreateAppropriateErrorResponse<ArgumentException>(actionExecutedContext, HttpStatusCode.BadRequest, e => e.Message + ":  " + e.Data, passFullError);
            }
            else if (actionExecutedContext.Exception is InvalidDataException)
            {
                CreateAppropriateErrorResponse<InvalidDataException>(actionExecutedContext, HttpStatusCode.BadRequest, e => e.Message, passFullError);
            }
            else if (actionExecutedContext.Exception is NotImplementedException)
            {
                CreateAppropriateErrorResponse<NotImplementedException>(actionExecutedContext, HttpStatusCode.NotImplemented, e => ApiResources.HttpStatusCodeNotImplemented, passFullError);
            }
            else
            {
                CreateAppropriateErrorResponse<Exception>(actionExecutedContext, HttpStatusCode.InternalServerError, e => e.Message, passFullError);
            }

            base.OnException(actionExecutedContext);
        }


        private static void CreateAppropriateErrorResponse<T>(HttpActionExecutedContext context, HttpStatusCode httpStatus, Func<T, string> messageOutput, bool passFullError) where T : Exception
        {
            //Only log non-validation exceptions to the eventlog
            Exception exception = context.Exception;
            if (!(exception is ApiException) && !(exception is InvalidDataException) && !(exception is NotImplementedException))
            {
                cEventlog.LogEntry("API Exception: \r\n" + context.Exception.Message + "\r\n" + context.Exception.StackTrace, true, EventLogEntryType.Error, cEventlog.ErrorCode.DebugInformation);
            }

            context.Response = passFullError
                ? context.Request.CreateErrorResponse(httpStatus, messageOutput.Invoke((T)context.Exception), context.Exception)
                : context.Request.CreateErrorResponse(httpStatus, messageOutput.Invoke((T)context.Exception));
        }

    }
}