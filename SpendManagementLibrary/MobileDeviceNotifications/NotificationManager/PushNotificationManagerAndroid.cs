namespace SpendManagementLibrary.MobileDeviceNotifications.NotificationManager
{
	using Common.Logging;
	using Microsoft.Azure.NotificationHubs;
	using System.Threading.Tasks;

	public class PushNotificationManagerAndroid : IPushNotificationManager
	{
		/// <summary>
		/// An instance of <see cref="ILog"/> for logging diagnostics and information.
		/// </summary>
		private static readonly ILog Log = new LogFactory<PushNotificationManagerAndroid>().GetLogger();

		/// <summary>
		/// Sets the Notification message.
		/// </summary>
		private readonly string notificationMessage;

		/// <summary>
		/// Sets the registered Tag.
		/// </summary>
		private readonly string registeredTag;

		/// <summary>
		/// Initializes a new instance of the <see cref="PushNotificationManagerAndroid"/> class.
		/// </summary>
		/// <param name="notificationMessage">The notification message</param>
		/// <param name="registeredTag">The  registered tag</param>
		public PushNotificationManagerAndroid(string notificationMessage, string registeredTag)
		{
			this.notificationMessage = notificationMessage;
			this.registeredTag = registeredTag;
		}

		/// <summary>
		/// Sends push message to the registered employee.
		/// </summary>
		/// <param name="azureHubConnectionString">The azure hub connection  string </param>
		/// <param name="azureHubName">the azure hub name</param>
		/// <returns>Status of sending push messages</retu
		public async Task<bool> SendPushMessageAsync(string azureHubConnectionString, string azureHubName)
		{
			NotificationHubClient hub = NotificationHubClient.CreateClientFromConnectionString(azureHubConnectionString, azureHubName);
			NotificationOutcome outcome = await hub.SendGcmNativeNotificationAsync(this.notificationMessage, this.registeredTag);

			if (outcome != null)
			{
				if (!(outcome.State == NotificationOutcomeState.Abandoned || outcome.State == NotificationOutcomeState.Unknown))
				{
					return true;
				}
			}

			Log.Error("Push message could not be send to tag : " + this.registeredTag);
			return false;
		}
	}
}
