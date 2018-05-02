namespace DutyOfCareServices.ApiCalls
{
    using System;
    using System.Collections.Generic;
    using System.Net;

    using APICallsHelper;

    using ApiClientHelper;
    using ApiClientHelper.Responses;

    /// <summary>
    /// The duty of care documents expiry reminder.
    /// </summary>
    public class DutyOfCareDocumentsExpiryReminder
    {
        /// <summary>
        /// Api URL for duty of care documents expiry reminder for approver.
        /// </summary>
        private const string ApiEndPointForDutyOfCareReminderForApprover = "DutyOfCare/NotifyApproverOnExpiryOfDutyOfCareDocument";

        /// <summary>
        /// Api URL for duty of care documents expiry reminder for claimant.
        /// </summary>
        private const string ApiEndPointForDutyOfCareReminderForClaimant = "DutyOfCare/NotifyClaimantOnExpiryOfDutyOfCareDocument";

        /// <summary>
        /// Api URL to the Get Accounts With duty of care documents expiry reminders enabled.
        /// </summary>
        private const string ApiEndPointGetAccountForApproverMails = "DutyOfCare/GetAccountsWithApproverDutyOfCareRemindersEnabled";

        /// <summary>
        /// Api URL to the Get Accounts Withduty of care documents expiry reminders enabled.
        /// </summary>
        private const string ApiEndPointGetAccountForClaimantMails = "DutyOfCare/GetAccountsWithClaimantDutyOfCareRemindersEnabled";

        /// <summary>
        /// Common substring of all event log messages for notifying duty of care documents expiry.
        /// </summary>
        private const string LogMessage = "Notify Duty of care documents expiry :";

        /// <summary>
        /// The _api username.
        /// </summary>
        private readonly string _apiUsername;

        /// <summary>
        /// The api password.
        /// </summary>
        private readonly string _apiPassword;

        /// <summary>
        /// The api url path.
        /// </summary>
        private readonly string _apiUrlPath;

        /// <summary>
        /// The _default company id.
        /// </summary>
        private readonly string _defaultCompanyId;

        /// <summary>
        /// The logger.
        /// </summary>
        private readonly EventLogger _logger;

        /// <summary>
        /// The API client.
        /// </summary>
        private Client _apiClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="DutyOfCareDocumentsExpiryReminder"/> class.
        /// </summary>
        /// <param name="apiUrlPath">
        /// The api url path.
        /// </param>
        /// <param name="defaultCompanyId">
        /// The default company id.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        public DutyOfCareDocumentsExpiryReminder(string apiUrlPath, string defaultCompanyId, EventLogger logger)
        {
            this._apiUrlPath = apiUrlPath;
            this._defaultCompanyId = defaultCompanyId;
            this._logger = logger;
        }

        /// <summary>
        /// The send mail to users on expiry of documents.
        /// </summary>
        /// <param name="accountList">
        /// The account list.
        /// </param>
        /// <param name="isApprover">
        /// The is approver.
        /// </param>
        private void SendMailToUsersOnExpiryOfDocuments(List<AccountForparticularGeneralOption> accountList, bool isApprover)
        {
            var procedueName = isApprover == true ? ApiEndPointForDutyOfCareReminderForApprover : ApiEndPointForDutyOfCareReminderForClaimant;
            try
            {
                foreach (var account in accountList)
                {
                    // Switch accounts
                    this._apiClient.SetCompany(account.CompanyId);
                    Console.WriteLine(@"Sending emails out");

                    var result = this._apiClient.EmailNotificationForEnabledServices(procedueName + "/" + account.AccountId);
                    if (result.Result.Data.IsSendingSuccessful)
                    {
                        Console.WriteLine("Email successfully sent");
                        this._logger.MakeEventLogEntry(LogMessage, procedueName, "Emails were sent successfully");
                    }
                    else
                    {
                        this._logger.MakeEventLogEntry(LogMessage, procedueName, "No emails sent for account: " + account.AccountId);
                    }
                }
            }
            catch (Exception ex)
            {
                this._logger.MakeEventLogEntry(LogMessage + " Error :", procedueName, ex.Message, HttpStatusCode.InternalServerError);
            }
        }

        /// <summary>
        /// The remind users of Duty of care documents expiry.
        /// </summary>
        /// <param name="isApprover">
        /// The is Approver.
        /// </param>
        public void RemindUsersOfExpiringDutyOfCareDocuments(bool isApprover)
        {
            var generalOptionsApi = isApprover ? ApiEndPointGetAccountForApproverMails : ApiEndPointGetAccountForClaimantMails;
            this._apiClient = new Client(this._defaultCompanyId, this._apiUrlPath);
            var accountList = this._apiClient.GetAccountsForParticularGeneralOptionEnabled(generalOptionsApi);
            Console.WriteLine("Fetching accounts for Duty of care documents expiry reminders");
            if (accountList != null && accountList.Result.Data.AccountList.Count == 0)
            {
                Console.WriteLine("No accounts with Duty of care documents expiry reminder check general option enabled");
                this._logger.MakeEventLogEntry("Accounts", generalOptionsApi, "No accounts with Duty of care documents expiry reminder check general option enabled");
                return;
            }

            if (accountList == null)
            {
                this._logger.MakeEventLogEntry("Failed to load accounts", generalOptionsApi, "Error in fetching accounts");
                return;
            }

            this.SendMailToUsersOnExpiryOfDocuments(accountList.Result.Data.AccountList, isApprover);
        }
    }
}
