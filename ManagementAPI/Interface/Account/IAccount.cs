namespace ManagementAPI.Interface.Account
{
    using ManagementAPI.Enums;
    using System;

    public interface IAccount
    {
        int AccountId { get; set; }
        
        string CompanyName { get; set; }
        
        string CompanyId { get; set; }
        
        string Contact { get; set; }
        
        DateTime Expiry { get; set; }
        
        int DatabaseServer { get; set; }
        
        string DbName { get; set; }
        
        string DbUsername { get; set; }
        
        string DbPassword { get; set; }
        
        bool Archived { get; set; }
        
        bool EmployeeSearchEnabled { get; set; }
        
        bool HotelReviewsEnabled { get; set; }
        
        bool AdvancesEnabled { get; set; }
        
        bool IsNhsCustomer { get; set; }
        
        string ContactEmail { get; set; }
        
        int NumberOfUsers { get; set; }
        
        bool PostcodeAnywhereEnabled { get; set; }
        
        bool CorporateCardsEnabled { get; set; }
        
        bool ContactHelpDeskAllowed { get; set; }
        
        string LicensedUsers { get; set; }
        
        bool MapsEnabled { get; set; }
        
        bool SingleSignonEnabled { get; set; }
        
        AddressLookupProviderEnum AddressLookupProvider { get; set; }
        
        bool AddressLookupsChargeable { get; set; }
        
        bool AddressLookupPsmaAgreement { get; set; }
        
        bool AddressInternationalLookupsAndCoordinates { get; set; }
        
        int AddressLookupsRemaining { get; set; }
        
        int AddressDistanceLookupsRemaining { get; set; }
        
        LicenceTypeEnum LicenceType { get; set; }
        
        bool AnnualContract { get; set; }
        
        string PostcodeAnywhereKey { get; set; }
        
        string PostcodeAnywherePaymentServiceKey { get; set; }
        
        string DvlaLookupKey { get; set; }
        
        bool ReceiptServiceEnabled { get; set; }
        
        bool ValidationServiceEnabled { get; set; }
        
        bool PaymentServiceEnabled { get; set; }
        
        int? DaysToWaitBeforeSentEnvelopeIsMissing { get; set; }
        
        decimal? FundLimit { get; set; }
    }
}