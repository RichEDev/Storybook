namespace SpendManagementApi.Controllers.Expedite.V1
{
    using System.Collections.Generic;
    using System.Web.Http;
    using System.Web.Http.Description;

    using SpendManagementApi.Attributes;
    using SpendManagementApi.Interfaces;
    using SpendManagementApi.Models.Common;
    using SpendManagementApi.Models.Requests.Expedite;
    using SpendManagementApi.Models.Responses.Expedite;
    using SpendManagementApi.Models.Types.Expedite;
    using SpendManagementApi.Repositories.Expedite;

    using SpendManagementLibrary;
    using System;

    /// <summary>
    /// Contains account specific actions.
    /// </summary>
    [RoutePrefix("Expedite/ExpenseValidationResults")]
    [Version(1)]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class ExpenseValidationResultsV1Controller : BaseApiController<ExpenseValidationResult>
    {
        /// <summary>
        /// Gets ALL of the available end points from the ExpenseValidationResult API.
        /// </summary>
        /// <returns>A list of available Links</returns>
        [HttpOptions]
        public List<Link> Options()
        {
            return this.Links("Options");
        }

        #region ExpenseValidationResult Methods

        /// <summary>
        /// Gets all <see cref="ExpenseValidationResult">ExpenseValidationResult</see> in the system.
        /// </summary>
        [HttpGet, Route("ForExpenseItem/{expenseItemId:int}")]
        [InternalSelenityMethod]
        [AuthAudit(SpendManagementElement.Api, AccessRoleType.View)]
        public GetExpenseValidationResultsResponse GetForExpenseItem([FromUri] int expenseItemId)
        {
            var response = this.InitialiseResponse<GetExpenseValidationResultsResponse>();
            response.List = ((ExpenseValidationResultRepository)this.Repository).GetAllForExpenseItem(expenseItemId);
            return response;
        }

        /// <summary>
        /// Gets a single <see cref="ExpenseValidationResult">ExpenseValidationResult</see>, by its Id.
        /// </summary>
        /// <param name="id">The Id of the item to get.</param>
        /// <returns>A ExpenseValidationResultResponse, containing the <see cref="ExpenseValidationResult">ExpenseValidationResult</see> if found.</returns>
        [HttpGet, Route("{id:int}")]
        [InternalSelenityMethod]
        [AuthAudit(SpendManagementElement.Api, AccessRoleType.View)]
        public ExpenseValidationResultResponse Get([FromUri] int id)
        {
            cExpenseItem expenseItem;
            var response = this.InitialiseResponse<ExpenseValidationResultResponse>();
            response.Item = ((ExpenseValidationResultRepository)this.Repository).Get(id, out expenseItem);
            return response;
        }

        /// <summary>
        /// Adds the supplied <see cref="ExpenseItemValidationResults">ExpenseItemValidationResults</see>.
        /// </summary>
        /// <param name="request">The ExpenseItemValidationResults to add.</param>
        /// <returns>The newly added ExpenseValidationResult.</returns>
        [HttpPost, Route("ResultsForExpenseItem")]
        [InternalSelenityMethod]
        [AuthAudit(SpendManagementElement.Api, AccessRoleType.Add)]
        public void PostResultsForExpenseItem([FromBody] ExpenseItemValidationResults request)
        {
            ((ExpenseValidationResultRepository)this.Repository).AddResultsForExpenseItem(request);
        }

        /// <summary>
        /// Deletes a <see cref="ExpenseValidationResult">ExpenseValidationResult</see> associated with the current user
        /// </summary>
        /// <param name="id">ExpenseValidationResult Id</param>
        /// <returns>An ExpenseValidationResultResponse with the item set to null upon a successful delete.</returns>
        [Route("{id:int}")]
        [InternalSelenityMethod]
        [AuthAudit(SpendManagementElement.Api, AccessRoleType.Delete)]
        public ExpenseValidationResultResponse Delete(int id)
        {
            var response = this.InitialiseResponse<ExpenseValidationResultResponse>();
            response.Item = ((ExpenseValidationResultRepository)this.Repository).Delete(id);
            return response;
        }

        /// <summary>
        /// Reset the blocked validation progress of blocked opereations
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("ResetTheValidationProgress")]
        [InternalSelenityMethod]
        [AuthAudit(SpendManagementElement.Api, AccessRoleType.Delete)]
        public ExpenseValidationResultResponse ResetTheValidationProgress()
        {
            var response = this.InitialiseResponse<ExpenseValidationResultResponse>();
            ((ExpenseValidationResultRepository)this.Repository).TerminateValidationProgressForBlockedExpenses(this.CurrentUser.AccountID);
            response.IsUpdated = true;
            return response;
        }


        /// <summary>
        /// Gets the <see cref="ExpenseItemValidationResultsResponse">ExpenseItemValidationResultsResponse</see> with details of the validation results for the provided expense Id
        /// </summary>
        /// <param name="expenseItemId">
        /// The expense Item Id.
        /// </param>
        /// <returns>
        /// The <see cref="ExpenseItemValidationResultsResponse">ExpenseItemValidationResultsResponse</see>
        /// </returns>
        [HttpGet, Route("GetExpenseItemValidationResults/{expenseItemId:int}")]
        [AuthAudit(SpendManagementElement.None, AccessRoleType.View)]
        public ExpenseItemValidationResultsResponse GetExpenseItemValidationResults([FromUri] int expenseItemId)
        {
            var response = this.InitialiseResponse<ExpenseItemValidationResultsResponse>();
            response.Results = ((ExpenseValidationResultRepository)this.Repository).GetExpenseItemValidationResults(expenseItemId);       
            return response;
        }

        #endregion ExpenseValidationResult Methods



    }
}