namespace EsrFramework.Models
{
    using System;

    public class EsrPosition
    {
        #region Public Properties

        public decimal? BudgetedFte { get; set; }
        public string DeaneryPostNumber { get; set; }
        public string EppFlag { get; set; }
        public DateTime EsrLastUpdateDate { get; set; }
        public long? EsrOrganisationId { get; set; }
        public long EsrPositionId { get; set; }
        public DateTime? EffectiveFromDate { get; set; }
        public DateTime? EffectiveToDate { get; set; }
        public string GradeStep { get; set; }
        public string HiringStatus { get; set; }
        public string IsaRegulatedPost { get; set; }
        public string JobRole { get; set; }
        public string JobStaffGroup { get; set; }
        public string ManagingDeaneryBody { get; set; }
        public string OhProcessingEligible { get; set; }
        public string OccupationCode { get; set; }
        public string Payscale { get; set; }
        public string PositionName { get; set; }
        public long PositionNumber { get; set; }
        public string PositionType { get; set; }
        public string SubjectiveCode { get; set; }
        public string SubjectiveCodeDescription { get; set; }
        public string WorkplaceOrgCode { get; set; }

        #endregion
    }
}