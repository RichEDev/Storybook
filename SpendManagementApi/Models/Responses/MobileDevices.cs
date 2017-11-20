using System.Collections.Generic;
using SpendManagementApi.Models.Common;
using SpendManagementApi.Models.Types;

namespace SpendManagementApi.Models.Responses
{
    /// <summary>
    /// A response containing a list of <see cref="MobileDevice">MobileDevice</see>s.
    /// </summary>
    public class GetMobileDevicesResponse : GetApiResponse<MobileDevice>
    {
        /// <summary>
        /// Creates a new GetMobileDevicesResponse.
        /// </summary>
        public GetMobileDevicesResponse()
        {
            List = new List<MobileDevice>();
        }
    }

    /// <summary>
    /// A response containing a particular <see cref="MobileDevice">MobileDevice</see>.
    /// </summary>
    public class MobileDeviceResponse : ApiResponse<MobileDevice>
    {

    }
}
