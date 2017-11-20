namespace SpendManagementApi.Models.Responses
{
    using System.Collections.Generic;

    using SpendManagementApi.Models.Common;
    using SpendManagementApi.Models.Types;

    /// <summary>
    /// The sorn vehicles response.
    /// </summary>
    public class SornVehiclesResponse : ApiResponse
    {
        /// <summary>
        /// Gets or sets the list of <see cref="SornVehicle">SornVehicle</see>.
        /// </summary>
        public List<SornVehicle> List { get; set; }
    }
}