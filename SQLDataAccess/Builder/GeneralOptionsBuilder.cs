namespace SQLDataAccess.Builder
{
    using System;
    using System.Collections.Generic;
    using BusinessLogic;
    using BusinessLogic.AccountProperties;
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
    /// Defines a <see cref="GeneralOptionsBuilder"/> and it's members
    /// </summary>
    public class GeneralOptionsBuilder : GeneralOptions
    {
        private readonly IList<IAccountProperty> _accountProperties;

        /// <summary>
        /// Initializes a new instance of the <see cref="GeneralOptionsBuilder"/> class.
        /// </summary>
        /// <param name="accountProperties">The <see cref="IList{IAccountProperty}"/> use to build the <see cref="GeneralOptionsBuilder"/></param>
        public GeneralOptionsBuilder(IList<IAccountProperty> accountProperties)
        {
            Guard.ThrowIfNull(accountProperties, nameof(accountProperties));

            this._accountProperties = accountProperties;

            foreach (var accountProperty in this._accountProperties)
            {
                switch (accountProperty.AccountPropertyKey)
                {
                    case AccountPropertyKeys.ApplicationURL:
                        this.ApplicationURL = accountProperty.Value;
                        break;
                    case AccountPropertyKeys.Language:
                        this.Language = accountProperty.Value;
                        break;
                    default:
                        break;
                }
            }

            if (this._accountProperties.Count != 0)
            {
                this.SubAccountID = this._accountProperties[0].SubAccountId;
            }
        }

        /// <summary>
        /// Sets all options
        /// </summary>
        /// <returns>An instance of <see cref="IGeneralOptions"/></returns>
        public override IGeneralOptions WithAll()
        {
            this.WithAccountMessages();
            this.WithAddEditExpense();
            this.WithAddress();
            this.WithAdmin();
            this.WithApprovalMatrix();
            this.WithAuditLog();
            this.WithCar();
            this.WithClaim();
            this.WithCodeAllocation();
            this.WithColour();
            this.WithCompanyPolicy();
            this.WithCountry();
            this.WithCurrency();
            this.WithDelegate();
            this.WithDutyOfCare();
            this.WithEmail();
            this.WithEmployee();
            this.WithESR();
            this.WithExpedite();
            this.WithFlag();
            this.WithAttachments();
            this.WithContracts();
            this.WithInvoices();
            this.WithRecharge();
            this.WithSchedule();
            this.WithSearch();
            this.WithSupplier();
            this.WithTask();
            this.WithHelpAndSupport();
            this.WithMileage();
            this.WithMobile();
            this.WithMyDetails();
            this.WithPassword();
            this.WithRegionalSettings();
            this.WithReport();
            this.WithSelfRegistration();
            this.WithSessionTimeout();
            this.WithValidate();
            this.WithVatCalculation();
            this.WithNotes();
            this.WithPage();
            this.WithCorporateDiligence();
            this.WithCache();
            this.WithHotel();
            this.WithFinancialYear();
            this.WithReminders();

            return this;
        }

        /// <summary>
        /// Sets the <see cref="IAccountMessagesOptions"/>
        /// </summary>
        /// <returns>An instance of <see cref="IGeneralOptions"/></returns>
        public override IGeneralOptions WithAccountMessages()
        {
            if (this.AccountMessages != null) return this;

            this.AccountMessages = new AccountMessageOptions();

            foreach (var accountProperty in this._accountProperties)
            {
                switch (accountProperty.AccountPropertyKey)
                {
                    case AccountPropertyKeys.AccountCurrentlyLockedMessage:
                        this.AccountMessages.AccountCurrentlyLockedMessage = accountProperty.Value;
                        break;
                    case AccountPropertyKeys.AccountLockedMessage:
                        this.AccountMessages.AccountLockedMessage = accountProperty.Value;
                        break;
                    default:
                        break;
                }
            }

            return this;
        }

        /// <summary>
        /// Sets the <see cref="IAddEditExpenseOptions"/>
        /// </summary>
        /// <returns>An instance of <see cref="IGeneralOptions"/></returns>
        public override IGeneralOptions WithAddEditExpense()
        {
            if (this.AddEditExpense != null) return this;

            this.AddEditExpense = new AddEditExpenseOptions();

            foreach (var accountProperty in this._accountProperties)
            {
                switch (accountProperty.AccountPropertyKey)
                {
                    case AccountPropertyKeys.ExchangeReadOnly:
                        this.AddEditExpense.ExchangeReadOnly = Convert.ToBoolean(Convert.ToByte(accountProperty.Value));
                        break;
                    case AccountPropertyKeys.ClaimantsCanAddCompanyLocations:
                        this.AddEditExpense.ClaimantsCanAddCompanyLocations = Convert.ToBoolean(Convert.ToByte(accountProperty.Value));
                        break;
                    case AccountPropertyKeys.IncludeAssignmentDetails:
                        this.AddEditExpense.IncludeAssignmentDetails = (IncludeEsrDetails)Convert.ToByte(accountProperty.Value);
                        break;
                    case AccountPropertyKeys.DisableCarOutsideOfStartEndDate:
                        this.AddEditExpense.DisableCarOutsideOfStartEndDate = Convert.ToBoolean(Convert.ToByte(accountProperty.Value));
                        break;
                    case AccountPropertyKeys.HomeAddressKeyword:
                        this.AddEditExpense.HomeAddressKeyword = accountProperty.Value;
                        break;
                    case AccountPropertyKeys.WorkAddressKeyword:
                        this.AddEditExpense.WorkAddressKeyword = accountProperty.Value;
                        break;
                    case AccountPropertyKeys.ForceAddressNameEntry:
                        this.AddEditExpense.ForceAddressNameEntry = Convert.ToBoolean(Convert.ToByte(accountProperty.Value));
                        break;
                    case AccountPropertyKeys.AddressNameEntryMessage:
                        this.AddEditExpense.AddressNameEntryMessage = accountProperty.Value;
                        break;
                    case AccountPropertyKeys.DisplayEsrAddressesInSearchResults:
                        this.AddEditExpense.DisplayEsrAddressesInSearchResults = Convert.ToBoolean(Convert.ToByte(accountProperty.Value));
                        break;
                    case AccountPropertyKeys.MultipleWorkAddress:
                        this.AddEditExpense.MultipleWorkAddress =
                            Convert.ToBoolean(Convert.ToByte(accountProperty.Value));
                        break;
                    default:
                        break;
                }
            }

            return this;
        }

        /// <summary>
        /// Sets the <see cref="IAddressOptions"/>
        /// </summary>
        /// <returns>An instance of <see cref="IGeneralOptions"/></returns>
        public override IGeneralOptions WithAddress()
        {
            if (this.Addresses != null) return this;

            this.Addresses = new AddressOptions();

            foreach (var accountProperty in this._accountProperties)
            {
                switch (accountProperty.AccountPropertyKey)
                {
                    case AccountPropertyKeys.RetainLabelsTime:
                        this.Addresses.RetainLabelsTime = accountProperty.Value;
                        break;
                    default:
                        break;
                }
            }

            return this;
        }

        /// <summary>
        /// Sets the <see cref="IAdminOptions"/>
        /// </summary>
        /// <returns>An instance of <see cref="IGeneralOptions"/></returns>
        public override IGeneralOptions WithAdmin()
        {
            if (this.Admin != null) return this;

            this.Admin = new AdminOptions();

            foreach (var accountProperty in this._accountProperties)
            {
                switch (accountProperty.AccountPropertyKey)
                {
                    case AccountPropertyKeys.MainAdministrator:
                        this.Admin.MainAdministrator = Convert.ToInt32(accountProperty.Value);
                        break;
                    default:
                        break;
                }
            }

            return this;
        }

        /// <summary>
        /// Sets the <see cref="IApprovalMatrixOptions"/>
        /// </summary>
        /// <returns>An instance of <see cref="IGeneralOptions"/></returns>
        public override IGeneralOptions WithApprovalMatrix()
        {
            if (this.ApprovalMatrix != null) return this;

            this.ApprovalMatrix = new ApprovalMatrixOptions();

            foreach (var accountProperty in this._accountProperties)
            {
                switch (accountProperty.AccountPropertyKey)
                {
                    case AccountPropertyKeys.NumberOfApproversToRememberForClaimantInApprovalMatrixClaim:
                        this.ApprovalMatrix.NumberOfApproversToRememberForClaimantInApprovalMatrixClaim =
                            Convert.ToInt32(accountProperty.Value);
                        break;
                    default:
                        break;
                }
            }

            return this;
        }

        /// <summary>
        /// Sets the <see cref="IAuditLogOptions"/>
        /// </summary>
        /// <returns>An instance of <see cref="IGeneralOptions"/></returns>
        public override IGeneralOptions WithAuditLog()
        {
            if (this.AuditLog != null) return this;

            this.AuditLog = new AuditLogOptions();

            foreach (var accountProperty in this._accountProperties)
            {
                switch (accountProperty.AccountPropertyKey)
                {
                    case AccountPropertyKeys.AuditorEmailAddress:
                        this.AuditLog.AuditorEmailAddress = accountProperty.Value;
                        break;
                    default:
                        break;
                }
            }

            return this;
        }

        /// <summary>
        /// Sets the <see cref="ICarOptions"/>
        /// </summary>
        /// <returns>An instance of <see cref="IGeneralOptions"/></returns>
        public override IGeneralOptions WithCar()
        {
            if (this.Car != null) return this;

            this.Car = new CarOptions();

            foreach (var accountProperty in this._accountProperties)
            {
                switch (accountProperty.AccountPropertyKey)
                {
                    case AccountPropertyKeys.ShowMileageCatsForUsers:
                        this.Car.ShowMileageCatsForUsers = Convert.ToBoolean(Convert.ToByte(accountProperty.Value));
                        break;
                    case AccountPropertyKeys.ActivateCarOnUserAdd:
                        this.Car.ActivateCarOnUserAdd = Convert.ToBoolean(Convert.ToByte(accountProperty.Value));
                        break;
                    case AccountPropertyKeys.AllowUsersToAddCars:
                        this.Car.AllowUsersToAddCars = Convert.ToBoolean(Convert.ToByte(accountProperty.Value));
                        break;
                    case AccountPropertyKeys.AllowEmpToSpecifyCarStartDateOnAdd:
                        this.Car.AllowEmpToSpecifyCarStartDateOnAdd = Convert.ToBoolean(Convert.ToByte(accountProperty.Value));
                        break;
                    case AccountPropertyKeys.AllowEmpToSpecifyCarDOCOnAdd:
                        this.Car.AllowEmpToSpecifyCarDOCOnAdd = Convert.ToBoolean(Convert.ToByte(accountProperty.Value));
                        break;
                    case AccountPropertyKeys.EmpToSpecifyCarStartDateOnAddMandatory:
                        this.Car.EmpToSpecifyCarStartDateOnAddMandatory = Convert.ToBoolean(Convert.ToByte(accountProperty.Value));
                        break;
                    case AccountPropertyKeys.PopulateDocumentsFromVehicleLookup:
                        this.Car.PopulateDocumentsFromVehicleLookup = Convert.ToBoolean(Convert.ToInt32(accountProperty.Value));
                        break;
                    case AccountPropertyKeys.DrivingLicenceReviewReminderDays:
                        this.Car.DrivingLicenceReviewReminderDays = Convert.ToByte(accountProperty.Value);
                        break;
                    default:
                        break;
                }
            }

            return this;
        }

        /// <summary>
        /// Sets the <see cref="IClaimOptions"/>
        /// </summary>
        /// <returns>An instance of <see cref="IGeneralOptions"/></returns>
        public override IGeneralOptions WithClaim()
        {

            if (this.Claim != null) return this;

            this.Claim = new ClaimOptions();

            foreach (var accountProperty in this._accountProperties)
            {
                switch (accountProperty.AccountPropertyKey)
                {
                    case AccountPropertyKeys.SingleClaim:
                        this.Claim.SingleClaim = Convert.ToBoolean(Convert.ToByte(accountProperty.Value));
                        break;
                    case AccountPropertyKeys.AttachReceipts:
                        this.Claim.AttachReceipts = Convert.ToBoolean(Convert.ToByte(accountProperty.Value));
                        break;
                    case AccountPropertyKeys.PreApproval:
                        this.Claim.PreApproval = Convert.ToBoolean(Convert.ToByte(accountProperty.Value));
                        break;
                    case AccountPropertyKeys.PartSubmit:
                        this.Claim.PartSubmit = Convert.ToBoolean(Convert.ToByte(accountProperty.Value));
                        break;
                    case AccountPropertyKeys.OnlyCashCredit:
                        this.Claim.OnlyCashCredit = Convert.ToBoolean(Convert.ToByte(accountProperty.Value));
                        break;
                    case AccountPropertyKeys.FrequencyType:
                        this.Claim.FrequencyType = accountProperty.Value == string.Empty ? (byte)0 : Convert.ToByte(accountProperty.Value);
                        break;
                    case AccountPropertyKeys.FrequencyValue:
                        this.Claim.FrequencyValue = accountProperty.Value == string.Empty ? 0 : Convert.ToInt32(accountProperty.Value);
                        break;
                    case AccountPropertyKeys.ClaimantDeclaration:
                        this.Claim.ClaimantDeclaration = Convert.ToBoolean(Convert.ToByte(accountProperty.Value));
                        break;
                    case AccountPropertyKeys.DeclarationMsg:
                        this.Claim.DeclarationMsg = accountProperty.Value;
                        break;
                    case AccountPropertyKeys.ApproverDeclarationMsg:
                        this.Claim.ApproverDeclarationMsg = accountProperty.Value;
                        break;
                    case AccountPropertyKeys.AllowTeamMemberToApproveOwnClaim:
                        this.Claim.AllowTeamMemberToApproveOwnClaim = Convert.ToBoolean(Convert.ToByte(accountProperty.Value));
                        break;
                    case AccountPropertyKeys.AllowEmployeeInOwnSignoffGroup:
                        this.Claim.AllowEmployeeInOwnSignoffGroup = Convert.ToBoolean(Convert.ToByte(accountProperty.Value));
                        break;
                    case AccountPropertyKeys.ShowFullHomeAddressOnClaims:
                        this.Claim.ShowFullHomeAddressOnClaims = Convert.ToBoolean(Convert.ToByte(accountProperty.Value));
                        break;
                    case AccountPropertyKeys.EditPreviousClaims:
                        this.Claim.EditPreviousClaims = Convert.ToBoolean(Convert.ToByte(accountProperty.Value));
                        break;
                    case AccountPropertyKeys.BlockUnmatchedExpenseItemsBeingSubmitted:
                        this.Claim.BlockUnmatchedExpenseItemsBeingSubmitted = Convert.ToBoolean(Convert.ToInt32(accountProperty.Value));
                        break;
                    default:
                        break;
                }
            }

            return this;
        }

        /// <summary>
        /// Sets the <see cref="ICodeAllocationOptions"/>
        /// </summary>
        /// <returns>An instance of <see cref="IGeneralOptions"/></returns>
        public override IGeneralOptions WithCodeAllocation()
        {
            if (this.CodeAllocation != null) return this;

            this.CodeAllocation = new CodeAllocationOptions();

            foreach (var accountProperty in this._accountProperties)
            {
                switch (accountProperty.AccountPropertyKey)
                {
                    case AccountPropertyKeys.UseCostCodes:
                        this.CodeAllocation.UseCostCodes = Convert.ToBoolean(Convert.ToByte(accountProperty.Value));
                        break;
                    case AccountPropertyKeys.UseDepartmentCodes:
                        this.CodeAllocation.UseDepartmentCodes = Convert.ToBoolean(Convert.ToByte(accountProperty.Value));
                        break;
                    case AccountPropertyKeys.UseCostCodeDescription:
                        this.CodeAllocation.UseCostCodeDescription = Convert.ToBoolean(Convert.ToByte(accountProperty.Value));
                        break;
                    case AccountPropertyKeys.UseDepartmentDescription:
                        this.CodeAllocation.UseDepartmentDescription = Convert.ToBoolean(Convert.ToByte(accountProperty.Value));
                        break;
                    case AccountPropertyKeys.UseProjectCodes:
                        this.CodeAllocation.UseProjectCodes = Convert.ToBoolean(Convert.ToByte(accountProperty.Value));
                        break;
                    case AccountPropertyKeys.UseProjectCodeDesc:
                        this.CodeAllocation.UseProjectCodeDesc = Convert.ToBoolean(Convert.ToByte(accountProperty.Value));
                        break;
                    case AccountPropertyKeys.CostCodesOn:
                        this.CodeAllocation.CostCodesOn = Convert.ToBoolean(Convert.ToByte(accountProperty.Value));
                        break;
                    case AccountPropertyKeys.DepartmentsOn:
                        this.CodeAllocation.DepartmentsOn = Convert.ToBoolean(Convert.ToByte(accountProperty.Value));
                        break;
                    case AccountPropertyKeys.ProjectCodesOn:
                        this.CodeAllocation.ProjectCodesOn = Convert.ToBoolean(Convert.ToByte(accountProperty.Value));
                        break;
                    case AccountPropertyKeys.AutoAssignAllocation:
                        this.CodeAllocation.AutoAssignAllocation = Convert.ToBoolean(Convert.ToByte(accountProperty.Value));
                        break;
                    case AccountPropertyKeys.UseCostCodeOnGenDetails:
                        this.CodeAllocation.UseCostCodeOnGenDetails = Convert.ToBoolean(Convert.ToByte(accountProperty.Value));
                        break;
                    case AccountPropertyKeys.UseDeptOnGenDetails:
                        this.CodeAllocation.UseDeptOnGenDetails = Convert.ToBoolean(Convert.ToByte(accountProperty.Value));
                        break;
                    case AccountPropertyKeys.UseProjectCodeOnGenDetails:
                        this.CodeAllocation.UseProjectCodeOnGenDetails = Convert.ToBoolean(Convert.ToByte(accountProperty.Value));
                        break;
                    case AccountPropertyKeys.CostCodeOwnerBaseKey:
                        this.CodeAllocation.CostCodeOwnerBaseKey = accountProperty.Value;
                        break;
                    default:
                        break;
                }
            }

            return this;
        }

        /// <summary>
        /// Sets the <see cref="IColourOptions"/>
        /// </summary>
        /// <returns>An instance of <see cref="IGeneralOptions"/></returns>
        public override IGeneralOptions WithColour()
        {
            if (this.Colour != null) return this;

            this.Colour = new ColourOptions();

            foreach (var accountProperty in this._accountProperties)
            {
                switch (accountProperty.AccountPropertyKey)
                {
                    case AccountPropertyKeys.ColoursHeaderBackground:
                        this.Colour.ColoursHeaderBackground = accountProperty.Value;
                        break;
                    case AccountPropertyKeys.ColoursHeaderBreadcrumbText:
                        this.Colour.ColoursHeaderBreadcrumbText = accountProperty.Value;
                        break;
                    case AccountPropertyKeys.ColoursPageTitleText:
                        this.Colour.ColoursPageTitleText = accountProperty.Value;
                        break;
                    case AccountPropertyKeys.ColoursSectionHeadingUnderline:
                        this.Colour.ColoursSectionHeadingUnderline = accountProperty.Value;
                        break;
                    case AccountPropertyKeys.ColoursSectionHeadingText:
                        this.Colour.ColoursSectionHeadingText = accountProperty.Value;
                        break;
                    case AccountPropertyKeys.ColoursFieldText:
                        this.Colour.ColoursFieldText = accountProperty.Value;
                        break;
                    case AccountPropertyKeys.ColoursPageOptionsBackground:
                        this.Colour.ColoursPageOptionsBackground = accountProperty.Value;
                        break;
                    case AccountPropertyKeys.ColoursPageOptionsText:
                        this.Colour.ColoursPageOptionsText = accountProperty.Value;
                        break;
                    case AccountPropertyKeys.ColoursTableHeaderBackground:
                        this.Colour.ColoursTableHeaderBackground = accountProperty.Value;
                        break;
                    case AccountPropertyKeys.ColoursTableHeaderText:
                        this.Colour.ColoursTableHeaderText = accountProperty.Value;
                        break;
                    case AccountPropertyKeys.ColoursTabOptionBackground:
                        this.Colour.ColoursTabOptionBackground = accountProperty.Value;
                        break;
                    case AccountPropertyKeys.ColoursTabOptionText:
                        this.Colour.ColoursTabOptionText = accountProperty.Value;
                        break;
                    case AccountPropertyKeys.ColoursRowBackground:
                        this.Colour.ColoursRowBackground = accountProperty.Value;
                        break;
                    case AccountPropertyKeys.ColoursRowText:
                        this.Colour.ColoursRowText = accountProperty.Value;
                        break;
                    case AccountPropertyKeys.ColoursAlternateRowBackground:
                        this.Colour.ColoursAlternateRowBackground = accountProperty.Value;
                        break;
                    case AccountPropertyKeys.ColoursAlternateRowText:
                        this.Colour.ColoursAlternateRowText = accountProperty.Value;
                        break;
                    case AccountPropertyKeys.ColoursMenuOptionHoverText:
                        this.Colour.ColoursMenuOptionHoverText = accountProperty.Value;
                        break;
                    case AccountPropertyKeys.ColoursMenuOptionStandardText:
                        this.Colour.ColoursMenuOptionStandardText = accountProperty.Value;
                        break;
                    case AccountPropertyKeys.ColoursTooltipBackground:
                        this.Colour.ColoursTooltipBackground = accountProperty.Value;
                        break;
                    case AccountPropertyKeys.ColoursTooltipText:
                        this.Colour.ColoursTooltipText = accountProperty.Value;
                        break;
                    case AccountPropertyKeys.ColoursGreenLightField:
                        this.Colour.ColoursGreenLightField = accountProperty.Value;
                        break;
                    case AccountPropertyKeys.ColoursGreenLightSectionText:
                        this.Colour.ColoursGreenLightSectionText = accountProperty.Value;
                        break;
                    case AccountPropertyKeys.ColoursGreenLightSectionBackground:
                        this.Colour.ColoursGreenLightSectionBackground = accountProperty.Value;
                        break;
                    case AccountPropertyKeys.ColoursGreenLightSectionUnderline:
                        this.Colour.ColoursGreenLightSectionUnderline = accountProperty.Value;
                        break;
                    default:
                        break;
                }
            }

            return this;
        }

        /// <summary>
        /// Sets the <see cref="ICompanyPolicyOptions"/>
        /// </summary>
        /// <returns>An instance of <see cref="IGeneralOptions"/></returns>
        public override IGeneralOptions WithCompanyPolicy()
        {
            if (this.CompanyPolicy != null) return this;

            this.CompanyPolicy = new CompanyPolicyOptions();

            foreach (var accountProperty in this._accountProperties)
            {
                switch (accountProperty.AccountPropertyKey)
                {
                    case AccountPropertyKeys.CompanyPolicy:
                        this.CompanyPolicy.CompanyPolicy = accountProperty.Value;
                        break;
                    case AccountPropertyKeys.PolicyType:
                        this.CompanyPolicy.PolicyType = Convert.ToByte(accountProperty.Value);
                        break;
                    default:
                        break;
                }
            }

            return this;
        }

        /// <summary>
        /// Sets the <see cref="ICountryOptions"/>
        /// </summary>
        /// <returns>An instance of <see cref="IGeneralOptions"/></returns>
        public override IGeneralOptions WithCountry()
        {
            if (this.Country != null) return this;

            this.Country = new CountryOptions();

            foreach (var accountProperty in this._accountProperties)
            {
                switch (accountProperty.AccountPropertyKey)
                {
                    case AccountPropertyKeys.HomeCountry:
                        this.Country.HomeCountry = Convert.ToInt32(accountProperty.Value);
                        break;
                    default:
                        break;
                }
            }

            return this;
        }

        /// <summary>
        /// Sets the <see cref="ICurrencyOptions"/>
        /// </summary>
        /// <returns>An instance of <see cref="IGeneralOptions"/></returns>
        public override IGeneralOptions WithCurrency()
        {
            if (this.Currency != null) return this;

            this.Currency = new CurrencyOptions();

            foreach (var accountProperty in this._accountProperties)
            {
                switch (accountProperty.AccountPropertyKey)
                {
                    case AccountPropertyKeys.CurrencyType:
                        this.Currency.CurrencyType = (CurrencyType)Convert.ToInt32(accountProperty.Value);
                        break;
                    case AccountPropertyKeys.BaseCurrency:
                        if (accountProperty.Value == string.Empty)
                        {
                            this.Currency.BaseCurrency = null;
                        }
                        else
                        {
                            this.Currency.BaseCurrency = Convert.ToInt32(accountProperty.Value);
                        }
                        break;
                    case AccountPropertyKeys.EnableAutoUpdateOfExchangeRates:
                        this.Currency.EnableAutoUpdateOfExchangeRates = Convert.ToBoolean(Convert.ToByte(accountProperty.Value));
                        break;
                    case AccountPropertyKeys.EnableAutoUpdateOfExchangeRatesActivatedDate:
                        if (accountProperty.Value == string.Empty)
                        {
                            this.Currency.EnableAutoUpdateOfExchangeRatesActivatedDate = null;
                        }
                        else
                        {
                            try
                            {
                                this.Currency.EnableAutoUpdateOfExchangeRatesActivatedDate = Convert.ToDateTime(accountProperty.Value);
                            }
                            catch (Exception)
                            {
                                this.Currency.EnableAutoUpdateOfExchangeRatesActivatedDate = null;
                            }
                        }
                        break;
                    case AccountPropertyKeys.ExchangeRateProvider:
                        this.Currency.ExchangeRateProvider = (ExchangeRateProvider)Convert.ToInt32(accountProperty.Value);
                        break;
                    default:
                        break;
                }
            }

            return this;
        }

        /// <summary>
        /// Sets the <see cref="IDelegateOptions"/>
        /// </summary>
        /// <returns>An instance of <see cref="IGeneralOptions"/></returns>
        public override IGeneralOptions WithDelegate()
        {
            if (this.Delegate != null) return this;

            this.Delegate = new DelegateOptions();

            foreach (var accountProperty in this._accountProperties)
            {
                switch (accountProperty.AccountPropertyKey)
                {
                    case AccountPropertyKeys.DelSetup:
                        this.Delegate.DelSetup = Convert.ToBoolean(Convert.ToByte(accountProperty.Value));
                        break;
                    case AccountPropertyKeys.DelEmployeeAccounts:
                        this.Delegate.DelEmployeeAccounts = Convert.ToBoolean(Convert.ToByte(accountProperty.Value));
                        break;
                    case AccountPropertyKeys.DelEmployeeAdmin:
                        this.Delegate.DelEmployeeAdmin = Convert.ToBoolean(Convert.ToByte(accountProperty.Value));
                        break;
                    case AccountPropertyKeys.DelReports:
                        this.Delegate.DelReports = Convert.ToBoolean(Convert.ToByte(accountProperty.Value));
                        break;
                    case AccountPropertyKeys.DelReportsClaimants:
                        this.Delegate.DelReportsClaimants = Convert.ToBoolean(Convert.ToByte(accountProperty.Value));
                        break;
                    case AccountPropertyKeys.DelCheckAndPay:
                        this.Delegate.DelCheckAndPay = Convert.ToBoolean(Convert.ToByte(accountProperty.Value));
                        break;
                    case AccountPropertyKeys.DelQEDesign:
                        this.Delegate.DelQEDesign = Convert.ToBoolean(Convert.ToByte(accountProperty.Value));
                        break;
                    case AccountPropertyKeys.DelCorporateCards:
                        this.Delegate.DelCorporateCards = Convert.ToBoolean(Convert.ToByte(accountProperty.Value));
                        break;
                    case AccountPropertyKeys.DelApprovals:
                        this.Delegate.DelApprovals = Convert.ToBoolean(Convert.ToByte(accountProperty.Value));
                        break;
                    case AccountPropertyKeys.DelSubmitClaim:
                        this.Delegate.DelSubmitClaim = Convert.ToBoolean(Convert.ToByte(accountProperty.Value));
                        break;
                    case AccountPropertyKeys.DelExports:
                        this.Delegate.DelExports = Convert.ToBoolean(Convert.ToByte(accountProperty.Value));
                        break;
                    case AccountPropertyKeys.DelAuditLog:
                        this.Delegate.DelAuditLog = Convert.ToBoolean(Convert.ToByte(accountProperty.Value));
                        break;
                    case AccountPropertyKeys.EnableDelegateOptionsForDelegateAccessRole:
                        this.Delegate.EnableDelegateOptionsForDelegateAccessRole = Convert.ToBoolean(Convert.ToByte(accountProperty.Value));
                        break;
                    default:
                        break;
                }
            }

            return this;
        }

        /// <summary>
        /// Sets the <see cref="IDutyOfCareOptions"/>
        /// </summary>
        /// <returns>An instance of <see cref="IGeneralOptions"/></returns>
        public override IGeneralOptions WithDutyOfCare()
        {
            if (this.DutyOfCare != null) return this;

            this.DutyOfCare = new DutyOfCareOptions();

            foreach (var accountProperty in this._accountProperties)
            {
                switch (accountProperty.AccountPropertyKey)
                {
                    case AccountPropertyKeys.EnableAutomaticDrivingLicenceLookup:
                        this.DutyOfCare.EnableAutomaticDrivingLicenceLookup = Convert.ToBoolean(Convert.ToByte(accountProperty.Value));
                        break;
                    case AccountPropertyKeys.FrequencyOfConsentRemindersLookup:
                        this.DutyOfCare.FrequencyOfConsentRemindersLookup = accountProperty.Value;
                        break;
                    case AccountPropertyKeys.DrivingLicenceLookupFrequency:
                        this.DutyOfCare.DrivingLicenceLookupFrequency = accountProperty.Value;
                        break;
                    case AccountPropertyKeys.BlockDrivingLicence:
                        this.DutyOfCare.BlockDrivingLicence = Convert.ToBoolean(Convert.ToByte(accountProperty.Value));
                        break;
                    case AccountPropertyKeys.BlockTaxExpiry:
                        this.DutyOfCare.BlockTaxExpiry = Convert.ToBoolean(Convert.ToByte(accountProperty.Value));
                        break;
                    case AccountPropertyKeys.BlockMOTExpiry:
                        this.DutyOfCare.BlockMOTExpiry = Convert.ToBoolean(Convert.ToByte(accountProperty.Value));
                        break;
                    case AccountPropertyKeys.BlockInsuranceExpiry:
                        this.DutyOfCare.BlockInsuranceExpiry = Convert.ToBoolean(Convert.ToByte(accountProperty.Value));
                        break;
                    case AccountPropertyKeys.BlockBreakdownCoverExpiry:
                        this.DutyOfCare.BlockBreakdownCoverExpiry = Convert.ToBoolean(Convert.ToByte(accountProperty.Value));
                        break;
                    case AccountPropertyKeys.UseDateOfExpenseForDutyOfCareChecks:
                        this.DutyOfCare.UseDateOfExpenseForDutyOfCareChecks = Convert.ToBoolean(Convert.ToByte(accountProperty.Value));
                        break;
                    case AccountPropertyKeys.RemindClaimantOnDOCDocumentExpiryDays:
                        this.DutyOfCare.RemindClaimantOnDOCDocumentExpiryDays = Convert.ToInt32(accountProperty.Value);
                        break;
                    case AccountPropertyKeys.RemindApproverOnDOCDocumentExpiryDays:
                        this.DutyOfCare.RemindApproverOnDOCDocumentExpiryDays = Convert.ToInt32(accountProperty.Value);
                        break;
                    case AccountPropertyKeys.DutyOfCareTeamAsApprover:
                        this.DutyOfCare.DutyOfCareTeamAsApprover = accountProperty.Value;
                        break;
                    case AccountPropertyKeys.DutyOfCareApprover:
                        this.DutyOfCare.DutyOfCareApprover = accountProperty.Value;
                        break;
                    case AccountPropertyKeys.EnableDrivingLicenceReview:
                        this.DutyOfCare.EnableDrivingLicenceReview = Convert.ToBoolean(Convert.ToByte(accountProperty.Value));
                        break;
                    case AccountPropertyKeys.DrivingLicenceReviewFrequency:
                        this.DutyOfCare.DrivingLicenceReviewFrequency = Convert.ToInt32(accountProperty.Value);
                        break;
                    case AccountPropertyKeys.DrivingLicenceReviewReminder:
                        this.DutyOfCare.DrivingLicenceReviewReminder = Convert.ToBoolean(Convert.ToByte(accountProperty.Value));
                        break;
                    case AccountPropertyKeys.DrivingLicenceReviewReminderDays:
                        this.DutyOfCare.DrivingLicenceReviewReminderDays = Convert.ToByte(accountProperty.Value);
                        break;
                    default:
                        break;
                }
            }

            return this;
        }

        /// <summary>
        /// Sets the <see cref="IEmailOptions"/>
        /// </summary>
        /// <returns>An instance of <see cref="IGeneralOptions"/></returns>
        public override IGeneralOptions WithEmail()
        {
            if (this.Email != null) return this;

            this.Email = new EmailOptions();

            foreach (var accountProperty in this._accountProperties)
            {
                switch (accountProperty.AccountPropertyKey)
                {
                    case AccountPropertyKeys.EmailServerAddress:
                        this.Email.EmailServerAddress = accountProperty.Value;
                        break;
                    case AccountPropertyKeys.EmailServerFromAddress:
                        this.Email.EmailServerFromAddress = accountProperty.Value;
                        break;
                    case AccountPropertyKeys.SourceAddress:
                        this.Email.SourceAddress = Convert.ToByte(accountProperty.Value);
                        break;
                    case AccountPropertyKeys.ErrorEmailAddress:
                        this.Email.ErrorEmailAddress = accountProperty.Value;
                        break;
                    case AccountPropertyKeys.ErrorEmailFromAddress:
                        this.Email.ErrorEmailFromAddress = accountProperty.Value;
                        break;
                    case AccountPropertyKeys.EmailAdministrator:
                        this.Email.EmailAdministrator = accountProperty.Value;
                        break;
                    default:
                        break;
                }
            }

            return this;
        }

        /// <summary>
        /// Sets the <see cref="IEmployeeOptions"/>
        /// </summary>
        /// <returns>An instance of <see cref="IGeneralOptions"/></returns>
        public override IGeneralOptions WithEmployee()
        {
            if (this.Employee != null) return this;

            this.Employee = new EmployeeOptions();

            foreach (var accountProperty in this._accountProperties)
            {
                switch (accountProperty.AccountPropertyKey)
                {
                    case AccountPropertyKeys.SearchEmployees:
                        this.Employee.SearchEmployees = Convert.ToBoolean(Convert.ToByte(accountProperty.Value));
                        break;
                    default:
                        break;
                }
            }

            return this;
        }

        /// <summary>
        /// Sets the <see cref="IESROptions"/>
        /// </summary>
        /// <returns>An instance of <see cref="IGeneralOptions"/></returns>
        public override IGeneralOptions WithESR()
        {
            if (this.ESR != null) return this;

            this.ESR = new ESROptions();

            foreach (var accountProperty in this._accountProperties)
            {
                switch (accountProperty.AccountPropertyKey)
                {
                    case AccountPropertyKeys.AutoArchiveType:
                        this.ESR.AutoArchiveType = (AutoArchiveType)Convert.ToByte(accountProperty.Value);
                        break;
                    case AccountPropertyKeys.AutoActivateType:
                        this.ESR.AutoActivateType = (AutoActivateType)Convert.ToByte(accountProperty.Value);
                        break;
                    case AccountPropertyKeys.ArchiveGracePeriod:
                        this.ESR.ArchiveGracePeriod = Convert.ToInt16(accountProperty.Value);
                        break;
                    case AccountPropertyKeys.ImportUsernameFormat:
                        this.ESR.ImportUsernameFormat = accountProperty.Value;
                        break;
                    case AccountPropertyKeys.ImportHomeAddressFormat:
                        this.ESR.ImportHomeAddressFormat = accountProperty.Value;
                        break;
                    case AccountPropertyKeys.CheckESRAssignmentOnEmployeeAdd:
                        this.ESR.CheckESRAssignmentOnEmployeeAdd = Convert.ToBoolean(Convert.ToByte(accountProperty.Value));
                        break;
                    case AccountPropertyKeys.EnableEsrDiagnostics:
                        this.ESR.EnableEsrDiagnostics = Convert.ToBoolean(Convert.ToByte(accountProperty.Value));
                        break;
                    case AccountPropertyKeys.EsrAutoActivateCar:
                        this.ESR.EsrAutoActivateCar = Convert.ToBoolean(Convert.ToByte(accountProperty.Value));
                        break;
                    case AccountPropertyKeys.SummaryEsrInboundFile:
                        this.ESR.SummaryEsrInboundFile = Convert.ToBoolean(Convert.ToByte(accountProperty.Value));
                        break;
                    case AccountPropertyKeys.EsrRounding:
                        this.ESR.EsrRounding = (EsrRoundingType)Convert.ToByte(accountProperty.Value);
                        break;
                    case AccountPropertyKeys.EnableESRManualAssignmentSupervisor:
                        this.ESR.EnableESRManualAssignmentSupervisor = Convert.ToBoolean(Convert.ToByte(accountProperty.Value));
                        break;
                    case AccountPropertyKeys.EsrPrimaryAddressOnly:
                        this.ESR.EsrPrimaryAddressOnly = Convert.ToBoolean(Convert.ToInt32(accountProperty.Value));
                        break;
                    default:
                        break;
                }
            }

            return this;
        }

        /// <summary>
        /// Sets the <see cref="IExpediteOptions"/>
        /// </summary>
        /// <returns>An instance of <see cref="IGeneralOptions"/></returns>
        public override IGeneralOptions WithExpedite()
        {
            if (this.Expedite != null) return this;

            this.Expedite = new ExpediteOptions();

            foreach (var accountProperty in this._accountProperties)
            {
                switch (accountProperty.AccountPropertyKey)
                {
                    case AccountPropertyKeys.AllowViewFundDetails:
                        this.Expedite.AllowViewFundDetails = Convert.ToBoolean(Convert.ToByte(accountProperty.Value));
                        break;
                    case AccountPropertyKeys.AllowReceiptTotalToPassValidation:
                        this.Expedite.AllowReceiptTotalToPassValidation = Convert.ToBoolean(Convert.ToByte(accountProperty.Value));
                        break;
                    default:
                        break;
                }
            }

            return this;
        }

        /// <summary>
        /// Sets the <see cref="IFlagOptions"/>
        /// </summary>
        /// <returns>An instance of <see cref="IGeneralOptions"/></returns>
        public override IGeneralOptions WithFlag()
        {
            if (this.Flag != null) return this;

            this.Flag = new FlagOptions();

            foreach (var accountProperty in this._accountProperties)
            {
                switch (accountProperty.AccountPropertyKey)
                {
                    case AccountPropertyKeys.FlagMessage:
                        this.Flag.FlagMessage = accountProperty.Value;
                        break;
                    default:
                        break;
                }
            }

            return this;
        }

        /// <summary>
        /// Sets the <see cref="IAttachmentOptions"/>
        /// </summary>
        /// <returns>An instance of <see cref="IGeneralOptions"/></returns>
        public override IGeneralOptions WithAttachments()
        {
            if (this.Attachment != null) return this;

            this.Attachment = new AttachmentOptions();

            foreach (var accountProperty in this._accountProperties)
            {
                switch (accountProperty.AccountPropertyKey)
                {
                    case AccountPropertyKeys.DocumentRepository:
                        this.Attachment.DocumentRepository = accountProperty.Value;
                        break;
                    case AccountPropertyKeys.OpenSaveAttachments:
                        this.Attachment.OpenSaveAttachments = Convert.ToByte(accountProperty.Value);
                        break;
                    case AccountPropertyKeys.EnableAttachmentHyperlink:
                        this.Attachment.EnableAttachmentHyperlink = Convert.ToBoolean(Convert.ToByte(accountProperty.Value));
                        break;
                    case AccountPropertyKeys.EnableAttachmentUpload:
                        this.Attachment.EnableAttachmentUpload = Convert.ToBoolean(Convert.ToByte(accountProperty.Value));
                        break;
                    case AccountPropertyKeys.LinkAttachmentDefault:
                        this.Attachment.LinkAttachmentDefault = Convert.ToByte(accountProperty.Value);
                        break;
                    case AccountPropertyKeys.MaxUploadSize:
                        this.Attachment.MaxUploadSize = Convert.ToInt32(accountProperty.Value);
                        break;
                    default:
                        break;
                }
            }

            return this;
        }

        /// <summary>
        /// Sets the <see cref="IContractOptions"/>
        /// </summary>
        /// <returns>An instance of <see cref="IGeneralOptions"/></returns>
        public override IGeneralOptions WithContracts()
        {
            if (this.Contract != null) return this;

            this.Contract = new ContractOptions();

            foreach (var accountProperty in this._accountProperties)
            {
                switch (accountProperty.AccountPropertyKey)
                {
                    case AccountPropertyKeys.AllowMenuContractAdd:
                        this.Contract.AllowMenuContractAdd = Convert.ToBoolean(Convert.ToByte(accountProperty.Value));
                        break;
                    case AccountPropertyKeys.EnableFlashingNotesIcon:
                        this.Contract.EnableFlashingNotesIcon = Convert.ToBoolean(Convert.ToByte(accountProperty.Value));
                        break;
                    case AccountPropertyKeys.EnableContractNumUpdate:
                        this.Contract.EnableContractNumUpdate = Convert.ToBoolean(Convert.ToByte(accountProperty.Value));
                        break;
                    case AccountPropertyKeys.AutoUpdateAnnualContractValue:
                        this.Contract.AutoUpdateAnnualContractValue = Convert.ToBoolean(Convert.ToByte(accountProperty.Value));
                        break;
                    case AccountPropertyKeys.ContractKey:
                        this.Contract.ContractKey = accountProperty.Value;
                        break;
                    case AccountPropertyKeys.RechargeUnrecoveredTitle:
                        this.Contract.RechargeUnrecoveredTitle = accountProperty.Value;
                        break;
                    case AccountPropertyKeys.AutoUpdateCVRechargeLive:
                        this.Contract.AutoUpdateCVRechargeLive = Convert.ToBoolean(Convert.ToByte(accountProperty.Value));
                        break;
                    case AccountPropertyKeys.PenaltyClauseTitle:
                        this.Contract.PenaltyClauseTitle = accountProperty.Value;
                        break;
                    case AccountPropertyKeys.ContractScheduleDefault:
                        this.Contract.ContractScheduleDefault = accountProperty.Value;
                        break;
                    case AccountPropertyKeys.UseCPExtraInfo:
                        this.Contract.UseCPExtraInfo = Convert.ToBoolean(Convert.ToByte(accountProperty.Value));
                        break;
                    case AccountPropertyKeys.EnableRecharge:
                        this.Contract.EnableRecharge = Convert.ToBoolean(Convert.ToByte(accountProperty.Value));
                        break;
                    case AccountPropertyKeys.ContractDatesMandatory:
                        this.Contract.ContractDatesMandatory = Convert.ToBoolean(Convert.ToByte(accountProperty.Value));
                        break;
                    case AccountPropertyKeys.AutoUpdateLicenceTotal:
                        this.Contract.AutoUpdateLicenceTotal = Convert.ToBoolean(Convert.ToByte(accountProperty.Value));
                        break;
                    case AccountPropertyKeys.ContractCategoryTitle:
                        this.Contract.ContractCategoryTitle = accountProperty.Value;
                        break;
                    case AccountPropertyKeys.InflatorActive:
                        this.Contract.InflatorActive = Convert.ToBoolean(Convert.ToByte(accountProperty.Value));
                        break;
                    case AccountPropertyKeys.InvoiceFreqActive:
                        this.Contract.InvoiceFreqActive = Convert.ToBoolean(Convert.ToByte(accountProperty.Value));
                        break;
                    case AccountPropertyKeys.TermTypeActive:
                        this.Contract.TermTypeActive = Convert.ToBoolean(Convert.ToByte(accountProperty.Value));
                        break;
                    case AccountPropertyKeys.ValueComments:
                        this.Contract.ValueComments = accountProperty.Value;
                        break;
                    case AccountPropertyKeys.ContractDescTitle:
                        this.Contract.ContractDescTitle = accountProperty.Value;
                        break;
                    case AccountPropertyKeys.ContractDescShortTitle:
                        this.Contract.ContractDescShortTitle = accountProperty.Value;
                        break;
                    case AccountPropertyKeys.ContractNumGen:
                        this.Contract.ContractNumGen = Convert.ToBoolean(Convert.ToByte(accountProperty.Value));
                        break;
                    case AccountPropertyKeys.ContractNumSeq:
                        this.Contract.ContractNumSeq = Convert.ToInt32(accountProperty.Value);
                        break;
                    case AccountPropertyKeys.ContractCatMandatory:
                        this.Contract.ContractCatMandatory = Convert.ToBoolean(Convert.ToByte(accountProperty.Value));
                        break;
                    default:
                        break;
                }
            }

            return this;
        }

        /// <summary>
        /// Sets the <see cref="IInvoicesOptions"/>
        /// </summary>
        /// <returns>An instance of <see cref="IGeneralOptions"/></returns>
        public override IGeneralOptions WithInvoices()
        {
            if (this.Invoices != null) return this;

            this.Invoices = new InvoicesOptions();

            foreach (var accountProperty in this._accountProperties)
            {
                switch (accountProperty.AccountPropertyKey)
                {
                    case AccountPropertyKeys.PONumberGenerate:
                        this.Invoices.PONumberGenerate = Convert.ToBoolean(Convert.ToByte(accountProperty.Value));
                        break;
                    case AccountPropertyKeys.PONumberSequence:
                        this.Invoices.PONumberSequence = Convert.ToInt32(accountProperty.Value);
                        break;
                    case AccountPropertyKeys.PONumberFormat:
                        this.Invoices.PONumberFormat = accountProperty.Value;
                        break;
                    case AccountPropertyKeys.KeepInvoiceForecasts:
                        this.Invoices.KeepInvoiceForecasts = Convert.ToBoolean(Convert.ToByte(accountProperty.Value));
                        break;
                    default:
                        break;
                }
            }

            return this;
        }

        /// <summary>
        /// Sets the <see cref="IRechargeOptions"/>
        /// </summary>
        /// <returns>An instance of <see cref="IGeneralOptions"/></returns>
        public override IGeneralOptions WithRecharge()
        {
            if (this.Recharge != null) return this;

            this.Recharge = new RechargeOptions();

            foreach (var accountProperty in this._accountProperties)
            {
                switch (accountProperty.AccountPropertyKey)
                {
                    case AccountPropertyKeys.ReferenceAs:
                        this.Recharge.ReferenceAs = accountProperty.Value;
                        break;
                    case AccountPropertyKeys.StaffRepAs:
                        this.Recharge.StaffRepAs = accountProperty.Value;
                        break;
                    case AccountPropertyKeys.RechargePeriod:
                        this.Recharge.RechargePeriod = Convert.ToInt32(accountProperty.Value);
                        break;
                    case AccountPropertyKeys.FinYearCommence:
                        this.Recharge.FinYearCommence = Convert.ToInt32(accountProperty.Value);
                        break;
                    case AccountPropertyKeys.CPDeleteAction:
                        this.Recharge.CPDeleteAction = Convert.ToInt32(accountProperty.Value);
                        break;
                    default:
                        break;
                }
            }

            return this;
        }

        /// <summary>
        /// Sets the <see cref="IScheduleOptions"/>
        /// </summary>
        /// <returns>An instance of <see cref="IGeneralOptions"/></returns>
        public override IGeneralOptions WithSchedule()
        {
            if (this.Schedule != null) return this;

            this.Schedule = new ScheduleOptions();

            foreach (var accountProperty in this._accountProperties)
            {
                switch (accountProperty.AccountPropertyKey)
                {
                    case AccountPropertyKeys.EnableVariationAutoSeq:
                        this.Schedule.EnableVariationAutoSeq = Convert.ToBoolean(Convert.ToByte(accountProperty.Value));
                        break;
                    default:
                        break;
                }
            }

            return this;
        }

        /// <summary>
        /// Sets the <see cref="ISearchOptions"/>
        /// </summary>
        /// <returns>An instance of <see cref="IGeneralOptions"/></returns>
        public override IGeneralOptions WithSearch()
        {
            if (this.Search != null) return this;

            this.Search = new SearchOptions();

            foreach (var accountProperty in this._accountProperties)
            {
                switch (accountProperty.AccountPropertyKey)
                {
                    case AccountPropertyKeys.ShowProductInSearch:
                        this.Search.ShowProductInSearch = Convert.ToBoolean(Convert.ToByte(accountProperty.Value));
                        break;
                    default:
                        break;
                }
            }

            return this;
        }

        /// <summary>
        /// Sets the <see cref="ISupplierOptions"/>
        /// </summary>
        /// <returns>An instance of <see cref="IGeneralOptions"/></returns>
        public override IGeneralOptions WithSupplier()
        {
            if (this.Supplier != null) return this;

            this.Supplier = new SupplierOptions();

            foreach (var accountProperty in this._accountProperties)
            {
                switch (accountProperty.AccountPropertyKey)
                {
                    case AccountPropertyKeys.StatusEnforced:
                        this.Supplier.SupplierStatusEnforced = Convert.ToBoolean(Convert.ToByte(accountProperty.Value));
                        break;
                    case AccountPropertyKeys.LastFinStatusEnabled:
                        this.Supplier.SupplierLastFinStatusEnabled = Convert.ToBoolean(Convert.ToByte(accountProperty.Value));
                        break;
                    case AccountPropertyKeys.LastFinCheckEnabled:
                        this.Supplier.SupplierLastFinCheckEnabled = Convert.ToBoolean(Convert.ToByte(accountProperty.Value));
                        break;
                    case AccountPropertyKeys.FYEEnabled:
                        this.Supplier.SupplierFYEEnabled = Convert.ToBoolean(Convert.ToByte(accountProperty.Value));
                        break;
                    case AccountPropertyKeys.NumEmployeesEnabled:
                        this.Supplier.SupplierNumEmployeesEnabled = Convert.ToBoolean(Convert.ToByte(accountProperty.Value));
                        break;
                    case AccountPropertyKeys.TurnoverEnabled:
                        this.Supplier.SupplierTurnoverEnabled = Convert.ToBoolean(Convert.ToByte(accountProperty.Value));
                        break;
                    case AccountPropertyKeys.IntContactEnabled:
                        this.Supplier.SupplierIntContactEnabled = Convert.ToBoolean(Convert.ToByte(accountProperty.Value));
                        break;
                    case AccountPropertyKeys.CatTitle:
                        this.Supplier.SupplierCatTitle = accountProperty.Value;
                        break;
                    case AccountPropertyKeys.RegionTitle:
                        this.Supplier.SupplierRegionTitle = accountProperty.Value;
                        break;
                    case AccountPropertyKeys.PrimaryTitle:
                        this.Supplier.SupplierPrimaryTitle = accountProperty.Value;
                        break;
                    case AccountPropertyKeys.VariationTitle:
                        this.Supplier.SupplierVariationTitle = accountProperty.Value;
                        break;
                    case AccountPropertyKeys.CatMandatory:
                        this.Supplier.SupplierCatMandatory = Convert.ToBoolean(Convert.ToByte(accountProperty.Value));
                        break;
                    default:
                        break;
                }
            }

            return this;
        }

        /// <summary>
        /// Sets the <see cref="ITaskOptions"/>
        /// </summary>
        /// <returns>An instance of <see cref="IGeneralOptions"/></returns>
        public override IGeneralOptions WithTask()
        {
            if (this.Task != null) return this;

            this.Task = new TaskOptions();

            foreach (var accountProperty in this._accountProperties)
            {
                switch (accountProperty.AccountPropertyKey)
                {
                    case AccountPropertyKeys.TaskEscalationRepeat:
                        this.Task.TaskEscalationRepeat = Convert.ToInt32(accountProperty.Value);
                        break;
                    case AccountPropertyKeys.TaskStartDateMandatory:
                        this.Task.TaskStartDateMandatory = Convert.ToBoolean(Convert.ToByte(accountProperty.Value));
                        break;
                    case AccountPropertyKeys.TaskEndDateMandatory:
                        this.Task.TaskEndDateMandatory = Convert.ToBoolean(Convert.ToByte(accountProperty.Value));
                        break;
                    case AccountPropertyKeys.TaskDueDateMandatory:
                        this.Task.TaskDueDateMandatory = Convert.ToBoolean(Convert.ToByte(accountProperty.Value));
                        break;
                    default:
                        break;
                }
            }

            return this;
        }

        /// <summary>
        /// Sets the <see cref="IHelpAndSupportOptions"/>
        /// </summary>
        /// <returns>An instance of <see cref="IGeneralOptions"/></returns>
        public override IGeneralOptions WithHelpAndSupport()
        {
            if (this.HelpAndSupport != null) return this;

            this.HelpAndSupport = new HelpAndSupportOptions();

            foreach (var accountProperty in this._accountProperties)
            {
                switch (accountProperty.AccountPropertyKey)
                {
                    case AccountPropertyKeys.CustomerHelpInformation:
                        this.HelpAndSupport.CustomerHelpInformation = accountProperty.Value;
                        break;
                    case AccountPropertyKeys.CustomerHelpContactName:
                        this.HelpAndSupport.CustomerHelpContactName = accountProperty.Value;
                        break;
                    case AccountPropertyKeys.CustomerHelpContactTelephone:
                        this.HelpAndSupport.CustomerHelpContactTelephone = accountProperty.Value;
                        break;
                    case AccountPropertyKeys.CustomerHelpContactFax:
                        this.HelpAndSupport.CustomerHelpContactFax = accountProperty.Value;
                        break;
                    case AccountPropertyKeys.CustomerHelpContactAddress:
                        this.HelpAndSupport.CustomerHelpContactAddress = accountProperty.Value;
                        break;
                    case AccountPropertyKeys.CustomerHelpContactEmailAddress:
                        this.HelpAndSupport.CustomerHelpContactEmailAddress = accountProperty.Value;
                        break;
                    case AccountPropertyKeys.EnableInternalSupportTickets:
                        this.HelpAndSupport.EnableInternalSupportTickets = Convert.ToBoolean(Convert.ToByte(accountProperty.Value));
                        break;
                    default:
                        break;
                }
            }

            return this;
        }

        /// <summary>
        /// Sets the <see cref="IMileageOptions"/>
        /// </summary>
        /// <returns>An instance of <see cref="IGeneralOptions"/></returns>
        public override IGeneralOptions WithMileage()
        {
            if (this.Mileage != null) return this;

            this.Mileage = new MileageOptions();

            foreach (var accountProperty in this._accountProperties)
            {
                switch (accountProperty.AccountPropertyKey)
                {
                    case AccountPropertyKeys.MandatoryPostcodeForAddresses:
                        this.Mileage.MandatoryPostcodeForAddresses = Convert.ToBoolean(Convert.ToByte(accountProperty.Value));
                        break;
                    case AccountPropertyKeys.OdometerDay:
                        this.Mileage.OdometerDay = Convert.ToByte(accountProperty.Value);
                        break;
                    case AccountPropertyKeys.AddLocations:
                        this.Mileage.AddLocations = Convert.ToBoolean(Convert.ToByte(accountProperty.Value));
                        break;
                    case AccountPropertyKeys.EnterOdometerOnSubmit:
                        this.Mileage.EnterOdometerOnSubmit = Convert.ToBoolean(Convert.ToByte(accountProperty.Value));
                        break;
                    case AccountPropertyKeys.AllowMultipleDestinations:
                        this.Mileage.AllowMultipleDestinations = Convert.ToBoolean(Convert.ToByte(accountProperty.Value));
                        break;
                    case AccountPropertyKeys.UseMapPoint:
                        this.Mileage.UseMapPoint = Convert.ToBoolean(Convert.ToByte(accountProperty.Value));
                        break;
                    case AccountPropertyKeys.HomeToOffice:
                        this.Mileage.HomeToOffice = Convert.ToBoolean(Convert.ToByte(accountProperty.Value));
                        break;
                    case AccountPropertyKeys.AutoCalcHomeToLocation:
                        this.Mileage.AutoCalcHomeToLocation = Convert.ToBoolean(Convert.ToByte(accountProperty.Value));
                        break;
                    case AccountPropertyKeys.MileageCalcType:
                        this.Mileage.MileageCalcType = Convert.ToByte(accountProperty.Value);
                        break;
                    case AccountPropertyKeys.RecordOdometer:
                        this.Mileage.RecordOdometer = Convert.ToBoolean(Convert.ToByte(accountProperty.Value));
                        break;
                    case AccountPropertyKeys.Mileage:
                        this.Mileage.Mileage = Convert.ToInt32(accountProperty.Value);
                        break;
                    default:
                        break;
                }
            }

            return this;
        }

        /// <summary>
        /// Sets the <see cref="IMobileOptions"/>
        /// </summary>
        /// <returns>An instance of <see cref="IGeneralOptions"/></returns>
        public override IGeneralOptions WithMobile()
        {
            if (this.Mobile != null) return this;

            this.Mobile = new MobileOptions();

            foreach (var accountProperty in this._accountProperties)
            {
                switch (accountProperty.AccountPropertyKey)
                {
                    case AccountPropertyKeys.UseMobileDevices:
                        this.Mobile.UseMobileDevices = Convert.ToBoolean(Convert.ToByte(accountProperty.Value));
                        break;
                    default:
                        break;
                }
            }

            return this;
        }

        /// <summary>
        /// Sets the <see cref="IMyDetailsOptions"/>
        /// </summary>
        /// <returns>An instance of <see cref="IGeneralOptions"/></returns>
        public override IGeneralOptions WithMyDetails()
        {
            if (this.MyDetails != null) return this;

            this.MyDetails = new MyDetailsOptions();

            foreach (var accountProperty in this._accountProperties)
            {
                switch (accountProperty.AccountPropertyKey)
                {
                    case AccountPropertyKeys.AllowEmployeeToNotifyOfChangeOfDetails:
                        this.MyDetails.AllowEmployeeToNotifyOfChangeOfDetails = Convert.ToBoolean(Convert.ToByte(accountProperty.Value));
                        break;
                    case AccountPropertyKeys.EditMyDetails:
                        this.MyDetails.EditMyDetails = Convert.ToBoolean(Convert.ToByte(accountProperty.Value));
                        break;
                    default:
                        break;
                }
            }

            return this;
        }

        /// <summary>
        /// Sets the <see cref="IPasswordOptions"/>
        /// </summary>
        /// <returns>An instance of <see cref="IGeneralOptions"/></returns>
        public override IGeneralOptions WithPassword()
        {
            if (this.Password != null) return this;

            this.Password = new PasswordOptions();

            foreach (var accountProperty in this._accountProperties)
            {
                switch (accountProperty.AccountPropertyKey)
                {
                    case AccountPropertyKeys.PwdMustContainNumbers:
                        this.Password.PwdMustContainNumbers = Convert.ToBoolean(Convert.ToByte(accountProperty.Value));
                        break;
                    case AccountPropertyKeys.PwdMustContainUpperCase:
                        this.Password.PwdMustContainUpperCase = Convert.ToBoolean(Convert.ToByte(accountProperty.Value));
                        break;
                    case AccountPropertyKeys.PwdMustContainSymbol:
                        this.Password.PwdMustContainSymbol = Convert.ToBoolean(Convert.ToByte(accountProperty.Value));
                        break;
                    case AccountPropertyKeys.PwdExpires:
                        this.Password.PwdExpires = Convert.ToBoolean(Convert.ToByte(accountProperty.Value));
                        break;
                    case AccountPropertyKeys.PwdExpiryDays:
                        this.Password.PwdExpiryDays = Convert.ToInt32(accountProperty.Value);
                        break;
                    case AccountPropertyKeys.PwdConstraint:
                        this.Password.PwdConstraint = (PasswordLength)Convert.ToByte(accountProperty.Value);
                        break;
                    case AccountPropertyKeys.PwdLength1:
                        this.Password.PwdLength1 = Convert.ToInt32(accountProperty.Value);
                        break;
                    case AccountPropertyKeys.PwdLength2:
                        this.Password.PwdLength2 = Convert.ToInt32(accountProperty.Value);
                        break;
                    case AccountPropertyKeys.PwdMaxRetries:
                        this.Password.PwdMaxRetries = Convert.ToByte(accountProperty.Value);
                        break;
                    case AccountPropertyKeys.PwdHistoryNum:
                        this.Password.PwdHistoryNum = Convert.ToInt32(accountProperty.Value);
                        break;
                    default:
                        break;
                }
            }

            return this;
        }

        /// <summary>
        /// Sets the <see cref="IRegionalSettingsOptions"/>
        /// </summary>
        /// <returns>An instance of <see cref="IGeneralOptions"/></returns>
        public override IGeneralOptions WithRegionalSettings()
        {
            if (this.RegionalSettings != null) return this;

            this.RegionalSettings = new RegionalSettingsOptions();

            foreach (var accountProperty in this._accountProperties)
            {
                switch (accountProperty.AccountPropertyKey)
                {
                    case AccountPropertyKeys.GlobalLocaleID:
                        this.RegionalSettings.GlobalLocaleID = Convert.ToInt32(accountProperty.Value);
                        break;
                    default:
                        break;
                }
            }

            return this;
        }

        /// <summary>
        /// Sets the <see cref="IReportOptions"/>
        /// </summary>
        /// <returns>An instance of <see cref="IGeneralOptions"/></returns>
        public override IGeneralOptions WithReport()
        {
            if (this.Report != null) return this;

            this.Report = new ReportOptions();

            foreach (var accountProperty in this._accountProperties)
            {
                switch (accountProperty.AccountPropertyKey)
                {
                    case AccountPropertyKeys.DrilldownReport:
                        if (string.IsNullOrEmpty(accountProperty.Value))
                        {
                            this.Report.DrilldownReport = null;
                        }
                        else
                        {
                            this.Report.DrilldownReport = new Guid(accountProperty.Value);
                        }
                        break;
                    default:
                        break;
                }
            }

            return this;
        }

        /// <summary>
        /// Sets the <see cref="ISelfRegistrationOptions"/>
        /// </summary>
        /// <returns>An instance of <see cref="IGeneralOptions"/></returns>
        public override IGeneralOptions WithSelfRegistration()
        {
            if (this.SelfRegistration != null) return this;

            this.SelfRegistration = new SelfRegistrationOptions();

            foreach (var accountProperty in this._accountProperties)
            {
                switch (accountProperty.AccountPropertyKey)
                {
                    case AccountPropertyKeys.AllowSelfReg:
                        this.SelfRegistration.AllowSelfReg = Convert.ToBoolean(Convert.ToByte(accountProperty.Value));
                        break;
                    case AccountPropertyKeys.AllowSelfRegRole:
                        this.SelfRegistration.AllowSelfRegRole = Convert.ToBoolean(Convert.ToByte(accountProperty.Value));
                        break;
                    case AccountPropertyKeys.AllowSelfRegItemRole:
                        this.SelfRegistration.AllowSelfRegItemRole = Convert.ToBoolean(Convert.ToByte(accountProperty.Value));
                        break;
                    case AccountPropertyKeys.AllowSelfRegEmployeeContact:
                        this.SelfRegistration.AllowSelfRegEmployeeContact = Convert.ToBoolean(Convert.ToByte(accountProperty.Value));
                        break;
                    case AccountPropertyKeys.AllowSelfRegHomeAddress:
                        this.SelfRegistration.AllowSelfRegHomeAddress = Convert.ToBoolean(Convert.ToByte(accountProperty.Value));
                        break;
                    case AccountPropertyKeys.AllowSelfRegEmployeeInfo:
                        this.SelfRegistration.AllowSelfRegEmployeeInfo = Convert.ToBoolean(Convert.ToByte(accountProperty.Value));
                        break;
                    case AccountPropertyKeys.AllowSelfRegSignOff:
                        this.SelfRegistration.AllowSelfRegSignOff = Convert.ToBoolean(Convert.ToByte(accountProperty.Value));
                        break;
                    case AccountPropertyKeys.AllowSelfRegAdvancesSignOff:
                        this.SelfRegistration.AllowSelfRegAdvancesSignOff = Convert.ToBoolean(Convert.ToByte(accountProperty.Value));
                        break;
                    case AccountPropertyKeys.AllowSelfRegDepartmentCostCode:
                        this.SelfRegistration.AllowSelfRegDepartmentCostCode = Convert.ToBoolean(Convert.ToByte(accountProperty.Value));
                        break;
                    case AccountPropertyKeys.AllowSelfRegBankDetails:
                        this.SelfRegistration.AllowSelfRegBankDetails = Convert.ToBoolean(Convert.ToByte(accountProperty.Value));
                        break;
                    case AccountPropertyKeys.AllowSelfRegCarDetails:
                        this.SelfRegistration.AllowSelfRegCarDetails = Convert.ToBoolean(Convert.ToByte(accountProperty.Value));
                        break;
                    case AccountPropertyKeys.AllowSelfRegUDF:
                        this.SelfRegistration.AllowSelfRegUDF = Convert.ToBoolean(Convert.ToByte(accountProperty.Value));
                        break;
                    case AccountPropertyKeys.DefaultRole:
                        if (accountProperty.Value == string.Empty)
                        {
                            this.SelfRegistration.DefaultRole = null;
                        }
                        else
                        {
                            this.SelfRegistration.DefaultRole = Convert.ToInt32(accountProperty.Value);
                        }
                        break;
                    case AccountPropertyKeys.DefaultItemRole:
                        if (accountProperty.Value == string.Empty)
                        {
                            this.SelfRegistration.DefaultItemRole = null;
                        }
                        else
                        {
                            this.SelfRegistration.DefaultItemRole = Convert.ToInt32(accountProperty.Value);
                        }
                        break;
                    default:
                        break;
                }
            }

            return this;
        }

        /// <summary>
        /// Sets the <see cref="ISessionTimeoutOptions"/>
        /// </summary>
        /// <returns>An instance of <see cref="IGeneralOptions"/></returns>
        public override IGeneralOptions WithSessionTimeout()
        {
            if (this.SessionTimeout != null) return this;

            this.SessionTimeout = new SessionTimeoutOptions();

            foreach (var accountProperty in this._accountProperties)
            {
                switch (accountProperty.AccountPropertyKey)
                {
                    case AccountPropertyKeys.IdleTimeout:
                        this.SessionTimeout.IdleTimeout = Convert.ToInt32(accountProperty.Value);
                        break;
                    case AccountPropertyKeys.CountdownTimer:
                        this.SessionTimeout.CountdownTimer = Convert.ToInt32(accountProperty.Value);
                        break;
                    default:
                        break;
                }
            }

            return this;
        }

        /// <summary>
        /// Sets the <see cref="IValidateOptions"/>
        /// </summary>
        /// <returns>An instance of <see cref="IGeneralOptions"/></returns>
        public override IGeneralOptions WithValidate()
        {
            if (this.Validate != null) return this;

            this.Validate = new ValidateOptions();

            foreach (var accountProperty in this._accountProperties)
            {
                switch (accountProperty.AccountPropertyKey)
                {
                    case AccountPropertyKeys.NotifyWhenEnvelopeNotReceivedDays:
                        this.Validate.NotifyWhenEnvelopeNotReceivedDays = Convert.ToInt32(Convert.ToByte(accountProperty.Value));
                        break;
                    default:
                        break;
                }
            }

            return this;
        }

        /// <summary>
        /// Sets the <see cref="IVatCalculationOptions"/>
        /// </summary>
        /// <returns>An instance of <see cref="IGeneralOptions"/></returns>
        public override IGeneralOptions WithVatCalculation()
        {
            if (this.VatCalculation != null) return this;

            this.VatCalculation = new VatCalculationOptions();

            foreach (var accountProperty in this._accountProperties)
            {
                switch (accountProperty.AccountPropertyKey)
                {
                    case AccountPropertyKeys.EnableCalculationsForAllocatingFuelReceiptVatToMileage:
                        this.VatCalculation.EnableCalculationsForAllocatingFuelReceiptVatToMileage = Convert.ToBoolean(Convert.ToByte(accountProperty.Value));
                        break;
                    default:
                        break;
                }
            }

            return this;
        }

        /// <summary>
        /// Sets the <see cref="ICacheOptions"/>
        /// </summary>
        /// <returns>An instance of <see cref="IGeneralOptions"/></returns>
        public override IGeneralOptions WithCache()
        {
            if (this.Cache != null) return this;

            this.Cache = new CacheOptions();

            foreach (var accountProperty in this._accountProperties)
            {
                switch (accountProperty.AccountPropertyKey)
                {
                    case AccountPropertyKeys.CacheTimeout:
                        this.Cache.CacheTimeout = Convert.ToInt32(accountProperty.Value);
                        break;
                    case AccountPropertyKeys.CachePeriodShort:
                        this.Cache.CachePeriodShort = Convert.ToInt32(accountProperty.Value);
                        break;
                    case AccountPropertyKeys.CachePeriodNormal:
                        this.Cache.CachePeriodNormal = Convert.ToInt32(accountProperty.Value);
                        break;
                    case AccountPropertyKeys.CachePeriodLong:
                        this.Cache.CachePeriodLong = Convert.ToInt32(accountProperty.Value);
                        break;
                    default:
                        break;
                }
            }

            return this;
        }

        /// <summary>
        /// Sets the <see cref="ICorporateDiligenceOptions"/>
        /// </summary>
        /// <returns>An instance of <see cref="IGeneralOptions"/></returns>
        public override IGeneralOptions WithCorporateDiligence()
        {
            if (this.CorporateDiligence != null) return this;

            this.CorporateDiligence = new CorporateDiligenceOptions();

            foreach (var accountProperty in this._accountProperties)
            {
                switch (accountProperty.AccountPropertyKey)
                {
                    case AccountPropertyKeys.CorporateDStartPage:
                        this.CorporateDiligence.CorporateDStartPage = accountProperty.Value;
                        break;
                    default:
                        break;
                }
            }

            return this;
        }

        /// <summary>
        /// Sets the <see cref="IPageOptions"/>
        /// </summary>
        /// <returns>An instance of <see cref="IGeneralOptions"/></returns>
        public override IGeneralOptions WithPage()
        {
            if (this.Page != null) return this;

            this.Page = new PageOptions();

            foreach (var accountProperty in this._accountProperties)
            {
                switch (accountProperty.AccountPropertyKey)
                {
                    case AccountPropertyKeys.DefaultPageSize:
                        this.Page.DefaultPageSize = Convert.ToInt32(accountProperty.Value);
                        break;
                    default:
                        break;
                }
            }

            return this;
        }

        /// <summary>
        /// Sets the <see cref="IFinancialYearOptions"/>
        /// </summary>
        /// <returns>An instance of <see cref="IGeneralOptions"/></returns>
        public override IGeneralOptions WithFinancialYear()
        {
            if (this.FinancialYear != null) return this;

            this.FinancialYear = new FinancialYearOptions();

            foreach (var accountProperty in this._accountProperties)
            {
                switch (accountProperty.AccountPropertyKey)
                {
                    case AccountPropertyKeys.FYStarts:
                        this.FinancialYear.FYStarts = accountProperty.Value;
                        break;
                    case AccountPropertyKeys.FYEnds:
                        this.FinancialYear.FYEnds = accountProperty.Value;
                        break;
                    default:
                        break;
                }
            }

            return this;
        }

        /// <summary>
        /// Sets the <see cref="IHotelOptions"/>
        /// </summary>
        /// <returns>An instance of <see cref="IGeneralOptions"/></returns>
        public override IGeneralOptions WithHotel()
        {
            if (this.Hotel != null) return this;

            this.Hotel = new HotelOptions();

            foreach (var accountProperty in this._accountProperties)
            {
                switch (accountProperty.AccountPropertyKey)
                {
                    case AccountPropertyKeys.ShowReviews:
                        this.Hotel.ShowReviews = Convert.ToBoolean(Convert.ToByte(accountProperty.Value));
                        break;
                    case AccountPropertyKeys.SendReviewRequests:
                        this.Hotel.SendReviewRequests = Convert.ToBoolean(Convert.ToByte(accountProperty.Value));
                        break;
                    default:
                        break;
                }
            }

            return this;
        }

        /// <summary>
        /// Sets the <see cref="INotesOptions"/>
        /// </summary>
        /// <returns>An instance of <see cref="IGeneralOptions"/></returns>
        public override IGeneralOptions WithNotes()
        {
            if (this.Notes != null) return this;

            this.Notes = new NotesOptions();

            foreach (var accountProperty in this._accountProperties)
            {
                switch (accountProperty.AccountPropertyKey)
                {
                    case AccountPropertyKeys.AllowArchivedNotesAdd:
                        this.Notes.AllowArchivedNotesAdd = Convert.ToBoolean(Convert.ToByte(accountProperty.Value));
                        break;
                    default:
                        break;
                }
            }

            return this;
        }

        /// <summary>
        /// Sets the <see cref="IReminderOptions"/>
        /// </summary>
        /// <returns>An instance of <see cref="IGeneralOptions"/></returns>
        public override IGeneralOptions WithReminders()
        {
            if (this.Reminders != null) return this;

            this.Reminders = new ReminderOptions();

            foreach (var accountProperty in this._accountProperties)
            {
                switch (accountProperty.AccountPropertyKey)
                {
                    case AccountPropertyKeys.EnableClaimApprovalReminders:
                        this.Reminders.EnableClaimApprovalReminders = Convert.ToBoolean(Convert.ToByte(accountProperty.Value));
                        break;
                    case AccountPropertyKeys.ClaimApprovalReminderFrequency:
                        this.Reminders.ClaimApprovalReminderFrequency = Convert.ToInt32(accountProperty.Value);
                        break;
                    case AccountPropertyKeys.EnableCurrentClaimsReminders:
                        this.Reminders.EnableCurrentClaimsReminders = Convert.ToBoolean(Convert.ToByte(accountProperty.Value));
                        break;
                    case AccountPropertyKeys.CurrentClaimsReminderFrequency:
                        this.Reminders.CurrentClaimsReminderFrequency = Convert.ToInt32(accountProperty.Value);
                        break;
                    default:
                        break;
                }
            }

            return this;
        }
    }
}
