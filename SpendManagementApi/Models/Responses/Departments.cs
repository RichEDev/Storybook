using System.Collections.Generic;
using SpendManagementApi.Models.Common;
using SpendManagementApi.Models.Types;

namespace SpendManagementApi.Models.Responses
{
    /// <summary>
    /// A response containing a list of <see cref="Department">Department</see>s.
    /// </summary>
    public class GetDepartmentsResponse : GetApiResponse<Department>
    {
        /// <summary>
        /// Creates a new GetDepartmentsResponse.
        /// </summary>
        public GetDepartmentsResponse()
        {
            List = new List<Department>();
        }
    }

    /// <summary>
    /// A response containing a particular <see cref="Department">Department</see>.
    /// </summary>
    public class DepartmentResponse : ApiResponse<Department>
    {

    }
}
