namespace SpendManagementApi.Models.Requests
{
    using Common;

    /// <summary>
    /// Facilitates the finding of MobileDevices, by providing a few optional search / filter parameters.
    /// </summary>
    public class FindMobileDevicesRequest : FindRequest
    {
        /// <summary>
        /// Search by name.
        /// </summary>
        public string DeviceName { get; set; }

        /// <summary>
        /// Search by The Id of the MobileDeviceType.
        /// </summary>
        public int? Type { get; set; }

        /// <summary>
        /// Search by Employee Id.
        /// </summary>
        public int? EmployeeId { get; set; }
    }
}