namespace SpendManagementLibrary.Mobile
{
    /// <summary>
    /// The mobile device Operating System type.
    /// </summary>
    public class MobileDeviceOsType
    {
        #region Private Variables

        /// <summary>
        /// The mobile device Operating System type id.
        /// </summary>
        private int mobileDeviceOsTypeId;

        /// <summary>
        /// The mobile device install from location.
        /// </summary>
        private string mobileDeviceInstallFrom;

        /// <summary>
        /// The mobile device image to display in the register device screen.
        /// </summary>
        private string mobileDeviceImage;

        #endregion

        /// <summary>
        /// Initialises a new instance of the <see cref="MobileDeviceOsType"/> class.
        /// </summary>
        /// <param name="mobileDevicesOsTypeId">
        /// The mobile devices Operating System type id.
        /// </param>
        /// <param name="mobileInstallFrom">
        /// The mobile install from text.
        /// </param>
        /// <param name="mobileImage">
        /// The mobile image.
        /// </param>
        public MobileDeviceOsType(int mobileDevicesOsTypeId, string mobileInstallFrom, string mobileImage)
        {
            this.mobileDeviceOsTypeId = mobileDevicesOsTypeId;
            this.mobileDeviceInstallFrom = mobileInstallFrom;
            this.mobileDeviceImage = mobileImage;
        }

        /// <summary>
        /// Gets or sets the mobile device Operating System type id.
        /// </summary>
        public int MobileDeviceOsTypeId
        {
            get
            {
                return this.mobileDeviceOsTypeId;
            }
            set
            {
                this.mobileDeviceOsTypeId = value;
            }
        }

        /// <summary>
        /// Gets or sets the mobile device install from path.
        /// </summary>
        public string MobileDeviceInstallFrom
        {
            get
            {
                return this.mobileDeviceInstallFrom;
            }
            set
            {
                this.mobileDeviceInstallFrom = value;
            }
        }

        /// <summary>
        /// Gets or sets the mobile device image when user is registering a device.
        /// </summary>
        public string MobileDeviceImage
        {
            get
            {
                return this.mobileDeviceImage;
            }
            set
            {
                this.mobileDeviceImage = value;
            }
        }
    }
}
