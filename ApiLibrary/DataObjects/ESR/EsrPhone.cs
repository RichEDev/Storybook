namespace ApiLibrary.DataObjects.ESR
{
    using System;
    using System.Runtime.Serialization;
    using ApiLibrary.DataObjects.Base;
    using ApiLibrary.DataObjects.Spend_Management;

    /// <summary>
    /// The ESR phone.
    /// </summary>
    [DataContract(Namespace = "http://software-europe.com/API/2013/02")]
    [DataClass(ElementType = TemplateMapping.ImportElementType.Phone, TableId = "E663770C-FB8D-4EDE-93D4-CB214E944D3A")]
    public class EsrPhone : DataClassBase
    {
        /// <summary>
        /// The ESR phone id.
        /// </summary>
        [DataClass(IsKeyField = true, ColumnRef = 1, FieldId = "A5D8869A-7E28-4F43-A9DB-9DA06A1B6B73")]
        [DataMember(IsRequired = true)]
        public long ESRPhoneId = 0;

        /// <summary>
        /// The ESR person id.
        /// </summary>
        [DataMember(IsRequired = true)]
        [DataClass(ColumnRef = 2, FieldId = "87CB2610-3242-416C-A387-DE7A0B39DD0E")]
        public long ESRPersonId = 0;

        /// <summary>
        /// The phone type.
        /// </summary>
        [DataMember(IsRequired = true)]
        [DataClass(ColumnRef = 3, FieldId = "0BA28C4F-6C01-4397-B21C-D9F8D8E65AC8")]
        public string PhoneType = string.Empty;

        /// <summary>
        /// The phone number.
        /// </summary>
        [DataMember(IsRequired = true)]
        [DataClass(ColumnRef = 4, FieldId = "B8C22CC7-763E-483C-9AF1-C85D3A9C575F")]
        public string PhoneNumber = string.Empty;

        /// <summary>
        /// The effective start date.
        /// </summary>
        [DataMember(IsRequired = true)]
        [DataClass(ColumnRef = 5, FieldId = "14A17772-C54E-4013-A594-46ADCE3C9572")]
        public DateTime EffectiveStartDate ;

        /// <summary>
        /// The effective end date.
        /// </summary>
        [DataMember]
        [DataClass(ColumnRef = 6, FieldId = "CDD39326-EA61-4A3C-9188-21815AC6AD14")]
        public DateTime? EffectiveEndDate = null;

        /// <summary>
        /// The ESR last update.
        /// </summary>
        [DataMember]
        [DataClass(ColumnRef = 7, FieldId = "4F21E9B0-E14E-472F-B696-10E627EEE8A7")]
        public DateTime? ESRLastUpdate = null;
    }
}