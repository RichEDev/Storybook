
using DutyOfCareAPI.DutyOfCare.LicenceCheck.VehicleLookup;
using SpendManagementApi.Common.Enums;
using SpendManagementLibrary.Vehicles;
using Spend_Management;
using Vehicle = SpendManagementApi.Models.Types.Vehicle;

namespace SpendManagementApi.Utilities
{
    /// <summary>
    /// A class to manage the creation of Vehicles based on a given response.
    /// </summary>
    internal class VehicleFactory
    {
        /// <summary>
        /// Creaqte a new <see cref="SpendManagementApi.Models.Types.Vehicle"/> from a given <seealso cref="DutyOfCareAPI.DutyOfCare.LicenceCheck.VehicleLookup.IVehicleLookupResult"/>
        /// </summary>
        /// <param name="lookupResult"></param>
        /// <returns></returns>
        public static Vehicle New(IVehicleLookupResult lookupResult)
        {
            if (lookupResult.Code != "200")
            {
                return null;
            }

            var result = new Vehicle
            {
                EngineSize = lookupResult.Vehicle.EngineCapacity,
                Make = lookupResult.Vehicle.Make,
                Model = lookupResult.Vehicle.Model,
                FuelType = FuelTypeFactory.Convert(lookupResult.Vehicle.FuelType, cMisc.GetCurrentUser()),
                Registration = lookupResult.Vehicle.RegistrationNumber,
                VehicleTypeId = (VehicleType) VehicleTypeFactory.Convert(lookupResult.Vehicle.VehicleType)

            };
            return result;
        }
    }
}