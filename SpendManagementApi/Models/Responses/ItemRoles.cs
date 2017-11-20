using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SpendManagementApi.Models.Common;
using SpendManagementApi.Models.Types;

namespace SpendManagementApi.Models.Responses
{
    /// <summary>
    /// Get Item Roles Response
    /// </summary>
    public class GetItemRolesResponse : GetApiResponse<ItemRole>
    {
        /// <summary>
        /// Creates a new, empty GetItemRolesResponse.
        /// </summary>
        public GetItemRolesResponse()
        {
            List = new List<ItemRole>();
        }
    }

    /// <summary>
    /// A response containing a particular <see cref="ItemRole">ItemRole</see>.
    /// </summary>
    public class ItemRoleResponse : ApiResponse<ItemRole>
    {
    }
}