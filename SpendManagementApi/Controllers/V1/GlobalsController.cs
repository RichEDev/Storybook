namespace SpendManagementApi.Controllers.V1
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Http;

    using SpendManagementApi.Attributes;
    using SpendManagementApi.Models.Common;
    using SpendManagementApi.Models.Responses;
    using SpendManagementApi.Models.Types;
    using SpendManagementApi.Models.Types.Employees;

    using Spend_Management;

    /// <summary>
    /// Manages operations on Global <see cref="Locale">Locales</see>.
    /// </summary>
    [RoutePrefix("Globals")]
    [Version(1)]
    public class GlobalsV1Controller : BaseApiController
    {
        /// <summary>
        /// Gets all of the available end points from the Globals part of the API.
        /// </summary>
        /// <returns>A list of available resource Links</returns>
        [HttpOptions, Route("")]
        public List<Link> Options()
        {
            return this.Links("Options");
        }

        /// <summary>
        /// Returns list of <see cref="Locale">Locales</see>.
        /// </summary>
        /// <returns>A list of Locales.</returns>
        [HttpGet, Route("Locales")]
        [AuthAudit(SpendManagementElement.None, AccessRoleType.View)]
        public GetLocalesResponse Locales()
        {
            var response = this.InitialiseResponse<GetLocalesResponse>();
            var list = new cLocales();
            response.List = list.GetAllLocales().Select(l => l.Cast<Locale>()).ToList();
            return response;
        }

        /// <summary>
        /// Returns list of <see cref="GlobalCountry">GlobalCountries</see>.
        /// </summary>
        /// <returns>A list of GlobalCountries.</returns>
        [HttpGet, Route("GlobalCountries")]
        [AuthAudit(SpendManagementElement.None, AccessRoleType.View)]
        public GetGlobalCountriesResponse GlobalCountries()
        {
            var response = this.InitialiseResponse<GetGlobalCountriesResponse>();
            var list = new cGlobalCountries();
            response.List = list.GetList().Select(l => l.Value.Cast<GlobalCountry>()).ToList();
            return response;
        }

        /// <summary>
        /// Returns list of <see cref="GlobalCurrency">GlobalCurrencies</see>.
        /// </summary>
        /// <returns>A list of GlobalCurrencies.</returns>
        [HttpGet, Route("GlobalCurrencies")]
        [AuthAudit(SpendManagementElement.None, AccessRoleType.View)]
        public GetGlobalCurrenciesResponse GlobalCurrencies()
        {
            var response = this.InitialiseResponse<GetGlobalCurrenciesResponse>();
            var list = new cGlobalCurrencies();
            response.List = list.globalcurrencies.Values.Select(l => l.Cast<GlobalCurrency>()).ToList();
            return response;
        }

        /// <summary>
        /// Method to be implemented by derived class to initialise repository
        /// </summary>
        protected override void Init() { }
    }
}
