namespace EsrFramework.Models
{
    using System;

    public class EsrAssignment
    {
        #region Public Properties

        public string AFCFlag { get; set; }
        public string AccrualPlan { get; set; }
        public bool Active { get; set; }
        public decimal? AnnualSalaryValue { get; set; }
        public string AssignmentAddressCountry { get; set; }
        public string AssignmentAddressCounty { get; set; }
        public string AssignmentAddressLine1 { get; set; }
        public string AssignmentAddressLine2 { get; set; }
        public string AssignmentAddressPostcode { get; set; }
        public string AssignmentAddressTown { get; set; }
        public string AssignmentCategory { get; set; }
        public long? AssignmentId { get; set; }
        public string AssignmentLocation { get; set; }
        public string AssignmentNumber { get; set; }
        public string AssignmentStatus { get; set; }
        public string AssignmentType { get; set; }
        public string AvailabilitySchedule { get; set; }
        public DateTime? ContractEndDate { get; set; }
        public int? CreatedBy { get; set; }
        public long? DepartmentManagerPersonId { get; set; }
        public DateTime? EsrLastUpdate { get; set; }
        public long? EsrLocationId { get; set; }
        public long? EsrOrganisationId { get; set; }
        public long? EsrPersonId { get; set; }
        public long? EsrPositionId { get; set; }
        public DateTime? EarliestAssignmentStartDate { get; set; }
        public DateTime? EffectiveEndDate { get; set; }
        public DateTime? EffectiveStartDate { get; set; }
        public string EmployeeCategory { get; set; }
        public string EmployeeStatusFlag { get; set; }
        public decimal? Fte { get; set; }
        public DateTime? FinalAssignmentEndDate { get; set; }
        public string FlexibleWorkingPattern { get; set; }
        public string Grade { get; set; }
        public double? GradeContractHours { get; set; }
        public string GradeStep { get; set; }
        public string Group { get; set; }
        public DateTime? IncrementDate { get; set; }
        public string JobName { get; set; }
        public string JobRole { get; set; }
        public DateTime? LastWorkingDay { get; set; }
        public string LegalEntity { get; set; }
        public string ManagerFlag { get; set; }
        public string MaximumPartTimeFlag { get; set; }
        public int? ModifiedBy { get; set; }
        public string NightWorkerOptOut { get; set; }
        public double? NoOfSessions { get; set; }
        public double? NormalHours { get; set; }
        public string NormalHoursFrequency { get; set; }
        public string OccupationCode { get; set; }
        public string Organisation { get; set; }
        public string PayrollName { get; set; }
        public string PayrollPayType { get; set; }
        public string PayrollPeriodType { get; set; }
        public string PositionName { get; set; }
        public bool PrimaryAssignment { get; set; }
        public string PrimaryAssignmentString { get; set; }
        public DateTime? ProjectedHireDate { get; set; }
        public string SessionsFrequency { get; set; }
        public DateTime? StartDateInGrade { get; set; }
        public long? SupervisorAssignmentId { get; set; }
        public string SupervisorAssignmentNumber { get; set; }
        public string SupervisorEmployeeNumber { get; set; }
        public int? SupervisorEsrAssignId { get; set; }
        public string SupervisorFlag { get; set; }
        public string SupervisorFullName { get; set; }
        public long? SupervisorPersonId { get; set; }
        public string SystemAssignmentStatus { get; set; }
        public string TandAFlag { get; set; }
        public int? VacancyId { get; set; }
        public string WorkPatternDetails { get; set; }
        public string WorkPatternStartDay { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string EksfSpinalPoint { get; set; }
        public int EmployeeId { get; set; }
        public int EsrAssignId { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string OldProjectedHireDate { get; set; }

        #endregion
    }
}