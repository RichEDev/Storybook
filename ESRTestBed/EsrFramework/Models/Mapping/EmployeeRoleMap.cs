using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace EsrFramework.Models.Mapping
{
    public class EmployeeRoleMap : EntityTypeConfiguration<EmployeeRole>
    {
        public EmployeeRoleMap()
        {
            // Primary Key
            this.HasKey(t => new { employeeid = t.EmployeeId, itemroleid = t.ItemRoleId, order = t.Order });

            // Properties
            this.Property(t => t.EmployeeId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.ItemRoleId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.Order)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            // Table & Column Mappings
            this.ToTable("employee_roles");
            this.Property(t => t.EmployeeId).HasColumnName("employeeid");
            this.Property(t => t.ItemRoleId).HasColumnName("itemroleid");
            this.Property(t => t.Order).HasColumnName("order");
        }
    }
}
