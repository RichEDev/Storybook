using System.Collections.Generic;
using SpendManagementApi.Models.Common;

namespace SpendManagementApi.Models.Responses
{
    using SpendManagementApi.Models.Types.Employees;

    /// <summary>
    /// A response containing a list of <see cref="UserDefinedField">UserDefinedField</see>s.
    /// </summary>
    public class GetUserDefinedFieldsResponse : GetApiResponse<UserDefinedField>
    {
        /// <summary>
        /// Creates a new GetCostCodesResponse.
        /// </summary>
        public GetUserDefinedFieldsResponse()
        {
            List = new List<UserDefinedField>();
        }
    }

    /// <summary>
    /// Defines the api response format for a <see cref="UserDefinedField">UserDefinedField</see>
    /// </summary>
    public class UserDefinedFieldResponse : ApiResponse<UserDefinedField>
    {
    }
}
