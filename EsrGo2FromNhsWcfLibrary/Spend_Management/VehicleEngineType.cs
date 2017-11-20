namespace EsrGo2FromNhsWcfLibrary.Spend_Management
{
    using System;
    using System.Runtime.Serialization;

    using Base;

    /// <summary>
    /// The vehicle engine type.
    /// </summary>
    [DataContract(Namespace = "http://software-europe.com/API/2013/02")]
    public class VehicleEngineType : DataClassBase
    {
        /// <summary>
        /// The vehicle engine type id.
        /// </summary>
        [DataMember]
        [DataClass(IsKeyField = true)]
        public int VehicleEngineTypeId;

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
        /// The code.
        /// </summary>
        [DataMember]
        public string Code;

        /// <summary>
        /// The name.
        /// </summary>
        [DataMember]
        public string Name;

    }
}
