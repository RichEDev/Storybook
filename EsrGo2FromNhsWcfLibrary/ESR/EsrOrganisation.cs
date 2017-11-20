namespace EsrGo2FromNhsWcfLibrary.ESR
{
    using System;
    using System.Runtime.Serialization;

    using EsrGo2FromNhsWcfLibrary.Base;
    using EsrGo2FromNhsWcfLibrary.Spend_Management;

    /// <summary>
    /// The ESR organisation.
    /// </summary>
    [DataContract(Namespace = "http://software-europe.com/API/2013/02")]
    [DataClass(ElementType = TemplateMapping.ImportElementType.Organisation, TableId = "7EC6044C-2F3D-4F3B-A348-38426C460BBC")]
    public class EsrOrganisation : DataClassBase
    {
        /// <summary>
        /// The ESR organisation id.
        /// </summary>
        [DataClass(IsKeyField = true, ColumnRef = 1, FieldId = "97C23760-B082-4400-94DB-272180922EC8")]
        [DataMember(IsRequired = true)]
        public long ESROrganisationId = 0;

        /// <summary>
        /// The organisation name.
        /// </summary>
        [DataMember]
        [DataClass(ColumnRef = 2, FieldId = "813A9EB8-9526-4189-B9BF-BC46A7A769D2")]
        public string OrganisationName = string.Empty;

        /// <summary>
        /// The organisation type.
        /// </summary>
        [DataMember]
        [DataClass(ColumnRef = 3, FieldId = "98940532-A1B5-4AE4-A2CC-CAD5F474A1F1")]
        public string OrganisationType = string.Empty;

        /// <summary>
        /// The effective from.
        /// </summary>
        [DataMember(IsRequired = true)]
        [DataClass(ColumnRef = 4, FieldId = "D4199A54-28FF-49C1-A482-F0AF05750678")]
        public DateTime EffectiveFrom;

        /// <summary>
        /// The effective to.
        /// </summary>
        [DataMember]
        [DataClass(ColumnRef = 5, FieldId = "BF5FD119-D6DE-4C51-9FBB-1E31CEB4810D")]
        public DateTime? EffectiveTo;

        /// <summary>
        /// The hierarchy version id.
        /// </summary>
        [DataMember]
        [DataClass(ColumnRef = 6, FieldId = "4CACFE56-6CDD-4E17-8039-0996C24C349F")]
        public long HierarchyVersionId = 0;

        /// <summary>
        /// The hierarchy version from.
        /// </summary>
        [DataMember]
        [DataClass(ColumnRef = 7, FieldId = "D0B53949-6C97-4931-9764-CA51890FB97A")]
        public DateTime? HierarchyVersionFrom;

        /// <summary>
        /// The hierarchy version to.
        /// </summary>
        [DataMember]
        [DataClass(ColumnRef = 8, FieldId = "A6985961-260D-4424-849A-76CB1B7132DA")]
        public DateTime? HierarchyVersionTo;

        /// <summary>
        /// The default cost centre.
        /// </summary>
        [DataMember]
        [DataClass(ColumnRef = 9, FieldId = "E75F176C-53A3-41E8-8BA5-7A53AF99DC6A")]
        public string DefaultCostCentre = string.Empty;

        /// <summary>
        /// The parent organisation id.
        /// </summary>
        [DataMember]
        [DataClass(ColumnRef = 10, FieldId = "8156B391-85D7-49AA-AB79-6031F859E1C2")]
        public long? ParentOrganisationId;

        [DataMember]
        public long? SafeParentOrganisationId { get; set; }

        /// <summary>
        /// The NACS code.
        /// </summary>
        [DataMember]
        [DataClass(ColumnRef = 11, FieldId = "724A5693-C988-405E-8BCF-2DB548BCECA6")]
        public string NACSCode = string.Empty;

        /// <summary>
        /// The ESR location id.
        /// </summary>
        [DataMember]
        [DataClass(ColumnRef = 12, FieldId = "96F35F9A-B6DA-496C-8885-E45D64260D23")]
        public long? ESRLocationId;

        /// <summary>
        /// The ESR last update date.
        /// </summary>
        [DataMember]
        [DataClass(ColumnRef = 13, FieldId = "78411E5B-C069-4F92-AF45-736E1CAC9146")]
        public DateTime? ESRLastUpdateDate;

        /// <summary>
        /// The cost centre description.
        /// </summary>
        [DataMember]
        [DataClass(ColumnRef = 14, FieldId = "9E2F9196-493F-44E0-909B-B9EF34ACBBC7")]
        public string CostCentreDescription = string.Empty;

        /// <summary>
        /// Notes the position index within the source collection (used for post processing batches)
        /// </summary>
        [DataMember]
        public int RecordPositionIndex { get; set; }
    }
}
