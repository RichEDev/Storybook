namespace SpendManagementApi.Models.Requests.MobileMetricData
{
    using SpendManagementApi.Models.Common;

    /// <summary>
    /// The set device status request.
    /// </summary>
    public class SetDeviceStatusRequest : ApiRequest
    {
        /// <summary>
        /// Gets or sets the device id.
        /// </summary>
        public string DeviceId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the device is active.
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the device is allowed to receive notifications.
        /// </summary>
        public bool AllowNotifications { get; set; }
    }
}