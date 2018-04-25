namespace SpendManagementLibrary.Logic_Classes
{
    using System.Data;

    using SpendManagementLibrary.API;
    using SpendManagementLibrary.Helpers;

    /// <summary>
    /// Manages the data access for an API Licence Status
    /// </summary>
    public class ApiLicenseStatusManager
    {  
        /// <summary>
        /// Resets the daily free call limits for each account.
        /// </summary>
        public void ResetDailyFreeCallLimits()
        {
            // Reset count via SP
            using (var db = new DatabaseConnection(GlobalVariables.MetabaseConnectionString))
            {
                db.ExecuteProc("ResetDailyFreeCalls");
            }
        }

        /// <summary>
        /// Gets the values for the API licence status for the given <see cref="accountid"/> from the database.
        /// </summary>
        /// <param name="accountid">The <see cref="accountid"/> to get the API licensing status for.</param>
        /// <returns>The values for the API licence status.</returns>
        public ApiLicenseStatus Get(int accountid)
        {
            return this.GetApiLicensingStatus(accountid);
        }

        /// <summary>
        /// Update the API license status values in the database
        /// </summary>
        /// <param name="status">The current <see cref="status"/> of the API license.</param>
        public void Update(ApiLicenseStatus status)
        {
            using (var db = new DatabaseConnection(GlobalVariables.MetabaseConnectionString))
            {
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

                db.sqlexecute.Parameters.Clear();
            }                            
        }

        /// <summary>
        /// Get the API status from the database for a single account
        /// </summary>
        /// <param name="accountId">The <see cref="accountId"/> to get the API licensing status for.</param>
        /// <returns>The values of <paramref name="apiLicenseStatus"/>.</returns>
        private ApiLicenseStatus GetApiLicensingStatus(int accountId)
        {
            using (var db = new DatabaseConnection(GlobalVariables.MetabaseConnectionString))
            {
                db.AddWithValue("@accountId", accountId);
                using (var reader = db.GetReader("SELECT [AccountId],[TotalCalls],[FreeToday],[HourLimit],[HourRemaining],[HourLast],[MinuteLimit],[MinuteRemaining],[MinuteLast] FROM [ApiLicensing] WHERE [AccountId] = @accountId", CommandType.Text))
                {
                    ApiLicenseStatus apiLicenseStatus = null;
                    while (reader.Read())
                    {
                         apiLicenseStatus = new ApiLicenseStatus
                                        {
                                            AccountId = reader.GetInt32(reader.GetOrdinal("AccountId")),
                                            TotalLicensedCalls = reader.GetInt32(reader.GetOrdinal("TotalCalls")),
                                            FreeCallsToday = reader.GetInt32(reader.GetOrdinal("FreeToday")),
                                            HourLimit = reader.GetInt32(reader.GetOrdinal("HourLimit")),
                                            HourRemainingCalls = reader.GetInt32(reader.GetOrdinal("HourRemaining")),
                                            HourLastResetDate = reader.GetDateTime(reader.GetOrdinal("HourLast")).ToUniversalTime(),
                                            MinuteLimit = reader.GetInt32(reader.GetOrdinal("MinuteLimit")),
                                            MinuteRemainingCalls = reader.GetInt32(reader.GetOrdinal("MinuteRemaining")),
                                            MinuteLastResetDate = reader.GetDateTime(reader.GetOrdinal("MinuteLast")).ToUniversalTime(),
                                        };
                    }

                    return apiLicenseStatus;
                }
            }
        }        
    }
}
