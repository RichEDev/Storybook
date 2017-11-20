namespace SpendManagementApi.Attributes.Validation
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Reflection;
    using System.Web;
    using Models.Types;
    using Spend_Management;

    /// <summary>
    /// A class do deal with the property value for validation
    /// </summary>
    public abstract class ApiRequestValidationBase : ValidationAttribute
    {
        /// <summary>
        /// The name of the property that is being validated.
        /// </summary>
        public string PropertyName { get; set; }

        /// <summary>
        /// The parsed property value
        /// </summary>
        public object ParsedValue { get; set; }

        /// <summary>
        /// The property type
        /// </summary>
        public string PropertyType { get; set; }

        /// <summary>
        /// Ensures this property is populated.
        /// </summary>
        /// <param name="propertyName">The name of the property.</param>
        protected ApiRequestValidationBase(String propertyName)
        {
            PropertyName = propertyName;
        }

        /// <summary>
        /// Sets the parsed property value based on its type
        /// </summary>
        /// <param name="context">The validation context</param>
        /// <returns>The outcome of the property value manipulation</returns>
        protected bool IsValid(ValidationContext context)
        {
            var instance = context.ObjectInstance;
            Type type = instance.GetType();

            if (this.PropertyName != null)
            {
                PropertyInfo propertyInfo = type.GetProperty(this.PropertyName);
                var propertyValue = propertyInfo.GetValue(instance, null).ToString();
                this.PropertyType = propertyInfo.PropertyType.Name;
                ParsedValue = null;
               
                switch (PropertyType)
                {
                    case "Int32":
                        int id;

                        if (Int32.TryParse(propertyValue, out id))
                        {
                            ParsedValue = id;
                        }

                        break;
                    case "DateTime":
                        DateTime dateTime;

                        if (DateTime.TryParse(propertyValue, out dateTime))
                        {
                            ParsedValue = dateTime;
                        }

                        break;
                }
            }
            else
            {
                return false;
            }

            return ParsedValue != null;
        }

        /// <summary>
        /// Gets the accountId from the AuthToken if it cannot be obtained by GetCurrentUser
        /// </summary>
        /// <returns>The accountId</returns>
        protected int GetAccountId(ICurrentUser currentUser, string token)
        {
            
            if (currentUser == null)
            {                      
                if (token != null)
                {
                    ApiDetails.AuthTokenElements tokenElements = ApiDetails.GetAuthTokenElements(token);
            
                    return tokenElements.Result == ApiDetails.ApiDetailsValidity.Valid ? tokenElements.AccountId : 0;
                }
                else
                {
                    return 0;
                }
            }
            else
            {
                return currentUser.AccountID;
            }       
        }
    }
}