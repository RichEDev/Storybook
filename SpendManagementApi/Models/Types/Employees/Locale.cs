namespace SpendManagementApi.Models.Types.Employees
{
    using System;
    using System.IO;

    using Interfaces;
    using SpendManagementLibrary;

    /// <summary>
    /// Represents the Locale of an object. This will usually be the language specific information for a user.
    /// </summary>
    public class Locale : BaseExternalType, IRequiresValidation, IEquatable<Locale>
    {
        /// <summary>
        /// The unique Id of this Locale.
        /// </summary>
        public int LocaleId { get; set; }

        /// <summary>
        /// The unique code of this Locale.
        /// </summary>
        public string LocaleCode { get; set; }

        /// <summary>
        /// The name of this Locale.
        /// </summary>
        public string LocaleName { get; set; }

        /// <summary>
        /// Whether the locale is currently active.
        /// </summary>
        public bool IsActive { get; set; }

        public void Validate(IActionContext actionContext)
        {
            if (LocaleId <= 0)
            {
                throw new InvalidDataException("Valid LocaleId must be provided");
            }
        }

        public bool Equals(Locale other)
        {
            if (other == null)
            {
                return false;
            }
            return this.IsActive.Equals(other.IsActive) && this.LocaleCode.Equals(other.LocaleCode) 
                   && this.LocaleName.Equals(other.LocaleName);
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as Locale);
        }
    }

    internal static class LocaleConversion
    {
        internal static TResult Cast<TResult>(this cLocale locale) where TResult : Locale, new()
        {
            return new TResult
                       {
                           LocaleCode = locale.LocaleCode,
                           LocaleId = locale.LocaleID,
                           LocaleName = locale.LocaleName,
                           IsActive = locale.Active
                       };
        }

        internal static cLocale Cast<TResult>(this Locale locale) where TResult : cLocale, new()
        {
            return new cLocale(locale.LocaleId, locale.LocaleName, locale.LocaleCode, locale.IsActive);
        }
    }
}