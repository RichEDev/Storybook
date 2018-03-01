namespace SpendManagementLibrary.MobileDeviceNotifications.NotificationManager
{
	using System.Threading.Tasks;

	/// <summary>
	/// The IPushNotificationManager Contract.
	/// </summary>
	public interface IPushNotificationManager
	{
		/// <summary>
		/// Sends push messages to the users.
		/// </summary>
		/// <param name="azureConnectionString">Azure coonection string</param>
		/// <param name="azureHubName">Azure hub name</param>
		/// <returns>A Task<see cref="Task"/> containing the outcome of operation.</returns>
		Task<bool> SendPushMessageAsync(string azureConnectionString,string azureHubName);
	}
}
