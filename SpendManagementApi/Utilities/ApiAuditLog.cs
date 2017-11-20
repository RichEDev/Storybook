namespace SpendManagementApi.Utilities
{
    using System;
    using System.Configuration;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Attributes;
    using SpendManagementLibrary;
    using SpendManagementLibrary.API;
    using SpendManagementLibrary.Helpers;
    using SpendManagementLibrary.Interfaces;

    /// <summary>
    /// A helper DAL class to enable the <see cref="AuthAuditAttribute"/> to log method calls and results to the database, as well as 
    /// determine if the account is licensed to access the API. 
    /// </summary>
    public class ApiAuditLog : ILicenseAndAudit
    {
        /// <summary>
        /// Determines whether the account has any credits left. This is done over several time thresholds.
        /// See the <see cref="ApiAuditLogCallResult"/> class for more details.
        /// </summary>
        /// <param name="accountId">The account Id to check the remaining credits for</param>
        /// <param name="decreaseTransactionCount">Whether to just check access and not decrement yet</param>
        /// <param name="decrementBy">The amount to decrement the credits by if there are any left.</param>
        /// <param name="connection">A connection to use. One will be created if not.</param>
        /// <param name="updateDatabase">Whether to update the database (default true).s</param>
        /// <returns>An <see cref="ApiAuditLogCallResult"/> representing the state of the operation and some useful user information.</returns>
        public ApiAuditLogCallResult DetermineAccessAndDecrement(int accountId, bool decreaseTransactionCount, int decrementBy = 1, IDBConnection connection = null, bool updateDatabase = true, bool mobileRequest = false)
        {
            #region init

            var account = cAccounts.CachedAccounts.FirstOrDefault(x => x.Key == accountId).Value;
            var result = new ApiAuditLogCallResult
            {
                Allowed = true,
                ApiLicenseStatus = account.ApiLicenseStatus
            };

            #endregion init

            #region Determine Access

            // throw out if no threshold is configured
            if (result.ApiLicenseStatus == null)
            {
                result.Allowed = false;
                result.FriendlySummary = ApiResources.HttpStatusCodeForbiddenLicense;
                return result;
            }

            // throw out if total paid and free is zero
            if (result.ApiLicenseStatus.TotalLicensedCalls <= 0 && result.ApiLicenseStatus.FreeCallsToday <= 0 && !mobileRequest)
            {
                result.Allowed = false;
                result.FriendlySummary = ApiResources.AuditMessageTotalRemainingIsZero;
                return result;
            }

            // throw out if there are not enough calls to complete the transaction
            if (result.ApiLicenseStatus.TotalLicensedCalls + result.ApiLicenseStatus.FreeCallsToday < decrementBy && !mobileRequest)
            {
                result.Allowed = false;
                result.FriendlySummary = ApiResources.AuditMessageThresholdLimitNotEnough;
                return result;
            }

            // check the thresholds
            var now = DateTime.Now;
            var hourSpan = TimeSpan.FromHours(1);
            var minuteSpan = TimeSpan.FromMinutes(1);
            
            // Check if the minute needs updating first..
            if (now > (result.ApiLicenseStatus.MinuteLastResetDate + minuteSpan))
            {
                result.ApiLicenseStatus.MinuteRemainingCalls = result.ApiLicenseStatus.MinuteLimit;
                result.ApiLicenseStatus.MinuteLastResetDate = now;
                result.ApiLicenseStatus.MinuteLastResetDate = result.ApiLicenseStatus.MinuteLastResetDate.AddTicks(-result.ApiLicenseStatus.MinuteLastResetDate.Ticks % 10000000);
            }

            // Now check hourly limit
            if (result.ApiLicenseStatus.MinuteRemainingCalls < decrementBy)
            {
                var output = "(which resets at " + (result.ApiLicenseStatus.MinuteLastResetDate + minuteSpan) + ")";
                result.Allowed = false;
                result.FriendlySummary = ApiResources.AuditMessageThresholdLimitHitMinute + output;
            }

            // Check if the hour needs updating first..
            if (now > (result.ApiLicenseStatus.HourLastResetDate + hourSpan))
            {
                result.ApiLicenseStatus.HourRemainingCalls = result.ApiLicenseStatus.HourLimit;
                result.ApiLicenseStatus.HourLastResetDate = now;
                result.ApiLicenseStatus.HourLastResetDate = result.ApiLicenseStatus.HourLastResetDate.AddTicks(-result.ApiLicenseStatus.HourLastResetDate.Ticks % 10000000);
            }

            // Now check hourly limit
            if (result.ApiLicenseStatus.HourRemainingCalls < decrementBy)
            {
                var output = "(which resets at " + (result.ApiLicenseStatus.HourLastResetDate + hourSpan) + ")";
                result.Allowed = false;
                result.FriendlySummary = ApiResources.AuditMessageThresholdLimitHitHour + output;
            }
            
            #endregion Determine Access

            #region Decrease in Memory and DB

            if (decreaseTransactionCount)
            {
                // do the actual decrement if all thresholds pass
                if (result.Allowed)
                {
                    // decrement per hour / minute
                    result.ApiLicenseStatus.HourRemainingCalls -= decrementBy;
                    result.ApiLicenseStatus.MinuteRemainingCalls -= decrementBy;

                    // decrement free and total
                    if (result.ApiLicenseStatus.FreeCallsToday < decrementBy)
                    {
                        decrementBy -= result.ApiLicenseStatus.FreeCallsToday;
                        result.ApiLicenseStatus.FreeCallsToday = 0;
                        result.ApiLicenseStatus.TotalLicensedCalls -= decrementBy;
                    }
                    else
                    {
                        result.ApiLicenseStatus.FreeCallsToday -= decrementBy;
                    }
                }

                if (updateDatabase)
                {
                    if (ConfigurationManager.AppSettings["BackgroundUpdateCallStatusInDatabase"] == "1")
                    {
                        var task = new Task(() => UpdateDatabase(result.ApiLicenseStatus, connection));
                        task.Start();
                    }
                    else
                    {
                        UpdateDatabase(result.ApiLicenseStatus, connection);
                    }
                }
            }

            #endregion Decrease in Memory and DB

            #region Return if not allowed

            // return now if failed
            if (!result.Allowed)
            {
                return result;
            }

            #endregion

            #region Build friendly string if the call was allowed

            // build friendly string
            var sb = new StringBuilder();
            sb.Append("Total licensed remaining: " + result.ApiLicenseStatus.TotalLicensedCalls + " | ");
            sb.Append("Free remaining today: " + result.ApiLicenseStatus.FreeCallsToday + " | ");
            sb.Append("Remaining this hour: " + result.ApiLicenseStatus.HourRemainingCalls + "/" + result.ApiLicenseStatus.HourLimit);
            sb.Append(" (which resets at " + (result.ApiLicenseStatus.HourLastResetDate + hourSpan) + ") | ");
            sb.Append("Remaining this minute: " + result.ApiLicenseStatus.MinuteRemainingCalls + "/" + result.ApiLicenseStatus.MinuteLimit);
            sb.Append(" (which resets at " + (result.ApiLicenseStatus.MinuteLastResetDate + minuteSpan) + ") | ");
            result.FriendlySummary = sb.ToString();

            #endregion Build friendly string if the call was allowed

            return result;
        }

        /// <summary>
        /// Enter into audit log a user action.
        /// </summary>
        /// <param name="user">The user from which log details will be accessed.</param>
        /// <param name="uri">The url that was called.</param>
        /// <param name="result">The api response status that was set back.</param>
        public void RecordApiAction(ICurrentUserBase user, string uri, string result)
        {
            using (var expdata = new DatabaseConnection(cAccounts.getConnectionString(user.AccountID)))
            {
                expdata.AddWithValue("@employeeId", user.EmployeeID);
                expdata.AddWithValue("@uri", uri);
                expdata.AddWithValue("@result", result);
                expdata.ExecuteProc("AddSuccessfulApiCallToAuditLog");
            }
        }

        private void UpdateDatabase(ApiLicenseStatus status, IDBConnection connection)
        {
            // create the connection if not passed in externally
            var externalDbConnection = connection == null;
            var db = connection ?? new DatabaseConnection(GlobalVariables.MetabaseConnectionString);

            // clean up first if external
            if (!externalDbConnection)
            {
                db.sqlexecute.Parameters.Clear();
            }

            db.AddWithValue("@accountId", status.AccountId);
            db.AddWithValue("@totalCalls", status.TotalLicensedCalls);
            db.AddWithValue("@freeToday", status.FreeCallsToday);
            db.AddWithValue("@hourLimit", status.HourLimit);
            db.AddWithValue("@hourRemaining", status.HourRemainingCalls);
            db.AddWithValue("@hourLast", status.HourLastResetDate);
            db.AddWithValue("@minuteLimit", status.MinuteLimit);
            db.AddWithValue("@minuteRemaining", status.MinuteRemainingCalls);
            db.AddWithValue("@minuteLast", status.MinuteLastResetDate);
            db.ExecuteProc("UpdateApiAccountLicenses");

            // clean up
            db.sqlexecute.Parameters.Clear();
            if (!externalDbConnection)
            {
                db.Dispose();
            }
        }
    }


    /// <summary>
    /// Defines the contract for a call licenser and auditer.
    /// </summary>
    public interface ILicenseAndAudit
    {
        /// <summary>
        /// Determines whether the account has any credits left. This is done over several time thresholds.
        /// See the <see cref="ApiAuditLogCallResult"/> class for more details.
        /// </summary>
        /// <param name="accountId">The account Id to check the remaining credits for</param>
        /// <param name="decreaseTranCount">Decrease the transaction count</param>
        /// <param name="decrementBy">The amount to decrement the credits by if there are any left.</param>
        /// <param name="connection">A connection to use. One will be created if not.</param>
        /// <param name="updateDatabase">Whether to update the database (default true).s</param>
        /// <param name="mobileRequest">Whether the request came from mobile and therefore whether to bypass any API licensing.</param>
        /// <returns>An <see cref="ApiAuditLogCallResult"/> representing the state of the operation and some useful user information.</returns>
        ApiAuditLogCallResult DetermineAccessAndDecrement(int accountId, bool decreaseTranCount, int decrementBy = 1, IDBConnection connection = null, bool updateDatabase = true, bool mobileRequest = false);

        /// <summary>
        /// Records the action that was called.
        /// </summary>
        /// <param name="user">The user that made the call.</param>
        /// <param name="uri">The URI of the call.</param>
        /// <param name="result"></param>
        void RecordApiAction(ICurrentUserBase user, string uri, string result);
    }


    /// <summary>
    /// Represents the result of a checking whether the user can make a licensed call to the API.
    /// This helps throttle API usage. Usage is as follows..:
    /// Free calls are decremented before paid calls, and are reset at 00:00 UTC.
    /// Once the free calls are used up, the paid calls start decrementing.
    /// In order to throttle usage, there are per-hour and per-minute limits. These get decremented also on each call.
    /// These are not reset each day, but rather at the point a call is made:
    /// The remaining-per-hour is reset if the last-hour-update DateTime is more than an hour ago.
    /// The remaining-per-minute is reset if the last-minute-update DateTime is more than a minute ago.
    /// If either the minute or the hour thresholds reach zero, the call is thrown out.
    /// If the TotalCalls reaches zero, the call is thrown out.
    /// </summary>
    public class ApiAuditLogCallResult
    {
        /// <summary>
        /// Whether the call is allowed to proceed.
        /// </summary>
        public bool Allowed { get; set; }

        /// <summary>
        /// This should contain a string that is human-readable summary of the amount of calls
        /// the account has left, in total and all thresholds, or at least a refusal reason.
        /// </summary>
        public string FriendlySummary { get; set; }

        /// <summary>
        /// The current status of the free and paid api calls available for this account.
        /// </summary>
        public ApiLicenseStatus ApiLicenseStatus { get; set; }
    }



}