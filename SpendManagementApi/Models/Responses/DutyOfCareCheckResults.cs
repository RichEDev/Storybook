namespace SpendManagementApi.Models.Responses
{
    using System.Collections.Generic;

    using SpendManagementApi.Models.Common;
    using SpendManagementApi.Models.Types;

    /// <summary>
    /// The results of the duty of care checks
    /// </summary>
    public class DutyOfCareCheckResults : ApiResponse

    {
        /// <summary>
        /// The list of <see cref="DocumentExpiry">DocumentExpiry</see>
        /// </summary>
        public List<DocumentExpiry> List;
    }
}