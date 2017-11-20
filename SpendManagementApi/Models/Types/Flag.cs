namespace SpendManagementApi.Models.Types
{
    using System;
    using Interfaces;
    using SpendManagementLibrary.Flags;

    /// <summary>
    /// The flag request.
    /// </summary>
    public class Flag : BaseExternalType
    {
        /// <summary>
        /// Gets or sets the flag id.
        /// </summary>
        public int FlagId { get; set; }

        /// <summary>
        /// Gets or sets the flag type.
        /// </summary>
        public FlagType FlagType { get; set; }

        /// <summary>
        /// Gets or sets the flag action.
        /// </summary>
        public FlagAction FlagAction { get; set;}

        /// <summary>
        /// Gets or sets the custom flag text.
        /// </summary>
        public string CustomFlagText { get; set;}

        /// <summary>
        /// Gets or sets the invalid date flag type.
        /// </summary>
        public InvalidDateFlagType InvalidDateFlagType { get; set; }

        /// <summary>
        /// Gets or sets the date.
        /// </summary>
        public DateTime? Date { get; set; }

        /// <summary>
        /// Convert from a data access layer Type to an api Type.
        /// </summary>
        /// <param name="dbType">The instance of the data access layer Type to convert from.</param>
        /// <param name="actionContext">The actionContext which contains DAL classes.</param>
        /// <returns>An api Type</returns>
        internal Flag From(SpendManagementLibrary.Flags.Flag dbType, IActionContext actionContext)
        {
            if (dbType == null)
            {
                return null;
            }

            this.FlagId = dbType.FlagID;
            this.FlagType = dbType.FlagType;
            this.Active = dbType.Active;
            this.FlagAction = dbType.Action;
            this.FlagLevel = dbType.FlagLevel;
            this.CustomFlagText = dbType.CustomFlagText;
            this.Description = dbType.Description;
            this.ClaimantJustificationRequired = dbType.ClaimantJustificationRequired;
            this.DisplayImmediately = dbType.DisplayFlagImmediately;
            this.FlagDescritpion = dbType.FlagDescription;
            this.FlagTypeDescription = dbType.FlagTypeDescription;

            return this;
        }

        /// <summary>
        /// Gets or sets the months.
        /// </summary>
        public byte? Months { get; set; }

        /// <summary>
        /// Gets or sets the amber tolerance.
        /// </summary>
        public decimal? AmberTolerance { get; set; }

        /// <summary>
        /// Gets or sets the frequency.
        /// </summary>
        public byte? Frequency { get; set; }

        /// <summary>
        /// Gets or sets the flag frequency type.
        /// </summary>
        public FlagFrequencyType FlagFrequencyType { get; set; }

        /// <summary>
        /// Gets or sets the period.
        /// </summary>
        public byte? Period { get; set; }

        /// <summary>
        /// Gets or sets the flag period type.
        /// </summary>
        public FlagPeriodType FlagPeriodType { get; set; }

        /// <summary>
        /// Gets or sets the limit.
        /// </summary>
        public decimal? Limit { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether active.
        /// </summary>
        public bool Active { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether claimant justification required.
        /// </summary>
        public bool ClaimantJustificationRequired { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether display immediately.
        /// </summary>
        public bool DisplayImmediately { get; set; }

        /// <summary>
        /// Gets or sets the flag tolerance percentage.
        /// </summary>
        public decimal? FlagTolerancePercentage { get; set; }

        /// <summary>
        /// Gets or sets the financial year.
        /// </summary>
        public int? FinancialYear { get; set; }

        /// <summary>
        /// Gets or sets the tip limit.
        /// </summary>
        public decimal? TipLimit { get; set; }

        /// <summary>
        /// Gets or sets the flag level.
        /// </summary>
        public FlagColour FlagLevel { get; set;}

        /// <summary>
        /// Gets or sets a value indicating whether approver justification required.
        /// </summary>
        public  bool ApproverJustificationRequired { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether increase limit by num others.
        /// </summary>
        public bool IncreaseLimitByNumOthers { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether display limit.
        /// </summary>
        public bool DisplayLimit { get; set; }

        /// <summary>
        /// Gets or sets the notesforauthoriser.
        /// </summary>
        public string NotesForAuthoriser { get; set; }

        /// <summary>
        /// Gets or sets the item role inclusion type.
        /// </summary>
        public  FlagInclusionType ItemRoleInclusionType { get; set; }

        /// <summary>
        /// Gets or sets the expense item inclusion type.
        /// </summary>
        public FlagInclusionType ExpenseItemInclusionType { get; set; }

        /// <summary>
        /// Gets or sets the passenger limit.
        /// </summary>
        public int? PassengerLimit { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether validate selected expense item.
        /// </summary>
        public bool ValidateSelectedExpenseItem { get; set; }

        /// <summary>
        /// Gets or sets the flag description.
        /// </summary>
        public string FlagDescritpion { get; set; }

        /// <summary>
        /// Gets or sets the flag item description.
        /// </summary>
        public string FlagTypeDescription { get; private set; }

        /// <summary>
        /// Gets or sets the Daily Mileage Limit (used for <see cref="RestrictDailyMileageFlag"/> Flag types
        /// </summary>
        public decimal? DailyMileageLimit { get; set; }
    }
}