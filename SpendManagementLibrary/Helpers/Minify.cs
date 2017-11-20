using System;
using System.Text;

namespace SpendManagementLibrary.Helpers
{
    /// <summary>
    /// This class contains useful helpers for minifying strings within stringbuilders, etc.
    /// </summary>
    public static class Minify
    {
        /// <summary>
        /// An extended version of StringBuilder.ToString() that performs some basic removal of tabs and newlines
        /// - there are more advanced operations we could perform 
        /// - but for now keeping it simple as they could impact strings within the string in the case of javascript
        /// - don't put "to-end-of-line" comments in ie. "//" or you'll break anything that should come after
        /// </summary>
        /// <param name="sb">This is automatically the StringBuilder you are using MinifyToString on</param>
        /// <returns>string with double-spaces, newlines and tabs removed</returns>
        public static string MinifyToString(this StringBuilder sb)
        {
            return sb
                .Replace(Environment.NewLine, string.Empty)
                .Replace("\\t", string.Empty)
                .Replace("  ", string.Empty)
                //.Replace(" {", "{")
                //.Replace(" :", ":")
                //.Replace(": ", ":")
                //.Replace(", ", ",")
                //.Replace("; ", ";")
                //.Replace(";}", "}")
                .ToString();
        }
    }
}
