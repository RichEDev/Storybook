

namespace SpendManagementApi.Attributes.Validation
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Utilities;
    using System.Web;
    using Spend_Management;

    /// <summary>
    /// Checks the CurrencyId is valid.
    /// </summary>
    public class CurrencyIdValidation : ApiRequestValidationBase
    {
        /// <summary>
        /// Ensures this property name is populated.
        /// </summary>
        /// <param name="propertyName">The name of the property.</param>
        public CurrencyIdValidation(String propertyName)
            : base(propertyName)
        {

        }

        /// <summary>
        /// Validates the CurrencyId against set criteria.
        /// </summary>
        /// <param name="value">The value (required to perform the override)</param>
        /// <param name="context">The validation context</param>
        /// <returns>The <see cref="ValidationResult">ValidationResult</see>/></returns>
        protected override ValidationResult IsValid(object value, ValidationContext context)
        {
            {
                string validationFailMessage = string.Empty;
                if (base.IsValid(context))
                {
                    var currencyId = (int)this.ParsedValue;

                    if (currencyId != 0)
                    {
                        //Put request for existing expense item, so check Id is valid

                        string token = null;
                        
                        if (HttpContext.Current != null)
                        {
                          token = HttpContext.Current.Request.Headers["AuthToken"];
                        }
                
                        var accountId = base.GetAccountId(cMisc.GetCurrentUser(),token);
    
                        if (accountId != 0)
                        {
                            var currencies = new cCurrencies(accountId, null);
                            var item = currencies.getCurrencyById(Convert.ToInt32(this.ParsedValue));

                            if (item == null)
                            {
                                validationFailMessage = string.Format("Property {0} must be a valid Currency Id ", PropertyName);
                            }
                        }
                        else
                        {
                            validationFailMessage = ApiResources.HttpStatusCodeUnauthorised;
                        }
                    }
                }
                else
                {
                    validationFailMessage = string.Format("Property {0} cannot be parsed to Int32 ", PropertyName);
                }

                return validationFailMessage != string.Empty ? new ValidationResult(validationFailMessage, new List<string> { PropertyName }) : ValidationResult.Success;
            }
        }
    }
}
