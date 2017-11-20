namespace SpendManagementApi.Models
{
    using SpendManagementApi.Common.Enums;
    using SpendManagementApi.Models.Types;

    using SpendManagementLibrary;

    /// <summary>
    /// The vehicle basic class.
    /// </summary>
    public class VehicleBasic
    {
        /// <summary>
        /// The unique ID of this vehicle.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Get or Sets the vehcile description
        /// </summary>
        public string VehicleDescription { get; set; }

        /// <summary>
        /// Gets or sets the unit of measure.
        /// </summary>
        public MileageUOM UnitOfMeasure { get; set; }

        /// <summary>
        /// Gets or sets the registration.
        /// </summary>
        public string Registration { get; set; }

        /// <summary>
        /// Gets or sets the vehicle type id.
        /// </summary>
        public VehicleType VehicleTypeId { get; set; }
 
    }
}