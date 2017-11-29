namespace SpendManagementApi.Repositories
{
    using SpendManagementApi.Interfaces;

    using SpendManagementLibrary.MobileMetrics;

    using Spend_Management;
    using System;
    using System.Collections.Generic;

    using SpendManagementLibrary.MobileDeviceNotifications;

    using MobileMetricData = SpendManagementApi.Models.Types.MobileMetricData.MobileMetricData;

    /// <summary>
    /// The mobile metric data repository.
    /// </summary>
    internal class MobileMetricDataRepository : BaseRepository<MobileMetricData>,  ISupportsActionContext
    {
        /// <summary>
        /// Gets or sets the action context.
        /// </summary>
        public IActionContext ActionContext { get; set; }

        public MobileMetricDataRepository(ICurrentUser user, Func<MobileMetricData, int> idSelector, Func<MobileMetricData, string> nameSelector)
            : base(user, idSelector, nameSelector)
        {
        }

        public MobileMetricDataRepository(ICurrentUser user, IActionContext actionContext)
            : base(user, x => (int)x.EmployeeId, x => user.Employee.EmployeeNumber)
        {
        }

        /// <summary>
        /// Updates the database with the mobile metric data.
        /// </summary>
        /// <param name="data">
        /// The data.
        /// </param>
        /// <returns>
        /// The <see cref="int"/> Id of the internal identifier.
        /// </returns>
        public int UpdateMetricData(IList<KeyValuePair<string, string>> data)
        {
            return UpdateMobileMetricData.UpdateDatabase(data, User.AccountID, User.EmployeeID);
        }

        /// <summary>
        /// Sets the mobile device status.
        /// </summary>
        /// <param name="deviceId">
        /// The device id.
        /// </param>
        /// <param name="isActive">
        /// Whether the device is active.
        /// </param>
        /// <param name="allowNotifications">
        /// Whether to allow notifications.
        /// </param>
        /// <returns>
        /// The <see cref="int"/> the notification status, whereby 1 is allow notifications and 0 is disallow notifications.
        /// </returns>
        public int SetMobileDeviceStatus(string deviceId, bool isActive, bool allowNotifications)
        {
            return MobileDeviceStatus.UpdateDatabase(deviceId, isActive, allowNotifications, User.EmployeeID, User.AccountID);
        }

        public override IList<MobileMetricData> GetAll()
        {
            throw new NotImplementedException();
        }

        public override MobileMetricData Get(int id)
        {
            throw new NotImplementedException();
        }

    
    }
}