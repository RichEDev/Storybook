namespace SpendManagementApi.Repositories
{
    using System.Collections.Generic;
    using System.Linq;
    using Interfaces;
    using Models.Common;
    using Models.Types;
    using Utilities;
    using Spend_Management;

    internal class NhsTrustRepository : ArchivingBaseRepository<NhsTrust>
    {
        private cESRTrusts _data;

        /// <summary>
        /// Creates a new NhsTrustRepository with the passed in user.
        /// </summary>
        /// <param name="user"></param>
        public NhsTrustRepository(ICurrentUser user)
            : base(user, x => x.Id, x => x.Label)
        {
            _data = new cESRTrusts(User.AccountID);
        }

        /// <summary>
        /// Creates a new NhsTrustsRepository with the passed in user.
        /// </summary>
        /// <param name="user">The current user.</param>
        /// <param name="actionContext">An implementation of ISupportsActionContext</param>
        public NhsTrustRepository(ICurrentUser user, IActionContext actionContext) 
            : base(user, actionContext, x => x.Id, x => x.Label)
        {
            _data = ActionContext.NhsTrusts;
        }

        /// <summary>
        /// Gets all the NhsTrusts within the system.
        /// </summary>
        /// <returns></returns>
        public override IList<NhsTrust> GetAll()
        {
            return _data.Trusts.Select(b => new NhsTrust().From(b.Value, ActionContext)).ToList();
        }

        /// <summary>
        /// Gets a single NhsTrust by it's id.
        /// </summary>
        /// <param name="id">The id of the NhsTrust to get.</param>
        /// <returns>The NhsTrust.</returns>
        public override NhsTrust Get(int id)
        {
            _data = new cESRTrusts(User.AccountID);
            var item = _data.GetESRTrustByID(id);
            if (item == null)
            {
                return null;
            }
            return new NhsTrust().From(item, ActionContext);
        }

        /// <summary>
        /// Adds a NhsTrust.
        /// </summary>
        /// <param name="item">The NhsTrust to add.</param>
        /// <returns></returns>
        public override NhsTrust Add(NhsTrust item)
        {
            item = base.Add(item);
            return SaveNhsTrust(item);
        }

        /// <summary>
        /// Updates a NhsTrust.
        /// </summary>
        /// <param name="item">The item to update.</param>
        /// <returns>The updated NhsTrust.</returns>
        public override NhsTrust Update(NhsTrust item)
        {
            item = base.Update(item);
            return SaveNhsTrust(item);
        }

        /// <summary>
        /// Deletes a NhsTrust, given it's ID.
        /// </summary>
        /// <param name="id">The Id of the NhsTrust to delete.</param>
        /// <returns>The deleted NhsTrust.</returns>
        public override NhsTrust Delete(int id)
        {
            var item = base.Delete(id);

            var result = _data.DeleteTrust(id);
            if (result != 0)
            {
                throw new ApiException(ApiResources.ApiErrorDeleteUnsuccessful,
                    ApiResources.ApiErrorDeleteUnsuccessfulMessage);
            }

            return item;
        }

        /// <summary>
        /// Archives / unarchives the item with the specified id.
        /// </summary>
        /// <param name="id">The Id of the item to Archive/Unarchive.</param>
        /// <param name="archive">Whether to archive or unarchive.</param>
        /// <returns>The edited NhsTrust</returns>
        public override NhsTrust Archive(int id, bool archive)
        {
            var item = base.Archive(id, archive);
            _data.ArchiveTrust(id);
            return item;
        }


        private NhsTrust SaveNhsTrust(NhsTrust item)
        {
            var id = _data.SaveTrust(item.To(ActionContext));

            // throw here for when the description is in use.
            if (id.Contains(-2))
            {
                throw new ApiException(ApiResources.ApiErrorSaveUnsuccessful,
                        ApiResources.ApiErrorNhsTrustVPDExists);
            }

            // no change on the label
            if (id.Contains(-1))
            {
                throw new ApiException(ApiResources.ApiErrorSaveUnsuccessful,
                        ApiResources.ApiErrorNhsTrustExists);
            }

            return Get(id[0]);
        }
    }
}