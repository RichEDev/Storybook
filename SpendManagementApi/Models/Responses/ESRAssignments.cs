namespace SpendManagementApi.Models.Responses
{
    using System.Collections.Generic;
    using Common;
    using Types;

    /// <summary>
    /// A response containing a list of <see cref="ESRAssignments">Item</see>s.
    /// </summary>
    public class GetESRResponse : GetApiResponse<ESRAssignments>
    {
        /// <summary>
        /// Creates a new GetESRAssignmentsResponse.
        /// </summary>
        public GetESRResponse()
        {
            List = new List<ESRAssignments>();
        }
    }

    /// <summary>
    /// A response containing a particular <see cref="ESRAssignments">ESRAssignments</see>.
    /// </summary>
    public class ESRResponse : ApiResponse<ESRAssignments>
    {

    }
}