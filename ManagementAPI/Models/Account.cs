namespace ManagementAPI.Models
{
    using System;
    using ManagementAPI.Enums;

    public class Account
    {
        public int AccountId { get; set; }

        // General Details
        public string CompanyName { get; set; }

        public string CompanyId { get; set; }

        public string Contact { get; set; }

        public DateTime Expiry { get; set; }

        public int DatabaseServer { get; set; }

        public string DbName { get; set; }

        public string DbUsername { get; set; }

        public string DbPassword { get; set; }

        public bool Archived { get; set; }

        public bool EmployeeSearchEnabled { get; set; }

        public bool HotelReviewsEnabled { get; set; }

        public bool AdvancesEnabled { get; set; }

        public bool IsNhsCustomer { get; set; }

        public string ContactEmail { get; set; }

        // Licensing
        public int NumberOfUsers { get; set; }

        public bool PostcodeAnywhereEnabled { get; set; }

        public bool CorporateCardsEnabled { get; set; }

        public bool ContactHelpDeskAllowed { get; set; }

        public string LicensedUsers { get; set; }

        public bool MapsEnabled { get; set; }

        public bool SingleSignonEnabled { get; set; }

        public AddressLookupProviderEnum AddressLookupProvider { get; set; }

        public bool AddressLookupsChargeable { get; set; }

        public bool AddressLookupPsmaAgreement { get; set; }

        public bool AddressInternationalLookupsAndCoordinates { get; set; }

        public int AddressLookupsRemaining { get; set; }

        public int AddressDistanceLookupsRemaining { get; set; }

        public LicenceTypeEnum LicenceType { get; set; }

        public bool AnnualContract { get; set; }

        // API Keys
        public string PostcodeAnywhereKey { get; set; }

        public string PostcodeAnywherePaymentServiceKey { get; set; }

        public string DvlaLookupKey { get; set; }

        // Expedite
        public bool ReceiptServiceEnabled { get; set; }

        public bool ValidationServiceEnabled { get; set; }

        public bool PaymentServiceEnabled { get; set; }

        public int? DaysToWaitBeforeSentEnvelopeIsMissing { get; set; }

        public decimal? FundLimit { get; set; }

        public ProductEnum Product { get; set; }
    }
}