namespace DutyOfCareAPI.DutyOfCare.LicenceCheck.VehicleLookup
{
    using System;
    using System.Globalization;
    using System.Net;

    using DutyOfCareAPI.VDLVehicleLookup;

    /// <summary>
    /// An implementation of <see cref="IVehicleLookup"/> yusing the Licence Check system
    /// </summary>
    public class LicenceCheck : IVehicleLookup
    {
        /// <summary>
        /// The a private _network credential.
        /// </summary>
        private readonly NetworkCredential _networkCredential;

        /// <summary>
        /// Initializes a new instance of the <see cref="LicenceCheck"/> class.
        /// </summary>
        /// <param name="networkCredential">
        /// The network credential to use to connect to Licence check.
        /// </param>
        public LicenceCheck(NetworkCredential networkCredential)
        {
            this._networkCredential = networkCredential;
        }

        /// <summary>
        /// Lookup a vehicle based on the registration number
        /// </summary>
        /// <param name="registrationNumber">The registration number of the vehicle to look up</param>
        /// <param name="lookupLogger">An instance of <see cref="ILookupLogger"/></param>
        /// <param name="populateDocumentsFromVehicleLookup">Whether vehicle document statuses are invalid after lookup</param>
        /// <returns>An instance of <see cref="IVehicleLookupResult"/></returns>
        public IVehicleLookupResult Lookup(
            string registrationNumber,
            ILookupLogger lookupLogger,
            bool populateDocumentsFromVehicleLookup)
        {
            var service = new VDLServiceClient();
            var response = service.GetAdvancedVehicleData(this._networkCredential.UserName, this._networkCredential.Password, registrationNumber);
            lookupLogger.Write(registrationNumber, response.ResponseMessage.Code, response.ResponseMessage.Description);
            if (response.Success)
            {
                var engineCapacity = 0;
                int.TryParse(response.VehicleData.ExactCc, out engineCapacity);
                if (engineCapacity == 0)
                {
                    int.TryParse(response.VehicleData.Cc, out engineCapacity);
                }

                var registrationDate = DateTime.MinValue;
                DateTime.TryParseExact(response.VehicleData.DateOfRegistration, "ddMMyyyy", null, DateTimeStyles.None, out registrationDate);

                var motDueDate = response.VehicleData.MotExpiry == DateTime.MinValue && response.VehicleData.MotStatus == "MOT"
                    ? registrationDate.AddYears(3)
                    : response.VehicleData.MotExpiry;

                var result = new VehicleLookupSuccess
                {
                    Message = response.ResponseMessage.Description,
                    Code = response.ResponseMessage.Code,
                    Vehicle = new Vehicle
                    {
                        RegistrationNumber = response.VehicleData.VRM,
                        Model = response.VehicleData.DvlaModel,
                        Make = response.VehicleData.DvlaMake,
                        FuelType = response.VehicleData.Fuel,
                        EngineCapacity = engineCapacity,
                        VehicleType = string.IsNullOrEmpty(response.VehicleData.VehicleType) ? response.VehicleData.BodyStyle : response.VehicleData.VehicleType,
                        TaxExpiry = response.VehicleData.TaxExpiry,
                        TaxStatus = populateDocumentsFromVehicleLookup ? response.VehicleData.TaxStatus : string.Empty,
                        MotExpiry = motDueDate,
                        MotStatus = populateDocumentsFromVehicleLookup ? response.VehicleData.MotStatus : string.Empty,
                    }
                };

                return result;
            }
            else
            {
                return new VehicleLookupFailed(response.ResponseMessage.Code, response.ResponseMessage.Description);
            }
        }
    }
}
