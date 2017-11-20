namespace SpendManagementApi.Common
{
    using Interfaces;
    using Models.Types;
    using Spend_Management;
    using System;
    using System.Configuration;
    using System.Linq;
    using System.Security.Principal;
    using System.Web;
    using System.Collections.Generic;
    using Attributes;
    using Controllers;

    internal class Helper
    {
        internal ICurrentUser GetDummyUser()
        {
            string identity = ConfigurationManager.AppSettings["dummyUser"];
            string[] identityParts = identity.Split(',');
            int accountId = Int32.Parse(identityParts[0]);
            int employeeId = Int32.Parse(identityParts[1]);
            GenericIdentity genericIdentity = new GenericIdentity(identity);
            GenericPrincipal principal = new GenericPrincipal(genericIdentity, new string[] { });
            if (HttpContext.Current != null)
            {
                HttpContext.Current.User = principal;
            }
            return new CurrentUser(accountId, employeeId, employeeId, Modules.SpendManagement, 1);
        }

        internal static T NullIf<T>(T objToCheck)
            where T : class, new()
        {
            return objToCheck ?? new T();
        }

        internal static void ValidateIfNotNull(IRequiresValidation objToValidate, IActionContext actionContext, int? accountId)
        {
            if (objToValidate != null)
            {
                if (objToValidate is BaseExternalType && accountId.HasValue)
                {
                    ((BaseExternalType)objToValidate).AccountId = accountId.Value;
                }

                objToValidate.Validate(actionContext);
            }
        }

        /// <summary>
        /// Whether or not the request to this controller came from one of our mobile applications. We cannot use the property of <see cref="BaseApiController"/> in all of these methods because they don't all have the <see cref="AuthAuditAttribute"/>.
        /// </summary>
        /// <param name="userAgent">The user-agent from the request</param>
        /// <returns>
        /// True if the request originated from mobile, false otherwise.
        /// </returns>
        public static bool IsMobileRequest(string userAgent)
        {
            string mobileHeaders = ConfigurationManager.AppSettings["MobileUserAgent"] ?? string.Empty;
            List<string> mobileHeadList = mobileHeaders.Split(Char.Parse("|")).ToList();
            return mobileHeadList.Contains(userAgent);
        }
      
        /// <summary>
        /// The determine currency description from a currency Id.
        /// </summary>
        /// <param name="currencies">
        /// An instance of <see cref="cCurrencies"/>
        /// </param>
        /// <param name="globalCurrencies">
        /// An instance of <see cref="cGlobalCurrencies"/>
        /// </param>
        /// <param name="currencyId">
        /// The currency Id.
        /// </param>
        /// <returns>
        /// The <see cref="string"/> with the currency description.
        /// </returns>
        public static string GetCurrencyDescriptionFromId(cCurrencies currencies, cGlobalCurrencies globalCurrencies, int currencyId)
        {      
            var globalCurrencyId = currencies.getCurrencyById(currencyId).globalcurrencyid;      
            return globalCurrencies.getGlobalCurrencyById(globalCurrencyId).label;          
        }  
    }

    /// <summary>
    /// Struct Extensions.
    /// </summary>
    public static class StructExtensions
    {
        /// <summary>Checks whether the current struct value is in the provided.</summary>
        /// <param name="val">The struct value to check.</param>
        /// <param name="values">The list of possible values.</param>
        /// <typeparam name="T">The generic type of the struct.</typeparam>
        /// <returns>Whether the values contains the current value.</returns>
        public static bool In<T>(this T val, params T[] values)
        {
            return values.Contains(val);
        }
    }
}