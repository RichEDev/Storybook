using SpendManagementLibrary.Hotels;
using Spend_Management.expenses.code.Claims;

namespace SpendManagementApi.Interfaces
{
    using Spend_Management;
    using Spend_Management.expenses.code;
    using SpendManagementLibrary;
    using SpendManagementLibrary.Addresses;
    using SpendManagementLibrary.Cards;
    using SpendManagementLibrary.Employees;
    using SpendManagementLibrary.Employees.DutyOfCare;
    using SpendManagementLibrary.Holidays;
    using SpendManagementLibrary.Interfaces;
    using SpendManagementLibrary.Interfaces.Expedite;
    using SpendManagementLibrary.MobileAppReview;

    using Spend_Management.shared.code;

    public interface IActionContext
    {
        cEmployees Employees { get; set; }

        cMileagecats MileageCategories { get; set; }

        cLocales Locales { get; set; }

        cCountries Countries { get; set; }

        cCurrencies Currencies { get; set; }

        cMonthlyCurrencies MonthlyCurrencies { get; set; }

        cRangeCurrencies RangeCurrencies { get; set; }

        cEmployeeCorporateCards EmployeeCorporateCards { get; set; }

        cGroups SignoffGroups { get; set; }

        CardProviders CardProviders { get; set; }

        cAccessRoles AccessRoles { get; set; }

        cAllowances Allowances { get; set; }

        cBudgetholders BudgetHolders { get; set; }

        cReasons ClaimReasons { get; set; }

        cCostcodes CostCodes { get; set; }

        cDepartments Departments { get; set; }

        cTeams Teams { get; set; }

        cGlobalCurrencies GlobalCurrencies { get; set; }

        cGlobalCountries GlobalCountries { get; set; }

        cSubcats SubCategories { get; set; }

        cPoolCars PoolCars { get; set; }

        cProjectCodes ProjectCodes { get; set; }

        cCategories Categories { get; set; }

        cItemRoles ItemRoles { get; set; }

        cUserdefinedFields UserDefinedFields { get; set; }

        cTables Tables { get; set; }

        cMobileDevices MobileDevices { get; set; }

        cEmployeeCars EmployeeCars { get; set; }

        cESRAssignments EsrAssignments { get; set; }

        cAccountSubAccounts SubAccounts { get; set; }

        cESRTrusts NhsTrusts { get; set; }

        Addresses Addresses { get; set; }

        AddressLabels AddressLabels { get; set; }

        EmployeeHomeAddresses HomeAddresses { get; set; }

        EmployeeWorkAddresses WorkAddresses { get; set; }

        cClaims Claims { get; set; }

        ClaimSubmission ClaimSubmission { get; set; }

        IManageEnvelopes Envelopes { get; set; }

        IManageReceipts Receipts { get; set; }

        IManageExpenseValidation ExpenseValidation { get; set; }

        cCustomEntities CustomEntities { get; set; }

        cEmailTemplates Emails { get; set; }

        cFields Fields { get; set; }

        cAccounts Accounts { get; set; }

        Organisations Organisations { get; set; }

        cExpenseItems ExpenseItems { get; set; }

        cFilterRules FilterRules { get; set; }

        FlagManagement FlagManagement { get; set; }

        //**APIiser_Marker**//

        int? SubAccountId { get; set; }

        int AccountId { get; set; }

        int EmployeeId { get; set; }

        SpendManagementLibrary.GeneralOptions.GeneralOptions GeneralOptions { get; set; }
       
        IManageFunds Fund { get; set; }

        /// <summary>
        /// Interface for Payment Service
        /// </summary>
        IManagePayment Payment { get; set; }

        /// <summary>
        /// /THe Hotels class
        /// </summary>
        Hotels Hotels { get; set; }

        /// <summary>
        /// The Hotel class
        /// </summary>
        Hotel Hotel { get; set; }

        /// <summary>
        /// The Bank Accounts class
        /// </summary>
        BankAccounts BankAccounts { get; set; }

        /// <summary>
        /// Gets or sets an instance of the Favourites class.
        /// </summary>
        Favourites Favourites { get; set; }

        /// <summary>
        /// Gets or sets an instance of the postcode anywhere class
        /// </summary>
        PostcodeAnywhere PostcodeAnywhere { get; set; }

        /// <summary>
        /// Gets or sets the duty of care documents.
        /// </summary>
        DutyOfCareDocuments DutyOfCareDocuments { get; set; }

        /// <summary>
        /// Gets or sets the cFloat class
        /// </summary>
        cFloats Advances { get; set; }

        /// <summary>
        /// Gets or sets the cCardStatements class.
        /// </summary>
        cCardStatements CardStatements { get; set; }

        /// <summary>
        /// Gets or sets an instance of cCardTemplates
        /// </summary>
        cCardTemplates CardTemplates { get; set; }

        /// <summary>
        /// Gets or sets an instance of CorporateCards
        /// </summary>
        CorporateCards CorporateCards { get; set; }

        /// <summary>
        /// Gets or sets an instance of Holidays.
        /// </summary>
        Holidays Holidays { get; set; }

        /// <summary>
        /// Gets or sets an instance of cMisc
        /// </summary>
        cMisc Misc { get; set; }

        /// <summary>
        /// Gets an instance of <see cref="EmployeeAppReviewPreference"/>
        /// </summary>
        EmployeeAppReviewPreference EmployeeAppReviewPreference { get;  }
    }    
}
