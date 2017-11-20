namespace EsrGo2FromNhsWcfLibrary.Spend_Management
{
    using System.Runtime.Serialization;

    using EsrGo2FromNhsWcfLibrary.Base;

    /// <summary>
    /// The employee cost codes.
    /// </summary>
    [DataContract(Namespace = "http://software-europe.com/API/2013/02")]
    [DataClass(ElementType = TemplateMapping.ImportElementType.Costing, TableId = "04ACC980-73A7-4866-8162-7B244679C676")]
    public class EmployeeCostCode : DataClassBase
    {
        [DataMember(IsRequired = true)]
        [DataClass(IsKeyField = true, FieldId = "D1DEA6AB-3BE5-409B-AB58-BD551E34DFB0")]
        public int employeecostcodeid;

        [DataMember(IsRequired = true)]
        [DataClass(FieldId = "BEC89567-9FF1-4922-A453-1AD5090E8CFB")]
        public int employeeid;

        [DataMember]
        [DataClass(FieldId = "CAC12A35-EFC7-47E0-B870-B58803307DA8")]
        public int? departmentid;

        [DataMember]
        [DataClass(FieldId = "2FA3AD65-B3F2-4658-94D7-08394B6EB43E")]
        public int? costcodeid;

        [DataMember]
        [DataClass(FieldId = "1AD6F5E8-B30C-4A61-84F1-ACB764A027ED")]
        public int? projectcodeid;

        [DataMember(IsRequired = true)]
        [DataClass(FieldId = "A52C41E1-B7ED-4B56-A7A2-2DB5BCE7467A")]
        public int percentused;
    }
}
