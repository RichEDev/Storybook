namespace SpendManagementLibrary.Helpers
{
    using System;
    using System.Data.SqlTypes;

    static class DateValidators
    {
        /// <summary>
        /// Checks that the date is not greater than today or less than the minimum SQLDateTime (01/01/1753)
        /// </summary>
        /// <param name="dateTime">The date time to check</param>
        /// <returns>The outcome of the check</returns>
        internal static bool IsDateNoGreaterThanTodayOrLess01011753(DateTime dateTime)
        {
            return dateTime <= DateTime.Now && dateTime > SqlDateTime.MinValue.Value;
        }

        /// <summary>
        /// Checks that the date is not greater than today or less than the minimum SQLDateTime (01/01/1753)
        /// </summary>
        /// <param name="dateTime">The date time to check</param>
        /// <returns>The outcome of the check</returns>
        internal static bool IsDateNoGreaterThanTodayOrLess01011753(DateTime? dateTime)
        {
            if (!dateTime.HasValue)
            {
                return true;
            }

            return dateTime.Value <= DateTime.Today && dateTime.Value > SqlDateTime.MinValue.Value;
        }
    }
}
