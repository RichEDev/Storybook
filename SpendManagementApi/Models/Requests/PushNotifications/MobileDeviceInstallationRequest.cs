namespace SpendManagementApi.Models.Requests.PushNotifications
{
	using SpendManagementApi.Common.Enum;
	using Common;

	/// <summary>
	/// Request class is been used for installing devices on Azure Push Notifications.
	/// </summary>
	public class MobileDeviceInstallationRequest : ApiRequest
	{
		/// <summary>
		/// Gets or sets the notificationS platform type.
		/// </summary>
		public MobileNotificationPlatform NotificationPlatform { get; set; }

		/// <summary>
		/// Gets or sets tokenid for IOS, registration id for Android and channel uri for Winphone/Windows devices.
		/// </summary>
		public string PushChannel { get; set; }

		/// <summary>
		/// Gets or sets Registration Id of device of Azure Hub.
		/// </summary>
		public string RegistrationId { get; set; }

		/// <summary>
		/// Gets or sets the Mobile Device Id for employee.
		/// </summary>
		public int MobileDeviceId { get; set; }
	}
}