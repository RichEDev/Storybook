namespace SpendManagementLibrary.Mobile
{
    /// <summary>
    /// A mobile device used in the system
    /// </summary>
    public class MobileDevice
    {
        /// <summary>
        /// Empty constructor for Ajax - not to be used
        /// </summary>
        public MobileDevice()
        {
        }
  
        /// <summary>
        /// Initialises a new instance of the <see cref="MobileDevice"/> class. 
        /// Constructor for MobileDevice
        /// </summary>
        /// <param name="mobileDeviceId">
        /// The mobile device ID (primary key)
        /// </param>
        /// <param name="employeeId">
        /// The employeeID of the employee who owns this mobile device
        /// </param>
        /// <param name="mobileDeviceType">
        /// The mobile device type
        /// </param>
        /// <param name="deviceName">
        /// The name given to this device by the user
        /// </param>
        /// <param name="pairingKey">
        /// The key used on the mobile device to pair with the system
        /// </param>
        /// <param name="serialKey">
        /// The unique serial key for the mobile device paired with
        /// </param>
        public MobileDevice(int mobileDeviceId, int employeeId, MobileDeviceType mobileDeviceType, string deviceName, string pairingKey, string serialKey)
        {
            this.MobileDeviceID = mobileDeviceId;
            this.EmployeeID = employeeId;
            this.DeviceType = mobileDeviceType;
            this.DeviceName = deviceName;
            this.PairingKey = pairingKey;
            this.SerialKey = serialKey;
        }    

        #region Properties - All properties are get and set for the ajax interface
        /// <summary>
        /// The mobile device ID (primary key)
        /// </summary>
        public int MobileDeviceID { get; set; }

        /// <summary>
        /// The employeeID of the employee who owns this mobile device
        /// </summary>
        public int EmployeeID { get; set; }

        /// <summary>
        /// The mobile device type
        /// </summary>
        public MobileDeviceType DeviceType { get; set; }

        /// <summary>
        /// The name given to this device by the user
        /// </summary>
        public string DeviceName { get; set; }

        /// <summary>
        /// The model/make of the phone
        /// </summary>
        public string DeviceModel { get; set; }

        /// <summary>
        /// The key used on the mobile device to pair with the system
        /// </summary>
        public string PairingKey { get; set; }

        /// <summary>
        /// The serial key of the device - only available after pairing
        /// </summary>
        public string SerialKey { get; set; }

        /// <summary>
        /// Returns true if the device has been paired with the system
        /// </summary>
        public bool IsPaired
        {
            get { return !string.IsNullOrWhiteSpace(SerialKey); }
        }

        #endregion Properties - All properties are get and set for the ajax interface
    }
}
