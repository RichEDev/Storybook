namespace SpendManagementApi.Repositories
{
    using System;
    using System.Linq;

    using BusinessLogic.DataConnections;
    using BusinessLogic.GeneralOptions;

    using Models.Types;

    using SpendManagementLibrary.Expedite;

    using Spend_Management;

    using System.Collections.Generic;

    using Interfaces;

    using Models.Common;

    using Utilities;

    using ExpenseItem = Models.Types.ExpenseItem;


    using Models.Responses;
    using Models.Types.Employees;

    using SpendManagementLibrary;
    using SpendManagementLibrary.ExpenseItems;
    using SpendManagementLibrary.Enumerators.Expedite;

    using Common;
    using Common.Enums;

    using SpendManagementApi.Common.Enum;

    using BankAccount = SpendManagementLibrary.Employees.BankAccount;
    using Claim = SpendManagementApi.Models.Types.Claim;
    using CostCentreBreakdown = SpendManagementApi.Models.Types.Employees.CostCentreBreakdown;
    using ExpenseItemFlagSummary = SpendManagementApi.Models.Types.ExpenseItemFlagSummary;
    using ExpenseValidationProgress = SpendManagementApi.Models.Types.Expedite.ExpenseValidationProgress;
    using FlagAction = SpendManagementLibrary.Flags.FlagAction;
    using FlagColour = SpendManagementLibrary.Flags.FlagColour;
    using FlagFrequencyType = SpendManagementLibrary.Flags.FlagFrequencyType;
    using FlaggedItemsManager = SpendManagementApi.Models.Types.FlaggedItemsManager;
    using FlagInclusionType = SpendManagementLibrary.Flags.FlagInclusionType;
    using FlagPeriodType = SpendManagementLibrary.Flags.FlagPeriodType;
    using FlagSummary = SpendManagementApi.Models.Types.FlagSummary;
    using FlagType = SpendManagementLibrary.Flags.FlagType;
    using ValidationPoint = SpendManagementApi.Common.Enums.ValidationPoint;
    using ValidationType = SpendManagementApi.Common.Enums.ValidationType;

    /// <summary>
    /// Manages data access for <see cref="ExpenseItem">ExpenseItems</see>.
    /// </summary>
    internal class ExpenseItemRepository : BaseRepository<ExpenseItem>, ISupportsActionContext, IValidateCheckPay
    {
        private readonly cClaims _claimsData;

        private readonly cExpenseItems _expenseItemData;

        const string UdfExpenseItemsTableName = "userdefinedExpenses";

        const string UnapprovalOfExpenseItem = "Unapproval of Expense Item";

        private readonly IDataFactory<IGeneralOptions, int> _generalOptionsFactory;

        /// <summary>
        /// Creates a new ExpenseItemRespository.
        /// </summary>
        /// <param name="user">The Current User.</param>
        /// <param name="actionContext">Although it appears not to be used, this constructor context parameter is used with reflection and generics.</param>
        public ExpenseItemRepository(ICurrentUser user, IActionContext actionContext)
            : base(user, x => x.Id, null)
        {
            this._claimsData = ActionContext.Claims;
            this._expenseItemData = ActionContext.ExpenseItems;
            this._generalOptionsFactory = WebApiApplication.container.GetInstance<IDataFactory<IGeneralOptions, int>>();
        }

        /// <summary>
        /// Get all of T
        /// </summary>
        /// <returns></returns>
        public override IList<ExpenseItem> GetAll()
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Gets all expense items for this account that require Validation.
        /// </summary>
        /// <returns></returns>
        public List<ValidatableExpenseInfo> GetIdsRequiringValidation()
        {
            var list = new List<ValidatableExpenseInfo>();

            this.ActionContext.Accounts.GetAccountsWithValidationServiceEnabled().ForEach(
                a =>
                {
                    this.ActionContext.ExpenseValidation.AccountId = a.accountid;
                    list.AddRange(this.ActionContext.ExpenseValidation.GetExpenseItemIdsRequiringValidation());
                });

            return list.OrderBy(i => i.ModifiedOn).ToList();
        }

        /// <summary>
        /// Gets the Jorney Steps for the expenseId
        /// </summary>
        /// <param name="expenseId"></param>
        /// <returns>A list of <see cref="JourneyStep"></see>/>JourneyStep</returns>
        public List<JourneyStep> GetJourneySteps(int expenseId)
        {
            cExpenseItem item = GetExpenseItem(expenseId);

            //Checks claim ownership before proceeding
            new ClaimRepository(this.User, this.ActionContext).Get(item.claimid);

            var journeySteps = this._expenseItemData.GetJourneySteps(expenseId);

            if (journeySteps == null)
            {
                throw new ApiException(
                    string.Format(ApiResources.ApiErrorXUnsuccessfulMessage, "Get Journey Steps"),
                    string.Format(ApiResources.ApiErrorAnNonExistentX, "Journey Steps for the Expense Item"));
            }

            var addressRepository = new AddressRepository(this.User, this.ActionContext);
            List<Address> homeAndOfficeAddresses = addressRepository.GetHomeAndOfficeAddresses(item.createdby);

            List<JourneyStep> stepList = this.ProcessSteps(expenseId, journeySteps, item.carid, homeAndOfficeAddresses);

            this._expenseItemData.AuditExpenseItemsViewed($"{item.refnum}, {item.date}, {this.ActionContext.SubCategories.GetSubcatById(item.subcatid).subcat}, {item.total:0.00}", this.ActionContext.Claims.GetClaimOwnerByClaimId(item.claimid), this.User);

            return stepList.Count > 0 ? stepList : null;
        }

        /// <summary>
        /// Processes the journey steps against an expense item. Converts each <see cref="JourneyStep">JourneyStep</see> and gets the recommended or custom distance for the step
        /// </summary>
        /// <param name="expenseId">
        /// The expense id.
        /// </param>
        /// <param name="journeySteps">
        /// The journey steps.
        /// </param>
        /// <param name="vehicleId">
        /// The vehicle id.
        /// </param>
        /// <param name="homeAndOfficeAddresses">
        /// The home and office addresses.
        /// </param>
        /// <param name="labels">
        /// The address labels.
        /// </param>
        /// <returns>
        /// A list of <see cref="JourneyStep"/>
        /// </returns>
        public List<JourneyStep> ProcessSteps(
            int expenseId,
            SortedList<int, cJourneyStep> journeySteps,
            int vehicleId,
            List<Address> homeAndOfficeAddresses          )
        {
            var stepList = new List<JourneyStep>();

            foreach (var step in journeySteps)
            {
                JourneyStep journeyStep = new JourneyStep(expenseId).From(step.Value, this.ActionContext);

                if (step.Value.startlocation == null || step.Value.endlocation == null)
                {
                    journeyStep.RecordedMiles = 0;
                    stepList.Add(journeyStep);
                }
                else
                {
                var addressRecommendedDistanceRepository = new AddressRecommendedDistanceRepository(this.User, this.ActionContext);

                    decimal? recommendedDistance = addressRecommendedDistanceRepository.GetRecommendedOrCustomDistance(
                        step.Value.startlocation.Identifier,
                        step.Value.endlocation.Identifier,
                        vehicleId);
                journeyStep.RecordedMiles = recommendedDistance ?? 0;

                this.SetHomeOfficeAddress(homeAndOfficeAddresses, journeyStep.StartLocation);
                this.SetHomeOfficeAddress(homeAndOfficeAddresses, journeyStep.EndLocation);
                stepList.Add(journeyStep);
            }
            }

            return stepList;
        }

        /// <summary>
        /// Set home address
        /// </summary>
        /// <param name="homeAndOfficeAddresses">Home address collection</param>
        /// <param name="address">Address item</param>
        private void SetHomeOfficeAddress(List<Address> homeAndOfficeAddresses, Address address)
        {
            if (homeAndOfficeAddresses != null && homeAndOfficeAddresses.Any(x => x.Id == address.Id))
            {
                var homeOfficeAddress = homeAndOfficeAddresses.FirstOrDefault(x => x.Id == address.Id);
                if (homeOfficeAddress != null)
                {
                    address.GlobalIdentifier = homeOfficeAddress.GlobalIdentifier;
                    address.Line1 = homeOfficeAddress.Line1;
                    address.Line2 = homeOfficeAddress.Line2;
                    address.Line3 = homeOfficeAddress.Line3;
                    address.City = homeOfficeAddress.City;
                    address.County = homeOfficeAddress.County;
                    address.Postcode = homeOfficeAddress.Postcode;
                    address.StartDate = homeOfficeAddress.StartDate;
                    address.EndDate = homeOfficeAddress.EndDate;
                    address.FriendlyName = homeOfficeAddress.FriendlyName;
                    address.IsHomeAddress = homeOfficeAddress.IsHomeAddress;
                    address.IsOfficeAddress = homeOfficeAddress.IsOfficeAddress;
                }
            }
        }

        /// <summary>
        /// Gets the message explaining what, if any, the home to calculation type is for this expense.
        /// </summary>
        /// <param name="expenseId">
        /// A valid expenseID which the user has permission to see
        /// </param>
        /// <returns>
        /// The home to office calculation
        /// </returns>
        public string GetHomeToOfficeMileageComment(int expenseId)
        {
            var expenseItem = this.GetExpenseItem(expenseId);
            //Checks claim ownership before proceeding and get claim for expense id
            var claim = new ClaimRepository(this.User, this.ActionContext).Get(expenseItem.claimid);
            var message = new MileageCategoryRepository(this.User, this.ActionContext).GetComment(expenseItem.carid, expenseItem.subcatid, expenseItem.date, expenseItem.mileageid, claim.EmployeeId)[2];

            return message;
        }

        #region Non-applicable methods

        /// <summary>
        /// Gets an Expense Item by its id.
        /// </summary>
        /// <param name="id">The Id</param>
        /// <returns>An <see cref="ExpenseItem">ExpenseItem</see></returns>
        public override ExpenseItem Get(int id)
        {
            cExpenseItem item = GetExpenseItem(id);

            //Checks claim ownership before proceeding
            new ClaimRepository(this.User, this.ActionContext).Get(item.claimid);
            var expenseIds = new List<int> { id };

            item.flags = _expenseItemData.GetFlaggedItems(expenseIds).First;
            item.costcodebreakdown = _expenseItemData.getCostCodeBreakdown(item.expenseid);

            var expenseItem = new ExpenseItem().From(item, this.ActionContext);

            if (expenseItem.ClaimReasonId > 0)
            {
                expenseItem.ClaimReasonInfo =
                    new ClaimReasonRepository(User, ActionContext).Get(expenseItem.ClaimReasonId).Description;
            }

            if (expenseItem.CompanyId > 0)
            {
                Organisation company =
                    new OrganisationRepository(this.User, this.ActionContext).Get(expenseItem.CompanyId);

                if (company != null)
                {
                    expenseItem.CompanyName = company.Label;
                }
            }

            if (expenseItem.HotelId > 0)
            {
                Hotel hotel = new HotelRepository(this.User, this.ActionContext).Get(expenseItem.HotelId);

                if (hotel != null)
                {
                    expenseItem.HotelName = hotel.HotelName;
                }
            }

            expenseItem.JourneySteps = this.GetJourneySteps(expenseItem.Id);

            return expenseItem;
        }

        /// <summary>
        /// Sets an item to unapproved, providing it passes the required checks
        /// </summary>
        /// <param name="claimId"></param>
        /// <param name="expenseIds">The expenseId</param>
        /// <returns></returns>
        public List<int> UnapproveExpenseItem(int claimId, List<int> expenseIds)
        {
            //Checks claim ownership before proceeding
            Models.Types.Claim claim = new ClaimRepository(this.User, this.ActionContext).Get(claimId);
            List<cExpenseItem> expenseItems = expenseIds.Select(expenseId => GetExpenseItem(expenseId)).ToList();
            ValidateUnapproveExpenseItemAction(expenseItems, claim);
            var failedExpenseIds = new List<int>();

            foreach (var item in expenseItems)
            {

                if (_claimsData.UnapproveItem(item) > 0)
                {
                    _claimsData.DeleteClaimApproverDetailByExpensesId(item.expenseid, this.User.EmployeeID);
                }
                else
                {
                    failedExpenseIds.Add(item.expenseid);
                }
            }

            return failedExpenseIds;
        }

        /// <summary>
        ///  Sets expense items to returned, providing they pass the required checks
        /// </summary>
        /// <param name="claimId">The claim Id</param>
        /// <param name="expenseIds">The expense Id</param>
        /// <param name="reason">The reason</param>
        /// <returns></returns>
        public int ReturnExpenseItems(int claimId, List<int> expenseIds, string reason)
        {
            //Checks claim ownership before proceeding
            Models.Types.Claim claim = new ClaimRepository(this.User, this.ActionContext).Get(claimId);

            ValidateApproveReturnExpenseItemsAction(claim, expenseIds, CheckAndPayAction.ReturnItems);

            bool outcome = _claimsData.ReturnExpenses(
                claim.To(ActionContext),
                expenseIds,
                reason,
                User.EmployeeID,
                null);
            return outcome ? 1 : 0;
        }

        /// <summary>
        ///  Sets expense items to approved, providing they pass the required checks
        /// </summary>
        /// <param name="claimId">The claim Id</param>
        /// <param name="expenseIds">The expense Id</param>
        /// <returns></returns>
        public AllowExpenseItems ApproveExpenseItems(int claimId, List<int> expenseIds)
        {
            //Checks claim ownership before proceeding
            Models.Types.Claim claim = new ClaimRepository(this.User, this.ActionContext).Get(claimId);

            ValidateApproveReturnExpenseItemsAction(claim, expenseIds, CheckAndPayAction.ApproveItems);

            AllowExpenseItemsResult allowExpenseItemsResult = _claimsData.AllowExpenseItems(
                User.AccountID,
                User.EmployeeID,
                null,
                claimId,
                expenseIds);

            AllowExpenseItems allowExpenseItems = new AllowExpenseItems().From(allowExpenseItemsResult, this.ActionContext);

            if (allowExpenseItemsResult.FlaggedItemsManager != null)
            {
                var flagSummaries = new List<ExpenseItemFlagSummary>();

                foreach (SpendManagementLibrary.Flags.ExpenseItemFlagSummary flagSummary in allowExpenseItemsResult.FlaggedItemsManager.List)
                {
                    if (expenseIds.Contains(flagSummary.ExpenseID))
                    {
                        //Add only flag details for the expense items in the request.
                        flagSummaries.Add(new ExpenseItemFlagSummary().From(flagSummary, this.ActionContext));
                    }

                }

                if (flagSummaries.Count > 0)
                {
                    allowExpenseItems.FlaggedItemsManager.ExpenseCollection = flagSummaries;
                }
                else
                {
                    //If there are no relevant flags for to the expense ids in the request, then we can set the FlaggedItemsManager to null
                    allowExpenseItems.FlaggedItemsManager = null;
                }
            }

            return allowExpenseItems;
        }


        /// <summary>
        /// Gets a list of <see cref="Models.Types.Employees.CostCentreBreakdown"> CostCentreBreakdown</see>s />
        /// </summary>
        /// <param name="expenseId">The expense Id</param>
        /// <param name="response">The response</param>
        /// <returns>A list of <see cref="Models.Types.Employees.CostCentreBreakdown"> CostCentreBreakdown</see>s</returns>
        public List<Models.Types.Employees.CostCentreBreakdown> GetCostCodeBreakdown(
            int expenseId,
            GetExpenseItemCostCodeBreakdownResponse response)
        {
            cExpenseItem item = GetExpenseItem(expenseId);

            //Checks claim ownership before proceeding
            new ClaimRepository(this.User, ActionContext).Get(item.claimid);

            List<cDepCostItem> css = this._expenseItemData.getCostCodeBreakdown(expenseId);

            if (css == null)
            {
                throw new ApiException(
                    string.Format(ApiResources.ApiErrorXUnsuccessfulMessage, "Get Cost Code Breakdown"),
                    string.Format(ApiResources.ApiErrorAnNonExistentX, "Cost Code Breakdown for the Expense Item"));
            }

            response.List = css.Cast<List<Models.Types.Employees.CostCentreBreakdown>>().ToList();
            return response.List;
        }

        /// <summary>
        /// Adds an ExpenseItem.
        /// </summary>
        /// <param name="item">The expense item to add.</param>
        /// <returns>A ExpenseItemResponse containing the added <see cref="ExpenseItem">ExpenseItem</see>.</returns>
        public override ExpenseItem Add(ExpenseItem item)
        {
            item = base.Add(item);
            var subcat = this.ActionContext.SubCategories.GetSubcatById(item.ExpenseSubCategoryId);
            item.UserDefined = UdfValidator.Validate(
                item.UserDefined,
                this.ActionContext,
                UdfExpenseItemsTableName,
                true,
                subcat.associatedudfs);

            return SaveExpenseItem(item, false);
        }

        /// <summary>
        /// Updates an ExpenseItem.
        /// </summary>
        /// <param name="item">
        /// The expense item to update.
        /// </param>
        /// <param name="oldClaimId">
        /// The old claim Id. Used when moving an expense item to a new claim.
        /// </param>
        /// <param name="offlineItem">
        /// Is an offline Expense Item.
        /// </param>
        /// <param name="reasonForAmendment">
        /// The reason For amendment.
        /// </param>
        /// <returns>
        /// A ExpenseItemResponse containing the added <see cref="ExpenseItem">ExpenseItem</see>.
        /// </returns>
        public ExpenseItem Update(ExpenseItem item, int oldClaimId, bool offlineItem, string reasonForAmendment)
        {
            item = base.Update(item);
            var subcat = this.ActionContext.SubCategories.GetSubcatById(item.ExpenseSubCategoryId);
            item.UserDefined = UdfValidator.Validate(
                item.UserDefined,
                this.ActionContext,
                UdfExpenseItemsTableName,
                true,
                subcat.associatedudfs);
            return UpdateExpenseItem(item, oldClaimId, offlineItem, false, reasonForAmendment);
        }

        /// <summary>
        /// Saves an expense item
        /// </summary>
        /// <param name="item">
        /// The ExpenseItem
        /// </param>
        /// <param name="isMobileRequest">
        /// The is Mobile Request.
        /// </param>
        /// <returns>
        /// An <see cref="ExpenseItem">ExpenseItem</see>
        /// </returns>
        public ExpenseItem SaveExpenseItem(ExpenseItem item, bool isMobileRequest)
        {
            var subAccounts = new cAccountSubAccounts(this.User.AccountID);
            cAccountProperties accountProperties = subAccounts.getSubAccountById(this.User.CurrentSubAccountId).SubAccountProperties;

            if (CanSaveExpenseItem(item.ClaimId, item.Returned, item.ItemCheckerId, item.Edited, accountProperties))
            {
                if (item.ESRAssignmentId != 0 && item.JourneySteps != null)
                {
                    item.JourneySteps = this.UpdateOfficeAddressBasedOnEsrAssignment(item, accountProperties);
                }

                //set correct validation progress when adding a new expense item
                item.ValidationProgress = ExpenseValidationProgress.ValidationServiceDisabled;

                cExpenseItem cExpenseItem = item.To(ActionContext);

                FlaggedItemsManager blockedFlags = this.CheckExpenseItemForBlockedFlags(cExpenseItem, this.User.EmployeeID);

                if (blockedFlags != null && blockedFlags.Count > 0)
                {
                    item.BlockedFlags = blockedFlags;
                    return item;
                }

                if (item.SplitItems != null)
                {
                    //Process split items for expense item
                    foreach (ExpenseItem split in item.SplitItems)
                    {
                        cExpenseItem splitItem = this.ProcessSplitItem(split, cExpenseItem);
                        cExpenseItem.addSplitItem(splitItem);
                    }
                }

                var id = _expenseItemData.addItem(cExpenseItem, User.Employee);

                ExpenseItem expenseItem;

                if (isMobileRequest)
                {
                    expenseItem = new ExpenseItem();
                    switch (id)
                    {
                        case -1:
                            expenseItem.ExpenseActionOutcome = ExpenseActionOutcome.CreditCardItemsAlreadyReconciled;
                            break;
                        case -2:
                            expenseItem.ExpenseActionOutcome = ExpenseActionOutcome.PurchaseCardItemsAlreadyReconciled;
                            break;
                        case -3:
                            expenseItem.ExpenseActionOutcome = ExpenseActionOutcome.ExpenseDateNotInBetweenVehicleDates;
                            break;
                        case -4:
                            expenseItem.ExpenseActionOutcome = ExpenseActionOutcome.RecommendedMileageExceeded;
                            break;
                        case -5:
                            expenseItem.ExpenseActionOutcome = ExpenseActionOutcome.AddressesNotMatched;
                            break;
                        case -7:
                            expenseItem.ExpenseActionOutcome = ExpenseActionOutcome.BankAccountCouldntBeAssigned;
                            break;
                        case 0:
                            return null;
                        default:
                            expenseItem.ExpenseActionOutcome = ExpenseActionOutcome.Successs;
                            break;
                    }

                    if (expenseItem.ExpenseActionOutcome != ExpenseActionOutcome.Successs)
                    {
                        return expenseItem;
                    }

                }
                else
                {
                    switch (id)
                    {
                        case -1:
                            throw new ApiException(
                                ApiResources.ApiErrorSaveUnsuccessful,
                                ApiResources.ApiErrorCreditCardReconcile);
                        case -2:
                            throw new ApiException(
                                ApiResources.ApiErrorSaveUnsuccessful,
                                ApiResources.ApiErrorPurchaseCardReconcile);
                        case -3:
                            throw new ApiException(
                                ApiResources.ApiErrorSaveUnsuccessful,
                                ApiResources.ApiErrorVehicleDates);
                        case -4:
                            throw new ApiException(ApiResources.ApiErrorSaveUnsuccessful, ApiResources.ApiErrorMileage);
                        case -5:
                            throw new ApiException(
                                ApiResources.ApiErrorSaveUnsuccessful,
                                ApiResources.ApiErrorAddressMatch);
                        case 0:
                            return null;
                    }
                }

                List<SpendManagementLibrary.Flags.FlagSummary> flags = this.GetFlagSummaries(id, User.EmployeeID);
                if (flags != null)
                {
                    //Save flag details
                    ActionContext.FlagManagement.FlagItem(id, flags);
                }

                expenseItem = this.Get(id);
                expenseItem.ExpenseActionOutcome = ExpenseActionOutcome.Successs;
                return expenseItem;
            }

            return null;
        }

        /// <summary>
        /// Updates an expense item
        /// </summary>
        /// <param name="item">
        /// The expense item to update.
        /// </param>
        /// <param name="oldClaimId">
        /// The old claim Id. Used when moving an expense item to a new claim.
        /// </param>
        /// <param name="offlineItem">
        /// Is an offline expense Item.
        /// </param>
        /// <param name="isMobileRequest">
        /// The is Mobile Request.
        /// </param>
        /// <param name="reasonForAmendment">
        /// The reason For amendment.
        /// </param>
        /// <returns>
        /// An <see cref="ExpenseItem"> ExpenseItem</see>.
        /// </returns>
        public ExpenseItem UpdateExpenseItem(ExpenseItem item, int oldClaimId, bool offlineItem, bool isMobileRequest, string reasonForAmendment)
        {
            var subAccounts = new cAccountSubAccounts(this.User.AccountID);
            cAccountProperties accountProperties = subAccounts.getSubAccountById(this.User.CurrentSubAccountId).SubAccountProperties;

            if (CanSaveExpenseItem(item.ClaimId, item.Returned, item.ItemCheckerId, item.Edited, accountProperties, reasonForAmendment))
            {
                if (item.ESRAssignmentId != 0 && item.JourneySteps != null)
                {

                    item.JourneySteps = this.UpdateOfficeAddressBasedOnEsrAssignment(item, accountProperties);
                }

                cExpenseItem cExpenseItem = item.To(ActionContext);

                var expenseIds = new List<int> { cExpenseItem.expenseid };

                if (item.SplitItems != null)
                {
                    //Process split items for expense item
                    foreach (ExpenseItem split in item.SplitItems)
                    {
                        expenseIds.Add(split.Id);
                        cExpenseItem splitItem = this.ProcessSplitItem(split, cExpenseItem);
                        cExpenseItem.addSplitItem(splitItem);
                    }
                }

                var claim = this.ActionContext.Claims.getClaimById(item.ClaimId);

                if (!claim.submitted)
                {
                    // only check for blocked flags if editing as claimant
                    FlaggedItemsManager blockedFlags = this.CheckExpenseItemForBlockedFlags(
                        cExpenseItem,
                        claim.employeeid);

                if (blockedFlags != null && blockedFlags.Count > 0)
                {
                 
                    var flagsManager = new FlaggedItemsManager();
                    var expenseCollection = new List<ExpenseItemFlagSummary>();
           
                    foreach (var flag in blockedFlags.ExpenseCollection)
                    {
                        expenseCollection.Add(flag);
                    }

                    flagsManager.ExpenseCollection = expenseCollection;

                    item.BlockedFlags = flagsManager;
                    return item;
                }
                }

                var id = _expenseItemData.updateItem(cExpenseItem, claim.employeeid, oldClaimId, offlineItem);

                ExpenseItem expenseItem;
                if (isMobileRequest)
                {
                    expenseItem = new ExpenseItem();
                    switch (id)
                    {
                        case -1:
                            expenseItem.ExpenseActionOutcome = ExpenseActionOutcome.VATGreaterThanTotal;
                            break;
                        case -3:
                            expenseItem.ExpenseActionOutcome = ExpenseActionOutcome.ExpenseDateNotInBetweenVehicleDates;
                            break;
                        case -4:
                            expenseItem.ExpenseActionOutcome = ExpenseActionOutcome.RecommendedMileageExceeded;
                            break;
                        case -7:
                            expenseItem.ExpenseActionOutcome = ExpenseActionOutcome.BankAccountCouldntBeAssigned;
                            break;
                        default:
                            expenseItem.ExpenseActionOutcome = ExpenseActionOutcome.Successs;
                            break;
                    }

                    if (expenseItem.ExpenseActionOutcome != ExpenseActionOutcome.Successs)
                    {
                        return expenseItem;
                    }

                }
                else
                {
                    switch (id)
                    {
                        case -1:
                            throw new ApiException(
                                ApiResources.ApiErrorSaveUnsuccessful,
                                ApiResources.ApiErrorVATGreaterThanTotal);
                        case -3:
                            throw new ApiException(
                                ApiResources.ApiErrorSaveUnsuccessful,
                                ApiResources.ApiErrorVehicleDates);
                        case -4:
                            throw new ApiException(ApiResources.ApiErrorSaveUnsuccessful, ApiResources.ApiErrorMileage);
                    }
                }


                List<SpendManagementLibrary.Flags.FlagSummary> flags = this.GetFlagSummaries(item.Id, claim.employeeid);

                if (flags != null)
                {
                    //Save flag details
                    ActionContext.FlagManagement.FlagItem(item.Id, flags);
                }

                if (!string.IsNullOrEmpty(reasonForAmendment))
                {
                    this.SaveReasonForAmendment(reasonForAmendment, claim, item.ReferenceNumber);
                }

                expenseItem = this.Get(item.Id);
                expenseItem.ExpenseActionOutcome = ExpenseActionOutcome.Successs;
                return expenseItem;
            }

            return null;
        }

        /// <summary>
        /// Save reason an approver has amended an expense item. 
        /// </summary>
        /// <param name="reasonForAmendment">
        /// The reason for amendment.
        /// </param>
        /// <param name="claim">
        /// An instance of <see cref="cClaim">cClaim</see>
        /// </param>
        /// <param name="referenceNumber">
        /// The expense item reference number
        /// </param>
        private void SaveReasonForAmendment(string reasonForAmendment, cClaim claim, string referenceNumber)
        {    
            var reason = $"Expense amended by authoriser.\r\nAuthoriser's Comments: {reasonForAmendment}";
            ActionContext.Claims.addComment(claim, this.User.EmployeeID, reason, referenceNumber);
        }

        /// <summary>
        /// Gets existing flag details for the expense items relating to the provided expense Ids.
        /// </summary>
        /// <param name="expenseIds">
        /// A list of expenseIds
        /// </param>  
        /// <returns>
        /// The <see cref="FlaggedItemsManager">FlaggedItemsManager</see> containing the flag details
        /// </returns>
        private FlaggedItemsManager GetFlagsManagerForExpenseItems(List<int> expenseIds)
        {
            var flagsManager = new FlaggedItemsManager();
            var flaggedItems = this._expenseItemData.GetFlaggedItems(expenseIds);

            var expenseCollection = new List<ExpenseItemFlagSummary>();

            foreach (SpendManagementLibrary.Flags.ExpenseItemFlagSummary flagSummary in flaggedItems.List)
            {
                var flagSummaries = new List<SpendManagementLibrary.Flags.FlagSummary>();

                foreach (SpendManagementLibrary.Flags.FlagSummary itemFlag in flagSummary.FlagCollection)
                {
                    flagSummaries.Add(itemFlag);
                }

                var expenseItemFlagSummaries = new SpendManagementLibrary.Flags.ExpenseItemFlagSummary(flagSummary.ExpenseID, flagSummaries);
                expenseCollection.Add(new ExpenseItemFlagSummary().From(expenseItemFlagSummaries, this.ActionContext));
            }

            flagsManager.ExpenseCollection = expenseCollection;
            return flagsManager;
        }

        /// <summary>
        /// Determines if the expense item can be saved. 
        /// </summary>
        /// <param name="claimId">
        /// The claim Id
        /// </param>
        /// <param name="itemReturned">
        /// Has the item been returned?
        /// </param>
        /// <param name="itemCheckerId">
        /// The employee Id of the item checker
        /// </param>
        /// <param name="edited">
        /// had the item been edited?
        /// </param>
        /// <param name="accountProperties">
        /// An instance of <see cref="cAccountProperties"/>
        /// </param>
        /// <param name="reasonForAmendment">
        /// The reason For Amendment.
        /// </param>
        /// <returns>
        /// Returns true, if all tests pass, else, throws an API exeception
        /// </returns>
        private bool CanSaveExpenseItem(int claimId, bool itemReturned, int? itemCheckerId, bool edited, cAccountProperties accountProperties, string reasonForAmendment = "")
        {
            var claims = new cClaims(User.AccountID);
            var claim = claims.getClaimById(claimId);

            if (claim == null)
            {
                throw new ApiException(
                    string.Format(ApiResources.ApiErrorCreateExpenseItemMessage, "Expense Item"),
                    string.Format(ApiResources.ApiErrorAnNonExistentX, "Claim"));
            }

            cAccountProperties properties =
                ActionContext.SubAccounts.getSubAccountById(User.CurrentSubAccountId).SubAccountProperties;
            var result =
                (ExpenseItemPermissionResult)
                _expenseItemData.ExpenseItemPermissionCheck(
                    claim.ClaimStage,
                    claim.employeeid,
                    claim.checkerid,
                    itemReturned,
                    itemCheckerId,
                    edited,
                    properties.EditPreviousClaims,
                    User,
                    reasonForAmendment,
                    accountProperties);
            const string ExpenseItem = "Expense Item";

            switch (result)
            {
                case ExpenseItemPermissionResult.EmployeeDoesNotOwnClaim:
                    throw new ApiException(
                        string.Format(ApiResources.ApiErrorCreateExpenseItemMessage, ExpenseItem),
                        (ApiResources.ApiErrorEpenseItemEmployeeDoesNotOwnClaim));
                case ExpenseItemPermissionResult.ClaimHasBeenSubmitted:
                    throw new ApiException(
                        string.Format(ApiResources.ApiErrorCreateExpenseItemMessage, ExpenseItem),
                        (ApiResources.ApiErrorExpenseItemClaimSubmitted));
                case ExpenseItemPermissionResult.ClaimHasBeenApproved:
                    throw new ApiException(
                        string.Format(ApiResources.ApiErrorCreateExpenseItemMessage, ExpenseItem),
                        (ApiResources.ApiErrorExpenseItemClaimApproved));
                case ExpenseItemPermissionResult.ExpenseItemHasBeenEdited:
                    throw new ApiException(
                        string.Format(ApiResources.ApiErrorCreateExpenseItemMessage, ExpenseItem),
                        (ApiResources.ApiErrorExpenseItemEdited));
                case ExpenseItemPermissionResult.NoReasonForAmendmentProvidedByApprover:
                    throw new ApiException(
                        string.Format(ApiResources.ApiErrorEditExpenseItemMessage, ExpenseItem),
                        (ApiResources.ApiErrorExpenseItemNoReasonForAmendmentProvidedByApprover));
            }

            return true;
        }

        public cExpenseItem GetExpenseItem(int expenseId)
        {
            var item = this._claimsData.getExpenseItemById(expenseId);

            if (item == null)
            {
                throw new ApiException(
                    string.Format(ApiResources.ApiErrorXUnsuccessfulMessage, "Get Expense Item"),
                    string.Format(ApiResources.ApiErrorAnNonExistentX, "Expense Item"));
            }

            return item;
        }

        /// <summary>
        /// Validates the expense item before the unapprove action can take place.
        /// Throws an API expection on any checks that fail
        /// </summary>
        /// <param name="expenseItems">The list of expense items to be unapproved</param>
        /// <param name="claim">The claim</param>
        public void ValidateUnapproveExpenseItemAction(IEnumerable<cExpenseItem> expenseItems, Models.Types.Claim claim)
        {

            var expenseIds = new List<int>();

            foreach (cExpenseItem item in expenseItems)
            {

                if (item.claimid != claim.Id)
                {
                    throw new ApiException(
                        string.Format(ApiResources.ApiErrorXUnsuccessfulMessage, UnapprovalOfExpenseItem),
                        string.Format(ApiResources.ApiErrorExpenseItemDoesNotBelongToClaim));
                }

                if (!item.tempallow)
                {
                    throw new ApiException(
                        string.Format(ApiResources.ApiErrorXUnsuccessfulMessage, UnapprovalOfExpenseItem),
                        string.Format(ApiResources.ApiErrorExpenseItemUnapprovalApproved));
                }

                if (item.returned)
                {
                    throw new ApiException(
                        string.Format(ApiResources.ApiErrorXUnsuccessfulMessage, UnapprovalOfExpenseItem),
                        string.Format(ApiResources.ApiErrorExpenseItemUnapprovalReturned));
                }

                expenseIds.Add(item.expenseid);
            }

            if (ExpenseItemsBelongToClaim(claim.Id, expenseIds, UnapprovalOfExpenseItem))
            {
                this.ValidateClaimForCheckAndPayAction(claim);
            }
        }


        /// <summary>
        /// Validates each expense item before the check and pay action can take place.
        /// Throws an API expection on any checks that fail
        /// </summary>
        /// <param name="claim">The claim Id</param>
        /// <param name="expenseIds">The expense Ids</param>
        /// <param name="action">The action being performed</param>
        public void ValidateApproveReturnExpenseItemsAction(
            Models.Types.Claim claim,
            IEnumerable<int> expenseIds,
            CheckAndPayAction action)
        {
            string message = action == CheckAndPayAction.ReturnItems ? "Return Expense Items" : "Approve Expense Items";

            if (ExpenseItemsBelongToClaim(claim.Id, expenseIds, message))
            {

                foreach (cExpenseItem item in expenseIds.Select(expenseId => GetExpenseItem(expenseId)))
                {
                    if (item.tempallow)
                    {
                        throw new ApiException(
                            string.Format(ApiResources.ApiErrorXUnsuccessfulMessage, message),
                            string.Format(ApiResources.ApiErrorExpenseItemAlreadyApproved));
                    }

                    if (action == CheckAndPayAction.ReturnItems && item.returned)
                    {
                        throw new ApiException(
                            string.Format(ApiResources.ApiErrorXUnsuccessfulMessage, message),
                            string.Format(ApiResources.ApiErrorExpenseItemAlreadyReturned));
                    }

                }
                this.ValidateClaimForCheckAndPayAction(claim);
            }
        }

        /// <summary>
        /// Runs the claim through a series of checks before the check and pay action can take place. Throws an API expection on any checks that fail.
        /// </summary>
        /// <param name="claim">The claim</param>
        public void ValidateClaimForCheckAndPayAction(Models.Types.Claim claim)
        {
            if (!claim.Submitted)
            {
                throw new ApiException(
                    string.Format(ApiResources.ApiErrorXUnsuccessfulMessage, UnapprovalOfExpenseItem),
                    string.Format(ApiResources.ApiErrorExpenseItemClaimSubmitted));
            }

            if (claim.Approved)
            {
                throw new ApiException(
                    string.Format(ApiResources.ApiErrorXUnsuccessfulMessage, UnapprovalOfExpenseItem),
                    string.Format(ApiResources.ApiErrorExpenseItemClaimApproved));
            }

            if (claim.ClaimStage != Common.Enums.ClaimStage.Submitted)
            {
                throw new ApiException(
                    string.Format(ApiResources.ApiErrorXUnsuccessfulMessage, UnapprovalOfExpenseItem),
                    string.Format(ApiResources.ApiErrorExpenseItemUnapprovalClaimNotAtSubmittedStage));
            }

            cAccountProperties properties =
                ActionContext.SubAccounts.getSubAccountById(User.CurrentSubAccountId).SubAccountProperties;

            if (!properties.AllowTeamMemberToApproveOwnClaim && claim.EmployeeId == this.User.EmployeeID)
            {
                throw new ApiException(
                    string.Format(ApiResources.ApiErrorXUnsuccessfulMessage, UnapprovalOfExpenseItem),
                    string.Format(ApiResources.ApiErrorExpenseItemUnapprovalCannotSelfApprove));
            }
        }

        /// <summary>
        /// Checks if the expenseItemsIds belong to the claim
        /// </summary>
        /// <param name="claimId">The claim Id</param>
        /// <param name="expenseIds">The expense Ids</param>
        /// <param name="ApiErrorMessage">The error message for the API to throw</param>
        /// <returns></returns>
        public bool ExpenseItemsBelongToClaim(int claimId, IEnumerable<int> expenseIds, string ApiErrorMessage)
        {
            foreach (cExpenseItem item in expenseIds.Select(expenseId => GetExpenseItem(expenseId)))
            {
                if (item.claimid != claimId)
                {
                    throw new ApiException(
                        string.Format(ApiResources.ApiErrorXUnsuccessfulMessage, ApiErrorMessage),
                        string.Format(ApiResources.ApiErrorExpenseItemDoesNotBelongToClaim));
                }
            }

            return true;
        }

        /// <summary>
        /// Gets the flags for an expense item
        /// </summary>
        /// <param name="expenseId">The expenseId</param>
        /// <param name="validationPoint">The validationPoint</param>
        /// <param name="validationType">The validation type</param>
        /// <returns></returns>
        public List<FlagSummary> GetExpenseItemFlags(
            int expenseId,
            ValidationPoint validationPoint,
            ValidationType validationType)
        {
            //checks ownership before proceeding 
            var expenseItem = this.Get(expenseId);
            new ClaimRepository(User, ActionContext).Get(expenseItem.ClaimId);

            SpendManagementLibrary.Flags.ValidationPoint smlValdationPoint =
                (SpendManagementLibrary.Flags.ValidationPoint)validationPoint;
            SpendManagementLibrary.Flags.ValidationType smlValidationType =
                (SpendManagementLibrary.Flags.ValidationType)validationType;
            var properties = ActionContext.SubAccounts.getSubAccountById(User.CurrentSubAccountId);
            List<SpendManagementLibrary.Flags.FlagSummary> flagSummaries =
                ActionContext.FlagManagement.CheckItemForFlags(
                    expenseItem.To(ActionContext),
                    User.EmployeeID,
                    smlValdationPoint,
                    smlValidationType,
                    properties,
                    User);

            var expenseItemFlags = new List<FlagSummary>();

            foreach (var flag in flagSummaries)
            {
                FlagSummary flagSummary = new FlagSummary().From(flag, ActionContext);
                var flagDetails = ActionContext.FlagManagement.GetBy(flag.FlagID);

                flagSummary.FlaggedItem.DisplayFlagImmediately = flagDetails.DisplayFlagImmediately;

                expenseItemFlags.Add(flagSummary);
            }

            return expenseItemFlags;
        }

        /// <summary>
        /// Gets the flag summaries for an expense item
        /// </summary>
        /// <param name="expenseItem">
        /// The expense item
        /// </param>
        /// <param name="employeeId">
        /// The employee Id of the claim owner.
        /// </param>
        /// <returns> 
        /// A list of <see cref="SpendManagementLibrary.Flags.FlagSummary"/>
        /// </returns>
        private List<SpendManagementLibrary.Flags.FlagSummary> GetFlagSummaries(cExpenseItem expenseItem, int employeeId)
        {
            var properties = ActionContext.SubAccounts.getSubAccountById(User.CurrentSubAccountId);

            List<SpendManagementLibrary.Flags.FlagSummary> flags =
                ActionContext.FlagManagement.CheckItemForFlags(
                    expenseItem,
                   employeeId,
                    SpendManagementLibrary.Flags.ValidationPoint.AddExpense,
                    SpendManagementLibrary.Flags.ValidationType.Any,
                    properties,
                    User);

            return flags;
        }

        /// <summary>
        /// Gets the blocking flags for an expense item
        /// </summary>
        /// <param name="flags">The flag summaries</param>
        /// <param name="expenseItem">The expense item</param>
        private void GetBlockedFlags(List<SpendManagementLibrary.Flags.FlagSummary> flags, ref ExpenseItem expenseItem)
        {
            if (ActionContext.FlagManagement.ContainsBlockedItem(flags))
            {
                List<FlagSummary> expenseItemFlags =
                    flags.Select(flag => new FlagSummary().From(flag, ActionContext)).ToList();
                expenseItem.Flags = expenseItemFlags;
                expenseItem.BlockedFlags = new FlaggedItemsManager(true, false, false, true, "ClaimViewer");
                var expenseItemFlagSummary = new ExpenseItemFlagSummary(0, expenseItemFlags);
                expenseItem.BlockedFlags.Add(expenseItemFlagSummary);
            }
        }

        /// <summary>
        /// Update the operator validation progress status .Update the  ExpediteValidationProgress flag in saved expense table
        /// </summary>
        /// <param name="id">Expense Id</param>
        /// <param name="operatorValidationProgress">Expense Validation Progress</param>
        public int UpdateOperatorValidationStatus(int id, int operatorValidationProgress)
        {
            var validationManager = new ExpenseValidationManager(User.AccountID);
            return validationManager.UpdateOperatorProgressForExpenseItem(
                id,
                (ExpediteOperatorValidationProgress)operatorValidationProgress);
        }

        /// <summary>
        /// Deletes an expense item by its Id
        /// </summary>
        /// <param name="expenseId">The expenseId</param>
        public int DeleteExpenseItem(int expenseId)
        {
            cExpenseItem item = GetExpenseItem(expenseId);
            //Checks claim ownership before proceeding
            new ClaimRepository(this.User, this.ActionContext).Get(item.claimid);
            cClaim claim = ActionContext.Claims.getClaimById(item.claimid);

            if (claim.paid == true)
            {
                throw new ApiException(
                    string.Format(ApiResources.ApiErrorExpenseItemCannotDeleteExpenseItem, "Expense Item"),
                    string.Format(ApiResources.ApiErrorDeleteExpenseItemMessage, "Expense Item"));
            }

            return ActionContext.Claims.deleteExpense(claim, item, false, this.User);
        }

        #endregion Non-applicable methods

        /// <summary>
        /// Gets the default definition for an expense item, which is used to build the Add/Edit expense form
        /// </summary>
        /// <returns>A <see cref="ExpenseItemDefinition">ExpenseItemDefinition</see></returns>
        public ExpenseItemDefinition GetExpenseItemDefinition(int expenseId)
        {

            ExpenseItem expenseItem = null;

            expenseItem = expenseId > 0 ? this.Get(expenseId) : new ExpenseItem();

            int accountId = User.AccountID;
            var subAccounts = new cAccountSubAccounts(accountId);
            cAccountProperties subAccountProperties =
                subAccounts.getSubAccountById(User.CurrentSubAccountId).SubAccountProperties;

            OrganisationSettings organisationSettings =
                (OrganisationSettings)FieldSettingFactory<OrganisationSettings>.PopulateFieldSettings(accountId);
            organisationSettings.CanAddNewOrganisation = subAccountProperties.ClaimantsCanAddCompanyLocations;

            bool exchangeRateReadOnly = subAccountProperties.ExchangeReadOnly;

            cCountries countries = new cCountries(User.AccountID, User.CurrentSubAccountId);
            var globalPcaCountries = countries.GetPostcodeAnywhereEnabledCountries();
            var pcaCountries = globalPcaCountries.Select(country => country.Cast<GlobalCountry>()).ToList();

            var defaultGlobalCountryId = countries.getCountryById(subAccountProperties.HomeCountry).GlobalCountryId;

            IFieldSetting reasonSettings = FieldSettingFactory<ReasonSettings>.PopulateFieldSettings(accountId);
            IFieldSetting countrySettings = FieldSettingFactory<CountrySettings>.PopulateFieldSettings(accountId);
            IFieldSetting currencySettings = FieldSettingFactory<CurrencySettings>.PopulateFieldSettings(accountId);
            IFieldSetting otherDetailsSettings = FieldSettingFactory<OtherDetailsSettings>.PopulateFieldSettings(accountId);
            IFieldSetting toSettings = FieldSettingFactory<ToSettings>.PopulateFieldSettings(accountId);
            IFieldSetting fromSettings = FieldSettingFactory<FromSettings>.PopulateFieldSettings(accountId);
            IFieldSetting departmentSettings = FieldSettingFactory<DepartmentSettings>.PopulateFieldSettings(accountId);
            IFieldSetting costCodeSettings = FieldSettingFactory<CostcodeSettings>.PopulateFieldSettings(accountId);
            IFieldSetting projectCodeSettings = FieldSettingFactory<ProjectcodeSettings>.PopulateFieldSettings(accountId);

            var employees = new cEmployees(accountId);
            var employee = employees.GetEmployeeById(User.EmployeeID);

            var defaultCostcodeBreakdowns = employee.GetCostBreakdown().Get().Cast<List<Models.Types.Employees.CostCentreBreakdown>>();

            var generalOptions = this._generalOptionsFactory[this.User.CurrentSubAccountId].WithCurrency().WithCountry();

            var primaryCurrencyId = employee.PrimaryCurrency == 0 ? (int)generalOptions.Currency.BaseCurrency : employee.PrimaryCurrency;
            var primaryCountryId = employee.PrimaryCountry == 0 ? generalOptions.Country.HomeCountry : employee.PrimaryCountry;
            int employeeGlobalPrimaryCountryId = employee.PrimaryCountry == 0 ? generalOptions.Country.HomeCountry : countries.getCountryById(primaryCountryId).GlobalCountryId;

            IList<Models.Types.Currency> activeCurrencies = new List<Models.Types.Currency>();

            if (currencySettings.DisplayForCash || currencySettings.DisplayOnIndividualItem)
            {
                activeCurrencies = new CurrencyRepository(this.User, this.ActionContext).GetActiveCurrencies();
            }

            IList<Country> activeCountries = new List<Country>();

            if (countrySettings.DisplayForCash || countrySettings.DisplayOnIndividualItem)
            {
                activeCountries = new CountryRepository(this.User, this.ActionContext).GetActiveCountries();
            }

            IList<ClaimReason> reasons = new List<ClaimReason>();

            if (reasonSettings.DisplayForCash || reasonSettings.DisplayOnIndividualItem)
            {
                reasons = new ClaimReasonRepository(this.User, this.ActionContext).GetAllUnarchived();
            }

            List<SpendManagementLibrary.Account.BankAccount> employeeBankAccounts =
                this.ActionContext.BankAccountsCurrentUser.GetActiveAccountByEmployeeId(User.EmployeeID);
            List<Models.Types.Employees.BankAccount> bankAccounts = new List<Models.Types.Employees.BankAccount>();

            foreach (var employeeBankAccount in employeeBankAccounts)
            {
                var bank = new Models.Types.Employees.BankAccount
                {
                    AccountName = employeeBankAccount.AccountName,
                    AccountId = employeeBankAccount.BankAccountId
                };

                bankAccounts.Add(bank);
            }

            // Get all acitve UDFs which are not item-specific
            var userDefinedFields = new List<UserDefinedFieldType>();
            var expenseItemGeneralUserDefinedFields = this._expenseItemData.GetExpenseItemDefinitionUDFs(this.User.AccountID);
            if (expenseItemGeneralUserDefinedFields != null)
            {
                foreach (SpendManagementLibrary.UserDefinedFields.UserDefinedFieldValue userDefinedField in expenseItemGeneralUserDefinedFields)
                {
                    var userDefinedFiledValue = (cUserDefinedField)userDefinedField.Value;

                    if (!userDefinedFiledValue.Specific)
                    {
                        var userDefinedFieldType = new UserDefinedFieldType().From(userDefinedFiledValue, this.ActionContext);
                        userDefinedFields.Add(userDefinedFieldType);
                    }
                }
            }

            var expenseItemDefinition = new ExpenseItemDefinition
            {
                ExpenseItem = expenseItem,
                OrganisationSettings = organisationSettings,
                ReasonSettings = reasonSettings,
                CountrySettings = countrySettings,
                CurrencySettings = currencySettings,
                OtherDetailsSettings = otherDetailsSettings,
                ToSettings = toSettings,
                FromSettings = fromSettings,
                DepartmentSettings = departmentSettings,
                CostcodeSettings = costCodeSettings,
                ProjectcodeSettings = projectCodeSettings,
                CostCenterBreakdowns = defaultCostcodeBreakdowns,
                PrimaryCurrencyId = primaryCurrencyId,
                CurrencyList = activeCurrencies,
                PrimaryCountryId = primaryCountryId,
                CountryList = activeCountries,
                ClaimReasons = reasons,
                ExchangeRateReadOnly = exchangeRateReadOnly,
                EmployeeBankAccounts = bankAccounts,
                PostCodeAnywhereCountries = pcaCountries,
                DefaultGlobalCountryId = defaultGlobalCountryId,
                EmployeeGlobalPrimaryCountryId = employeeGlobalPrimaryCountryId,
                CanEditCostCode = this.User.CanEditCostCodes,
                CanEditDepartment = this.User.CanEditDepartments,
                CanEditProjectCode = this.User.CanEditProjectCodes,
                UserDefinedFields = userDefinedFields
            };

            return expenseItemDefinition;
        }


        /// <summary>
        /// Gets the default definition for an expense item for an approver when editing a claimant's expense item
        /// The <see cref="ExpenseItemDefinition"/> returned has the lookup values for Id's returned
        /// </summary>
        /// <param name="expenseId">
        /// The Expense Id the expense belongs to.
        /// </param>
        /// <param name="employeeId">
        /// The Employee Id of the claimant.
        /// </param>
        /// <returns>
        /// A <see cref="ExpenseItemDefinition">ExpenseItemDefinition</see> with lookup values populated
        /// </returns>
        public ExpenseItemDefinition GetExpenseItemDefinitionForClaimantAsApprover(int expenseId, int employeeId)
        {
            var expenseItemRepository = new ExpenseItemRepository(this.User, this.ActionContext);
            expenseItemRepository.ClaimantDataPermissionCheck(expenseId, employeeId);

            var expenseItemDefinition = this.GetExpenseItemDefinition(expenseId);
            var expenseItem = expenseItemDefinition.ExpenseItem;

            var subAccounts = new cAccountSubAccounts(User.AccountID);
            cAccountProperties subAccountProperties =
                subAccounts.getSubAccountById(User.CurrentSubAccountId).SubAccountProperties;

            if (expenseItem.CarId > 0)
            {
                Vehicle vehicle = new VehicleRepository(this.User).Get(expenseItem.CarId);

                if (vehicle != null)
                {
                    expenseItem.VehicleDescription = vehicle.VehicleDescription;
                }
            }

            if (expenseItem.ESRAssignmentId > 0)
            {

                cESRAssignments assignments = new cESRAssignments(User.AccountID, employeeId);
                cESRAssignment assignment = assignments.getAssignmentById((int)expenseItem.ESRAssignmentId);

                string assignmentDescription = assignments.GenerateAssignmentText(assignment, subAccountProperties);
                expenseItem.ESRAssignmentDescription = assignmentDescription;
            }

            if (expenseItem.ExpenseSubCategoryId > 0)
            {
                ExpenseSubCategory expenseSubCat = new ExpenseSubCategoryRepository(this.User, this.ActionContext).Get(expenseItem.ExpenseSubCategoryId);

                if (expenseSubCat != null)
                {
                    expenseItem.ExpenseSubCategoryDescription = expenseSubCat.Description;
                }
            }

            if (expenseItem.FloatId > 0)
            {
                Advance advance = new AdvancesRepository(this.User, this.ActionContext).Get(expenseItem.FloatId);

                if (advance != null)
                {
                    expenseItem.ExpenseSubCategoryDescription = advance.DisplayName;
                }
            }

            if (expenseItem.ToId > 0 || expenseItem.FromId > 0)
            {
                AddressRepository addressRepository = new AddressRepository(this.User, this.ActionContext);

                if (expenseItem.ToId > 0)
                {
                    Address toAddress = addressRepository.Get(expenseItem.ToId);

                    if (toAddress != null)
                    {
                        expenseItem.ToDescription = toAddress.AddressFriendlyText;
                    }
                }

                if (expenseItem.FromId > 0)
                {
                    Address fromAddress = addressRepository.Get(expenseItem.ToId);

                    if (fromAddress != null)
                    {
                        expenseItem.FromDescription = fromAddress.AddressFriendlyText;
                    }
                }
            }

            //Get claimants cost code breakdown
            var employees = new cEmployees(User.AccountID);
            var employee = employees.GetEmployeeById(employeeId);
            var defaultCostcodeBreakdowns = employee.GetCostBreakdown().Get().Cast<List<Models.Types.Employees.CostCentreBreakdown>>();

            expenseItemDefinition.CostCenterBreakdowns = defaultCostcodeBreakdowns;

            if (expenseItem.CostCentreBreakdowns.Count > 0 || expenseItemDefinition.CostCenterBreakdowns.Count > 0)
            {
                var costCodeRepository = new CostCodeRepository(this.User, this.ActionContext);
                var departmentRepository = new DepartmentRepository(this.User, this.ActionContext);
                var projectCodeRepository = new ProjectCodeRepository(this.User);

                var costCentreBreakdownsForExpense = new List<CostCentreBreakdown>();

                if (expenseItem.CostCentreBreakdowns.Count > 0)
                {
                    foreach (var breakdown in expenseItem.CostCentreBreakdowns)
                    {
                        CostCodeBreakdownWithLabelData breakdownLabelData = DetermineCostCodeBreakdownDescriptions(costCodeRepository, departmentRepository, projectCodeRepository, breakdown, subAccountProperties);

                        var costCentreBreakdown = new CostCentreBreakdown
                        {
                            CostCodeId = breakdown.CostCodeId,
                            CostCodeDescription = breakdownLabelData.CostCodeDecription,
                            DepartmentId = breakdown.DepartmentId,
                            DepartmentDescription = breakdownLabelData.DepartmentDecription,
                            ProjectCodeId = breakdown.ProjectCodeId,
                            ProjectCodeDescription = breakdownLabelData.ProjectDecription
                        };

                        costCentreBreakdownsForExpense.Add(costCentreBreakdown);
                    }

                    expenseItem.CostCentreBreakdowns = costCentreBreakdownsForExpense;
                }

                if (expenseItemDefinition.CostCenterBreakdowns.Count > 0)
                {
                    var costCenterEmployeeDefaultBreakdowns = new List<CostCodeBreakdownWithLabelData>();
                    costCenterEmployeeDefaultBreakdowns = GetCostCodeBreakdownDescriptions(expenseItemDefinition.CostCenterBreakdowns.ToList(), costCodeRepository, departmentRepository, projectCodeRepository, costCenterEmployeeDefaultBreakdowns, subAccountProperties);
                    expenseItemDefinition.CostCenterEmployeeDefaultBreakdownWithLabelData = costCenterEmployeeDefaultBreakdowns;
                }
            }

            return expenseItemDefinition;
        }

        /// <summary>
        /// Saves the employee's reason for disputing a returned item
        /// </summary>
        /// <param name="expenseId">The id of the expense item</param>
        /// <param name="reason">The reason the item is being disputed</param>
        /// <param name="isMobileRequest">If the request has come from mobile</param>
        /// <returns>0 if failure or 1 if success</returns>
        public int DisputeExpenseItem(int expenseId, string reason, bool isMobileRequest)
        {
            cExpenseItem item = GetExpenseItem(expenseId);

            if (!item.returned)
            {
                if (!isMobileRequest)
                {
                    throw new ApiException(
                        string.Format(ApiResources.ApiErrorExpenseItemCannotDeleteExpenseItem, "Expense Item"),
                        string.Format(
                            ApiResources.ApiErrorExpenseItemCannotDisputeExpenseItemNotReturned,
                            "Expense Item"));
                }
                else
                {
                    return 0;
                }
            }

            //Checks claim ownership before proceeding
            new ClaimRepository(this.User, this.ActionContext).Get(item.claimid);
            cClaim claim = ActionContext.Claims.getClaimById(item.claimid);

            if (claim.paid)
            {
                if (!isMobileRequest)
                {
                    throw new ApiException(
                        string.Format(ApiResources.ApiErrorExpenseItemCannotDeleteExpenseItem, "Expense Item"),
                        string.Format(
                            ApiResources.ApiErrorExpenseItemCannotDisputeExpenseItemAlreadyPaid,
                            "Expense Item"));
                }
                else
                {
                    return 0;
                }
            }

            ActionContext.Claims.DisputeExpense(claim, item, reason, isMobileRequest);

            return 1;
        }

        /// <summary>
        /// Checks if the current user can acccess the expense item data of a claimant.
        /// </summary>
        /// <param name="expenseId">
        /// The expense id.
        /// </param>
        /// <exception cref="ApiException">
        /// </exception>
        public void ClaimantDataPermissionCheck(int expenseId, int employeeId)
        {
            var expenseItemRepository = new ExpenseItemRepository(this.User, this.ActionContext);
            cExpenseItem item = expenseItemRepository.GetExpenseItem(expenseId);
            cClaim claim = ActionContext.Claims.getClaimById(item.claimid);

            bool isValid = true;

            if (claim.employeeid != employeeId)
            {
                isValid = false;
            }

            if (claim.checkerid != this.User.EmployeeID && item.itemCheckerId != this.User.EmployeeID && claim.employeeid != this.User.EmployeeID)
            {
                isValid = false;
            }

            if (!isValid)
            {
                throw new ApiException(ApiResources.ApiErrorGetClaimantExpenseDataUnSuccessful, ApiResources.ApiErrorNoPermissionExpenseData);
            }
        }

        /// <summary>
        /// Checks if the current user is the owner of the expense item.
        /// </summary>
        /// <param name="expenseId">
        /// The expense Od.
        /// </param>  
        /// <param name="claimRepository">
        /// An instance of <see cref="ClaimRepository"/>
        /// </param>
        public void CheckCurrentUserIsExpenseItemOwner(ExpenseItem expenseItem, ClaimRepository claimRepository)
        {
            Claim claim = claimRepository.Get(expenseItem.ClaimId);

            if (claim.EmployeeId != this.User.EmployeeID)
            {
                throw new ApiException(
                    ApiResources.ApiErrorInsufficientPermission,
                    ApiResources.ApiErrorNotExpenseItemOwner);
            }
        }
    
    /// <summary>
        /// Gets the flag summaries for an expense item
        /// </summary>
        /// <param name="id">
        /// The id of the expense item.
        /// </param>
    /// <param name="employeeId">
    /// The employee Id of the claim owner.
    /// </param>
        /// <returns>
        /// A List of <see cref="SpendManagementLibrary.Flags.FlagSummary">FlagSummary</see>
        /// </returns>
    private List<SpendManagementLibrary.Flags.FlagSummary> GetFlagSummaries(int id, int employeeId)
        {
            cExpenseItem expense = this.GetExpenseItem(id);

            // Get any journey steps for expense so they are correctly checked for mileage related flags
            SortedList<int, cJourneyStep> journeySteps = this.ActionContext.ExpenseItems.GetJourneySteps(id);
            expense.journeysteps = journeySteps;

            List<SpendManagementLibrary.Flags.FlagSummary> flags = this.GetFlagSummaries(expense, employeeId);

            return flags;
        }

        /// <summary>
        /// Processes the expense split item to assign values from its parent
        /// </summary>
        /// <param name="split">
        /// The <see cref="ExpenseItem">split item</see>
        /// </param>
        /// <param name="parentExpenseItem">
        /// The <see cref="ExpenseItem">parent item</see>
        /// </param>
        /// <returns>
        /// The processed <see cref="cExpenseItem">split item</see>
        /// </returns>
        private cExpenseItem ProcessSplitItem(ExpenseItem split, cExpenseItem parentExpenseItem)
        {
            cSubcat subcat = this.ActionContext.SubCategories.GetSubcatById(split.ExpenseSubCategoryId);

            if (subcat.vatapp)
            {
                split.NormalReceipt = parentExpenseItem.normalreceipt;
                split.UserSaysHasReceipts = parentExpenseItem.receipt;
            }

            cExpenseItem splitItem = split.To(this.ActionContext);
            splitItem.setPrimaryItem(parentExpenseItem);

            return splitItem;
        }

        /// <summary>
        /// Checks a <see cref="cExpenseItem">expense item</see> for blocked flags
        /// </summary>
        /// <param name="expenseItem">
        /// The <see cref="cExpenseItem">expense item</see>
        /// </param>
        /// <param name="employeeId">
        /// The employee Id of the claim owner.
        /// </param>
        /// <returns>
        /// The <see cref="FlaggedItemsManager">FlaggedItemsManager</see> with the blocking flag details
        /// </returns>
        private FlaggedItemsManager CheckExpenseItemForBlockedFlags(cExpenseItem expenseItem, int employeeId)
        {
            SpendManagementLibrary.Flags.FlaggedItemsManager blockedFlags = new SpendManagementLibrary.Flags.FlaggedItemsManager(true, false, false, true, "ClaimViewer");
            this.ActionContext.ExpenseItems.CheckItemForBlockedFlags(User.AccountID, employeeId, expenseItem, SpendManagementLibrary.Flags.ValidationType.DoesNotRequireSave, blockedFlags, 0);

            if (blockedFlags.Count == 0)
            {
                return null;
            }

            var blockingFlags = new FlaggedItemsManager();
            var blockingFlagSummaries = new List<ExpenseItemFlagSummary>();

            foreach (SpendManagementLibrary.Flags.ExpenseItemFlagSummary blockedFlag in blockedFlags.List)
            {
                blockingFlagSummaries.Add(new ExpenseItemFlagSummary().From(blockedFlag, ActionContext));
            }

            blockingFlags.ExpenseCollection = blockingFlagSummaries;

            return blockingFlags;
        }

        /// <summary>
        /// Updates the office address/distances for any journey steps, based on the assignment saved against the expense item. 
        /// This is to ensure the correct office address and distances get saved when the office keyword is used in conjunction with an assignment. 
        /// </summary>
        /// <param name="item">
        /// The expense item.
        /// </param>
        /// <param name="accountProperties">
        /// The account properties.
        /// </param>
        /// <returns>
        /// A list of <see cref="JourneyStep"/>s
        /// </returns>
        private List<JourneyStep> UpdateOfficeAddressBasedOnEsrAssignment(ExpenseItem item, cAccountProperties accountProperties)
        {
            SpendManagementLibrary.Addresses.Address work = SpendManagementLibrary.Addresses.Address.GetByReservedKeyword(
                this.User,
                accountProperties.WorkAddressKeyword,
                Convert.ToDateTime(item.Date),
                accountProperties,
                (int?)item.ESRAssignmentId);

            foreach (JourneyStep journey in item.JourneySteps)
            {

                if (journey.StartLocation != null || journey.EndLocation != null)
                {
                Address startAddress = journey.StartLocation;
                Address endAddress = journey.EndLocation;

                bool updateDistance = false;

                if (startAddress.IsOfficeAddress)
                {
                    startAddress = new Address().From(work, this.ActionContext);
                    updateDistance = true;
                }

                if (endAddress.IsOfficeAddress)
                {
                    endAddress = new Address().From(work, this.ActionContext);
                    updateDistance = true;
                }

                if (updateDistance)
                {
                    var addressRecommendedDistanceRepository = new AddressRecommendedDistanceRepository(this.User, this.ActionContext);
                        decimal? distance = addressRecommendedDistanceRepository.GetRecommendedOrCustomDistance(startAddress.Id, endAddress.Id,item.CarId);
                    decimal updatedDistance = 0;

                    if (distance != null)
                    {
                        updatedDistance = (decimal)distance;
                    }

                    journey.NumberOfActualMiles = updatedDistance;
                    journey.NumberOfMiles = updatedDistance;
                    journey.RecordedMiles = updatedDistance;
                }

                journey.StartLocation = startAddress;
                journey.EndLocation = endAddress;
            }
            }

            return item.JourneySteps;
        }

        /// <summary>
        /// Gets descriptions for the project, depatment and costcode that make up the cost code breakdowns
        /// </summary>
        /// <param name="costCentreBreakdowns">
        /// The cost centre breakdowns.
        /// </param>
        /// <param name="costCodeRepository">
        /// An instance of the cost code repository.
        /// </param>
        /// <param name="departmentRepository">
        /// An instance of the department repository.
        /// </param>
        /// <param name="projectCodeRepository">
        /// An instance of the project code repository.
        /// </param>
        /// <param name="list">
        /// An instance of a list of <see cref="DetermineCostCodeBreakdownDescriptions"/>
        /// </param>
        /// <param name="subAccountProperties">
        /// An instance of <see cref="subAccountProperties"/>
        /// </param>
        /// <returns>
        /// An instance of a list of <see cref="DetermineCostCodeBreakdownDescriptions"/> with populated data
        /// </returns>
        private static List<CostCodeBreakdownWithLabelData> GetCostCodeBreakdownDescriptions(
           List<Models.Types.Employees.CostCentreBreakdown> costCentreBreakdowns,
            CostCodeRepository costCodeRepository,
            DepartmentRepository departmentRepository,
            ProjectCodeRepository projectCodeRepository,
            List<CostCodeBreakdownWithLabelData> list,
            cAccountProperties subAccountProperties)
        {
            foreach (var costCentreBreakdown in costCentreBreakdowns)
            {
                var breakdown = DetermineCostCodeBreakdownDescriptions(costCodeRepository, departmentRepository, projectCodeRepository, costCentreBreakdown, subAccountProperties);

                list.Add(breakdown);
            }

            return list;
        }

        /// <summary>
        /// The determines the descriptions for the cost code, project code, department that make up a cost centre
        /// </summary>
        /// <param name="costCodeRepository">
        /// An instance of the cost code repository.
        /// </param>
        /// <param name="departmentRepository">
        /// An instance of the department repository.
        /// </param>
        /// <param name="projectCodeRepository">
        /// An instance of the project code repository.
        /// </param>
        /// <param name="costCentreBreakdown">
        /// The cost centre to determine the descriptions for.
        /// </param>
        /// <param name="subAccountProperties">
        /// An instance of <see cref="subAccountProperties"/>
        /// </param>
        /// <returns>
        /// The <see cref="CostCodeBreakdownWithLabelData"/>.
        /// </returns>
        private static CostCodeBreakdownWithLabelData DetermineCostCodeBreakdownDescriptions(
            CostCodeRepository costCodeRepository,
            DepartmentRepository departmentRepository,
            ProjectCodeRepository projectCodeRepository,
            CostCentreBreakdown costCentreBreakdown,
            cAccountProperties subAccountProperties)
        {
            var breakdown = new CostCodeBreakdownWithLabelData();

            if (costCentreBreakdown.CostCodeId != null && costCentreBreakdown.CostCodeId.Value > 0)
            {
                CostCode costCode = costCodeRepository.Get(costCentreBreakdown.CostCodeId.Value);
                breakdown.CostCodeId = costCentreBreakdown.CostCodeId.Value;
                breakdown.CostCodeDecription = subAccountProperties.UseCostCodeDescription ? costCode.Description : costCode.Label;
            }
            else
            {
                var defaultCostCode = costCodeRepository.GetAllActive().FirstOrDefault();

                if (defaultCostCode != null)
                {
                    breakdown.CostCodeId = defaultCostCode.Id;
                    breakdown.CostCodeDecription = subAccountProperties.UseCostCodeDescription ? defaultCostCode.Description : defaultCostCode.Label;
                }
                else
                {
                    breakdown.CostCodeId = null;
                    breakdown.CostCodeDecription = string.Empty;
                }
            }

            if (costCentreBreakdown.ProjectCodeId != null && costCentreBreakdown.ProjectCodeId.Value > 0)
            {
                ProjectCode projectCode = projectCodeRepository.Get(costCentreBreakdown.ProjectCodeId.Value);
                breakdown.ProjectCodeId = costCentreBreakdown.ProjectCodeId.Value;
                breakdown.ProjectDecription = subAccountProperties.UseProjectCodeDescription ? projectCode.Description : projectCode.Label;
            }
            else
            {
                var defaultProjectCode = projectCodeRepository.GetAllActive().FirstOrDefault();

                if (defaultProjectCode != null)
                {
                    breakdown.ProjectCodeId = defaultProjectCode.Id;
                    breakdown.ProjectDecription = subAccountProperties.UseProjectCodeDescription ? defaultProjectCode.Description : defaultProjectCode.Label;
                }
                else
                {
                    breakdown.ProjectCodeId = null;
                    breakdown.ProjectDecription = string.Empty;
                }
            }

            if (costCentreBreakdown.DepartmentId != null && costCentreBreakdown.DepartmentId.Value > 0)
            {
                Department departmentCode = departmentRepository.Get(costCentreBreakdown.DepartmentId.Value);
                breakdown.DepartmentId = costCentreBreakdown.DepartmentId.Value;
                breakdown.DepartmentDecription = subAccountProperties.UseDepartmentCodeDescription ? departmentCode.Description : departmentCode.Label;
            }
            else
            {
                var defaultDepartment = departmentRepository.GetAllActive().FirstOrDefault();

                if (defaultDepartment != null)
                {
                    breakdown.DepartmentId = defaultDepartment.Id;
                    breakdown.DepartmentDecription = subAccountProperties.UseDepartmentCodeDescription ? defaultDepartment.Description : defaultDepartment.Label;
                }
                else
                {
                    breakdown.DepartmentId = null;
                    breakdown.DepartmentDecription = string.Empty;
                }
            }
            return breakdown;
        }
    }
}


