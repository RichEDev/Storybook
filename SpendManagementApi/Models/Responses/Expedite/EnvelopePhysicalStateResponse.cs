namespace SpendManagementApi.Models.Responses.Expedite
{
    using System.Collections.Generic;
    using Common;
    using Types.Expedite;

    /// <summary>
    /// A response containing a list of <see cref="EnvelopeType">EnvelopeType</see>s.
    /// </summary>
    public class GetEnvelopePhysicalStatesResponse : GetApiResponse<EnvelopePhysicalState>
    {
        /// <summary>
        /// Creates a new GetEnvelopeTypeResponse.
        /// </summary>
        public GetEnvelopePhysicalStatesResponse()
        {
            this.List = new List<EnvelopePhysicalState>();
        }


    }

    /// <summary>
    /// A response containing a particular <see cref="EnvelopeType">EnvelopeType</see>.
    /// </summary>
    public class EnvelopePhysicalStateResponse : ApiResponse<EnvelopePhysicalState>
    {
    }

}