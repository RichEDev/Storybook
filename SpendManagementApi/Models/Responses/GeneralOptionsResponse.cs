
namespace SpendManagementApi.Models.Responses
{
    using SpendManagementApi.Models.Common;
    using SpendManagementApi.Models.Types;
    using System.Collections.Generic;

    public class GeneralOptionsResponse : GetApiResponse<GeneralOption>
    {
        /// <summary>
        /// Creates a new GeneralOptionsResponse.
        /// </summary>
        public GeneralOptionsResponse()
        {
            List = new List<GeneralOption>();
        }
    }

    /// <summary>
    /// A response containing a particular <see cref="GeneralOption">GeneralOption</see>.
    /// </summary>
    public class GeneralOptionResponse : ApiResponse<GeneralOption>
    {

    }
}