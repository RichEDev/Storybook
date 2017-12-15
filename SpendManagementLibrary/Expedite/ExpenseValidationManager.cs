namespace SpendManagementLibrary.Expedite
{
    using SpendManagementLibrary.Mobile;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.IO;
    using System.Linq;
    using Enumerators.Expedite;
    using Interfaces.Expedite;
    using Helpers;
    using BusinessLogic.Cache;
    using Common.Logging;

    /// <summary>
    /// Manages data access for validation.
    /// </summary>
    public class ExpenseValidationManager : IManageExpenseValidation
    {

        /// <summary>
        /// An instance of <see cref="ILog"/> for logging <see cref="ICache{T,TK}"/> diagnostics and information.
        /// </summary>
        private static readonly ILog Log = new LogFactory<ExpenseValidationManager>().GetLogger();

        #region SQL / Private Constants / Column Names / Param Keys / Vars

        // metabase criterion columns (and fields view join later on)
        private const string ColumnNameCriterionId = "CriterionId";
        private const string ColumnNameCriterionRequirements = "Requirements";
        private const string ColumnNameCriterionFieldId = "FieldId";
        private const string ColumnNameCriterionAccountId = "AccountId";
        private const string ColumnNameCriterionSubcatId = "SubcatId";
        private const string ColumnNameCriterionEnabled = "Enabled";
        private const string ColumnNameCriterionFraudulentIfFailsVAT = "FraudulentIfFailsVAT";
        private const string ColumnNameCriterionMessageFoundAndMatched = "FriendlyMessageFoundAndMatched";
        private const string ColumnNameCriterionMessageFoundNotMatched = "FriendlyMessageFoundNotMatched";
        private const string ColumnNameCriterionMessageFoundNotReadable = "FriendlyMessageFoundNotReadable";
        private const string ColumnNameCriterionMessageNotFound = "FriendlyMessageNotFound";

        // customer results columns
        private const string ColumnNameResultId = "ResultId";
        private const string ColumnNameResultExpenseId = "ExpenseId";
        private const string ColumnNameResultCriterionId = "CriterionId";
        private const string ColumnNameResultValidationStatusBusiness = "ValidationStatusBusiness";
        private const string ColumnNameResultValidationStatusVAT = "ValidationStatusVAT";
        private const string ColumnNameResultPossibleFraud = "PossiblyFraudulent";
        private const string ColumnNameResultValidationCompleted = "ValidationCompleted";
        private const string ColumnNameResultData = "Data";
        private const string ColumnNameResultComments = "Comments";
        private const string ColumnNameResultMatchingResult = "MatchingResult";

        // expense columns
        private const string ColumnNameExpenseId = "expenseid";
        private const string ColumnNameExpenseClaimId = "claimid";
        private const string ColumnNameClaimModifiedOn = "ModifiedOn";

        // sproc params
        private const string ParamId = "@id";
        private const string ParamFieldId = "@fieldId";
        private const string ParamExpenseId = "@expenseId";
        private const string ParamSubcatId = "@subcatId";
        private const string ParamAccountId = "@accountId";
        private const string ParamEnabled = "@enabled";
        private const string ParamCriterionId = "@criterionId";
        private const string ParamFraudulent = "@fraudulentIfFailsVAT";
        private const string ParamRequirements = "@requirements";
        private const string ParamValidationStatusBusiness = "@validationStatusBusiness";
        private const string ParamValidationStatusVAT = "@validationStatusVAT";
        private const string ParamValidationPossibleFraud = "@possibleFraud";
        private const string ParamValidationCompleted = "@validationCompleted";
        private const string ParamValidationCount = "@validationCount";
        private const string ParamComments = "@comments";
        private const string ParamValidationProgress = "@validationProgress";
        private const string ParamData = "@data";
        private const string ParamMatchingResult = "@matchingResult";
        private const string ParamReasonId = "@reasonId";
        private const string ParamIsVAT = "@isVAT";
        private const string ParamTotal = "@total";
        private const string ParamFoundAndMatched = "@friendlyMessageFoundAndMatched";
        private const string ParamFoundNotMatched = "@friendlyMessageFoundNotMatched";
        private const string ParamFoundNotReadable = "@friendlyMessageFoundNotReadable";
        private const string ParamNotFound = "@friendlyMessageNotFound";
        private const string ParamExpediteValidationProgress = "@expediteValidationProgress";

        // sproc names
        private const string StoredProcDeleteExpenseValidationCriterion = "DeleteExpenseValidationCriterion";
        private const string StoredProcSaveExpenseValidationCriterion = "SaveExpenseValidationCriterion";
        private const string StoredProcDeleteExpenseValidationResult = "DeleteExpenseValidationResult";
        private const string StoredProcSaveExpenseValidationResult = "SaveExpenseValidationResult";
        private const string StoredProcUpdateExpenseItemValidationProgress = "UpdateValidationProgressForExpenseItem";
        private const string StoredProcUpdateExpenseItemCount = "UpdateValidationCountForExpenseItem";
        private const string StoredProcDetermineExpenseValidationResult = "DetermineExpenseValidationResult";
        private const string StoredProcUpdateExpediteOperatorValidationProgress = "UpdateExpediteOperatorValidationProgress";
        /*
        Build the base SELECT SQL statements here using a compile time concat, which uses no overhead.
        This is here so that there are not loads of strings littered throughout the class, and a column or param can be changed centrally.
        */

        /// <summary>
        /// Gets all the expense items that need validation from the current account.
        /// </summary>
        private const string BaseGetValidatableExpenseItemIdsSql = "SELECT * FROM [dbo].[ExpensesRequiringReceiptValidation]";

        /// <summary>
        /// Gets all the criteria, which are from a view to the metabase.
        /// </summary>
        private const string BaseGetCriteriaSql = "SELECT " + ColumnNameCriterionId + ", " + ColumnNameCriterionRequirements + ", " + ColumnNameCriterionFieldId + ", " + ColumnNameCriterionAccountId +
                                                            ", " + ColumnNameCriterionSubcatId + ", " + ColumnNameCriterionFraudulentIfFailsVAT + ", " + ColumnNameCriterionEnabled +
                                                            ", " + ColumnNameCriterionMessageFoundAndMatched + ", " + ColumnNameCriterionMessageFoundNotMatched +
                                                            ", " + ColumnNameCriterionMessageFoundNotReadable + ", " + ColumnNameCriterionMessageNotFound +
                                                            " FROM [dbo].[ExpenseValidationCriteria] ";
        /// <summary>
        /// Fetches all the results, along with their criteria and reasons.
        /// </summary>
        private const string BaseGetResultsSql = "SELECT evr." + ColumnNameResultId + ", evr." + ColumnNameResultExpenseId + ", evr." + ColumnNameResultValidationStatusBusiness +
            ", evr." + ColumnNameResultValidationStatusVAT + ", evr." + ColumnNameResultPossibleFraud + ", evr." + ColumnNameResultValidationCompleted +
            ", evr." + ColumnNameResultComments + ", evr." + ColumnNameResultData + ", evr." + ColumnNameResultMatchingResult + ", cri." + ColumnNameCriterionId +
            ", cri." + ColumnNameCriterionRequirements + ", cri." + ColumnNameCriterionFraudulentIfFailsVAT + ", cri." + ColumnNameCriterionFieldId +
            ", cri." + ColumnNameCriterionAccountId + ", cri." + ColumnNameCriterionSubcatId + ", cri." + ColumnNameCriterionEnabled +
            ", cri." + ColumnNameCriterionFraudulentIfFailsVAT + ", cri." + ColumnNameCriterionMessageFoundAndMatched + ", cri." + ColumnNameCriterionMessageFoundNotMatched +
            ", cri." + ColumnNameCriterionMessageFoundNotReadable + ", cri." + ColumnNameCriterionMessageNotFound +
            " FROM [dbo].[ExpenseValidationResults] AS evr LEFT OUTER JOIN [dbo].[ExpenseValidationCriteria] AS cri ON cri." + ColumnNameCriterionId + " = evr." + ColumnNameResultCriterionId;

        private static readonly InvalidDataException FailedNotFoundError = new InvalidDataException("No item found with this Id.");
        private static readonly InvalidDataException OperationFailedUnknown = new InvalidDataException("Operation failed - reason unknown.");
        private static readonly InvalidDataException DeleteFailedDependencies = new InvalidDataException("Deleting the item failed as it is referenced by one or more dependents.");
        private static readonly InvalidDataException InvalidExpenseIdError = new InvalidDataException("The ExpenseItemId must be set to a valid ExpenseItem.");
        private static readonly InvalidDataException InvalidCriterionIdError = new InvalidDataException("The CriterionId must be set to a valid Criterion.");
        private static readonly InvalidDataException InvalidAccountIdError = new InvalidDataException("The AccountId must be set to a valid Account.");
        private static readonly InvalidDataException InvalidFieldIdError = new InvalidDataException("The FieldId must be set to a valid Field.");


        #endregion

        #region Fields

        /// <summary>
        /// The account Id which is used to access results.
        /// </summary>
        private int _accountId;

        /// <summary>
        /// The connection string which is built from the account Id.
        /// </summary>
        private string _connectionString;

        #endregion SQL / Private Constants / Column Names / Param Keys / Vars

        #region Constructor

        /// <summary>
        /// Creates a new ExpenseValidationManager, with the specified account id.
        /// </summary>
        /// <param name="accountId">A valid account id.</param>
        public ExpenseValidationManager(int accountId)
        {
            AccountId = accountId;

        }

        #endregion Constructor

        #region Public Properties

        /// <summary>
        /// Gets or sets the AccountId that this manager uses for database access.
        /// Setting it affects the connection string used internally.
        /// </summary>
        public int AccountId
        {
            get { return _accountId; }
            set
            {
                _accountId = value;
                _connectionString = cAccounts.getConnectionString(_accountId);
            }
        }

        #endregion Public Properties

        #region Public Methods

        #region ExpenseItem Management

        /// <summary>
        /// Gets the Expense Items that require validation.
        /// </summary>
        /// <returns></returns>
        public List<ValidatableExpenseInfo> GetExpenseItemIdsRequiringValidation()
        {
            return GetListOfIdsFromSql(BaseGetValidatableExpenseItemIdsSql);
        }

        /// <summary>
        /// Updates the ValidationProgrogress in the data store, if it is different to before.
        /// Returns the new one.
        /// </summary>
        /// <param name="expenseid">The Id of the Expense Item to validate.</param>
        /// <param name="originalProgress">The initial progress of the expense item.</param>
        /// <param name="newProgress">The new progress of the expense item.</param>
        /// <returns>The new progress.</returns>
        public ExpenseValidationProgress UpdateProgressForExpenseItem(int expenseid, ExpenseValidationProgress originalProgress, ExpenseValidationProgress newProgress)
        {
            if (originalProgress != newProgress)
            {
                var args = new List<KeyValuePair<string, object>>
                {
                    new KeyValuePair<string, object>(ParamExpenseId, expenseid),
                    new KeyValuePair<string, object>(ParamValidationProgress, (int)newProgress)
                };
                RunSingleStoredProcedure(StoredProcUpdateExpenseItemValidationProgress, args);
            }

            return newProgress;
        }


        /// <summary>
        /// Updates the Operator Validation Progress in the data base
        /// </summary>
        /// <param name="expenseid">The Id of the Expense Item to validate.</param>
        /// <param name="newProgress">The new progress of the expense item.</param>
        public int UpdateOperatorProgressForExpenseItem(int expenseid, ExpediteOperatorValidationProgress newProgress)
        {
           var args = new List<KeyValuePair<string, object>>
                {
                    new KeyValuePair<string, object>(ParamExpenseId, expenseid),
                    new KeyValuePair<string, object>(ParamExpediteValidationProgress, (int)newProgress)
                };
           var result = this.RunSingleStoredProcedure(StoredProcUpdateExpediteOperatorValidationProgress, args);
            if (Log.IsDebugEnabled)
            {
                Log.Debug($"Update Operator Progress For Expense Item {expenseid} to value {newProgress} returned a value of {result}");
            }

           return result;


        }

        /// <summary>
        /// update expedite validation progress flag to 0 where the flag is set to 1
        /// </summary>
        public void UpdateExpediteValidationProgressForBlockedExpenseItems()
        {
            int result = RunSingleStoredProcedure("UpdateExpediteValidationProgressForBlockedExpenses");
            if (result != 0 && result != 1)
            {
                throw new Exception("An error occurred");
            }
        }

        /// <summary>
        /// Updates the ValidationCount in the data store.
        /// </summary>
        /// <param name="expenseId">The Id of the Expense Item to update.</param>
        /// <param name="count">The new count.</param>
        public void UpdateCountForExpenseItem(int expenseId, int count)
        {
            var args = new List<KeyValuePair<string, object>>
                {
                    new KeyValuePair<string, object>(ParamExpenseId, expenseId),
                    new KeyValuePair<string, object>(ParamValidationCount, count)
                };
            RunSingleStoredProcedure(StoredProcUpdateExpenseItemCount, args);
        }

        #endregion ExpenseItem Management

        #region Criteria Management

        /// <summary>
        /// Gets the entire list of criteria for validating expenses from the metabase.
        /// </summary>
        /// <returns>A list of criteria.</returns>
        public List<ExpenseValidationCriterion> GetCriteria()
        {
            const string sql = BaseGetCriteriaSql + " WHERE " + ColumnNameCriterionSubcatId + " IS NULL";
            return ExtractValidationCriteriaFromSql(sql).ToList();
        }

        /// <summary>
        /// Gets only the custom criteria that are for the expense item's Subcat.
        /// </summary>
        /// <returns>A list of criteria.</returns>
        public List<ExpenseValidationCriterion> GetCriteriaForSubcat(int subcatId)
        {
            const string sql = BaseGetCriteriaSql + " WHERE " + ColumnNameCriterionAccountId + " = " + ParamAccountId + " AND " + ColumnNameCriterionSubcatId + " = " + ParamSubcatId;
            var args = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(ParamSubcatId, subcatId),
                new KeyValuePair<string, object>(ParamAccountId, _accountId)
            };
            return ExtractValidationCriteriaFromSql(sql, args).ToList();
        }


        /// <summary>
        /// Gets a single criterion for validating expenses from the metabase.
        /// </summary>
        /// <param name="id">The Id of the criterion to get.</param>
        /// <returns>A single criterion or null.</returns>
        public ExpenseValidationCriterion GetCriterion(int id)
        {
            const string sql = BaseGetCriteriaSql + " WHERE " + ColumnNameCriterionId + " = " + ParamId;
            var args = new List<KeyValuePair<string, object>> { new KeyValuePair<string, object>(ParamId, id) };
            return ExtractValidationCriteriaFromSql(sql, args).FirstOrDefault();
        }

        /// <summary>
        /// Gets all criteria that are standard and any that are enabled and for the expense item's Subcat.
        /// </summary>
        /// <param name="subcatId">The Id of the subcat.</param>
        /// <returns>A single criterion or null.</returns>
        public List<ExpenseValidationCriterion> GetAllSubcatCriteria(int subcatId)
        {
            // fetch from db if not in memory (updates the memory collection)
            const string sql = BaseGetCriteriaSql + " WHERE (" + ColumnNameCriterionSubcatId + " IS NULL) OR (" + ColumnNameCriterionAccountId + " = " + ParamAccountId + " AND " + ColumnNameCriterionSubcatId + " = " + ParamSubcatId + ")";
            var args = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(ParamSubcatId, subcatId),
                new KeyValuePair<string, object>(ParamAccountId, _accountId)
            };

            return ExtractValidationCriteriaFromSql(sql, args).ToList();
        }

        /// <summary>
        /// Adds a new ExpenseValidationCriterion to the metabase.
        /// </summary>
        /// <param name="item">The item to add.</param>
        /// <returns>The added criterion.</returns>
        public ExpenseValidationCriterion AddCriterion(ExpenseValidationCriterion item)
        {
            // convert
            var args = ConvertCriterionToSqlParams(item);

            // make the call, getting the result
            var result = RunSingleStoredProcedure(StoredProcSaveExpenseValidationCriterion, args, true);

            // handle errors
            switch (result)
            {
                case -3: throw InvalidFieldIdError;
                case -2: throw InvalidAccountIdError;
                case 0: throw OperationFailedUnknown;
            }

            // update cache and return
            return GetCriterion(result);
        }

        /// <summary>
        /// Edits an existing ExpenseValidationCriterion in the metabase.
        /// </summary>
        /// <param name="item">The item to edit.</param>
        /// <returns>The modified criterion.</returns>
        public ExpenseValidationCriterion EditCriterion(ExpenseValidationCriterion item)
        {
            // convert
            var args = ConvertCriterionToSqlParams(item);

            // make the call
            var result = RunSingleStoredProcedure(StoredProcSaveExpenseValidationCriterion, args, true);

            // handle errors
            switch (result)
            {
                case -3: throw InvalidFieldIdError;
                case -2: throw InvalidAccountIdError;
                case -1: throw FailedNotFoundError;
                case 0: throw OperationFailedUnknown;
            }

            // update cache and return
            return GetCriterion(item.Id);
        }

        /// <summary>
        /// Deletes an existing ExpenseValidationCriterion from the metabase.
        /// </summary>
        /// <param name="id">The Id of the criterion to delete.</param>
        /// <exception cref="InvalidDataException">Throws if the item couldn't be found or is in use.</exception>
        public void DeleteCriterion(int id)
        {
            // paramaterise the Id.
            var args = new List<KeyValuePair<string, object>> { new KeyValuePair<string, object>(ParamId, id) };

            // execute SP
            var result = RunSingleStoredProcedure(StoredProcDeleteExpenseValidationCriterion, args, true);

            // error trap
            switch (result)
            {
                case -1: throw FailedNotFoundError;
                case -2: throw DeleteFailedDependencies;
                case 0: return;
                default: throw OperationFailedUnknown;
            }
        }

        #endregion Criteria Management

        #region Result Management

        /// <summary>
        /// Gets all of the results of validation for an expense item, given its Id.
        /// </summary>
        /// <param name="expenseItemId">The ID of the ExpenseItem to get results for.</param>
        /// <returns>An ExpenseValidationResults object containing all results.</returns>
        public IList<ExpenseValidationResult> GetResultsForExpenseItem(int expenseItemId)
        {
            const string sql = BaseGetResultsSql + " WHERE evr." + ColumnNameResultExpenseId + " = " + ParamExpenseId;
            var args = new List<KeyValuePair<string, object>> { new KeyValuePair<string, object>(ParamExpenseId, expenseItemId) };
            return ExtractValidationResultsFromSql(sql, args).ToList();
        }

        /// <summary>
        /// Gets a single ExpenseValidationResult for validating expenses from the metabase.
        /// </summary>
        /// <param name="id">The Id of the Result to get.</param>
        /// <returns>A single result or null.</returns>
        public ExpenseValidationResult GetResult(int id)
        {
            const string sql = BaseGetResultsSql + " WHERE evr." + ColumnNameResultId + " = " + ParamId;
            var args = new List<KeyValuePair<string, object>> { new KeyValuePair<string, object>(ParamId, id) };
            return ExtractValidationResultsFromSql(sql, args).FirstOrDefault();
        }

        /// <summary>
        /// Adds a new ExpenseValidationResult to the metabase.
        /// </summary>
        /// <param name="item">The item to add.</param>
        /// <returns>The added Result.</returns>
        public ExpenseValidationResult AddResult(ExpenseValidationResult item)
        {
            // error trap
            if (item.ExpenseItemId == 0)
            {
                throw InvalidExpenseIdError;
            }

            // convert
            var args = ConvertResultToSqlParams(item);

            // make the call, getting the result
            item.Id = RunSingleStoredProcedure(StoredProcSaveExpenseValidationResult, args);

            // handle errors
            switch (item.Id)
            {
                case -3: throw InvalidCriterionIdError;
                case -2: throw InvalidExpenseIdError;
                case -1: throw FailedNotFoundError;
                case 0: throw OperationFailedUnknown;
            }

            return GetResult(item.Id);
        }

        /// <summary>
        /// Edits an existing ExpenseValidationResult in the customer db.
        /// </summary>
        /// <param name="item">The item to edit.</param>
        /// <returns>The modified Result.</returns>
        public ExpenseValidationResult EditResult(ExpenseValidationResult item)
        {
            // error trap
            if (item.ExpenseItemId == 0)
            {
                throw InvalidExpenseIdError;
            }

            // convert
            var args = ConvertResultToSqlParams(item);

            // make the call, getting the result
            var result = RunSingleStoredProcedure(StoredProcSaveExpenseValidationResult, args);

            // handle errors
            switch (result)
            {
                case -3: throw InvalidCriterionIdError;
                case -2: throw InvalidExpenseIdError;
                case -1: throw FailedNotFoundError;
                case 0: throw OperationFailedUnknown;
            }

            // get fresh
            return GetResult(result);
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
            if (isForVAT && !total.HasValue)
            {
                throw new ArgumentNullException("total", "You must populate the total property if you set isForVAT to true");
            }

            // set up args
            var args = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(ParamCriterionId, criterionId),
                new KeyValuePair<string, object>(ParamReasonId, reasonId),
                new KeyValuePair<string, object>(ParamIsVAT, isForVAT),
                new KeyValuePair<string, object>(ParamTotal, total),
            };

            // make the call, getting the result
            return (ExpenseValidationResultStatus)RunSingleStoredProcedure(StoredProcDetermineExpenseValidationResult, args, true);
        }

        /// <summary>
        /// Deletes an existing ExpenseValidationResult from the metabase.
        /// </summary>
        /// <param name="id">The Id of the Result to delete.</param>
        /// <exception cref="InvalidDataException">Throws if the item couldn't be found or is in use.</exception>
        public void DeleteResult(int id)
        {
            // set up args
            var args = new List<KeyValuePair<string, object>> { new KeyValuePair<string, object>(ParamId, id) };

            // make the call, getting the result
            var result = RunSingleStoredProcedure(StoredProcDeleteExpenseValidationResult, args);

            // error check
            if (result == -1)
            {
                throw FailedNotFoundError;
            }
        }

        /// <summary>
        /// Gets the Business, Vat, and Fraud validation results form a list of <see cref="ExpenseValidationResult"/> .
        /// </summary>
        /// <param name="initialResults">
        /// The initial validation results for the expense item.
        /// </param>
        /// <param name="statusIcons">
        /// The status icons.
        /// </param>
        /// <param name="invalidated">
        /// Whether the expense item has been invalidated.
        /// </param>
        /// <param name="statusDescriptions">
        /// The status descriptions.
        /// </param>
        /// <param name="fields">
        /// An instance of <see cref="cFields"/>.
        /// </param>
        /// <returns>
        /// The <see cref="ExpenseValidationResults"/>.
        /// </returns>
        public ExpenseValidationResults GetExpenseValidationResults(
            List<ExpenseValidationResult> initialResults,
            Dictionary<ExpenseValidationResultStatus, string> statusIcons,
            bool invalidated,
            Dictionary<ExpenseValidationResultStatus, string> statusDescriptions,
            cFields fields)
        {
            var businessResults = new List<SpendManagementLibrary.Expedite.ValidationResult>();
            var vatResults = new List<SpendManagementLibrary.Expedite.ValidationResult>();
            var fraudResults = new List<SpendManagementLibrary.Expedite.ValidationResult>();

            foreach (var result in initialResults)
            {
                var isCustom = result.Criterion.SubcatId.HasValue;

                var criterionMessage = "";
                switch (result.MatchingResult)
                {
                    case ExpenseValidationMatchingResult.FoundAndMatched:
                        criterionMessage = result.Criterion.FriendlyMessageFoundAndMatched;
                        break;
                    case ExpenseValidationMatchingResult.FoundNotMatched:
                        criterionMessage = result.Criterion.FriendlyMessageFoundNotMatched;
                        break;
                    case ExpenseValidationMatchingResult.FoundUnreadable:
                        criterionMessage = result.Criterion.FriendlyMessageFoundNotReadable;
                        break;
                    case ExpenseValidationMatchingResult.NotFound:
                        criterionMessage = result.Criterion.FriendlyMessageNotFound;
                        break;
                }

                // any custom rules should just have their requirements as the message.
                if (isCustom)
                {
                    criterionMessage = result.Criterion.Requirements;
                }

                businessResults.Add(
                    new SpendManagementLibrary.Expedite.ValidationResult
                    {
                        Status = result.BusinessStatus,
                        StatusIconUrl =
                                string.Format(
                                    statusIcons[result.BusinessStatus],
                                    invalidated ? "_invalid" : string.Empty),
                        StatusIconTooltip =
                                invalidated
                                    ? string.Format(
                                        "Invalidated (Previously {0})",
                                        statusDescriptions[result.BusinessStatus])
                                    : statusDescriptions[result.BusinessStatus],
                        Type = ExpediteValidationType.Business,
                        TypeIconUrl = isCustom ? ExpediteValidationHelper.CustomIcon : ExpediteValidationHelper.BusinessIcon,
                        TypeIconTooltip =
                                isCustom ? ExpediteValidationHelper.CustomTypeDescription : ExpediteValidationHelper.BusinessTypeDescription,
                        FriendlyMessage =
                                result.Criterion.FieldId.HasValue
                                    ? fields.GetFieldByID(result.Criterion.FieldId.Value)
                                          .FieldName + ": " + criterionMessage
                                    : criterionMessage,
                        Comments = result.Comments
                    });

                // add VAT / Fraud for non-custom criteria
                if (!isCustom)
                {
                    vatResults.Add(
                        new SpendManagementLibrary.Expedite.ValidationResult
                        {
                            Status = result.VATStatus,
                            StatusIconUrl =
                                    string.Format(
                                        statusIcons[result.VATStatus],
                                        invalidated ? "_invalid" : string.Empty),
                            StatusIconTooltip =
                                    invalidated
                                        ? string.Format(
                                            "Invalidated (Previously {0})",
                                            statusDescriptions[result.VATStatus])
                                        : statusDescriptions[result.VATStatus],
                            Type = ExpediteValidationType.VAT,
                            TypeIconUrl = ExpediteValidationHelper.VatIcon,
                            TypeIconTooltip = ExpediteValidationHelper.VatTypeDescription,
                            FriendlyMessage =
                                    result.Criterion.FieldId.HasValue
                                        ? fields.GetFieldByID(result.Criterion.FieldId.Value)
                                              .FieldName + ": " + criterionMessage
                                        : criterionMessage,
                            Comments = result.Comments
                        });

                    if (result.PossiblyFraudulent)
                    {
                        fraudResults.Add(
                            new SpendManagementLibrary.Expedite.ValidationResult
                            {
                                Status = ExpenseValidationResultStatus.Fail,
                                StatusIconUrl =
                                        string.Format(
                                            ExpediteValidationHelper.WarningIcon,
                                            invalidated ? "_invalid" : string.Empty),
                                StatusIconTooltip =
                                        invalidated
                                            ? string.Format(
                                                "Invalidated (Previously {0})",
                                                ExpediteValidationHelper.FraudTypeDescription)
                                            : ExpediteValidationHelper.FraudTypeDescription,
                                Type = ExpediteValidationType.Fraud,
                                TypeIconUrl = ExpediteValidationHelper.FraudIcon,
                                TypeIconTooltip = ExpediteValidationHelper.FraudTypeDescription,
                                FriendlyMessage =
                                        result.Criterion.FieldId.HasValue
                                            ? fields.GetFieldByID(
                                                result.Criterion.FieldId.Value).FieldName
                                              + ": " + criterionMessage
                                            : criterionMessage,
                                Comments = result.Comments
                            });
                    }
                }
            }

            var validationResults = new ExpenseValidationResults
            {
                BusinessResults = businessResults,
                VatResults = vatResults,
                FraudResults = fraudResults
            };

            return validationResults;
        }
    
    #endregion Result Management

    #endregion Public Methods

    #region Private Database Helpers

    #region Criteria

    /// <summary>
    /// Reads the database and builds ValidationCriteria for each unique row in the query result.
    /// </summary>
    /// <param name="sql">The SQL to execute.</param>
    /// <param name="addWithValueArgs">Any arguments to pass to the SQL.</param>
    /// <returns>A list of ValidationCriteron.</returns>
    private IEnumerable<ExpenseValidationCriterion> ExtractValidationCriteriaFromSql(string sql, List<KeyValuePair<string, object>> addWithValueArgs = null)
        {
            var results = new List<ExpenseValidationCriterion>();
            using (var connection = new DatabaseConnection(GlobalVariables.MetabaseConnectionString))
            {
                // add arguments to the connection
                if (addWithValueArgs != null)
                {
                    addWithValueArgs.ForEach(x => connection.AddWithValue(x.Key, x.Value));
                }

                // read the db
                using (IDataReader reader = connection.GetReader(sql))
                {
                    while (reader.Read())
                    {
                        results.Add(GetCriterionFromReader(reader));
                    }
                }

                connection.ClearParameters();
            }
            return results;
        }

        /// <summary>
        /// Gets a criterion from an IDataReader. Keeps things DRY.
        /// </summary>
        /// <param name="reader">An implementation of IDataReader.</param>
        /// <returns>An ExenseValidationCriterion.</returns>
        private ExpenseValidationCriterion GetCriterionFromReader(IDataReader reader)
        {
            return new ExpenseValidationCriterion
            {
                Id = reader.GetRequiredValue<int>(ColumnNameCriterionId),
                Requirements = reader.GetRequiredValue<string>(ColumnNameCriterionRequirements),
                FieldId = reader.GetNullable<Guid>(ColumnNameCriterionFieldId),
                AccountId = reader.GetNullable<int>(ColumnNameCriterionAccountId),
                SubcatId = reader.GetNullable<int>(ColumnNameCriterionSubcatId),
                Enabled = reader.GetRequiredValue<bool>(ColumnNameCriterionEnabled),
                FraudulentIfFailsVAT = reader.GetRequiredValue<bool>(ColumnNameCriterionFraudulentIfFailsVAT),
                FriendlyMessageFoundAndMatched = reader.GetRequiredValue<string>(ColumnNameCriterionMessageFoundAndMatched),
                FriendlyMessageFoundNotMatched = reader.GetRequiredValue<string>(ColumnNameCriterionMessageFoundNotMatched),
                FriendlyMessageFoundNotReadable = reader.GetRequiredValue<string>(ColumnNameCriterionMessageFoundNotReadable),
                FriendlyMessageNotFound = reader.GetRequiredValue<string>(ColumnNameCriterionMessageNotFound)

            };
        }

        /// <summary>
        /// Converts all the properties of a criterion to a list of KeyValuePairs, ready to be iterated upon and added as SQL parameters.
        /// </summary>
        /// <param name="criterion">The criterion to convert.</param>
        /// <returns>a list of arguments.</returns>
        private static List<KeyValuePair<string, object>> ConvertCriterionToSqlParams(ExpenseValidationCriterion criterion)
        {
            var args = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(ParamId, criterion.Id),
                new KeyValuePair<string, object>(ParamFieldId, criterion.FieldId),
                new KeyValuePair<string, object>(ParamRequirements, criterion.Requirements),
                new KeyValuePair<string, object>(ParamAccountId, criterion.AccountId),
                new KeyValuePair<string, object>(ParamSubcatId, criterion.SubcatId),
                new KeyValuePair<string, object>(ParamEnabled, criterion.Enabled),
                new KeyValuePair<string, object>(ParamFraudulent, criterion.FraudulentIfFailsVAT),
                new KeyValuePair<string, object>(ParamFoundAndMatched, criterion.FriendlyMessageFoundAndMatched),
                new KeyValuePair<string, object>(ParamFoundNotMatched, criterion.FriendlyMessageFoundNotMatched),
                new KeyValuePair<string, object>(ParamFoundNotReadable, criterion.FriendlyMessageFoundNotReadable),
                new KeyValuePair<string, object>(ParamNotFound, criterion.FriendlyMessageNotFound),
            };

            return args;
        }

        #endregion Criteria
    
    #region Results

    /// <summary>
    /// Reads the database and builds ValidationResults for each unique row in the query result.
    /// </summary>
    /// <param name="sql">The SQL to execute.</param>
    /// <param name="addWithValueArgs">Any arguments to pass to the SQL.</param>
    /// <returns>A list of ValidationResults.</returns>
    private IList<ExpenseValidationResult> ExtractValidationResultsFromSql(string sql, List<KeyValuePair<string, object>> addWithValueArgs = null)
        {
            var results = new List<ExpenseValidationResult>();
            using (var connection = new DatabaseConnection(_connectionString))
            {
                // add arguments to the connection
                if (addWithValueArgs != null)
                {
                    addWithValueArgs.ForEach(x => connection.AddWithValue(x.Key, x.Value));
                }

                // read the db
                using (IDataReader reader = connection.GetReader(sql))
                {
                    while (reader.Read())
                    {
                        var resultId = reader.GetRequiredValue<int>(ColumnNameResultId);
                        ExpenseValidationResult result = results.FirstOrDefault(r => r.Id == resultId);

                        // create if doesn't exist and set base properties.
                        if (result == null)
                        {
                            result = new ExpenseValidationResult
                            {
                                Id = resultId,
                                ExpenseItemId = reader.GetRequiredValue<int>(ColumnNameResultExpenseId),
                                BusinessStatus = reader.GetRequiredEnumValue<ExpenseValidationResultStatus>(ColumnNameResultValidationStatusBusiness),
                                VATStatus = reader.GetRequiredEnumValue<ExpenseValidationResultStatus>(ColumnNameResultValidationStatusVAT),
                                PossiblyFraudulent = reader.GetRequiredValue<bool>(ColumnNameResultPossibleFraud),
                                Timestamp = reader.GetRequiredValue<DateTime>(ColumnNameResultValidationCompleted),
                                Comments = reader.GetValueOrDefault(ColumnNameResultComments, (string)null),
                                Data = reader.GetValueOrDefault(ColumnNameResultData, (string)null),
                                MatchingResult = reader.GetRequiredEnumValue<ExpenseValidationMatchingResult>(ColumnNameResultMatchingResult),
                                Criterion = GetCriterionFromReader(reader)
                            };

                            results.Add(result);
                        }
                    }
                }

                connection.ClearParameters();
            }
            return results;
        }

        /// <summary>
        /// Converts all the properties of a result to a list of KeyValuePairs, ready to be iterated upon and added as SQL parameters.
        /// </summary>
        /// <param name="result">The result to convert.</param>
        /// <returns>a list of arguments.</returns>
        private static List<KeyValuePair<string, object>> ConvertResultToSqlParams(ExpenseValidationResult result)
        {
            var args = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(ParamId, result.Id),
                new KeyValuePair<string, object>(ParamExpenseId, result.ExpenseItemId),
                new KeyValuePair<string, object>(ParamCriterionId, result.Criterion.Id),
                new KeyValuePair<string, object>(ParamValidationStatusBusiness, result.BusinessStatus),
                new KeyValuePair<string, object>(ParamValidationStatusVAT, result.VATStatus),
                new KeyValuePair<string, object>(ParamValidationPossibleFraud, result.PossiblyFraudulent),
                new KeyValuePair<string, object>(ParamComments, result.Comments),
                new KeyValuePair<string, object>(ParamValidationCompleted, result.Timestamp),
                new KeyValuePair<string, object>(ParamData, result.Data),
                new KeyValuePair<string, object>(ParamMatchingResult, result.MatchingResult)
            };

            return args;
        }

        #endregion Results

        #region Misc

        /// <summary>
        /// Executes a stored procedure on a single item, returning the result.
        /// </summary>
        /// <param name="sql">The name of the stored procedure.</param>
        /// <param name="addWithValueArgs">The arguments to pass to the stored procedure.</param>
        /// <param name="isForMetabase">Whether to perform the call on the metabase rather than the constructed account id.</param>
        /// <returns>An integer result.</returns>
        private int RunSingleStoredProcedure(string sql, List<KeyValuePair<string, object>> addWithValueArgs = null, bool isForMetabase = false)
        {
            const string returnValueKey = "returnvalue";
            int result;

            using (var connection = new DatabaseConnection(isForMetabase ? GlobalVariables.MetabaseConnectionString : _connectionString))
            {
                if (addWithValueArgs != null)
                {
                    addWithValueArgs.ForEach(x => connection.AddWithValue(x.Key, x.Value));
                }

                connection.AddReturn(returnValueKey, SqlDbType.Int);
                connection.ExecuteProc(sql);
                result = connection.GetReturnValue<int>(returnValueKey);
                connection.ClearParameters();
            }

            return result;
        }

        /// <summary>
        /// Gets a list of Id values from the database.
        /// </summary>
        /// <param name="sql">The sql, which should select a list of integers.</param>
        /// <returns>A list of tuples.</returns>
        private List<ValidatableExpenseInfo> GetListOfIdsFromSql(string sql)
        {
            var results = new List<ValidatableExpenseInfo>();
            using (var connection = new DatabaseConnection(_connectionString))
            {
                // read the db
                using (IDataReader reader = connection.GetReader(sql))
                {
                    while (reader.Read())
                    {
                        var claimId = reader.GetRequiredValue<int>(ColumnNameExpenseClaimId);
                        var expenseId = reader.GetRequiredValue<int>(ColumnNameExpenseId);
                        var modifiedOn = reader.GetNullable<DateTime>(ColumnNameClaimModifiedOn);
                        results.Add(new ValidatableExpenseInfo
                        {
                            AccountId = AccountId,
                            ClaimId = claimId,
                            ExpenseId = expenseId,
                            ModifiedOn = modifiedOn
                        });
                    }
                }
            }

            return results;
        }

        #endregion Misc

        #endregion Private Database Helpers
    }
}