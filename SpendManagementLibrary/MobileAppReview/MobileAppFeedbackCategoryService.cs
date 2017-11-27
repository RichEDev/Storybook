namespace SpendManagementLibrary.MobileAppReview
{
    using System.Collections.Generic;
    using System.Data;

    using SpendManagementLibrary.Helpers;

    /// <summary>
    /// The mobile app feedback category service.
    /// </summary>
    public static class MobileAppFeedbackCategoryService
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
    }
}
