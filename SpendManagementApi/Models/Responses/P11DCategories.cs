using System.Collections.Generic;
using SpendManagementApi.Models.Common;
using SpendManagementApi.Models.Types;

namespace SpendManagementApi.Models.Responses
{
    /// <summary>
    /// A response that contains a list of zero to many <see cref="P11DCategory">P11D Categories</see>
    /// </summary>
    public class GetP11DCategoriesResponse : GetApiResponse<P11DCategory>
    {
    }

    /// <summary>
    /// A response that contains a <see cref="P11DCategory">P11D Category</see>
    /// </summary>
    public class P11DCategoryResponse : ApiResponse<P11DCategory>
    {
    }
}
