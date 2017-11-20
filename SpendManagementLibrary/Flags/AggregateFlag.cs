namespace SpendManagementLibrary.Flags
{
    using System;
    using System.Collections.Generic;

    using SpendManagementLibrary.Interfaces;

    /// <summary>
    /// The aggregate flag.
    /// </summary>
    [Serializable]
    public class AggregateFlag : Flag
    {
        /// <summary>
        /// The lst flags.
        /// </summary>
        List<int> lstFlags;

        /// <summary>
        /// Initialises a new instance of the <see cref="AggregateFlag"/> class.
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
        /// <param name="flags">
        /// A list of flags that form this aggregate flag.
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
        public AggregateFlag(int flagid, FlagType flagtype, FlagAction action, string flagtext, List<int> associateditemroles, List<AssociatedExpenseItem> associatedexpenseitems, DateTime createdon, int? createdby, DateTime? modifiedon, int? modifiedby, List<int> flags, string description, bool active, int accountid, bool claimantjustificationrequired, bool displayflagimmediately, FlagColour flaglevel, bool approverjustificationrequired, string notesforauthoriser, FlagInclusionType itemroleinclusiontype, FlagInclusionType expenseiteminclusiontype)
            : base(flagid, flagtype, action, flagtext, associateditemroles, associatedexpenseitems, createdon, createdby, modifiedon, modifiedby, description, active, accountid, claimantjustificationrequired, displayflagimmediately, flaglevel, approverjustificationrequired, notesforauthoriser, "Aggregate", string.Empty, itemroleinclusiontype, expenseiteminclusiontype, true, false, true, true, true)
        {
            this.lstFlags = flags;
        }

        /// <summary>
        /// Validates the flag to see if it has been breached.
        /// </summary>
        /// <param name="expenseItem">
        /// The expense item the flag is being checked against.
        /// </param>
        /// <param name="employeeId">
        /// The id of the employee the expense item belongs.
        /// </param>
        /// <param name="properties">
        /// The properties.
        /// </param>
        /// <param name="connection">
        /// The connection.
        /// </param>
        /// <returns>
        /// The <see cref="FlaggedItem"/>.
        /// </returns>
        /// <exception cref="NotImplementedException">
        /// Not currently used
        /// </exception>
        public override List<FlaggedItem> Validate(cExpenseItem expenseItem, int employeeId, cAccountProperties properties, IDBConnection connection = null)
        {
            throw new NotImplementedException();
        }
    }
}
