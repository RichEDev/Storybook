namespace SpendManagementLibrary.Interfaces.Expedite
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Enumerators.Expedite;
    using SpendManagementLibrary.Expedite;

    /// <summary>
    /// Defines the contract for managing expense validation. 
    /// This includes basic crud / data access methods for 
    /// <see cref="ExpenseValidationCriterion">Criteria</see> and
    /// <see cref="ExpenseValidationResult">Results</see>.
    /// </summary>
    public interface IManageExpenseValidation
    {
        #region Properties
        
        int AccountId { get; set; }
        
        #endregion Properties

        #region ExpenseItemManagement

        /// <summary>
        /// Gets a list of the Ids of all items that require validation.
        /// </summary>
        /// <returns>A list of Ids.</returns>
        List<ValidatableExpenseInfo> GetExpenseItemIdsRequiringValidation();

        /// <summary>
        /// Updates the ValidationProgress in the data store, if it is different to before.
        /// Returns the new one.
        /// </summary>
        /// <param name="expenseid">The Id of the Expense Item to update.</param>
        /// <param name="originalProgress">The initial progress of the expense item.</param>
        /// <param name="newProgress">The new progress of the expense item.</param>
        /// <returns>The new progress.</returns>
        ExpenseValidationProgress UpdateProgressForExpenseItem(int expenseid, ExpenseValidationProgress originalProgress, ExpenseValidationProgress newProgress);

        /// <summary>
        /// Updates the ValidationCount in the data store.
        /// </summary>
        /// <param name="expenseId">The Id of the Expense Item to update.</param>
        /// <param name="count">The new count.</param>
        void UpdateCountForExpenseItem(int expenseId, int count);

        #endregion ExpenseItemManagement

        #region Criteria Management

        /// <summary>
        /// Gets the entire list of criteria for validating expenses from the metabase.
        /// </summary>
        /// <returns>A list of criteria.</returns>
        List<ExpenseValidationCriterion> GetCriteria();

        /// <summary>
        /// Gets all criteria that are standard and any that are enabled and for the expense item's Subcat.
        /// </summary>
        /// <returns>A list of criteria.</returns>
        List<ExpenseValidationCriterion> GetCriteriaForSubcat(int subcatId);

        /// <summary>
        /// Gets a single criterion for validating expenses from the metabase.
        /// </summary>
        /// <param name="id">The Id of the criterion to get.</param>
        /// <returns>A single criterion or null.</returns>
        ExpenseValidationCriterion GetCriterion(int id);

        /// <summary>
        /// Gets the criteria which are converted from and to the validator
        /// notes on a subcat for validating expenses from the metabase.
        /// </summary>
        /// <param name="subcatId">The Id of the subcat.</param>
        /// <returns>A single criterion or null.</returns>
        List<ExpenseValidationCriterion> GetAllSubcatCriteria(int subcatId);

        /// <summary>
        /// Adds a new ExpenseValidationCriterion to the metabase.
        /// </summary>
        /// <param name="item">The item to add.</param>
        /// <returns>The added criterion.</returns>
        ExpenseValidationCriterion AddCriterion(ExpenseValidationCriterion item);

        /// <summary>
        /// Edits an existing ExpenseValidationCriterion in the metabase.
        /// </summary>
        /// <param name="item">The item to edit.</param>
        /// <returns>The modified criterion.</returns>
        ExpenseValidationCriterion EditCriterion(ExpenseValidationCriterion item);

        /// <summary>
        /// Deletes an existing ExpenseValidationCriterion from the metabase.
        /// </summary>
        /// <param name="id">The Id of the criterion to delete.</param>
        /// <exception cref="InvalidDataException">Throws if the item couldn't be found or is in use.</exception>
        void DeleteCriterion(int id);

        #endregion Criteria Management

        #region Result Management

        /// <summary>
        /// Gets all of the results of validation for an expense item, given its Id.
        /// </summary>
        /// <param name="expenseItemId">The ID of the ExpenseItem to get results for.</param>
        /// <returns>An ExpenseValidationResults object containing all results.</returns>
        IList<ExpenseValidationResult> GetResultsForExpenseItem(int expenseItemId);

        /// <summary>
        /// Gets a single ExpenseValidationResult for validating expenses from the metabase.
        /// </summary>
        /// <param name="id">The Id of the Result to get.</param>
        /// <returns>A single result or null.</returns>
        ExpenseValidationResult GetResult(int id);

        /// <summary>
        /// Adds a new ExpenseValidationResult to the metabase.
        /// </summary>
        /// <param name="item">The item to add.</param>
        /// <returns>The added Result.</returns>
        ExpenseValidationResult AddResult(ExpenseValidationResult item);

        /// <summary>
        /// Edits an existing ExpenseValidationResult in the customer db.
        /// </summary>
        /// <param name="item">The item to edit.</param>
        /// <returns>The modified Result.</returns>
        ExpenseValidationResult EditResult(ExpenseValidationResult item);

        /// <summary>
        /// Determines what the status should be from the lookup table, using the reason and the total.
        /// Note that the total 
        /// </summary>
        /// <param name="criterionId">The Id of the crierion the result is for.</param>
        /// <param name="reasonId">The Id of the reason - the result of receipt matching.</param>
        /// <param name="isForVAT">The whether to look for VAT, in which case the total will not be ignored.</param>
        /// <param name="total">The total of the Receipt - to pass when looking for VAT.</param>
        /// <returns>The modified Result.</returns>
        ExpenseValidationResultStatus DetermineStatusFromReason(int criterionId, int reasonId, bool isForVAT = false, decimal? total = null);
        
        /// <summary>
        /// Deletes an existing ExpenseValidationResult from the metabase.
        /// </summary>
        /// <param name="id">The Id of the Result to delete.</param>
        /// <exception cref="InvalidDataException">Throws if the item couldn't be found or is in use.</exception>
        void DeleteResult(int id);

        #endregion Result Management
    }
}
