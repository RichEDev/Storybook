namespace SpendManagementLibrary.MobileDeviceNotifications.TemplateManager.MobileDeviceNotifications
{
	/// <summary>
	/// A contract for getting platform specific templates.
	/// </summary>
	public interface IPushNotificationTemplate
	{
		/// <summary>
		/// Gets the notification template.
		/// </summary>
		/// <param name="notificationMessage">The notification template message</param>
		/// <param name="parameter">The notification parameter</param>
		/// <returns>Azure notification template</returns>
		string GetPushNotificationTemplate(string notificationMessage, string parameter);
	}
}
