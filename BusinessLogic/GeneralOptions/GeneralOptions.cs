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
    using BusinessLogic.GeneralOptions.Reminders;
    using BusinessLogic.GeneralOptions.Reports;
    using BusinessLogic.GeneralOptions.SelfRegistration;
    using BusinessLogic.GeneralOptions.SessionTimeout;
    using BusinessLogic.GeneralOptions.Validate;
    using BusinessLogic.GeneralOptions.VatCalculation;

    /// <summary>
    /// Defines a <see cref="GeneralOptions"/> and it's members
    /// </summary>
    public abstract class GeneralOptions : IGeneralOptions
    {
        /// <summary>
        /// Sets the <see cref="IAccountMessagesOptions"/>
        /// </summary>
        /// <returns>An instance of <see cref="IGeneralOptions"/></returns>
        public abstract IGeneralOptions WithAccountMessages();

        /// <summary>
        /// Sets the <see cref="IAddEditExpenseOptions"/>
        /// </summary>
        /// <returns>An instance of <see cref="IGeneralOptions"/></returns>
        public abstract IGeneralOptions WithAddEditExpense();

        /// <summary>
        /// Sets the <see cref="IAddressOptions"/>
        /// </summary>
        /// <returns>An instance of <see cref="IGeneralOptions"/></returns>
        public abstract IGeneralOptions WithAddress();

        /// <summary>
        /// Sets the <see cref="IAdminOptions"/>
        /// </summary>
        /// <returns>An instance of <see cref="IGeneralOptions"/></returns>
        public abstract IGeneralOptions WithAdmin();

        /// <summary>
        /// Sets the <see cref="IApprovalMatrixOptions"/>
        /// </summary>
        /// <returns>An instance of <see cref="IGeneralOptions"/></returns>
        public abstract IGeneralOptions WithApprovalMatrix();

        /// <summary>
        /// Sets the <see cref="IAuditLogOptions"/>
        /// </summary>
        /// <returns>An instance of <see cref="IGeneralOptions"/></returns>
        public abstract IGeneralOptions WithAuditLog();

        /// <summary>
        /// Sets the <see cref="ICarOptions"/>
        /// </summary>
        /// <returns>An instance of <see cref="IGeneralOptions"/></returns>
        public abstract IGeneralOptions WithCar();

        /// <summary>
        /// Sets the <see cref="IClaimOptions"/>
        /// </summary>
        /// <returns>An instance of <see cref="IGeneralOptions"/></returns>
        public abstract IGeneralOptions WithClaim();

        /// <summary>
        /// Sets the <see cref="ICodeAllocationOptions"/>
        /// </summary>
        /// <returns>An instance of <see cref="IGeneralOptions"/></returns>
        public abstract IGeneralOptions WithCodeAllocation();

        /// <summary>
        /// Sets the <see cref="IColourOptions"/>
        /// </summary>
        /// <returns>An instance of <see cref="IGeneralOptions"/></returns>
        public abstract IGeneralOptions WithColour();

        /// <summary>
        /// Sets the <see cref="ICompanyPolicyOptions"/>
        /// </summary>
        /// <returns>An instance of <see cref="IGeneralOptions"/></returns>
        public abstract IGeneralOptions WithCompanyPolicy();

        /// <summary>
        /// Sets the <see cref="ICountryOptions"/>
        /// </summary>
        /// <returns>An instance of <see cref="IGeneralOptions"/></returns>
        public abstract IGeneralOptions WithCountry();

        /// <summary>
        /// Sets the <see cref="ICurrencyOptions"/>
        /// </summary>
        /// <returns>An instance of <see cref="IGeneralOptions"/></returns>
        public abstract IGeneralOptions WithCurrency();

        /// <summary>
        /// Sets the <see cref="IDelegateOptions"/>
        /// </summary>
        /// <returns>An instance of <see cref="IGeneralOptions"/></returns>
        public abstract IGeneralOptions WithDelegate();

        /// <summary>
        /// Sets the <see cref="IDutyOfCareOptions"/>
        /// </summary>
        /// <returns>An instance of <see cref="IGeneralOptions"/></returns>
        public abstract IGeneralOptions WithDutyOfCare();

        /// <summary>
        /// Sets the <see cref="IEmailOptions"/>
        /// </summary>
        /// <returns>An instance of <see cref="IGeneralOptions"/></returns>
        public abstract IGeneralOptions WithEmail();

        /// <summary>
        /// Sets the <see cref="IEmployeeOptions"/>
        /// </summary>
        /// <returns>An instance of <see cref="IGeneralOptions"/></returns>
        public abstract IGeneralOptions WithEmployee();

        /// <summary>
        /// Sets the <see cref="IESROptions"/>
        /// </summary>
        /// <returns>An instance of <see cref="IGeneralOptions"/></returns>
        public abstract IGeneralOptions WithESR();

        /// <summary>
        /// Sets the <see cref="IExpediteOptions"/>
        /// </summary>
        /// <returns>An instance of <see cref="IGeneralOptions"/></returns>
        public abstract IGeneralOptions WithExpedite();

        /// <summary>
        /// Sets the <see cref="IFlagOptions"/>
        /// </summary>
        /// <returns>An instance of <see cref="IGeneralOptions"/></returns>
        public abstract IGeneralOptions WithFlag();

        /// <summary>
        /// Sets the <see cref="IAttachmentOptions"/>
        /// </summary>
        /// <returns>An instance of <see cref="IGeneralOptions"/></returns>
        public abstract IGeneralOptions WithAttachments();

        /// <summary>
        /// Sets the <see cref="IContractOptions"/>
        /// </summary>
        /// <returns>An instance of <see cref="IGeneralOptions"/></returns>
        public abstract IGeneralOptions WithContracts();

        /// <summary>
        /// Sets the <see cref="IInvoicesOptions"/>
        /// </summary>
        /// <returns>An instance of <see cref="IGeneralOptions"/></returns>
        public abstract IGeneralOptions WithInvoices();

        /// <summary>
        /// Sets the <see cref="INotesOptions"/>
        /// </summary>
        /// <returns>An instance of <see cref="IGeneralOptions"/></returns>
        public abstract IGeneralOptions WithNotes();

        /// <summary>
        /// Sets the <see cref="IRechargeOptions"/>
        /// </summary>
        /// <returns>An instance of <see cref="IGeneralOptions"/></returns>
        public abstract IGeneralOptions WithRecharge();

        /// <summary>
        /// Sets the <see cref="IScheduleOptions"/>
        /// </summary>
        /// <returns>An instance of <see cref="IGeneralOptions"/></returns>
        public abstract IGeneralOptions WithSchedule();

        /// <summary>
        /// Sets the <see cref="ISearchOptions"/>
        /// </summary>
        /// <returns>An instance of <see cref="IGeneralOptions"/></returns>
        public abstract IGeneralOptions WithSearch();

        /// <summary>
        /// Sets the <see cref="ISupplierOptions"/>
        /// </summary>
        /// <returns>An instance of <see cref="IGeneralOptions"/></returns>
        public abstract IGeneralOptions WithSupplier();

        /// <summary>
        /// Sets the <see cref="ITaskOptions"/>
        /// </summary>
        /// <returns>An instance of <see cref="IGeneralOptions"/></returns>
        public abstract IGeneralOptions WithTask();

        /// <summary>
        /// Sets the <see cref="IHelpAndSupportOptions"/>
        /// </summary>
        /// <returns>An instance of <see cref="IGeneralOptions"/></returns>
        public abstract IGeneralOptions WithHelpAndSupport();

        /// <summary>
        /// Sets the <see cref="IMileageOptions"/>
        /// </summary>
        /// <returns>An instance of <see cref="IGeneralOptions"/></returns>
        public abstract IGeneralOptions WithMileage();

        /// <summary>
        /// Sets the <see cref="IMobileOptions"/>
        /// </summary>
        /// <returns>An instance of <see cref="IGeneralOptions"/></returns>
        public abstract IGeneralOptions WithMobile();

        /// <summary>
        /// Sets the <see cref="IMyDetailsOptions"/>
        /// </summary>
        /// <returns>An instance of <see cref="IGeneralOptions"/></returns>
        public abstract IGeneralOptions WithMyDetails();

        /// <summary>
        /// Sets the <see cref="IPasswordOptions"/>
        /// </summary>
        /// <returns>An instance of <see cref="IGeneralOptions"/></returns>
        public abstract IGeneralOptions WithPassword();

        /// <summary>
        /// Sets the <see cref="IRegionalSettingsOptions"/>
        /// </summary>
        /// <returns>An instance of <see cref="IGeneralOptions"/></returns>
        public abstract IGeneralOptions WithRegionalSettings();

        /// <summary>
        /// Sets the <see cref="IReportOptions"/>
        /// </summary>
        /// <returns>An instance of <see cref="IGeneralOptions"/></returns>
        public abstract IGeneralOptions WithReport();

        /// <summary>
        /// Sets the <see cref="ISelfRegistrationOptions"/>
        /// </summary>
        /// <returns>An instance of <see cref="IGeneralOptions"/></returns>
        public abstract IGeneralOptions WithSelfRegistration();

        /// <summary>
        /// Sets the <see cref="ISessionTimeoutOptions"/>
        /// </summary>
        /// <returns>An instance of <see cref="IGeneralOptions"/></returns>
        public abstract IGeneralOptions WithSessionTimeout();

        /// <summary>
        /// Sets the <see cref="IValidateOptions"/>
        /// </summary>
        /// <returns>An instance of <see cref="IGeneralOptions"/></returns>
        public abstract IGeneralOptions WithValidate();

        /// <summary>
        /// Sets the <see cref="IVatCalculationOptions"/>
        /// </summary>
        /// <returns>An instance of <see cref="IGeneralOptions"/></returns>
        public abstract IGeneralOptions WithVatCalculation();

        /// <summary>
        /// Sets the <see cref="ICacheOptions"/>
        /// </summary>
        /// <returns>An instance of <see cref="IGeneralOptions"/></returns>
        public abstract IGeneralOptions WithCache();

        /// <summary>
        /// Sets the <see cref="ICorporateDiligenceOptions"/>
        /// </summary>
        /// <returns>An instance of <see cref="IGeneralOptions"/></returns>
        public abstract IGeneralOptions WithCorporateDiligence();

        /// <summary>
        /// Sets the <see cref="IPageOptions"/>
        /// </summary>
        /// <returns>An instance of <see cref="IGeneralOptions"/></returns>
        public abstract IGeneralOptions WithPage();

        /// <summary>
        /// Sets the <see cref="IFinancialYearOptions"/>
        /// </summary>
        /// <returns>An instance of <see cref="IGeneralOptions"/></returns>
        public abstract IGeneralOptions WithFinancialYear();

        /// <summary>
        /// Sets the <see cref="IHotelOptions"/>
        /// </summary>
        /// <returns>An instance of <see cref="IGeneralOptions"/></returns>
        public abstract IGeneralOptions WithHotel();

        /// <summary>
        /// Sets the <see cref="IReminderOptions"/>
        /// </summary>
        /// <returns>An instance of <see cref="IGeneralOptions"/></returns>
        public abstract IGeneralOptions WithReminders();

        /// <summary>
        /// Sets all options
        /// </summary>
        /// <returns>An instance of <see cref="IGeneralOptions"/></returns>
        public abstract IGeneralOptions WithAll();

        #region Properties

        /// <summary>
        /// Gets or set the <see cref="ICountryOptions"/>
        /// </summary>
        public ICountryOptions Country { get; set; }

        /// <summary>
        /// Gets or set the <see cref="ICurrencyOptions"/>
        /// </summary>
        public ICurrencyOptions Currency { get; set; }

        /// <summary>
        /// Gets or set the <see cref="IDelegateOptions"/>
        /// </summary>
        public IDelegateOptions Delegate { get; set; }

        /// <summary>
        /// Gets or set the <see cref="IDutyOfCareOptions"/>
        /// </summary>
        public IDutyOfCareOptions DutyOfCare { get; set; }

        /// <summary>
        /// Gets or set the <see cref="IEmailOptions"/>
        /// </summary>
        public IEmailOptions Email { get; set; }

        /// <summary>
        /// Gets or set the <see cref="IEmployeeOptions"/>
        /// </summary>
        public IEmployeeOptions Employee { get; set; }

        /// <summary>
        /// Gets or set the <see cref="IESROptions"/>
        /// </summary>
        public IESROptions ESR { get; set; }

        /// <summary>
        /// Gets or set the <see cref="IExpediteOptions"/>
        /// </summary>
        public IExpediteOptions Expedite { get; set; }

        /// <summary>
        /// Gets or set the <see cref="IFlagOptions"/>
        /// </summary>
        public IFlagOptions Flag { get; set; }

        /// <summary>
        /// Gets or set the <see cref="IAttachmentOptions"/>
        /// </summary>
        public IAttachmentOptions Attachment { get; set; }

        /// <summary>
        /// Gets or set the <see cref="IContractOptions"/>
        /// </summary>
        public IContractOptions Contract { get; set; }

        /// <summary>
        /// Gets or set the <see cref="IInvoicesOptions"/>
        /// </summary>
        public IInvoicesOptions Invoices { get; set; }

        /// <summary>
        /// Gets or set the <see cref="INotesOptions"/>
        /// </summary>
        public INotesOptions Notes { get; set; }

        /// <summary>
        /// Gets or set the <see cref="IRechargeOptions"/>
        /// </summary>
        public IRechargeOptions Recharge { get; set; }

        /// <summary>
        /// Gets or set the <see cref="IScheduleOptions"/>
        /// </summary>
        public IScheduleOptions Schedule { get; set; }

        /// <summary>
        /// Gets or set the <see cref="ISearchOptions"/>
        /// </summary>
        public ISearchOptions Search { get; set; }

        /// <summary>
        /// Gets or set the <see cref="ISupplierOptions"/>
        /// </summary>
        public ISupplierOptions Supplier { get; set; }

        /// <summary>
        /// Gets or set the <see cref="ITaskOptions"/>
        /// </summary>
        public ITaskOptions Task { get; set; }

        /// <summary>
        /// Gets or set the <see cref="IHelpAndSupportOptions"/>
        /// </summary>
        public IHelpAndSupportOptions HelpAndSupport { get; set; }

        /// <summary>
        /// Gets or set the <see cref="IMileageOptions"/>
        /// </summary>
        public IMileageOptions Mileage { get; set; }

        /// <summary>
        /// Gets or set the <see cref="IMobileOptions"/>
        /// </summary>
        public IMobileOptions Mobile { get; set; }

        /// <summary>
        /// Gets or set the <see cref="IMyDetailsOptions"/>
        /// </summary>
        public IMyDetailsOptions MyDetails { get; set; }

        /// <summary>
        /// Gets or set the <see cref="IPasswordOptions"/>
        /// </summary>
        public IPasswordOptions Password { get; set; }

        /// <summary>
        /// Gets or set the <see cref="IRegionalSettingsOptions"/>
        /// </summary>
        public IRegionalSettingsOptions RegionalSettings { get; set; }

        /// <summary>
        /// Gets or set the <see cref="IReportOptions"/>
        /// </summary>
        public IReportOptions Report { get; set; }

        /// <summary>
        /// Gets or set the <see cref="ISelfRegistrationOptions"/>
        /// </summary>
        public ISelfRegistrationOptions SelfRegistration { get; set; }

        /// <summary>
        /// Gets or set the <see cref="ISessionTimeoutOptions"/>
        /// </summary>
        public ISessionTimeoutOptions SessionTimeout { get; set; }

        /// <summary>
        /// Gets or set the <see cref="IValidateOptions"/>
        /// </summary>
        public IValidateOptions Validate { get; set; }

        /// <summary>
        /// Gets or set the <see cref="IVatCalculationOptions"/>
        /// </summary>
        public IVatCalculationOptions VatCalculation { get; set; }

        /// <summary>
        /// Gets or set the <see cref="ICacheOptions"/>
        /// </summary>
        public ICacheOptions Cache { get; set; }

        /// <summary>
        /// Gets or set the <see cref="ICorporateDiligenceOptions"/>
        /// </summary>
        public ICorporateDiligenceOptions CorporateDiligence { get; set; }

        /// <summary>
        /// Gets or set the <see cref="IPageOptions"/>
        /// </summary>
        public IPageOptions Page { get; set; }

        /// <summary>
        /// Gets or set the <see cref="IFinancialYearOptions"/>
        /// </summary>
        public IFinancialYearOptions FinancialYear { get; set; }

        /// <summary>
        /// Gets or set the <see cref="IAccountMessagesOptions"/>
        /// </summary>
        public IAccountMessagesOptions AccountMessages { get; set; }

        /// <summary>
        /// Gets or set the <see cref="IAddressOptions"/>
        /// </summary>
        public IAddressOptions Addresses { get; set; }

        /// <summary>
        /// Gets or set the <see cref="IAdminOptions"/>
        /// </summary>
        public IAdminOptions Admin { get; set; }

        /// <summary>
        /// Gets or set the <see cref="IApprovalMatrixOptions"/>
        /// </summary>
        public IApprovalMatrixOptions ApprovalMatrix { get; set; }

        /// <summary>
        /// Gets or set the <see cref="IAuditLogOptions"/>
        /// </summary>
        public IAuditLogOptions AuditLog { get; set; }

        /// <summary>
        /// Gets or set the <see cref="ICarOptions"/>
        /// </summary>
        public ICarOptions Car { get; set; }

        /// <summary>
        /// Gets or set the <see cref="IClaimOptions"/>
        /// </summary>
        public IClaimOptions Claim { get; set; }

        /// <summary>
        /// Gets or set the <see cref="ICodeAllocationOptions"/>
        /// </summary>
        public ICodeAllocationOptions CodeAllocation { get; set; }

        /// <summary>
        /// Gets or set the <see cref="IColourOptions"/>
        /// </summary>
        public IColourOptions Colour { get; set; }

        /// <summary>
        /// Gets or set the <see cref="ICompanyPolicyOptions"/>
        /// </summary>
        public ICompanyPolicyOptions CompanyPolicy { get; set; }

        /// <summary>
        /// Gets or set the <see cref="IAddEditExpenseOptions"/>
        /// </summary>
        public IAddEditExpenseOptions AddEditExpense { get; set; }

        /// <summary>
        /// Gets or set the <see cref="IHotelOptions"/>
        /// </summary>
        public IHotelOptions Hotel { get; set; }

        /// <summary>
        /// Gets or set the <see cref="IReminderOptions"/>
        /// </summary>
        public IReminderOptions Reminders { get; set; }

        /// <summary>
        /// Gets or set the language option
        /// </summary>
        public string Language { get; set; }

        /// <summary>
        /// Gets or sets the SubAccountId
        /// </summary>
        public int SubAccountID { get; set; }

        /// <summary>
        /// Gets or sets the ApplicationURL
        /// </summary>
        public string ApplicationURL { get; set; }

        #endregion
    }
}
