namespace DutyOfCareAPI.DutyOfCare.LicenceCheck.VehicleLookup
{
    /// <summary>
    /// A interface to define the actions allowed when lookup up vehicles from a data source.
    /// </summary>
    public interface IVehicleLookup
    {
        /// <summary>
        /// Lookup a vehicle based on the registration number
        /// </summary>
        /// <param name="registrationNumber">The registration of the vehicle to look up</param>
        /// <param name="lookupLogger">An instance of <see cref="ILookupLogger"/></param>
        /// <param name="populateDocumentsFromVehicleLookup">Whether vehicle document statuses are invalid after lookup</param>
        /// <returns>An instance of <see cref="IVehicleLookupResult"/></returns>
        IVehicleLookupResult Lookup(string registrationNumber, ILookupLogger lookupLogger, bool populateDocumentsFromVehicleLookup);
    }
}
