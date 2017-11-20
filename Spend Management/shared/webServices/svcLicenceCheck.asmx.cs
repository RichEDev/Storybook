
namespace Spend_Management.shared.webServices
{
    using System;
    using System.Web.Script.Services;
    using System.Web.Services;
    using SpendManagementLibrary;
    using SpendManagementLibrary.DVLA;
    using SpendManagementLibrary.Helpers;

    using code.DVLA;
    using EmployeesToPopulateDrivingLicence = code.DVLA.EmployeesToPopulateDrivingLicence;

    /// <summary>
    /// Summary description for svcLicenceCheck
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]

    [ScriptService]
    public class svcLicenceCheck : WebService
    {
        /// <summary>
        /// Get Security code to access DVLA Consent Portal and the driving licence information. The email is sent to employee with access details
        /// </summary>
        /// <param name="firstName">Firstname for driving licence consent</param>
        /// <param name="surname">Surname for driving licence consent</param>
        /// <param name="email">email for driving licence consent</param>
        /// <param name="drivingLicencePlate">Driving licence number for driving licence consent</param>
        /// <param name="dateOfBirth">Date of birth for driving licence consent</param>
        /// <param name="sex">Sex of the driving licence holder</param>
        /// <param name="middleName">Middlename of the driving licence holder</param>
        /// <returns>Contains details about the driver, security code to access DVLA portal,error message,portal url and custom menu id to navigate page</returns>
        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public EmployeeDvlaConsentDetails GetConsentPortalAccess(string firstName, string surname, string email, string drivingLicencePlate, string dateOfBirth, string sex, string middleName)
        {
            var user = cMisc.GetCurrentUser();
            var dvlaLookUp = new DvlaConsentLookUp();
            return dvlaLookUp.GetConsentPortalAccess(user, firstName, surname, email, drivingLicencePlate, dateOfBirth, sex, middleName);
        }

        /// <summary>
        /// Checks whether or not the provided driver has valid consent on record.
        /// </summary>
        /// <param name="driverId">The driver to check</param>
        /// <returns>true if consent is on record, false if not</returns>
        [WebMethod(EnableSession = true)]
        [ScriptMethod(UseHttpGet = true)]
        public bool HasUserProvidedConsent(int driverId)
        {
            var dvlaLookUp = new DvlaConsentLookUp();
            return dvlaLookUp.HasUserProvidedConsent(driverId);
        }

        /// <summary>
        /// Get Driving Licence information of logged in user and save.
        /// </summary>
        /// <param name="isLookupDateUpdated">Employees lookupdate has updated with reviewdate when enabling the general option or from any previous lookup</param>
        /// <returns>returns object array with driving licence error code,driving licence details and aa flag if the user has support access or not</returns>
        [WebMethod(EnableSession = true)]
        [ScriptMethod(UseHttpGet = true)]
        public object[] GetLicenceData(bool isLookupDateUpdated)
        {
            var user = cMisc.GetCurrentUser();
            var employees = new cEmployees(user.AccountID);
            var employee = employees.GetEmployeeById(user.EmployeeID);
            var employeeDetails = new EmployeesToPopulateDrivingLicence();
            var autoPopulateDrivingLicence = new AutoPopulatedDrivingLicence();
            var connection = new DatabaseConnection(cAccounts.getConnectionString(user.AccountID));
            var lookupResponseInformation = new object[3];
            employeeDetails.EmployeeList.Add(new EmployeeToPopulateDrivingLicence
            {
                DriverId = Convert.ToInt64(employee.DriverId),
                DrivingLicenceDateOfBirth = Convert.ToDateTime(employee.DrivingLicenceDateOfBirth),
                DrivingLicenceFirstname = employee.DrivingLicenceFirstname,
                DrivingLicenceNumber = employee.DrivingLicenceNumber,
                DrivingLicenceSex = Convert.ToInt32(employee.DrivingLicenceSex),
                DrivingLicenceSurname = employee.DrivingLicenceSurname,
                EmployeeId = employee.EmployeeID,
                DvlaLookUpDate = Convert.ToDateTime(employee.DvlaLookUpDate)
            });
            var drivingLicenceDetails =
                autoPopulateDrivingLicence.PopulateDrivingLicences(employeeDetails, user.AccountID, true);

            if (drivingLicenceDetails != null && drivingLicenceDetails.DrivingLicenceInformationList.Count > 0)
            {
                var record = autoPopulateDrivingLicence.AddDrivingLicenceInformation(employees, user.AccountID, drivingLicenceDetails.DrivingLicenceInformationList[0], true,Convert.ToBoolean(isLookupDateUpdated), connection);
                lookupResponseInformation[0] = drivingLicenceDetails.DrivingLicenceInformationList[0].ResponseCode;
                lookupResponseInformation[1] = this.EmployeeCanRaiseSupportTicket();
                lookupResponseInformation[2] = record;
            }
            else
            {
                lookupResponseInformation[0] = string.Empty;
                lookupResponseInformation[1] = this.EmployeeCanRaiseSupportTicket(); 
                lookupResponseInformation[2] = null;
            }
            return lookupResponseInformation;
        }
        
        /// <summary>
        /// This method checks if the employee has permission to raise a support ticket and account which the User belongs to has permission to contact service desk
        /// </summary>
        /// <returns>Returns a boolean value if the user has permission to raise support ticket</returns>
        [WebMethod(EnableSession = true)]
        public bool EmployeeCanRaiseSupportTicket()
        {
            var user = cMisc.GetCurrentUser();
            var accountProperties = new cAccountSubAccounts(user.AccountID).getSubAccountById(user.CurrentSubAccountId).SubAccountProperties;
            return (user.Account.ContactHelpDeskAllowed || accountProperties.EnableInternalSupportTickets || user.Employee.ContactHelpDeskAllowed);
        }
    }
}
