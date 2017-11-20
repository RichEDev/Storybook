namespace EsrFramework.Models
{
    using System;

    public class Employee
    {
        #region Public Properties

        public DateTime? CacheExpiry { get; set; }
        public bool ContactHelpDeskAllowed { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public byte CreationMethod { get; set; }
        public DateTime? EsrEffectiveEndDate { get; set; }
        public DateTime? EsrEffectiveStartDate { get; set; }
        public long? EsrPersonId { get; set; }
        public string EsrPersonType { get; set; }
        public string EmployeeNumber { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public int? NhsTrustId { get; set; }
        public string NhsUniqueId { get; set; }
        public string PreferredName { get; set; }
        public string AccountNumber { get; set; }
        public string AccountType { get; set; }
        public bool Active { get; set; }
        public int AddItems { get; set; }
        public bool AdminOnly { get; set; }
        public int? AdvanceGroupId { get; set; }
        public bool ApplicantActiveStatusFlag { get; set; }
        public string ApplicantNumber { get; set; }
        public bool Archived { get; set; }
        public string CardNum { get; set; }
        public int? CostCodeId { get; set; }
        public string Country { get; set; }
        public string Creditor { get; set; }
        public int CurClaimNo { get; set; }
        public int CurRefNum { get; set; }
        public bool CustomisedItems { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public int? DefaultSubAccountId { get; set; }
        public int? DepartmentId { get; set; }
        public string Email { get; set; }
        public int EmployeeId { get; set; }
        public string Extension { get; set; }
        public string FaxNo { get; set; }
        public bool FirstLogon { get; set; }
        public string FirstName { get; set; }
        public string Gender { get; set; }
        public int? GroupId { get; set; }
        public int? GroupIdCc { get; set; }
        public int? GroupIdPc { get; set; }
        public string Hint { get; set; }
        public DateTime? HireDate { get; set; }
        public string HomeEmail { get; set; }
        public int? HomeLocationId { get; set; }
        public DateTime? LastChange { get; set; }
        public int? LicenceAttachId { get; set; }
        public int? LicenceCheckedBy { get; set; }
        public DateTime? LicenceExpiry { get; set; }
        public DateTime? LicenceLastChecked { get; set; }
        public string LicenceNumber { get; set; }
        public int? LineManager { get; set; }
        public int? LocaleId { get; set; }
        public bool Locked { get; set; }
        public int LogonCount { get; set; }
        public string MaidenName { get; set; }
        public string MiddleNames { get; set; }
        public int Mileage { get; set; }
        public int MileagePrev { get; set; }
        public int? MileageTotal { get; set; }
        public DateTime? MileageTotalDate { get; set; }
        public string MobileNo { get; set; }
        public string Name { get; set; }
        public string NiNumber { get; set; }
        public int? OfficeLocationId { get; set; }
        public string PagerNo { get; set; }
        public string Password { get; set; }
        public byte? PasswordMethod { get; set; }
        public string Payroll { get; set; }
        public string Position { get; set; }
        public int? PrimaryCountry { get; set; }
        public int? PrimaryCurrency { get; set; }
        public string Reference { get; set; }
        public int RetryCount { get; set; }
        public int? RoleId { get; set; }
        public string SortCode { get; set; }
        public int? Speedo { get; set; }
        public int? SupportPortalAccountId { get; set; }
        public string SupportPortalPassword { get; set; }
        public string Surname { get; set; }
        public string TelNo { get; set; }
        public DateTime? TerminationDate { get; set; }
        public string Title { get; set; }
        public string Username { get; set; }
        public bool UseRole { get; set; }
        public bool Verified { get; set; }

        #endregion
    }
}