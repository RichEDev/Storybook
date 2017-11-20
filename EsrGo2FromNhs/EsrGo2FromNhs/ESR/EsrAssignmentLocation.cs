namespace EsrGo2FromNhs.ESR
{
    using System;
    using System.Runtime.Serialization;

    using EsrGo2FromNhs.Base;

    /// <summary>
    /// The ESR assignments Data Class.
    /// </summary>
    [DataContract(Namespace = "http://software-europe.com/API/2013/02")]
    [DataClass]
    public class EsrAssignmentLocation : DataClassBase
    {
        /// <summary>
        /// The esr assign id. 
        /// </summary>
        [DataMember(IsRequired = true)]
        [DataClass]
        public int EsrAssignId;

        /// <summary>
        /// The esr location id.
        /// </summary>
        [DataMember(IsRequired = true)]
        [DataClass]
        public long EsrLocationId;

        /// <summary>
        /// The start date
        /// </summary>
        [DataMember(IsRequired = true)]
        [DataClass]
        public DateTime StartDate;

        /// <summary>
        /// The date this record was deleted
        /// </summary>
        [DataMember]
        [DataClass]
        public DateTime? DeletedDateTime;
    }
}
