namespace ApiLibrary.DataObjects.ESR
{
    using System;
    using System.Runtime.Serialization;
    using ApiLibrary.DataObjects.Base;
    using ApiLibrary.DataObjects.Spend_Management;

    /// <summary>
    /// The ESR position.
    /// </summary>
    [DataContract(Namespace = "http://software-europe.com/API/2013/02")]
    [DataClass(ElementType = TemplateMapping.ImportElementType.Position, TableId = "26777F45-1D24-4623-9F18-8639E7D6227F")]
    public class EsrPosition : DataClassBase
    {
        /// <summary>
        /// The ESR position id.
        /// </summary>
        [DataClass(IsKeyField = true, ColumnRef = 1, FieldId = "4D98ADEA-68E6-42A4-8962-FFF40C5A79A8")]
        [DataMember(IsRequired = true)]
        public long ESRPositionId = 0;

        /// <summary>
        /// The effective from date.
        /// </summary>
        [DataMember]
        [DataClass(ColumnRef = 2, FieldId = "4C9F18D4-8643-4FE1-94E1-477FC19CF54E")]
        public DateTime? EffectiveFromDate;

        /// <summary>
        /// The effective to date.
        /// </summary>
        [DataMember]
        [DataClass(ColumnRef = 3, FieldId = "F177E812-DA3B-47C7-9D90-93F28BCBD185")]
        public DateTime? EffectiveToDate;

        /// <summary>
        /// The position number.
        /// </summary>
        [DataMember]
        [DataClass(ColumnRef = 4, FieldId = "357CE8FC-6D97-44D7-AA4B-ECA4628A0664")]
        public long PositionNumber = 0;

        /// <summary>
        /// The position name.
        /// </summary>
        [DataMember(IsRequired = true)]
        [DataClass(ColumnRef = 5, FieldId = "ECCA2EF5-1973-4FC7-BFA4-38D2F7BDF302")]
        public string PositionName = string.Empty;

        /// <summary>
        /// The budgeted FTE.
        /// </summary>
        [DataMember]
        [DataClass(ColumnRef = 6, FieldId = "B1F1371D-755F-46E2-A57D-65279DE48436")]
        public decimal? BudgetedFTE;

        /// <summary>
        /// The subjective code.
        /// </summary>
        [DataMember]
        [DataClass(ColumnRef = 7, FieldId = "BF41CB01-9755-4315-83E4-667D732E6B32")]
        public string SubjectiveCode = string.Empty;

        /// <summary>
        /// The job staff group.
        /// </summary>
        [DataMember]
        [DataClass(ColumnRef = 8, FieldId = "F622BAD9-E202-4BFB-874B-679BF145A40F")]
        public string JobStaffGroup = string.Empty;

        /// <summary>
        /// The job role.
        /// </summary>
        [DataMember]
        [DataClass(ColumnRef = 9, FieldId = "61CE419A-E146-49A1-B534-7166D5B792EC")]
        public string JobRole = string.Empty;

        /// <summary>
        /// The occupation code.
        /// </summary>
        [DataMember]
        [DataClass(ColumnRef = 10, FieldId = "C238949A-E9E4-4E80-AAF2-4686A744A1BA")]
        public string OccupationCode = string.Empty;

        /// <summary>
        /// The pay scale.
        /// </summary>
        [DataMember]
        [DataClass(ColumnRef = 11, FieldId = "49E9E0E2-2879-4229-BD34-468E3B5A91F4")]
        public string Payscale = string.Empty;

        /// <summary>
        /// The grade step.
        /// </summary>
        [DataMember]
        [DataClass(ColumnRef = 12, FieldId = "931AC944-7F12-474F-807D-68C88BDCCD1D")]
        public string GradeStep = string.Empty;

        /// <summary>
        /// The is a regulated post.
        /// </summary>
        [DataMember]
        [DataClass(ColumnRef = 13, FieldId = "AC0B259D-7E14-4946-8D5E-8E342947C427")]
        public string ISARegulatedPost = string.Empty;

        /// <summary>
        /// The ESR organisation id.
        /// </summary>
        [DataMember]
        [DataClass(ColumnRef = 14, FieldId = "E8A7EFDE-5AF9-4853-A925-5902547DD07C")]
        public long? ESROrganisationId;

        /// <summary>
        /// The hiring status.
        /// </summary>
        [DataMember]
        [DataClass(ColumnRef = 15, FieldId = "510A84B9-F07D-4177-B635-E2A9920BE042")]
        public string HiringStatus = string.Empty;

        /// <summary>
        /// The position type.
        /// </summary>
        [DataMember]
        [DataClass(ColumnRef = 16, FieldId = "CBA0156D-55A0-4789-924F-22E45B22C8C4")]
        public string PositionType = string.Empty;

        /// <summary>
        /// The OH processing eligible.
        /// </summary>
        [DataMember]
        [DataClass(ColumnRef = 17, FieldId = "9F797795-BC62-4D94-95BA-C92C67D7E0EC")]
        public string OHProcessingEligible = string.Empty;

        /// <summary>
        /// The EPP flag.
        /// </summary>
        [DataMember]
        [DataClass(ColumnRef = 18, FieldId = "001FCCCC-FF2E-44FA-BCF4-430FDC964F20")]
        public string EPPFlag = string.Empty;

        /// <summary>
        /// The deanery post number.
        /// </summary>
        [DataMember]
        [DataClass(ColumnRef = 19, FieldId = "E7AB72C3-7B62-44AF-BCCB-38CACD836BEB")]
        public string DeaneryPostNumber = string.Empty;

        /// <summary>
        /// The managing deanery body.
        /// </summary>
        [DataMember]
        [DataClass(ColumnRef = 20, FieldId = "98E7365B-27BA-4AEC-A538-787710EB5126")]
        public string ManagingDeaneryBody = string.Empty;

        /// <summary>
        /// The workplace ORG code.
        /// </summary>
        [DataMember]
        [DataClass(ColumnRef = 21, FieldId = "BFEA0BD3-3934-483B-BD68-9EBA7FA3C913")]
        public string WorkplaceOrgCode = string.Empty;

        /// <summary>
        /// The ESR last update date.
        /// </summary>
        [DataMember(IsRequired = true)]
        [DataClass(ColumnRef = 22, FieldId = "90AE30A1-7788-42F7-A6F8-49CCF2788A10")]
        public DateTime ESRLastUpdateDate;

        /// <summary>
        /// The subjective code description.
        /// </summary>
        [DataMember]
        [DataClass(ColumnRef = 23, FieldId = "55DE1CD1-37A8-4815-BD01-499C83964C7D")]
        public string SubjectiveCodeDescription = string.Empty;
    }
}
