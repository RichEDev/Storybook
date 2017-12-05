namespace SpendManagementLibrary.MobileAppReview
{
    using System;
    using SpendManagementLibrary.Interfaces;

    /// <summary>
    /// The mobile app review.
    /// </summary>
    public class EmployeeAppReviewPreference
    {
        /// <summary>
        /// A class which determines if an employee should be prompted to provide an app review.
        /// Checks if the employee has specifically opted out of giving reviews, if they haven't then a random mechanism will determine if the employee
        /// should be prompted to provide a review 
        /// </summary>
        /// <param name="employeeId">
        /// The employee id.
        /// </param>
        /// <param name="accountId">
        /// The account id.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool PermittedToAskEmployeeForReview(int employeeId, int accountId)
        {
            bool permittedToAskEmployeeForReview = new EmployeeAppStoreReviewPreference().PermittedToAskEmployeeForReview(employeeId, accountId);
            
            if (!permittedToAskEmployeeForReview)
            {
                return false;
            }

            return ShouldRandomlyRequestAppReview();

        }

        /// <summary>
        /// Sets the current employees preference so that they are never prompted to provide an app review
        /// </summary>
        /// <param name="employeeId">
        /// The employee id.
        /// </param>
        /// <param name="accountId">
        /// The account id.
        /// </param>
        /// <returns>
        /// <see cref="bool">bool</see> with the outcome of the action.
        /// </returns>
        public bool DoNotPromptEmployeeForAppReviews(int employeeId, int accountId)
        {
            bool outcome = new EmployeeAppStoreReviewPreference().SetNeverPromptEmployeeForReview(employeeId, accountId);
            return outcome;
        }


        /// <summary>
        /// Determines if an employee should be selected to provide an app review, with a 1 in 20 chance of returning true.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool ShouldRandomlyRequestAppReview()
        {
            Random random = new Random();
            int randomNumber = random.Next(0, 20);
            return randomNumber == 10;
        }
    }
}
