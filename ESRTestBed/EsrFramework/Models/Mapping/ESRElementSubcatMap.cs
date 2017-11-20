using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace EsrFramework.Models.Mapping
{
    public class EsrElementSubcatMap : EntityTypeConfiguration<EsrElementSubcat>
    {
        public EsrElementSubcatMap()
        {
            // Primary Key
            this.HasKey(t => new { elementSubcatID = t.ElementSubcatId, elementID = t.ElementId, subcatID = t.SubcatId });

            // Properties
            this.Property(t => t.ElementSubcatId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.ElementId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.SubcatId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            // Table & Column Mappings
            this.ToTable("ESRElementSubcats");
            this.Property(t => t.ElementSubcatId).HasColumnName("elementSubcatID");
            this.Property(t => t.ElementId).HasColumnName("elementID");
            this.Property(t => t.SubcatId).HasColumnName("subcatID");
        }
    }
}
