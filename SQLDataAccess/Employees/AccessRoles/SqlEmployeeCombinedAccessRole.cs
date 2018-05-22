namespace SQLDataAccess.Employees.AccessRoles
{
    using System.Collections.Generic;

    using BusinessLogic;
    using BusinessLogic.AccessRoles;
    using BusinessLogic.DataConnections;
    using BusinessLogic.Employees.AccessRoles;
    using BusinessLogic.Identity;

    using CacheDataAccess.Caching;

    using Common.Logging;

    /// <summary>
    /// Members to obtain a single combined <see cref="IEmployeeAccessScope"/> based on their assigned <see cref="IAccessRole"/> objects.
    /// </summary>
    public class SqlEmployeeCombinedAccessRolesFactory : IEmployeeCombinedAccessRoles
    {
        /// <summary>
        /// Access role factory to obtain instances of an <see cref="IAccessRole"/>.
        /// </summary>
        private readonly IDataFactory<IAccessRole, int> _accessRoleFactory;

        /// <summary>
        /// Assigned access roles to obtain a list of assigned access roles for an employee.
        /// </summary>
        private readonly IAssignedAccessRolesFactory _assignedAccessRoles;

        /// <summary>
        /// Cache factory for storing and obtaining instances of <see cref="IAssignedAccessRole"/> from cache.
        /// </summary>
        private readonly IAccountCacheFactory<IEmployeeAccessScope, string> _cacheFactory;

        /// <summary>
        /// Identity provider to obtain the current <see cref="UserIdentity"/>.
        /// </summary>
        private readonly IIdentityProvider _identityProvider;

        /// <summary>
        /// Logger to enable logging within this <see cref="SqlEmployeeCombinedAccessRolesFactory"/>.
        /// </summary>
        private readonly ILog _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlEmployeeCombinedAccessRolesFactory"/> class.
        /// </summary>
        /// <param name="assignedAccessRolesFactory">Assigned access roles to obtain a list of assigned access roles for an employee.</param>
        /// <param name="accessRolesFactory">Access role factory to obtain instances of an <see cref="IAccessRole"/>.</param>
        /// <param name="cacheFactory">Cache factory for storing and obtaining instances of <see cref="IAssignedAccessRole"/> from cache.</param>
        /// <param name="identityProvider">Identity provider to obtain the current <see cref="UserIdentity"/>.</param>
        /// <param name="logger">Logger to enable logging within this <see cref="SqlEmployeeCombinedAccessRolesFactory"/>.</param>
        public SqlEmployeeCombinedAccessRolesFactory(IAssignedAccessRolesFactory assignedAccessRolesFactory, IDataFactory<IAccessRole, int> accessRolesFactory, IAccountCacheFactory<IEmployeeAccessScope, string> cacheFactory, IIdentityProvider identityProvider, ILog logger)
        {
            Guard.ThrowIfNull(accessRolesFactory, nameof(accessRolesFactory));
            Guard.ThrowIfNull(assignedAccessRolesFactory, nameof(assignedAccessRolesFactory));
            Guard.ThrowIfNull(cacheFactory, nameof(cacheFactory));
            Guard.ThrowIfNull(identityProvider, nameof(identityProvider));
            Guard.ThrowIfNull(logger, nameof(logger));

            this._accessRoleFactory = accessRolesFactory;
            this._assignedAccessRoles = assignedAccessRolesFactory;
            this._cacheFactory = cacheFactory;
            this._identityProvider = identityProvider;
            this._logger = logger;
        }

        /// <inheritdoc />
        public IEmployeeAccessScope Get(string compositeId)
        {
            if (this._logger.IsDebugEnabled)
            {
                   this._logger.Debug($"SqlEmployeeCombinedAccessRole Get({compositeId}) started."); 
            }

            UserIdentity userIdentity = this._identityProvider.GetUserIdentity();
            IEmployeeAccessScope employeeAccessScope = this.Get(userIdentity.EmployeeId, userIdentity.SubAccountId);
            /* Disabled until assigned access roles is replaced, until then caching is not enabled with AssignedAccessRoles or CombinedAccessRoles
             * IEmployeeAccessScope employeeAccessScope = this._cacheFactory[compositeId] ?? this.Get(userIdentity.EmployeeId, userIdentity.SubAccountId);
            */
            
            if (this._logger.IsDebugEnabled)
            {
                this._logger.Debug($"SqlEmployeeCombinedAccessRole Get({compositeId}) completed.");
            }

            return employeeAccessScope;
        }

        /// <inheritdoc />
        public IEmployeeAccessScope Get(int employeeId, int subAccountId)
        {
            if (this._logger.IsDebugEnabled)
            {
                this._logger.Debug($"SqlEmployeeCombinedAccessRole Get({employeeId}, {subAccountId}) started.");
            }

            IEnumerable<IAssignedAccessRole> assignedAccessRoles = this._assignedAccessRoles.Get(employeeId, accessRole => accessRole.SubAccountId == subAccountId);
            List<IAccessRole> accessRoles = new List<IAccessRole>();

            foreach (IAssignedAccessRole assignedAccessRole in assignedAccessRoles)
            {
                IAccessRole accessRole = this._accessRoleFactory[assignedAccessRole.AccessRoleId];
                accessRoles.Add(accessRole);
            }

            IEmployeeAccessScope combinedAccessScope = null;

            if (accessRoles.Count > 0)
            {
                combinedAccessScope = new EmployeeCombinedAccessRole(accessRoles);
            }

            /* Disabled until assigned access roles is replaced, until then caching is not enabled with AssignedAccessRoles or CombinedAccessRoles
             * this._cacheFactory.Add(combinedAccessScope);
            */

            if (this._logger.IsDebugEnabled)
            {
                this._logger.Debug($"SqlEmployeeCombinedAccessRole Get({employeeId}, {subAccountId}) completed.");
            }

            return combinedAccessScope;
        }
    }
}
