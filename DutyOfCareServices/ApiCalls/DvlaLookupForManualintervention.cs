namespace DutyOfCareServices.ApiCalls
{
    using System;
    using System.Net;

    using APICallsHelper;
    using ApiClientHelper;

    /// <summary>
    /// Populates driving licence details for the drivinglicence numbers for which the Dvla manual consent has been granted.
    /// </summary>
    public class DvlaLookupForManualintervention
    {
        /// <summary>
        /// Api URL to  the Get Accounts With DVLA auto populate enabled .
        /// </summary>
        private const string ApiEndPointGetAccount = "PopulateDrivingLicence/GetAccountsWithAutoDvlaConsentEnabled";

        /// <summary>
        /// Api URL for saving driving licence.
        /// </summary>
        private const string ApiEndPointToSaveDrivingLicence = "PopulateDrivingLicence/SaveDvlaDrivingLicenceInformation";

        /// <summary>
        /// Api URL to get valid employee who has provided the manual consent for which driving licence should be populated.
        /// </summary>
        private const string ApiEndPointToGetEmployeeWithDvlaConstraint = "PopulateDrivingLicence/GetEmployeeWithManualIntervention";

        /// <summary>
        /// Api URL to get employee  driving licence date from the duty of care licence check api.
        /// </summary>
        private const string ApiEndPointToPopulateDrivingLicence = "PopulateDrivingLicence/PopulateDrivingLicenceInformation";

        /// <summary>
        /// Common log message
        /// </summary>
        public const string LogMessage = "Saving driving licence for manual intervention :";

        /// <summary>
        /// The API client.
        /// </summary>
        private Client _apiClient;

        /// <summary>
        /// Method which calls the api to get accounts and then Api for populating drivinglicence.
        /// </summary>
        /// <param name="apiUrlPath">
        /// The api Url Path.
        /// </param>
        /// <param name="defaultCompanyId">
        /// The default Company Id.
        /// </param>
        /// <param name="passwordToCheckLicence">
        /// The password To Check Licence.
        /// </param>
        /// <param name="usernametoCheckLicence">
        /// The usernameto Check Licence.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        public void PopulateEmployeeDrivingLicences(string apiUrlPath, string defaultCompanyId, string passwordToCheckLicence, string usernametoCheckLicence, EventLogger logger)
        {
            this._apiClient = new Client(defaultCompanyId, apiUrlPath);
            Console.WriteLine(@"Populating accounts with dvla autopopulate general option enabled");
            var accountList = this._apiClient.GetAccountsForParticularGeneralOptionEnabled(ApiEndPointGetAccount);

            if (accountList.Result.Data.AccountList.Count == 0)
            {
                logger.MakeEventLogEntry(LogMessage + "Accounts", ApiEndPointGetAccount, "No accounts with dvla autopopulate general option enabled");
                return;
            }

            foreach (var account in accountList.Result.Data.AccountList)
            {
                // Switch accounts
                this._apiClient.SetCompany(account.CompanyId);
                Console.WriteLine(@"Fetch employees details from Dvla who have submitted manual consent ");

                var employeeDetails = this._apiClient.GetEmployeeToPopulateDrivingLicence(ApiEndPointToGetEmployeeWithDvlaConstraint + "/" + account.AccountId);
                if (employeeDetails.EmployeeList.Count == 0)
                {
                    Console.WriteLine("No employees to populate driving licence");
                    logger.MakeEventLogEntry(LogMessage + "No employees to populate driving licence", ApiEndPointToGetEmployeeWithDvlaConstraint, "for account: " + account.AccountId);
                    continue;
                }

                Console.WriteLine(@"Populate employee's driving licence details from dvla portal");

                var drivingLicenceDetails = this._apiClient.PopulateDrivingLicences(ApiEndPointToPopulateDrivingLicence + "/" + account.AccountId, employeeDetails);
                if (drivingLicenceDetails != null && drivingLicenceDetails.DrivingLicenceDetails.Count == 0)
                {
                    Console.WriteLine("No employees driving licence details found");
                    logger.MakeEventLogEntry(LogMessage + "Failed to load employees driving licence details", "from dvla portal", "for account: " + account.AccountId);
                    continue;
                }

                Console.WriteLine(@"Saving driving licence information for the account " + account.AccountId);

                this._apiClient.SaveCustomEntity(ApiEndPointToSaveDrivingLicence + "/" + account.AccountId, drivingLicenceDetails);
                Console.WriteLine(@"Saving driving licence completed successfully for the account " + account.AccountId);
            }
        }
    }
}
