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

    /// <summary>
    /// Handles the creation, deletion and update of <see cref="Country">Countries</see> within the SpendManagementAPI.
    /// </summary>
    [RoutePrefix("Countries")]
    [Version(1)]
    public class CountriesV1Controller : ArchivingApiController<Country>
    {
        /// <summary>
        /// Gets all of the available end points from the <see cref="Country">Countries</see> part of the API.
        /// </summary>
        /// <returns>A list of available resource Links.</returns>
        [HttpOptions, Route("")]
        public List<Link> Options()
        {
            return base.Links();
        }

        /// <summary>
        /// Gets all <see cref="Country">Countries</see> available for the user.
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("")]
        [AuthAudit(SpendManagementElement.None, AccessRoleType.View)]
        public GetCountriesResponse GetAll()
        {
            return this.GetAll<GetCountriesResponse>();
        }
            
        /// <summary>
        /// Gets a <see cref="Country">Country</see> matching the specified id
        /// </summary>
        /// <param name="id">Id of country</param>
        /// <returns>A CountryResponse containing the country if found.</returns>
        [HttpGet, Route("{id:int}")]
        [AuthAudit(SpendManagementElement.None, AccessRoleType.View)]
        public CountryResponse Get([FromUri] int id)
        {
            return this.Get<CountryResponse>(id);
            }
            
        // POST api/<controller>
        /// <summary>
        /// Adds a <see cref="Country">Country</see> for the current user and sets up VAT rates for that country
        /// </summary>
        /// <param name="request">Country</param>
        /// <returns>Newly added country and VAT rates</returns>
        [Route("")]
        [AuthAudit(SpendManagementElement.Countries, AccessRoleType.Add)]
        public CountryResponse Post([FromBody]Country request)
        {
            return this.Post<CountryResponse>(request);
            }
            
        // PUT api/<controller>
        /// <summary>
        /// Archive/ unarchive an existing country record.
        /// Add/ update/ delete VAT rates. 
        /// Use the ForDelete flag on the VatRate to delete a VAT rate associated with the country. 
        /// </summary>
        /// <param name="id">The Id of the Country to modify</param>
        /// <param name="request">Country</param>
        /// <returns>Updated country data and VAT rates</returns>
        [Route("{id:int}")]
        [AuthAudit(SpendManagementElement.Countries, AccessRoleType.Edit)]
        public CountryResponse Put([FromUri] int id, [FromBody]Country request)
        {
            request.CountryId = id;
            return this.Put<CountryResponse>(request);
        }

        /// <summary>
        /// Archives or un-archives a <see cref="Country">Country</see>, depending on what is passed in.
        /// </summary>
        /// <param name="id">The id of the Country to be archived/un-archived.</param>
        /// <param name="archive">Whether to archive or un-archive this <see cref="Country">Country</see>.</param>
        /// <returns>A CountryResponse containing the freshly (un)archived Item.</returns>
        [HttpPatch, Route("{id:int}/Archive/{archive:bool}")]
        [AuthAudit(SpendManagementElement.Countries, AccessRoleType.Edit)]
        public CountryResponse Archive(int id, bool archive)
        {
            return this.Archive<CountryResponse>(id, archive);
        }

        /// <summary>
        /// Deletes a <see cref="Country">Country</see> associated with the current user
        /// </summary>
        /// <param name="id">Country Id</param>
        /// <returns>A CountryResponse with the item set to null upon a successful delete.</returns>
        [Route("{id:int}")]
        [AuthAudit(SpendManagementElement.Countries, AccessRoleType.Delete)]
        public CountryResponse Delete(int id)
        {
            return this.Delete<CountryResponse>(id);
        }

        /// <summary>
        /// Finds all <see cref="Country">Countries</see>  matching specified criteria. 
        /// Search for CountryId = 0 to get all unmapped global countries. 
        /// Available querystring parameters : SearchOperator,Alpha3CountryCode,Country,CountryCode,CountryId,GlobalCountryId,Numeric3CountryCode,Archived. 
        /// Use SearchOperator=0 to specify an AND query or SearchOperator=1 for an OR query
        /// </summary>
        /// <param name="criteria">Find query</param>
        /// <returns>A GetCountriesResponse containing matching <see cref="Country">Countries</see>.</returns>
        [HttpGet, Route("Find")]
        [AuthAudit(SpendManagementElement.Countries, AccessRoleType.View)]
        public FindCountriesResponse Find([FromUri]FindCountryRequest criteria)
        {
            var findCountriesResponse = this.InitialiseResponse<FindCountriesResponse>();
            var conditions = new List<Expression<Func<Country, bool>>>();
            if (!string.IsNullOrEmpty(criteria.Alpha3CountryCode))
            {
                conditions.Add(global =>
                    Repository.ActionContext.GlobalCountries.getGlobalCountryById(global.CountryId) != null &&
                    Repository.ActionContext.GlobalCountries.getGlobalCountryById(global.CountryId)
                        .Alpha3CountryCode.ToLower()
                        .Contains(criteria.Alpha3CountryCode.Trim().ToLower()));
            }

            if (!string.IsNullOrEmpty(criteria.Label))
            {
                conditions.Add(global =>
                    Repository.ActionContext.GlobalCountries.getGlobalCountryById(global.CountryId) != null &&
                    Repository.ActionContext.GlobalCountries.getGlobalCountryById(global.CountryId)
                        .Country.ToLower().Contains(criteria.Label.Trim().ToLower()));
            }

            if (!string.IsNullOrEmpty(criteria.CountryCode))
            {
                conditions.Add(global =>
                    Repository.ActionContext.GlobalCountries.getGlobalCountryById(global.CountryId) != null &&
                    Repository.ActionContext.GlobalCountries.getGlobalCountryById(global.CountryId)
                        .CountryCode.ToLower().Contains(criteria.CountryCode.Trim().ToLower()));
            }

            if (criteria.CountryId.HasValue)
            {
                conditions.Add(country => country.CountryId == criteria.CountryId);
            }

            if (criteria.GlobalCountryId.HasValue)
            {
                conditions.Add(country => country.GlobalCountryId == criteria.GlobalCountryId);
            }

            if (criteria.Numeric3CountryCode.HasValue)
            {
                conditions.Add(global =>
                    Repository.ActionContext.GlobalCountries.getGlobalCountryById(global.CountryId) != null &&
                    Repository.ActionContext.GlobalCountries.getGlobalCountryById(global.CountryId)
                        .Numeric3CountryCode == criteria.Numeric3CountryCode);
            }

            if (criteria.Archived.HasValue)
            {
                conditions.Add(country => country.Archived == criteria.Archived);
            }

            findCountriesResponse.List = this.RunFindQuery(this.Repository.GetAll().AsQueryable(), criteria, conditions);
            return findCountriesResponse;
        }

    }
}