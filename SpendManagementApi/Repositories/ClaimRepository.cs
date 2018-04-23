namespace SpendManagementApi.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using System.Collections;
    using System.Net.Http;

    using Models.Common;
    using Models.Responses;
    using Models.Types;
    using Models.Types.ClaimSubmission;
    using Utilities;

    using SpendManagementLibrary;
    using SpendManagementLibrary.Helpers;
    using Spend_Management;
    using Interfaces;
    using Spend_Management.expenses.code.Claims;

    using Common.Enums;
    using ClaimDefinitionOutcome = SpendManagementLibrary.Enumerators.ClaimDefinitionOutcome;

    using Claim = Models.Types.Claim;
    using ClaimBasic = Models.Types.ClaimBasic;
    using ClaimStage = Common.Enums.ClaimStage;
    using ClaimSubmissionApi = Models.Types.ClaimSubmission.ClaimSubmissionApi;
    using DisplayField = SpendManagementLibrary.Mobile.DisplayField;
    using Models.Requests;

    using SpendManagementApi.Common.Enum;
    using SpendManagementApi.Models.Types.Flags;

    using SpendManagementLibrary.Claims;
    using SpendManagementLibrary.Employees;
    using SpendManagementLibrary.Enumerators;

    using Spend_Management.expenses.code;

    using ClaimEnvelopeAttachmentResults = SpendManagementLibrary.Expedite.ClaimEnvelopeAttachmentResults;
    using ClaimEnvelopeInfo = SpendManagementLibrary.ClaimEnvelopeInfo;
    using ClaimStatus = SpendManagementApi.Common.Enums.ClaimStatus;
    using ClaimUnsubmittableReason = SpendManagementApi.Common.Enum.ClaimUnsubmittableReason;
    using ExpenseItem = SpendManagementApi.Models.Types.ExpenseItem;
    using MileageFlaggedItem = SpendManagementLibrary.Flags.MileageFlaggedItem;

    /// <summary>
    /// Manages data access for <see cref="Claim">Claims</see>
    /// </summary>
    internal class ClaimRepository : BaseRepository<Claim>, ISupportsActionContext
    {
        private readonly IActionContext _actionContext;

        private const string UdfClaimTableName = "userdefinedClaims";

        /// <summary>
        /// Creates a new ExpenseCategoryRespository.
        /// </summary>
        /// <param name="user">The Current User.</param>
        /// <param name="actionContext">The action context.</param>
        public ClaimRepository(ICurrentUser user, IActionContext actionContext)
            : base(user, x => x.Id, x => x.Name)
        {
            this._actionContext = actionContext;
        }

        /// <summary>
        /// Get all of T
        /// </summary>
        /// <returns></returns>
        public override IList<Claim> GetAll()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gives claim access status for logged in user.
        /// </summary>
        /// <param name="claim">
        /// Claim to validate.
        /// </param>
        /// <param name="checkUserIsClaimOwner">
        /// Check if claim owner is logged in user .
        /// </param>
        private void CheckClaimAndOwnership(cClaim claim, bool checkUserIsClaimOwner = false)
        {
            ClaimToAccessStatus canAccessClaim = this.ActionContext.Claims.CheckClaimAndOwnership(claim, this.User, checkUserIsClaimOwner);
            switch (canAccessClaim)
            {
                case ClaimToAccessStatus.ClaimNotFound:
                    throw new ApiException(string.Format(ApiResources.ApiErrorClaimNotFound, "Claim"), string.Format(ApiResources.ApiErrorGetClaimMessage, "Claim"));
                case ClaimToAccessStatus.InSufficientAccess:
                    throw new ApiException(string.Format(ApiResources.ApiErrorXUnsuccessfulMessage, "Get claim"), ApiResources.ApiErrorInsufficientPermission);
            }
        }

        /// <summary>
        /// Gets a Claim by it's id.
        /// </summary>
        /// <param name="id">The Id</param>
        /// <returns>A Claim.</returns>
        public override Claim Get(int id)
        {
            var claim = this._actionContext.Claims.getClaimById(id);
           
            this.CheckClaimAndOwnership(claim, true);

            return new Claim().From(claim, this._actionContext);
        }

        /// <summary>
        /// Gets a Claim by it's id and audits the view in the audit log.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="Claim"/>.
        /// </returns>
        public Claim GetAndAudit(int id)
        {
            var claim = this.Get(id);

            this._actionContext.Claims.AuditViewClaim(SpendManagementElement.Claims, claim.Name, claim.EmployeeId, this.User);

            return claim;
        }

        /// <summary>
        /// Get claim basic item by claim id
        /// </summary>
        /// <param name="id">Claim id</param>
        /// <returns>Claim basic item</returns>
        public ClaimBasic GetClaimBasic(int id)
        {
            var claim = this._actionContext.Claims.GetClaimBasicById(id, this.ActionContext.AccountId, this.User.EmployeeID);
            this._actionContext.Claims.AuditViewClaim(SpendManagementElement.Claims, claim.ClaimName, claim.EmployeeId, this.User);
            var claimBasic = new ClaimBasic().From(claim, this.ActionContext);
            var currencyRepository = new CurrencyRepository(this.User, this._actionContext);
            claimBasic.CurrencyLabel = currencyRepository.GetGlobalCurrencyFromCurrencyId(claimBasic.BaseCurrency).Label;

            return claimBasic;
        }

        /// <summary>
        /// Gets a claim by it's reference number.
        /// </summary>
        /// <param name="referenceNumber">The Claim Reference Number (CRN).</param>
        /// <returns>A Claim.</returns>
        public Claim GetByReferenceNumber(string referenceNumber)
        {
            var claim = this._actionContext.Claims.GetClaimByReferenceNumber(referenceNumber);

            this._actionContext.Claims.AuditViewClaim(SpendManagementElement.Claims, claim.name, claim.employeeid, this.User);
            

            this.CheckClaimAndOwnership(claim, true);

            return new Claim().From(claim, this._actionContext);
        }

        /// <summary>
        /// Gets the number of claims for the specified employee at the specified stage in the system.
        /// </summary>
 
        /// <param name="employeeId">The employee Id (optional)</param>
        /// <param name="claimStage">The claim stage</param>
        /// <returns>The number of claims for the specified employee and claim stage.</returns>
        public int GetCount(int employeeId, ClaimStage claimStage)
        {
            return this._actionContext.Claims.getCount(employeeId, (SpendManagementLibrary.ClaimStage)claimStage);
        }

        /// <summary>
        /// Gets the total number of claims in the system.
        /// </summary>
        /// <returns>The number of claims in the system.</returns>
        public int GetCount()
        {
            return this._actionContext.Claims.getCount();
        }

        /// <summary>
        /// Builds up the claim counts for claims at the stages of Current, Submitted, and Previous for the current employee.
        /// </summary>
        /// <returns>The <see cref="ClaimCountForAllStages">ClaimCountForAllStages></see></returns>
        public ClaimCountForAllStages GetClaimCountForAllStages()
        {
            var claimCounts = new ClaimCountForAllStages
            {
                CurrentClaimsCount = this.GetCount(User.EmployeeID, ClaimStage.Current),
                SubmittedClaimsCount = this.GetCount(User.EmployeeID, ClaimStage.Submitted),
                PreviousClaimsCount = this.GetCount(User.EmployeeID, ClaimStage.Previous)
            };

            return claimCounts;
        }

        /// <summary>
        /// Gets the default claim number for an emplyee with a specified claim stage.
        /// </summary>
        /// <returns>The default claim number.</returns>
        public int GetDefaultClaim(int employeeId)
        {
            return this._actionContext.Claims.getDefaultClaim(SpendManagementLibrary.ClaimStage.Current, employeeId);
        }

        /// <summary>
        /// Adds a default claim for an employee.
        /// </summary>
        /// <returns>The default claim number id.</returns>
        public int AddDefaultClaim(int employeeId)
        {
            return this._actionContext.Claims.insertDefaultClaim(employeeId);
        }

        /// <summary>
        /// Gets unsubmitted claims for a given employee id.
        /// </summary>
        /// <param name="employeeId">The employee id</param>
        /// <returns>A list of <see cref="Models.Types.ClaimBasic">ClaimBasic</see></returns>
        public List<ClaimBasic> GetUnsubmittedClaims(int employeeId)
        {
            var smlClaims = new List<SpendManagementLibrary.ClaimBasic>();
            var claims = this._actionContext.Claims.GetUnsubmittedClaims(employeeId, smlClaims, this.ActionContext.SignoffGroups, this.ActionContext.Employees, this.ActionContext.SubAccounts);
            var currencyRepository = new CurrencyRepository(this.User, this._actionContext);
            var unsubmittedClaims = new List<ClaimBasic>();

            foreach (ClaimBasic claimBasic in claims.Select(claim => new ClaimBasic().From(claim, this._actionContext)))
            {
                claimBasic.CurrencyLabel = currencyRepository.GetGlobalCurrencyFromCurrencyId(claimBasic.BaseCurrency).Label;
                unsubmittedClaims.Add(claimBasic);
            }

            return unsubmittedClaims;
        }

        /// <summary>
        /// Gets submitted claims for a given employee id.
        /// </summary>
        /// <param name="employeeId">The employee id</param>
        /// <returns>A list of <see cref="Models.Types.ClaimBasic">ClaimBasic</see></returns>
        public List<ClaimBasic> GetSubmittedClaims(int employeeId)
        {
            var smlClaims = new List<SpendManagementLibrary.ClaimBasic>();
            var claims = this._actionContext.Claims.GetSubmittedClaims(employeeId, smlClaims, this.ActionContext.SignoffGroups, this.ActionContext.Employees, this.ActionContext.SubAccounts);
            var currencyRepository = new CurrencyRepository(this.User, this._actionContext);
            var submittedClaims = new List<ClaimBasic>();

            foreach (ClaimBasic claimBasic in claims.Select(claim => new ClaimBasic().From(claim, this._actionContext)))
            {
                claimBasic.CurrencyLabel = currencyRepository.GetGlobalCurrencyFromCurrencyId(claimBasic.BaseCurrency).Label;
                submittedClaims.Add(claimBasic);
            }

            return submittedClaims;
        }

        /// <summary>
        /// Gets previous claims for a given employee id.
        /// </summary>
        /// <param name="employeeId">The employee id</param>
        /// <returns>A list of <see cref="Models.Types.ClaimBasic">ClaimBasic</see></returns>
        public List<ClaimBasic> GetPreviousClaims(int employeeId)
        {
            var smlClaims = new List<SpendManagementLibrary.ClaimBasic>();
            var claims = this._actionContext.Claims.GetPreviousClaims(employeeId, smlClaims, this.ActionContext.SignoffGroups, this.ActionContext.Employees, this.ActionContext.SubAccounts);
            var currencyRepository = new CurrencyRepository(this.User, this._actionContext);
            var previousClaims = new List<ClaimBasic>();

            foreach (ClaimBasic claimBasic in claims.Select(claim => new ClaimBasic().From(claim, this._actionContext)))
            {
                claimBasic.CurrencyLabel = currencyRepository.GetGlobalCurrencyFromCurrencyId(claimBasic.BaseCurrency).Label;
                previousClaims.Add(claimBasic);
            }

            return previousClaims;
        }

        /// <summary>
        /// Deletes a claim given a claim id.
        /// </summary>
        /// <param name="claimId">The claim id</param>
        public void DeleteClaim(int claimId)
        {
            var claim = this._actionContext.Claims.getClaimById(claimId);

            this.CheckClaimAndOwnership(claim, true);

            try
            {
                this._actionContext.Claims.DeleteClaim(claim);
            }
            catch (Exception)
            {
                throw new ApiException(ApiResources.ApiErrorGeneralError, ApiResources.ApiErrorDeleteUnsuccessful);
            }

        }

        /// <summary>
        /// Creates a claim on the system for the user.
        /// </summary>
        /// <param name="claimDefinition">Basic claim details including user defined field values. See <see cref="Models.Types.ClaimDefinitionResponse">ClaimDefinition</see> for details on the parameter.</param>
        /// <param name="isMobileRequest">Wether the request is from mobile</param>
        /// <returns>The <see cref="Claim">Claim</see> for the created claim</returns>
        public ClaimBasic CreateClaim(Models.Types.ClaimDefinition claimDefinition, bool isMobileRequest)
        {

            var claimBasic = new ClaimBasic();

            if (claimDefinition.Id < 0)
            {
                if (!isMobileRequest)
                {
                    throw new ApiException(string.Format(ApiResources.ApiErrorCreateClaimIdNonZeroMessage, "Claim"), string.Format(ApiResources.ApiErrorCreateClaimMessage, "Claim"));
                }
                else
                {
                    claimBasic.ClaimBasicOutcome = ClaimBasicOutcome.NewClaimIdMustBeZero;
                    return claimBasic;
                }
            }

            if (SingleClaimCheck())
            {
                if (!isMobileRequest)
                {
                    throw new ApiException(string.Format(ApiResources.ApiErrorCreateUpdateClaimAccountSingleClaim), string.Format(ApiResources.ApiErrorCreateClaimMessage, "Claim"));
                }
                else
                {
                    claimBasic.ClaimBasicOutcome = ClaimBasicOutcome.SingleClaimOnly;
                    return claimBasic;
                }
            }

            claimDefinition.UserDefinedFields = UdfValidator.Validate(claimDefinition.UserDefinedFields, this._actionContext, UdfClaimTableName);

            var udfList = ProcessUDFData(claimDefinition.UserDefinedFields);

            int newClaimId = this._actionContext.ClaimSubmission.addClaim(
                this._actionContext.EmployeeId,
                claimDefinition.Name,
                claimDefinition.Description,
                udfList);
       
            if (newClaimId == -1)
            {
                claimBasic.ClaimBasicOutcome = ClaimBasicOutcome.DuplicateClaimName;
            }
            else
            {             
                var claim = ActionContext.Claims.GetClaimBasicById(newClaimId, User.AccountID, User.EmployeeID);
                claimBasic = new ClaimBasic().From(claim, this._actionContext);
                claimBasic.ClaimBasicOutcome = ClaimBasicOutcome.Success;
            }

            return claimBasic;
        }

        /// <summary>
        /// Updates basic claim details for the user, including user defined field values.
        /// </summary>
        /// <param name="claimDefinition">Basic claim details including user defined field values. See <see cref="Models.Types.ClaimDefinitionResponse">ClaimDefinition</see> for details on the parameter.</param>
        /// <param name="isMobileRequest">Wether the request is from mobile</param>
        /// <returns>The <see cref="Claim">Claim</see> for the updated claim</returns>
        public ClaimBasic UpdateClaim(Models.Types.ClaimDefinition claimDefinition, bool isMobileRequest)
        {
            if (!isMobileRequest)
            {
                if (claimDefinition.Id <= 0)
                {
                    throw new ApiException(string.Format(ApiResources.ApiErrorClaimNotFound, "Claim"), string.Format(ApiResources.ApiErrorUpdateClaimMessage, "Claim"));
                }        
            }

            var claim = this._actionContext.Claims.getClaimById(claimDefinition.Id);
            this.CheckClaimAndOwnership(claim, true);

            claimDefinition.UserDefinedFields = UdfValidator.Validate(claimDefinition.UserDefinedFields, this._actionContext, UdfClaimTableName);

            var udfList = ProcessUDFData(claimDefinition.UserDefinedFields);

            int returnValue = this._actionContext.Claims.updateClaim(claimDefinition.Id,
                claimDefinition.Name,
                claimDefinition.Description,
                udfList,
                this._actionContext.EmployeeId);


            var updatdeClaim = ActionContext.Claims.GetClaimBasicById(claimDefinition.Id, User.AccountID, User.EmployeeID);
            var claimBasic = new ClaimBasic().From(updatdeClaim, this._actionContext);

            if (returnValue == -1)
            {
                claimBasic.ClaimBasicOutcome = ClaimBasicOutcome.DuplicateClaimName;
            }
           
            return claimBasic;
        }

        private static SortedList<int, object> ProcessUDFData(List<UserDefinedFieldValue> udfValues)
        {
            var udfList = new SortedList<int, object>();

            foreach (var udf in udfValues)
            {
                udfList.Add(udf.Id, udf.Value);
            }
            return udfList;
        }

        /// <summary>
        /// Gets the fields for current user account.
        /// </summary>
        /// <returns>
        /// <see cref="List">List of available fields</see>.
        /// </returns>
        public List<DisplayField> GetFields()
        {
            var misc = new cMisc(this.User.AccountID);
            var screenFields = misc.GetAddScreenFields();
            var list = new List<DisplayField>();

            foreach (var keyValuePairScreenFieldValue in screenFields)
            {
                switch (keyValuePairScreenFieldValue.Key)
                {
                    case "reason":
                    case "currency":
                    case "from":
                    case "to":
                    case "otherdetails":
                        var displayField = new DisplayField
                        {
                            code = keyValuePairScreenFieldValue.Value.code,
                            description = keyValuePairScreenFieldValue.Value.description,
                            display = keyValuePairScreenFieldValue.Value.display,
                            individualItem = keyValuePairScreenFieldValue.Value.individual
                        };

                        list.Add(displayField);
                        break;
                }
            }

            return list;
        }

        /// <summary>
        /// Gets the basic expense item detials for a claim Id
        /// </summary>
        /// <param name="claimId">
        /// The claim id.
        /// </param>
        /// <returns>
        /// The a list of <see cref="ClaimExpenseOverview">ClaimExpenseOverview</see>
        /// </returns>
        /// <exception cref="ApiException">
        /// </exception>
        public List<ClaimExpenseOverview> GetClaimExpenseItemsOverview(int claimId)
        {
            var returnList = new List<ClaimExpenseOverview>();

            var claim = this._actionContext.Claims.getClaimById(claimId);

            this._actionContext.Claims.AuditViewClaim(SpendManagementElement.Claims, claim.name, claim.employeeid, this.User);

            if (claim == null)
            {
                throw new ApiException(string.Format(ApiResources.ApiErrorClaimNotFound, "Claim"), string.Format(ApiResources.ApiErrorCreateClaimMessage, "Claim"));
            }

            var subCategories = this._actionContext.SubCategories;
            SortedList<int, cExpenseItem> expenseItems = this._actionContext.Claims.getExpenseItemsFromDB(claimId);

            var expItems = new cExpenseItems(User.AccountID);
            foreach (var item in expenseItems.Values)
            {
                if (claim.employeeid == User.EmployeeID || claim.checkerid == User.EmployeeID
                    || (item.itemCheckerId.HasValue && item.itemCheckerId.Value == User.EmployeeID))

                {
                    var claimExpenseItem = new ClaimExpenseOverview();

                    if (item.subcatid > 0)
                    {
                        var subcats = subCategories.GetSubcatById(item.subcatid);
                        claimExpenseItem.SubCatDescription = subcats.subcat;

                        var expenseIds = new List<int> { item.expenseid };

                        item.flags = expItems.GetFlaggedItems(expenseIds).First;
                        item.journeysteps = expItems.GetJourneySteps(item.expenseid);

                        var expenseItem = new Models.Types.ExpenseItem().From(item, _actionContext);

                        claimExpenseItem.Id = expenseItem.Id;
                        claimExpenseItem.JourneySteps = expenseItem.JourneySteps;
                        claimExpenseItem.Flags = expenseItem.Flags;
                        claimExpenseItem.BaseCurrency = expenseItem.BaseCurrency;
                        claimExpenseItem.CurrencySymbol = expenseItem.CurrencySymbol;
                        claimExpenseItem.Date = expenseItem.Date;
                        claimExpenseItem.Returned = expenseItem.Returned;
                        claimExpenseItem.GlobalTotal = expenseItem.GlobalTotal;
                        claimExpenseItem.GrandTotalVat = expenseItem.GrandTotalVAT;
                        claimExpenseItem.HasSplitItems = expenseItem.SplitItems.Count > 0;

                        returnList.Add(claimExpenseItem);
                    }
                }
            }

            return returnList;
        }

        /// <summary>
        /// The get claim expense items by claimId.
        /// </summary>
        /// <param name="claimId">
        /// The claim id.
        /// </param>
        /// <returns>A <see cref="ClaimExpenseItemsResponse">ClaimExpenseItemsResponse</see></returns>
        public List<ClaimExpenseItem> GetClaimExpenseItems(int claimId)
        {
            var returnList = new List<ClaimExpenseItem>();

            try
            {
                var claim = this._actionContext.Claims.getClaimById(claimId);

                this._actionContext.Claims.AuditViewClaim(SpendManagementElement.Claims, claim.name, claim.employeeid, this.User);

                if (claim == null)
                {
                    throw new ApiException(string.Format(ApiResources.ApiErrorClaimNotFound, "Claim"), string.Format(ApiResources.ApiErrorCreateClaimMessage, "Claim"));
                }

                cEmployeeCorporateCards cards = new cEmployeeCorporateCards(this.User.AccountID);
                SortedList<int, cEmployeeCorporateCard> employeeCards = cards.GetEmployeeCorporateCards(claim.employeeid);
                bool enableCorporateCards = employeeCards != null && employeeCards.Count > 0;

                var subCategories = this._actionContext.SubCategories;
                SortedList<int, cExpenseItem> expenseItems = this._actionContext.Claims.getExpenseItemsFromDB(claimId);

                var expItems = new cExpenseItems(User.AccountID);
                foreach (var item in expenseItems.Values)
                {
                    if (claim.employeeid == User.EmployeeID || claim.checkerid == User.EmployeeID
                        || (item.itemCheckerId.HasValue && item.itemCheckerId.Value == User.EmployeeID))
                    {
                        var claimExpenseItem = new ClaimExpenseItem();

                        if (item.subcatid > 0)
                        {
                            var subcats = subCategories.GetSubcatById(item.subcatid);
                            var expenseSubCategory = subcats.Cast<ExpenseSubCategory>(
                                null,
                                User.AccountID,
                                _actionContext);

                            var expenseIds = new List<int> { item.expenseid };
                            item.flags = expItems.GetFlaggedItems(expenseIds).First;
                            item.journeysteps = expItems.GetJourneySteps(item.expenseid);
                            item.costcodebreakdown = expItems.getCostCodeBreakdown(item.expenseid);

                            if (!enableCorporateCards)
                            {
                                item.transactionid = 0;
                            }
                             
                            var expenseItem = new Models.Types.ExpenseItem().From(
                                item,
                                _actionContext);
                            claimExpenseItem.ExpenseItem = expenseItem;
                            if (expenseItem.ClaimReasonId > 0)
                            {
                                claimExpenseItem.ExpenseItem.ClaimReasonInfo = new ClaimReasonRepository(User, ActionContext).Get(expenseItem.ClaimReasonId).Description;
                            }

                            claimExpenseItem.Subcat = expenseSubCategory;

                            var currenciesRepository = new CurrencyRepository(this.User, this.ActionContext);
                            claimExpenseItem.ExpenseItem.CurrencySymbol = currenciesRepository.DetermineCurrencySymbol(item.currencyid);

                            if (expenseItem.CompanyId > 0)
                            {
                                Organisation company = new OrganisationRepository(this.User, this.ActionContext).Get(expenseItem.CompanyId);

                                if (company != null)
                                {
                                    claimExpenseItem.ExpenseItem.CompanyName = company.Label;
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
                        }

                        returnList.Add(claimExpenseItem);
                    }
                }
            }
            catch (Exception)
            {
                throw new ApiException(
                    string.Format(string.Format(ApiResources.ApiErrorClaimNotFound, "ExpenseItems"), "Expense Items"),
                    string.Format(ApiResources.ApiErrorGetClaimMessage, "Expense Items"));
            }

            return returnList;
        }


        /// <summary>
        /// Generates the GetApprover HTML, which is used to allow users to select who the claim should go to in the next stage.
        /// </summary>
        /// <param name="total">The claim total</param>
        /// <param name="employeeId">The employee identification number</param>
        /// <returns> A string of HTML data, detailing possible approvers the claim can go to.</returns>   
        public string GetApprover(decimal total, int employeeId)
        {
            var employees = new cEmployees(User.AccountID);
            var employee = employees.GetEmployeeById(employeeId);
            var groups = new cGroups(User.AccountID);

            cGroup group = groups.GetGroupById(employee.SignOffGroupID);
            SortedList stages = groups.sortStages(group);
            var stage = (cStage)stages.GetByIndex(0);

            return this._actionContext.Claims.GetApprover(total, stage, employeeId, employee.AccountID, employee.DefaultSubAccount);
        }

        /// <summary>
        /// Saves envelope information against a claim
        /// </summary>
        /// <param name="claimId">The id of the claim the envelope data is being saved against</param>
        /// <param name="claimEnvelopes">The envelope data</param>
        /// <returns>The <see cref="ClaimEnvelopeAttachmentResults">ClaimEnvelopeAttachmentResults</see></returns>
        public Models.Responses.ClaimEnvelopeAttachmentResults SaveEnvelopeInformationAgainstAClaim(int claimId, List<Models.Requests.ClaimEnvelopeInfo> claimEnvelopes)
        {
            //check claim ownership before proceeding
            this.Get(claimId);

            List<ClaimEnvelopeInfo> list = ConvertToSMLClaimEnvelopeInfoList(claimEnvelopes);
            ClaimEnvelopeAttachmentResults claim = this._actionContext.Claims.SaveClaimEnvelopeNumbers(list, claimId ,User.AccountID);
            var result = new Models.Responses.ClaimEnvelopeAttachmentResults().From(claim, this._actionContext);

            return result;
        }

        /// <summary>
        /// The updates the claim history.
        /// </summary>
        /// <param name="claim">
        /// The claim.
        /// </param>
        /// <param name="comment">
        /// The comment.
        /// </param>
        /// <param name="employeeId">
        /// The employeeId of the employee updating the claim history.
        /// </param>
        public void UpdateClaimHistory(cClaim claim, string comment, int employeeId)
        {
           ActionContext.Claims.UpdateClaimHistory(claim, comment, employeeId);    
        }

        /// <summary>
        /// Get the number of claims for check and pay for a given employee.
        /// </summary>
        /// <param name="accountId">The account id.</param>
        /// <param name="employeeId">The employee id.</param>
        /// <returns>The number of claims to check.</returns>
        internal int GetClaimsToCheckCount(int accountId, int employeeId)
        {
            int count;

            try
            {
                if (!cAccessRoles.CanCheckAndPay(accountId, employeeId))
                {
                    throw new ApiException(ApiResources.ApiErrorInsufficientPermission, string.Format(ApiResources.ApiErrorCheckAndPay, "Employee"));
                }

                count = this._actionContext.Claims.getClaimsToCheckCount(employeeId, false, null);
            }
            catch (Exception)
            {
                throw new ApiException(string.Format(ApiResources.ApiErrorClaimNotFound, "Claims"), string.Format(ApiResources.ApiErrorGetClaimMessage, "Awaiting Approval Count"));
            }

            return count;
        }

        /// <summary>
        /// Allocates a claim to a person in a signoff group.
        /// </summary>
        /// <param name="accountId">The account id</param>
        /// <param name="employeeId">The employee id</param>
        /// <param name="claimId">The claim id</param>
        internal void Allocate(int accountId, int employeeId, int claimId)
        {
            try
            {
                if (cAccessRoles.CanCheckAndPay(accountId, employeeId))
                {
                    cClaim claim = this._actionContext.Claims.getClaimById(claimId);

                    if (claim == null)
                    {
                        throw new ApiException(string.Format(ApiResources.ApiErrorClaimNotFound, "Claim"), string.Format(ApiResources.ApiErrorCreateClaimMessage, "Claim"));
                    }

                    if (this._actionContext.Claims.AllowClaimProgression(claimId) == 2)
                    {
                        this._actionContext.Claims.payClaim(claim, employeeId, null);
                    }
                    else
                    {
                        throw new ApiException(ApiResources.ApiErrorGeneralError, string.Format(ApiResources.ApiErrorXUnsuccessfulMessage, "Allocate"));
                    }
                }
                else
                {
                    throw new ApiException(ApiResources.ApiErrorInsufficientPermission, string.Format(ApiResources.ApiErrorCheckAndPay, "Employee"));
                }
            }
            catch (Exception)
            {
                throw new ApiException(ApiResources.ApiErrorGeneralError, string.Format(ApiResources.ApiErrorXUnsuccessfulMessage, "Allocate"));
            }
        }

        /// <summary>
        /// Gets a list of claims awaiting for approval by the employee .
        /// </summary>
        /// <param name="accountId">The account id</param>
        /// <param name="employeeId">The employee id</param>
        /// <returns>A list of <see cref="ClaimBasic">ClaimBasic</see></returns>
        internal List<ClaimBasic> GetClaimsWaitingApproval(int accountId, int employeeId)
        {
            var claimsAwaitingApproval = new List<ClaimBasic>();

            try
            {
                if (!cAccessRoles.CanCheckAndPay(accountId, employeeId))
                {
                    throw new ApiException(
                        ApiResources.ApiErrorInsufficientPermission,
                        string.Format(ApiResources.ApiErrorCheckAndPay, "Employee"));
                }

                claimsAwaitingApproval = this.GetClaimsAllocatedForApproval(accountId, employeeId);

                return claimsAwaitingApproval;
            }
            catch (Exception)
            {
                throw new ApiException(string.Format(ApiResources.ApiErrorClaimNotFound, "Claims"), string.Format(ApiResources.ApiErrorGetClaimMessage, "Claims Awaiting Approval"));
            }
        }
  
        /// <summary>
        /// Gets a list of claims awaiting for approval by the employee. Those that are assigned and unassigned.
        /// </summary>
        /// <param name="accountId">The account id</param>
        /// <param name="employeeId">The employee id</param>
        /// <returns>A list of <see cref="ClaimBasic">ClaimBasic</see></returns>
        internal List<ClaimBasic> GetClaimsWaitingApprovalAssignedAndUnassigned(int accountId, int employeeId)
        {
            var claimsAwaitingApproval = new List<ClaimBasic>();
            var unassignedClaims = new List<ClaimBasic>();

            try
            {            
                Employee employee = this.ActionContext.Employees.GetEmployeeById(employeeId);

                if (!employee.AdminOverride && !cAccessRoles.CanCheckAndPay(accountId, employeeId))
                {
                    throw new ApiException(
                        ApiResources.ApiErrorInsufficientPermission,
                        string.Format(ApiResources.ApiErrorCheckAndPay, "Employee"));
                }

                claimsAwaitingApproval = this.GetClaimsAllocatedForApproval(accountId, employeeId);
                unassignedClaims = this.GetClaimsForApprovalUnassigned(accountId, employeeId);

                return claimsAwaitingApproval.Concat(unassignedClaims).ToList();

            }
            catch (Exception)
            {
                throw new ApiException(string.Format(ApiResources.ApiErrorClaimNotFound, "Claims"), string.Format(ApiResources.ApiErrorGetClaimMessage, "Claims Awaiting Approval"));
            }
        }

        /// <summary>
        /// The get claims allocated for approval with the current approver.
        /// </summary>
        /// <param name="accountId">
        /// The account id.
        /// </param>
        /// <param name="employeeId">
        /// The employee id.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        private List<ClaimBasic> GetClaimsAllocatedForApproval(int accountId, int employeeId)
        {
            var smlClaims = new List<SpendManagementLibrary.ClaimBasic>();
            var claims = cClaims.GetClaimsListAwaitingApproval(accountId, employeeId, smlClaims, this.ActionContext.SignoffGroups, this.ActionContext.Employees, this.ActionContext.SubAccounts);
            var currencyRepository = new CurrencyRepository(this.User, this._actionContext);
            var currencyId = 0;
            var currencyLabel = string.Empty;
            var claimsAwaitingApproval = new List<ClaimBasic>();

            foreach (ClaimBasic claimBasic in claims.Select(claim => new ClaimBasic().From(claim, this._actionContext)))
            {
                this._actionContext.Claims.AuditViewClaim(SpendManagementElement.CheckAndPay, claimBasic.ClaimName, claimBasic.EmployeeId, this.User);

                if (currencyId != claimBasic.BaseCurrency)
                {
                    currencyLabel = currencyRepository.GetGlobalCurrencyFromCurrencyId(claimBasic.BaseCurrency).Label;
                }

                claimBasic.CurrencyLabel = currencyLabel;
                currencyId = claimBasic.BaseCurrency;
                claimsAwaitingApproval.Add(claimBasic);
            }

            return claimsAwaitingApproval;
        }


        /// <summary>
        /// The get the claims unassigned claims available to the current approver.
        /// </summary>
        /// <param name="accountId">
        /// The account id.
        /// </param>
        /// <param name="employeeId">
        /// The employee id.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        private List<ClaimBasic> GetClaimsForApprovalUnassigned(int accountId, int employeeId)
        {
            var claimBasics = new List<SpendManagementLibrary.ClaimBasic>();
            var connection = new DatabaseConnection(cAccounts.getConnectionString(accountId));

            var claims = cClaims.GetClaimsListAwaitingApprovalUnassigned(accountId, employeeId, claimBasics, this.ActionContext.SignoffGroups, this.ActionContext.Employees, connection, this.ActionContext.SubAccounts, this.ActionContext.Teams);
            var currencyRepository = new CurrencyRepository(this.User, this._actionContext);
            var currencyId = 0;
            var currencyLabel = string.Empty;
            var claimsAwaitingApproval = new List<ClaimBasic>();

            foreach (ClaimBasic claimBasic in claims.Select(claim => new ClaimBasic().From(claim, this._actionContext)))
            {
                if (currencyId != claimBasic.BaseCurrency)
                {
                    currencyLabel = currencyRepository.GetGlobalCurrencyFromCurrencyId(claimBasic.BaseCurrency).Label;
                }

                claimBasic.CurrencyLabel = currencyLabel;
                currencyId = claimBasic.BaseCurrency;
                claimsAwaitingApproval.Add(claimBasic);
            }

            return claimsAwaitingApproval;
        }

        /// <summary>
        /// Checks that the flags associated against the expense items relating to the claim Id have justifications from the authoriser, if mandatory 
        /// </summary>
        /// <param name="claimId">
        /// The claim Id.
        /// </param>
        /// <returns>
        /// A list of expense item ids that require justifications from the approver before submitting
        /// </returns>
        internal List<int> CheckJustificationsHaveBeenProvidedByAuthoriser(int claimId)
        {        
            if (!cAccessRoles.CanCheckAndPay(User.AccountID, User.EmployeeID))
            {
                throw new ApiException(ApiResources.ApiErrorInsufficientPermission, string.Format(ApiResources.ApiErrorCheckAndPay, "Employee"));
            }

            cClaim claim = this._actionContext.Claims.getClaimById(claimId);

            if (claim == null)
            {
                throw new ApiException(
                    string.Format(ApiResources.ApiErrorClaimNotFound, "Claim"),
                    string.Format(ApiResources.ApiErrorCreateClaimMessage, "Claim"));
            }

            List<int> expenseIds = new List<int>();
            FlagManagement flagMan = new FlagManagement(User.AccountID);

            var flagResults = flagMan.CheckJustificationsHaveBeenProvidedByAuthoriser(claimId);

            if (flagResults.Count > 0)
            {
                expenseIds.AddRange(flagResults.List.Select(FlaggedItem => FlaggedItem.ExpenseID));
            }

            return expenseIds;
        }

        /// <summary>
        /// Approves a claim.
        /// </summary>
        /// <param name="accountId">The account id</param>
        /// <param name="employeeId">The employee id</param>
        /// <param name="claimId">The claim id</param>
        internal ClaimSubmissionResult ApproveClaim(int accountId, int employeeId, int claimId)
        {        
                if (cAccessRoles.CanCheckAndPay(accountId, employeeId))
                {
                    cClaim claim = this._actionContext.Claims.getClaimById(claimId);

                if (claim == null)
                {
                    throw new ApiException(string.Format(ApiResources.ApiErrorClaimNotFound, "Claim"), string.Format(ApiResources.ApiErrorCreateClaimMessage, "Claim"));
                }

                this._actionContext.Claims.AuditViewClaim(SpendManagementElement.CheckAndPay, claim.name, claim.employeeid, this.User);

                if (this._actionContext.Claims.AllowClaimProgression(claimId) == 1)
                    {                 
          
                     var claimSubmission = new ClaimSubmission(accountId, employeeId);
                     SpendManagementLibrary.Claims.SubmitClaimResult approveClaimResult = claimSubmission.SendClaimToNextStage(claim, false, 0, employeeId, null);        
                     Models.Types.SubmitClaimResult submitClaimResult = new Models.Types.SubmitClaimResult().From(approveClaimResult, ActionContext);
                     return new ClaimSubmissionResult() { ClaimSubmitRejectionReason = this.GetSubmitRejectionReason(submitClaimResult), ClaimId = claimId, SubmitClaimResult = submitClaimResult };
           
                    }
                    else
                    {
                        throw new ApiException(ApiResources.ApiErrorGeneralError, string.Format(ApiResources.ApiErrorXUnsuccessfulMessage, "Approve"));
                    }
                }
                else
                {
                    throw new ApiException(ApiResources.ApiErrorInsufficientPermission, string.Format(ApiResources.ApiErrorCheckAndPay, "Employee"));
                }                     
        }

        /// <summary>
        /// Assigns the claims related to the provided claimIds to the current user.
        /// </summary>
        /// <param name="claimIds">
        /// The claim Ids to allocate to the current user.
        /// </param>
        /// <returns>
        /// A list of <see cref="AssignClaim">AssignClaim</see>
        /// </returns>
        internal List<AssignClaim> AssignClaimsForApproval(List<int> claimIds)
        {
            var assignedClaims = new List<AllocateClaimResult>();
       
            foreach (var claimId in claimIds)
            {
                var claim = this._actionContext.Claims.getClaimById(claimId);
                this.CheckClaimAndOwnership(claim, true);
                assignedClaims.Add(this.ActionContext.Claims.AllocateClaim(claimId, User.EmployeeID));
            }
          
            return assignedClaims.Select(claim => new AssignClaim().ToApiType(claim, this.ActionContext)).ToList();
        }

        /// <summary>
        /// Unassigns the claim related to the provided claimId from the current user
        /// </summary>
        /// <param name="claimId">
        /// The claim Id to unassign from the current user
        /// </param>
        /// <returns>
        /// The <see cref="int"/> outcome of the action, whereby 1 is succcess, and 0 is fail
        /// </returns>
        internal int UnassignClaimForApproval(int claimId)
        {
            var claim = this._actionContext.Claims.getClaimById(claimId);
            this.CheckClaimAndOwnership(claim, true);

            try
            {
                this.ActionContext.Claims.unallocateClaim(claimId, this.User.EmployeeID);
            }
            catch (Exception)
            {
                return 0;
            }
         
            return 1;
        }

        /// <summary>
        /// Gets Claim Submission details by its Id.
        /// </summary>
        /// <param name="claimId">The claim id to get the submission details for.</param>
        /// <returns>
        /// ClaimSubmission details <see cref="ClaimSubmissionApi"></see>.
        /// </returns>
        /// <exception cref="ApiException">
        /// </exception>
        internal ClaimSubmissionApi ClaimSubmissionDetails(int claimId)
        {
            SpendManagementLibrary.Claims.SubmitClaimResult claimCanBeSubmittedResult = this._actionContext.Claims.DetermineIfClaimCanBeSubmitted(this.User.AccountID, claimId, this.User.EmployeeID, 0);
            if (claimCanBeSubmittedResult == null)
            {
                throw new ApiException(string.Format(ApiResources.ApiErrorClaimNotFound, "Claim Submission Details"), string.Format(ApiResources.ApiErrorGetClaimMessage, "Claim Submission Details"));
            }

            Models.Types.SubmitClaimResult result = new Models.Types.SubmitClaimResult().From(claimCanBeSubmittedResult, ActionContext);

            string reason = this.GetSubmitRejectionReason(result);

            if (string.IsNullOrWhiteSpace(reason) == false)
            {
                return new ClaimSubmissionApi() { SubmitClaimResult = claimCanBeSubmittedResult, ClaimSubmitRejectionReason = reason };
            }

            var claimSubmissionDetails = _actionContext.Claims.GetClaimSubmissionDetailsForApi(this.User, claimId);
            if (claimSubmissionDetails == null)
            {
                throw new ApiException(string.Format(ApiResources.ApiErrorClaimNotFound, "Claim Submission Details"), string.Format(ApiResources.ApiErrorGetClaimMessage, "Claim Submission Details"));
            }

            return claimSubmissionDetails.Cast<ClaimSubmissionApi>(reason, claimCanBeSubmittedResult);
        }

        /// <summary>
        /// Converts a list of <see cref="Models.Requests.ClaimEnvelopeInfo">ClaimEnvelopeInformation </see> to
        /// a list of SML type <see cref="ClaimEnvelopeInfo">ClaimEnvelopeInfo</see>
        /// </summary>
        /// <param name="claimEnvelopes">
        /// The claim envelopes.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        private static List<ClaimEnvelopeInfo> ConvertToSMLClaimEnvelopeInfoList(List<Models.Requests.ClaimEnvelopeInfo> claimEnvelopes)
        {
            var claimEnvelopeInformationList = new List<ClaimEnvelopeInfo>();

            foreach (var claimEnvelope in claimEnvelopes)
            {
                var claimEnvelopeInfo = new ClaimEnvelopeInfo
                {
                    ClaimEnvelopeId = claimEnvelope.ClaimEnvelopeId,
                    EnvelopeNumber = claimEnvelope.EnvelopeNumber,
                    ExcessCharge = claimEnvelope.ExcessCharge,
                    PhysicalState = claimEnvelope.PhysicalState,
                    ProcessedAfterMarkedLost = claimEnvelope.ProcessedAfterMarkedLost
                };

                claimEnvelopeInformationList.Add(claimEnvelopeInfo);
            }

            return claimEnvelopeInformationList;
        }

        /// <summary>
        /// Get submit rejection reason.
        /// </summary>
        /// <param name="submitRejectionReason">
        /// Claim submit rejection reason.
        /// </param>
        /// <returns>
        /// <see cref="string"/>.
        /// </returns>
        private string GetSubmitRejectionReason(Models.Types.SubmitClaimResult submitRejectionReason)
        {
            switch (submitRejectionReason.Reason.GetHashCode())
            {
                case 0:
                    return string.Empty;
                case 1: //NoItems
                    return "A claim must contain at least one item if you wish to submit it for approval.";
                case 2: //NoSignoffGroup
                    return "You cannot submit a claim because you have not been allocated a signoff group.";
                case 3: //DelegatesProhibited
                    return "Delegates cannot submit claims.";

                case 4: //CannotSignoffOwnClaim
                    return "You cannot submit this claim because your current signoff group would allow you to sign off your own claim, please contact your administrator to have this corrected.";

                case 5: //OutstandingFlags
                    return "Unfortunately you cannot submit your claim as one or more items have been blocked or require you to provide further justification.";

                case 6: //MinimumAmountNotReached
                    return string.Format("You cannot submit this claim because the total claim amount is below {0}.", submitRejectionReason.MinimumAmount);

                case 7: //MaximumAmountNotReached
                    return string.Format("You cannot submit this claim because the total claim amount is above {0}.", submitRejectionReason.MaximumAmount);

                case 8: //No Line Manager
                    return "You cannot submit this claim because you have not been allocated a line manager.";

                case 9: //ApproverOnHoliday
                    return "This approver is currently set as “on holiday”, this may mean that your claim will not be processed until they return. Alternatively, you may cancel and change your selected approver. Would you like continue and send your claim to this person anyway?";

                case 10: //FrequencyLimitBreached
                    var frequencyMessage = string.Format("This claim cannot be submitted because you cannot submit more than {0} claim(s) per ", submitRejectionReason.FrequencyValue);
                    switch (submitRejectionReason.FrequencyPeriod)
                    {
                        case 1:
                            frequencyMessage += "month";
                            break;
                        case 2:
                            frequencyMessage += "week";
                            break;
                    }

                    frequencyMessage += '.';
                    return frequencyMessage;

                case 11: //AssignmentSupervisorNotSpecified
                    return "This claim cannot currently be submitted as one or more assignment numbers associated to claim items do not have a supervisor specified.";

                case 12: //CostcodeOwnerNotSpecified
                    return "This claim cannot currently be submitted due to either one or more expense item cost code owners missing, no default cost code owner defined or no line manager defined for claimant. Next stage cannot be determined.";

                case 13:
                    return "This claim cannot be submitted because there are credit card items that have not been reconciled. Please reconcile these items and try again.";

                case 14:
                    return "This claim cannot be submitted because there are credit card items that have not been matched. Please match these items and try again.";

                case 15:
                    return "This claim cannot be submitted as the claimant would be able to approve their own claim.";

                case 16:
                    return "The claim cannot advance to the next stage as the claimant would be able to approve their own claim.";

                case 17:
                    return "The claim cannot be approved as you are not the approver of the claim";

                case 22:
                    return "A claim cannot be approved while there are still items waiting approval.";

                case 23:
                    return "The claim cannot be submitted as the name you have entered already exists.";

                case 25: //AssignmentSupervisorNotSpecified
                    return "This claim cannot currently be approved as one or more assignment numbers associated to claim items do not have a supervisor specified.";

                case 26: //CostcodeOwnerNotSpecified
                    return "This claim cannot currently be approved due to either one or more expense item cost code owners missing, no default cost code owner defined or no line manager defined for claimant. Next stage cannot be determined.";

                case 27:
                    return "You cannot submit this claim because it has already been submitted.";                    
                default:
                    return string.Empty;
            }
        }

        /// <summary>
        /// Get unsubmit rejection reason.
        /// </summary>
        /// <param name="unsubmitRejectionReason">
        /// Claim unsubmit rejection reason.
        /// </param>
        /// <returns>
        /// <see cref="string"/>
        /// </returns>
        private string GetUnsubmitRejectionReason(UnsubmitClaimResult unsubmitRejectionReason)
        {
            switch (unsubmitRejectionReason.UnsubmitReason.GetHashCode())
            {
                case 0:
                    return string.Empty;
                case 1:
                    return string.Empty;
                case -1:
                    return "This claim cannot be unsubmitted as there are other approvers at this stage that have approved items on the claim.";
                case -2:
                    return "This claim cannot be unsubmitted because it has already been approved.";
                case -3:
                    return "This claim cannot be unsubmitted as there are other approvers at this stage that have not returned all of their items on the claim.";
                case -4:
                    return "This claim cannot be unsubmitted as items on the claim have already been paid.";
                case -5:
                    return "This claim cannot be unsubmitted as it has already started the approval process.";
                case -6:
                    return "This claim cannot be unsubmitted as it has either passed or is currently in an Expedite Stage - Scan & Attach or Validation.";
                case -7:
                    return "This claim cannot be unsubmitted as it has been escalated to you by someone else.";
                default:
                    return string.Empty;
            }
        }

        /// <summary>
        /// Save claimant justifications.
        /// </summary>
        /// <param name="justifications">
        /// Claimant justifications.
        /// </param>
        internal void SaveClaimantJustifications(List<FlagJustification> justifications)
        {
            foreach (var justification in justifications)
            {
                CheckFlagJustification(justification, "Save Claimant Justification");
                this._actionContext.Claims.SaveClaimantJustification(justification.FlaggedItemId,justification.Justification);
            }
        }

        /// <summary>
        /// Save approver justifications.
        /// </summary>
        /// <param name="justifications">
        /// approver justifications.
        /// </param>
        internal void SaveApproverJustifications(List<FlagJustification> justifications)
        {
            foreach (var justification in justifications)
            {
                CheckFlagJustification(justification,"Save Approver Justification");
                this._actionContext.Claims.SaveApproverJustification(justification.FlaggedItemId, justification.Justification);
            }
        }

        private void CheckFlagJustification(FlagJustification justification, string action)
        {
            //check claim ownership
            var claim = this.Get(justification.ClaimId);


            //check expense Id belongs to claim expense items
            ExpenseItem item = null;
            List<int> expenseIds = claim.ExpenseItems;

            var claimContainsExpenseId = expenseIds.Contains(justification.ExpenseId);
            var expenseRepository = new ExpenseItemRepository(User, this.ActionContext);

            if (!claimContainsExpenseId)
            {
                //expense id is not in parent expenses, so check split items for the expense id
                var parentExpenseId =  ActionContext.Claims.getParentId(justification.ExpenseId);
                ExpenseItem parentExpense = expenseRepository.Get(parentExpenseId);

                item = parentExpense.SplitItems.Find(x => x.Id == justification.ExpenseId);                          
            }
            else
            {
                item = expenseRepository.Get(justification.ExpenseId);
            }

            if (item == null)
            {
                throw new ApiException(string.Format(ApiResources.ApiErrorXUnsuccessfulMessage, action), string.Format(ApiResources.ApiErrorExpenseItemDoesNotBelongToClaim));
            }

            //Does the flagId belong to the expenseItem flags
            SpendManagementLibrary.Flags.FlaggedItemsManager flagSummaries = ActionContext.ExpenseItems.GetFlaggedItems(new List<int> { item.Id });

            if (flagSummaries == null)
            {
                throw new ApiException(string.Format(ApiResources.ApiErrorXUnsuccessfulMessage, action), string.Format(ApiResources.ApiErrorExpenseItemNoFlagsFound));

            }

            List<int> flaggedItemIds = new List<int>();

            foreach (var flagSummary in flagSummaries.First)
            {
                if (flagSummary.FlaggedItem.Flagtype == SpendManagementLibrary.Flags.FlagType.MileageExceeded
                    || flagSummary.FlaggedItem.Flagtype == SpendManagementLibrary.Flags.FlagType.NumberOfPassengersLimit
                    || flagSummary.FlaggedItem.Flagtype == SpendManagementLibrary.Flags.FlagType.HomeToLocationGreater)
                {
                    MileageFlaggedItem mileageFlag = (MileageFlaggedItem)flagSummary.FlaggedItem;

                    flaggedItemIds.AddRange(mileageFlag.FlaggedJourneySteps.Select(step => step.FlaggedItemID));
                }
                else
                {
                    flaggedItemIds.Add(flagSummary.FlaggedItem.FlaggedItemId);
                }
            }

            if (!flaggedItemIds.Contains(justification.FlaggedItemId))
            {
                throw new ApiException(string.Format(ApiResources.ApiErrorXUnsuccessfulMessage, action), string.Format(ApiResources.ApiErrorFlagIsNotAssociatedWithExpense));
            }
        }
          
        /// <summary>
        /// Checks if a claim can be submitted
        /// </summary>
        /// <param name="claimId">The Id of the claim</param>
        /// <returns>The<see cref="ProcessClaimSubmissionResult">ClaimSubmissionResult with the outcome of the checks</see>/></returns>
        internal ClaimSubmissionResult DetermineifClaimCanBeSubmitted(int claimId)
        {
            var claim = this._actionContext.Claims.getClaimById(claimId);
            this.CheckClaimAndOwnership(claim, true);

            SpendManagementLibrary.Claims.SubmitClaimResult result = this._actionContext.Claims.DetermineIfClaimCanBeSubmitted(this.User.AccountID, claimId, this.User.EmployeeID, new byte());

            return ProcessClaimSubmissionResult(claimId, claim.status,result); 
        }

        /// <summary>
        /// Checks if the claim can be unsubmitted
        /// </summary>
        /// <param name="claimId">The Id of the claim</param>
        /// <returns>The <see cref="UnsubmitClaimResult">Unsubmit Claim Result with the outcome of the checks</see></returns>
        internal UnsubmitClaimResult DetermineIfClaimCanBeUnsubmitted(int claimId)
        {
            var claim = this._actionContext.Claims.getClaimById(claimId);

            this.CheckClaimAndOwnership(claim, true);

            var claimUnsubmittableReason = this._actionContext.Claims.DetermineIfClaimCanBeUnsubmitted(claimId);

            UnsubmitClaimResult unsubmitClaimResponse = new UnsubmitClaimResult();

            unsubmitClaimResponse.UnsubmitReason = (ClaimUnsubmittableReason)claimUnsubmittableReason;
            unsubmitClaimResponse.UnsubmitRejectionReason = this.GetUnsubmitRejectionReason(unsubmitClaimResponse);

            return unsubmitClaimResponse;
        }

        /// <summary>
        ///  Submit the claim. Checks that the user can submit the claim before updating.
        /// </summary>
        /// <param name="claimId">
        /// Claim id to submit.
        /// </param>
        /// <param name="claimName">
        /// Claim name to update.
        /// </param>
        /// <param name="description">
        /// Claim description to update.
        /// </param>
        /// <param name="cash">
        /// True if a cach claim
        /// </param>
        /// <param name="credit">
        /// True if credit claim.
        /// </param>
        /// <param name="purchase">
        /// True if purchase claim.
        /// </param>
        /// <param name="approver">
        /// Claim approver id to update.
        /// </param>
        /// <param name="odometerReadings">
        /// Odometer readings.
        /// </param>
        /// <param name="businessMileage">
        /// Business mileage.
        /// </param>
        /// <param name="ignoreApproverOnHoliday">
        /// Ignore approver on holiday if true.
        /// </param>
        /// <param name="viewfilter">
        /// Viewfilter.
        /// </param>
        /// <param name="continueAlthoughAuthoriserIsOnHoliday">
        /// Continue although authoriser is on holiday if true.
        /// </param>
        /// <returns>
        /// The <see cref="ClaimSubmissionResult"></see>.
        /// </returns>
        internal ClaimSubmissionResult SubmitClaim(int claimId,
            string claimName,
            string description,
            bool cash,
            bool credit,
            bool purchase,
            int? approver,
            List<List<object>> odometerReadings,
            bool businessMileage,
            bool ignoreApproverOnHoliday,
            byte viewfilter,
            bool continueAlthoughAuthoriserIsOnHoliday = false)
        {
            SpendManagementLibrary.Claims.SubmitClaimResult result = this._actionContext.Claims.SubmitClaim(
                claimId,
                claimName,
                description,
                cash,
                credit,
                purchase,
                approver,
                odometerReadings,
                businessMileage,
                ignoreApproverOnHoliday,
                viewfilter,
                continueAlthoughAuthoriserIsOnHoliday);

            var claim = this._actionContext.Claims.getClaimById(claimId);

            return ProcessClaimSubmissionResult(claimId, claim.status, result);
        }

        /// <summary>
        /// Unsubmit the claim. Checks that the user can unsubmit the claim before updating.
        /// </summary>
        /// <param name="claimId">ID of claim to unsubmit.</param>
        /// <returns>The <see cref="UnsubmitClaimResult"/></returns>
        internal UnsubmitClaimResult UnsubmitClaim(int claimId)
        {
            var claim = this._actionContext.Claims.getClaimById(claimId);

            this.CheckClaimAndOwnership(claim, true);

            var claimUnsubmittableReason = this._actionContext.Claims.UnSubmitclaim(claim, false, User.EmployeeID, null);

            UnsubmitClaimResult unsubmitClaimResponse = new UnsubmitClaimResult();

            unsubmitClaimResponse.UnsubmitReason = (ClaimUnsubmittableReason)claimUnsubmittableReason;
            unsubmitClaimResponse.UnsubmitRejectionReason = this.GetUnsubmitRejectionReason(unsubmitClaimResponse);

            return unsubmitClaimResponse;
        }

        /// <summary>
        /// Get claim history.
        /// </summary>
        /// <param name="claimId">Id of the claim</param>
        /// <returns></returns>
        internal GetClaimHistoryResult GetClaimHistory(int claimId)
        {
            GetClaimHistoryResult claimHistoryResponse = new GetClaimHistoryResult();

            claimHistoryResponse.ClaimHistory = this._actionContext.Claims.GetClaimHistory(claimId);

            return claimHistoryResponse;
        }

        /// <summary>
        /// Processes the Claim Submission result to return a ClaimSubmissionResult type
        /// </summary>
        /// <param name="claimId">
        /// The claimId
        /// </param>
        /// <param name="claimStatus">
        /// The claim Status.
        /// </param>
        /// <param name="result">
        /// The result of checking if a claim can be submitted or submitting a claim
        /// </param>
        /// <returns>
        /// The <see cref="ClaimSubmissionResult">ClaimSubmissionResult type</see>
        /// </returns>
        private ClaimSubmissionResult ProcessClaimSubmissionResult(int claimId, SpendManagementLibrary.ClaimStatus claimStatus, SpendManagementLibrary.Claims.SubmitClaimResult result)
        {
            if (result == null)
            {
                throw new ApiException(ApiResources.ApiErrorClaimSubmission,
                    ApiResources.ApiErrorClaimSubmissionUnsuccessfulMessage);
            }

            Models.Types.SubmitClaimResult submitClaimResult = new Models.Types.SubmitClaimResult().From(result, ActionContext);

            var determineifClaimCanBeSubmitted = new ClaimSubmissionResult()
            {
                ClaimSubmitRejectionReason = this.GetSubmitRejectionReason(submitClaimResult),
                ClaimStatus = (ClaimStatus)claimStatus,
                ClaimId = claimId,
                SubmitClaimResult = submitClaimResult
            };
            return determineifClaimCanBeSubmitted; ;
        }

        /// <summary>
        /// Gets a claim definition via the claim id
        /// </summary>
        /// <param name="id">The claim id</param>
        /// <returns>A <see cref="Models.Types.ClaimDefinitionResponse">ClaimDefinition</see></returns>
        internal Models.Types.ClaimDefinitionResponse GetClaimDefinition(int id)
        {
            var claim = this._actionContext.Claims.getClaimById(id);
            this._actionContext.Claims.AuditViewClaim(SpendManagementElement.Claims, claim.name, claim.employeeid, this.User);
            this.CheckClaimAndOwnership(claim, true);
            var claimDefinition = this._actionContext.Claims.GetClaimDefinition(id);
            claimDefinition.ClaimDefinitionOutcome = claimDefinition.ClaimId > 0 ? ClaimDefinitionOutcome.Success : ClaimDefinitionOutcome.Fail;
            Models.Types.ClaimDefinitionResponse Claim = new Models.Types.ClaimDefinitionResponse().From(claimDefinition, this._actionContext);

            var table = new cTables(User.AccountID);
            cTable userdefinedClaimsTable = table.GetTableByID(new Guid("f70d6e0d-8e38-4a1d-a681-cc9d310c2ae9"));
            var fields = new cFields(User.AccountID);
            SortedList<int, object> userdefinedList = ActionContext.UserDefinedFields.GetRecord(userdefinedClaimsTable, id, table, fields);

            // Get the Value for the UDF and popualte the Value property
            foreach (Models.Types.UserDefinedFieldType field in Claim.UserDefinedFields)
            {
                object value;
                userdefinedList.TryGetValue(field.UserDefinedFieldId, out value);
                field.Value = value;
            }

            return Claim;
        }

        /// <summary>
        /// Returns the determined claim name and UDF details for a new claim.
        /// </summary>
        /// <returns>The <see cref="Models.Types.ClaimDefinitionResponse">ClaimDefinition</see>/></returns>
        internal Models.Types.ClaimDefinitionResponse GetDefaultClaimDefinition()
        {
            var claimDefinition = new SpendManagementLibrary.ClaimDefinition();

            if (SingleClaimCheck())
            {
                 claimDefinition.ClaimDefinitionOutcome = ClaimDefinitionOutcome.SingleClaimOnly;             
            }
            else
            {
                string newClaimName = this.ActionContext.Claims.GenerateNewClaimName(User);
                List<SpendManagementLibrary.UserDefinedFields.UserDefinedFieldValue> claimUDFs = this.ActionContext.Claims.GetClaimDefinitionUDFs();
                claimDefinition = new SpendManagementLibrary.ClaimDefinition { Name = newClaimName,UserDefinedFields = claimUDFs, ClaimDefinitionOutcome = ClaimDefinitionOutcome.Success};
            }
                 
            return new Models.Types.ClaimDefinitionResponse().From(claimDefinition, this._actionContext);
        }

        /// <summary>
        /// Returns the <see cref="FlaggedItemsManager">FlaggedItemManager</see> for the expenses flags grid
        /// </summary>
        /// <param name="claimId">The claimId</param>
        /// <param name="expenseIds"></param>
        /// <param name="pageSource"></param>
        /// <returns>The <see cref="FlaggedItemsManager">FlaggedItemsManager</see></returns>
        internal FlaggedItemsManager GetFlagsGrid(int claimId, List<int> expenseIds, string pageSource)
        {
            //checks claim ownership
            var claim = this.Get(claimId);
            
            //check expenseIds belong to claim. 
            new ExpenseItemRepository(this.User, this.ActionContext).ExpenseItemsBelongToClaim(claimId, expenseIds, "Get Flags Grid");
     
            SpendManagementLibrary.Flags.FlaggedItemsManager flagManagement = _actionContext.FlagManagement.CreateFlagsGrid(claimId, expenseIds, pageSource, User.EmployeeID);
            FlaggedItemsManager flaggedItemsManager = new FlaggedItemsManager().From(flagManagement, ActionContext);
            var expenseItemFlagSummaries = flagManagement.List.Select(r => new ExpenseItemFlagSummary().From(r, ActionContext)).ToList();
            flaggedItemsManager.ExpenseCollection = expenseItemFlagSummaries;

            return flaggedItemsManager;
        }

        private bool SingleClaimCheck()
        {
            var subAccounts = new cAccountSubAccounts(User.AccountID);
            cAccountProperties subAccountProperties = subAccounts.getSubAccountById(User.CurrentSubAccountId).SubAccountProperties;

            if (subAccountProperties.SingleClaim)
            {
                if (ActionContext.Claims.getCount(User.EmployeeID, SpendManagementLibrary.ClaimStage.Current) > 1)
                {
                    return true;
                }
            }

            return false;
        }
    }
}