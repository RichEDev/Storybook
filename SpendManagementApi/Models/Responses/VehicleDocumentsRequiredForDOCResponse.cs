namespace SpendManagementApi.Models.Responses
{
    using System.Collections.Generic;

    using SpendManagementApi.Models.Common;
    using SpendManagementApi.Models.Types;

    /// <summary>
    /// The results of the duty of care documents required for adding a new vehicle
    /// </summary>
    public class VehicleDocumentsRequiredForDOCResponse : ApiResponse

    {

        /// <summary>
        /// The list of <see cref="DutyOfCareDocument">DocumentExpiry</see>
        /// </summary>
        public List<DutyOfCareDocument> List;
    }
}
