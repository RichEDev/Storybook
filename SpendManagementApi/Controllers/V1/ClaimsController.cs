namespace SpendManagementApi.Controllers.V1
{
    using System;
    using System.Collections.Generic;
    using System.Web.Http;
    using System.Web.Http.Description;

    using Attributes;
    using Common.Enums;
    using Models.Common;
    using Models.Requests;
    using Models.Responses;
    using Models.Types;
    using Models.Types.ClaimSubmission;
    using Repositories;
    using ApiResources = Utilities.ApiResources;
    using SpendManagementApi.Common;
    
    using Spend_Management;
    using Claim = Models.Types.Claim;

    /// <summary>
    /// Manages operations on <see cref="Claim">Claims</see>.
    /// </summary>
    [RoutePrefix("Claims")]
    [Version(1)]
    public class ClaimsV1Controller : BaseApiController<Claim>
    {
        //BaseApiController<Claim>
        #region Api Methods

        /// <summary>
        /// Gets all of the available end points from the <see cref="Claim">Claims</see> part of the API.
        /// </summary>
        /// <returns>A list of available resource Links</returns>
        [HttpOptions, Route("")]
        public List<Link> Options()
        {
            return base.Links();
        }

        /// <summary>
        /// Gets a single <see cref="Claim">Claim</see>, by its Id.
        /// </summary>
        /// <param name="id">The Id of the item to get.</param>
        /// <returns>A ClaimResponse, containing the <see cref="Claim">Claim</see> if found.</returns>
        [HttpGet, Route("{id:int}")]
        [AuthAudit(SpendManagementElement.None, AccessRoleType.View)]
        public ClaimResponse Get([FromUri] int id)
        {
            var response = this.InitialiseResponse<ClaimResponse>();
            response.Item = ((ClaimRepository)this.Repository).GetAndAudit(id);
            return response;
        }

        /// <summary>
        /// Gets a single <see cref="Claim">Claim</see>, by its Id.
        /// </summary>
        /// <param name="id">The Id of the item to get.</param>
        /// <returns>A ClaimResponse, containing the <see cref="Claim">Claim</see> if found.</returns>
        [HttpGet, Route("GetClaimBasicById/{id:int}")]
        [AuthAudit(SpendManagementElement.None, AccessRoleType.View)]
        public ClaimBasicItemResponse GetClaimBasicById([FromUri] int id)
        {
            var response = this.InitialiseResponse<Models.Responses.ClaimBasicItemResponse>();
            response.Item = ((ClaimRepository)this.Repository).GetClaimBasic(id);
            return response;
        }

        /// <summary>
        /// Gets a single <see cref="Claim">Claim</see>, by its ClaimReferenceNumber (CRN).
        /// </summary>
        /// <param name="crn">The ClaimReferenceNumber (CRN) of the claim to get.</param>
        /// <returns>A ClaimResponse, containing the <see cref="Claim">Claim</see> if found.</returns>
        [HttpGet, Route("{crn}")]
        [AuthAudit(SpendManagementElement.None, AccessRoleType.View)]
        public ClaimResponse Get([FromUri] string crn)
        {
            var response = this.InitialiseResponse<ClaimResponse>();
            response.Item = ((ClaimRepository) this.Repository).GetByReferenceNumber(crn);
            return response;
        }

        /// <summary>
        /// Gets a single <see cref="Models.Types.ClaimDefinitionResponse">ClaimDefinitionResponse</see>, by its Id.
        /// </summary>
        /// <param name="id">The Id of the <see cref="ClaimDefinition">ClaimDefinition</see> to get.</param>
        /// <returns>A <see cref="Models.Responses.ClaimDefinitionResponse">ClaimDefinitionResponse</see>, containing the claim name, description and user defined field information.</returns>
        [HttpGet, Route("GetClaimDefinition")]
        [AuthAudit(SpendManagementElement.None, AccessRoleType.View)]
        public Models.Responses.ClaimDefinitionResponse GetClaimDefinition([FromUri] int id)
        {
            var response = this.InitialiseResponse<Models.Responses.ClaimDefinitionResponse>();
            response.Item = ((ClaimRepository)this.Repository).GetClaimDefinition(id);
            return response;
        }

        /// <summary>
        /// This is used to help build the new claim form. 
        /// Gets a single <see cref="Models.Types.ClaimDefinitionResponse">ClaimDefinitionResponse</see> with the determined claim name and UDF information.
        /// </summary>
        /// <returns>A <see cref="Models.Responses.ClaimDefinitionResponse">ClaimDefinitionResponse</see>, containing the claim name and user defined field information.</returns>
        [HttpGet, Route("GetDefaultClaimDefinition")] [AuthAudit(SpendManagementElement.None, AccessRoleType.View)]
        public Models.Responses.ClaimDefinitionResponse GetDefaultClaimDefinition()
        {
            var response = this.InitialiseResponse<Models.Responses.ClaimDefinitionResponse>();
            response.Item = ((ClaimRepository)this.Repository).GetDefaultClaimDefinition();
            return response;
        }

        /// <summary>
        /// Allocates a claim for payment.
        /// </summary>
        /// <param name="claimId">The claim to allocate</param>
        /// <returns>A <see cref="ApiResponse">ApiResponse</see> detailing success or failure.</returns>
        [HttpPut]
        [AuthAudit(SpendManagementElement.CheckAndPay, AccessRoleType.Edit)]
        [Route("Allocate")]
        public ApiResponse Allocate(int claimId)
        {
            var response = this.InitialiseResponse();
            ((ClaimRepository)this.Repository).Allocate(
                this.CurrentUser.AccountID,
                this.CurrentUser.EmployeeID,
                claimId);
            return response;
        }

        /// <summary>
        /// Checks that the flags associated against the expense items relating to the claim Id have justifications from the authoriser, if mandatory 
        /// </summary>
        /// <param name="id">
        /// The Id of the claim.
        /// </param>
        /// <returns>
        /// The <see cref="ListIntResponse"> ListIntResponse</see>of expense item ids that require justifications from the approver before submitting
        /// </returns>
        [HttpGet]
        [AuthAudit(SpendManagementElement.CheckAndPay, AccessRoleType.Edit)]
        [Route("CheckJustificationsHaveBeenProvidedByAuthoriser")]
        public ListIntResponse CheckJustificationsHaveBeenProvidedByAuthoriser([FromUri] int id)
        {
            var response = this.InitialiseResponse<ListIntResponse>();
            response.List = ((ClaimRepository)this.Repository).CheckJustificationsHaveBeenProvidedByAuthoriser(id);
            return response;
        }

        /// <summary>
        /// Approves a claim, sending to the next stage if applicable.
        /// </summary>
        /// <param name="claimId">The claim to approve</param>
        /// <returns>A <see cref="ClaimSubmissionResponse">ClaimSubmissionResponse</see> detailing success or failure of the approval.</returns>
        [HttpPut]
        [AuthAudit(SpendManagementElement.CheckAndPay, AccessRoleType.Edit)]
        [Route("ApproveClaim")]
        public ClaimSubmissionResponse ApproveClaim(int claimId)
        {
            var response = this.InitialiseResponse<ClaimSubmissionResponse>();
            response.Item = ((ClaimRepository)this.Repository).ApproveClaim(this.CurrentUser.AccountID, this.CurrentUser.EmployeeID, claimId);
            return response;
        }


        /// <summary>
        /// Gets all of the expense items on a claim.
        /// </summary>
        /// <param name="claimId">The claim to get the expense items for.</param>
        /// <returns>A list of expense items contained in a <see cref="ClaimExpenseItemsResponse">ClaimExpenseItemsResponse</see></returns>
        [HttpGet]
        [AuthAudit(SpendManagementElement.None, AccessRoleType.View)]
        [Route("ExpenseItems")]
        public ClaimExpenseItemsResponse ExpenseItems(int claimId)
        {
            var response = this.InitialiseResponse<ClaimExpenseItemsResponse>();
            var claimRepository = (ClaimRepository)this.Repository;
            response.List = claimRepository.GetClaimExpenseItems(claimId);
            response.DisplayFields = claimRepository.GetFields();
            return response;
        }

        /// <summary>
        /// Gets the basic expense item details that belong to a claim
        /// </summary>
        /// <param name="claimId">The claim to get the expense items for.</param>
        /// <returns>A list of basic expense items details contained in a <see cref="ClaimExpenseOverviewResponse">ClaimExpenseItemsResponse</see></returns>
        [HttpGet]
        [AuthAudit(SpendManagementElement.None, AccessRoleType.View)]
        [Route("GetClaimExpenseOverview")]
        public ClaimExpenseOverviewResponse GetClaimExpenseOverview(int claimId)
        {
            var response = this.InitialiseResponse<ClaimExpenseOverviewResponse>();
            var claimRepository = (ClaimRepository)this.Repository;
            response.List = claimRepository.GetClaimExpenseItemsOverview(claimId);
       
            return response;
        }

        /// <summary>
        /// Gets the list of claims awaiting approval by the employee
        /// </summary>
        /// <returns>A list of <see cref="ClaimBasicResponse">ClaimBasicResponse</see></returns>
        [HttpGet]
        [AuthAudit(SpendManagementElement.CheckAndPay, AccessRoleType.View)]
        [Route("AwaitingApproval")]
        public ClaimBasicResponse AwaitingApproval()
        {
            var response = this.InitialiseResponse<ClaimBasicResponse>();
            response.List = ((ClaimRepository)this.Repository).GetClaimsWaitingApproval(
                this.CurrentUser.AccountID,
                this.CurrentUser.EmployeeID);
            return response;
        }

        /// <summary>
        /// Gets the list of unsubmitted claims that belong to the current user.
        /// </summary>
        /// <returns>A list of <see cref="ClaimBasic">ClaimBasic</see>containing the unsubmitted claims that belong to the employee</returns>
        [HttpGet]
        [AuthAudit(SpendManagementElement.None, AccessRoleType.View)]
        [Route("GetUnsubmitted")]
        public ClaimBasicResponse GetUnsubmitted()
        {
            var response = this.InitialiseResponse<ClaimBasicResponse>();
            response.List = ((ClaimRepository)this.Repository).GetUnsubmittedClaims(this.CurrentUser.EmployeeID);
            return response;
        }

        /// <summary>
        /// Gets the list of submitted claims that belong to the current user.
        /// </summary>
        /// <returns>A list of <see cref="ClaimBasic">ClaimBasic</see>containing the submitted claims that belong to the employee</returns>
        [HttpGet]
        [AuthAudit(SpendManagementElement.None, AccessRoleType.View)]
        [Route("GetSubmitted")]
        public ClaimBasicResponse GetSubmitted()
        {
            var response = this.InitialiseResponse<ClaimBasicResponse>();
            response.List = ((ClaimRepository)this.Repository).GetSubmittedClaims(this.CurrentUser.EmployeeID);
            return response;
        }

        /// <summary>
        /// Gets the list of previous claims that belong to the current user.
        /// </summary>
        /// <returns>A list of <see cref="ClaimBasic">ClaimBasic</see>containing the previous claims that belong to the employee</returns>
        [HttpGet]
        [AuthAudit(SpendManagementElement.None, AccessRoleType.View)]
        [Route("GetPrevious")]
        public ClaimBasicResponse GetPrevious()
        {
            var response = this.InitialiseResponse<ClaimBasicResponse>();
            response.List = ((ClaimRepository)this.Repository).GetPreviousClaims(this.CurrentUser.EmployeeID);
            return response;
        }

        /// <summary>
        /// Gets the count of claims awaiting approval by the mobile user.
        /// </summary>
        /// <returns>The number of claims awaiting approval as an <see cref="int">int</see> contained in a <see cref="NumericResponse">NumericResponse</see></returns>
        [HttpGet]
        [AuthAudit(SpendManagementElement.CheckAndPay, AccessRoleType.View)]
        [Route("AwaitingApprovalCount")]
        public NumericResponse AwaitingApprovalCount()
        {
            var response = this.InitialiseResponse<NumericResponse>();
            response.Item = ((ClaimRepository)this.Repository).GetClaimsToCheckCount(this.CurrentUser.AccountID, this.CurrentUser.EmployeeID);
            return response;
        }

        /// <summary>
        /// Gets the total number of claims in the account for the current employee at a given <see cref="ClaimStage">claim stage</see>. If not supplied, the <see cref="ClaimStage">claim stage</see> will default to ClaimStage.Any
        /// </summary>
        /// <param name="claimStage">The <see cref="ClaimStage">claim stage.</see></param>
        /// <returns>The number of claims in the system for the employee at the given <see cref="ClaimStage">claim stage</see>.</returns>
        [HttpGet, Route("GetClaimCountByEmployeeAndClaimStage")]
        [AuthAudit(SpendManagementElement.None, AccessRoleType.View)]
        public ClaimNumericResponse GetClaimCountByEmployeeAndClaimStage(ClaimStage claimStage = ClaimStage.Any)
        {
            var response = this.InitialiseResponse<ClaimNumericResponse>();
            response.Item = ((ClaimRepository)this.Repository).GetCount(this.CurrentUser.EmployeeID, claimStage);
            return response;
        }

        /// <summary>
        /// Gets the total number of claims in the system for the account
        /// </summary>
        /// <returns>The number of claims in the system for the account. <see cref="ClaimNumericResponse">ClaimNumericResponse</see></returns>
        [HttpGet, Route("GetTotalClaimCount")]
        [AuthAudit(SpendManagementElement.None, AccessRoleType.View)]
        public ClaimNumericResponse GetTotalClaimCount()
        {
            var response = this.InitialiseResponse<ClaimNumericResponse>();
            response.Item = ((ClaimRepository)this.Repository).GetCount();
            return response;
        }

        /// <summary>
        /// Gets the default claim number
        /// </summary>
        /// <param name="employeeId">The employee identification number</param>
        /// <returns>The number of claims in the system for the account. <see cref="ClaimNumericResponse">ClaimNumericResponse</see></returns>
        [HttpGet, Route("GetDefaultClaimNumber")]
        [AuthAudit(SpendManagementElement.None, AccessRoleType.View)]
        public ClaimNumericResponse GetDefaultClaimNumber(int employeeId)
        {
            var response = this.InitialiseResponse<ClaimNumericResponse>();
            response.Item = ((ClaimRepository)this.Repository).GetDefaultClaim(employeeId);
            return response;
        }

        /// <summary>
        /// Adds the default claim number.
        /// </summary>
        /// <param name="employeeId">The employee identification number</param>
        /// <returns>The id number of the default claim. <see cref="ClaimNumericResponse">ClaimNumericResponse</see></returns>
        [HttpPost, Route("AddDefaultClaim")]
        [AuthAudit(SpendManagementElement.None, AccessRoleType.View)]
        public ClaimNumericResponse AddDefaultClaim(int employeeId)
        {
            var response = this.InitialiseResponse<ClaimNumericResponse>();
            response.Item = ((ClaimRepository)this.Repository).AddDefaultClaim(employeeId);
            return response;
        }

        /// <summary>
        /// Deletes a claim for the user.
        /// </summary>
        /// <param name="claimId">The claim identification number</param>
        /// <returns>Status code 200 on success with a null item. <see cref="ClaimResponse">ClaimResponse</see></returns>
        [HttpDelete, Route("DeleteClaim")]
        [AuthAudit(SpendManagementElement.None, AccessRoleType.Delete)]
        public ApiResponse DeleteClaim(int claimId)
        {
            var response = this.InitialiseResponse();
            ((ClaimRepository)this.Repository).DeleteClaim(claimId);
            return response;
        }

        /// <summary>
        /// Creates a new claim for the user.
        /// </summary>
        /// <param name="claim">The <see cref="ClaimDefinition">ClaimDefinition</see> object to create the claim with.</param>
        /// <returns>The <see cref="ClaimBasicResponse">ClaimResponse</see> for the created claim</returns>
        [HttpPost, Route("CreateClaim")]
        [AuthAudit(SpendManagementElement.None, AccessRoleType.Add)]
        public ClaimBasicItemResponse CreateClaim([FromBody] ClaimDefinition claim)
        {
            var response = this.InitialiseResponse<ClaimBasicItemResponse>();     
            response.Item = ((ClaimRepository)this.Repository).CreateClaim(claim, IsMobileRequest());
            return response;
        }

        /// <summary>
        /// Updates the claim details for the user.
        /// </summary>
        /// <param name="claim">The <see cref="ClaimDefinition">ClaimDefinition</see> object to update the claim with.</param>
        /// <returns>The <see cref="ClaimResponse">ClaimResponse</see> for the updated claim</returns>
        [HttpPut, Route("UpdateClaim")]
        [AuthAudit(SpendManagementElement.None, AccessRoleType.Edit)]
        public ClaimBasicItemResponse UpdateClaim([FromBody] ClaimDefinition claim)
        {
            var response = this.InitialiseResponse<ClaimBasicItemResponse>();
            response.Item = ((ClaimRepository)this.Repository).UpdateClaim(claim, IsMobileRequest());
            return response;
        }

        /// <summary>
        /// Gets <see cref="ClaimSubmissionApi">ClaimSubmission details</see>, by its Id.
        /// </summary>
        /// <param name="id">Claim id to get the submission details for.</param>
        /// <returns>A ClaimSubmissionResponse, containing the <see cref="ClaimSubmissionApi">ClaimSubmission details</see> if found.</returns>
        [HttpGet, Route("GetClaimSubmissionDetails")]
        [AuthAudit(SpendManagementElement.None, AccessRoleType.View)]
        public ClaimSubmissionDetailResponse GetClaimSubmissionDetails(int id)
        {
            if (id <= 0)
            {
               throw new ApiException(ApiResources.ApiErrorInvalidClaimId, ApiResources.ApiErrorProvideValidClaimIdMessage);
            }

            var response = this.InitialiseResponse<ClaimSubmissionDetailResponse>();
            response.Item = ((ClaimRepository)this.Repository).ClaimSubmissionDetails(id);
            return response;
        }

        /// <summary>
        /// Checks if a claim can be submitted
        /// </summary>
        /// <param name="claimId">The Id of the claim</param>
        /// <returns>The<see cref="ClaimSubmissionResponse">ClaimSubmissionResponse with the outcome of the checks</see>/></returns>
        [HttpPost]
        [Route("DetermineIfClaimCanBeSubmitted")]
        [AuthAudit(SpendManagementElement.None, AccessRoleType.View)]
        public ClaimSubmissionResponse DetermineIfClaimCanBeSubmitted(int claimId)
        {
            var response = this.InitialiseResponse<ClaimSubmissionResponse>();
            response.Item = ((ClaimRepository) this.Repository).DetermineifClaimCanBeSubmitted(claimId);

            return response;
        }

        /// <summary>
        /// Checks if a claim can be unsubmitted
        /// </summary>
        /// <param name="claimId">The Id of the claim</param>
        /// <returns>The <see cref="UnsubmitClaimResponse">Unsubmit Claim Response with the outcome of the checks</see></returns>
        [HttpPost]
        [Route("DetermineIfClaimCanBeUnsubmitted")]
        [AuthAudit(SpendManagementElement.None, AccessRoleType.View)]
        public UnsubmitClaimResponse DetermineIfClaimCanBeUnsubmitted(int claimId)
        {
            var response = this.InitialiseResponse<UnsubmitClaimResponse>();
            response.Item = ((ClaimRepository)this.Repository).DetermineIfClaimCanBeUnsubmitted(claimId);

            return response;
        }

        /// <summary>
        /// Submit claim.
        /// </summary>
        /// <param name="claimSubmissionRequest">
        /// ClaimSubmission request.
        /// </param>
        /// <returns>
        /// <see cref="ClaimSubmissionResponse">ClaimSubmission Response</see>.
        /// </returns>
        /// <exception cref="ApiException">
        /// </exception>
        [HttpPost]
        [Route("SubmitClaim")]
        [AuthAudit(SpendManagementElement.None, AccessRoleType.Edit)]
        public ClaimSubmissionResponse SubmitClaim([FromBody] ClaimSubmissionRequest claimSubmissionRequest)
        {
            if (claimSubmissionRequest == null)
            {
                throw new ApiException(ApiResources.ApiErrorInvalidRequest, ApiResources.ApiErrorInvalidClaimSubmissionMessage);
            }

            var response = this.InitialiseResponse<ClaimSubmissionResponse>();
            response.Item = ((ClaimRepository)this.Repository).SubmitClaim(
                claimSubmissionRequest.ClaimId,
                claimSubmissionRequest.ClaimName,
                claimSubmissionRequest.Description,
                claimSubmissionRequest.Cash,
                claimSubmissionRequest.Credit,
                claimSubmissionRequest.Purchase,
                claimSubmissionRequest.Approver,
                claimSubmissionRequest.OdometerReadings,
                claimSubmissionRequest.BusinessMileage,
                claimSubmissionRequest.IgnoreApproverOnHoliday,
                claimSubmissionRequest.ViewFilter,
                claimSubmissionRequest.ContinueAlthoughAuthoriserIsOnHoliday);

            return response;
        }

        /// <summary>
        /// Unsubmit claim.
        /// </summary>
        /// <param name="claimId">ID of claim to be unsubmitted.</param>
        /// <returns>
        /// <see cref="UnsubmitClaimResponse">Unsubmit Claim Response</see>
        /// </returns>
        [HttpPost]
        [Route("UnsubmitClaim")]
        [AuthAudit(SpendManagementElement.None, AccessRoleType.Edit)]
        public UnsubmitClaimResponse UnsubmitClaim(int claimId)
        {
            var response = this.InitialiseResponse<UnsubmitClaimResponse>();
            response.Item = ((ClaimRepository)this.Repository).UnsubmitClaim(claimId);

            return response;
        }
        
        /// <summary>
        /// Gets claim history
        /// </summary>
        /// <param name="claimId">Id of the claim</param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetClaimHistory")]
        [AuthAudit(SpendManagementElement.None, AccessRoleType.View)]
        public GetClaimHistoryResponse GetClaimHistory(int claimId)
        {
            var response = this.InitialiseResponse<GetClaimHistoryResponse>();
            response.Item = ((ClaimRepository)this.Repository).GetClaimHistory(claimId);

            return response;
        }




        /// <summary>
        /// Generates the GetApprover HTML, which is used to allow users to select who the claim should go to in the next stage.
        /// </summary>
        /// <param name="total">The claim total</param>
        /// <param name="employeeId">The employee identification number</param>
        /// <returns> <see cref="GetApproverResponse">GetApproverResponse</see> which details the possible approvers the claim can go to</returns>
        [HttpPost, Route("GetApprover")]
        [AuthAudit(SpendManagementElement.None, AccessRoleType.View)]
        public GetApproverResponse GetApprover(decimal total, int employeeId)
        {
            var response = this.InitialiseResponse<GetApproverResponse>();
            response.Item = ((ClaimRepository)this.Repository).GetApprover(total, employeeId);
            return response;
        }

        /// <summary>
        /// Saves justifications provided by the claimant for the given flagged item ids.
        /// </summary>
        /// <param name="flagsJustificationRequest">
        /// The <see cref="FlagsJustificationRequest">FlagsJustificationRequest</see>.
        /// </param>
        /// <returns>
        /// The <see cref="ApiResponse"></see>.
        /// </returns>
        [HttpPost, Route("SaveClaimantJustifications")]
        [AuthAudit(SpendManagementElement.None, AccessRoleType.Edit)]
        public ApiResponse SaveClaimantJustification([FromBody] FlagsJustificationRequest flagsJustificationRequest)
        {
            if (flagsJustificationRequest == null)
            {
                throw new ApiException(ApiResources.ApiErrorInvalidRequest, ApiResources.ApiErrorInvalidClaimantJustificationsSaveMessage);
            }

            var response = this.InitialiseResponse();
            ((ClaimRepository)this.Repository).SaveClaimantJustifications(flagsJustificationRequest.FlagsJustificationRequests);
            return response;
        }

        /// <summary>
        /// Saves justifications provided by the approver for the given flagged item ids.
        /// </summary>
        /// <param name="flagsJustificationRequest">
        /// Claimant justification request.
        /// </param>
        /// <returns>
        /// The <see cref="ApiResponse"></see>.
        /// </returns>
        [HttpPut]
        [Route("SaveAuthoriserJustification")]
        [AuthAudit(SpendManagementElement.CheckAndPay, AccessRoleType.Edit)]
        public ApiResponse SaveAuthoriserJustification([FromBody] FlagsJustificationRequest flagsJustificationRequest)
        {
            if (flagsJustificationRequest == null)
            {
                throw new ApiException(ApiResources.ApiErrorInvalidRequest, ApiResources.ApiErrorInvalidApproverJustificationsSaveMessage);
            }

            var response = this.InitialiseResponse();
            ((ClaimRepository)this.Repository).SaveApproverJustifications(flagsJustificationRequest.FlagsJustificationRequests);
            return response;
        }

        /// <summary>
        /// Generates the data for the expense flag grid
        /// </summary>
        /// <param name="flagsGridRequest">
        /// The <see cref="GetFlagsGridRequest">FlagsGridRequest</see> 
        /// </param>
        /// <returns>
        /// The <see cref="FlagItemsManagerResponse">FlagItemsManagerResponse</see>.
        /// </returns>
        [HttpPost]
        [Route("GenerateFlagsGrid")]
        [AuthAudit(SpendManagementElement.None, AccessRoleType.Edit)]
        public FlagItemsManagerResponse GenerateFlagsGrid([FromBody] GetFlagsGridRequest flagsGridRequest)
        {
            if (flagsGridRequest == null)
            {
                throw new ApiException(ApiResources.ApiErrorInvalidRequest, ApiResources.ApiErrorInvalidRequestToGetMessage);
            }

            var response = this.InitialiseResponse<FlagItemsManagerResponse>();
            response.Item = ((ClaimRepository)this.Repository).GetFlagsGrid(flagsGridRequest.claimId, flagsGridRequest.expenseIds, flagsGridRequest.pageSource);

            return response;
        }

        /// <summary>
        /// Gets the number of claims at the stages of Current, Submitted, and Previous for the current employee.
        /// </summary>
        /// <returns>
        /// The <see cref="ClaimCountForAllStagesResponse">ClaimCountForAllStagesResponse</see>.
        /// </returns>
        [HttpGet]
        [Route("GetClaimCountForAllStages")]
        [AuthAudit(SpendManagementElement.None, AccessRoleType.Edit)]
        public ClaimCountForAllStagesResponse GetClaimCountForAllStages()
        {               
            var response = this.InitialiseResponse<ClaimCountForAllStagesResponse>();
            response.Item = ((ClaimRepository)this.Repository).GetClaimCountForAllStages();
            return response;
        }

        /// <summary>
        /// To notify approvers of pending claims.
        /// </summary>
        /// <returns>
        /// The <see cref="ClaimReminderResponse"/>.
        /// </returns>
        [HttpGet, Route("NotifyApproversOfPendingClaims")]
        [ApiExplorerSettings(IgnoreApi = true)]
        [InternalSelenityMethod]
        public ClaimReminderResponse NotifyApproversOfPendingClaims()
        {
            var response = this.InitialiseResponse<ClaimReminderResponse>();
            try
            {
                response.IsSendingSuccessful = new cClaims().NotifyClaimApproverOfPendingClaims();
            }
            catch (Exception ex)
            {
                response.ErrorMessage =
                    $"Error details:{ex.Message}{Environment.NewLine} Stack Trace:{ex.StackTrace}";
            }
            return response;
        }

        /// <summary>
        /// To notify approvers of pending claims.
        /// </summary>
        /// <returns>
        /// The <see cref="ClaimReminderResponse"/>.
        /// </returns>
        [HttpGet, Route("NotifyApproversOfPendingClaims/{accountId:int}")]
        [ApiExplorerSettings(IgnoreApi = true)]
        [InternalSelenityMethod]
        public ClaimReminderResponse NotifyApproversOfPendingClaims([FromUri] int accountId)
        {
            var response = this.InitialiseResponse<ClaimReminderResponse>();
            try
            {
                response.IsSendingSuccessful = new cClaims().NotifyClaimApproverOfPendingClaims(accountId);
            }
            catch (Exception ex)
            {
                response.ErrorMessage =
                    $"Error details:{ex.Message}{Environment.NewLine} Stack Trace:{ex.StackTrace}";
            }
            return response;
        }

        /// <summary>
        /// To notify claimants for current claims.
        /// </summary>
        /// <returns>
        /// The <see cref="ClaimReminderResponse"/>.
        /// </returns>
        [HttpGet, Route("NotifyClaimantsOfCurrentClaims")]
        [ApiExplorerSettings(IgnoreApi = true)]
        [InternalSelenityMethod]
        public CurrentClaimReminderResponse NotifyClaimantsOfCurrentClaims()
        {

            var response = this.InitialiseResponse<CurrentClaimReminderResponse>();
            try
            {
                response.IsSendingSuccessful = new cClaims().NotifyClaimantsOfCurrentClaims();
            }
            catch (Exception ex)
            {
                response.ErrorMessage = ex.Message + "Error details" + ex.InnerException;
            }
            return  response;
        }

        /// <summary>
        /// To notify claimants for current claims.
        /// </summary>
        /// <returns>
        /// The <see cref="ClaimReminderResponse"/>.
        /// </returns>
        [HttpGet, Route("NotifyClaimantsOfCurrentClaims/{accountId:int}")]
        [ApiExplorerSettings(IgnoreApi = true)]
        [InternalSelenityMethod]
        public CurrentClaimReminderResponse NotifyClaimantsOfCurrentClaims([FromUri] int accountId)
        {

            var response = this.InitialiseResponse<CurrentClaimReminderResponse>();
            try
            {
                response.IsSendingSuccessful = new cClaims().NotifyClaimantsOfCurrentClaims(accountId);
            }
            catch (Exception ex)
            {
                response.ErrorMessage = ex.Message + "Error details" + ex.InnerException;
            }
            return response;
        }



        /// <summary>
        /// Saves envelope information against a claim
        /// </summary>
        /// <param name="saveEnvelopesRequest">
        /// The save Envelopes Request.
        /// </param>
        /// <returns>
        /// The <see cref="ClaimEnvelopeAttachmentResults">ClaimEnvelopeAttachmentResults</see>
        /// </returns>
        [HttpPost, Route("SaveEnvelopeInformationAgainstAClaim")]
        [AuthAudit(SpendManagementElement.None, AccessRoleType.Edit)]
        public ClaimEnvelopeAttachmentResultsResponse SaveEnvelopeInformationAgainstAClaim([FromBody] SaveEnvelopesRequest saveEnvelopesRequest)
        {
            var response = this.InitialiseResponse<ClaimEnvelopeAttachmentResultsResponse>();
            response.Item = ((ClaimRepository)this.Repository).SaveEnvelopeInformationAgainstAClaim(saveEnvelopesRequest.ClaimId, saveEnvelopesRequest.ClaimEnvelopes);
            return response;
        }

        /// <summary>
        /// Assign the claims related to the provided claimIds to the current user.
        /// </summary>
        /// <param name="request">
        /// The <see cref="AssignClaimsRequest">AssignClaimsRequest</see>.
        /// </param>
        /// <returns>
        /// The <see cref="AssignClaimsResponse">AssignClaimsResponse</see>
        /// </returns>
        [HttpPost, Route("AssignClaims")]
        [AuthAudit(SpendManagementElement.CheckAndPay, AccessRoleType.Edit)]
        public AssignClaimsResponse AssignClaims([FromBody] AssignClaimsRequest request)
        {
            var response = this.InitialiseResponse<AssignClaimsResponse>();
            response.List = ((ClaimRepository)this.Repository).AssignClaimsForApproval(request.ClaimIds);
            return response;
        }

        /// <summary>
        /// Unassigns the claim related to the provided claimId from the current user
        /// </summary>
        /// <param name="claimId">
        /// The claim Id.
        /// </param>
        /// <returns>
        /// The <see cref="NumericResponse">NumericResponse</see>
        /// </returns>
        [HttpPost, Route("UnassignClaim")]
        [AuthAudit(SpendManagementElement.CheckAndPay, AccessRoleType.Edit)]
        public NumericResponse UnassignClaim([FromUri] int claimId)
        {
            var response = this.InitialiseResponse<NumericResponse>();
            response.Item = ((ClaimRepository)this.Repository).UnassignClaimForApproval(claimId);
            return response;
        }

        /// <summary>
        /// The is mobile request.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool IsMobileRequest()
        {
            return Helper.IsMobileRequest(this.Request.Headers.UserAgent.ToString());
        }

        #endregion Api Methods
    }
}

