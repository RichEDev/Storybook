using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SpendManagementApi.Models.Common;
using SpendManagementApi.Models.Types;
using SpendManagementApi.Models.Types.Employees;

namespace SpendManagementApi.Models.Responses
{
    /// <summary>
    /// A response containing a list of <see cref="Locale">Locale</see>s.
    /// </summary>
    public class GetLocalesResponse : GetApiResponse<Locale>
    {
        /// <summary>
        /// Creates a new GetLocalesResponse.
        /// </summary>
        public GetLocalesResponse()
        {
            List = new List<Locale>();
        }
    }

    
    /// <summary>
    /// A response containing a list of <see cref="GlobalCountry">GlobalCountries</see>.
    /// </summary>
    public class GetGlobalCountriesResponse : GetApiResponse<GlobalCountry>
    {
        /// <summary>
        /// Creates a new GetGlobalCountriesResponse.
        /// </summary>
        public GetGlobalCountriesResponse()
        {
            List = new List<GlobalCountry>();
        }
    }
    
    /// <summary>
    /// A response containing a list of <see cref="GlobalCurrency">GlobalCurrencies</see>.
    /// </summary>
    public class GetGlobalCurrenciesResponse : GetApiResponse<GlobalCurrency>
    {
        /// <summary>
        /// Creates a new GetGlobalCurrenciesResponse.
        /// </summary>
        public GetGlobalCurrenciesResponse()
        {
            List = new List<GlobalCurrency>();
        }
    }

    
    
}