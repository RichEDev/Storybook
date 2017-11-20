namespace SpendManagementApi.Models.Responses
{
    using System.Collections.Generic;

    using SpendManagementApi.Models.Common;

    /// <summary>
    /// The department basic response.
    /// </summary>
    public class DepartmentBasicResponse : ApiResponse
    {
        /// <summary>
        /// Gets or sets a list of <see cref="DepartmentBasic">DepartmentBasic</see>
        /// </summary>
        public List<DepartmentBasic> List { get; set; }
    }
}