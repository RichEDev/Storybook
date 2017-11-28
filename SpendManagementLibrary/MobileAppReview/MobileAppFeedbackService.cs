namespace SpendManagementLibrary.MobileAppReview
{
    using System;
    using System.Collections.Generic;
    using System.Data;

    using SpendManagementLibrary.Helpers;

    /// <summary>
    /// The mobile app feedback category service.
    /// </summary>
    public static class MobileAppFeedbackService
    {
        /// <summary>
        /// Gets a list of active <see cref="MobileAppFeedbackCategory"/> from the DB
        /// </summary>
        /// <param name="accountId">
        /// The account id.
        /// </param>
        /// <returns>
        /// The <see cref="List"/> of <see cref="MobileAppFeedbackCategory"/>
        /// </returns>
        public static List<MobileAppFeedbackCategory> GetActiveMobileAppFeedbackCategories(int accountId)
        {
            var feedbackCategoriesMaster = new List<MobileAppFeedbackCategory>();
            using (var databaseConnection = new DatabaseConnection(cAccounts.getConnectionString(accountId)))
            {
                databaseConnection.sqlexecute.Parameters.Clear();
                using (IDataReader reader = databaseConnection.GetReader(
                    "SELECT categoryId, description FROM [MobileAppFeedbackCategories] WHERE Active = 1;"))
                {
                    int categoryIdOrd = reader.GetOrdinal("CategoryId");
                    int descriptionOrd = reader.GetOrdinal("Description");

                    while (reader.Read())
                    {
                        int categoryId = reader.GetInt32(categoryIdOrd);
                        string description = reader.GetString(descriptionOrd) ?? string.Empty;

                        var feedbackCategory = new MobileAppFeedbackCategory(categoryId, description);
                        feedbackCategoriesMaster.Add(feedbackCategory);
                    }

                    return feedbackCategoriesMaster;
                }
            }
        }

        /// <summary>
        /// Save mobile app feedback to the DB.
        /// </summary>
        /// <param name="accountId">
        /// The account id.
        /// </param>
        /// <param name="feedbackCategoryId">
        /// The feedback category id.
        /// </param>
        /// <param name="feedback">
        /// The details of the feedback.
        /// </param>
        /// <param name="email">
        /// The email address of the employee providing feedback.
        /// </param>
        /// <param name="mobileMetricId">
        /// The internal mobile metric id.
        /// </param>
        /// <param name="appVersion">
        /// The app version the feedback is against.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/> with the outcome of the save.
        /// </returns>
        public static bool SaveMobileAppFeedback(int accountId, int feedbackCategoryId, string feedback, string email, int mobileMetricId, string appVersion)
        {
            using (var databaseConnection = new DatabaseConnection(cAccounts.getConnectionString(accountId)))
            {
                databaseConnection.sqlexecute.Parameters.Clear();
                databaseConnection.sqlexecute.Parameters.AddWithValue("@FeedbackCategoryID", feedbackCategoryId);
                databaseConnection.sqlexecute.Parameters.AddWithValue("@Feedback", feedback);

                if (email == string.Empty)
                {
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@Email", DBNull.Value);
                }
                else
                {
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@Email", email);
                }

                databaseConnection.sqlexecute.Parameters.AddWithValue("@MobileMetricId", mobileMetricId);
                databaseConnection.sqlexecute.Parameters.AddWithValue("@AppVersion", appVersion);
                databaseConnection.sqlexecute.Parameters.Add("@ReturnValue", SqlDbType.Int);
                databaseConnection.sqlexecute.Parameters["@ReturnValue"].Direction = ParameterDirection.ReturnValue;

                databaseConnection.ExecuteProc("SaveMobileAppFeedback");

                var returnValue = (int)databaseConnection.sqlexecute.Parameters["@ReturnValue"].Value;

                return returnValue > 0;
            }
        }
    }
}
