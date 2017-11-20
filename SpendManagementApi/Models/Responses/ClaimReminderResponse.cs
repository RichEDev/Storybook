namespace SpendManagementApi.Models.Responses
{
    using Common;

    /// <summary>
    /// The claim reminder response.
    /// </summary>
    public class ClaimReminderResponse : ApiResponse
    {
        /// <summary>
        /// flag telling whether sending email was successful or not
        /// </summary>
        public bool IsSendingSuccessful { get; set; }

        /// <summary>
        /// error message to be set if any exceptions arise
        /// </summary>
        public string ErrorMessage { get; set; }
    }
}