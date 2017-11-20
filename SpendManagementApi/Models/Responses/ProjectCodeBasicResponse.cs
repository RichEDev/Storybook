namespace SpendManagementApi.Models.Responses
{
    using System.Collections.Generic;

    using SpendManagementApi.Models.Common;

    /// <summary>
    /// The project code basic response.
    /// </summary>
    public class ProjectCodeBasicResponse : ApiResponse
    {
        /// <summary>
        /// Gets or sets a list of <see cref="DepartmentBasic">DepartmentBasic</see>
        /// </summary>
        public List<ProjectCodeBasic> List { get; set; }
    }
}