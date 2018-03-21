namespace DutyOfCareAPI.DutyOfCare.VehicleSmart.VehicleLookup
{
    /// <summary>
    /// The result of a VehicleSmart lookup (http://getvehiclesmart.com/)
    /// </summary>
    public class VehicleSmartLookup
    {
        /// <summary>
        /// Gets or sets a value indicating whether the lookup was a success
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="VehicleDetails"/>
        /// </summary>
        public VehicleDetails VehicleDetails { get; set; }
    }
}
