using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace EsrFramework.Models.Mapping
{
    public class EmployeeAccessRoleMap : EntityTypeConfiguration<EmployeeAccessRole>
    {
        public EmployeeAccessRoleMap()
        {
            // Primary Key
            this.HasKey(t => new { employeeID = t.EmployeeId, accessRoleID = t.AccessRoleId, subAccountID = t.SubAccountId });

            // Properties
            this.Property(t => t.EmployeeId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.AccessRoleId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.SubAccountId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            // Table & Column Mappings
            this.ToTable("employeeAccessRoles");
            this.Property(t => t.EmployeeId).HasColumnName("employeeID");
            this.Property(t => t.AccessRoleId).HasColumnName("accessRoleID");
            this.Property(t => t.SubAccountId).HasColumnName("subAccountID");
        }
    }
}
