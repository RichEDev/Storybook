namespace EsrGo2FromNhs.Spend_Management
{
    using System;
    using System.Runtime.Serialization;

    using EsrGo2FromNhs.Base;

    /// <summary>
    /// The field.
    /// </summary>
    [DataContract(Namespace = "http://software-europe.com/API/2013/02")]
    [DataClass(ElementType = TemplateMapping.ImportElementType.None)]
    public class Field : DataClassBase
    {
        /// <summary>
        /// The field id.
        /// </summary>
        [DataMember]
        public Guid fieldId;

        /// <summary>
        /// The tableid.
        /// </summary>
        [DataMember]
        public Guid tableid;

        /// <summary>
        /// The description.
        /// </summary>
        [DataMember]
        public string description;

        /// <summary>
        /// The field name.
        /// </summary>
        [DataMember]
        public string field;

        /// <summary>
        /// The table name.
        /// </summary>
        [DataMember]
        public string tablename;

        /// <summary>
        /// Gets or sets the element type.
        /// </summary>
        [DataMember]
        public TemplateMapping.ImportElementType ElementType { get; set; }
    }
}
