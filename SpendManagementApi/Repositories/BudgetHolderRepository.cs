using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using SpendManagementApi.Interfaces;
using SpendManagementApi.Models.Common;
using SpendManagementApi.Models.Types;
using SpendManagementApi.Utilities;
using Spend_Management;

namespace SpendManagementApi.Repositories
{
    /// <summary>
    /// BudgetHolderRepository manages data access for BudgetHolders.
    /// </summary>
    internal class BudgetHolderRepository : BaseRepository<BudgetHolder>, ISupportsActionContext
    {
        private cBudgetholders _data;

        /// <summary>
        /// Creates a new BudgetHolderRepository with the passed in user.
        /// </summary>
        /// <param name="user">The current user.</param>
        /// <param name="actionContext">An implementation of ISupportsActionContext</param>
        public BudgetHolderRepository(ICurrentUser user, IActionContext actionContext)
            : base(user, actionContext, x => x.Id, x => x.Label)
        {
            _data = ActionContext.BudgetHolders;
        }

        /// <summary>
        /// Gets all the BudgetHolders within the system.
        /// </summary>
        /// <returns></returns>
        public override IList<BudgetHolder> GetAll()
        {
            return _data.GetList().Select(b => new BudgetHolder().From(b, ActionContext)).ToList();
        }

        /// <summary>
        /// Gets a single BudgetHolder by it's id.
        /// </summary>
        /// <param name="id">The id of the BudgetHolder to get.</param>
        /// <returns>The budget holder.</returns>
        public override BudgetHolder Get(int id)
        {
            _data = new cBudgetholders(User.AccountID);
            var item = _data.getBudgetHolderById(id);
            if (item == null)
            {
                return null;
            }
            return new BudgetHolder().From(item, ActionContext);
        }

        /// <summary>
        /// Adds a budget holder.
        /// </summary>
        /// <param name="item">The budget holder to add.</param>
        /// <returns></returns>
        public override BudgetHolder Add(BudgetHolder item)
        {
            var empId = item.EmployeeId;
            item = base.Add(item);
            item.EmployeeId = empId;

            // check employee here.
            var employees = new cEmployees(User.AccountID);
// ReSharper disable once PossibleInvalidOperationException
            if (employees.GetEmployeeById(item.EmployeeId.Value) == null)
            {
                throw new ApiException(ApiResources.ApiErrorWrongEmployeeId, 
                    ApiResources.ApiErrorWrongEmployeeIdMessage);
            }
            
            var id = _data.saveBudgetHolder(item.To(ActionContext));
            if (id < 1)
            {
                throw new ApiException(ApiResources.ApiErrorSaveUnsuccessful,
                        ApiResources.ApiErrorSaveUnsuccessfulMessage);
            }
            if (id == 1)
            {
                throw new ApiException(ApiResources.ApiErrorRecordAlreadyExists,
                        ApiResources.ApiErrorRecordAlreadyExistsMessage);
            }

            var result = Get(id);
            return result;
        }

        /// <summary>
        /// Updates a budget holder.
        /// </summary>
        /// <param name="item">The item to update.</param>
        /// <returns>The updated BudgetHolder.</returns>
        public override BudgetHolder Update(BudgetHolder item)
        {
            item = base.Update(item);

            // check employee here.
            var employees = new cEmployees(User.AccountID);
// ReSharper disable once PossibleInvalidOperationException
            if (employees.GetEmployeeById(item.EmployeeId.Value) == null)
            {
                throw new ApiException(ApiResources.ApiErrorWrongEmployeeId,
                    ApiResources.ApiErrorWrongEmployeeIdMessage);
            }

            var id = _data.saveBudgetHolder(item.To(ActionContext));
            if (id == 1)
            {
                throw new ApiException(ApiResources.ApiErrorRecordAlreadyExists,
                        ApiResources.ApiErrorRecordAlreadyExistsMessage);
            }

            return Get(id);
        }

        /// <summary>
        /// Deletes a budget holder, given it's ID.
        /// </summary>
        /// <param name="id">The Id of the BudgetHolder to delete.</param>
        /// <returns>The deleted budget holder.</returns>
        public override BudgetHolder Delete(int id)
        {
            var item = base.Delete(id);

            var result = _data.deleteBudgetHolder(id);
            if (result != 0)
            {
                throw new ApiException(ApiResources.ApiErrorDeleteUnsuccessful,
                    ApiResources.ApiErrorDeleteUnsuccessfulMessage);
            }

            return item;
        }
    }
}
