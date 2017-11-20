namespace EsrGo2FromNhsWcfLibrary.ESR
{
    using System;
    using System.Runtime.Serialization;

    using EsrGo2FromNhsWcfLibrary.Base;
    using EsrGo2FromNhsWcfLibrary.Spend_Management;

    /// <summary>
    /// The ESR assignment COSTINGS.  
    /// </summary>
    [DataContract(Namespace = "http://software-europe.com/API/2013/02")]
    [DataClass(ElementType = TemplateMapping.ImportElementType.Costing, TableId = "3EFB3D39-0583-42F2-8557-580319F2082A")]
    public class EsrAssignmentCostings : DataClassBase
    {
        /// <summary>
        /// The esr COSTING allocation id.
        /// </summary>
        [DataClass(IsKeyField = true, ColumnRef = 3, FieldId = "2656EEC9-8D4E-49D8-AF4E-1C750B607EDE")]
        [DataMember(IsRequired = true)]
        public long ESRCostingAllocationId = 0;

        /// <summary>
        /// The ESR person id.
        /// </summary>
        [DataMember(IsRequired = true)]
        [DataClass(ColumnRef = 1, FieldId = "EE5BC925-544D-4896-B101-532009110AE7")]
        public long ESRPersonId = 0;

        /// <summary>
        /// The ESR assignment id.
        /// </summary>
        [DataMember(IsRequired = true)]
        [DataClass(ColumnRef = 2, FieldId = "ADB54ACB-1B85-4AC0-BC19-BF7B1A012A17")]
        public long ESRAssignmentId = 0;

        /// <summary>
        /// The effective start date.
        /// </summary>
        [DataMember(IsRequired = true)]
        [DataClass(ColumnRef = 4, FieldId = "F6FE871D-7D49-4EF8-8FF3-1E0A146BE8E3")]
        public DateTime EffectiveStartDate;

        /// <summary>
        /// The effective end date.
        /// </summary>
        [DataMember]
        [DataClass(ColumnRef = 5, FieldId = "12FD9127-507B-4B3C-8505-69250614651D")]
        public DateTime? EffectiveEndDate;

        /// <summary>
        /// The entity code.
        /// </summary>
        [DataMember]
        [DataClass(ColumnRef = 6, FieldId = "8DC6914E-1491-4544-B37A-F8A0BE5580AE")]
        public string EntityCode = string.Empty;

        /// <summary>
        /// The charitable indicator.
        /// </summary>
        [DataMember]
        [DataClass(ColumnRef = 7, FieldId = "0442C960-117B-4280-BD35-120CC7FB53A8")]
        public string CharitableIndicator = string.Empty;

        /// <summary>
        /// The cost centre.
        /// </summary>
        [DataMember]
        [DataClass(ColumnRef = 8, FieldId = "AA2FC0B7-9D8D-48C3-AA79-C4780CC19453")]
        public string CostCentre = string.Empty;

        /// <summary>
        /// The subjective.
        /// </summary>
        [DataMember]
        [DataClass(ColumnRef = 9, FieldId = "1BFB53B4-5510-43B5-A56F-0101C1F075D3")]
        public string Subjective = string.Empty;

        /// <summary>
        /// The analysis 1.
        /// </summary>
        [DataMember]
        [DataClass(ColumnRef = 10, FieldId = "8379781E-057C-403D-8138-813932AA8E68")]
        public string Analysis1 = string.Empty;

        /// <summary>
        /// The analysis 2.
        /// </summary>
        [DataMember]
        [DataClass(ColumnRef = 11, FieldId = "8F9E933B-E596-4CCE-B3E8-2689F97DA51E")]
        public string Analysis2 = string.Empty;

        /// <summary>
        /// The element number.
        /// </summary>
        [DataMember]
        [DataClass(ColumnRef = 12, FieldId = "03B5BD8A-583B-4B49-B537-7784474E92D9")]
        public int? ElementNumber = null;

        /// <summary>
        /// The spare segment.
        /// </summary>
        [DataMember]
        [DataClass(ColumnRef = 13, FieldId = "9E1A3E33-6D11-46BA-A696-24A4D91DB522")]
        public string SpareSegment = string.Empty;

        /// <summary>
        /// The percentage split.
        /// </summary>
        [DataMember]
        [DataClass(ColumnRef = 14, FieldId = "F7A90C80-3013-4FF5-8B61-4DA6CF2EB58C")]
        public decimal? PercentageSplit;

        /// <summary>
        /// The esr last update.
        /// </summary>
        [DataMember]
        [DataClass(ColumnRef = 15, FieldId = "86F8C14F-136D-48BC-917A-44609CDB2386")]
        public DateTime? ESRLastUpdate;

        /// <summary>
        /// The ESR assign id.
        /// </summary>
        [DataMember]
        [DataClass]
        public int? ESRAssignId;
    }
}