namespace BusinessLogic.GeneralOptions.Mileage
{
    /// <summary>
    /// Defines a <see cref="MileageOptions"/> and it's members
    /// </summary>
    public class MileageOptions : IMileageOptions
    {
        /// <summary>
        /// Gets or sets the mandatory postcode for addresses
        /// </summary>
        public bool MandatoryPostcodeForAddresses { get; set; }

        /// <summary>
        /// Gets or sets the record odometer
        /// </summary>
        public bool RecordOdometer { get; set; }

        /// <summary>
        /// Gets or sets the odometer day
        /// </summary>
        public byte OdometerDay { get; set; }

        /// <summary>
        /// Gets or sets the add locations
        /// </summary>
        public bool AddLocations { get; set; }

        /// <summary>
        /// Gets or sets the enter odometer on submit
        /// </summary>
        public bool EnterOdometerOnSubmit { get; set; }

        /// <summary>
        /// Gets or sets the allow multiple destinations
        /// </summary>
        public bool AllowMultipleDestinations { get; set; }

        /// <summary>
        /// Gets or sets the use map points
        /// </summary>
        public bool UseMapPoint { get; set; }

        /// <summary>
        /// Gets or sets the mileage calc type
        /// </summary>
        public byte MileageCalcType { get; set; }

        /// <summary>
        /// Gets or sets the auto calculate home to location
        /// </summary>
        public bool AutoCalcHomeToLocation { get; set; }

        /// <summary>
        /// Gets or sets the home to office
        /// </summary>
        public bool HomeToOffice { get; set; }

        /// <summary>
        /// Gets or sets the mileage
        /// </summary>
        public int Mileage { get; set; }
    }
}
