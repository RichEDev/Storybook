namespace ApiLibrary.DataObjects.ESR
{
    using System;
    using System.Runtime.Serialization;

    using ApiLibrary.DataObjects.Base;
    using ApiLibrary.DataObjects.Spend_Management;

    /// <summary>
    /// The ESR address.
    /// </summary>
    [DataContract(Namespace = "http://software-europe.com/API/2013/02")]
    [DataClass(ElementType = TemplateMapping.ImportElementType.Address, TableId = "4A793F19-E4A8-4DC9-8249-DE20A53E5BCB")]
    public class EsrAddress : DataClassBase
    {
        /// <summary>
        /// The ESR address id.
        /// </summary>
        [DataClass(IsKeyField = true, FieldId = "C149B36C-D04A-48B0-BB0A-66ABDBE76B8F", ColumnRef = 1)]
        [DataMember(IsRequired = true)]
        public long ESRAddressId;

        /// <summary>
        /// The ESR person id.
        /// </summary>
        [DataMember(IsRequired = true)]
        [DataClass(FieldId = "2AD23788-ED3B-4C67-8547-F57094C4F5E2", ColumnRef = 2)]
        public long ESRPersonId;

        /// <summary>
        /// The address type.
        /// </summary>
        [DataMember]
        [DataClass(FieldId = "C50C92DA-7EA8-4038-AD48-C718B20E4890", ColumnRef = 3)]
        public string AddressType;

        /// <summary>
        /// The address style.
        /// </summary>
        [DataMember(IsRequired = true)]
        [DataClass(FieldId = "371A6BD0-CDB4-492B-BDAB-AF124B749A3A", ColumnRef = 4)]
        public string AddressStyle;

        /// <summary>
        /// The primary flag.
        /// </summary>
        [DataMember(IsRequired = true)]
        [DataClass(FieldId = "71584448-AEF9-4DA7-987F-94DBBD045076", ColumnRef = 5)]
        public string PrimaryFlag;

        /// <summary>
        /// The address line 1.
        /// </summary>
        [DataMember]
        [DataClass(FieldId = "DDDBC0A8-5045-4553-9925-3FF89729B23B", ColumnRef = 6)]
        public string AddressLine1;

        /// <summary>
        /// The address line 2.
        /// </summary>
        [DataMember]
        [DataClass(FieldId = "4B6DF052-BDB8-4E2E-95DA-21826152BF11", ColumnRef = 7)]
        public string AddressLine2;

        /// <summary>
        /// The address line 3.
        /// </summary>
        [DataMember]
        [DataClass(FieldId = "EDC81EDF-9503-4ABC-803B-5EDB333BE635", ColumnRef = 8)]
        public string AddressLine3;

        /// <summary>
        /// The address town.
        /// </summary>
        [DataMember]
        [DataClass(FieldId = "86B3BD14-2B8B-4EDC-B678-BFCC617FDCA8", ColumnRef = 9)]
        public string AddressTown;

        /// <summary>
        /// The address county.
        /// </summary>
        [DataMember]
        [DataClass(FieldId = "1D8AC30D-13A7-4EEC-B10D-2617064C8817", ColumnRef = 10)]
        public string AddressCounty;

        /// <summary>
        /// The address postcode.
        /// </summary>
        [DataMember]
        [DataClass(FieldId = "6B80D26A-064F-4857-BACB-601BFC3EA487", ColumnRef = 11)]
        public string AddressPostcode;

        /// <summary>
        /// The address country.
        /// </summary>
        [DataMember]
        [DataClass(FieldId = "293CB310-BDEC-472A-81E7-324882424A4F", ColumnRef = 12)]
        public string AddressCountry;

        /// <summary>
        /// The effective start date.
        /// </summary>
        [DataMember(IsRequired = true)]
        [DataClass(FieldId = "BF28E2EF-A325-463C-B9A6-26F15AD104B1", ColumnRef = 13)]
        public DateTime EffectiveStartDate;

        /// <summary>
        /// The effective end date.
        /// </summary>
        [DataMember]
        [DataClass(FieldId = "37CD9F5E-8ADD-43D6-9F86-CC2F4E88C912", ColumnRef = 14)]
        public DateTime? EffectiveEndDate;

        /// <summary>
        /// The ESR last update.
        /// </summary>
        [DataMember]
        [DataClass(FieldId = "BE7C11FF-BD09-4A1C-8046-B58054697A57", ColumnRef = 15)]
        public DateTime? ESRLastUpdate;
    }
}
