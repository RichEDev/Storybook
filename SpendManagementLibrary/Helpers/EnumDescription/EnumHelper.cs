namespace SpendManagementLibrary.Helpers.EnumDescription
{
    using System;
    using System.ComponentModel;
    using System.Globalization;
    using System.Reflection;

    /// <summary>
    /// The enum helper.
    /// </summary>
    public static class EnumHelper
    {
        /// <summary>
        /// Gets the description attrtube from an enum 
        /// </summary>
        /// <param name="enumerator">
        /// The enumator to check.
        /// </param>
        /// <typeparam name="T">The generic type of the enum.</typeparam>
        /// <returns>
        /// A <see cref="string"/> string .
        /// </returns>
        public static string GetDescription<T>(this T enumerator) where T : IConvertible
        {
            string description = string.Empty;

            if (enumerator is Enum)
            {
                Type type = enumerator.GetType();
                Array values = Enum.GetValues(type);

                foreach (int val in values)
                {
                    if (val == enumerator.ToInt32(CultureInfo.InvariantCulture))
                    {
                        MemberInfo[] memInfo = type.GetMember(type.GetEnumName(val));
                        object[] descriptionAttributes = memInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);

                        if (descriptionAttributes.Length > 0)
                        {                         
                            description = ((DescriptionAttribute)descriptionAttributes[0]).Description;
                        }

                        break;
                    }
                }
            }

            return description;
        }
    }
}
