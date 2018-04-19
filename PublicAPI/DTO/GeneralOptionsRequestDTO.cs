namespace PublicAPI.DTO
{
    using BusinessLogic.GeneralOptions;
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
    /// Defines a <see cref="GeneralOptionsRequestDTO"/> and all it's members
    /// </summary>
    public class GeneralOptionsRequestDTO
    {
        /// <summary>
        /// Defines whether to build the <see cref="IAddEditExpenseOptions"/>
        /// </summary>
        public bool AddEditExpense { get; set; }

        /// <summary>
        /// Defines whether to build the <see cref="IAccountMessagesOptions"/>
        /// </summary>
        public bool AccountMessages { get; set; }

        /// <summary>
        /// Defines whether to build the <see cref="IAddressOptions"/>
        /// </summary>
        public bool Addresses { get; set; }

        /// <summary>
        /// Defines whether to build the <see cref="IAdminOptions"/>
        /// </summary>
        public bool Admin { get; set; }

        /// <summary>
        /// Defines whether to build the <see cref="IApprovalMatrixOptions"/>
        /// </summary>
        public bool ApprovalMatrix { get; set; }

        /// <summary>
        /// Defines whether to build the <see cref="IAuditLogOptions"/>
        /// </summary>
        public bool AuditLog { get; set; }

        /// <summary>
        /// Defines whether to build the <see cref="ICarOptions"/>
        /// </summary>
        public bool Car { get; set; }

        /// <summary>
        /// Defines whether to build the <see cref="IClaimOptions"/>
        /// </summary>
        public bool Claim { get; set; }

        /// <summary>
        /// Defines whether to build the <see cref="ICodeAllocationOptions"/>
        /// </summary>
        public bool CodeAllocation { get; set; }

        /// <summary>
        /// Defines whether to build the <see cref="IColourOptions"/>
        /// </summary>
        public bool Colour { get; set; }

        /// <summary>
        /// Defines whether to build the <see cref="ICompanyPolicyOptions"/>
        /// </summary>
        public bool CompanyPolicy { get; set; }

        /// <summary>
        /// Defines whether to build the <see cref="ICountryOptions"/>
        /// </summary>
        public bool Country { get; set; }

        /// <summary>
        /// Defines whether to build the <see cref="ICurrencyOptions"/>
        /// </summary>
        public bool Currency { get; set; }

        /// <summary>
        /// Defines whether to build the <see cref="IDelegateOptions"/>
        /// </summary>
        public bool Delegate { get; set; }

        /// <summary>
        /// Defines whether to build the <see cref="IDutyOfCareOptions"/>
        /// </summary>
        public bool DutyOfCare { get; set; }

        /// <summary>
        /// Defines whether to build the <see cref="IEmailOptions"/>
        /// </summary>
        public bool Email { get; set; }

        /// <summary>
        /// Defines whether to build the <see cref="IEmployeeOptions"/>
        /// </summary>
        public bool Employee { get; set; }

        /// <summary>
        /// Defines whether to build the <see cref="IESROptions"/>
        /// </summary>
        public bool ESR { get; set; }

        /// <summary>
        /// Defines whether to build the <see cref="IExpediteOptions"/>
        /// </summary>
        public bool Expedite { get; set; }

        /// <summary>
        /// Defines whether to build the <see cref="IFlagOptions"/>
        /// </summary>
        public bool Flag { get; set; }

        /// <summary>
        /// Defines whether to build the <see cref="IAttachmentOptions"/>
        /// </summary>
        public bool Attachment { get; set; }

        /// <summary>
        /// Defines whether to build the <see cref="IContractOptions"/>
        /// </summary>
        public bool Contract { get; set; }

        /// <summary>
        /// Defines whether to build the <see cref="IInvoicesOptions"/>
        /// </summary>
        public bool Invoices { get; set; }

        /// <summary>
        /// Defines whether to build the <see cref="IRechargeOptions"/>
        /// </summary>
        public bool Recharge { get; set; }

        /// <summary>
        /// Defines whether to build the <see cref="IScheduleOptions"/>
        /// </summary>
        public bool Schedule { get; set; }

        /// <summary>
        /// Defines whether to build the <see cref="ISearchOptions"/>
        /// </summary>
        public bool Search { get; set; }

        /// <summary>
        /// Defines whether to build the <see cref="ISupplierOptions"/>
        /// </summary>
        public bool Supplier { get; set; }

        /// <summary>
        /// Defines whether to build the <see cref="ITaskOptions"/>
        /// </summary>
        public bool Task { get; set; }

        /// <summary>
        /// Defines whether to build the <see cref="IHelpAndSupportOptions"/>
        /// </summary>
        public bool HelpAndSupport { get; set; }

        /// <summary>
        /// Defines whether to build the <see cref="IMileageOptions"/>
        /// </summary>
        public bool Mileage { get; set; }

        /// <summary>
        /// Defines whether to build the <see cref="IMobileOptions"/>
        /// </summary>
        public bool Mobile { get; set; }

        /// <summary>
        /// Defines whether to build the <see cref="IMyDetailsOptions"/>
        /// </summary>
        public bool MyDetails { get; set; }

        /// <summary>
        /// Defines whether to build the <see cref="IPasswordOptions"/>
        /// </summary>
        public bool Password { get; set; }

        /// <summary>
        /// Defines whether to build the <see cref="IRegionalSettingsOptions"/>
        /// </summary>
        public bool RegionalSettings { get; set; }

        /// <summary>
        /// Defines whether to build the <see cref="IReportOptions"/>
        /// </summary>
        public bool Report { get; set; }

        /// <summary>
        /// Defines whether to build the <see cref="ISelfRegistrationOptions"/>
        /// </summary>
        public bool SelfRegistration { get; set; }

        /// <summary>
        /// Defines whether to build the <see cref="ISessionTimeoutOptions"/>
        /// </summary>
        public bool SessionTimeout { get; set; }

        /// <summary>
        /// Defines whether to build the <see cref="IValidateOptions"/>
        /// </summary>
        public bool Validate { get; set; }

        /// <summary>
        /// Defines whether to build the <see cref="IVatCalculationOptions"/>
        /// </summary>
        public bool VatCalculation { get; set; }

        /// <summary>
        /// Defines whether to build the <see cref="ICacheOptions"/>
        /// </summary>
        public bool Cache { get; set; }

        /// <summary>
        /// Defines whether to build the <see cref="ICorporateDiligenceOptions"/>
        /// </summary>
        public bool CorporateDiligence { get; set; }

        /// <summary>
        /// Defines whether to build the <see cref="IPageOptions"/>
        /// </summary>
        public bool Page { get; set; }

        /// <summary>
        /// Defines whether to build the <see cref="IFinancialYearOptions"/>
        /// </summary>
        public bool FinancialYear { get; set; }

        /// <summary>
        /// Defines whether to build the <see cref="IHotelOptions"/>
        /// </summary>
        public bool Hotel { get; set; }

        /// <summary>
        /// Defines whether to build the <see cref="IReminderOptions"/>
        /// </summary>
        public bool Reminders { get; set; }

        /// <summary>
        /// Defines whether to build the <see cref="INotesOptions"/>
        /// </summary>
        public bool Notes { get; set; }

        /// <summary>
        /// Generate a <see cref="IGeneralOptions"/> to return over the API
        /// </summary>
        /// <param name="generalOptions">The instance of a <see cref="IGeneralOptions"/> to build upon</param>
        /// <returns>The <see cref="IGeneralOptions"/> to return over the API call</returns>
        public IGeneralOptions Generate(IGeneralOptions generalOptions)
        {
            if (this.AddEditExpense)
            {
                generalOptions.WithAddEditExpense();
            }

            if (this.AccountMessages)
            {
                generalOptions.WithAccountMessages();
            }

            if (this.Addresses)
            {
                generalOptions.WithAddress();
            }

            if (this.Admin)
            {
                generalOptions.WithAdmin();
            }

            if (this.ApprovalMatrix)
            {
                generalOptions.WithApprovalMatrix();
            }

            if (this.AuditLog)
            {
                generalOptions.WithAuditLog();
            }

            if (this.Car)
            {
                generalOptions.WithClaim();
            }

            if (this.Claim)
            {
                generalOptions.WithClaim();
            }

            if (this.CodeAllocation)
            {
                generalOptions.WithCodeAllocation();
            }

            if (this.Colour)
            {
                generalOptions.WithColour();
            }

            if (this.CompanyPolicy)
            {
                generalOptions.WithCompanyPolicy();
            }

            if (this.Country)
            {
                generalOptions.WithCountry();
            }

            if (this.Currency)
            {
                generalOptions.WithCurrency();
            }

            if (this.Delegate)
            {
                generalOptions.WithDelegate();
            }

            if (this.DutyOfCare)
            {
                generalOptions.WithDutyOfCare();
            }

            if (this.Email)
            {
                generalOptions.WithEmail();
            }

            if (this.Employee)
            {
                generalOptions.WithEmployee();
            }

            if (this.ESR)
            {
                generalOptions.WithESR();
            }

            if (this.Expedite)
            {
                generalOptions.WithExpedite();
            }

            if (this.Flag)
            {
                generalOptions.WithFlag();
            }

            if (this.Attachment)
            {
                generalOptions.WithAttachments();
            }

            if (this.Contract)
            {
                generalOptions.WithContracts();
            }

            if (this.Invoices)
            {
                generalOptions.WithInvoices();
            }

            if (this.Recharge)
            {
                generalOptions.WithRecharge();
            }

            if (this.Schedule)
            {
                generalOptions.WithSchedule();
            }

            if (this.Search)
            {
                generalOptions.WithSearch();
            }

            if (this.Supplier)
            {
                generalOptions.WithSupplier();
            }

            if (this.Task)
            {
                generalOptions.WithTask();
            }

            if (this.HelpAndSupport)
            {
                generalOptions.WithHelpAndSupport();
            }

            if (this.Mileage)
            {
                generalOptions.WithMileage();
            }

            if (this.Mobile)
            {
                generalOptions.WithMobile();
            }

            if (this.MyDetails)
            {
                generalOptions.WithMyDetails();
            }

            if (this.Password)
            {
                generalOptions.WithPassword();
            }

            if (this.RegionalSettings)
            {
                generalOptions.WithRegionalSettings();
            }

            if (this.Report)
            {
                generalOptions.WithReport();
            }

            if (this.SelfRegistration)
            {
                generalOptions.WithSelfRegistration();
            }

            if (this.SessionTimeout)
            {
                generalOptions.WithSessionTimeout();
            }

            if (this.Validate)
            {
                generalOptions.WithValidate();
            }

            if (this.VatCalculation)
            {
                generalOptions.WithVatCalculation();
            }

            if (this.Cache)
            {
                generalOptions.WithCache();
            }

            if (this.CorporateDiligence)
            {
                generalOptions.WithCorporateDiligence();
            }

            if (this.Page)
            {
                generalOptions.WithPage();
            }

            if (this.FinancialYear)
            {
                generalOptions.WithFinancialYear();
            }

            if (this.Hotel)
            {
                generalOptions.WithHotel();
            }

            if (this.Reminders)
            {
                generalOptions.WithReminders();
            }

            if (this.Notes)
            {
                generalOptions.WithNotes();
            }

            return generalOptions;
        }
    }
}
