namespace CacheDataAccess.Caching
{
    using System;
    using System.Collections.Generic;

    using BusinessLogic;
    using BusinessLogic.Accounts;
    using BusinessLogic.Announcements;
    using BusinessLogic.DataConnections;
    using BusinessLogic.Employees.AccessRoles;
    using BusinessLogic.Identity;

    using Common.Logging;

    /// <summary>
    /// Implements methods to retreive and create instances of <see cref="IAnnouncement"/> in <see cref="IDataConnection{T}"/>
    /// </summary>
    public class AnnouncementsCacheFactory : IDataFactory<IAnnouncement, Guid>
    {
        /// <summary>
        /// An instance of <see cref="CacheFactory{T,TK}"/> to handle caching of <see cref="IAnnouncement"/> instances.
        /// </summary>
        private readonly IMetabaseCacheFactory<IAnnouncement, Guid> _cacheFactory;

        /// <summary>
        /// An instance of the <see cref="IdentityProvider"/> for obtaining the currently logged in user.
        /// </summary>
        private readonly IdentityProvider _identityProvider;

        /// <summary>
        /// An instance of the <see cref="ILog"/> for obtaining the currently logged in user.
        /// </summary>
        private readonly ILog _logger;

        /// <summary>
        /// An instance of <see cref="IAccount"/> of the current user.
        /// </summary>
        private readonly IAccount _account;

        /// <summary>
        /// An instance of the <see cref="IEmployeeCombinedAccessRoles"/> for obtaining the current user's access roles.
        /// </summary>
        private readonly IEmployeeCombinedAccessRoles _combinedEmployeesAccessRolesFactory;

        private readonly ReadReceiptFactory _readReceiptFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="AnnouncementsCacheFactory"/> class.
        /// </summary>
        /// <param name="cacheFactory">
        /// An instance of <see cref="CacheFactory{T,TK}"/> to handle caching of <see cref="IAnnouncement"/> instances.
        /// </param>
        /// <param name="identityProvider">
        /// An instance of the <see cref="IdentityProvider"/> for obtaining the currently logged in user.
        /// </param>
        /// <param name="logger">
        /// An instance of <see cref="ILog"/> to use when logging information.
        /// </param>
        /// <param name="account">
        /// An instance of the current <see cref="IAccount"/>.
        /// </param>
        /// <param name="combinedEmployeeAccessRolesFactory">
        /// An instance of <see cref="IEmployeeCombinedAccessRoles"/>.
        /// </param>
        /// <param name="readReceiptFactory">
        /// An instance of <see cref="ReadReceiptFactory"/>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="cacheFactory"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="identityProvider"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="logger"/> is <see langword="null"/>.
        /// </exception>
        /// /// <exception cref="ArgumentNullException">
        /// <paramref name="account"/> is <see langword="null"/>.
        /// </exception>
        /// /// <exception cref="ArgumentNullException">
        /// <paramref name="combinedEmployeeAccessRolesFactory"/> is <see langword="null"/>.
        /// </exception>
        /// /// <exception cref="ArgumentNullException">
        /// <paramref name="readReceiptFactory"/> is <see langword="null"/>.
        /// </exception>
        public AnnouncementsCacheFactory(IMetabaseCacheFactory<IAnnouncement, Guid> cacheFactory, IdentityProvider identityProvider, ILog logger, IAccount account, IEmployeeCombinedAccessRoles combinedEmployeeAccessRolesFactory, ReadReceiptFactory readReceiptFactory)
        {
            Guard.ThrowIfNull(cacheFactory, nameof(cacheFactory));
            Guard.ThrowIfNull(identityProvider, nameof(identityProvider));
            Guard.ThrowIfNull(logger, nameof(logger));
            Guard.ThrowIfNull(combinedEmployeeAccessRolesFactory, nameof(combinedEmployeeAccessRolesFactory));
            Guard.ThrowIfNull(readReceiptFactory, nameof(readReceiptFactory));

            this._cacheFactory = cacheFactory;
            this._identityProvider = identityProvider;
            this._logger = logger;
            this._account = account;
            this._combinedEmployeesAccessRolesFactory = combinedEmployeeAccessRolesFactory;
            this._readReceiptFactory = readReceiptFactory;
        }

