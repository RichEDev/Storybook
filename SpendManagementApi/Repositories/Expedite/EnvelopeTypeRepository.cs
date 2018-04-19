using SpendManagementLibrary.Expedite;

namespace SpendManagementApi.Repositories.Expedite
{
    using System.Collections.Generic;
    using System.Linq;
    using Interfaces;
    using Models.Common;
    using Models.Types.Expedite;
    using Utilities;
    using SpendManagementLibrary.Interfaces.Expedite;
    using Spend_Management;

    /// <summary>
    /// EnvelopeRepository manages data access for Envelopes.
    /// </summary>
    internal class EnvelopeTypeRepository : BaseRepository<EnvelopeType>, ISupportsActionContext
    {
        /// <summary>
        /// An instance of <see cref="IManageEnvelopes"/>.
        /// </summary>
        private readonly IManageEnvelopes _data;

        /// <summary>
        /// An instance of <see cref="IActionContext"/>
        /// </summary>
        private readonly IActionContext _actionContext = null;

        /// <summary>
        /// Creates a new EnvelopeTypeRepository with the passed in user.
        /// </summary>
        /// <param name="user">The current user.</param>
        /// <param name="actionContext">An implementation of ISupportsActionContext</param>
        public EnvelopeTypeRepository(ICurrentUser user, IActionContext actionContext)
            : base(user, actionContext, x => x.Id, x => x.Label)
        {
            _data = ActionContext.Envelopes;
        }
        
        /// <summary>
        /// Creates a new EnvelopeTypeRepository with the passed in user.
        /// </summary>
        /// <param name="user"></param>
        public EnvelopeTypeRepository(ICurrentUser user) 
            : base(user, x => x.Id, x => x.Label)
        {
            _data = new Envelopes();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EnvelopeTypeRepository"/> class.
        /// </summary>
        public EnvelopeTypeRepository()
        {     
            _data = new Envelopes();
        }

        /// <summary>
        /// Gets all the EnvelopeTypes within the system.
        /// </summary>
        /// <returns>A list of EnvelopeTypes.</returns>
        public override IList<EnvelopeType> GetAll()
        {
            return _data.GetAllEnvelopeTypes().Select(e => new EnvelopeType().From(e, this._actionContext)).ToList();
        }

        /// <summary>
        /// Gets an EnvelopeType by it's Id.
        /// </summary>
        /// <param name="id">The Id of the EnvelopeType.</param>
        /// <returns>The EnvelopeType with the matching Id.</returns>
        public override EnvelopeType Get(int id)
        {
            return new EnvelopeType().From(_data.GetEnvelopeTypeById(id), ActionContext);
        }
        
        /// <summary>
        /// Creates an EnvelopeType in the system and returns the newly created EnvelopeType.
        /// </summary>
        /// <param name="item">The item to create.</param>
        /// <returns>The newly created EnvelopeType.</returns>
        public override EnvelopeType Add(EnvelopeType item)
        {
            return new EnvelopeType().From(_data.AddEnvelopeType(item.Label), ActionContext);
        }

        /// <summary>
        /// Edits an EnvelopeType. Ensure the Id is set correctly.
        /// </summary>
        /// <param name="item">The EnvelopeType to edit.</param>
        /// <returns>The edited EnvelopeType.</returns>
        public override EnvelopeType Update(EnvelopeType item)
        {
            return new EnvelopeType().From(_data.EditEnvelopeType(item.Id, item.Label), ActionContext);
        }
        
        /// <summary>
        /// Deletes an EnvelopeType, given it's ID.
        /// </summary>
        /// <param name="id">The Id of the EnvelopeType to delete.</param>
        /// <returns>Null if the EnvelopeType was deleted successfully.</returns>
        public override EnvelopeType Delete(int id)
        {
            var item = base.Delete(id);

            var result = _data.DeleteEnvelopeType(item.Id);
            if (!result)
            {
                throw new ApiException(ApiResources.ApiErrorDeleteUnsuccessful,
                    ApiResources.ApiErrorDeleteUnsuccessfulMessage);
            }

            return item;
        }
    }
}