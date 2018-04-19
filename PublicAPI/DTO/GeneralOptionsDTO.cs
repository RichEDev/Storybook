namespace PublicAPI.DTO
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
    /// Defines a <see cref="GeneralOptionsDTO"/> and it's members
    /// </summary>
    public class GeneralOptionsDTO
    {
        /// <summary>
        /// Gets or sets the <see cref="ICountryOptions"/>
        /// </summary>
        public ICountryOptions Country { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="ICurrencyOptions"/>
        /// </summary>
        public ICurrencyOptions Currency { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IDelegateOptions"/>
        /// </summary>
        public IDelegateOptions Delegate { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IDutyOfCareOptions"/>
        /// </summary>
        public IDutyOfCareOptions DutyOfCare { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IEmailOptions"/>
        /// </summary>
        public IEmailOptions Email { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IEmployeeOptions"/>
        /// </summary>
        public IEmployeeOptions Employee { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IESROptions"/>
        /// </summary>
        public IESROptions ESR { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IExpediteOptions"/>
        /// </summary>
        public IExpediteOptions Expedite { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IFlagOptions"/>
        /// </summary>
        public IFlagOptions Flag { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IAttachmentOptions"/>
        /// </summary>
        public IAttachmentOptions Attachment { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IContractOptions"/>
        /// </summary>
        public IContractOptions Contract { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IInvoicesOptions"/>
        /// </summary>
        public IInvoicesOptions Invoices { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="INotesOptions"/>
        /// </summary>
        public INotesOptions Notes { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IRechargeOptions"/>
        /// </summary>
        public IRechargeOptions Recharge { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IScheduleOptions"/>
        /// </summary>
        public IScheduleOptions Schedule { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="ISearchOptions"/>
        /// </summary>
        public ISearchOptions Search { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="ISupplierOptions"/>
        /// </summary>
        public ISupplierOptions Supplier { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="ITaskOptions"/>
        /// </summary>
        public ITaskOptions Task { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IHelpAndSupportOptions"/>
        /// </summary>
        public IHelpAndSupportOptions HelpAndSupport { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IMileageOptions"/>
        /// </summary>
        public IMileageOptions Mileage { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IMobileOptions"/>
        /// </summary>
        public IMobileOptions Mobile { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IMyDetailsOptions"/>
        /// </summary>
        public IMyDetailsOptions MyDetails { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IPasswordOptions"/>
        /// </summary>
        public IPasswordOptions Password { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IRegionalSettingsOptions"/>
        /// </summary>
        public IRegionalSettingsOptions RegionalSettings { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IReportOptions"/>
        /// </summary>
        public IReportOptions Report { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="ISelfRegistrationOptions"/>
        /// </summary>
        public ISelfRegistrationOptions SelfRegistration { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="ISessionTimeoutOptions"/>
        /// </summary>
        public ISessionTimeoutOptions SessionTimeout { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IValidateOptions"/>
        /// </summary>
        public IValidateOptions Validate { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IVatCalculationOptions"/>
        /// </summary>
        public IVatCalculationOptions VatCalculation { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="ICacheOptions"/>
        /// </summary>
        public ICacheOptions Cache { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="ICorporateDiligenceOptions"/>
        /// </summary>
        public ICorporateDiligenceOptions CorporateDiligence { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IPageOptions"/>
        /// </summary>
        public IPageOptions Page { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IFinancialYearOptions"/>
        /// </summary>
        public IFinancialYearOptions FinancialYear { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IAccountMessagesOptions"/>
        /// </summary>
        public IAccountMessagesOptions AccountMessages { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IAddressOptions"/>
        /// </summary>
        public IAddressOptions Addresses { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IAdminOptions"/>
        /// </summary>
        public IAdminOptions Admin { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IApprovalMatrixOptions"/>
        /// </summary>
        public IApprovalMatrixOptions ApprovalMatrix { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IAuditLogOptions"/>
        /// </summary>
        public IAuditLogOptions AuditLog { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="ICarOptions"/>
        /// </summary>
        public ICarOptions Car { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IClaimOptions"/>
        /// </summary>
        public IClaimOptions Claim { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="ICodeAllocationOptions"/>
        /// </summary>
        public ICodeAllocationOptions CodeAllocation { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IColourOptions"/>
        /// </summary>
        public IColourOptions Colour { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="ICompanyPolicyOptions"/>
        /// </summary>
        public ICompanyPolicyOptions CompanyPolicy { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IAddEditExpenseOptions"/>
        /// </summary>
        public IAddEditExpenseOptions AddEditExpense { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IHotelOptions"/>
        /// </summary>
        public IHotelOptions Hotel { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IReminderOptions"/>
        /// </summary>
        public IReminderOptions Reminders { get; set; }

        /// <summary>
        /// Gets or sets the language option
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
    }
}