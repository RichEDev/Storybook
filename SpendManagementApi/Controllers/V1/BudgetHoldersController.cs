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
    /// Manages operations on <see cref="BudgetHolder">BudgetHolders</see>
    /// </summary>
    [RoutePrefix("BudgetHolders")]
    [Version(1)]
    public class BudgetHoldersV1Controller : BaseApiController<BudgetHolder>
    {
        #region Api Methods

        /// <summary>
        /// Gets all of the available end points from the <see cref="BudgetHolder">BudgetHolders</see> part of the API.
        /// </summary>
        /// <returns>A list of available resource Links</returns>
        [HttpOptions, Route("")]
        public List<Link> Options()
        {
            return base.Links();
        }

        /// <summary>
        /// Gets all <see cref="BudgetHolder">BudgetHolders</see> in the system.
        /// </summary>
        [HttpGet, Route("")]
        [AuthAudit(SpendManagementElement.BudgetHolders, AccessRoleType.View)]
        public GetBudgetHoldersResponse GetAll()
        {
            return this.GetAll<GetBudgetHoldersResponse>();
        }

        /// <summary>
        /// Gets a single <see cref="BudgetHolder">BudgetHolder</see>, by its Id.
        /// </summary>
        /// <param name="id">The Id of the item to get.</param>
        /// <returns>A BudgetHolderResponse, containing the <see cref="BudgetHolder">BudgetHolder</see> if found.</returns>
        [HttpGet, Route("{id:int}")]
        [AuthAudit(SpendManagementElement.BudgetHolders, AccessRoleType.View)]
        public BudgetHolderResponse Get([FromUri] int id)
        {
            return this.Get<BudgetHolderResponse>(id);
        }

        /// <summary>
        /// Finds all <see cref="BudgetHolder">BudgetHolder</see> matching specified criteria.<br/>
        /// Currently available querystring parameters: <br/>Label<br/>
        /// Use SearchOperator=0 to specify an AND query or SearchOperator=1 for an OR query
        /// </summary>
        /// <param name="criteria">Find query</param>
        /// <returns>A GetBudgetHoldersRolesResponse containing matching <see cref="BudgetHolder">BudgetHolders</see>.</returns>
        [HttpGet, Route("Find")]
        [AuthAudit(SpendManagementElement.BudgetHolders, AccessRoleType.View)]
        public GetBudgetHoldersResponse Find([FromUri] FindBudgetHoldersRequest criteria)
        {
            var response = this.InitialiseResponse<GetBudgetHoldersResponse>();
            var conditions = new List<Expression<Func<BudgetHolder, bool>>>();

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

            if (criteria.EmployeeId != null)
            {
                conditions.Add(b => b.EmployeeId == criteria.EmployeeId);
            }

            response.List = this.RunFindQuery(this.Repository.GetAll().AsQueryable(), criteria, conditions);
            return response;
        }

        /// <summary>
        /// Adds a <see cref="BudgetHolder">BudgetHolder</see>.
        /// </summary>
        /// <param name="request">The <see cref="BudgetHolder">BudgetHolder</see> to add.
        /// When adding a new <see cref="BudgetHolder">BudgetHolder</see> through the API, the following properties are required: 
        /// Id: Must be set to 0, or the add will throw an error.
        /// Label: Must be set to something meaningful, or the add will throw an error.
        /// EmployeeId: The Id of the Employee to link as the budget holder.
        /// </param>
        /// <returns>A BudgetHolderResponse.</returns>
        [Route("")]
        [AuthAudit(SpendManagementElement.BudgetHolders, AccessRoleType.Add)]
        public BudgetHolderResponse Post([FromBody] BudgetHolder request)
        {
            return this.Post<BudgetHolderResponse>(request);
        }

        /// <summary>
        /// Edits a <see cref="BudgetHolder">BudgetHolder</see>.
        /// </summary>
        /// <param name="id">The Id of the Item to edit.</param>
        /// <param name="request">The Item to edit.</param>
        /// <returns>The edited Budget Holder</returns>
        [Route("{id:int}")]
        [AuthAudit(SpendManagementElement.BudgetHolders, AccessRoleType.Edit)]
        public BudgetHolderResponse Put([FromUri] int id, [FromBody] BudgetHolder request)
        {
            request.Id = id;
            return this.Put<BudgetHolderResponse>(request);
        }

        /// <summary>
        /// Deletes a <see cref="BudgetHolder">BudgetHolder</see>.
        /// </summary>
        /// <param name="id">The id of the <see cref="BudgetHolder">BudgetHolder</see> to be deleted.</param>
        /// <returns>A BudgetHolderResponse with the item set to null upon a successful delete.</returns>
        [Route("{id:int}")]
        [AuthAudit(SpendManagementElement.BudgetHolders, AccessRoleType.Delete)]
        public BudgetHolderResponse Delete(int id)
        {
            return this.Delete<BudgetHolderResponse>(id);
        }

        #endregion Api Methods
    }

    
}
