namespace SQLDataAccess.AccessRoles
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;

    using BusinessLogic.Accounts;
    using BusinessLogic.AccessRoles;
    using BusinessLogic.AccessRoles.ReportsAccess;
    using BusinessLogic.Accounts.Elements;
    using BusinessLogic.Cache;
    using BusinessLogic.DataConnections;

    using CacheDataAccess.Caching;

    /// <summary>
    /// Implements methods to retrieve and create instances of <see cref="IAccessRole"/> from SQL
    /// </summary>
    public class SqlAccessRolesFactory : IDataFactory<IAccessRole, int>
    {
        /// <summary>
        /// An instance of <see cref="CacheFactory{T,TK}"/> to retrieve and create instances of <see cref="IAccessRole"/> in cache via <see cref="ICache{T,TK}"/>.
        /// </summary>
        private readonly AccountCacheFactory<IAccessRole, int> _cacheFactory;

        /// <summary>
        /// An instance of <see cref="ICustomerDataConnection{T}"/> to use for accessing <see cref="ICustomerDataConnection{T}"/>
        /// </summary>
        private readonly ICustomerDataConnection<SqlParameter> _customerDataConnection;

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlAccessRolesFactory"/> class to use when creating or retrieving new instances of <see cref="IAccessRole"/>
        /// </summary>
        /// <param name="cacheFactory">
        /// An instance of <see cref="CacheFactory{T,TK}"/> to retrieve and create instances of <see cref="IAccessRole"/> in cache via <see cref="ICache{T,TK}"/>.
        /// </param>
        /// <param name="customerDataConnection">
        /// An instance of <see cref="ICustomerDataConnection{T}"/> to use when accessing <see cref="SqlAccessRolesFactory"/>
        /// </param>
        public SqlAccessRolesFactory(AccountCacheFactory<IAccessRole, int> cacheFactory, ICustomerDataConnection<SqlParameter> customerDataConnection)
        {
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
                    this._cacheFactory.Save(accessRole);
                }

                return accessRole;
            }
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
            return this._cacheFactory.Save(entity);
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
        public IAccessRole Get(int id)
        {
            AccessRole accessRole;

            ElementAccessCollections elementAccessCollections = this.GetElementAccessCollections(id);

            const string Sql = "SELECT roleID, rolename, description, expenseClaimMinimumAmount, expenseClaimMaximumAmount, employeesCanAmendDesignatedCostCode, employeesCanAmendDesignatedDepartment, employeesCanAmendDesignatedProjectCode, roleAccessLevel, allowWebsiteAccess, allowMobileAccess, allowApiAccess, employeesMustHaveBankAccount FROM accessRoles where roleID = @accessRoleID";

            this._customerDataConnection.Parameters.Add(new SqlParameter("@accessRoleID", SqlDbType.Int) { Value = id});
   
            // Skipping the creation of the custom entities section of access roles for now since its not needed.
            // IList<CustomEntityElementAccessLevelCollection> customEntityElementAccessLevelCollections = GetCustomEntityElementAcessLevels();

            LinkedAccessRoleCollections linkedAccessRoleCollections = this.GetLinkedAccessRoles();

            using (IDataReader reader = this._customerDataConnection.GetReader(Sql))
            {
                int accessRoleIdOrdinal = reader.GetOrdinal("roleID");
                int acessRoleNameOrdinal = reader.GetOrdinal("rolename");
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

                reader.Read();
                int accessRoleId = reader.GetInt32(accessRoleIdOrdinal);
                string acessRoleName = reader.GetString(acessRoleNameOrdinal);
                string description = reader.GetString(descriptionOrdinal);
                bool allowWebsiteAccess = reader.GetBoolean(allowWebsiteAccessOrdinal);
                bool allowMobileAccess = reader.GetBoolean(allowMobileAccessOrdinal);
                bool allowApiAccess = reader.GetBoolean(allowApiAccessOrdinal);

                IReportsAccess reportAccess = null;
                ReportingAccess roleAccessLevel = (ReportingAccess)reader.GetInt32(roleAccessLevelOrdinal);

                switch (roleAccessLevel)
                {
                    case ReportingAccess.AllData:
                        reportAccess = new AllDataReportsAccess();
                        break;
                    case ReportingAccess.EmployeesResponsibleFor:
                        reportAccess = new EmployeesResponsibleFor();
                        break;
                    case ReportingAccess.SelectedRoles:
                        reportAccess = new SelectedAccessRoles(linkedAccessRoleCollections[accessRoleId]);
                        break;
                }

                accessRole = new AccessRole(accessRoleId, acessRoleName, description, allowApiAccess, allowMobileAccess, allowWebsiteAccess, reportAccess, elementAccessCollections[accessRoleId]);

                // Creation of an ExpenseAccessRole - not sure how we will decide to create this or not yet.
                bool employeesMustHaveBankAccount = reader.GetBoolean(employeesMustHaveBankAccountOrdinal);
                decimal? expenseClaimMinimumAmount = reader.IsDBNull(expenseClaimMinimumAmountOrdinal) ? (decimal?)null : reader.GetDecimal(expenseClaimMinimumAmountOrdinal);
                decimal? expenseClaimMaximumAmount = reader.IsDBNull(expenseClaimMaximumAmountOrdinal) ? (decimal?)null : reader.GetDecimal(expenseClaimMaximumAmountOrdinal);
                bool employeesCanAmendDesignatedCostCode = reader.GetBoolean(employeesCanAmendDesignatedCostCodeOrdinal);
                bool employeesCanAmendDesignatedDepartment = reader.GetBoolean(employeesCanAmendDesignatedDepartmentOrdinal);
                bool employeesCanAmendDesignatedProjectCode = reader.GetBoolean(employeesCanAmendDesignatedProjectCodeOrdinal);

                accessRole = new ExpensesAccessRole(accessRole, expenseClaimMaximumAmount, expenseClaimMinimumAmount, employeesCanAmendDesignatedCostCode, employeesCanAmendDesignatedDepartment, employeesCanAmendDesignatedProjectCode, employeesMustHaveBankAccount);
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
        /// An instance of <see cref="ElementAccessCollections"/> for this <see cref="IAccessRole"/>
        /// </returns>
        private ElementAccessCollections GetElementAccessCollections(int accessRoleId)
        {
            const string Sql = "SELECT elementID, updateAccess, insertAccess, deleteAccess, viewAccess FROM dbo.accessRoleElementDetails WHERE roleID=@accessRoleID";
            this._customerDataConnection.Parameters.Add(new SqlParameter("@accessRoleID", SqlDbType.Int) { Value = accessRoleId});
            
            ElementAccessCollections elementAccessCollections = new ElementAccessCollections();

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
                    bool updateAccess = reader.GetBoolean(updateAccessOrdinal);
                    bool insertAccess = reader.GetBoolean(insertAccessOrdinal);
                    bool deleteAccess = reader.GetBoolean(deleteAccessOrdinal);
                    bool viewAccess = reader.GetBoolean(viewAccessOrdinal);

                    ElementAccessLevel elementAccess = new ElementAccessLevel(elementId, viewAccess, insertAccess, updateAccess, deleteAccess);

                    if (elementAccessCollections.Contains(accessRoleId) == false)
                    {
                        elementAccessCollections.Add(new ElementAccessCollection(accessRoleId));
                    }

                    elementAccessCollections[accessRoleId].Add(element, elementAccess);
                }
            }

            this._customerDataConnection.Parameters.Clear();

            return elementAccessCollections;
        }

        /// <summary>
        /// Returns a list of AccessRole links, i.e. AccessRoleID 1 may be able to report on AccessRole 2, 4 and 6
        /// </summary>
        /// <returns>
        /// A <see cref="LinkedAccessRoleCollection"/> containing all linked <see cref="IAccessRole"/> IDs for this <see cref="IAccount"/>
        /// </returns>
        private LinkedAccessRoleCollections GetLinkedAccessRoles()
        {
            LinkedAccessRoleCollections linkedAccessRoleCollections = new LinkedAccessRoleCollections();

            const string Sql = "SELECT primaryAccessRoleID, secondaryAccessRoleID FROM dbo.accessRolesLink";

            // primaryAccessRoleID
            using (IDataReader reader = this._customerDataConnection.GetReader(Sql))
            {
                int primaryAccessRoleIdOrdinal = reader.GetOrdinal("primaryAccessRoleID");
                int secondaryAccessRoleIdOrdinal = reader.GetOrdinal("secondaryAccessRoleID");

                while (reader.Read())
                {
                    int primaryAccessRoleId = reader.GetInt32(primaryAccessRoleIdOrdinal);
                    int secondaryAccessRoleId = reader.GetInt32(secondaryAccessRoleIdOrdinal);

                    if (linkedAccessRoleCollections.Contains(primaryAccessRoleId) == false)
                    {
                        linkedAccessRoleCollections.Add(new LinkedAccessRoleCollection(primaryAccessRoleId));
                    }

                    linkedAccessRoleCollections[primaryAccessRoleId].Add(secondaryAccessRoleId);
                }
            }
            
            return linkedAccessRoleCollections;
        }

        public int Delete(int id)
        {
            throw new System.NotImplementedException();
        }

        public IList<IAccessRole> Get()
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public IList<IAccessRole> Get(Predicate<IAccessRole> predicate)
        {
            throw new NotImplementedException();
        }
    }
}
