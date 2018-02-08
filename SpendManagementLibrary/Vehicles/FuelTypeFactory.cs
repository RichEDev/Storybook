using System;
using VehicleEngineType = SpendManagementLibrary.VehicleEngineType;

namespace SpendManagementLibrary.Vehicles
{
    /// <summary>
    /// A class to manage vehicle fuel types from the lookup service
    /// </summary>
    public static class FuelTypeFactory
    {
        /// <summary>
        /// Convert a string to a number based on the test
        /// </summary>
        /// <param name="vehicleFuelType">The string to find</param>
        /// <param name="currentUser">An instance of <see cref="ICurrentUserBase"/></param>
        /// <returns>A valid int referring to a matching fuel type or zero.</returns>
        public static int Convert(string vehicleFuelType, ICurrentUserBase currentUser)
        {
            var vehicleEngineTypes = SpendManagementLibrary.VehicleEngineType.GetAll(currentUser);
            var result = 0;
            foreach (VehicleEngineType type in vehicleEngineTypes)
            {
                if (string.Equals(type.Name, vehicleFuelType, StringComparison.CurrentCultureIgnoreCase) && type.VehicleEngineTypeId.HasValue)
                {
                    return type.VehicleEngineTypeId.Value;
                }
            }

            return result;
        }
    }
}