namespace SpendManagementLibrary.Interfaces
{
    /// <summary>
    /// An interface to define the operations allowed when validating a vehicle.
    /// </summary>
    public interface IVehicleValidator
    {
        /// <summary>
        /// Validate a car against an external service.
        /// </summary>
        /// <param name="vehicle">An instance of <see cref="cCar"/>to validate</param>
        /// <returns>An updated instance of <see cref="cCar"/></returns>
        cCar ValidateCar(cCar vehicle);
    }
}