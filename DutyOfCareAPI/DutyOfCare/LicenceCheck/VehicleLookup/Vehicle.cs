namespace DutyOfCareAPI.DutyOfCare.LicenceCheck.VehicleLookup
{
    /// <summary>
    /// A simple vehicle used for <see cref="IVehicleLookupResult"/>
    /// </summary>
    public class Vehicle
    {
        /// <summary>
        /// Gets or sets the registration of the vehivle
        /// </summary>
        public string RegistrationNumber { get; set; }

        /// <summary>
        /// Gets or sets the model of the vehicle
        /// </summary>
        public string Model { get; set; }

        /// <summary>
        /// Gets or sets the make of the vehicle
        /// </summary>
        public string Make { get; set; }

        /// <summary>
        /// Gets or sets the type of fuel used.
        /// </summary>
        public string FuelType { get; set; }

        /// <summary>
        /// Gets or sets the capacity of the engine
        /// </summary>
        public int EngineCapacity { get; set; }

        /// <summary>
        /// Gets or sets the type of vehicle (car, motorcycle etc.)
        /// </summary>
        public string VehicleType { get; set; }
    }
}