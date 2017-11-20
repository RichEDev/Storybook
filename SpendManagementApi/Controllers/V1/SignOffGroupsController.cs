namespace SpendManagementApi.Controllers.V1
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Web.Http;

    using SpendManagementApi.Attributes;
    using SpendManagementApi.Interfaces;
    using SpendManagementApi.Models.Common;
    using SpendManagementApi.Models.Requests;
    using SpendManagementApi.Models.Responses;
    using SpendManagementApi.Models.Types;

    /// <summary>
    /// Manages Operations on <see cref="SignOffGroup">SignOffGroups</see>.
    /// </summary>
    [RoutePrefix("SignOffGroups")]
    [Version(1)]
    public class SignOffGroupsV1Controller : BaseApiController<SignOffGroup>
    {
        /// <summary>
        /// Gets all of the available end points from the <see cref="SignOffGroup">SignOffGroups</see> part of the API.
        /// </summary>
        /// <returns>A list of available resource Links</returns>
        [HttpOptions, Route("")]
        public List<Link> Options()
        {
            return base.Links();
        }

        /// <summary>
        /// Gets all <see cref="SignOffGroup">SignOffGroups</see> in the system.
        /// </summary>
        /// <returns>A GetSignOffGroupsResponse containing all <see cref="SignOffGroup">SignOffGroups</see>.</returns>
        [HttpGet, Route("")]
        [AuthAudit(SpendManagementElement.AccessRoles, AccessRoleType.View)]
        public GetSignOffGroupsResponse GetAll()
        {
            return this.GetAll<GetSignOffGroupsResponse>();
        }

        /// <summary>
        /// Gets a single <see cref="SignOffGroup">SignOffGroup</see>, by its Id.
        /// </summary>
        /// <param name="id">The Id of the <see cref="SignOffGroup">SignOffGroup</see> to get.</param>
        /// <returns>An SignOffGroupsResponse object containing the matching <see cref="SignOffGroup">SignOffGroups</see>.</returns>
        [HttpGet, Route("{id:int}")]
        [AuthAudit(SpendManagementElement.SignOffGroups, AccessRoleType.View)]
        public SignOffGroupResponse Get([FromUri] int id)
        {
            return this.Get<SignOffGroupResponse>(id);
        }

        /// <summary>
        /// Adds a <see cref="SignOffGroup">SignOffGroup</see>
        /// </summary>
        /// <param name="request">The <see cref="SignOffGroup">SignOffGroup</see> to add.</param>
        /// <returns>An SignOffGroupsResponse object containing the added <see cref="SignOffGroup">SignOffGroups</see>.</returns>
        [Route("")]
        [AuthAudit(SpendManagementElement.SignOffGroups, AccessRoleType.Add)]
        public SignOffGroupResponse Post([FromBody]SignOffGroup request)
        {
            return this.Post<SignOffGroupResponse>(request);
        }

        /// <summary>
        /// Edits a <see cref="SignOffGroup">SignOffGroup</see>.
        /// </summary>
        /// <param name="id">The Id of the <see cref="SignOffGroup">SignOffGroup</see> to edit.</param>
        /// <param name="request">The <see cref="SignOffGroup">SignOffGroup</see> to edit</param>
        /// <returns>An SignOffGroupsResponse object containing the edited <see cref="SignOffGroup">SignOffGroups</see>.</returns>
        [Route("{id:int}")]
        [AuthAudit(SpendManagementElement.SignOffGroups, AccessRoleType.Edit)]
        public SignOffGroupResponse Put([FromUri] int id, [FromBody] SignOffGroup request)
        {
            request.GroupId = id;
            return this.Put<SignOffGroupResponse>(request);
        }

        /// <summary>
        /// Deletes a <see cref="SignOffGroup">SignOffGroup</see> with the specified id.
        /// </summary>
        /// <param name="id">The Id of the <see cref="SignOffGroup">SignOffGroup</see></param>
        /// <returns>An SignOffGroupsResponse with the item set to null upon a successful delete.</returns>
        [Route("{id:int}")]
        [AuthAudit(SpendManagementElement.SignOffGroups, AccessRoleType.Delete)]
        public SignOffGroupResponse Delete(int id)
        {
            return this.Delete<SignOffGroupResponse>(id);
        }

        /// <summary>
        /// Finds all <see cref="SignOffGroup">SignOffGroups</see> matching specified criteria. 
        /// Available querystring parameters : SearchOperator,GroupId,GroupName
        /// Use SearchOperator=0 to specify an AND query or SearchOperator=1 for an OR query
        /// </summary>
        /// <param name="criteria">Find query</param>
        /// <returns>A GetSignOffGroupsResponse containing matching <see cref="SignOffGroup">SignOffGroups</see>.</returns>
        [HttpGet, Route("Find")]
        [AuthAudit(SpendManagementElement.SignOffGroups, AccessRoleType.View)]
        public FindSignOffGroupsResponse Find([FromUri]FindSignOffGroupsRequest criteria)
        {
            var findSignOffGroupsResponse = this.InitialiseResponse<FindSignOffGroupsResponse>();
            var conditions = new List<Expression<Func<SignOffGroup, bool>>>();
            
            if (criteria.GroupId.HasValue)
            {
                conditions.Add(group => group.GroupId == criteria.GroupId);
            }

            if (!string.IsNullOrEmpty(criteria.GroupName))
            {
                conditions.Add(group => group.GroupName.ToLower().Contains(criteria.GroupName.Trim().ToLower()));
            }

            findSignOffGroupsResponse.List = this.RunFindQuery(this.Repository.GetAll().AsQueryable(), criteria, conditions);
            return findSignOffGroupsResponse;
        }
    }
    
}