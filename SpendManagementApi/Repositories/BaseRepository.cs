namespace SpendManagementApi.Repositories
{
    using System;
    using System.Collections.Generic;
    using Utilities;
    using Common;
    using Interfaces;
    using Models.Common;
    using Models.Types;
    using Spend_Management;

    /// <summary>
    /// Archiving Base Repository
    /// </summary>
    /// <typeparam name="T">Type of repository</typeparam>
    internal abstract class ArchivingBaseRepository<T> : BaseRepository<T>, IArchivingRepository<T> where T : ArchivableBaseExternalType
    {
        /// <summary>
        /// Archiving Base repository constructor, without the action context.
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="idSelector">Id property of T</param>
        /// <param name="nameSelector">Name property of T</param>
        protected ArchivingBaseRepository(ICurrentUser user, Func<T, int> idSelector, Func<T, string> nameSelector) 
            : base(user, idSelector, nameSelector)
        {
        }

        /// <summary>
        /// Archiving Base repository constructor, without the action context.
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="idSelector">Id property of T</param>
        /// <param name="nameSelector">Name property of T</param>
        protected ArchivingBaseRepository(ICurrentUser user, Func<T, Guid> idSelector, Func<T, string> nameSelector)
            : base(user, idSelector, nameSelector)
        {
        }

        /// <summary>
        /// Archiving Base repository constructor, taking an action context.
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="actionContext">ActionContext</param>
        /// <param name="idSelector">Id property of T</param>
        /// <param name="nameSelector">Name property of T</param>
        protected ArchivingBaseRepository(ICurrentUser user, IActionContext actionContext, Func<T, int> idSelector, Func<T, string> nameSelector)
            : this(user, idSelector, nameSelector)
        {
            SetDependencies(actionContext);
        }


        public virtual T Archive(int id, bool archive)
        {
            var item = Get(id);
            if (item == null)
            {
                throw new ApiException(ApiResources.ApiErrorRecordDoesntExist,
                    ApiResources.ApiErrorRecordDoesntExistMessage);
            }
            item.Archived = archive;
            return item;
        }
    }

    /// <summary>
    /// Base Repository
    /// </summary>
    /// <typeparam name="T">Type of repository</typeparam>
    internal abstract class BaseRepository<T> : IRepository<T> where T : BaseExternalType
    {
        /// <summary>
        /// User
        /// </summary>
        public ICurrentUser User { get; private set; }

        private readonly Func<T, int> _idSelector;

        private readonly Func<T, string> _nameSelector;

        private readonly Func<T, Guid> _guidSelector;

        private IActionContext _actionContext;

        public IActionContext ActionContext {
            get
            {
                _actionContext = _actionContext ?? new ActionContext(User);
                return _actionContext;
            }
            set
            {
                _actionContext = value;
            }
        }

        /// <summary>
        /// Base repository constructor
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="idSelector">Id property of T</param>
        /// <param name="nameSelector">Name property of T</param>
        protected BaseRepository(ICurrentUser user, Func<T, int> idSelector, Func<T, string> nameSelector)
        {
            User = user;
            _idSelector = idSelector;
            _nameSelector = nameSelector;
        }

        /// <summary>
        /// Base repository constructor
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="guidSelector">Guid Id property of T</param>
        /// <param name="nameSelector">Name property of T</param>
        protected BaseRepository(ICurrentUser user, Func<T, Guid> guidSelector, Func<T, string> nameSelector)
        {
            User = user;
            _guidSelector = guidSelector;
            _nameSelector = nameSelector;
        }

        /// <summary>
        /// Base repository constructor
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="actionContext">ActionContext</param>
        /// <param name="idSelector">Id property of T</param>
        /// <param name="nameSelector">Name property of T</param>
        protected BaseRepository(ICurrentUser user, IActionContext actionContext, Func<T, int> idSelector, Func<T, string> nameSelector)
            : this(user, idSelector, nameSelector)
        {
            SetDependencies(actionContext);
        }

        /// <summary>
        /// Base repository constructor
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="actionContext">ActionContext</param>
        /// <param name="idSelector">Id property of T</param>
        /// <param name="nameSelector">Name property of T</param>
        protected BaseRepository(ICurrentUser user, IActionContext actionContext, Func<T, Guid> idSelector, Func<T, string> nameSelector)
            : this(user, idSelector, nameSelector)
        {
            SetDependencies(actionContext);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseRepository{T}"/> class.
        /// </summary>
        protected BaseRepository()
        {       
        }

        internal void SetDependencies(IActionContext actionContext)
        {
            if (actionContext == null)
            {
                actionContext = new ActionContext(User);
            }
            ActionContext = actionContext;
        }

        /// <summary>
        /// Get all of T
        /// </summary>
        /// <returns></returns>
        public abstract IList<T> GetAll();

        /// <summary>
        /// Get item with id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public abstract T Get(int id);

        /// <summary>
        /// Add instance of T
        /// </summary>
        /// <param name="dataToAdd">Instance to add</param>
        /// <returns>Added instance</returns>
        public virtual T Add(T dataToAdd)
        {
            if (_idSelector(dataToAdd) != 0)
            {
                throw new ApiException(ApiResources.ApiErrorRecordAlreadyExists,
                    ApiResources.ApiErrorRecordAlreadyExistsMessage);
            }

            if (_nameSelector != null && string.IsNullOrEmpty(_nameSelector(dataToAdd)))
            {
                throw new ApiException(ApiResources.ApiErrorInvalidName, ApiResources.ApiErrorInvalidNameMessage);
            }

            dataToAdd.AccountId = User.AccountID;
            dataToAdd.EmployeeId = User.EmployeeID;
            dataToAdd.CreatedById = User.EmployeeID;
            dataToAdd.CreatedOn = DateTime.UtcNow;
            return dataToAdd;
        }

        /// <summary>
        /// Updates instance
        /// </summary>
        /// <param name="dataToUpdate"></param>
        /// <returns></returns>
        public virtual T Update(T dataToUpdate)
        {
            if (_idSelector(dataToUpdate) <= 0)
            {
                throw new ApiException(ApiResources.ApiErrorUpdateObjectWithWrongId, 
                    ApiResources.ApiErrorUpdateObjectWithWrongIdMessage);
            }
            
            if (_nameSelector != null && string.IsNullOrEmpty(_nameSelector(dataToUpdate)))
            {
                throw new ApiException(ApiResources.ApiErrorInvalidName, ApiResources.ApiErrorInvalidNameMessage);
            }

            dataToUpdate.AccountId = User.AccountID;
            dataToUpdate.EmployeeId = User.EmployeeID;
            dataToUpdate.ModifiedById = User.EmployeeID;
            dataToUpdate.ModifiedOn = DateTime.UtcNow;
            return dataToUpdate;
        }

        /// <summary>
        /// Deletes instance
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual T Delete(int id)
        {
            var item = Get(id);
            if (item == null)
            {
                throw new ApiException(ApiResources.ApiErrorRecordDoesntExist,
                    ApiResources.ApiErrorRecordDoesntExistMessage);
            }
            return item;
        }
    }
}