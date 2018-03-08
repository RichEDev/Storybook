namespace ManagementAPI.Controllers
{
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Web.Http;
    using ManagementAPI.Common;
    using ManagementAPI.Common.Enum;
    using ManagementAPI.Models.Responses;

    /// <summary>
    /// The base api controller.
    /// </summary>
    public class BaseApiController : ApiController
    {
        /// <summary>
        /// Initialises the response object
        /// </summary>
        /// <typeparam name="TResult">Response Type</typeparam>
        /// <returns>Initialises response</returns>
        protected TResult InitialiseResponse<TResult>([CallerMemberName] string callerName = null) where TResult : ApiResponse, new()
        {
            var result = new TResult
            {
                ResponseInformation = new ApiResponseInformation
                {
                    Status = ApiStatusCode.Success,
                    Errors = new List<ApiErrorDetail>(),

                }
            };

            return result;
        }
    }
}