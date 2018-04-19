namespace BusinessLogic.GeneralOptions.Mileage
{
    /// <summary>
    /// Defines a <see cref="IMileageOptions"/> and it's members
    /// </summary>
    public interface IMileageOptions
    {
        /// <summary>
        /// Gets or sets the mandatory postcode for addresses
        /// </summary>
        bool MandatoryPostcodeForAddresses { get; set; }

        /// <summary>
        /// Gets or sets the record odometer
        /// </summary>
        bool RecordOdometer { get; set; }

        /// <summary>
        /// Gets or sets the odometer day
        /// </summary>
        byte OdometerDay { get; set; }

        /// <summary>
        /// Gets or sets the add locations
        /// </summary>
        bool AddLocations { get; set; }

        /// <summary>
        /// Gets or sets the enter odometer on submit
        /// </summary>
        bool EnterOdometerOnSubmit { get; set; }

        /// <summary>
        /// Gets or sets the allow multiple destinations
        /// </summary>
        bool AllowMultipleDestinations { get; set; }

        /// <summary>
        /// Gets or sets the use map points
        /// </summary>
        bool UseMapPoint { get; set; }

        /// <summary>
        /// Gets or sets the mileage calc type
        /// </summary>
        byte MileageCalcType { get; set; }

        /// <summary>
        /// Gets or sets the auto calculate home to location
        /// </summary>
        bool AutoCalcHomeToLocation { get; set; }

        /// <summary>
        /// Gets or sets the home to office
        /// </summary>
        bool HomeToOffice { get; set; }

        /// <summary>
        /// Gets or sets the mileage
        /// </summary>
        int Mileage { get; set; }
    }
}
