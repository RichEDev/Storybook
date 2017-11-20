namespace ApiLibrary.DataObjects.Spend_Management
{
    using System.Runtime.Serialization;
    using ApiLibrary.DataObjects.Base;

    /// <summary>
    /// Lookup related table results.
    /// </summary>
    [DataContract(Namespace = "http://software-europe.com/API/2013/02")]
    [DataClass(ElementType = TemplateMapping.ImportElementType.None)]
    public class Lookup : DataClassBase
    {
        /// <summary>
        /// Gets or sets the first column name.
        /// </summary>
        [DataMember]
        public string FirstColumnName { get; set; }

        /// <summary>
        /// Gets or sets the first column value.
        /// </summary>
        [DataMember]
        public long? FirstColumnValue { get; set; }

        /// <summary>
        /// Gets or sets the second column name.
        /// </summary>
        [DataMember]
        public string SecondColumnName { get; set; }

        /// <summary>
        /// Gets or sets the second column value.
        /// </summary>
        [DataMember]
        public long? SecondColumnValue { get; set; }
    }
}
