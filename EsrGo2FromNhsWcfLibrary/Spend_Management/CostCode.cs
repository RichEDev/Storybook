namespace EsrGo2FromNhsWcfLibrary.Spend_Management
{
    using System;
    using System.Runtime.Serialization;

    using EsrGo2FromNhsWcfLibrary.Base;

    /// <summary>
    /// The cost code.
    /// </summary>
    [DataContract(Namespace = "http://software-europe.com/API/2013/02")]
    [DataClass(ElementType = TemplateMapping.ImportElementType.Costing, TableId = "02009E21-AA1D-4E0D-908A-4E9D73DDFBDF")]
    public class CostCode : DataClassBase
    {
        [DataMember(IsRequired = true)]
        [DataClass(IsKeyField = true, FieldId = "177E6AC3-DF38-45F7-A1E7-1393D915AAB9")]
        public int costcodeid;

        [DataMember(IsRequired = true)]
        [DataClass(FieldId = "359DFAC9-74E6-4BE5-949F-3FB224B1CBFC")]
        public string costcode;

        [DataMember(IsRequired = true)]
        [DataClass(FieldId = "AF80D035-6093-4721-8AFC-061424D2AB72")]
        public string description;

        [DataMember(IsRequired = true)]
        [DataClass(FieldId = "8178629C-5908-4458-89F6-D7EE7438314D")]
        public bool archived;

        [DataMember(IsRequired = true)]
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
