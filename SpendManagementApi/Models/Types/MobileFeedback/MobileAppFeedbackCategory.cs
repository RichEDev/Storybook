namespace SpendManagementApi.Models.Types.MobileFeedback
{
    using SpendManagementApi.Interfaces;

    /// <summary>
    /// The MobileAppFeedbackCategory class
    /// </summary>
    public class MobileAppFeedbackCategory : BaseExternalType, IBaseClassToAPIType<SpendManagementLibrary.MobileAppReview.MobileAppFeedbackCategory, MobileAppFeedbackCategory>
    {
        /// <summary>
        /// Gets or sets the feedback category Id
        /// </summary>
        public int CategoryId { get; set; }

        /// <summary>
        /// Gets or sets the feedback category Description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Converts a spend management library type to a API type
        /// </summary>
        /// <param name="dbType">
        /// The db type.
        /// </param>
        /// <param name="actionContext">
        /// The action context.
        /// </param>
        /// <returns>
        /// The <see cref="MobileAppFeedbackCategory"/>.
        /// </returns>
        public MobileAppFeedbackCategory ToApiType(SpendManagementLibrary.MobileAppReview.MobileAppFeedbackCategory dbType, IActionContext actionContext)
        {
            if (dbType == null)
            {
                return null;
            }

            this.CategoryId = dbType.CategoryId;
            this.Description = dbType.Description;

            return this;
        }

    }
}