namespace SpendManagementLibrary.MobileDeviceNotifications.TemplateManager.MobileDeviceNotifications
{
	/// <summary>
	/// The PushNotificationTemplateUWP
	/// </summary>
	public static class PushNotificationTemplateUWP
	{
		/// <summary>
		/// Gets the notification template for UWP.
		/// </summary>
		/// <param name="notificationMessage">The notification template message</param>
		/// <param name="parameter">The notification parameter</param>
		/// <returns>UWP notification template</returns>
		public static string GetNotificationMessage(string notificationMessage, string notificationParameter)
		{
			string template = $"<?xml version=\"1.0\" encoding=\"utf - 8\"?> < toast launch = \"{notificationParameter}\" > < visual >< binding template = \"ToastText02\">< text id = \"1\" >Expenses</ text >< text id = \"2\">{notificationMessage}</ text ></ binding ></ visual ></ toast >";
			return template;
		}
	}
}
