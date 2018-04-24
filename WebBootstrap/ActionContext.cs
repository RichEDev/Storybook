namespace WebBootstrap
{
    using BusinessLogic.Identity;
    using SpendManagementLibrary;
    using Spend_Management;
    using Spend_Management.expenses.code;
    using Spend_Management.shared.code;

    /// <summary>
    /// A class to manage the creation of dependencies
    /// </summary>
    public class ActionContext : IActionContext
    {
        /// <summary>
        /// A private value of employee id
        /// </summary>
        private int _employeeId;

        /// <summary>
        /// A private instance of <see cref="cAccounts"/>
        /// </summary>
        private cAccounts _accounts;

        /// <summary>
        /// A private instance of <see cref="BankAccounts"/>
        /// </summary>
        private BankAccounts _bankAcounts;

        /// <summary>
        /// A private instance of <see cref="cCardStatements"/>
        /// </summary>
        private cCardStatements _cardStatements;

        /// <summary>
        /// A private instance of <see cref="cCategories"/>
        /// </summary>
        private cCategories _categories;

        /// <summary>
        /// A private instance of <see cref="cReasons"/>
        /// </summary>
        private cReasons _claimReasons;

        /// <summary>
        /// A private instance of <see cref="cClaims"/>
        /// </summary>
        private cClaims _claims;

        /// <summary>
        /// A private instance of <see cref="cCostcodes"/>
        /// </summary>
        private cCostcodes _costCodes;

        /// <summary>
        /// A private instance of <see cref="cCountries"/>
        /// </summary>
        private cCountries _countries;

        /// <summary>
        /// A private instance of <see cref="cCurrencies"/>
        /// </summary>
        private cCurrencies _currencies;

        /// <summary>
        /// A private instance of <see cref="ICurrentUser"/>
        /// </summary>
        private ICurrentUser _currentUser;

        /// <summary>
        /// A private instance of <see cref="cDepartments"/>
        /// </summary>
        private cDepartments _departments;

        /// <summary>
        /// A private instance of <see cref="cMobileDevices"/>
        /// </summary>
        private cMobileDevices _devices;

        /// <summary>
        /// A private instance of <see cref="EmployeeCars"/>
        /// </summary>
        private cEmployeeCars _employeeCars;

        /// <summary>
        /// A private instance of <see cref="cEmployees"/>
        /// </summary>
        private cEmployees _employees;

        /// <summary>
        /// A private instance of <see cref="cESRAssignments"/>
        /// </summary>
        private cESRAssignments _esrAssignments;

        /// <summary>
        /// A private instance of <see cref="cExpenseItems"/>
        /// </summary>
        private cExpenseItems _expenseItems;

        /// <summary>
        /// A private instance of <see cref="cFields"/>
        /// </summary>
        private cFields _fields;

        /// <summary>
        /// A private instance of <see cref="cFilterRules"/>
        /// </summary>
        private cFilterRules _filterrules;

        /// <summary>
        /// A private instance of <see cref="FlagManagement"/>
        /// </summary>
        private FlagManagement _flagManagement;

        /// <summary>
        /// A private instance of <see cref="cGlobalCurrencies"/>
        /// </summary>
        private cGlobalCurrencies _globalCurrencies;

        /// <summary>
        /// A private instance of <see cref="cMileagecats"/>
        /// </summary>
        private cMileagecats _mileagecats;

        /// <summary>
        /// A private instance of <see cref="cMisc"/>
        /// </summary>
        private cMisc _misc;

        /// <summary>
        /// A private instance of <see cref="cAccountProperties"/>
        /// </summary>
        private cAccountProperties _properties;

        /// <summary>
        /// A private instance of <see cref="cAccountSubAccounts"/>
        /// </summary>
        private cAccountSubAccounts _subAccounts;

        /// <summary>
        /// A private instance of <see cref="cSubcats"/>
        /// </summary>
        private cSubcats _subCategories;

        /// <summary>
        /// A private instance of <see cref="cTables"/>
        /// </summary>
        private cTables _tables;

        /// <summary>
        /// A private instance of <see cref="cUserdefinedFields"/>
        /// </summary>
        private cUserdefinedFields _userdefinedFields;

        /// <summary>
        /// A private instance of <see cref="cEmployeeCorporateCards"/>
        /// </summary>
        private cEmployeeCorporateCards _employeeCorporateCards;

        /// <summary>
        /// A private instance of <see cref="cFloats"/>
        /// </summary>
        private cFloats _floats;

        /// <summary>
        /// A private instance of <see cref="cAllowances"/>
        /// </summary>
        private cAllowances _allowances;

        /// <summary>
        /// Initialize a new instance of <see cref="ActionContext"/>
        /// </summary>
        public ActionContext(IIdentityProvider identityProvider)
        {
            UserIdentity user = identityProvider.GetUserIdentity();
            this.User = user;
            this.AccountId = user.AccountId;
            this.EmployeeId = user.EmployeeId;
            this.SubAccountId = user.SubAccountId < 0 ? this.CurrentUser.CurrentSubAccountId : user.SubAccountId;
        }


        /// <summary>
        /// Get an instance of <see cref="Spend_Management.ICurrentUser"/>
        /// </summary>
        public ICurrentUser CurrentUser => this._currentUser ?? (this._currentUser = cMisc.GetCurrentUser());


        /// <summary>
        /// Get an instance of <see cref="UserIdentity"/> used for creating the object.
        /// </summary>
        public UserIdentity User { get; }

        /// <summary>
        /// Gets or sets the Current Employee ID
        /// </summary>
        public int EmployeeId
        {
            get
            {
                return this._employeeId;
            }
            set
            {
                if (this._employeeId != value)
                {
                    this._esrAssignments = null;
                    this._employeeCars = null;
                    this._bankAcounts = null;
                }

                this._employeeId = value;
            }
        }

        /// <summary>
        /// Gets the current Account ID
        /// </summary>
        public int AccountId { get;  }

        /// <summary>
        /// Gets the current Sub Account ID
        /// </summary>
        public int? SubAccountId { get; }


        /// <summary>
        /// Get an instance of <see cref="cEmployees"/>
        /// </summary>
        public cEmployees Employees => this._employees ?? (this._employees = new cEmployees(this.AccountId));

        /// <summary>
        /// Get an instance of <see cref="cMileagecats"/>
        /// </summary>
        public cMileagecats MileageCategories => this._mileagecats ?? (this._mileagecats = new cMileagecats(this.AccountId));

        /// <summary>
        /// Gets an instance of <see cref="cCountries"/>
        /// </summary>
        public cCountries Countries => this._countries ?? (this._countries = new cCountries(this.AccountId, null));

        /// <summary>
        /// Gets an instance of <see cref="cCurrencies"/>
        /// </summary>
        public cCurrencies Currencies => this._currencies ?? (this._currencies = new cCurrencies(this.AccountId, null));

        /// <summary>
        /// Gets an instance of <see cref="cGlobalCurrencies"/>
        /// </summary>
        public cGlobalCurrencies GlobalCurrencies => this._globalCurrencies ?? (this._globalCurrencies = new cGlobalCurrencies());

        /// <summary>
        /// Gets an instance of <see cref="cSubcats"/>
        /// </summary>
        public cSubcats SubCategories => this._subCategories ?? (this._subCategories = new cSubcats(this.AccountId));


        /// <summary>
        /// Gets an instance of <see cref="cCategories"/>
        /// </summary>
        public cCategories Categories => this._categories ?? (this._categories = new cCategories(this.AccountId));

        /// <summary>
        /// Gets an instance of <see cref="cReasons"/>
        /// </summary>
        public cReasons ClaimReasons => this._claimReasons ?? (this._claimReasons = new cReasons(this.AccountId));

        /// <summary>
        /// Gets an instance of <see cref="cCostcodes"/>
        /// </summary>
        public cCostcodes CostCodes => this._costCodes ?? (this._costCodes = new cCostcodes(this.AccountId));

        /// <summary>
        /// Gets an instance of <see cref="cDepartments"/>
        /// </summary>
        public cDepartments Departments => this._departments ?? (this._departments = new cDepartments(this.AccountId));

        /// <summary>
        /// Gets an instance of <see cref="cUserdefinedFields"/>
        /// </summary>
        public cUserdefinedFields UserDefinedFields => this._userdefinedFields ?? (this._userdefinedFields = new cUserdefinedFields(this.AccountId));

        /// <summary>
        /// Gets an instance of <see cref="cTables"/>
        /// </summary>
        public cTables Tables => this._tables ?? (this._tables = new cTables(this.AccountId));

        /// <summary>
        /// Gets an instance of <see cref="cMobileDevices"/>
        /// </summary>
        public cMobileDevices MobileDevices => this._devices ?? (this._devices = new cMobileDevices(this.AccountId));

        /// <summary>
        /// Gets an instance of <see cref="cEmployeeCars"/>
        /// </summary>
        public cEmployeeCars EmployeeCars 
            => this._employeeCars ?? (this._employeeCars = new cEmployeeCars(this.AccountId, this.EmployeeId));

        /// <summary>
        /// Gets an instance of <see cref="cESRAssignments"/>
        /// </summary>
        public cESRAssignments EsrAssignments => this._esrAssignments ??
                                                 (this._esrAssignments = new cESRAssignments(this.AccountId, this.EmployeeId));

        /// <summary>
        /// Gets an instance of <see cref="cAccountSubAccounts"/>
        /// </summary>
        public cAccountSubAccounts SubAccounts => this._subAccounts ?? (this._subAccounts = new cAccountSubAccounts(this.AccountId));


        /// <summary>
        /// Gets an instance of <see cref="cClaims"/>
        /// </summary>
        public cClaims Claims => this._claims ?? (this._claims = new cClaims(this.AccountId));


        /// <summary>
        /// Gets an instance of <see cref="cFields"/>
        /// </summary>
        public cFields Fields => this._fields ?? (this._fields = new cFields(this.AccountId));

        /// <summary>
        /// Gets an instance of <see cref="cAccounts"/>
        /// </summary>
        public cAccounts Accounts => this._accounts ?? (this._accounts = new cAccounts());

        /// <summary>
        /// Gets an instance of <see cref="cExpenseItems"/>
        /// </summary>
        public cExpenseItems ExpenseItems => this._expenseItems ?? (this._expenseItems = new cExpenseItems(this.AccountId));


        /// <summary>
        /// Gets an instance of <see cref="cFilterRules"/>
        /// </summary>
        public cFilterRules FilterRules => this._filterrules ?? (this._filterrules = new cFilterRules(this.AccountId));

        /// <summary>
        /// Gets an instance of <see cref="IActionContext.FlagManagement"/>
        /// </summary>
        public FlagManagement FlagManagement => this._flagManagement ?? (this._flagManagement = new FlagManagement(this.AccountId));


        /// <summary>
        ///  Gets the <see cref="IActionContext.BankAccounts"/> class
        /// </summary>
        public BankAccounts BankAccounts => this._bankAcounts ?? (this._bankAcounts = new BankAccounts(this.AccountId, this.EmployeeId));

        /// <summary>
        ///     Gets or sets the an instance of the cCardStatements class.
        /// </summary>
        public cCardStatements CardStatements => this._cardStatements ?? (this._cardStatements = new cCardStatements(this.AccountId));

        /// <summary>
        /// Gets an instance of <see cref="cAccountProperties"/>
        /// </summary>
        public cAccountProperties Properties => this._properties ??
                                                (this._properties = this.SubAccounts.getFirstSubAccount().SubAccountProperties);

        /// <summary>
        /// Gets an instance of <see cref="cMisc"/>
        /// </summary>
        public cMisc Misc => this._misc ?? (this._misc = new cMisc(this.AccountId));

        /// <summary>
        /// Gets an instance of <see cref="cEmployeeCorporateCards"/>
        /// </summary>
        public cEmployeeCorporateCards EmployeeCorporateCards => this._employeeCorporateCards ??(this._employeeCorporateCards = new cEmployeeCorporateCards(this.AccountId));

        /// <summary>
        /// Gets an instance of <see cref="cFloats"/>
        /// </summary>
        public cFloats Floats => this._floats??(this._floats = new cFloats(this.AccountId));

        /// <summary>
        /// Gets an instance of <see cref="cAllowances"/>
        /// </summary>
        public cAllowances Allowances => this._allowances ?? (this._allowances = new cAllowances(this.AccountId));
    }
}