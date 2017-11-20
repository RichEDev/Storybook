using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace EsrFramework.Models.Mapping
{
    public class EsrAssignmentCostingMap : EntityTypeConfiguration<EsrAssignmentCosting>
    {
        public EsrAssignmentCostingMap()
        {
            // Primary Key
            this.HasKey(t => new { ESRCostingAllocationId = t.EsrCostingAllocationId, ESRPersonId = t.EsrPersonId, ESRAssignmentId = t.EsrAssignmentId, t.EffectiveStartDate });

            // Properties
            this.Property(t => t.EsrCostingAllocationId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.EsrPersonId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.EsrAssignmentId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.EntityCode)
                .HasMaxLength(3);

            this.Property(t => t.CharitableIndicator)
                .HasMaxLength(1);

            this.Property(t => t.CostCentre)
                .HasMaxLength(15);

            this.Property(t => t.Subjective)
                .HasMaxLength(15);

            this.Property(t => t.Analysis1)
                .HasMaxLength(15);

            this.Property(t => t.Analysis2)
                .HasMaxLength(15);

            this.Property(t => t.SpareSegment)
                .HasMaxLength(60);

            // Table & Column Mappings
            this.ToTable("ESRAssignmentCostings");
            this.Property(t => t.EsrCostingAllocationId).HasColumnName("ESRCostingAllocationId");
            this.Property(t => t.EsrPersonId).HasColumnName("ESRPersonId");
            this.Property(t => t.EsrAssignmentId).HasColumnName("ESRAssignmentId");
            this.Property(t => t.EffectiveStartDate).HasColumnName("EffectiveStartDate");
            this.Property(t => t.EffectiveEndDate).HasColumnName("EffectiveEndDate");
            this.Property(t => t.EntityCode).HasColumnName("EntityCode");
            this.Property(t => t.CharitableIndicator).HasColumnName("CharitableIndicator");
            this.Property(t => t.CostCentre).HasColumnName("CostCentre");
            this.Property(t => t.Subjective).HasColumnName("Subjective");
            this.Property(t => t.Analysis1).HasColumnName("Analysis1");
            this.Property(t => t.Analysis2).HasColumnName("Analysis2");
            this.Property(t => t.ElementNumber).HasColumnName("ElementNumber");
            this.Property(t => t.SpareSegment).HasColumnName("SpareSegment");
            this.Property(t => t.PercentageSplit).HasColumnName("PercentageSplit");
            this.Property(t => t.EsrLastUpdate).HasColumnName("ESRLastUpdate");
            this.Property(t => t.EsrAssignId).HasColumnName("ESRAssignId");
        }
    }
}
