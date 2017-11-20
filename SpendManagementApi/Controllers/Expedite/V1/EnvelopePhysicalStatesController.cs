namespace SpendManagementApi.Controllers.Expedite.V1
{
    using System.Collections.Generic;
    using System.Web.Http;
    using System.Web.Http.Description;

    using SpendManagementApi.Attributes;
    using SpendManagementApi.Interfaces;
    using SpendManagementApi.Models.Common;
    using SpendManagementApi.Models.Responses.Expedite;
    using SpendManagementApi.Models.Types.Expedite;

    /// <summary>
    /// Contains account specific actions.
    /// </summary>
    [RoutePrefix("Expedite/EnvelopePhysicalStates")]
    [Version(1)]
    [InternalSelenityMethod, ApiExplorerSettings(IgnoreApi = true)]
    public class EnvelopePhysicalStatesV1Controller : BaseApiController<EnvelopePhysicalState>
    {
        /// <summary>
        /// Gets ALL of the available end points from the EnvelopePhysicalStates API.
        /// </summary>
        /// <returns>A list of available Links</returns>
        [HttpOptions]
        public List<Link> Options()
        {
            return this.Links("Options");
        }

        #region Envelope Methods

        /// <summary>
        /// Gets all <see cref="EnvelopePhysicalState">EnvelopePhysicalStates</see> in the system.
        /// </summary>
        [HttpGet, Route("")]
        [AuthAudit(SpendManagementElement.Api, AccessRoleType.View)]
        public GetEnvelopePhysicalStatesResponse GetAll()
        {
            return this.GetAll<GetEnvelopePhysicalStatesResponse>();
        }
        
        /// <summary>
        /// Gets a single <see cref="EnvelopePhysicalState">EnvelopePhysicalState</see>, by its Id.
        /// </summary>
        /// <param name="id">The Id of the item to get.</param>
        /// <returns>A EnvelopePhysicalStatesResponse, containing the <see cref="EnvelopePhysicalState">EnvelopePhysicalState</see> if found.</returns>
        [HttpGet, Route("{id:int}")]
        [AuthAudit(SpendManagementElement.Api, AccessRoleType.View)]
        public EnvelopePhysicalStateResponse Get([FromUri] int id)
        {
            return this.Get<EnvelopePhysicalStateResponse>(id);
        }

        
        /// <summary>
        /// Adds the supplied <see cref="EnvelopePhysicalState">EnvelopePhysicalState</see>.
        /// </summary>
        /// <param name="request">The EnvelopePhysicalState to add.</param>
        /// <returns>The newly added EnvelopePhysicalState.</returns>
        [Route("")]
        [AuthAudit(SpendManagementElement.Api, AccessRoleType.Add)]
        public EnvelopePhysicalStateResponse Post([FromBody] EnvelopePhysicalState request)
        {
            return this.Post<EnvelopePhysicalStateResponse>(request);
        }
        
        /// <summary>
        /// Edits an existing <see cref="EnvelopePhysicalState">EnvelopePhysicalState</see>.
        /// </summary>
        /// <param name="id">The Id of the EnvelopePhysicalState to modify</param>
        /// <param name="request">The EnvelopePhysicalState to modify.</param>
        /// <returns>The updated EnvelopePhysicalState.</returns>
        [Route("{id:int}")]
        [AuthAudit(SpendManagementElement.Api, AccessRoleType.Edit)]
        public EnvelopePhysicalStateResponse Put([FromUri] int id, [FromBody] EnvelopePhysicalState request)
        {
            request.EnvelopePhysicalStateId = id;
            return this.Put<EnvelopePhysicalStateResponse>(request);
        }
        
        /// <summary>
        /// Deletes a <see cref="EnvelopePhysicalState">EnvelopePhysicalState</see> associated with the current user
        /// </summary>
        /// <param name="id">EnvelopePhysicalState Id</param>
        /// <returns>An EnvelopePhysicalState with the item set to null upon a successful delete.</returns>
        [Route("{id:int}")]
        [AuthAudit(SpendManagementElement.Api, AccessRoleType.Delete)]
        public EnvelopePhysicalStateResponse Delete(int id)
        {
            return this.Delete<EnvelopePhysicalStateResponse>(id);
        }

        #endregion Envelope Methods

    }
}
