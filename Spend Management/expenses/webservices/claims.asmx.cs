namespace Spend_Management
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Web.Script.Services;
    using System.Web.Services;

    using SpendManagementLibrary;
    using SpendManagementLibrary.Enumerators;
    using SpendManagementLibrary.Enumerators.Expedite;
    using SpendManagementLibrary.Expedite;
    using SpendManagementLibrary.Expedite.DTO;
    using SpendManagementLibrary.Helpers;
    using SpendManagementLibrary.ExpenseItems;
    using expenses.code.Claims;

    using System.Data;
    using System.Linq;
    using System.Security.Permissions;
    using System.Web.UI.WebControls;

    using SpendManagementLibrary.Claims;
    using SpendManagementLibrary.Employees;

    using System.Text;
    using System.Collections;

    using SpendManagementLibrary.SalesForceApi;

    using expenses.code;

    using SpendManagementLibrary.Flags;
    using SpendManagementLibrary.Helpers;
    using SpendManagementLibrary.Mileage;

    /// <summary>
    /// Summary description for claims
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    [ScriptService]
    public class claims : WebService
    {
        /// <summary>
        /// Gets recent claims for a user.
        /// </summary>
        /// <param name="employeeid">The Id of the employee.</param>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public TreeDTO GetRecentClaimsForEmployee(int employeeid)
        {
            var user = cMisc.GetCurrentUser();
            var claims = new cClaims(user.AccountID);
            var claimIds = claims.getClaimIds(employeeid, true);
            var claimTree = new TreeDTO { Children = new List<EnvelopeManagementDTONode>() };

            claimIds.Reverse();
            claimIds.ToList().ForEach(
                id =>
            {
                var claim = claims.getClaimById(id);

                // is more than a year old, ignore it
                if (claim.modifiedon.AddYears(-1) > DateTime.UtcNow.AddYears(-1))
                {
                            claimTree.Children.Add(
                                new LeafDTO
                    {
                        id = claim.claimid.ToString(),
                                        text =
                                            string.Format(
                                                "{0}, {1} item{2}, total: {3}, date: {4}",
                                                claim.ReferenceNumber == null
                                                    ? claim.name
                                                    : claim.name + " (" + claim.ReferenceNumber + ")",
                                                claim.NumberOfItems,
                                                claim.NumberOfItems == 1 ? string.Empty : "s",
                                                claim.Total.ToString("C"),
                                                claim.datesubmitted.ToShortDateString()),
                                        iconUrl =
                                            GlobalVariables.StaticContentLibrary + "/icons/16/plain/folder2.png"
                    });
                }
            });

            return claimTree;
        }

        /// <summary>
        /// Gets a tree of receipts for one claim.
        /// </summary>
        /// <param name="claimId">The Id of the claim.</param>
        /// <param name="fromClaimSelector">Whether the page making the call has it's fromClaimSelector property set.</param>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public ReceiptManagementClaim GetClaimReceiptTree(int claimId, bool fromClaimSelector)
        {
            var user = cMisc.GetCurrentUser();
            var claims = new cClaims(user.AccountID);
            var claim = claims.getClaimById(claimId);
            var expenseItems = claims.getExpenseItemsFromDB(claim.claimid).Values.Where(x =>  !x.Edited).ToList();
            var receipts = new SpendManagementLibrary.Expedite.Receipts(user.AccountID, user.EmployeeID);
            var subcats = new cSubcats(user.AccountID);
            var fetchedSubcats = new List<cSubcat>();
            var account = new cAccounts().GetAccountByID(user.AccountID);
            var allExpensesHaveReceiptsAttached =
                receipts.CheckIfAllValidatableClaimLinesHaveReceiptsAttached(claim.claimid);
            var signoffStages = claims.GetSignoffStagesAsTypes(claim);
            var allEnvelopesComplete = true;
            var userIsClaimsCurrentApprover = claims.IsUserClaimsCurrentApprover(
                user,
                claim,
                fromClaimSelector,
                expenseItems);

            if (!string.IsNullOrEmpty(claim.ReferenceNumber))
            {
                int completed, total;
                allEnvelopesComplete = new Envelopes().AreAllEnvelopesCompleteForClaim(
                    claim.ReferenceNumber,
                    out total,
                    out completed);
            }

            bool preventDelete;
            string reason = string.Empty;
            
            if (user.EmployeeID == claim.employeeid)
            {
                preventDelete =
                    !claims.CanUserDeleteReceiptsForCurrentClaim(
                        out reason,
                        account,
                        signoffStages,
                        user,
                        claim,
                        null,
                        allEnvelopesComplete,
                        allExpensesHaveReceiptsAttached,
                        fromClaimSelector);
            }
            else
            {
                preventDelete = fromClaimSelector || !userIsClaimsCurrentApprover;
                reason = preventDelete
                             ? "Only the claim's approver can modify receipts."
                             : "Drag me to move or copy me.";
            }

            var clscurrencies = new cCurrencies(user.AccountID, user.CurrentSubAccountId);
            var reqcurrency = clscurrencies.getCurrencyById(claim.currencyid);
            string symbol;

            if (reqcurrency != null)
            {
                var clsglobalcurrencies = new cGlobalCurrencies();
                symbol = clsglobalcurrencies.getGlobalCurrencyById(reqcurrency.globalcurrencyid).symbol;
            }
            else
            {
                symbol = "£";
            }

            var output = new ReceiptManagementClaim(claim, symbol);
            var header = new ReceiptManagementHeader
            {
                Name = "Header",
                                 Children =
                                     new List<ReceiptManagementReceipt>(
                                     receipts.GetByClaim(claim.claimid)
                                     .Select(
                                         x =>
                                         new ReceiptManagementReceipt(
                                             x,
                                             preventDelete,
                                             reason,
                                             receipts.CheckIfReceiptIsImageAndOverwriteUrl(x)))
                                     .ToList())
            };
            
            // first child is always the header
            output.Header = header;
  
            // now do the expenses
            
            expenseItems.ForEach(
                e =>
            {
                // grab subcat
                var subcat = fetchedSubcats.FirstOrDefault(x => x.subcatid == e.subcatid);
                if (subcat == null)
                {
                    subcat = subcats.GetSubcatById(e.subcatid);
                    fetchedSubcats.Add(subcat);
                }

                // can it be deleted?
                        preventDelete = (user.EmployeeID != claim.employeeid)
                                            ? fromClaimSelector || !userIsClaimsCurrentApprover
                                            : !claims.CanUserDeleteReceiptsForCurrentClaim(
                                                out reason,
                                                account,
                                                signoffStages,
                                                user,
                                                claim,
                                                e,
                                                allEnvelopesComplete,
                                                allExpensesHaveReceiptsAttached,
                                                fromClaimSelector);
                
                // create branch
                var expense = new ReceiptManagementExpense(e, subcat, preventDelete, reason, symbol);

                // receipts for expense
                var receiptsForExpense = receipts.GetByClaimLine(e, user, subcat, claim).ToList();
                        receiptsForExpense.ForEach(
                            receipt =>
                            expense.Children.Add(
                                new ReceiptManagementReceipt(
                                receipt,
                                preventDelete,
                                reason,
                                receipts.CheckIfReceiptIsImageAndOverwriteUrl(receipt))));

                // add expense to claim
                output.Children.Add(expense);
            });

            return output;
        }

        /// <summary>
        /// Updates a tree of claims sent to (and returned by) the Receipt Management page, from GetEmployeeClaimTree.
        /// </summary>
        /// <param name="tree">The updated json tree from the Receipt Management page.</param>
        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void UpdateClaimReceiptTree(ReceiptManagementClaim tree)
        {
            // check permissions
            var user = cMisc.GetCurrentUser();
            var claims = new cClaims(user.AccountID);
            var claim = claims.getClaimById(tree.Id);

            // check the claim exists
            if (claim == null)
            {
                throw new Exception("This claim doesn't exist.");
            }

            // either the user owns the claim, or they are the approver
            if (user.EmployeeID != claim.employeeid
                && (claim.ClaimStage == ClaimStage.Submitted && user.EmployeeID != claim.checkerid))
            {
                throw new Exception("You don't have the rights to modify receipts for this claim.");
            }
            
            var receipts = new SpendManagementLibrary.Expedite.Receipts(user.AccountID, user.EmployeeID);
            var fetchedReceipts = new List<Receipt>();
            var expenseIdsToUpdate = new List<int>();
            Receipt receipt;

            // record the parent Id of each receipt attached to an expense
            tree.Children.ForEach(expense => expense.Children.ForEach(r => r.ParentId = expense.Id));

            // move all the receipts to the header now they have a ParentId, so we only need one loop
            tree.Header.Children.AddRange(tree.Children.SelectMany(expense => expense.Children));
            
            // flatten the claim tree's receipts (all from header into new list)
            var flattened = new List<ReceiptManagementReceipt>();
            tree.Header.Children.ForEach(
                linked =>
            {
                // get the actual receipt
                        receipt = fetchedReceipts.FirstOrDefault(r => r.ReceiptId == linked.Id)
                                  ?? receipts.GetById(linked.Id);

                // save in case needed again
                if (!fetchedReceipts.Contains(receipt))
                {
                    fetchedReceipts.Add(receipt);
                }
                
                // add to flat list
                flattened.Add(linked);
            });

            // first go through any deleted / unlinked receipts
            tree.DeletedReceipts.ForEach(
                deleted =>
            {
                // get receipts on this claim header
                        receipt = fetchedReceipts.FirstOrDefault(r => r.ReceiptId == deleted.Id)
                                  ?? receipts.GetById(deleted.Id);
                
                // save in case needed later
                if (!fetchedReceipts.Contains(receipt))
                {
                    fetchedReceipts.Add(receipt);
                }

                // check whether it was a line or header removal
                if (deleted.ParentId.HasValue)
                {
                    // remove from claim line if it exists
                    if (receipt.OwnershipInfo.ClaimLines.Contains(deleted.ParentId.Value))
                    {
                        receipt.OwnershipInfo.ClaimLines.Remove(deleted.ParentId.Value);
                        receipts.UnlinkFromClaimLine(receipt.ReceiptId, deleted.ParentId.Value);
                        
                        // record the expense to update
                        if (!expenseIdsToUpdate.Contains(deleted.ParentId.Value))
                        {
                            expenseIdsToUpdate.Add(deleted.ParentId.Value);
                        }
                    }
                }
                else
                {
                    // remove from the header if it was set
                    if (receipt.OwnershipInfo.ClaimId == claim.claimid)
                    {
                        // link it to the user instead (any claimlines will remain linked).
                        receipt.OwnershipInfo.ClaimId = null; 
                        receipt.OwnershipInfo.EmployeeId = claim.employeeid;
                        receipts.LinkToClaimant(receipt.ReceiptId, claim.employeeid);
                    }
                }

                // see if the receipt actually needs deleting
                        if (receipt.OwnershipInfo.ClaimLines.Count == 0 && !receipt.OwnershipInfo.ClaimId.HasValue
                            && flattened.FirstOrDefault(r => r.Id == receipt.ReceiptId) == null)
                {
                    // delete the receipt (sproc updates the ownership)
                    receipts.DeleteReceipt(receipt.ReceiptId);
                   
                            //check to see whether it is being deleted by an authoriser to redo the "receipt not attached flag"
                    if (user.EmployeeID == claim.checkerid && expenseIdsToUpdate.Count > 0)
                    {
                        SortedList<int, cExpenseItem> expenseItems =
                            claims.getExpenseItemsFromDB(expenseIdsToUpdate);
                        FlagManagement flags = new FlagManagement(user.AccountID);
                        cAccountSubAccounts clsSubAccounts = new cAccountSubAccounts(user.AccountID);
                        cAccountSubAccount subaccount = clsSubAccounts.getFirstSubAccount();
                        foreach (cExpenseItem item in expenseItems.Values)
                        {
                            List<FlagSummary> flaggedItems = flags.CheckItemForFlags(
                                item,
                                claim.employeeid,
                                ValidationPoint.SubmitClaim,
                                SpendManagementLibrary.Flags.ValidationType.Any,
                                subaccount,
                                user);
                            // does receipt not attached exist?
                            if (flaggedItems.Count > 0)
                            {
                                bool hasReceiptFlag = flaggedItems.Where(summary => summary.FlaggedItem.Flagtype == FlagType.ReceiptNotAttached).Any();

                                if (hasReceiptFlag)
                                {
                                    flags.FlagItem(item.expenseid, flaggedItems.Where(flag => flag.FlaggedItem.Flagtype == FlagType.ReceiptNotAttached));
                                }
                            }
                        }
                    }    

                    // add any notes from the delete to the claim history
                    if (!string.IsNullOrWhiteSpace(deleted.Reason))
                    {
                                claims.UpdateClaimHistory(
                                    claim,
                                    string.Format("Receipt {0} deleted: {1}", receipt.ReceiptId, deleted.Reason),
                                    user.EmployeeID);
                    }
                }
            });
            

            // unstack the flattened list
            while (flattened.Count > 0)
            {
                // grab the linkage and pop off the top
                var linked = flattened[0];
                flattened.Remove(linked);

                // get the actual receipt
                receipt = fetchedReceipts.FirstOrDefault(r => r.ReceiptId == linked.Id) ?? receipts.GetById(linked.Id);

                // save in case needed again
                if (!fetchedReceipts.Contains(receipt))
                {
                    fetchedReceipts.Add(receipt);
                }

                // check whether it is attached to an expense or the header
                if (linked.ParentId.HasValue)
                {
                    // if the existing receipt isn't liked to the claim line, link it
                    if (!receipt.OwnershipInfo.ClaimLines.Contains(linked.ParentId.Value))
                    {
                        receipt.OwnershipInfo.ClaimLines.Add(linked.ParentId.Value);
                        receipts.LinkToClaimLine(receipt.ReceiptId, linked.ParentId.Value);

                        // record the expense to update
                        if (!expenseIdsToUpdate.Contains(linked.ParentId.Value))
                        {
                            expenseIdsToUpdate.Add(linked.ParentId.Value);
                        }
                    }
                }
                else
                {
                    // link to the header if it wasn't already set
                    if (receipt.OwnershipInfo.ClaimId != claim.claimid)
                    {
                        // link it to the user instead (any claimlines will remain linked).
                        receipt.OwnershipInfo.ClaimId = claim.claimid;
                        receipts.LinkToClaim(receipt.ReceiptId, claim.claimid);
                    }
                }
            }

            // loop through any expense items that need updating and update their validation progress.
            var items = claims.getExpenseItemsFromDB(claim.claimid);
            
            if (items.Any())
            {
                var employee = user.Employee;
                if (user.EmployeeID == claim.checkerid)
                {
                    employee = Employee.Get(claim.employeeid, user.AccountID);
                }
                
                var validationManager = new ExpenseValidationManager(user.AccountID);
                var subcats = new cSubcats(user.AccountID);
                var claimStageContainsValidate =
                    new cGroups(user.AccountID).GetGroupById(user.Employee.SignOffGroupID)
                        .stages.Values.Select(s => s.signofftype)
                        .Contains(SignoffType.SELValidation);

                foreach (var item in items.Values)
                {
                    item.DetermineValidationProgress(
                        user.Account,
                        subcats.GetSubcatById(item.subcatid).Validate,
                        claimStageContainsValidate,
                        validationManager);
                    var expenseItems = new cExpenseItems(user.AccountID);
                    var clssubcats = new cSubcats(user.AccountID);
                    var subcat = clssubcats.GetSubcatById(item.subcatid);

                   // send the expense to expedite validation service for receipt validation if claimant uploaded new receipt .
                    if (user.Account.ValidationServiceEnabled
                        && (item.ValidationProgress >= ExpenseValidationProgress.WaitingForClaimant))
                    {
                        // update the validation progress flag back to 0  "Validation Required" .
                        expenseItems.ReturnToValidationStageIfAny(item, user, claims, claim, subcat);
                        var expenseValidationManager = new ExpenseValidationManager(user.AccountID);
                        // update the validation count to 0 
                        expenseValidationManager.UpdateCountForExpenseItem(item.expenseid,0);
                        // update the claim history 
                        claims.UpdateClaimHistory(claim,"This expense has been amended.", claim.employeeid);
                    }
                }
            }
        }

        /// <summary>
        /// Assigns an envelope to a claim.
        /// </summary>
        /// <param name="envelopeId">The Id of the Envelope.</param>
        /// <param name="claimId">The Id of the Claim.</param>
        /// <returns>A list of receipts</returns>
        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void AssignEnvelopeToClaim(int envelopeId, int claimId)
        {
            // check permissions
            var user = cMisc.GetCurrentUser();

            // check permissions
            if (!user.Account.ReceiptServiceEnabled
                || !user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.EnvelopeManagement, true))
            {
                throw new Exception("You don't have permission to perform this operation.");
            }

            var envelopes = new Envelopes();
            var envelope = envelopes.GetEnvelopeById(envelopeId);
            var claims = new cClaims(user.AccountID);
            var claim = claims.getClaimById(claimId);
            var receiptManager = new SpendManagementLibrary.Expedite.Receipts(user.AccountID, user.EmployeeID);
            

            if (envelope == null || claim == null)
            {
                throw new Exception("Either the claim or the envelope was not found.");
            }
            
            // update claim history.
            var history = string.Format(
                "Your administrator has assigned an envelope ({0}) to your claim.",
                envelope.EnvelopeNumber);
            claims.UpdateClaimHistory(claim, history, claim.employeeid);

            var receipts = receiptManager.GetByEnvelope(envelope.EnvelopeId).ToList();
            receipts.ForEach(r => receiptManager.LinkToClaim(r.ReceiptId, claim.claimid));
            envelopes.AttachToClaim(envelopeId, claim, user);
            envelopes.MarkComplete(envelopeId, user);

            // check to see if claim can be advanced
            var envelopesForClaim = envelopes.GetEnvelopesByClaimReferenceNumber(claim.ReferenceNumber);
            var signoffGroup = new cGroups(user.AccountID).GetGroupById(user.Employee.SignOffGroupID);

            if (signoffGroup.stages.Values[claim.stage - 1].signofftype == SignoffType.SELScanAttach
                && envelopesForClaim.All(
                    e =>
                    (e.Status == EnvelopeStatus.ConfirmedSent && e.DeclaredLostInPost)
                    || (e.Status >= EnvelopeStatus.ReceiptsAttached)))
            {
                var delegateOrEmployeeId = user.isDelegate ? user.Delegate.EmployeeID : user.EmployeeID;

                // update claim history
                claims.UpdateClaimHistory(
                    claim,
                    "All envelopes for this claim are either declared lost in the post or have finished the scan & attach process. Advancing claim to the next stage.",
                    delegateOrEmployeeId);

                var submission = new ClaimSubmission(user);
                submission.SendClaimToNextStage(
                    claim,
                    false,
                    claim.checkerid,
                    delegateOrEmployeeId,
                    delegateOrEmployeeId);
            }
        }

        [WebMethod(EnableSession = true)]
        public SubmitClaimResult DetermineIfClaimCanBeSubmitted(
            int accountid,
            int claimid,
            int employeeid,
            byte viewfilter)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();
            var clsclaims = new cClaims(accountid);
            cClaim claim = clsclaims.getClaimById(claimid);
            ClaimSubmission claimSubmission = new ClaimSubmission(currentUser);
            return claimSubmission.DetermineIfClaimCanBeSubmitted(claim, viewfilter);
        }

        [WebMethod(EnableSession = true)]
        public SubmitClaimResult SubmitClaim(
            int claimId,
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
            object[] returnValues = new object[2];
            CurrentUser user = cMisc.GetCurrentUser();

            SubmitClaimResult result = new SubmitClaimResult();
            var clsEmployeeCards = new cEmployeeCorporateCards(user.AccountID);
            SortedList<int, cEmployeeCorporateCard> cards = clsEmployeeCards.GetEmployeeCorporateCards(user.EmployeeID);

            var clsclaims = new cClaims(user.AccountID);
            cClaim reqclaim;

            if (!ignoreApproverOnHoliday && approver.HasValue)
                {
                cGroups groups = new cGroups(user.AccountID);
                cGroup group = groups.GetGroupById(user.Employee.SignOffGroupID);
                SortedList lststages = groups.sortStages(group);
                var stage = (cStage)lststages.GetByIndex(0);
                if (stage.signofftype == SignoffType.DeterminedByClaimantFromApprovalMatrix
                    && clsclaims.userOnHoliday(SignoffType.Employee, approver.Value))
                {

                }
            }

            if (clsclaims.updateClaim(claimId, claimName, description, null, user.EmployeeID) == -1)
            {
                result.Reason = SubmitRejectionReason.ClaimNameAlreadyExists;
                return result;
            }

            reqclaim = clsclaims.getClaimById(claimId);
            if (reqclaim.submitted)
                {
                result.Reason = SubmitRejectionReason.Success;
                result.NewLocationURL = "expenses/claimViewer.aspx?claimId=" + claimId;
                return result;
            }

            if (cards != null && cards.Count > 0)
            {
                var clsstatements = new cCardStatements(user.AccountID);
                ListItem[] lstStatements = clsstatements.createStatementDropDown(user.EmployeeID);

                foreach (ListItem itm in lstStatements)
                {
                    int statementID = 0;

                    if (int.TryParse(itm.Value, out statementID))
                    {
                        cCardStatement statement = clsstatements.getStatementById(statementID);

                        if (statement != null)
                        {
                            if (statement.Corporatecard.blockcash)
                            {
                                if (clsstatements.getUnallocatedItemCount(user.EmployeeID, statementID) > 0)
                                {
                                   result.Reason = SubmitRejectionReason.CreditCardHasUreconciledItems;
                                   return result;
                                }
                            }
                        }
                    }
                }
            }

            var clsSubAccounts = new cAccountSubAccounts(user.AccountID);

            cAccountProperties properties = clsSubAccounts.getSubAccountById(user.CurrentSubAccountId).SubAccountProperties;
            if (properties.BlockUnmachedExpenseItemsBeingSubmitted)
            {
                SortedList<int, cExpenseItem> lstItems = clsclaims.getExpenseItemsFromDB(claimId);
                bool unallocatedItems = 
                    lstItems.Values.Any(
                        item => ((item.itemtype == ItemType.CreditCard && credit) || (item.itemtype == ItemType.PurchaseCard && purchase)) && item.transactionid == 0);

                if (unallocatedItems)
                {
                    result.Reason = SubmitRejectionReason.EmployeeHasUnmatchedCardItems;
                    return result;
                }
            }

            if (approver.HasValue)
                        {
                clsclaims.AddClaimantSelectedApprover(user, (int)approver);
            }
            else
                            {
                approver = 0;
            }

            int? delegateid = null;
            if (this.Session["myid"] != null)
            {
                delegateid = (int)this.Session["myid"];
                            }

            var currentOdometerReadings = new Dictionary<int, cOdometerReading>();
            if (odometerReadings != null)
            {
                cEmployeeCars employeeCars = new cEmployeeCars(user.AccountID, user.EmployeeID);

                currentOdometerReadings = this.UpdateOdometerReadings(odometerReadings, employeeCars, user);

                if (properties.SingleClaim && properties.EnterOdometerOnSubmit)
                {
                    var businessMilesPerCar = this.GetCurrentClaimBusinessMilesPerCar(clsclaims, reqclaim, user);

                    foreach (cOdometerReading currentOdometerReading in currentOdometerReadings.Values)
                    {
                        var totalOdometerDifference = currentOdometerReading.newreading
                                                      - currentOdometerReading.oldreading;
                        decimal businessMiles;
                        if (businessMilesPerCar.ContainsKey(currentOdometerReading.carid))
                        {
                            businessMiles = businessMilesPerCar[currentOdometerReading.carid];
                        }
                        else
                        {
                            businessMiles = 0;
                        }
                        
                        if (totalOdometerDifference < businessMiles)
                        {
                            var car = employeeCars.GetCarByID(currentOdometerReading.carid);
                            result.Reason = SubmitRejectionReason.OdometerReadingLessThanBusinessMileage;
                            result.Message = $"The difference between the last reading ({currentOdometerReading.oldreading}) and the new reading ({currentOdometerReading.newreading})\n must be greater than or equal to the business mileage of {businessMiles}\n on the vehicle {car} for this claim.";
                            return result;
                        }
                    }
                }
            }

            var claimSubmission = new ClaimSubmission(user);
            result = claimSubmission.SubmitClaim(
                reqclaim,
                cash,
                credit,
                purchase,
                approver.Value,
                user.EmployeeID,
                delegateid,
                viewfilter, false,
                continueAlthoughAuthoriserIsOnHoliday);
            if (result.Reason != SubmitRejectionReason.Success
                && result.Reason != SubmitRejectionReason.ClaimSentToNextStage)
                            {
                return result;
            }

            // save the odometer readings
            if (odometerReadings != null)
                                {
                cEmployees employees = new cEmployees(user.AccountID);
                foreach (cOdometerReading reading in currentOdometerReadings.Values)
                {
                    employees.saveOdometerReading(
                        0,
                        user.EmployeeID,
                        reading.carid,
                        null,
                        reading.oldreading,
                        reading.newreading,
                        Convert.ToByte(businessMileage));
                                }

                if (properties.EnterOdometerOnSubmit)
                {
                    clsclaims.calculatePencePerMile(reqclaim.employeeid, reqclaim);
                            }

                clsclaims.calculateReimbursableFuelCardMileage(user.EmployeeID, reqclaim.claimid, reqclaim.employeeid);
                        }

            if ((cash == false || credit == false || purchase == false) && result.ClaimID > 0)
            {
                // part submittal
                result.NewLocationURL = "claimViewer.aspx?claimId=" + result.ClaimID;
                }
            else
                {
                result.NewLocationURL = "claimViewer.aspx?claimId=" + claimId;
                }

            return result;
            }

        private Dictionary<int, decimal> GetCurrentClaimBusinessMilesPerCar(cClaims clsclaims, cClaim reqclaim, CurrentUser user)
        {
            SortedList<int, cExpenseItem> expenseItemsList = clsclaims.getExpenseItemsFromDB(reqclaim.claimid);
            decimal count = 0;
            cExpenseItems expenseItems = new cExpenseItems(user.AccountID);
            var businessMilesPerCar = new Dictionary<int, decimal>();
            foreach (var expenseItem in expenseItemsList)
            {
                var carId = expenseItem.Value.carid;
                if (!businessMilesPerCar.ContainsKey(carId))
                {
                    businessMilesPerCar.Add(carId, 0);
                }

                var journeySteps = expenseItems.GetJourneySteps(expenseItem.Value.expenseid);
                foreach (var step in journeySteps)
                {
                    businessMilesPerCar[carId] += step.Value.nummiles;
                }
            }
            return businessMilesPerCar;
        }

        private Dictionary<int, cOdometerReading> UpdateOdometerReadings(List<List<object>> odometerReadings, cEmployeeCars employeeCars, CurrentUser user)
        {
            var currentOdometerReadings = new Dictionary<int, cOdometerReading>();

            foreach (List<object> reading in odometerReadings)
            {
                var carid = (int)reading[0];
                if (!currentOdometerReadings.ContainsKey(carid))
                {
                    var car = employeeCars.GetCarByID(carid);
                    var lastReading = car.getLastOdometerReading();
                    var storedReading = new cOdometerReading(
                        0,
                        carid,
                        DateTime.Now,
                        lastReading?.newreading ?? 0,
                        Convert.ToInt32(reading[2]),
                        DateTime.Now,
                        user.EmployeeID);
                    currentOdometerReadings.Add(carid, storedReading);
                }
            }
            return currentOdometerReadings;
        }

        [WebMethod(EnableSession = true)]
        public ClaimUnsubmittableReason unsubmitClaim(int accountId, int employeeId, int? delegateId, int claimId)
        {
            var clsclaims = new cClaims(accountId);

            cClaim reqclaim = clsclaims.getClaimById(claimId);
            if (delegateId == 0)
            {
                delegateId = null;
            }

            return clsclaims.UnSubmitclaim(reqclaim, false, employeeId, delegateId);
        }

        /// <summary>
        /// For a list of Envelopes, sets whether they have been sent and therefore must be missing,
        /// or whether the user has said they will post them but hasn't yet within the given timeframe.
        /// </summary>
        /// <param name="claimId">The Id of the claim.</param>
        /// <param name="statuses">A dictionary of EnvelopeId / HasSent.</param>
        /// <returns>A bool indicating the success.</returns>
        [WebMethod(EnableSession = true)]
        public bool UpdateEnvelopeMissingStatus(int claimId, EnvelopeMissingStatus[] statuses)
        {
            var envelopes = new Envelopes();
            var currentUser = cMisc.GetCurrentUser();
            var delegateOrEmployeeId = currentUser.isDelegate ? currentUser.Delegate.EmployeeID : currentUser.EmployeeID;
            var claims = new cClaims(currentUser.AccountID);
            var claim = claims.getClaimById(claimId);

            // error check
            if (claim == null || claim.ReferenceNumber == null)
            {
                return false;
            }

            foreach (var status in statuses)
            {
                var envelope = envelopes.GetEnvelopeById(status.Id); 
                
                // error check
                if (envelope == null || envelope.Status != EnvelopeStatus.UnconfirmedNotSent || !status.HasSent.HasValue)
                {
                    return false;
                }
                
                if (status.HasSent.Value)
                {
                    // the envelope is technically lost in the post.
                    envelope.DeclaredLostInPost = true;
                    envelope.Status = EnvelopeStatus.ConfirmedSent;
                    envelopes.EditEnvelope(envelope, currentUser);

                    // update claim history
                    claims.UpdateClaimHistory(
                        claim,
                        string.Format(
                            "Claimant has declared envelope {0} as lost in the post.",
                            envelope.EnvelopeNumber),
                        delegateOrEmployeeId);
                }
                else
                {
                    // reset to attached to claim (and the date too)
                    envelopes.AttachToClaim(envelope.EnvelopeId, claim, currentUser);

                    // update claim history
                    claims.UpdateClaimHistory(
                        claim,
                        string.Format(
                            "Claimant has confirmed that envelope {0} will be sent for scan & attach as soon as possible.",
                            envelope.EnvelopeNumber),
                        delegateOrEmployeeId);
                }
            }

            // check to see if claim can be advanced
            var envelopesForClaim = envelopes.GetEnvelopesByClaimReferenceNumber(claim.ReferenceNumber);
            var signoffGroup = new cGroups(currentUser.AccountID).GetGroupById(currentUser.Employee.SignOffGroupID);

            if (signoffGroup.stages.Values[claim.stage - 1].signofftype == SignoffType.SELScanAttach
                && envelopesForClaim.All(
                    e =>
                    (e.Status == EnvelopeStatus.ConfirmedSent && e.DeclaredLostInPost)
                    || (e.Status >= EnvelopeStatus.ReceiptsAttached)))
            {
                // update claim history
                claims.UpdateClaimHistory(
                    claim,
                    "All envelopes for this claim are either declared lost in the post or have finished the scan & attach process. Advancing claim to the next stage.",
                    delegateOrEmployeeId);

                var submission = new ClaimSubmission(currentUser);
                submission.SendClaimToNextStage(
                    claim,
                    false,
                    claim.checkerid,
                    delegateOrEmployeeId,
                    delegateOrEmployeeId);
            }

            return true;
        }

        /// <summary>
        /// Clears the Claim Id and ClaimReferenceNumber from all the envelopes attached to a claim.
        /// </summary>
        /// <param name="claimId">The ID of the claim to use</param>
        /// <param name="accountId">The current account ID</param>
        /// <param name="envelopeId">The Id of the envelope to unlink.</param>
        /// <returns>The reference number for the specified claim ID</returns>
        [WebMethod(EnableSession = true)]
        public string ClearEnvelopeLinkage(int accountId, int claimId, int envelopeId)
        {
            var user = cMisc.GetCurrentUser();

            if (user == null)
            {
                throw new Exception("The employee supplied is incorrect.");
            }

            // unlink
                var claims = new cClaims(user.AccountID);
            return claims.UnlinkEnvelopeFromClaim(claimId, envelopeId, user);
        }

        [WebMethod]
        public string[] filterExpenseGrid(
            int accountId,
            int employeeId,
            int claimId,
            Filter filter,
            UserView viewType,
            bool enableReceiptAttachments,
            bool enableJourneyDetailsLink,
            bool enableCorporateCardLink,
            string symbol)
        {
            cExpenseItems expitems = new cExpenseItems(accountId);
            ;
            var claims = new cClaims(accountId);
            var claim = claims.getClaimById(claimId);

            return expitems.generateClaimGrid(
                employeeId,
                claim,
                "gridExpenses",
                viewType,
                filter,
                false,
                enableReceiptAttachments,
                enableJourneyDetailsLink,
                enableCorporateCardLink,
                symbol);
        }

        [WebMethod]
        public string checkTransactionCurrencyAndCountry(int transactionid, int accountid)
        {
            string strInvalid = "";

            CurrentUser user = cMisc.GetCurrentUser();
            cCardStatements clsStatements = new cCardStatements(accountid);

            cCardTransaction transaction = clsStatements.getTransactionById(transactionid);
            cCountries clsCountries = new cCountries(accountid, user.CurrentSubAccountId);
            cCurrencies clsCurrencies = new cCurrencies(accountid, user.CurrentSubAccountId);

            if (transaction.globalcurrencyid != null)
            {
                cCurrency curr = clsCurrencies.getCurrencyByGlobalCurrencyId((int)transaction.globalcurrencyid);

                if (curr == null)
                {
                    cGlobalCurrencies clsGlobCurrencies = new cGlobalCurrencies();
                    cGlobalCurrency globCurr = clsGlobCurrencies.getGlobalCurrencyById(
                        (int)transaction.globalcurrencyid);

                    if (globCurr == null)
                    {
                        strInvalid =
                            "- There is a problem with the currencies on this credit card statement please contact the support desk at Selenity Ltd to resolve. \n\r";
                    }
                    else
                    {
                        strInvalid = "- Currency '" + globCurr.label
                                     + "' does not exist, please contact your administrator. \n\r";
                    }
                }
            }

            if (transaction.globalcountryid != null)
            {
                cCountry country = clsCountries.getCountryByGlobalCountryId((int)transaction.globalcountryid);

                if (country == null)
                {
                    cGlobalCountries clsGlobCountries = new cGlobalCountries();
                    cGlobalCountry globCountry = clsGlobCountries.getGlobalCountryById((int)transaction.globalcountryid);

                    if (globCountry == null)
                    {
                        strInvalid +=
                            "- There is a problem with the countries on this credit card statement please contact the support desk at Selenity Ltd to resolve. \n\r";
                    }
                    else
                    {
                        strInvalid += "- Country '" + globCountry.Country + "' with country code '" + globCountry.CountryCode + "' does not exist, please contact your administrator. \n\r";
                    }
                }
                else if (country.Archived)
                {
                    cGlobalCountries clsGlobCountries = new cGlobalCountries();
                    cGlobalCountry globCountry = clsGlobCountries.getGlobalCountryById((int)transaction.globalcountryid);

                    strInvalid += "- Country '" + globCountry.Country + "' with country code '" + globCountry.CountryCode + "' is archived, please contact your administrator. \n\r";
                }
            }

            return strInvalid;
        }

        [WebMethod]
        public string getTransactionDetails(int accountId, int transactionId)
        {
            cCardStatements templates = new cCardStatements(accountId);
            return templates.getTransactionDetails(transactionId);
        }

        [WebMethod]
        public string[] getCardStatementMatchView(
            int accountId,
            int employeeId,
            int claimId,
            UserView viewType,
            int transactionId,
            string symbol)
        {
            int itemType;
            cExpenseItems expenseItems = new cExpenseItems(accountId);
            cCardStatements clsstatements = new cCardStatements(accountId);
            cCardTransaction transaction = clsstatements.getTransactionById(transactionId);
            cCardStatement statement = clsstatements.getStatementById(transaction.statementid);
            if (statement.Corporatecard.cardprovider.cardtype == CorporateCardType.CreditCard)
            {
                itemType = 2;
            }
            else
            {
                itemType = 3;
            }
            string[] gridData = clsstatements.generateCardMatchingGrid(employeeId, claimId, transactionId, itemType);
            return gridData;
        }

        [WebMethod]
        public bool matchTransaction(
            int accountId,
            int employeeId,
            int statementId,
            int transactionId,
            int claimId,
            int expenseId)
        {
            cCardStatements statements = new cCardStatements(accountId);
            cCardStatement statement = statements.getStatementById(statementId);
            cCardTransaction transaction = statements.getTransactionById(transactionId);
            cClaims claims = new cClaims(accountId);

            cExpenseItem item = claims.getExpenseItemById(expenseId);
            return statements.matchTransaction(statement, transaction, item);

        }

        [WebMethod]
        public void unmatchTransaction(int accountId, int expenseId, int transactionId, int claimId)
        {
            cCardStatements statements = new cCardStatements(accountId);
            statements.unmatchItem(expenseId);
        }

        [WebMethod]
        public string[] generateCardTransactionGrid(int accountId, int employeeId, int statementId)
        {
            cCardStatements statements = new cCardStatements(accountId);
            return statements.generateCardTransactionGrid(statementId, employeeId);
        }

        /// <summary>
        /// Deletes an expense item
        /// </summary>
        /// <param name="accountid">The ID of the current account</param>
        /// <param name="expenseid">The ID of the current expense</param>
        /// <param name="claimid">The ID of the current claim</param>
        /// <param name="employeeid">The ID of the current employee</param>
        /// <returns>A boolean that tells the client side to redirect to My Claims or not</returns>
        [WebMethod(EnableSession = true)]
        public Boolean deleteExpense(int accountid, int expenseid, int claimid, int employeeid)
        {
            ICurrentUser user = cMisc.GetCurrentUser();
            cClaims clsclaims = new cClaims(accountid);
            cClaim reqclaim = clsclaims.getClaimById(claimid);
            cExpenseItem item = clsclaims.getExpenseItemById(expenseid);
            var redirectToMyClaims = Convert.ToBoolean(clsclaims.deleteExpense(reqclaim, item, false, user));
            return redirectToMyClaims;
        }

        [WebMethod(EnableSession = true)]
        public string[] deleteMobileExpenseItem(int accountid, int mobileitemid, int employeeid)
        {
            cMobileDevices clsMobileDevices = new cMobileDevices(accountid);
            clsMobileDevices.DeleteMobileItemByID(mobileitemid);
            return new string[] { };
        }

        /// <summary>
        /// Deletes a mobile journey
        /// </summary>
        /// <param name="mobilejourneyid">The mobile journey ID to delete</param>
        /// <param name="employeeid">The employee who the journey is for</param>
        /// <returns>A <see cref="string[]"/></returns>
        [WebMethod(EnableSession = true)]
        public string[] DeleteMobileJourney(int mobilejourneyid, int employeeid)
        {
            ICurrentUser user = cMisc.GetCurrentUser();

            cMobileDevices clsMobileDevices = new cMobileDevices(user.AccountID);

            if (user.EmployeeID == employeeid)
            {
                clsMobileDevices.DeleteMobileJourney(mobilejourneyid);
            }

            return new string[] { };

        }

        /// <summary>
        /// Sets expense items to approved, providing there a no outstanding flag justifications
        /// </summary>
        /// <param name="accountId">The accountId</param>
        /// <param name="currentUserId">The current user Id</param>
        /// <param name="delegateId">The delegate Id</param>
        /// <param name="claimId">The claim Id</param>
        /// <param name="expenseItemIds">The expense item Ids</param>
        /// <returns>The <see cref="AllowExpenseItemsResult">AllowExpenseItemResult</see>/></returns>
        [WebMethod]
        public AllowExpenseItemsResult AllowSelected(int accountId, int currentUserId, int? delegateId, int claimId, List<int> expenseItemIds)
        {
            var claims = new cClaims(accountId);        
            return claims.AllowExpenseItems(accountId, currentUserId, delegateId, claimId, expenseItemIds);        
            }

        [WebMethod]
        public int unapproveItem(int accountId,int employeeId,int? delegateId ,int expenseId)
        {
            cClaims clsclaims = new cClaims(accountId);
            List<cExpenseItem> expenseItems = new List<cExpenseItem>();

            expenseItems = clsclaims.GetSplitExpenseItems(accountId, unapprovedExpenseId: expenseId);

            foreach (cExpenseItem expenseItem in expenseItems)
            {
                clsclaims.UnapproveItem(expenseItem);
                clsclaims.DeleteClaimApproverDetailByExpensesId(expenseId, employeeId);
            }

            return 1;
        }

        [WebMethod(EnableSession = true)]
        public bool returnExpenses(
            int accountId,
            int employeeId,
            int? delegateId,
            int claimId,
            List<int> items,
            string reason)
        {
            cClaims claims = new cClaims(accountId);

            cClaim claim = claims.getClaimById(claimId);


            if (delegateId == 0)
            {
                delegateId = null;
            }

            return claims.ReturnExpenses(claim, items, reason, employeeId, delegateId);

            }

        [WebMethod]
        public SubmitClaimResult approveClaim(int accountId, int employeeId, int? delegateId, int claimId)
        {

            var claimSubmission = new ClaimSubmission(cMisc.GetCurrentUser());
            var reqclaim = claimSubmission.getClaimById(claimId);

            SubmitClaimResult result = new SubmitClaimResult();
            if (reqclaim.NumberOfUnapprovedItems > 0)
            {
                result.Reason = SubmitRejectionReason.ItemsStillToApprove;
                return result;
            }
            if (delegateId == 0)
            {
                delegateId = null;
            }

            //does the approver need to justify any items??
            FlagManagement flagMan = new FlagManagement(accountId);

            //revalidate flags to see if claim needs stopping

            FlaggedItemsManager flagResults = flagMan.CheckJustificationsHaveBeenProvidedByAuthoriser(claimId);
            if (flagResults.Count > 0)
            {
                result.Reason = SubmitRejectionReason.OutstandingFlagsRequiringJustificationByApprover;
                
                flagResults.Claimant = false;
                flagResults.Stage = reqclaim.stage;
                flagResults.Authorising = true;
                flagResults.SubmittingClaim = false;
                flagResults.OnlyDisplayBlockedItems = false;
                flagResults.AllowingOrApproving = true;
                cEmployees employees = new cEmployees(accountId);
                Employee employee = employees.GetEmployeeById(reqclaim.employeeid);
                flagResults.ClaimantName = employee.FullName;
                result.FlagResults = flagResults;
                return result;
            }

            // store the return code and check, rather than potentially sending the claim to the next stage multiple times

            return claimSubmission.SendClaimToNextStage(reqclaim, false, 0, employeeId, delegateId);

        }

        [WebMethod(EnableSession = true)]
        public void deleteClaim(int accountId, int claimId)
        {
            cClaims clsclaims = new cClaims(accountId);
            clsclaims.DeleteClaim(clsclaims.getClaimById(claimId));
        }

        /// <summary>
        /// Returns all the validation results for an expense item.
        /// </summary>
        /// <param name="accountId">The accountId.</param>
        /// <param name="claimId">The Id of the claim under which the expense item sits.</param>
        /// <param name="expenseId">The Id of the expense item.</param>
        /// <returns>And object to be parsed client-side.</returns>
        [WebMethod]
        public string DisplayValidation(int accountId, int claimId, int expenseId)
        {
       
            var validationManager = new ExpenseValidationManager(accountId);
            List<ExpenseValidationResult> initialResults = validationManager.GetResultsForExpenseItem(expenseId).ToList();
                              
            // set up reference dictionaries for quick access later 
            Dictionary<ExpenseValidationResultStatus, string> statusIcons = ExpediteValidationHelper.GetStatusIcons();
            Dictionary<ExpenseValidationResultStatus, string> statusDescriptions = ExpediteValidationHelper.GetStatusDescriptions();

            var builder = new StringBuilder("<div class='sectiontitle'>Receipt Validation Information </div><div>");
            if (initialResults.Count == 0)
            {
                builder.Append("There are no validation results to display.");
            }
            else
            {
                var fields = new cFields(accountId);
                var employee = cMisc.GetCurrentUser();
                var claims = new cClaims(accountId);
                var claim = claims.getClaimById(claimId);
                var expense = claims.getExpenseItemById(expenseId);
                var invalidated = expense.ValidationProgress > ExpenseValidationProgress.CompletedPassed;

                // show a notice if the expense item has been invalidated
                if (invalidated)
                {
                    builder.AppendFormat(
                        "<p><img src=\"{0}\" alt=\"Validation Invalidated\" /> &nbsp;<span class='important'>This claim line has been edited since validation was performed; the validation may not reflect the current state of the item.</span></p>",
                        GlobalVariables.StaticContentLibrary + "/icons/16/plain/validation_header_invalid.png");
                }
                
                // duplicate each result now, to split out into Business/Custom, VAT and fraud.
                var results = validationManager.GetExpenseValidationResults(
                    initialResults,
                    statusIcons,
                    invalidated,
                    statusDescriptions,
                    fields);

                // work out which tab should be open based on errors. Default to top tab.
                var activeTab = results.BusinessResults.Any(r => r.Status == ExpenseValidationResultStatus.Fail)
                    ? 0
                    : results.VatResults.Any(r => r.Status == ExpenseValidationResultStatus.Fail)
                        ? 1
                                          : results.FraudResults.Any(r => r.Status == ExpenseValidationResultStatus.Fail)
                                                ? 2
                                                : 0;
                
                // accordion start
                builder.AppendFormat("<div class='validation-results-accordion' data-active='{0}'>", activeTab);

                // BUSINESS Accordion
                results.BusinessResults = results.BusinessResults.OrderBy(r => r.Status).ToList();
                    var compoundResult = results.BusinessResults.All(x => x.Status != ExpenseValidationResultStatus.Fail);
                var chosenIcon =
                    string.Format(
                        compoundResult
                            ? statusIcons[ExpenseValidationResultStatus.Pass]
                            : statusIcons[ExpenseValidationResultStatus.Fail],
                        invalidated ? "_invalid" : string.Empty);
                var chosenDescription = compoundResult
                                            ? statusDescriptions[ExpenseValidationResultStatus.Pass]
                                            : statusDescriptions[ExpenseValidationResultStatus.Fail];
                builder.AppendFormat(
                    "<h3 class='validation-results-header'><img src='{0}' title='Business Rule Validation: {1}' alt='Business {1}'/> <strong>Business {1}</strong> <img class='validation-results-status' src='{2}' title='Compound Status: {1}' alt='Compound Status: {1}'/></h3><div class='validation-results-container'><div>Receipt validation failures for business checks mean that there is a possibility that an expense might not get approved.</div><br/>",
                    ExpediteValidationHelper.BusinessIcon,
                    chosenDescription,
                    chosenIcon);
                    BuildTableForValidationResultSet(builder, results.BusinessResults, ExpediteValidationHelper.HeaderIcon);
                    builder.Append("</div>");

                // VAT Accordion
                results.VatResults = results.VatResults.OrderBy(r => r.Status).ToList();
                if (results.VatResults.Any(r => r.Status != ExpenseValidationResultStatus.NotApplicable))
                {
                    compoundResult = results.VatResults.All(x => x.Status != ExpenseValidationResultStatus.Fail);
                    chosenIcon =
                        string.Format(
                            compoundResult
                                ? statusIcons[ExpenseValidationResultStatus.Pass]
                                : statusIcons[ExpenseValidationResultStatus.Fail],
                            invalidated ? "_invalid" : string.Empty);
                    chosenDescription = compoundResult
                        ? statusDescriptions[ExpenseValidationResultStatus.Pass]
                        : statusDescriptions[ExpenseValidationResultStatus.Fail];
                    builder.AppendFormat(
                        "<h3 class='validation-results-header'><img src='{0}' title='VAT Rule Validation: {1}' alt='VAT Validation {1}'/> <strong>VAT {1}</strong> <img class='validation-results-status' src='{2}' title='Compound Status: {1}' alt='Compound Status: {1}'/></h3><div class='validation-results-container'><div>Receipt validation failures for VAT checks mean that there is a possibility that your company will not be able to claim back the VAT on expense payments.</div><br/>",
                        ExpediteValidationHelper.VatIcon,
                        chosenDescription,
                        chosenIcon);
                    this.BuildTableForValidationResultSet(builder, results.VatResults, ExpediteValidationHelper.HeaderIcon);
                    builder.Append("</div>");
                }
                    

                    // if the employee is the approver
                    if (employee.EmployeeID == claim.checkerid)
                    {
                    // FRAUD Accordion
                    results.FraudResults = results.FraudResults.OrderBy(r => r.Status).ToList();
                        compoundResult = results.FraudResults.All(x => x.Status != ExpenseValidationResultStatus.Fail);
                    chosenIcon =
                        string.Format(
                            compoundResult ? statusIcons[ExpenseValidationResultStatus.Pass] : ExpediteValidationHelper.WarningIcon,
                            invalidated ? "_invalid" : string.Empty);
                    chosenDescription = compoundResult
                                            ? statusDescriptions[ExpenseValidationResultStatus.Pass]
                                            : ExpediteValidationHelper.FraudTypeDescription;
                    builder.AppendFormat(
                        "<h3 class='validation-results-header'><img src='{0}' title='Possible Fraud Notices' alt='Possible Fraud Notices'/> <strong>Possible Fraud Indication {1}</strong> <img class='validation-results-status' src='{2}' title='Compound Status: {1}' alt='Compound Status: {1}'/></h3><div class='validation-results-container'><div>Fraud indicators are purely to alert approvers of <strong>possibly</strong> fraudulent receipts.</div><br/>",
                        ExpediteValidationHelper.FraudIcon,
                        chosenDescription,
                        chosenIcon);
                        BuildTableForValidationResultSet(builder, results.FraudResults, ExpediteValidationHelper.HeaderIcon);
                        builder.Append("</div>");
                    }

                // accordion end
                builder.Append("</div>");
            }

            // close
            builder.Append("</div>");

            return builder.ToString();
        }

       
   

        /// <summary>
        /// 
        /// </summary>
        /// <param name="accountID"></param>
        /// <param name="claimID"></param>
        /// <param name="expenseIDs"></param>
        /// <returns></returns>
        [WebMethod]
        public FlaggedItemsManager DisplayFlags(int accountID, int claimID, string expenseIDs, string pageSource)
                            {
            svcFlagRules flagRules = new svcFlagRules();


            return flagRules.CreateFlagsGrid(claimID, expenseIDs, pageSource);
        }

        /// <summary>
        /// Allocates a claim to a team member
        /// </summary>
        /// <param name="accountId">The Account Id</param>
        /// <param name="employeeId">The team member employee id</param>
        /// <param name="claimIds">The Claim Ids</param>
        /// <returns>The allocating claim result</returns>
        [WebMethod]
        public List<AllocateClaimResult> AllocateClaims(int accountId, int employeeId, List<int> claimIds)
            {
            var  claims = new cClaims(accountId);

            return claimIds.Select(id => claims.AllocateClaim(id, employeeId)).ToList();
        }

        [WebMethod]
        public void unallocateClaim(int accountId, int employeeId, int claimId)
        {
            cClaims claims = new cClaims(accountId);
            claims.unallocateClaim(claimId, employeeId);
        }

        [WebMethod]
        public SubmitClaimResult OneClickApproveClaim(int accountId, int employeeId, int? delegateId, int claimId)
        {
            var claims =
                new ClaimSubmission(
                    new CurrentUser(
                        accountId,
                        employeeId,
                        delegateId.HasValue ? delegateId.Value : 0,
                        Modules.expenses,
                        -1));
            if (delegateId == 0)
            {
                delegateId = null;
            }

            cClaim claim = claims.getClaimById(claimId);

            // store the return code and check, rather than potentially sending the claim to the next stage multiple times

            return claims.SendClaimToNextStage(claim, false, 0, employeeId, delegateId);
        }

        [WebMethod]
        public void payClaim(int accountId, int employeeId, int? delegateId, int claimId)
        {
            var claims = new cClaims(accountId);
            var claim = claims.getClaimById(claimId);

            if (delegateId == 0)
            {
                delegateId = null;
            }

            claims.payClaim(claim, employeeId, delegateId);
        }

        [WebMethod(EnableSession = true)]
        public string[] filterCheckAndPayClaimGrid(int accountId, int employeeId, string surname, byte filter)
        {
            CurrentUser user = cMisc.GetCurrentUser();
            cClaims claims = new cClaims(accountId);
            return claims.generateClaimsToCheckGrid(
                employeeId,
                surname,
                filter,
                user.isDelegate ? user.Delegate.EmployeeID : (int?)null);
        }

        [WebMethod(EnableSession = true)]
        public string[] filterCheckAndPayUnallocatedGrid(int accountId, int employeeId, string surname, byte filter)
        {
            CurrentUser user = cMisc.GetCurrentUser();
            cClaims claims = new cClaims(accountId);
            return claims.generateUnallocatedClaimsGrid(
                employeeId,
                surname,
                filter,
                user.isDelegate ? user.Delegate.EmployeeID : (int?)null);
        }

        [WebMethod]
        public List<string> GetClaimTotalAndAmountPayable(int accountId, int? currentSubAccountId, int claimId)
        {
            cClaims claims = new cClaims(accountId);
            cClaim claim = claims.getClaimById(claimId);
            cGlobalCurrencies clsglobalcurrencies = new cGlobalCurrencies();
            cCurrencies clscurrencies = new cCurrencies(accountId, currentSubAccountId);
            cCurrency reqcurrency = clscurrencies.getCurrencyById(claim.currencyid);

            string symbol;
            if (reqcurrency != null)
            {
                symbol = clsglobalcurrencies.getGlobalCurrencyById(reqcurrency.globalcurrencyid).symbol;
            }
            else
            {
                symbol = "£";

            }
            List<string> values = new List<string>();
            values.Add(Math.Round(claim.Total, 2, MidpointRounding.AwayFromZero).ToString(symbol + "###,###,##0.00"));
            values.Add(
                Math.Round(claim.AmountPayable, 2, MidpointRounding.AwayFromZero).ToString(symbol + "###,###,##0.00"));
            values.Add(claim.NumberOfItems.ToString());
            return values;
        }

        /// <summary>
        /// Approver level unsubmit claim
        /// </summary>
        /// <param name="claimId">The claimid being unsubmitted</param>
        /// <param name="reason">The reason for unsubmission</param>
        /// <returns>Return code indicating success (0) or failure (negative)</returns>
        [WebMethod(EnableSession = true)]
        public int UnsubmitClaimAsApprover(int claimId, string reason)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();
            cClaims claims = new cClaims(currentUser.AccountID);
            return claims.UnsubmitClaimAsApprover(
                currentUser.EmployeeID,
                                                  currentUser.isDelegate ? currentUser.Delegate.EmployeeID : (int?)null,
                                                  claimId,
                reason,
                currentUser);
        }

        /// <summary>
        /// Are all claim items returned by all other approvers
        /// </summary>
        /// <param name="claimId">The claim id</param>
        /// <returns>A bool indicating whether or not it can be unsubmitting</returns>
        [WebMethod(EnableSession = true)]
        public int IsClaimUnsubmittable(int claimId)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();
            var claims = new cClaims(currentUser.AccountID);
            var result = claims.IsClaimUnsubmittable(currentUser.EmployeeID, claimId);
            
            return (int)result;
        }


        /// <summary>
        /// Check claim total is within the min and max values for the current user.
        /// </summary>
        /// <param name="claim">The claim object to check</param>
        /// <param name="cash">True if a cash claim</param>
        /// <param name="credit">True if a credit card claim</param>
        /// <param name="purchase">True is a purchase card claim</param>
        /// <returns>0 - ok
        /// 1 - total is less than the minimum claim value
        /// 2 - total is more than the maximum claim value</returns>
        internal SubmitRejectionReason CheckClaimTotal(
            cClaim claim,
            bool cash,
            bool credit,
            bool purchase,
            CurrentUser _user)
        {
            decimal? minclaim = _user.ExpenseClaimMinimumValue;
            decimal? maxclaim = _user.ExpenseClaimMaximumValue;

            if (cash == false && (credit || purchase))
            {
                return SubmitRejectionReason.Success;
            }

            if ((minclaim != null && minclaim != 0) || (maxclaim != null && maxclaim != 0))
            {
                decimal total = claim.Total;

                if (cash != credit)
                {
                    total -= claim.CreditCardTotal;
                }
                if (cash != purchase)
                {
                    total -= claim.PurchaseCardTotal;
                }

                if (minclaim != null)
                {
                    if (total < minclaim)
                    {
                        return SubmitRejectionReason.MinimumAmountNotReached;
                    }
                }
                if (maxclaim != null)
                {
                    if (total > maxclaim)
                    {
                        return SubmitRejectionReason.MaximumAmountExceeded;
                    }
                }
            }

            return SubmitRejectionReason.Success;
        }

        internal bool CheckFrequency(int employeeid, CurrentUser _user)
        {
            int frequencyvalue;
            int count;
            using (var expdata = new DatabaseConnection(cAccounts.getConnectionString(_user.AccountID)))
            {
                var clsmisc = new cMisc(_user.AccountID);
                var clsproperties = clsmisc.GetGlobalProperties(_user.AccountID);

                if (clsproperties.limitfrequency == false)
                {
                    return true;
                }

                byte frequencytype = clsproperties.frequencytype;
                frequencyvalue = clsproperties.frequencyvalue;

                DateTime startdate;

                if (frequencytype == 1) //monthly
                {
                    startdate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 01);
                }
                else //weekly
                {
                    startdate = DateTime.Today;
                    while (startdate.DayOfWeek != DayOfWeek.Monday)
                    {
                        startdate = startdate.AddDays(-1);
                    }
                }

                //get count
                const string strsql =
                    "select count(*) from claims_base where employeeid = @employeeid and datesubmitted >= @date";
                expdata.sqlexecute.Parameters.AddWithValue("@employeeid", employeeid);
                expdata.sqlexecute.Parameters.AddWithValue(
                    "@date",
                    startdate.Year + "/" + startdate.Month + "/" + startdate.Day);
                count = expdata.ExecuteScalar<int>(strsql);
                expdata.sqlexecute.Parameters.Clear();
            }

            return count < frequencyvalue;
        }

        [WebMethod]
        public ClaimSubmissionDetails generateClaimSubmitScreen(int claimId)
        {

            ClaimSubmissionDetails details = new ClaimSubmissionDetails();
            CurrentUser user = cMisc.GetCurrentUser();
            StringBuilder output = new StringBuilder();
            cAccountSubAccounts clsSubAccounts = new cAccountSubAccounts(user.AccountID);
            cAccountProperties reqProperties = clsSubAccounts.getFirstSubAccount().SubAccountProperties;
            cClaims clsclaims = new cClaims(user.AccountID);
            cClaim reqclaim = clsclaims.getClaimById(claimId);

            #region claim and description

            details.ClaimName = reqclaim.name;
            //determine the description
            string description;
            if (reqclaim.description == string.Empty)
            {
                string startdate = "";
                string enddate = "";
                if (reqclaim.StartDate != null)
                {
                    startdate = ((DateTime)reqclaim.StartDate).ToShortDateString();
                }
                if (reqclaim.EndDate != null)
                {
                    enddate = ((DateTime)reqclaim.EndDate).ToShortDateString();
                }
                description = "Expense Claim " + reqclaim.claimno + ":" + startdate + " - " + enddate;
            }
            else
            {
                description = reqclaim.description;
            }
            details.ClaimDescription = description;

            #endregion

            #region credit cards

            details.PartSubmittal = (reqProperties.PartSubmit && reqclaim.containsCashAndCredit());
            if (details.PartSubmittal)
        {


                string fieldType;
                if (reqProperties.OnlyCashCredit)
                {
                    fieldType = "radio";
                }
                else
            {
                    fieldType = "checkbox";
                }
                output.Append("<div class=\"sectiontitle\">Which items would you like to submit*?</div>");
                output.Append(
                    "<div class=\"twocolumn\"><label>Cash items</label><span class=\"inputs\"><input name=\"tosubmit\" id=\"tosubmitcash\" type=\""
                    + fieldType + "\" value=\"1\"");
                output.Append(" />");
                output.Append(
                    "</span><span class=\"inputicon\"></span><span class=\"inputtooltipfield\"></span><span class=\"inputvalidatorfield\"><span id=\"compCashCredit\" style=\"color:red\">*</span></span>");
                output.Append("</div>");
                output.Append(
                    "<div class=\"twocolumn\"><label>Credit Card Items</label><span class=\"inputs\"><input name=\"tosubmit\" id=\"tosubmitcredit\" type=\""
                    + fieldType + "\" value=\"2\"");
                output.Append(" />");
                output.Append(
                    "</span><span class=\"inputicon\"></span><span class=\"inputtooltipfield\"></span><span class=\"inputvalidatorfield\"></span></div>");
                output.Append(
                    "<div class=\"twocolumn\"><label>Purchase Card Items</label><span class=\"inputs\"><input name=\"tosubmit\" id=\"tosubmitpurchase\" type=\""
                    + fieldType + "\" value=\"3\"");
                output.Append(" />");
                output.Append(
                    "</span><span class=\"inputicon\"></span><span class=\"inputtooltipfield\"></span><span class=\"inputvalidatorfield\"></span><span class=\"inputs\"></span><span class=\"inputicon\"></span><span class=\"inputtooltipfield\"></span><span class=\"inputvalidatorfield\"></span></div>");
                details.PartSubmittalForm = output.ToString();
            }



            #endregion

            #region approver list

            cEmployees clsemployees = new cEmployees(user.AccountID);
            Employee reqemp = clsemployees.GetEmployeeById(user.EmployeeID);
            cGroups clsgroups = new cGroups(user.AccountID);

            cGroup reqgroup = clsgroups.GetGroupById(reqemp.SignOffGroupID);
            SortedList lststages = clsgroups.sortStages(reqgroup);
            var stage = (cStage)lststages.GetByIndex(0);
            details.Approvers = clsclaims.GetApprover(reqclaim.Total, stage, reqemp.EmployeeID, user.AccountID, user.CurrentSubAccountId);

            #endregion

            #region Declaration

            if (reqProperties.ClaimantDeclaration)
            {
                details.Declaration = reqProperties.DeclarationMsg.Replace("\r\n", "<br />");
            }

            #endregion

            #region FlagMessage

            if (reqProperties.FlagMessage != string.Empty && reqclaim.HasFlaggedItems)
            {
                details.FlagMessage = reqProperties.FlagMessage.Replace("\r\n", "<br />");
            }
            else
                {
                details.FlagMessage = string.Empty;
                }

            #endregion

            #region Odometer Readings

            if (reqProperties.RecordOdometer && reqProperties.EnterOdometerOnSubmit)
            {
                cEmployeeCars clsEmployeeCars = new cEmployeeCars(user.AccountID, user.EmployeeID);

                var cars = clsEmployeeCars.GetActiveCars().Where(e => e.fuelcard == true).ToList();
                details.OdometerRequired = cars.Count != 0;

                if (details.OdometerRequired)
                {
                    output = new StringBuilder();

                    DateTime startDate = DateTime.Today;
                    DateTime earliestDate = DateTime.Today;
                    cClaims claims = new cClaims(user.AccountID);
                    details.ClaimIncludesFuelCardMileage = claims.IncludesFuelCardMileage(claimId);
                    List<OdometerReading> odometerReadings = new List<OdometerReading>();
                    OdometerReading odometerReading;
                    int i = 0;
                    cOdometerReading reading;
                    int oldreading;
                    foreach (cCar car in cars)
                    {
                        odometerReading = new OdometerReading();
                        odometerReading.CarID = car.carid;

                        reading = car.getLastOdometerReading();
                        if (reading == null)
                        {
                            oldreading = 0;
                            startDate = DateTime.Now;
                        }
                        else
            {
                            oldreading = reading.newreading;
                            startDate = reading.datestamp;
            }

                        odometerReading.LastReadingDate = startDate.ToShortDateString();
                        odometerReading.LastReading = oldreading;
                        odometerReading.CarMakeModel = car.make + " " + car.model;
                        odometerReading.CarRegistration = car.registration.ToUpper();
                        odometerReadings.Add(odometerReading);

                        if (startDate < earliestDate)
            {
                            earliestDate = startDate;
                        }

                        i++;
                    }

                    details.OdometerReadings = odometerReadings;
                    details.ShowBusinessMileage = clsemployees.getBusinessMiles(reqclaim.employeeid, earliestDate, DateTime.Today) == 0;

                    //details.OdometerReadingForm = output.ToString();
                }

            }

            #endregion

            return details;
        }

        /// <summary>
        /// Saves the justification provided by the authoriser
        /// </summary>
        /// <param name="justifications">The justifications provided by the claimant</param>
        [WebMethod(EnableSession = true)]
        public void SaveClaimantJustifications(List<List<object>> justifications)
        {
            CurrentUser user = cMisc.GetCurrentUser();
            FlagManagement flagMan = new FlagManagement(user.AccountID);
            foreach (List<object> o in justifications)
            {
                flagMan.SaveClaimantJustification(Convert.ToInt32(o[0]), o[1].ToString(), user);
            }
            }

        /// <summary>
        /// Saves the justification provided by the authoriser
        /// </summary>
        /// <param name="justifications">The justifications to save</param>
        [WebMethod(EnableSession = true)]
        public void SaveAuthoriserJustifications(List<List<object>> justifications)
        {
            CurrentUser user = cMisc.GetCurrentUser();
            FlagManagement flagMan = new FlagManagement(user.AccountID);

            
            foreach (List<object> o in justifications)
            {
                flagMan.SaveAuthoriserJustification(Convert.ToInt32(o[0]), o[1].ToString(), user.EmployeeID, user);
            }
        }



        /// <summary>
        /// Given a string builder and a list of results, populates an html table with the result set.
        /// </summary>
        /// <param name="builder">The string builder.</param>
        /// <param name="results">The result set.</param>
        /// <param name="headerIcon">The url to the header icon</param>
        /// <returns>The number of rows added</returns>
        private void BuildTableForValidationResultSet(
            StringBuilder builder,
            List<ValidationResult> results,
            string headerIcon)
        {
            if (results.Any())
            {
                // get rid of any that don't apply
                results.RemoveAll(r => r.Status == ExpenseValidationResultStatus.NotApplicable);
                
                builder.AppendFormat(
                    "<table class='validation-results'><tr class='validation-results-header cGrid'><th class='validation-results-label cGrid'>Validation Rules</th><th class='validation-results-status cGrid'><img src='{0}' alt='Validation Status' title='Validation Status' /></th></tr>",
                    headerIcon);
                results.ForEach(
                    result =>
                {
                    // do the business case first
                    builder.Append("<tr>");

                    // check if the validator has added comments
                            var commentsIfAny = string.IsNullOrWhiteSpace(result.Comments)
                                                    ? string.Empty
                                                    : string.Format(
                                                        "<br/><span class='important validator-comments'>Validator comments: {0}</span>",
                                                        result.Comments);

                    // requirements
                            builder.AppendFormat(
                                "<td title='{0}' class='validation-results-rule'><span>{1}</span>{2}</td>",
                                result.TypeIconTooltip,
                                result.FriendlyMessage,
                                commentsIfAny);
                    
                    // result icon
                            builder.AppendFormat(
                                "<td class='validation-results-status'><img src='{0}' alt='{1}' title='{1}'/></td>",
                                result.StatusIconUrl,
                                result.StatusIconTooltip);

                    builder.Append("</tr>");
                });

                // close table
                builder.Append("</table>");
            }
            else
            {
                builder.Append("<div>There are no validation results for this type of rule.</div>");
            }
        }



       




        /// <summary>
        /// A temp working object for working with the UpdateReceiptTree method above.
        /// </summary>
        internal class NodeParentPairing
        {
            public NodeParentPairing()
            {
                
            }

            public NodeParentPairing(IdDTONode node, int parentId, ReceiptPairingType parentType)
            {
                // parse info for this node
                Type = !node.isFolder
                    ? (node.id.Contains("clone_") ? ReceiptPairingType.ReceiptClone : ReceiptPairingType.Receipt)
                    : ((node.id.Contains("claim_")
                        ? ReceiptPairingType.Claim
                        : (node.id.Contains("header_")
                            ? ReceiptPairingType.ClaimHeader
                            : (node.id.Contains("expenseItem_")
                                ? ReceiptPairingType.ExpenseItem
                                : ReceiptPairingType.Unknown))));

                if (Type == ReceiptPairingType.Unknown) Id = 0;
                else if (Type == ReceiptPairingType.Receipt) Id = int.Parse(node.id);
                else Id = int.Parse(node.id.Split(Convert.ToChar("_")).Last());

                
                // Id = Type == ReceiptPairingType.Unknown ? 0 : Type == ReceiptPairingType.Receipt ? int.Parse(node.id) : int.Parse(node.id.Split(Convert.ToChar("_")).Last());

                // parent info
                ParentId = parentId;
                ParentType = parentType;
                
                // recurse
                Children = new List<NodeParentPairing>();
                foreach (var child in node.children.Select(childNode => new NodeParentPairing(childNode, Id, Type)))
                {
                    Children.Add(child);
                }
                
            }

            public int Id { get; set; }

            public ReceiptPairingType Type { get; set; }

            public int ParentId { get; set; }

            public ReceiptPairingType ParentType { get; set; }

            public List<NodeParentPairing> Children { get; set; }

            public IEnumerable<NodeParentPairing> Descendants()
            {
                var nodes = new Stack<NodeParentPairing>(new[] { this });
                while (nodes.Any())
                {
                    var node = nodes.Pop();
                    yield return node;
                    foreach (var n in node.Children) nodes.Push(n);
                }
            }
        }
        
        internal enum ReceiptPairingType
        {
            Unknown = -1,

            Claim = 0,

            ClaimHeader = 1,

            ExpenseItem = 2,

            Receipt = 3,
         
            ReceiptClone = 4
        }

    }

    /// <summary>
    /// Represents the result of a user declaring that their envelope is 
    /// either lost in the post or they haven't sent it.
    /// </summary>
    [Serializable]
    public class EnvelopeMissingStatus
    {
        /// <summary>
        /// The Id of the Envelope.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The EnvelopeNumber - not needed but goes down to the view and so gets returned.
        /// </summary>
        public string EnvelopeNumber { get; set; }

        /// <summary>
        /// The date assumed to have been posted.
        /// </summary>
        public string DatePosted { get; set; }

        /// <summary>
        /// Whether the Envelope has been sent.
        /// </summary>
        public bool? HasSent { get; set; }
    }

}
    
    

