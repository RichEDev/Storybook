namespace SpendManagementApi.Models.Requests
{
    using SpendManagementApi.Models.Common;
    using SpendManagementApi.Models.Types;

    /// <summary>
    /// The Esr Assignment request.
    /// </summary>
    public class EsrAssignmentRequest : ApiRequest
    {
        /// <summary>
        /// Gets or sets the ESR assignment.
        /// </summary>
        public ESRAssignments EsrAssignment { get; set; }
    }
}