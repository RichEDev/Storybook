namespace SpendManagementLibrary.Flags
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data.SqlClient;
    using System.Linq;

    using SpendManagementLibrary.Interfaces;

    /// <summary>
    /// The group limit flag.
    /// </summary>
    [Serializable]
    public class GroupLimitFlag : ToleranceFlag
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="GroupLimitFlag"/> class.
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
        /// <param name="associatedroles">
        /// The associatedroles.
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
        /// <param name="limit">
        /// The monetary limit of the flag.
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
        /// <param name="itemroleinclusiontype">
        /// The inclusion type of item roles for the flag. All items roles or a specified list.
        /// </param>
        /// <param name="expenseiteminclusiontype">
        /// The inclusion type of expense items for the flag. All expense items or a specified list.
        /// </param>
        public GroupLimitFlag(int flagid, FlagType flagtype, FlagAction action, string flagtext, List<int> associatedroles, List<AssociatedExpenseItem> associatedexpenseitems, DateTime createdon, int? createdby, DateTime? modifiedon, int? modifiedby, decimal? ambertolerance, decimal limit, string description, bool active, int accountid, bool claimantjustificationrequired, bool displayflagimmediately, decimal? flagTolerancePercentage, FlagColour flaglevel, bool approverjustificationrequired, string notesforauthoriser, FlagInclusionType itemroleinclusiontype, FlagInclusionType expenseiteminclusiontype)
            : base(flagid, flagtype, action, flagtext, associatedroles, associatedexpenseitems, createdon, createdby, modifiedon, modifiedby, ambertolerance, description, active, accountid, claimantjustificationrequired, displayflagimmediately, flagTolerancePercentage, flaglevel, approverjustificationrequired, notesforauthoriser, string.Empty, string.Empty, itemroleinclusiontype, expenseiteminclusiontype, true, true, false, true)
        {
            this.Limit = limit;
            this.FlagTypeDescription = this.GetFlagTypeDescription();
            this.FlagDescription = this.GetFlagDescription();
        }

        #region properties

        /// <summary>
        /// Gets the limit.
        /// </summary>
        public decimal Limit { get; private set; }

        /// <summary>
        /// Gets the flag type description.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string GetFlagTypeDescription()
        {
            return this.FlagType == FlagType.GroupLimitWithoutReceipt
                       ? "Group limit without a receipt"
                       : (this.FlagType == FlagType.GroupLimitWithReceipt
                              ? "Group limit with a receipt"
                              : string.Empty);
        }

        /// <summary>
        /// Gets the flag description.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string GetFlagDescription()
        {
           return "This item has exceeded a group limit of {CurrencySymbol}" + this.Limit + "."; 
        }
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
        /// <param name="properties">
        /// The properties.
        /// </param>
        /// <param name="connection">
        /// The connection.
        /// </param>
        /// <returns>
        /// The <see cref="FlaggedItem"/>.
        /// </returns>
        public override List<FlaggedItem> Validate(cExpenseItem expenseItem, int employeeId, cAccountProperties properties, IDBConnection connection = null)
        {
            if (this.HasFlagAlreadyBeenAssociatedWithExpense(expenseItem.expenseid))
            {
                return null;
            }

            List<int> associatedItems = new List<int>();
            cFields clsfields = new cFields(AccountID);
            FlaggedItem flagResult = null;
            if ((this.FlagType == FlagType.GroupLimitWithReceipt && !expenseItem.normalreceipt) || (this.FlagType == FlagType.GroupLimitWithoutReceipt && expenseItem.normalreceipt))
            {
                return null;
            }

            cTables clstables = new cTables(AccountID);

            cQueryBuilder addedItemQuery = new cQueryBuilder(AccountID, cAccounts.getConnectionString(AccountID), ConfigurationManager.ConnectionStrings["metabase"].ConnectionString, clstables.GetTableByID(new Guid("D70D9E5F-37E2-4025-9492-3BCF6AA746A8")), new cTables(AccountID), new cFields(AccountID));
            addedItemQuery.addColumn(clsfields.GetFieldByID(new Guid("A528DE93-3037-46F6-974C-A76BD0C8642A")));
            addedItemQuery.addColumn(clsfields.GetFieldByID(new Guid("C3C64EB9-C0E1-4B53-8BE9-627128C55011")));
            addedItemQuery.addFilter(clsfields.GetFieldByID(new Guid("8F61ABE2-96DE-4D3F-9E91-FDF2D47800CB")), ConditionType.Equals, (from associatedItem in this.AssociatedExpenseItems select associatedItem.SubcatID).Cast<object>().ToArray(), null, ConditionJoiner.And, null);
            addedItemQuery.addFilter(clsfields.GetFieldByID(new Guid("A52B4423-C766-47BB-8BF3-489400946B4C")), ConditionType.Equals, new object[] { expenseItem.date }, null, ConditionJoiner.And, null);
            addedItemQuery.addFilter(clsfields.GetFieldByID(new Guid("2501BE3D-AA94-437D-98BB-A28788A35DC4")), ConditionType.Equals, new object[] { employeeId }, new object[] { }, ConditionJoiner.And, null);
            addedItemQuery.addFilter(
                clsfields.GetFieldByID(new Guid("B52C86F2-FEFC-465C-9163-A6E29A57061A")),
                ConditionType.Equals,
                this.FlagType == FlagType.GroupLimitWithoutReceipt ? new object[] { 0 } : new object[] { 1 },
                null,
                ConditionJoiner.And,
                null);

            if (expenseItem.expenseid > 0)
            {
                addedItemQuery.addFilter(clsfields.GetFieldByID(new Guid("A528DE93-3037-46F6-974C-A76BD0C8642A")), ConditionType.DoesNotEqual, new object[] { expenseItem.expenseid }, null, ConditionJoiner.And, null);
            }

            decimal total = 0;
            using (SqlDataReader reader = addedItemQuery.getReader())
            {
                while (reader.Read())
                {
                    if (!reader.IsDBNull(1))
                    {
                        total += reader.GetDecimal(1);
                        associatedItems.Add(reader.GetInt32(0));
                    }
                }
            }

            total += expenseItem.total;
            if (total > this.Limit)
            {
                FlagColour colour = CheckTolerance(total, this.Limit);
                if (colour == FlagColour.None)
                {
                    return null;
                }

                decimal exceededlimit = total - this.Limit;
                Flags flags = new Flags(this.AccountID);
                flagResult = new LimitFlaggedItem(this.FlagDescription, this.CustomFlagText, this, colour, flags.GetAssociatedExpenses(associatedItems, connection), exceededlimit, this.FlagTypeDescription, this.NotesForAuthoriser, this.AssociatedExpenseItems, this.Action, this.CustomFlagText, this.ClaimantJustificationRequired, false);
            }
            
            return new List<FlaggedItem> { flagResult };
        }
    }
}
