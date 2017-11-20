namespace SpendManagementApi.Models.Requests
{
    using System.Collections.Generic;
    using SpendManagementLibrary.DVLA;

    /// <summary>
    /// Request for save drivinglicence information
    /// </summary>
    public class DrivingLicenceRequest
    {
        /// <summary>
        /// Driving licence details
        /// </summary>
        public List<DrivingLicenceDetailsResponse> DrivingLicenceDetails = new List<DrivingLicenceDetailsResponse>();
    }
}