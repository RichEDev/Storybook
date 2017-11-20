namespace SpendManagementApi.Models.Responses
{
    using System.Collections.Generic;
    using Common;
    using Types;

    /// <summary>
    /// A response containing a list of <see cref="Vehicle">Vehicle</see>s.
    /// </summary>
    public class GetVehiclesResponse : GetApiResponse<Vehicle>
    {
        /// <summary>
        /// Creates a new GetVehiclesResponse.
        /// </summary>
        public GetVehiclesResponse()
        {
            List = new List<Vehicle>();
        }
    }

    /// <summary>
    /// A response containing a particular <see cref="Vehicle">Vehicle</see>.
    /// </summary>
    public class VehicleResponse : ApiResponse<Vehicle>
    {

    }
}