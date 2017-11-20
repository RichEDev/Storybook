namespace SpendManagementLibrary.Flags
{
    using SpendManagementLibrary.Interfaces;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The limit flag.
    /// </summary>
    [Serializable]
    public class LimitFlag : ToleranceFlag
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="LimitFlag"/> class.
        /// </summary>
        /// <param name="flagid">
        /// The flagid.
        /// </param>
        /// <param name="flagtype">
        /// The type of flag. Duplicate limit with a receipt etc.
        /// </param>
        /// <param name="action">
        /// The action to take if the flag is breached.
        /// </param>
        /// <param name="flagtext">
        /// The flagtext.
        /// </param>
        /// <param name="associateditemroles">
        /// The associateditemroles.
        /// </param>
        /// <param name="associatedexpenseitems">
        /// The expense items this flag applies to.
        /// </param>
        /// <param name="createdon">
        /// The date the flag was created.
        /// </param>
        /// <param name="createdby">
        /// The employee the flag was created by.
        /// </param>
        /// <param name="modifiedon">
        /// The date the flag was last modified on on.
        /// </param>
        /// <param name="modifiedby">
        /// The employee the flag was last modified by.
        /// </param>
        /// <param name="ambertolerance">
        /// The percentage over the limit a claimant has gone where an amber flag is displayed rather than red.
        /// </param>
        /// <param name="description">
        /// The general description of the flag.
        /// </param>
        /// <param name="active">
        /// Whether the flag is currently active.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="claimantjustificationrequired">
        /// Whether a claimant must provide a justification for a flag in order to submit their claim.
        /// </param>
        /// <param name="displayflagimmediately">
        /// Whether the flag will be displayed as soon as the expense is added.
        /// </param>
        /// <param name="flagTolerancePercentage">
        /// If the limit is breached, it will not be flagged if below the no flag tolerance percentage
        /// </param>
        /// <param name="flaglevel">
        /// The severity level of the flag.
        /// </param>
        /// <param name="approverjustificationrequired">
        /// Whether an approver needs to provide a justification in order to authorise the claim.
        /// </param>
        /// <param name="increaseByNumOthers">
        /// Whether to increase the limit by the number of others fields as well as the number of employees field.
        /// </param>
        /// <param name="displayLimit">
        /// Whether to display the claimant their limit when adding an expense.
        /// </param>
        /// <param name="notesforauthoriser">
        /// Notes seen by the authoriser to guide them on how to deal with the flag.
        /// </param>
        /// <param name="itemroleinclusiontype">
        /// The inclusion type of item roles for the flag. All items roles or a specified list.
        /// </param>
        /// <param name="expenseiteminclusiontype">
        /// The inclusion type of expense items for the flag. All expense items or a specified list.
        /// </param>
        public LimitFlag(int flagid, FlagType flagtype, FlagAction action, string flagtext, List<int> associateditemroles, List<AssociatedExpenseItem> associatedexpenseitems, DateTime createdon, int? createdby, DateTime? modifiedon, int? modifiedby, decimal? ambertolerance, string description, bool active, int accountid, bool claimantjustificationrequired, bool displayflagimmediately, decimal? flagTolerancePercentage, FlagColour flaglevel, bool approverjustificationrequired, bool increaseByNumOthers, bool displayLimit, string notesforauthoriser, FlagInclusionType itemroleinclusiontype, FlagInclusionType expenseiteminclusiontype)
            : base(flagid, flagtype, action, flagtext, associateditemroles, associatedexpenseitems, createdon, createdby, modifiedon, modifiedby, ambertolerance, description, active, accountid, claimantjustificationrequired, displayflagimmediately, flagTolerancePercentage, flaglevel, approverjustificationrequired, notesforauthoriser, string.Empty, string.Empty, itemroleinclusiontype, expenseiteminclusiontype, true, false, false, false)
        {
            this.IncreaseByNumOthers = increaseByNumOthers;
            this.DisplayLimit = displayLimit;
            if (this.FlagType == FlagType.LimitWithReceipt)
            {
                this.FlagTypeDescription = "Maximum limit with receipt exceeded";
            }
            else if (this.FlagType == FlagType.LimitWithoutReceipt)
            {
                this.FlagTypeDescription = "Maximum limit without a receipt exceeded";
            }
        }

        #region Properties

        /// <summary>
        /// Gets a value indicating whether when applying a limit the limit should be increased by the number of others fields not just the number of staff
        /// </summary>
        public bool IncreaseByNumOthers { get; private set; }

        /// <summary>
        /// Gets a value indicating whether to show the claimant's limit on the add expense screen
        /// </summary>
        public bool DisplayLimit { get; private set; }

        #endregion

        /// <summary>
        /// Validates the flag to see if it has been breached.
        /// </summary>
        /// <param name="expenseItem">
        /// The expense item the flag is being checked against.
        /// </param>
        /// <param name="employeeId">
        /// The id of the employee the expense item belongs.
        /// </param>
        /// <returns>
        /// The <see cref="FlaggedItem"/>.
        /// </returns>
        /// <exception cref="NotImplementedException">
        /// Implemented elsewhere.
        /// </exception>
        public override List<FlaggedItem> Validate(cExpenseItem expenseItem, int employeeId, cAccountProperties properties, IDBConnection connection = null)
        {
            throw new NotImplementedException();
        }
    }
}
