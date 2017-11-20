namespace SpendManagementApi.Controllers.Expedite.V1
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Http;
    using System.Web.Http.Description;

    using SpendManagementApi.Attributes;
    using SpendManagementApi.Interfaces;
    using SpendManagementApi.Models.Common;
    using SpendManagementApi.Models.Responses.Expedite;
    using SpendManagementApi.Models.Types.Expedite;
    using SpendManagementApi.Repositories.Expedite;

    /// <summary>
    /// Contains account specific actions.
    /// </summary>
    [RoutePrefix("Expedite/ExpenseValidationCriteria")]
    [Version(1)]
    [InternalSelenityMethod, ApiExplorerSettings(IgnoreApi = true)]
    public class ExpenseValidationCriteriaV1Controller : BaseApiController<ExpenseValidationCriterion>
    {
        /// <summary>
        /// Gets ALL of the available end points from the ExpenseValidationCriterion API.
        /// </summary>
        /// <returns>A list of available Links</returns>
        [HttpOptions]
        public List<Link> Options()
        {
            return this.Links("Options");
        }

        #region ExpenseValidationCriterion Methods

        /// <summary>
        /// Gets all <see cref="ExpenseValidationCriterion">ExpenseValidationCriteria</see> in the system.
        /// </summary>
        [HttpGet, Route("")]
        [AuthAudit(SpendManagementElement.Api, AccessRoleType.View)]
        public GetExpenseValidationCriteriaResponse GetAll()
        {
            return this.GetAll<GetExpenseValidationCriteriaResponse>();
        }

        /// <summary>
        /// Gets all <see cref="ExpenseValidationCriterion">ExpenseValidationCriteria</see> that must be validated
        /// for a particular expense item. These include any custom, account created subcat criteria.
        /// </summary>
        [HttpGet, Route("ForExpenseItem/{expenseItemId:int}")]
        [AuthAudit(SpendManagementElement.Api, AccessRoleType.View)]
        public GetExpenseValidationCriteriaResponse GetForExpenseItem(int expenseItemId)
        {
            var response = this.InitialiseResponse<GetExpenseValidationCriteriaResponse>();
            response.List = ((ExpenseValidationCriterionRepository) this.Repository).GetForExpenseItem(expenseItemId).ToList();
            return response;
        }
        
        /// <summary>
        /// Gets a single <see cref="ExpenseValidationCriterion">ExpenseValidationCriterion</see>, by its Id.
        /// </summary>
        /// <param name="id">The Id of the item to get.</param>
        /// <returns>A ExpenseValidationCriterionResponse, containing the <see cref="ExpenseValidationCriterion">ExpenseValidationCriterion</see> if found.</returns>
        [HttpGet, Route("{id:int}")]
        [AuthAudit(SpendManagementElement.Api, AccessRoleType.View)]
        public ExpenseValidationCriterionResponse Get([FromUri] int id)
        {
            return this.Get<ExpenseValidationCriterionResponse>(id);
        }

        /// <summary>
        /// Adds the supplied <see cref="ExpenseValidationCriterion">ExpenseValidationCriterion</see>.
        /// </summary>
        /// <param name="request">The ExpenseValidationCriterion to add.</param>
        /// <returns>The newly added ExpenseValidationCriterion.</returns>
        [Route("")]
        [AuthAudit(SpendManagementElement.Api, AccessRoleType.Add)]
        public ExpenseValidationCriterionResponse Post([FromBody] ExpenseValidationCriterion request)
        {
            return this.Post<ExpenseValidationCriterionResponse>(request);
        }
        
        /// <summary>
        /// Edits an existing <see cref="ExpenseValidationCriterion">ExpenseValidationCriterion</see>.
        /// </summary>
        /// <param name="id">The Id of the ExpenseValidationCriterion to modify</param>
        /// <param name="request">The ExpenseValidationCriterion to modify.</param>
        /// <returns>The updated ExpenseValidationCriterion.</returns>
        [Route("{id:int}")]
        [AuthAudit(SpendManagementElement.Api, AccessRoleType.Edit)]
        public ExpenseValidationCriterionResponse Put([FromUri] int id, [FromBody] ExpenseValidationCriterion request)
        {
            request.Id = id;
            return this.Put<ExpenseValidationCriterionResponse>(request);
        }

        /// <summary>
        /// Deletes a <see cref="ExpenseValidationCriterion">ExpenseValidationCriterion</see> associated with the current user
        /// </summary>
        /// <param name="id">ExpenseValidationCriterion Id</param>
        /// <returns>An ExpenseValidationCriterionResponse with the item set to null upon a successful delete.</returns>
        [Route("{id:int}")]
        [AuthAudit(SpendManagementElement.Api, AccessRoleType.Delete)]
        public ExpenseValidationCriterionResponse Delete(int id)
        {
            return this.Delete<ExpenseValidationCriterionResponse>(id);
        }

        #endregion ExpenseValidationCriterion Methods

    }
}