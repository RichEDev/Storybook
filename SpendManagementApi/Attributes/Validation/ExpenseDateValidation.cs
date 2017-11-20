namespace SpendManagementApi.Attributes.Validation
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Data.SqlTypes;

    /// <summary>
    /// A custom date validation class for data annotations
    /// </summary>
    public class ExpenseDateValidation : ApiRequestValidationBase
    {
        /// <summary>
        /// Ensures this property is populated.
        /// </summary>
        /// <param name="propertyName">The name of the property.</param>
        public ExpenseDateValidation(string propertyName)
            : base(propertyName)
        {
        }

        /// <summary>
        /// Checks if the date is less than today and greater than the minimum datetime value
        /// </summary>
        /// <param name="value">
        /// The datetime object.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <returns>
        /// The outcome of the validation<see cref="ValidationResult"/>
        /// </returns>
        protected override ValidationResult IsValid(object value, ValidationContext context)
        {
            string validationFailMessage = string.Empty;

            if (base.IsValid(context))
            {
                DateTime date = Convert.ToDateTime(value);
                if (date > DateTime.Today && date <= DateTime.MinValue)
                {
                    validationFailMessage = string.Format(
                        "Property {0} must be a greater than {1} and no greater than {2}."
                        , this.PropertyName, SqlDateTime.MinValue.Value.ToShortDateString(), DateTime.Today.AddDays(1));
                }
            }
            else
            {
                validationFailMessage = string.Format("Property {0} cannot be parsed to a DateTime.", this.PropertyName);
            }

            return validationFailMessage != string.Empty ? new ValidationResult(validationFailMessage, new List<string> { this.PropertyName }) : ValidationResult.Success;
        }      
    }
}