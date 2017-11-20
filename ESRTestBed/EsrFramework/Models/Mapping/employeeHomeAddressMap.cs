using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace EsrFramework.Models.Mapping
{
    public class EmployeeHomeAddressMap : EntityTypeConfiguration<EmployeeHomeAddress>
    {
        public EmployeeHomeAddressMap()
        {
            // Primary Key
            this.HasKey(t => new { t.EmployeeHomeAddressId, t.EmployeeId, t.AddressId });

            // Properties
            this.Property(t => t.EmployeeHomeAddressId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.EmployeeId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.AddressId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            // Table & Column Mappings
            this.ToTable("employeeHomeAddresses");
            this.Property(t => t.EmployeeHomeAddressId).HasColumnName("EmployeeHomeAddressId");
            this.Property(t => t.EmployeeId).HasColumnName("EmployeeId");
            this.Property(t => t.AddressId).HasColumnName("AddressId");
            this.Property(t => t.StartDate).HasColumnName("StartDate");
            this.Property(t => t.EndDate).HasColumnName("EndDate");
            this.Property(t => t.CreatedOn).HasColumnName("CreatedOn");
            this.Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            this.Property(t => t.ModifiedOn).HasColumnName("ModifiedOn");
            this.Property(t => t.ModifiedBy).HasColumnName("ModifiedBy");
        }
    }
}
