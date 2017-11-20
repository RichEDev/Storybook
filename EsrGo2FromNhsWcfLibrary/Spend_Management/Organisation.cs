namespace EsrGo2FromNhsWcfLibrary.Spend_Management
{
    using System;
    using System.Runtime.Serialization;

    using EsrGo2FromNhsWcfLibrary.Base;

    /// <summary>
    /// The Organisations data table.
    /// </summary>
    [DataContract(Namespace = "http://software-europe.com/API/2013/02")]
    [DataClass(ElementType = TemplateMapping.ImportElementType.Organisation, TableId = "")]
    public class Organisation : DataClassBase
    {
        [DataMember]
        [DataClass(IsKeyField = true, FieldId = "")]
        public int OrganisationID;

        [DataMember]
        [DataClass(FieldId = "4D0F2409-0705-4F0F-9824-42057B25AEBE")]
        public string Name;

        [DataMember]
        [DataClass(FieldId = "9BB72DFE-CAFD-459C-AF10-97FFCFA1B06F")]
        public int? ParentOrganisationID;

        [DataMember]
        [DataClass(FieldId = "0D08813C-00E3-45EB-BDCC-0EA512615ADE")]
        public string Comment;

        [DataMember]
        [DataClass(FieldId = "AC87C4C4-9107-4555-B2A3-27109B3EBFBB")]
        public string Code;

        [DataMember]
        [DataClass(FieldId = "4B7873D6-8EDC-44D4-94F7-B8ABCBD87692")]
        public bool IsArchived;

        [DataMember]
        [DataClass(FieldId = "B74065BB-6D13-464F-92E8-D6A49AE2F65E")]
        public int? PrimaryAddressID;

        [DataMember]
        [DataClass(FieldId = "")]
        public DateTime? CreatedOn;

        [DataMember]
        [DataClass(FieldId = "")]
        public int? CreatedBy;

        [DataMember]
        [DataClass(FieldId = "")]
        public DateTime? ModifiedOn;

        [DataMember]
        [DataClass(FieldId = "")]
        public int? ModifiedBy;
    }
}
