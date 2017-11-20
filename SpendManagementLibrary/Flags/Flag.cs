namespace SpendManagementLibrary.Flags
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;

    using SpendManagementLibrary.Helpers;
    using SpendManagementLibrary.Interfaces;

    using Spend_Management;

    using Utilities.DistributedCaching;

    /// <summary>
    /// The flag class.
    /// </summary>
    [Serializable]
    public abstract class Flag
    {
        /// <summary>
        /// The cache area.
        /// </summary>
        public const string CacheArea = "flags";

        /// <summary>
        /// Initialises a new instance of the <see cref="Flag"/> class.
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
        /// <param name="customflagtext">
        /// The custom flag text provided by administrators to show to claimants in the event of a breach.
        /// </param>
        /// <param name="associateditemroles">
        /// The item roles this flag applies to.
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
        /// <param name="flagTypeDescription">
        /// The flag Type Description.
        /// </param>
        /// <param name="flagDescription">
        /// The flag Description.
        /// </param>
        /// <param name="itemroleinclusiontype">
        /// The inclusion type of item roles for the flag. All items roles or a specified list.
        /// </param>
        /// <param name="expenseiteminclusiontype">
        /// The inclusion type of expense items for the flag. All expense items or a specified list.
        /// </param>
        /// <param name="validateWhenAddingAnExpense">
        /// The validate When Adding An Expense.
        /// </param>
        /// <param name="expenseitemselectionmandatory">
        /// A list of expense items must be specified for this flag and it cannot be applied to all items. Typically used when comparing groups of items.
        /// </param>
        /// <param name="requiresSaveToValidate">
        /// Whether the item needs to be saved to the database before validation can occur.
        /// </param>
        /// <param name="allowMultipleFlagsOfThisType">
        /// Whether multiple flags of this type can be created or whether it is subject to an item role/expense item combination check
        /// </param>
        protected Flag(int flagid, FlagType flagtype, FlagAction action, string customflagtext, List<int> associateditemroles, List<AssociatedExpenseItem> associatedexpenseitems, DateTime createdon, int? createdby, DateTime? modifiedon, int? modifiedby, string description, bool active, int accountid, bool claimantjustificationrequired, bool displayflagimmediately, FlagColour flaglevel, bool approverjustificationrequired, string notesforauthoriser, string flagTypeDescription, string flagDescription, FlagInclusionType itemroleinclusiontype, FlagInclusionType expenseiteminclusiontype, bool validateWhenAddingAnExpense, bool expenseitemselectionmandatory, bool requiresSaveToValidate, bool allowMultipleFlagsOfThisType, bool requiresRevalidationOnClaimSubmittal)
        {
            this.FlagID = flagid;
            this.FlagType = flagtype;
            this.Action = action;
            this.CustomFlagText = customflagtext;
            this.AssociatedExpenseItems = associatedexpenseitems;
            this.AssociatedItemRoles = associateditemroles;
            this.CreatedBy = createdby;
            this.CreatedOn = createdon;
            this.ModifiedBy = modifiedby;
            this.ModifiedOn = modifiedon;
            this.Description = description;
            this.Active = active;
            this.AccountID = accountid;
            this.ClaimantJustificationRequired = claimantjustificationrequired;
            this.DisplayFlagImmediately = displayflagimmediately;
            this.FlagLevel = flaglevel;
            this.ApproverJustificationRequired = approverjustificationrequired;
            this.NotesForAuthoriser = notesforauthoriser;
            this.FlagTypeDescription = flagTypeDescription;
            this.FlagDescription = flagDescription;
            this.ItemRoleInclusionType = itemroleinclusiontype;
            this.ExpenseItemInclusionType = expenseiteminclusiontype;
            this.ValidateWhenAddingAnExpense = validateWhenAddingAnExpense;
            this.ExpenseItemSelectionMandatory = expenseitemselectionmandatory;
            this.RequiresSaveToValidate = requiresSaveToValidate;
            this.AllowMultipleFlagsOfThisType = allowMultipleFlagsOfThisType;
            this.RequiresRevalidationOnClaimSubmittal = requiresRevalidationOnClaimSubmittal;
        }

        #region properties
        
        /// <summary>
        /// Gets the unique identifer of the flag
        /// </summary>
        public int FlagID { get; private set; }

        /// <summary>
        /// Gets the type of flag. E.g. duplicate, item with receipt exceeded
        /// </summary>
        public FlagType FlagType { get; private set; }

        /// <summary>
        /// Gets the action to take if the item needs to be flagged. E.g Block the item or flag it
        /// </summary>
        public FlagAction Action { get; private set; }

        /// <summary>
        /// Gets the custom message to be displayed when an item is flagged
        /// </summary>
        public string CustomFlagText { get; private set; }

        /// <summary>
        /// Gets the date the flag rule was created
        /// </summary>
        public DateTime CreatedOn { get; private set; }

        /// <summary>
        /// Gets the ID of the user who created the flag rule
        /// </summary>
        public int? CreatedBy { get; private set; }

        /// <summary>
        /// Gets the last date the flag rule was modified
        /// </summary>
        public DateTime? ModifiedOn { get; private set; }

        /// <summary>
        /// Gets the ID of the user who last modified the flag rule
        /// </summary>
        public int? ModifiedBy { get; private set; }

        /// <summary>
        /// Gets the description of the flag
        /// </summary>
        public string Description { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the flag is active or not
        /// </summary>
        public bool Active { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the claimant must supply a justification if the item has been flagged
        /// </summary>
        public bool ClaimantJustificationRequired { get; private set; }

        /// <summary>
        /// Gets a value indicating whether as soon as the expense has been added the claimant should be notified about this flag
        /// </summary>
        public bool DisplayFlagImmediately { get; private set; }

        /// <summary>
        /// Gets which item roles to include, all or a list provided
        /// </summary>
        public FlagInclusionType ItemRoleInclusionType { get; private set; }

        /// <summary>
        /// Gets the associated roles.
        /// </summary>
        public List<int> AssociatedItemRoles { get; private set; }

        /// <summary>
        /// Gets which expense items to include, all or a list provided
        /// </summary>
        public FlagInclusionType ExpenseItemInclusionType { get; private set; }

        /// <summary>
        /// Gets the associated expense items.
        /// </summary>
        public List<AssociatedExpenseItem> AssociatedExpenseItems { get; private set; }

        /// <summary>
        /// Gets the flag level of the flag. Red, amber or information only
        /// </summary>
        public FlagColour FlagLevel { get; private set; }

        /// <summary>
        /// Gets a value indicating whether an approver must provide a justification for allowing the expense
        /// </summary>
        public bool ApproverJustificationRequired { get; private set; }

        /// <summary>
        /// Gets or sets the friendly description of the type of this flag. Duplicate Expenses, Limit without a receipt for example
        /// </summary>
        public string FlagTypeDescription { get; protected set; }

        /// <summary>
        /// Gets or sets the friendly detailed message as to what this flag is
        /// </summary>
        public string FlagDescription { get; protected set; }

        /// <summary>
        /// Gets the notes for authoriser.
        /// </summary>
        public string NotesForAuthoriser { get; private set; }

        /// <summary>
        /// Gets whether the flag should be validated when the claimant is adding the expense
        /// </summary>
        public bool ValidateWhenAddingAnExpense { get; private set; }

        /// <summary>
        /// Gets whether a list of expense items must be specified for this flag and it cannot be applied to all items. Typically used when comparing groups of items.
        /// </summary>
        public bool ExpenseItemSelectionMandatory { get; private set; }

        /// <summary>
        /// Gets or sets the account id.
        /// </summary>
        protected int AccountID { get; set; }

        /// <summary>
        /// Gets whether the item needs to be saved to the database before validation can occur.
        /// </summary>
        public bool RequiresSaveToValidate { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether multiple flags of this type can be created or whether it is subject to an item role/expense item combination check
        /// </summary>
        public bool AllowMultipleFlagsOfThisType { get; protected set; }

        /// <summary>
        /// Gets or sets whether te flag needs to be revalidated when a claim is submitted
        /// </summary>
        public bool RequiresRevalidationOnClaimSubmittal { get; protected set; }
        #endregion

        /// <summary>
        /// Adds a new flag or updates an existing one
        /// </summary>
        /// <param name="connection">The database connection</param>
        /// <returns>The ID of the new flag</returns>
        public int Save(ICurrentUserBase currentUser, IDBConnection connection = null)
        {
            using (
                var databaseConnection = connection
                                         ?? new DatabaseConnection(cAccounts.getConnectionString(this.AccountID)))
            {
                databaseConnection.sqlexecute.Parameters.AddWithValue("@flagID", this.FlagID);
                databaseConnection.sqlexecute.Parameters.AddWithValue("@flagType", (byte)this.FlagType);
                databaseConnection.sqlexecute.Parameters.AddWithValue("@action", (byte)this.Action);
                if (this.CustomFlagText == string.Empty)
                {
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@flagText", this.CustomFlagText);
                }
                else
                {
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@flagText", this.CustomFlagText);
                }

                databaseConnection.sqlexecute.Parameters.AddWithValue("@amberTolerance", DBNull.Value);
                databaseConnection.sqlexecute.Parameters.AddWithValue("@noFlagTolerance", DBNull.Value);
                databaseConnection.sqlexecute.Parameters.AddWithValue("@frequency", DBNull.Value);
                databaseConnection.sqlexecute.Parameters.AddWithValue("@frequencyType", DBNull.Value);
                databaseConnection.sqlexecute.Parameters.AddWithValue("@period", DBNull.Value);
                databaseConnection.sqlexecute.Parameters.AddWithValue("@periodType", DBNull.Value);
                databaseConnection.sqlexecute.Parameters.AddWithValue("@limit", DBNull.Value);
                databaseConnection.sqlexecute.Parameters.AddWithValue("@dateComparison", DBNull.Value);
                databaseConnection.sqlexecute.Parameters.AddWithValue("@dateToCompare", DBNull.Value);
                databaseConnection.sqlexecute.Parameters.AddWithValue("@numberOfMonths", DBNull.Value);
                databaseConnection.sqlexecute.Parameters.AddWithValue("@financialYear", DBNull.Value);
                databaseConnection.sqlexecute.Parameters.AddWithValue("@tipLimit", DBNull.Value);
                databaseConnection.sqlexecute.Parameters.AddWithValue("@increaseByNumOthers", 0);
                databaseConnection.sqlexecute.Parameters.AddWithValue("@displayLimit", 0);
                if (this.Description == string.Empty)
                {
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@description", DBNull.Value);
                }
                else
                {
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@description", this.Description);
                }

                databaseConnection.sqlexecute.Parameters.AddWithValue("@active", Convert.ToByte(this.Active));
                databaseConnection.sqlexecute.Parameters.AddWithValue(
                    "@claimantJustificationRequired",
                    Convert.ToByte(this.ClaimantJustificationRequired));
                databaseConnection.sqlexecute.Parameters.AddWithValue(
                    "@displayFlagImmediately",
                    Convert.ToByte(this.DisplayFlagImmediately));
                databaseConnection.sqlexecute.Parameters.AddWithValue("@flagLevel", (byte)this.FlagLevel);
                databaseConnection.sqlexecute.Parameters.AddWithValue(
                    "@approverJustificationRequired",
                    Convert.ToByte(this.ApproverJustificationRequired));
                if (this.NotesForAuthoriser == string.Empty)
                {
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@notesForAuthoriser", DBNull.Value);
                }
                else
                {
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@notesForAuthoriser", this.NotesForAuthoriser);
                }

                databaseConnection.sqlexecute.Parameters.AddWithValue("@itemRoleInclusionType", (byte)this.ItemRoleInclusionType);
                databaseConnection.sqlexecute.Parameters.AddWithValue(
                    "@expenseItemInclusionType",
                    (byte)this.ExpenseItemInclusionType);
                databaseConnection.sqlexecute.Parameters.AddWithValue(
                    "@performItemRoleExpenseCheck",
                    Convert.ToByte(!this.AllowMultipleFlagsOfThisType));
                databaseConnection.sqlexecute.Parameters.Add("@identity", SqlDbType.Int);
                databaseConnection.sqlexecute.Parameters["@identity"].Direction = ParameterDirection.ReturnValue;
                if (this.FlagID > 0)
                {
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@date", this.ModifiedOn);
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@userid", this.ModifiedBy);
                }
                else
                {
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@date", this.CreatedOn);
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@userid", this.CreatedBy);
                }

                if (currentUser != null)
                {
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@employeeID", currentUser.EmployeeID);
                    if (currentUser.isDelegate)
                    {
                        databaseConnection.sqlexecute.Parameters.AddWithValue("@delegateID", currentUser.Delegate.EmployeeID);
                    }
                    else
                    {
                        databaseConnection.sqlexecute.Parameters.AddWithValue("@delegateID", DBNull.Value);
                    }
                }
                else
                {
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@employeeID", 0);
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@delegateID", DBNull.Value);
                }

                databaseConnection.sqlexecute.Parameters.AddWithValue("@passengerLimit", DBNull.Value);
                switch (FlagType)
                {
                    case FlagType.GroupLimitWithoutReceipt:
                    case FlagType.GroupLimitWithReceipt:
                        GroupLimitFlag grouplimitflag = (GroupLimitFlag)this;
                        if (grouplimitflag.AmberTolerance != null)
                        {
                            databaseConnection.sqlexecute.Parameters["@amberTolerance"].Value = grouplimitflag.AmberTolerance;
                        }

                        if (grouplimitflag.NoFlagTolerance != null)
                        {
                            databaseConnection.sqlexecute.Parameters["@noFlagTolerance"].Value = grouplimitflag.NoFlagTolerance;
                        }

                        databaseConnection.sqlexecute.Parameters["@limit"].Value = grouplimitflag.Limit;
                        break;
                    case FlagType.LimitWithoutReceipt:
                    case FlagType.LimitWithReceipt:
                        LimitFlag limitflag = (LimitFlag)this;
                        if (limitflag.AmberTolerance != null)
                        {
                            databaseConnection.sqlexecute.Parameters["@amberTolerance"].Value = limitflag.AmberTolerance;
                        }

                        if (limitflag.NoFlagTolerance != null)
                        {
                            databaseConnection.sqlexecute.Parameters["@noFlagTolerance"].Value = limitflag.NoFlagTolerance;
                        }

                        databaseConnection.sqlexecute.Parameters["@increaseByNumOthers"].Value =
                            Convert.ToByte(limitflag.IncreaseByNumOthers);
                        databaseConnection.sqlexecute.Parameters["@displayLimit"].Value = Convert.ToByte(limitflag.DisplayLimit);
                        break;
                    case FlagType.MileageExceeded:
                        MileageFlag mileageflag = (MileageFlag)this;
                        if (mileageflag.AmberTolerance != null)
                        {
                            databaseConnection.sqlexecute.Parameters["@amberTolerance"].Value = mileageflag.AmberTolerance;
                        }

                        if (mileageflag.NoFlagTolerance != null)
                        {
                            databaseConnection.sqlexecute.Parameters["@noFlagTolerance"].Value = mileageflag.NoFlagTolerance;
                        }

                        break;
                    case FlagType.TipLimitExceeded:
                        TipFlag tipFlag = (TipFlag)this;
                        if (tipFlag.AmberTolerance != null)
                        {
                            databaseConnection.sqlexecute.Parameters["@amberTolerance"].Value = tipFlag.AmberTolerance;
                        }

                        if (tipFlag.NoFlagTolerance != null)
                        {
                            databaseConnection.sqlexecute.Parameters["@noFlagTolerance"].Value = tipFlag.NoFlagTolerance;
                        }

                        databaseConnection.sqlexecute.Parameters["@tipLimit"].Value = tipFlag.TipLimit;
                        break;
                    case FlagType.FrequencyOfItemCount:
                    case FlagType.FrequencyOfItemSum:
                        FrequencyFlag frequencyflag = (FrequencyFlag)this;
                        if (frequencyflag.Frequency != null)
                        {
                            databaseConnection.sqlexecute.Parameters["@frequency"].Value = frequencyflag.Frequency;
                        }

                        databaseConnection.sqlexecute.Parameters["@frequencyType"].Value = (byte)frequencyflag.FrequencyType;
                        databaseConnection.sqlexecute.Parameters["@period"].Value = frequencyflag.Period;
                        databaseConnection.sqlexecute.Parameters["@periodType"].Value = (byte)frequencyflag.PeriodType;
                        if (frequencyflag.Limit != null)
                        {
                            databaseConnection.sqlexecute.Parameters["@limit"].Value = frequencyflag.Limit;
                        }

                        if (frequencyflag.PeriodType == FlagPeriodType.FinancialYears)
                        {
                            databaseConnection.sqlexecute.Parameters["@financialYear"].Value = frequencyflag.FinancialYear;
                        }

                        break;
                    case FlagType.InvalidDate:
                        InvalidDateFlag invaliddateflag = (InvalidDateFlag)this;
                        databaseConnection.sqlexecute.Parameters["@dateComparison"].Value = (byte)invaliddateflag.InvalidDateFlagType;
                        if (invaliddateflag.Date != null)
                        {
                            databaseConnection.sqlexecute.Parameters["@dateToCompare"].Value = invaliddateflag.Date;
                        }

                        if (invaliddateflag.Months != null)
                        {
                            databaseConnection.sqlexecute.Parameters["@numberOfMonths"].Value = invaliddateflag.Months;
                        }

                        break;
                        case FlagType.NumberOfPassengersLimit:
                        NumberOfPassengersFlag passengersFlag = (NumberOfPassengersFlag)this;
                        databaseConnection.sqlexecute.Parameters["@passengerLimit"].Value =
                            passengersFlag.PassengerLimit;
                        break;
                    case FlagType.RestrictDailyMileage:
                        var restrictMileageFlag = (RestrictDailyMileageFlag)this;
                        databaseConnection.sqlexecute.Parameters["@limit"].Value = restrictMileageFlag.DailyMileageLimit;
                        break;
                }

                databaseConnection.ExecuteProc("saveFlagRule");
                this.FlagID = (int)databaseConnection.sqlexecute.Parameters["@identity"].Value;
                databaseConnection.sqlexecute.Parameters.Clear();

                if (this.FlagID < 0)
                {
                    return this.FlagID;
                }

                if (FlagType == FlagType.Custom)
                {
                    this.SaveFlagCriteria(databaseConnection);
                }
            }

            Cache caching = new Cache();
            caching.Delete(this.AccountID, CacheArea, "0");

            return this.FlagID;
        }
        
        /// <summary>
        /// The contains roles.
        /// </summary>
        /// <param name="roles">
        /// The roles.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool ContainsItemRoles(List<int> roles)
        {
            return roles.Any(i => this.AssociatedItemRoles.Contains(i));
        }

        /// <summary>
        /// The validate.
        /// </summary>
        /// <param name="expenseItem">
        /// The expense item.
        /// </param>
        /// <param name="employeeId">
        /// The employee id.
        /// </param>
        /// <returns>
        /// The <see cref="FlaggedItem"/>.
        /// </returns>
        public abstract List<FlaggedItem> Validate(cExpenseItem expenseItem, int employeeId, cAccountProperties properties, IDBConnection connection = null);

        /// <summary>
        /// The save flag criteria.
        /// </summary>
        /// <param name="connection">
        /// The connection.
        /// </param>
        private void SaveFlagCriteria(IDBConnection connection = null)
        {
            using (
                var databaseConnection = connection
                                         ?? new DatabaseConnection(cAccounts.getConnectionString(this.AccountID)))
            {
                foreach (cReportCriterion criteria in ((CustomFlag)this).Criteria.Values)
                {
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@flagid", this.FlagID);
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@fieldid", criteria.field.FieldID);
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@condition", (byte)criteria.condition);
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@order", criteria.order);
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@andor", (byte)criteria.joiner);
                    if (criteria.field.GenList == false)
                    {
                        if (criteria.value1 == null)
                        {
                            databaseConnection.sqlexecute.Parameters.AddWithValue("@value1", DBNull.Value);
                        }
                        else
                        {
                            databaseConnection.sqlexecute.Parameters.AddWithValue(
                                "@value1",
                                criteria.value1[0] ?? DBNull.Value);
                        }

                        if (criteria.condition == ConditionType.Between && !criteria.runtime)
                        {
                            databaseConnection.sqlexecute.Parameters.AddWithValue("@value2", criteria.value2[0]);
                        }
                        else
                        {
                            databaseConnection.sqlexecute.Parameters.AddWithValue("@value2", DBNull.Value);
                        }
                    }
                    else
                    {
                        if (criteria.value1 == null)
                        {
                            databaseConnection.sqlexecute.Parameters.AddWithValue("@value1", DBNull.Value);
                        }
                        else
                        {
                            databaseConnection.sqlexecute.Parameters.AddWithValue(
                                "@value1",
                                criteria.value1[0] ?? DBNull.Value);
                        }

                        databaseConnection.sqlexecute.Parameters.AddWithValue("@value2", DBNull.Value);
                    }

                    if (criteria.groupnumber == 0)
                    {
                        databaseConnection.sqlexecute.Parameters.AddWithValue("@groupnumber", DBNull.Value);
                    }
                    else
                    {
                        databaseConnection.sqlexecute.Parameters.AddWithValue("@groupnumber", criteria.groupnumber);
                    }

                    if (criteria.JoinVia == null)
                    {
                        databaseConnection.sqlexecute.Parameters.AddWithValue("@joinViaID", DBNull.Value);
                    }
                    else
                    {
                        databaseConnection.sqlexecute.Parameters.AddWithValue("@joinViaID", criteria.JoinVia.JoinViaID);
                    }

                    databaseConnection.ExecuteProc("saveFlagCriteria");

                    databaseConnection.sqlexecute.Parameters.Clear();
                }
            }
        }

        /// <summary>
        /// Checks whether the expense has already been associated with another expense as part of a flag
        /// </summary>
        /// <param name="expenseID">
        /// The id of the expense to check.
        /// </param>
        /// <param name="connection">
        /// The connection.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        protected bool HasFlagAlreadyBeenAssociatedWithExpense(int expenseID, IDBConnection connection = null)
        {
            using (
                var databaseConnection = connection
                                         ?? new DatabaseConnection(cAccounts.getConnectionString(this.AccountID)))
            {
                databaseConnection.sqlexecute.Parameters.AddWithValue("@expenseid", expenseID);
                databaseConnection.sqlexecute.Parameters.AddWithValue("@flagID", this.FlagID);
                int count = databaseConnection.ExecuteScalar<int>(
                    "GetExpensesBeenAssociatedWithFlagCount",
                    CommandType.StoredProcedure);
                databaseConnection.sqlexecute.Parameters.Clear();
                return count != 0;
            }
        }
    }
}
