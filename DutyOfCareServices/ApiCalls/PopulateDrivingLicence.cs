namespace DutyOfCareServices.ApiCalls
{
    using System;
    using APICallsHelper;
    using ApiClientHelper;

    /// <summary>
    /// Populates driving licence details from portal
    /// </summary>
    public class PopulateDrivingLicence
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
        /// Api URL to get valid employee for which driving licence should be populated.
        /// </summary>
        private const string ApiEndPointToGetEmployeeWithDvlaConstraint = "PopulateDrivingLicence/GetEmployeeToPopulateDrivingLicence";

        /// <summary>
        /// Api URL to get valid employee for which driving licence should be populated.
        /// </summary>
        private const string ApiEndPointToPopulateDrivingLicence = "PopulateDrivingLicence/PopulateDrivingLicenceInformation";

        /// <summary>
        /// Common log message
        /// </summary>
        public const string LogMessage = "Saving driving licence :";

        /// <summary>
        /// The API client.
        /// </summary>
        private Client _apiClient;

        /// <summary>
        /// Method which calls the api to get accounts and then Api  for populating drivinglicence.
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
        /// The username to Check Licence.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        public void PopulateEmployeeDrivingLicence(string apiUrlPath, string defaultCompanyId, string passwordToCheckLicence, string usernametoCheckLicence, EventLogger logger)
        {
            this._apiClient = new Client(defaultCompanyId,apiUrlPath);
            Console.WriteLine(@"Populating accounts with dvla autopopulate general option enabled");
            var accountList = this._apiClient.GetAccountsForParticularGeneralOptionEnabled(ApiEndPointGetAccount);

            if (accountList != null && accountList.Result.Data.AccountList.Count == 0)
            {
                Console.WriteLine("No accounts with dvla autopopulate general option enabled");
                logger.MakeEventLogEntry(LogMessage + "Accounts", ApiEndPointGetAccount, "No accounts with dvla autopopulate general option enabled");
                return;
            }

            if (accountList == null)
            {
                Console.WriteLine("Error in fetching accounts");
                logger.MakeEventLogEntry(LogMessage + "Failed to load accounts", ApiEndPointGetAccount, "Error in fetching accounts");
                return;
            }

            foreach (var account in accountList.Result.Data.AccountList)
            {
                // Switch accounts
                this._apiClient.SetCompany(account.CompanyId);
                Console.WriteLine(@"Populating employees details");

                var employeeDetails = this._apiClient.GetEmployeeToPopulateDrivingLicence(ApiEndPointToGetEmployeeWithDvlaConstraint + "/" + account.AccountId);
                if (employeeDetails != null && employeeDetails.EmployeeList.Count == 0)
                {
                    Console.WriteLine("No employees to populate driving licence");
                    logger.MakeEventLogEntry(
                        "No employees to populate driving licence",
                        ApiEndPointToGetEmployeeWithDvlaConstraint,
                        "for account: " + account.AccountId);
                    continue;
                }

                Console.WriteLine(@"Populating employees driving licence from dvla portal");

                var drivingLicenceDetails = this._apiClient.PopulateDrivingLicences(ApiEndPointToPopulateDrivingLicence + "/" + account.AccountId, employeeDetails);
                if (drivingLicenceDetails != null && drivingLicenceDetails.DrivingLicenceDetails.Count == 0)
                {
                    Console.WriteLine("Failed to load driving licence details for account " + account.AccountId);
                    logger.MakeEventLogEntry(LogMessage + "Failed to load employees driving licence details", "from dvla portal", "for account: " + account.AccountId);
                    continue;
                }
                Console.WriteLine(@"Saving driving licence information for the account " + account.AccountId);

               var result = this._apiClient.SaveCustomEntity(ApiEndPointToSaveDrivingLicence + "/" + account.AccountId, drivingLicenceDetails);
                if (result)
                {
                    Console.WriteLine(@"Saving driving licence completed successfully for the account " + account.AccountId);
                }
                else
                {
                    Console.WriteLine("Failed to save driving licence details for account " + account.AccountId);
                }
            }
        }
    }
}