        /// <summary>
        /// Gets an instance of <see cref="IAnnouncement"/> with a matching <paramref name="id"/>
        /// </summary>
        /// <param name="id">The ID of the <see cref="IAnnouncement"/> you want to retrieve</param>
        /// <returns>The required <see cref="IAnnouncement"/> with matching ID or <see langword="null" /> if it cannot be found.</returns>
        public IAnnouncement this[Guid id]
        {
            get
            {
                IAnnouncement announcement = this._cacheFactory[id];                                
                return announcement;                
            }
        }

        /// <summary>
        /// Adds or updates the specified instance of <see cref="IAnnouncement"/> to <see cref="CacheFactory{T,TK}"/>
        /// </summary>
        /// <param name="announcement">The <see cref="IAnnouncement"/> to be saved.</param>
        /// <returns>The <see cref="IAnnouncement"/> saved</returns>
        public virtual IAnnouncement Save(IAnnouncement announcement)
        {
            if (announcement == null)
            {
                return null;
            }

            if (this._logger.IsDebugEnabled)
            {
                this._logger.Debug("AnnouncementsCacheFactory.Add(announcement) called.");
                this._logger.Debug(announcement);
            }

            if (announcement.Id == Guid.Empty)
            {
                announcement.Id = Guid.NewGuid();
            }

            announcement = this._cacheFactory.Save(announcement);

            if (this._logger.IsDebugEnabled)
            {
                this._logger.DebugFormat($"Add completed with id {announcement.Id}.");
            }

            return announcement;
        }

        /// <summary>
        /// Deletes the instance of <see cref="IAnnouncement"/> with a matching <paramref name="id"/>
        /// </summary>
        /// <param name="id">The id of the <see cref="IAnnouncement"/> to delete.</param>
        /// <returns>An <see cref="int"/> containing the result of the delete.</returns>
        public int Delete(Guid id)
        {
            if (this._logger.IsDebugEnabled)
            {
                this._logger.Debug($"AnnouncementsCacheFactory.Delete({id}) called.");
            }

            bool deleted = this._cacheFactory.Delete(id);            
            
            if (this._logger.IsDebugEnabled)
            {
                this._logger.DebugFormat($"Delete completed with id {id} and returned {deleted} for delete result.");
            }

            return Convert.ToInt32(deleted);
        }

        /// <summary>
        /// Gets a list of all available <see cref="IAnnouncement"/>
        /// </summary>
        /// <returns>The list of <see cref="IAnnouncement"/></returns>
        public IList<IAnnouncement> Get()
        {
            IList<IAnnouncement> announcements = this._cacheFactory.Get();
            return announcements;
        }

        /// <summary>
        /// Gets all instances of <see cref="IAnnouncement"/> from <see cref="CacheFactory{T,TK}"/> that match <paramref name="predicate"/> and are available for the current user
        /// </summary>
        /// <param name="predicate">Criteria to match on, before filtering.</param>
        /// <returns>An instance of <see cref="IList{T}"/> containing all available <see cref="IAnnouncement"/> that match <paramref name="predicate"/> and are available for the current user.</returns>
        public IList<IAnnouncement> Get(Predicate<IAnnouncement> predicate)
        {
            IList<IAnnouncement> baseAnnouncements = this.Get();

            if (baseAnnouncements == null)
            {
                return new List<IAnnouncement>();
            }

            var userIdentity = this._identityProvider.GetUserIdentity();                                 

            List<IAnnouncement> typeFilteredAnnouncements = new List<IAnnouncement>();            

            foreach (IAnnouncement announcement in baseAnnouncements)
            {
                if (announcement.Valid(this._account, userIdentity, this._combinedEmployeesAccessRolesFactory) && announcement.Active)
                {
                    typeFilteredAnnouncements.Add(announcement);
                }
            }

            if (predicate == null)
            {
                return typeFilteredAnnouncements;
            }

            List<IAnnouncement> matchAnnouncements = new List<IAnnouncement>();

            foreach (IAnnouncement announcement in typeFilteredAnnouncements)
            {
                if (predicate.Invoke(announcement) && announcement.Active)
                {
                    matchAnnouncements.Add(announcement);
                }
            }

            return matchAnnouncements;
        }

        /// <summary>
        /// Mark any announcements the current user has open as "read".
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>True if the operation is sucessful.
        /// </returns>
        public bool MarkAsRead()
        {
            var readReceipt = this._readReceiptFactory.Create(this._identityProvider);
            IEnumerable<IAnnouncement> announcements = this.Get(announcement => !announcement.ReadReceipts.Contains(readReceipt));

            foreach (var announcement in announcements)
            {
                announcement.ReadReceipts.Add(readReceipt);
                this.Save(announcement);
            }

            return true;
        }
    }    
}
