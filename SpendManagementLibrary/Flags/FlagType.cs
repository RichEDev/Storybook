namespace SpendManagementLibrary.Flags
{
    using System.ComponentModel;

    /// <summary>
    /// The flag type.
    /// </summary>
    public enum FlagType
    {
        /// <summary>
        /// Duplicate flag.
        /// </summary>
        [FlagInclusionTypeAttribute(FlagInclusionType.All)]
        [Description("Duplicate expense")]    
        Duplicate = 1,

        /// <summary>
        /// Limit without a receipt.
        /// </summary>
        [FlagInclusionTypeAttribute(FlagInclusionType.All)]
        [Description("Limit without a receipt")]
        LimitWithoutReceipt = 2,

        /// <summary>
        /// Limit with a receipt.
        /// </summary>
        [FlagInclusionTypeAttribute(FlagInclusionType.All)]
        [Description("Limit with a receipt")]
        LimitWithReceipt = 3,

        /// <summary>
        /// Item on a weekend.
        /// </summary>
        [FlagInclusionTypeAttribute(FlagInclusionType.All)]
        [Description("Item on a weekend")]
        ItemOnAWeekend = 4,

        /// <summary>
        /// Invalid date (fixed date or number of months).
        /// </summary>
        [FlagInclusionTypeAttribute(FlagInclusionType.All)]
        [Description("Invalid date")]
        InvalidDate = 5,

        /// <summary>
        /// Frequency of item (count).
        /// </summary>
        [FlagInclusionTypeAttribute(FlagInclusionType.All)]
        [Description("Frequency of item (count)")]
        FrequencyOfItemCount = 6,

        /// <summary>
        /// Frequency of item (sum).
        /// </summary>
        [FlagInclusionTypeAttribute(FlagInclusionType.All)]
        [Description("Frequency of item (sum)")]
        FrequencyOfItemSum = 7,

        /// <summary>
        /// Group limit without a receipt.
        /// </summary>
        [FlagInclusionTypeAttribute(FlagInclusionType.List)]
        [Description("Group limit without a receipt")]
        GroupLimitWithoutReceipt = 8,

        /// <summary>
        /// Group limit with a receipt.
        /// </summary>
        [FlagInclusionTypeAttribute(FlagInclusionType.List)]
        [Description("Group limit with a receipt")]
        GroupLimitWithReceipt = 9,

        /// <summary>
        /// Custom flag.
        /// </summary>
        [FlagInclusionTypeAttribute(FlagInclusionType.All)]
        [Description("Custom")]
        Custom = 10,

        /// <summary>
        /// Aggregate flag.
        /// </summary>
        [FlagInclusionTypeAttribute(FlagInclusionType.All)]
        [Description("Aggregate")]
        Aggregate = 11,

        /// <summary>
        /// Item is not reimbursable.
        /// </summary>
        [FlagInclusionTypeAttribute(FlagInclusionType.All)]
        [Description("Item not reimbursable")]
        ItemNotReimbursable = 12,

        /// <summary>
        /// Unused allowance available.
        /// </summary>
        [FlagInclusionTypeAttribute(FlagInclusionType.All)]
        [Description("Unused advance available")]
        UnusedAllowanceAvailable = 13,

        /// <summary>
        /// Tip limit exceeded.
        /// </summary>
        [FlagInclusionTypeAttribute(FlagInclusionType.All)]
        [Description("Tip limit exceeded")]
        TipLimitExceeded = 14,

        /// <summary>
        /// Home to location greater.
        /// </summary>
        [FlagInclusionTypeAttribute(FlagInclusionType.All)]
        [Description("Home to location greater")]
        HomeToLocationGreater = 15,

        /// <summary>
        /// Recommended mileage exceeded.
        /// </summary>
        [FlagInclusionTypeAttribute(FlagInclusionType.All)]
        [Description("Recommended distance exceeded")]
        MileageExceeded = 16,

        /// <summary>
        /// Item is reimbursable.
        /// </summary>
        [FlagInclusionTypeAttribute(FlagInclusionType.All)]
        [Description("Item reimbursable")]
        ItemReimbursable = 17,

        /// <summary>
        /// Receipt not attached
        /// </summary>
        [FlagInclusionTypeAttribute(FlagInclusionType.All)]
        [Description("Receipt not attached")]
        ReceiptNotAttached = 18,

        /// <summary>
        /// Number of passengers limit
        /// </summary>
        [FlagInclusionTypeAttribute(FlagInclusionType.All)]
        [Description("Passenger limit")]
        NumberOfPassengersLimit = 19,

        /// <summary>
        /// One item in a group
        /// </summary>
        [FlagInclusionTypeAttribute(FlagInclusionType.List)]
        [Description("One item in a group")]
        OneItemInAGroup = 20,

        /// <summary>
        /// Journey does not start and finish at home or office.
        /// </summary>
        [FlagInclusionTypeAttribute(FlagInclusionType.All)]
        [Description("Journey does not start and finish at home or office")]
        JourneyDoesNotStartAndFinishAtHomeOrOffice = 21,

        /// <summary>
        /// Restrict the number of miles on a single day.
        /// </summary>
        [FlagInclusionTypeAttribute(FlagInclusionType.All)]
        [Description("Restrict number of miles per day")]
        RestrictDailyMileage = 22
    }
}
