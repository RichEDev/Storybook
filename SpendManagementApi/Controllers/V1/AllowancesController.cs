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
    /// Manages operations on <see cref="Allowance">Allowances</see>.
    /// </summary>
    [RoutePrefix("Allowances")]
    [Version(1)]
    public class AllowancesV1Controller : BaseApiController<Allowance>
    {
        #region Api Methods

        /// <summary>
        /// Gets all of the available end points from the <see cref="Allowance">Allowances</see> part of the API.
        /// </summary>
        /// <returns>A list of available resource Links</returns>
        [HttpOptions, Route("")]
        public List<Link> Options()
        {
            return base.Links();
        }

        /// <summary>
        /// Gets all <see cref="Allowance">Allowances</see> in the system.
        /// </summary>
        [HttpGet, Route("")]
        [AuthAudit(SpendManagementElement.None, AccessRoleType.View)]
        public GetAllowancesResponse GetAll()
        {
            return this.GetAll<GetAllowancesResponse>();
        }

        /// <summary>
        /// Gets a single <see cref="Allowance">Allowance</see>, by its Id.
        /// </summary>
        /// <param name="id">The Id of the item to get.</param>
        /// <returns>An AllowancesResponse object, which will contain an <see cref="Allowance">Allowance</see> if one was found.</returns>
        [HttpGet, Route("{id:int}")]
        [AuthAudit(SpendManagementElement.None, AccessRoleType.View)]
        public AllowanceResponse Get([FromUri] int id)
        {
            return this.Get<AllowanceResponse>(id);
        }

        /// <summary>
        /// Finds all <see cref="Allowance">Allowances</see> matching specified criteria.<br/>
        /// Currently available querystring parameters: <br/>
        /// Label, Description<br/>
        /// Use SearchOperator=0 to specify an AND query or SearchOperator=1 for an OR query
        /// </summary>
        /// <param name="criteria">Find query</param>
        /// <returns>A GetAllowancesRolesResponse containing matching <see cref="Allowance">Allowances</see>.</returns>
        [HttpGet, Route("Find")]
        [AuthAudit(SpendManagementElement.Allowances, AccessRoleType.View)]
        public GetAllowancesResponse Find([FromUri] FindAllowancesRequest criteria)
        {
            var response = this.InitialiseResponse<GetAllowancesResponse>();
            var conditions = new List<Expression<Func<Allowance, bool>>>();

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
        /// Adds an <see cref="Allowance">Allowance</see>.
        /// </summary>
        /// <param name="request">The <see cref="Allowance">Allowance</see> to add. <br/>
        /// When adding a new <see cref="Allowance">Allowance</see> through the API, the following properties are required:<br/>
        /// Id: Must be set to 0, or the add will throw an error.
        /// Label: Must be set to something meaningful, or the add will throw an error.
        /// </param>
        /// <returns>An AllowanceResponse.</returns>
        [Route("")]
        [AuthAudit(SpendManagementElement.Allowances, AccessRoleType.Add)]
        public AllowanceResponse Post([FromBody] Allowance request)
        {
            return this.Post<AllowanceResponse>(request);
        }

        /// <summary>
        /// Edits an <see cref="Allowance">Allowance</see>.
        /// </summary>
        /// <param name="id">The Id of the Item to edit.</param>
        /// <param name="request">The Item to edit.</param>
        /// <returns>The edited <see cref="Allowance">Allowance</see> in an AllowanceResponse.</returns>
        [Route("{id:int}")]
        [AuthAudit(SpendManagementElement.Allowances, AccessRoleType.Edit)]
public AllowanceResponse Put([FromUri] int id, [FromBody] Allowance request)
        {
            request.Id = id;
            return this.Put<AllowanceResponse>(request);
        }

        /// <summary>
        /// Deletes an <see cref="Allowance">Allowance</see>.
        /// </summary>
        /// <param name="id">The id of the <see cref="Allowance">Allowance</see> to be deleted.</param>
        /// <returns>An AllowanceResponse with the item set to null upon a successful delete.</returns>
        [Route("{id:int}")]
        [AuthAudit(SpendManagementElement.Allowances, AccessRoleType.Delete)]
        public AllowanceResponse Delete(int id)
        {
            return this.Delete<AllowanceResponse>(id);
        }

        #endregion Api Methods
    }

    
}
