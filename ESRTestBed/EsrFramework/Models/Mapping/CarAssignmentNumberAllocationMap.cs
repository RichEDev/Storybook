using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace EsrFramework.Models.Mapping
{
    public class CarAssignmentNumberAllocationMap : EntityTypeConfiguration<CarAssignmentNumberAllocation>
    {
        public CarAssignmentNumberAllocationMap()
        {
            // Primary Key
            this.HasKey(t => new { ESRVehicleAllocationId = t.EsrVehicleAllocationId, ESRAssignId = t.EsrAssignId, t.CarId, t.Archived });

            // Properties
            this.Property(t => t.EsrVehicleAllocationId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.EsrAssignId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.CarId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            // Table & Column Mappings
            this.ToTable("CarAssignmentNumberAllocations");
            this.Property(t => t.EsrVehicleAllocationId).HasColumnName("ESRVehicleAllocationId");
            this.Property(t => t.EsrAssignId).HasColumnName("ESRAssignId");
            this.Property(t => t.CarId).HasColumnName("CarId");
            this.Property(t => t.Archived).HasColumnName("Archived");
        }
    }
}
