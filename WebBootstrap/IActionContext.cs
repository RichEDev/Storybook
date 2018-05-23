namespace WebBootstrap
{
    using SpendManagementLibrary;
    using Spend_Management;
    using Spend_Management.expenses.code;
    using Spend_Management.shared.code;

    using BusinessLogic.Identity;
    /// <summary>
    /// An interface to describe the Dependency context for add/edit expense.
    /// </summary>
    public interface IActionContext
    {
        /// <summary>
        /// Get an instance of <see cref="UserIdentity"/> used for creating the object.
        /// </summary>
        UserIdentity User { get; }

        /// <summary>
        /// Get an instance of <see cref="Spend_Management.ICurrentUser"/>
        /// </summary>
        Spend_Management.ICurrentUser CurrentUser { get; }

        /// <summary>
        /// Get an instance of <see cref="cEmployees"/>
        /// </summary>
        cEmployees Employees { get;  }

        /// <summary>
        /// Get an instance of <see cref="cMileagecats"/>
        /// </summary>
        cMileagecats MileageCategories { get;  }

        /// <summary>
        /// Gets an instance of <see cref="cCountries"/>
        /// </summary>
        cCountries Countries { get;  }

        /// <summary>
        /// Gets an instance of <see cref="cCurrencies"/>
        /// </summary>
        cCurrencies Currencies { get;  }

        /// <summary>
        /// Gets an instance of <see cref="cCostcodes"/>
        /// </summary>
        cCostcodes CostCodes { get;  }

        /// <summary>
        /// Gets an instance of <see cref="cDepartments"/>
        /// </summary>
        cDepartments Departments { get;  }

        /// <summary>
        /// Gets an instance of <see cref="cGlobalCurrencies"/>
        /// </summary>
        cGlobalCurrencies GlobalCurrencies { get;  }

        /// <summary>
        /// Gets an instance of <see cref="cSubcats"/>
        /// </summary>
        cSubcats SubCategories { get;  }
        
        /// <summary>
        /// Gets an instance of <see cref="cCategories"/>
        /// </summary>
        cCategories Categories { get;  }

        /// <summary>
        /// Gets an instance of <see cref="cUserdefinedFields"/>
        /// </summary>
        cUserdefinedFields UserDefinedFields { get;  }

        /// <summary>
        /// Gets an instance of <see cref="cTables"/>
        /// </summary>
        cTables Tables { get;  }

        /// <summary>
        /// Gets an instance of <see cref="cMobileDevices"/>
        /// </summary>
        cMobileDevices MobileDevices { get;  }

        /// <summary>
        /// Gets an instance of <see cref="cEmployeeCars"/>
        /// </summary>
        cEmployeeCars EmployeeCars { get;  }

        /// <summary>
        /// Gets an instance of <see cref="cESRAssignments"/>
        /// </summary>
        cESRAssignments EsrAssignments { get;  }

        /// <summary>
        /// Gets an instance of <see cref="cAccountSubAccounts"/>
        /// </summary>
        cAccountSubAccounts SubAccounts { get;  }

        /// <summary>
        /// Gets an instance of <see cref="cClaims"/>
        /// </summary>
        cClaims Claims { get;  }

        /// <summary>
        /// Gets an instance of <see cref="cFields"/>
        /// </summary>
        cFields Fields { get;  }

        /// <summary>
        /// Gets an instance of <see cref="cAccounts"/>
        /// </summary>
        cAccounts Accounts { get;  }

        /// <summary>
        /// Gets an instance of <see cref="cExpenseItems"/>
        /// </summary>
        cExpenseItems ExpenseItems { get;  }

        /// <summary>
        /// Gets an instance of <see cref="cFilterRules"/>
        /// </summary>
        cFilterRules FilterRules { get;  }

        /// <summary>
        /// Gets an instance of <see cref="FlagManagement"/>
        /// </summary>
        FlagManagement FlagManagement { get;  }

        /// <summary>
        /// Gets the current Sub Account ID
        /// </summary>
        int? SubAccountId { get;  }

        /// <summary>
        /// Gets the current Account ID
        /// </summary>
        int AccountId { get;  }

        /// <summary>
        /// Gets or sets the Current Employee ID
        /// </summary>
        int EmployeeId { get; set; }

 
        /// <summary>
        ///  Gets the <see cref="BankAccounts"/> class
        /// </summary>
        BankAccounts BankAccounts { get;  }

        /// <summary>
        ///     Gets the <see cref="cCardStatements"/> class.
        /// </summary>
        cCardStatements CardStatements { get;  }

        /// <summary>
        /// Gets an instance of <see cref="cAccountProperties"/>
        /// </summary>
        cAccountProperties Properties { get;  }

        /// <summary>
        /// Gets an instance of <see cref="cMisc"/>
        /// </summary>
        cMisc Misc { get;  }

        /// <summary>
        /// Gets an instance of <see cref="cEmployeeCorporateCards"/>
        /// </summary>
        cEmployeeCorporateCards EmployeeCorporateCards { get;  }

        /// <summary>
        /// Gets an instance of <see cref="cFloats"/>
        /// </summary>
        cFloats Floats { get;  }

        /// <summary>
        /// Gets an instance of <see cref="cAllowances"/>
        /// </summary>
        cAllowances Allowances { get;  }
    }
}