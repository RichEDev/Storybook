namespace SpendManagementApi.Common.Enum
{
	/// <summary>
	/// The notification platform type.
	/// </summary>
	public enum MobileNotificationPlatform
	{
		/// <summary>
		/// WNS Installation Platform
		/// </summary>
		Wns = 1,

		/// <summary>
		/// Apns Installation Platform
		/// </summary>
		Apns = 2,

		/// <summary>
		/// Mpns Installation Platform
		/// </summary>
		Mpns = 3,

		/// <summary>
		/// Gcm Installation Platform
		/// </summary>
		Gcm = 4,
	}
}