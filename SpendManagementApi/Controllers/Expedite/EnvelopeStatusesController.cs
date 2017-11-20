namespace SpendManagementApi.Controllers.Expedite
{
    using System.Collections.Generic;
    using System.Web.Http;
    using Attributes;
    using Interfaces;
    using Models.Common;
    using Models.Responses.Expedite;
    using Models.Types.Expedite;

    /// <summary>
    /// Contains account specific actions.
    /// </summary>
    [RoutePrefix("Expedite/EnvelopeStatuses")]
    //[ApiExplorerSettings(IgnoreApi = true)]
    public class EnvelopeStatusesController : BaseApiController<EnvelopeStatus>
    {
        #region Constructor

        /// <summary>
        /// Creates a new EnvelopesController with the specified repo.
        /// </summary>
        /// <param name="envelopeRepository">The repo.</param>
        public EnvelopeStatusesController(IRepository<EnvelopeStatus> envelopeRepository)
            : base(envelopeRepository)
        {
        }

        /// <summary>
        /// Creates a new EnvelopesController.
        /// </summary>
        public EnvelopeStatusesController()
        {
        }

        #endregion Constructor

        /// <summary>
        /// Gets ALL of the available end points from the EnvelopeStatus API.
        /// </summary>
        /// <returns>A list of available Links</returns>
        [HttpOptions]
        public List<Link> Options()
        {
            return Links("Options");
        }

        #region Envelope Methods

        /// <summary>
        /// Gets all <see cref="Envelope">EnvelopeStatus</see> in the system.
        /// </summary>
        [HttpGet, Route("")]
        [AuthAudit(SpendManagementElement.Expedite, AccessRoleType.View)]
        public GetEnvelopeStatusResponse GetAll()
        {
            return GetAll<GetEnvelopeStatusResponse>();
        }
        
        /// <summary>
        /// Gets a single <see cref="Envelope">Envelope</see>, by its Id.
        /// </summary>
        /// <param name="id">The Id of the item to get.</param>
        /// <returns>A EnvelopeStatusResponse, containing the <see cref="Envelope">Envelope</see> if found.</returns>
        [HttpGet, Route("{id:int}")]
        [AuthAudit(SpendManagementElement.Expedite, AccessRoleType.View)]
        public EnvelopeStatusResponse Get([FromUri] int id)
        {
            return Get<EnvelopeStatusResponse>(id);
        }

        /// <summary>
        /// Adds the supplied <see cref="Envelope">Envelope</see>.
        /// </summary>
        /// <param name="request">The Envelope to add.</param>
        /// <returns>The newly added Envelope.</returns>
        [Route("")]
        [AuthAudit(SpendManagementElement.Expedite, AccessRoleType.Add)]
        public EnvelopeStatusResponse Post([FromBody] EnvelopeStatus request)
        {
            return Post<EnvelopeStatusResponse>(request);
        }
        
        /// <summary>
        /// Edits an existing <see cref="Envelope">Envelope</see>.
        /// </summary>
        /// <param name="id">The Id of the Envelope to modify</param>
        /// <param name="request">The Envelope to modify.</param>
        /// <returns>The updated Envelope.</returns>
        [Route("{id:int}")]
        [AuthAudit(SpendManagementElement.Expedite, AccessRoleType.Edit)]
        public EnvelopeStatusResponse Put([FromUri] int id, [FromBody] EnvelopeStatus request)
        {
            request.Id = id;
            return Put<EnvelopeStatusResponse>(request);
        }

        /// <summary>
        /// Deletes a <see cref="Envelope">Envelope</see> associated with the current user
        /// </summary>
        /// <param name="id">Envelope Id</param>
        /// <returns>An EnvelopeStatusResponse with the item set to null upon a successful delete.</returns>
        [Route("{id:int}")]
        [AuthAudit(SpendManagementElement.Expedite, AccessRoleType.Delete)]
        public EnvelopeStatusResponse Delete(int id)
        {
            return Delete<EnvelopeStatusResponse>(id);
        }

        #endregion Envelope Methods

    }
}