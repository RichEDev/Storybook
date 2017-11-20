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
    /// Manages operations on <see cref="Team">Teams</see>.
    /// </summary>
    [RoutePrefix("Teams")]
    [Version(1)]
    public class TeamsV1Controller : BaseApiController<Team>
    {
        #region Api Methods

        /// <summary>
        /// Gets all of the available end points from the <see cref="Team">Teams</see> part of the API.
        /// </summary>
        /// <returns>A list of available resource Links</returns>
        [HttpOptions, Route("")]
        public List<Link> Options()
        {
            return base.Links();
        }

        /// <summary>
        /// Gets all <see cref="Team">Teams</see> in the system.
        /// </summary>
        /// <returns>A GetTeamsResponse containing matching <see cref="Team">Teams</see>.</returns>
        [HttpGet, Route("")]
        [AuthAudit(SpendManagementElement.Teams, AccessRoleType.View)]
        public GetTeamsResponse GetAll()
        {
            return this.GetAll<GetTeamsResponse>();
        }

        /// <summary>
        /// Gets a single <see cref="Team">Team</see>, by its Id.
        /// </summary>
        /// <param name="id">The Id of the <see cref="Team">Team</see> to get.</param>
        /// <returns>A TeamResponse containing the matching <see cref="Team">Team</see>.</returns>
        [HttpGet, Route("{id:int}")]
        [AuthAudit(SpendManagementElement.Teams, AccessRoleType.View)]
        public TeamResponse Get([FromUri] int id)
        {
            return this.Get<TeamResponse>(id);
        }

        /// <summary>
        /// Finds all <see cref="Team">Teams</see> matching specified criteria.<br/>
        /// Currently available querystring parameters: <br/>
        /// Label, Description<br/>
        /// Use SearchOperator=0 to specify an AND query or SearchOperator=1 for an OR query
        /// </summary>
        /// <param name="criteria">Find query</param>
        /// <returns>A GetTeamsResponse containing matching <see cref="Team">Teams</see>.</returns>
        [HttpGet, Route("Find")]
        [AuthAudit(SpendManagementElement.Teams, AccessRoleType.View)]
        public GetTeamsResponse Find([FromUri] FindTeamsRequest criteria)
        {
            var response = this.InitialiseResponse<GetTeamsResponse>();
            var conditions = new List<Expression<Func<Team, bool>>>();

            if (criteria == null)
            {
                throw new ArgumentException(ApiResources.ApiErrorMissingCritera);
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
        /// Adds a <see cref="Team">Team</see>.
        /// </summary>
        /// <param name="request">The <see cref="Team">Team</see> to add. <br/>
        /// When adding a new <see cref="Team">Team</see> through the API, the following properties are required:<br/>
        /// Id: Must be set to 0, or the add will throw an error.
        /// Label: Must be set to something meaningful, or the add will throw an error.
        /// </param>
        /// <returns>A TeamResponse containing the added <see cref="Team">Team</see>.</returns>
        [Route("")]
        [AuthAudit(SpendManagementElement.Teams, AccessRoleType.Add)]
        public TeamResponse Post([FromBody] Team request)
        {
            return this.Post<TeamResponse>(request);
        }

        /// <summary>
        /// Edits a <see cref="Team">Team</see>.
        /// </summary>
        /// <param name="id">The Id of the <see cref="Team">Team</see> to edit.</param>
        /// <param name="request">The <see cref="Team">Team</see> to edit.</param>
        /// <returns>A TeamResponse containing the edited <see cref="Team">Team</see>.</returns>
        [Route("{id:int}")]
        [AuthAudit(SpendManagementElement.Teams, AccessRoleType.Edit)]
        public TeamResponse Put([FromUri] int id, [FromBody] Team request)
        {
            request.Id = id;
            return this.Put<TeamResponse>(request);
        }

        /// <summary>
        /// Modifies the list of employees for a given <see cref="Team">Team</see>.
        /// </summary>
        /// <param name="id">The Id of the <see cref="Team">Team</see> to edit.</param>
        /// <param name="request">The <see cref="Team">Team</see> to edit.</param>
        /// <returns>A TeamResponse containing the edited <see cref="Team">Team</see>.</returns>
        [HttpPatch, Route("{id:int}/ModifyEmployees")]
        [AuthAudit(SpendManagementElement.Teams, AccessRoleType.Edit)]
        public TeamResponse ModifyEmployees([FromUri] int id, [FromBody] TeamLinkToMembersRequest request)
        {
            var response = this.InitialiseResponse<TeamResponse>();
            response.Item = ((TeamRepository)this.Repository).LinkEmployees(id, request.EmployeeIds);
            return response;
        }

        /// <summary>
        /// Changes the team leader of the <see cref="Team">Team</see>.
        /// </summary>
        /// <param name="id">The Id of the <see cref="Team">Team</see> to edit.</param>
        /// <param name="teamLeaderId">The Id of the new team leader.</param>
        /// <returns>A TeamResponse containing the edited <see cref="Team">Team</see>.</returns>
        [HttpPatch, Route("{id:int}/ChangeLeader/{teamLeaderId:int}")]
        [AuthAudit(SpendManagementElement.Teams, AccessRoleType.Edit)]
        public TeamResponse ChangeTeamLeader([FromUri] int id, int teamLeaderId)
        {
            var response = this.InitialiseResponse<TeamResponse>();
            response.Item = ((TeamRepository)this.Repository).ChangeTeamLeader(id, teamLeaderId);
            return response;
        }

        /// <summary>
        /// Deletes a <see cref="Team">Team</see>.
        /// </summary>
        /// <param name="id">The id of the <see cref="Team">Team</see> to be deleted</param>
        /// <returns>A TeamResponse with the item set to null upon a successful delete.</returns>
        [Route("{id:int}")]
        [AuthAudit(SpendManagementElement.Teams, AccessRoleType.Delete)]
        public TeamResponse Delete(int id)
        {
            return this.Delete<TeamResponse>(id);
        }

        #endregion Api Methods
    }

    
}
