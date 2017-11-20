namespace SpendManagementApi.Models.Responses
{
    using System.Collections.Generic;
    using Common;
    using Types;

    /// <summary>
    /// A response containing a list of <see cref="OdometerReading">OdometerReading</see>s.
    /// </summary>
    public class GetOdometerReadingsResponse : GetApiResponse<OdometerReading>
    {
        /// <summary>
        /// Creates a new GetOdometerReadingsResponse.
        /// </summary>
        public GetOdometerReadingsResponse()
        {
            List = new List<OdometerReading>();
        }
    }

    /// <summary>
    /// A response containing a particular <see cref="OdometerReading">OdometerReading</see>.
    /// </summary>
    public class OdometerReadingResponse : ApiResponse<OdometerReading>
    {

    }
}
