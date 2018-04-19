namespace BusinessLogic.GeneralOptions.Cars
{
    /// <summary>
    /// Defines a <see cref="ICarOptions"/> and it's members
    /// </summary>
    public interface ICarOptions
    {
        /// <summary>
        /// Gets or sets a value indicating whether show mileage categories for users.
        /// </summary>
        bool ShowMileageCatsForUsers { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to activate a car on user add.
        /// </summary>
        bool ActivateCarOnUserAdd { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to allow users to add cars.
        /// </summary>
        bool AllowUsersToAddCars { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to allow an employee to specify car DOC on adding a vehicle.
        /// </summary>
        bool AllowEmpToSpecifyCarDOCOnAdd { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to allow an employee to specify car start date on adding a vehicle.
        /// </summary>
        bool AllowEmpToSpecifyCarStartDateOnAdd { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether a car start date is mandatory on add.
        /// </summary>
        bool EmpToSpecifyCarStartDateOnAddMandatory { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether vehicle lookup is enabled.
        /// </summary>
        bool VehicleLookup { get; set; }

        /// <summary>
        /// Gets or sets the driving licence review reminder days
        /// </summary>
        byte DrivingLicenceReviewReminderDays { get; set; }
    }
}
