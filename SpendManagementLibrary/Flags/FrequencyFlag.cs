namespace SpendManagementLibrary.Flags
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data.SqlClient;
    using System.Text;

    using SpendManagementLibrary.Interfaces;

    /// <summary>
    /// The frequency flag.
    /// </summary>
    [Serializable]
    public class FrequencyFlag : Flag
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="FrequencyFlag"/> class.
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
        /// <param name="frequency">
        /// The frequency.
        /// </param>
        /// <param name="frequencyType">
        /// The frequency the flag will be checked on. Every/In the last
        /// </param>
        /// <param name="period">
        /// The period the flag will be checked against.
        /// </param>
        /// <param name="periodtype">
        /// The period type. Daily, weekly, monthly etc
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
        public FrequencyFlag(int flagid, FlagType flagtype, FlagAction action, string flagtext, List<int> associateditemroles, List<AssociatedExpenseItem> associatedexpenseitems, DateTime createdon, int? createdby, DateTime? modifiedon, int? modifiedby, byte? frequency, FlagFrequencyType frequencyType, byte period, FlagPeriodType periodtype, decimal? limit, string description, bool active, int accountid, bool claimantjustificationrequired, bool displayflagimmediately, FlagColour flaglevel, bool approverjustificationrequired, string notesforauthoriser, FlagInclusionType itemroleinclusiontype, FlagInclusionType expenseiteminclusiontype)
            : base(flagid, flagtype, action, flagtext, associateditemroles, associatedexpenseitems, createdon, createdby, modifiedon, modifiedby, description, active, accountid, claimantjustificationrequired, displayflagimmediately, flaglevel, approverjustificationrequired, notesforauthoriser, string.Empty, string.Empty, itemroleinclusiontype, expenseiteminclusiontype, true, false, false, true, true)
        {
            this.Frequency = frequency;
            this.FrequencyType = frequencyType;
            this.Period = period;
            this.PeriodType = periodtype;
            this.Limit = limit;
            this.FlagTypeDescription = this.GetFlagTypeDescription();
            this.FlagDescription = this.GetFlagDescription();
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="FrequencyFlag"/> class. 
        /// The frequency flag.
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
        /// <param name="frequency">
        /// The frequency.
        /// </param>
        /// <param name="frequencyType">
        /// The frequency the flag will be checked on. Every/In the last
        /// </param>
        /// <param name="period">
        /// The period the flag will be checked against.
        /// </param>
        /// <param name="periodtype">
        /// The period type. Daily, weekly, monthly etc
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
        /// <param name="financialYear">
        /// The id of the financial year
        /// </param>
        /// <param name="financialYearStart">
        /// The financial Year Start.
        /// </param>
        /// <param name="finacialYearEnd">
        /// The finacial Year End.
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
        public FrequencyFlag(int flagid, FlagType flagtype, FlagAction action, string flagtext, List<int> associatedroles, List<AssociatedExpenseItem> associatedexpenseitems, DateTime createdon, int? createdby, DateTime? modifiedon, int? modifiedby, byte? frequency, FlagFrequencyType frequencyType, byte period, FlagPeriodType periodtype, decimal? limit, string description, bool active, int accountid, bool claimantjustificationrequired, bool displayflagimmediately, int financialYear, DateTime financialYearStart, DateTime finacialYearEnd, FlagColour flaglevel, bool approverjustificationrequired, string notesforauthoriser, FlagInclusionType itemroleinclusiontype, FlagInclusionType expenseiteminclusiontype)
            : base(flagid, flagtype, action, flagtext, associatedroles, associatedexpenseitems, createdon, createdby, modifiedon, modifiedby, description, active, accountid, claimantjustificationrequired, displayflagimmediately, flaglevel, approverjustificationrequired, notesforauthoriser, string.Empty, string.Empty, itemroleinclusiontype, expenseiteminclusiontype, true, false, false, true, true)
        {
            this.Frequency = frequency;
            this.FrequencyType = frequencyType;
            this.Period = period;
            this.PeriodType = periodtype;
            this.Limit = limit;
            this.FinancialYear = financialYear;
            this.FinancialYearStart = financialYearStart;
            this.FinancialYearEnd = finacialYearEnd;
            this.FlagTypeDescription = this.GetFlagTypeDescription();
            this.FlagDescription = this.GetFlagDescription();
        }

        #region properties
        /// <summary>
        /// Gets the frequency allowed before the item is flagged
        /// </summary>
        public byte? Frequency { get; private set; }

        /// <summary>
        /// Gets the frequency type of the flag
        /// </summary>
        public FlagFrequencyType FrequencyType { get; private set; }

        /// <summary>
        /// Gets the period allowed before the item is flagged. Used in conjunction with PeriodType. E.g. 2 years
        /// </summary>
        public byte Period { get; private set; }

        /// <summary>
        /// Gets the period type, used in conjunction with Period. E.g Weeks, Months, Years
        /// </summary>
        public FlagPeriodType PeriodType { get; private set; }

        /// <summary>
        /// Gets the monetary limit allowed for a SUM flag
        /// </summary>
        public decimal? Limit { get; private set; }

        /// <summary>
        /// Gets the financial year.
        /// </summary>
        public int? FinancialYear { get; private set; }

        /// <summary>
        /// Gets the financial year start.
        /// </summary>
        public DateTime? FinancialYearStart { get; private set; }

        /// <summary>
        /// Gets the financial year end.
        /// </summary>
        public DateTime? FinancialYearEnd { get; private set; }
        
        #endregion

        /// <summary>
        /// Validates the flag to see if it has been breached.
        /// </summary>
        /// <param name="item">
        /// The expense item the flag is being checked against.
        /// </param>
        /// <param name="employeeId">
        /// The id of the employee the expense item belongs.
        /// </param>
        /// <param name="properties">
        /// The global account properties.
        /// </param>
        /// <param name="connection">
        /// The connection.
        /// </param>
        /// <returns>
        /// The <see cref="FlaggedItem"/>.
        /// </returns>
        public override List<FlaggedItem> Validate(cExpenseItem item, int employeeId, cAccountProperties properties, IDBConnection connection = null)
        {
            if (this.HasFlagAlreadyBeenAssociatedWithExpense(item.expenseid))
            {
                return null;
            }

            return this.FlagType == FlagType.FrequencyOfItemCount
                       ? this.CheckFrequencyCountFlag(item, employeeId, connection)
                       : this.CheckFrequencySumFlag(item, employeeId, connection);
        }
        
        /// <summary>
        /// The get date range.
        /// </summary>
        /// <param name="itemDate">
        /// The item date.
        /// </param>
        /// <param name="startDate">
        /// The start date.
        /// </param>
        /// <param name="endDate">
        /// The end date.
        /// </param>
        internal void GetDateRange(DateTime itemDate, out DateTime startDate, out DateTime endDate)
        {
            startDate = DateTime.Today;
            endDate = DateTime.Today;
            switch (this.PeriodType)
            {
                case FlagPeriodType.Days:
                    startDate = itemDate.AddDays((this.Period - 1) / -1.0);
                    endDate = itemDate.AddDays(this.Period - 1);
                    break;
                case FlagPeriodType.CalendarWeeks:
                    startDate = itemDate.Date;
                    while (startDate.DayOfWeek != DayOfWeek.Sunday)
                    {
                        startDate = startDate.AddDays(-1);
                    }

                    int numDays = (this.Period - 1) * 7;
                    startDate = startDate.AddDays(numDays / -1.0);
                    endDate = startDate.AddDays((numDays * 2) + 6);
                    break;
                case FlagPeriodType.CalendarMonths:
                    startDate = new DateTime(itemDate.Year, itemDate.Month, 01);
                    endDate = startDate.AddMonths((this.Period - 1) + 1);
                    startDate = startDate.AddMonths((this.Period - 1) / -1);
                    endDate = endDate.AddDays(-1);
                    break;
                case FlagPeriodType.CalendarYears:
                    startDate = new DateTime(itemDate.Year, 01, 01);
                    endDate = startDate.AddYears((this.Period - 1) + 1);
                    startDate = startDate.AddYears((this.Period - 1) / -1);
                    endDate = endDate.AddDays(-1);
                    break;
                case FlagPeriodType.FinancialYears:
                    startDate = !this.FinancialYearStart.HasValue
                                    ? DateTime.Today
                                    : new DateTime(
                                          itemDate.Year,
                                          this.FinancialYearStart.Value.Month,
                                          this.FinancialYearStart.Value.Day);

                    if (startDate > itemDate)
                    {
                        // financial year started last year
                        startDate = startDate.AddYears(-1);
                    }

                    endDate = !this.FinancialYearEnd.HasValue ? DateTime.Today : new DateTime(startDate.Year + 1, this.FinancialYearEnd.Value.Month, this.FinancialYearEnd.Value.Day);
                    
                    startDate = startDate.AddYears((this.Period - 1) / -1);
                    endDate = endDate.AddYears(this.Period - 1);
                    break;
            }
        }

        /// <summary>
        /// The get initial flag date.
        /// </summary>
        /// <param name="date">
        /// The date of the expense.
        /// </param>
        /// <returns>
        /// The <see cref="DateTime"/>.
        /// </returns>
        internal DateTime GetInitialFlagDate(DateTime date)
        {
            switch (this.PeriodType)
            {
                case FlagPeriodType.Days:
                    date = date.AddDays(this.Period / -1.0);
                    break;
                case FlagPeriodType.Weeks:
                    date = date.AddDays((this.Period * 7) / -1.0);
                    break;
                case FlagPeriodType.Months:
                    date = date.AddMonths(this.Period / -1);
                    break;
                case FlagPeriodType.Years:
                    date = date.AddYears(this.Period / -1);
                    break;
            }

            return date;
        }

        /// <summary>
        /// The check frequency sum flag.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        /// <param name="employeeId">
        /// The employee id.
        /// </param>
        /// <param name="connection">
        /// The connection.
        /// </param>
        /// <returns>
        /// The <see cref="FlaggedItem"/>.
        /// </returns>
        private List<FlaggedItem> CheckFrequencySumFlag(cExpenseItem item, int employeeId, IDBConnection connection)
        {
            cFields clsfields = new cFields(AccountID);
            FlaggedItem flagResult = null;
            cTables clstables = new cTables(AccountID);

            DateTime startDate, endDate;

            if (this.FrequencyType == FlagFrequencyType.Every)
            {
                this.GetDateRange(item.date, out startDate, out endDate);
            }
            else
            {
                startDate = this.GetInitialFlagDate(item.date);
                endDate = item.date;
            }

            cQueryBuilder addedItemQuery = new cQueryBuilder(AccountID, cAccounts.getConnectionString(AccountID), ConfigurationManager.ConnectionStrings["metabase"].ConnectionString, clstables.GetTableByID(new Guid("D70D9E5F-37E2-4025-9492-3BCF6AA746A8")), new cTables(AccountID), new cFields(AccountID));
            addedItemQuery.addColumn(clsfields.GetFieldByID(new Guid("A528DE93-3037-46F6-974C-A76BD0C8642A")));
            addedItemQuery.addColumn(clsfields.GetFieldByID(new Guid("C3C64EB9-C0E1-4B53-8BE9-627128C55011")));
            addedItemQuery.addFilter(clsfields.GetFieldByID(new Guid("8F61ABE2-96DE-4D3F-9E91-FDF2D47800CB")), ConditionType.Equals, new object[] { item.subcatid }, null, ConditionJoiner.And, null);
            addedItemQuery.addFilter(clsfields.GetFieldByID(new Guid("A52B4423-C766-47BB-8BF3-489400946B4C")), ConditionType.Between, new object[] { startDate }, new object[] { endDate }, ConditionJoiner.And, null);
            addedItemQuery.addFilter(clsfields.GetFieldByID(new Guid("2501BE3D-AA94-437D-98BB-A28788A35DC4")), ConditionType.Equals, new object[] { employeeId }, new object[] { }, ConditionJoiner.And, null);
            if (item.expenseid > 0)
            {
                addedItemQuery.addFilter(clsfields.GetFieldByID(new Guid("A528DE93-3037-46F6-974C-A76BD0C8642A")), ConditionType.DoesNotEqual, new object[] { item.expenseid }, new object[] { }, ConditionJoiner.And, null);
            }

            decimal grandTotal = 0;

            List<int> associatedExpenses = new List<int>();
            using (SqlDataReader reader = addedItemQuery.getReader())
            {
                while (reader.Read())
                {
                    decimal? total = reader.GetDecimal(1);
                    grandTotal += total.Value;
                    associatedExpenses.Add(reader.GetInt32(0));
                }
            }

            grandTotal += item.total;
            if (grandTotal > this.Limit)
            {
                Flags flags = new Flags(this.AccountID);
                flagResult = new FlaggedItem(this.FlagDescription, this.CustomFlagText, this, FlagColour.Red, flags.GetAssociatedExpenses(associatedExpenses, connection), this.FlagTypeDescription, this.NotesForAuthoriser, this.AssociatedExpenseItems, this.Action, this.CustomFlagText, this.ClaimantJustificationRequired, false);
            }

            return new List<FlaggedItem> { flagResult };
        }

        /// <summary>
        /// The check frequency count flag.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        /// <param name="employeeId">
        /// The employee id.
        /// </param>
        /// <param name="connection">
        /// The connection.
        /// </param>
        /// <returns>
        /// The <see cref="FlaggedItem"/>.
        /// </returns>
        private List<FlaggedItem> CheckFrequencyCountFlag(cExpenseItem item, int employeeId, IDBConnection connection)
        {
            cFields clsfields = new cFields(AccountID);
            FlaggedItem flagResult = null;
            cTables clstables = new cTables(AccountID);
            DateTime startDate, endDate;

            if (this.FrequencyType == FlagFrequencyType.Every)
            {
                this.GetDateRange(item.date, out startDate, out endDate);
            }
            else
            {
                startDate = this.GetInitialFlagDate(item.date);
                endDate = item.date;
            }

            cQueryBuilder addedItemQuery = new cQueryBuilder(AccountID, cAccounts.getConnectionString(AccountID), ConfigurationManager.ConnectionStrings["metabase"].ConnectionString, clstables.GetTableByID(new Guid("D70D9E5F-37E2-4025-9492-3BCF6AA746A8")), new cTables(AccountID), new cFields(AccountID));
            addedItemQuery.addColumn(clsfields.GetFieldByID(new Guid("A528DE93-3037-46F6-974C-A76BD0C8642A")));
            addedItemQuery.addFilter(clsfields.GetFieldByID(new Guid("8F61ABE2-96DE-4D3F-9E91-FDF2D47800CB")), ConditionType.Equals, new object[] { item.subcatid }, null, ConditionJoiner.And, null);
            addedItemQuery.addFilter(clsfields.GetFieldByID(new Guid("A52B4423-C766-47BB-8BF3-489400946B4C")), ConditionType.Between, new object[] { startDate }, new object[] { endDate }, ConditionJoiner.And, null);
            addedItemQuery.addFilter(clsfields.GetFieldByID(new Guid("2501BE3D-AA94-437D-98BB-A28788A35DC4")), ConditionType.Equals, new object[] { employeeId }, new object[] { }, ConditionJoiner.And, null);
            if (item.expenseid > 0)
            {
                addedItemQuery.addFilter(clsfields.GetFieldByID(new Guid("A528DE93-3037-46F6-974C-A76BD0C8642A")), ConditionType.DoesNotEqual, new object[] { item.expenseid }, new object[] { }, ConditionJoiner.And, null);
            }

            List<int> associatedExpenses = new List<int>();
            using (SqlDataReader reader = addedItemQuery.getReader())
            {
                while (reader.Read())
                {
                    associatedExpenses.Add(reader.GetInt32(0));
                }
            }

            int count = associatedExpenses.Count + 1;
            if (count > this.Frequency)
            {
                Flags flags = new Flags(this.AccountID);
                flagResult = new FlaggedItem(this.FlagDescription, this.CustomFlagText, this, FlagLevel, flags.GetAssociatedExpenses(associatedExpenses, connection), this.FlagTypeDescription, this.NotesForAuthoriser, this.AssociatedExpenseItems, this.Action, this.CustomFlagText, this.ClaimantJustificationRequired, false);
            }

            return new List<FlaggedItem> { flagResult };
        }

        /// <summary>
        /// Gets the flag type description.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string GetFlagTypeDescription()
        {
            if (this.FlagType == FlagType.FrequencyOfItemCount)
            {
                return "Frequency of item (count)";
            }

            if (this.FlagType == FlagType.FrequencyOfItemSum)
            {
                return "Frequency of item (sum)";
            }

            return string.Empty;
        }

        /// <summary>
        /// Gets the flag description.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string GetFlagDescription()
        {
            StringBuilder output = new StringBuilder();
            if (this.FlagType == FlagType.FrequencyOfItemCount)
            {
                output.Append("This item can only be claimed " + this.Frequency + " time(s) ");
            }
            else
            {
                output.Append("There is a limit for this item which is {CurrencySymbol}" + this.Limit);
            }

            output.Append(this.FrequencyType == FlagFrequencyType.Every ? " every " : " in the last ");

            output.Append(this.Period + " ");
            switch (this.PeriodType)
            {
                case FlagPeriodType.CalendarMonths:
                    output.Append("calendar month(s)");
                    break;
                case FlagPeriodType.CalendarWeeks:
                    output.Append("calendar week(s)");
                    break;
                case FlagPeriodType.CalendarYears:
                    output.Append("calendar year(s)");
                    break;
                case FlagPeriodType.Days:
                    output.Append("day(s)");
                    break;
                case FlagPeriodType.FinancialYears:
                    output.Append("financial year(s)");
                    break;
                case FlagPeriodType.Months:
                    output.Append("month(s)");
                    break;
                case FlagPeriodType.Weeks:
                    output.Append("week(s)");
                    break;
                case FlagPeriodType.Years:
                    output.Append("year(s)");
                    break;
            }

            return output.ToString();
        }
    }
}
