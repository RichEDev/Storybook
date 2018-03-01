namespace SpendManagementLibrary.MobileMetrics
{
	/// <summary>
	/// The mobile metric data.
	/// </summary>
	public class MobileMetricData
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="MobileMetricData"/> class.
		/// </summary>
		/// <param name="mobileDeviceId">
		/// The mobile Device Id.
		/// </param>
		/// <param name="employeeId">
		/// The employee Id.
		/// </param>
		/// <param name="allowNotifications">
		/// The allow Notifications.
		/// </param>
		/// <param name="registered">
		/// The registered.
		/// </param>
		/// <param name="pushChannel">
		/// The push Channel.
		/// </param>
		/// <param name="registrationId">
		/// The Registration Id.
		/// </param>
		/// <param name="registeredTag">
		/// The Registered Tag.
		/// </param>
		/// <param name="registeredPlatform">
		/// The Registered Platform.
		/// </param>
		public MobileMetricData(int mobileDeviceId, int employeeId, bool allowNotifications, bool registered, string pushChannel, string registrationId, string registeredTag,string registeredPlatform)
		{
			this.MobileDeviceId = mobileDeviceId;
			this.RegistrationId = registrationId;
			this.PushChannel = pushChannel;
			this.Registered = registered;
			this.AllowNotifications = allowNotifications;
			this.EmployeeId = employeeId;
			this.RegisteredTag = registeredTag;
			this.MobilePlatform = registeredPlatform;
		}

		/// <summary>
		/// Gets the mobile device id.
		/// </summary>
		public int MobileDeviceId { get; }

		/// <summary>
		/// Gets the employee id.
		/// </summary>
		public int EmployeeId { get; }

		/// <summary>
		/// Gets a value indicating whether allowed notifications or not.
		/// </summary>
		public bool AllowNotifications { get; }

		/// <summary>
		/// Gets a value indicating whether registered or not for push notifications.
		/// </summary>
		public bool Registered { get; }

		/// <summary>
		/// Gets the push channel.
		/// </summary>
		public string PushChannel { get; }

		/// <summary>
		/// Gets the registration id.
		/// </summary>
		public string RegistrationId { get; }

		/// <summary>
		/// Gets the Registered Tag.
		/// </summary>
		public string RegisteredTag { get; }

		/// <summary>
		/// Gets the Mobile Platform.
		/// </summary>
		public string MobilePlatform { get; }
	}
}
