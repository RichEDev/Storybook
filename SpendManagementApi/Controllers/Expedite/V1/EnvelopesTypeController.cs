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
    [RoutePrefix("Expedite/EnvelopeTypes")]
    [Version(1)]
    [InternalSelenityMethod, ApiExplorerSettings(IgnoreApi = true)]
    public class EnvelopeTypesV1Controller : BaseApiController<EnvelopeType>
    {
        /// <summary>
        /// Gets ALL of the available end points from the EnvelopeType API.
        /// </summary>
        /// <returns>A list of available Links</returns>
        [HttpOptions]
        public List<Link> Options()
        {
            return this.Links("Options");
        }

        #region Envelope Methods

        /// <summary>
        /// Gets all <see cref="Envelope">EnvelopeType</see> in the system.
        /// </summary>
        [HttpGet, Route("")]
        [NoAuthorisationRequired]
        public GetEnvelopeTypeResponse GetAll()
        {
            return this.GetAll<GetEnvelopeTypeResponse>();
        }

        /// <summary>
        /// Gets a single <see cref="Envelope">Envelope</see>, by its Id.
        /// </summary>
        /// <param name="id">The Id of the item to get.</param>
        /// <returns>A EnvelopeTypeResponse, containing the <see cref="Envelope">Envelope</see> if found.</returns>
        [HttpGet, Route("{id:int}")]
        [AuthAudit(SpendManagementElement.None, AccessRoleType.View)]
        public EnvelopeTypeResponse Get([FromUri] int id)
        {
            return this.Get<EnvelopeTypeResponse>(id);
        }

        /// <summary>
        /// Adds the supplied <see cref="Envelope">Envelope</see>.
        /// </summary>
        /// <param name="request">The Envelope to add.</param>
        /// <returns>The newly added Envelope.</returns>
        [Route("")]
        [AuthAudit(SpendManagementElement.None, AccessRoleType.Add)]
        public EnvelopeTypeResponse Post([FromBody] EnvelopeType request)
        {
            return this.Post<EnvelopeTypeResponse>(request);
        }

        /// <summary>
        /// Edits an existing <see cref="Envelope">Envelope</see>.
        /// </summary>
        /// <param name="id">The Id of the Envelope to modify</param>
        /// <param name="request">The Envelope to modify.</param>
        /// <returns>The updated Envelope.</returns>
        [Route("{id:int}")]
        [AuthAudit(SpendManagementElement.None, AccessRoleType.Edit)]
        public EnvelopeTypeResponse Put([FromUri] int id, [FromBody] EnvelopeType request)
        {
            request.Id = id;
            return this.Put<EnvelopeTypeResponse>(request);
        }

        /// <summary>
        /// Deletes a <see cref="Envelope">Envelope</see> associated with the current user
        /// </summary>
        /// <param name="id">Envelope Id</param>
        /// <returns>An EnvelopeTypeResponse with the item set to null upon a successful delete.</returns>
        [Route("{id:int}")]
        [AuthAudit(SpendManagementElement.None, AccessRoleType.Delete)]
        public EnvelopeTypeResponse Delete(int id)
        {
            return this.Delete<EnvelopeTypeResponse>(id);
        }

        #endregion Envelope Methods

    }
}