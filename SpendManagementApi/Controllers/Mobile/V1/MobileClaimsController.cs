namespace SpendManagementApi.Controllers.Mobile.V1
{
    using System;
    using System.Collections.Generic;
    using System.Web.Http;
    using System.Web.Http.Description;

    using SpendManagementApi.Attributes;
    using SpendManagementApi.Attributes.Mobile;

    using SpendManagementLibrary;
    using SpendManagementLibrary.Flags;

    using Spend_Management;
    using Spend_Management.expenses.code.Claims;

    using cExpenseItemResult = SpendManagementLibrary.Mobile.cExpenseItemResult;
    using Claim = SpendManagementLibrary.Mobile.Claim;
    using ClaimToCheckCountResult = SpendManagementLibrary.Mobile.ClaimToCheckCountResult;
    using ClaimToCheckResult = SpendManagementLibrary.Mobile.ClaimToCheckResult;
    using ExpenseItemsResult = SpendManagementLibrary.Mobile.ExpenseItemsResult;
    using ServiceResultMessage = SpendManagementLibrary.Mobile.ServiceResultMessage;
    using SpendManagementLibrary.Claims;

    /// <summary>
    /// The controller to handle all claims awaiting approval by the mobile user.
    /// </summary>
    [RoutePrefix("mobile/claims")]
    [Version(1)]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class MobileClaimsV1Controller : BaseMobileApiController
    {
        /// <summary>
        /// Approves an item on a claim.
        /// </summary>
        /// <param name="expenseId">The expense item to approve</param>
        /// <returns>A <see cref="ServiceResultMessage"/> detailing success or failure.</returns>
        [HttpPut]
        [MobileAuth]
        [Route("items/{expenseId}/approve")]
        public ServiceResultMessage ApproveItem(int expenseId)
        {
            ServiceResultMessage result = new ServiceResultMessage { FunctionName = "ApproveItems", ReturnCode = this.ServiceResultMessage.ReturnCode };

            switch (this.ServiceResultMessage.ReturnCode)
            {
                case MobileReturnCode.Success:
                    try
                    {
                        if (cAccessRoles.CanCheckAndPay(this.PairingKeySerialKey.PairingKey.AccountID, this.PairingKeySerialKey.PairingKey.EmployeeID))
                        {
                            cClaims clsclaims = new cClaims(this.PairingKeySerialKey.PairingKey.AccountID);
                            List<int> approveItems = new List<int> { expenseId };
                            var item = clsclaims.getExpenseItemById(expenseId);
                            
                            clsclaims.AllowExpenseItems(this.PairingKeySerialKey.PairingKey.AccountID, this.PairingKeySerialKey.PairingKey.EmployeeID, null, item.claimid, approveItems);
                            
                            result.Message = "Approved";
                        }
                        else
                        {
                            result.ReturnCode = MobileReturnCode.EmployeeHasInsufficientPermissions;
                        }
                    }
                    catch (Exception ex)
                    {
                        cEventlog.LogEntry("MobileAPI.approveItems():Error:{ Pairingkey: " + this.PairingKeySerialKey.PairingKey + "\nSerialKey: " + this.PairingKeySerialKey.SerialKey + "\nMessage: " + ex.Message + " }", true, System.Diagnostics.EventLogEntryType.Information, cEventlog.ErrorCode.DebugInformation);

                        // ReSharper disable once PossibleIntendedRethrow
                        throw ex;
                    }

                    break;
            }

            return result;
        }

        /// <summary>
        /// Unapproves an expense item on a claim.
        /// </summary>
        /// <param name="expenseid">The expense item to unapprove</param>
        /// <returns>A <see cref="ServiceResultMessage"/> detailing success or failure.</returns>
        [HttpPut]
        [MobileAuth]
        [Route("items/{expenseid}/unapprove")]
        public ServiceResultMessage UnapproveItem(int expenseid)
        {
            ServiceResultMessage result = new ServiceResultMessage { FunctionName = "UnapproveItem", ReturnCode = this.ServiceResultMessage.ReturnCode };

            switch (this.ServiceResultMessage.ReturnCode)
            {
                case MobileReturnCode.Success:
                    try
                    {
                        if (cAccessRoles.CanCheckAndPay(this.PairingKeySerialKey.PairingKey.AccountID, this.PairingKeySerialKey.PairingKey.EmployeeID))
                        {
                            cClaims clsclaims = new cClaims(this.PairingKeySerialKey.PairingKey.AccountID);

                            cExpenseItem expItem = clsclaims.getExpenseItemById(expenseid);

                            if (expItem != null)
                            {
                                clsclaims.UnapproveItem(expItem);
                                result.Message = "Unapproved";
                            }
                        }
                        else
                        {
                            result.ReturnCode = MobileReturnCode.EmployeeHasInsufficientPermissions;
                        }
                    }
                    catch (Exception ex)
                    {
                        cEventlog.LogEntry("MobileAPI.Claims.UnapproveItem():Error:{ Pairingkey: " + this.PairingKeySerialKey.PairingKey.Pairingkey + "\nSerialKey: " + this.PairingKeySerialKey.SerialKey + "\nEmployeeID: " + this.PairingKeySerialKey.PairingKey.EmployeeID + "\nMessage: " + ex.Message + " }", true, System.Diagnostics.EventLogEntryType.Information, cEventlog.ErrorCode.DebugInformation);

// ReSharper disable once PossibleIntendedRethrow
                        throw ex;
                    }

                    break;
            }

            return result;
        }

        /// <summary>
        /// Returns an expense item on a claim back to the claimant.
        /// </summary>
        /// <param name="expenseId">The expense item to unapprove</param>
        /// <param name="claimId">The claim the item is on</param>
        /// <param name="reason">The reason for returning the expense</param>
        /// <returns>A <see cref="ServiceResultMessage"/> detailing success or failure.</returns>
        [HttpPut]
        [MobileAuth]
        [Route("{claimId}/items/{expenseId}/return")]
        public ServiceResultMessage ReturnItem(int expenseId, int claimId, string reason)
        {
            ServiceResultMessage result = new ServiceResultMessage { FunctionName = "ReturnItems", ReturnCode = this.ServiceResultMessage.ReturnCode };

            switch (this.ServiceResultMessage.ReturnCode)
            {
                case MobileReturnCode.Success:
                    try
                    {
                        if (cAccessRoles.CanCheckAndPay(this.PairingKeySerialKey.PairingKey.AccountID, this.PairingKeySerialKey.PairingKey.EmployeeID))
                        {
                            cClaims clsclaims = new cClaims(this.PairingKeySerialKey.PairingKey.AccountID);
                            cClaim curClaim = clsclaims.getClaimById(claimId);
                            List<int> lstItems = new List<int> { expenseId };
                            clsclaims.ReturnExpenses(curClaim, lstItems, reason, this.PairingKeySerialKey.PairingKey.EmployeeID, null);
                        }
                        else
                        {
                            result.ReturnCode = MobileReturnCode.EmployeeHasInsufficientPermissions;
                        }
                    }
                    catch (Exception ex)
                    {
                        cEventlog.LogEntry("MobileAPI.returnItems():Error:{ Pairingkey: " + this.PairingKeySerialKey.PairingKey + "\nSerialKey: " + this.PairingKeySerialKey.SerialKey + "\nClaimID: " + claimId + "\nReason: " + reason + "\nMessage: " + ex.Message + " }", true, System.Diagnostics.EventLogEntryType.Information, cEventlog.ErrorCode.DebugInformation);

// ReSharper disable once PossibleIntendedRethrow
                        throw ex;
                    }

                    break;
            }

            return result;
        }

        /// <summary>
        /// Allocates a claim for payment.
        /// </summary>
        /// <param name="claimid">The claim to allocate</param>
        /// <returns>A <see cref="ServiceResultMessage"/> detailing success or failure.</returns>
        [HttpPut]
        [MobileAuth]
        [Route("{claimid}/allocate")]
        public ServiceResultMessage Allocate(int claimid)
        {
            ServiceResultMessage result = new ServiceResultMessage { FunctionName = "AllocateClaimForPayment", ReturnCode = this.ServiceResultMessage.ReturnCode };

            switch (this.ServiceResultMessage.ReturnCode)
            {
                case MobileReturnCode.Success:
                    try
                    {
                        if (cAccessRoles.CanCheckAndPay(this.PairingKeySerialKey.PairingKey.AccountID, this.PairingKeySerialKey.PairingKey.EmployeeID))
                        {
                            cClaims clsclaims = new cClaims(this.PairingKeySerialKey.PairingKey.AccountID);
                            cClaim claim = clsclaims.getClaimById(claimid);

                            if (clsclaims.AllowClaimProgression(claimid) == 2)
                            {
                                clsclaims.payClaim(claim, this.PairingKeySerialKey.PairingKey.EmployeeID, null);
                            }
                            else
                            {
                                result.Message = "-1";
                                result.ReturnCode = MobileReturnCode.AllocateForPaymentFailed;
                            }
                        }
                        else
                        {
                            result.ReturnCode = MobileReturnCode.EmployeeHasInsufficientPermissions;
                        }
                    }
                    catch (Exception ex)
                    {
                        cEventlog.LogEntry("MobileAPI.AllocateClaimForPayment():Error:{ Pairingkey: " + this.PairingKeySerialKey.PairingKey + "\nSerialKey: " + this.PairingKeySerialKey.SerialKey + "\nClaimID: " + claimid + "\nMessage: " + ex.Message + " }", true, System.Diagnostics.EventLogEntryType.Information, cEventlog.ErrorCode.DebugInformation);

// ReSharper disable once PossibleIntendedRethrow
                        throw ex;
                    }

                    break;
            }

            return result;
        }

        /// <summary>
        /// Approves a claim, sending to the next stage if applicable.
        /// </summary>
        /// <param name="claimid">The claim to approve</param>
        /// <returns>A <see cref="ServiceResultMessage"/> detailing success or failure.</returns>
        [HttpPut]
        [MobileAuth]
        [Route("{claimid}/approve")]
        public ServiceResultMessage ApproveClaim(int claimid)
        {
            ServiceResultMessage result = new ServiceResultMessage { FunctionName = "ApproveClaim", ReturnCode = this.ServiceResultMessage.ReturnCode };

            switch (this.ServiceResultMessage.ReturnCode)
            {
                case MobileReturnCode.Success:
                    try
                    {
                        if (cAccessRoles.CanCheckAndPay(this.PairingKeySerialKey.PairingKey.AccountID, this.PairingKeySerialKey.PairingKey.EmployeeID))
                        {
                            cClaims clsclaims = new cClaims(this.PairingKeySerialKey.PairingKey.AccountID);
                            cClaim reqclaim = clsclaims.getClaimById(claimid);

                            if (clsclaims.AllowClaimProgression(claimid) == 1)
                            {
                                if (reqclaim != null && reqclaim.NumberOfUnapprovedItems > 0)
                                {
                                    result.Message = "1";
                                }

                                var claimSubmission = new ClaimSubmission(this.PairingKeySerialKey.PairingKey);
                                // store the return code and check, rather than potentially sending the claim to the next stage multiple times
                                SubmitClaimResult submitResult = claimSubmission.SendClaimToNextStage(reqclaim, false, 0, PairingKeySerialKey.PairingKey.EmployeeID, null);
                                switch (submitResult.Reason)
                                {
                                    case SubmitRejectionReason.ApproverOnHoliday:
                                        result.Message = "2";
                                        break;
                                    case SubmitRejectionReason.AssignmentSupervisorNotSpecified:
                                        result.Message = "5";
                                        break;
                                    case SubmitRejectionReason.CostCodeOwnerNotSpecified:
                                        result.Message = "6";
                                        break;
                                    case SubmitRejectionReason.CannotSignoffOwnClaim:
                                        result.Message = "3";
                                        break;
                                    case SubmitRejectionReason.UserNotAllowedToApproveOwnClaimDespiteSignoffGroup:
                                        result.Message = "4";
                                        break;
                                }
                            }
                            else
                            {
                                result.Message = "-1";
                                result.ReturnCode = MobileReturnCode.ApproveClaimFailed;
                            }
                        }
                        else
                        {
                            result.ReturnCode = MobileReturnCode.EmployeeHasInsufficientPermissions;
                            result.Message = string.Empty;
                        }
                    }
                    catch (Exception ex)
                    {
                        cEventlog.LogEntry("MobileAPI.approveClaim():Error:{ Pairingkey: " + this.PairingKeySerialKey.PairingKey + "\nSerialKey: " + this.PairingKeySerialKey.SerialKey + "\nClaimID: " + claimid + "\nMessage: " + ex.Message + " }", true, System.Diagnostics.EventLogEntryType.Information, cEventlog.ErrorCode.DebugInformation);

// ReSharper disable once PossibleIntendedRethrow
                        throw ex;
                    }

                    break;
            }

            return result;
        }

        /// <summary>
        /// Gets all of the expense items on a claim.
        /// </summary>
        /// <param name="claimId">The claim to get the expense items for.</param>
        /// <returns>A list of expense items contained in a <see cref="ExpenseItemsResult"/></returns>
        [HttpGet]
        [MobileAuth]
        [Route("{claimId}")]
        public ExpenseItemsResult ExpenseItems(int claimId)
        {
            ExpenseItemsResult result = new ExpenseItemsResult { FunctionName = "GetExpenseItemsByClaimID", ReturnCode = this.ServiceResultMessage.ReturnCode };

            switch (this.ServiceResultMessage.ReturnCode)
            {
                case MobileReturnCode.Success:
                    try
                    {
                        cSubcats clssubcats = new cSubcats(this.PairingKeySerialKey.PairingKey.AccountID);
                        cClaims clsclaims = new cClaims(this.PairingKeySerialKey.PairingKey.AccountID);
                        svcFlagRules flags = new svcFlagRules();
                        // ReSharper disable once SuggestUseVarKeywordEvident
                        List<cExpenseItemResult> items = new List<cExpenseItemResult>();

                        result.isApprover = cAccessRoles.CanCheckAndPay(this.PairingKeySerialKey.PairingKey.AccountID, this.PairingKeySerialKey.PairingKey.EmployeeID);

                        cClaim claim = clsclaims.getClaimById(claimId);
                        
                        SortedList<int, cExpenseItem> expenseItems = clsclaims.getExpenseItemsFromDB(claimId);
                        
                        foreach (var item in expenseItems.Values)
                        {
                            if (claim.checkerid == this.PairingKeySerialKey.PairingKey.EmployeeID || (item.itemCheckerId.HasValue && item.itemCheckerId.Value == this.PairingKeySerialKey.PairingKey.EmployeeID))
                            {
                                cExpenseItemResult details = new cExpenseItemResult { ExpenseItem = item };
                                if (item.subcatid > 0)
                                {
                                    details.Subcat = clssubcats.GetSubcatById(item.subcatid);
                                    details.Flags = flags.CreateFlagsGrid(
                                        claimId,
                                        item.expenseid.ToString(),
                                        "CheckAndPay",
                                        this.PairingKeySerialKey.PairingKey.AccountID,
                                        this.PairingKeySerialKey.PairingKey.EmployeeID);
                                    item.flags = new List<FlagSummary>();
                                }

                                items.Add(details);
                            }
                        }

                        result.List = items;
                    }
                    catch (Exception ex)
                    {
                        cEventlog.LogEntry("MobileAPI.getExpenseItemsByClaimID():Error:{ Pairingkey: " + this.PairingKeySerialKey.PairingKey + "\nSerialKey: " + this.PairingKeySerialKey.SerialKey + "\nClaimID: " + claimId + "\nMessage: " + ex.Message + " }", true, System.Diagnostics.EventLogEntryType.Information, cEventlog.ErrorCode.DebugInformation);

// ReSharper disable once PossibleIntendedRethrow
                        throw ex;
                    }

                    break;
            }

            return result;
        }

        /// <summary>
        /// Gets the list of claims awaiting approval.
        /// </summary>
        /// <returns>A list of claims contained in a <see cref="ClaimToCheckResult"/></returns>
        [HttpGet]
        [MobileAuth]
        [Route("")]
        public ClaimToCheckResult AwaitingApproval()
        {
            ClaimToCheckResult result = new ClaimToCheckResult { FunctionName = "GetClaimsAwaitingApproval", ReturnCode = this.ServiceResultMessage.ReturnCode };
            try
            {
                switch (this.ServiceResultMessage.ReturnCode)
                {
                    case MobileReturnCode.Success:
                        
                        result.isApprover = cAccessRoles.CanCheckAndPay(this.PairingKeySerialKey.PairingKey.AccountID, this.PairingKeySerialKey.PairingKey.EmployeeID);

                        if (!result.isApprover)
                        {
                            result.List = new List<Claim>();
                            break;
                        }

                        result.List = cClaims.GetClaimsAwaitingApproval(this.PairingKeySerialKey.PairingKey.AccountID, this.PairingKeySerialKey.PairingKey.EmployeeID);

                        break;
                }
            }
            catch (Exception ex)
            {
                cEventlog.LogEntry("MobileAPI.GetClaimsAwaitingApproval():Error:{ Pairingkey: " + this.PairingKeySerialKey.PairingKey + "\nSerialKey: " + this.PairingKeySerialKey.PairingKey + "\nMessage: " + ex.Message + " }", true, System.Diagnostics.EventLogEntryType.Information, cEventlog.ErrorCode.DebugInformation);

// ReSharper disable once PossibleIntendedRethrow
                throw ex;
            }

            return result;
        }

        /// <summary>
        /// Gets the count of claims awaiting approval by the mobile user.
        /// </summary>
        /// <returns>The number of claims awaiting approval as an <see cref="int"/> contained in a <see cref="ClaimToCheckCountResult"/></returns>
        [HttpGet]
        [MobileAuth]
        [Route("count")]
        public ClaimToCheckCountResult Count()
        {
            ClaimToCheckCountResult result = new ClaimToCheckCountResult { FunctionName = "GetClaimsAwaitingApprovalCount", ReturnCode = this.ServiceResultMessage.ReturnCode };
            try
            {
                if (this.ServiceResultMessage.ReturnCode == MobileReturnCode.Success)
                {
                    result.isApprover = cAccessRoles.CanCheckAndPay(this.PairingKeySerialKey.PairingKey.AccountID, this.PairingKeySerialKey.PairingKey.EmployeeID);

                    if (!result.isApprover)
                    {
                        result.Count = 0;
                    }

                    cClaims claims = new cClaims(this.PairingKeySerialKey.PairingKey.AccountID);

                    result.Count = claims.getClaimsToCheckCount(this.PairingKeySerialKey.PairingKey.EmployeeID, true, null);
                }
            }
            catch (Exception ex)
            {
                cEventlog.LogEntry("MobileAPI.GetClaimsAwaitingApprovalCount():Error:{ Pairingkey: " + this.PairingKeySerialKey.PairingKey.Pairingkey + "\nSerialKey: " + this.PairingKeySerialKey.SerialKey + "\nMessage: " + ex.Message + " }", true, System.Diagnostics.EventLogEntryType.Information, cEventlog.ErrorCode.DebugInformation);

// ReSharper disable once PossibleIntendedRethrow
                throw ex;
            }

            return result;
        }
    }
}
