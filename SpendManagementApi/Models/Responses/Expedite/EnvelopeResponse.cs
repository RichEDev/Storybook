namespace SpendManagementApi.Models.Responses.Expedite
{
    using System.Collections.Generic;
    using Common;
    using Types.Expedite;

    /// <summary>
    /// A response containing a list of <see cref="Envelope">Envelope</see>s.
    /// </summary>
    public class GetEnvelopesResponse : GetApiResponse<Envelope>
    {
        /// <summary>
        /// Creates a new GetEnvelopesResponse.
        /// </summary>
        public GetEnvelopesResponse()
        {
            List = new List<Envelope>();
        }
    }

    /// <summary>
    /// A response containing a particular <see cref="Envelope">Envelope</see>.
    /// </summary>
    public class EnvelopeResponse : ApiResponse<Envelope>
    {
    }

}