namespace SpendManagementApi.Models.Types
{
    using System;
    using SpendManagementLibrary.DVLA;

    /// <summary>
    /// Gives employee details to fetch driving licence
    /// </summary>
    public class EmployeeToPopulateDrivingLicence
    {
        /// <summary>
        /// Licence number
        /// </summary>
        public string DrivingLicenceNumber { get; set; }

        /// <summary>
        /// Firstname as in licence
        /// </summary>
        public string DrivingLicenceFirstname { get; set; }

        /// <summary>
        /// Surname as in licence
        /// </summary>
        public string DrivingLicenceSurname { get; set; }

        /// <summary>
        /// Date of birth as in licence
        /// </summary>
        public DateTime DrivingLicenceDateOfBirth { get; set; }

        /// <summary>
        /// Gender
        /// </summary>
        public int DrivingLicenceSex { get; set; }

        /// <summary>
        /// Driver id which we get while providing consent
        /// </summary>
        public long DriverId { get; set; }

        /// <summary>
        /// Employeeid 
        /// </summary>
        public int EmployeeId { get; set; }

        /// <summary>
        /// Constructor for EmployeeDrivingLicenceDetails class
        /// </summary>
        /// <param name="details"> employee details</param>
        public EmployeeToPopulateDrivingLicence(EmployeeDetailsToPopulateDrivingLicence details)
        {
            this.DrivingLicenceNumber = details.DrivingLicenceNumber;
            this.DrivingLicenceFirstname = details.DrivingLicenceFirstname;
            this.DrivingLicenceSurname = details.DrivingLicenceSurname;
            this.DrivingLicenceDateOfBirth = details.DrivingLicenceDateOfBirth;
            this.DrivingLicenceSex = details.DrivingLicenceSex;
            this.DriverId = details.DriverId;
            this.EmployeeId = details.EmployeeId;
        }
    }
}