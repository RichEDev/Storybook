using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace EsrFramework.Models.Mapping
{
    public class EsrElementMap : EntityTypeConfiguration<EsrElement>
    {
        public EsrElementMap()
        {
            // Primary Key
            this.HasKey(t => new { elementID = t.ElementId, globalElementID = t.GlobalElementId, NHSTrustID = t.NhsTrustId });

            // Properties
            this.Property(t => t.ElementId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.GlobalElementId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.NhsTrustId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            // Table & Column Mappings
            this.ToTable("ESRElements");
            this.Property(t => t.ElementId).HasColumnName("elementID");
            this.Property(t => t.GlobalElementId).HasColumnName("globalElementID");
            this.Property(t => t.NhsTrustId).HasColumnName("NHSTrustID");
        }
    }
}
