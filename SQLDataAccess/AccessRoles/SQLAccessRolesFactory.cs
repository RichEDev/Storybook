namespace SQLDataAccess.AccessRoles
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;

    using BusinessLogic;
    using BusinessLogic.AccessRoles;
    using BusinessLogic.AccessRoles.ApplicationAccess;
    using BusinessLogic.AccessRoles.ReportsAccess;
    using BusinessLogic.AccessRoles.Scopes;
    using BusinessLogic.Accounts.Elements;
    using BusinessLogic.Cache;
    using BusinessLogic.DataConnections;

    using CacheDataAccess.Caching;

    using Common.Logging;

    /// <summary>
    /// Implements methods to retrieve and create instances of <see cref="IAccessRole"/> from SQL
    /// </summary>
    public class SqlAccessRolesFactory : IDataFactory<IAccessRole, int>
    {
        /// <summary>
        /// An instance of <see cref="CacheFactory{T,TK}"/> to retrieve and create instances of <see cref="IAccessRole"/> in cache via <see cref="ICache{T,TK}"/>.
        /// </summary>
        private readonly IAccountCacheFactory<IAccessRole, int> _cacheFactory;

        /// <summary>
        /// An instance of <see cref="ICustomerDataConnection{T}"/> to use for accessing <see cref="ICustomerDataConnection{T}"/>
        /// </summary>
        private readonly ICustomerDataConnection<SqlParameter> _customerDataConnection;

        /// <summary>
        /// A logger instance to enable logging in this <see cref="SqlAccessRolesFactory"/>.
        /// </summary>
        private ILog _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlAccessRolesFactory"/> class to use when creating or retrieving new instances of <see cref="IAccessRole"/>
        /// </summary>
        /// <param name="cacheFactory">An instance of <see cref="CacheFactory{T,TK}"/> to retrieve and create instances of <see cref="IAccessRole"/> in cache via <see cref="ICache{T,TK}"/>.</param>
        /// <param name="customerDataConnection">An instance of <see cref="ICustomerDataConnection{T}"/> to use when accessing <see cref="SqlAccessRolesFactory"/>.</param>
        /// <param name="logger">A logger instance to enable logging in this <see cref="SqlAccessRolesFactory"/>.</param>
        public SqlAccessRolesFactory(IAccountCacheFactory<IAccessRole, int> cacheFactory, ICustomerDataConnection<SqlParameter> customerDataConnection, ILog logger)
        {
            Guard.ThrowIfNull(cacheFactory, nameof(cacheFactory));
            Guard.ThrowIfNull(customerDataConnection, nameof(customerDataConnection));
            Guard.ThrowIfNull(logger, nameof(logger));

            this._customerDataConnection = customerDataConnection;
            this._cacheFactory = cacheFactory;
        }

        /// <summary>
        /// Gets an instance of <see cref="IAccessRole"/> with a matching ID from memory if possible, if not it will search cache for an entry 
        /// </summary>
        /// <param name="id">The ID of the <see cref="IAccessRole"/> you want to retrieve</param>
        /// <returns>The required <see cref="IAccessRole"/> or null if it cannot be found</returns>
        public IAccessRole this[int id]
        {
            get
            {
                IAccessRole accessRole = this._cacheFactory[id];

                if (accessRole == null)
                {
                    accessRole = this.Get(id);

                    if (accessRole != null)
                    {
                        this._cacheFactory.Save(accessRole);
                    }
                }

                return accessRole;
            }
        }

        /// <inheritdoc />
        public int Delete(int id)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public IList<IAccessRole> Get()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public IList<IAccessRole> Get(Predicate<IAccessRole> predicate)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Adds an instance of <see cref="IAccessRole"/> to the collection
        /// </summary>
        /// <param name="entity">
        /// The <see cref="IAccessRole"/> you want to add
        /// </param>
        /// <returns>
        /// The <see cref="IAccessRole"/> returned by <see cref="CacheFactory{T,TK}"/>.
        /// </returns>
        public IAccessRole Save(IAccessRole entity)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets a <see cref="IAccessRole">IAccess</see> by Id
        /// </summary>
        /// <param name="id">
        /// The Access Role Id
        /// </param>
        /// <returns>
        /// The <see cref="IAccessRole">IAccess</see>
        /// </returns>
        private IAccessRole Get(int id)
        {
            AccessRole accessRole = null;

            const string Sql = "SELECT rolename, description, expenseClaimMinimumAmount, expenseClaimMaximumAmount, employeesCanAmendDesignatedCostCode, employeesCanAmendDesignatedDepartment, employeesCanAmendDesignatedProjectCode, roleAccessLevel, allowWebsiteAccess, allowMobileAccess, allowApiAccess, employeesMustHaveBankAccount FROM accessRoles where roleID = @accessRoleID";

            this._customerDataConnection.Parameters.Add(new SqlParameter("@accessRoleID", SqlDbType.Int) { Value = id });
   
            using (IDataReader reader = this._customerDataConnection.GetReader(Sql))
            {
                int accessRoleNameOrdinal = reader.GetOrdinal("rolename");
                int descriptionOrdinal = reader.GetOrdinal("description");
                int expenseClaimMinimumAmountOrdinal = reader.GetOrdinal("expenseClaimMinimumAmount");
                int expenseClaimMaximumAmountOrdinal = reader.GetOrdinal("expenseClaimMaximumAmount");
                int employeesCanAmendDesignatedCostCodeOrdinal = reader.GetOrdinal("employeesCanAmendDesignatedCostCode");
                int employeesCanAmendDesignatedDepartmentOrdinal = reader.GetOrdinal("employeesCanAmendDesignatedDepartment");
                int employeesCanAmendDesignatedProjectCodeOrdinal = reader.GetOrdinal("employeesCanAmendDesignatedProjectCode");
                int roleAccessLevelOrdinal = reader.GetOrdinal("roleAccessLevel");
                int allowWebsiteAccessOrdinal = reader.GetOrdinal("allowWebsiteAccess");
                int allowMobileAccessOrdinal = reader.GetOrdinal("allowMobileAccess");
                int allowApiAccessOrdinal = reader.GetOrdinal("allowApiAccess");
                int employeesMustHaveBankAccountOrdinal = reader.GetOrdinal("employeesMustHaveBankAccount");

                while (reader.Read())
                {
                    string accessRoleName = reader.GetString(accessRoleNameOrdinal);
                    string description = reader.GetString(descriptionOrdinal);
                    bool allowWebsiteAccess = reader.GetBoolean(allowWebsiteAccessOrdinal);
                    bool allowMobileAccess = reader.GetBoolean(allowMobileAccessOrdinal);
                    bool allowApiAccess = reader.GetBoolean(allowApiAccessOrdinal);
                    
                    ReportingAccess roleAccessLevel = (ReportingAccess)reader.GetInt16(roleAccessLevelOrdinal);

                    accessRole = new AccessRole(id, accessRoleName, description, this.GetApplicationScopes(allowApiAccess, allowWebsiteAccess, allowMobileAccess), this.GetReportAccess(id, roleAccessLevel), this.GetAccessScopeCollection(id));

                    // Creation of an ExpenseAccessRole - not sure how we will decide to create this or not yet.
                    bool employeesMustHaveBankAccount = reader.GetBoolean(employeesMustHaveBankAccountOrdinal);
                    decimal? expenseClaimMinimumAmount = reader.IsDBNull(expenseClaimMinimumAmountOrdinal) ? (decimal?)null : reader.GetDecimal(expenseClaimMinimumAmountOrdinal);
                    decimal? expenseClaimMaximumAmount = reader.IsDBNull(expenseClaimMaximumAmountOrdinal) ? (decimal?)null : reader.GetDecimal(expenseClaimMaximumAmountOrdinal);
                    bool employeesCanAmendDesignatedCostCode = reader.GetBoolean(employeesCanAmendDesignatedCostCodeOrdinal);
                    bool employeesCanAmendDesignatedDepartment = reader.GetBoolean(employeesCanAmendDesignatedDepartmentOrdinal);
                    bool employeesCanAmendDesignatedProjectCode = reader.GetBoolean(employeesCanAmendDesignatedProjectCodeOrdinal);

                    accessRole = new ExpensesAccessRole(accessRole, expenseClaimMaximumAmount, expenseClaimMinimumAmount, employeesCanAmendDesignatedCostCode, employeesCanAmendDesignatedDepartment, employeesCanAmendDesignatedProjectCode, employeesMustHaveBankAccount);
                }
            }

            this._customerDataConnection.Parameters.Clear();

            return accessRole;
        }

        /// <summary>
        /// Returns a list containing all element access for all roles
        /// </summary>
        /// <param name="accessRoleId">
        /// The access Role ID.
        /// </param>
        /// <returns>
        /// An instance of <see cref="AccessScopeCollection"/> for this <see cref="IAccessRole"/>
        /// </returns>
        private AccessScopeCollection GetAccessScopeCollection(int accessRoleId)
        {
            const string Sql = "SELECT elementID, updateAccess, insertAccess, deleteAccess, viewAccess FROM dbo.accessRoleElementDetails WHERE roleID=@accessRoleID";
            this._customerDataConnection.Parameters.Add(new SqlParameter("@accessRoleID", SqlDbType.Int) { Value = accessRoleId });

            AccessScopeCollection accessScopeCollection = new AccessScopeCollection();

            using (var reader = this._customerDataConnection.GetReader(Sql))
            {
                int elementIdOrdinal = reader.GetOrdinal("elementID");
                int updateAccessOrdinal = reader.GetOrdinal("updateAccess");
                int insertAccessOrdinal = reader.GetOrdinal("insertAccess");
                int deleteAccessOrdinal = reader.GetOrdinal("deleteAccess");
                int viewAccessOrdinal = reader.GetOrdinal("viewAccess");

                while (reader.Read())
                {
                    int elementId = reader.GetInt32(elementIdOrdinal);
                    ModuleElements element = (ModuleElements)elementId;
                    bool editAccess = reader.GetBoolean(updateAccessOrdinal);
                    bool addAccess = reader.GetBoolean(insertAccessOrdinal);
                    bool deleteAccess = reader.GetBoolean(deleteAccessOrdinal);
                    bool viewAccess = reader.GetBoolean(viewAccessOrdinal);

                    AccessScope elementAccess = new AccessScope(element, viewAccess, addAccess, editAccess, deleteAccess);
                    accessScopeCollection.Add(elementAccess);
                }
            }

            this._customerDataConnection.Parameters.Clear();

            return accessScopeCollection;
        }

        /// <summary>
        /// Creates an instance of <see cref="ApplicationScopeCollection"/> based on the arguments passed.
        /// </summary>
        /// <param name="api">Whether access to the api application is granted.</param>
        /// <param name="website">Whether access to the website application is granted.</param>
        /// <param name="mobile">Whether access to the mobile application is granted.</param>
        /// <returns>The <see cref="ApplicationScopeCollection"/> populated with instances of <see cref="IApplicationAccessScope"/>.</returns>
        private ApplicationScopeCollection GetApplicationScopes(bool api, bool website, bool mobile)
        {
            ApplicationScopeCollection applicationScopesCollection = new ApplicationScopeCollection();

            if (api)
            {
                applicationScopesCollection.Add(new ApiAccess());
            }

            if (website)
            {
                applicationScopesCollection.Add(new WebsiteAccess());
            }

            if (mobile)
            {
                applicationScopesCollection.Add(new MobileAccess());
            }

            return applicationScopesCollection;
        }

        /// <summary>
        /// Returns a list of AccessRole links, i.e. AccessRoleID 1 may be able to report on AccessRole 2, 4 and 6
        /// </summary>
        /// <param name="accessRoleId">The primary access role to check links for.</param>
        /// <returns>
        /// An <see cref="IEnumerable{T}"/> instance containing all linked <see cref="IAccessRole"/> IDs for the specified <see cref="IAccessRole"/>.
        /// </returns>
        private IEnumerable<int> GetLinkedAccessRoles(int accessRoleId)
        {
            List<int> linkedAccessRoleCollections = new List<int>();

            const string Sql = "SELECT secondaryAccessRoleID FROM dbo.accessRolesLink WHERE primaryAccessRoleID=@accessRoleId";

            this._customerDataConnection.Parameters.Add(new SqlParameter("@accessRoleId", accessRoleId));

            using (IDataReader reader = this._customerDataConnection.GetReader(Sql))
            {
                int secondaryAccessRoleIdOrdinal = reader.GetOrdinal("secondaryAccessRoleID");

                while (reader.Read())
                {
                    int secondaryAccessRoleId = reader.GetInt32(secondaryAccessRoleIdOrdinal);

                    if (linkedAccessRoleCollections.Contains(secondaryAccessRoleId) == false)
                    {
                        linkedAccessRoleCollections.Add(secondaryAccessRoleId);
                    }
                }
            }

            return linkedAccessRoleCollections;
        }

        /// <summary>
        /// Creates an instance of <see cref="IReportsAccess"/> based on the parameters passed.
        /// </summary>
        /// <param name="accessRoleId">The access role id.</param>
        /// <param name="reportingAccess">The reporting access.</param>
        /// <returns>
        /// The <see cref="IReportsAccess"/> instance based on the <paramref name="accessRoleId"/> and <paramref name="reportingAccess"/>.
        /// </returns>
        /// <exception cref="ArgumentException">Thrown if the reporting access is not valid.</exception>
        private IReportsAccess GetReportAccess(int accessRoleId, ReportingAccess reportingAccess)
        {
            IReportsAccess reportAccess;
            switch (reportingAccess)
            {
                case ReportingAccess.AllData:
                    reportAccess = new AllDataReportsAccess();
                    break;
                case ReportingAccess.EmployeesResponsibleFor:
                    reportAccess = new EmployeesResponsibleFor();
                    break;
                case ReportingAccess.SelectedRoles:
                    reportAccess = new SelectedAccessRoles(this.GetLinkedAccessRoles(accessRoleId));
                    break;
                default:
                    throw new ArgumentException($"Unhandled report access {reportingAccess}.", nameof(reportingAccess));
            }

            return reportAccess;
        }
    }
}
