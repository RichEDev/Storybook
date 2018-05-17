using System.Net;
using System.Web.Http;
using SpendManagementApi.Interfaces;

namespace SpendManagementApi.Repositories
{
    using System.Collections.Generic;
    using System.Linq;
    using Models.Common;
    using Models.Types;
    using Utilities;
    using Spend_Management;


    /// <summary>
    /// ClaimReasonRepository manages data access for ClaimReasons.
    /// </summary>
    internal class ClaimReasonRepository : BaseRepository<ClaimReason>, ISupportsActionContext
    {
        private cReasons _data;

        /// <summary>
        /// Creates a new ClaimReasonRepository with the passed in user.
        /// </summary>
        /// <param name="user">The current user.</param>
        /// <param name="actionContext">An implementation of ISupportsActionContext</param>
        public ClaimReasonRepository(ICurrentUser user, IActionContext actionContext) 
            : base(user, actionContext, x => x.Id, x => x.Label)
        {
            _data = ActionContext.ClaimReasons;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClaimReasonRepository"/> class.
        /// </summary>
        public ClaimReasonRepository()
        {
            
        }

        /// <summary>
        /// Gets all the ClaimReasons within the system.
        /// </summary>
        /// <returns>A list of <see cref="ClaimReason"/></returns>
        public override IList<ClaimReason> GetAll()
        {
            return _data.CachedList().Select(b => new ClaimReason().From(b, ActionContext)).ToList();
        }

        /// <summary>
        /// Gets all the unarchived <see cref="ClaimReason"/> within the system.
        /// </summary>
        /// <returns>A list of <see cref="ClaimReason"/></returns>
        public IList<ClaimReason> GetAllUnarchived()
        {
            return this._data.CachedList().Select(b => new ClaimReason().From(b, ActionContext)).Where(e => e.Archived == false).ToList();
        }

        /// <summary>
        /// Gets a single ClaimReason by it's id.
        /// </summary>
        /// <param name="id">The id of the ClaimReason to get.</param>
        /// <returns>The budget holder.</returns>
        public override ClaimReason Get(int id)
        {
            _data = new cReasons(User.AccountID);
            var item = _data.getReasonById(id);
          
            if (item == null)
            {
                throw new ApiException(string.Format(ApiResources.ApiErrorXUnsuccessfulMessage, "Get Claim Reason"),
                     string.Format(ApiResources.ApiErrorAnNonExistentX, "Claim Reason"));
            }

            return new ClaimReason().From(item, ActionContext);
        }

        /// <summary>
        /// Adds a budget holder.
        /// </summary>
        /// <param name="item">The budget holder to add.</param>
        /// <returns></returns>
        public override ClaimReason Add(ClaimReason item)
        {
            item = base.Add(item);

            var id = _data.saveReason(item.To(ActionContext));
            
            if (id == -1)
            {
                throw new ApiException(ApiResources.ApiErrorRecordAlreadyExists,
                        ApiResources.ApiErrorRecordAlreadyExistsMessage);
            }
            
            if (id < 1)
            {
                throw new ApiException(ApiResources.ApiErrorSaveUnsuccessful,
                        ApiResources.ApiErrorSaveUnsuccessfulMessage);
            }

            var result = Get(id);
            return result;
        }

        /// <summary>
        /// Updates a budget holder.
        /// </summary>
        /// <param name="item">The item to update.</param>
        /// <returns>The updated ClaimReason.</returns>
        public override ClaimReason Update(ClaimReason item)
        {
            item = base.Update(item);

            var id = _data.saveReason(item.To(ActionContext));
            if (id == 1)
            {
                throw new ApiException(ApiResources.ApiErrorRecordAlreadyExists,
                        ApiResources.ApiErrorRecordAlreadyExistsMessage);
            }

            item = Get(id);
            return item;
        }

        /// <summary>
        /// Deletes a budget holder, given it's ID.
        /// </summary>
        /// <param name="id">The Id of the ClaimReason to delete.</param>
        /// <returns>The deleted budget holder.</returns>
        public override ClaimReason Delete(int id)
        {
            var item = base.Delete(id);

            var result = _data.deleteReason(id);
            if (result != 0)
            {
                throw new ApiException(ApiResources.ApiErrorDeleteUnsuccessful,
                    ApiResources.ApiErrorDeleteUnsuccessfulMessage + ApiResources.ApiErrorDeleteUnsuccessfulMessageUserDefinedClaimReasons);
            }
            
            return item;
        }
    }
}