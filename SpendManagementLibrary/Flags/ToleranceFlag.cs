namespace SpendManagementLibrary.Flags
{
    using SpendManagementLibrary.Interfaces;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The tolerance flag.
    /// </summary>
    [Serializable]
    public class ToleranceFlag : Flag
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="ToleranceFlag"/> class.
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
        /// The item roles associated with the flag.
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
        /// <param name="notesforauthoriser">
        /// Notes seen by the authoriser to guide them on how to deal with the flag.
        /// </param>
        /// <param name="flagTypeDescription">
        /// The flag Type Description.
        /// </param>
        /// <param name="flagDescription">
        /// The flag Description.
        /// </param>
        /// <param name="itemroleinclusiontype">
        /// The inclusion type of item roles for the flag. All items roles or a specified list.
        /// </param>
        /// <param name="expenseiteminclusiontype">
        /// The inclusion type of expense items for the flag. All expense items or a specified list.
        /// </param>
        /// ///
        public ToleranceFlag(int flagid, FlagType flagtype, FlagAction action, string flagtext, List<int> associateditemroles, List<AssociatedExpenseItem> associatedexpenseitems, DateTime createdon, int? createdby, DateTime? modifiedon, int? modifiedby, decimal? ambertolerance, string description, bool active, int accountid, bool claimantjustificationrequired, bool displayflagimmediately, decimal? flagTolerancePercentage, FlagColour flaglevel, bool approverjustificationrequired, string notesforauthoriser, string flagTypeDescription, string flagDescription, FlagInclusionType itemroleinclusiontype, FlagInclusionType expenseiteminclusiontype, bool validateOnAddExpense, bool expenseitemselectionmandatory, bool requiresSaveToValidate, bool allowMultipleFlagsOfThisType)
            : base(flagid, flagtype, action, flagtext, associateditemroles, associatedexpenseitems, createdon, createdby, modifiedon, modifiedby, description, active, accountid, claimantjustificationrequired, displayflagimmediately, flaglevel, approverjustificationrequired, notesforauthoriser, flagTypeDescription, flagDescription, itemroleinclusiontype, expenseiteminclusiontype, validateOnAddExpense, expenseitemselectionmandatory, requiresSaveToValidate, allowMultipleFlagsOfThisType, false)
        {
            this.AmberTolerance = ambertolerance;
            this.NoFlagTolerance = flagTolerancePercentage;
        }

        #region properties

        /// <summary>
        /// Gets or sets the amber tolerance percentage
        /// </summary>
        public decimal? AmberTolerance { get; protected set; }

        /// <summary>
        /// Gets or sets the tolerance to check before flagging an item
        /// </summary>
        public decimal? NoFlagTolerance { get; protected set; }

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
        /// Done externally.
        /// </exception>
        public override List<FlaggedItem> Validate(cExpenseItem expenseItem, int employeeId, cAccountProperties properties, IDBConnection connection = null)
        {
            throw new NotImplementedException("Validate done externally.");
        }

        /// <summary>
        /// The check tolerance.
        /// </summary>
        /// <param name="total">
        /// The total.
        /// </param>
        /// <param name="limit">
        /// The limit.
        /// </param>
        /// <returns>
        /// The <see cref="FlagColour"/>.
        /// </returns>
        public FlagColour CheckTolerance(decimal total, decimal limit)
        {
            decimal percentageOverTolerance;
            if (limit == 0)
            {
                percentageOverTolerance = 100;
            }
            else
            {
                percentageOverTolerance = ((total / limit) * 100) - 100;    
            }
            
            if (this.NoFlagTolerance.HasValue && percentageOverTolerance <= this.NoFlagTolerance)
            {
                // doesn't exceed the no flag tolerance level so not flag needed
                return FlagColour.None;
            }

            if (this.AmberTolerance.HasValue && percentageOverTolerance <= this.AmberTolerance)
            {
                return FlagColour.Amber;
            }

            return FlagColour.Red;
        }
    }
}
