namespace DutyOfCareAPI.DutyOfCare.LicenceCheck.VehicleLookup
{
    public class VehicleLookupSuccess : IVehicleLookupResult
    {
        /// <summary>
        /// The code returned from the datasource
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// The message returned from the datasource
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// The vehicle (if any).
        /// </summary>
        public Vehicle Vehicle { get; set; }
    }
}