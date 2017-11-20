namespace SpendManagementApi.Controllers.V1
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Web.Http;

    using SpendManagementApi.Attributes;
    using SpendManagementApi.Models.Common;
    using SpendManagementApi.Models.Requests;
    using SpendManagementApi.Models.Responses;
    using SpendManagementApi.Models.Types;
    using SpendManagementApi.Repositories;

    /// <summary>
    /// Manages operations on <see cref="ProjectCode">ProjectCodes</see>
    /// </summary>
    [RoutePrefix("ProjectCodes")]
    [Version(1)]
    public class ProjectCodesV1Controller : ArchivingApiController<ProjectCode>
    {
        #region Api Methods

        /// <summary>
        /// Gets all of the available end points from the <see cref="ProjectCode">ProjectCodes</see> part of the API.
        /// </summary>
        /// <returns>A list of available resource Links</returns>
        [HttpOptions, Route("")]
        public List<Link> Options()
        {
            return base.Links();
        }

        /// <summary>
        /// Gets all <see cref="ProjectCode">ProjectCodes</see> in the system.
        /// </summary>
        /// <returns>A GetProjectCodeResponse containing all <see cref="ProjectCode">ProjectCodes</see>.</returns>
        [HttpGet, Route("")]
        [AuthAudit(SpendManagementElement.None, AccessRoleType.View)]
        public GetProjectCodesResponse GetAll()
        {
            return this.GetAll<GetProjectCodesResponse>();
        }

        /// <summary>
        /// Gets a single <see cref="ProjectCode">ProjectCode</see>, by its Id.
        /// </summary>
        /// <param name="id">The Id of the <see cref="ProjectCode">ProjectCode</see> to get.</param>
        /// <returns>A ProjectCodeResponse containing the matching <see cref="ProjectCode">ProjectCode</see>.</returns>
        [HttpGet, Route("{id:int}")]
        [AuthAudit(SpendManagementElement.ProjectCodes, AccessRoleType.View)]
        public ProjectCodeResponse Get([FromUri] int id)
        {
            return this.Get<ProjectCodeResponse>(id);
        }

        /// <summary>
        /// Gets all active project codes
        /// </summary>
        /// <returns>
        /// The <see cref="ProjectCodeBasicResponse">ProjectCodeBasicResponse"</see>
        /// </returns>
        [HttpGet, Route("GetAllActive")]
        [AuthAudit(SpendManagementElement.None, AccessRoleType.View)]
        public ProjectCodeBasicResponse GetAllActive()
        {
            var response = this.InitialiseResponse<ProjectCodeBasicResponse>();
            response.List = ((ProjectCodeRepository)this.Repository).GetAllActive();
            return response;
        }

        /// <summary>
        /// Finds all <see cref="ProjectCode">ProjectCodes</see> matching specified criteria.<br/>
        /// Currently available querystring parameters: <br/>Label<br/>
        /// Use SearchOperator=0 to specify an AND query or SearchOperator=1 for an OR query
        /// </summary>
        /// <param name="criteria">Find query</param>
        /// <returns>A GetProjectCodeResponse containing matching <see cref="ProjectCode">ProjectCodes</see>.</returns>
        [HttpGet, Route("Find")]
        [AuthAudit(SpendManagementElement.ProjectCodes, AccessRoleType.View)]
        public GetProjectCodesResponse Find([FromUri] FindProjectCodesRequest criteria)
        {
            var response = this.InitialiseResponse<GetProjectCodesResponse>();
            var conditions = new List<Expression<Func<ProjectCode, bool>>>();

            if (criteria == null)
            {
                throw new ArgumentException();
            }

            if (!string.IsNullOrWhiteSpace(criteria.Label))
            {
                conditions.Add(b => b.Label.ToLower().Contains(criteria.Label.Trim().ToLower()));
            }

            if (!string.IsNullOrWhiteSpace(criteria.Description))
            {
                conditions.Add(b => b.Description.ToLower().Contains(criteria.Description.Trim().ToLower()));
            }

            response.List = this.RunFindQuery(this.Repository.GetAll().AsQueryable(), criteria, conditions);
            return response;
        }

        /// <summary>
        /// Adds a <see cref="ProjectCode">ProjectCode</see>.
        /// </summary>
        /// <param name="request">The <see cref="ProjectCode">ProjectCode</see> to add. <br/>
        /// When adding a new ProjectCode through the API, the following properties are required:<br/>
        /// Id: Must be set to 0, or the add will throw an error.
        /// Label: Must be set to something meaningful, or the add will throw an error.
        /// </param>
        /// <returns>A ProjectCodeResponse containing the added <see cref="ProjectCode">ProjectCode</see>.</returns>
        [Route("")]
        [AuthAudit(SpendManagementElement.ProjectCodes, AccessRoleType.Add)]
        public ProjectCodeResponse Post([FromBody] ProjectCode request)
        {
            return this.Post<ProjectCodeResponse>(request);
        }

        /// <summary>
        /// Edits a <see cref="ProjectCode">ProjectCode</see>.
        /// </summary>
        /// <param name="id">The Id of the <see cref="ProjectCode">ProjectCode</see> to edit.</param>
        /// <param name="request">The <see cref="ProjectCode">ProjectCode</see> to edit.</param>
        /// <returns>A ProjectCodeResponse containing the edited <see cref="ProjectCode">ProjectCode</see>.</returns>
        [Route("{id:int}")]
        [AuthAudit(SpendManagementElement.ProjectCodes, AccessRoleType.Edit)]
        public ProjectCodeResponse Put([FromUri] int id, [FromBody] ProjectCode request)
        {
            request.Id = id;
            return this.Put<ProjectCodeResponse>(request);
        }

        /// <summary>
        /// Archives or un-archives a <see cref="ProjectCode">ProjectCode</see>, depeding on what is passed in.
        /// </summary>
        /// <param name="id">The id of the <see cref="ProjectCode">ProjectCode</see> to be archived/un-archived.</param>
        /// <param name="archive">Whether to archive or un-archive this <see cref="ProjectCode">ProjectCode</see>.</param>
        /// <returns>A ProjectCodeResponse containing the archived <see cref="ProjectCode">ProjectCode</see>.</returns>
        [HttpPatch, Route("{id:int}/Archive/{archive:bool}")]
        [AuthAudit(SpendManagementElement.ProjectCodes, AccessRoleType.Edit)]
        public ProjectCodeResponse Archive(int id, bool archive)
        {
            return this.Archive<ProjectCodeResponse>(id, archive);
        }

        /// <summary>
        /// Deletes a <see cref="ProjectCode">ProjectCode</see>.
        /// </summary>
        /// <param name="id">The id of the <see cref="ProjectCode">ProjectCode</see> to be deleted</param>
        /// <returns>A ProjectCodeResponse with the item set to null upon a successful delete.</returns>
        [Route("{id:int}")]
        [AuthAudit(SpendManagementElement.ProjectCodes, AccessRoleType.Delete)]
        public ProjectCodeResponse Delete(int id)
        {
            return this.Delete<ProjectCodeResponse>(id);
        }

        #endregion Api Methods
    }
}
