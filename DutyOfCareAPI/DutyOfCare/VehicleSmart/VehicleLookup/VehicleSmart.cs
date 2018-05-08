namespace DutyOfCareAPI.DutyOfCare.VehicleSmart.VehicleLookup
{
    using System;
    using System.Configuration;

    using DutyOfCareAPI.DutyOfCare.LicenceCheck.VehicleLookup;
    using RestSharp;

    /// <summary>
    /// An implementation of <see cref="IVehicleLookup"/> that uses the Vehicle Smart service
    /// </summary>
    public class VehicleSmart : IVehicleLookup
    {
        /// <summary>
        /// Lookup a vehicle based on the registration number
        /// </summary>
        /// <param name="registrationNumber">The registration number of the vehicle to look up</param>
        /// <param name="lookupLogger">An instance of <see cref="ILookupLogger"/></param>
        /// <param name="populateDocumentsFromVehicleLookup"></param>
        /// <returns>An instance of <see cref="IVehicleLookupResult"/></returns>
        public IVehicleLookupResult Lookup(
            string registrationNumber,
            ILookupLogger lookupLogger,
            bool populateDocumentsFromVehicleLookup)
        {
            var url = ConfigurationManager.AppSettings["VehicleSmartUrl"];
            var apiKey = ConfigurationManager.AppSettings["VehicleSmartApiKey"];
            var client = new RestClient($"{url}?reg={registrationNumber}&appid={apiKey}&isRefreshing=false&dvsaFallbackMode=false");
            var request = new RestRequest(Method.GET); 
            request.AddHeader("accept", "application/json");
            request.AddHeader("content-type", "application/text");
            IRestResponse<VehicleSmartLookup> response = client.Execute<VehicleSmartLookup>(request);
            lookupLogger.Write(registrationNumber, (int)response.StatusCode, response.StatusDescription);

            if (response.Data != null && response.Data.Success)
            {
                var vehicleData = response.Data.VehicleDetails;
                var newVehicle = false;
                if (string.IsNullOrEmpty(vehicleData.MotExpiryDate))
                {
                    vehicleData.MotExpiryDate = vehicleData.MotExpiryDateDvsa;
                    newVehicle = true;
                }

                var motDueDate = !string.IsNullOrEmpty(vehicleData.MotExpiryDate) && vehicleData.MotExpiryDate.Length > 10 ? DateTime.ParseExact(vehicleData.MotExpiryDate.Substring(0, 10), "yyyy-MM-dd", null) : DateTime.MinValue;
                var motStartDate = newVehicle ? DateTime.ParseExact(vehicleData.DateOfRegistration.Substring(0, 10), "yyyy-MM-dd", null) : motDueDate.AddYears(-1).AddDays(-1);

                if (!vehicleData.Motd && motDueDate > DateTime.Today)
                {
                    vehicleData.Motd = true;
                }

                var taxDueDate = !string.IsNullOrEmpty(vehicleData.TaxDueDate) && vehicleData.TaxDueDate.Length > 10 ? DateTime.ParseExact(vehicleData.TaxDueDate.Substring(0, 10), "yyyy-MM-dd", null) : DateTime.MinValue;
                if (!vehicleData.Taxed && taxDueDate > DateTime.Today)
                {
                    vehicleData.Taxed = true;
                }

                var carType = string.Empty;

                switch (vehicleData.VehicleTypeApproval)
                {
                    case "M1":
                        carType = "CAR";
                        break;
                    case "M2":
                        carType = "MINIBUS";
                        break;
                    case "M3":
                        carType = "MINIBUS";
                        break;
                    case "N1":
                        carType = "VAN";
                        break;
                    case "N2":
                    case "N3":
                        carType = "TRUCK";
                        break;
                    case "L1":
                        carType = "MOPED";
                        break;
                    case "L3":
                    case "L4":
                        carType = "MOTORCYCLE";
                        break;
                }

                if (string.IsNullOrEmpty(carType))
                {
                    switch (vehicleData.Wheelplan)
                    {
                        case "2 WHEEL":
                            carType = "MOTORCYCLE";
                            break;
                        case "2 AXLE RIGID BODY":
                            carType = "CAR";
                            break;
                    }
                }

                var result = new VehicleLookupSuccess
                {
                    Message = response.StatusDescription,
                    Code = ((int)response.StatusCode).ToString(),
                    Vehicle = new Vehicle
                    {
                        RegistrationNumber = vehicleData.Registration,
                        Model = vehicleData.Model,
                        Make = vehicleData.Make,
                        FuelType = vehicleData.Fuel,
                        EngineCapacity = (int)vehicleData.CylinderCapacity,
                        VehicleType = carType,
                        TaxExpiry = taxDueDate,
                        TaxStatus = populateDocumentsFromVehicleLookup ? (vehicleData.Taxed ? "Taxed" : "NotTaxed") : string.Empty,
                        MotExpiry = motDueDate,
                        MotStatus = populateDocumentsFromVehicleLookup ? (vehicleData.Motd ? "MOT" : "MOTINVALID") : string.Empty,
                        MotStart = motStartDate
                    }
                };

                return result;
            }

            return new VehicleLookupFailed(response.StatusCode.ToString(), response.StatusDescription);
        }
    }
}
