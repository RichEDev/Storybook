namespace DutyOfCareAPI.DutyOfCare.LicenceCheck.VehicleLookup
{
    public class VehicleLookupFailed : IVehicleLookupResult
    {
        public VehicleLookupFailed(string code, string message)
        {
            this.Code = code;
            this.Message = message;
        }

        /// <summary>
        /// The code returned from the datasource
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// The error message
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Always null for an error state
        /// </summary>
        public Vehicle Vehicle { get; set; }
    }
}