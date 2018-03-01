using Common.Logging;

namespace SpendManagementApi.Repositories
{
	using System.Collections.Generic;
	using Spend_Management;
	using Models.Requests.PushNotifications;
	using System;
	using API = Models.Types.MobileMetrics;
	using SpendManagementLibrary.MobileDeviceNotifications;
	using SpendManagementLibrary.MobileMetrics;
	using System.Threading.Tasks;
	using System.Configuration;
	using BusinessLogic.Cache;

	/// <summary>
	/// The push notifications repository.
	/// </summary>
	internal class PushNotificationsRepository : BasicRepository
	{
		/// <summary>
		/// An instance of <see cref="ILog"/> for logging <see cref="ICache{T,TK}"/> diagnostics and information.
		/// </summary>
		private static readonly ILog Log = new LogFactory<PushNotificationsRepository>().GetLogger();

		/// <summary>
		/// Azure Hub Connection String with full access.
		/// </summary>
		private readonly string azureHubConnectionString;

		/// <summary>
		/// Azure hub Name.
		/// </summary>
		private readonly string azureHubName;

		/// <summary>
		/// Initializes a new instance of the <see cref="PushNotificationsRepository"/> class.
		/// </summary>
		/// <param name="user">
		/// The user. <see cref="ICurrentUser"/>
		/// </param>
		public PushNotificationsRepository(ICurrentUser user)
			: base(user)
		{
			this.azureHubConnectionString = ConfigurationManager.AppSettings["AzureHubConnectionString"];
			this.azureHubName = ConfigurationManager.AppSettings["AzureHubName"];
		}

		/// <summary>
		///  Method used for registering device on azure hub for push notification.
		/// </summary>
		/// <param name="request">The MobileDeviceInstallationRequest <see cref="MobileDeviceInstallationRequest"/></param>
		/// <returns>The MobileMetricData <see cref="MobileMetricData"/></returns>
		public async Task<API.MobileMetricData> RegisterForPushNotifications(MobileDeviceInstallationRequest request)
		{
			try
			{
				var mobileDeviceInstallationService = new MobileDeviceInstallationService(this.azureHubConnectionString, this.azureHubName);

				MobileDeviceInstallation mobileDeviceInstallation = new MobileDeviceInstallation
				{
					Platform = (MobileNotificationPlatform)request.NotificationPlatform,
					RegistrationId = request.RegistrationId,
					PushChannel = request.PushChannel,
					Tags = new HashSet<string> { request.MobileDeviceId + this.User.EmployeeID.ToString() + this.User.AccountID }
				};

				var result = await mobileDeviceInstallationService.RegisterAsync(mobileDeviceInstallation);
				if (result != null)
				{
					return this.UpdateAndGetMobileMetricDetails(request.MobileDeviceId, true, result);
				}
			}
			catch (ArgumentNullException)
			{
				Log.Error("MobileDeviceInstallation object is null for registering mobile device for push notification services.");
			}
			catch (InvalidOperationException)
			{
				Log.Error("MobileDeviceInstallation registration id is null for registering mobile device for push notification services.");
			}
			catch (Exception exception)
			{
				Log.Error(exception.Message);
			}

			return null;
		}

		/// <summary>
		///  Method used for unregistering device on azure hub for push notification.
		/// </summary>
		/// <param name="request">The MobileDeviceInstallationRequest <see cref="MobileDeviceInstallationRequest"/></param>
		/// <returns>The MobileMetricData <see cref="MobileMetricData"/></returns>
		public async Task<API.MobileMetricData> UnregisterForPushNotifications(MobileDeviceInstallationRequest request)
		{
			try
			{
				var mobileDeviceInstallationService = new MobileDeviceInstallationService(this.azureHubConnectionString, this.azureHubName);

				MobileDeviceInstallation mobileDeviceInstallation = new MobileDeviceInstallation
				{
					Platform = (MobileNotificationPlatform)request.NotificationPlatform,
					RegistrationId = request.RegistrationId,
					PushChannel = request.PushChannel,
					Tags = new HashSet<string> { request.MobileDeviceId + this.User.EmployeeID.ToString() + this.User.AccountID }
				};

				var result = await mobileDeviceInstallationService.DeRegisterAsync(mobileDeviceInstallation);

				if (result)
				{
					return this.UpdateAndGetMobileMetricDetails(request.MobileDeviceId, false, mobileDeviceInstallation);
				}
			}
			catch (ArgumentNullException)
			{
				Log.Error("MobileDeviceInstallation object is null for unregistering mobile device for push notification services.");
			}
			catch (InvalidOperationException)
			{
				Log.Error("MobileDeviceInstallation registration id is null for unregistering mobile device for push notification services.");
			}
			catch (Exception exception)
			{
				Log.Error(exception.Message);
			}

			return null;
		}

		/// <summary>
		///  Updates the registartion time to live for devices on hub. Do not call it once time to live has been set.
		/// </summary>
		/// <returns>Returns true for successful otherwise false.</returns>
		public async Task<bool> UpdateRegistrationTimeOnHub()
		{
			return await new MobileDeviceInstallationService(this.azureHubConnectionString, this.azureHubName).UpdateRegistrationTimeOnHubAsync();
		}

		/// <summary>
		/// Updates the mobile Metric details after push operations.
		/// </summary>
		/// <param name="mobileDeviceId">The mobile device id</param>
		/// <param name="isRegistered">Indicates whether device registered or not. </param>
		/// <param name="mbileDeviceInstallation">MobileDeviceInstalltion instance <see cref="MobileDeviceInstallation"/></param>
		/// <returns> The <see cref="API.MobileMetricData"/> MobileMetricData.</returns>
		private API.MobileMetricData UpdateAndGetMobileMetricDetails(int mobileDeviceId, bool isRegistered, MobileDeviceInstallation mbileDeviceInstallation)
		{
			var mobilemetricData = UpdateMobileMetricData.UpdateDatabase(mobileDeviceId, isRegistered, mbileDeviceInstallation, this.User.AccountID, this.User.EmployeeID);

			if (mobilemetricData == null)
			{
				Log.Error("Error occurred when updating Mobile Metric details after push notifications operation.");
			}

			return new API.MobileMetricData().ToApiType(mobilemetricData, this.ActionContext);
		}
	}
}