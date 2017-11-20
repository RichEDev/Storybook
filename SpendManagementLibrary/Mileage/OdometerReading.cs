namespace SpendManagementLibrary.Mileage
{
    using SpendManagementLibrary.Enumerators;

    /// <summary>
    /// The odometer reading.
    /// </summary>
    public class OdometerReading
    {
        /// <summary>
        /// Gets or sets the ID of the car the reading belongs to
        /// </summary>
        public int CarID { get; set; }

        /// <summary>
        /// Gets or sets the car make and model
        /// </summary>
        public string CarMakeModel { get; set; }

        /// <summary>
        /// Gets or sets the registration of the car
        /// </summary>
        public string CarRegistration { get; set; }

        /// <summary>
        /// Gets or Sets the Type of Vehicle, i.e. car, bike
        /// </summary>
        public CarTypes.VehicleType VehicleType { get; set; }
     
        /// <summary>
        /// Gets or sets the date the last odometer reading was taken
        /// </summary>
        public string LastReadingDate { get; set; }

        /// <summary>
        /// Gets or sets the last odometer reading
        /// </summary>
        public int LastReading { get; set; }
    }
}
