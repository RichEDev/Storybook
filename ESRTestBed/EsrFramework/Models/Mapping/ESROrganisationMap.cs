using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace EsrFramework.Models.Mapping
{
    public class EsrOrganisationMap : EntityTypeConfiguration<EsrOrganisation>
    {
        public EsrOrganisationMap()
        {
            // Primary Key
            this.HasKey(t => new { ESROrganisationId = t.EsrOrganisationId, t.EffectiveFrom, t.HierarchyVersionId });

            // Properties
            this.Property(t => t.EsrOrganisationId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.OrganisationName)
                .HasMaxLength(240);

            this.Property(t => t.OrganisationType)
                .HasMaxLength(80);

            this.Property(t => t.HierarchyVersionId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.DefaultCostCentre)
                .HasMaxLength(15);

            this.Property(t => t.NacsCode)
                .HasMaxLength(30);

            this.Property(t => t.CostCentreDescription)
                .HasMaxLength(240);

            // Table & Column Mappings
            this.ToTable("ESROrganisations");
            this.Property(t => t.EsrOrganisationId).HasColumnName("ESROrganisationId");
            this.Property(t => t.OrganisationName).HasColumnName("OrganisationName");
            this.Property(t => t.OrganisationType).HasColumnName("OrganisationType");
            this.Property(t => t.EffectiveFrom).HasColumnName("EffectiveFrom");
            this.Property(t => t.EffectiveTo).HasColumnName("EffectiveTo");
            this.Property(t => t.HierarchyVersionId).HasColumnName("HierarchyVersionId");
            this.Property(t => t.HierarchyVersionFrom).HasColumnName("HierarchyVersionFrom");
            this.Property(t => t.HierarchyVersionTo).HasColumnName("HierarchyVersionTo");
            this.Property(t => t.DefaultCostCentre).HasColumnName("DefaultCostCentre");
            this.Property(t => t.ParentOrganisationId).HasColumnName("ParentOrganisationId");
            this.Property(t => t.NacsCode).HasColumnName("NACSCode");
            this.Property(t => t.EsrLocationId).HasColumnName("ESRLocationId");
            this.Property(t => t.EsrLastUpdateDate).HasColumnName("ESRLastUpdateDate");
            this.Property(t => t.CostCentreDescription).HasColumnName("CostCentreDescription");
        }
    }
}
