namespace DutyOfCareServices.ApiCalls
{
    using System;

    using APICallsHelper;

    using ApiClientHelper;

    /// <summary>
    /// The driving licence review expiry reminder.
    /// </summary>
    public class DrivingLicenceReviewExpiryReminder
    {
        /// <summary>
        /// Api URL for the driving licence review expiry reminder.
        /// </summary>
        private const string ApiEndPointForEmailReminders = "DutyOfCare/NotifyOnExpiryOfDrivingLicenceReview";

        /// <summary>
        /// Api URL to  the Get Accounts With Driving licence reviews enabled.
        /// </summary>
        private const string ApiEndPointGetAccount = "DutyOfCare/GetAccountsWithDrivingLicenceReviewsEnabled";

        /// <summary>
        /// The API client.
        /// </summary>
        private Client _apiClient;

        /// <summary>
        /// The notify claimants on expired driving licence reviews.
        /// </summary>
        /// <param name="apiUrlPath">
        /// The api url path.
        /// </param>
        /// <param name="defaultCompanyId">
        /// The default company id.
        /// </param>
        /// <param name="logger">
        /// The Event logger instance.
        /// </param>
        public void NotifyClaimantsOnExpiredDrivingLicenceReviews(string apiUrlPath, string defaultCompanyId, EventLogger logger)
        {
            this._apiClient = new Client(defaultCompanyId, apiUrlPath);
            Console.WriteLine(@"Populating accounts with Driving licence review check general option enabled");
            var accountList = this._apiClient.GetAccountsForParticularGeneralOptionEnabled(ApiEndPointGetAccount);

            if (accountList != null && accountList.Result.Data.AccountList.Count == 0)
            {
                Console.WriteLine("No accounts with Driving licence review check general option enabled");
                logger.MakeEventLogEntry("Failed to load accounts", ApiEndPointGetAccount, "No accounts with Driving licence review check general option enabled");
                return;
            }

            if (accountList == null)
            {
                logger.MakeEventLogEntry("Failed to load accounts", ApiEndPointGetAccount, "Error in fetching accounts");
                return;
            }

            foreach (var account in accountList.Result.Data.AccountList)
            {
                // Switch accounts
                this._apiClient.SetCompany(account.CompanyId);
                Console.WriteLine(@"Sending emails out");

                var result = this._apiClient.EmailNotificationForEnabledServices(ApiEndPointForEmailReminders + "/" + account.AccountId);
                if (result != null && result.Result.Data.IsSendingSuccessful)
                {
                    Console.WriteLine("Email successfully sent");
                }
                else
                {
                    logger.MakeEventLogEntry(
                        "Failed to send mails ",
                        ApiEndPointForEmailReminders,
                        "for account: " + account.AccountId);
                }
            }
        }
    }
}
