namespace DutyOfCareServices.ApiCalls
{
    using System;

    using APICallsHelper;

    using ApiClientHelper;

    /// <summary>
    /// The consent reminder class.
    /// </summary>
    public class ConsentReminder
    {
        /// <summary>
        /// Api URL for the consent reminder.
        /// </summary>
        private const string ApiEndPointForConsentExpiry = "Dvla/NotifyOnExpiryOfConsent";

        /// <summary>
        /// Common substring of all event log messages for notifying consent reminders.
        /// </summary>
        private const string LogMessage = "Notify Claimants Of Consent Expiry :";

        /// <summary>
        /// Api URL to  the Get Accounts With Driving licence reviews enabled.
        /// </summary>
        private const string ApiEndPointGetAccount = "DutyOfCare/GetAccountsWithConsentExpiryRemindersEnabled";

        /// <summary>
        /// The API client.
        /// </summary>
        private Client _apiClient;

        /// <summary>
        /// Method which calls the api to notify the claimants of expiring consent.
        /// </summary>
        /// <param name="apiUrlPath">
        /// The api Url Path.
        /// </param>
        /// <param name="defaultCompanyId">
        /// The default Company Id.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        public void RemindUsersOfExpiringConsents(string apiUrlPath, string defaultCompanyId, EventLogger logger)
        {
            this._apiClient = new Client(defaultCompanyId, apiUrlPath);
            Console.WriteLine(@"Populating accounts with consent expiry notification general option enabled");
            var accountList = this._apiClient.GetAccountsForParticularGeneralOptionEnabled(ApiEndPointGetAccount);

            if (accountList != null && accountList.Result.Data.AccountList.Count == 0)
            {
                Console.WriteLine("No accounts with consent expiry notification general option enabled");
                logger.MakeEventLogEntry(LogMessage + " : Failed to load accounts", ApiEndPointGetAccount, "No accounts with consent expiry notification general option enabled");
                return;
            }

            if (accountList == null)
            {
                logger.MakeEventLogEntry(LogMessage + " : Failed to load accounts", ApiEndPointGetAccount, "Error in fetching accounts");
                return;
            }

            foreach (var account in accountList.Result.Data.AccountList)
            {
                // Switch accounts
                this._apiClient.SetCompany(account.CompanyId);
                Console.WriteLine(@"Sending emails out");

                var result = this._apiClient.EmailNotificationForEnabledServices(ApiEndPointForConsentExpiry + "/" + account.AccountId);
                if (result != null && result.Result.Data.ResponseMessage.Trim().Length > 0)
                {
                    Console.WriteLine(result.Result.Data.ResponseMessage);
                    logger.MakeEventLogEntry(LogMessage, ApiEndPointForConsentExpiry, result.Result.Data.ResponseMessage);
                }
                else
                {
                    logger.MakeEventLogEntry(LogMessage, ApiEndPointForConsentExpiry, "No emails to send for account: " + account.AccountId);
                }
            }
        }
    }
}
