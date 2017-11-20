namespace SpendManagementApi.Models.Responses
{
    using System.Collections.Generic;

    using Common;
    using Types;

    /// <summary>
    /// A response containing a list of <see cref="Organisation">Organisation</see>s.
    /// </summary>
    public class GetOrganisationsResponse : GetApiResponse<Organisation>
    {
        /// <summary>
        /// Creates a new GetOrganisationsResponse.
        /// </summary>
        public GetOrganisationsResponse()
        {
            List = new List<Organisation>();
        }
    }

    /// <summary>
    /// A response containing a particular <see cref="Organisation">Organisation</see>.
    /// </summary>
    public class OrganisationResponse : ApiResponse<Organisation>
    {
    }
}
