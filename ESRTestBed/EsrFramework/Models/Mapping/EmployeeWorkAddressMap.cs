using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace EsrFramework.Models.Mapping
{
    public class EmployeeWorkAddressMap : EntityTypeConfiguration<EmployeeWorkAddress>
    {
        public EmployeeWorkAddressMap()
        {
            // Primary Key
            this.HasKey(t => new { t.EmployeeWorkAddressId, t.EmployeeId, t.AddressId, t.Active, t.Temporary });

            // Properties
            this.Property(t => t.EmployeeWorkAddressId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.EmployeeId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.AddressId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            // Table & Column Mappings
            this.ToTable("EmployeeWorkAddresses");
            this.Property(t => t.EmployeeWorkAddressId).HasColumnName("EmployeeWorkAddressId");
            this.Property(t => t.EmployeeId).HasColumnName("EmployeeId");
            this.Property(t => t.AddressId).HasColumnName("AddressId");
            this.Property(t => t.StartDate).HasColumnName("StartDate");
            this.Property(t => t.EndDate).HasColumnName("EndDate");
            this.Property(t => t.Active).HasColumnName("Active");
            this.Property(t => t.Temporary).HasColumnName("Temporary");
            this.Property(t => t.CreatedOn).HasColumnName("CreatedOn");
            this.Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            this.Property(t => t.ModifiedOn).HasColumnName("ModifiedOn");
            this.Property(t => t.ModifiedBy).HasColumnName("ModifiedBy");
        }
    }
}
