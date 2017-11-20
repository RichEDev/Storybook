namespace SpendManagementLibrary.Flags
{
    using SpendManagementLibrary.Interfaces;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The invalid date flag.
    /// </summary>
    [Serializable]
    public class InvalidDateFlag : Flag
    {
        /// <summary>
        /// The invalid date flag type.
        /// </summary>
        private readonly InvalidDateFlagType invalidDateFlagType;

        /// <summary>
        /// Initialises a new instance of the <see cref="InvalidDateFlag"/> class.
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
        /// <param name="dateflagtype">
        /// Whether it is a set date or number of months
        /// </param>
        /// <param name="date">
        /// The set date if using the Initial Date dateflagtype
        /// </param>
        /// <param name="months">
        /// The number of months if using the number of months date flag type.
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
        public InvalidDateFlag(int flagid, FlagType flagtype, FlagAction action, string flagtext, List<int> associateditemroles, List<AssociatedExpenseItem> associatedexpenseitems, DateTime createdon, int? createdby, DateTime? modifiedon, int? modifiedby, InvalidDateFlagType dateflagtype, DateTime? date, byte? months, string description, bool active, int accountid, bool claimantjustificationrequired, bool displayflagimmediately, FlagColour flaglevel, bool approverjustificationrequired, string notesforauthoriser, FlagInclusionType itemroleinclusiontype, FlagInclusionType expenseiteminclusiontype)
            : base(flagid, flagtype, action, flagtext, associateditemroles, associatedexpenseitems, createdon, createdby, modifiedon, modifiedby, description, active, accountid, claimantjustificationrequired, displayflagimmediately, flaglevel, approverjustificationrequired, notesforauthoriser, "Invalid Date", string.Empty, itemroleinclusiontype, expenseiteminclusiontype, true, false, false, true, true)
        {
            this.invalidDateFlagType = dateflagtype;
            this.Date = date;
            this.Months = months;
            this.FlagDescription = this.GetFlagDescription();
        }
        #region properties
        /// <summary>
        /// Gets the type of date the flag monitors. E.g a set date or in the last X months
        /// </summary>
        public InvalidDateFlagType InvalidDateFlagType
        {
            get { return this.invalidDateFlagType; }
        }

        /// <summary>
        /// Gets the set date that if an item is before should be flagged
        /// </summary>
        public DateTime? Date { get; private set; }

        /// <summary>
        /// Gets the number of months the flag should allow before creating a flag
        /// </summary>
        public byte? Months { get; private set; }

        /// <summary>
        /// Gets the flag description.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string GetFlagDescription()
        {
            if (InvalidDateFlagType == InvalidDateFlagType.SetDate)
                {
                    if (!this.Date.HasValue)
                    {
                        return string.Empty;
                    }

                    return "Our policy does not permit claims before the " + this.Date.Value.ToLongDateString() + ".";
                }

                if (!this.Months.HasValue)
                {
                    return string.Empty;
                }

                return "Our policy does not permit claims older than " + this.Months + " months.";
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
        /// <returns>
        /// The <see cref="FlaggedItem"/>.
        /// </returns>
        public override List<FlaggedItem> Validate(cExpenseItem expenseItem, int employeeId, cAccountProperties properties, IDBConnection connection = null)
        {
            FlaggedItem flagResult = null;
            bool flag = false;

            DateTime expenseItemDate = expenseItem.date;

            if (InvalidDateFlagType == InvalidDateFlagType.SetDate)
            {
                if (!this.Date.HasValue)
                {
                    return null;
                }

                DateTime initialdate = this.Date.Value;
                if (expenseItemDate < initialdate)
                {
                    flag = true;
                }
            }
            else
            {
                DateTime initialdate = DateTime.Today;
                if (this.Months == null)
                {
                    return null;
                }

                initialdate = initialdate.AddMonths((int)this.Months / -1);
                if (expenseItemDate < initialdate)
                {
                    flag = true;
                }
            }

            if (flag)
            {
                flagResult = new FlaggedItem(this.FlagDescription, this.CustomFlagText, this, FlagLevel, this.FlagTypeDescription, this.NotesForAuthoriser, this.AssociatedExpenseItems, this.Action, this.CustomFlagText, this.ClaimantJustificationRequired, false);
            }

            return new List<FlaggedItem>() { flagResult };
        }
    }
}
