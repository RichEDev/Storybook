namespace EsrGo2FromNhsWcfLibrary.Spend_Management
{
    using System.Runtime.Serialization;

    using EsrGo2FromNhsWcfLibrary.Base;

    /// <summary>
    /// The car assignment number allocation.
    /// </summary>
    [DataContract(Namespace = "http://software-europe.com/API/2013/02")]
    [DataClass(ElementType = TemplateMapping.ImportElementType.None, TableId = "")]
    public class CarAssignmentNumberAllocation : DataClassBase
    {
        /// <summary>
        /// The ESR vehicle allocation id.
        /// </summary>
        [DataClass(IsKeyField = true)]
        [DataMember]
        public long ESRVehicleAllocationId;

        /// <summary>
        /// The ESR assign id.
        /// </summary>
        [DataMember]
        public int ESRAssignId;

        /// <summary>
        /// The car id.
        /// </summary>
        [DataMember]
        public int CarId;

        /// <summary>
        /// The archived flag.
        /// </summary>
        [DataMember]
        public bool Archived;
    }
}
