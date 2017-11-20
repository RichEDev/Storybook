namespace SpendManagementLibrary.MobileInformationMessage
{
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data;

    using SpendManagementLibrary.Helpers;
    using SpendManagementLibrary.Interfaces;

    /// <summary>
    /// The mobile information messages class.
    /// </summary>
    public static class MobileInformationMessages
    {
        /// <summary>
        /// Gets a list of <see cref="MobileInformationMessage"/>
        /// </summary>
        /// <param name="connection">
        /// An instance of <see cref="IDBConnection"/>.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public static List<MobileInformationMessage> GetMobileInformationMessages(IDBConnection connection = null)
        {
            var mobileMessages = new List<MobileInformationMessage>();

            using (var databaseConnection =
                connection ?? new DatabaseConnection(
                    ConfigurationManager.ConnectionStrings["metabase"].ConnectionString))
            {
                databaseConnection.sqlexecute.Parameters.Clear();

                using (IDataReader reader =
                    databaseConnection.GetReader("dbo.GetMobileInformationMessages", CommandType.StoredProcedure))
                {

                    int informationIdOrd = reader.GetOrdinal("informationID");
                    int titleOrd = reader.GetOrdinal("title");
                    int mobileInformationMessageOrd = reader.GetOrdinal("mobileInformationMessage");

                    while (reader.Read())
                    {
                        int informationId = reader.GetInt32(informationIdOrd);
                        string title = reader.GetString(titleOrd) ?? string.Empty;
                        string message = reader.GetString(mobileInformationMessageOrd);

                        var mobileInformationMessage = new MobileInformationMessage(informationId, title, message);
                        mobileMessages.Add(mobileInformationMessage);
                    }

                    return mobileMessages;
                }
            }
        }
    }
}

