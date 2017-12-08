namespace SpendManagementLibrary.MobileDeviceNotifications
{
	using System;
	using System.Collections.Generic;

	/// <summary>
	/// The mobile device installation.
	/// </summary>
	public class MobileDeviceInstallation
	{
		/// <summary>
		///  Gets or sets unique identifier for the installation
		/// </summary>
		public string RegistrationId { get; set; }

		/// <summary>
		///   Gets or sets registration id, token or URI obtained from platform-specific notification
		/// </summary>
		public string PushChannel { get; set; }

		/// <summary>
		///     Gets or sets notification platform for the installation
		/// </summary>
		public MobileNotificationPlatform Platform { get; set; }

		/// <summary>
		///     Gets or sets expiration for the installation
		/// </summary>
		public DateTime? ExpirationTime { get; set; }

		/// <summary>
		///    Gets or sets collection of tags
		/// </summary>
		public ISet<string> Tags { get; set; }
	}
}