namespace DutyOfCareAPI.DutyOfCare.LicenceCheck.VehicleLookup
{
    /// <summary>
    /// An interface to define the result of a vehicle lookup.
    /// </summary>
    public interface IVehicleLookupResult
    {
        /// <summary>
        /// The code returned from the datasource
        /// </summary>
        string Code {get;set; }

        /// <summary>
        /// The message returned from the datasource
        /// </summary>
        string Message { get;set; }

        /// <summary>
        /// The vehicle (if any).
        /// </summary>
        Vehicle Vehicle { get; set; }
    }
}