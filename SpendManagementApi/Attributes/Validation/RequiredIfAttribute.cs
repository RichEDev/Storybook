using System;
using System.ComponentModel.DataAnnotations;

namespace SpendManagementApi.Attributes.Validation
{
    /// <summary>
    /// Ensures this property is required if the supplied property is populated.
    /// </summary>
    public class RequiredIfAttribute : RequiredAttribute
    {
        /// <summary>
        /// The name of the other property that must be populated in order for this property to be required.
        /// </summary>
        public String PropertyName { get; set; }
        
        /// <summary>
        /// Ensures this property is required if the supplied property is populated.
        /// </summary>
        /// <param name="propertyName">The name of the other property.</param>
        public RequiredIfAttribute(String propertyName)
        {
            PropertyName = propertyName;
        }

        protected override ValidationResult IsValid(object value, ValidationContext context)
        {
            var instance = context.ObjectInstance;
            var type = instance.GetType();
            var propertyValue = type.GetProperty(PropertyName).GetValue(instance, null);
            return propertyValue == null ? new ValidationResult(string.Format("Property {0} must not be null", PropertyName)) : ValidationResult.Success;
        }
    }

    /// <summary>
    /// Ensures this property is required if the supplied property is populated to the desired value.
    /// </summary>
    public class RequiredIfDesiredAttribute : RequiredAttribute
    {
        /// <summary>
        /// The name of the property to check.
        /// </summary>
        public String PropertyName { get; set; }

        /// <summary>
        /// The desired value of the property.
        /// </summary>
        public Object DesiredValue { get; set; }

        /// <summary>
        /// Ensures this property is required if the supplied property is populated to the desired value.
        /// </summary>
        /// <param name="propertyName">The name of the other property.</param>
        /// <param name="desiredvalue">The desired value.</param>
        public RequiredIfDesiredAttribute(String propertyName, Object desiredvalue)
        {
            PropertyName = propertyName;
            DesiredValue = desiredvalue;
        }

        protected override ValidationResult IsValid(object value, ValidationContext context)
        {
            var instance = context.ObjectInstance;
            var type = instance.GetType();
            var proprtyvalue = type.GetProperty(PropertyName).GetValue(instance, null);
            if (proprtyvalue.ToString() == DesiredValue.ToString())
            {
                var result = base.IsValid(value, context);
                return result;
            }
            return ValidationResult.Success;
        }
    }
}