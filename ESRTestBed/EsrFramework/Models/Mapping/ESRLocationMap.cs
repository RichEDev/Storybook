using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace EsrFramework.Models.Mapping
{
    public class EsrLocationMap : EntityTypeConfiguration<EsrLocation>
    {
        public EsrLocationMap()
        {
            // Primary Key
            this.HasKey(t => new { ESRLocationId = t.EsrLocationId, ESRLastUpdate = t.EsrLastUpdate });

            // Properties
            this.Property(t => t.EsrLocationId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.LocationCode)
                .HasMaxLength(60);

            this.Property(t => t.Description)
                .HasMaxLength(240);

            this.Property(t => t.AddressLine1)
                .HasMaxLength(240);

            this.Property(t => t.AddressLine2)
                .HasMaxLength(240);

            this.Property(t => t.AddressLine3)
                .HasMaxLength(240);

            this.Property(t => t.Town)
                .HasMaxLength(30);

            this.Property(t => t.County)
                .HasMaxLength(70);

            this.Property(t => t.Postcode)
                .HasMaxLength(30);

            this.Property(t => t.Country)
                .HasMaxLength(60);

            this.Property(t => t.Telephone)
                .HasMaxLength(60);

            this.Property(t => t.Fax)
                .HasMaxLength(60);

            this.Property(t => t.PayslipDeliveryPoint)
                .HasMaxLength(1);

            this.Property(t => t.SiteCode)
                .HasMaxLength(2);

            this.Property(t => t.WelshLocationTranslation)
                .HasMaxLength(60);

            this.Property(t => t.WelshAddress1)
                .HasMaxLength(60);

            this.Property(t => t.WelshAddress2)
                .HasMaxLength(60);

            this.Property(t => t.WelshAddress3)
                .HasMaxLength(60);

            this.Property(t => t.WelshTownTranslation)
                .HasMaxLength(60);

            // Table & Column Mappings
            this.ToTable("ESRLocations");
            this.Property(t => t.EsrLocationId).HasColumnName("ESRLocationId");
            this.Property(t => t.LocationCode).HasColumnName("LocationCode");
            this.Property(t => t.Description).HasColumnName("Description");
            this.Property(t => t.InactiveDate).HasColumnName("InactiveDate");
            this.Property(t => t.AddressLine1).HasColumnName("AddressLine1");
            this.Property(t => t.AddressLine2).HasColumnName("AddressLine2");
            this.Property(t => t.AddressLine3).HasColumnName("AddressLine3");
            this.Property(t => t.Town).HasColumnName("Town");
            this.Property(t => t.County).HasColumnName("County");
            this.Property(t => t.Postcode).HasColumnName("Postcode");
            this.Property(t => t.Country).HasColumnName("Country");
            this.Property(t => t.Telephone).HasColumnName("Telephone");
            this.Property(t => t.Fax).HasColumnName("Fax");
            this.Property(t => t.PayslipDeliveryPoint).HasColumnName("PayslipDeliveryPoint");
            this.Property(t => t.SiteCode).HasColumnName("SiteCode");
            this.Property(t => t.WelshLocationTranslation).HasColumnName("WelshLocationTranslation");
            this.Property(t => t.WelshAddress1).HasColumnName("WelshAddress1");
            this.Property(t => t.WelshAddress2).HasColumnName("WelshAddress2");
            this.Property(t => t.WelshAddress3).HasColumnName("WelshAddress3");
            this.Property(t => t.WelshTownTranslation).HasColumnName("WelshTownTranslation");
            this.Property(t => t.EsrLastUpdate).HasColumnName("ESRLastUpdate");
        }
    }
}
