
namespace Spend_Management.expenses.code.Claims
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;

    using BusinessLogic.DataConnections;
    using BusinessLogic.GeneralOptions;

    using SpendManagementLibrary;
    using SpendManagementLibrary.Employees;
    using SpendManagementLibrary.Enumerators;
    using SpendManagementLibrary.Enumerators.Expedite;
    using SpendManagementLibrary.Expedite;
    using SpendManagementLibrary.Flags;
    using SpendManagementLibrary.Helpers;
    using SpendManagementLibrary.Interfaces;
    using SpendManagementLibrary.Mobile;
    using SpendManagementLibrary.MobileDeviceNotifications;
    using SpendManagementLibrary.Claims;
    using Receipts = SpendManagementLibrary.Expedite.Receipts;

    using shared.code.ApprovalMatrix;

    /// <summary>
    ///     Methods for claim submission
    /// </summary>
    public class ClaimSubmission
	{
		private readonly cClaims _claims;
		private cAccountSubAccounts _subAccounts;
		private cAuditLog _auditLog;
		private cBudgetholders _budgetHolders;
		private cCostcodes _costCodes;
		private cEmployees _employees;
		private cExpenseItems _expenseItems;
		private cMisc _misc;
		private cGroups _groups;
		private cTeams _teams;

		private readonly ICurrentUser _user;

        private readonly IDataFactory<IGeneralOptions, int> _generalOptionsFactory = FunkyInjector.Container.GetInstance<IDataFactory<IGeneralOptions, int>>();

        internal enum GetClaimItemCheckerFor
        {
            All = 0,
            TeamsOnly = 1,
            IndividualsOnly = 2
        }

		/// <summary>
		///     Claim submission methods
		/// </summary>
		/// <param name="user"></param>
		public ClaimSubmission(ICurrentUser user)
		{
			_user = user;
			_claims = new cClaims(_user.AccountID);
		}

		/// <summary>
		/// </summary>
		/// <param name="pairing"></param>
		public ClaimSubmission(PairingKey pairing)
		{
			_user = new CurrentUser(pairing.AccountID, pairing.EmployeeID, 0, Modules.expenses, -1);
			_claims = new cClaims(pairing.AccountID);
		}

        public ClaimSubmission(int accountId, int employeeId)
        {
            _user = new CurrentUser(accountId, employeeId, 0, Modules.expenses, 1);
            _claims = new cClaims(accountId);
        }

		#region Public Methods

		/// <summary>
		/// Splits a claim.
		/// </summary>
		/// <param name="claim">The original claim.</param>
		/// <param name="cash">Whether cash was selected.</param>
		/// <param name="credit">Whether credit was selected.</param>
		/// <param name="purchase">Whether purchase was selected.</param>
		/// <returns></returns>
		public int SplitClaim(cClaim claim, bool cash, bool credit, bool purchase)
		{
			var newClaimId = _claims.insertDefaultClaim(claim.employeeid);
			this.moveItems(claim.claimid, newClaimId, cash, credit, purchase);
			return newClaimId;
		}

		/// <summary>
		/// Submit the claim.  Checks that the user can submit the claim before updating.
		/// </summary>
		/// <param name="claim">Claim object to update</param>
		/// <param name="cash">True if a cach claim</param>
		/// <param name="credit">True if a credit card claim</param>
		/// <param name="purchase">True if a purchase card claim</param>
		/// <param name="authoriser">the Employee ID of the authoriser</param>
		/// <param name="employeeid">The employee ID of the claimant</param>
		/// <param name="delegateid">The employee ID of the delegate</param>
		/// <param name="validating">Whether to override the default behaviour and just test the claim stages.</param>
		/// <returns>
		/// 1 - Claim has no items
		/// 4 - Claim value is less than the minimum claim allowed.
		/// 5 - Claim value is more than the maximum claim allowed.
		/// 8 - The claimant has already made a claim within the minimum time allowed.
		/// Any other int - Claim id, unless of course it is less than 9?!
		/// ZERO - Submitted</returns>
		public SubmitClaimResult SubmitClaim(cClaim claim, bool cash, bool credit, bool purchase, int authoriser, int employeeid,
			int? delegateid, byte viewfilter, bool validating = false, bool ContinueAlthoughAuthoriserIsOnHoliday = false)
		{


			SubmitClaimResult result = this.DetermineIfClaimCanBeSubmitted(claim, viewfilter, authoriser, ContinueAlthoughAuthoriserIsOnHoliday);
			if (result.Reason != SubmitRejectionReason.Success)
			{
				return result;
			}

			//check claim total
			result = this.CheckClaimTotal(claim, cash, credit, purchase);
			if (result.Reason != SubmitRejectionReason.Success)
			{
				return result;
			}


			if ((cash == false && claim.HasCashItems) || (credit == false && claim.HasCreditCardItems) || (purchase == false && claim.HasPurchaseCardItems))
			{
				int newClaimId = this.SplitClaim(claim, cash, credit, purchase);
				cClaim newclaim = this._claims.getClaimById(newClaimId);

				result = SubmitClaim(newclaim, cash, credit, purchase, authoriser, employeeid, delegateid, viewfilter, false, ContinueAlthoughAuthoriserIsOnHoliday);

				if (result.Reason == SubmitRejectionReason.Success || result.Reason == SubmitRejectionReason.ClaimSentToNextStage)
				{
					result.ClaimID = newClaimId;
				}

				return result;
			}


			if (!validating)
			{
				//insert first entry into claim history
				DateTime dateStamp = DateTime.Now.ToUniversalTime();
				InsertFirstEntryIntoClaimHistory(claim, employeeid, dateStamp, dateStamp);
			}


			result = SendClaimToNextStage(claim, true, authoriser, employeeid, delegateid);

			if (result.Reason != SubmitRejectionReason.Success && result.Reason != SubmitRejectionReason.ClaimSentToNextStage)
			{
				return result;
			}

			if (!validating)
			{
				claim.submitClaim();
				_auditLog = _auditLog ?? new cAuditLog(_user.AccountID, employeeid);
				_auditLog.editRecord(claim.claimid, claim.name, SpendManagementElement.Claims, new Guid("47DB6E7D-78AC-4322-8211-359DDCA0C1AB"), "Unsubmitted", "Submitted");
			}

			var log = new cAuditLog();
			log.editRecord(claim.claimid, claim.name, SpendManagementElement.Claims,
				new Guid("47DB6E7D-78AC-4322-8211-359DDCA0C1AB"), "Unsubmitted", "Submitted");

			return result;
		}

		public SubmitClaimResult DetermineIfClaimCanBeSubmitted(cClaim claim, byte viewfilter, int? approver = null, bool continueAlthoughAuthoriserIsOnHoliday = false)
		{
			this._employees = this._employees ?? new cEmployees(this._user.AccountID);
			this._subAccounts = this._subAccounts ?? new cAccountSubAccounts(this._user.AccountID);
			this._budgetHolders = this._budgetHolders ?? new cBudgetholders(this._user.AccountID);
			this._groups = this._groups ?? new cGroups(this._user.AccountID);
			this._misc = this._misc ?? new cMisc(this._user.AccountID);

			var result = new SubmitClaimResult { Reason = SubmitRejectionReason.Success };

			if (claim.submitted)
			{
				result.Reason = SubmitRejectionReason.AlreadySubmitted;
				return result;
			}

			if (claim.NumberOfItems == 0)
			{
				result.Reason = SubmitRejectionReason.NoItems;
				return result;
			}

			if (ClaimCanBeSubmittedBasedOnDefaultAuthoriser() == false)
			{
				result.NoDefaultAuthoriserPresent = true;
				return result;
			}
			else
			{
				result.NoDefaultAuthoriserPresent = false;
			}

			cAccountProperties globalProperties = this._subAccounts.getSubAccountById(this._user.CurrentSubAccountId).SubAccountProperties;
			if (_user.isDelegate && globalProperties.DelSubmitClaim == false)
			{
				result.Reason = SubmitRejectionReason.DelegatesProhibited;
				return result;
			}

			Employee claimemp = this._employees.GetEmployeeById(claim.employeeid);
			cGroup reqgroup = this._groups.GetGroupById(claimemp.SignOffGroupID);
			if (globalProperties.OnlyCashCredit && globalProperties.PartSubmit)
			{
				switch (claim.claimtype)
				{
					case ClaimType.Credit:
						reqgroup = this._groups.GetGroupById(claimemp.CreditCardSignOffGroup);
						break;
					case ClaimType.Purchase:
						reqgroup = this._groups.GetGroupById(claimemp.PurchaseCardSignOffGroup);
						break;
				}
			}

			if (reqgroup == null || reqgroup.stages.Count == 0)
			{
				result.Reason = SubmitRejectionReason.NoSignoffGroup;
				return result;
			}

			//approver on holiday
			if (!continueAlthoughAuthoriserIsOnHoliday)
			{
				if (reqgroup.stages.Values[0].signofftype == SignoffType.DeterminedByClaimantFromApprovalMatrix && approver.HasValue
					&& this._claims.userOnHoliday(SignoffType.Employee, approver.Value))
				{
					result.Reason = SubmitRejectionReason.ApproverOnHoliday;
					return result;
				}
			}

			// check to see if employees are allowed to sign off their own claim and then check the signoff
			if (globalProperties.AllowEmployeeInOwnSignoffGroup == false
				&& this._groups.ClaimantSubmittingClaimInSignoffGroup(reqgroup, claimemp, this._budgetHolders, this._employees))
			{
				result.Reason = SubmitRejectionReason.CannotSignoffOwnClaim;
				return result;
			}

			foreach (cStage stage in reqgroup.stages.Values)
			{
				if (stage.notify == 2)
				{
					if (!this.CanUserApproveOwnClaim(reqgroup, stage, claim, globalProperties, result, true, true))
					{
						return result;
					}

					claim.stage++;

					switch (stage.signofftype)
					{
						case SignoffType.LineManager:
							//does the employee have one?
							if (claimemp.LineManager == 0)
							{
								result.Reason = SubmitRejectionReason.NoLineManager;
								return result;
							}

							break;
						case SignoffType.CostCodeOwner:
							if (this._claims.UpdateItemCheckers(claim, ItemCheckerType.Normal, true) == -1)
							{
								result.Reason = SubmitRejectionReason.CostCodeOwnerNotSpecified;
								return result;
							}

							break;
						case SignoffType.AssignmentSignOffOwner:
							if (this._claims.UpdateItemCheckers(claim, ItemCheckerType.AssignmentSupervisor, true) == -1)
							{
								result.Reason = SubmitRejectionReason.AssignmentSupervisorNotSpecified;
								return result;
							}

							break;
					}
				}
			}

			claim.stage = 0;

			//frequency limit
			if (!CheckFrequency(claim.employeeid, globalProperties))
			{
				result.Reason = SubmitRejectionReason.FrequencyLimitBreached;
				result.FrequencyValue = globalProperties.FrequencyValue;
				result.FrequencyPeriod = globalProperties.FrequencyType;
				return result;
			}

			//revalidate flags
			FlagManagement flagMan = new FlagManagement(_user.AccountID);

			//revalidate flags to see if claim needs stopping
			bool hasBlockedItems;
			FlaggedItemsManager flagResults = flagMan.RevalidateClaim(claim.claimid, claim.employeeid, out hasBlockedItems, _user);

			if (hasBlockedItems)
			{
				result.Reason = SubmitRejectionReason.OutstandingFlags;
				result.FlagResults = flagResults;
				return result;
			}


			return result;
		}

		/// <summary>
		/// Determines with the user can approve their own claim based on the stage in the group
		/// </summary>
		/// <param name="group">
		/// The group to be checked
		/// </param>
		/// <param name="stage">
		/// The stage to be checked
		/// </param>
		/// <param name="claim">
		/// The claim to review
		/// </param>
		/// <param name="globalProperties">
		/// An instance of global properties
		/// </param>
		/// <param name="result">
		/// The current submitclaimresult to update with the outcome
		/// </param>
		/// <param name="prevalidating">
		/// Is the claim currently being prevalidated prior to actual submission
		/// </param>
		/// <returns>
		/// Whether the claimant can approve their own claim or not
		/// </returns>
		public bool CanUserApproveOwnClaim(
			cGroup group,
			cStage stage,
			cClaim claim,
			cAccountProperties globalProperties,
			SubmitClaimResult result,
			bool prevalidating,
			bool submitting)
		{
			if (stage.signofftype == SignoffType.CostCodeOwner)
			{
				var employeeResult = GetExpenseItemCheckerIds(
					claim.claimid,
					claim.employeeid,
					_user.CurrentSubAccountId,
					GetClaimItemCheckerFor.IndividualsOnly);
				var employeeList = employeeResult as IList<int> ?? employeeResult.ToList();
				if (!globalProperties.AllowEmployeeInOwnSignoffGroup)
				{
					if (employeeList.Contains(claim.employeeid))
					{
						result.Reason = SubmitRejectionReason.UserNotAllowedToApproveOwnClaimDespiteSignoffGroup;
						return false;
					}
				}

				var teamResult = GetExpenseItemCheckerIds(
					claim.claimid,
					claim.employeeid,
					_user.CurrentSubAccountId,
					GetClaimItemCheckerFor.TeamsOnly);
				var teamList = teamResult as IList<int> ?? teamResult.ToList();
				if (!globalProperties.AllowTeamMemberToApproveOwnClaim)
				{
					if (teamList.Contains(claim.employeeid))
					{
						teamList.Remove(claim.employeeid);
					}
				}

				if ((!teamList.Any() && teamResult.Any()) || (!employeeList.Any() && employeeResult.Any()))
				{
					result.Reason = SubmitRejectionReason.UserNotAllowedToApproveOwnClaimDespiteSignoffGroup;
					return false;
				}
			}
			else
			{
				if (globalProperties.AllowEmployeeInOwnSignoffGroup == false
					&& stage.signofftype != SignoffType.SELScanAttach)
				{
					// are they approving their own claim currently?
					if (!submitting && !prevalidating
						&& ((_user.EmployeeID == claim.employeeid
							 || (_user.isDelegate && _user.Delegate.EmployeeID > 0
								 && _user.Delegate.EmployeeID == claim.employeeid))))
					{
						result.Reason = SubmitRejectionReason.UserNotAllowedToApproveOwnClaim;
						return false;
					}

					// are they submitting a claim and in the signoff group at all?
					if (submitting
						&& (_groups.EmployeeInSignoffGroup(group.groupid, claim.employeeid)
							|| (stage.signofftype == SignoffType.AssignmentSignOffOwner)
							&& (GetExpenseItemCheckerIds(
								claim.claimid,
								claim.employeeid,
								_user.CurrentSubAccountId).Contains(claim.employeeid))))
					{
						result.Reason = SubmitRejectionReason.UserNotAllowedToApproveOwnClaimDespiteSignoffGroup;
						return false;
					}
				}
			}

			return true;
		}

		/// <summary>
		/// Send the claim to the next stage, approver etc.
		/// </summary>
		/// <param name="claim">The claim object to update</param>
		/// <param name="submitting">True if claim is being submitted for the first time</param>
		/// <param name="authoriser">the authoriser ID </param>
		/// <param name="employeeid">The employee ID that the claim belongs too</param>
		/// <param name="delegateid">The delegate Employee ID</param>
		/// <returns>1 = user not assigned to a group
		/// 2 = reached last stage, claim now needs paying
		/// 13 = user approving own claim (and disallowed)
		/// 14 = user in signoff group as an approver (and disallowed)
		/// </returns>
		public virtual SubmitClaimResult SendClaimToNextStage(cClaim claim, bool submitting, int authoriser, int employeeid, int? delegateid)
		{
			return SendClaimToNextStage(claim, submitting, authoriser, employeeid, delegateid, false);
		}

		/// <summary>
		/// Send the claim to the next stage, approver etc.
		/// </summary>
		/// <param name="claim">
		/// The claim object to update
		/// </param>
		/// <param name="submitting">
		/// True if claim is being submitted for the first time
		/// </param>
		/// <param name="authoriser">
		/// the authoriser ID 
		/// </param>
		/// <param name="employeeid">
		/// The employee ID that the claim belongs too
		/// </param>
		/// <param name="delegateid">
		/// The delegate Employee ID
		/// </param>
		/// <param name="validating">
		/// Whether we are just validating the claim stages during submit. Defaults false, advancing the claim.
		/// </param>
		/// <param name="payBeforeValidateUpdate">
		/// True if updating from the "pay" click from pay Before Validate.
		/// </param>
		/// <returns>
		/// 1 = user not assigned to a group
		/// 2 = reached last stage, claim now needs paying
		/// 13 = user approving own claim (and disallowed)
		/// 14 = user in signoff group as an approver (and disallowed)
		/// </returns>
		public virtual SubmitClaimResult SendClaimToNextStage(cClaim claim, bool submitting, int authoriser, int employeeid, int? delegateid, bool validating, bool payBeforeValidateUpdate = false)
		{
			if (claim.stage != 0)
			{
				this._claims.UpdateApproverLastRemindedDateWhenApproved(claim.claimid, this._user.EmployeeID);
			}

			var result = new SubmitClaimResult();
			result.NoDefaultAuthoriserPresent = false;
			var noDefaultAuthoriserPresent = false;
			bool skipSELStage;
			List<Tuple<cClaim, string, int?, string>> historyUpdates;
			using (var connection = new DatabaseConnection(cAccounts.getConnectionString(_user.AccountID)))
			{
				bool shouldProgressToNextStage = false;
				connection.sqlexecute.Parameters.Clear();
				skipSELStage = false;
				int stage = claim.stage - 1;
				int notify = 0;
				int[] nextCheckerId;
				decimal claimAmount = 0;
				string sql = string.Empty;
				int subAccountID;

				this._subAccounts = this._subAccounts ?? new cAccountSubAccounts(this._user.AccountID);
				var firstSubAccount = this._subAccounts.getFirstSubAccount();
				var reqProperties = firstSubAccount.SubAccountProperties;

                int? commenter = null;

                if (delegateid.HasValue)
                {
                    commenter = delegateid;
                }
                else if (employeeid != 0)
                {
                    commenter = employeeid;
                }
              
                try
                {
                    subAccountID = this._user.CurrentSubAccountId;
                }
                catch
                {
                    subAccountID = firstSubAccount.SubAccountID;
                }

				var notifications = new NotificationTemplates(this._user);

                this._employees = this._employees ?? new cEmployees(this._user.AccountID);
                Employee claimemp = this._employees.GetEmployeeById(claim.employeeid);
                this._misc = this._misc ?? new cMisc(this._user.AccountID);
                var reqgroup = this.GetGroupForClaim(claim, claimemp);
                var reqstage = GetStageForClaim(claim, submitting, reqgroup);

				// check to see if users are allowed to approve their own claims
				var submitClaimResult = new SubmitClaimResult();
				bool userCanApproveOwnClaim = this.CanUserApproveOwnClaim(
					reqgroup,
					reqstage,
						claim,
						reqProperties,
					submitClaimResult,
					true,
					submitting);

				if (!userCanApproveOwnClaim)
				{
					return submitClaimResult;
				}

				SubmitRejectionReason sendClaimToNextStage;
				if (this.ValidateItemChecker(claim, employeeid, reqstage, out sendClaimToNextStage) && !payBeforeValidateUpdate)
				{
					submitClaimResult.Reason = sendClaimToNextStage;
					return submitClaimResult;
				}

				//get the claimamount
				claimAmount = claim.Total;


				// store all claim history updates until the end.
				historyUpdates = new List<Tuple<cClaim, string, int?, string>>();

				if (!validating)
				{

					if (submitting)
					{
						claim.stage = 0;
						this._claims.UpdateClaimHistory(claim, "Claim submitted.", commenter);
					}

					//increment stage

					if ((claim.stage >= reqgroup.stages.Count || (claim.stage > 0 && reqgroup.stages.Values[claim.stage - 1].AllocateForPayment)) && submitting == false && payBeforeValidateUpdate == false)
					{
						this._claims.ApproveClaim(claim, employeeid, delegateid, claim.stage > 0 && reqgroup.stages.Values[claim.stage - 1].AllocateForPayment);

						if (reqgroup.oneclickauthorisation || reqgroup.stages.Values[claim.stage - 1].IsPostValidationCleanupStage)
						{
							// one click authorisation selected on signoff group so advance straight for payment.
							this._claims.payClaim(claim, employeeid, delegateid);
						}
						submitClaimResult.Reason = SubmitRejectionReason.ClaimPaid;
						return submitClaimResult;
					}

					this.ResetApproval(claim);
				}

				for (stage = claim.stage; stage < reqgroup.stages.Count; stage++)
				{
					reqstage = reqgroup.stages.Values[stage];
					bool includestage = true;
					SignoffType signofftype = reqstage.signofftype;
					int relid = reqstage.relid;

					if (signofftype == SignoffType.DeterminedByClaimantFromApprovalMatrix ||
						signofftype == SignoffType.ClaimantSelectsOwnChecker)
					{
						relid = authoriser;
					}

					// is the stage on holiday?
					bool isOnHoliday = this._claims.userOnHoliday(signofftype, relid, claimemp.EmployeeID);
					if (isOnHoliday)
					{
						this.CheckerIsOnHoliday(claim, reqstage, ref includestage, ref signofftype, ref relid, commenter.Value, true);
					}

					// got the stage we need. 
					if (includestage)
					{
						includestage = this.VerifyIncludeFlagBasedOnStageLimits(claim, ref notify, reqstage, claimAmount);
					}

					// check to see if limit existed. if this was exceeded includestage will have been changed to false
					if (includestage)
					{
						nextCheckerId = new int[1];
						if (signofftype == SignoffType.ClaimantSelectsOwnChecker ||
							signofftype == SignoffType.DeterminedByClaimantFromApprovalMatrix)
						{
							nextCheckerId[0] = authoriser;
						}
						else
						{
							decimal matrixAmount = claimAmount;

							// transform mandatory approval matrix step into the correct level's approver type and relid
							if (signofftype == SignoffType.ApprovalMatrix)
							{
								relid = this.SetRelIdforApprovalMatrixStage(claim, relid, reqstage, matrixAmount, claimemp, includestage, commenter, ref signofftype, true);
							}

							nextCheckerId[0] = this._claims.getNextCheckerId(claim, signofftype, relid);
						}

						if (nextCheckerId[0] == 0 &&
							signofftype != SignoffType.Team &&
							signofftype != SignoffType.ClaimantSelectsOwnChecker &&
							signofftype != SignoffType.CostCodeOwner &&
							signofftype != SignoffType.DeterminedByClaimantFromApprovalMatrix &&
							signofftype != SignoffType.AssignmentSignOffOwner &&
							signofftype != SignoffType.SELScanAttach &&
							signofftype != SignoffType.SELValidation)
						{
							submitClaimResult.Reason = SubmitRejectionReason.StageRequiresFurtherCheckers;
							return submitClaimResult;
						}

						claim.stage = stage + 1;

						claim.checkerid = nextCheckerId[0];
						cTeam reqteam;

						if (validating)
						{
							switch (signofftype)
							{
								case SignoffType.CostCodeOwner:

									if (this._claims.UpdateItemCheckers(claim, ItemCheckerType.Normal, true) == -1)
									{
										submitClaimResult.Reason = SubmitRejectionReason.CostCodeOwnerNotSpecified;
										return submitClaimResult;
									}
									break;

								case SignoffType.AssignmentSignOffOwner:

									if (this._claims.UpdateItemCheckers(claim, ItemCheckerType.AssignmentSupervisor, true) == -1)
									{
										submitClaimResult.Reason = SubmitRejectionReason.AssignmentSupervisorNotSpecified;
										return submitClaimResult;
									}
									break;

								case SignoffType.LineManager:

									if (this._employees.GetEmployeeById(claim.checkerid) == null)
									{
										submitClaimResult.Reason = SubmitRejectionReason.StageRequiresFurtherCheckers;
										return submitClaimResult;
									}

									break;
							}
						}
						else
						{
							int? tolerance = null;
							if (reqstage.IsPostValidationCleanupStage)
							{
								var payBeforeValidateStage = GetPayBeforeValidateStage(reqgroup);
								tolerance = payBeforeValidateStage.ValidationCorrectionThreshold;

								// check for changes and compare to tolerance.
								var expenseItems = this._claims.getExpenseItemsFromDB(claim.claimid);
								var itemsWithNoTolerance = ExpenseItemsOutsideChangeTolerance(expenseItems.Values, tolerance);
								if (itemsWithNoTolerance.Count == 0)
								{
									sql = "update claims_base SET PayBeforeValidate = 0 WHERE claimid = @claimid";
									connection.sqlexecute.Parameters.AddWithValue("@claimid", claim.claimid);
									connection.ExecuteSQL(sql);
									this._claims.payClaim(claim, employeeid, delegateid);
									return new SubmitClaimResult { Reason = SubmitRejectionReason.ClaimPaid };
								}
							}

							if (notify == 1)
							{
								// just notify of claim by email
								this.SendNotifyEmails(claim, employeeid, signofftype, relid, subAccountID, nextCheckerId, notifications);
							}
							else
							{
								DateTime modifiedon = DateTime.Now.ToUniversalTime();
								sql =
									"update claims_base set status = 3, stage = @stage, ModifiedOn = @modifiedon, ModifiedBy = @userid, checkerid = @checkerId, teamid = @teamid, splitApprovalStage = @splitApprovalStage WHERE claimid = @claimid";
								connection.sqlexecute.Parameters.AddWithValue("@stage", stage + 1);

                                this._teams = this._teams ?? new cTeams(this._user.AccountID, subAccountID);

								switch (signofftype)
								{
									case SignoffType.Team:
										connection.sqlexecute.Parameters.AddWithValue("@checkerId", DBNull.Value);
										connection.sqlexecute.Parameters.AddWithValue("@teamid", relid);
										connection.sqlexecute.Parameters.AddWithValue("@splitApprovalStage", 0);
										claim.teamid = relid;
										claim.checkerid = 0;
										reqteam = this._teams.GetTeamById(relid);

										nextCheckerId = new int[reqteam.teammembers.Count];
										foreach (int t in reqteam.teammembers)
										{
											nextCheckerId[reqteam.teammembers.IndexOf(t)] = t;
										}

										historyUpdates.Add(new Tuple<cClaim, string, int?, string>(claim, "The claim has been sent to the next stage and is waiting to be allocated to an approver", commenter, null));

										this.ResetItemCheckers(claim);
										break;

									case SignoffType.CostCodeOwner:
										connection.sqlexecute.Parameters.AddWithValue("@checkerId", DBNull.Value);
										connection.sqlexecute.Parameters.AddWithValue("@teamid", DBNull.Value);
										connection.sqlexecute.Parameters.AddWithValue("@splitApprovalStage", 1);

										int ccoRetVal = this._claims.UpdateItemCheckers(claim);

										if (ccoRetVal == -1)
										{
											historyUpdates.Add(new Tuple<cClaim, string, int?, string>(claim,
												"Claim submission halted due to one or more expense item cost codes not having an owner defined, default owner or line manager defined.",
												commenter, null));
											submitClaimResult.Reason = submitting ? SubmitRejectionReason.CostCodeOwnerNotSpecified : SubmitRejectionReason.CostCodeOwnerNotSpecifiedWhenApproving;

											return submitClaimResult;
										}

										// cache dependency unlikely to be quick enough
										claim = this._claims.getClaimById(claim.claimid);

										var ccoCheckers = new List<int>();
										ccoCheckers.AddRange(this.GetExpenseItemCheckerIds(claim.claimid, claim.employeeid, subAccountID));

										nextCheckerId = ccoCheckers.ToArray();

										historyUpdates.Add(new Tuple<cClaim, string, int?, string>(claim, "The claim has been sent to the next stage and is awaiting claim item approval by the cost code owner(s).", commenter, null));
										break;

									case SignoffType.AssignmentSignOffOwner:
										connection.sqlexecute.Parameters.AddWithValue("@checkerId", DBNull.Value);
										connection.sqlexecute.Parameters.AddWithValue("@teamid", DBNull.Value);
										connection.sqlexecute.Parameters.AddWithValue("@splitApprovalStage", 1);

										int retVal = this._claims.UpdateItemCheckers(claim, ItemCheckerType.AssignmentSupervisor);

										if (retVal == -1)
										{
											historyUpdates.Add(new Tuple<cClaim, string, int?, string>(claim,
												"Claim submission halted due to one or more assignment numbers not having a supervisor defined.",
												commenter, null));
											submitClaimResult.Reason = submitting ? SubmitRejectionReason.AssignmentSupervisorNotSpecified : SubmitRejectionReason.AssignmentSupervisorNotSpecifiedWhenApproving;
											return submitClaimResult;
										}

										claim = this._claims.getClaimById(claim.claimid);
										var expenseItems = this._claims.getExpenseItemsFromDB(claim.claimid);
										var asCheckers =
											(from x in expenseItems.Values
											 where x.itemCheckerId.HasValue
											 select x.itemCheckerId.Value).ToList();

										var teamIds = (from ei in expenseItems.Values
													   where ei.ItemCheckerTeamId != null
													   select (int)ei.ItemCheckerTeamId).ToArray();

										foreach (var teamId in teamIds)
										{
											reqteam = this._teams.GetTeamById(teamId);
											if (reqteam != null)
											{
												asCheckers.AddRange(reqteam.teammembers);
											}
										}

										nextCheckerId = asCheckers.Distinct().ToArray();
										historyUpdates.Add(new Tuple<cClaim, string, int?, string>(claim, "The claim has been sent to the next stage and is awaiting claim item approval by the assignment supervisor(s).", commenter, null));
										break;

									case SignoffType.SELScanAttach:
										connection.sqlexecute.Parameters.AddWithValue("@checkerId", DBNull.Value);
										connection.sqlexecute.Parameters.AddWithValue("@teamid", DBNull.Value);
										connection.sqlexecute.Parameters.AddWithValue("@splitApprovalStage", 0);

										// check here if there are no envelopes for this claim.
										// this will mean that we modify the history, and also advance the claim again.
										int total = 0;

										if (string.IsNullOrWhiteSpace(claim.ReferenceNumber))
										{
											skipSELStage = true;
										}
										else
										{
											int complete;
											var allEnvelopesComplete = new Envelopes().AreAllEnvelopesCompleteForClaim(claim.ReferenceNumber, out total, out complete);
											var allExpensesHaveReceipts = new Receipts(claim.accountid, claim.employeeid).CheckIfAllValidatableClaimLinesHaveReceiptsAttached(claim.claimid);
											skipSELStage = allEnvelopesComplete && allExpensesHaveReceipts;
										}

										historyUpdates.Add(
											new Tuple<cClaim, string, int?, string>(claim, skipSELStage ? string.Format("{0} therefore the stage will be skipped.", total == 0 ? "The claim has no envelopes for scan and attach," : "All envelopes have already been scanned and attached,") : "The claim has been sent to the next stage and is awaiting scan and attach.", null, null));
										break;

									case SignoffType.SELValidation:
										connection.sqlexecute.Parameters.AddWithValue("@checkerId", DBNull.Value);
										connection.sqlexecute.Parameters.AddWithValue("@teamid", DBNull.Value);
										connection.sqlexecute.Parameters.AddWithValue("@splitApprovalStage", 0);

										// check here if there are no envelopes for this claim and all items are not validatable.
										var claimLines = this._claims.getExpenseItemsFromDB(claim.claimid);
										if (
											claimLines.All(
												c =>
												!(c.Value.ValidationProgress == ExpenseValidationProgress.Required ||
												  c.Value.ValidationProgress == ExpenseValidationProgress.InProgress)))
										{
											skipSELStage = true;
										}

										// this will mean that we modify the history, and also advance the claim again.
										historyUpdates.Add(new Tuple<cClaim, string, int?, string>(claim, skipSELStage ? "The claim has no validatable items, therefore the stage will be skipped." : "The claim has been sent to the next stage and is awaiting validation.", null, null));
										break;

									default:
										connection.sqlexecute.Parameters.AddWithValue("@checkerid", nextCheckerId[0]);
										connection.sqlexecute.Parameters.AddWithValue("@teamid", DBNull.Value);
										connection.sqlexecute.Parameters.AddWithValue("@splitApprovalStage", 0);

										Employee nextchecker = this._employees.GetEmployeeById(nextCheckerId[0]);

										if (nextchecker == null)
										{
											submitClaimResult.Reason = SubmitRejectionReason.InvalidItemChecker;
											return submitClaimResult;
										}

										if (submitting)
										{
											historyUpdates.Add(new Tuple<cClaim, string, int?, string>(claim, string.Format("Claim No {0} submitted. The claim has been sent to the next stage and is awaiting approval by {1} {2} {3}", claim.claimno, nextchecker.Title, nextchecker.Forename, nextchecker.Surname), null, null));
										}
										else
										{
											if (reqstage.IsPostValidationCleanupStage)
											{
												historyUpdates.Add(new Tuple<cClaim, string, int?, string>(claim, string.Format("The Claim has been sent to the Verification stage and is awaiting approval by {0} {1} {2}.  This is due to changes which have exceeded the tolerance of {3}%.", nextchecker.Title, nextchecker.Forename, nextchecker.Surname, tolerance.Value), commenter, null));
											}
											else
											{
												historyUpdates.Add(new Tuple<cClaim, string, int?, string>(claim, string.Format("The claim has been sent to the next stage and is awaiting approval by {0} {1} {2}", nextchecker.Title, nextchecker.Forename, nextchecker.Surname), commenter, null));
											}
										}

										this.ResetItemCheckers(claim);
										break;


								}

								if (!skipSELStage)
								{
									connection.sqlexecute.Parameters.AddWithValue("@modifiedon", modifiedon);
									connection.sqlexecute.Parameters.AddWithValue("@userid", employeeid);
									connection.sqlexecute.Parameters.AddWithValue("@claimid", claim.claimid);
									connection.sqlexecute.Parameters.AddWithValue("@employeeid", claim.employeeid);
									connection.ExecuteSQL(sql);

                                    var claims = new cClaims(claim.accountid);
                                    var claimItems = claims.getExpenseItemsFromDB(claim.claimid);
                                    var expenseItemsIds = claimItems.Keys.ToList();

								    if (signofftype == SignoffType.SELValidation)
								    {
								        this.SendItemsForValidation(claimItems, reqstage);
								    }

                                    if (reqstage.sendmail)
									{
										try
										{
											var templateId = SendMessageEnum.GetEnumDescription(SendMessageDescription.SentToAnAdministratorAfterASetOfExpensesHaveBeenSubmitted);
											var currentNotificationTemplate = notifications.Get(templateId);

											// Send Email
											notifications.SendMessage(templateId, claim.employeeid, nextCheckerId, expenseItemsIds);

											// Send Push Messages
											new PushNotificationEngine(currentNotificationTemplate, nextCheckerId.ToList(), claim.claimid, this._user.AccountID).SendPushMessagesAsync();
										}
										catch
										{
										}
									}

									var arremp = new int[1];
									arremp[0] = claim.employeeid;
									if (nextCheckerId.Length > 0)
									{
										claim.checkerid = nextCheckerId[0];
										if (reqstage.claimantmail)
										{
											if (nextCheckerId[0] == 0)
											{
												nextCheckerId[0] = 0;
											}
											try
											{
												notifications.SendMessage(SendMessageEnum.GetEnumDescription(SendMessageDescription.SentToAClaimantWhenAClaimReachesTheNextStageInTheApprovalProcess), nextCheckerId[0], arremp, expenseItemsIds);
											}
											catch (Exception ex)
											{

											}
										}

										this._claims.UpdateApproverLastRemindedDateWhenApproved(claim.claimid, nextCheckerId[0]);
									}

								}
								break;
							}
						}
					}
				}

				this._claims.UpdateApproverLastRemindedDate(claim.claimid);
				connection.sqlexecute.Parameters.Clear();
			}

			// write out the history entries for this call.
			historyUpdates.ForEach(u => this._claims.UpdateClaimHistory(u.Item1, u.Item2, u.Item3, u.Item4));
			historyUpdates.Clear();

			// now check if the stage we've just moved to is Scan&Attach, 
			// as we'll need to know whether any envelopes were sent.
			if (skipSELStage)
			{
				return SendClaimToNextStage(claim, false, authoriser, employeeid, delegateid);
			}

			return new SubmitClaimResult { Reason = SubmitRejectionReason.Success, NoDefaultAuthoriserPresent = noDefaultAuthoriserPresent };
		}


		/// <summary>
		/// The expense items outside change tolerance.
		/// </summary>
		/// <param name="expenseItems">
		/// The expense items to check.
		/// </param>
		/// <param name="tolerance">
		/// The tolerance percentage.
		/// </param>
		/// <returns>
		/// The <see cref="List"/>.
		/// A list of expense items that are outside the given tolerance percentage.
		/// </returns>
		internal static List<int> ExpenseItemsOutsideChangeTolerance(IList<cExpenseItem> expenseItems, int? tolerance)
		{
			var itemsOutsideChangeTolerance = new List<int>();
			var originalItems = expenseItems.Where(x => x.Paid && x.Edited).ToList();
			var newItems = expenseItems.Where(x => x.OriginalExpenseId != null).ToList();
			foreach (cExpenseItem originalItem in originalItems)
			{
				decimal paidTotal = originalItem.amountpayable;
				decimal unpaidTotal =
					newItems.Where(expenseItem => expenseItem.OriginalExpenseId == originalItem.expenseid)
						.Sum(expenseItem => expenseItem.amountpayable);

				var difference = (unpaidTotal / paidTotal) * 100;
				if (difference > tolerance)
				{
					itemsOutsideChangeTolerance.Add(originalItem.expenseid);
				}
			}
			return itemsOutsideChangeTolerance;
		}

		private void SendNotifyEmails(
			cClaim claim,
			int employeeid,
			SignoffType signofftype,
			int relid,
			int subAccountID,
			IEnumerable<int> nextcheckerid,
			NotificationTemplates notifications)
		{
			var recipientIds = new List<int>();

			switch (signofftype)
			{
				case SignoffType.Team:
					this._teams = _teams ?? new cTeams(this._user.AccountID, this._user.CurrentSubAccountId);
					var reqteam = this._teams.GetTeamById(relid);

					recipientIds.AddRange(reqteam.teammembers);
					break;

				case SignoffType.CostCodeOwner:
					recipientIds.AddRange(
						this.GetExpenseItemCheckerIds(
							claim.claimid,
							claim.employeeid,
							subAccountID));
					break;

				case SignoffType.AssignmentSignOffOwner:
					recipientIds.AddRange(
						this.GetExpenseItemCheckerIds(claim.claimid, claim.employeeid, subAccountID));
					break;

				default:
					recipientIds.AddRange(nextcheckerid);
					break;
			}

			var claims = new cClaims(claim.accountid);
			var expenseItemsIds = claims.getExpenseItemsFromDB(claim.claimid).Keys.ToList();

			notifications.SendMessage(SendMessageEnum.GetEnumDescription(SendMessageDescription.ThisEmailIsSentToNotifyUsersOfAClaimBeingMade), claim.employeeid, recipientIds.ToArray(), expenseItemsIds);
		}

		public int SetRelIdforApprovalMatrixStage(
			cClaim claim,
			int relid,
			cStage reqstage,
			decimal matrixAmount,
			Employee claimemp,
			bool includestage,
			int? commenter,
			ref SignoffType signofftype, bool updateHistory)
		{
			bool isOnHoliday;
			var matrices = new ApprovalMatrices(this._user.AccountID);
			ApprovalMatrix matrix = matrices.GetById(relid);
			this._teams = _teams ?? new cTeams(this._user.AccountID, this._user.CurrentSubAccountId);

			if (reqstage.FromMyLevel)
			{
				decimal newLevel = ApprovalMatrices.GetClaimantsLevel(
					claim.employeeid,
					matrix,
					reqstage,
					this._budgetHolders,
					this._teams);
				if (newLevel > matrixAmount)
				{
					matrixAmount = newLevel;
				}
			}

			List<ApprovalMatrixLevel> matrixLevel =
				ApprovalMatrices.GetListOfLevelsForThisAmount(matrix, 0, matrixAmount).ToList();
			if (matrixLevel.Count == 1)
			{
				if (matrixLevel[0].ApproverEmployeeId.HasValue)
				{
					signofftype = SignoffType.Employee;
					relid = matrixLevel[0].ApproverEmployeeId.Value;
				}

				if (matrixLevel[0].ApproverTeamId.HasValue)
				{
					signofftype = SignoffType.Team;
					relid = matrixLevel[0].ApproverTeamId.Value;
				}

				if (matrixLevel[0].ApproverBudgetHolderId.HasValue)
				{
					signofftype = SignoffType.BudgetHolder;
					relid = matrixLevel[0].ApproverBudgetHolderId.Value;
				}

				isOnHoliday = this._claims.userOnHoliday(signofftype, relid, claimemp.EmployeeID);
				if (isOnHoliday)
				{
					this.CheckerIsOnHoliday(claim, reqstage, ref includestage, ref signofftype, ref relid, commenter.Value, updateHistory);
				}
			}
			return relid;
		}

		private bool VerifyIncludeFlagBasedOnStageLimits(cClaim claim, ref int notify, cStage reqstage, decimal claimamount)
		{
			notify = reqstage.notify;
			var include = reqstage.include;
			claim.changeStatus(ClaimStatus.NextStageAwaitingAction);
			switch (include)
			{
				case StageInclusionType.ClaimTotalExceeds: //claim exceeded specified amount
					var maxamount = reqstage.amount;
					if (claimamount < maxamount)
					{
						return false;
					}

					return true;
				case StageInclusionType.ClaimTotalBelow:
					var minamount = reqstage.amount;
					if (claimamount > minamount)
					{
						return false;
					}

					return true;
				case StageInclusionType.ExpenseItemExceeds: //an item flagged for going over
					return this.ClaimHasFlagExceeded(claim.claimid);
				case StageInclusionType.IncludesCostCode: //item assigned to costcode
					return this._claims.CheckCostCodeIncluded(this._user.AccountID, claim.claimid, reqstage.includeid);
				case StageInclusionType.IncludesExpenseItem:
					return this.CheckExpenseItemIncluded(claim.claimid, reqstage.includeid);
				case StageInclusionType.OlderThanDays:
					return this.IncludesItemOlderThanXDays(claim.claimid, (int)reqstage.amount);
				case StageInclusionType.IncludesDepartment: //// item assigned to department
					return this.CheckDepartmentIncluded(claim.claimid, reqstage.includeid);
				case StageInclusionType.ValidationFailedTwice:
					return this.CheckExpenseItemValidationCount(claim);
				case StageInclusionType.None:
					return false;
				case StageInclusionType.Always:
					return true;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		/// <summary>
		/// Check the maximun number of times that any expense item on the given claim has failed validation
		/// </summary>
		/// <param name="claim">An istance of <see cref="cClaim"/>to check</param>
		/// <returns>True if the maximum <see cref="cExpenseItem.ValidationCount"/>is greater than or equal to 2</returns>
		private bool CheckExpenseItemValidationCount(cClaim claim)
		{
			using (var data = new DatabaseConnection(cAccounts.getConnectionString(this._user.AccountID)))
			{
				data.sqlexecute.Parameters.AddWithValue("@claimId", claim.claimid);
				data.sqlexecute.Parameters.Add("@returnvalue", SqlDbType.Int);
				data.sqlexecute.Parameters["@returnvalue"].Direction = ParameterDirection.ReturnValue;
				data.ExecuteProc("CheckExpenseItemValidationCount");
				var maxValidationCount = Convert.ToInt32(data.sqlexecute.Parameters["@returnvalue"].Value);
				data.sqlexecute.Parameters.Clear();
				if (maxValidationCount >= 2)
				{
					return true;
				}
			}
			return false;
		}

		private bool ValidateItemChecker(cClaim claim, int employeeid, cStage reqstage, out SubmitRejectionReason sendClaimToNextStage)
		{
			if (claim.stage > 0)
			{
				if (reqstage != null && reqstage.signofftype != SignoffType.SELScanAttach
					&& reqstage.signofftype != SignoffType.SELValidation)
				{
					bool hasItemChecker = this.HasItemChecker(claim.claimid);
					bool hasItemCheckerWithEmployee = this.HasItemCheckerWithEmployee(claim.claimid, employeeid);
					if ((!claim.splitApprovalStage && claim.checkerid != employeeid)
						|| (claim.splitApprovalStage && hasItemChecker && !hasItemCheckerWithEmployee))
					{
						{
							sendClaimToNextStage = SubmitRejectionReason.InvalidItemChecker;
							return true;
						}
					}
				}
			}
			sendClaimToNextStage = SubmitRejectionReason.Success;
			return false;
		}

		private static cStage GetStageForClaim(cClaim claim, bool submitting, cGroup reqgroup)
		{
			var currentStage = submitting ? claim.stage : claim.stage - 1;
			cStage reqstage = reqgroup.stages.Values[currentStage];
			return reqstage;
		}

		/// <summary>
		/// The get pay before validate stage from the given group.
		/// </summary>
		/// <param name="group">
		/// The group to examine.
		/// </param>
		/// <returns>
		/// The <see cref="cStage"/>.
		/// The Pay Before Validate Stage (if any)
		/// </returns>
		public static cStage GetPayBeforeValidateStage(cGroup group)
		{
			return @group.stages.Values.FirstOrDefault(stage => stage.AllocateForPayment);
		}

        private cGroup GetGroupForClaim(cClaim claim, Employee claimemp)
        {
            this._groups = this._groups ?? new cGroups(this._user.AccountID);
            cGroup reqgroup = this._groups.GetGroupById(claimemp.SignOffGroupID);

            var generalOptions = this._generalOptionsFactory[this._user.CurrentSubAccountId].WithClaim();

            // there is a method GetGroupForClaim in cClaims.cs so any changes here might need to be reflected there
            if (generalOptions.Claim.OnlyCashCredit && generalOptions.Claim.PartSubmit)
            {
                if (claim.claimtype == ClaimType.Credit)
                {
                    reqgroup = this._groups.GetGroupById(claimemp.CreditCardSignOffGroup);
                }
                else if (claim.claimtype == ClaimType.Purchase)
                {
                    reqgroup = this._groups.GetGroupById(claimemp.PurchaseCardSignOffGroup);
                }
            }
            return reqgroup;
        }

		public void moveItems(int oldclaimid, int newclaimid, bool cash, bool credit, bool purchase)
		{
			using (var expdata = new DatabaseConnection(cAccounts.getConnectionString(_user.AccountID)))
			{
				string strsql = "update savedexpenses set claimid = @newclaimid where claimid = @oldclaimid and (";
				if (cash)
				{
					strsql += "itemtype = 1";
				}
				if (credit)
				{
					if (cash)
					{
						strsql += " or ";
					}
					strsql += "itemtype = 2";
				}
				if (purchase)
				{
					if (cash || credit)
					{
						strsql += " or ";
					}
					strsql += "itemtype = 3";
				}
				strsql += ")";

				expdata.sqlexecute.Parameters.AddWithValue("@newclaimid", newclaimid);
				expdata.sqlexecute.Parameters.AddWithValue("@oldclaimid", oldclaimid);
				expdata.sqlexecute.Parameters.AddWithValue("@credit", Convert.ToByte(credit));
				expdata.sqlexecute.Parameters.AddWithValue("@purchase", Convert.ToByte(purchase));
				expdata.ExecuteSQL(strsql);
				expdata.sqlexecute.Parameters.Clear();
			}
		}

        public int addClaim(int employeeid, string name, string description, SortedList<int, object> userdefined)
        {
            int claimid;
            using (var expdata = new DatabaseConnection(cAccounts.getConnectionString(_user.AccountID)))
            {
                var generalOptions = this._generalOptionsFactory[this._user.CurrentSubAccountId].WithCurrency();

                DateTime createdon = DateTime.Now.ToUniversalTime();
                int userid = 0;

				//does the claim already exist
				string strsql = "select count(claimid) from claims_base where employeeid = @employeeid and name = @name";
				expdata.sqlexecute.Parameters.AddWithValue("@employeeid", employeeid);
				expdata.sqlexecute.Parameters.AddWithValue("@name", name);
				var count = expdata.ExecuteScalar<int>(strsql);
				expdata.sqlexecute.Parameters.Clear();
				if (count > 0)
				{
					return -1;
				}

				int claimno = 0;

				_employees = _employees ?? new cEmployees(_user.AccountID);
				Employee reqemp = _employees.GetEmployeeById(employeeid);

                int basecurrency = (int)(reqemp.PrimaryCurrency != 0 ? reqemp.PrimaryCurrency : generalOptions.Currency.BaseCurrency);

				//get the last claim no and increment
				expdata.sqlexecute.Parameters.AddWithValue("@employeeid", employeeid);
				claimno = reqemp.CurrentClaimNumber;

				expdata.sqlexecute.Parameters.AddWithValue("@claimno", claimno);
				expdata.sqlexecute.Parameters.AddWithValue("@name", name);
				expdata.sqlexecute.Parameters.AddWithValue("@description",
					description.Length > 2000 ? description.Substring(0, 1999) : description);

				expdata.sqlexecute.Parameters.AddWithValue("@basecurrency", basecurrency);
				expdata.sqlexecute.Parameters.AddWithValue("@date", createdon);
				expdata.sqlexecute.Parameters.AddWithValue("@userid", userid);
				expdata.sqlexecute.Parameters.AddWithValue("@identity", SqlDbType.Int);
				expdata.sqlexecute.Parameters["@identity"].Direction = ParameterDirection.Output;

				strsql =
					"insert into claims_base (employeeid, claimno, name, description, currencyid, createdon, createdby) " +
					"values (@employeeid,@claimno,@name,@description,@basecurrency, @date, @userid);select @identity = @@identity";

				expdata.ExecuteSQL(strsql);
				claimid = (int)expdata.sqlexecute.Parameters["@identity"].Value;

				reqemp.IncrementClaimNumber(_user);

				expdata.sqlexecute.Parameters.Clear();
			}

			var clstables = new cTables(_user.AccountID);
			var clsfields = new cFields(_user.AccountID);
			cTable tbl = clstables.GetTableByID(new Guid("0efa50b5-da7b-49c7-a9aa-1017d5f741d0"));
			var clsuserdefined = new cUserdefinedFields(_user.AccountID);
			clsuserdefined.SaveValues(clstables.GetTableByID(tbl.UserDefinedTableID), claimid, userdefined, clstables,
				clsfields, _user, elementId: (int)SpendManagementElement.Claims, record: name);

			_auditLog = _auditLog ?? new cAuditLog(_user.AccountID, employeeid);
			_auditLog.addRecord(SpendManagementElement.Claims, name, claimid);
			return claimid;
		}


		/// <summary>
		/// Get the claim object from the given ID
		/// </summary>
		/// <param name="claimId">ID of the required claim</param>
		/// <returns>Claim object</returns>
		public cClaim getClaimById(int claimId)
		{
			return _claims.getClaimById(claimId);
		}

		#endregion

		#region Internal Methods

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
		internal SubmitClaimResult CheckClaimTotal(cClaim claim, bool cash, bool credit, bool purchase)
		{
			decimal? minclaim = _user.ExpenseClaimMinimumValue;
			decimal? maxclaim = _user.ExpenseClaimMaximumValue;

			SubmitClaimResult result = new SubmitClaimResult();
			if (cash == false && (credit || purchase))
			{
				result.Reason = SubmitRejectionReason.Success;
				return result;
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
						result.Reason = SubmitRejectionReason.MinimumAmountNotReached;
						result.MinimumAmount = minclaim;
						return result;
					}
				}
				if (maxclaim != null)
				{
					if (total > maxclaim)
					{
						result.Reason = SubmitRejectionReason.MaximumAmountExceeded;
						result.MaximumAmount = maxclaim;
						return result;
					}
				}
			}

			return result;
		}

		/// <summary>
		///     Check department included in claim.
		/// </summary>
		/// <param name="claim">
		///     The claim.
		/// </param>
		/// <param name="departmentId">
		///     The include id.
		/// </param>
		/// <returns>
		///     The <see cref="bool" />.
		/// </returns>
		private bool CheckDepartmentIncluded(int claimId, int departmentId)
		{
			bool hasItems;
			using (var data = new DatabaseConnection(cAccounts.getConnectionString(_user.AccountID)))
			{
				data.sqlexecute.Parameters.AddWithValue("@claimid", claimId);
				data.sqlexecute.Parameters.AddWithValue("@departmentId", departmentId);
				data.sqlexecute.Parameters.Add("@returnvalue", SqlDbType.Bit);
				data.sqlexecute.Parameters["@returnvalue"].Direction = ParameterDirection.ReturnValue;
				data.ExecuteProc("ClaimHasDepartmentIncluded");
				hasItems = Convert.ToBoolean(data.sqlexecute.Parameters["@returnvalue"].Value);
				data.sqlexecute.Parameters.Clear();
			}
			return hasItems;
		}

		internal bool CheckExpenseItemIncluded(int claimId, int subcatid)
		{
			bool hasItems;
			using (var data = new DatabaseConnection(cAccounts.getConnectionString(_user.AccountID)))
			{
				data.sqlexecute.Parameters.AddWithValue("@claimid", claimId);
				data.sqlexecute.Parameters.AddWithValue("@subcatId", subcatid);
				data.sqlexecute.Parameters.Add("@returnvalue", SqlDbType.Bit);
				data.sqlexecute.Parameters["@returnvalue"].Direction = ParameterDirection.ReturnValue;
				data.ExecuteProc("ClaimHasExpenseItemIncluded");
				hasItems = Convert.ToBoolean(data.sqlexecute.Parameters["@returnvalue"].Value);
				data.sqlexecute.Parameters.Clear();
			}
			return hasItems;
		}

		internal void CheckerIsOnHoliday(cClaim claim, cStage reqstage, ref bool includestage,
			ref SignoffType signofftype, ref int relid, int commenter, bool updateHistory)
		{
			switch (reqstage.onholiday)
			{
				case 1: //take no action - wait for user to get back of their hols
					includestage = true;
					break;
				case 2: //skip stage

					includestage = false;
					break;
				case 3: //assign to someoneelse
					signofftype = reqstage.holidaytype;
					relid = reqstage.holidayid;
					if (_claims.userOnHoliday(signofftype, relid, claim.employeeid) && updateHistory) //cannot go any further
					{
						_claims.UpdateClaimHistory(claim,
							"This claim may be delayed as the approver is currently on holiday.", commenter);
					}
					includestage = true;
					break;
			}
		}

		internal bool CheckFrequency(int employeeid, cAccountProperties clsproperties)
		{
			int frequencyvalue;
			int count;
			using (var expdata = new DatabaseConnection(cAccounts.getConnectionString(_user.AccountID)))
			{
				_misc = _misc ?? new cMisc(_user.AccountID);

				if (clsproperties.LimitFrequency == false)
				{
					return true;
				}

				byte frequencytype = clsproperties.FrequencyType;
				frequencyvalue = clsproperties.FrequencyValue;

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
				const string strsql = "select count(*) from claims_base where employeeid = @employeeid and datesubmitted >= @date";
				expdata.sqlexecute.Parameters.AddWithValue("@employeeid", employeeid);
				expdata.sqlexecute.Parameters.AddWithValue("@date", startdate.Year + "/" + startdate.Month + "/" + startdate.Day);
				count = expdata.ExecuteScalar<int>(strsql);
				expdata.sqlexecute.Parameters.Clear();
			}

			return count < frequencyvalue;
		}

		internal bool ClaimHasFlagExceeded(int claimId)
		{
			bool hasFlags;
			using (var data = new DatabaseConnection(cAccounts.getConnectionString(_user.AccountID)))
			{
				data.sqlexecute.Parameters.AddWithValue("@claimId", claimId);
				data.sqlexecute.Parameters.Add("@returnvalue", SqlDbType.Bit);
				data.sqlexecute.Parameters["@returnvalue"].Direction = ParameterDirection.ReturnValue;
				data.ExecuteProc("claimHasAnItemWithLimitExceeded");
				hasFlags = Convert.ToBoolean(data.sqlexecute.Parameters["@returnvalue"].Value);
				data.sqlexecute.Parameters.Clear();
			}
			return hasFlags;
		}

		/// <summary>
		/// Gets a unique list of employee Ids for the expense item checkers for a claim (including team members and default ownership)
		/// </summary>
		/// <param name="claimId">Claim to obtain item checker id list for</param>
		/// <param name="claimantEmployeeId">The Employee ID of the claimant</param>
		/// <param name="subAccountId">Current subaccount ID</param>
		/// <param name="itemsToCheck">Check just Team or Individuals.. Defaults to check everything.</param>
		/// <returns></returns>
		internal IEnumerable<int> GetExpenseItemCheckerIds(int claimId, int claimantEmployeeId, int subAccountId, GetClaimItemCheckerFor itemsToCheck = GetClaimItemCheckerFor.All)
		{
			var ids = new List<int>();
			this._subAccounts = this._subAccounts ?? new cAccountSubAccounts(this._user.AccountID);
			IOwnership ownership = this._subAccounts.GetDefaultCostCodeOwner(this._user.AccountID, subAccountId);
			var defaultIds = new List<int>();
			this._budgetHolders = this._budgetHolders ?? new cBudgetholders(this._user.AccountID);
			this._teams = this._teams ?? new cTeams(this._user.AccountID, this._user.CurrentSubAccountId);

			if (ownership != null)
			{
				if (ownership.OwnerElementType() == SpendManagementElement.None)
				{
					defaultIds.Add(ownership.ItemPrimaryID());
				}
				else
				{
					var ownerIdObject = ownership.OwnerId();
					if (ownerIdObject.HasValue && ownerIdObject.Value > 0)
					{
						switch (ownership.OwnerElementType())
						{
							case SpendManagementElement.Teams:
								if (itemsToCheck == GetClaimItemCheckerFor.All ||
									itemsToCheck == GetClaimItemCheckerFor.TeamsOnly)
								{
									cTeam defOwnerTeam = this._teams.GetTeamById(ownerIdObject.Value);
									defaultIds.AddRange(defOwnerTeam.teammembers);
								}

								break;
							case SpendManagementElement.Employees:
								if (itemsToCheck == GetClaimItemCheckerFor.All ||
									itemsToCheck == GetClaimItemCheckerFor.IndividualsOnly)
								{
									defaultIds.Add(ownerIdObject.Value);
								}

								break;
							case SpendManagementElement.BudgetHolders:
								if (itemsToCheck == GetClaimItemCheckerFor.All ||
									itemsToCheck == GetClaimItemCheckerFor.IndividualsOnly)
								{

									cBudgetHolder bh = this._budgetHolders.getBudgetHolderById(ownerIdObject.Value);
									if (bh != null)
									{
										defaultIds.Add(bh.employeeid);
									}
								}

								break;
						}
					}
				}
			}

			this._costCodes = this._costCodes ?? new cCostcodes(_user.AccountID);
			this._expenseItems = this._expenseItems ?? new cExpenseItems(_user.AccountID);
			SortedList<int, cExpenseItem> expenseItems = this._claims.getExpenseItemsFromDB(claimId);

			foreach (cExpenseItem expenseItem in expenseItems.Values)
			{
				var currentIds = new List<int>();
				if (expenseItem.itemCheckerId.HasValue)
				{
					currentIds.Add(expenseItem.itemCheckerId.Value);
				}
				else
				{
					expenseItem.costcodebreakdown = _expenseItems.getCostCodeBreakdown(expenseItem.expenseid);
					// is it allocated to a team or default team
					if (expenseItem.costcodebreakdown != null && expenseItem.costcodebreakdown.Count > 0)
					{
						foreach (cDepCostItem depCostItem in expenseItem.costcodebreakdown)
						{
							if (depCostItem.costcodeid > 0)
							{
								cCostCode cc = _costCodes.GetCostcodeById(depCostItem.costcodeid);
								if (cc.OwnerId().HasValue && cc.OwnerId().Value > 0)
								{
									switch (cc.OwnerElementType())
									{
										case SpendManagementElement.Teams:
											if (itemsToCheck == GetClaimItemCheckerFor.All || itemsToCheck == GetClaimItemCheckerFor.TeamsOnly)
											{
												cTeam ccOwnerTeam = this._teams.GetTeamById(cc.OwnerId().Value);
												var teamMembers = ccOwnerTeam.teammembers;

												currentIds.AddRange(teamMembers);
											}
											break;
										case SpendManagementElement.Employees:
											if (itemsToCheck == GetClaimItemCheckerFor.All || itemsToCheck == GetClaimItemCheckerFor.IndividualsOnly)
											{
												if (cc.OwnerId().HasValue)
												{
													currentIds.Add(cc.OwnerId().Value);
												}
											}
											break;
										case SpendManagementElement.BudgetHolders:
											if (itemsToCheck == GetClaimItemCheckerFor.All || itemsToCheck == GetClaimItemCheckerFor.IndividualsOnly)
											{
												cBudgetHolder budgetHolder = this._budgetHolders.getBudgetHolderById(cc.OwnerId().Value);
												if (budgetHolder != null)
												{
													currentIds.Add(budgetHolder.employeeid);
												}
											}
											break;
									}
								}
								else
								{
									if (defaultIds.Count > 0)
									{
										currentIds.AddRange(defaultIds);
									}
									else
									{
										// no default defined
										this._employees = this._employees ?? new cEmployees(this._user.AccountID);
										Employee claimant = _employees.GetEmployeeById(claimantEmployeeId);
										currentIds.Add(claimant.LineManager);
									}
								}
							}
						}
					}
					else
					{
						// need to allocate to default
						currentIds.AddRange(defaultIds);
					}
				}

				ids.AddRange(currentIds.Distinct());
			}

			return ids.Distinct();
		}

		internal bool HasItemChecker(int claimId)
		{
			bool hasItemCheker;

			using (var data = new DatabaseConnection(cAccounts.getConnectionString(_user.AccountID)))
			{
				data.sqlexecute.Parameters.AddWithValue("@claimid", claimId);
				data.sqlexecute.Parameters.Add("@returnvalue", SqlDbType.Bit);
				data.sqlexecute.Parameters["@returnvalue"].Direction = ParameterDirection.ReturnValue;
				data.ExecuteProc("ClaimHasItemChecker");
				hasItemCheker = Convert.ToBoolean(data.sqlexecute.Parameters["@returnvalue"].Value);
				data.sqlexecute.Parameters.Clear();
			}

			return hasItemCheker;
		}

		internal bool HasItemCheckerWithEmployee(int claimId, int employeeId)
		{
			bool hasItemCheker;
			using (var data = new DatabaseConnection(cAccounts.getConnectionString(_user.AccountID)))
			{
				data.sqlexecute.Parameters.AddWithValue("@claimid", claimId);
				data.sqlexecute.Parameters.AddWithValue("@employeeid", employeeId);
				data.sqlexecute.Parameters.Add("@returnvalue", SqlDbType.Bit);
				data.sqlexecute.Parameters["@returnvalue"].Direction = ParameterDirection.ReturnValue;
				data.ExecuteProc("ClaimHasItemCheckerWithEmployeeId");
				hasItemCheker = Convert.ToBoolean(data.sqlexecute.Parameters["@returnvalue"].Value);
				data.sqlexecute.Parameters.Clear();
			}

			return hasItemCheker;
		}

		internal bool IncludesItemOlderThanXDays(int claimId, int days)
		{
			DateTime date = DateTime.Today;
			date = date.AddDays(days / -1);

			bool hasItems;
			using (var data = new DatabaseConnection(cAccounts.getConnectionString(_user.AccountID)))
			{
				data.sqlexecute.Parameters.AddWithValue("@claimid", claimId);
				data.sqlexecute.Parameters.AddWithValue("@date", date);
				data.sqlexecute.Parameters.Add("@returnvalue", SqlDbType.Bit);
				data.sqlexecute.Parameters["@returnvalue"].Direction = ParameterDirection.ReturnValue;
				data.ExecuteProc("ClaimHasItemOlderThanXDays");
				hasItems = Convert.ToBoolean(data.sqlexecute.Parameters["@returnvalue"].Value);
				data.sqlexecute.Parameters.Clear();
			}
			return hasItems;
		}

		internal void InsertFirstEntryIntoClaimHistory(cClaim claim, int employeeid, DateTime modifiedon, DateTime datestamp)
		{
			using (var expdata = new DatabaseConnection(cAccounts.getConnectionString(_user.AccountID)))
			{
				expdata.sqlexecute.Parameters.AddWithValue("@claimid", claim.claimid);
				expdata.sqlexecute.Parameters.AddWithValue("@modifiedon", modifiedon);
				expdata.sqlexecute.Parameters.AddWithValue("@userid", employeeid);

				string strsql = "update claims_base set submitted = 1, status = 1, datesubmitted = '" + datestamp.Year +
								"/" +
								datestamp.Month + "/" + datestamp.Day +
								"', ModifiedOn = @modifiedon, ModifiedBy = @userid where claimid = @claimid";
				expdata.ExecuteSQL(strsql);

				expdata.sqlexecute.Parameters.Clear();
			}

			_claims.Cache.Delete(_user.AccountID, cClaims.CacheArea, claim.employeeid.ToString());
		}

		internal void ResetApproval(cClaim claim)
		{
			using (var expdata = new DatabaseConnection(cAccounts.getConnectionString(_user.AccountID)))
			{
				expdata.sqlexecute.Parameters.Clear();

				expdata.sqlexecute.Parameters.AddWithValue("@ClaimId", claim.claimid);
				expdata.ExecuteProc("dbo.ResetExpenseItem");
				expdata.sqlexecute.Parameters.Clear();
			}
		}

        /// <summary>
        ///     Set the ItemChecker Id on each claim item for a stage back to NULL
        /// </summary>
        /// <param name="claim"></param>
        internal void ResetItemCheckers(cClaim claim)
        {
            using (var expdata = new DatabaseConnection(cAccounts.getConnectionString(_user.AccountID)))
            {
                const string sql = "UPDATE savedexpenses SET itemCheckerId = NULL WHERE claimid = @claimId";
                expdata.sqlexecute.Parameters.AddWithValue("@claimId", claim.claimid);
                expdata.ExecuteSQL(sql);
            }
        }
        /// <summary>
        /// Determine Default Authoriser is Present or not
        /// </summary>
        /// <returns></returns>
        public bool ClaimCanBeSubmittedBasedOnDefaultAuthoriser()
        {
            bool isDefaultAuthoriserPresent = false;
            using (IDBConnection expdata = new DatabaseConnection(cAccounts.getConnectionString(_user.AccountID)))
            {
                expdata.sqlexecute.Parameters.Clear();
                isDefaultAuthoriserPresent = expdata.ExecuteScalar<bool>("dbo.DetermineDefaultUserPresent", CommandType.StoredProcedure);
                expdata.sqlexecute.Parameters.Clear();
            }
            return isDefaultAuthoriserPresent;
        }

        private void SendItemsForValidation(SortedList<int, cExpenseItem> claimItems, cStage stage)
        {
            decimal percentageOfItemsToValidate = this.GetPercentageOfItemsToValidate(stage.signoffid);

            // from claimItems get the list of items that can be sent to validation 
            //var itemsThatCanBeValidated = claimItems.Where(x => x.Value.receiptattached && x.Value.ValidationProgress == 0).Select(x => x.Key).ToList();

            var itemsThatCanBeValidated = new SortedList<int, cExpenseItem>();

            foreach (var item in claimItems)
            {
                if (item.Value.receiptattached && item.Value.ValidationProgress == 0)
                {
                    itemsThatCanBeValidated.Add(item.Key, item.Value);
                }
            }
            
            // apply the percentage to select the items 
            decimal numberOfItemsToBeValidated = (itemsThatCanBeValidated.Count * percentageOfItemsToValidate) / 100;

            var random = new Random();

            // round up the number of items
            var numberOfItemsToBeValidatedPrecision = numberOfItemsToBeValidated - Math.Truncate(numberOfItemsToBeValidated);

            decimal roundedNumberOfItemsToBeValidated;

            // if the precision is 0.5 toss coin to decide if the item will be included or not
            if (numberOfItemsToBeValidatedPrecision == 0.5M)
            {
                roundedNumberOfItemsToBeValidated = random.Next(0, 2) == 0
                    ? numberOfItemsToBeValidated - 0.5M
                    :numberOfItemsToBeValidated + 0.5M;
            }
            else
            {
                roundedNumberOfItemsToBeValidated = Math.Round(numberOfItemsToBeValidated, 0);
            }
            
            int max = itemsThatCanBeValidated.Count;

            var numberOfItemsNotSendForValidation = itemsThatCanBeValidated.Count - roundedNumberOfItemsToBeValidated;

            // update the items that need to go for validation
            for (var i = 0; i < numberOfItemsNotSendForValidation; i++)
            {
                int index = random.Next(0, max);

                // set the state of the items that won't be send for validation
                this.PreventSendingItemForValidation(itemsThatCanBeValidated.Keys[index], itemsThatCanBeValidated.Values[index], ExpenseValidationProgress.NotSelectedForValidation);

                // removing the item from the list ensures that duplicates won't occur 
                itemsThatCanBeValidated.RemoveAt(index);
                max--;
            }
        }

        private void PreventSendingItemForValidation(int expenseId, cExpenseItem item, ExpenseValidationProgress sendForValidation)
        {
            var validationManager = new ExpenseValidationManager(this._user.AccountID);
            validationManager.UpdateProgressForExpenseItem(expenseId, item.ValidationProgress, sendForValidation);
        }

        private decimal GetPercentageOfItemsToValidate(int signoffStageId)
        {
            decimal percentageOfItemsToValidate = 0;

            using (var connection = new DatabaseConnection(cAccounts.getConnectionString(this._user.AccountID)))
            {
                connection.sqlexecute.Parameters.AddWithValue("@stageId", signoffStageId);

                using (var reader = connection.GetReader("GetPercentageOfClaimItemsForValidation", CommandType.StoredProcedure))
                {
                    while (reader.Read())
                    { 
                        percentageOfItemsToValidate = reader.GetNullable<decimal>("ClaimPercentageToValidate") ?? 100;
                    }

                   reader.Close();
                }

                connection.sqlexecute.Parameters.Clear();
            }

            return percentageOfItemsToValidate;

        }

        #endregion
    }
}