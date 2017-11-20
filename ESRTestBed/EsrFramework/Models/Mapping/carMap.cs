using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace EsrFramework.Models.Mapping
{
    public class CarMap : EntityTypeConfiguration<Car>
    {
        public CarMap()
        {
            // Primary Key
            this.HasKey(t => new { carid = t.CarId, make = t.Make, model = t.Model, registration = t.Registration, active = t.Active, fuelcard = t.FuelCard, default_unit = t.DefaultUnit, approved = t.Approved, exemptFromHomeToOffice = t.ExemptFromHomeToOffice });

            // Properties
            this.Property(t => t.CarId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.Make)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.Model)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.Registration)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.MotTestNumber)
                .HasMaxLength(50);

            this.Property(t => t.InsuranceNumber)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("cars");
            this.Property(t => t.CarId).HasColumnName("carid");
            this.Property(t => t.EmployeeId).HasColumnName("employeeid");
            this.Property(t => t.StartDate).HasColumnName("startdate");
            this.Property(t => t.EndDate).HasColumnName("enddate");
            this.Property(t => t.Make).HasColumnName("make");
            this.Property(t => t.Model).HasColumnName("model");
            this.Property(t => t.Registration).HasColumnName("registration");
            this.Property(t => t.MileageId).HasColumnName("mileageid");
            this.Property(t => t.CarTypeId).HasColumnName("cartypeid");
            this.Property(t => t.Active).HasColumnName("active");
            this.Property(t => t.Odometer).HasColumnName("odometer");
            this.Property(t => t.FuelCard).HasColumnName("fuelcard");
            this.Property(t => t.EndOdometer).HasColumnName("endodometer");
            this.Property(t => t.TaxExpiry).HasColumnName("taxexpiry");
            this.Property(t => t.TaxLastChecked).HasColumnName("taxlastchecked");
            this.Property(t => t.TaxCheckedBy).HasColumnName("taxcheckedby");
            this.Property(t => t.MotTestNumber).HasColumnName("mottestnumber");
            this.Property(t => t.MotLastChecked).HasColumnName("motlastchecked");
            this.Property(t => t.MotCheckedBy).HasColumnName("motcheckedby");
            this.Property(t => t.MotExpiry).HasColumnName("motexpiry");
            this.Property(t => t.InsuranceNumber).HasColumnName("insurancenumber");
            this.Property(t => t.InsuranceExpiry).HasColumnName("insuranceexpiry");
            this.Property(t => t.InsuranceLastChecked).HasColumnName("insurancelastchecked");
            this.Property(t => t.InsuranceCheckedBy).HasColumnName("insurancecheckedby");
            this.Property(t => t.ServiceExpiry).HasColumnName("serviceexpiry");
            this.Property(t => t.ServiceLastChecked).HasColumnName("servicelastchecked");
            this.Property(t => t.ServiceCheckedBy).HasColumnName("servicecheckedby");
            this.Property(t => t.CreatedOn).HasColumnName("CreatedOn");
            this.Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            this.Property(t => t.ModifiedOn).HasColumnName("ModifiedOn");
            this.Property(t => t.ModifiedBy).HasColumnName("ModifiedBy");
            this.Property(t => t.DefaultUnit).HasColumnName("default_unit");
            this.Property(t => t.EngineSize).HasColumnName("enginesize");
            this.Property(t => t.Approved).HasColumnName("approved");
            this.Property(t => t.ExemptFromHomeToOffice).HasColumnName("exemptFromHomeToOffice");
            this.Property(t => t.TaxAttachId).HasColumnName("taxAttachID");
            this.Property(t => t.MotAttachId).HasColumnName("MOTAttachID");
            this.Property(t => t.InsuranceAttachId).HasColumnName("insuranceAttachID");
            this.Property(t => t.ServiceAttachId).HasColumnName("serviceAttachID");
        }
    }
}
