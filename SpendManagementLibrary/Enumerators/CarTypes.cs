using System.ComponentModel;

namespace SpendManagementLibrary.Enumerators
{
    /// <summary>
    /// The car types Enumerations.
    /// </summary>
    public class CarTypes
    {
        /// <summary>
        /// The car engine type.
        /// See ESRStaticFieldMappings.xml for reference
        /// </summary>
        public enum CarEngineType
        {
            /// <summary>
            /// No engine type set.
            /// </summary>
            None = 0,

            /// <summary>
            /// Petrol engine.
            /// </summary>
            Petrol = 1,

            /// <summary>
            /// Diesel engine.
            /// </summary>
            Diesel = 2,

            /// <summary>
            /// LPG engine.
            /// </summary>
            LPG = 3,

            /// <summary>
            /// Hybrid engine.
            /// </summary>
            Hybrid = 4,

            /// <summary>
            /// Electric engine.
            /// </summary>
            Electric = 5,

            /// <summary>
            /// Diesel euro v engine.
            /// </summary>
            DieselEuroV = 6,

            /// <summary>
            /// Bi-Fuel engine.
            /// </summary>
            BiFuel = 7,

            /// <summary>
            /// Conversion engine.
            /// </summary>
            Conversion = 8,

            /// <summary>
            /// E85 engine.
            /// </summary>
            E85 = 9,

            /// <summary>
            /// Hybrid Electric engine.
            /// </summary>
            HybridElectric = 10,

            /// <summary>
            /// All Other engines.
            /// </summary>
            Other = 99
        }

        /// <summary>
        /// The vehicle type.
        /// </summary>
        public enum VehicleType
        {
            /// <summary>
            /// No vehicle type set.
            /// </summary>
            [Description("None")]
            None = 0,

            /// <summary>
            /// The bicycle.
            /// </summary>
            [Description("Bicycle")]
            Bicycle = 1,

            /// <summary>
            /// The car.
            /// </summary>
            [Description("Car")]
            Car = 2,

            /// <summary>
            /// The motorcycle.
            /// </summary>
            [Description("Motorcycle")]
            Motorcycle = 3,

            /// <summary>
            /// The moped.
            /// </summary>
            [Description("Moped")]
            Moped = 4,

            /// <summary>
            /// LGV
            /// </summary>
            [Description("Light Goods Vehicle (LGV)")]
            LGV = 5,

            /// <summary>
            /// HGV
            /// </summary>
            [Description("Heavy Goods Vehicle (HGV)")]
            HGV = 6,

            /// <summary>
            /// Minibus
            /// </summary>
            [Description("Minibus")]
            Minibus = 7,

            /// <summary>
            /// Bus
            /// </summary>
            [Description("Bus")]
            Bus = 8
        }
    }
}
