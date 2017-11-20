namespace SpendManagementApi.Models.Requests
{
    using System.Collections.Generic;

    using SpendManagementApi.Models.Common;

    /// <summary>
    /// The assign claims request.
    /// </summary>
    public class AssignClaimsRequest : ApiRequest
    {
        /// <summary>
        /// Gets or sets the claim ids.
        /// </summary>
        public List<int> ClaimIds { get; set; }
    }
}