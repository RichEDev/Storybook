namespace SpendManagementApi.Models.Requests.MobileAppFeedback
{
    using SpendManagementApi.Models.Common;
   
    /// <summary>
    /// The mobile app feedback request, containing details of the app feedback.
    /// </summary>
    public class MobileAppFeedbackRequest : ApiRequest
    {
        /// <summary>
        /// Gets or sets the feedback category id.
        /// </summary>
        public int FeedbackCategoryId { get; set; }

        /// <summary>
        /// Gets or sets the feedback.
        /// </summary>
        public string Feedback { get; set; }

        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the mobile metric id.
        /// </summary>
        public int MobileMetricId { get; set; }

        /// <summary>
        /// Gets or sets the app version the feedback request is for.
        /// </summary>
        public string AppVersion { get; set; }
    }
}