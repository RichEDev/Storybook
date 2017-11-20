using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SpendManagementApi.Models.Common;
using SpendManagementApi.Models.Types;

namespace SpendManagementApi.Models.Responses
{

    /// <summary>
    /// A response containing a list of <see cref="AddressRecommendedDistance">AddressRecommendedDistance</see>s.
    /// </summary>
    public class GetAddressRecommendedDistancesResponse : GetApiResponse<AddressRecommendedDistance>
    {
        /// <summary>
        /// Creates a new AddressRecommendedDistancesResponse.
        /// </summary>
        public GetAddressRecommendedDistancesResponse()
        {
            List = new List<AddressRecommendedDistance>();
        }
    }

    /// <summary>
    /// A response containing a particular <see cref="AddressRecommendedDistance">AddressRecommendedDistance</see>.
    /// </summary>
    public class AddressRecommendedDistanceResponse : ApiResponse<AddressRecommendedDistance>
    {

    }
}