using System;

namespace EsrGo2FromNhsWcfLibrary.ESR
{
    using System.Globalization;

    static class EsrDateHelpers
    {
        /// <summary>
        /// Parse an input string into a date using the DateTime TryParse Exact
        /// </summary>
        /// <param name="input">The string to cast</param>
        /// <param name="mask">The input mask to use</param>
        /// <param name="date">The <see cref="DateTime"/>if successful</param>
        /// <returns>True if the input string could be parsed as a date</returns>
        public static bool TryParseExact(string input, string mask, out DateTime date)
        {
            if (string.IsNullOrEmpty(input))
            {
                date = DateTime.MinValue;
                return false;
            }

            input = input.Replace("\r", string.Empty);
            var parseDate = input.Length > mask.Length ? input.Substring(0, mask.Length) : input;
            return DateTime.TryParseExact(
                parseDate,
                mask,
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out date);
        }
    }
}
