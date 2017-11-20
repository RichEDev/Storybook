namespace SpendManagementLibrary.DVLA
{
    using System;
    using Helpers;
    using System.Data;

    /// <summary>
    /// Class represents method for Dvla Manual intervention lookup
    /// </summary>
    public class EmployeesWithManualIntervention
    {
        /// <summary>
        /// Gets employee details whose lookup failed previously because mandate was not provided.The licence lookup need to run against these employees to check if they have provided a manual intervention
        /// </summary>
        /// <param name="accountId">Account id of customer</param>
        /// <returns>Employee details</returns>
        public EmployeesToPopulateDrivingLicence GetEmployeeWithManualIntervention(int accountId)
        {
            var employeeDetails = new EmployeesToPopulateDrivingLicence();

            using (var connection = new DatabaseConnection(cAccounts.getConnectionString(accountId)))
            {
                using (var reader = connection.GetReader("GetEmployeesWithManualIntervention", CommandType.StoredProcedure))
                {
                    var drivingLicenceFirstnameOrd = reader.GetOrdinal("drivingLicenceFirstname");
                    var drivingLicenceSurnameOrd = reader.GetOrdinal("drivingLicenceSurname");
                    var drivingLicenceDateOfBirthOrd = reader.GetOrdinal("drivingLicenceDateOfBirth");
                    var drivingLicenceSexOrd = reader.GetOrdinal("drivingLicenceSex");
                    var driverIdOrd = reader.GetOrdinal("DriverId");
                    var drivingLicenceOrd = reader.GetOrdinal("DrivingLicenceNumber");
                    var employeeIdOrd = reader.GetOrdinal("employeeId");
                    while (reader.Read())
                    {
                        var drivingLicenceFirstname = reader.IsDBNull(drivingLicenceFirstnameOrd) == false ? reader.GetString(drivingLicenceFirstnameOrd) : string.Empty;
                        var drivingLicenceSurname = reader.IsDBNull(drivingLicenceSurnameOrd) == false ? reader.GetString(drivingLicenceSurnameOrd) : string.Empty;
                        var drivingLicenceDateOfBirth = (reader.IsDBNull(drivingLicenceDateOfBirthOrd) == false) ? reader.GetDateTime(drivingLicenceDateOfBirthOrd) : DateTime.MinValue;
                        var drivingLicenceSex = (reader.IsDBNull(drivingLicenceSexOrd) == false) ? Convert.ToInt32(reader.GetString(drivingLicenceSexOrd)) : 0;
                        long driverId = (reader.IsDBNull(driverIdOrd) == false) ? reader.GetInt32(driverIdOrd) : 0;
                        var drivingLicence = reader.IsDBNull(drivingLicenceOrd) == false ? reader.GetString(drivingLicenceOrd) : string.Empty;
                        var employeeId = (reader.IsDBNull(employeeIdOrd) == false) ? reader.GetInt32(employeeIdOrd) : 0;
                        employeeDetails.EmployeeList.Add(
                        new EmployeeDetailsToPopulateDrivingLicence
                        {
                            DrivingLicenceFirstname = drivingLicenceFirstname,
                            DrivingLicenceSurname = drivingLicenceSurname,
                            DrivingLicenceDateOfBirth = drivingLicenceDateOfBirth,
                            DriverId = driverId,
                            DrivingLicenceNumber = drivingLicence,
                            DrivingLicenceSex = drivingLicenceSex,
                            EmployeeId = employeeId
                        });
                    }
                }
            }
            return employeeDetails;
        }

    }
}
