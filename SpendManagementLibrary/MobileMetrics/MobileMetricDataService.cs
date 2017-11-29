namespace SpendManagementLibrary.MobileMetrics
{
    using System.Collections.Generic;
    using System.Data;

    using SpendManagementLibrary.Helpers;
    using SpendManagementLibrary.Interfaces;

    /// <summary>
    /// The update mobile metric data.
    /// </summary>
    public static class MobileMetricDataService
    {
        /// <summary>
        /// Updates the database with the mobile metric data.
        /// </summary>
        /// <param name="metricData">
        /// The metric data.
        /// </param>
        /// <param name="accountId">
        /// The account id.
        /// </param>
        /// <param name="employeeId">
        /// The employee Id.
        /// </param>
        /// <param name="connection">
        /// An instance of <see cref="IDBConnection"/>.
        /// </param>
        /// <returns>
        /// The <see cref="int"/> with the internal identifer for the record.
        /// </returns>
        public static int UpdateDatabase(IList<KeyValuePair<string, string>> metricData, int accountId, int employeeId, IDBConnection connection = null)
        {
            int mobileId;
            using (var databaseConnection =
                connection ?? new DatabaseConnection(cAccounts.getConnectionString(accountId)))
            {
                foreach (var record in metricData)
                {
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@" + record.Key, record.Value);
                }

                databaseConnection.sqlexecute.Parameters.AddWithValue("@EmployeeId", employeeId);

                databaseConnection.sqlexecute.Parameters.Add("@identity", System.Data.SqlDbType.Int);
                databaseConnection.sqlexecute.Parameters["@identity"].Direction = ParameterDirection.ReturnValue;

                databaseConnection.ExecuteProc("UpdateMobileMetricData"); 
                mobileId = (int)databaseConnection.sqlexecute.Parameters["@identity"].Value;
            }

            return mobileId;
        }

        //public static IList<KeyValuePair<string, string>> GetMobileMetricDataForId(int id, int accountId)
        //{        
        //    List<KeyValuePair<string, string>> metricData = new List<KeyValuePair<string, string>>();

        //    using (var databaseConnection = new DatabaseConnection(cAccounts.getConnectionString(accountId)))
        //    {
        //        databaseConnection.sqlexecute.Parameters.Clear();

        //        using (IDataReader reader =
        //            databaseConnection.GetReader("Select ", CommandType.Text))
        //        {

        //            int informationIdOrd = reader.GetOrdinal("informationID");
        //            int titleOrd = reader.GetOrdinal("title");
        //            int mobileInformationMessageOrd = reader.GetOrdinal("mobileInformationMessage");

        //            while (reader.Read())
        //            {
                     
        //            }

        //            return mobileMessages;
        //        }
        //    }

        //    return metricData;

        //} 
    }
}
