namespace SpendManagementApi.Models.Responses
{
    using System.Collections.Generic;

    using SpendManagementApi.Models.Common;
    using SpendManagementApi.Models.Types;

    /// <summary>
    /// The esr assignments basic response.
    /// </summary>
    public class ESRAssignmentsBasicResponse : ApiResponse

{
        /// <summary>
        /// Gets or sets the list of <see cref="ESRAssignmentBasic">ESRAssignmentBasic</see>.
        /// </summary>
        public List<ESRAssignmentBasic> EsrAssignments { get; set; }
}
}