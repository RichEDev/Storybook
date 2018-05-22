namespace InternalAPI.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Web.Http;

    using BusinessLogic.Announcements;

    using CacheDataAccess.Caching;

    using InternalApi;
    using InternalApi.Common.Actions;
    using InternalApi.DTO;

    using InternalAPI.Common.Actions;

    /// <summary>
    /// Controller enabling actions to be executed on <see cref="IAnnouncement"/> using the <see cref="AnnouncementDto"/>.
    /// </summary>    
    public class AnnouncementController : ApiController, ICrud<AnnouncementDto, Guid>, IDeleteAll<AnnouncementDto>
    {
        /// <summary>
        /// Lazy loader for an instance of an Announcement
        /// </summary>
        private readonly Lazy<AnnouncementsCacheFactory> _announcements = new Lazy<AnnouncementsCacheFactory>(() => WebApiApplication.Container.GetInstance<AnnouncementsCacheFactory>());

        /// <summary>
        /// Controller action to get all available instances of <see cref="AnnouncementDto"/> that do not contain the predicate
        /// </summary>
        /// <remarks>
        /// GET: <a href="https://api.hostname/Announcement">https://api.hostname/Announcement</a>
        /// </remarks>
        /// <returns>A <see cref="IEnumerable{T}"/> containing all instances of <see cref="AnnouncementDto"/>.</returns>
        public IHttpActionResult Get()
        {
            IEnumerable<IAnnouncement> announcements = this._announcements.Value.Get();
            IEnumerable<AnnouncementDto> announcementsDtoCollection = MapObjects.Map<IEnumerable<AnnouncementDto>>(announcements);

            return this.Json(announcementsDtoCollection);
        }

        /// <summary>
        /// Controller action to get a specific <see cref="AnnouncementDto"/> with a matching <see cref="int"/> Id.
        /// </summary>
        /// <remarks>
        /// GET: <a href="https://api.hostname/Announcement/{id}">https://api.hostname/Announcement/{id}</a>
        /// </remarks>
        /// <param name="id">The id to match on</param>
        /// <returns>An instance of <see cref="AnnouncementDto"/> with a matching id, of nothing is not matched.</returns>
        public IHttpActionResult Get(Guid id)
        {
            AnnouncementDto announcement = MapObjects.Map<AnnouncementDto>(this._announcements.Value[id]);

            return this.Json(announcement);
        }

        /// <summary>
        /// Controller action to add (or update) an instance of <see cref="AnnouncementDto"/>.
        /// </summary>
        /// <remarks>
        /// POST: <a href="https://api.hostname/Announcement">https://api.hostname/Announcement</a>
        ///  Body: <see cref="AnnouncementDto"/>
        /// </remarks>
        /// <param name="value">The <see cref="AnnouncementDto"/> to update with it's associated values.</param>
        /// <returns>An instance of <see cref="AnnouncementDto"/> with updated properties post add (or update).</returns>
        public IHttpActionResult Post([FromBody]AnnouncementDto value)
        {
            IAnnouncement announcement = MapObjects.Map<Announcement>(value);
            announcement = this._announcements.Value.Save(announcement);
            value = MapObjects.Map<AnnouncementDto>(announcement);

            return this.Json(value);
        }

        /// <summary>
        /// Controller action to add (or update) an instance of <see cref="AnnouncementDto"/>.
        /// </summary>
        /// <remarks>
        /// PUT: <a href="https://api.hostname/Announcement">https://api.hostname/Announcement</a>
        ///  Body: <see cref="AnnouncementDto"/>
        /// </remarks>
        /// <param name="value">The <see cref="AnnouncementDto"/> to update with it's associated values.</param>
        /// <returns>An instance of <see cref="AnnouncementDto"/> with updated properties post add (or update).</returns>
        public IHttpActionResult Put(AnnouncementDto value)
        {
            IAnnouncement announcement = MapObjects.Map<Announcement>(value);
            announcement = this._announcements.Value.Save(announcement);
            value = MapObjects.Map<AnnouncementDto>(announcement);

            return this.Json(value);
        }

        /// <summary>
        /// Controller action to delete a specific <see cref="AnnouncementDto"/> with a matching <see cref="int"/> Id.
        /// </summary>
        /// <remarks>
        /// DELETE: <a href="https://api.hostname/Announcement/{id}">https://api.hostname/Announcement/{id}</a>
        /// </remarks>
        /// <param name="id">The id to match on</param>
        /// <returns>An <see cref="Guid"/> code with the response of the delete.</returns>
        public IHttpActionResult Delete(Guid id)
        {
            return this.Json(this._announcements.Value.Delete(id));
        }

        /// <summary>
        /// Delete all the announcements
        /// </summary>
        /// <remarks>
        /// DELETE: <a href="https://api.hostname/Announcement/">https://api.hostname/Announcement/</a>
        /// </remarks>
        /// <returns>True if this operation is sucessful</returns>
        public IHttpActionResult Delete()
        {
            IEnumerable<IAnnouncement> announcements = this._announcements.Value.Get();

            if (announcements == null)
            {
                return this.Json(false);
            }

            foreach (var announcement in announcements)
            {
                this._announcements.Value.Delete(announcement.Id);
            }

            return this.Json(true);
        }
    }
}