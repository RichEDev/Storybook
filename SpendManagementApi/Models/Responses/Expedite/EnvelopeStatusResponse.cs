namespace SpendManagementApi.Models.Responses.Expedite
{
    using System.Collections.Generic;
    using Common;
    using Types.Expedite;

    /// <summary>
    /// A response containing a list of <see cref="EnvelopeStatus">EnvelopeStatus</see>.
    /// </summary>
    public class GetEnvelopeStatusResponse : GetApiResponse<EnvelopeStatus>
    {
        /// <summary>
        /// Creates a new GetEnvelopeStatusResponse.
        /// </summary>
        public GetEnvelopeStatusResponse()
        {
            List = new List<EnvelopeStatus>();
        }
    }

    /// <summary>
    /// A response containing a particular <see cref="EnvelopeStatus">EnvelopeStatus</see>.
    /// </summary>
    public class EnvelopeStatusResponse : ApiResponse<EnvelopeStatus>
    {
    }

}