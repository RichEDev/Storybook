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
    using SpendManagementApi.Models.Types.Employees;
    using SpendManagementApi.Repositories;
    using SpendManagementApi.Utilities;

    /// <summary>
    /// Manages the majority of <see cref="AccessRole">AccessRole</see> operations.
    /// </summary>
    [RoutePrefix("AccessRoles")]
    [Version(1)]
    public class AccessRolesV1Controller : BaseApiController<AccessRole>
    {
        #region Api Methods

        /// <summary>
        /// Gets all of the available end points from the <see cref="AccessRole">AccessRole</see> part of the API.
        /// </summary>
        /// <returns>A list of available resource Links</returns>
        [Route(""), HttpOptions]
        public List<Link> Options()
        {
            return base.Links();
        }

        /// <summary>
        /// Gets all <see cref="AccessRole">AccessRoles</see> in the system.
        /// </summary>
        [HttpGet, Route("")]
        [AuthAudit(SpendManagementElement.AccessRoles, AccessRoleType.View)]
        public GetAccessRolesResponse GetAll()
        {
            return this.GetAll<GetAccessRolesResponse>();
        }

        /// <summary>
        /// Gets a single <see cref="AccessRole">AccessRole</see>, by its Id.
        /// </summary>
        /// <param name="id">The Id of the item to get.</param>
        /// <returns>An AccessRolesResponse object containing the matching <see cref="AccessRole">AccessRole</see>.</returns>
        [HttpGet, Route("{id:int}")]
        [AuthAudit(SpendManagementElement.AccessRoles, AccessRoleType.View)]
        public AccessRoleResponse Get([FromUri] int id)
        {
            return this.Get<AccessRoleResponse>(id);
        }

        /// <summary>
        /// Finds all <see cref="AccessRole">AccessRoles</see> matching specified criteria.<br/>
        /// Currently available querystring parameters: <br/>Label, CanEditCostCode, CanEditDepartment, CanEditProjectCode<br/>
        /// Use SearchOperator=0 to specify an AND query or SearchOperator=1 for an OR query
        /// </summary>
        /// <param name="criteria">Find query</param>
        /// <returns>A GetAccessRolesResponse containing matching <see cref="AccessRole">AccessRoles</see>.</returns>
        [HttpGet, Route("Find")]
        [AuthAudit(SpendManagementElement.AccessRoles, AccessRoleType.View)]
        public GetAccessRolesResponse Find([FromUri] FindAccessRolesRequest criteria)
        {
            var response = this.InitialiseResponse<GetAccessRolesResponse>();
            var conditions = new List<Expression<Func<AccessRole, bool>>>();

            if (criteria == null)
            {
                throw new ArgumentException(ApiResources.ApiErrorMissingCritera);
            }

            if (!string.IsNullOrWhiteSpace(criteria.Name))
            {
                conditions.Add(r => r.Label.ToLower().Contains(criteria.Name.ToLower()));
            }

            if (criteria.CanEditCostCode.HasValue)
            {
                conditions.Add(r => r.CanEditCostCode == criteria.CanEditCostCode);
            }

            if (criteria.CanEditDepartment.HasValue)
            {
                conditions.Add(r => r.CanEditDepartment == criteria.CanEditDepartment);
            }

            if (criteria.CanEditProjectCode.HasValue)
            {
                conditions.Add(r => r.CanEditProjectCode == criteria.CanEditProjectCode);
            }

            response.List = this.RunFindQuery(this.Repository.GetAll().AsQueryable(), criteria, conditions);
            return response;
        }

        /// <summary>
        /// Adds an <see cref="AccessRole">AccessRole</see>.
        /// </summary>
        /// <param name="request">The <see cref="AccessRole">AccessRole</see> to add.
        /// <br/> 
        /// When adding a new Access Role through the API, the following properties are required:
        /// <br/> 
        /// Id: Must be set to 0, or the add will throw an error.
        /// Label: Must be set to something meaningful, or the add will throw an error.
        /// ElementAccess: This is a list of all the <see cref="SpendManagementElement">SpendManagementElement</see>s in the system.
        /// </param>
        /// <returns>An AccessRoleResponse containing the added <see cref="AccessRole">AccessRole</see>.</returns>
        [Route("")]
        [AuthAudit(SpendManagementElement.AccessRoles, AccessRoleType.Add)]
        public AccessRoleResponse Post([FromBody] AccessRole request)
        {
            return this.Post<AccessRoleResponse>(request);
        }

        /// <summary>
        /// Edits an <see cref="AccessRole">AccessRole</see>.
        /// </summary>
        /// <param name="id">The Id of the <see cref="AccessRole">AccessRole</see> to edit.</param>
        /// <param name="request">The <see cref="AccessRole">AccessRole</see> to edit.</param>
        /// <returns>An AccessRoleResponse containing the edited <see cref="AccessRole">AccessRole</see>.</returns>
        [Route("{id:int}")]
        [AuthAudit(SpendManagementElement.AccessRoles, AccessRoleType.Edit)]
        public AccessRoleResponse Put([FromUri] int id, [FromBody] AccessRole request)
        {
            request.Id = id;
            return this.Put<AccessRoleResponse>(request);
        }

        /// <summary>
        /// Applies the supplied <see cref="AccessRole">AccessRole</see> to the supplied <see cref="Employee">Employee</see>.
        /// </summary>
        /// <param name="id">The Id of the <see cref="AccessRole">AccessRole</see> to apply.</param>
        /// <param name="eid">The Id of the <see cref="Employee">Employee</see>.</param>
        /// <returns>An EmployeeLinkageResponse, containing the two Ids.</returns>
        [HttpPatch, Route("{id:int}/AssignToEmployee/{eid:int}")]
        [AuthAudit(SpendManagementElement.Employees, AccessRoleType.Edit)]
        public EmployeeLinkageResponse AssignToEmployee([FromUri] int id, [FromUri] int eid)
        {
            var response = this.InitialiseResponse<EmployeeLinkageResponse>();
            ((AccessRoleRepository)this.Repository).UpdateEmployeeAccessRole(eid, id, true, this.Repository.User.CurrentSubAccountId);
            response.EmployeeId = eid;
            response.LinkedItemId = id;
            return response;
        }

        /// <summary>
        /// Unlinks the supplied <see cref="AccessRole">AccessRole</see> from the supplied <see cref="Employee">Employee</see>.
        /// </summary>
        /// <param name="id">The Id of the <see cref="AccessRole">AccessRole</see> to revoke.</param>
        /// <param name="eid">The Id of the <see cref="Employee">Employee</see>.</param>
        /// <returns>An EmployeeLinkageResponse, containing the two Ids.</returns>
        [HttpPatch, Route("{id:int}/RevokeFromEmployee/{eid:int}")]
        [AuthAudit(SpendManagementElement.Employees, AccessRoleType.Edit)]
        public EmployeeLinkageResponse RevokeFromEmployee([FromUri]int id, [FromUri]int eid)
        {
            var response = this.InitialiseResponse<EmployeeLinkageResponse>();
            ((AccessRoleRepository)this.Repository).UpdateEmployeeAccessRole(eid, id, false, this.Repository.User.CurrentSubAccountId);
            response.EmployeeId = eid;
            response.LinkedItemId = id;
            return response;
        }

        /// <summary>
        /// Deletes an <see cref="AccessRole">AccessRole</see>.
        /// </summary>
        /// <param name="id">The id of the <see cref="AccessRole">AccessRole</see> to be deleted</param>
        /// <returns>An AccessRoleResponse with the item set to null upon a successful delete.</returns>
        [Route("{id:int}")]
        [AuthAudit(SpendManagementElement.AccessRoles, AccessRoleType.Delete)]
        public AccessRoleResponse Delete(int id)
        {
            return this.Delete<AccessRoleResponse>(id);
        }

        #endregion Api Methods
    }
}
