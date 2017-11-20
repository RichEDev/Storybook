namespace Utilities.StringManipulation
{
    using System.Text;

    /// <summary>
    /// Spacing of strings class.
    /// </summary>
    public static class Spacing
    {
        /// <summary>
        /// The add space before capitals in input string
        /// e.g. 'ExampleString' would return 'Example String'.
        /// </summary>
        /// <param name="inputString">
        /// The input string.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string AddSpaceBeforeCapitals(string inputString)
        {
            var result = new StringBuilder();
            for (int i = 0; i < inputString.Length; i++)
            {
                var currentChar = inputString.Substring(i, 1);
                if (currentChar == currentChar.ToUpper() && i > 0)
                {
                    result.Append(" ");
                }

                result.Append(currentChar);
            }

            return result.ToString();
        }
    }
}
