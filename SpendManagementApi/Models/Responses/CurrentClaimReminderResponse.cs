
namespace SpendManagementApi.Models.Responses
{
    using Common;

    /// <summary>
    /// Current claims remainder response.
    /// </summary>
    public class CurrentClaimReminderResponse: ApiResponse
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