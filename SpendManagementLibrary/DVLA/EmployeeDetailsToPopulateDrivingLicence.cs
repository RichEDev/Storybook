namespace SpendManagementLibrary.DVLA
{
    using System;
    using System.Data;
    using Helpers;
    using Interfaces;

    /// <summary>
    /// Employees details used in fetching driving licence
    /// </summary>
    public class EmployeeDetailsToPopulateDrivingLicence
    {
        /// <summary>
        /// Driving licence number
        /// </summary>
        public string DrivingLicenceNumber { get; set; }

        /// <summary>
        /// Firstname as in driving licence
        /// </summary>
        public string DrivingLicenceFirstname { get; set; }

        /// <summary>
        /// Surname as in driving licence
        /// </summary>
        public string DrivingLicenceSurname { get; set; }

        /// <summary>
        /// Date of birth as in driving licence
        /// </summary>
        public DateTime DrivingLicenceDateOfBirth { get; set; }

        /// <summary>
        /// Sex of driving licence owner
        /// </summary>
        public int DrivingLicenceSex { get; set; }

        /// <summary>
        /// Driver id which got from consent portal
        /// </summary>
        public long DriverId { get; set; }

        /// <summary>
        /// Owner of driving licence
        /// </summary>
        public int EmployeeId { get; set; }

        /// <summary>
        /// Date on which last lookup performed agaist the DL for the employee
        /// </summary>
        public DateTime DvlaLookUpDate { get; set; }

        /// <summary>
        /// Gets employee details which will be used while populating driving licence from portal
        /// </summary>
        /// <param name="accountId">Account id of customer</param>
        /// <param name="connection">Database connection</param>
        /// <returns>Employee details</returns>
        public EmployeesToPopulateDrivingLicence GetEmployeesDetailsToPopulateDrivingLicence(int accountId, IDBConnection connection = null)
        {
            var employeeDetails = new EmployeesToPopulateDrivingLicence();

            using (var databaseConnection = connection ?? new DatabaseConnection(cAccounts.getConnectionString(accountId)))
            {
                using (var reader = databaseConnection.GetReader("GetEmployeesToPopulateDrivingLicence", CommandType.StoredProcedure))
                {
                    var drivingLicenceFirstnameOrd = reader.GetOrdinal("drivingLicenceFirstname");
                    var drivingLicenceSurnameOrd = reader.GetOrdinal("drivingLicenceSurname");
                    var drivingLicenceDateOfBirthOrd = reader.GetOrdinal("drivingLicenceDateOfBirth");
                    var drivingLicenceSexOrd = reader.GetOrdinal("drivingLicenceSex");
                    var driverIdOrd = reader.GetOrdinal("DriverId");
                    var drivingLicenceOrd = reader.GetOrdinal("DrivingLicenceNumber");
                    var employeeIdOrd = reader.GetOrdinal("employeeId");
                    var dvlalookUpDateOrd = reader.GetOrdinal("Dvlalookupdate");
                    while (reader.Read())
                    {
                        var drivingLicenceFirstname = reader.IsDBNull(drivingLicenceFirstnameOrd) == false ? reader.GetString(drivingLicenceFirstnameOrd) : string.Empty;
                        var drivingLicenceSurname = reader.IsDBNull(drivingLicenceSurnameOrd) == false ? reader.GetString(drivingLicenceSurnameOrd) : string.Empty;
                        var drivingLicenceDateOfBirth = (reader.IsDBNull(drivingLicenceDateOfBirthOrd) == false) ? reader.GetDateTime(drivingLicenceDateOfBirthOrd) : DateTime.MinValue;
                        var drivingLicenceSex = (reader.IsDBNull(drivingLicenceSexOrd) == false) ? Convert.ToInt32(reader.GetString(drivingLicenceSexOrd)) : 0;
                        long driverId = (reader.IsDBNull(driverIdOrd) == false) ? reader.GetInt32(driverIdOrd) : 0;
                        var drivingLicence = reader.IsDBNull(drivingLicenceOrd) == false ? reader.GetString(drivingLicenceOrd) : string.Empty;
                        var employeeId = (reader.IsDBNull(employeeIdOrd) == false) ? reader.GetInt32(employeeIdOrd) : 0;
                        var dvlaLookupDate = (reader.IsDBNull(dvlalookUpDateOrd) == false) ? reader.GetDateTime(dvlalookUpDateOrd) : DateTime.MinValue;
                        employeeDetails.EmployeeList.Add(
                        new EmployeeDetailsToPopulateDrivingLicence
                        {
                            DrivingLicenceFirstname = drivingLicenceFirstname,
                            DrivingLicenceSurname = drivingLicenceSurname,
                            DrivingLicenceDateOfBirth = drivingLicenceDateOfBirth,
                            DriverId = driverId,
                            DrivingLicenceNumber = drivingLicence,
                            DrivingLicenceSex = drivingLicenceSex,
                            EmployeeId = employeeId,
                            DvlaLookUpDate = dvlaLookupDate
                        });
                    }
                }
            }

            return employeeDetails;
        }
    }
}
