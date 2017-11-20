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
    using SpendManagementApi.Utilities;

    /// <summary>
    /// Manages operations on <see cref="Department">Departments</see>.
    /// </summary>
    [RoutePrefix("Departments")]
    [Version(1)]
    public class DepartmentsV1Controller : ArchivingApiController<Department>
    {
        #region Api Methods

        /// <summary>
        /// Gets all of the available end points from the <see cref="Department">Departments</see> part of the API.
        /// </summary>
        /// <returns>A list of available resource Links</returns>
        [HttpOptions, Route("")]
        public List<Link> Options()
        {
            return base.Links();
        }

        /// <summary>
        /// Gets all <see cref="Department">Departments</see> in the system.
        /// </summary>
        [HttpGet, Route("")]
        [AuthAudit(SpendManagementElement.None, AccessRoleType.View)]
        public GetDepartmentsResponse GetAll()
        {
            return this.GetAll<GetDepartmentsResponse>();
        }

        /// <summary>
        /// Gets all active departments
        /// </summary>
        /// <returns>
        /// The <see cref="DepartmentBasicResponse">DepartmentBasicResponse"</see>
        /// </returns>
        [HttpGet, Route("GetAllActive")]
        [AuthAudit(SpendManagementElement.None, AccessRoleType.View)]
        public DepartmentBasicResponse GetAllActive()
        {
            var response = this.InitialiseResponse<DepartmentBasicResponse>();
            response.List = ((DepartmentRepository)this.Repository).GetAllActive();
            return response;
        }

        /// <summary>
        /// Gets a single <see cref="Department">Department</see>, by its Id.
        /// </summary>
        /// <param name="id">The Id of the item to get.</param>
        /// <returns>A GetDepartmentsResponse object, which will contain a <see cref="Department">Department</see> if found.</returns>
        [HttpGet, Route("{id:int}")]
        [AuthAudit(SpendManagementElement.Departments, AccessRoleType.View)]
        public DepartmentResponse Get([FromUri] int id)
        {
            return this.Get<DepartmentResponse>(id);
        }

        /// <summary>
        /// Finds all <see cref="Department">Departments</see> matching specified criteria.<br/>
        /// Currently available querystring parameters: <br/>Label<br/>
        /// Use SearchOperator=0 to specify an AND query or SearchOperator=1 for an OR query
        /// </summary>
        /// <param name="criteria">Find query</param>
        /// <returns>A GetDepartmentsResponse containing matching <see cref="Department">Departments</see>.</returns>
        [HttpGet, Route("Find")]
        [AuthAudit(SpendManagementElement.Departments, AccessRoleType.View)]
        public GetDepartmentsResponse Find([FromUri] FindDepartmentsRequest criteria)
        {
            var response = this.InitialiseResponse<GetDepartmentsResponse>();
            var conditions = new List<Expression<Func<Department, bool>>>();

            if (criteria == null)
            {
                throw new ArgumentException(ApiResources.ApiErrorMissingCritera);
            }

            if (!string.IsNullOrWhiteSpace(criteria.Label))
            {
                conditions.Add(b => b.Label.ToLower().Contains(criteria.Label.ToLower()));
            }

            if (!string.IsNullOrWhiteSpace(criteria.Description))
            {
                conditions.Add(b => b.Description.ToLower().Contains(criteria.Description.ToLower()));
            }

            response.List = this.RunFindQuery(this.Repository.GetAll().AsQueryable(), criteria, conditions);
            return response;
        }

        /// <summary>
        /// Adds a <see cref="Department">Department</see>.
        /// </summary>
        /// <param name="request">The <see cref="Department">Department</see> to add. <br/>
        /// When adding a new <see cref="Department">Department</see> through the API, the following properties are required:<br/>
        /// Id: Must be set to 0, or the add will throw an error.
        /// Label: Must be set to something meaningful, or the add will throw an error.
        /// </param>
        /// <returns>A DepartmentResponse containing the added <see cref="Department">Department</see>.</returns>
        [Route("")]
        [AuthAudit(SpendManagementElement.Departments, AccessRoleType.Add)]
        public DepartmentResponse Post([FromBody] Department request)
        {
            return this.Post<DepartmentResponse>(request);
        }

        /// <summary>
        /// Edits a <see cref="Department">Department</see>.
        /// </summary>
        /// <param name="id">The Id of the <see cref="Department">Department</see> to edit.</param>
        /// <param name="request">The <see cref="Department">Department</see> to edit.</param>
        /// <returns>A DepartmentResponse containing the edited <see cref="Department">Department</see>.</returns>
        [Route("{id:int}")]
        [AuthAudit(SpendManagementElement.Departments, AccessRoleType.Edit)]
        public DepartmentResponse Put([FromUri] int id, [FromBody] Department request)
        {
            request.Id = id;
            return this.Put<DepartmentResponse>(request);
        }

        /// <summary>
        /// Archives or un-archives a <see cref="Department">Department</see>, depending on what is passed in.
        /// </summary>
        /// <param name="id">The id of the <see cref="Department">Department</see> to be archived/un-archived.</param>
        /// <param name="archive">Whether to archive or un-archive this <see cref="Department">Department</see>.</param>
        /// <returns>A DepartmentResponse containing the freshly archived <see cref="Department">Department</see>.</returns>
        [HttpPatch, Route("{id:int}/Archive/{archive:bool}")]
        [AuthAudit(SpendManagementElement.Departments, AccessRoleType.Edit)]
        public DepartmentResponse Archive(int id, bool archive)
        {
            return this.Archive<DepartmentResponse>(id, archive);
        }

        /// <summary>
        /// Deletes a <see cref="Department">Department</see>.
        /// </summary>
        /// <param name="id">The id of the <see cref="Department">Department</see> to be deleted</param>
        /// <returns>A DepartmentResponse with the item set to null upon a successful delete.</returns>
        [Route("{id:int}")]
        [AuthAudit(SpendManagementElement.Departments, AccessRoleType.Delete)]
        public DepartmentResponse Delete(int id)
        {
            return this.Delete<DepartmentResponse>(id);
        }

        #endregion Api Methods
    }
}
