﻿namespace SpendManagementApi.Repositories
{  
    using SpendManagementApi.Interfaces;
    using Spend_Management;

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
    }
}
