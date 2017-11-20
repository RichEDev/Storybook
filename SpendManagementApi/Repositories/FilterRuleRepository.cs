namespace SpendManagementApi.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.UI.WebControls;
    using Models.Responses;
    using Utilities;
    using Interfaces;
    using Models.Types;
    using Spend_Management;
    using SML = SpendManagementLibrary;
    using Common.Enums;
    using Models.Common;

    /// <summary>
    /// FilterRuleRepository manages data access for FilterRules.
    /// </summary>
    internal class FilterRuleRepository : BaseRepository<FilterRule>, ISupportsActionContext
    {

        private cFilterRules _cFilterRules;

        /// <summary>
        /// Creates a new FilterRuleRepository with the passed in user.
        /// </summary>
        /// <param name="user">The current user.</param>
        /// <param name="actionContext">An implementation of ISupportsActionContext</param>
        public FilterRuleRepository(ICurrentUser user, IActionContext actionContext)
            : base(user, actionContext, x => x.FilterId, null)
        {
            _cFilterRules = this.ActionContext.FilterRules;
        }

        /// <summary>
        /// Gets all the FilterRules within the system.
        /// </summary>
        /// <returns></returns>
        public override IList<FilterRule> GetAll()
        {
            var response = new GetFilterRulesResponse();
            return this.GetFilterRulesByType(FilterType.All, response);
        }

        /// <summary>
        /// Gets a single FilterRule by its id.
        /// </summary>
        /// <param name="id">The Id of the FilterRule to get.</param>
        /// <returns>The FilterRule.</returns>
        public override FilterRule Get(int id)
        {
            SML.cFilterRule cFilterRule = _cFilterRules.GetFilterRuleById(id);

            return cFilterRule.Cast<FilterRule>(User.AccountID, ActionContext);
        }

        /// <summary>
        /// Gets a List of <see cref="FilterRule">FilterRule</see> for the specified Filter Type
        /// </summary>
        /// <param name="filterType">The filter type</param>
        /// <param name="response">The response</param>
        /// <returns>The <see cref="GetFilterRulesResponse"></see> GetFilterRulesResponse</returns>
        public List<FilterRule> GetFilterRulesByType(FilterType filterType, GetFilterRulesResponse response)
        {
            Dictionary<int, SML.cFilterRule> filterRules = _cFilterRules.GetFilterRulesByType((SpendManagementLibrary.FilterType)filterType);

            if (filterRules == null)
            {
                throw new ApiException(string.Format(ApiResources.ApiErrorFilterRulesNotFound, "Filter Rules"), string.Format(ApiResources.ApiErrorFilterRulesNotFound, "Filter Rules"));
            }

            response.List = filterRules.Cast<List<FilterRule>>(ActionContext).ToList();
            return response.List;
        }

        /// <summary>
        /// Gets a List of <see cref="FilterRuleListItem">FilterRuleItem</see> for the specified Filter Type
        /// </summary>
        /// <param name="filterType">The filter type value</param>
        /// <param name="useDescription">Whether the item description should be used, or the item name</param>
        /// <param name="response">The response</param>
        /// <returns>The <see cref="GetFilterRulesResponse"></see> GetFilterRulesResponse</returns>
        public List<FilterRuleListItem> GetItems(FilterType filterType, bool useDescription, GetFilterRuleItemsResponse response)
        {
            List<ListItem> filterRules = _cFilterRules.GetItems((SpendManagementLibrary.FilterType)filterType, useDescription);

            if (filterRules == null)
            {
                throw new ApiException(string.Format(ApiResources.ApiErrorFilterRulesNotFound, "Filter Rules"), string.Format(ApiResources.ApiErrorFilterRulesNotFound, "Filter Rules"));
            }

            response.List = filterRules.Cast<List<FilterRuleListItem>>().ToList();
            return response.List;
        }

        /// <summary>
        /// Gets the <see cref="FilterRuleItem">FilterRuleItem</see> for the specified Filter Type
        /// </summary>
        /// <param name="filterType">The filter type value</param>
        /// <returns>The <see cref="FilterRuleItem"></see> FilterRuleItem</returns>
        public FilterRuleItem GetFilterRuleControlNameForChild(FilterType filterType)
        {
            string controlName = _cFilterRules.getChildTargetControl((SpendManagementLibrary.FilterType)filterType);
            return controlName.Cast<FilterRuleItem>();
        }

        /// <summary>
        /// Gets the <see cref="FilterRuleItem">FilterRuleItem</see> for the specified Filter Type
        /// </summary>
        /// <param name="filterType">The filter type value</param>
        /// <param name="id">The indentifier of the element i.e. costcode Id.</param>
        /// <param name="isParent">Whether the item is a parent and therefore the Id should be returned as well</param>
        /// <param name="useDescription">Whether the item description should be used, or the item name</param>
        /// <returns>The <see cref="FilterRuleItem"></see> FilterRuleItem</returns>
        public FilterRuleItem GetFilterRuleItemForParentOrChild(FilterType filterType, int id, bool isParent, bool useDescription)
        {
            string item = _cFilterRules.GetParentOrChildItem((SpendManagementLibrary.FilterType)filterType, id, isParent, useDescription);
            return item.Cast<FilterRuleItem>();
        }

        /// <summary>
        /// Adds a FilterRule.
        /// </summary>
        /// <param name="item">The FilterRule to add.</param>
        /// <returns></returns>
        public override FilterRule Add(FilterRule item)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Updates a FilterRule.
        /// </summary>
        /// <param name="item">The item to update.</param>
        /// <returns>The updated FilterRule.</returns>
        public override FilterRule Update(FilterRule item)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Deletes a FilterRule, given it's ID.
        /// </summary>
        /// <param name="id">The Id of the FilterRule to delete.</param>
        /// <returns>The deleted FilterRule.</returns>
        public override FilterRule Delete(int id)
        {
            throw new NotImplementedException();
        }

    }
}