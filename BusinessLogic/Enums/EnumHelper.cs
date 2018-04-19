namespace BusinessLogic.Enums
{
    using System;
    using System.ComponentModel;
    using System.Globalization;
    using System.Reflection;

    /// <summary>
    /// Defines an <see cref="EnumHelper"/> and all it's members
    /// Class to help get properties of an enum or the enum it's self
    /// </summary>
    public static class EnumHelper
    {
        /// <summary>
        /// Gets the description attribute from an enum 
        /// </summary>
        /// <param name="enumerator">
        /// The enumerator to check.
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

        /// <summary>
        /// Get the Enum via the description
        /// </summary>
        /// <typeparam name="T">The enum type to get</typeparam>
        /// <param name="description">The description to ge on</param>
        /// <returns>The enum</returns>
        public static T GetEnumValueFromDescription<T>(string description) where T : IConvertible
        {
            MemberInfo[] fis = typeof(T).GetFields();

            foreach (var fi in fis)
            {
                DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

                if (attributes != null && attributes.Length > 0 && attributes[0].Description == description)
                    return (T)Enum.Parse(typeof(T), fi.Name);
            }

            // If this is hit then not finding the enum
            return default(T);
        }
    }
}
