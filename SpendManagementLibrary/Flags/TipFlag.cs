namespace SpendManagementLibrary.Flags
{
    using SpendManagementLibrary.Interfaces;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The tip flag.
    /// </summary>
    [Serializable]
    public class TipFlag : ToleranceFlag
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="TipFlag"/> class.
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
        /// <param name="description">
        /// The general description of the flag.
        /// </param>
        /// <param name="active">
        /// Whether the flag is currently active.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="tipLimit">
        /// The tip Limit.
        /// </param>
        /// <param name="claimantjustificationrequired">
        /// Whether a claimant must provide a justification for a flag in order to submit their claim.
        /// </param>
        /// <param name="displayflagimmediately">
        /// Whether the flag will be displayed as soon as the expense is added.
        /// </param>
        /// <param name="amberTolerance">
        /// The percentage over the limit a claimant has gone where an amber flag is displayed rather than red.
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
        /// <param name="itemroleinclusiontype">
        /// The inclusion type of item roles for the flag. All items roles or a specified list.
        /// </param>
        /// <param name="expenseiteminclusiontype">
        /// The inclusion type of expense items for the flag. All expense items or a specified list.
        /// </param>
        public TipFlag(
            int flagid,
            FlagType flagtype,
            FlagAction action,
            string flagtext,
            List<int> associateditemroles,
            List<AssociatedExpenseItem> associatedexpenseitems,
            DateTime createdon,
            int? createdby,
            DateTime? modifiedon,
            int? modifiedby,
            string description,
            bool active,
            int accountid,
            decimal tipLimit,
            bool claimantjustificationrequired,
            bool displayflagimmediately,
            decimal? amberTolerance,
            decimal? flagTolerancePercentage,
            FlagColour flaglevel,
            bool approverjustificationrequired,
            string notesforauthoriser,
            FlagInclusionType itemroleinclusiontype,
            FlagInclusionType expenseiteminclusiontype)
            : base(
                flagid,
                flagtype,
                action,
                flagtext,
                associateditemroles,
                associatedexpenseitems,
                createdon,
                createdby,
                modifiedon,
                modifiedby,
                amberTolerance,
                description,
                active,
                accountid,
                claimantjustificationrequired,
                displayflagimmediately,
                flagTolerancePercentage,
                flaglevel,
                approverjustificationrequired,
                notesforauthoriser,
                "Tip limit exceeded",
                string.Empty,
                itemroleinclusiontype,
                expenseiteminclusiontype,
            true,
            false,
            false,
            false)
        {
            this.TipLimit = tipLimit;
            this.FlagDescription = this.GetFlagDescription();
        }
        
        #region properties

        /// <summary>
        /// Gets the tip limit.
        /// </summary>
        public decimal TipLimit { get; private set; }

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
        public override List<FlaggedItem> Validate(cExpenseItem expenseItem, int employeeId, cAccountProperties properties, IDBConnection connection = null)
        {
            FlaggedItem flagResult = null;

            if (expenseItem.tip == 0)
            {
                return null;
            }

            decimal totalLimit = ((expenseItem.grandtotal - expenseItem.tip) / 100) * this.TipLimit;
            if (expenseItem.tip > totalLimit)
            {
                // flag
                FlagColour colour = CheckTolerance(expenseItem.tip, totalLimit);
                if (colour == FlagColour.None)
                {
                    return null;
                }


                decimal exceededLimit = expenseItem.tip - totalLimit;
                flagResult = new LimitFlaggedItem(this.FlagDescription, this.CustomFlagText, this, colour, new List<AssociatedExpense>(), exceededLimit, this.FlagTypeDescription, this.NotesForAuthoriser, this.AssociatedExpenseItems, this.Action, this.CustomFlagText, this.ClaimantJustificationRequired, false);
            }

            return new List<FlaggedItem>() { flagResult };
        }

        /// <summary>
        /// Gets the flag description.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string GetFlagDescription()
        {
            return "A tip limit of " + this.TipLimit + "% has been exceeded.";
        }
    }
}
