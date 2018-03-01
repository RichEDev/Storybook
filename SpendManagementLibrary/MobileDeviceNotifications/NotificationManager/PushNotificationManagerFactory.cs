namespace SpendManagementLibrary.MobileDeviceNotifications.NotificationManager
{
	using System;

	/// <summary>
	/// The PushNotificationManagerFactory
	/// </summary>
	public static class PushNotificationManagerFactory
	{
		/// <summary>
		/// Gets the platform specific notification manager.
		/// </summary>
		/// <param name="platform">Provided platform</param>
		/// <param name="notificationMessage">Notification Message</param>
		/// <param name="registeredTag">Registered Tag</param>
		/// <returns>Platform specific notification manager</returns>
		public static IPushNotificationManager GetPushNotification(MobileNotificationPlatform platform,string notificationMessage,string registeredTag)
		{
			switch (platform)
			{
				case MobileNotificationPlatform.Apns:
					return new PushNotificationManagerApple(notificationMessage, registeredTag);

				case MobileNotificationPlatform.Gcm:
					return new PushNotificationManagerAndroid(notificationMessage, registeredTag);

				case MobileNotificationPlatform.Wns:
					return new PushNotificationManagerUWP(notificationMessage, registeredTag);

				case MobileNotificationPlatform.Mpns:
					return new PushNotificationManagerWP(notificationMessage, registeredTag);

				default:
					throw new ApplicationException("Push notification manager could not be created.");
			}
		}
	}
}
