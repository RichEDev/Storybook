namespace SpendManagementApi.Controllers.Expedite.V1
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Web.Http;
    using System.Web.Http.Description;

    using SpendManagementApi.Attributes;
    using SpendManagementApi.Common;
    using SpendManagementApi.Interfaces;
    using SpendManagementApi.Models.Common;
    using SpendManagementApi.Models.Responses.Expedite;
    using SpendManagementApi.Models.Types.Expedite;
    using SpendManagementApi.Repositories.Expedite;

    using Spend_Management;

    /// <summary>
    /// Contains account specific actions.
    /// </summary>
    [RoutePrefix("Expedite/Envelopes")]
    [Version(1)]
    [InternalSelenityMethod, ApiExplorerSettings(IgnoreApi = true)]
    public class EnvelopesV1Controller : BaseApiController<Envelope>
    {
        /// <summary>
        /// Gets ALL of the available end points from the Envelopes API.
        /// </summary>
        /// <returns>A list of available Links</returns>
        [HttpOptions]
        public List<Link> Options()
        {
            return this.Links("Options");
        }

        #region Envelope Methods

        /// <summary>
        /// Gets all <see cref="Envelope">Envelopes</see> in the system.
        /// </summary>
        [HttpGet, Route("")]
        [AuthAudit(SpendManagementElement.None, AccessRoleType.View)]
        public GetEnvelopesResponse GetAll()
        {
            return this.GetAll<GetEnvelopesResponse>();
        }

        /// <summary>
        /// Gets all <see cref="Envelope">Envelopes</see> for the specified account.
        /// </summary>
        [HttpGet, Route("ByAccount/{accountId:int}")]
        [AuthAudit(SpendManagementElement.None, AccessRoleType.View)]
        public GetEnvelopesResponse GetByAccount(int accountId)
        {
            var response = this.InitialiseResponse<GetEnvelopesResponse>();
            response.List = ((EnvelopeRepository)this.Repository).GetByAccount(accountId).ToList();
            return response;
        }

        /// <summary>
        /// Gets all <see cref="Envelope">Envelopes</see> for the specified account.
        /// </summary>
        [HttpGet, Route("ByBatchCode/{batchCode}")]
        [NoAuthorisationRequired]
        public GetEnvelopesResponse GetByBatchCode(string batchCode)
        {
            var response = this.InitialiseResponse<GetEnvelopesResponse>();
            response.List = ((EnvelopeRepository)this.Repository).GetByBatchCode(batchCode).ToList();          
            return response;
        }

        /// <summary>
        /// Gets a single <see cref="Envelope">Envelope</see>, by its Id.
        /// </summary>
        /// <param name="id">The Id of the item to get.</param>
        /// <returns>A EnvelopesResponse, containing the <see cref="Envelope">Envelope</see> if found.</returns>
        [HttpGet, Route("{id:int}")]
        [AuthAudit(SpendManagementElement.Api, AccessRoleType.View)]
        public EnvelopeResponse Get([FromUri] int id)
        {
            return this.Get<EnvelopeResponse>(id);
        }

        /// <summary>
        /// Gets a single <see cref="Envelope">Envelope</see>, by its EnvelopeNumber.
        /// Note that in some error cases there may be multiple Envelopes with the same number.
        /// </summary>
        /// <param name="envelopeNumber">The EnvelopeNumber of the <see cref="Envelope">Envelope</see>.</param>
        /// <returns>A GetEnvelopesResponse, containing any <see cref="Envelope">Envelope</see>s found.</returns>
        [HttpGet, Route("ByEnvelopeNumber/{envelopeNumber}")]
        [NoAuthorisationRequired]
        public GetEnvelopesResponse GetByEnvelopeNumber([FromUri] string envelopeNumber)
        {
            var response = this.InitialiseResponse<GetEnvelopesResponse>();
            response.List = ((EnvelopeRepository)this.Repository).GetByEnvelopeNumber(envelopeNumber).ToList();
            return response;
        }

        /// <summary>
        /// Gets a single <see cref="Envelope">Envelope</see>, by its ClaimReferenceNumber.
        /// </summary>
        /// <param name="claimReferenceNumber">The ClaimReferenceNumber of the <see cref="Envelope">Envelope</see>.</param>
        /// <returns>A EnvelopesResponse, containing the <see cref="Envelope">Envelope</see> if found.</returns>
        [HttpGet, Route("ByClaimReferenceNumber/{claimReferenceNumber}")]
        [AuthAudit(SpendManagementElement.None, AccessRoleType.View)]
        public GetEnvelopesResponse GetByClaimReferenceNumber([FromUri] string claimReferenceNumber)
        {
            var response = this.InitialiseResponse<GetEnvelopesResponse>();
            response.List = ((EnvelopeRepository)this.Repository).GetByClaimReferenceNumber(claimReferenceNumber).ToList();
            return response;
        }

        /// <summary>
        /// Adds the supplied <see cref="Envelope">Envelope</see>.
        /// </summary>
        /// <param name="request">The Envelope to add.</param>
        /// <returns>The newly added Envelope.</returns>
        [Route("")]
        [AuthAudit(SpendManagementElement.None, AccessRoleType.Add)]
        public EnvelopeResponse Post([FromBody] Envelope request)
        {
            return this.Post<EnvelopeResponse>(request);
        }

        /// <summary>
        /// Generates a batch of<see cref="Envelope">Envelope</see>s, adds them and returns them with populated Ids.
        /// </summary>
        /// <param name="envelopeType">The index of a valid <see cref="EnvelopeType">EnvelopeType</see>, which will be used for all the envelopes.</param>
        /// <returns>The list of newly added <see cref="Envelope">Envelope</see>s.</returns>
        [Route("Batch/OfType/{envelopeType:int}")]
        [NoAuthorisationRequired]
        public GetEnvelopesResponse PostBatch(int envelopeType)
        {
            var response = this.InitialiseResponse<GetEnvelopesResponse>();
            response.List = ((EnvelopeRepository)this.Repository).AddBatch(envelopeType).ToList();
            return response;
        }

        /// <summary>
        /// Adds the supplied <see cref="Envelope">Envelope</see>s the the system.
        /// </summary>
        /// <param name="request">The list of <see cref="Envelope">Envelope</see>s to add.</param>
        /// <returns>The list of newly added <see cref="Envelope">Envelope</see>s.</returns>
        [Route("Batch")]
        [AuthAudit(SpendManagementElement.None, AccessRoleType.Add)]
        public GetEnvelopesResponse PostBatch(IList<Envelope> request)
        {
            var response = this.InitialiseResponse<GetEnvelopesResponse>();
            response.List = ((EnvelopeRepository)this.Repository).AddBatch(request).ToList();
            return response;
        }

        /// <summary>
        /// Edits an existing <see cref="Envelope">Envelope</see>.
        /// </summary>
        /// <param name="id">The Id of the Envelope to modify</param>
        /// <param name="request">The Envelope to modify.</param>
        /// <returns>The updated Envelope.</returns>
        [Route("{id:int}")]
        [NoAuthorisationRequired]
        public EnvelopeResponse Put([FromUri] int id, [FromBody] Envelope request)
        {
            request.Id = id;
            return this.Put<EnvelopeResponse>(request);
        }

        /// <summary>
        /// Edits the supplied <see cref="Envelope">Envelope</see>s.
        /// </summary>
        /// <param name="request">The list of <see cref="Envelope">Envelope</see>s to edit.</param>
        /// <returns>The list of edited <see cref="Envelope">Envelope</see>s.</returns>
        [Route("Batch")]
        [NoAuthorisationRequired]
        public GetEnvelopesResponse PutBatch(IList<Envelope> request)
        {
            var response = this.InitialiseResponse<GetEnvelopesResponse>();
            response.List = ((EnvelopeRepository)this.Repository).UpdateBatch(request).ToList();
            return response;
        }

        /// <summary>
        /// Deletes a <see cref="Envelope">Envelope</see> associated with the current user
        /// </summary>
        /// <param name="id">Envelope Id</param>
        /// <returns>An EnvelopeResponse with the item set to null upon a successful delete.</returns>
        [Route("{id:int}")]
        [AuthAudit(SpendManagementElement.None, AccessRoleType.Delete)]
        public EnvelopeResponse Delete(int id)
        {
            return this.Delete<EnvelopeResponse>(id);
        }

        /// <summary>
        /// Issues a single <see cref="Envelope">Envelope</see> to a client.
        /// </summary>
        /// <param name="id">The id of the <see cref="Envelope">Envelope</see> to issue.</param>
        /// <param name="accountId">The Account Id to tie the <see cref="Envelope">Envelope</see> to.</param>
        /// <returns>The newly issued Envelope.</returns>
        [HttpPatch, Route("{id:int}/IssueToAccount/{accountId:int}")]
        [AuthAudit(SpendManagementElement.None, AccessRoleType.Edit)]
        public EnvelopeResponse IssueToAccount(int id, int accountId)
        {
            var response = this.InitialiseResponse<EnvelopeResponse>();
            response.Item = ((EnvelopeRepository)this.Repository).IssueToAccount(id, accountId);
            return response;
        }

        /// <summary>
        /// Issues a batch of <see cref="Envelope">Envelope</see>s to a client.
        /// </summary>
        /// <param name="batchCode">The batch code (central part of EnvelopeNumber) of the envelopes to issue.</param>
        /// <param name="accountId">The AccountId of the client to issue the batch to.</param>
        /// <returns>The newly issued Envelope.</returns>
        [HttpPatch, Route("{batchCode}/IssueBatchToAccount/{accountId:int}")]
        [NoAuthorisationRequired]
        public GetEnvelopesResponse IssueBatchToAccount(string batchCode, int accountId)
        {
            var response = this.InitialiseResponse<GetEnvelopesResponse>();
            response.List = ((EnvelopeRepository)this.Repository).IssueToAccount(batchCode, accountId).ToList();
            return response;
        }

        /// <summary>
        /// Drops a single <see cref="Envelope">Envelope</see> into an account bucket for the administrator to assign.
        /// </summary>
        /// <param name="id">The id of the <see cref="Envelope">Envelope</see> to update.</param>
        /// <returns>The updated Envelope.</returns>
        [HttpPatch, Route("{id:int}/AssignToAccountAdmin")]
        [AuthAudit(SpendManagementElement.None, AccessRoleType.Edit)]
        public EnvelopeResponse AssignToAccountAdmin(int id)
        {
            var response = this.InitialiseResponse<EnvelopeResponse>();
            response.Item = ((EnvelopeRepository)this.Repository).AssignToAccountAdmin(id);
            return response;
        }

        /// <summary>
        /// Attaches a single <see cref="Envelope">Envelope</see> to a claim.
        /// </summary>
        /// <param name="id">The id of the <see cref="Envelope">Envelope</see> to issue.</param>
        /// <param name="claimId">The id of the claim to attach.</param>
        /// <returns>The newly attached Envelope.</returns>
        [HttpPatch, Route("{id:int}/AttachToClaim/{claimId:int}")]
        [AuthAudit(SpendManagementElement.None, AccessRoleType.Edit)]
        public EnvelopeResponse AttachToClaim(int id, int claimId)
        {
            var response = this.InitialiseResponse<EnvelopeResponse>();
            response.Item = ((EnvelopeRepository)this.Repository).AttachToClaim(id, claimId);
            return response;
        }

        /// <summary>
        /// Marks a single <see cref="Envelope">Envelope</see> as received at SEL.
        /// </summary>
        /// <param name="id">The id of the <see cref="Envelope">Envelope</see> to update.</param>
        /// <returns>The updated Envelope.</returns>
        [HttpPatch, Route("{id:int}/MarkReceived")]
        [NoAuthorisationRequired]
        public EnvelopeResponse MarkReceived(int id)
        {
            var response = this.InitialiseResponse<EnvelopeResponse>();
            response.Item = ((EnvelopeRepository)this.Repository).MarkReceived(id);
            return response;
        }

        /// <summary>
        /// Marks a single <see cref="Envelope">Envelope</see> as scanned and attached by SEL.
        /// </summary>
        /// <param name="id">The id of the <see cref="Envelope">Envelope</see> to update.</param>
        /// <returns>The updated Envelope.</returns>
        [HttpPatch, Route("{id:int}/MarkComplete")]
        [AuthAudit(SpendManagementElement.None, AccessRoleType.Edit)]
        public EnvelopeResponse MarkComplete(int id)
        {
            var response = this.InitialiseResponse<EnvelopeResponse>();
            response.Item = ((EnvelopeRepository)this.Repository).MarkComplete(id);
            return response;
        }

        /// <summary>
        /// Updates the status of a single <see cref="Envelope">Envelope</see>.
        /// </summary>
        /// <param name="id">The id of the <see cref="Envelope">Envelope</see> to update the status for.</param>
        /// <param name="status">The <see cref="EnvelopeStatus">EnvelopeStatus</see> to assign to the <see cref="Envelope">Envelope</see></param>
        /// <returns>The updated Envelope.</returns>
        [HttpPatch, Route("{id:int}/UpdateStatus/{status}")]
        [AuthAudit(SpendManagementElement.None, AccessRoleType.Edit)]
        public EnvelopeResponse UpdateStatus(int id, EnvelopeStatus status)
        {
            // validate the input
            this.EnsureEnumValueIsValid<EnvelopeStatus>(status);

            var response = this.InitialiseResponse<EnvelopeResponse>();
            response.Item = ((EnvelopeRepository)this.Repository).UpdateEnvelopeStatus(id, status);
            return response;
        }

        /// <summary>
        /// Updates the physical state of a single <see cref="Envelope">Envelope</see>.
        /// </summary>
        /// <param name="id">The id of the <see cref="Envelope">Envelope</see> to update the status for.</param>
        /// <param name="states">The list of <see cref="Models.Types.Expedite.EnvelopePhysicalState">PhysicalEnvelopeState</see> to assign to the <see cref="Envelope">Envelope</see></param>
        /// <returns>The updated Envelope.</returns>
        [HttpPatch, Route("{id:int}/UpdatePhysicalState")]
        [AuthAudit(SpendManagementElement.Api, AccessRoleType.Edit)]
        public EnvelopeResponse UpdatePhysicalState([FromUri] int id, [FromBody] List<int> states)
        {
            var response = this.InitialiseResponse<EnvelopeResponse>();
            response.Item = ((EnvelopeRepository)this.Repository).UpdateEnvelopePhysicalState(id, states);
            return response;
        }

        /// <summary>
        /// Loops through all sent envelopes and notifies claimants that their envelopes
        /// have not been received, depending on the number of days set in their account.
        /// </summary>
        [HttpPut, Route("NotifyClaimantsOfUnsentEnvelopes")]
        public void NotifyClaimantsOfUnsentEnvelopes()
        {
            if (this.Request.IsLocal())
            {
                var user = new CurrentUser();
                var repo = new EnvelopeRepository(user, new ActionContext(user));
                repo.NotifyClaimantsOfUnsentEnvelopes();
            }
        }

        #endregion Envelope Methods

    }
}
