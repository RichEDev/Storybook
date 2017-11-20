namespace SpendManagementApi.Models.Types
{
    using System.Collections.Generic;

    using SpendManagementLibrary.Employees.DutyOfCare;

    /// <summary>
    /// Driving licence review request class
    /// </summary>
    public class DrivingLicenceReviewRequest
    {
        /// <summary>
        /// Gets or sets the driving licence details.
        /// </summary>
        public List<DrivingLicenceReviewDetails> DrivingLicenceDetails { get; set; }
    }
}