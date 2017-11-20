namespace SpendManagementApi.Repositories
{
    using System.Collections.Generic;
    using System.Linq;

    using SpendManagementApi.Interfaces;
    using SpendManagementApi.Models.Common;
    using SpendManagementApi.Models.Responses;
    using SpendManagementApi.Models.Types;
    using SpendManagementApi.Utilities;
    using Spend_Management;

    /// <summary>
    /// CostCodeRepository manages data access for CostCodes.
    /// </summary>
    internal class CostCodeRepository : ArchivingBaseRepository<CostCode>, ISupportsActionContext
    {
        private readonly cCostcodes _data;

        /// <summary>
        /// Creates a new CostCodeRepository with the passed in user.
        /// </summary>
        /// <param name="user">The current user.</param>
        /// <param name="actionContext">An implementation of ISupportsActionContext</param>
        public CostCodeRepository(ICurrentUser user, IActionContext actionContext) 
            : base(user, actionContext, x => x.Id, x => x.Label)
        {
            _data = ActionContext.CostCodes;
        }

        /// <summary>
        /// Gets all the CostCodes within the system.
        /// </summary>
        /// <returns></returns>
        public override IList<CostCode> GetAll()
        {
            return _data.CacheList().Select(b => new CostCode().From(b.Value, ActionContext)).ToList();
        }


        /// <summary>
        /// Gets all the active Cost Codes within the system.
        /// </summary>
        /// <returns>A list of <see cref="CostCodeBasic">CostCode</see></returns>
        public List<CostCodeBasic> GetAllActive()
        {
            return this._data.GetAllActiveCostCodes().Select(b => new CostCodeBasic().From(b, this.ActionContext)).ToList();
        }

        /// <summary>
        /// Gets a single CostCode by it's id.
        /// </summary>
        /// <param name="id">The id of the CostCode to get.</param>
        /// <returns>The CostCode.</returns>
        public override CostCode Get(int id)
        {
            var item = _data.GetCostcodeById(id);
            if (item == null)
            {
                return null;
            }
            return new CostCode().From(item, ActionContext);
        }

        /// <summary>
        /// Adds a CostCode.
        /// </summary>
        /// <param name="item">The CostCode to add.</param>
        /// <returns></returns>
        public override CostCode Add(CostCode item)
        {
            item = base.Add(item);
            return SaveCostCode(item);
        }

        /// <summary>
        /// Updates a CostCode.
        /// </summary>
        /// <param name="item">The item to update.</param>
        /// <returns>The updated CostCode.</returns>
        public override CostCode Update(CostCode item)
        {
            item = base.Update(item);
            return SaveCostCode(item);
        }

        /// <summary>
        /// Deletes a CostCode, given it's ID.
        /// </summary>
        /// <param name="id">The Id of the CostCode to delete.</param>
        /// <returns>The deleted CostCode.</returns>
        public override CostCode Delete(int id)
        {
            base.Delete(id);

            var result = _data.DeleteCostCode(id, User.EmployeeID);


            if (result == -4)
            {
                throw new ApiException(ApiResources.ApiErrorDeleteUnsuccessful, 
                    ApiResources.ApiErrorInvalidLinkedToExpenseItem);
            }
            
            if (result == -2)
            {
                throw new ApiException(ApiResources.ApiErrorDeleteUnsuccessful,
                        ApiResources.ApiErrorInvalidLinkedToEmployee);
            }
            
            if (result == -1)
            {
                throw new ApiException(ApiResources.ApiErrorDeleteUnsuccessful,
                        ApiResources.ApiErrorInvalidLinkedToSignoff);
            }
            
            if (result != 0)
            {
                throw new ApiException(ApiResources.ApiErrorDeleteUnsuccessful,
                    ApiResources.ApiErrorDeleteUnsuccessfulMessage);
            }

            return Get(id);
        }

        /// <summary>
        /// Archives / unarchives the item with the specified id.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="archive"></param>
        /// <returns></returns>
        public override CostCode Archive(int id, bool archive)
        {
            var item = Get(id);
            _data.ChangeStatus(id, archive);
            item.Archived = archive;
            return item;
        }


        private CostCode SaveCostCode(CostCode item)
        {
            // validate
            item.Validate(ActionContext);

            // save
            var id = _data.SaveCostcode(item.To(ActionContext));
            
            // throw here for when the description is in use.
            if (id == -2)
            {
                throw new ApiException(ApiResources.ApiErrorSaveUnsuccessful,
                        ApiResources.ApiErrorInvalidDescriptionAlreadyExists);
            }

            // no change on the label
            if (id == -1)
            {
                throw new ApiException(ApiResources.ApiErrorSaveUnsuccessful,
                        ApiResources.ApiErrorInvalidNameAlreadyExists);
            }

            if (id == 0)
            {
                throw new ApiException(ApiResources.ApiErrorRecordAlreadyExists,
                        ApiResources.ApiErrorRecordAlreadyExistsMessage);
            }

            return Get(id);
        }
    }
}