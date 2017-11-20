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
        private cP11dcats _data;
        private cSubcats _subCat;

        /// <summary>
        /// Creates a new P11DCategoryRepository
        /// </summary>
        /// <param name="user"></param>
        public P11DCategoryRepository(ICurrentUser user) : base(user, x => x.Id, x => x.Label)
        {
            _data = new cP11dcats(User.AccountID);
            _subCat = ActionContext.SubCategories;
        }


        /// <summary>
        /// Gets all P11DCategories, also populating their expenseItems property.
        /// </summary>
        /// <returns>A List of P11DCategory.</returns>
        public override IList<P11DCategory> GetAll()
        {
            var items = _data.GetAll().Select(ar => new P11DCategory
            {
                AccountId = User.AccountID,
                Label = ar.pdname,
                Id = ar.pdcatid
                
            }).ToList();

            var subCats = _subCat.GetSortedList();

            foreach (var item in items)
            {
                var itemId = item.Id;
                item.ExpenseSubCategoryIds = subCats.Where(x => x.P11DCategoryId == itemId).Select(x => x.SubcatId).ToList();
            }
            return items;
        }

        /// <summary>
        /// Gets a specific P11DCategory, by id.
        /// </summary>
        /// <param name="id">The Id of the P11DCategory to look for.</param>
        /// <returns>The P11DCategory.</returns>
        public override P11DCategory Get(int id)
        {
            _data = new cP11dcats(User.AccountID);
            var dbItem = _data.getP11dCatById(id);
            _subCat = new cSubcats(User.AccountID);
            var subCats = _subCat.GetSortedList().Where(x => x.P11DCategoryId == id).Select(x => x.SubcatId).ToList();
            
            var result = new P11DCategory
            {
                AccountId = User.AccountID,
                Label = dbItem.pdname,
                Id = dbItem.pdcatid,
                ExpenseSubCategoryIds = subCats
            };
            return result;
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

            var result = _data.addP11dCat(item.Label, item.ExpenseSubCategoryIds.ToArray());

            if (result == 1)
            {
                throw new ApiException(ApiResources.ApiErrorRecordAlreadyExists,
                      ApiResources.ApiErrorInvalidNameAlreadyExists);
            }
            
            if (result < 0 || result > 1)
            {
                throw new ApiException(ApiResources.ApiErrorGeneralError,
                      ApiResources.ApiErrorRecordAlreadyExistsMessage);
            }

            // we have a working p11DCat, so get it's id + repopulate expense items
            var id = _data.getP11DByName(item.Label).pdcatid;
            return Get(id);
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

            var result = _data.updateP11dCat(item.Label, item.Id, item.ExpenseSubCategoryIds.ToArray());
            
            if (result == 1)
            {
                throw new ApiException(ApiResources.ApiErrorRecordAlreadyExists,
                      ApiResources.ApiErrorRecordAlreadyExistsMessage);
            }

            if (result != 0)
            {
                throw new ApiException(ApiResources.ApiErrorGeneralError,
                      ApiResources.ApiErrorRecordAlreadyExistsMessage);
            }

            // we have a working p11DCat, so get it's id + repopulate expense items
            var id = _data.getP11DByName(item.Label).pdcatid;
            return Get(id);
        }

        /// <summary>
        /// Deletes a P11DCategory, by it's id.
        /// </summary>
        /// <param name="id">The id of the P11DCategory to delete.</param>
        /// <returns>The deleted P11DCategory.</returns>
        /// <exception cref="ApiException">Any error in deleting the P11DCategory</exception>
        public override P11DCategory Delete(int id)
        {
            var result = base.Delete(id);

            var deleteResult = _data.deleteP11dCat(id);

            if (deleteResult != 0)
            {
                throw new ApiException(ApiResources.ApiErrorDeleteUnsuccessful,
                    ApiResources.ApiErrorDeleteUnsuccessfulMessage + ": SubCategory.");
            }

            return result;
        }



        
    }
}
