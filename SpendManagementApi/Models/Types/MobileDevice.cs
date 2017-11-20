namespace SpendManagementApi.Models.Types
{
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using Spend_Management;
    using Interfaces;

    /// <summary>
    /// Represents a mobile device which has access to the Mobile Expenses 360 application.
    /// </summary>
    public class MobileDevice : BaseExternalType, IApiFrontForDbObject<SpendManagementLibrary.Mobile.MobileDevice, MobileDevice>
    {
        /// <summary>
        /// The unique Id of the mobile device entry in the database.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The employee that this mobile device relates to.
        /// </summary>
        public new int EmployeeId { get; set; }

        /// <summary>
        /// The Ids of the type of the device - see the <see cref="MobileDeviceType">MobileDeviceType</see>.
        /// </summary>
        [Required]
        public int? Type { get; set; }

        /// <summary>
        /// The name given to this device, or example, "Ben's Nexus 5".
        /// </summary>
        [Required]
        public string DeviceName { get; set; }

        /// <summary>
        /// The key required by the mobile device to activate the app with the system.
        /// Leave this as null if creating a new MobileDevice, since it will be generated
        /// and returned for use on the Employee's Expenses360 application.
        /// </summary>
        public string ActivationKey { get; internal set; }

        /// <summary>
        /// Convert from a data access layer Type to an api Type.
        /// </summary>
        /// <param name="dbType">The instance of the data access layer Type to convert from.</param>
        /// <param name="actionContext">The IActionContext.</param>
        /// <returns>An api Type</returns>
        public MobileDevice From(SpendManagementLibrary.Mobile.MobileDevice dbType, IActionContext actionContext)
        {
            Id = dbType.MobileDeviceID;
            EmployeeId = dbType.EmployeeID;
            Type = new MobileDeviceType().From(dbType.DeviceType, actionContext).DeviceTypeId;
            DeviceName = dbType.DeviceName;
            ActivationKey = dbType.PairingKey;
            return this;
        }

        /// <summary>
        /// Converts to a data access layer Type from an api Type.
        /// </summary>
        /// <returns>A data access layer Type</returns>
        public SpendManagementLibrary.Mobile.MobileDevice To(IActionContext actionContext)
        {
            // get the correct type.
            var type = actionContext.MobileDevices.MobileDeviceTypes.FirstOrDefault(t => t.Key == Type).Value;
            return new SpendManagementLibrary.Mobile.MobileDevice(Id, EmployeeId, type, DeviceName, ActivationKey, null);
        }
    }


    /// <summary>
    /// Represents a type of mobile device.
    /// </summary>
    public class MobileDeviceType : IApiFrontForDbObject<SpendManagementLibrary.Mobile.MobileDeviceType, MobileDeviceType>
    {
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
        internal int DeviceOsTypeId { get; set; }

        /// <summary>
        /// Convert from a data access layer Type to an api Type.
        /// </summary>
        /// <param name="dbType">The instance of the data access layer Type to convert from.</param>
        /// <param name="actionContext">The IActionContext.</param>
        /// <returns>An api Type</returns>
        public MobileDeviceType From(SpendManagementLibrary.Mobile.MobileDeviceType dbType, IActionContext actionContext)
        {
            DeviceTypeId = dbType.DeviceTypeId;
            DeviceTypeDescription = dbType.DeviceTypeDescription;
            DeviceOsTypeId = dbType.DeviceOsTypeId;
            return this;
        }

        /// <summary>
        /// Converts to a data access layer Type from an api Type.
        /// </summary>
        /// <returns>A data access layer Type</returns>
        public SpendManagementLibrary.Mobile.MobileDeviceType To(IActionContext actionContext)
        {
            return new SpendManagementLibrary.Mobile.MobileDeviceType(DeviceTypeId,
                                                                    DeviceTypeDescription,
                                                                    DeviceOsTypeId);
        }
    }
}