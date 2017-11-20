namespace EsrGo2FromNhs.Spend_Management
{
    using System;
    using System.Runtime.Serialization;
    using Base;

    /// <summary>
    /// The template mapping.
    /// </summary>
    [DataContract(Namespace = "http://software-europe.com/API/2013/02")]
    public class Template : DataClassBase
    {
        /// <summary>
        /// The template mapping id.
        /// </summary>
        [DataMember(IsRequired = true)]
        [DataClass(IsKeyField = true)]
        public int templateID;
        /// <summary>
        /// The template Name.
        /// </summary>
        [DataMember]
        public string templateName;
        /// <summary>
        /// The destination field.
        /// </summary>
        [DataMember]
        public short applicationType;
        /// <summary>
        /// The column ref.
        /// </summary>
        [DataMember]
        public bool isAutomated;
        /// <summary>
        /// The import element type.
        /// </summary>
        [DataMember]
        public int NHSTrustID;
        /// <summary>
        /// The mandatory.
        /// </summary>
        [DataMember]
        public DateTime createdOn;
        /// <summary>
        /// The data type.
        /// </summary>
        [DataMember]
        public int createdBy;
        /// <summary>
        /// The lookup table.
        /// </summary>
        [DataMember]
        public DateTime modifiedOn;
        /// <summary>
        /// The match field.
        /// </summary>
        [DataMember]
        public int modifiedBy;
        /// <summary>
        /// The override primary key.
        /// </summary>
        [DataMember]
        public Guid SignOffOwnerFieldId;
        /// <summary>
        /// The import field.
        /// </summary>
        [DataMember]
        public Guid LineManagerFieldId;
    }
}
