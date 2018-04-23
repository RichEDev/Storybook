namespace SpendManagementLibrary.Extentions
{
    /// <summary>
    /// The string redactor class.
    /// </summary>
    public static class StringRedactor
    {
        /// <summary>
        /// Redacts a string.
        /// </summary>
        /// <param name="stringToRedact">
        /// The string to redact.
        /// </param>
        /// <remarks>The string needs to be more than 4 characters</remarks>
        public static string Redact(this string stringToRedact)
        {
            var redactedString = string.Empty;

            if (stringToRedact.StartsWith("XXXX") || stringToRedact.Length <= 4)
            {
                return stringToRedact;
            }

            for (var i = 0; i < stringToRedact.Length - 4; i++)
            {
                redactedString += "X";
            }

            redactedString += stringToRedact.Substring(stringToRedact.Length - 4);

            return redactedString;
        }
    }
}
