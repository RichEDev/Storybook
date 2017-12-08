namespace SpendManagementApi.Repositories
{
	using System.Collections.Generic;
	using Models.Requests.MobileMetricData;
	using Spend_Management;
	using SpendManagementLibrary.MobileDeviceNotifications;
	using SpendManagementLibrary.MobileMetrics;
	using API = Models.Types.MobileMetrics;

	/// <summary>
	/// The mobile metric data repository.
	/// </summary>
	internal class MobileMetricDataRepository : BasicRepository
    {
		/// <summary>
		/// Initializes a new instance of the <see cref="MobileMetricDataRepository"/> class.
		/// </summary>
		/// <param name="user">
		/// The user.
		/// </param>
		public MobileMetricDataRepository(ICurrentUser user)
			: base(user)
		{
			// Do  Nothing
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
			var mobilemetricData = UpdateMobileMetricData.UpdateDatabase(data, this.User.AccountID, this.User.EmployeeID);
			return mobilemetricData?.MobileDeviceId ?? 0;
		}

		/// <summary>
		/// Updates the database with the mobile metric data.
		/// </summary>
		/// <param name="request">
		/// The data <see cref="MobileMetricDataRequest"/>.
		/// </param>
		/// <returns>
		/// The <see cref="API.MobileMetricData"/> MobileMetricData.
		/// </returns>
		public API.MobileMetricData UpdateMetricData(MobileMetricDataRequest request)
	    {
			var mobilemetricData = UpdateMobileMetricData.UpdateDatabase(request.MetricData, this.User.AccountID, this.User.EmployeeID);

		    if (mobilemetricData == null)
		    {
			    return null;
		    }

			return new API.MobileMetricData().ToApiType(mobilemetricData, this.ActionContext);
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
            return MobileDeviceStatus.UpdateDatabase(deviceId, isActive, allowNotifications, this.User.EmployeeID, this.User.AccountID);
        }
    }
}