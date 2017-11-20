namespace SpendManagementLibrary.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Web;

    /// <summary>
	/// Useful functions for string manipulation
	/// </summary>
	public static class StringManipulators
	{
		/// <summary>
		/// Used for putting a hyphen (or other string) into any words that are longer than a specified number of characters within a whitespace-separated string of words
		/// </summary>
		/// <param name="words">Sentence or other string that will be split using the separator</param>
		/// <param name="maxWordLength">The length at which a word becomes too long and needs hyphenating</param>
		/// <param name="hyphenator">Optional string to insert as a "hyphen" within the word, default is "-"</param>
		/// <param name="wordSeparator">Optional string to use to separate the string into words, default is " "</param>
		/// <returns>StringBuilder with the new form of the word</returns>
		public static StringBuilder HyphenateWordsLongerThanMaxWordLengthInString(string words, int maxWordLength, string hyphenator = "-", string wordSeparator = " ")
		{
			StringBuilder returnString = new StringBuilder();
			string[] separators = new string[] {wordSeparator};
			
			foreach (string word in words.Split(separators, StringSplitOptions.None))
			{
				returnString.Append(AddHyphensToWordLongerThan(word, maxWordLength, hyphenator)).Append(" ");
			}

			returnString = returnString.Remove(returnString.Length - 1, 1);

			return returnString;
		}


		/// <summary>
		/// Used for putting a hyphen (or other string) into a word if it is longer than a specified number of characters within a whitespace-separated string of words
		/// </summary>
		/// <param name="word">Word to check for length</param>
		/// <param name="maxWordLength">The maximum length before hyphenating the word</param>
		/// <param name="hyphenator">Optional string to insert as a "hyphen" within the word, default is "-"</param>
		/// <returns>StringBuilder with the new form of the word</returns>
		public static StringBuilder AddHyphensToWordLongerThan(string word, int maxWordLength, string hyphenator = "-")
		{
			return word.Length > maxWordLength
					? new StringBuilder(word.Substring(0, (maxWordLength - 1)))
						.Append(hyphenator)
						.Append(AddHyphensToWordLongerThan(word.Substring(maxWordLength - 1), maxWordLength, hyphenator))
					: new StringBuilder(word);
		}
	
        /// <summary>
        /// Used for making a string safe for output in a HTML document, encodes any special characters which would malform the markup and replaces new line characters with HTML linebreaks
        /// </summary>
        /// <param name="input">The string to be made safe</param>
        /// <returns>The new string</returns>
        public static string HtmlSafe(string input)
        {
            return HttpUtility.HtmlEncode(input).Replace(Environment.NewLine, "<br />");
        }

        /// <summary>
        /// Adds a space between every word in a camel-cased string
        /// E.g. "CamelCasedString" is returned as "Camel Cased String"
        /// </summary>
        /// <param name="camelString">The string to be converted</param>
        /// <returns>The new string</returns>
        public static string SplitCamel(this string camelString)
        {
            return Regex.Replace(camelString, "([A-Z])", " $1", RegexOptions.Compiled).Trim();
        }

        /// <summary>
        /// Trim the end of the string by x characters..
        /// </summary>
        /// <param name="trimString">
        /// The string to trim.
        /// </param>
        /// <param name="number">
        /// The number of characters to trim from the end.
        /// </param>
        /// <returns>
        /// The <see cref="string"/> the trimmed string.
        /// </returns>
        public static string TrimEnd(this string trimString, int number)
        {
            if (string.IsNullOrEmpty(trimString))
            {
                return string.Empty;
            }

            return number > trimString.Length ? string.Empty : trimString.Substring(0, trimString.Length - number);
        }

        /// <summary>
        /// Determines whether the beginning of this string instance matches the specified string.
        /// </summary>
        /// <param name="startsString">The string to test</param>
        /// <param name="strings">A comma seperated list of strings to test</param>
        /// <returns></returns>
	    public static bool StartsWith(this string startsString, params string[] strings)
	    {
	        return strings.Any(startsString.StartsWith);
	    }

        /// <summary>
        /// Splits a string of numbers by "," and converts and adds to a List of ints. Useful for when splinting out a string of expense ids.
        /// </summary>
        /// <param name="input">The string to be manipulated</param>
        /// <returns>The new list of ints</returns>
        public static List<int> SplitStringOfNumbersToIntList(this string input)
        {
            var splitString = input.Split(',');
            var ints = new List<int>();

            for (var i = 0; i < splitString.GetLength(0); i++)
            {
                ints.Add(Convert.ToInt32(splitString[i]));
            }

            return ints;
        }

        /// <summary>
        /// Replace ampersand in string where the ampersand is within an HREF link.
        /// </summary>
        /// <param name="htmlString">
        /// The HTML string to clean.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static void ReplaceAmpersandInHtmlString(ref string htmlString)
        {
            var matches = Regex.Matches(htmlString, @"(?:http://|www\.|https://)(\S+)");
            foreach (Match match in matches)
            {
                if (match.Value.Contains("&"))
                {
                    var newString = match.Value.Replace("&", "&amp;");
                    htmlString = htmlString.Replace(match.Value, newString);
                }
            }
        }

        /// <summary>
        /// Trim the end of a string by a given string (case insensitive)
        /// </summary>
        /// <param name="source">The string to trim</param>
        /// <param name="replaceEnd">The string to trim from the end of the source.</param>
        /// <returns>The trimmed string of the original.</returns>
        public static string TrimEnd(this string source, string replaceEnd)
	    {
            if (source.ToLower().EndsWith(replaceEnd.ToLower()))
            {
                return source.Substring(0, source.Length - replaceEnd.Length);
            }

            return source;
	    }
    }
}
