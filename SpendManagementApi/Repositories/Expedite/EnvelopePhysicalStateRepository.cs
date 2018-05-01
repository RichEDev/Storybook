namespace SpendManagementApi.Repositories.Expedite
{
    using System.Collections.Generic;
    using System.Linq;
    using Interfaces;
    using Models.Common;

    using SpendManagementLibrary.Expedite;

    using Utilities;
    using SpendManagementLibrary.Interfaces.Expedite;
    using Spend_Management;

    using EnvelopePhysicalState = SpendManagementApi.Models.Types.Expedite.EnvelopePhysicalState;

    /// <summary>
    /// EnvelopeRepository manages data access for Envelopes.
    /// </summary>
    internal class EnvelopePhysicalStateRepository : BaseRepository<EnvelopePhysicalState>, ISupportsActionContext
    {
        /// <summary>
        /// An instance of <see cref="IManageEnvelopes"/>
        /// </summary>
        private readonly IManageEnvelopes _data;

        /// <summary>
        /// An instance of <see cref="IActionContext"/>
        /// </summary>
        private readonly IActionContext _actionContext = null;

        /// <summary>
        /// Creates a new EnvelopeRepository with the passed in user.
        /// </summary>
        /// <param name="user">The current user.</param>
        /// <param name="actionContext">An implementation of ISupportsActionContext</param>
        public EnvelopePhysicalStateRepository(ICurrentUser user, IActionContext actionContext)
            : base(user, actionContext, x => x.EnvelopePhysicalStateId, x => x.Details)
        {
            _data = ActionContext.Envelopes;
        }

        public EnvelopePhysicalStateRepository()
        {
            this._data = new Envelopes();
        }

        /// <summary>
        /// Gets all the EnvelopePhysicalStates within the system.
        /// </summary>
        /// <returns>A list of envelopes.</returns>
        public override IList<EnvelopePhysicalState> GetAll()
        {
            return _data.GetAllEnvelopePhysicalStates().Select(e => new EnvelopePhysicalState().From(e, this._actionContext)).ToList();
        }
        
        /// <summary>
        /// Gets anEnvelopePhysicalState by it's Id.
        /// </summary>
        /// <param name="id">The Id of the envelope.</param>
        /// <returns>The envelope with the matching Id.</returns>
        public override EnvelopePhysicalState Get(int id)
        {
            return new EnvelopePhysicalState().From(_data.GetEnvelopePhysicalStateById(id), ActionContext);
        }
        
        /// <summary>
        /// Creates an EnvelopePhysicalState in the system and returns the newly created EnvelopePhysicalState.
        /// </summary>
        /// <param name="item">The item to create.</param>
        /// <returns>The newly created EnvelopePhysicalState.</returns>
        public override EnvelopePhysicalState Add(EnvelopePhysicalState item)
        {
            return new EnvelopePhysicalState().From(_data.AddEnvelopePhysicalState(item.Details), ActionContext);
        }
        
        /// <summary>
        /// Edits an EnvelopePhysicalState. Ensure the Id is set correctly.
        /// </summary>
        /// <param name="item">The EnvelopePhysicalState to edit.</param>
        /// <returns>The edited EnvelopePhysicalState.</returns>
        public override EnvelopePhysicalState Update(EnvelopePhysicalState item)
        {
            return new EnvelopePhysicalState().From(_data.EditEnvelopePhysicalState(item.EnvelopePhysicalStateId, item.Details), ActionContext);
        }
        
        /// <summary>
        /// Deletes an EnvelopePhysicalState, given it's ID.
        /// </summary>
        /// <param name="id">The Id of the EnvelopePhysicalState to delete.</param>
        /// <returns>Null if the EnvelopePhysicalState was deleted successfully.</returns>
        public override EnvelopePhysicalState Delete(int id)
        {
            var item = base.Delete(id);

            var result = _data.DeleteEnvelopePhysicalState(item.EnvelopePhysicalStateId);
            if (!result)
            {
                throw new ApiException(ApiResources.ApiErrorDeleteUnsuccessful,
                    ApiResources.ApiErrorDeleteUnsuccessfulMessage);
            }

            return item;
        }
    }
}