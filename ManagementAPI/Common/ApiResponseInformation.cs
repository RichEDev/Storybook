namespace ManagementAPI.Models.Responses
{
    using System.Collections.Generic;
    using ManagementAPI.Common;
    using ManagementAPI.Common.Enum;

    /// <summary>
    /// The api response information.
    /// </summary>
    public class ApiResponseInformation
    {
        /// <summary>
        /// The IList of ApiErrors that occured while processing the request.
        /// </summary>
        public IList<ApiErrorDetail> Errors { get; set; }

        /// <summary>
        /// A status code indicating the success failure of the message
        /// </summary>
        public ApiStatusCode Status { get; set; }   
    }
}