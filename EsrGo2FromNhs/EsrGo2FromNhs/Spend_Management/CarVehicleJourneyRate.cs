namespace EsrGo2FromNhs.Spend_Management
{
    using System.Runtime.Serialization;

    using EsrGo2FromNhs.Base;

    /// <summary>
    /// The car vehicle journey rate.
    /// </summary>
    public class CarVehicleJourneyRate : DataClassBase
    {
        /// <summary>
        /// The car id.
        /// </summary>
        [DataMember]
        [DataClass(IsKeyField = true)]
        public int CarId;

        /// <summary>
        /// The mileage id.
        /// </summary>
        [DataMember]
        public int MileageId;
    }
}
