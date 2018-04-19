namespace BusinessLogic.GeneralOptions
{
    using BusinessLogic.GeneralOptions.AccountMessages;
    using BusinessLogic.GeneralOptions.AddEditExpense;
    using BusinessLogic.GeneralOptions.Addresses;
    using BusinessLogic.GeneralOptions.Admin;
    using BusinessLogic.GeneralOptions.ApprovalMatrix;
    using BusinessLogic.GeneralOptions.AuditLog;
    using BusinessLogic.GeneralOptions.Cache;
    using BusinessLogic.GeneralOptions.Cars;
    using BusinessLogic.GeneralOptions.Claims;
    using BusinessLogic.GeneralOptions.CodeAllocation;
    using BusinessLogic.GeneralOptions.Colours;
    using BusinessLogic.GeneralOptions.CompanyPolicy;
    using BusinessLogic.GeneralOptions.CorporateDiligence;
    using BusinessLogic.GeneralOptions.Countries;
    using BusinessLogic.GeneralOptions.Currencies;
    using BusinessLogic.GeneralOptions.Delegates;
    using BusinessLogic.GeneralOptions.DutyOfCare;
    using BusinessLogic.GeneralOptions.Emails;
    using BusinessLogic.GeneralOptions.Employees;
    using BusinessLogic.GeneralOptions.ESR;
    using BusinessLogic.GeneralOptions.Expedite;
    using BusinessLogic.GeneralOptions.FinancialYear;
    using BusinessLogic.GeneralOptions.Flags;
    using BusinessLogic.GeneralOptions.Framework.Attachments;
    using BusinessLogic.GeneralOptions.Framework.Contracts;
    using BusinessLogic.GeneralOptions.Framework.Invoices;
    using BusinessLogic.GeneralOptions.Framework.Notes;
    using BusinessLogic.GeneralOptions.Framework.Recharge;
    using BusinessLogic.GeneralOptions.Framework.Schedules;
    using BusinessLogic.GeneralOptions.Framework.Search;
    using BusinessLogic.GeneralOptions.Framework.Suppliers;
    using BusinessLogic.GeneralOptions.Framework.Tasks;
    using BusinessLogic.GeneralOptions.HelpAndSupport;
    using BusinessLogic.GeneralOptions.Hotels;
    using BusinessLogic.GeneralOptions.Mileage;
    using BusinessLogic.GeneralOptions.Mobile;
    using BusinessLogic.GeneralOptions.MyDetails;
    using BusinessLogic.GeneralOptions.Page;
    using BusinessLogic.GeneralOptions.Password;
    using BusinessLogic.GeneralOptions.RegionalSettings;
    using BusinessLogic.GeneralOptions.Reports;
    using BusinessLogic.GeneralOptions.Reminders;
    using BusinessLogic.GeneralOptions.SelfRegistration;
    using BusinessLogic.GeneralOptions.SessionTimeout;
    using BusinessLogic.GeneralOptions.Validate;
    using BusinessLogic.GeneralOptions.VatCalculation;

    /// <summary>
    /// Defines a <see cref="IGeneralOptions"/> and it's members
    /// </summary>
    public interface IGeneralOptions
    {
        /// <summary>
        /// Sets the <see cref="IAccountMessagesOptions"/>
        /// </summary>
        /// <returns>An instance of <see cref="IGeneralOptions"/></returns>
        IGeneralOptions WithAccountMessages();

        /// <summary>
        /// Sets the <see cref="IAddEditExpenseOptions"/>
        /// </summary>
        /// <returns>An instance of <see cref="IGeneralOptions"/></returns>
        IGeneralOptions WithAddEditExpense();

        /// <summary>
        /// Sets the <see cref="IAddressOptions"/>
        /// </summary>
        /// <returns>An instance of <see cref="IGeneralOptions"/></returns>
        IGeneralOptions WithAddress();

        /// <summary>
        /// Sets the <see cref="IAdminOptions"/>
        /// </summary>
        /// <returns>An instance of <see cref="IGeneralOptions"/></returns>
        IGeneralOptions WithAdmin();

        /// <summary>
        /// Sets the <see cref="IApprovalMatrixOptions"/>
        /// </summary>
        /// <returns>An instance of <see cref="IGeneralOptions"/></returns>
        IGeneralOptions WithApprovalMatrix();

        /// <summary>
        /// Sets the <see cref="IAuditLogOptions"/>
        /// </summary>
        /// <returns>An instance of <see cref="IGeneralOptions"/></returns>
        IGeneralOptions WithAuditLog();

        /// <summary>
        /// Sets the <see cref="ICarOptions"/>
        /// </summary>
        /// <returns>An instance of <see cref="IGeneralOptions"/></returns>
        IGeneralOptions WithCar();

        /// <summary>
        /// Sets the <see cref="IClaimOptions"/>
        /// </summary>
        /// <returns>An instance of <see cref="IGeneralOptions"/></returns>
        IGeneralOptions WithClaim();

        /// <summary>
        /// Sets the <see cref="ICodeAllocationOptions"/>
        /// </summary>
        /// <returns>An instance of <see cref="IGeneralOptions"/></returns>
        IGeneralOptions WithCodeAllocation();

        /// <summary>
        /// Sets the <see cref="IColourOptions"/>
        /// </summary>
        /// <returns>An instance of <see cref="IGeneralOptions"/></returns>
        IGeneralOptions WithColour();

        /// <summary>
        /// Sets the <see cref="ICompanyPolicyOptions"/>
        /// </summary>
        /// <returns>An instance of <see cref="IGeneralOptions"/></returns>
        IGeneralOptions WithCompanyPolicy();

        /// <summary>
        /// Sets the <see cref="ICountryOptions"/>
        /// </summary>
        /// <returns>An instance of <see cref="IGeneralOptions"/></returns>
        IGeneralOptions WithCountry();

        /// <summary>
        /// Sets the <see cref="ICurrencyOptions"/>
        /// </summary>
        /// <returns>An instance of <see cref="IGeneralOptions"/></returns>
        IGeneralOptions WithCurrency();

        /// <summary>
        /// Sets the <see cref="IDelegateOptions"/>
        /// </summary>
        /// <returns>An instance of <see cref="IGeneralOptions"/></returns>
        IGeneralOptions WithDelegate();

        /// <summary>
        /// Sets the <see cref="IDutyOfCareOptions"/>
        /// </summary>
        /// <returns>An instance of <see cref="IGeneralOptions"/></returns>
        IGeneralOptions WithDutyOfCare();

        /// <summary>
        /// Sets the <see cref="IEmailOptions"/>
        /// </summary>
        /// <returns>An instance of <see cref="IGeneralOptions"/></returns>
        IGeneralOptions WithEmail();

        /// <summary>
        /// Sets the <see cref="IEmployeeOptions"/>
        /// </summary>
        /// <returns>An instance of <see cref="IGeneralOptions"/></returns>
        IGeneralOptions WithEmployee();

        /// <summary>
        /// Sets the <see cref="IESROptions"/>
        /// </summary>
        /// <returns>An instance of <see cref="IGeneralOptions"/></returns>
        IGeneralOptions WithESR();

        /// <summary>
        /// Sets the <see cref="IExpediteOptions"/>
        /// </summary>
        /// <returns>An instance of <see cref="IGeneralOptions"/></returns>
        IGeneralOptions WithExpedite();

        /// <summary>
        /// Sets the <see cref="IFlagOptions"/>
        /// </summary>
        /// <returns>An instance of <see cref="IGeneralOptions"/></returns>
        IGeneralOptions WithFlag();

        /// <summary>
        /// Sets the <see cref="IAttachmentOptions"/>
        /// </summary>
        /// <returns>An instance of <see cref="IGeneralOptions"/></returns>
        IGeneralOptions WithAttachments();

        /// <summary>
        /// Sets the <see cref="IContractOptions"/>
        /// </summary>
        /// <returns>An instance of <see cref="IGeneralOptions"/></returns>
        IGeneralOptions WithContracts();

        /// <summary>
        /// Sets the <see cref="IInvoicesOptions"/>
        /// </summary>
        /// <returns>An instance of <see cref="IGeneralOptions"/></returns>
        IGeneralOptions WithInvoices();

        /// <summary>
        /// Sets the <see cref="INotesOptions"/>
        /// </summary>
        /// <returns>An instance of <see cref="IGeneralOptions"/></returns>
        IGeneralOptions WithNotes();

        /// <summary>
        /// Sets the <see cref="IRechargeOptions"/>
        /// </summary>
        /// <returns>An instance of <see cref="IGeneralOptions"/></returns>
        IGeneralOptions WithRecharge();

        /// <summary>
        /// Sets the <see cref="IScheduleOptions"/>
        /// </summary>
        /// <returns>An instance of <see cref="IGeneralOptions"/></returns>
        IGeneralOptions WithSchedule();

        /// <summary>
        /// Sets the <see cref="ISearchOptions"/>
        /// </summary>
        /// <returns>An instance of <see cref="IGeneralOptions"/></returns>
        IGeneralOptions WithSearch();

        /// <summary>
        /// Sets the <see cref="ISupplierOptions"/>
        /// </summary>
        /// <returns>An instance of <see cref="IGeneralOptions"/></returns>
        IGeneralOptions WithSupplier();

        /// <summary>
        /// Sets the <see cref="ITaskOptions"/>
        /// </summary>
        /// <returns>An instance of <see cref="IGeneralOptions"/></returns>
        IGeneralOptions WithTask();

        /// <summary>
        /// Sets the <see cref="IHelpAndSupportOptions"/>
        /// </summary>
        /// <returns>An instance of <see cref="IGeneralOptions"/></returns>
        IGeneralOptions WithHelpAndSupport();

        /// <summary>
        /// Sets the <see cref="IMileageOptions"/>
        /// </summary>
        /// <returns>An instance of <see cref="IGeneralOptions"/></returns>
        IGeneralOptions WithMileage();

        /// <summary>
        /// Sets the <see cref="IMobileOptions"/>
        /// </summary>
        /// <returns>An instance of <see cref="IGeneralOptions"/></returns>
        IGeneralOptions WithMobile();

        /// <summary>
        /// Sets the <see cref="IMyDetailsOptions"/>
        /// </summary>
        /// <returns>An instance of <see cref="IGeneralOptions"/></returns>
        IGeneralOptions WithMyDetails();

        /// <summary>
        /// Sets the <see cref="IPasswordOptions"/>
        /// </summary>
        /// <returns>An instance of <see cref="IGeneralOptions"/></returns>
        IGeneralOptions WithPassword();

        /// <summary>
        /// Sets the <see cref="IRegionalSettingsOptions"/>
        /// </summary>
        /// <returns>An instance of <see cref="IGeneralOptions"/></returns>
        IGeneralOptions WithRegionalSettings();

        /// <summary>
        /// Sets the <see cref="IReportOptions"/>
        /// </summary>
        /// <returns>An instance of <see cref="IGeneralOptions"/></returns>
        IGeneralOptions WithReport();

        /// <summary>
        /// Sets the <see cref="ISelfRegistrationOptions"/>
        /// </summary>
        /// <returns>An instance of <see cref="IGeneralOptions"/></returns>
        IGeneralOptions WithSelfRegistration();

        /// <summary>
        /// Sets the <see cref="ISessionTimeoutOptions"/>
        /// </summary>
        /// <returns>An instance of <see cref="IGeneralOptions"/></returns>
        IGeneralOptions WithSessionTimeout();

        /// <summary>
        /// Sets the <see cref="IValidateOptions"/>
        /// </summary>
        /// <returns>An instance of <see cref="IGeneralOptions"/></returns>
        IGeneralOptions WithValidate();

        /// <summary>
        /// Sets the <see cref="IVatCalculationOptions"/>
        /// </summary>
        /// <returns>An instance of <see cref="IGeneralOptions"/></returns>
        IGeneralOptions WithVatCalculation();

        /// <summary>
        /// Sets the <see cref="ICacheOptions"/>
        /// </summary>
        /// <returns>An instance of <see cref="IGeneralOptions"/></returns>
        IGeneralOptions WithCache();

        /// <summary>
        /// Sets the <see cref="ICorporateDiligenceOptions"/>
        /// </summary>
        /// <returns>An instance of <see cref="IGeneralOptions"/></returns>
        IGeneralOptions WithCorporateDiligence();

        /// <summary>
        /// Sets the <see cref="IPageOptions"/>
        /// </summary>
        /// <returns>An instance of <see cref="IGeneralOptions"/></returns>
        IGeneralOptions WithPage();

        /// <summary>
        /// Sets the <see cref="IFinancialYearOptions"/>
        /// </summary>
        /// <returns>An instance of <see cref="IGeneralOptions"/></returns>
        IGeneralOptions WithFinancialYear();

        /// <summary>
        /// Sets the <see cref="IHotelOptions"/>
        /// </summary>
        /// <returns>An instance of <see cref="IGeneralOptions"/></returns>
        IGeneralOptions WithHotel();

        /// <summary>
        /// Sets the <see cref="IReminderOptions"/>
        /// </summary>
        /// <returns>An instance of <see cref="IGeneralOptions"/></returns>
        IGeneralOptions WithReminders();

        /// <summary>
        /// Sets all options
        /// </summary>
        /// <returns>An instance of <see cref="IGeneralOptions"/></returns>
        IGeneralOptions WithAll();

        /// <summary>
        /// Gets or set the <see cref="IAddEditExpenseOptions"/>
        /// </summary>
        IAddEditExpenseOptions AddEditExpense { get; set; }

        /// <summary>
        /// Gets or set the <see cref="IAccountMessagesOptions"/>
        /// </summary>
        IAccountMessagesOptions AccountMessages { get; set; }

        /// <summary>
        /// Gets or set the <see cref="IAddressOptions"/>
        /// </summary>
        IAddressOptions Addresses { get; set; }

        /// <summary>
        /// Gets or set the <see cref="IAdminOptions"/>
        /// </summary>
        IAdminOptions Admin { get; set; }

        /// <summary>
        /// Gets or set the <see cref="IApprovalMatrixOptions"/>
        /// </summary>
        IApprovalMatrixOptions ApprovalMatrix { get; set; }

        /// <summary>
        /// Gets or set the <see cref="IAuditLogOptions"/>
        /// </summary>
        IAuditLogOptions AuditLog { get; set; }

        /// <summary>
        /// Gets or set the <see cref="ICarOptions"/>
        /// </summary>
        ICarOptions Car { get; set; }

        /// <summary>
        /// Gets or set the <see cref="IClaimOptions"/>
        /// </summary>
        IClaimOptions Claim { get; set; }

        /// <summary>
        /// Gets or set the <see cref="ICodeAllocationOptions"/>
        /// </summary>
        ICodeAllocationOptions CodeAllocation { get; set; }

        /// <summary>
        /// Gets or set the <see cref="IColourOptions"/>
        /// </summary>
        IColourOptions Colour { get; set; }

        /// <summary>
        /// Gets or set the <see cref="ICompanyPolicyOptions"/>
        /// </summary>
        ICompanyPolicyOptions CompanyPolicy { get; set; }

        /// <summary>
        /// Gets or set the <see cref="ICountryOptions"/>
        /// </summary>
        ICountryOptions Country { get; set; }

        /// <summary>
        /// Gets or set the <see cref="ICurrencyOptions"/>
        /// </summary>
        ICurrencyOptions Currency { get; set; }

        /// <summary>
        /// Gets or set the <see cref="IDelegateOptions"/>
        /// </summary>
        IDelegateOptions Delegate { get; set; }

        /// <summary>
        /// Gets or set the <see cref="IDutyOfCareOptions"/>
        /// </summary>
        IDutyOfCareOptions DutyOfCare { get; set; }

        /// <summary>
        /// Gets or set the <see cref="IEmailOptions"/>
        /// </summary>
        IEmailOptions Email { get; set; }

        /// <summary>
        /// Gets or set the <see cref="IEmployeeOptions"/>
        /// </summary>
        IEmployeeOptions Employee { get; set; }

        /// <summary>
        /// Gets or set the <see cref="IESROptions"/>
        /// </summary>
        IESROptions ESR { get; set; }

        /// <summary>
        /// Gets or set the <see cref="IExpediteOptions"/>
        /// </summary>
        IExpediteOptions Expedite { get; set; }

        /// <summary>
        /// Gets or set the <see cref="IFlagOptions"/>
        /// </summary>
        IFlagOptions Flag { get; set; }

        /// <summary>
        /// Gets or set the <see cref="IAttachmentOptions"/>
        /// </summary>
        IAttachmentOptions Attachment { get; set; }

        /// <summary>
        /// Gets or set the <see cref="IContractOptions"/>
        /// </summary>
        IContractOptions Contract { get; set; }

        /// <summary>
        /// Gets or set the <see cref="IInvoicesOptions"/>
        /// </summary>
        IInvoicesOptions Invoices { get; set; }

        /// <summary>
        /// Gets or set the <see cref="INotesOptions"/>
        /// </summary>
        INotesOptions Notes { get; set; }

        /// <summary>
        /// Gets or set the <see cref="IRechargeOptions"/>
        /// </summary>
        IRechargeOptions Recharge { get; set; }

        /// <summary>
        /// Gets or set the <see cref="IScheduleOptions"/>
        /// </summary>
        IScheduleOptions Schedule { get; set; }

        /// <summary>
        /// Gets or set the <see cref="ISearchOptions"/>
        /// </summary>
        ISearchOptions Search { get; set; }

        /// <summary>
        /// Gets or set the <see cref="ISupplierOptions"/>
        /// </summary>
        ISupplierOptions Supplier { get; set; }

        /// <summary>
        /// Gets or set the <see cref="ITaskOptions"/>
        /// </summary>
        ITaskOptions Task { get; set; }

        /// <summary>
        /// Gets or set the <see cref="IHelpAndSupportOptions"/>
        /// </summary>
        IHelpAndSupportOptions HelpAndSupport { get; set; }

        /// <summary>
        /// Gets or set the <see cref="IMileageOptions"/>
        /// </summary>
        IMileageOptions Mileage { get; set; }

        /// <summary>
        /// Gets or set the <see cref="IMobileOptions"/>
        /// </summary>
        IMobileOptions Mobile { get; set; }

        /// <summary>
        /// Gets or set the <see cref="IMyDetailsOptions"/>
        /// </summary>
        IMyDetailsOptions MyDetails { get; set; }

        /// <summary>
        /// Gets or set the <see cref="IPasswordOptions"/>
        /// </summary>
        IPasswordOptions Password { get; set; }

        /// <summary>
        /// Gets or set the <see cref="IRegionalSettingsOptions"/>
        /// </summary>
        IRegionalSettingsOptions RegionalSettings { get; set; }

        /// <summary>
        /// Gets or set the <see cref="IReportOptions"/>
        /// </summary>
        IReportOptions Report { get; set; }

        /// <summary>
        /// Gets or set the <see cref="ISelfRegistrationOptions"/>
        /// </summary>
        ISelfRegistrationOptions SelfRegistration { get; set; }

        /// <summary>
        /// Gets or set the <see cref="ISessionTimeoutOptions"/>
        /// </summary>
        ISessionTimeoutOptions SessionTimeout { get; set; }

        /// <summary>
        /// Gets or set the <see cref="IValidateOptions"/>
        /// </summary>
        IValidateOptions Validate { get; set; }

        /// <summary>
        /// Gets or set the <see cref="IVatCalculationOptions"/>
        /// </summary>
        IVatCalculationOptions VatCalculation { get; set; }

        /// <summary>
        /// Gets or set the <see cref="ICacheOptions"/>
        /// </summary>
        ICacheOptions Cache { get; set; }

        /// <summary>
        /// Gets or set the <see cref="ICorporateDiligenceOptions"/>
        /// </summary>
        ICorporateDiligenceOptions CorporateDiligence { get; set; }

        /// <summary>
        /// Gets or set the <see cref="IPageOptions"/>
        /// </summary>
        IPageOptions Page { get; set; }

        /// <summary>
        /// Gets or set the <see cref="IFinancialYearOptions"/>
        /// </summary>
        IFinancialYearOptions FinancialYear { get; set; }

        /// <summary>
        /// Gets or set the <see cref="IHotelOptions"/>
        /// </summary>
        IHotelOptions Hotel { get; set; }

        /// <summary>
        /// Gets or set the <see cref="IReminderOptions"/>
        /// </summary>
        IReminderOptions Reminders { get; set; }

        /// <summary>
        /// Gets or set the language option
        /// </summary>
        string Language { get; set; }

        /// <summary>
        /// Gets or sets the SubAccountId
        /// </summary>
        int SubAccountID { get; set; }

        /// <summary>
        /// Gets or sets the ApplicationURL
        /// </summary>
        string ApplicationURL { get; set; }
    }
}
