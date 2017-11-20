using SpendManagementLibrary;
using SpendManagementLibrary.Helpers;

namespace Spend_Management.shared.code.Helpers
{
    /// <summary>
    /// Provide formatting helper methods.
    /// </summary>
    public static class Formatting
    {
        /// <summary>
        /// The format date value part of a filter object to remove either the time or the seconds portion.
        /// </summary>
        /// <param name="filter">
        /// The filter.
        /// </param>
        /// <param name="obj">
        /// The string object to format.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        internal static object FormatDateValue(cReportCriterion filter, object obj)
        {
            switch (filter.field.FieldType)
            {
                case "D":
                    obj = obj.ToString().Replace("00:00:00", string.Empty);
                    break;
                case "DT":
                    obj = obj.ToString().TrimEnd(":00");
                    break;
            }
            return obj;
        }

    }
}