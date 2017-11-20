namespace Spend_Management.expenses.code
{
    /// <summary>
    /// Helper class for the string format operation
    /// </summary>
    public static class StringFormatHelper
    {
        /// <summary>
        /// Method to Replace the white-space in the string and convert the string to lowercase
        /// </summary>
        /// <param name="inputString">string to format by converting to lowercase and remove the empty space</param>
        /// <returns>Formatted string</returns>
        public static string RemoveWhiteSpaceAndChangeToLowerCase(this string inputString)
        {
            return inputString.Replace(" ", string.Empty).ToLower();
        }
    }
}