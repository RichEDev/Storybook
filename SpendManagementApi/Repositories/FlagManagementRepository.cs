namespace SpendManagementApi.Repositories
{
    using System;
    using System.Collections.Generic;

    using SpendManagementApi.Interfaces;
    using SpendManagementApi.Models.Types;

    using Spend_Management;
    using Spend_Management.shared.code;
    using SpendManagementHelpers.TreeControl;
    //using Common.Enum;
    //using Common.Enums;
    using SpendManagementLibrary.Flags;
    //using Spend_Management.expenses.code;
    using Flag = SpendManagementApi.Models.Types.Flag;
    using Spend_Management.expenses.code;

    /// <summary>
    /// The flag management repository.
    /// </summary>
    internal class FlagManagementRepository : BaseRepository<Flag>, ISupportsActionContext
    {
        private readonly FlagManagement _flagManagement;

        /// <summary>
        /// Creates a new FlagManagementRepository.
        /// </summary>
        /// <param name="user">The Current User.</param>
        /// <param name="actionContext">The action context.</param>
        public FlagManagementRepository(ICurrentUser user, IActionContext actionContext)
            : base(user, x => x.FlagId, x => x.Description)
        {
            _flagManagement = this.ActionContext.FlagManagement;
        }

        public override IList<Flag> GetAll()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets a <see cref="Flag">Flag</see> by its Id.
        /// </summary>
        /// <param name="id">
        /// The Flag Id.
        /// </param>
        /// <returns>
        /// The <see cref="Flag">Flag</see>.
        /// </returns>
        public override Flag Get(int id)
        {
            var flag = _flagManagement.GetBy(id);
            return new Flag().From(flag, this.ActionContext);
        }

        /// <summary>
        /// Creates a flag
        /// </summary>
        /// <param name="flagRequest">
        /// The flag request.
        /// </param>
        /// <returns>
        /// The <see cref="int"> Id of the flag</see>
        /// </returns>
        public Flag CreateExpenseFlag(Flag flagRequest)
        {
            JavascriptTreeData treeData = null;

            var flagId = _flagManagement.Save(
                flagRequest.FlagId,
                (FlagType)flagRequest.FlagType,
                (FlagAction)flagRequest.FlagAction,
                flagRequest.CustomFlagText,
                flagRequest.InvalidDateFlagType,
                flagRequest.Date,
                flagRequest.Months,
                flagRequest.AmberTolerance,
                flagRequest.Frequency,
                (FlagFrequencyType)flagRequest.FlagFrequencyType,
                flagRequest.Period,
                (FlagPeriodType)flagRequest.FlagPeriodType,
                flagRequest.Limit,
                flagRequest.Description,
                flagRequest.Active,
                flagRequest.ClaimantJustificationRequired,
                flagRequest.DisplayImmediately,
                flagRequest.FlagTolerancePercentage,
                flagRequest.FinancialYear,
                flagRequest.TipLimit,
                (FlagColour)flagRequest.FlagLevel,
                flagRequest.ApproverJustificationRequired,
                flagRequest.IncreaseLimitByNumOthers,
                flagRequest.DisplayLimit,
                treeData,
                flagRequest.NotesForAuthoriser,
                (FlagInclusionType)flagRequest.ItemRoleInclusionType,
                (FlagInclusionType)flagRequest.ExpenseItemInclusionType,
                flagRequest.PassengerLimit,
                flagRequest.ValidateSelectedExpenseItem, 
                flagRequest.DailyMileageLimit);

            return Get(flagId);
        }

        /// <summary>
        /// ssociate item roles with flag.
        /// </summary>
        /// <param name="flagId">
        /// The flag id.
        /// </param>
        /// <param name="ItemRoles">
        /// The item roles.
        /// </param>
        /// <returns>
        /// The <see cref="int"/> outcome of the action
        /// </returns>
        public int AssociateItemRolesWithFlag(int flagId, List<int> ItemRoles)
        {
            return _flagManagement.SaveItemRoles(flagId, ItemRoles, User);
        }

        /// <summary>
        /// Associates expense items with flag.
        /// </summary>
        /// <param name="flagId">
        /// The flag id.
        /// </param>
        /// <param name="ExpenseItems">
        /// The expense items.
        /// </param>
        /// <returns>
        /// The <see cref="int"/> outcome of the action
        /// </returns>
        public int AssociateExpenseItemsWithFlag(int flagId, List<int> ExpenseItems)
        {
            return _flagManagement.SaveFlagRuleExpenseItems(flagId, ExpenseItems, User);
        }

        /// <summary>
        /// Associates fields with a flag
        /// </summary>
        /// <param name="flagId">
        /// The flag id.
        /// </param>
        /// <param name="FieldIds">
        /// The field ids.
        /// </param>
        public int AssociateFieldsWithFlag(int flagId, List<Guid> FieldIds)
        {
            _flagManagement.SaveFields(flagId, FieldIds, User);
            return 1;
        }

        /// <summary>
        /// The delete expense flag.
        /// </summary>
        /// <param name="flagId">
        /// The Id of the flag to delete
        /// </param>
        /// <returns>
        /// The <see cref="int"/> outcome of the action
        /// </returns>
        public int DeleteExpenseFlag(int flagId)
        {
            return _flagManagement.DeleteFlagRule(flagId, User);
        }
    }
}