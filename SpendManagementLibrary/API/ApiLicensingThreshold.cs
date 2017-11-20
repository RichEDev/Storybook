using System;

namespace SpendManagementLibrary.API
{
    /// <summary>
    /// Records how many calls an Account has left.
    /// </summary>
    public class ApiLicenseStatus
    {
        /// <summary>
        /// The Id of the Account that these calls are tied to.
        /// </summary>
        public int AccountId { get; set; }
        
        /// <summary>
        /// The total licensed (paid) calls available for this account.
        /// These get decremented after the number of free calls for the day reaches zero.
        /// </summary>
        public int TotalLicensedCalls { get; set; }

        /// <summary>
        /// The amount of calls left that can be made within this threshold's time period.
        /// </summary>
        public int FreeCallsToday { get; set; }

        /// <summary>
        /// The hourly call limit for this account.
        /// </summary>
        public int HourLimit { get; internal set; }

        /// <summary>
        /// The amount of calls left in this hourly period.
        /// </summary>
        public int HourRemainingCalls { get; set; }

        /// <summary>
        /// The last point in time at which the amount of Remaining calls (per hour) was reset.
        /// </summary>
        public DateTime HourLastResetDate { get; set; }

        /// <summary>
        /// The by-the-minute call limit for this account.
        /// </summary>
        public int MinuteLimit { get; internal set; }

        /// <summary>
        /// The amount of calls left in this minute period.
        /// </summary>
        public int MinuteRemainingCalls { get; set; }

        /// <summary>
        /// The last point in time at which the amount of Remaining calls (per minute) was reset.
        /// </summary>
        public DateTime MinuteLastResetDate { get; set; }

    }
}
