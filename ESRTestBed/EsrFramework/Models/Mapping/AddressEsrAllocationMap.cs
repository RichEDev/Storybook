using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace EsrFramework.Models.Mapping
{
    public class AddressEsrAllocationMap : EntityTypeConfiguration<AddressEsrAllocation>
    {
        public AddressEsrAllocationMap()
        {
            // Primary Key
            this.HasKey(t => new { t.AddressEsrAllocationId, t.AddressId });

            // Properties
            this.Property(t => t.AddressEsrAllocationId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.AddressId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            // Table & Column Mappings
            this.ToTable("AddressEsrAllocation");
            this.Property(t => t.AddressEsrAllocationId).HasColumnName("AddressEsrAllocationId");
            this.Property(t => t.Companyid).HasColumnName("companyid");
            this.Property(t => t.EsrLocationId).HasColumnName("ESRLocationID");
            this.Property(t => t.EsrAddressId).HasColumnName("ESRAddressID");
            this.Property(t => t.AddressId).HasColumnName("AddressId");
        }
    }
}
