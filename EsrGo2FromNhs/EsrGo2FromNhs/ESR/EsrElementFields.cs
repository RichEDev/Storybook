namespace EsrGo2FromNhs.ESR
{
    using System;
    using System.Runtime.Serialization;

    using EsrGo2FromNhs.Base;

    /// <summary>
    /// The ESR element fields.
    /// </summary>
    [DataContract(Namespace = "http://software-europe.com/API/2013/02")]
    public class EsrElementFields : DataClassBase
    {
        /// <summary>
        /// The element field id.
        /// </summary>
        [DataClass(IsKeyField = true)]
        [DataMember(IsRequired = true)]
        public int elementFieldID;

        /// <summary>
        /// The element id.
        /// </summary>
        [DataMember(IsRequired = true)]
        public int elementID;

        /// <summary>
        /// The global element field id.
        /// </summary>
        [DataMember(IsRequired = true)]
        public int globalElementFieldID;

        /// <summary>
        /// The aggregate.
        /// </summary>
        [DataMember]
        public Int16? aggregate;

        /// <summary>
        /// The order.
        /// </summary>
        [DataMember]
        public Int16 order;

        /// <summary>
        /// The report column id.
        /// </summary>
        [DataMember(IsRequired = true)]
        public Guid reportColumnID;
    }
}
