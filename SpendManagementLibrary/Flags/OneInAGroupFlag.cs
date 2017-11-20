namespace SpendManagementLibrary.Flags
{
    using SpendManagementLibrary.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data.SqlClient;
    using System.Linq;

    using Spend_Management;

    /// <summary>
    /// The one in a group flag.
    /// </summary>
    [Serializable]
    public class OneInAGroupFlag : Flag
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="OneInAGroupFlag"/> class.
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
        public OneInAGroupFlag(int flagid, FlagType flagtype, FlagAction action, string flagtext, List<int> associateditemroles, List<AssociatedExpenseItem> associatedexpenseitems, DateTime createdon, int? createdby, DateTime? modifiedon, int? modifiedby, string description, bool active, int accountid, bool claimantjustificationrequired, bool displayflagimmediately, FlagColour flaglevel, bool approverjustificationrequired, string notesforauthoriser, FlagInclusionType itemroleinclusiontype, FlagInclusionType expenseiteminclusiontype)
            : base(flagid, flagtype, action, flagtext, associateditemroles, associatedexpenseitems, createdon, createdby, modifiedon, modifiedby, description, active, accountid, claimantjustificationrequired, displayflagimmediately, flaglevel, approverjustificationrequired, notesforauthoriser, "One in a group", "Our policy does not allow for certain combinations of items.", itemroleinclusiontype, expenseiteminclusiontype, true, true, false, true, false)
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
        /// <returns>
        /// The <see cref="FlaggedItem"/>.
        /// </returns>
        public override List<FlaggedItem> Validate(cExpenseItem expenseItem, int employeeId, cAccountProperties properties, IDBConnection connection = null)
        {
            
            if (this.AssociatedExpenseItems.Count == 0)
            {
                return null;
            }

            if (this.HasFlagAlreadyBeenAssociatedWithExpense(expenseItem.expenseid))
            {
                return null;
            }

            List<int> associatedItems = new List<int>();
            cFields clsfields = new cFields(AccountID);
            FlaggedItem flagResult = null;

            cTables clstables = new cTables(AccountID);

            cQueryBuilder addedItemQuery = new cQueryBuilder(AccountID, cAccounts.getConnectionString(AccountID), ConfigurationManager.ConnectionStrings["metabase"].ConnectionString, clstables.GetTableByID(new Guid("D70D9E5F-37E2-4025-9492-3BCF6AA746A8")), new cTables(AccountID), new cFields(AccountID));
            addedItemQuery.addColumn(clsfields.GetFieldByID(new Guid("A528DE93-3037-46F6-974C-A76BD0C8642A")));

            addedItemQuery.addFilter(clsfields.GetFieldByID(new Guid("8F61ABE2-96DE-4D3F-9E91-FDF2D47800CB")), ConditionType.Equals, (from associatedItem in this.AssociatedExpenseItems select associatedItem.SubcatID).Cast<object>().ToArray(), null, ConditionJoiner.And, null);
            addedItemQuery.addFilter(clsfields.GetFieldByID(new Guid("A52B4423-C766-47BB-8BF3-489400946B4C")), ConditionType.Equals, new object[] { expenseItem.date }, null, ConditionJoiner.And, null);
            addedItemQuery.addFilter(clsfields.GetFieldByID(new Guid("2501BE3D-AA94-437D-98BB-A28788A35DC4")), ConditionType.Equals, new object[] { employeeId }, new object[] { }, ConditionJoiner.And, null);

            if (expenseItem.expenseid > 0)
            {
                addedItemQuery.addFilter(clsfields.GetFieldByID(new Guid("A528DE93-3037-46F6-974C-A76BD0C8642A")), ConditionType.DoesNotEqual, new object[] { expenseItem.expenseid }, null, ConditionJoiner.And, null);
            }

            int count = 0;
            using (SqlDataReader reader = addedItemQuery.getReader())
            {
                while (reader.Read())
                {
                    if (!reader.IsDBNull(0))
                    {
                        associatedItems.Add(reader.GetInt32(0));
                    }
                }
            }
            
            if (associatedItems.Count > 0)
            {
                Flags flags = new Flags(this.AccountID);
                List<AssociatedExpense> associatedExpenses = flags.GetAssociatedExpenses(associatedItems, connection);
                flagResult = new FlaggedItem(this.FlagDescription, this.CustomFlagText, this, this.FlagLevel, associatedExpenses, this.FlagTypeDescription, this.NotesForAuthoriser, this.AssociatedExpenseItems, this.Action, this.CustomFlagText, this.ClaimantJustificationRequired, false);
            }

            return new List<FlaggedItem>() { flagResult };
        }
    }
}
