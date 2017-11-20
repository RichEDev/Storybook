namespace SpendManagementLibrary.Flags
{
    using System.Data.SqlClient;

    using SpendManagementLibrary.Helpers;
    using SpendManagementLibrary.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.Data;

    /// <summary>
    /// The allowance available flag.
    /// </summary>
    [Serializable]
    public class AllowanceAvailableFlag : Flag
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="AllowanceAvailableFlag"/> class.
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
        public AllowanceAvailableFlag(int flagid, FlagType flagtype, FlagAction action, string flagtext, List<int> associateditemroles, List<AssociatedExpenseItem> associatedexpenseitems, DateTime createdon, int? createdby, DateTime? modifiedon, int? modifiedby, string description, bool active, int accountid, bool claimantjustificationrequired, bool displayflagimmediately, FlagColour flaglevel, bool approverjustificationrequired, string notesforauthoriser, FlagInclusionType itemroleinclusiontype, FlagInclusionType expenseiteminclusiontype)
            : base(flagid, flagtype, action, flagtext, associateditemroles, associatedexpenseitems, createdon, createdby, modifiedon, modifiedby, description, active, accountid, claimantjustificationrequired, displayflagimmediately, flaglevel, approverjustificationrequired, notesforauthoriser, "Unused advance available", "Cash has been claimed when one or more advances are available.", itemroleinclusiontype, expenseiteminclusiontype, true, false, false, false, false)
        {
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
        /// <param name="connection">
        /// The connection.
        /// </param>
        /// <returns>
        /// The <see cref="FlaggedItem"/>.
        /// </returns>
        public override List<FlaggedItem> Validate(cExpenseItem expenseItem, int employeeId, cAccountProperties properties, IDBConnection connection = null)
        {
            decimal floatavailable = 0;
            FlaggedItem flagResult = null;
            if (expenseItem.floatid > 0 || expenseItem.itemtype != ItemType.Cash)
            {
                return null;
            }

            using (
                var databaseConnection = connection
                                         ?? new DatabaseConnection(cAccounts.getConnectionString(this.AccountID)))
            {
                databaseConnection.sqlexecute.Parameters.AddWithValue("@employeeid", employeeId);
                databaseConnection.sqlexecute.Parameters.AddWithValue("@currencyid", expenseItem.currencyid);
                using (IDataReader reader = databaseConnection.GetReader("select sum([float]) - isnull((select sum(amount) from float_allocations inner join floats as innerFloats on innerFloats.floatid = float_allocations.floatid where employeeid = @employeeid and currencyid = @currencyid),0) from floats where approved = 1 and employeeid = @employeeid and currencyid = @currencyid"))
                {
                    while (reader.Read())
                    {
                        if (!reader.IsDBNull(0))
                        {
                            floatavailable = reader.GetDecimal(0);
                        }
                    }
                }
            }

            if (floatavailable > 0)
            {
                flagResult = new FlaggedItem(FlagDescription, CustomFlagText, this, FlagLevel, this.FlagTypeDescription, this.NotesForAuthoriser, this.AssociatedExpenseItems, this.Action, this.CustomFlagText, this.ClaimantJustificationRequired, false);
                return new List<FlaggedItem>() { flagResult };
            }

            return null;

        }
    }
}
