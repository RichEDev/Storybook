namespace SpendManagementApi.Repositories
{
    using System.Collections.Generic;
    using System.Linq;

    using SpendManagementApi.Interfaces;

    using SpendManagementLibrary.MobileAppReview;

    using Spend_Management;

    using MobileAppFeedbackCategory = SpendManagementApi.Models.Types.MobileFeedback.MobileAppFeedbackCategory;

    /// <summary>
    /// The app feedback repository which handles the employee's mobile app feedback preferences. 
    /// </summary>
    internal class AppFeedbackRepository : BasicRepository, ISupportsActionContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AppFeedbackRepository"/> class.
        /// </summary>
        /// <param name="user">
        /// An instance of <see cref="ICurrentUser"/>
        /// </param>
        public AppFeedbackRepository(ICurrentUser user)
            : base(user)
        {

        }

        /// <summary>
        /// Sets the current employees preference so that they are never prompted to an app review
        /// </summary>
        /// <returns>
        /// <see cref="bool">bool</see> with the outcome of the action.
        /// </returns>
        public bool DoNotPromptEmployeeForAppReviews()
        {
            return this.ActionContext.EmployeeAppReviewPreference.DoNotPromptEmployeeForAppReviews(this.User.EmployeeID,this.User.AccountID);
        }

        /// <summary>
        /// Gets the mobile app feedback categories
        /// </summary>
        /// <returns>
        /// The <see cref="List"/> of <see cref="MobileAppFeedbackCategory">MobileAppFeedbackCategory</see>
        /// </returns>
        public List<MobileAppFeedbackCategory> GetActiveMobileAppFeedbackCategories()
        {
            List<SpendManagementLibrary.MobileAppReview.MobileAppFeedbackCategory> mobileAppFeedbackCategories = MobileAppFeedbackCategoryService.GetActiveMobileAppFeedbackCategories(this.User.AccountID);
            return mobileAppFeedbackCategories.Select(mobileAppFeedbackCategory => new MobileAppFeedbackCategory().ToApiType(mobileAppFeedbackCategory, this.ActionContext)).ToList();
        }
    }
}
