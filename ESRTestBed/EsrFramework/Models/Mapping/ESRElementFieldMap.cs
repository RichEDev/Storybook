using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace EsrFramework.Models.Mapping
{
    public class EsrElementFieldMap : EntityTypeConfiguration<EsrElementField>
    {
        public EsrElementFieldMap()
        {
            // Primary Key
            this.HasKey(t => new { elementFieldID = t.ElementFieldId, elementID = t.ElementId, globalElementFieldID = t.GlobalElementFieldId, order = t.Order, reportColumnID = t.ReportColumnId });

            // Properties
            this.Property(t => t.ElementFieldId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.ElementId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.GlobalElementFieldId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            // Table & Column Mappings
            this.ToTable("ESRElementFields");
            this.Property(t => t.ElementFieldId).HasColumnName("elementFieldID");
            this.Property(t => t.ElementId).HasColumnName("elementID");
            this.Property(t => t.GlobalElementFieldId).HasColumnName("globalElementFieldID");
            this.Property(t => t.Aggregate).HasColumnName("aggregate");
            this.Property(t => t.Order).HasColumnName("order");
            this.Property(t => t.ReportColumnId).HasColumnName("reportColumnID");
        }
    }
}
