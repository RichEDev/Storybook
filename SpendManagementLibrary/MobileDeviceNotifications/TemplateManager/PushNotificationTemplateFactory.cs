
namespace SpendManagementLibrary.MobileDeviceNotifications.TemplateManager.MobileDeviceNotifications
{
	using System;

	/// <summary>
	/// A Factory class used for creating notification templates. 
	/// </summary>
	public class PushNotificationTemplateFactory
	{
		/// <summary>
		/// Sets the Notification Message
		/// </summary>
		private string notificationMessage;

		/// <summary>
		/// Sets the Notification Parameter
		/// </summary>
		private string notificationParameter;

		/// <summary>
		/// Initializes a new instance of the <see cref="PushNotificationTemplateFactory"/> class.
		/// </summary>
		/// <param name="notificationMessage">The notification message</param>
		/// <param name="notificationParameter">the notification parameter</param>
		public PushNotificationTemplateFactory(string notificationMessage, string notificationParameter)
		{
			this.notificationMessage = notificationMessage;
			this.notificationParameter = notificationParameter;
		}
		/// <summary>
		/// Gets the notification template based on platform <see cref="MobileNotificationPlatform"/>
		/// </summary>
		/// <param name="platform">The mobile notification platform <see cref="MobileNotificationPlatform"/></param>
		/// <returns>Notification template for provided platform.</returns>

		public string GetNotificationMessage(MobileNotificationPlatform platform)
		{
			switch (platform)
			{
				case MobileNotificationPlatform.Apns:
					return PushNotificationTemplateIOS.GetNotificationMessage(this.notificationMessage, this.notificationParameter);

				case MobileNotificationPlatform.Gcm:
					return PushNotificationTemplateAndroid.GetNotificationMessage(this.notificationMessage, this.notificationParameter); 

				case MobileNotificationPlatform.Wns:
					return PushNotificationTemplateUWP.GetNotificationMessage(this.notificationMessage, this.notificationParameter); 

				case MobileNotificationPlatform.Mpns:
					return PushNotificationTemplateWP.GetNotificationMessage(this.notificationMessage, this.notificationParameter); 

				default:
					throw new ApplicationException("Push notification template could not be created.");
			}
		}
	}
}
