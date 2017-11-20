using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace EsrFramework.Models.Mapping
{
    public class EsrVehicleMap : EntityTypeConfiguration<EsrVehicle>
    {
        public EsrVehicleMap()
        {
            // Primary Key
            this.HasKey(t => new { ESRVehicleAllocationId = t.EsrVehicleAllocationId, ESRPersonId = t.EsrPersonId, ESRAssignmentId = t.EsrAssignmentId });

            // Properties
            this.Property(t => t.EsrVehicleAllocationId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.EsrPersonId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.EsrAssignmentId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.RegistrationNumber)
                .HasMaxLength(30);

            this.Property(t => t.Make)
                .HasMaxLength(30);

            this.Property(t => t.Model)
                .HasMaxLength(30);

            this.Property(t => t.Ownership)
                .HasMaxLength(30);

            this.Property(t => t.UserRatesTable)
                .HasMaxLength(80);

            this.Property(t => t.FuelType)
                .HasMaxLength(30);

            // Table & Column Mappings
            this.ToTable("ESRVehicles");
            this.Property(t => t.EsrVehicleAllocationId).HasColumnName("ESRVehicleAllocationId");
            this.Property(t => t.EsrPersonId).HasColumnName("ESRPersonId");
            this.Property(t => t.EsrAssignmentId).HasColumnName("ESRAssignmentId");
            this.Property(t => t.EffectiveStartDate).HasColumnName("EffectiveStartDate");
            this.Property(t => t.EffectiveEndDate).HasColumnName("EffectiveEndDate");
            this.Property(t => t.RegistrationNumber).HasColumnName("RegistrationNumber");
            this.Property(t => t.Make).HasColumnName("Make");
            this.Property(t => t.Model).HasColumnName("Model");
            this.Property(t => t.Ownership).HasColumnName("Ownership");
            this.Property(t => t.InitialRegistrationDate).HasColumnName("InitialRegistrationDate");
            this.Property(t => t.EngineCc).HasColumnName("EngineCC");
            this.Property(t => t.EsrLastUpdate).HasColumnName("ESRLastUpdate");
            this.Property(t => t.UserRatesTable).HasColumnName("UserRatesTable");
            this.Property(t => t.FuelType).HasColumnName("FuelType");
            this.Property(t => t.EsrAssignId).HasColumnName("ESRAssignId");
        }
    }
}
