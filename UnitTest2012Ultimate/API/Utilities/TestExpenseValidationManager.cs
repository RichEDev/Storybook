namespace UnitTest2012Ultimate.API.Utilities
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using SpendManagementLibrary.Expedite;
    using SpendManagementLibrary.Interfaces.Expedite;
    using SpendManagementLibrary.Enumerators.Expedite;

    public class TestExpenseValidationManager : IManageExpenseValidation
    {
        #region Statics

        public static readonly string TestFieldLabel = "Test Field Label";
        public static readonly Guid TestFieldId = new Guid("F4090E87-B484-46D3-A0E3-4C79AA6AA1C2");
        public static readonly int TestAccountId = GlobalTestVariables.AccountId;
        public static readonly int ExpenseItemIdA = 1213100;
        public static readonly int ExpenseItemIdB = 1213101;

        public static IList<ExpenseValidationCriterion> Criteria;
        public static IList<ExpenseValidationResult> Results;

        public static readonly InvalidDataException FailedNotFoundError = new InvalidDataException("No item found with this Id.");
        public static readonly InvalidDataException OperationFailedUnknown = new InvalidDataException("Operation failed - reason unknown.");
        public static readonly InvalidDataException DeleteFailedDependencies = new InvalidDataException("Deleting the item failed as it is referenced by one or more dependents.");
        public static readonly InvalidDataException InvalidExpenseIdError = new InvalidDataException("The ExpenseItemId must be set to a valid ExpenseItem.");
        public static readonly InvalidDataException InvalidCriterionIdError = new InvalidDataException("The CriterionId must be set to a valid Criterion.");
        public static readonly InvalidDataException InvalidAccountIdError = new InvalidDataException("The AccountId must be set to a valid Account.");
        public static readonly InvalidDataException InvalidFieldIdError = new InvalidDataException("The FieldId must be set to a valid Field.");

        #endregion Statics

        #region Constructor & Data

        /// <summary>
        /// Creates a new Test ExpenseValidationManager
        /// </summary>
        /// <param name="accountId"></param>
        public TestExpenseValidationManager(int accountId)
        {
            ResetData(accountId);
        }

        /// <summary>
        /// Resets the data in this Test Class.
        /// </summary>
        public void ResetData(int accountId)
        {
            Criteria = new List<ExpenseValidationCriterion>
            {
                // first criterion not linked to any result so can test delete.
                new ExpenseValidationCriterion
                {
                    Id = 1,
                    AccountId = AccountId,
                    Requirements = "This is Criterion 1",
                    FieldId = null,
                    Enabled = true
                },
                new ExpenseValidationCriterion
                {
                    Id = 2,
                    AccountId = AccountId,
                    Requirements = "This is Criterion 2",
                    FieldId = null,
                    Enabled = true
                },
                new ExpenseValidationCriterion
                {
                    Id = 3,
                    AccountId = AccountId,
                    Requirements = "This is Criterion 3",
                    FieldId = null,
                    Enabled = false
                },
                new ExpenseValidationCriterion
                {
                    Id = 4,
                    AccountId = AccountId,
                    Requirements = "This is Criterion 4",
                    FieldId = null,
                    Enabled = true
                },
                new ExpenseValidationCriterion
                {
                    Id = 5,
                    AccountId = AccountId,
                    Requirements = "This is Criterion 5",
                    FieldId = TestFieldId,
                    Enabled = true
                }
            };

            Results = new List<ExpenseValidationResult>
            {
                new ExpenseValidationResult
                {
                    Id = 1,
                    ExpenseItemId = ExpenseItemIdA,
                    BusinessStatus = ExpenseValidationResultStatus.Pass,
                    VATStatus = ExpenseValidationResultStatus.Pass,
                    Criterion = Criteria[1],
                    PossiblyFraudulent = false,
                    Comments = "Comments for Result 1",
                    Timestamp = DateTime.UtcNow
                },
                new ExpenseValidationResult
                {
                    Id = 2,
                    ExpenseItemId = ExpenseItemIdA,
                    BusinessStatus = ExpenseValidationResultStatus.Pass,
                    VATStatus = ExpenseValidationResultStatus.Pass,
                    Criterion = Criteria[1],
                    PossiblyFraudulent = false,
                    Comments = "Comments for Result 2",
                    Timestamp = DateTime.UtcNow
                },
                new ExpenseValidationResult
                {
                    Id = 3,
                    ExpenseItemId = ExpenseItemIdA,
                    BusinessStatus = ExpenseValidationResultStatus.Pass,
                    VATStatus = ExpenseValidationResultStatus.Pass,
                    Criterion = Criteria[2],
                    PossiblyFraudulent = false,
                    Comments = "Comments for Result 3",
                    Timestamp = DateTime.UtcNow
                },
                new ExpenseValidationResult
                {
                    Id = 4,
                    ExpenseItemId = ExpenseItemIdB,
                    BusinessStatus = ExpenseValidationResultStatus.Pass,
                    VATStatus = ExpenseValidationResultStatus.Pass,
                    Criterion = Criteria[3],
                    PossiblyFraudulent = false,
                    Comments = "Comments for Result 4",
                    Timestamp = DateTime.UtcNow
                },
                new ExpenseValidationResult
                {
                    Id = 5,
                    ExpenseItemId = ExpenseItemIdB,
                    BusinessStatus = ExpenseValidationResultStatus.Pass,
                    VATStatus = ExpenseValidationResultStatus.Pass,
                    Criterion = Criteria[4],
                    PossiblyFraudulent = false,
                    Comments = "Comments for Result 5",
                    Timestamp = DateTime.UtcNow
                },
                new ExpenseValidationResult
                {
                    Id = 6,
                    ExpenseItemId = ExpenseItemIdB,
                    BusinessStatus = ExpenseValidationResultStatus.Pass,
                    VATStatus = ExpenseValidationResultStatus.Pass,
                    Criterion = Criteria[1],
                    PossiblyFraudulent = false,
                    Comments = "Comments for Result 6",
                    Timestamp = DateTime.UtcNow
                }
            };
        }

        #endregion Constructor & Data

        #region Public Properties

        private int _accountId;
        public int AccountId
        {
            get { return _accountId; }
            set
            {
                _accountId = value;
                ResetData(_accountId);
            }
        }

        #endregion Public Properties

        #region Test Methods

        /// <summary>
        /// Gets a list of the Ids of all items that require validation.
        /// </summary>
        /// <returns>A list of Ids.</returns>
        public List<ValidatableExpenseInfo> GetExpenseItemIdsRequiringValidation()
        {
            return new List<ValidatableExpenseInfo>
            {
                new ValidatableExpenseInfo{AccountId = 35, ClaimId = 12356, ExpenseId = ExpenseItemIdA, ModifiedOn = DateTime.UtcNow}
            };
        }

        /// <summary>
        /// Updates the ValidationProgrogress in the data store, if it is different to before.
        /// Returns the new one.
        /// </summary>
        /// <param name="expenseid">The Id of the Expense Item to validate.</param>
        /// <param name="originalProgress">The initial progress of the expense item.</param>
        /// <param name="newProgress">The new progress of the expense item.</param>
        /// <returns>The new progress.</returns>
        public ExpenseValidationProgress UpdateProgressForExpenseItem(int expenseid, ExpenseValidationProgress originalProgress,
            ExpenseValidationProgress newProgress)
        {
            return newProgress;
        }

        /// <summary>
        /// Updates the ValidationCount in the data store.
        /// </summary>
        /// <param name="expenseId">The Id of the Expense Item to update.</param>
        /// <param name="count">The new count.</param>
        public void UpdateCountForExpenseItem(int expenseId, int count)
        {
            
        }

        /// <summary>
        /// Gets the entire list of criteria for validating expenses from the metabase.
        /// </summary>
        /// <returns>A list of criteria.</returns>
        public List<ExpenseValidationCriterion> GetCriteria()
        {
            return Criteria.ToList();
        }

        /// <summary>
        /// Gets all criteria that are standard and any that are enabled and for the expense item's Subcat.
        /// </summary>
        /// <returns>A list of criteria.</returns>
        public List<ExpenseValidationCriterion> GetCriteriaForSubcat(int subcatId)
        {
            return Criteria.Where(c => c.SubcatId.HasValue).ToList();
        }

        /// <summary>
        /// Gets all criteria that are standard and any that are enabled and for the expense item's Subcat.
        /// </summary>
        /// <returns>A list of criteria.</returns>
        public List<ExpenseValidationCriterion> GetEnabledCriteriaIncludingCustomSubcat(int subcatId)
        {
            return Criteria.Where(c => c.Enabled && (c.SubcatId == subcatId && c.AccountId == _accountId)).ToList();
        }

        /// <summary>
        /// Gets a single criterion for validating expenses from the metabase.
        /// </summary>
        /// <param name="id">The Id of the criterion to get.</param>
        /// <returns>A single criterion or null.</returns>
        public ExpenseValidationCriterion GetCriterion(int id)
        {
            return Criteria.FirstOrDefault(x => x.Id == id);
        }

        /// <summary>
        /// Gets the criteria which are converted from and to the validator
        /// notes on a subcat for validating expenses from the metabase.
        /// </summary>
        /// <param name="subcatId">The Id of the subcat.</param>
        /// <returns>A single criterion or null.</returns>
        public List<ExpenseValidationCriterion> GetAllSubcatCriteria(int subcatId)
        {
            return Criteria.Where(x => x.AccountId == _accountId && x.SubcatId == subcatId).ToList();
        }

        /// <summary>
        /// Adds a new ExpenseValidationCriterion to the metabase.
        /// </summary>
        /// <param name="item">The item to add.</param>
        /// <returns>The added criterion.</returns>
        public ExpenseValidationCriterion AddCriterion(ExpenseValidationCriterion item)
        {
            ValidateField(item.FieldId);
            ValidateAccount(item.AccountId);

            item.Id = Criteria.Count;
            Criteria.Add(item);
            return item;
        }

        /// <summary>
        /// Edits an existing ExpenseValidationCriterion in the metabase.
        /// </summary>
        /// <param name="item">The item to edit.</param>
        /// <returns>The modified criterion.</returns>
        public ExpenseValidationCriterion EditCriterion(ExpenseValidationCriterion item)
        {
            ValidateField(item.FieldId);
            ValidateAccount(item.AccountId);

            var existing = TryGetCriterionAndThrow(item.Id);
            Criteria[Criteria.IndexOf(existing)] = item;
            return item;
        }

        /// <summary>
        /// Deletes an existing ExpenseValidationCriterion from the metabase.
        /// </summary>
        /// <param name="id">The Id of the criterion to delete.</param>
        /// <exception cref="System.IO.InvalidDataException">Throws if the item couldn't be found or is in use.</exception>
        public void DeleteCriterion(int id)
        {
            var existing = TryGetCriterionAndThrow(id);
            if (Results.Any(r => r.Criterion.Id == id))
            {
                throw DeleteFailedDependencies;
            }
            Criteria.Remove(existing);
        }

        /// <summary>
        /// Gets all of the results of validation for an expense item, given its Id.
        /// </summary>
        /// <param name="expenseItemId">The ID of the ExpenseItem to get results for.</param>
        /// <returns>An ExpenseValidationResults object containing all results.</returns>
        public IList<ExpenseValidationResult> GetResultsForExpenseItem(int expenseItemId)
        {
            ValidateExpense(expenseItemId);
            return Results.Where(x => x.ExpenseItemId == expenseItemId).ToList();
        }

        /// <summary>
        /// Gets a single ExpenseValidationResult for validating expenses from the metabase.
        /// </summary>
        /// <param name="id">The Id of the Result to get.</param>
        /// <returns>A single result or null.</returns>
        public ExpenseValidationResult GetResult(int id)
        {
            return Results.FirstOrDefault(x => x.Id == id);
        }

        /// <summary>
        /// Adds a new ExpenseValidationResult to the metabase.
        /// </summary>
        /// <param name="item">The item to add.</param>
        /// <returns>The added Result.</returns>
        public ExpenseValidationResult AddResult(ExpenseValidationResult item)
        {
            ValidateExpense(item.ExpenseItemId);
            TryGetCriterionAndThrow(item.Criterion.Id);
            item.Id = Results.Count;
            Results.Add(item);
            return item;
        }

        /// <summary>
        /// Edits an existing ExpenseValidationResult in the customer db.
        /// </summary>
        /// <param name="item">The item to edit.</param>
        /// <returns>The modified Result.</returns>
        public ExpenseValidationResult EditResult(ExpenseValidationResult item)
        {
            var existing = TryGetResultAndThrow(item.Id);
            ValidateExpense(item.ExpenseItemId);
            TryGetCriterionAndThrow(item.Criterion.Id);
            Results[Results.IndexOf(existing)] = item;
            return item;
        }

        /// <summary>
        /// Determines what the status should be from the lookup table, using the reason and the total.
        /// Note that the total 
        /// </summary>
        /// <param name="criterionId">The Id of the crierion the result is for.</param>
        /// <param name="reasonId">The Id of the reason - the result of receipt matching.</param>
        /// <param name="isForVAT">The whether to look for VAT, in which case the total will not be ignored.</param>
        /// <param name="total">The total of the Receipt - to pass when looking for VAT.</param>
        /// <returns>The modified Result.</returns>
        public ExpenseValidationResultStatus DetermineStatusFromReason(int criterionId, int reasonId, bool isForVAT = false, decimal? total = null)
        {
            return ExpenseValidationResultStatus.Pass;
        }

        /// <summary>
        /// Determines what the status should be from the lookup table, using the reason and the total.
        /// </summary>
        /// <param name="criterionId">The Id of the crierion the result is for.</param>
        /// <param name="reasonId">The Id of the reason - the result of receipt matching.</param>
        /// <param name="total">The total of the Receipt.</param>
        /// <returns>The modified Result.</returns>
        public ExpenseValidationResultStatus DetermineStatusFromReason(int criterionId, int reasonId, decimal total)
        {
            return ExpenseValidationResultStatus.Pass;
        }
        
        /// <summary>
        /// Deletes an existing ExpenseValidationResult from the metabase.
        /// </summary>
        /// <param name="id">The Id of the Result to delete.</param>
        /// <exception cref="System.IO.InvalidDataException">Throws if the item couldn't be found or is in use.</exception>
        public void DeleteResult(int id)
        {
            var existing = TryGetResultAndThrow(id);
            Results.Remove(existing);
        }

        #endregion Test Methods

        #region Utilities

        /// <summary>
        /// Attempts to get a Criterion by Id and throws if it doesn't exist.
        /// </summary>
        /// <param name="id">The Id of the Criterion.</param>
        /// <returns>The Criterion, or throws an error.</returns>
        /// <exception cref="InvalidDataException">Throws if the item couldn't be found.</exception>
        private ExpenseValidationCriterion TryGetCriterionAndThrow(int id)
        {
            var existing = Criteria.FirstOrDefault(x => x.Id == id);
            if (existing == null)
            {
                throw FailedNotFoundError;
            }

            return existing;
        }

        /// <summary>
        /// Attempts to get a Result by Id and throws if it doesn't exist.
        /// </summary>
        /// <param name="id">The Id of the Result.</param>
        /// <returns>The Result, or throws an error.</returns>
        /// <exception cref="InvalidDataException">Throws if the item couldn't be found.</exception>
        private ExpenseValidationResult TryGetResultAndThrow(int id)
        {
            var existing = Results.FirstOrDefault(x => x.Id == id);
            if (existing == null)
            {
                throw FailedNotFoundError;
            }

            return existing;
        }

        /// <summary>
        /// Checks whether the expense item id is one of either <see cref="ExpenseItemIdA"/> or <see cref="ExpenseItemIdB"/>.
        /// Throws an error if not.
        /// </summary>
        /// <param name="id">The Id of the ExpenseItem.</param>
        /// <exception cref="InvalidDataException">Throws if the Id isn't one of the two.</exception>
        private void ValidateExpense(int id)
        {
            if (id != ExpenseItemIdA && id != ExpenseItemIdB)
            {
                throw InvalidExpenseIdError;
            }
        }

        /// <summary>
        /// Checks whether the account id is the same as <see cref="TestAccountId"/>.
        /// Throws an error if not.
        /// </summary>
        /// <param name="id">The Id of the Account.</param>
        /// <exception cref="InvalidDataException">Throws if the Id isn't the same.</exception>
        private void ValidateAccount(int? id)
        {
            if (id.HasValue && id != TestAccountId)
            {
                throw InvalidAccountIdError;
            }
        }

        /// <summary>
        /// Checks whether the Field id is the same as <see cref="TestFieldId"/>.
        /// Throws an error if not.
        /// </summary>
        /// <param name="guid">The Field.</param>
        /// <exception cref="InvalidDataException">Throws if the Id isn't the same.</exception>
        private void ValidateField(Guid? guid)
        {
            if (guid.HasValue && guid != TestFieldId)
            {
                throw InvalidFieldIdError;
            }
        }
        
        #endregion Utilities
    }
}