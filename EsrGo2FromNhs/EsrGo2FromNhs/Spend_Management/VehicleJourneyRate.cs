namespace EsrGo2FromNhs.Spend_Management
{
    using System;
    using System.Runtime.Serialization;

    using EsrGo2FromNhs.Base;

    /// <summary>
    /// The vehicle journey rate.
    /// </summary>
    [DataContract(Namespace = "http://software-europe.com/API/2013/02")]
    public class VehicleJourneyRate : DataClassBase
    {
        /// <summary>
        /// The mileage id.
        /// </summary>
        [DataMember]
        [DataClass(IsKeyField = true)]
        public int MileageId;

        /// <summary>
        /// The car size.
        /// </summary>
        [DataMember]
        public string CarSize;

        /// <summary>
        /// The comment.
        /// </summary>
        [DataMember]
        public string Comment;

        /// <summary>
        /// The calc miles total.
        /// </summary>
        [DataMember]
        public bool CalcMilesTotal;

        /// <summary>
        /// The created on.
        /// </summary>
        [DataMember]
        public DateTime CreatedOn;

        /// <summary>
        /// The created by.
        /// </summary>
        [DataMember]
        public int CreatedBy;

        /// <summary>
        /// The modified on.
        /// </summary>
        [DataMember]
        public DateTime? ModifiedOn;

        /// <summary>
        /// The modified by.
        /// </summary>
        [DataMember]
        public int? ModifiedBy;

        /// <summary>
        /// The threshold type.
        /// </summary>
        [DataMember]
        public byte ThresholdType;

        /// <summary>
        /// The cat valid.
        /// </summary>
        [DataMember]
        public bool CatValid;

        /// <summary>
        /// The unit.
        /// </summary>
        [DataMember]
        public byte Unit;

        /// <summary>
        /// The cat valid comment.
        /// </summary>
        [DataMember]
        public string CatValidComment;

        /// <summary>
        /// The currency id.
        /// </summary>
        [DataMember]
        public int? CurrencyId;

        /// <summary>
        /// The user rates table.
        /// </summary>
        [DataMember]
        public string UserRatesTable;

        /// <summary>
        /// The user rates from engine size.
        /// </summary>
        [DataMember]
        public int? UserRatesFromEngineSize;

        /// <summary>
        /// The user rates to engine size.
        /// </summary>
        [DataMember]
        public int? UserRatesToEngineSize;
    }
}
