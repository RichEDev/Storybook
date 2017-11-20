namespace SpendManagementApi.Controllers.V1
{
    using System.Collections.Generic;
    using System.Web.Http;
    using Attributes;
    using Models.Common;
    using Models.Responses;
    using Models.Types;
    using Repositories;
    using FilterType = Common.Enums.FilterType;

    /// <summary>
    /// Manages operations on <see cref="FilterRule">FilterRules</see>.
    /// </summary>
    [RoutePrefix("FilterRules")]
    [Version(1)]
    public class FilterRulesV1Controller : BaseApiController<FilterRule>
    {
        /// <summary>
        /// Gets all of the available end points from the <see cref="FilterRule">FilterRule</see> part of the API.
        /// </summary>
        /// <returns>A list of available resource Links</returns>
        [HttpOptions, Route("")]
        public List<Link> Options()
        {
            return base.Links();
        }

        /// <summary>
        /// Gets all <see cref="FilterRule">FilterRules</see> in the system.
        /// </summary>
        [HttpGet, Route("")]
        [AuthAudit(SpendManagementElement.None, AccessRoleType.View)]
        public GetFilterRulesResponse GetAll()
        {
            return GetAll<GetFilterRulesResponse>();
        }

        /// <summary>
        /// Gets a filter rule <see cref="FilterRuleResponse">FilterRuleResponse </see>, by its Id.
        /// </summary>
        /// <param name="id">The Id of the filter rule.</param>
        /// <returns>A FilterRuleResponse, containing the <see cref="FilterRule">FilterRule</see> if found.</returns>
        [HttpGet, Route("{id:int}")]
        [AuthAudit(SpendManagementElement.Expenses, AccessRoleType.View)]
        public FilterRuleResponse Get([FromUri] int id)
        {
            return this.Get<FilterRuleResponse>(id);
        }

        /// <summary>
        /// Gets the <see cref="GetFilterRulesResponse">GetFilterRulesResponse</see> for a specific type.
        /// <param name="filterType">The filter type value</param>
        /// </summary>
        [HttpGet, Route("GetFilterRulesForAFilterType")]
        [AuthAudit(SpendManagementElement.Expenses, AccessRoleType.View)]
        public GetFilterRulesResponse GetFilterRulesByType(FilterType filterType)
        {
            var response = InitialiseResponse<GetFilterRulesResponse>();
            response.List = ((FilterRuleRepository)Repository).GetFilterRulesByType(filterType, response);
            return response;
        }

        /// <summary>
        /// Gets the <see cref="GetFilterRuleItemsResponse">FilterRules</see> for the crieria.
        /// <param name="filterType">The filter type value</param>
        /// <param name="useDescription">Whether the item description should be used, or the item name</param>
        /// </summary>
        [HttpGet, Route("GetItems")]
        [AuthAudit(SpendManagementElement.Expenses, AccessRoleType.View)]
        public GetFilterRuleItemsResponse GetItems(FilterType filterType, bool useDescription)
        {
            var response = InitialiseResponse<GetFilterRuleItemsResponse>();
            response.List = ((FilterRuleRepository)Repository).GetItems(filterType, useDescription, response);
            return response;
        }

        /// <summary>
        /// Gets the <see cref="FilterRuleItemResponse">FilterRuleControlNameResponse</see> for a specific type.
        /// <param name="filterType">The filter type value</param>
        /// </summary>
        [HttpGet, Route("GetFilterRuleControlNameForChild")]
        [AuthAudit(SpendManagementElement.Expenses, AccessRoleType.View)]
        public FilterRuleItemResponse GetFilterRuleControlNameForChild(FilterType filterType)
        {
            var response = InitialiseResponse<FilterRuleItemResponse>();
            response.Item = ((FilterRuleRepository)Repository).GetFilterRuleControlNameForChild(filterType);
            return response;
        }

        /// <summary>
        /// Gets the <see cref="FilterRuleItemResponse">FilterRuleControlNameResponse</see> for the criteria.
        /// <param name="filterType">The filter type value</param>
        /// <param name="id">The indentifier of the element i.e. costcode Id.</param>
        /// <param name="isParent">Whether the item is a parent and therefore the Id should be returned as well</param>
        /// <param name="useDescription">Whether the item description should be used, or the item name</param>
        /// </summary>
        [HttpGet, Route("GetParentOrChildItem")]
        [AuthAudit(SpendManagementElement.Expenses, AccessRoleType.View)]
        public FilterRuleItemResponse GetParentOrChildItem(FilterType filterType, int id, bool isParent, bool useDescription)
        {
            var response = InitialiseResponse<FilterRuleItemResponse>();
            response.Item = ((FilterRuleRepository)Repository).GetFilterRuleItemForParentOrChild(filterType, id, isParent,useDescription);
            return response;
        }
    }
}
