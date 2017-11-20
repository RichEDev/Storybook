using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SpendManagementApi.Models.Common;
using SpendManagementApi.Models.Types;

namespace SpendManagementApi.Models.Responses
{
    /// <summary>
    /// Get Sign Off Groups Response
    /// </summary>
    public class GetSignOffGroupsResponse : GetApiResponse<SignOffGroup>
    {
    }

    /// <summary>
    /// Find Sign Off Groups Response
    /// </summary>
    public class FindSignOffGroupsResponse : GetSignOffGroupsResponse
    {
    }

    /// <summary>
    /// Mileage Category Response
    /// </summary>
    public class SignOffGroupResponse : ApiResponse<SignOffGroup>
    {   
    }
}