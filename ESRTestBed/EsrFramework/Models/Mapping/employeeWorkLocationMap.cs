using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace EsrFramework.Models.Mapping
{
    public class EmployeeWorkLocationMap : EntityTypeConfiguration<EmployeeWorkLocation>
    {
        public EmployeeWorkLocationMap()
        {
            // Primary Key
            this.HasKey(t => new { employeeLocationID = t.EmployeeLocationId, employeeID = t.EmployeeId, locationID = t.LocationId, active = t.Active, temporary = t.Temporary, createdOn = t.CreatedOn, createdBy = t.CreatedBy });

            // Properties
            this.Property(t => t.EmployeeLocationId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.EmployeeId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.LocationId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.CreatedBy)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            // Table & Column Mappings
            this.ToTable("employeeWorkLocations");
            this.Property(t => t.EmployeeLocationId).HasColumnName("employeeLocationID");
            this.Property(t => t.EmployeeId).HasColumnName("employeeID");
            this.Property(t => t.LocationId).HasColumnName("locationID");
            this.Property(t => t.StartDate).HasColumnName("startDate");
            this.Property(t => t.EndDate).HasColumnName("endDate");
            this.Property(t => t.Active).HasColumnName("active");
            this.Property(t => t.Temporary).HasColumnName("temporary");
            this.Property(t => t.CreatedOn).HasColumnName("createdOn");
            this.Property(t => t.CreatedBy).HasColumnName("createdBy");
            this.Property(t => t.ModifiedOn).HasColumnName("modifiedOn");
            this.Property(t => t.ModifiedBy).HasColumnName("modifiedBy");
        }
    }
}
