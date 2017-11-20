namespace DutyOfCareAPI.DutyOfCare
{
    using System.Collections.Generic;

    /// <summary>
    /// Response with list of driving licence details
    /// </summary>
    public class DrivingLicenceResponse
    {
        /// <summary>
        /// List of driving licence details
        /// </summary>
        public List<DrivingLicenceDetailsResponse> DrivingLicenceDetails = new List<DrivingLicenceDetailsResponse>();
    }
}