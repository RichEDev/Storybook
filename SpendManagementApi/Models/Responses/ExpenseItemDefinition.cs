namespace SpendManagementApi.Models.Responses
{
    using Interfaces;
    using Common;
    using System.Collections.Generic;

    using SpendManagementApi.Models.Types.Employees;

    using Types;
    using BankAccount = SpendManagementApi.Models.Types.Employees.BankAccount;

    /// <summary>
    /// A class to help build the expense item form
    /// </summary>
    public class ExpenseItemDefinition : ApiResponse
    {
        /// <summary>
        /// The details of the expense item
        /// </summary>
        public ExpenseItem ExpenseItem { get; set; }

        /// <summary>
        /// The field settings for Organisations
        /// </summary>
        public IFieldSetting OrganisationSettings { get; set; }

        /// <summary>
        /// The field settings for Reason
        /// </summary>
        public IFieldSetting ReasonSettings { get; set; }

        /// <summary>
        /// The field settings for Country
        /// </summary>
        public IFieldSetting CountrySettings { get; set; }

        /// <summary>
        /// The field settings for Currency
        /// </summary>
        public IFieldSetting CurrencySettings { get; set; }

        /// <summary>
        /// The field settings for OtherDetails
        /// </summary>
        public IFieldSetting OtherDetailsSettings { get; set; }

        /// <summary>
        /// The field settings for department
        /// </summary>
        public IFieldSetting DepartmentSettings { get; set; }

        /// <summary>
        /// The field settings for cost code
        /// </summary>
        public IFieldSetting CostcodeSettings { get; set; }

        /// <summary>
        /// The field settings for project code
        /// </summary>
        public IFieldSetting ProjectcodeSettings { get; set; }

        /// <summary>
        /// Gets or sets the default cost center breakdown
        /// </summary>
        public IList<Types.Employees.CostCentreBreakdown> CostCenterBreakdowns { get; set; }

        /// <summary>
        /// Gets or sets the default cost center breakdown with label data
        /// </summary>
        public IList<CostCodeBreakdownWithLabelData> CostCenterEmployeeDefaultBreakdownWithLabelData { get; set; }

        /// <summary>
        /// The field settings for ToSettings
        /// </summary>
        public IFieldSetting ToSettings { get; set; }

        /// <summary>
        /// The field settings for FromSettings
        /// </summary>
        public IFieldSetting FromSettings { get; set; }

        /// <summary>
        /// The primary currency Id.
        /// </summary>
        public int PrimaryCurrencyId { get; set; }

        /// <summary>
        /// A list of active Currencies
        /// </summary>
        public IList<Currency> CurrencyList { get; set; }

        /// <summary>
        /// The primary country Id.
        /// </summary>
        public int PrimaryCountryId { get; set; }

        /// <summary>
        /// A list of active countries
        /// </summary>
        public IList<Country> CountryList { get; set; }

        /// <summary>
        /// A list of claim reasons
        /// </summary>
        public IList<ClaimReason> ClaimReasons { get; set; }

        /// <summary>
        /// Whether the exchange rate is read-only on an expense item.
        /// </summary>
        public bool ExchangeRateReadOnly { get; set; }

        /// <summary>
        /// Gets or sets the employee's bank accounts.
        /// </summary>
        public IList<BankAccount> EmployeeBankAccounts { get; set; }

        /// <summary>
        /// Gets or sets the post code anywhere countries that can be selected in the address search
        /// </summary>
        public List<GlobalCountry> PostCodeAnywhereCountries { get; set; }

        /// <summary>
        /// Gets or sets the default global country Id for the subaccount
        /// </summary>
        public int DefaultGlobalCountryId { get; set; }

        /// <summary>
        /// Gets or sets whether the current user can edit a cost code on an expense
        /// </summary>
        public bool CanEditCostCode { get; set; }

        /// <summary>
        /// Gets or sets whether the current user can edit a project code on an expense
        /// </summary>
        public bool CanEditProjectCode { get; set; }

        /// <summary>
        /// Gets or sets whether the current user can edit a department on an expense
        /// </summary>
        public bool CanEditDepartment { get; set; }

        /// <summary>
        /// Gets or sets a list containing all the UDFs which are not item-specific.
        /// </summary>
        public List<UserDefinedFieldType> UserDefinedFields { get; set; }
    }
}