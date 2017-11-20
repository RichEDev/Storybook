namespace SpendManagementApi.Attributes.Validation
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Data.SqlTypes;

    /// <summary>
    /// Validation checks for the Curreny Exchange Rate date 
    /// </summary>
    public class CurrencyExchangeRateDateValidation : ApiRequestValidationBase
    {

        /// <summary>
        /// Ensures this property name is populated.
        /// </summary>
        /// <param name="propertyName">The name of the property.</param>
        public CurrencyExchangeRateDateValidation(String propertyName)
            : base(propertyName)
        {

        }

        /// <summary>
        /// Validates the Curreny Exchange Rate date against set criteria.
        /// </summary>
        /// <param name="value">The value (required to perform the override)</param>
        /// <param name="context">The validation context</param>
        /// <returns>The <see cref="ValidationResult">ValidationResult</see>/></returns>
        protected override ValidationResult IsValid(object value, ValidationContext context)
        {
            string validationFailMessage = string.Empty;

            if (base.IsValid(context))
            {
                var dateOfExchangeRate = Convert.ToDateTime(ParsedValue);

                if (dateOfExchangeRate > DateTime.Today || dateOfExchangeRate < SqlDateTime.MinValue.Value)
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
