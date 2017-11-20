using EsrGo2FromNhsWcfLibrary.Spend_Management;
using SpendManagementLibrary.Employees;
using SpendManagementLibrary.Hotels;
using SpendManagementLibrary.Interfaces;
using SpendManagementLibrary.Interfaces.Expedite;
using Spend_Management.expenses.code;
using Spend_Management.expenses.code.Claims;

namespace UnitTest2012Ultimate.API.Utilities
{
    using System;
    using Moq;

    using SpendManagementApi.Interfaces;
    using SpendManagementLibrary;
    using SpendManagementLibrary.Addresses;
    using SpendManagementLibrary.Employees.DutyOfCare;

    using Spend_Management;
    using Spend_Management.shared.code;

    public class TestActionContext : IActionContext
    {
        private Mock<cEmployees> _employees;

        private Mock<cMileagecats> _mileageCats;

        private Mock<cLocales> _locales;

        private Mock<cCountries> _countries;

        private Mock<cCurrencies> _currencies;

        private Mock<cEmployeeCorporateCards> _corporateCards;

        private Mock<cGroups> _groups;

        private Mock<CardProviders> _cardProviders;

        private Mock<cGlobalCurrencies> _globalCurrencies;

        private Mock<cGlobalCountries> _globalCountries;

        private Mock<cSubcats> _subCategories;

        private Mock<cCategories> _expenseCategories;

        private Mock<cP11dcats> _p11DCategories;

        private Mock<cAccessRoles> _accessRoles;

        private Mock<cAllowances> _allowances;

        private Mock<cBudgetholders> _budgetHolders;

        private Mock<cReasons> _claimReasons;

        private Mock<cCostcodes> _costCodes;

        private Mock<cDepartments> _departments;

        private Mock<cTeams> _teams;

        private Mock<cPoolCars> _poolCars;

        private Mock<cProjectCodes> _projectCodes;

        private Mock<cCategories> _categories;

        private Mock<cItemRoles> _itemRoles;

        private Mock<cUserdefinedFields> _userDefinedFields;

        private Mock<cTables> _tables;

        private Mock<cMobileDevices> _devices;

        private Mock<cEmployeeCars> _employeeCars;

        private Mock<cESRAssignments> _esrAssignments;

        private Mock<cAccountSubAccounts> _subAccounts;

        private Mock<cESRTrusts> _nhsTrusts;

        private Mock<Addresses> _addresses;

        private Mock<AddressLabels> _addressLabels;

        private Mock<EmployeeHomeAddresses> _homeAddresses;

        private Mock<EmployeeWorkAddresses> _workAddresses;

        private Mock<cCustomEntities> _customEntities;

        private Mock<cClaims> _claims;

        private Mock<ClaimSubmission> _claimSubmission;

        private IManageEnvelopes _envelopes;

        private IManageReceipts _receipts;

        private IManageExpenseValidation _expenseValidation;

        private Mock<cEmailTemplates> _emails;

        private Mock<cFields> _fields;

        private Mock<cAccounts> _accounts;

        private Mock<Organisations> _organisations;

        private IManageFunds _funds;

        private IManagePayment _payment;

        private Hotels _hotels;

        private Hotel _hotel;

        private Mock<cFilterRules> _filterrules;

        //**APIiser_Marker**//

        private int? _SubAccountId;
        private int? _accountId;

        public ICurrentUserBase CurrentUser { get; set; }


