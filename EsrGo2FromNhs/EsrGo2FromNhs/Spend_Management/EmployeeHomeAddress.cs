namespace EsrGo2FromNhs.Spend_Management
{
    using System;
    using System.Runtime.Serialization;

    using EsrGo2FromNhs.Base;

    /// <summary>
    /// The employee home address.
    /// </summary>
    [DataContract(Namespace = "http://software-europe.com/API/2013/02")]
    [DataClass(ElementType = TemplateMapping.ImportElementType.None, TableId = "5026DDAE-5042-45B6-A1CD-6C6AB5FF8204")]
    public class EmployeeHomeAddress : DataClassBase
    {
        [DataMember]
        [DataClass(FieldId = "3BDB3BD7-6017-43FB-BF67-650F4EC01C5F", IsKeyField = true)]
        public int EmployeeHomeAddressId;

        [DataMember]
        [DataClass(FieldId = "682133C3-7807-4DC4-BA54-1E78EB9738EE")]
        public int EmployeeId;

        [DataMember]
        [DataClass(FieldId = "1A6D18F5-0538-4737-BD4D-6D6B1B0726AA")]
        public int AddressId;

        [DataMember]
        [DataClass(FieldId = "A7F445B1-B5AF-4AA4-B199-2D0C84611071")]
        public DateTime? StartDate;

        [DataMember]
        [DataClass(FieldId = "507D86FC-1187-467B-91C9-E133834AD561")]
        public DateTime? EndDate;

        [DataMember]
        [DataClass(FieldId = "02C1DEC7-910A-4CCE-B9AE-E815B5C28B75")]
        public DateTime? CreatedOn;

        [DataMember]
        [DataClass(FieldId = "1141A989-1632-4CD5-8A27-E28009EA0FC4")]
        public int? CreatedBy;

        [DataMember]
        [DataClass(FieldId = "3363937B-811E-4A6C-8CBC-E347819B8B3E")]
        public DateTime? ModifiedOn;

        [DataMember]
        [DataClass(FieldId = "29B46900-C373-4B5B-90DE-467F94682816")]
        public int? ModifiedBy;
    }
}
