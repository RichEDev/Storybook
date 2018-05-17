namespace SpendManagementApi.Controllers.Expedite.V1
{
    using System;
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

    using ExpediteReceipt = SpendManagementApi.Models.Types.Expedite.ExpediteReceipt;

    /// <summary>
    /// Contains account specific actions.
    /// </summary>
    [RoutePrefix("Expedite/Receipts")]
    [Version(1)]
    [InternalSelenityMethod, ApiExplorerSettings(IgnoreApi = true)]
    public class ReceiptsV1Controller : BaseApiController<Receipt>
    {
        /// <summary>
        /// Gets ALL of the available end points from the Receipts API.
        /// </summary>
        /// <returns>A list of available Links</returns>
        [HttpOptions]
        public List<Link> Options()
        {
            return this.Links("Options");
        }

        #region Receipt Methods

        /// <summary>
        /// Gets all receipts for an account that are not assigned to a user, claim, or claim line (savedexpense).
        /// Only an account administrator should have visibility of these.
        /// </summary>
        /// <param name="accountId">The optional accountId. 
        /// This defaults to the currently logged on user's account. 
        /// This will only be available if you are a SEL admin.
        /// </param>
        /// <returns>A list of Receipts.</returns>
        [HttpGet, Route("~/Expedite/{accountId:int?}/Receipts")]
        [AuthAudit(SpendManagementElement.None, AccessRoleType.View)]
        public GetReceiptsResponse GetUnassignedForAccount(int? accountId)
        {
            var response = this.InitialiseResponse<GetReceiptsResponse>();
            response.List = ((ReceiptRepository)this.Repository).GetUnassigned().ToList();
            return response;
        }

        /// <summary>
        /// Gets all the Receipts for a specific Claimant, or Employee.
        /// </summary>
        /// <param name="employeeId">The EmployeeId of the claimant.</param>
        /// <returns>A list of Receipts.</returns>
        [HttpGet, Route("~/Expedite/Receipts/ByClaimant/{employeeId:int}")]
        [AuthAudit(SpendManagementElement.None, AccessRoleType.View)]
        public GetReceiptsResponse GetByClaimant(int employeeId)
        {
            var response = this.InitialiseResponse<GetReceiptsResponse>();
            response.List = ((ReceiptRepository)this.Repository).GetByClaimant(employeeId).ToList();
            return response;
        }

        /// <summary>
        /// Gets all the Receipts for a specific Claim header.
        /// </summary>
        /// <param name="claimId">The Id of the claim.</param>
        /// <returns>A list of Receipts.</returns>
        [HttpGet, Route("~/Expedite/Receipts/ByClaim/{claimId:int}")]
        [AuthAudit(SpendManagementElement.None, AccessRoleType.View)]
        public GetReceiptsResponse GetByClaim(int claimId)
        {
            var response = this.InitialiseResponse<GetReceiptsResponse>();
            response.List = ((ReceiptRepository)this.Repository).GetByClaim(claimId).ToList();
            return response;
        }

        /// <summary>
        /// Gets all the Receipts for a specific claim line, or 'savedexpense'.
        /// </summary>
        /// <param name="savedExpenseId">The Id of the expense.</param>
        /// <param name="accountId">The account Id the saved expense belongs to.</param>
        /// <returns>A list of Receipts.</returns>
        [HttpGet, Route("~/Expedite/Receipts/ByClaimLine/{savedExpenseId:int}/{accountId:int}")]
        [NoAuthorisationRequired]
        public GetReceiptsResponse GetByClaimLine([FromUri] int savedExpenseId, int accountId)
        {
            var response = this.InitialiseResponse<GetReceiptsResponse>();
            response.List = ((ReceiptRepository)this.Repository).GetByClaimLine(savedExpenseId, accountId).ToList();
            return response;
        }

        /// <summary>
        /// Gets the data for a receipt from the cloud, and returns the Receipt with its data property populated.
        /// </summary>
        /// <param name="id">The Id of the receipt.</param>
        /// <returns>The Receipt.</returns>
        [HttpGet, Route("~/Expedite/Receipts/{id:int}")]
        [AuthAudit(SpendManagementElement.Api, AccessRoleType.View)]
        public ReceiptResponse Get(int id)
        {
            return this.Get<ReceiptResponse>(id);
        }

        /// <summary>
        /// Gets the data for a receipt from the cloud, and returns the Receipt with its data property populated.
        /// </summary>
        /// <param name="receiptId">
        /// The receipt Id.
        /// </param>
        /// <param name="accountId">
        /// The account Id.
        /// </param>
        /// <returns>
        /// The Receipt.
        /// </returns>
        [HttpGet, Route("~/Expedite/GetReceiptByIdForExpedite/{receiptId:int}/{accountId:int}")]
        [NoAuthorisationRequired]
        public ReceiptResponse GetReceiptByIdForExpedite([FromUri] int receiptId, int accountId)
        {
            var response = this.InitialiseResponse<ReceiptResponse>();
            response.Item = ((ReceiptRepository)this.Repository).GetReceiptByIdForExpedite(receiptId, accountId);
            return response;        
        }

        /// <summary>
        /// Gets the data for an orphaned receipt from the cloud, and returns the Receipt with its data property populated.
        /// </summary>
        /// <param name="id">The Id of the receipt.</param>
        /// <returns>The Receipt.</returns>
        [HttpGet, Route("~/Expedite/OrphanedReceipts/{id:int}")]
        [AuthAudit(SpendManagementElement.Api, AccessRoleType.View)]
        public ReceiptResponse GetOrphan(int id)
        {
            var response = this.InitialiseResponse<ReceiptResponse>();
            response.Item = ((ReceiptRepository)this.Repository).GetOrphan(id);
            return response;
        }

        /// <summary>
        /// Adds the supplied <see cref="Models.Types.Expedite.Receipt">Receipt</see>.
        /// <strong>Make sure the Extension, EnvelopeId, and Data properties are populated.</strong>
        /// The others will be set and returned when the addition of the Receipt is complete.<br/>
        /// Also note that that you can attach at the same time as adding. If you populate
        /// the ClaimLines, ClaimId and EmployeeId, these will be assigned in that order, but only 
        /// one of them will be assigned - as the receipt can only be against either claim lines, 
        /// a claim header or an employee. The most granular setting will be used. 
        /// Alternatively, use the attach patch methods.
        /// </summary>
        /// <param name="request">The Receipt to add.</param>
        /// <returns>The newly added Receipt.</returns>
        [Route("~/Expedite/Receipts")]
        [AuthAudit(SpendManagementElement.None, AccessRoleType.Add)]
        public ReceiptResponse Post([FromBody] Receipt request)
        {
            request.ModifiedOn = DateTime.UtcNow;
            request.ModifiedById = this.CurrentUser.EmployeeID;
            request.CreationMethod = ReceiptCreationMethod.UploadedByExpedite;
            var response = this.InitialiseResponse<ReceiptResponse>();
            response.Item = ((ReceiptRepository)this.Repository).Add(request);
            return response;
        }

        /// <summary>
        /// Adds the supplied <see cref="Models.Types.Expedite.Receipt">Receipt</see> to an account.
        /// <strong>Make sure the Extension, EnvelopeId, and Data properties are populated.</strong>
        /// The others will be set and returned when the addition of the Receipt is complete.<br/>
        /// Also note that that you can attach at the same time as adding. If you populate
        /// the ClaimLines, ClaimId and EmployeeId, these will be assigned in that order, but only 
        /// one of them will be assigned - as the receipt can only be against either claim lines, 
        /// a claim header or an employee. The most granular setting will be used. 
        /// Alternatively, use the attach patch methods.
        /// </summary>
        /// <param name="request">The Receipt to add.</param>
        /// <returns>The newly added Receipt.</returns>
        [Route("~/Expedite/PostExpediteReceipt")]
        [NoAuthorisationRequired]
        public ReceiptResponse PostExpediteReceipt([FromBody] ExpediteReceipt request)
        {
            request.ModifiedOn = DateTime.UtcNow;
            request.CreationMethod = ReceiptCreationMethod.UploadedByExpedite;
            var response = this.InitialiseResponse<ReceiptResponse>();
            response.Item = ((ReceiptRepository)this.Repository).AddExpediteReceipt(request);
            return response;
        }

        /// <summary>
        /// Adds the supplied <see cref="Models.Types.Expedite.Receipt">Receipt</see> to the Metabase as an orphan...
        /// <strong>Make sure the Extension and Data properties are populated.</strong>
        /// A receipt is an orphan when there are no account details to assign it to.
        /// It floats in a holding area until its ownership can be resolved manually.
        /// </summary>
        /// <param name="request">The Orphaned Receipt to add.</param>
        /// <returns>The newly added Receipt.</returns>
        [Route("~/Expedite/OrphanedReceipts")]
        [AuthAudit(SpendManagementElement.None, AccessRoleType.Add)]
        public ReceiptResponse PostOrphan([FromBody] Receipt request)
        {
            request.ModifiedOn = DateTime.UtcNow;
            request.ModifiedById = this.CurrentUser.EmployeeID;
            request.CreationMethod = ReceiptCreationMethod.UploadedByExpedite;
            var response = this.InitialiseResponse<ReceiptResponse>();
            response.Item = ((ReceiptRepository)this.Repository).AddOrphan(request);
            return response;
        }

        /// <summary>
        /// Attaches a single <see cref="Models.Types.Expedite.Receipt">Receipt</see> to a claim line (savedexpense).
        /// </summary>
        /// <param name="id">The id of the <see cref="Models.Types.Expedite.Receipt">Receipt</see> to issue.</param>
        /// <param name="claimLineId">The id of the claim line (savedexpense) to attach.</param>
        /// <returns>The newly attached Receipt.</returns>
        [HttpPatch, Route("~/Expedite/Receipts/{id:int}/AttachToClaimLine/{claimLineId:int}")]
        [AuthAudit(SpendManagementElement.None, AccessRoleType.Edit)]
        public ReceiptResponse AttachToClaimLine(int id, int claimLineId)
        {
            var response = this.InitialiseResponse<ReceiptResponse>();
            response.Item = ((ReceiptRepository)this.Repository).LinkToClaimLine(id, claimLineId);
            return response;
        }

        /// <summary>
        /// Detaches a <see cref="Models.Types.Expedite.Receipt">Receipt</see> from a claim line (savedexpense).
        /// </summary>
        /// <param name="id">The id of the <see cref="Models.Types.Expedite.Receipt">Receipt</see> to issue.</param>
        /// <param name="claimLineId">The id of the claim line (savedexpense) to attach.</param>
        /// <returns>The newly attached Receipt.</returns>
        [HttpPatch, Route("~/Expedite/Receipts/{id:int}/DetatchFromClaimLine/{claimLineId:int}")]
        [AuthAudit(SpendManagementElement.None, AccessRoleType.Edit)]
        public ReceiptResponse DetatchFromClaimLine(int id, int claimLineId)
        {
            var response = this.InitialiseResponse<ReceiptResponse>();
            response.Item = ((ReceiptRepository)this.Repository).UnlinkFromClaimLine(id, claimLineId);
            return response;
        }

        /// <summary>
        /// Attaches a single <see cref="Models.Types.Expedite.Receipt">Receipt</see> to a claim.
        /// </summary>
        /// <param name="id">The id of the <see cref="Models.Types.Expedite.Receipt">Receipt</see> to issue.</param>
        /// <param name="claimId">The id of the claim to attach.</param>
        /// <returns>The newly attached Receipt.</returns>
        [HttpPatch, Route("~/Expedite/Receipts/{id:int}/AttachToClaim/{claimId:int}")]
        [AuthAudit(SpendManagementElement.None, AccessRoleType.Edit)]
        public ReceiptResponse AttachToClaim(int id, int claimId)
        {
            var response = this.InitialiseResponse<ReceiptResponse>();
            response.Item = ((ReceiptRepository)this.Repository).LinkToClaim(id, claimId);
            return response;
        }

        /// <summary>
        /// Attaches a single <see cref="Models.Types.Expedite.Receipt">Receipt</see> to a claimant (employee).
        /// </summary>
        /// <param name="id">The id of the <see cref="Models.Types.Expedite.Receipt">Receipt</see> to issue.</param>
        /// <param name="employeeId">The id of the employee to attach.</param>
        /// <returns>The newly attached Receipt.</returns>
        [HttpPatch, Route("~/Expedite/Receipts/{id:int}/AttachToClaimant/{employeeId:int}")]
        [AuthAudit(SpendManagementElement.None, AccessRoleType.Edit)]
        public ReceiptResponse AttachToClaimant(int id, int employeeId)
        {
            var response = this.InitialiseResponse<ReceiptResponse>();
            response.Item = ((ReceiptRepository)this.Repository).LinkToClaimant(id, employeeId);
            return response;
        }

        /// <summary>
        /// Deletes an Envelope, by setting its deleted flag to true. Receipts on an account cannot be deleted.
        /// </summary>
        /// <param name="id">The id of the <see cref="Models.Types.Expedite.Receipt">Receipt</see> to delete.</param>
        /// <returns>No content, or throws an error.</returns>
        [HttpDelete, Route("~/Expedite/Receipts/{id:int}")]
        [AuthAudit(SpendManagementElement.None, AccessRoleType.Edit)]
        public ReceiptResponse Delete(int id)
        {
            var response = this.InitialiseResponse<ReceiptResponse>();
            response.Item = ((ReceiptRepository)this.Repository).Delete(id);
            return response;
        }

        /// <summary>
        /// Deletes an Orphaned Envelope from the metabase. This delete is permanent.
        /// You should only do it after you have assigned a copy to an account.
        /// </summary>
        /// <param name="id">The id of the Orphaned. <see cref="Models.Types.Expedite.Receipt">Receipt</see> to delete.</param>
        /// <returns>No content, or throws an error.</returns>
        [HttpDelete, Route("~/Expedite/OrphanedReceipts/{id:int}")]
        [AuthAudit(SpendManagementElement.None, AccessRoleType.Edit)]
        public ReceiptResponse DeleteOrphan(int id)
        {
            var response = this.InitialiseResponse<ReceiptResponse>();
            response.Item = ((ReceiptRepository)this.Repository).DeleteOrphan(id);
            return response;
        }

        #endregion Receipt Methods
    }
}
