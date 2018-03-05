using BusinessLogic.DataConnections;
using BusinessLogic.P11DCategories;

namespace SpendManagementApi.Repositories
{
    using System.Collections.Generic;
    using System.Linq;
    using Models.Common;
    using Models.Types;
    using Utilities;
    using Spend_Management;

    /// <summary>
    /// Manages data access for the P11D Category.
    /// </summary>
    internal class P11DCategoryRepository : BaseRepository<P11DCategory>
    {
        private readonly IDataFactory<IP11DCategory, int> _p11DCategories;

        /// <summary>
        /// Creates a new P11DCategoryRepository
        /// </summary>
        /// <param name="user"></param>
        public P11DCategoryRepository(ICurrentUser user) : base(user, x => x.Id, x => x.Label)
        {
            this._p11DCategories = WebApiApplication.container.GetInstance<IDataFactory<IP11DCategory, int>>();
        }


        /// <summary>
        /// Gets all P11DCategories, also populating their expenseItems property.
        /// </summary>
        /// <returns>A List of P11DCategory.</returns>
        public override IList<P11DCategory> GetAll()
        {
            return this._p11DCategories.Get().Select(a => new P11DCategory().From(a, this.ActionContext)).ToList();
        }

        /// <summary>
        /// Gets a specific P11DCategory, by id.
        /// </summary>
        /// <param name="id">The Id of the P11DCategory to look for.</param>
        /// <returns>The P11DCategory.</returns>
        public override P11DCategory Get(int id)
        {
            IP11DCategory p11DCategory = this._p11DCategories[id];

            if (p11DCategory == null)
            {
                return null;
            }

            return new P11DCategory().From(p11DCategory, this.ActionContext);
        }

        /// <summary>
        /// Adds a P11Category.
        /// </summary>
        /// <param name="item">The P11DCategory to add.</param>
        /// <returns>The added P11DCategory.</returns>
        /// <exception cref="ApiException">Any error in adding the P11DCategory</exception>
        public override P11DCategory Add(P11DCategory item)
        {
            item = base.Add(item);
            return this.SaveP11DCategory(item);
        }

        /// <summary>
        /// Updates a PD11Category.
        /// </summary>
        /// <param name="item">The P11DCategory to update.</param>
        /// <returns>The update P11DCategory.</returns>
        /// <exception cref="ApiException">Any error in update the P11DCategory</exception>
        public override P11DCategory Update(P11DCategory item)
        {
            item = base.Update(item);
            return this.SaveP11DCategory(item);
        }

        /// <summary>
        /// Deletes a P11DCategory, by it's id.
        /// </summary>
        /// <param name="id">The id of the P11DCategory to delete.</param>
        /// <returns>The deleted P11DCategory.</returns>
        /// <exception cref="ApiException">Any error in deleting the P11DCategory</exception>
        public override P11DCategory Delete(int id)
        {
            var item = base.Delete(id);

            var result = this._p11DCategories.Delete(id);

            if (result != 0)
            {
                throw new ApiException(ApiResources.ApiErrorDeleteUnsuccessful,
                    ApiResources.ApiErrorDeleteUnsuccessfulMessage + ": SubCategory.");
            }

            return item;
        }

        private P11DCategory SaveP11DCategory(P11DCategory item)
        {
            var result = this._p11DCategories.Save(item.To(this.ActionContext));

            if (result.Id > 1)
            {
                item.Id = result.Id;

                var subcats = new cSubcats(this.ActionContext.AccountId);
                subcats.AssignP11DToSubcats(item.ExpenseSubCategoryIds.ToArray(), item.Id);
            }

            if (result.Id == -1)
            {
                throw new ApiException(ApiResources.ApiErrorRecordAlreadyExists,
                    ApiResources.ApiErrorRecordAlreadyExistsMessage);
            }

            if (result.Id == 0)
            {
                throw new ApiException(ApiResources.ApiErrorGeneralError,
                    ApiResources.ApiErrorRecordAlreadyExistsMessage);
            }

            return this.Get(item.Id);
        }
    }
}
