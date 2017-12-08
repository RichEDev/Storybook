namespace SpendManagementLibrary.MobileDeviceNotifications
{
	using System.Threading.Tasks;
	using System.Collections.Generic;
	using System;
	using Common.Logging;
	using Microsoft.Azure.NotificationHubs;


	/// <summary>
	/// The MobileDeviceInstallationService
	/// </summary>
	public class MobileDeviceInstallationService
	{
		/// <summary>
		/// An instance of <see cref="ILog"/> for logging diagnostics and information.
		/// </summary>
		private static readonly ILog Log = new LogFactory<MobileDeviceInstallationService>().GetLogger();

		/// <summary>
		/// Azure Hub Connection String with full access.
		/// </summary>
		private readonly string azureHubConnectionString;

		/// <summary>
		/// Azure hub Name.
		/// </summary>
		private readonly string azureHubName;

		/// /// <summary>
		/// Initializes a new instance of the <see cref="MobileDeviceInstallationService"/> class.
		/// </summary>
		/// <param name="azureHubConnectionString">Azure Hub Connection String with Full Access </param>
		/// <param name="azureHubName">Azure Hub Name</param>
		public MobileDeviceInstallationService(string azureHubConnectionString, string azureHubName)
		{
			this.azureHubConnectionString = azureHubConnectionString;
			this.azureHubName = azureHubName;
		}

		/// <summary>
		/// Method is used for registering the devices with Azure Push Notification Services.
		/// </summary>
		/// <param name="deviceRegistration">Mobile Device Installation details <see cref="MobileDeviceInstallation"/>.</param>
		/// <returns>Returns Mobile Device Installation detail after registration.</returns>
		public async Task<MobileDeviceInstallation> Regsiter(MobileDeviceInstallation deviceRegistration)
		{
			NotificationHubClient hub = NotificationHubClient.CreateClientFromConnectionString(this.azureHubConnectionString, this.azureHubName);
			if (string.IsNullOrWhiteSpace(deviceRegistration.RegistrationId))
			{
				deviceRegistration.RegistrationId = await hub.CreateRegistrationIdAsync();
			}

			RegistrationDescription registration = null;
			switch (deviceRegistration.Platform)
			{
				case MobileNotificationPlatform.Mpns:
					registration = new MpnsRegistrationDescription(deviceRegistration.PushChannel);
					break;
				case MobileNotificationPlatform.Wns:
					registration = new WindowsRegistrationDescription(deviceRegistration.PushChannel);
					break;
				case MobileNotificationPlatform.Apns:
					registration = new AppleRegistrationDescription(deviceRegistration.PushChannel);
					break;
				case MobileNotificationPlatform.Gcm:
					registration = new GcmRegistrationDescription(deviceRegistration.PushChannel);
					break;
			}

			if (registration != null)
			{
				registration.RegistrationId = deviceRegistration.RegistrationId;
				registration.Tags = new HashSet<string>(deviceRegistration.Tags);
				try
				{
					await hub.CreateOrUpdateRegistrationAsync(registration);
					return deviceRegistration;
				}
				catch (ArgumentNullException)
				{
					Log.Error("The device registration for push notification service could not be done because RegistrationDescription object is null.");
					return null;
				}
			}

			Log.Error("The device registration for push notification service could not be done because RegistrationDescription object is null.");
			return null;
		}

		/// <summary>
		/// Method is used for deregistering the devices with Azure Push Notification Services.
		/// </summary>
		/// <param name="deviceRegistration">Mobile Device Installation details <see cref="MobileDeviceInstallation"/>.</param>
		/// <returns>Returns true for successful deregistration otherwise false.</returns>
		public async Task<bool> DeRegister(MobileDeviceInstallation deviceRegistration)
		{
			try
			{

				NotificationHubClient hub = NotificationHubClient.CreateClientFromConnectionString(this.azureHubConnectionString, this.azureHubName);
				await hub.DeleteRegistrationAsync(deviceRegistration.RegistrationId);
				return true;
			}
			catch
			{
				Log.Error("The device unregistration for push notification service could not be done because RegistrationId is not valid.");
				return false;
			}

		}

		/// <summary>
		///  Updates the registration time to live for devices on hub. Do not call it once time to live has been set.
		/// </summary>
		/// <returns>Returns true for successful otherwise false.</returns>
		public async Task<bool> UpdateRegistrationTimeOnHub()
		{
			try
			{
				var namespaceManager = NamespaceManager.CreateFromConnectionString(this.azureHubConnectionString);
				NotificationHubDescription notificationHubDescription = namespaceManager.GetNotificationHub(this.azureHubName);
				notificationHubDescription.RegistrationTtl = TimeSpan.FromDays(60.0);
				await namespaceManager.UpdateNotificationHubAsync(notificationHubDescription);
				return true;
			}
			catch
			{
				Log.Error("The total time to live for registration could not be updated.");
				return false;
			}

		}
	}
}