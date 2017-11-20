using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using SpendManagementApi.Common;
using SpendManagementApi.Utilities;

namespace SpendManagementApi.Attributes.Validation
{
    /// <summary>
    /// Ensures an enum has a valid value.
    /// </summary>
    [AttributeUsage(AttributeTargets.Enum | AttributeTargets.Property)]
    public class IsSpendManagementValueAttribute : ValidationAttribute
    {
        /// <summary>
        /// The list of valid SpendManagementElement values this property can be.
        /// </summary>
        public SpendManagementElement[] ValidValueList;
        private readonly bool allowNull;

        /// <summary>
        /// Ensures the marked property is not only an enum value but also is one of the specififed values.
        /// </summary>
        /// <param name="validValues">The list of valid Enum values.</param>
        /// <param name="allowNull">Whether to permit a null value</param>
        public IsSpendManagementValueAttribute(bool allowNull = false, params SpendManagementElement[] validValues)
        {
            ValidValueList = validValues;
            this.allowNull = allowNull;
            ErrorMessage = ApiResources.ApiErrorSpendManagementElementEnum +
                           ValidValueList.Aggregate("", (current, @enum) => current + @enum);
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (allowNull) return ValidationResult.Success;

            var enumType = value.GetType();
            var valid = Enum.IsDefined(enumType, value);
            if (!valid)
            {
                return new ValidationResult(String.Format("{0} is not a valid value for type {1}", value, enumType.Name));
            }

            return value.In(ValidValueList) ? ValidationResult.Success : new ValidationResult(ErrorMessage);
        }
    }
}