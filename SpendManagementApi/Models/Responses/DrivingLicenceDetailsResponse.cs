namespace SpendManagementApi.Models.Responses
{
    using System.Collections.Generic;
    using Common;
    using SpendManagementLibrary.DVLA;
    using Types;

    /// <summary>
    /// List of driving licence details
    /// </summary>
    public class DrivingLicenceResponse : ApiResponse<DrivingLicenceDetails>
    {
        /// <summary>
        /// List of driving licence
        /// </summary>
        public List<DrivingLicenceDetailsResponse> DrivingLicenceDetails = new List<DrivingLicenceDetailsResponse>();
    }
}