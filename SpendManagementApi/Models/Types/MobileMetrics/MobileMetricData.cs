namespace SpendManagementApi.Models.Types.MobileMetrics
{
	using Interfaces;

	/// <summary>
	/// The mobile metric data class.
	/// </summary>
	public class MobileMetricData : BaseExternalType, IBaseClassToAPIType<SpendManagementLibrary.MobileMetrics.MobileMetricData, MobileMetricData>
	{
		/// <summary>
		/// Gets or sets the mobile device id.
		/// </summary>
		public int MobileDeviceId { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether allowed notifications or not.
		/// </summary>
		public bool AllowNotifications { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether registered or not for push notifications.
		/// </summary>
		public bool Registered { get; set; }

		/// <summary>
		/// Gets or sets the push channel.
		/// </summary>
		public string PushChannel { get; set; }

		/// <summary>
		/// Gets or sets the registration id.
		/// </summary>
		public string RegistrationId { get; set; }

		/// <summary>
		/// Gets or sets the Registered Tag.
		/// </summary>
		public string RegisteredTag { get; set; }

		/// <summary>
		/// Converts a spend management library type to a API type
		/// </summary>
		/// <param name="dbType">
		/// The db type.
		/// </param>
		/// <param name="actionContext">
		/// The action context.
		/// </param>
		/// <returns>
		/// The <see cref="MobileMetricData"/>.
		/// </returns>
		public MobileMetricData ToApiType(SpendManagementLibrary.MobileMetrics.MobileMetricData dbType, IActionContext actionContext)
		{
			if (dbType == null)
			{
				return null;
			}

			this.MobileDeviceId = dbType.MobileDeviceId;
			this.AllowNotifications = dbType.AllowNotifications;
			this.Registered = dbType.Registered;
			this.PushChannel = dbType.PushChannel;
			this.RegistrationId = dbType.RegistrationId;
			this.EmployeeId = dbType.EmployeeId;
			this.RegisteredTag = dbType.RegisteredTag;

			return this;
		}
	}
}