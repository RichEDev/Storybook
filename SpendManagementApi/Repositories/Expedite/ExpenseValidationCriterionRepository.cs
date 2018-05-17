namespace SpendManagementApi.Repositories.Expedite
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Interfaces;

    using SpendManagementLibrary.Expedite;

    using Utilities;
    using SpendManagementLibrary.Interfaces.Expedite;
    using Spend_Management;

    using ExpenseValidationCriterion = SpendManagementApi.Models.Types.Expedite.ExpenseValidationCriterion;

    /// <summary>
    /// EnvelopeRepository manages data access for Envelopes.
    /// </summary>
    internal class ExpenseValidationCriterionRepository : BaseRepository<ExpenseValidationCriterion>, ISupportsActionContext
    {
        private readonly IManageExpenseValidation _data;

        /// <summary>
        /// Creates a new EnvelopeRepository with the passed in user.
        /// </summary>
        /// <param name="user">The current user.</param>
        /// <param name="actionContext">An implementation of ISupportsActionContext</param>
        public ExpenseValidationCriterionRepository(ICurrentUser user, IActionContext actionContext)
            : base(user, actionContext, x => x.Id, x => x.Requirements)
        {
            _data = ActionContext.ExpenseValidation;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpenseValidationCriterionRepository"/> class.
        /// </summary>
        public ExpenseValidationCriterionRepository()
        {
        }
        
        /// <summary>
        /// Gets all common Criteria.
        /// </summary>
        /// <returns>A list of all "common" criteria, i.e. not the customer specific ones.</returns>
        public override IList<ExpenseValidationCriterion> GetAll()
        {
         
            return this._data.GetCriteria().Where(c => c.AccountId == null).Select(c => new ExpenseValidationCriterion().From(c, null)).ToList();
        }

        /// <summary>
        /// The get expense validation criteria for account.
        /// </summary>
        /// <returns>
        /// A list of <see cref="ExpenseValidationCriterion"/>.
        /// </returns>
        public IList<ExpenseValidationCriterion> GetExpenseValidationCriteriaForAccount()
        {
            IManageExpenseValidation expenseValidation = new ExpenseValidationManager();

            return expenseValidation.GetCriteria().Where(c => c.AccountId == null).Select(c => new ExpenseValidationCriterion().From(c, null)).ToList();
        }

        /// <summary>
        /// Gets custom Criteria for a particular expense item.
        /// </summary>
        /// <param name="expenseItemId">
        /// The expense Item Id.
        /// </param>
        /// <param name="accountId">
        /// The account Id the expense belongs to.
        /// </param>
        /// <returns>
        /// A list of Criteria.
        /// </returns>
        public IList<ExpenseValidationCriterion> GetForExpenseItem(int expenseItemId, int accountId)
        {
            var expenseItem = new cClaims(accountId).getExpenseItemById(expenseItemId);
          
            if (expenseItem == null)
            {
                throw new InvalidDataException(ApiResources.ApiErrorValidationResultInvalidExpense);
            }

            IManageExpenseValidation expenseValidation = new ExpenseValidationManager(accountId);

            return expenseValidation.GetCriteriaForSubcat(expenseItem.subcatid).Select(c => new ExpenseValidationCriterion().From(c, null)).ToList();
        }

        /// <summary>
        /// Gets a Criterion by it's id.
        /// </summary>
        /// <param name="id">The Id of the Criterion.</param>
        /// <returns>Either an existing Criterion matching the Id or null.</returns>
        public override ExpenseValidationCriterion Get(int id)
        {
            return new ExpenseValidationCriterion().From(_data.GetCriterion(id), ActionContext);
        }

        /// <summary>
        /// Adds a new Criterion.
        /// </summary>
        /// <param name="item">The Criterion to add.</param>
        /// <returns>The newly added Criterion.</returns>
        public override ExpenseValidationCriterion Add(ExpenseValidationCriterion item)
        {
            _data.AccountId = item.AccountId ?? User.AccountID;
            item = base.Add(item);
            return new ExpenseValidationCriterion().From(_data.AddCriterion(item.To(ActionContext)), ActionContext);
        }

        /// <summary>
        /// Edits a Criterion
        /// </summary>
        /// <param name="item">The Criterion to edit.</param>
        /// <returns>The edited Criterion.</returns>
        public override ExpenseValidationCriterion Update(ExpenseValidationCriterion item)
        {
            _data.AccountId = item.AccountId ?? User.AccountID;
            item = base.Update(item);
            return new ExpenseValidationCriterion().From(_data.EditCriterion(item.To(ActionContext)), ActionContext);
        }

        /// <summary>
        /// Deletes a Criterion.
        /// </summary>
        /// <param name="id">The Id of the Criterion to delete.</param>
        /// <returns>Null upon a successful delete.</returns>
        public override ExpenseValidationCriterion Delete(int id)
        {
            base.Delete(id);
            _data.DeleteCriterion(id);
            return Get(id);
        }
    }
}