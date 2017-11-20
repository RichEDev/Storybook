namespace SpendManagementApi.Models.Responses.Expedite
{
    using System.Collections.Generic;
    using Common;
    using Types.Expedite;

    /// <summary>
    /// A response containing a list of <see cref="EnvelopeType">EnvelopeType</see>s.
    /// </summary>
    public class GetEnvelopeTypeResponse : GetApiResponse<EnvelopeType>
    {
        /// <summary>
        /// Creates a new GetEnvelopeTypeResponse.
        /// </summary>
        public GetEnvelopeTypeResponse()
        {
            List = new List<EnvelopeType>();
        }
    }

    /// <summary>
    /// A response containing a particular <see cref="EnvelopeType">EnvelopeType</see>.
    /// </summary>
    public class EnvelopeTypeResponse : ApiResponse<EnvelopeType>
    {
    }

}