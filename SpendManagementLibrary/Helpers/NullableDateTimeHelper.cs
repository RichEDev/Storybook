using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpendManagementLibrary.Helpers
{
    /// <summary>
    /// A helper class for nullable Date Time types
    /// </summary>
    public static class NullableDateTimeHelper
    {
        /// <summary>
        /// Parse a string into a Nullable date time type (or null if fails).
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static DateTime? Parse(string text)
        {
            DateTime date;
            if (DateTime.TryParse(text, out date))
            {
                return date;
            }
            else
            {
                return null;
            }
        }
    }
}
