namespace SpendManagementApi.Utilities
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using SpendManagementApi.Models.Types;

    /// <summary>
    /// Extension methods for use within the API project.
    /// </summary>
    public static class ExtensionMethods
    {
        /// <summary>
        /// Removes the version number from an API controller string e.g. AccountsV23 becomes Accounts
        /// </summary>
        /// <param name="original">The string to remove the version number from</param>
        /// <returns></returns>
        public static string RemoveApiVersionNumber(this string original)
        {
            int versionNumberIndex = original.LastIndexOf("V", StringComparison.Ordinal);

            return versionNumberIndex > 0 ? original.Substring(0, versionNumberIndex) : original;
        }

        /// <summary>
        /// Converts a sorted list of int by object  into a list of <see cref="UserDefinedFieldValue">UserDefinedFieldValue</see>
        /// </summary>
        /// <returns></returns>
        public static List<UserDefinedFieldValue> ToUserDefinedFieldValueList(this SortedList<int, object> sortedList)
        {
            return sortedList.Select(kvp => new UserDefinedFieldValue(kvp.Key, kvp.Value)).ToList();
        }

        /// <summary>
        /// Converts a sorted list of int by object  into a list of <see cref="UserDefinedFieldValue">UserDefinedFieldValue</see>
        /// </summary>
        /// <returns></returns>
        public static SortedList<int, object> ToSortedList(this List<UserDefinedFieldValue> valueList)
        {
            var returnList = new SortedList<int, object>();
            if (valueList == null)
            {
                return returnList;
            }

            foreach (var userDefinedFieldValue in valueList)
            {
                returnList.Add(
                    userDefinedFieldValue.Id,
                    userDefinedFieldValue.Value != null && userDefinedFieldValue.Value.GetType() == typeof(UserDefinedFieldValue)
                        ? ((UserDefinedFieldValue)(userDefinedFieldValue.Value)).Value
                        : userDefinedFieldValue.Value);
            }

            return returnList;
        }
    }
}
