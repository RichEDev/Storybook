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
    using SpendManagementApi.Utilities;

    /// <summary>
    /// Manages operations on <see cref="ClaimReason">ClaimReasons</see>
    /// </summary>
    [RoutePrefix("ClaimReasons")]
    [Version(1)]
    public class ClaimReasonsV1Controller : BaseApiController<ClaimReason>
    {
        #region Api Methods

        /// <summary>
        /// Gets all of the available end points from the <see cref="ClaimReason">ClaimReasons</see> part of the API.
        /// </summary>
        /// <returns>A list of available resource Links</returns>
        [HttpOptions, Route("")]
        public List<Link> Options()
        {
            return base.Links();
        }

        /// <summary>
        /// Gets all <see cref="ClaimReason">ClaimReasons</see> in the system.
        /// </summary>
        [HttpGet, Route("")]
        [AuthAudit(SpendManagementElement.None, AccessRoleType.View)]
        public GetClaimReasonsResponse GetAll()
        {
            return this.GetAll<GetClaimReasonsResponse>();
        }

        /// <summary>
        /// Gets a single <see cref="ClaimReason">ClaimReason</see>, by its Id.
        /// </summary>
        /// <param name="id">The Id of the item to get.</param>
        /// <returns>A GetClaimReasonsResponse object, which will contain a list of zero-to-many List, depending on how the criteria object is configured.</returns>
        [HttpGet, Route("{id:int}")]
        [AuthAudit(SpendManagementElement.None, AccessRoleType.View)]
        public ClaimReasonResponse Get([FromUri] int id)
        {
            return this.Get<ClaimReasonResponse>(id);
        }

        /// <summary>
        /// Finds all <see cref="ClaimReason">ClaimReasons</see> matching specified criteria.<br/>
        /// Currently available querystring parameters: <br/>Label<br/>
        /// Use SearchOperator=0 to specify an AND query or SearchOperator=1 for an OR query
        /// </summary>
        /// <param name="criteria">Find query</param>
        /// <returns>A GetClaimReasonsRolesResponse containing matching <see cref="ClaimReason">ClaimReasons</see>.</returns>
        [HttpGet, Route("Find")]
        [AuthAudit(SpendManagementElement.Reasons, AccessRoleType.View)]
        public GetClaimReasonsResponse Find([FromUri] FindClaimReasonsRequest criteria)
        {
            var response = this.InitialiseResponse<GetClaimReasonsResponse>();
            var conditions = new List<Expression<Func<ClaimReason, bool>>>();

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
        /// Adds a <see cref="ClaimReason">ClaimReason</see>.
        /// </summary>
        /// <param name="request">The <see cref="ClaimReason">ClaimReason</see> to add.
        /// When adding a new <see cref="ClaimReason">ClaimReason</see> through the API, the following properties are required:
        /// <br/>
        /// Id: Must be set to 0, or the add will throw an error.
        /// Label: Must be set to something meaningful, or the add will throw an error.
        /// </param>
        /// <returns>A ClaimReasonResponse containing the created <see cref="ClaimReason">ClaimReason</see>.</returns>
        [Route("")]
        [AuthAudit(SpendManagementElement.Reasons, AccessRoleType.Add)]
        public ClaimReasonResponse Post([FromBody] ClaimReason request)
        {
            return this.Post<ClaimReasonResponse>(request);
        }

        /// <summary>
        /// Edits a <see cref="ClaimReason">ClaimReason</see>.
        /// </summary>
        /// <param name="id">The Id of the <see cref="ClaimReason">ClaimReason</see> to edit.</param>
        /// <param name="request">The <see cref="ClaimReason">ClaimReason</see> to edit.</param>
        /// <returns>A ClaimReasonResponse containing the edited <see cref="ClaimReason">ClaimReason</see>.</returns>
        [Route("{id:int}")]
        [AuthAudit(SpendManagementElement.Reasons, AccessRoleType.Edit)]
        public ClaimReasonResponse Put([FromUri] int id, [FromBody] ClaimReason request)
        {
            request.Id = id;
            return this.Put<ClaimReasonResponse>(request);
        }

        /// <summary>
        /// Deletes a <see cref="ClaimReason">ClaimReason</see>.
        /// </summary>
        /// <param name="id">The id of the <see cref="ClaimReason">ClaimReason</see> to be deleted</param>
        /// <returns>A ClaimReasonResponse with the item set to null upon a successful delete.</returns>
        [Route("{id:int}")]
        [AuthAudit(SpendManagementElement.Reasons, AccessRoleType.Delete)]
        public ClaimReasonResponse Delete(int id)
        {
            return this.Delete<ClaimReasonResponse>(id);
        }

        #endregion Api Methods
    }

    
}
