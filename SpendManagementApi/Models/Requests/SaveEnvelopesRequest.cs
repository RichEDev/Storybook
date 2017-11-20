namespace SpendManagementApi.Models.Requests
{
    using System.Collections.Generic;
    using SpendManagementApi.Models.Common;

    /// <summary>
    /// The save envelopes request.
    /// </summary>
    public class SaveEnvelopesRequest : ApiRequest
    {
        /// <summary>
        /// Gets or sets the claim id.
        /// </summary>
        public int ClaimId { get; set; }

        /// <summary>
        /// Gets or sets a list of <see cref="ClaimEnvelopeInfo">ClaimEnvelopeInfo</see>.
        /// </summary>
        public List<ClaimEnvelopeInfo> ClaimEnvelopes { get; set; }      
    }
}