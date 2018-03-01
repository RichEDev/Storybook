namespace SpendManagementLibrary.MobileDeviceNotifications.TemplateManager.MobileDeviceNotifications
{
	/// <summary>
	/// The PushNotificationTemplateIOS
	/// </summary>
	public static class PushNotificationTemplateIOS
	{
		/// <summary>
		/// Gets the notification template for IOS.
		/// </summary>
		/// <param name="notificationMessage">The notification template message</param>
		/// <param name="parameter">The notification parameter</param>
		/// <returns>IOS notification template</returns>
		public static string GetNotificationMessage(string notificationMessage, string notificationParameter)
		{
			string template = template = $"{{\"aps\": {{\"alert\":\"{notificationMessage}\", \"val\":\"{notificationParameter}\" }} }}";
			return template;
		}
	}
}
