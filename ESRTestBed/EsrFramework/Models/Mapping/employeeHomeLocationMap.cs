using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace EsrFramework.Models.Mapping
{
    public class EmployeeHomeLocationMap : EntityTypeConfiguration<EmployeeHomeLocation>
    {
        public EmployeeHomeLocationMap()
        {
            // Primary Key
            this.HasKey(t => new { employeeLocationID = t.EmployeeLocationId, employeeID = t.EmployeeId, startDate = t.StartDate, t.CreatedOn, t.CreatedBy });

            // Properties
            this.Property(t => t.EmployeeLocationId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.EmployeeId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.CreatedBy)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            // Table & Column Mappings
            this.ToTable("employeeHomeLocations");
            this.Property(t => t.EmployeeLocationId).HasColumnName("employeeLocationID");
            this.Property(t => t.EmployeeId).HasColumnName("employeeID");
            this.Property(t => t.LocationId).HasColumnName("locationID");
            this.Property(t => t.StartDate).HasColumnName("startDate");
            this.Property(t => t.EndDate).HasColumnName("endDate");
            this.Property(t => t.CreatedOn).HasColumnName("CreatedOn");
            this.Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            this.Property(t => t.ModifiedOn).HasColumnName("ModifiedOn");
            this.Property(t => t.ModifiedBy).HasColumnName("ModifiedBy");
        }
    }
}
