using System.Collections.Generic;
using SpendManagementApi.Models.Common;
using SpendManagementApi.Models.Types;

namespace SpendManagementApi.Models.Responses
{
    /// <summary>
    /// A response containing a list of <see cref="CostCode">CostCode</see>s.
    /// </summary>
    public class GetCostCodesResponse : GetApiResponse<CostCode>
    {
        /// <summary>
        /// Creates a new GetCostCodesResponse.
        /// </summary>
        public GetCostCodesResponse()
        {
            List = new List<CostCode>();
        }
    }

    /// <summary>
    /// A response containing a particular <see cref="CostCode">CostCode</see>.
    /// </summary>
    public class CostCodeResponse : ApiResponse<CostCode>
    {
    }
}
