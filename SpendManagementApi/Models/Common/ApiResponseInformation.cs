using System.Collections.Generic;
using SpendManagementApi.Common;

namespace SpendManagementApi.Models.Common
{
    /// <summary>
    /// ApiResponseInformation is returned when, despite Http and Auth succeeding, 
    /// there has been an error with processing the data provided to the API. 
    /// This comes in the form of a list of <see cref="ApiErrorDetail">Error</see> objects.
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

        /// <summary>
        /// The status of your remaining API calls.
        /// </summary>
        public string LicensedCallStatus { get; set; }

        /// <summary>
        /// A list of links that represent the available actions related to this resource.
        /// </summary>
        public List<Link> Links { get; set; }
    }
}