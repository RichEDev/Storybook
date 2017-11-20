namespace SpendManagementLibrary.Mobile
{
    /// <summary>
    /// MobileDeviceType class
    /// </summary>
    public class MobileDeviceType
    {
        /// <summary>
        /// Empty constructor for Ajax - not to be used
        /// </summary>
        public MobileDeviceType()
        {
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="MobileDeviceType"/> class. 
        /// Constructor for MobileDeviceType
        /// </summary>
        /// <param name="mobileDeviceTypeId">
        /// The mobile device type id
        /// </param>
        /// <param name="mobileDeviceTypeDescription">
        /// The mobile device type description
        /// </param>
        /// <param name="mobileDeviceOsTypeId">
        /// The Mobile Device Operating System type
        /// </param>
        public MobileDeviceType(int mobileDeviceTypeId, string mobileDeviceTypeDescription, int mobileDeviceOsTypeId)
        {
            this.DeviceTypeId = mobileDeviceTypeId;
            this.DeviceTypeDescription = mobileDeviceTypeDescription;
            this.DeviceOsTypeId = mobileDeviceOsTypeId;
        }
        #region Properties

        /// <summary>
        /// Gets or Sets the Device Type Id
        /// </summary>
        public int DeviceTypeId { get; set; }

        /// <summary>
        /// Gets or Sets the Type Description
        /// </summary>
        public string DeviceTypeDescription { get; set; }

        /// <summary>
        /// Gets or Sets the Operating System Type Id
        /// </summary>
        public int DeviceOsTypeId { get; set; }

        #endregion Properties
    }
}
