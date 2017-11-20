namespace ApiClientHelper.Responses
{
    using System;

    /// <summary>
    /// The employee details to populate driving licence.
    /// </summary>
    public class EmployeeDetailsToPopulateDrivingLicence
    {
        /// <summary>
        /// Gets or sets driving licence number
        /// </summary>
        public string DrivingLicenceNumber { get; set; }

        /// <summary>
        /// Gets or sets firstname as in driving licence
        /// </summary>
        public string DrivingLicenceFirstname { get; set; }

        /// <summary>
        /// Gets or sets surname as in driving licence
        /// </summary>
        public string DrivingLicenceSurname { get; set; }

        /// <summary>
        /// Gets or sets date of birth as in driving licence
        /// </summary>
        public DateTime DrivingLicenceDateOfBirth { get; set; }
        //{
        //    get
        //    {
        //      //  return DateTime.SpecifyKind(DrivingLicenceDateOfBirth, DateTimeKind.Utc); 
        //    };
        //    set
        //    {
        //       // var unspecified = new DateTime(2016, 12, 12, 10, 10, 10, DateTimeKind.Unspecified);
        //       // var specified = DateTime.SpecifyKind(value, DateTimeKind.Utc);
        //       // DrivingLicenceDateOfBirth = DateTime.SpecifyKind(value, DateTimeKind.Utc); 
        //    };
        //}

        /// <summary>
        /// Gets or sets sex of driving licence owner
        /// </summary>
        public int DrivingLicenceSex { get; set; }

        /// <summary>
        /// Gets or sets driver id which got from consent portal
        /// </summary>
        public long DriverId { get; set; }

        /// <summary>
        /// Gets or sets owner of driving licence
        /// </summary>
        public int EmployeeId { get; set; }
    }
}