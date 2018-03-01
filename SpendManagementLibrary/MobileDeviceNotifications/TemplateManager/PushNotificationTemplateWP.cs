namespace SpendManagementLibrary.MobileDeviceNotifications.TemplateManager.MobileDeviceNotifications
{
	/// <summary>
	/// The PushNotificationTemplateWP
	/// </summary>
	public static class PushNotificationTemplateWP 
	{
		/// <summary>
		/// Gets the notification template for WP.
		/// </summary>
		/// <param name="notificationMessage">The notification template message</param>
		/// <param name="parameter">The notification parameter</param>
		/// <returns>WP notification template</returns>
		public static string GetNotificationMessage(string notificationMessage, string notificationParameter)
		{
			string template = $"<?xml version=\"1.0\" encoding=\"utf - 8\"?>< wp:Notification xmlns:wp = \"WPNotification\" >< wp:Toast >< wp:Text1 >Expenses</ wp:Text1 >< wp:Text2 >{notificationMessage}</ wp:Text2 >< wp:Param >?{notificationParameter}</ wp : Param ></ wp:Toast ></ wp:Notification > ";
			return template;
		}
	}
}