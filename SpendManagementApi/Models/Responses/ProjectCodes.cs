using System.Collections.Generic;
using SpendManagementApi.Models.Common;
using SpendManagementApi.Models.Types;

namespace SpendManagementApi.Models.Responses
{
    /// <summary>
    /// A response containing a list of <see cref="ProjectCode">Item</see>s.
    /// </summary>
    public class GetProjectCodesResponse : GetApiResponse<ProjectCode>
    {
        /// <summary>
        /// Creates a new GetProjectCodesResponse.
        /// </summary>
        public GetProjectCodesResponse()
        {
            List = new List<ProjectCode>();
        }
    }

    /// <summary>
    /// A response containing a particular <see cref="ProjectCode">ProjectCode</see>.
    /// </summary>
    public class ProjectCodeResponse : ApiResponse<ProjectCode>
    {

    }
}