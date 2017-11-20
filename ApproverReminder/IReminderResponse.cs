namespace ApproverReminder
{
    /// <summary>
    /// An interface defining all Reminder Responses
    /// </summary>
    public interface IReminderResponse
    {
        /// <summary>
        /// Gets or sets error message if any.
        /// </summary>
        string ErrorMessage { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether sending email was successful or not.
        /// </summary>
        bool IsSendingSuccessful { get; set; }
    }
}