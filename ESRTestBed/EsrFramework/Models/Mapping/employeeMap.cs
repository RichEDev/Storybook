using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace EsrFramework.Models.Mapping
{
    public class EmployeeMap : EntityTypeConfiguration<Employee>
    {
        public EmployeeMap()
        {
            // Primary Key
            this.HasKey(t => new { employeeid = t.EmployeeId, username = t.Username, title = t.Title, firstname = t.FirstName, surname = t.Surname, currefnum = t.CurRefNum, curclaimno = t.CurClaimNo, archived = t.Archived, additems = t.AddItems, userole = t.UseRole, mileage = t.Mileage, mileageprev = t.MileagePrev, customiseditems = t.CustomisedItems, active = t.Active, verified = t.Verified, applicantactivestatusflag = t.ApplicantActiveStatusFlag, logonCount = t.LogonCount, retryCount = t.RetryCount, firstLogon = t.FirstLogon, t.CreationMethod, adminonly = t.AdminOnly, locked = t.Locked, t.ContactHelpDeskAllowed });

            // Properties
            this.Property(t => t.EmployeeId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.Username)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.Password)
                .HasMaxLength(250);

            this.Property(t => t.Title)
                .IsRequired()
                .HasMaxLength(30);

            this.Property(t => t.FirstName)
                .IsRequired()
                .HasMaxLength(150);

            this.Property(t => t.Surname)
                .IsRequired()
                .HasMaxLength(150);

            this.Property(t => t.Email)
                .HasMaxLength(250);

            this.Property(t => t.CurRefNum)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.CurClaimNo)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.Payroll)
                .HasMaxLength(50);

            this.Property(t => t.Position)
                .HasMaxLength(250);

            this.Property(t => t.TelNo)
                .HasMaxLength(50);

            this.Property(t => t.Creditor)
                .HasMaxLength(50);

            this.Property(t => t.Hint)
                .HasMaxLength(1000);

            this.Property(t => t.AddItems)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.CardNum)
                .HasMaxLength(50);

            this.Property(t => t.Extension)
                .HasMaxLength(50);

            this.Property(t => t.PagerNo)
                .HasMaxLength(50);

            this.Property(t => t.MobileNo)
                .HasMaxLength(50);

            this.Property(t => t.FaxNo)
                .HasMaxLength(50);

            this.Property(t => t.HomeEmail)
                .HasMaxLength(250);

            this.Property(t => t.Mileage)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.MileagePrev)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.LicenceNumber)
                .HasMaxLength(50);

            this.Property(t => t.Country)
                .HasMaxLength(100);

            this.Property(t => t.NiNumber)
                .HasMaxLength(50);

            this.Property(t => t.MaidenName)
                .HasMaxLength(150);

            this.Property(t => t.MiddleNames)
                .HasMaxLength(150);

            this.Property(t => t.Gender)
                .HasMaxLength(6);

            this.Property(t => t.ApplicantNumber)
                .HasMaxLength(50);

            this.Property(t => t.Name)
                .HasMaxLength(50);

            this.Property(t => t.AccountNumber)
                .HasMaxLength(50);

            this.Property(t => t.AccountType)
                .HasMaxLength(50);

            this.Property(t => t.SortCode)
                .HasMaxLength(50);

            this.Property(t => t.Reference)
                .HasMaxLength(50);

            this.Property(t => t.LogonCount)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.RetryCount)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.SupportPortalPassword)
                .HasMaxLength(250);

            this.Property(t => t.PreferredName)
                .HasMaxLength(150);

            this.Property(t => t.EmployeeNumber)
                .HasMaxLength(30);

            this.Property(t => t.NhsUniqueId)
                .HasMaxLength(15);

            this.Property(t => t.EsrPersonType)
                .HasMaxLength(80);

            // Table & Column Mappings
            this.ToTable("employees");
            this.Property(t => t.EmployeeId).HasColumnName("employeeid");
            this.Property(t => t.Username).HasColumnName("username");
            this.Property(t => t.Password).HasColumnName("password");
            this.Property(t => t.Title).HasColumnName("title");
            this.Property(t => t.FirstName).HasColumnName("firstname");
            this.Property(t => t.Surname).HasColumnName("surname");
            this.Property(t => t.MileageTotal).HasColumnName("mileagetotal");
            this.Property(t => t.Email).HasColumnName("email");
            this.Property(t => t.CurRefNum).HasColumnName("currefnum");
            this.Property(t => t.CurClaimNo).HasColumnName("curclaimno");
            this.Property(t => t.Speedo).HasColumnName("speedo");
            this.Property(t => t.Payroll).HasColumnName("payroll");
            this.Property(t => t.Position).HasColumnName("position");
            this.Property(t => t.TelNo).HasColumnName("telno");
            this.Property(t => t.Creditor).HasColumnName("creditor");
            this.Property(t => t.Archived).HasColumnName("archived");
            this.Property(t => t.GroupId).HasColumnName("groupid");
            this.Property(t => t.RoleId).HasColumnName("roleid");
            this.Property(t => t.Hint).HasColumnName("hint");
            this.Property(t => t.LastChange).HasColumnName("lastchange");
            this.Property(t => t.AddItems).HasColumnName("additems");
            this.Property(t => t.CardNum).HasColumnName("cardnum");
            this.Property(t => t.UseRole).HasColumnName("userole");
            this.Property(t => t.CostCodeId).HasColumnName("costcodeid");
            this.Property(t => t.DepartmentId).HasColumnName("departmentid");
            this.Property(t => t.Extension).HasColumnName("extension");
            this.Property(t => t.PagerNo).HasColumnName("pagerno");
            this.Property(t => t.MobileNo).HasColumnName("mobileno");
            this.Property(t => t.FaxNo).HasColumnName("faxno");
            this.Property(t => t.HomeEmail).HasColumnName("homeemail");
            this.Property(t => t.LineManager).HasColumnName("linemanager");
            this.Property(t => t.AdvanceGroupId).HasColumnName("advancegroupid");
            this.Property(t => t.Mileage).HasColumnName("mileage");
            this.Property(t => t.MileagePrev).HasColumnName("mileageprev");
            this.Property(t => t.CustomisedItems).HasColumnName("customiseditems");
            this.Property(t => t.Active).HasColumnName("active");
            this.Property(t => t.PrimaryCountry).HasColumnName("primarycountry");
            this.Property(t => t.PrimaryCurrency).HasColumnName("primarycurrency");
            this.Property(t => t.Verified).HasColumnName("verified");
            this.Property(t => t.LicenceExpiry).HasColumnName("licenceexpiry");
            this.Property(t => t.LicenceLastChecked).HasColumnName("licencelastchecked");
            this.Property(t => t.LicenceCheckedBy).HasColumnName("licencecheckedby");
            this.Property(t => t.LicenceNumber).HasColumnName("licencenumber");
            this.Property(t => t.GroupIdCc).HasColumnName("groupidcc");
            this.Property(t => t.GroupIdPc).HasColumnName("groupidpc");
            this.Property(t => t.CreatedOn).HasColumnName("CreatedOn");
            this.Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            this.Property(t => t.ModifiedOn).HasColumnName("ModifiedOn");
            this.Property(t => t.ModifiedBy).HasColumnName("ModifiedBy");
            this.Property(t => t.Country).HasColumnName("country");
            this.Property(t => t.NiNumber).HasColumnName("ninumber");
            this.Property(t => t.MaidenName).HasColumnName("maidenname");
            this.Property(t => t.MiddleNames).HasColumnName("middlenames");
            this.Property(t => t.Gender).HasColumnName("gender");
            this.Property(t => t.DateOfBirth).HasColumnName("dateofbirth");
            this.Property(t => t.HireDate).HasColumnName("hiredate");
            this.Property(t => t.TerminationDate).HasColumnName("terminationdate");
            this.Property(t => t.HomeLocationId).HasColumnName("homelocationid");
            this.Property(t => t.OfficeLocationId).HasColumnName("officelocationid");
            this.Property(t => t.ApplicantNumber).HasColumnName("applicantnumber");
            this.Property(t => t.ApplicantActiveStatusFlag).HasColumnName("applicantactivestatusflag");
            this.Property(t => t.PasswordMethod).HasColumnName("passwordMethod");
            this.Property(t => t.Name).HasColumnName("name");
            this.Property(t => t.AccountNumber).HasColumnName("accountnumber");
            this.Property(t => t.AccountType).HasColumnName("accounttype");
            this.Property(t => t.SortCode).HasColumnName("sortcode");
            this.Property(t => t.Reference).HasColumnName("reference");
            this.Property(t => t.LocaleId).HasColumnName("localeID");
            this.Property(t => t.NhsTrustId).HasColumnName("NHSTrustID");
            this.Property(t => t.LogonCount).HasColumnName("logonCount");
            this.Property(t => t.RetryCount).HasColumnName("retryCount");
            this.Property(t => t.FirstLogon).HasColumnName("firstLogon");
            this.Property(t => t.LicenceAttachId).HasColumnName("licenceAttachID");
            this.Property(t => t.DefaultSubAccountId).HasColumnName("defaultSubAccountId");
            this.Property(t => t.CacheExpiry).HasColumnName("CacheExpiry");
            this.Property(t => t.SupportPortalAccountId).HasColumnName("supportPortalAccountID");
            this.Property(t => t.SupportPortalPassword).HasColumnName("supportPortalPassword");
            this.Property(t => t.CreationMethod).HasColumnName("CreationMethod");
            this.Property(t => t.MileageTotalDate).HasColumnName("mileagetotaldate");
            this.Property(t => t.AdminOnly).HasColumnName("adminonly");
            this.Property(t => t.Locked).HasColumnName("locked");
            this.Property(t => t.EsrPersonId).HasColumnName("ESRPersonId");
            this.Property(t => t.EsrEffectiveStartDate).HasColumnName("ESREffectiveStartDate");
            this.Property(t => t.EsrEffectiveEndDate).HasColumnName("ESREffectiveEndDate");
            this.Property(t => t.PreferredName).HasColumnName("PreferredName");
            this.Property(t => t.EmployeeNumber).HasColumnName("EmployeeNumber");
            this.Property(t => t.NhsUniqueId).HasColumnName("NHSUniqueId");
            this.Property(t => t.EsrPersonType).HasColumnName("ESRPersonType");
            this.Property(t => t.ContactHelpDeskAllowed).HasColumnName("ContactHelpDeskAllowed");
        }
    }
}
