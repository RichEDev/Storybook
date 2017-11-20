namespace EsrGo2FromNhsWcfLibrary.ESR
{
    using System.Runtime.Serialization;

    using EsrGo2FromNhsWcfLibrary.Base;

    /// <summary>
    /// The ESR element.
    /// </summary>
    [DataContract(Namespace = "http://software-europe.com/API/2013/02")]
    public class EsrElement : DataClassBase
    {
        /// <summary>
        /// The element id.
        /// </summary>
        [DataClass(IsKeyField = true)]
        [DataMember(IsRequired = true)]
        public int elementID = 0;

        /// <summary>
        /// The global element id.
        /// </summary>
        [DataMember(IsRequired = true)]
        public int globalElementID = 0;

        /// <summary>
        /// The NHS TRUST ID.
        /// </summary>
        [DataMember(IsRequired = true)]
        public int NHSTrustID = 0;
    }
}
