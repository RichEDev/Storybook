namespace SpendManagementApi.Common.Enum
{
    /// <summary>
    /// The flag type.
    /// </summary>
    public enum FlagType
    {
        /// <summary>
        /// Duplicate flag.  
        /// </summary>        
        Duplicate = 1,

        /// <summary>
        /// Limit without a receipt.
        /// </summary>
        LimitWithoutReceipt = 2,

        /// <summary>
        /// Limit with a receipt.
        /// </summary>        
        LimitWithReceipt = 3,

        /// <summary>
        /// Item on a weekend.
        /// </summary>         
        ItemOnAWeekend = 4,

        /// <summary>
        /// Invalid date (fixed date or number of months).
        /// </summary>     
        InvalidDate = 5,

        /// <summary>
        /// Frequency of item (count).
        /// </summary>       
        FrequencyOfItemCount = 6,

        /// <summary>
        /// Frequency of item (sum).
        /// </summary>       
        FrequencyOfItemSum = 7,

        /// <summary>
        /// Group limit without a receipt.
        /// </summary>      
        GroupLimitWithoutReceipt = 8,

        /// <summary>
        /// Group limit with a receipt.
        /// </summary>        
        GroupLimitWithReceipt = 9,

        /// <summary>
        /// Custom flag.
        /// </summary>           
        Custom = 10,

        /// <summary>
        /// Aggregate flag.
        /// </summary>        
        Aggregate = 11,

        /// <summary>
        /// Item is not reimbursable.
        /// </summary>         
        ItemNotReimbursable = 12,

        /// <summary>
        /// Unused allowance available.
        /// </summary>      
        UnusedAllowanceAvailable = 13,

        /// <summary>
        /// Tip limit exceeded.
        /// </summary>         
        TipLimitExceeded = 14,

        /// <summary>
        /// Home to location greater.
        /// </summary>         
        HomeToLocationGreater = 15,

        /// <summary>
        /// Recommended mileage exceeded.
        /// </summary>         
        MileageExceeded = 16,

        /// <summary>
        /// Item is reimbursable.
        /// </summary>   
        ItemReimbursable = 17,

        /// <summary>
        /// Receipt not attached
        /// </summary>        
        ReceiptNotAttached = 18,

        /// <summary>
        /// Number of passengers limit
        /// </summary>        
        NumberOfPassengersLimit = 19,

        /// <summary>
        /// One item in a group
        /// </summary>       
        OneItemInAGroup = 20,

        /// <summary>
        /// Journey item does not start or end  with home or office address
        /// </summary>
        JourneyDoesNotStartAndFinishAtHomeOrOffice=21,

        /// <summary>
        /// Restrict the number of miles on a single day.
        /// </summary>
        RestrictDailyMileage = 22
    }
}
