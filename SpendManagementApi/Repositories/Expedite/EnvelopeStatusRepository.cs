namespace SpendManagementApi.Repositories.Expedite
{
    using System.Collections.Generic;
    using System.Linq;
    using Interfaces;
    using Models.Common;
    using Utilities;
    using SpendManagementLibrary.Interfaces.Expedite;
    using Spend_Management;
    using SpendManagementLibrary.Expedite;
    using EnvelopeStatus = Models.Types.Expedite.EnvelopeStatus;
    

    /// <summary>
    /// EnvelopeStatusRepository manages data access for EnvelopeStatus.
    /// </summary>
    internal class EnvelopeStatusRepository : BaseRepository<EnvelopeStatus>, ISupportsActionContext
    {
        private readonly IManageEnvelopes _data;

        /// <summary>
        /// Creates a new EnvelopeStatusRepository with the passed in user.
        /// </summary>
        /// <param name="user">The current user.</param>
        /// <param name="actionContext">An implementation of ISupportsActionContext</param>
        public EnvelopeStatusRepository(ICurrentUser user, IActionContext actionContext)
            : base(user, actionContext, x => x.Id, x => x.Label)
        {
            _data = ActionContext.Envelopes;
        }
        
        /// <summary>
        /// Creates a new EnvelopeTypeRepository with the passed in user.
        /// </summary>
        /// <param name="user"></param>
        public EnvelopeStatusRepository(ICurrentUser user) 
            : base(user, x => x.Id, x => x.Label)
        {
            _data = new Envelopes();
        }
        
        /// <summary>
        /// Gets all the EnvelopeStatuss within the system.
        /// </summary>
        /// <returns>A list of EnvelopeStatuss.</returns>
        public override IList<EnvelopeStatus> GetAll()
        {
            return _data.GetAllEnvelopeStatuses().Select(e => new EnvelopeStatus().From(e, ActionContext)).ToList();
        }

        /// <summary>
        /// Gets an EnvelopeStatus by it's Id.
        /// </summary>
        /// <param name="id">The Id of the EnvelopeStatus.</param>
        /// <returns>The EnvelopeStatus with the matching Id.</returns>
        public override EnvelopeStatus Get(int id)
        {
            return new EnvelopeStatus().From(_data.GetEnvelopeStatusById(id), ActionContext);
        }
        
        /// <summary>
        /// Creates an EnvelopeStatus in the system and returns the newly created EnvelopeStatus.
        /// </summary>
        /// <param name="item">The item to create.</param>
        /// <returns>The newly created EnvelopeStatus.</returns>
        public override EnvelopeStatus Add(EnvelopeStatus item)
        {
            return new EnvelopeStatus().From(_data.AddEnvelopeStatus(item.Label), ActionContext);
        }

        /// <summary>
        /// Edits an EnvelopeStatus. Ensure the Id is set correctly.
        /// </summary>
        /// <param name="item">The EnvelopeStatus to edit.</param>
        /// <returns>The edited EnvelopeStatus.</returns>
        public override EnvelopeStatus Update(EnvelopeStatus item)
        {
            return new EnvelopeStatus().From(_data.EditEnvelopeStatus(item.Id, item.Label), ActionContext);
        }
        
        /// <summary>
        /// Deletes an EnvelopeStatus, given it's ID.
        /// </summary>
        /// <param name="id">The Id of the EnvelopeStatus to delete.</param>
        /// <returns>Null if the EnvelopeStatus was deleted successfully.</returns>
        public override EnvelopeStatus Delete(int id)
        {
            var item = base.Delete(id);

            var result = _data.DeleteEnvelopeStatus(item.Id);
            if (!result)
            {
                throw new ApiException(ApiResources.ApiErrorDeleteUnsuccessful,
                    ApiResources.ApiErrorDeleteUnsuccessfulMessage);
            }

            return item;
        }
    }
}