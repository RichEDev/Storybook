namespace InternalApi.Models
{
    /// <summary>
    /// Result of sending claim reminders
    /// </summary>
    public class ClaimReminderResponse
    {
        /// <summary>
        /// Gets or sets flag telling whether sending email was successful or not
        /// </summary>
        public bool IsSendingSuccessful { get; set; }

        /// <summary>
        /// Gets or sets error message if any exceptions arise
        /// </summary>
        public string ErrorMessage { get; set; }
    }
}