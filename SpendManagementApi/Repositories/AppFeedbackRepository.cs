﻿namespace SpendManagementApi.Repositories
{
    using System.Collections.Generic;
    using System.Linq;

    using SpendManagementApi.Interfaces;
    using SpendManagementApi.Models.Requests.MobileAppFeedback;

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
            List<SpendManagementLibrary.MobileAppReview.MobileAppFeedbackCategory> mobileAppFeedbackCategories = MobileAppFeedbackService.GetActiveMobileAppFeedbackCategories(this.User.AccountID);
            return mobileAppFeedbackCategories.Select(mobileAppFeedbackCategory => new MobileAppFeedbackCategory().ToApiType(mobileAppFeedbackCategory, this.ActionContext)).ToList();
        }


        /// <summary>
        /// Passes the mobile app feedback request to the MobileAppFeedbackService.
        /// </summary>
        /// <param name="request">
        /// The <see cref="MobileAppFeedbackRequest"/>
        /// </param>
        /// <returns>
        /// The <see cref="bool"/> with the outcome of the action.
        /// </returns>
        public bool SaveMobileAppFeedback(MobileAppFeedbackRequest request)
        {     
            if (MobileAppFeedbackService.SaveMobileAppFeedback(
                this.User.AccountID,
                request.FeedbackCategoryId,
                request.Feedback,
                request.Email,
                request.MobileMetricId,
                request.AppVersion))
            {
                //Saved to database, so generate email to notify service desk.
                var feedbackCategory = MobileAppFeedbackService.GetMobileAppFeedbackCategoryDescriptionById(request.FeedbackCategoryId, User.AccountID);
                bool sendEmailOutcome = new MobileAppFeedbackEmailGenerator().SendEmailToServiceDesk(this.User,feedbackCategory , request.Feedback, request.Email, request.AppVersion, request.MobileMetricId);
                return sendEmailOutcome;
            }

            return false;
        }
    }
}
