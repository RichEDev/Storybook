namespace SpendManagementLibrary.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Reflection;

    public static class EnumHelpers<TEnum> where TEnum : struct, IConvertible
    {
        public static IList<TEnum> GetValues(Enum value)
        {
            return value.GetType().GetFields(BindingFlags.Static | BindingFlags.Public).Select(fi => (TEnum) Enum.Parse(value.GetType(), fi.Name, false)).ToList();
        }

        public static TEnum Parse(string value)
        {
            return (TEnum)Enum.Parse(typeof(TEnum), value, true);
        }

        public static IList<string> GetNames(Enum value)
        {
            return value.GetType().GetFields(BindingFlags.Static | BindingFlags.Public).Select(fi => fi.Name).ToList();
        }

        public static IList<string> GetDisplayValues(Enum value)
        {
            return GetNames(value).Select(obj => GetDisplayValue(Parse(obj))).ToList();
        }

        /// <summary>
        /// Returns the [Display(Name="")] annotation on an object.
        /// </summary>
        /// <param name="value">The enum value.</param>
        /// <returns>The strig for the name property.</returns>
        public static string GetDisplayValue(TEnum value)
        {
            var fieldInfo = value.GetType().GetField(value.ToString());

            var descriptionAttributes = fieldInfo.GetCustomAttributes(typeof(DisplayAttribute), false) as DisplayAttribute[];

            if (descriptionAttributes == null)
            {
                return string.Empty;
            }

            return (descriptionAttributes.Length > 0) ? descriptionAttributes[0].Name : value.ToString();
        }

        /// <summary>
        /// Gets the enum description based on the value of the enum
        /// </summary>
        /// <param name="value">The value of the enum</param>
        /// <returns>The description of the enum</returns>
        public static string GetDescription(object value)
        {
            var fieldInfo = value.GetType().GetField(value.ToString());

            DescriptionAttribute[] attributes =
                (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);

            return attributes.Length > 0 ? attributes[0].Description : value.ToString();
        }

        /// <summary>
        /// Checks if an enum contains a value with a specific description
        /// </summary>
        /// <param name="description">The description to check</param>
        /// <returns>True if the description is contained in the enum or false otherwise</returns>
        public static bool ContainsDescription(string description)
        {
            Type enumerator = typeof(TEnum);

            if (!enumerator.IsEnum)
            {
                throw new InvalidOperationException("Object is not an Enum.");
            }

            var enumDescriptions = new List<string>();

            foreach (var value in Enum.GetValues(enumerator))
            {
                enumDescriptions.Add(GetDescription(value).ToUpper());
            }

            return enumDescriptions.Contains(description);
        }
    }
}
