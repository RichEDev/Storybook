namespace SpendManagementLibrary.Flags
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;

    using SpendManagementLibrary.Interfaces;

    /// <summary>
    /// The custom flag.
    /// </summary>
    [Serializable]
    public class CustomFlag : Flag
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="CustomFlag"/> class.
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
        /// <param name="associateditemsroles">
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
        /// <param name="criteria">
        /// The custom criteria to validate.
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
        public CustomFlag(int flagid, FlagType flagtype, FlagAction action, string flagtext, List<int> associateditemsroles, List<AssociatedExpenseItem> associatedexpenseitems, DateTime createdon, int? createdby, DateTime? modifiedon, int? modifiedby, string description, bool active, int accountid, bool claimantjustificationrequired, bool displayflagimmediately, FlagColour flaglevel, bool approverjustificationrequired, Dictionary<int, cReportCriterion> criteria, string notesforauthoriser, FlagInclusionType itemroleinclusiontype, FlagInclusionType expenseiteminclusiontype)
            : base(flagid, flagtype, action, flagtext, associateditemsroles, associatedexpenseitems, createdon, createdby, modifiedon, modifiedby, description, active, accountid, claimantjustificationrequired, displayflagimmediately, flaglevel, approverjustificationrequired, notesforauthoriser, "Custom", string.Empty, itemroleinclusiontype, expenseiteminclusiontype, true, false, true, true, false)
        {
            this.Criteria = criteria;
        }
        
        #region Properties

        /// <summary>
        /// Gets the criteria.
        /// </summary>
        public Dictionary<int, cReportCriterion> Criteria { get; private set; }
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
        /// Not currently used
        /// </exception>
        public override List<FlaggedItem> Validate(cExpenseItem expenseItem, int employeeId, cAccountProperties properties, IDBConnection connection = null)
        {
            FlaggedItem flagResult = null;

            cTables clstables = new cTables(AccountID);
            cFields clsfields = new cFields(AccountID);

            cField expenseIDField = clsfields.GetFieldByID(new Guid("A528DE93-3037-46F6-974C-A76BD0C8642A"));

            cQueryBuilder query = new cQueryBuilder(AccountID, cAccounts.getConnectionString(AccountID), ConfigurationManager.ConnectionStrings["metabase"].ConnectionString, clstables.GetTableByID(new Guid("D70D9E5F-37E2-4025-9492-3BCF6AA746A8")), new cTables(AccountID), new cFields(AccountID));
            query.addColumn(expenseIDField, SelectType.Count);

            query.addFilter(new cQueryFilter(expenseIDField, ConditionType.Equals, new List<object>() {expenseItem.expenseid} , null, ConditionJoiner.And, null));
            foreach (cReportCriterion criterion in this.Criteria.Values)
            {
                object[] value1 = criterion.value1.ToArray();
                object[] value2 = criterion.value2.ToArray();
                query.addFilter(criterion.field, criterion.condition, value1, value2, criterion.joiner, criterion.JoinVia);    
            }

            int count = query.GetCount();
            if (count > 0)
            {
                flagResult = new FlaggedItem(
                        this.FlagDescription,
                        this.CustomFlagText,
                        this,
                        FlagLevel,
                        this.FlagTypeDescription, 
                        this.NotesForAuthoriser,
                        this.AssociatedExpenseItems,
                        this.Action,
                        this.CustomFlagText,
                        this.ClaimantJustificationRequired,
                        false);
            }
            return new List<FlaggedItem>() { flagResult };
        }
    }
}
