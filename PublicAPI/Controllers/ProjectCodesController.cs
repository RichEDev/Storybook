namespace PublicAPI.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Web.Http;
    using System.Web.Http.Results;
    using BusinessLogic.DataConnections;
    using BusinessLogic.ProjectCodes;

    using Common.Actions;
    using DTO;
    using Security.Filters;
    
    /// <summary>
    /// Controller enabling actions to be executed on <see cref="IProjectCodeWithUserDefinedFields"/> using the <see cref="ProjectCodeDto"/>.
    /// </summary>
    [JwtAuthentication]
    public class ProjectCodesController : ApiController, ICrud<ProjectCodeDto, int>, IArchive<int>
    {
        private readonly Lazy<IDataFactoryCustom<IProjectCodeWithUserDefinedFields, int>> _projectCodes = new Lazy<IDataFactoryCustom<IProjectCodeWithUserDefinedFields, int>>(() => WebApiApplication.container.GetInstance<IDataFactoryCustom<IProjectCodeWithUserDefinedFields, int>>());

        /// <summary>
        /// Controller action to get all available instances of <see cref="ProjectCodeDto"/>.
        /// </summary>
        /// <remarks>
        /// GET: <a href="https://api.hostname/ProjectCodes">https://api.hostname/ProjectCodes</a>
        /// </remarks>
        /// <returns>A <see cref="IEnumerable{T}"/> containing all instances of <see cref="ProjectCodeDto"/>.</returns>
        public IHttpActionResult Get()
        {
            IEnumerable<IProjectCodeWithUserDefinedFields> projectCodes = this._projectCodes.Value.Get();
            IEnumerable<ProjectCodeDto> projectCodesDtoCollection = MapObjects.Map<IEnumerable<ProjectCodeDto>>(projectCodes);

            return this.Json(projectCodesDtoCollection);
        }

        /// <summary>
        /// Controller action to get a specific <see cref="ProjectCodeDto"/> with a matching <see cref="int"/> Id.
        /// </summary>
        /// <remarks>
        /// GET: <a href="https://api.hostname/ProjectCodes/{id}">https://api.hostname/ProjectCodes/{id}</a>
        /// </remarks>
        /// <param name="id">The id to match on</param>
        /// <returns>An instance of <see cref="ProjectCodeDto"/> with a matching id, of nothing is not matched.</returns>
        public IHttpActionResult Get(int id)
        {
            ProjectCodeDto projectCode = MapObjects.Map<ProjectCodeDto>(this._projectCodes.Value[id]);

            return this.Json(projectCode);
        }

        /// <summary>
        /// Controller action to add (or update) an instance of <see cref="ProjectCodeDto"/>.
        /// </summary>
        /// <remarks>
        /// POST: <a href="https://api.hostname/ProjectCodes">https://api.hostname/ProjectCodes</a>
        ///  Body: <see cref="ProjectCodeDto"/>
        /// </remarks>
        /// <param name="value">The <see cref="ProjectCodeDto"/> to update with it's associated values.</param>
        /// <returns>An instance of <see cref="ProjectCodeDto"/> with updated properties post add (or update).</returns>
        public IHttpActionResult Post([FromBody]ProjectCodeDto value)
        {
            IProjectCodeWithUserDefinedFields projectCode = MapObjects.Map<ProjectCodeWithUserDefinedFields>(value);
            projectCode = this._projectCodes.Value.Add(projectCode);
            value = MapObjects.Map<ProjectCodeDto>(projectCode);
            
            return this.Json(value);
        }

        /// <summary>
        /// Controller action to add (or update) an instance of <see cref="ProjectCodeDto"/>.
        /// </summary>
        /// <remarks>
        /// PUT: <a href="https://api.hostname/ProjectCodes">https://api.hostname/ProjectCodes</a>
        ///  Body: <see cref="ProjectCodeDto"/>
        /// </remarks>
        /// <param name="value">The <see cref="ProjectCodeDto"/> to update with it's associated values.</param>
        /// <returns>An instance of <see cref="ProjectCodeDto"/> with updated properties post add (or update).</returns>
        public IHttpActionResult Put([FromBody]ProjectCodeDto value)
        {
            IProjectCodeWithUserDefinedFields projectCode = MapObjects.Map<ProjectCodeWithUserDefinedFields>(value);
            projectCode = this._projectCodes.Value.Add(projectCode);
            value = MapObjects.Map<ProjectCodeDto>(projectCode);

            return this.Json(value);
        }

        /// <summary>
        /// Controller action to delete a specific <see cref="ProjectCodeDto"/> with a matching <see cref="int"/> Id.
        /// </summary>
        /// <remarks>
        /// DELETE: <a href="https://api.hostname/ProjectCodes/{id}">https://api.hostname/ProjectCodes/{id}</a>
        /// </remarks>
        /// <param name="id">The id to match on</param>
        /// <returns>An <see cref="int"/> code with the response of the delete.</returns>
        public IHttpActionResult Delete(int id)
        {
            return this.Json(this._projectCodes.Value.Delete(id));
        }

        /// <summary>
        /// Controller action to archive an object.
        /// </summary>
        /// <remarks>
        /// PUT: <a href="https://api.hostname/ProjectCodes/Archive/{id}">https://api.hostname/ProjectCodes/Archive/{id}</a>
        /// </remarks>
        /// <param name="id">The id of the object to archive.</param>
        /// <returns>A <see cref="bool"/> exposing the current archive state after execution.</returns>
        [Route("ProjectCodes/Archive/{id}"), HttpPut]
        public IHttpActionResult Archive(int id)
        {
            return this.Json(this._projectCodes.Value.Archive(id));
        }
    }
}
