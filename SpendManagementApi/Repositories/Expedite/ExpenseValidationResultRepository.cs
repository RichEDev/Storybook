namespace SpendManagementApi.Repositories.Expedite
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Interfaces;
    using Models.Requests.Expedite;
    using Models.Types.Expedite;
    using Utilities;
    using SpendManagementLibrary;
    using SpendManagementLibrary.Interfaces.Expedite;
    using Spend_Management;

    using DALEnum = SpendManagementLibrary.Enumerators.Expedite;
    using DAL = SpendManagementLibrary.Expedite;
    using System.Xml;

    using SpendManagementApi.Models.Responses.Expedite;

    /// <summary>
    /// EnvelopeRepository manages data access for Envelopes.
    /// </summary>
    internal class ExpenseValidationResultRepository : BaseRepository<ExpenseValidationResult>, ISupportsActionContext
    {
        private readonly IManageExpenseValidation _data;

        /// <summary>
        /// Creates a new EnvelopeRepository with the passed in user.
        /// </summary>
        /// <param name="user">The current user.</param>
        /// <param name="actionContext">An implementation of ISupportsActionContext</param>
        public ExpenseValidationResultRepository(ICurrentUser user, IActionContext actionContext)
            : base(user, actionContext, x => x.Id, null)
        {
            _data = ActionContext.ExpenseValidation;
        }

        #region Inapplicable implementations

        /// <summary>
        /// Do not use.
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public override IList<ExpenseValidationResult> GetAll()
        {
            throw new NotImplementedException(ApiResources.ApiErrorValidationResultGetAll);
        }

        /// <summary>
        /// Do not use.
        /// </summary>
        /// <param name="id">The Id of the Result.</param>
        /// <exception cref="NotImplementedException"></exception>
        public override ExpenseValidationResult Get(int id)
        {
            throw new NotImplementedException(ApiResources.ApiErrorValidationResultGetById);
        }

        #endregion Inapplicable implementations

        #region Public Methods

        /// <summary>
        /// Gets all Results.
        /// </summary>
        /// <returns>A list of all Results.</returns>
        public List<ExpenseValidationResult> GetAllForExpenseItem(int expenseId)
        {
            var expenseItem = TryGetExpenseItem(expenseId);
            var results = _data.GetResultsForExpenseItem(expenseId)
                                .Select(c => new ExpenseValidationResult().From(c, ActionContext))
                                .ToList();
            results.ForEach(r => r.ExpenseSubCategoryId = expenseItem.subcatid);
            return results;
        }

        /// <summary>
        /// Gets a Result by it's id.
        /// </summary>
        /// <param name="id">The Id of the Result.</param>
        /// <param name="expenseItem">Pass in an out reference to populate with the expense item also</param>
        /// <returns>Either an existing Result matching the Id or null.</returns>
        public ExpenseValidationResult Get(int id, out cExpenseItem expenseItem)
        {
            // this will fetch from the cache if the expenseId exists.
            var result = new ExpenseValidationResult().From(_data.GetResult(id), ActionContext);
            expenseItem = null;

            if (result != null)
            {
                expenseItem = TryGetExpenseItem(result.ExpenseItemId);

                if (expenseItem != null)
                {
                    result.ExpenseSubCategoryId = expenseItem.subcatid;
                }
            }

            return result;
        }

        /// <summary>
        /// Adds and converts all posted results to actual ExpenseValidationResults.
        /// </summary>
        /// <param name="request">The requset to validate an entire item at once.</param>
        public void AddResultsForExpenseItem(ExpenseItemValidationResults request)
        {
            // ensure the expense exists
            var expenseItem = TryGetExpenseItem(request.ExpenseItemId);

            // validate progress
            ValidateExpenseValidationProgress(expenseItem);

            // get all the criteria for this item
            var criteria = _data.GetAllSubcatCriteria(expenseItem.subcatid);

            // throw if there are not enough results to cover the criteria
            if (request.Results.Count != criteria.Count)
            {
                throw new InvalidDataException(ApiResources.ApiErrorValidationNotEnoughResults);
            }

            var validationManager = new DAL.ExpenseValidationManager(User.AccountID);

            request.Results.ForEach(r =>
            {
                var criterion = criteria.FirstOrDefault(c => c.Id == r.CriterionId);

                // make sure it exists
                if (criterion == null)
                {
                    throw new InvalidDataException(ApiResources.ApiErrorValidationCriterionDoesntExist);
                }

                // make sure it's enabled
                if (!criterion.Enabled)
                {
                    throw new InvalidDataException(ApiResources.ApiErrorValidationCriterionDisabled);
                }

                // check the criteria doesn't already have a result for this item
         
                expenseItem.ValidationResults = validationManager.GetResultsForExpenseItem(expenseItem.expenseid).ToList();
                var existingValidationResult = expenseItem.ValidationResults.SingleOrDefault(er => er.Criterion.Id == criterion.Id);
                if (existingValidationResult != null)
                {
                    // delete it.
                    _data.DeleteResult(existingValidationResult.Id);
                }

                var result = new DAL.ExpenseValidationResult
                {
                    Criterion = criterion,
                    Comments = r.Comments,
                    Data = r.Data,
                    ExpenseItemId = request.ExpenseItemId,
                    Timestamp = DateTime.UtcNow,
                    MatchingResult = (DALEnum.ExpenseValidationMatchingResult)r.Reason
                };

                // update both the Business and VAT status
                if (criterion.SubcatId.HasValue)
                {
                    result.BusinessStatus = r.Reason == ExpenseValidationMatchingResult.FoundAndMatched
                        ? DALEnum.ExpenseValidationResultStatus.Pass
                        : DALEnum.ExpenseValidationResultStatus.Fail;
                    result.VATStatus = DALEnum.ExpenseValidationResultStatus.NotApplicable;
                }
                else
                {
                    result.BusinessStatus = _data.DetermineStatusFromReason(criterion.Id, (int)r.Reason);
                    result.VATStatus = _data.DetermineStatusFromReason(criterion.Id, (int)r.Reason, true, request.Total);

                    //Override the existing validation result from "Fail" to "Pass"
                    //Customers with "Allow receipts for a higher value than the expense claimed to pass validation" configured 
                    //General Option->Expedite->Validation Options

                    var clsSubAccounts = new cAccountSubAccounts(ActionContext.AccountId);
                    cAccountProperties reqProperties = clsSubAccounts.getFirstSubAccount().SubAccountProperties.Clone();

                    if (reqProperties.AllowReceiptTotalToPassValidation && ((((DALEnum.CriterionIdentifiers)criterion.Id == DALEnum.CriterionIdentifiers.AmountIncVat) || ((DALEnum.CriterionIdentifiers)criterion.Id == DALEnum.CriterionIdentifiers.InvoiceTotal)) && r.Reason == ExpenseValidationMatchingResult.FoundNotMatched))
                    {
                            XmlDocument xmlDoc = new XmlDocument();
                            xmlDoc.LoadXml(r.Data);
                            string currencyValue = xmlDoc.SelectSingleNode("/MatchingResultData/CurrencyValue").InnerText;

                            //Check if the receipt amount is more than the amount claimed
                            if (currencyValue != null && Convert.ToDecimal(currencyValue) > expenseItem.total)
                            {
                                result.VATStatus = result.VATStatus == DALEnum.ExpenseValidationResultStatus.Fail
                                    ? DALEnum.ExpenseValidationResultStatus.Pass
                                    : result.VATStatus;
                            result.BusinessStatus = result.BusinessStatus == DALEnum.ExpenseValidationResultStatus.Fail
                                    ? DALEnum.ExpenseValidationResultStatus.Pass
                                    : result.BusinessStatus;
                            }
                        }
                    }

                // set the fraud marker
                result.PossiblyFraudulent = criterion.FraudulentIfFailsVAT && result.VATStatus == DALEnum.ExpenseValidationResultStatus.Fail || criterion.FraudulentIfFailsVAT && result.VATStatus == DALEnum.ExpenseValidationResultStatus.Fail;

                // commit the result to the database, getting back the modified db object
                _data.AddResult(result);
            });

            // update the validation count
            ActionContext.ExpenseValidation.UpdateCountForExpenseItem(expenseItem.expenseid, ++expenseItem.ValidationCount);

            // re-fetch the item, as now the results have changed.
            expenseItem = TryGetExpenseItem(expenseItem.expenseid);

            // determine progress
            expenseItem.DetermineValidationProgress(User.Account, true, true, ActionContext.ExpenseValidation);

            // get the claims and claim
            var claims = new cClaims(ActionContext.AccountId);
            var claim = claims.getClaimById(expenseItem.claimid);


            expenseItem.ValidationResults = validationManager.GetResultsForExpenseItem(expenseItem.expenseid).ToList();
            // If any results fail for business reasons, return the expense and update the history.
            if (expenseItem.ValidationResults.Any(r => r.BusinessStatus == DALEnum.ExpenseValidationResultStatus.Fail))
            {
                // return - only if not already returned
                if (!expenseItem.returned)
                {
                    if (expenseItem.ValidationCount < 2)
                    {
                        claims.ReturnExpenses(claim, new List<int> { expenseItem.expenseid },
                            "Expense returned because it failed receipt validation for business reasons.",
                            User.EmployeeID, null, null, true);
                    }
                    else
                    {
                        var subCat = ActionContext.SubCategories.GetSubcatById(expenseItem.subcatid);
                        expenseItem.DetermineValidationProgress(User.Account, subCat.Validate, true, _data);
                    }
                }
            }

            validationManager.UpdateOperatorProgressForExpenseItem(expenseItem.expenseid,
                DALEnum.ExpediteOperatorValidationProgress.Available);

            // advance the claim if all items are complete or don't require validation
            var expenseItems = claims.getExpenseItemsFromDB(claim.claimid);
            var resultsAllowAdvance = expenseItems.All(i => i.Value.ValidationProgress > DALEnum.ExpenseValidationProgress.WaitingForClaimant || i.Value.ValidationProgress < DALEnum.ExpenseValidationProgress.Required);

            if (resultsAllowAdvance)
            {
                // check that the claim is in a Valiadtion stage.
                var claimEmployee = ActionContext.Employees.GetEmployeeById(claim.employeeid);
                var signoffGroup = ActionContext.SignoffGroups.GetGroupById(claimEmployee.SignOffGroupID);

                // approve any expense items that might have been returned.
                claims.approveItems(expenseItems.Select(i => i.Value.expenseid).ToList());

                // only advance if it is.
                if (signoffGroup.stages.Values[claim.stage - 1].signofftype == SignoffType.SELValidation)
                {
                    ActionContext.ClaimSubmission.SendClaimToNextStage(claim, false, User.EmployeeID, User.EmployeeID, User.isDelegate ? User.Delegate.EmployeeID : (int?)null);
                }
            }
        }

        /// <summary>
        /// Deletes a Result.
        /// </summary>
        /// <param name="id">The Id of the Result to delete.</param>
        /// <returns>Null upon a successful delete.</returns>
        public override ExpenseValidationResult Delete(int id)
        {
            cExpenseItem expenseItem;

            // check the item exists
            var item = Get(id, out expenseItem);

            if (item == null)
            {
                throw new InvalidDataException(ApiResources.ApiErrorValidationResultDoesntExist);
            }

            // validate progress
            ValidateExpenseValidationProgress(expenseItem);

            // delete
            _data.DeleteResult(id);

            // update the expenseItem
            expenseItem.DetermineValidationProgress(User.Account, true, true, _data);

            return null;
        }

        /// <summary>
        /// Terminate blocked validation progress
        /// </summary>
        /// <param name="accountId">account id of current user</param>
        public void TerminateValidationProgressForBlockedExpenses(int accountId)
        {
            var expenseManager = new DAL.ExpenseValidationManager(accountId);
            expenseManager.UpdateExpediteValidationProgressForBlockedExpenseItems();
        }

        /// <summary>
        /// Gets the <see cref="ExpenseValidationResults"/> for the provided expense item
        /// </summary>
        /// <param name="expenseId">
        /// The expense id.
        /// </param>
        /// <returns>
        /// The <see cref="ExpenseValidationResults"/>.
        /// </returns>
        public ExpenseValidationResults GetExpenseItemValidationResults(int expenseId)
        {
            //check expense item ownership
            var expenseItemRepository = new ExpenseItemRepository(this.User, this.ActionContext);
            expenseItemRepository.Get(expenseId);

            var validationManager = new DAL.ExpenseValidationManager(User.AccountID);
            List<DAL.ExpenseValidationResult> initialResults = validationManager.GetResultsForExpenseItem(expenseId).ToList();
            Dictionary<DALEnum.ExpenseValidationResultStatus, string> statusIcons = DAL.ExpediteValidationHelper.GetStatusIcons();
            Dictionary<DALEnum.ExpenseValidationResultStatus, string> statusDescriptions = DAL.ExpediteValidationHelper.GetStatusDescriptions();
            cExpenseItem expense = this.ActionContext.Claims.getExpenseItemById(expenseId);

            var invalidated = expense.ValidationProgress > DALEnum.ExpenseValidationProgress.CompletedPassed;

            var results = validationManager.GetExpenseValidationResults(
                   initialResults,
                   statusIcons,
                   invalidated,
                   statusDescriptions,
                   this.ActionContext.Fields);

            var expenseValidationResults = new ExpenseValidationResults();
            ExpenseValidationArea businessResults = this.ProcessValidationArea(results.BusinessResults, "Business ", statusDescriptions);
            ExpenseValidationArea vatResults = this.ProcessValidationArea(results.VatResults, "VAT ", statusDescriptions);

            var expenseValidationAreaResults = new List<ExpenseValidationArea> { businessResults, vatResults };

            cClaim claim = this.ActionContext.Claims.getClaimById(expense.claimid);

            if (this.User.EmployeeID == claim.checkerid)
            {
                var fraudResults = this.ProcessValidationAreaFraud(results.FraudResults, statusDescriptions);
                expenseValidationAreaResults.Add(fraudResults);
            }

            expenseValidationResults.ValidationResults = expenseValidationAreaResults;

            return expenseValidationResults;
        }

        #endregion Public Methods

        #region Private

        /// <summary>
        /// The process validation area.
        /// </summary>
        /// <param name="validationResult">
        /// The validation result.
        /// </param>
        /// <param name="headerText">
        /// The header text.
        /// </param>
        /// <param name="statusDescriptions">
        /// The status descriptions.
        /// </param>
        /// <returns>
        /// The <see cref="ExpenseValidationArea"/>.
        /// </returns>
        private ExpenseValidationArea ProcessValidationArea(List<DAL.ValidationResult> validationResult, string headerText, Dictionary<DALEnum.ExpenseValidationResultStatus, string> statusDescriptions)
        {
            var validationArea = new ExpenseValidationArea();
            var hasPassedValidationCriterion = validationResult.All(x => x.Status != DALEnum.ExpenseValidationResultStatus.Fail);
            string areaValidationOutcome = hasPassedValidationCriterion
                                   ? statusDescriptions[DALEnum.ExpenseValidationResultStatus.Pass]
                                   : statusDescriptions[DALEnum.ExpenseValidationResultStatus.Fail];

            validationArea.HeaderText = headerText + areaValidationOutcome;
            validationArea.ExpenseValidationStatus = hasPassedValidationCriterion ? ExpenseValidationStatus.Pass : ExpenseValidationStatus.Fail;

            validationArea.ValidationCriterionResults = this.BuildCriterion(validationResult);

            return validationArea;
        }

        /// <summary>
        /// The process validation area for fraud.
        /// </summary>
        /// <param name="validationResult">
        /// The <see cref="DAL.ValidationResult"/> ValidationResult.
        /// </param>
        /// <param name="statusDescriptions">
        /// The status descriptions.
        /// </param>
        /// <returns>
        /// The <see cref="ExpenseValidationArea"/>.
        /// </returns>
        private ExpenseValidationArea ProcessValidationAreaFraud(
            List<DAL.ValidationResult> validationResult,
            Dictionary<DALEnum.ExpenseValidationResultStatus, string> statusDescriptions)
        {
            var validationArea = new ExpenseValidationArea();
            var hasPassedValidationCriterion = validationResult.All(x => x.Status != DALEnum.ExpenseValidationResultStatus.Fail);

            string headerText = hasPassedValidationCriterion
                                            ? statusDescriptions[DALEnum.ExpenseValidationResultStatus.Pass]
                                            : DAL.ExpediteValidationHelper.FraudTypeDescription;

            validationArea.HeaderText = "Possible Fraud Indication " + headerText;
            validationArea.ExpenseValidationStatus = hasPassedValidationCriterion ? ExpenseValidationStatus.Pass : ExpenseValidationStatus.Warning;
            validationArea.ValidationCriterionResults = this.BuildCriterion(validationResult);

            return validationArea;
        }

        /// <summary>
        /// Builds up the crition results from the provided list of <see cref="DAL.ValidationResult"/>
        /// </summary>
        /// <param name="results">
        /// The list of <see cref="DAL.ValidationResult"/>
        /// </param>
        /// <returns>
        /// A list of <see cref="ValidationCriterion"/>
        /// </returns>
        private List<ValidationCriterion> BuildCriterion(List<DAL.ValidationResult> results)
        {
            var validationCriterionResults = new List<ValidationCriterion>();

            if (results.Any())
            {
                results.RemoveAll(r => r.Status == DALEnum.ExpenseValidationResultStatus.NotApplicable);

                results.ForEach(
                    result =>
                    {
                        var criterion = new ValidationCriterion();

                        // check if the validator has added comments
                        var commentsIfAny = string.IsNullOrWhiteSpace(result.Comments)
                                                ? string.Empty
                                                : result.Comments;

                        criterion.Comment = commentsIfAny;
                        criterion.Criterion = result.FriendlyMessage;

                        criterion.ExpenseValidationStatus = result.Type == DAL.ExpediteValidationType.Fraud
                                                                ? ExpenseValidationStatus.Warning
                                                                : (ExpenseValidationStatus)result.Status;

                        validationCriterionResults.Add(criterion);
                    });

                //order so failed criteria show first
                return validationCriterionResults.OrderBy(x => x.ExpenseValidationStatus).ToList();
            }
            else
            {
                return validationCriterionResults;
            }
        }

        /// <summary>
        /// Attemps to get an expense item, throwing if not.
        /// </summary>
        /// <param name="expenseId">The Expense Id.</param>
        /// <returns>The Expense Item, then null.</returns>
        private cExpenseItem TryGetExpenseItem(int expenseId)
        {
            // error check
            var expenseItem = ActionContext.Claims.getExpenseItemById(expenseId);
            if (expenseItem == null)
            {
                throw new InvalidDataException(ApiResources.ApiErrorValidationResultInvalidExpense);
            }
            return expenseItem;
        }

        /// <summary>
        /// Throws errors based on the status of the expense item.
        /// </summary>
        /// <param name="expenseItem">The item to check</param>
        private void ValidateExpenseValidationProgress(cExpenseItem expenseItem)
        {
            // handle errors on the progress of the expense item
            switch (expenseItem.ValidationProgress)
            {
                case DALEnum.ExpenseValidationProgress.ValidationServiceDisabled:
                    throw new InvalidDataException(ApiResources.ApiErrorValidationResultValidationDisabledAccount);
                case DALEnum.ExpenseValidationProgress.SubcatValidationDisabled:
                    throw new InvalidDataException(ApiResources.ApiErrorValidationResultValidationDisabledSubcat);
                case DALEnum.ExpenseValidationProgress.StageNotInSignoffGroup:
                    throw new InvalidDataException(ApiResources.ApiErrorValidationResultValidationDisabledSignoff);
                case DALEnum.ExpenseValidationProgress.CompletedFailed:
                case DALEnum.ExpenseValidationProgress.CompletedWarning:
                case DALEnum.ExpenseValidationProgress.CompletedPassed:
                    if (expenseItem.ValidationCount > 0)
                    {
                        throw new InvalidDataException(ApiResources.ApiErrorValidationResultValidationCompleted);
                    }
                    break;
                case DALEnum.ExpenseValidationProgress.NotRequired:
                    throw new InvalidDataException(ApiResources.ApiErrorValidationResultValidationNotRequired);
            }
        }

        #endregion Private
    }
}