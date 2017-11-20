
namespace SpendManagementApi.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using SpendManagementApi.Models.Common;
    using SpendManagementApi.Utilities;

    using Spend_Management;
    using SpendManagementApi.Interfaces;
    using SpendManagementApi.Models.Types.ClaimableItem;

    /// <summary>
    /// Manages data access for <see cref="ClaimableItem">ClaimableItem</see>
    /// </summary>
    internal class ClaimableItemsRepository : BaseRepository<ClaimableItem>, ISupportsActionContext
    {
        private readonly cSubcats _data;

        /// <summary>
        /// Gets all of the available end points from the <see cref="ClaimableItem">ClaimableItem</see> part of the API.
        /// </summary>
        /// <param name="user">
        /// The current user.
        /// </param>
        /// <param name="actionContext">
        /// The action context.
        /// </param>
        public ClaimableItemsRepository(ICurrentUser user, IActionContext actionContext)
            : base(user, actionContext, x => x.AccountId.Value, null)
        {
            _data = ActionContext.SubCategories;
        }

        /// <summary>
        /// The get all ClaimableItems.
        /// </summary>
        /// <returns>
        /// The <see cref="IList">list of ClaimableItems</see>.
        /// </returns>
        public override IList<ClaimableItem> GetAll()
        {
            var roleItems = this._data.GetSubCatsByEmployeeItemRoles(this.User.EmployeeID);
            if (roleItems == null)
            {
                throw new ApiException(
                    ApiResources.ApiErrorClaimableItemsError,
                    ApiResources.ApiErrorClaimableItemNotFoundMessage);
            }

            return roleItems.Select(x => x.Cast<ClaimableItem>(this.User, this.ActionContext)).ToList();
        }

        /// <summary>
        /// The add claimable items will appear when user adds a new expense.
        /// </summary>
        /// <param name="subCatIds">
        /// The list of sub category ids.
        /// </param>
        /// <returns>
        /// The <see cref="IList">list of ClaimableItems</see>.
        /// </returns>
        public IList<ClaimableItem> AddClaimableItems(int[] subCatIds)
        {
            try
            {
                var employeeContext = this.ActionContext.Employees;
                var requestedEmployee = employeeContext.GetEmployeeById(this.User.EmployeeID);
                requestedEmployee.GetSubCategories().AddMultiple(subCatIds);
                return this.GetAll();
            }
            catch (Exception ex)
            {
                throw new ApiException(ApiResources.ApiErrorClaimableItemError, ApiResources.ApiErrorClaimableItemNotAddedMessage);
            }
        }

        public override ClaimableItem Get(int id)
        {
            throw new System.NotImplementedException();
        }
    }
}