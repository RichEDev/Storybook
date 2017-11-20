using Spend_Management;

namespace SpendManagementApi.Controllers.V1
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Web.Http;

    using Microsoft.Ajax.Utilities;

    using SpendManagementApi.Attributes;
    using SpendManagementApi.Interfaces;
    using SpendManagementApi.Models.Common;
    using SpendManagementApi.Models.Requests;
    using SpendManagementApi.Models.Responses;
    using SpendManagementApi.Models.Types;
    using SpendManagementApi.Repositories;
    using SpendManagementLibrary;

    /// <summary>
    /// Manages operations on <see cref="Currency">Currencies</see>.
    /// </summary>
    [RoutePrefix("Currencies")]
    [Version(1)]
    public class CurrenciesV1Controller : ArchivingApiController<Currency>
    {
        /// <summary>
        /// Gets all of the available end points from the <see cref="Currency">Currencies</see> part of the API.
        /// </summary>
        /// <returns>A list of available resource Links</returns>
        [HttpOptions, Route("")]
        public List<Link> Options()
        {
            return base.Links();
        }

        /// <summary>
        /// Gets all <see cref="Currency">Currencies</see> available.
        /// </summary>
        /// <returns>A GetCurrenciesResponse containing all <see cref="Currency">Currencies</see>.</returns>
        [HttpGet, Route("")]
        [AuthAudit(SpendManagementElement.None, AccessRoleType.View)]
        public GetCurrenciesResponse GetAll()
        {
            return this.GetAll<GetCurrenciesResponse>();
        }

        /// <summary>
        /// Gets a <see cref="Currency">Currency</see> matching the specified id.
        /// </summary>
        /// <param name="id">The Id of the <see cref="Currency">Currency</see>.</param>
        /// <returns>A CurrencyResponse containing the matching <see cref="Currency">Currency</see>.</returns>
        [HttpGet, Route("{id:int}")]
        [AuthAudit(SpendManagementElement.None, AccessRoleType.View)]
        public CurrencyResponse Get([FromUri] int id)
        {
            return this.Get<CurrencyResponse>(id);
        }

        /// <summary>
        /// Gets a <see cref="Currency">Currency</see> matching the specific code, as per ISO 3166-1
        /// </summary>
        /// <param name="id">The ISO Numeric code of the <see cref="Currency">Currency</see>.</param>
        /// <returns>A CurrencyResponse containing the matching <see cref="Currency">Currency</see>.</returns>
        [HttpGet, Route("ByNumericCode/{id:int}")]
        [AuthAudit(SpendManagementElement.Currencies, AccessRoleType.View)]
        public CurrencyResponse ByNumericCode([FromUri] int id)
        {
            var response = this.InitialiseResponse<CurrencyResponse>();
            response.Item = ((CurrencyRepository)this.Repository).ByNumericCode(id);
            return response;
        }

        /// <summary>
        /// Gets a <see cref="Currency">Currency</see> matching the specific APLHA code, as per ISO 3166-1 - Alpha3
        /// </summary>
        /// <param name="id">The ISO Alpha code of the <see cref="Currency">Currency</see>.</param>
        /// <returns>A CurrencyResponse containing the matching <see cref="Currency">Currency</see>.</returns>
        [HttpGet, Route("ByAlphaCode/{id}")]
        [AuthAudit(SpendManagementElement.Currencies, AccessRoleType.View)]
        public CurrencyResponse ByAlphaCode([FromUri] string id)
        {
            var response = this.InitialiseResponse<CurrencyResponse>();
            response.Item = ((CurrencyRepository)this.Repository).ByAlphaCode(id);
            return response;
        }

        /// <summary>
        /// Gets a <see cref="Currency">Currency</see> matching the global currency I.D.
        /// </summary>
        /// <param name="id">The Id of the global currency <see cref="Currency">Currency</see>.</param>
        /// <returns>A CurrencyResponse containing the matching <see cref="Currency">Currency</see>.</returns>
        [HttpGet, Route("ByGlobalCurrencyId/{id}")]
        [AuthAudit(SpendManagementElement.Currencies, AccessRoleType.View)]
        public CurrencyResponse ByGlobalCurrencyId([FromUri] int id)
        {
            var response = InitialiseResponse<CurrencyResponse>();
            response.Item = ((CurrencyRepository)Repository).ByGlobalCurrencyId(id);
            return response;
        }

        /// <summary>
        /// Finds all <see cref="Currency">Currencies</see> matching specified criteria. 
        /// Available querystring parameters : SearchOperator,AlphaCode,Name,NumericCode,CurrencyId,GlobalCurrencyId,Symbol,Archived. 
        /// Use SearchOperator=0 to specify an AND query or SearchOperator=1 for an OR query
        /// </summary>
        /// <param name="criteria">Search parameters</param>
        /// <returns>A FindCurrenciesResponse containing matching <see cref="Currency">Currencies</see>.</returns>
        [HttpGet, Route("Find")]
        [AuthAudit(SpendManagementElement.Currencies, AccessRoleType.View)]
        public GetCurrenciesResponse Find([FromUri] FindCurrencyRequest criteria)
        {
            var findCurrenciesResponse = this.InitialiseResponse<GetCurrenciesResponse>();
            var conditions = new List<Expression<Func<Currency, bool>>>();

            if (!string.IsNullOrEmpty(criteria.Label))
            {
                Expression<Func<Currency, bool>> expression = global =>
                    Repository.ActionContext.GlobalCurrencies.getGlobalCurrencyById(global.CurrencyId, null) != null &&
                    Repository.ActionContext.GlobalCurrencies.getGlobalCurrencyById(global.CurrencyId, null)
                        .label.ToLower().Contains(criteria.Label.Trim().ToLower());

                conditions.Add(expression);
            }
            if (criteria.CurrencyId.HasValue)
            {
                conditions.Add(currency => currency.CurrencyId == criteria.CurrencyId);
            }
            if (criteria.GlobalCurrencyId.HasValue)
            {
                conditions.Add(currency => currency.GlobalCurrencyId == criteria.GlobalCurrencyId);
            }
            if (!string.IsNullOrEmpty(criteria.Symbol))
            {
                conditions.Add(global =>
                    Repository.ActionContext.GlobalCurrencies.getGlobalCurrencyById(global.CurrencyId, null) != null &&
                    Repository.ActionContext.GlobalCurrencies.getGlobalCurrencyById(global.CurrencyId, null)
                        .symbol.ToLower().Contains(criteria.Symbol.Trim().ToLower()));
            }
            if (criteria.Archived.HasValue)
            {
                conditions.Add(currency => currency.Archived == criteria.Archived);
            }

            findCurrenciesResponse.List = this.RunFindQuery(this.Repository.GetAll().AsQueryable(), criteria, conditions);
            return findCurrenciesResponse;
        }

        /// <summary>
        /// Gets all the active currencies for a specific account.
        /// </summary>
        /// <param name="accountId">The Account ID you want currencies for</param>
        /// <returns></returns>
        [HttpGet, Route("GetActiveCurrenciesForAccount/{accountId:int}")]
        [InternalSelenityMethod]
        public GetCurrenciesResponse GetActiveCurrenciesForAccount(int accountId)
        {
            var response = this.InitialiseResponse<GetCurrenciesResponse>();
            response.List = new List<Currency>();

            // List of associated currencies
            var cCurrencies = new cCurrencies(accountId, null).currencyList;
            var cGlobalCurrencies = new cGlobalCurrencies();

            List<Currency> currencies = cCurrencies.Values.OfType<cCurrency>().Select(c => c.Cast<Currency>(cGlobalCurrencies)).ToList();

            List<int> associatedGlobalCurrencyIds = currencies.Select(c => c.GlobalCurrencyId).ToList();

            // List of unassociated currencies
            List<int> unassociatedGlobalCurrencyIds = cGlobalCurrencies.getGlobalCurrencyIds().Except(associatedGlobalCurrencyIds).ToList();
            List<Currency> unassociatedGlobalCurrencies = cGlobalCurrencies.globalcurrencies.Values.Where(gc => gc.sUnicodeSymbol.Trim().Length > 0 && unassociatedGlobalCurrencyIds.Contains(gc.globalcurrencyid)).Select(gc => gc.Cast<Currency>(true)).ToList();

            currencies = currencies.Union(unassociatedGlobalCurrencies).ToList();

            foreach (var currency in currencies.Where(c => c.Archived == false))
            {
                currency.CurrencyName = cGlobalCurrencies.getGlobalCurrencyById(currency.GlobalCurrencyId).label;
                currency.AlphaCode = cGlobalCurrencies.getGlobalCurrencyById(currency.GlobalCurrencyId).alphacode;

                response.List.Add(currency);
            }

            return response;
        }

        /// <summary>
        /// Adds a <see cref="Currency">Currency</see> for the current user.
        /// <see cref="Currency">Currency</see> type for the user can also be provided with the currency.
        /// </summary>
        /// <param name="request">The <see cref="Currency">Currency</see> to be added</param>
        /// <returns>A CurrencyResponse containing the edited <see cref="Currency">Currency</see>.</returns>
        [Route("")]
        [AuthAudit(SpendManagementElement.Currencies, AccessRoleType.Add)]
        public CurrencyResponse Post([FromBody]Currency request)
        {
            return this.Post<CurrencyResponse>(request);
        }

        /// <summary>
        /// Archive/ unarchive the specified <see cref="Currency">Currency</see> by setting the Archived property to true
        /// Updates the <see cref="Currency">Currency</see> type for the current user.
        /// Cannot be used to update the GlobalCountry associated with the currency
        /// </summary>
        /// <param name="id">The Id of the <see cref="Currency">Currency</see> to update.</param>
        /// <param name="request">The <see cref="Currency">Currency</see> to update</param>
        /// <returns>A CurrencyResponse containing the edited <see cref="Currency">Currency</see>.</returns>
        [Route("{id:int}")]
        [AuthAudit(SpendManagementElement.Currencies, AccessRoleType.Edit)]
        public CurrencyResponse Put([FromUri] int id, [FromBody]Currency request)
        {
            request.CurrencyId = id;
            return this.Put<CurrencyResponse>(request);
        }

        /// <summary>
        /// Archives or un-archives a <see cref="Currency">Currency</see>, depending on what is passed in.
        /// </summary>
        /// <param name="id">The id of the Currency to be archived/un-archived.</param>
        /// <param name="archive">Whether to archive or un-archive this <see cref="Currency">Currency</see>.</param>
        /// <returns>A CurrencyResponse containing the freshly (un)archived Item.</returns>
        [HttpPatch, Route("{id:int}/Archive/{archive:bool}")]
        [AuthAudit(SpendManagementElement.Currencies, AccessRoleType.Edit)]
        public CurrencyResponse Archive(int id, bool archive)
        {
            return this.Archive<CurrencyResponse>(id, archive);
        }

        /// <summary>
        /// Deleted the specified <see cref="Currency">Currency</see>.
        /// </summary>
        /// <param name="id">The Id of the <see cref="Currency">Currency</see> to delete.</param>
        /// <returns>A CurrencyResponse with the item set to null upon a successful delete.</returns>
        [Route("{id:int}")]
        [AuthAudit(SpendManagementElement.Currencies, AccessRoleType.Delete)]
        public CurrencyResponse Delete(int id)
        {
            return this.Delete<CurrencyResponse>(id);
        }

    }
}