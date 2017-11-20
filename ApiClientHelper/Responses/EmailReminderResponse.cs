namespace ApiClientHelper.Responses
{
    /// <summary>
    /// The email reminder response.
    /// </summary>
    public class EmailReminderResponse
    {
        /// <summary>
        /// Gets or sets a value indicating whether flag telling whether sending email was successful or not
        /// </summary>
        public bool IsSendingSuccessful { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether error message to be set if any exceptions arise
        /// </summary>
        public string ResponseMessage { get; set; }
    }
}
