namespace EsrGo2FromNhs.Spend_Management
{
    using System;
    using System.Runtime.Serialization;

    using EsrGo2FromNhs.Base;

    /// <summary>
    /// The user defined match field.
    /// </summary>
    [DataContract(Namespace = "http://software-europe.com/API/2013/02")]
    public class UserDefinedMatchField : DataClassBase
    {
        /// <summary>
        /// The user define field id.
        /// </summary>
        [DataMember(IsRequired = true)]
        [DataClass(IsKeyField = true)]
        public int userdefineid;

        /// <summary>
        /// The field id.
        /// </summary>
        [DataMember]
        public Guid fieldId;
    }
}
