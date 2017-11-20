namespace EsrFramework.Models
{
    using System;

    public class EsrPerson
    {
        #region Public Properties

        public DateTime? ActualTerminationDate { get; set; }
        public DateTime? Csd12Months { get; set; }
        public DateTime? Csd3Months { get; set; }
        public string CountryOfBirth { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string DisabilityFlag { get; set; }
        public DateTime? EsrLastUpdateDate { get; set; }
        public long EsrPersonId { get; set; }
        public DateTime? EffectiveEndDate { get; set; }
        public DateTime? EffectiveStartDate { get; set; }
        public string EmployeeNumber { get; set; }
        public string EmployeeStatusFlag { get; set; }
        public string EthnicOrigin { get; set; }
        public string FirstName { get; set; }
        public string Gender { get; set; }
        public DateTime? HireDate { get; set; }
        public string LastName { get; set; }
        public string LegacyPayrollNumber { get; set; }
        public string MaidenName { get; set; }
        public string MaritalStatus { get; set; }
        public string MiddleNames { get; set; }
        public string NhsCrsuuId { get; set; }
        public DateTime? NhsStartDate { get; set; }
        public string NhsUniqueId { get; set; }
        public string NiNumber { get; set; }
        public string Nationality { get; set; }
        public string OfficeEmailAddress { get; set; }
        public string PreferredName { get; set; }
        public string PreviousEmployer { get; set; }
        public string PreviousEmployerType { get; set; }
        public string PreviousLastName { get; set; }
        public string SystemPersonType { get; set; }
        public string TerminationReason { get; set; }
        public string Title { get; set; }
        public string UserPersonType { get; set; }
        public string WtrOptOut { get; set; }
        public DateTime? WtrOptOutDate { get; set; }

        #endregion
    }
}