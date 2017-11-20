using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace EsrFramework.Models.Mapping
{
    public class EsrPersonMap : EntityTypeConfiguration<EsrPerson>
    {
        public EsrPersonMap()
        {
            // Primary Key
            this.HasKey(t => new { ESRPersonId = t.EsrPersonId, t.LastName });

            // Properties
            this.Property(t => t.EsrPersonId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.EmployeeNumber)
                .HasMaxLength(30);

            this.Property(t => t.Title)
                .HasMaxLength(30);

            this.Property(t => t.LastName)
                .IsRequired()
                .HasMaxLength(150);

            this.Property(t => t.FirstName)
                .HasMaxLength(150);

            this.Property(t => t.MiddleNames)
                .HasMaxLength(60);

            this.Property(t => t.MaidenName)
                .HasMaxLength(150);

            this.Property(t => t.PreferredName)
                .HasMaxLength(80);

            this.Property(t => t.PreviousLastName)
                .HasMaxLength(150);

            this.Property(t => t.Gender)
                .HasMaxLength(30);

            this.Property(t => t.NiNumber)
                .HasMaxLength(30);

            this.Property(t => t.NhsUniqueId)
                .HasMaxLength(15);

            this.Property(t => t.TerminationReason)
                .HasMaxLength(30);

            this.Property(t => t.EmployeeStatusFlag)
                .HasMaxLength(3);

            this.Property(t => t.WtrOptOut)
                .HasMaxLength(3);

            this.Property(t => t.EthnicOrigin)
                .HasMaxLength(30);

            this.Property(t => t.MaritalStatus)
                .HasMaxLength(30);

            this.Property(t => t.CountryOfBirth)
                .HasMaxLength(30);

            this.Property(t => t.PreviousEmployer)
                .HasMaxLength(240);

            this.Property(t => t.PreviousEmployerType)
                .HasMaxLength(30);

            this.Property(t => t.NhsCrsuuId)
                .HasMaxLength(12);

            this.Property(t => t.SystemPersonType)
                .HasMaxLength(30);

            this.Property(t => t.UserPersonType)
                .HasMaxLength(80);

            this.Property(t => t.OfficeEmailAddress)
                .HasMaxLength(240);

            this.Property(t => t.DisabilityFlag)
                .HasMaxLength(1);

            this.Property(t => t.LegacyPayrollNumber)
                .HasMaxLength(150);

            this.Property(t => t.Nationality)
                .HasMaxLength(30);

            // Table & Column Mappings
            this.ToTable("ESRPersons");
            this.Property(t => t.EsrPersonId).HasColumnName("ESRPersonId");
            this.Property(t => t.EffectiveStartDate).HasColumnName("EffectiveStartDate");
            this.Property(t => t.EffectiveEndDate).HasColumnName("EffectiveEndDate");
            this.Property(t => t.EmployeeNumber).HasColumnName("EmployeeNumber");
            this.Property(t => t.Title).HasColumnName("Title");
            this.Property(t => t.LastName).HasColumnName("LastName");
            this.Property(t => t.FirstName).HasColumnName("FirstName");
            this.Property(t => t.MiddleNames).HasColumnName("MiddleNames");
            this.Property(t => t.MaidenName).HasColumnName("MaidenName");
            this.Property(t => t.PreferredName).HasColumnName("PreferredName");
            this.Property(t => t.PreviousLastName).HasColumnName("PreviousLastName");
            this.Property(t => t.Gender).HasColumnName("Gender");
            this.Property(t => t.DateOfBirth).HasColumnName("DateOfBirth");
            this.Property(t => t.NiNumber).HasColumnName("NINumber");
            this.Property(t => t.NhsUniqueId).HasColumnName("NHSUniqueId");
            this.Property(t => t.HireDate).HasColumnName("HireDate");
            this.Property(t => t.ActualTerminationDate).HasColumnName("ActualTerminationDate");
            this.Property(t => t.TerminationReason).HasColumnName("TerminationReason");
            this.Property(t => t.EmployeeStatusFlag).HasColumnName("EmployeeStatusFlag");
            this.Property(t => t.WtrOptOut).HasColumnName("WTROptOut");
            this.Property(t => t.WtrOptOutDate).HasColumnName("WTROptOutDate");
            this.Property(t => t.EthnicOrigin).HasColumnName("EthnicOrigin");
            this.Property(t => t.MaritalStatus).HasColumnName("MaritalStatus");
            this.Property(t => t.CountryOfBirth).HasColumnName("CountryOfBirth");
            this.Property(t => t.PreviousEmployer).HasColumnName("PreviousEmployer");
            this.Property(t => t.PreviousEmployerType).HasColumnName("PreviousEmployerType");
            this.Property(t => t.Csd3Months).HasColumnName("CSD3Months");
            this.Property(t => t.Csd12Months).HasColumnName("CSD12Months");
            this.Property(t => t.NhsCrsuuId).HasColumnName("NHSCRSUUID");
            this.Property(t => t.SystemPersonType).HasColumnName("SystemPersonType");
            this.Property(t => t.UserPersonType).HasColumnName("UserPersonType");
            this.Property(t => t.OfficeEmailAddress).HasColumnName("OfficeEmailAddress");
            this.Property(t => t.NhsStartDate).HasColumnName("NHSStartDate");
            this.Property(t => t.EsrLastUpdateDate).HasColumnName("ESRLastUpdateDate");
            this.Property(t => t.DisabilityFlag).HasColumnName("DisabilityFlag");
            this.Property(t => t.LegacyPayrollNumber).HasColumnName("LegacyPayrollNumber");
            this.Property(t => t.Nationality).HasColumnName("Nationality");
        }
    }
}
