namespace SpendManagementApi.Models.Responses
{
    using System.Collections.Generic;
    using Common;
    using Types;

    /// <summary>
    /// A response containing a list of <see cref="FilterRule">FilterRule</see>s.
    /// </summary>
    public class GetFilterRulesResponse : GetApiResponse<FilterRule>
    {
        /// <summary>
        /// Creates a new GetFilterRulesResponse.
        /// </summary>
        public GetFilterRulesResponse()
        {
            List = new List<FilterRule>();
        }
    }

    /// <summary>
    /// A response containing a particular <see cref="FilterRule">FilterRule</see>.
    /// </summary>
    public class FilterRuleResponse : ApiResponse<FilterRule>
    {
    }

    /// <summary>
    /// A response containing a particular <see cref="FilterRuleListItem">FilterRuleControlName</see>.
    /// </summary>
    public class GetFilterRuleItemsResponse : GetApiResponse<FilterRuleListItem>
    {
        /// <summary>
        /// Creates a new GetFilterRuleItemsResponse.
        /// </summary>
        public GetFilterRuleItemsResponse()
        {
            List = new List<FilterRuleListItem>();
        }
    }

    /// <summary>
    /// A response containing a particular <see cref="FilterRuleItem">FilterRuleControlName</see>.
    /// </summary>
    public class FilterRuleItemResponse : ApiResponse<FilterRuleItem>
    {
    }
}
