namespace PublicAPI.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Web.Http;

    using BusinessLogic.DataConnections;
    using BusinessLogic.Reasons;

    using PublicAPI.Common.Actions;
    using PublicAPI.DTO;
    using PublicAPI.Security.Filters;

    /// <summary>
    /// Controller enabling actions to be executed on <see cref="IReason"/> using the <see cref="ReasonDTO"/>.
    /// </summary>
    [JwtAuthentication]
    public class ReasonsController : ApiController, ICrud<ReasonDTO, int>, IArchive<int>
    {
        private readonly Lazy<IDataFactoryArchivable<IReason, int, int>> _reasons = new Lazy<IDataFactoryArchivable<IReason, int, int>>(() => WebApiApplication.container.GetInstance<IDataFactoryArchivable<IReason, int, int>>());

        /// <summary>
        /// Controller action to get all available instances of <see cref="ReasonDTO"/>.
        /// </summary>
        /// <remarks>
        /// GET: <a href="https://api.hostname/Reasons">https://api.hostname/Reasons</a>
        /// </remarks>
        /// <returns>A <see cref="IEnumerable{T}"/> containing all instances of <see cref="ReasonDTO"/>.</returns>
        public IHttpActionResult Get()
        {
            IEnumerable<IReason> reasons = this._reasons.Value.Get();
            IEnumerable<ReasonDTO> reasonsDtoCollection = MapObjects.Map<IEnumerable<ReasonDTO>>(reasons);

            return this.Json(reasonsDtoCollection);
        }

        /// <summary>
        /// Controller action to get a specific <see cref="ReasonDTO"/> with a matching <see cref="int"/> Id.
        /// </summary>
        /// <remarks>
        /// GET: <a href="https://api.hostname/Reasons/{id}">https://api.hostname/Reasons/{id}</a>
        /// </remarks>
        /// <param name="id">The id to match on</param>
        /// <returns>An instance of <see cref="ReasonDTO"/> with a matching id, of nothing is not matched.</returns>
        public IHttpActionResult Get(int id)
        {
            ReasonDTO reason = MapObjects.Map<ReasonDTO>(this._reasons.Value[id]);

            return this.Json(reason);
        }

        /// <summary>
        /// Controller action to add (or update) an instance of <see cref="ReasonDTO"/>.
        /// </summary>
        /// <remarks>
        /// POST: <a href="https://api.hostname/Reasons">https://api.hostname/Reasons</a>
        ///  Body: <see cref="ReasonDTO"/>
        /// </remarks>
        /// <param name="value">The <see cref="ReasonDTO"/> to update with it's associated values.</param>
        /// <returns>An instance of <see cref="ReasonDTO"/> with updated properties post add (or update).</returns>
        public IHttpActionResult Post([FromBody]ReasonDTO value)
        {
            IReason reason = MapObjects.Map<Reason>(value);
            reason = this._reasons.Value.Save(reason);
            value = MapObjects.Map<ReasonDTO>(reason);

            return this.Json(value);
        }

        /// <summary>
        /// Controller action to add (or update) an instance of <see cref="ReasonDTO"/>.
        /// </summary>
        /// <remarks>
        /// PUT: <a href="https://api.hostname/Reasons">https://api.hostname/Reasons</a>
        ///  Body: <see cref="ReasonDTO"/>
        /// </remarks>
        /// <param name="value">The <see cref="ReasonDTO"/> to update with it's associated values.</param>
        /// <returns>An instance of <see cref="ReasonDTO"/> with updated properties post add (or update).</returns>
        public IHttpActionResult Put([FromBody]ReasonDTO value)
        {
            IReason reason = MapObjects.Map<Reason>(value);
            reason = this._reasons.Value.Save(reason);
            value = MapObjects.Map<ReasonDTO>(reason);

            return this.Json(value);
        }

        /// <summary>
        /// Controller action to delete a specific <see cref="ReasonDTO"/> with a matching <see cref="int"/> Id.
        /// </summary>
        /// <remarks>
        /// DELETE: <a href="https://api.hostname/Reasons/{id}">https://api.hostname/Reasons/{id}</a>
        /// </remarks>
        /// <param name="id">The id to match on</param>
        /// <returns>An <see cref="int"/> code with the response of the delete.</returns>
        public IHttpActionResult Delete(int id)
        {
            return this.Json(this._reasons.Value.Delete(id));
        }

        /// <summary>
        /// Controller action to archive an object.
        /// </summary>
        /// <remarks>
        /// PUT: <a href="https://api.hostname/Reasons/Archive/{id}">https://api.hostname/Reasons/Archive/{id}</a>
        /// </remarks>
        /// <param name="id">The id of the object to archive.</param>
        /// <returns>A <see cref="bool"/> exposing the current archive state after execution.</returns>
        [Route("Reasons/Archive/{id}"), HttpPut]
        public IHttpActionResult Archive(int id)
        {
            return this.Json(this._reasons.Value.Archive(id));
        }
    }
}