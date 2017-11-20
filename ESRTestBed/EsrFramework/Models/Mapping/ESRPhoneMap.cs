using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace EsrFramework.Models.Mapping
{
    public class EsrPhoneMap : EntityTypeConfiguration<EsrPhone>
    {
        public EsrPhoneMap()
        {
            // Primary Key
            this.HasKey(t => new { ESRPhoneId = t.EsrPhoneId, ESRPersonId = t.EsrPersonId, t.PhoneType, t.PhoneNumber, t.EffectiveStartDate });

            // Properties
            this.Property(t => t.EsrPhoneId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.EsrPersonId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.PhoneType)
                .IsRequired()
                .HasMaxLength(30);

            this.Property(t => t.PhoneNumber)
                .IsRequired()
                .HasMaxLength(60);

            // Table & Column Mappings
            this.ToTable("ESRPhones");
            this.Property(t => t.EsrPhoneId).HasColumnName("ESRPhoneId");
            this.Property(t => t.EsrPersonId).HasColumnName("ESRPersonId");
            this.Property(t => t.PhoneType).HasColumnName("PhoneType");
            this.Property(t => t.PhoneNumber).HasColumnName("PhoneNumber");
            this.Property(t => t.EffectiveStartDate).HasColumnName("EffectiveStartDate");
            this.Property(t => t.EffectiveEndDate).HasColumnName("EffectiveEndDate");
            this.Property(t => t.EsrLastUpdate).HasColumnName("ESRLastUpdate");
        }
    }
}
