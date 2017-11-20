using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace EsrFramework.Models.Mapping
{
    public class EsrAssignmentMap : EntityTypeConfiguration<EsrAssignment>
    {
        public EsrAssignmentMap()
        {
            // Primary Key
            this.HasKey(t => new { employeeid = t.EmployeeId, t.AssignmentNumber, t.PrimaryAssignment, esrAssignID = t.EsrAssignId, t.Active });

            // Properties
            this.Property(t => t.EmployeeId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.AssignmentNumber)
                .IsRequired()
                .HasMaxLength(30);

            this.Property(t => t.AssignmentStatus)
                .HasMaxLength(80);

            this.Property(t => t.PayrollPayType)
                .HasMaxLength(10);

            this.Property(t => t.PayrollName)
                .HasMaxLength(80);

            this.Property(t => t.PayrollPeriodType)
                .HasMaxLength(30);

            this.Property(t => t.AssignmentAddressLine1)
                .HasMaxLength(60);

            this.Property(t => t.AssignmentAddressLine2)
                .HasMaxLength(60);

            this.Property(t => t.AssignmentAddressTown)
                .HasMaxLength(30);

            this.Property(t => t.AssignmentAddressCounty)
                .HasMaxLength(70);

            this.Property(t => t.AssignmentAddressPostcode)
                .HasMaxLength(30);

            this.Property(t => t.AssignmentAddressCountry)
                .HasMaxLength(60);

            this.Property(t => t.SupervisorFlag)
                .HasMaxLength(1);

            this.Property(t => t.SupervisorAssignmentNumber)
                .HasMaxLength(30);

            this.Property(t => t.SupervisorEmployeeNumber)
                .HasMaxLength(30);

            this.Property(t => t.SupervisorFullName)
                .HasMaxLength(240);

            this.Property(t => t.AccrualPlan)
                .HasMaxLength(80);

            this.Property(t => t.EmployeeCategory)
                .HasMaxLength(30);

            this.Property(t => t.AssignmentCategory)
                .HasMaxLength(30);

            this.Property(t => t.NormalHoursFrequency)
                .HasMaxLength(30);

            this.Property(t => t.SessionsFrequency)
                .HasMaxLength(4);

            this.Property(t => t.WorkPatternDetails)
                .HasMaxLength(80);

            this.Property(t => t.WorkPatternStartDay)
                .HasMaxLength(30);

            this.Property(t => t.FlexibleWorkingPattern)
                .HasMaxLength(30);

            this.Property(t => t.AvailabilitySchedule)
                .HasMaxLength(30);

            this.Property(t => t.Organisation)
                .HasMaxLength(240);

            this.Property(t => t.LegalEntity)
                .HasMaxLength(3);

            this.Property(t => t.PositionName)
                .HasMaxLength(240);

            this.Property(t => t.JobRole)
                .HasMaxLength(60);

            this.Property(t => t.OccupationCode)
                .HasMaxLength(5);

            this.Property(t => t.AssignmentLocation)
                .HasMaxLength(60);

            this.Property(t => t.Grade)
                .HasMaxLength(240);

            this.Property(t => t.JobName)
                .HasMaxLength(120);

            this.Property(t => t.Group)
                .HasMaxLength(240);

            this.Property(t => t.TandAFlag)
                .HasMaxLength(240);

            this.Property(t => t.NightWorkerOptOut)
                .HasMaxLength(3);

            this.Property(t => t.EsrAssignId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.OldProjectedHireDate)
                .HasMaxLength(8);

            this.Property(t => t.AssignmentType)
                .HasMaxLength(1);

            this.Property(t => t.SystemAssignmentStatus)
                .HasMaxLength(30);

            this.Property(t => t.EmployeeStatusFlag)
                .HasMaxLength(1);

            this.Property(t => t.PrimaryAssignmentString)
                .HasMaxLength(30);

            this.Property(t => t.GradeStep)
                .HasMaxLength(10);

            this.Property(t => t.MaximumPartTimeFlag)
                .HasMaxLength(30);

            this.Property(t => t.AFCFlag)
                .HasMaxLength(1);

            this.Property(t => t.EksfSpinalPoint)
                .HasMaxLength(30);

            this.Property(t => t.ManagerFlag)
                .HasMaxLength(30);

            // Table & Column Mappings
            this.ToTable("esr_assignments");
            this.Property(t => t.EmployeeId).HasColumnName("employeeid");
            this.Property(t => t.AssignmentId).HasColumnName("AssignmentID");
            this.Property(t => t.AssignmentNumber).HasColumnName("AssignmentNumber");
            this.Property(t => t.EarliestAssignmentStartDate).HasColumnName("EarliestAssignmentStartDate");
            this.Property(t => t.FinalAssignmentEndDate).HasColumnName("FinalAssignmentEndDate");
            this.Property(t => t.AssignmentStatus).HasColumnName("AssignmentStatus");
            this.Property(t => t.PayrollPayType).HasColumnName("PayrollPayType");
            this.Property(t => t.PayrollName).HasColumnName("PayrollName");
            this.Property(t => t.PayrollPeriodType).HasColumnName("PayrollPeriodType");
            this.Property(t => t.AssignmentAddressLine1).HasColumnName("AssignmentAddressLine1");
            this.Property(t => t.AssignmentAddressLine2).HasColumnName("AssignmentAddressLine2");
            this.Property(t => t.AssignmentAddressTown).HasColumnName("AssignmentAddressTown");
            this.Property(t => t.AssignmentAddressCounty).HasColumnName("AssignmentAddressCounty");
            this.Property(t => t.AssignmentAddressPostcode).HasColumnName("AssignmentAddressPostcode");
            this.Property(t => t.AssignmentAddressCountry).HasColumnName("AssignmentAddressCountry");
            this.Property(t => t.SupervisorFlag).HasColumnName("SupervisorFlag");
            this.Property(t => t.SupervisorAssignmentNumber).HasColumnName("SupervisorAssignmentNumber");
            this.Property(t => t.SupervisorEmployeeNumber).HasColumnName("SupervisorEmployeeNumber");
            this.Property(t => t.SupervisorFullName).HasColumnName("SupervisorFullName");
            this.Property(t => t.AccrualPlan).HasColumnName("AccrualPlan");
            this.Property(t => t.EmployeeCategory).HasColumnName("EmployeeCategory");
            this.Property(t => t.AssignmentCategory).HasColumnName("AssignmentCategory");
            this.Property(t => t.PrimaryAssignment).HasColumnName("PrimaryAssignment");
            this.Property(t => t.NormalHours).HasColumnName("NormalHours");
            this.Property(t => t.NormalHoursFrequency).HasColumnName("NormalHoursFrequency");
            this.Property(t => t.GradeContractHours).HasColumnName("GradeContractHours");
            this.Property(t => t.NoOfSessions).HasColumnName("NoOfSessions");
            this.Property(t => t.SessionsFrequency).HasColumnName("SessionsFrequency");
            this.Property(t => t.WorkPatternDetails).HasColumnName("WorkPatternDetails");
            this.Property(t => t.WorkPatternStartDay).HasColumnName("WorkPatternStartDay");
            this.Property(t => t.FlexibleWorkingPattern).HasColumnName("FlexibleWorkingPattern");
            this.Property(t => t.AvailabilitySchedule).HasColumnName("AvailabilitySchedule");
            this.Property(t => t.Organisation).HasColumnName("Organisation");
            this.Property(t => t.LegalEntity).HasColumnName("LegalEntity");
            this.Property(t => t.PositionName).HasColumnName("PositionName");
            this.Property(t => t.JobRole).HasColumnName("JobRole");
            this.Property(t => t.OccupationCode).HasColumnName("OccupationCode");
            this.Property(t => t.AssignmentLocation).HasColumnName("AssignmentLocation");
            this.Property(t => t.Grade).HasColumnName("Grade");
            this.Property(t => t.JobName).HasColumnName("JobName");
            this.Property(t => t.Group).HasColumnName("Group");
            this.Property(t => t.TandAFlag).HasColumnName("TAndAFlag");
            this.Property(t => t.NightWorkerOptOut).HasColumnName("NightWorkerOptOut");
            this.Property(t => t.ProjectedHireDate).HasColumnName("ProjectedHireDate");
            this.Property(t => t.VacancyId).HasColumnName("VacancyID");
            this.Property(t => t.CreatedOn).HasColumnName("createdon");
            this.Property(t => t.ModifiedOn).HasColumnName("modifiedon");
            this.Property(t => t.EsrAssignId).HasColumnName("esrAssignID");
            this.Property(t => t.Active).HasColumnName("Active");
            this.Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            this.Property(t => t.ModifiedBy).HasColumnName("ModifiedBy");
            this.Property(t => t.OldProjectedHireDate).HasColumnName("oldProjectedHireDate");
            this.Property(t => t.EsrPersonId).HasColumnName("ESRPersonId");
            this.Property(t => t.AssignmentType).HasColumnName("AssignmentType");
            this.Property(t => t.EffectiveStartDate).HasColumnName("EffectiveStartDate");
            this.Property(t => t.EffectiveEndDate).HasColumnName("EffectiveEndDate");
            this.Property(t => t.SystemAssignmentStatus).HasColumnName("SystemAssignmentStatus");
            this.Property(t => t.EmployeeStatusFlag).HasColumnName("EmployeeStatusFlag");
            this.Property(t => t.EsrLocationId).HasColumnName("ESRLocationId");
            this.Property(t => t.SupervisorPersonId).HasColumnName("SupervisorPersonId");
            this.Property(t => t.SupervisorAssignmentId).HasColumnName("SupervisorAssignmentId");
            this.Property(t => t.SupervisorEsrAssignId).HasColumnName("SupervisorEsrAssignId");
            this.Property(t => t.DepartmentManagerPersonId).HasColumnName("DepartmentManagerPersonId");
            this.Property(t => t.PrimaryAssignmentString).HasColumnName("PrimaryAssignmentString");
            this.Property(t => t.Fte).HasColumnName("FTE");
            this.Property(t => t.EsrOrganisationId).HasColumnName("ESROrganisationId");
            this.Property(t => t.EsrPositionId).HasColumnName("ESRPositionId");
            this.Property(t => t.GradeStep).HasColumnName("GradeStep");
            this.Property(t => t.StartDateInGrade).HasColumnName("StartDateInGrade");
            this.Property(t => t.AnnualSalaryValue).HasColumnName("AnnualSalaryValue");
            this.Property(t => t.ContractEndDate).HasColumnName("ContractEndDate");
            this.Property(t => t.IncrementDate).HasColumnName("IncrementDate");
            this.Property(t => t.MaximumPartTimeFlag).HasColumnName("MaximumPartTimeFlag");
            this.Property(t => t.AFCFlag).HasColumnName("AFCFlag");
            this.Property(t => t.EsrLastUpdate).HasColumnName("ESRLastUpdate");
            this.Property(t => t.LastWorkingDay).HasColumnName("LastWorkingDay");
            this.Property(t => t.EksfSpinalPoint).HasColumnName("eKSFSpinalPoint");
            this.Property(t => t.ManagerFlag).HasColumnName("ManagerFlag");
        }
    }
}
