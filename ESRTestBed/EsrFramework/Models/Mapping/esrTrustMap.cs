using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace EsrFramework.Models.Mapping
{
    public class EsrTrustMap : EntityTypeConfiguration<EsrTrust>
    {
        public EsrTrustMap()
        {
            // Primary Key
            this.HasKey(t => new { trustID = t.TrustId, trustVPD = t.TrustVpd, periodType = t.PeriodType, periodRun = t.PeriodRun, runSequenceNumber = t.RunSequenceNumber, archived = t.Archived, trustName = t.TrustName, delimiterCharacter = t.DelimiterCharacter, t.EsrVersionNumber });

            // Properties
            this.Property(t => t.TrustId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.TrustVpd)
                .IsRequired()
                .HasMaxLength(3);

            this.Property(t => t.PeriodType)
                .IsRequired()
                .HasMaxLength(1);

            this.Property(t => t.PeriodRun)
                .IsRequired()
                .HasMaxLength(1);

            this.Property(t => t.RunSequenceNumber)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.FtpAddress)
                .HasMaxLength(100);

            this.Property(t => t.FtpUsername)
                .HasMaxLength(100);

            this.Property(t => t.FtpPassword)
                .HasMaxLength(100);

            this.Property(t => t.TrustName)
                .IsRequired()
                .HasMaxLength(150);

            this.Property(t => t.DelimiterCharacter)
                .IsRequired()
                .HasMaxLength(5);

            // Table & Column Mappings
            this.ToTable("esrTrusts");
            this.Property(t => t.TrustId).HasColumnName("trustID");
            this.Property(t => t.TrustVpd).HasColumnName("trustVPD");
            this.Property(t => t.PeriodType).HasColumnName("periodType");
            this.Property(t => t.PeriodRun).HasColumnName("periodRun");
            this.Property(t => t.RunSequenceNumber).HasColumnName("runSequenceNumber");
            this.Property(t => t.FtpAddress).HasColumnName("ftpAddress");
            this.Property(t => t.FtpUsername).HasColumnName("ftpUsername");
            this.Property(t => t.FtpPassword).HasColumnName("ftpPassword");
            this.Property(t => t.Archived).HasColumnName("archived");
            this.Property(t => t.CreatedOn).HasColumnName("createdOn");
            this.Property(t => t.ModifiedOn).HasColumnName("modifiedOn");
            this.Property(t => t.TrustName).HasColumnName("trustName");
            this.Property(t => t.DelimiterCharacter).HasColumnName("delimiterCharacter");
            this.Property(t => t.EsrVersionNumber).HasColumnName("ESRVersionNumber");
            this.Property(t => t.CurrentOutboundSequence).HasColumnName("currentOutboundSequence");
        }
    }
}
