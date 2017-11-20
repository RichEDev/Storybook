namespace SpendManagementLibrary.MobileDeviceNotifications
{
    using SpendManagementLibrary.Helpers;
    using SpendManagementLibrary.Interfaces;

    /// <summary>
    /// The set mobile device status.
    /// </summary>
    public static class MobileDeviceStatus
    {
        /// <summary>
        /// Updates database with the status for the device.
        /// </summary>
        /// <param name="deviceId">
        /// The device Id.
        /// </param>
        /// <param name="isActive">
        /// Whether the device is active.
        /// </param>
        /// <param name="allowNotifications">
        /// Whether to allow notifications.
        /// </param>
        /// <param name="employeeId">
        /// The employee Id.
        /// </param>
        /// <param name="accountId">
        /// The account Id.
        /// </param>
        /// <param name="connection">
        /// An instance of <see cref="IDBConnection"/>
        /// </param>
        /// <returns>
        /// The <see cref="int"/> the notification status, whereby 1 is allow notifications and 0 is disallow notifications.
        /// </returns>
        public static int UpdateDatabase(string deviceId, bool isActive, bool allowNotifications, int employeeId, int accountId, IDBConnection connection = null)
        {
            using (var databaseConnection =
                connection ?? new DatabaseConnection(cAccounts.getConnectionString(accountId)))
            {
                databaseConnection.sqlexecute.Parameters.AddWithValue("@EmployeeId", employeeId);
                databaseConnection.sqlexecute.Parameters.AddWithValue("@deviceId", deviceId);
                databaseConnection.sqlexecute.Parameters.AddWithValue("@allowNotifications", allowNotifications);
                databaseConnection.sqlexecute.Parameters.AddWithValue("@active", isActive);    
                databaseConnection.ExecuteProc("SetMobileDeviceStatus");
            }

            return allowNotifications ? 1 : 0;
        }
    }
}
