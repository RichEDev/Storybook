using System.Collections.Generic;
using Common.Logging;

namespace ApproverReminder
{
    using APICallsHelper;
    using System;
    using System.IO;
    using System.Net;

    /// <summary>
    /// Class to remind approvers of ther pending claims
    /// </summary>
    public class ClaimReminder
    {
        private readonly RequestHelper _requestHelper;
        private readonly ResponseHelper _responseHelper;
        private List<cAccount> _accounts;

        /// <summary>
        /// An instance of <see cref="ILog"/> for logging information.
        /// </summary>
        private readonly ILog _log;

        /// <summary>
        /// Api URL for the approver reminder.
        /// </summary>
        private const string ApiEndPoint = "Claims/NotifyApproversOfPendingClaims/{0}";

        /// <summary>
        /// Api URL for the current claims reminder.
        /// </summary>
        private const string CurrentClaimsApiEndPoint = "Claims/NotifyClaimantsOfCurrentClaims/{0}";

        /// <summary>
        /// Api Url to get the current active accounts.
        /// </summary>
        private const string ActiveAccounts = "Account/GetAccountsWithClaimRemindersEnabled";

        private List<cAccount> Accounts
        {
            get
            {
                if (this._accounts == null)
                {
                    var result = this.GetActiveAccounts();
                    if (result != null)
                    {
                        this._accounts = result.AccountList;
                    }
                    
                }

                return this._accounts;
            }
        }

        /// <summary>
        /// Create a new instance of <see cref="ClaimReminder"/>
        /// </summary>
        /// <param name="requestHelper">object of type <see cref="RequestHelper"/> which helps making API requests.</param>
        /// <param name="responseHelper">object of type <see cref="ResponseHelper"/>  which helps to carry out operations on response of api request.</param>
        /// <param name="log">An instance of <see cref="ILog"/> for logging <see cref="ICache{T,TK}"/> diagnostics and information.</param>
        public ClaimReminder(RequestHelper requestHelper, ResponseHelper responseHelper, ILog log)
        {
            this._requestHelper = requestHelper;
            this._responseHelper = responseHelper;
            this._log = log;
        }

        /// <summary>
        /// Get a list of the Active Accounts from the API
        /// </summary>
        /// <returns></returns>
        private GetAccountsResponse GetActiveAccounts()
        {
            this._log.Debug("Method GetActiveAccounts has started.");

            var request = this._requestHelper.GetHttpWebRequest(ActiveAccounts);
            request.Timeout = 300000;
            try
            {
                var response = (HttpWebResponse)request.GetResponse();
                var getAccountsResponse = this._responseHelper.GetResponseObject<GetAccountsResponse>(response, new StreamReader(response.GetResponseStream()));

                if (this._log.IsInfoEnabled)
                {
                    this._log.Info($"Found {getAccountsResponse.AccountList.Count} active accounts to send email reminders for.");
                }

                this._log.Debug("Method GetActiveAccounts has completed.");

                return getAccountsResponse;
            }
            catch (Exception ex)
            {
                if (this._log.IsErrorEnabled)
                {
                    this._log.Error($"Error - {ActiveAccounts}, {ex.Message}", ex);
                }
            }

            this._log.Debug("Method GetActiveAccounts has completed.");

            return new GetAccountsResponse();
        }

        /// <summary>
        /// Method which calls the api to notify the approvers of pending claims.
        /// </summary>
        public void RemindApproversOfPendingClaims()
        {
            this._log.Debug("Method RemindApproversOfPendingClaims has started.");

            var reminder = new Reminder<ApproverEmailReminderResponse>(this._requestHelper, this._responseHelper, this.Accounts, new LogFactory<Reminder<ApproverEmailReminderResponse>>().GetLogger());
            reminder.GenerateReminders(ApiEndPoint);
            
            this._log.Debug("Method RemindApproversOfPendingClaims has completed.");
        }

        /// <summary>
        /// Method which calls the api to notify the claimants for unsubmitted claims.
        /// </summary>
        public void RemindClaimantsOfCurrentClaims()
        {
            this._log.Debug("Method RemindClaimantsOfCurrentClaims has started.");

            var reminder = new Reminder<ApproverEmailReminderResponse>(this._requestHelper, this._responseHelper, this.Accounts, new LogFactory<Reminder<ApproverEmailReminderResponse>>().GetLogger());
            reminder.GenerateReminders(CurrentClaimsApiEndPoint);
            
            this._log.Debug("Method RemindClaimantsOfCurrentClaims has completed.");
        }
    }
}
