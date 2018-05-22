namespace PublicAPI.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Web.Http;

    using BusinessLogic.Announcements;
    using BusinessLogic.Identity;

    using CacheDataAccess.Caching;

    using Common.Actions;
    using DTO;
    using Security.Filters;

    using BusinessLogic.Identity;

    /// <summary>
    /// Controller enabling actions to be executed on <see cref="IAnnouncement"/> using the <see cref="AnnouncementDto"/>.
    /// </summary>
    [JwtAuthentication]
    public class AnnouncementController : ApiController, IGet<AnnouncementDto>, IMarkAsRead<AnnouncementDto>
    {
        /// <summary>
        /// Lazy loader for an instance of an Announcement
        /// </summary>
        private readonly Lazy<AnnouncementsCacheFactory> _announcements = new Lazy<AnnouncementsCacheFactory>(() => WebApiApplication.container.GetInstance<AnnouncementsCacheFactory>());

        /// <summary>
        /// Lazy loader for an instance of IdentityProvider
        /// </summary>

        /// <summary>
        /// Lazy loader for an instance of IdentityProvider
        /// </summary>
        private readonly Lazy<IIdentityProvider> _identity = new Lazy<IIdentityProvider>(() => WebApiApplication.container.GetInstance<IIdentityProvider>());

        /// <summary>
        /// Lazy loader for the <see cref="ReadReceiptFactory"/>.
        /// </summary>
        private readonly  Lazy<ReadReceiptFactory> _readReceiptFactory = new Lazy<ReadReceiptFactory>(() => WebApiApplication.container.GetInstance<ReadReceiptFactory>());

        /// <summary>
        /// Controller action to get all available instances of <see cref="AnnouncementDto"/> that do not contain the predicate
        /// </summary>
        /// <remarks>
        /// GET: <a href="https://api.hostname/Announcement">https://api.hostname/Announcement</a>
        /// </remarks>
        /// <returns>A <see cref="IEnumerable{T}"/> containing all instances of <see cref="AnnouncementDto"/>.</returns>
        public IHttpActionResult Get()
        {
            IEnumerable<IAnnouncement> announcements = this._announcements.Value.Get(announcement => !announcement.ReadReceipts.Contains(this._readReceiptFactory.Value.Create(this._identity.Value)));
            IEnumerable<AnnouncementDto> announcementsDtoCollection = MapObjects.Map<IEnumerable<AnnouncementDto>>(announcements);            

            return this.Json(announcementsDtoCollection);
        }        

        /// <summary>
        /// Mark all outstanding annouments for the current identity as "read"
        /// </summary>
        /// <remarks>
        /// READ: <a href="https://api.hostname/Announcement">https://api.hostname/Announcement</a>
        /// </remarks>
        /// <returns>A <see cref="IEnumerable{T}"/> containing all instances of <see cref="AnnouncementDto"/> that matched the predicate.</returns>
        public IHttpActionResult MarkAsRead()
        {
            var result = this._announcements.Value.MarkAsRead();

            return this.Json(result);
        }
    }
}
