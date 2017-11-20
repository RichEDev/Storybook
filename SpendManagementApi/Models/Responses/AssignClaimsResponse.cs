namespace SpendManagementApi.Models.Responses
{
    using System.Collections.Generic;
    using SpendManagementApi.Models.Common;

    /// <summary>
    /// The assign claims response.
    /// </summary>
    public class AssignClaimsResponse : ApiResponse<AssignClaim>
    {
        /// <summary>
        /// Gets or sets the list of <see cref="AssignClaim">AssignClaim</see>.
        /// </summary>
        public List<AssignClaim> List { get; set; }
    }
}