namespace SpendManagementLibrary.MobileAppReview
{
    /// <summary>
    /// The mobile app feedback category.
    /// </summary>
    public class MobileAppFeedbackCategory
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MobileAppFeedbackCategory"/> class.
        /// </summary>
        /// <param name="categoryId">
        /// The category Id.
        /// </param>
        /// <param name="description">
        /// The description.
        /// </param>
        public MobileAppFeedbackCategory(int categoryId , string description)
        {
            this.CategoryId = categoryId;
            this.Description = description;
        }

        /// <summary>
        /// Gets or sets the feedback category Id
        /// </summary>
        public int CategoryId { get; set; }

        /// <summary>
        /// Gets or sets the feedback category description.
        /// </summary>
        public string Description { get; set; }
    }
}
