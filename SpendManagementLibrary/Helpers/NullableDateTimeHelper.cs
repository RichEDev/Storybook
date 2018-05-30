namespace SpendManagementLibrary.Helpers
{
    using System;

    /// <summary>
    /// A helper class for nullable Date Time types
    /// </summary>
    public static class NullableDateTimeHelper
    {
        /// <summary>
        /// Parse a string into a Nullable date time type (or null if fails).
        /// </summary>
        /// <param name="text">A text representation of a date /time</param>
        /// <returns>A nullable<see cref="DateTime"/> based on the given date time string.</returns>
        public static DateTime? Parse(string text)
        {
            DateTime date;
            if (string.IsNullOrEmpty(text))
            {
                return null;
            }

            var textSplit = text.Split(' ');

            if (textSplit.Length > 0 && DateTime.TryParse(textSplit[0], out date))
            {
                if (textSplit.Length > 1 && textSplit[1] == "23:00:00Z")
                {
                    date = date.AddDays(1);
                }

                return date;
            }
            else
            {
                return null;
            }
        }
    }
}
