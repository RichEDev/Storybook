namespace SpendManagementApi.Attributes.Validation
{
    using System;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Ensures an enum has a valid value.
    /// </summary>
    [AttributeUsage(AttributeTargets.Enum | AttributeTargets.Property)]
    public class ValidEnumValueAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var enumType = value.GetType();
            var valid = Enum.IsDefined(enumType, value);
            return !valid ? new ValidationResult(String.Format("{0} is not a valid value for type {1}", value, enumType.Name)) : ValidationResult.Success;
        }
    }


    /// <summary>
    /// Ensures that the integer this is tagged to is a valid value in the enum specified.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class IdIsValidEnumValueAttribute : ValidEnumValueAttribute
    {
        private readonly Type _enumType;

        /// <summary>
        /// Creates a new Attribute that ensures the Id this is tagged to fits one of the values in the specified enum.
        /// </summary>
        /// <param name="enumType">The Enum</param>
        public IdIsValidEnumValueAttribute(Type enumType)
        {
            if (!enumType.IsEnum)
            {
                throw new NotSupportedException("Please make sure the type supplied is an enum type.");
            }
            this._enumType = enumType;
        }

        /// <summary>
        /// Determines whether the Id this attribute is attached to can be cast to a value of the specified enum.
        /// </summary>
        /// <param name="value">The Value (which should be an int)</param>
        /// <param name="validationContext">The validation context.</param>
        /// <returns>A Validation result.</returns>
        /// <exception cref="NotSupportedException">Throws if the property cannot be cast as an int.</exception>
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            int intValue = (int) value;
            try
            {
                var valid = Enum.IsDefined(_enumType, intValue);
                return !valid ? new ValidationResult(String.Format("{0} is not a valid value for type {1}", intValue, _enumType)) : ValidationResult.Success;
            }
            catch (Exception)
            {
                throw new NotSupportedException("Please use this attribute only on integer types.");
            }
        }
    }
}