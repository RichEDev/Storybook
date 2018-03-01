namespace SpendManagementLibrary.MobileDeviceNotifications.TemplateManager.MobileDeviceNotifications
{
	/// <summary>
	/// The PushNotificationTemplateAndroid
	/// </summary>
	public static class PushNotificationTemplateAndroid 
	{
		/// <summary>
		/// Gets the notification template for android platform.
		/// </summary>
		/// <param name="notificationMessage">The notification template message</param>
		/// <param name="parameter">The notification parameter</param>
		/// <returns>Android notification template</returns>
		public static string GetNotificationMessage(string notificationMessage, string notificationParameter)
		{
			string template = $"{{\"data\":{{\"message\":\"{notificationMessage}\", \"val\":\"{notificationParameter}\" }} }}";
			return template;
		}
	}
}
