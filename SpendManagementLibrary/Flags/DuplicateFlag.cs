namespace SpendManagementLibrary.Flags
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data;
    using System.Data.SqlClient;
    using System.Linq;

    using SpendManagementLibrary.Helpers;
    using SpendManagementLibrary.Interfaces;

    /// <summary>
    /// The duplicate flag.
    /// </summary>
    [Serializable]
    public class DuplicateFlag : Flag
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="DuplicateFlag"/> class.
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
        /// <param name="associatedfields">
        /// The fields to check when validating if it is a duplicate.
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
        public DuplicateFlag(int flagid, FlagType flagtype, FlagAction action, string flagtext, List<int> associateditemroles, List<AssociatedExpenseItem> associatedexpenseitems, DateTime createdon, int? createdby, DateTime? modifiedon, int? modifiedby, List<Guid> associatedfields, string description, bool active, int accountid, bool claimantjustificationrequired, bool displayflagimmediately, FlagColour flaglevel, bool approverjustificationrequired, string notesforauthoriser, FlagInclusionType itemroleinclusiontype, FlagInclusionType expenseiteminclusiontype)
            : base(flagid, flagtype, action, flagtext, associateditemroles, associatedexpenseitems, createdon, createdby, modifiedon, modifiedby, description, active, accountid, claimantjustificationrequired, displayflagimmediately, flaglevel, approverjustificationrequired, notesforauthoriser, "Duplicate expense", "We seem to have a very similar item that you have already added.", itemroleinclusiontype, expenseiteminclusiontype, true, false, true, true, false)
        {
            this.AssociatedFields = associatedfields;
        }

        #region properties

        /// <summary>
        /// Gets the associated fields.
        /// </summary>
        public List<Guid> AssociatedFields { get; private set; }
        
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
        /// The global properties class.
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

            FlaggedItem flagResult = null;

            if (this.AssociatedFields.Count == 0 || !this.AllFieldsAvailableToValidate(expenseItem.subcatid, connection))
            {
                return null;
            }
            
            cTables clstables = new cTables(AccountID);
            cFields clsfields = new cFields(AccountID);
            cQueryBuilder addedItemQuery = this.GetItemValuesQuery(clstables, clsfields, expenseItem.expenseid, connection);
            SortedList<string, object> values = new SortedList<string, object>();
            using (SqlDataReader reader = addedItemQuery.getReader())
            {
                while (reader.Read())
                {
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        Guid fieldName;
                        if (Guid.TryParse(reader.GetName(i), out fieldName))
                        {
                            values.Add(reader.GetName(i), reader.GetValue(i));
                        }
                    }
                }

                reader.Close();
            }
                        
            List<object> v;
            cQueryBuilder query = new cQueryBuilder(AccountID, cAccounts.getConnectionString(AccountID), ConfigurationManager.ConnectionStrings["metabase"].ConnectionString, clstables.GetTableByID(new Guid("D70D9E5F-37E2-4025-9492-3BCF6AA746A8")), new cTables(AccountID), new cFields(AccountID));
            query.addColumn(clsfields.GetFieldByID(new Guid("A528DE93-3037-46F6-974C-A76BD0C8642A")));

            foreach (KeyValuePair<string, object> i in values)
            {
                v = new List<object> { i.Value };
                cField field = clsfields.GetFieldByID(new Guid(i.Key));
                
                query.addFilter(new cQueryFilter(field, ConditionType.Equals, v, new List<object>(), ConditionJoiner.And, null));
            }

            v = new List<object> { expenseItem.expenseid };
            
            query.addFilter(new cQueryFilter(clsfields.GetFieldByID(new Guid("A528DE93-3037-46F6-974C-A76BD0C8642A")), ConditionType.DoesNotEqual, v, new List<object>(), ConditionJoiner.And, null));
            query.addFilter(clsfields.GetFieldByID(new Guid("8F61ABE2-96DE-4D3F-9E91-FDF2D47800CB")), ConditionType.Equals, new object[] { expenseItem.subcatid }, new object[] { }, ConditionJoiner.And, null);
            query.addFilter(clsfields.GetFieldByID(new Guid("2501BE3D-AA94-437D-98BB-A28788A35DC4")), ConditionType.Equals, new object[] { employeeId }, new object[] { }, ConditionJoiner.And, null);
            using (SqlDataReader reader = query.getReader())
            {
                if (reader.HasRows)
                {
                    reader.Read();
                    int duplicateId = reader.GetInt32(0);
                    Flags flags = new Flags(this.AccountID);
                    List<int> ids = new List<int> { duplicateId };
                    List<AssociatedExpense> expenses = flags.GetAssociatedExpenses(ids, connection);
                    cFields fields = new cFields(this.AccountID);
                    List<string> duplicateFields = this.AssociatedFields.Select(fieldId => fields.GetFieldByID(fieldId).Description).ToList();

                    duplicateFields.Sort();
                    if (expenses != null)
                    {
                        flagResult = new DuplicateFlaggedItem(
                            this.FlagDescription,
                            this.CustomFlagText,
                            this,
                            FlagLevel,
                            expenses[0],
                            this.FlagTypeDescription,
                            this.NotesForAuthoriser,
                            this.AssociatedExpenseItems,
                            this.Action,
                            this.CustomFlagText,
                            duplicateFields,
                            this.ClaimantJustificationRequired,
                            false);
                    }
                }

                reader.Close();
            }

            return new List<FlaggedItem> { flagResult };
        }

        /// <summary>
        /// Gets whether all the fields that should be checked by this flag are enabled for the claimant to input a value
        /// </summary>
        /// <param name="subcatID">The ID of the sub category to check</param>
        /// <param name="connection">Database connection</param>
        /// <returns>Whether the flag can be validated or not</returns>
        private bool AllFieldsAvailableToValidate(int subcatID, IDBConnection connection = null)
        {
            bool canValidateFlag = true;
            using (
                var databaseConnection = connection
                                         ?? new DatabaseConnection(cAccounts.getConnectionString(this.AccountID)))
            {
                databaseConnection.sqlexecute.Parameters.AddWithValue("@flagID", this.FlagID);
                databaseConnection.sqlexecute.Parameters.AddWithValue("@subcatID", subcatID);
                using (
                    IDataReader reader = databaseConnection.GetReader(
                        "GetFieldStatusForDuplicateChecking",
                        CommandType.StoredProcedure))
                {
                    while (reader.Read())
                    {
                        if (reader.GetInt32(0) > 0)
                        {
                            canValidateFlag = false;
                        }
                    }

                    reader.Close();
                }

                databaseConnection.sqlexecute.Parameters.Clear();
            }

            return canValidateFlag;
        }

        /// <summary>
        /// Gets the query to determine a duplicate.
        /// </summary>
        /// <param name="clstables">
        /// An instance of the tables class.
        /// </param>
        /// <param name="clsfields">
        /// an instance of the fields class.
        /// </param>
        /// <param name="expenseid">
        /// The expenseid.
        /// </param>
        /// <param name="connection">
        /// The connection.
        /// </param>
        /// <returns>
        /// The <see cref="cQueryBuilder"/>.
        /// </returns>
        internal cQueryBuilder GetItemValuesQuery(
            cTables clstables,
            cFields clsfields,
            int expenseid,
            IDBConnection connection = null)
        {
            cQueryBuilder addedItemQuery = new cQueryBuilder(AccountID, cAccounts.getConnectionString(AccountID), ConfigurationManager.ConnectionStrings["metabase"].ConnectionString, clstables.GetTableByID(new Guid("D70D9E5F-37E2-4025-9492-3BCF6AA746A8")), new cTables(AccountID), new cFields(AccountID));
            foreach (Guid fieldid in this.AssociatedFields)
            {
                addedItemQuery.addColumn(clsfields.GetFieldByID(fieldid), fieldid.ToString());
            }

            addedItemQuery.addFilter(clsfields.GetFieldByID(new Guid("A528DE93-3037-46F6-974C-A76BD0C8642A")), ConditionType.Equals, new object[] { expenseid }, new object[] { }, ConditionJoiner.None, null);
            return addedItemQuery;
        }
    }
}
