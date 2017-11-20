using SpendManagementLibrary.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpendManagementLibrary.Flags
{
    [Serializable]
    public class NumberOfPassengersFlag : Flag
    {
                // <summary>
        /// Initialises a new instance of the <see cref="NonReimbursableFlag"/> class.
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
        public NumberOfPassengersFlag(int flagid, FlagType flagtype, FlagAction action, string flagtext, List<int> associateditemroles, List<AssociatedExpenseItem> associatedexpenseitems, DateTime createdon, int? createdby, DateTime? modifiedon, int? modifiedby, string description, bool active, int accountid, bool claimantjustificationrequired, bool displayflagimmediately, FlagColour flaglevel, bool approverjustificationrequired, string notesforauthoriser, FlagInclusionType itemroleinclusiontype, FlagInclusionType expenseiteminclusiontype, int passengerLimit)
            : base(flagid, flagtype, action, flagtext, associateditemroles, associatedexpenseitems, createdon, createdby, modifiedon, modifiedby, description, active, accountid, claimantjustificationrequired, displayflagimmediately, flaglevel, approverjustificationrequired, notesforauthoriser, "Passenger limit exceeded", string.Empty, itemroleinclusiontype, expenseiteminclusiontype, true, false, false, true, false)
        {
            this.PassengerLimit = passengerLimit;
        }

        #region Properties
        /// <summary>
        /// Gets the number of passengers this flag should be limited to
        /// </summary>
        public int PassengerLimit { get; private set; }
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
        /// Implemented elsewhere.
        /// </exception>
        public override List<FlaggedItem> Validate(cExpenseItem expenseItem, int employeeId, cAccountProperties properties, IDBConnection connection = null)
        {
            if (expenseItem.journeysteps.Count == 0)
            {
                return null;
            }

            List<FlaggedItem> flaggedItems = new List<FlaggedItem>();
            MileageFlaggedItem flaggedItem = null;
            foreach (cJourneyStep step in expenseItem.journeysteps.Values)
            {
                if (step.passengers == null)
                {
                    return null;
                }

                if (step.passengers.Count() > this.PassengerLimit)
                {
                    if (flaggedItem == null)
                    {
                        flaggedItem =
                            new MileageFlaggedItem(string.Empty
                                ,
                                this.CustomFlagText,
                                this,
                                this.FlagLevel,
                                this.FlagTypeDescription,
                                this.NotesForAuthoriser,
                                this.AssociatedExpenseItems,
                                this.Action,
                                this.CustomFlagText,
                                this.ClaimantJustificationRequired,
                                false);
                    }
                    flaggedItem.AddFlaggedJourneyStep(step.stepnumber+1, step.passengers.Count() - this.PassengerLimit,"The number of passengers for step " + (step.stepnumber+1) + " is " + step.passengers.Count() + ". The number of passengers allowed is " + this.PassengerLimit + ".", 0);
                }
            }
            flaggedItems.Add(flaggedItem);
            return flaggedItems;
        }
    }
}
