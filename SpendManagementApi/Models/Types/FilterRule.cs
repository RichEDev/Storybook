namespace SpendManagementApi.Models.Types
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.UI.WebControls;
    using SpendManagementApi.Common.Enums;
    using Interfaces;

    using SML = SpendManagementLibrary;

    /// <summary>
    /// A class to deal with information relating to a FilterRule
    /// </summary>
    public class FilterRule : BaseExternalType, IApiFrontForDbObject<SML.cFilterRule, FilterRule>
    {
        /// <summary>
        /// The filter Id
        /// </summary>
        public int FilterId { get; set; }

        /// <summary>
        /// The Filter Type Parent
        /// </summary>     
        public FilterType Parent { get; set; }

        /// <summary>
        /// The Filter Type Child
        /// </summary> 
        public FilterType Child { get; set; }

        /// <summary>
        /// The List of Rule Values that apply to this Filter Rule
        /// </summary>  
        public Dictionary<int, FilterRuleValues> RuleValues { get; set; }

        /// <summary>
        /// The Parent Udf Id
        /// </summary>    
        public int ParentUdfId { get; set; }

        /// <summary>
        /// The Child Udf Id
        /// </summary>          
        public int ChildUdfId { get; set; }

        /// <summary>
        /// Is enabled?
        /// </summary>   
        public bool Enabled { get; set; }

        /// <summary>
        /// The created on date
        /// </summary> 
        public new DateTime CreatedOn { get; set; }

        /// <summary>
        /// The id of the person who created the filter rule
        /// </summary>  
        public int CreatedBy { get; set; }

        public FilterRule From(SML.cFilterRule dbType, IActionContext actionContext)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Converts from the API type to the DAL type.
        /// </summary>
        /// <returns>The DAL type.</returns>
        public SML.cFilterRule To(IActionContext actionContext)
        {  
            return new SML.cFilterRule(FilterId, (SML.FilterType)Parent, (SML.FilterType)Child, new Dictionary<int, SML.cFilterRuleValue>(), ParentUdfId, ChildUdfId, Enabled, CreatedOn, CreatedBy);
        }

    }

    internal static class FilterRuleByFilterTypeExtension
    {
        public static TResult Cast<TResult>(this Dictionary<int, SML.cFilterRule> expenseSubCategoryNames, IActionContext actionContext)
              where TResult : List<FilterRule>, new()
        {
            List<FilterRule> filterRules = new List<FilterRule>();

            foreach (SML.cFilterRule rule in expenseSubCategoryNames.Values)
            {
                var ruleValues = rule.rulevals;
                var list = new Dictionary<int, FilterRuleValues>();

                foreach (KeyValuePair<int, SML.cFilterRuleValue> ruleValue in ruleValues)
                {             
                    list.Add(ruleValue.Key, new FilterRuleValues().From(ruleValue.Value, actionContext));
                }

                var filterRule = new FilterRule
                             {
                                 FilterId = rule.filterid,
                                 Child = (FilterType)rule.child,
                                 ChildUdfId = rule.childuserdefineid,
                                 Parent = (FilterType)rule.parent,
                                 ParentUdfId = rule.paruserdefineid,
                                 Enabled = rule.enabled,
                                 CreatedOn = rule.createdon,
                                 CreatedBy = rule.createdby,                            
                                 RuleValues = list
                             };

                filterRules.Add(filterRule);
            }

            return (TResult)filterRules;
        }
    }

    internal static class FilterRuleExtension
    {
        internal static TRes Cast<TRes>(
        this SML.cFilterRule cFilterRule,
        int accountId,
        IActionContext actionContext) where TRes : FilterRule, new()
        {
            if (cFilterRule == null)
            {
                return null;
            }

            var ruleValues = cFilterRule.rulevals;

            var filterRuleValues = new Dictionary<int, FilterRuleValues>();

            foreach (KeyValuePair<int, SML.cFilterRuleValue> ruleValue in ruleValues)
            {
             
                filterRuleValues.Add(ruleValue.Key, new FilterRuleValues().From(ruleValue.Value, actionContext));
            }

            return new TRes
            {
                FilterId = cFilterRule.filterid,
                Child = (FilterType)cFilterRule.parent,
                ChildUdfId = cFilterRule.childuserdefineid,
                RuleValues = filterRuleValues,
                Parent = (FilterType)cFilterRule.parent,
                Enabled = cFilterRule.enabled,
                ParentUdfId = cFilterRule.paruserdefineid,
                CreatedOn = cFilterRule.createdon,
                CreatedBy = cFilterRule.createdby
            };

        }
    }

    /// <summary>
    /// A class to deal with the ListItem information of a filter rule
    /// </summary>
    public class FilterRuleListItem : BaseExternalType
    {
        /// <summary>
        /// The item description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The identifier
        /// </summary>
        public string Identifier { get; set; }
    }

    internal static class FilterRuleItemExtension
    {
        public static TResult Cast<TResult>(this List<ListItem> expenseSubCategoryNames)
            where TResult : List<FilterRuleListItem>, new()
        {
            List<FilterRuleListItem> filterRuleItems =
                expenseSubCategoryNames.Select(
                    item => new FilterRuleListItem { Description = item.Text, Identifier = item.Value }).ToList();
            return (TResult)filterRuleItems;
        }
    }

    /// <summary>
    /// A class to deal with a filter rule Item. This could be a control name or details or element details
    /// </summary>
    public class FilterRuleItem : BaseExternalType
    {
        /// <summary>
        /// The Item details
        /// </summary>
        public string Item { get; set; }
    }

    internal static class FilterRuleControlNameExtension
    {
        public static TResult Cast<TResult>(this string originalItem)
            where TResult : FilterRuleItem, new()
        {
            return (TResult)new FilterRuleItem { Item = originalItem };
        }
    }
}