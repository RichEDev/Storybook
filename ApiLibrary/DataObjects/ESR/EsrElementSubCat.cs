namespace ApiLibrary.DataObjects.ESR
{
    using System.Runtime.Serialization;
    using ApiLibrary.DataObjects.Base;

    /// <summary>
    /// The ESR element SUB CATS.
    /// </summary>
    [DataContract(Namespace = "http://software-europe.com/API/2013/02")]
    public class EsrElementSubCat : DataClassBase
    {
        /// <summary>
        /// The element SUBCAT id.
        /// </summary>
        [DataClass(IsKeyField = true)]
        [DataMember(IsRequired = true)]
        public int elementSubcatID = 0;

        /// <summary>
        /// The element id.
        /// </summary>
        [DataMember(IsRequired = true)]
        public int elementID = 0;

        /// <summary>
        /// The SUBCAT id.
        /// </summary>
        [DataMember(IsRequired = true)]
        public int subcatID = 0;
    }
}
