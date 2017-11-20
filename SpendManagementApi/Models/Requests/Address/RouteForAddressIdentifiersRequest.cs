namespace SpendManagementApi.Models.Requests.Address
{
    using System.Collections.Generic;

    using SpendManagementApi.Models.Common;

    /// <summary>
    /// The route for address identifiers request.
    /// </summary>
    public class RouteForAddressIdentifiersRequest : ApiRequest
    {
        /// <summary>
        /// Gets or sets the address identifiers.
        /// </summary>
        public List<int> AddressIdentifiers { get; set; }

        /// <summary>
        /// Gets or sets the claim employee Id.
        /// </summary>
        public int ClaimEmployeeId { get; set; }
    }
}