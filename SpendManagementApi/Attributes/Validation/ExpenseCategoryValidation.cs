namespace SpendManagementApi.Attributes.Validation
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Spend_Management;

    /// <summary>
    /// Validates the supplied expense category if not 0
    /// </summary>
    public class ExpenseCategoryValidation : ApiRequestValidationBase
    {

        /// <summary>
        /// Ensures this property is required if the supplied property is populated.
        /// </summary>
        /// <param name="propertyName">The name of the property.</param>
        public ExpenseCategoryValidation(String propertyName)
            : base(propertyName)
        {
        }

        /// <summary>
        /// Validates the expense categoet Id against set criteria.
        /// </summary>
        /// <param name="value">The value (required to perform the override)</param>
        /// <param name="context">The validation context</param>
        /// <returns>The <see cref="ValidationResult">ValidationResult</see>/></returns>
        protected override ValidationResult IsValid(object value, ValidationContext context)
        {
            if (base.IsValid(context))
            {
                var expenseCatId = (int)this.ParsedValue;

                if (expenseCatId == 0)
                {
                    //0 is valid for a Post
                    return ValidationResult.Success;
                }
                else
                {
                    //Put request for existing expense category, so check Id is valid
                    CurrentUser user = cMisc.GetCurrentUser();
                    var expenseCategories = new cCategories(user.AccountID);
                    var item = expenseCategories.FindById(expenseCatId);

                    return item == null ? new ValidationResult(string.Format("Property {0} must be a valid Expense Category Id ", PropertyName), new List<string> { PropertyName }) : ValidationResult.Success;
                }
            }
            else
            {
                return new ValidationResult(string.Format("Property {0} cannot be parsed to Int32 ", PropertyName),
                   new List<string> { PropertyName });
            }
        }
    }
}