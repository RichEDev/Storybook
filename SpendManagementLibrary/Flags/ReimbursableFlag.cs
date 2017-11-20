namespace SpendManagementLibrary.Flags
{
    using SpendManagementLibrary.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.IO;

    /// <summary>
    /// The reimbursable flag.
    /// </summary>
    [Serializable]
    public class ReimbursableFlag : Flag
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="ReimbursableFlag"/> class.
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
        /// <param name="claimantjustificationrequired">
        /// Whether a claimant must provide a justification for a flag in order to submit their claim.
        /// </param>
        /// <param name="displayflagimmediately">
        /// Whether the flag will be displayed as soon as the expense is added.
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
        public ReimbursableFlag(
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
            bool claimantjustificationrequired,
            bool displayflagimmediately,
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
                description,
                active,
                accountid,
                claimantjustificationrequired,
                displayflagimmediately,
                flaglevel,
            approverjustificationrequired,
            notesforauthoriser,
            "Item reimbursable",
            "This item is reimbursable to the company and the total has been deducted from the claim.",
            itemroleinclusiontype,
            expenseiteminclusiontype,
            true,
            false,
            false,
            false,
            false)
        {
        }

        /// <summary>
        /// Gets or sets the reimbursable.
        /// </summary>
        public bool? Reimbursable { get; set; }

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
        /// Implemented elsewhere
        /// </exception>
        public override List<FlaggedItem> Validate(cExpenseItem expenseItem, int employeeId, cAccountProperties properties, IDBConnection connection = null)
        {
            FlaggedItem flagResult = null;

            if (!this.Reimbursable.HasValue)
            {
                throw new InvalidDataException("Please set Reimbursable before calling Validate.");
            }

            if (this.Reimbursable.Value)
            {
                flagResult = new FlaggedItem(this.FlagDescription, this.CustomFlagText, this, FlagLevel, this.FlagTypeDescription, this.NotesForAuthoriser, this.AssociatedExpenseItems, this.Action, this.CustomFlagText, this.ClaimantJustificationRequired, false);
            }

            return new List<FlaggedItem>() { flagResult };
        }
    }
}
