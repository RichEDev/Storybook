namespace SpendManagementApi.Common
{
    using Interfaces;
    using Spend_Management;
    using Spend_Management.expenses.code;
    using Spend_Management.expenses.code.Claims;
    using SpendManagementLibrary;
    using SpendManagementLibrary.Addresses;
    using SpendManagementLibrary.Cards;
    using SpendManagementLibrary.Employees;
    using SpendManagementLibrary.Employees.DutyOfCare;
    using SpendManagementLibrary.Expedite;
    using SpendManagementLibrary.Helpers;
    using SpendManagementLibrary.Holidays;
    using SpendManagementLibrary.Interfaces.Expedite;
    using GeneralOptions = SpendManagementLibrary.GeneralOptions.GeneralOptions;
    using SpendManagementLibrary.Hotels;

    using Spend_Management.shared.code;

 

    internal class ActionContext : IActionContext
    {
        private cEmployees _employees;

        private cMileagecats _mileagecats;

        private cLocales _locales;

        private cCountries _countries;

        private cEmployeeCorporateCards _employeeCorporateCards;

        private cCurrencies _currencies;

        private cMonthlyCurrencies _monthlyCurrencies;

        private cRangeCurrencies _rangeCurrencies;

        private cGroups _groups;

        private CardProviders _cardProviders;

        private cGlobalCurrencies _globalCurrencies;

        private cGlobalCountries _globalCountries;

        private cSubcats _subCategories;

        private cCategories _categories;

        private cItemRoles _itemRoles;

        private cAccessRoles _accessRoles;

        private cAllowances _allowances;

        private cBudgetholders _budgetHolders;

        private cReasons _claimReasons;

        private cCostcodes _costCodes;

        private cDepartments _departments;

        private cTeams _teams;

        private cP11dcats _p11Dcats;

        private cPoolCars _poolCars;

        private cProjectCodes _projectCodes;

        private cUserdefinedFields _userdefinedFields;

        private cTables _tables;

        private cMobileDevices _devices;

        private cEmployeeCars _employeeCars;

        private cESRAssignments _esrAssignments;

        private cAccountSubAccounts _subAccounts;

        private cESRTrusts _nhsTrusts;

        private Addresses _addresses;

        private AddressLabels _addressLabels;

        private EmployeeHomeAddresses _homeAddresses;

        private EmployeeWorkAddresses _workAddresses;

        private cClaims _claims;

        private ClaimSubmission _claimSubmission;

        private IManageEnvelopes _envelopes;

        private IManageReceipts _receipts;

        private IManageExpenseValidation _expenseValidation;

        private cFields _fields;

        private cAccounts _accounts;

        private cCustomEntities _customEntities;

        private cEmailTemplates _emails;

        private Organisations _organisations;

        private cExpenseItems _expenseItems;

        private cFilterRules _filterrules;

        private FlagManagement _flagManagement;

        private GeneralOptions _generalOptions;

        private IManageFunds _funds;

        private IManagePayment _payments;

        private Hotels _hotels;

        private Hotel _hotel;

        private BankAccounts _bankAcounts;

        private Favourites _favourites;

        private PostcodeAnywhere _postcodeAnywhere;

        private cFloats _Advances;

        private DutyOfCareDocuments _dutyOfCareDocuments;

        /// <summary>
        /// An instance of cCardStatements
        /// </summary>
        private cCardStatements _cardStatements;

        /// <summary>
        /// The an instance of cCardTemplates
        /// </summary>
        private cCardTemplates _cardTemplates;

        /// <summary>
        /// An instance of cCorporateCards
        /// </summary>
        private CorporateCards _corporateCards;

        /// <summary>
        /// An instance of Holidays.
        /// </summary>
        private Holidays _holidays;

        /// <summary>
        /// An instance of cMisc
        /// </summary>
        private cMisc _misc;

        //**APIiser_Marker**//

        internal ActionContext(ICurrentUser currentUser)
            : this(currentUser.AccountID, currentUser.EmployeeID, currentUser.CurrentSubAccountId)
        {
            CurrentUser = currentUser;
        }

        internal ActionContext(int accountId, int employeeId, int subAccountId)
        {
            EmployeeId = employeeId;
            AccountId = accountId;
            SubAccountId = subAccountId;
        }

        public ICurrentUser CurrentUser { get; set; }

        public int EmployeeId { get; set; }

        public int AccountId { get; set; }

        public int? SubAccountId { get; set; }


        public cEmployees Employees
        {
            get
            {
                return (_employees = new cEmployees(AccountId));
            }
            set
            {
                _employees = value;
            }
        }

        public cMileagecats MileageCategories
        {
            get
            {
                return this._mileagecats ?? (this._mileagecats = new cMileagecats(this.AccountId));
            }
            set
            {
                _mileagecats = value;
            }
        }

        public cLocales Locales
        {
            get
            {
                return this._locales ?? (this._locales = new cLocales());
            }
            set
            {
                _locales = value;
            }
        }


        public cCountries Countries
        {
            get
            {
                return this._countries ?? (this._countries = new cCountries(this.AccountId, this.SubAccountId));
            }
            set
            {
                _countries = value;
            }
        }

        public cCurrencies Currencies
        {
            get
            {
                return this._currencies ?? (this._currencies = new cCurrencies(this.AccountId, this.SubAccountId));
            }
            set
            {
                _currencies = value;
            }
        }

        public cMonthlyCurrencies MonthlyCurrencies
        {
            get
            {
                return this._monthlyCurrencies
                       ?? (this._monthlyCurrencies = new cMonthlyCurrencies(this.AccountId, null));
            }
            set
            {
                _monthlyCurrencies = value;
            }
        }

        public cRangeCurrencies RangeCurrencies
        {
            get
            {
                return this._rangeCurrencies ?? (this._rangeCurrencies = new cRangeCurrencies(this.AccountId, null));
            }
            set
            {
                _rangeCurrencies = value;
            }
        }

        public cEmployeeCorporateCards EmployeeCorporateCards
        {
            get
            {
                return this._employeeCorporateCards ?? (this._employeeCorporateCards = new cEmployeeCorporateCards(this.AccountId));
            }
            set
            {
                _employeeCorporateCards = value;
            }
        }

        public cGroups SignoffGroups
        {
            get
            {
                return this._groups ?? (this._groups = new cGroups(this.AccountId));
            }
            set
            {
                _groups = value;
            }
        }

        public CardProviders CardProviders
        {
            get
            {
                return this._cardProviders ?? (this._cardProviders = new CardProviders());
            }
            set
            {
                _cardProviders = value;
            }
        }

        public cAccessRoles AccessRoles
        {
            get
            {
                return _accessRoles
                       ?? (_accessRoles = new cAccessRoles(AccountId, cAccounts.getConnectionString(AccountId)));
            }
            set
            {
                _accessRoles = value;
            }
        }

        public cAllowances Allowances
        {
            get
            {
                return _allowances ?? (_allowances = new cAllowances(AccountId));
            }
            set
            {
                _allowances = value;
            }
        }

        public cGlobalCurrencies GlobalCurrencies
        {
            get
            {
                return this._globalCurrencies ?? (this._globalCurrencies = new cGlobalCurrencies());
            }
            set
            {
                _globalCurrencies = value;
            }
        }


        public cGlobalCountries GlobalCountries
        {
            get
            {
                return this._globalCountries ?? (this._globalCountries = new cGlobalCountries());
            }
            set
            {
                _globalCountries = value;
            }
        }

        public cSubcats SubCategories
        {
            get
            {
                return this._subCategories ?? (this._subCategories = new cSubcats(this.AccountId));
            }
            set
            {
                _subCategories = value;
            }
        }


        public cCategories Categories
        {
            get
            {
                _categories = _categories ?? new cCategories(AccountId);
                return _categories;
            }
            set
            {
                _categories = value;
            }
        }

        public cItemRoles ItemRoles
        {
            get
            {
                _itemRoles = _itemRoles ?? new cItemRoles(AccountId);
                return _itemRoles;
            }
            set
            {
                _itemRoles = value;
            }
        }

        public cBudgetholders BudgetHolders
        {
            get
            {
                return _budgetHolders ?? (_budgetHolders = new cBudgetholders(AccountId));
            }
            set
            {
                _budgetHolders = value;
            }
        }

        public cReasons ClaimReasons
        {
            get
            {
                return _claimReasons ?? (_claimReasons = new cReasons(AccountId));
            }
            set
            {
                _claimReasons = value;
            }
        }

        public cCostcodes CostCodes
        {
            get
            {
                return _costCodes ?? (_costCodes = new cCostcodes(AccountId));
            }
            set
            {
                _costCodes = value;
            }
        }

        public cDepartments Departments
        {
            get
            {
                return _departments ?? (_departments = new cDepartments(AccountId));
            }
            set
            {
                _departments = value;
            }
        }

        public cTeams Teams
        {
            get
            {
                return _teams ?? (_teams = new cTeams(AccountId, null));
            }
            set
            {
                _teams = value;
            }
        }

        public cP11dcats P11DCats
        {
            get
            {
                return _p11Dcats ?? (_p11Dcats = new cP11dcats(AccountId));
            }
            set
            {
                _p11Dcats = value;
            }
        }

        public cPoolCars PoolCars
        {
            get
            {
                return _poolCars ?? (_poolCars = new cPoolCars(AccountId));
            }
            set
            {
                _poolCars = value;
            }
        }

        public cProjectCodes ProjectCodes
        {
            get
            {
                return _projectCodes ?? (_projectCodes = new cProjectCodes(AccountId));
            }
            set
            {
                _projectCodes = value;
            }
        }

        public cUserdefinedFields UserDefinedFields
        {
            get
            {
                return _userdefinedFields ?? (_userdefinedFields = new cUserdefinedFields(AccountId));
            }
            set
            {
                _userdefinedFields = value;
            }
        }

        public cTables Tables
        {
            get
            {
                return _tables ?? (_tables = new cTables(AccountId));
            }
            set
            {
                _tables = value;
            }
        }

        public cMobileDevices MobileDevices
        {
            get
            {
                return _devices ?? (_devices = new cMobileDevices(AccountId));
            }
            set
            {
                _devices = value;
            }
        }

        public cEmployeeCars EmployeeCars
        {
            get
            {
                _employeeCars = new cEmployeeCars(AccountId, EmployeeId);
                return _employeeCars;
            }
            set
            {
                _employeeCars = value;
            }
        }

        public cESRAssignments EsrAssignments
        {
            get
            {
                _esrAssignments = new cESRAssignments(AccountId, EmployeeId);
                return _esrAssignments;
            }
            set
            {
                _esrAssignments = value;
            }
        }

        public cAccountSubAccounts SubAccounts
        {
            get
            {
                _subAccounts = new cAccountSubAccounts(AccountId);
                return _subAccounts;
            }
            set
            {
                _subAccounts = value;
            }
        }

        public cESRTrusts NhsTrusts
        {
            get
            {
                _nhsTrusts = new cESRTrusts(AccountId);
                return _nhsTrusts;
            }
            set
            {
                _nhsTrusts = value;
            }
        }

        public Addresses Addresses
        {
            get
            {
                _addresses = new Addresses(AccountId);
                return _addresses;
            }
            set
            {
                _addresses = value;
            }
        }

        public AddressLabels AddressLabels
        {
            get
            {
                _addressLabels = new AddressLabels(AccountId);
                return _addressLabels;
            }
            set
            {
                _addressLabels = value;
            }
        }

        public EmployeeHomeAddresses HomeAddresses
        {
            get
            {
                _homeAddresses = new EmployeeHomeAddresses(AccountId, EmployeeId);
                return _homeAddresses;
            }
            set
            {
                _homeAddresses = value;
            }
        }

        public EmployeeWorkAddresses WorkAddresses
        {
            get
            {
                _workAddresses = new EmployeeWorkAddresses(AccountId, EmployeeId);
                return _workAddresses;
            }
            set
            {
                _workAddresses = value;
            }
        }

        public cClaims Claims
        {
            get
            {
                return (_claims = new cClaims(AccountId));
            }
            set
            {
                _claims = value;
            }
        }

        public IManageEnvelopes Envelopes
        {
            get
            {
                return _envelopes ?? (_envelopes = new Envelopes());
            }
            set
            {
                _envelopes = value;
            }
        }

        public IManageReceipts Receipts
        {
            get
            {
                return _receipts ?? (_receipts = new SpendManagementLibrary.Expedite.Receipts(AccountId, EmployeeId));
            }
            set
            {
                _receipts = value;
            }
        }

        public IManageExpenseValidation ExpenseValidation
        {
            get
            {
                return _expenseValidation
                       ?? (_expenseValidation = new SpendManagementLibrary.Expedite.ExpenseValidationManager(AccountId));
            }
            set
            {
                _expenseValidation = value;
            }
        }

        public cCustomEntities CustomEntities
        {
            get
            {
                _customEntities = new cCustomEntities(CurrentUser);
                return _customEntities;
            }
            set
            {
                _customEntities = value;
            }
        }

        public cEmailTemplates Emails
        {
            get
            {
                return
                    (_emails =
                     new cEmailTemplates(
                         AccountId,
                         CurrentUser.EmployeeID,
                         string.Empty,
                         0,
                         CurrentUser.CurrentActiveModule));
            }
            set
            {
                _emails = value;
            }
        }

        public cFields Fields
        {
            get
            {
                return (_fields = new cFields(AccountId));
            }
            set
            {
                _fields = value;
            }
        }

        public cAccounts Accounts
        {
            get
            {
                return _accounts ?? (_accounts = new cAccounts());
            }
            set
            {
                _accounts = value;
            }
        }

        public ClaimSubmission ClaimSubmission
        {
            get
            {
                return _claimSubmission ?? (_claimSubmission = new ClaimSubmission(CurrentUser));
            }
            set
            {
                _claimSubmission = value;
            }
        }


        public Organisations Organisations
        {
            get
            {
                return this._organisations ?? (this._organisations = new Organisations());
            }
            set
            {
                this._organisations = value;
            }
        }

        public cExpenseItems ExpenseItems
        {
            get
            {
                return this._expenseItems ?? (this._expenseItems = new cExpenseItems(AccountId));
            }
            set
            {
                this._expenseItems = value;
            }
        }

        /// <summary>
        /// Gets or sets the filter rules.
        /// </summary>
        public cFilterRules FilterRules
        {
            get
            {
                return this._filterrules ?? (this._filterrules = new cFilterRules(AccountId));
            }
            set
            {
                this._filterrules = value;
            }
        }

        /// <summary>
        /// Gets or sets the flag management.
        /// </summary>
        public FlagManagement FlagManagement
        {
            get
            {
                return this._flagManagement ?? (this._flagManagement = new FlagManagement(AccountId));
            }
            set
            {
                this._flagManagement = value;
            }
        }

        /// <summary>
        /// Gets or sets the general options.
        /// </summary>
        public GeneralOptions GeneralOptions
        {
            get
            {
                return _generalOptions ?? (_generalOptions = new GeneralOptions(AccountId));
            }
            set
            {
                _generalOptions = value;
            }
        }

        public IManageFunds Fund
        {
            get
            {
                return _funds ?? (_funds = new SpendManagementLibrary.Expedite.Funds(AccountId));
            }
            set
            {
                _funds = value;
            }
        }


        /// <summary>
        /// Property to set the Payment Action context
        /// </summary>
        public IManagePayment Payment
        {
            get
            {
                return _payments ?? (_payments = new SpendManagementLibrary.Expedite.PaymentService(AccountId));
            }
            set
            {
                _payments = value;
            }
        }

        public Hotels Hotels
        {
            get
            {
                return _hotels ?? (_hotels = new Hotels(CurrentUser));
            }
            set
            {
                _hotels = value;
            }
        }

        public Hotel Hotel
        {
            get
            {
                return _hotel ?? (_hotel = new Hotel());
            }
            set
            {
                _hotel = value;
            }
        }

        public BankAccounts BankAccounts
        {
            get
            {
                return _bankAcounts ?? (_bankAcounts = new BankAccounts(AccountId, EmployeeId));
            }
            set
            {
                _bankAcounts = value;
            }
        }

        /// <summary>
        /// Gets or sets an instance of the Favourites class.
        /// </summary>
        public Favourites Favourites
        {
            get
            {
                return _favourites ?? (_favourites = new Favourites(AccountId));
            }
            set
            {
                this._favourites = value;
            }
        }

        /// <summary>
        /// Gets or sets an instance of the postcode anywhere class
        /// </summary>
        public PostcodeAnywhere PostcodeAnywhere
        {
            get
            {
                return this._postcodeAnywhere ?? (this._postcodeAnywhere = new PostcodeAnywhere(AccountId));
            }
            set
            {
                this._postcodeAnywhere = value;
            }
        }

        /// <summary>
        /// Gets or sets the duty of care documents.
        /// </summary>
        public DutyOfCareDocuments DutyOfCareDocuments
        {
            get
            {
                return this._dutyOfCareDocuments ?? (this._dutyOfCareDocuments = new DutyOfCareDocuments());
            }
            set
            {
                this._dutyOfCareDocuments = value;
            }
        }

        /// <summary>
        /// Gets or sets the cMisc class
        /// </summary>
        public cMisc Misc
        {
            get
            {
                return this._misc ?? (this._misc = new cMisc(this.AccountId));
            }
            set
            {
                this._misc = value;
            }
        }

        /// <summary>
        /// Gets or an instances of the cFloats class.
        /// </summary>
        public cFloats Advances
        {
            get
            {
                return this._Advances ?? (this._Advances = new cFloats(AccountId));
            }
            set
            {
                this._Advances = value;
            }
        }

        /// <summary>
        /// Gets or sets the an instance of the cCardStatements class.
        /// </summary>
        public cCardStatements CardStatements
        {
            get
            {
                return this._cardStatements ?? (this._cardStatements = new cCardStatements(AccountId));
            }
            set
            {
                this._cardStatements = value;
            }
        }

        /// <summary>
        /// Gets or sets the an instance of the cCardTemplates class.
        /// </summary>
        public cCardTemplates CardTemplates
        {
            get
            {
                return this._cardTemplates ?? (this._cardTemplates = new cCardTemplates(AccountId));
            }
            set
            {
                this._cardTemplates = value;
            }
        }

        /// <summary>
        /// Gets or sets the CorporateCards class
        /// </summary>
        public CorporateCards CorporateCards
        {
            get
            {
                return this._corporateCards ?? (this._corporateCards = new CorporateCards(AccountId));
            }
            set
            {
                this._corporateCards = value;
            }
        }

        /// <summary>
        /// Gets or sets an instance of Holidays.
        /// </summary>
        public Holidays Holidays
        {
            get
            {
                if (this._holidays != null)
                {
                    return this._holidays;
                }
                else
                {
                    var connection = new DatabaseConnection(cAccounts.getConnectionString(this.CurrentUser.AccountID));
                    return this._holidays = new Holidays(connection);
                }
              
            }
            set
            {
                this._holidays = value;
            }
        }
    }
}
