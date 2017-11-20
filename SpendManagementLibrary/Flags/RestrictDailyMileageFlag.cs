

namespace SpendManagementLibrary.Flags
{
    using System;
    using System.Data;
    using System.Linq;
    using Helpers;
    using System.Collections.Generic;
    using Interfaces;

    /// <summary>
    /// A class to flag any claimants claiming more than a specific number of miles in a single day.
    /// </summary>
    [Serializable]
    public class RestrictDailyMileageFlag : Flag
    {
        /// <summary>
        /// Initialises a new instance of <see cref="RestrictDailyMileageFlag"/>
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
        /// <param name="customflagtext">The custom flag text (if any)</param>
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
        /// <param name="dailyMileageLimit">the mileage limit for this flag.</param>
        public RestrictDailyMileageFlag(int flagid, FlagType flagtype, FlagAction action, string customflagtext, List<int> associateditemroles, List<AssociatedExpenseItem> associatedexpenseitems, DateTime createdon, int? createdby, DateTime? modifiedon, int? modifiedby, string description, bool active, int accountid, bool claimantjustificationrequired, bool displayflagimmediately, FlagColour flaglevel, bool approverjustificationrequired, string notesforauthoriser,  FlagInclusionType itemroleinclusiontype, FlagInclusionType expenseiteminclusiontype, decimal dailyMileageLimit) 
            : base(flagid, flagtype, action, customflagtext, associateditemroles, associatedexpenseitems, createdon, createdby, modifiedon, modifiedby, description, active, accountid, claimantjustificationrequired, displayflagimmediately, flaglevel, approverjustificationrequired, notesforauthoriser, "Restrict daily mileage", string.Empty, itemroleinclusiontype, expenseiteminclusiontype, true, false, false, true, false)

        {
            this.DailyMileageLimit = dailyMileageLimit;
        }

        
        /// <summary>
        /// Gets the maximum number of miles allowed in a single day.
        /// </summary>
        public decimal DailyMileageLimit { get; private set; }

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
            var result = new List<FlaggedItem>();

            using (
                var databaseConnection = connection
                                         ?? new DatabaseConnection(cAccounts.getConnectionString(this.AccountID)))
            {
                databaseConnection.AddWithValue("@employeeid", employeeId);
                databaseConnection.AddWithValue("@date", expenseItem.date);
                var sql =
                    "select numActualmiles, savedexpenses.expenseid from savedexpenses_journey_steps inner join savedexpenses on savedexpenses.expenseid = savedexpenses_journey_steps.expenseid inner join claims_base on claims_base.claimid = savedexpenses.claimid inner join subcats on subcats.subcatid = savedexpenses.subcatid WHERE([claims_base].[employeeid]  IN(@employeeid)) AND([savedexpenses].[date]  IN(@date))  ";
                if (this.AssociatedExpenseItems != null && this.AssociatedExpenseItems.Count > 0)
                {
                    var subcats = this.AssociatedExpenseItems.Select(associatedExpenseItem => associatedExpenseItem.SubcatID).ToList();
                    databaseConnection.AddWithValue("@subcats", subcats);
                    sql += "AND ([savedexpenses].[subcatid]   IN (SELECT c1 from @subcats))";
                }
                var totalMiles = decimal.Zero;
                var currentMiles = decimal.Zero;
                if (expenseItem.expenseid == 0)
                {
                    currentMiles = expenseItem.miles;  //The expenseItem is not correctly populated when called after edit or delete.
                }

                var associatedExpenses = new List<int>();
                using (IDataReader reader = databaseConnection.GetReader(sql)) 
                {
                    if (reader != null)
                    {
                        while (reader.Read())
                        {
                            if (!reader.IsDBNull(0))
                            {
                                var expenseId = reader.GetInt32(1);
                                if (expenseId == expenseItem.expenseid)
                                {
                                    currentMiles = reader.GetDecimal(0);
                                }
                                else
                                {
                                    totalMiles += reader.GetDecimal(0);
                                    if (!associatedExpenses.Contains(expenseId))
                                    {
                                        associatedExpenses.Add(expenseId);
                                    }
                                }
                                
                            }
                        }
                    }
                }

                if (totalMiles + currentMiles > this.DailyMileageLimit)
                {
                    Flags flags = new Flags(this.AccountID);
                    var message = $"The daily mileage limit is {this.DailyMileageLimit} miles, the total mileage of all expense items on this date exceeds the limit by {totalMiles + currentMiles - this.DailyMileageLimit} miles.";
                    result.Add(new FlaggedItem(message, this.CustomFlagText, this, this.FlagLevel, flags.GetAssociatedExpenses(associatedExpenses), this.FlagTypeDescription, this.NotesForAuthoriser, this.AssociatedExpenseItems, this.Action, this.CustomFlagText, this.ClaimantJustificationRequired, false));
                }
            }

            return result;
        }
    }
}
