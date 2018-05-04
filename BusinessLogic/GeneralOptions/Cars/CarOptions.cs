namespace BusinessLogic.GeneralOptions.Cars
{
    /// <summary>
    /// The car options.
    /// </summary>
    public class CarOptions : ICarOptions
    {
        /// <summary>
        /// Gets or sets a value indicating whether show mileage categories for users.
        /// </summary>
        public bool ShowMileageCatsForUsers { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to activate a car on user add.
        /// </summary>
        public bool ActivateCarOnUserAdd { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to allow users to add cars.
        /// </summary>
        public bool AllowUsersToAddCars { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to allow an employee to specify car DOC on adding a vehicle.
        /// </summary>
        public bool AllowEmpToSpecifyCarDOCOnAdd { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to allow an employee to specify car start date on adding a vehicle.
        /// </summary>
        public bool AllowEmpToSpecifyCarStartDateOnAdd { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether a car start date is mandatory on add.
        /// </summary>
        public bool EmpToSpecifyCarStartDateOnAddMandatory { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether vehicle lookup is enabled.
        /// </summary>
        public bool PopulateDocumentsFromVehicleLookup { get; set; }

        /// <summary>
        /// Gets or sets the driving licence review reminder days
        /// </summary>
        public byte DrivingLicenceReviewReminderDays { get; set; }
    }
}
