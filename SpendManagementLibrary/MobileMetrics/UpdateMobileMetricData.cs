using System.Linq;

namespace SpendManagementLibrary.MobileMetrics
{
	using System.Collections.Generic;
	using System.Data;
	using Helpers;
	using MobileDeviceNotifications;
	using System;

	/// <summary>
	/// The update mobile metric data.
	/// </summary>
	public static class UpdateMobileMetricData
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
		/// <returns>
		/// Returns MobileMetricData <see cref="MobileMetricData"/> for the employee.
		/// </returns>
		public static MobileMetricData UpdateDatabase(IList<KeyValuePair<string, string>> metricData, int accountId, int employeeId)
		{
			using (var databaseConnection = new DatabaseConnection(cAccounts.getConnectionString(accountId)))
			{
				databaseConnection.sqlexecute.Parameters.Clear();
				foreach (var record in metricData)
				{
					databaseConnection.sqlexecute.Parameters.AddWithValue("@" + record.Key, record.Value);
				}

				databaseConnection.sqlexecute.Parameters.AddWithValue("@EmployeeId", employeeId);

				var reader = databaseConnection.GetReader("dbo.UpdateMobileMetricData", CommandType.StoredProcedure);

				var idOrd = reader.GetOrdinal("Id");
				var allowNotificationsOrd = reader.GetOrdinal("AllowNotifications");
				var registeredOrd = reader.GetOrdinal("Registered");
				var pushChannelOrd = reader.GetOrdinal("PushChannel");
				var registrationIdOrd = reader.GetOrdinal("RegistrationId");
				var registeredTagOrd = reader.GetOrdinal("RegisteredTag");

				while (reader.Read())
				{
					int id = reader.GetInt32(idOrd);
					bool allowNotifications = reader.GetBoolean(allowNotificationsOrd);
					bool registered = reader.GetBoolean(registeredOrd);
					string pushChannel = Convert.ToString(reader.GetString(pushChannelOrd));
					string registrationId = Convert.ToString(reader.GetString(registrationIdOrd));
					string registeredTag = Convert.ToString(reader.GetString(registeredTagOrd));

					var mobileMetricData = new MobileMetricData(id, employeeId, allowNotifications, registered, pushChannel, registrationId, registeredTag);
					return mobileMetricData;
				}

				return null;
			}
		}

		/// <summary>
		/// Updates the Mobile Metric Details with the push notification capabilities.
		/// </summary>
		/// <param name="mobileDeviceId">
		/// </param>
		/// The mobile metric mobile device id.
		/// <param name="isRegistered">
		/// Indicates whether device registered or not. 
		/// </param>
		/// <param name="mobileDeviceInstallation">
		/// The mobile device installation.
		/// </param>
		/// <param name="accountId">
		/// The account id.
		/// </param>
		/// <param name="employeeId">
		/// The employee Id.
		/// </param>
		/// <returns>
		/// Returns MobileMetricData <see cref="MobileMetricData"/> of the employee.
		/// </returns>
		public static MobileMetricData UpdateDatabase(int mobileDeviceId, bool isRegistered, MobileDeviceInstallation mobileDeviceInstallation, int accountId, int employeeId)
		{
			using (var databaseConnection = new DatabaseConnection(cAccounts.getConnectionString(accountId)))
			{
				databaseConnection.sqlexecute.Parameters.Clear();

				databaseConnection.sqlexecute.Parameters.AddWithValue("@MobileDeviceId", mobileDeviceId);
				databaseConnection.sqlexecute.Parameters.AddWithValue("@Registered", isRegistered);
				databaseConnection.sqlexecute.Parameters.AddWithValue("@PushChannel", mobileDeviceInstallation.PushChannel);
				databaseConnection.sqlexecute.Parameters.AddWithValue("@RegistrationId", mobileDeviceInstallation.RegistrationId);
				databaseConnection.sqlexecute.Parameters.AddWithValue("@RegisteredTag", mobileDeviceInstallation.Tags.First());

				var reader = databaseConnection.GetReader("dbo.UpdatePushNotificationData", CommandType.StoredProcedure);

				var idOrd = reader.GetOrdinal("Id");
				var allowNotificationsOrd = reader.GetOrdinal("AllowNotifications");
				var registeredOrd = reader.GetOrdinal("Registered");
				var pushChannelOrd = reader.GetOrdinal("PushChannel");
				var registrationIdOrd = reader.GetOrdinal("RegistrationId");
				var registeredTagOrd = reader.GetOrdinal("RegisteredTag");

				while (reader.Read())
				{
					int id = reader.GetInt32(idOrd);
					bool allowNotifications = reader.GetBoolean(allowNotificationsOrd);
					bool registered = reader.GetBoolean(registeredOrd);
					string pushChannel = Convert.ToString(reader.GetString(pushChannelOrd));
					string registrationId = Convert.ToString(reader.GetString(registrationIdOrd));
					string registeredTag = Convert.ToString(reader.GetString(registeredTagOrd));

					var mobileMetricData = new MobileMetricData(id, employeeId, allowNotifications, registered, pushChannel, registrationId, registeredTag);
					return mobileMetricData;
				}

				return null;
			}
		}
	}
}