using System.Collections.Generic;
using SpendManagementApi.Models.Common;
using SpendManagementApi.Models.Types;

namespace SpendManagementApi.Models.Responses
{
    /// <summary>
    /// A response containing a list of <see cref="Team">Item</see>s.
    /// </summary>
    public class GetTeamsResponse : GetApiResponse<Team>
    {
        /// <summary>
        /// Creates a new GetTeamsResponse.
        /// </summary>
        public GetTeamsResponse()
        {
            List = new List<Team>();
        }
    }

    /// <summary>
    /// A response containing a particular <see cref="Team">Team</see>.
    /// </summary>
    public class TeamResponse : ApiResponse<Team>
    {

    }
}