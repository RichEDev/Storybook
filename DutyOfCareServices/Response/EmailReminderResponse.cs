namespace DutyOfCareServices.Response
{
    /// <summary>
    /// The email reminder response.
    /// </summary>
    public class EmailReminderResponse
    {
        /// <summary>
        /// Gets or sets a value indicating whether sending email was successful or not.
        /// </summary>
        public bool IsSendingSuccessful { get; set; }

        /// <summary>
        /// Gets or sets error message if any.
        /// </summary>
        public string ErrorMessage { get; set; }
    }
}
