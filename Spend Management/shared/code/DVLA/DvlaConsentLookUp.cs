namespace Spend_Management.shared.code.DVLA
{
    using System;
    using System.Diagnostics;
    using DutyOfCareAPI.DutyOfCare;
    using SpendManagementLibrary;
    using SpendManagementLibrary.DVLA;
    using SpendManagementLibrary.Enumerators;
    using Spend_Management.Bootstrap;
    using Spend_Management.expenses.code;
    using System.Configuration;

    using BusinessLogic.Modules;

    using SpendManagementLibrary.Employees;
    using SpendManagementLibrary.Helpers;

    /// <summary>
    /// The dvla consent look up.
    /// </summary>
    public class DvlaConsentLookUp
    {
        /// <summary>
        /// Get Security code to access DVLA Consent Portal and the driving licence information. The email is sent to employee with access details
        /// </summary>
        /// <param name="user">Current user instance</param>
        /// <param name="firstName">Firstname for driving licence consent</param>
        /// <param name="surname">Surname for driving licence consent</param>
        /// <param name="email">email for driving licence consent</param>
        /// <param name="drivingLicencePlate">Driving licence number for driving licence consent</param>
        /// <param name="dateOfBirth">Date of birth for driving licence consent</param>
        /// <param name="sex">Sex of the driving licence holder</param>
        /// <param name="middleName">Middle name of the driving licence holder</param>
        /// <returns>Contains details about the driver, security code to access DVLA portal,error message and portal url</returns>
        public EmployeeDvlaConsentDetails GetConsentPortalAccess(CurrentUser user, string firstName, string surname, string email, string drivingLicencePlate, string dateOfBirth, string sex, string middleName)
        {
            var employeeConsentDetails = new EmployeeDvlaConsentDetails();
            var account = user.Account;

            if (account.DvlaLookUpLicenceKey == string.Empty)
            {
                employeeConsentDetails.ResponseCode = "000";
                return employeeConsentDetails;
            }

            IDutyOfCareApi dvlaApi = BootstrapDvla.CreateNew();
            var response = dvlaApi.GetConsentPortalAccess(account.DvlaLookUpLicenceKey, firstName, surname, drivingLicencePlate, email,middleName, sex, Convert.ToDateTime(dateOfBirth));
            var employee = user.Employee;
            var oldDriverId = employee.DriverId == null ? 0 : Convert.ToInt64(employee.DriverId);
            var oldSecurityCode = employee.SecurityCode == Guid.Empty ? string.Empty : employee.SecurityCode.ToString();
            var oldDrivingLicenceNumber = string.IsNullOrEmpty(employee.DrivingLicenceNumber) ? string.Empty : employee.DrivingLicenceNumber;
            var recipients = new int[1];
            recipients[0] = user.EmployeeID;
            var dvlaLookUpConsent = new DvlaLookUpConsent();

            if (response == null)
            {
                employeeConsentDetails.ResponseCode = "000";
                return employeeConsentDetails;
            }

            employeeConsentDetails.DriverId = response.DriverId;
            employeeConsentDetails.SecurityCode = response.SecurityCode ?? string.Empty;
            employeeConsentDetails.ResponseCode = response.ResponseCode;
            employeeConsentDetails.LicencePortalUrl = dvlaApi.LicencePortalUrl;
            // LookupDate is updated with Manual review date when the autolookup is enabled || Consent Expired and user submit the consent again (employee has a lookupdate recorded from last Lookup run)
            employeeConsentDetails.EmployeeLookUpDateHasUpdated =((employee.DvlaConsentDate == null && employee.DvlaLookUpDate != null) ||(employee.DvlaConsentDate != null && employee.AgreeToProvideConsent.HasValue == false));
            NotificationTemplates notifications;

            if (string.IsNullOrEmpty(response.ResponseCode) && response.SecurityCode != string.Empty && response.SecurityCode != new Guid().ToString() && response.DriverId > 0 && oldSecurityCode != response.SecurityCode)
            {
                dvlaLookUpConsent.SaveDvlaConsentAccessInformation(employeeConsentDetails.SecurityCode, DateTime.Now, employeeConsentDetails.DriverId, user.EmployeeID, user.AccountID, firstName, surname, Convert.ToDateTime(dateOfBirth), (Gender)Convert.ToInt32(sex), email, drivingLicencePlate, middleName,string.Empty);
            }
            else if (!string.IsNullOrEmpty(response.ResponseCode) && response.ResponseCode == "312")
            {
                // If it's a 312 then they've already got a record on the portal and must have revoked and then given consent again. If this is the case then take the key from their employee record rather than the API response
                dvlaLookUpConsent.SaveDvlaConsentAccessInformation(employee.SecurityCode.ToString(), DateTime.Now, employeeConsentDetails.DriverId, user.EmployeeID, user.AccountID, firstName, surname, Convert.ToDateTime(dateOfBirth), (Gender)Convert.ToInt32(sex), email, drivingLicencePlate, middleName, response.ResponseCode);
            }
            else if(!string.IsNullOrEmpty(response.ResponseCode))
            {
                dvlaLookUpConsent.SaveDvlaServiceResponseInformation(user.EmployeeID, user.AccountID, response.ResponseCode);
            }

            if (oldDriverId > 0 && string.IsNullOrEmpty(response.SecurityCode) && !string.IsNullOrEmpty(oldSecurityCode))
            {
                if (response.ResponseCode == "312" && drivingLicencePlate== oldDrivingLicenceNumber)
                {
                    employeeConsentDetails.SecurityCode = string.IsNullOrEmpty(response.SecurityCode) ? ((string.IsNullOrEmpty(oldSecurityCode) ? string.Empty : oldSecurityCode)) : string.Empty;
                }
                else if(response.ResponseCode == "313"|| response.ResponseCode == "314" || response.ResponseCode == "401" || response.ResponseCode == "301")
                {
                    employeeConsentDetails.SecurityCode = string.Empty;
                }
               
                employeeConsentDetails.ResponseCode = response.ResponseCode;
            }

            if (!string.IsNullOrEmpty(employeeConsentDetails.ResponseCode))
            {
                notifications = new NotificationTemplates(user, ConfigurationManager.AppSettings["SupportEmailAddress"]);
                DvlaServiceEventLogAndEmailNotification.DvlaLogEntry(response.ResponseCode, response.ResponseMessage, account.companyid, "Consent Submission", notifications, employee);
                if (!(response.ResponseCode == "312" && drivingLicencePlate == oldDrivingLicenceNumber))
                {
                    return employeeConsentDetails;
                }
            }

            if (!string.IsNullOrEmpty(employeeConsentDetails.SecurityCode))
            {
                notifications = new NotificationTemplates(user, email);
                notifications.SendMessage(
                    string.IsNullOrEmpty(response.ResponseCode)
                        ? new Guid("61C92AE2-2FAA-4E81-9B7A-71F25AE32936")
                        : new Guid("A7BB1638-8368-4814-9DB7-3754BE55DBDB"),
                    user.EmployeeID,
                    recipients);
            }

            return employeeConsentDetails;
        }


        /// <summary>
        /// Sends an email to the dofc approver when an user denies or opts out from lookup consent.
        /// </summary>
        /// <param name="user">
        /// The currently logged in user.
        /// </param>
        public static void NotifyApproverOnDenyOfConsent(CurrentUser user)
        {
            var subAccounts = new cAccountSubAccounts(user.AccountID);
            var subAccount = subAccounts.getFirstSubAccount();
            var accountProperties = subAccount.SubAccountProperties;
            var notifications = new NotificationTemplates(user.AccountID, user.EmployeeID, string.Empty, 0, Modules.Expenses);
            var teams = new cTeams(user.AccountID, null);
            int[] recipientsId;
            if (accountProperties.DutyOfCareApprover.ToUpper() != "LINE MANAGER")
            {
                var teamDetails = teams.GetTeamById(int.Parse(accountProperties.DutyOfCareTeamAsApprover));
                recipientsId = teamDetails.teammembers.ToArray();
            }
            else
            {
                recipientsId = new[] { Employee.Get(user.EmployeeID, user.AccountID).LineManager };
            }

            notifications.SendMessage(user.Employee.DvlaConsentDate != null ? SendMessageEnum.GetEnumDescription(SendMessageDescription.SentToApproverWhenClaimantOptsOutOfConsentCheck) : SendMessageEnum.GetEnumDescription(SendMessageDescription.SentToApproverWhenClaimantDeniesConsentCheck), user.EmployeeID, recipientsId);
        }

        /// <summary>
        /// Deny dvla consent for logged in user.
        /// </summary>
        /// <param name="currentUser">
        /// The current user.
        /// </param>
        /// <param name="databaseConnection">
        /// The database connection.
        /// </param>
        public static void DenyDvlaConsent(ICurrentUser currentUser, DatabaseConnection databaseConnection)
        {
                using (databaseConnection)
                {
                    databaseConnection.sqlexecute.Parameters.Clear();
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@employeeId", currentUser.EmployeeID);
                    databaseConnection.ExecuteProc("DenyDvlaConsent");
                    User.CacheRemove(currentUser.EmployeeID, currentUser.AccountID);
                }
        }

        /// <summary>
        /// Returns dvla lookup frenquency for information message
        /// </summary>
        /// <param name="drivingLicenceLookupFrequency">
        /// account properties object
        /// </param>
        /// <returns>
        /// The months value for information message<see cref="string"/>.
        /// </returns>
        public static string BuildFrequencyMessage(string drivingLicenceLookupFrequency)
        {
            var frequencyInMonths = int.Parse(drivingLicenceLookupFrequency) > 1 ? "every " + drivingLicenceLookupFrequency + " months " : "every month ";
            return string.IsNullOrEmpty(drivingLicenceLookupFrequency) ? "at the frequency set by your administrator" : frequencyInMonths;
        }


        /// <summary>
        /// Send email to Approver on claimants Duty Of Care documents expiry.
        /// </summary>
        /// <param name="account"> The <see cref="cAccount"/> </param>
        /// <param name="subAccount"> The <see cref="cAccountSubAccount"/> </param>
        /// <returns>
        /// flag indicating success/failure of notification
        /// </returns>
        public string NotifyUserOnExpiryOfConsent(cAccount account, cAccountSubAccount subAccount)
        {
            var accountId = account.accountid;
            string responseMessage=string.Empty;
            var claimantIds = DvlaConsentReminders.GetClaimantDetailsForConsentReminders(accountId);
            if (claimantIds == null || claimantIds.Count == 0)
            {
                responseMessage = "Consent Reminder: Error while retrieving claimant ID's for consent reminder";
                cEventlog.LogEntry(responseMessage, true, EventLogEntryType.Error);
              
            }

            foreach (var claimantDetails in claimantIds)
            {

                if (subAccount.SubAccountProperties.EnableAutomaticDrivingLicenceLookup)
                {
                    if (!account.HasDvlaLookupKeyAndDvlaConnectLicenceElement(SpendManagementElement.DvlaConnect))
                    {
                        responseMessage = "Consent Reminder: Could not access Driver & Vehicle Check service: Driver & Vehicle Check key is absent or Account licenced element is not available for the account: " + accountId;
                        cEventlog.LogEntry(responseMessage, true, EventLogEntryType.Error);
                    }
                    else if (string.IsNullOrEmpty(claimantDetails.SecurityKey))
                    {
                        responseMessage = "Consent Reminder: Security code absent for employee " +claimantDetails.ClaimantId + ": Account Id: " + accountId;
                        cEventlog.LogEntry(responseMessage, true, EventLogEntryType.Error);
                    }
                    else
                    {
                        var dvlaApi = BootstrapDvla.CreateNew();
                        var consentRevoked = dvlaApi.InvalidateUsersConsent(new Guid(claimantDetails.SecurityKey));

                        if (consentRevoked)
                        {
                            int[] recipientsId = {claimantDetails.ClaimantId};
                            var user = cMisc.GetCurrentUser($"{accountId}, {claimantDetails.ClaimantId}");
                            var dvlaLookUpConsent = new DvlaLookUpConsent();
                            dvlaLookUpConsent.DeleteConsentAfterRevoking(claimantDetails.ClaimantId, accountId);
                            var notifications = new NotificationTemplates(user, claimantDetails.EmailId);
                            notifications.SendMessage(SendMessageEnum.GetEnumDescription(SendMessageDescription.SentToClaimantWhenConsentIsDueToExpire), senderid: user.EmployeeID,recipientsId: recipientsId);
                            responseMessage = "Email successfully sent " + ": Account Id : " + accountId;
                        }
                        else
                        {
                            responseMessage = "Consent Reminder : Unable to revoke the consent for the employee : " +claimantDetails.ClaimantId;
                            cEventlog.LogEntry(responseMessage, true, EventLogEntryType.Error);
                        }
                    }
                }
            }
            return responseMessage;
        }

        /// <summary>
        /// Checks whether or not the provided driver has valid consent on record.
        /// </summary>
        /// <param name="driverId">The driver to check</param>
        /// <returns>true if consent is on record, false if not</returns>
        public bool HasUserProvidedConsent(int driverId)
        {
            IDutyOfCareApi dvlaApi = BootstrapDvla.CreateNew();
            return dvlaApi.HasUserProvidedConsent(driverId);
        }
    }
}