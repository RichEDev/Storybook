namespace Spend_Management.shared.code.DVLA
{
    using System;

    /// <summary>
    /// Employee details to populate driving licence 
    /// </summary>
    public class EmployeeToPopulateDrivingLicence
    {
        /// <summary>
        /// Driving licence number
        /// </summary>
        public string DrivingLicenceNumber { get; set; }

        /// <summary>
        /// Firstname from driving licence
        /// </summary>
        public string DrivingLicenceFirstname { get; set; }

        /// <summary>
        /// Surname from driving licence
        /// </summary>
        public string DrivingLicenceSurname { get; set; }

        /// <summary>
        /// Date of birth from driving licence
        /// </summary>
        public DateTime DrivingLicenceDateOfBirth { get; set; }

        /// <summary>
        /// Sex of driving licence owner
        /// </summary>
        public int DrivingLicenceSex { get; set; }

        /// <summary>
        /// Driver id which got while providing consent
        /// </summary>
        public long DriverId { get; set; }

        /// <summary>
        /// Owner of driving licence
        /// </summary>
        public int EmployeeId { get; set; }

        /// <summary>
        /// Date on which last lookup ran agaist the DL for the employee
        /// </summary>
        public DateTime DvlaLookUpDate { get; set; }


    }
}
