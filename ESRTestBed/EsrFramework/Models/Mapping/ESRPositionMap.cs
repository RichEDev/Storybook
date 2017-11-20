using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace EsrFramework.Models.Mapping
{
    public class EsrPositionMap : EntityTypeConfiguration<EsrPosition>
    {
        public EsrPositionMap()
        {
            // Primary Key
            this.HasKey(t => new { ESRPositionId = t.EsrPositionId, t.PositionNumber, t.PositionName, ESRLastUpdateDate = t.EsrLastUpdateDate });

            // Properties
            this.Property(t => t.EsrPositionId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.PositionNumber)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.PositionName)
                .IsRequired()
                .HasMaxLength(240);

            this.Property(t => t.SubjectiveCode)
                .HasMaxLength(15);

            this.Property(t => t.JobStaffGroup)
                .HasMaxLength(40);

            this.Property(t => t.JobRole)
                .HasMaxLength(60);

            this.Property(t => t.OccupationCode)
                .HasMaxLength(5);

            this.Property(t => t.Payscale)
                .HasMaxLength(10);

            this.Property(t => t.GradeStep)
                .HasMaxLength(30);

            this.Property(t => t.IsaRegulatedPost)
                .HasMaxLength(15);

            this.Property(t => t.HiringStatus)
                .HasMaxLength(80);

            this.Property(t => t.PositionType)
                .HasMaxLength(80);

            this.Property(t => t.OhProcessingEligible)
                .HasMaxLength(30);

            this.Property(t => t.EppFlag)
                .HasMaxLength(30);

            this.Property(t => t.DeaneryPostNumber)
                .HasMaxLength(30);

            this.Property(t => t.ManagingDeaneryBody)
                .HasMaxLength(10);

            this.Property(t => t.WorkplaceOrgCode)
                .HasMaxLength(10);

            this.Property(t => t.SubjectiveCodeDescription)
                .HasMaxLength(240);

            // Table & Column Mappings
            this.ToTable("ESRPositions");
            this.Property(t => t.EsrPositionId).HasColumnName("ESRPositionId");
            this.Property(t => t.EffectiveFromDate).HasColumnName("EffectiveFromDate");
            this.Property(t => t.EffectiveToDate).HasColumnName("EffectiveToDate");
            this.Property(t => t.PositionNumber).HasColumnName("PositionNumber");
            this.Property(t => t.PositionName).HasColumnName("PositionName");
            this.Property(t => t.BudgetedFte).HasColumnName("BudgetedFTE");
            this.Property(t => t.SubjectiveCode).HasColumnName("SubjectiveCode");
            this.Property(t => t.JobStaffGroup).HasColumnName("JobStaffGroup");
            this.Property(t => t.JobRole).HasColumnName("JobRole");
            this.Property(t => t.OccupationCode).HasColumnName("OccupationCode");
            this.Property(t => t.Payscale).HasColumnName("Payscale");
            this.Property(t => t.GradeStep).HasColumnName("GradeStep");
            this.Property(t => t.IsaRegulatedPost).HasColumnName("ISARegulatedPost");
            this.Property(t => t.EsrOrganisationId).HasColumnName("ESROrganisationId");
            this.Property(t => t.HiringStatus).HasColumnName("HiringStatus");
            this.Property(t => t.PositionType).HasColumnName("PositionType");
            this.Property(t => t.OhProcessingEligible).HasColumnName("OHProcessingEligible");
            this.Property(t => t.EppFlag).HasColumnName("EPPFlag");
            this.Property(t => t.DeaneryPostNumber).HasColumnName("DeaneryPostNumber");
            this.Property(t => t.ManagingDeaneryBody).HasColumnName("ManagingDeaneryBody");
            this.Property(t => t.WorkplaceOrgCode).HasColumnName("WorkplaceOrgCode");
            this.Property(t => t.EsrLastUpdateDate).HasColumnName("ESRLastUpdateDate");
            this.Property(t => t.SubjectiveCodeDescription).HasColumnName("SubjectiveCodeDescription");
        }
    }
}
