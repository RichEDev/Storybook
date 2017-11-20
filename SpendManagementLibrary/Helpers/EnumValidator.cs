
namespace SpendManagementLibrary.Helpers
{
    using System;

    static class EnumValidator
    {
        /// <summary>
        /// Detmines if the value belongs to the enum
        /// </summary>
        /// <param name="enumType">The enum type</param>
        /// <param name="value">The comparative value</param>
        /// <returns>The outcome</returns>
        internal static bool EnumValueIsDefined(Type enumType, object value)
        {
           return Enum.IsDefined(enumType, value);
        }
    }
}