        public int EmployeeId
        {
            get
            {
                return GlobalTestVariables.EmployeeId;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public SpendManagementLibrary.GeneralOptions.GeneralOptions GeneralOptions { get; set; }

        public int AccountId
        {
            get
            {
                if (_accountId == null)
                {
                    _accountId = GlobalTestVariables.AccountId;
                }
                return _accountId.Value;
            }
            set { _accountId = value; }
        }

        private int? _subAccountId;


        public cExpenseItems ExpenseItems { get; set; }
        public cFilterRules filterRules { get; set; }

        public FlagManagement FlagManagement { get; set; }

        public int? SubAccountId
        {
            get
            {
                return GlobalTestVariables.SubAccountId;
            }
            set { _subAccountId = value; }
        }

        internal void SetEmployeesMock(Mock<cEmployees> employees)
        {
            _employees = employees;
        }

        internal void SetCorporateCardsMock(Mock<cEmployeeCorporateCards> corporateCards)
        {
            _corporateCards = corporateCards;
        }

        internal void SetMileageCategoriesMock(Mock<cMileagecats> mileageCategories)
        {
            _mileageCats = mileageCategories;
        }

        internal void SetNhsTrustsMock(Mock<cESRTrusts> esrTrusts)
        {
            _nhsTrusts = esrTrusts;
        }

        internal void SetLocalesMock(Mock<cLocales> locales)
        {
            _locales = locales;
        }

        internal void SetCountriesMock(Mock<cCountries> countries)
        {
            _countries = countries;
        }

        internal void SetCurrenciesMock(Mock<cCurrencies> currencies)
        {
            _currencies = currencies;
        }

        internal void SetGroupsMock(Mock<cGroups> groups)
        {
            _groups = groups;
        }

        internal void SetCardProvidersMock(Mock<CardProviders> cardProviders)
        {
            _cardProviders = cardProviders;
        }

        internal void SetClaimReasonsMock(Mock<cReasons> reasons)
        {
            _claimReasons = reasons;
        }

        internal void SetCostCodesMock(Mock<cCostcodes> costCodes)
        {
            _costCodes = costCodes;
        }

        internal void SetDepartmentsMock(Mock<cDepartments> departments)
        {
            _departments = departments;
        }

        internal void SetUserDefinedFieldsMock(Mock<cUserdefinedFields> userDefinedFields)
        {
            _userDefinedFields = userDefinedFields;
        }

        internal void SetTablesMock(Mock<cTables> tables)
        {
            _tables = tables;
        }

        internal void SetGlobalCountriesMock(Mock<cGlobalCountries> globalCountries)
        {
            _globalCountries = globalCountries;
        }

        internal void SetSubCategoriesMock(Mock<cSubcats> subcats)
        {
            _subCategories = subcats;
        }

        internal void SetCategoriesMock(Mock<cCategories> categories)
        {
            _expenseCategories = categories;
        }

        internal void SetP11DCategoriesMock(Mock<cP11dcats> p11DCategories)
        {
            _p11DCategories = p11DCategories;
        }

        internal void SetPoolCarsMock(Mock<cPoolCars> poolCars)
        {
            _poolCars = poolCars;
        }

        internal void SetProjectCodesMock(Mock<cProjectCodes> projectCodes)
        {
            _projectCodes = projectCodes;
        }

        internal void SetItemRolesMock(Mock<cItemRoles> itemRoles)
        {
            _itemRoles = itemRoles;
        }

        internal void SetAddressesMock(Mock<Addresses> addresses)
        {
            _addresses = addresses;
        }

        internal void SetAddressLabelsMock(Mock<AddressLabels> labels)
        {
            _addressLabels = labels;
        }

        internal void SetEmployeeHomeAddressesMock(Mock<EmployeeHomeAddresses> homeAddresses)
        {
            _homeAddresses = homeAddresses;
        }

        internal void SetEmployeeWorkAddressesMock(Mock<EmployeeWorkAddresses> workAddresses)
        {
            _workAddresses = workAddresses;
        }

        internal void SetClaimsMock(Mock<cClaims> claims)
        {
            _claims = claims;
        }

        internal void SetEnvelopes(IManageEnvelopes envelopes)
        {
            _envelopes = envelopes;
        }

        internal void SetReceipts(IManageReceipts receipts)
        {
            _receipts = receipts;
        }

        internal void SetExpenseValidation(IManageExpenseValidation expenseValidation)
        {
            _expenseValidation = expenseValidation;
        }

        internal void SetAllowancesMock(Mock<cAllowances> allowances)
        {
            _allowances = allowances;
        }

        internal void SetEmailsMock(Mock<cEmailTemplates> emails)
        {
            _emails = emails;
        }

        internal void SetFieldsMock(Mock<cFields> fields)
        {
            _fields = fields;
        }

        internal void SetAccountsMock(Mock<cAccounts> accounts)
        {
            _accounts = accounts;
        }

        internal void SetClaimSubmissionMock(Mock<ClaimSubmission> claimSubmission)
        {
            _claimSubmission = claimSubmission;
        }

        internal void SetOrganisationsMock(Mock<Organisations> organisations)
        {
            this._organisations = organisations;
        }

        internal void SetFilterRulesMock(Mock<cFilterRules> filterrules)
        {
            this._filterrules = filterrules;
        }



        internal void SetFunds(IManageFunds fund)
        {
            _funds = fund;
        }

        internal void SetPayment(IManagePayment payment)
        {
            _payment = payment;
        }


        //**APIiser_Marker**//

        #region Public Accessors

        public cAccounts Accounts
        {
            get { return (_accounts ?? (_accounts = new Mock<cAccounts>(MockBehavior.Default))).Object; }
            set { throw new NotImplementedException(); }
        }

        public cEmployees Employees
        {
            get
            {
                if (_employees == null)
                {
                    _employees = new Mock<cEmployees>(AccountId);
                }
                return _employees.Object;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public cMileagecats MileageCategories
        {
            get
            {
                if (_mileageCats == null)
                {
                    _mileageCats = new Mock<cMileagecats>(AccountId);
                }
                return _mileageCats.Object;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public cLocales Locales
        {
            get
            {
                if (_locales == null)
                {
                    _locales = new Mock<cLocales>();
                }
                return _locales.Object;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public cCountries Countries
        {
            get
            {
                if (_countries == null)
                {
                    _countries = new Mock<cCountries>(MockBehavior.Strict, AccountId, null);
                }
                return _countries.Object;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public cCurrencies Currencies
        {
            get
            {
                if (_currencies == null)
                {
                    _currencies = new Mock<cCurrencies>(MockBehavior.Strict, AccountId, null);
                }
                return _currencies.Object;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public cEmployeeCorporateCards CorporateCards
        {
            get
            {
                if (_corporateCards == null)
                {
                    _corporateCards = new Mock<cEmployeeCorporateCards>(MockBehavior.Strict, AccountId);
                }
                return _corporateCards.Object;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public cGroups SignoffGroups
        {
            get
            {
                if (_groups == null)
                {
                    _groups = new Mock<cGroups>(MockBehavior.Strict, AccountId);
                }
                return _groups.Object;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public CardProviders CardProviders
        {
            get
            {
                if (_cardProviders == null)
                {
                    _cardProviders = new Mock<CardProviders>(MockBehavior.Strict);
                }
                return _cardProviders.Object;
            }
            set
            {
                throw new NotImplementedException();
            }
        }


        public cAccessRoles AccessRoles
        {
            get
            {
                if (_accessRoles == null) _accessRoles = new Mock<cAccessRoles>(MockBehavior.Strict, AccountId, cAccounts.getConnectionString(AccountId));
                return _accessRoles.Object;
            }
            set { throw new NotImplementedException(); }
        }

        public cAllowances Allowances
        {
            get
            {
                if (_allowances == null)
                {
                    _allowances = new Mock<cAllowances>(MockBehavior.Strict, AccountId);
                }
                return _allowances.Object;
            }
            set { throw new NotImplementedException(); }
        }

        public cBudgetholders BudgetHolders
        {
            get
            {
                if (_budgetHolders == null)
                {
                    _budgetHolders = new Mock<cBudgetholders>(MockBehavior.Strict, AccountId);
                }
                return _budgetHolders.Object;
            }
            set { throw new NotImplementedException(); }
        }


        public cGlobalCurrencies GlobalCurrencies
        {
            get
            {
                if (_globalCurrencies == null)
                {
                    _globalCurrencies = new Mock<cGlobalCurrencies>(MockBehavior.Strict);
                }
                return _globalCurrencies.Object;
            }
            set
            {
                throw new NotImplementedException();
            }
        }


        public cGlobalCountries GlobalCountries
        {
            get
            {
                if (_globalCountries == null)
                {
                    _globalCountries = new Mock<cGlobalCountries>(MockBehavior.Strict);
                }
                return _globalCountries.Object;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public cSubcats SubCategories
        {
            get
            {
                if (_subCategories == null)
                {
                    _subCategories = new Mock<cSubcats>(MockBehavior.Strict);
                }
                return _subCategories.Object;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public cCategories ExpenseCategories
        {
            get
            {
                if (_expenseCategories == null)
                {
                    _expenseCategories = new Mock<cCategories>(MockBehavior.Strict, AccountId);
                }
                return _expenseCategories.Object;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public cP11dcats P11DCategories
        {
            get
            {
                if (_p11DCategories == null)
                {
                    _p11DCategories = new Mock<cP11dcats>(MockBehavior.Strict, AccountId);
                }
                return _p11DCategories.Object;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public cReasons ClaimReasons
        {
            get
            {
                if (_claimReasons == null)
                {
                    _claimReasons = new Mock<cReasons>(MockBehavior.Strict, AccountId);
                }
                return _claimReasons.Object;
            }
            set { throw new NotImplementedException(); }
        }

        public cCostcodes CostCodes
        {
            get
            {
                if (_costCodes == null)
                {
                    _costCodes = new Mock<cCostcodes>(MockBehavior.Strict, AccountId);
                }
                return _costCodes.Object;
            }
            set { throw new NotImplementedException(); }
        }

        public cDepartments Departments
        {
            get
            {
                if (_departments == null)
                {
                    _departments = new Mock<cDepartments>(MockBehavior.Loose, AccountId);
                }
                return _departments.Object;
            }
            set { throw new NotImplementedException(); }
        }

        public cTeams Teams
        {
            get
            {
                if (_teams == null)
                {
                    _teams = new Mock<cTeams>(MockBehavior.Strict, AccountId);
                }
                return _teams.Object;
            }
            set { throw new NotImplementedException(); }
        }

        public cPoolCars PoolCars
        {
            get
            {
                if (_poolCars == null)
                {
                    _poolCars = new Mock<cPoolCars>(MockBehavior.Strict, AccountId);
                }
                return _poolCars.Object;
            }
            set { throw new NotImplementedException(); }
        }

        public cProjectCodes ProjectCodes
        {
            get
            {
                if (_projectCodes == null)
                {
                    _projectCodes = new Mock<cProjectCodes>(MockBehavior.Strict, AccountId, null);
                }
                return _projectCodes.Object;
            }
            set { throw new NotImplementedException(); }
        }


        public cCategories Categories
        {
            get
            {
                _categories = _categories ?? new Mock<cCategories>(MockBehavior.Strict, GlobalTestVariables.AccountId);
                return _categories.Object;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public cItemRoles ItemRoles
        {
            get
            {
                _itemRoles = _itemRoles ?? new Mock<cItemRoles>(MockBehavior.Strict, GlobalTestVariables.AccountId);
                return _itemRoles.Object;
            }
            set
            {
                throw new NotImplementedException();
            }
        }


        public cMonthlyCurrencies MonthlyCurrencies
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public cRangeCurrencies RangeCurrencies
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public cUserdefinedFields UserDefinedFields
        {
            get
            {
                if (_userDefinedFields == null)
                {
                    _userDefinedFields = new Mock<cUserdefinedFields>(MockBehavior.Strict, GlobalTestVariables.AccountId);
                }
                return _userDefinedFields.Object;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public cTables Tables
        {
            get
            {
                if (_tables == null)
                {
                    _tables = new Mock<cTables>(MockBehavior.Strict, GlobalTestVariables.AccountId);
                }
                return _tables.Object;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public cMobileDevices MobileDevices
        {
            get
            {
                if (_devices == null)
                {
                    _devices = new Mock<cMobileDevices>(MockBehavior.Strict, GlobalTestVariables.AccountId);
                }
                return _devices.Object;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public cEmployeeCars EmployeeCars
        {
            get
            {
                if (_employeeCars == null)
                {
                    _employeeCars = new Mock<cEmployeeCars>(MockBehavior.Strict, GlobalTestVariables.AccountId, GlobalTestVariables.EmployeeId);
                }
                return _employeeCars.Object;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public cESRAssignments EsrAssignments
        {
            get
            {
                if (_esrAssignments == null)
                {
                    _esrAssignments = new Mock<cESRAssignments>(MockBehavior.Strict, GlobalTestVariables.AccountId, GlobalTestVariables.EmployeeId);
                }
                return _esrAssignments.Object;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public cAccountSubAccounts SubAccounts
        {
            get
            {
                if (_subAccounts == null)
                {
                    _subAccounts = new Mock<cAccountSubAccounts>(MockBehavior.Strict, GlobalTestVariables.AccountId, null);
                }
                return _subAccounts.Object;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public cESRTrusts NhsTrusts
        {
            get
            {
                if (_nhsTrusts == null)
                {
                    _nhsTrusts = new Mock<cESRTrusts>(MockBehavior.Strict, GlobalTestVariables.AccountId);
                }
                return _nhsTrusts.Object;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public Addresses Addresses
        {
            get
            {
                if (_addresses == null)
                {
                    _addresses = new Mock<Addresses>(MockBehavior.Strict, GlobalTestVariables.AccountId);
                }
                return _addresses.Object;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public AddressLabels AddressLabels
        {
            get
            {
                if (_addressLabels == null)
                {
                    _addressLabels = new Mock<AddressLabels>(MockBehavior.Strict, GlobalTestVariables.AccountId);
                }
                return _addressLabels.Object;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public EmployeeHomeAddresses HomeAddresses
        {
            get
            {
                if (_homeAddresses == null)
                {
                    _homeAddresses = new Mock<EmployeeHomeAddresses>(MockBehavior.Strict, GlobalTestVariables.AccountId, GlobalTestVariables.EmployeeId);
                }
                return _homeAddresses.Object;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public EmployeeWorkAddresses WorkAddresses
        {
            get
            {
                if (_workAddresses == null)
                {
                    _workAddresses = new Mock<EmployeeWorkAddresses>(MockBehavior.Strict, GlobalTestVariables.AccountId, GlobalTestVariables.EmployeeId);
                }
                return _workAddresses.Object;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public cClaims Claims
        {
            get
            {
                if (_claims == null)
                {
                    _claims = new Mock<cClaims>(MockBehavior.Loose, AccountId);
                }
                return _claims.Object;
            }
            set
            {
                throw new NotImplementedException();
            }
        }


        public IManageEnvelopes Envelopes
        {
            get { return _envelopes ?? (_envelopes = new TestEnvelopes()); }
            set
            {
                throw new NotImplementedException();
            }
        }

        public IManageReceipts Receipts
        {
            get { return _receipts ?? (_receipts = new TestReceipts(AccountId, EmployeeId)); }
            set
            {
                throw new NotImplementedException();
            }
        }

        public IManageExpenseValidation ExpenseValidation
        {
            get { return (_expenseValidation = new TestExpenseValidationManager(AccountId)); }
            set
            {
                throw new NotImplementedException();
            }
        }

        public cCustomEntities CustomEntities
        {
            get
            {
                if (_customEntities == null)
                {
                    _customEntities = new Mock<cCustomEntities>(MockBehavior.Strict, GlobalTestVariables.AccountId);
                }

                return _customEntities.Object;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public cEmailTemplates Emails
        {
            get
            {
                if (_emails == null)
                {
                    _emails = new Mock<cEmailTemplates>(MockBehavior.Strict, GlobalTestVariables.AccountId);
                }

                return _emails.Object;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public cFields Fields
        {
            get
            {
                if (_fields == null)
                {
                    _fields = new Mock<cFields>(MockBehavior.Default, AccountId, null);
                }

                return _fields.Object;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public ClaimSubmission ClaimSubmission
        {
            get
            {
                if (_claimSubmission == null)
                {
                    _claimSubmission = new Mock<ClaimSubmission>(MockBehavior.Strict, CurrentUser);
                }

                return _claimSubmission.Object;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public Organisations Organisations
        {
            get { return (this._organisations ?? (this._organisations = new Mock<Organisations>(MockBehavior.Strict, GlobalTestVariables.AccountId))).Object; }
            set { throw new NotImplementedException(); }
        }

        public cFilterRules FilterRules
        {
            get { return (this._filterrules ?? (this._filterrules = new Mock<cFilterRules>(MockBehavior.Strict, GlobalTestVariables.AccountId))).Object; }
            set { throw new NotImplementedException(); }
        }




        //**APIiser_Marker**//

        #endregion

        public IManageFunds Fund
        {
            get { return _funds = null; }
            set
            {
                throw new NotImplementedException();
            }
        }

        public IManagePayment Payment
        {
            get { return _payment = null; }
            set
            {
                throw new NotImplementedException();
            }
        }

        public Hotels Hotels
        {
            get { return _hotels = null; }
            set
            {
                throw new NotImplementedException();
            }
        }

        public Hotel Hotel
        {
            get { return _hotel = null; }
            set
            {
                throw new NotImplementedException();
            }
        }

        public BankAccounts BankAccounts { get; set; }

        /// <summary>
        /// Gets or sets an instance of the Favourites class.
        /// </summary>
        public Favourites Favourites { get; set; }

        /// <summary>
        /// Gets or sets an instance of the postcode anywhere class
        /// </summary>
        public PostcodeAnywhere PostcodeAnywhere { get; set; }

        /// <summary>
        /// Gets or sets the duty of care documents.
        /// </summary>
        public DutyOfCareDocuments DutyOfCareDocuments { get; set; }

        public cFloats Advances
        {
            get; set;
        }
    }
}
