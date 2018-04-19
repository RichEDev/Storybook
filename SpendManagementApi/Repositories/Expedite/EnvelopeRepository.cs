namespace SpendManagementApi.Repositories.Expedite
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Interfaces;
    using Models.Common;
    using Models.Types.Expedite;
    using Utilities;
    using SpendManagementLibrary.Interfaces.Expedite;
    using Spend_Management;
    using SpendManagementLibrary;
    using SpendManagementLibrary.Expedite;

    using Spend_Management.expenses.code;

    using Envelope = SpendManagementApi.Models.Types.Expedite.Envelope;

    /// <summary>
    /// EnvelopeRepository manages data access for Envelopes.
    /// </summary>
    internal class EnvelopeRepository : BaseRepository<Envelope>, ISupportsActionContext
    {
        /// <summary>
        /// An instance of <see cref="IManageEnvelopes"/>.
        /// </summary>
        private readonly IManageEnvelopes _data;

        /// <summary>
        /// An instance of <see cref="IActionContext"/>.
        /// </summary>
        private readonly IActionContext _actionContext = null;

        /// <summary>
        /// Creates a new EnvelopeRepository with the passed in user.
        /// </summary>
        /// <param name="user">The current user.</param>
        /// <param name="actionContext">An implementation of ISupportsActionContext</param>
        public EnvelopeRepository(ICurrentUser user, IActionContext actionContext)
            : base(user, actionContext, x => x.Id, x => x.EnvelopeNumber)
        {
            _data = ActionContext.Envelopes;
            _actionContext = ActionContext;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EnvelopeRepository"/> class.
        /// </summary>
        public EnvelopeRepository()
        {
            _data = new Envelopes();          
        }

        /// <summary>
        /// Gets all the Envelopes within the system.
        /// </summary>
        /// <returns>A list of envelopes.</returns>
        public override IList<Envelope> GetAll()
        {
            return _data.GetAllEnvelopes().Select(e => new Envelope().From(e, _actionContext)).ToList();
        }

        /// <summary>
        /// Gets all the Envelopes for a specific Account.
        /// </summary>
        /// <returns>A list of envelopes.</returns>
        public IList<Envelope> GetByAccount(int accountId)
        {
            return _data.GetEnvelopesByAccount(accountId).Select(e => new Envelope().From(e, _actionContext)).ToList();
        }

        /// <summary>
        /// Gets all the Envelopes for a specific batch.
        /// </summary>
        /// <returns>A list of envelopes.</returns>
        public IList<Envelope> GetByBatchCode(string batchCode)
        {
     
            return _data.GetEnvelopesByBatch(batchCode).Select(e => new Envelope().From(e, _actionContext)).ToList();
        }

        /// <summary>
        /// Gets an envelope by it's EnvelopeId.
        /// </summary>
        /// <param name="id">The Id of the envelope.</param>
        /// <returns>The envelope with the matching Id.</returns>
        public override Envelope Get(int id)
        {
            return new Envelope().From(_data.GetEnvelopeById(id), _actionContext);
        }

        /// <summary>
        /// Gets envelopes by EnvelopeNumber. There should only be one,
        /// but sometimes this might not be the case.
        /// </summary>
        /// <param name="envelopeNumber">The EnvelopeNumber of the envelope.</param>
        /// <returns>The envelope(s) with the matching EnvelopeNumber.</returns>
        public IList<Envelope> GetByEnvelopeNumber(string envelopeNumber)
        {
            return
                _data.GetEnvelopesByEnvelopeNumber(envelopeNumber)
                    .Select(e => new Envelope().From(e, _actionContext))
                    .ToList();
        }

        /// <summary>
        /// Gets envelopes by a ClaimReferenceNumber.
        /// </summary>
        /// <param name="claimReferenceNumber">The ClaimReferenceNumber of the envelopes.</param>
        /// <returns>The envelopes with the matching ClaimReferenceNumber.</returns>
        public IList<Envelope> GetByClaimReferenceNumber(string claimReferenceNumber)
        {
            return
                _data.GetEnvelopesByClaimReferenceNumber(claimReferenceNumber)
                    .Select(e => new Envelope().From(e, _actionContext))
                    .ToList();
        }

        /// <summary>
        /// Creates an envelope in the system and returns the newly created envelope.
        /// </summary>
        /// <param name="item">The item to create.</param>
        /// <returns>The newly created envelope.</returns>
        public override Envelope Add(Envelope item)
        {
            if (item.Status == EnvelopeStatus.Unknown)
            {
                item.Status = EnvelopeStatus.Generated;
            }

            if (!item.EnvelopeType.HasValue)
            {
                item.EnvelopeType = 1;
            }
            return new Envelope().From(_data.AddEnvelope(item.To(_actionContext), User), _actionContext);
        }

        /// <summary>
        /// Creates a batch of envelopes in the system and returns the newly create envelopes.
        /// </summary>
        /// <param name="envelopeType">The type of the envelope, from EnvelopeTypes.</param>
        /// <returns>A list of the newly created envelopes.</returns>
        public IList<Envelope> AddBatch(int envelopeType)
        {
            return _data.AddEnvelopeBatch(envelopeType).Select(e => new Envelope().From(e, _actionContext)).ToList();
        }

        /// <summary>
        /// Creates a batch of envelopes in the system and returns the envelopes with populated Ids.
        /// </summary>
        /// <returns>A list of the newly created envelopes.</returns>
        public IList<Envelope> AddBatch(IList<Envelope> items)
        {
            items.ToList().ForEach(i =>
            {
                if (i.Status == EnvelopeStatus.Unknown)
                {
                    i.Status = EnvelopeStatus.Generated;
                }

                if (!i.EnvelopeType.HasValue)
                {
                    i.EnvelopeType = 1;
                }
            });
            return _data.AddEnvelopeBatch(items.Select(e => e.To(_actionContext)).ToList(), User).Select(e => new Envelope().From(e, _actionContext)).ToList();
        }

        /// <summary>
        /// Edits an envelope. Ensure the EnvelopeId is set correctly.
        /// </summary>
        /// <param name="item">The envelope to edit.</param>
        /// <returns>The edited envelope.</returns>
        public override Envelope Update(Envelope item)
        {
            return new Envelope().From(_data.EditEnvelope(item.To(_actionContext), User), _actionContext);
        }

        /// <summary>
        /// Edits a batch of envelopes. Ensure the EnvelopeId is set correctly in each.
        /// </summary>
        /// <param name="items">The envelope to edit.</param>
        /// <returns>The edited envelopes.</returns>
        public IList<Envelope> UpdateBatch(IList<Envelope> items)
        {
            return _data.EditEnvelopeBatch(items.Select(i => i.To(_actionContext)).ToList()).Select(e => new Envelope().From(e, _actionContext)).ToList();
        }

        /// <summary>
        /// Utility method for issuing a single envelope to an account.
        /// </summary>
        /// <param name="envelopeId">The EnvelopeId of the envelope to issue.</param>
        /// <param name="accountId">The AccountId of the account to issue the envelope to.</param>
        /// <returns>The newly modified envelope.</returns>
        public Envelope IssueToAccount(int envelopeId, int accountId)
        {
            return new Envelope().From(_data.IssueToAccount(envelopeId, accountId, User), _actionContext);
        }

        /// <summary>
        /// Utility method for issuing multiple envelopes to an account.
        /// </summary>
        /// <param name="batchCode">The central part of the envelope number (the batch code) to select the envelopes to issue.</param>
        /// <param name="accountId">The AccountId of the account to issue the envelopes to.</param>
        /// <returns>The newly modified envelopes.</returns>
        public IList<Envelope> IssueToAccount(string batchCode, int accountId)
        {
            return _data.IssueBatchToAccount(batchCode, accountId).Select(e => new Envelope().From(e, _actionContext)).ToList();
        }

        /// <summary>
        /// Utility method for attaching a single envelope to a claim.
        /// </summary>
        /// <param name="envelopeId">The EnvelopeId of the envelope to issue.</param>
        public Envelope AssignToAccountAdmin(int envelopeId)
        {
            var envelope = TryGetEnvelopeAndThrow(envelopeId);
            envelope.AccountId = User.AccountID;
            envelope.Status = EnvelopeStatus.PendingAdminReassignment;
            var result = new Envelope().From(_data.EditEnvelope(envelope.To(_actionContext), User), _actionContext);

            // drop the administrator an email.
            var properties = ActionContext.SubAccounts.getFirstSubAccount().SubAccountProperties;
            var notifications = ActionContext.Notifications;
            var claim = ActionContext.Claims.getClaimById(envelope.ClaimId.Value);
            var expenseItemIds = ActionContext.Claims.getExpenseItemsFromDB(claim.claimid).Keys.ToList();
            notifications.SendMessage(SendMessageEnum.GetEnumDescription(SendMessageDescription.SentToAnAccountAdministratorWhenAPreviouslyUnidentifiedEnvelopeHasBeenAttachedToTheirAccount), User.EmployeeID, new[] { properties.MainAdministrator }, expenseItemIds);

            return result;
        }

        /// <summary>
        /// Utility method for attaching a single envelope to a claim.
        /// </summary>
        /// <param name="envelopeId">The EnvelopeId of the envelope to issue.</param>
        /// <param name="claimId">The ClaimId of the claim to attach the envelope to.</param>
        public Envelope AttachToClaim(int envelopeId, int claimId)
        {
            var envelope = TryGetEnvelopeAndThrow(envelopeId);
            envelope.ClaimId = claimId;
            var claim = TryGetClaimAndThrow(envelope);

            return new Envelope().From(_data.AttachToClaim(envelopeId, claim, User), _actionContext);
        }

        /// <summary>
        /// Marks a single <see cref="Envelope">Envelope</see> as received at SEL.
        /// </summary>
        /// <param name="id">The id of the <see cref="Envelope">Envelope</see> to update.</param>
        /// <returns>The updated Envelope.</returns>
        public Envelope MarkReceived(int id)
        {
            var envelope = TryGetEnvelopeAndThrow(id);

            // make sure to throw in light of a missing claim.
            var claim = TryGetClaimAndThrow(envelope);

            // mark received.
            envelope = envelope.From(_data.MarkReceived(id, User), _actionContext);

            // update claim history.
            var history = string.Format("Envelope: {0} received for scan & attach.", envelope.EnvelopeNumber);
            _actionContext.Claims.UpdateClaimHistory(claim, history, User.EmployeeID);

            // if the signoff group settings say so, send a notification.
            var employee = _actionContext.Employees.GetEmployeeById(claim.employeeid);
            var signoffGroup = _actionContext.SignoffGroups.GetGroupById(employee.SignOffGroupID);
            if (signoffGroup.NotifyClaimantWhenEnvelopeReceived == true)
            {
                var notifications = ActionContext.Notifications;
                var expenseItemIds = ActionContext.Claims.getExpenseItemsFromDB(claim.claimid).Keys.ToList();
                notifications.SendMessage(SendMessageEnum.GetEnumDescription(SendMessageDescription.SentToAClaimantWhenTheirEnvelopeIsReceivedForScanAndAttach), User.EmployeeID, new[] { claim.employeeid }, expenseItemIds);
            }

            return envelope;
        }

        /// <summary>
        /// Marks a single <see cref="Envelope">Envelope</see> as completed scanning and attach by SEL.
        /// </summary>
        /// <param name="id">The id of the <see cref="Envelope">Envelope</see> to update.</param>
        /// <returns>The updated Envelope.</returns>
        public Envelope MarkComplete(int id)
        {
            var envelope = TryGetEnvelopeAndThrow(id);
            _actionContext.AccountId = envelope.AccountId ?? User.AccountID;
            var claim = TryGetClaimAndThrow(envelope);

            // check that the claim is in a SELScanAttach stage.
            var claims = _actionContext.Claims;
            var claimEmployee = _actionContext.Employees.GetEmployeeById(claim.employeeid);
            var claimStages = claims.GetSignoffStagesAsTypes(claim, null, claimEmployee);

            if (claims.HasClaimPreviouslyPassedSelStage(claim, SignoffType.SELScanAttach, claimStages))
            {
                // mark complete and overwrite properties.
                envelope = envelope.From(_data.MarkComplete(id, User), _actionContext);

                // update claim history.
                var history = string.Format("Envelope scanned and attached: {0}.", envelope.EnvelopeNumber);
                claims.UpdateClaimHistory(claim, history, User.EmployeeID);
            }
            else
            {
                // mark complete and overwrite properties.
                envelope = envelope.From(_data.MarkComplete(id, User), _actionContext);

                // find the number of completed envelopes.
                int total, completed;
                var envelopesComplete = _data.AreAllEnvelopesCompleteForClaim(envelope.ClaimReferenceNumber, out total, out completed);


                // if all of the claim's envelopes are complete:
                if (!envelopesComplete)
                {
                    // update claim history.
                    var history = string.Format("Scan & attach complete for envelope: {0}. {1} remaining of {2} total.", envelope.EnvelopeNumber, completed, total);
                    claims.UpdateClaimHistory(claim, history, User.EmployeeID);
                }
                else
                {
                    // before advancing the claim, we need to check whether we need to prompt the user to declare matching is complete.
                    var allExpensesHaveReceipts = _actionContext.Receipts.CheckIfAllValidatableClaimLinesHaveReceiptsAttached(claim.claimid);
                    var canAdvance = !claimStages.Contains(SignoffType.SELValidation) || allExpensesHaveReceipts;

                    // update claim history.
                    claims.UpdateClaimHistory(claim, canAdvance
                        ? "Scan & attach complete for all envelopes in this claim."
                        : "All envelopes marked complete, but not all expenses have receipts attached.",
                        User.EmployeeID);

                    // only advance the claim if we can
                    if (canAdvance)
                    {
                        _actionContext.ClaimSubmission.SendClaimToNextStage(claim, false, User.EmployeeID, claim.employeeid, User.isDelegate ? User.Delegate.EmployeeID : (int?) null);
                    }
                    else
                    {
                        var notifications = ActionContext.Notifications;
                        var tempClaims = new cClaims(claim.accountid);
                        var expenseItemsIds = tempClaims.getExpenseItemsFromDB(claim.claimid).Keys.ToList();
                        notifications.SendMessage(SendMessageEnum.GetEnumDescription(SendMessageDescription.SentToAClaimantWhenAnReceiptMatchingForAllExpenseCannotBeFullyCompleted), User.EmployeeID, new[] { claim.employeeid }, expenseItemsIds);
                    }
                }
            }

            return envelope;
        }

        /// <summary>
        /// Utility method for updating the status of a single envelope.
        /// </summary>
        /// <param name="envelopeId">The EnvelopeId of the envelope to issue.</param>
        /// <param name="status">The new status of the envelope.</param>
        /// <returns>The newly modified envelope.</returns>
        public Envelope UpdateEnvelopeStatus(int envelopeId, EnvelopeStatus status)
        {
            var envelope = TryGetEnvelopeAndThrow(envelopeId);
            return envelope.From(_data.UpdateEnvelopeStatus(envelopeId, (SpendManagementLibrary.Enumerators.Expedite.EnvelopeStatus)status, User), _actionContext);
        }

        /// <summary>
        /// Utility method for updating the physical state of a single envelope.
        /// </summary>
        /// <param name="envelopeId">The EnvelopeId of the envelope to update.</param>
        /// <param name="states">The new Ids of the new physical states of the envelope.</param>
        /// <returns>The newly modified envelope.</returns>
        public Envelope UpdateEnvelopePhysicalState(int envelopeId, List<int> states)
        {
            var envelope = TryGetEnvelopeAndThrow(envelopeId);
            return envelope.From(_data.UpdateEnvelopesPhysicalStates(envelopeId, states), _actionContext);
        }

        /// <summary>
        /// Deletes an envelope, given it's ID.
        /// </summary>
        /// <param name="id">The Id of the Envelope to delete.</param>
        /// <returns>Null if the envelope was deleted successfully.</returns>
        public override Envelope Delete(int id)
        {
            var item = base.Delete(id);

            var result = _data.DeleteEnvelope(item.Id, User);
            if (!result)
            {
                throw new ApiException(ApiResources.ApiErrorDeleteUnsuccessful,
                    ApiResources.ApiErrorDeleteUnsuccessfulMessage);
            }

            return item;
        }

        /// <summary>
        /// Loops through all sent envelopes and notifies claimants that their envelopes
        /// have not been received, depending on the number of days set in their account.
        /// </summary>
        public void NotifyClaimantsOfUnsentEnvelopes()
        {
            var accounts = _actionContext.Accounts.GetAccountsWithReceiptServiceEnabled();
            var defaultDaysToWait = int.Parse(GlobalVariables.GetAppSetting("DefaultDaysToWaitBeforeEnvelopeIsMissing"));

            accounts.ForEach(account =>
            {
                // get the envelopes within the target date that are attached and not received.
                var days = account.DaysToWaitUntilSentEnvelopeIsMissing ?? defaultDaysToWait;
                var targetDate = DateTime.UtcNow.AddDays(-days);
                var envelopes = _data.GetEnvelopesByAccountWhichAreSentButNotReceived(account.accountid, targetDate).ToList();

                envelopes.ForEach(envelope =>
                {
                    if (envelope.AccountId.HasValue)
                    {
                        _actionContext.AccountId = envelope.AccountId.Value;
                        // ReSharper disable once PossibleInvalidOperationException
                        var claim = _actionContext.Claims.getClaimById(envelope.ClaimId.Value);

                        if (claim != null)
                        {
                            var employee = _actionContext.Employees.GetEmployeeById(claim.employeeid);

                            // update claim history.
                            var history =
                                string.Format(
                                    "Envelope: {0} not received for scan & attach within the given timeframe ({1} days).",
                                    envelope.EnvelopeNumber, days);
                            _actionContext.Claims.UpdateClaimHistory(claim, history, User.EmployeeID);

                            // if the signoff group settings say so, send a notification.
                            var signoffGroup = _actionContext.SignoffGroups.GetGroupById(employee.SignOffGroupID);
                            if (signoffGroup.NotifyClaimantWhenEnvelopeNotReceived == true)
                            {
                                var notifications = ActionContext.Notifications;
                                var expenseItemIds =new List<int>(ActionContext.Claims.getExpenseItemsFromDB(claim.claimid).Keys);
                                notifications.SendMessage(SendMessageEnum.GetEnumDescription(SendMessageDescription.SentToAClaimantWhenTheirEnvelopeIsMarkedAsSentButHasNotBeenReceivedAfterAspecifiedNumberOfDays), employee.EmployeeID, new[] { claim.employeeid }, expenseItemIds);
                            }

                            // update the envelope so it can be identified and also doesn't trigger this again.
                            _data.UpdateEnvelopeStatus(envelope.EnvelopeId,
                                SpendManagementLibrary.Enumerators.Expedite.EnvelopeStatus.UnconfirmedNotSent, User);
                        }
                    }
                });
            });
        }

        /// <summary>
        /// Calls get and throws an error if the envelope doesn't exist.
        /// </summary>
        /// <param name="id">The id of the envelope.</param>
        /// <returns>The Envelope.</returns>
        private Envelope TryGetEnvelopeAndThrow(int id)
        {
            var envelope = Get(id);

            if (envelope == null)
            {
                throw new InvalidDataException(ApiResources.ApiErrorEnvelopeDoesntExist);
            }

            return envelope;
        }

        /// <summary>
        /// Attempts to get a claim from an envelope. Throws if claim doesn't exist.
        /// </summary>
        /// <param name="envelope">The envelope, who's claim Id will be read to attemp to find the claim.</param>
        /// <returns>The claim.</returns>
        private cClaim TryGetClaimAndThrow(Envelope envelope)
        {
            if (!envelope.ClaimId.HasValue)
            {
                throw new InvalidDataException(ApiResources.ApiErrorEnvelopeDoesntHaveClaim);
            }

            var claims = _actionContext.Claims;
            var claim = claims.getClaimById(envelope.ClaimId.Value);

            if (claim == null)
            {
                throw new InvalidDataException(ApiResources.ApiErrorClaimDoesntExist);
            }

            return claim;
        }

    }
}