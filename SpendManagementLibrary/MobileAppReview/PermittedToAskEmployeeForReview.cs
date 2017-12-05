namespace SpendManagementLibrary.MobileAppReview
{
    using System.Data;

    using SpendManagementLibrary.Helpers;


    /// <summary>
    /// A class to return an employees app store review preferences from the database.
    /// </summary>
    public class EmployeeAppStoreReviewPreference
    {
        /// <summary>
        /// Returns whether the employee has permitted app store reviews to be requested.
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
            bool permittedToAskUserForReview = true;

            using (var databaseConnection = new DatabaseConnection(cAccounts.getConnectionString(accountId)))
            {
                databaseConnection.sqlexecute.Parameters.Clear();
                databaseConnection.sqlexecute.Parameters.AddWithValue("@employeeid", employeeId);
                databaseConnection.sqlexecute.Parameters.Add("@returnVal", SqlDbType.Bit);
                databaseConnection.sqlexecute.Parameters["@returnVal"].Direction = ParameterDirection.ReturnValue;

                using (IDataReader reader = databaseConnection.GetReader(
                    "SELECT NeverPromptForReview FROM EmployeeMobileAppReviewPreferences WHERE employeeId = @employeeId;"))
                {
                    var neverPromptForReviewOrd = reader.GetOrdinal("NeverPromptForReview");

                    while (reader.Read())
                    {
                        var neverPromptForReview = reader.GetBoolean(neverPromptForReviewOrd);
                        permittedToAskUserForReview = !neverPromptForReview;
                    }
                }

                return permittedToAskUserForReview;
            }
        }

        /// <summary>
        /// Sets the current employees preference in the database so that they are never prompted to provide an app review.
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
        public bool SetNeverPromptEmployeeForReview(int employeeId, int accountId)
        {
            using (var databaseConnection = new DatabaseConnection(cAccounts.getConnectionString(accountId)))
            {
                databaseConnection.sqlexecute.Parameters.Clear();
                databaseConnection.sqlexecute.Parameters.AddWithValue("@employeeId", employeeId);
                databaseConnection.sqlexecute.Parameters.Add("@returnValue", SqlDbType.Int);
                databaseConnection.sqlexecute.Parameters["@returnValue"].Direction = ParameterDirection.ReturnValue;

                databaseConnection.ExecuteProc("SetNeverPromptEmployeeForReview");

                var returnValue = (int)databaseConnection.sqlexecute.Parameters["@returnValue"].Value;

                return returnValue > 0;
            }
        }
    }
}

