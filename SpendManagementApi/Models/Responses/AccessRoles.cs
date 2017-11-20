using System.Collections;
using System.Collections.Generic;
using SpendManagementApi.Models.Common;
using SpendManagementApi.Models.Types;

namespace SpendManagementApi.Models.Responses
{
    /// <summary>
    /// A response containing a list of <see cref="AccessRole">AccessRole</see>s.
    /// </summary>
    public class GetAccessRolesResponse : GetApiResponse<AccessRole>
    {
        /// <summary>
        /// Creates a new AccessRolesResponse.
        /// </summary>
        public GetAccessRolesResponse()
        {
            List = new List<AccessRole>();
        }
    }

    /// <summary>
    /// A response containing a particular <see cref="AccessRole">AccessRole</see>.
    /// </summary>
    public class AccessRoleResponse : ApiResponse<AccessRole>
    {

    }
}