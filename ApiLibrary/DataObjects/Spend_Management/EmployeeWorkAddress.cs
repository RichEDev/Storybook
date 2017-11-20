namespace ApiLibrary.DataObjects.Spend_Management
{
    using System;
    using System.Runtime.Serialization;
    using ApiLibrary.DataObjects.Base;

    /// <summary>
    /// The employee work address.
    /// </summary>
    [DataContract(Namespace = "http://software-europe.com/API/2013/02")]
    [DataClass(ElementType = TemplateMapping.ImportElementType.None, TableId = "BF306B4F-9D76-4DD2-A1C9-0F73BB31CCD7")]
    public class EmployeeWorkAddress : DataClassBase
    {
        [DataMember]
        [DataClass(IsKeyField = true)]
        public int EmployeeWorkAddressId;

        [DataMember]
        public int EmployeeId;

        [DataMember]
        public int AddressId;

        [DataMember]
        public DateTime? StartDate;

        [DataMember]
        public DateTime? EndDate;

        [DataMember]
        public bool Active;

        [DataMember]
        public bool Temporary;

        [DataMember]
        public DateTime? CreatedOn;

        [DataMember]
        public int? CreatedBy;

        [DataMember]
        public DateTime? ModifiedOn;

        [DataMember]
        public int? ModifiedBy;
    }
}
