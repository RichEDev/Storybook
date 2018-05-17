namespace SpendManagementApi.Controllers.V1
{
    using System.Collections.Generic;
    using System.Web.Http;
    using System.Web.Http.Description;

    using Attributes;
    using Models.Common;
    using Models.Responses;
    using Models.Types;
    using Repositories;
    using SpendManagementLibrary;
    using Models.Requests;
    using Common.Enums;
    using Utilities;
    using SpendManagementApi.Common;
    using SpendManagementApi.Models;
    using SpendManagementApi.Models.Responses.Expedite;
    using SpendManagementApi.Models.Types.Expedite;
    /// <summary>
    /// Manages operations on <see cref="ExpenseItem">ExpenseItems</see>.
    /// </summary>
    [RoutePrefix("ExpenseItems")]
    [Version(1)]
    public class ExpenseItemsV1Controller : BaseApiController<ExpenseItem>
    {
        #region Api Methods

        /// <summary>
        /// Gets all of the available end points from the <see cref="ExpenseItem">ExpenseItems</see> part of the API.
        /// </summary>
        /// <returns>A list of available resource Links</returns>
        [HttpOptions, Route("")]
        public List<Link> Options()
        {
            return base.Links();
        }

        /// <summary>
        /// Gets a single <see cref="ExpenseItem">ExpenseItem</see>, by its Id.
        /// </summary>
        /// <param name="id">The Id of the item to get.</param>
        /// <returns>A ExpenseItemResponse, containing the <see cref="ExpenseItem">ExpenseItem</see> if found.</returns>
        [HttpGet, Route("{id:int}")]
        [AuthAudit(SpendManagementElement.None, AccessRoleType.View)]
        public ExpenseItemResponse Get([FromUri] int id)
        {
            return this.Get<ExpenseItemResponse>(id);
        }

        /// <summary>
        /// Gets a single <see cref="ExpenseItem">ExpenseItem</see>, by its Id.
        /// </summary>
        /// <param name="expenseId">
        /// The expense Id.
        /// </param>
        /// <param name="accountId">
        /// The account Id the expense belongs to.
        /// </param>
        /// <returns>
        /// A ExpenseItemResponse, containing the <see cref="ExpenseItem">ExpenseItem</see> if found.
        /// </returns>
        [HttpGet, Route("GetExpenseItemByIdForExpedite/{expenseId:int}/{accountId:int}")]
        [InternalSelenityMethod]
        [NoAuthorisationRequired]
        public ExpediteExpenseItemResponse GetExpenseItemByIdForExpedite([FromUri] int expenseId, int accountId)
        {
            var response = InitialiseResponse<ExpediteExpenseItemResponse>();
            response.Item = ((ExpenseItemRepository)Repository).GetExpenseItemByIdForExpedite(expenseId, accountId);
            return response;
        }

        /// <summary>
        /// Gets a list of <see cref="ExpenseItem">ExpenseItems</see>, that require validation.
        /// This is defined by the ExpenseSubCategory.Validate, whether receipts are attached, and the validation count.
        /// </summary>
        /// <returns>A ExpenseItemResponse, containing the <see cref="ExpenseItem">ExpenseItems</see> if found.</returns>
        [HttpGet, Route("RequiringValidation")]
        [InternalSelenityMethod]
        [NoAuthorisationRequired]      
        public GetIdsReponse RequiringValidation()
        {
            var response = InitialiseResponse<GetIdsReponse>();
            response.List = ((ExpenseItemRepository) Repository).GetIdsRequiringValidation();
            return response;
        }


        /// <summary>
        /// Gets the costcode breakdown for an expense item by the expense item Id 
        /// </summary>
        /// <param name="id">The Id of the expense item.</param>
        /// <returns>A GetExpenseItemCostCodeBreakdownResponse, containing a list of <see cref="CostCodeBreakdown">CostCodeBreakdown</see>s.</returns>
        [HttpGet, Route("GetExpenseItemCostCodeBreakdown/{id:int}")]
        [AuthAudit(SpendManagementElement.None, AccessRoleType.View)]
        public GetExpenseItemCostCodeBreakdownResponse GetCostCodeBreakdown([FromUri] int id)
        {
            var response = InitialiseResponse<GetExpenseItemCostCodeBreakdownResponse>();
            response.List = ((ExpenseItemRepository)Repository).GetCostCodeBreakdown(id, response);
            return response; 
        }

        /// <summary>
        /// Adds an <see cref="ExpenseItem">ExpenseItem</see>.
        /// </summary>
        /// <param name="request">The <see cref="cExpenseItem">ExpenseItem</see> to add.<br/>
        /// When adding a new <see cref="cExpenseItem">ExpenseItem </see> through the API, the following properties are required:<br/>
        /// Id: Must be set to 0.
        /// All Ids e.g. currencyId, reasonId must be valid.
        /// </param>
        /// <returns>A ExpenseItemResponse containing the added <see cref="cExpenseItem">ExpenseItem </see>.</returns>
        [Route("")]
        [AuthAudit(SpendManagementElement.None, AccessRoleType.Add)]
        public ExpenseItemResponse Post([FromBody] ExpenseItem request)
        {
            return Post<ExpenseItemResponse>(request);
        }

        [Route("SaveExpenseItem")]
        [AuthAudit(SpendManagementElement.None, AccessRoleType.Add)]
        public ExpenseItemResponse Save(ExpenseItem request)
        {
            var response = InitialiseResponse<ExpenseItemResponse>();
            response.Item = ((ExpenseItemRepository)Repository).SaveExpenseItem(request, this.IsMobileRequest());
            return response;
        }

        /// <summary>
        /// Updates an <see cref="ExpenseItem">ExpenseItem</see>.
        /// </summary>
        /// <param name="request">The <see cref="UpdateExpenseItemRequest">UpdateExpenseItemRequest</see> to update. <br/>
        /// </param>
        /// <returns>A ExpenseItemResponse containing the updated <see cref="cExpenseItem">ExpenseItem </see>.</returns>

        [HttpPut, Route("UpdateExpenseItem")]
        [AuthAudit(SpendManagementElement.None, AccessRoleType.Add)]
        public ExpenseItemResponse Update(UpdateExpenseItemRequest request)
        {
            var response = InitialiseResponse<ExpenseItemResponse>();
            response.Item = ((ExpenseItemRepository)Repository).UpdateExpenseItem(request.ExpenseItem,request.OldClaimId,request.OfflineItem, this.IsMobileRequest(), request.ReasonForAmendment);
            return response;
        }
  
        /// <summary>
        /// Gets all the <see cref="JourneyStep">journey steps</see> for an expense id.
        /// </summary>
        /// <param name="expenseId">The associated expense id for the journey steps.</param>
        /// <returns>A JourneyStepsResponse, containing a list of <see cref="JourneyStep">journey steps</see> if found.</returns>
        [HttpGet]
        [Route("GetJourneySteps")]
        [AuthAudit(SpendManagementElement.None, AccessRoleType.Add)]
        public JourneyStepsResponse GetJourneySteps(int expenseId)
        {
            var response = this.InitialiseResponse<JourneyStepsResponse>();
            response.List = ((ExpenseItemRepository)this.Repository).GetJourneySteps(expenseId);
            return response;

        }

        /// <summary>
        /// Gets the message explaining what, if any, the home to calculation type is for this expense.
        /// </summary>
        /// <param name="expenseId">A valid expenseID which the user has permission to see</param>
        /// <returns>The home to office calculation</returns>
        [HttpGet, Route("GetHomeToOfficeMileageComment")]
        [AuthAudit(SpendManagementElement.None, AccessRoleType.View)]
        public StringResponse GetHomeToOfficeMileageComment(int expenseId)
        {
            var response = new StringResponse
            {
                Value = ((ExpenseItemRepository) this.Repository).GetHomeToOfficeMileageComment(expenseId)
            };

            return response;
        }

        /// <summary>
        /// Sets <see cref="ExpenseItem">ExpenseItems</see> to approved.
        /// </summary>
        /// <param name="request">The CheckAndPayActionRequest</param>
        /// <returns>The outcome of the action. 0 indicates failure, 1 indicates success</returns>
        [HttpPut]
        [Route("ApproveItems")]
        [AuthAudit(SpendManagementElement.CheckAndPay, AccessRoleType.Edit)]
        public ApprovedExpenseItemsResponse ApproveExpenseItems(CheckAndPayActionRequest request)
        {
            var response = this.InitialiseResponse<ApprovedExpenseItemsResponse>();
            response.Item = ((ExpenseItemRepository)this.Repository).ApproveExpenseItems(request.claimId, request.expenseIds);
            return response;
        }

        /// <summary>
        /// Sets an<see cref="ExpenseItem"> ExpenseItem</see> to unapproved.
        /// </summary>
        /// <param name="request">The CheckAndPayActionRequest</param>
        /// <returns>The <see cref="ListIntResponse"> ListIntResponse</see>of expense item ids that failed to be unapproved</returns>
        [HttpPut]
        [Route("UnapproveItem")]
        [AuthAudit(SpendManagementElement.CheckAndPay, AccessRoleType.Edit)]
        public ListIntResponse UnapproveExpenseItem(CheckAndPayActionRequest request)
        {
            var response = this.InitialiseResponse<ListIntResponse>();
            response.List = ((ExpenseItemRepository)this.Repository).UnapproveExpenseItem(request.claimId, request.expenseIds);
            return response;
        }

        /// <summary>
        /// Sets <see cref="ExpenseItem">ExpenseItems</see> to returned.
        /// </summary>
        /// <param name="request">The <see cref="ReturnExpenseItemsRequest">ReturnExpenseItemsRequest</see> request</param>
        [HttpPut]
        [Route("ReturnExpenseItems")]
        [AuthAudit(SpendManagementElement.CheckAndPay, AccessRoleType.Edit)]
        public NumericResponse ReturnExpenseItems(ReturnExpenseItemsRequest request)
        {
            var response = this.InitialiseResponse<NumericResponse>();
            response.Item = ((ExpenseItemRepository)this.Repository).ReturnExpenseItems(request.claimId, request.expenseIds, request.reason);
            return response;
        }

        /// <summary>
        /// Returns the list of <see cref="FlagSummary">FlagSummary</see> associated with an expense item
        /// </summary>
        /// <param name="expenseId">The Id of the expense to check</param>
        /// <param name="validationPointId">The enum value for the <see cref="ValidationPoint">ValidationPoint</see>/></param>
        /// <param name="validationTypeId">The enum value for the <see cref="ValidationType">validationType</see>/></param>
        /// <returns>The <see cref="FlagSummaryResponse">FlagSummaryResponse</see></returns>
        [HttpGet]
        [Route("GetExpenseItemFlags")]
        [AuthAudit(SpendManagementElement.None, AccessRoleType.Edit)]
        public FlagSummaryResponse GetExpenseItemFlags([FromUri] int expenseId, int validationPointId, int validationTypeId)
        {
            ValidationPoint validationPoint;
            ValidationType validationType;

            const string expenseItemFlagsError = "Get Expense Item Flags";

            if (typeof(ValidationPoint).IsEnumDefined(validationPointId))
            {
                validationPoint = (ValidationPoint)validationPointId;
            }
            else
            {
                throw new ApiException(string.Format(ApiResources.ApiErrorXUnsuccessfulMessage, expenseItemFlagsError),
                        string.Format(ApiResources.APIErrorValidationPointNotFound));
            }

            if (typeof(ValidationType).IsEnumDefined(validationTypeId))
            {
                validationType = (ValidationType)validationTypeId;
            }
            else
            {
                throw new ApiException(string.Format(ApiResources.ApiErrorXUnsuccessfulMessage, expenseItemFlagsError),
                       string.Format(ApiResources.APIErrorValidationTypeNotFound));
            }

            var response = this.InitialiseResponse<FlagSummaryResponse>();
            response.List = ((ExpenseItemRepository)this.Repository).GetExpenseItemFlags(expenseId, validationPoint, validationType);
            return response;
        }

        /// <summary>
        /// Update the operator validation progress status .Update the  ExpediteValidationProgress flag in saved expense table
        /// </summary>
        /// <param name="id">Expense Id</param>
        /// <param name="operatorValidationProgress">Expense Validation Progress</param>
        /// <param name="accountId">The accountId the expense belongs to.</param>
        /// <returns>ExpenseItemResponse</returns>
        [HttpPost, Route("UpdateOperatorValidationStatus/{Id:int}/{OperatorValidationProgress:int}/{accountId:int}")]
        [InternalSelenityMethod]
        [NoAuthorisationRequired]
        public ExpediteExpenseItemResponse UpdateOperatorValidationStatus(int id, int operatorValidationProgress, int accountId)
        {
            var result = ((ExpenseItemRepository)this.Repository).UpdateOperatorValidationStatus(id, operatorValidationProgress, accountId);
            var expenseItem = this.GetExpenseItemByIdForExpedite(id, accountId);
            expenseItem.Item.OperatorValidationProgress = (ExpediteOperatorValidationProgress) result;
            return expenseItem;
        }

        /// <summary>
        /// Deletes an expense item by its Id
        /// </summary>
        /// <param name="id">Expense Id</param>
        /// <returns>A <see cref="NumericResponse">NumericResponse</see></returns>
        [HttpPost, Route("DeleteExpenseItem/{Id:int}")]
        [AuthAudit(SpendManagementElement.Api, AccessRoleType.Add)]
        public NumericResponse DeleteExpenseItem([FromUri] int id)
        {
            var response = this.InitialiseResponse<NumericResponse>();     
            response.Item = ((ExpenseItemRepository)this.Repository).DeleteExpenseItem(id);
            return response;        
        }

        /// <summary>
        /// This endpoint helps to build the  add/edit expense form.   
        /// </summary>
        /// <param name="id">Expense Id, set to 0 if it's for a new expense item</param>
        /// <returns>A <see cref="ExpenseItemDefinitionResponse">ExpenseItemDefinitionResponse</see></returns>
        [HttpGet, Route("GetExpenseItemDefinition")]
        [AuthAudit(SpendManagementElement.None, AccessRoleType.View)]
        public ExpenseItemDefinitionResponse GetExpenseItemDefinition ([FromUri] int id)
        {
            var response = this.InitialiseResponse<ExpenseItemDefinitionResponse>();
            response.Item = ((ExpenseItemRepository) this.Repository).GetExpenseItemDefinition(id);
            return response;
        }

        /// <summary>
        /// This endpoint helps to build the edit expense form when viewing as an approver 
        /// </summary>
        /// <param name="request">
        /// The <see cref="GetClaimantsRequest">GetClaimantsRequest</see>.
        /// </param>
        /// <returns>
        /// A <see cref="ExpenseItemDefinitionResponse">ExpenseItemDefinitionResponse</see>
        /// </returns>
        [HttpPost, Route("GetExpenseItemDefinitionForClaimantAsApprover"), ApiExplorerSettings(IgnoreApi = true)]
        [AuthAudit(SpendManagementElement.None, AccessRoleType.View)]
        public ExpenseItemDefinitionResponse GetExpenseItemDefinitionForClaimantAsApprover(GetClaimantsRequest request)
        {
            var response = this.InitialiseResponse<ExpenseItemDefinitionResponse>();
            response.Item = ((ExpenseItemRepository)this.Repository).GetExpenseItemDefinitionForClaimantAsApprover(request.ExpenseId, request.EmployeeId);
            return response;
        }

        /// <summary>
        /// Saves the employee's reason for disputing a returned item
        /// </summary>
        /// <param name="id">The id of the expense item being disputed</param>
        /// <param name="reason">The reason for the disputes</param>
        /// <returns><see cref="NumericResponse">NumericResponse</see>Whereby 0 is fail, 1 is success</returns>
        [HttpPost, Route("DisputeExpenseItem")]
        [AuthAudit(SpendManagementElement.None, AccessRoleType.View)]
        public NumericResponse DisputeExpenseItem([FromUri] int id, [FromBody] string reason)
        {
            var response = this.InitialiseResponse<NumericResponse>();
            response.Item = ((ExpenseItemRepository)this.Repository).DisputeExpenseItem(id, reason, this.IsMobileRequest());
            return response;
        }
     

        #endregion Api Methods

        private bool IsMobileRequest()
        {
            return Helper.IsMobileRequest(this.Request.Headers.UserAgent.ToString());
        }
    }
}