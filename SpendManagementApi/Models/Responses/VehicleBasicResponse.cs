namespace SpendManagementApi.Models.Responses
{
    using System.Collections.Generic;

    using SpendManagementApi.Models.Common;

    /// <summary>
    /// The vehicle basic response.
    /// </summary>
    public class VehicleBasicResponse : ApiResponse
    {
        /// <summary>
        /// Gets or sets the list of <see cref="VehicleBasic">VehicleBasic</see>
        /// </summary>
        public List<VehicleBasic> List { get; set; }
    }
}