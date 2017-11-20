namespace SpendManagementApi.Models.Responses
{
    using SpendManagementApi.Models.Common;

    public class SessionTimeoutSettingsResponse : ApiResponse
    {
        /// <summary>
        /// Gets or sets a value indicating how many minutes the app can be idle before the user is automatically logged out
        /// </summary>
        public int IdleTimeoutPeriod { get; set; }

        /// <summary>
        /// Gets or sets a value indicating how many seconds before automatic logout the warning message appears
        /// </summary>
        public int IdleTimeoutWarningPeriod { get; set; }
    }
}