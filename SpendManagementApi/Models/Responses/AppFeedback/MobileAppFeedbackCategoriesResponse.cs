namespace SpendManagementApi.Models.Responses.AppFeedback
{
    using System.Collections.Generic;

    using SpendManagementApi.Models.Common;
    using SpendManagementApi.Models.Types.MobileFeedback;

    /// <summary>
    /// The FeedbackCategoryMastersResponse
    /// </summary>
    public class MobileAppFeedbackCategoriesResponse : GetApiResponse<MobileAppFeedbackCategory>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MobileAppFeedbackCategoriesResponse"/> class.
        /// </summary>
        public MobileAppFeedbackCategoriesResponse()
        {
            this.List = new List<MobileAppFeedbackCategory>();
        }
    }
}