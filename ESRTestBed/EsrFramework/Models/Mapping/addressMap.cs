using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace EsrFramework.Models.Mapping
{
    public class AddressMap : EntityTypeConfiguration<Address>
    {
        public AddressMap()
        {
            // Primary Key
            this.HasKey(t => new { AddressID = t.AddressId, t.Archived, t.AccountWideFavourite, t.Line1Lookup, t.Line2Lookup, t.CityLookup, t.PostcodeLookup, t.Udprn, t.Obsolete });

            // Properties
            this.Property(t => t.AddressId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.Postcode)
                .HasMaxLength(32);

            this.Property(t => t.Line1)
                .HasMaxLength(256);

            this.Property(t => t.Line2)
                .HasMaxLength(256);

            this.Property(t => t.Line3)
                .HasMaxLength(256);

            this.Property(t => t.City)
                .HasMaxLength(256);

            this.Property(t => t.County)
                .HasMaxLength(256);

            this.Property(t => t.GlobalIdentifier)
                .HasMaxLength(50);

            this.Property(t => t.Longitude)
                .HasMaxLength(20);

            this.Property(t => t.Latitude)
                .HasMaxLength(20);

            this.Property(t => t.Line1Lookup)
                .IsRequired()
                .HasMaxLength(256);

            this.Property(t => t.Line2Lookup)
                .IsRequired()
                .HasMaxLength(256);

            this.Property(t => t.CityLookup)
                .IsRequired()
                .HasMaxLength(256);

            this.Property(t => t.PostcodeLookup)
                .IsRequired()
                .HasMaxLength(256);

            this.Property(t => t.Udprn)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.AddressName)
                .HasMaxLength(250);

            this.Property(t => t.AddressNameLookup)
                .HasMaxLength(250);

            // Table & Column Mappings
            this.ToTable("addresses");
            this.Property(t => t.AddressId).HasColumnName("AddressID");
            this.Property(t => t.Postcode).HasColumnName("Postcode");
            this.Property(t => t.Line1).HasColumnName("Line1");
            this.Property(t => t.Line2).HasColumnName("Line2");
            this.Property(t => t.Line3).HasColumnName("Line3");
            this.Property(t => t.City).HasColumnName("City");
            this.Property(t => t.County).HasColumnName("County");
            this.Property(t => t.Country).HasColumnName("Country");
            this.Property(t => t.Archived).HasColumnName("Archived");
            this.Property(t => t.CreationMethod).HasColumnName("CreationMethod");
            this.Property(t => t.LookupDate).HasColumnName("LookupDate");
            this.Property(t => t.SubAccountId).HasColumnName("SubAccountID");
            this.Property(t => t.GlobalIdentifier).HasColumnName("GlobalIdentifier");
            this.Property(t => t.Longitude).HasColumnName("Longitude");
            this.Property(t => t.Latitude).HasColumnName("Latitude");
            this.Property(t => t.CreatedOn).HasColumnName("CreatedOn");
            this.Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            this.Property(t => t.ModifiedOn).HasColumnName("ModifiedOn");
            this.Property(t => t.ModifiedBy).HasColumnName("ModifiedBy");
            this.Property(t => t.AccountWideFavourite).HasColumnName("AccountWideFavourite");
            this.Property(t => t.Line1Lookup).HasColumnName("Line1Lookup");
            this.Property(t => t.Line2Lookup).HasColumnName("Line2Lookup");
            this.Property(t => t.CityLookup).HasColumnName("CityLookup");
            this.Property(t => t.PostcodeLookup).HasColumnName("PostcodeLookup");
            this.Property(t => t.Udprn).HasColumnName("Udprn");
            this.Property(t => t.Obsolete).HasColumnName("Obsolete");
            this.Property(t => t.AddressName).HasColumnName("AddressName");
            this.Property(t => t.AddressNameLookup).HasColumnName("AddressNameLookup");
        }
    }
}
