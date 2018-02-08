using System.Linq;
using SpendManagementLibrary.Enumerators;
using SpendManagementLibrary.Helpers;

namespace SpendManagementLibrary.Vehicles
{
    /// <summary>
    /// A class to convert a string type to an enum
    /// </summary>
    public class VehicleTypeFactory
    {
        /// <summary>
        /// Convert a string to a <see cref="SpendManagementLibrary.Enumerators.CarTypes.VehicleType"/>
        /// </summary>
        /// <param name="vehicleType">The type to find</param>
        /// <returns>A valid <see cref="SpendManagementLibrary.Enumerators.CarTypes.VehicleType"/> <seealso cref="SpendManagementLibrary.Enumerators.CarTypes.VehicleType.None"/> when not found</returns>
        public static int Convert(string vehicleType)
        {
            if (vehicleType == "SCOOTER")
            {
                return (int) CarTypes.VehicleType.Moped;
            }

            var values = EnumHelpers<CarTypes.VehicleType>.GetValues(CarTypes.VehicleType.Bicycle);
            return (int) values.FirstOrDefault(type => EnumHelpers<CarTypes.VehicleType>.GetDisplayValue(type).ToLower() == vehicleType.ToLower());
        }
    }
}