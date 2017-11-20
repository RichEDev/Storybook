using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace EsrFramework.Models.Mapping
{
    public class EsrAddressMap : EntityTypeConfiguration<EsrAddress>
    {
        public EsrAddressMap()
        {
            // Primary Key
            this.HasKey(t => new { ESRAddressId = t.EsrAddressId, t.AddressStyle, t.PrimaryFlag, t.EffectiveStartDate });

            // Properties
            this.Property(t => t.EsrAddressId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.AddressType)
                .HasMaxLength(30);

            this.Property(t => t.AddressStyle)
                .IsRequired()
                .HasMaxLength(30);

            this.Property(t => t.PrimaryFlag)
                .IsRequired()
                .HasMaxLength(30);

            this.Property(t => t.AddressLine1)
                .HasMaxLength(240);

            this.Property(t => t.AddressLine2)
                .HasMaxLength(240);

            this.Property(t => t.AddressLine3)
                .HasMaxLength(240);

            this.Property(t => t.AddressTown)
                .HasMaxLength(30);

            this.Property(t => t.AddressCounty)
                .HasMaxLength(70);

            this.Property(t => t.AddressPostcode)
                .HasMaxLength(30);

            this.Property(t => t.AddressCountry)
                .HasMaxLength(60);

            // Table & Column Mappings
            this.ToTable("ESRAddresses");
            this.Property(t => t.EsrAddressId).HasColumnName("ESRAddressId");
            this.Property(t => t.EsrPersonId).HasColumnName("ESRPersonId");
            this.Property(t => t.AddressType).HasColumnName("AddressType");
            this.Property(t => t.AddressStyle).HasColumnName("AddressStyle");
            this.Property(t => t.PrimaryFlag).HasColumnName("PrimaryFlag");
            this.Property(t => t.AddressLine1).HasColumnName("AddressLine1");
            this.Property(t => t.AddressLine2).HasColumnName("AddressLine2");
            this.Property(t => t.AddressLine3).HasColumnName("AddressLine3");
            this.Property(t => t.AddressTown).HasColumnName("AddressTown");
            this.Property(t => t.AddressCounty).HasColumnName("AddressCounty");
            this.Property(t => t.AddressPostcode).HasColumnName("AddressPostcode");
            this.Property(t => t.AddressCountry).HasColumnName("AddressCountry");
            this.Property(t => t.EffectiveStartDate).HasColumnName("EffectiveStartDate");
            this.Property(t => t.EffectiveEndDate).HasColumnName("EffectiveEndDate");
            this.Property(t => t.EsrLastUpdate).HasColumnName("ESRLastUpdate");
        }
    }
}
