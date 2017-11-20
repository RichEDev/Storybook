using System.Collections.Generic;
using SpendManagementApi.Models.Common;
using SpendManagementApi.Models.Types;

namespace SpendManagementApi.Models.Responses
{
    /// <summary>
    /// A response containing a list of <see cref="ClaimReason">ClaimReason</see>s.
    /// </summary>
    public class GetClaimReasonsResponse : GetApiResponse<ClaimReason>
    {
        /// <summary>
        /// Creates a new GetClaimReasonsResponse.
        /// </summary>
        public GetClaimReasonsResponse()
        {
            List = new List<ClaimReason>();
        }
    }

    /// <summary>
    /// A response containing a particular <see cref="ClaimReason">ClaimReason</see>.
    /// </summary>
    public class ClaimReasonResponse : ApiResponse<ClaimReason>
    {

    }
}
