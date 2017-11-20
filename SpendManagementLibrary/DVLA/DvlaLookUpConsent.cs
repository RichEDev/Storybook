namespace SpendManagementLibrary.DVLA
{
    using System;
    using Employees;
    using Enumerators;
    using Helpers;
    using Interfaces;

    /// <summary>
    /// Class represents method for DVLA licence check actions for the employees
    /// </summary>
    public class DvlaLookUpConsent
    {
        /// <summary>
        /// Save DVLA Consent Access Information like security code , driverid, conset date
        /// </summary>
        /// <param name="securityCode">Security Code to access the consent portal</param>
        /// <param name="consentDate">Date on which the consent request is submitted</param>
        /// <param name="driverId">Driver Id returned by the Licence Api</param>
        /// <param name="employeeId"> Employee Id of logged in user</param>
        /// <param name="accountId">Account Id of the employee</param>
        /// <param name="firstname">Firstname used in driving licence consent</param> 
        /// <param name="surname">Surname used in driving licence consent</param>
        /// <param name="dateOfBirth">Date of birth used in driving licence consent</param>
        /// <param name="sexValue">Sex of the driving licence holder</param>
        /// <param name="email">EmailId used in driving licence consent</param>
        /// <param name="drivingLicenceNumber">Licencen number of the driving licence holder</param>
        /// <param name="middleName">Middle name of the driving licence holder</param>
        /// <param name="responseCode">Response code returned by Dvla create driver api</param>
        /// <param name="connection">Db connection</param>
        public void SaveDvlaConsentAccessInformation(string securityCode, DateTime consentDate, long driverId, int employeeId, int accountId, string firstname, string surname, DateTime dateOfBirth, Gender sexValue, string email, string drivingLicenceNumber, string middleName,string responseCode, IDBConnection connection = null)
        {
            using (var databaseConnection = connection ?? new DatabaseConnection(cAccounts.getConnectionString(accountId)))
            {
                databaseConnection.sqlexecute.Parameters.Clear();
                databaseConnection.sqlexecute.Parameters.AddWithValue("@employeeId", employeeId);
                databaseConnection.sqlexecute.Parameters.AddWithValue("@driverId", driverId);
                databaseConnection.sqlexecute.Parameters.AddWithValue("@securityCode", securityCode);
                databaseConnection.sqlexecute.Parameters.AddWithValue("@consentDate", consentDate);
                databaseConnection.sqlexecute.Parameters.AddWithValue("@firstname", firstname);
                databaseConnection.sqlexecute.Parameters.AddWithValue("@surname", surname);
                databaseConnection.sqlexecute.Parameters.AddWithValue("@dateofbirth", dateOfBirth);
                databaseConnection.sqlexecute.Parameters.AddWithValue("@sex", (int)sexValue);
                databaseConnection.sqlexecute.Parameters.AddWithValue("@licenceNumber", drivingLicenceNumber);
                databaseConnection.sqlexecute.Parameters.AddWithValue("@email", email);
                databaseConnection.sqlexecute.Parameters.AddWithValue("@middleName", middleName);
                databaseConnection.sqlexecute.Parameters.AddWithValue("@responseCode", responseCode);
                databaseConnection.ExecuteProc("SaveEmployeeConsentPortalAccessInformation");
                User.CacheRemove(employeeId, accountId);
            }
        }

        /// <summary>
        /// Save the reponse information by the DVLA consent api
        /// </summary>
        /// <param name="employeeId">Id of the employee who submit the consent request</param>
        /// <param name="accountId">Account Id of the logged in user</param>
        /// <param name="responseCode">Response code returned by the Dvla Api</param>
        /// <param name="connection"><see cref="IDBConnection"/></param>
        public void SaveDvlaServiceResponseInformation(int employeeId, int accountId, string responseCode, IDBConnection connection = null)
        {
            using (var databaseConnection = connection ?? new DatabaseConnection(cAccounts.getConnectionString(accountId)))
            {
                databaseConnection.sqlexecute.Parameters.Clear();
                databaseConnection.sqlexecute.Parameters.AddWithValue("@employeeId", employeeId);
                databaseConnection.sqlexecute.Parameters.AddWithValue("@responseCode", responseCode);
                databaseConnection.ExecuteProc("SaveDvlaServiceResponseInformation");
                User.CacheRemove(employeeId, accountId);
            }
        }

        /// <summary>
        /// The delete consent after revoking.
        /// </summary>
        /// <param name="employeeId">
        /// The employee id.
        /// </param>
        /// <param name="accountId">
        /// The account id.
        /// </param>
        /// <param name="connection">
        /// The connection.
        /// </param>
        public void DeleteConsentAfterRevoking(int employeeId, int accountId, IDBConnection connection = null)
        {
            using (var databaseConnection = connection ?? new DatabaseConnection(cAccounts.getConnectionString(accountId)))
            {
                databaseConnection.sqlexecute.Parameters.Clear();
                databaseConnection.sqlexecute.Parameters.AddWithValue("@employeeId", employeeId);
                databaseConnection.ExecuteProc("DeleteConsentDetailsAfterRevokingTheConsent");
                User.CacheRemove(employeeId, accountId);
            }
        }
     }
}
