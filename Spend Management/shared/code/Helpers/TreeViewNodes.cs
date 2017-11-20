using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using SpendManagementHelpers.TreeControl;
using SpendManagementLibrary;
using SpendManagementLibrary.Definitions.JoinVia;

namespace Spend_Management.shared.code.Helpers
{
    using System.Web;

    /// <summary>
    ///     Help class for Tree View Nodes.
    /// </summary>
    public static class TreeViewNodes
    {
        #region TreeNodes

        /// <summary>
        ///     Create a JavascriptTreeViewNode.
        /// </summary>
        /// <param name="prefixId">the prefix to add to the ID</param>
        /// <param name="field"></param>
        /// <param name="preCrumb">The text to add to the front of the breadcrumb text.</param>
        /// <param name="crumbDescription">Breadcrumb description.</param>
        /// <returns></returns>
        public static JavascriptTreeData.JavascriptTreeNode CreateJavascriptNode(string prefixId, cField field,
            string preCrumb,
            string crumbDescription)
        {
            string guidPrefix;
            JavascriptTreeData.JavascriptTreeNode node = NewJavascriptTreeNode();

            if (field.IsForeignKey && field.GetRelatedTable() != null && field.FieldType != "CL" &&
                !String.IsNullOrEmpty(field.GetRelatedTable().Description))
            {
                guidPrefix = ClosedNode(node);
                node.data = field.Description.ToLower().EndsWith("id")
                    ? String.Format("{0}({1})", field.GetRelatedTable().Description, field.Description)
                    : field.Description;
            }
            else
            {
                guidPrefix = NormalNode(field, node);
            }

            node.attr.id = prefixId + "_" + guidPrefix + field.FieldID;
            node.attr.internalId = node.attr.id;
            node.attr.crumbs = HttpUtility.HtmlEncode(preCrumb + crumbDescription);
            node.attr.fieldid = field.FieldID.ToString();
            node.attr.fieldtype = field.FieldType;
            node.attr.comment = HttpUtility.HtmlEncode(field.Comment);
            return node;
        }


        /// <summary>
        ///     Create a JavascriptTreeViewNode.
        /// </summary>
        /// <param name="entityField"></param>
        /// <param name="okToAdd"></param>
        /// <param name="includeAllowReportOn"></param>
        /// <returns></returns>
        public static JavascriptTreeData.JavascriptTreeNode CreateJavascriptNode(cField entityField,
            bool includeAllowReportOn = false)
        {
            JavascriptTreeData.JavascriptTreeNode node = NewJavascriptTreeNode();
            // g for Group - table join, k for node linK - foreign key join, n for node - field
            string guidPrefix;

            if (entityField.IsForeignKey && entityField.FieldType != "AT")
            {
                guidPrefix = ClosedNode(node);

                cTable table = entityField.GetRelatedTable();
                if (table != null && (!includeAllowReportOn || (includeAllowReportOn && table.AllowReportOn)))
                {
                    if (entityField.Description.ToLower().EndsWith("id") && !string.IsNullOrEmpty(entityField.Comment))
                    {
                        node.data = entityField.Comment;
                    }
                    else if (string.IsNullOrEmpty(table.Description) || !entityField.Description.ToLower().EndsWith("id"))
                    {
                        node.data = entityField.Description;
                    }
                    else
                    {
                        node.data = string.IsNullOrEmpty(table.Description) ? table.TableName : table.Description;
                    }
                }
                else
                {
                    return null;
                }
            }
            else
            {
                guidPrefix = NormalNode(entityField, node);
            }

            node.attr.id = guidPrefix + entityField.FieldID;
            node.attr.internalId = node.attr.id;
            node.attr.crumbs = string.Empty;
            node.attr.fieldid = entityField.FieldID.ToString();
            node.attr.fieldtype = entityField.FieldType;
            node.attr.comment = HttpUtility.HtmlEncode(entityField.Comment);
            return node;
        }

        /// <summary>
        ///     Create a JavascriptTreeViewNode.
        /// </summary>
        /// <param name="customEntityAttribute"></param>
        /// <param name="sourcePrefix"></param>
        /// <param name="relationshipAttribute"></param>
        /// <returns></returns>
        public static JavascriptTreeData.JavascriptTreeNode CreateJavascriptNode(cAttribute customEntityAttribute,
            string sourcePrefix = null,
            cAttribute relationshipAttribute = null)
        {
            string guidPrefix;
            JavascriptTreeData.JavascriptTreeNode node = NewJavascriptTreeNode();

            if (customEntityAttribute.GetType() == typeof (cManyToOneRelationship) ||
                customEntityAttribute.GetType() == typeof (cOneToManyRelationship) )
            {
                guidPrefix = ClosedNode(node);
            }
            else
            {
                guidPrefix = NormalNode(node);
            }

            if (relationshipAttribute == null)
            {
                node.attr.id = guidPrefix + customEntityAttribute.fieldid;
                node.attr.internalId = node.attr.id;
                node.attr.crumbs = String.Empty;
            }
            else
            {
                node.attr.id = sourcePrefix + "_" + guidPrefix + customEntityAttribute.fieldid;
                node.attr.internalId = node.attr.id;
                node.attr.crumbs = HttpUtility.HtmlEncode(relationshipAttribute.displayname);
                // crumbs needs the initial table name to prefix as effective already expanded from a n:1 field
            }

            node.attr.fieldid = customEntityAttribute.fieldid.ToString();
            node.data = customEntityAttribute.displayname;
            return node;
        }

        /// <summary>
        ///     Create a JavascriptTreeViewNode.
        /// </summary>
        /// <param name="field"></param>
        /// <param name="sourcePrefix"></param>
        /// <param name="relationshipAttribute"></param>
        /// <returns></returns>
        public static JavascriptTreeData.JavascriptTreeNode CreateJavascriptNode(cField field, string sourcePrefix,
            cAttribute relationshipAttribute)
        {
            string guidPrefix;
            JavascriptTreeData.JavascriptTreeNode node = NewJavascriptTreeNode();

            if (field.IsForeignKey && field.GetRelatedTable() != null)
            {
                guidPrefix = ClosedNode(node);
            }
            else
            {
                guidPrefix = NormalNode(field, node);
            }

            node.attr.id = sourcePrefix + "_" + guidPrefix + field.FieldID;
            node.attr.internalId = node.attr.id;
            node.attr.crumbs = HttpUtility.HtmlEncode(relationshipAttribute.displayname);
            // crumbs needs the initial table name to prefix as effective already expanded from a n:1 field
            node.attr.fieldid = field.FieldID.ToString();
            node.data = field.Description;
            return node;
        }

        /// <summary>
        ///     Create a JavascriptTreeViewNode.
        /// </summary>
        /// <param name="fieldFilter"></param>
        /// <param name="duplicateFilters"></param>
        /// <param name="fields"></param>
        /// <param name="currentUser"></param>
        /// <returns></returns>
        public static JavascriptTreeData.JavascriptTreeNode CreateJavascriptNode(FieldFilter fieldFilter,
            List<string> duplicateFilters, cFields fields, ICurrentUser currentUser)
        {
            StringBuilder nodeID;
            Dictionary<string, object> metadata;
            JavascriptTreeData.JavascriptTreeNode node = NewJavascriptTreeNode();
            string dupeKey = new StringBuilder(fieldFilter.Field.FieldID.ToString()).Append("_")
                .Append(fieldFilter.JoinVia != null ? fieldFilter.JoinVia.JoinViaID.ToString(CultureInfo.InvariantCulture) : "0")
                .ToString();
            int dupes = (from x in duplicateFilters where x == dupeKey select x).Count();
            duplicateFilters.Add(dupeKey);

            node.attr.rel = "node";
            nodeID = new StringBuilder();
            for (int i = 0; i <= dupes; i++)
            {
                nodeID.Append("copy_");
            }

            if (fieldFilter.JoinVia != null)
            {
                nodeID = FormatNodeIdFromJoinViaParts(fieldFilter.JoinVia, nodeID);

                node.attr.crumbs = HttpUtility.HtmlEncode(fieldFilter.JoinVia.Description);
                node.attr.joinviaid = fieldFilter.JoinVia.JoinViaID;
            }

            bool isListItem = false;
            bool genListMe = false;
            bool myHierarchy = false;
            bool isParentFilter = fieldFilter.IsParentFilter;
            bool myCostCodeHierarchy = false;
            string firstListItemText = String.Empty;
            string firstListItemID = String.Empty;

            if (fieldFilter.Field.ValueList || fieldFilter.Field.GenList || fieldFilter.Field.FieldType == "CL")
            {
                isListItem = true;

                if (fieldFilter.Field.GenList && fieldFilter.ValueOne == "@ME")
                {
                    genListMe = true;
                }
                else if (fieldFilter.Field.GenList && fieldFilter.ValueOne == "@MY_HIERARCHY")
                {
                    myHierarchy = true;
                }
                else if (fieldFilter.Field.GenList && fieldFilter.ValueOne == "@MY_COST_CODE_HIERARCHY")
                {
                    myCostCodeHierarchy = true;
                }
                else
                {
                    if (fieldFilter.ValueOne.IndexOf(',') == -1)
                    {
                        firstListItemID = fieldFilter.ValueOne;
                    }
                    else
                    {
                        firstListItemID = fieldFilter.ValueOne.Substring(0, fieldFilter.ValueOne.IndexOf(','));
                    }

                    if (firstListItemID.Length > 0)
                    {
                        if (fieldFilter.Field.FieldType == "CL")
                        {
                            firstListItemText =
                                Currencies.GetCurrencyList(currentUser.AccountID, currentUser.CurrentSubAccountId)
                                    .Find(x => x.Value == firstListItemID)
                                    .Text;
                        }
                        else if (fieldFilter.Field.ValueList)
                        {
                            cField field = fields.GetFieldByID(fieldFilter.Field.FieldID);

                            firstListItemText =
                                field.ListItems.Where(x => x.Key.ToString() == firstListItemID).Select(
                                    x => x.Value).FirstOrDefault();
                        }
                        else if (fieldFilter.Field.GenList)
                        {
                            firstListItemText = isParentFilter ? fieldFilter.ValueOne :
                                FieldFilters.GetGenListItemsForField(fieldFilter.Field.FieldID, currentUser).Find(
                                    x => x.Value == firstListItemID).Text;
                        }
                        else
                        {
                            foreach (var listItem in fieldFilter.Field.ListItems)
                            {
                                if (listItem.Key.ToString() == firstListItemID)
                                {
                                    firstListItemText = listItem.Value;
                                }
                            }
                        }
                    }
                }
            }

            string conditionTypeText = FieldFilters.GetConditionTypeTextForFilter(fieldFilter, currentUser.AccountID);

            metadata = CreateCriteriaMetadata(
                genListMe ? ConditionType.AtMe : (myHierarchy ? ConditionType.AtMyHierarchy :(isParentFilter ? ConditionType.Equals : fieldFilter.Conditiontype)),
                genListMe ? "@ME" : (myHierarchy ? "@MY_HIERARCHY" :(fieldFilter.ValueOne)),
                fieldFilter.ValueTwo,
                genListMe || myHierarchy || isParentFilter ? ConditionType.Equals.ToString() : conditionTypeText,
                fieldFilter.Field.FieldType,
                isListItem,
                firstListItemText,
                false,
                fieldFilter.Joiner,
                0,
                fieldFilter.IsParentFilter);

            node.attr.id = nodeID.Append("n").Append(fieldFilter.Field.FieldID).ToString();
            node.attr.internalId = node.attr.id;
            node.attr.fieldid = fieldFilter.Field.FieldID.ToString();
            node.data = fieldFilter.Field.Description;
            node.attr.fieldtype = fieldFilter.Field.FieldType;
            node.attr.comment = HttpUtility.HtmlEncode(fieldFilter.Field.Comment);
            node.metadata = metadata;
            return node;
        }

        /// <summary>
        /// Method that returns a KeyValuePair which contains the numeric value and the display name of a filter condition. 
        /// If in the future more information is required the use of an object would be more preferable
        /// </summary>
        /// <param name="fieldFilter">
        /// The field filter.
        /// </param>
        /// <param name="genListMe">
        /// The gen list me.
        /// </param>
        /// <param name="myHierarchy">
        /// The my hierarchy.
        /// </param>
        /// <param name="myCostCodeHierarchy">
        /// The my cost code hierarchy.
        /// </param>
        /// <returns>
        /// The <see cref="KeyValuePair"/> of the filter condition.
        /// </returns>
        private static KeyValuePair<int, string> GetConditionTypeAndValue(FieldFilter fieldFilter, bool genListMe, bool myHierarchy, bool myCostCodeHierarchy)
        {
            int key;
            string value;

            if (genListMe)
            {
                key = (int)ConditionType.AtMe;
                value = "@ME";
            }
            else if (myHierarchy)
            {
                key = (int)ConditionType.AtMyHierarchy;
                value = "@MY_HIERARCHY";
            }
            else if (myCostCodeHierarchy)
            {
                key = (int)ConditionType.AtMyCostCodeHierarchy;
                value = "@MY_COST_CODE_HIERARCHY";
            }
            else
            {
                key = (int)fieldFilter.Conditiontype;
                value = fieldFilter.ValueOne;
            }

            return new KeyValuePair<int, string>(key, value);
        }

        /// <summary>
        ///     Create a JavascriptTreeViewNode.
        /// </summary>
        /// <param name="ldf"></param>
        /// <param name="duplicateLookupDisplayFields"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        public static JavascriptTreeData.JavascriptTreeNode CreateLookupDisplayFieldJavascriptNode(
            LookupDisplayField ldf, List<string> duplicateLookupDisplayFields, cFields fields)
        {
            JavascriptTreeData.JavascriptTreeNode node = NewJavascriptTreeNode();
            string dupeKey = new StringBuilder(ldf.TriggerDisplayFieldId.ToString()).Append("_")
                .Append(ldf.TriggerJoinViaId != null ? ldf.TriggerJoinViaId.ToString() : "0").ToString();
            int dupes = (from x in duplicateLookupDisplayFields where x == dupeKey select x).Count();
            duplicateLookupDisplayFields.Add(dupeKey);

            node.attr.rel = "node";
            var nodeID = new StringBuilder();
            for (int i = 0; i <= dupes; i++)
            {
                nodeID.Append("copy_");
            }

            if (ldf.TriggerJoinVia != null)
            {
                node.attr.crumbs = HttpUtility.HtmlEncode(ldf.TriggerJoinVia.Description);
            }

            if (ldf.TriggerJoinViaId != null)
            {
                node.attr.joinviaid = (int) ldf.TriggerJoinViaId;
            }
            if (ldf.TriggerJoinVia != null)
            {
                nodeID = FormatNodeIdFromJoinViaParts(ldf.TriggerJoinVia, nodeID);
            }

            var metadata = new Dictionary<string, object>
            {
                {"fieldType", ldf.fieldtype},
                {"isListItem", false}
            };

            node.attr.id = nodeID.Append("n").Append(ldf.TriggerDisplayFieldId).ToString();
            node.attr.internalId = node.attr.id;
            node.attr.fieldid = ldf.TriggerDisplayFieldId.ToString();
            cField currentField = fields.GetFieldByID((Guid) ldf.TriggerDisplayFieldId);
            node.data = currentField.Description;
            node.attr.fieldtype = currentField.FieldType;
            node.attr.comment = HttpUtility.HtmlEncode(currentField.Comment);
            node.metadata = metadata;
            return node;
        }

        /// <summary>
        /// Create a JavascriptTreeViewNode for a custom entity filter field.
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="duplicateFilters"></param>
        /// <param name="user"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        public static JavascriptTreeData.JavascriptTreeNode CreateCustomEntityFilterJavascriptNode(
            cReportCriterion filter, List<string> duplicateFilters, CurrentUser user,
            cFields fields)
        {
            JavascriptTreeData.JavascriptTreeNode node = NewJavascriptTreeNode();
            string dupeKey =
                new StringBuilder(filter.field.FieldID.ToString()).Append("_")
                    .Append(
                        filter.JoinVia != null
                            ? filter.JoinVia.JoinViaID
                                .ToString(
                                    CultureInfo.InvariantCulture)
                            : "0")
                    .ToString();
            int dupes = (from x in duplicateFilters where x == dupeKey select x).Count();
            duplicateFilters.Add(dupeKey);

            node.attr.rel = "node";
            var nodeID = new StringBuilder();
            for (int i = 0; i <= dupes; i++)
            {
                nodeID.Append("copy_");
            }

            if (filter.JoinVia != null)
            {
                nodeID = FormatNodeIdFromJoinViaParts(filter.JoinVia, nodeID);

                node.attr.crumbs = HttpUtility.HtmlEncode(filter.JoinVia.Description);
                node.attr.joinviaid = filter.JoinVia.JoinViaID;
            }

            bool nodeIsListItem = false;
            bool genListMe = false;
            bool usingMyHeirachy = false;
            bool usingMyCostCodeHierarchy = false;
            string firstListItemText = String.Empty;

            if (filter.condition != ConditionType.ContainsData  && !filter.runtime && filter.condition != ConditionType.DoesNotContainData && ( filter.field.ValueList || filter.field.GenList || filter.field.FieldType == "CL"))
            {
                nodeIsListItem = true;
                if (filter.field.GenList && filter.value1.ToString() == "@ME")
                {
                    genListMe = true;
                }
                else if (filter.field.GenList && filter.value1.ToString() == "@MY_HIERARCHY")
                {
                    usingMyHeirachy = true;
                }
                else if (filter.field.GenList && filter.value1[0].ToString() == "@MY_COST_CODE_HIERARCHY")
                {
                    usingMyCostCodeHierarchy = true;
                }
                else
                {
                    if (filter.value1 != null && filter.value1[0] == null)
                    {
                        filter.value1[0] = string.Empty;
                    }

                    if (filter.value2 != null && filter.value2[0] == null)
                    {
                        filter.value2[0] = string.Empty;
                    }

                    string firstListItemID;
                    if (filter.value1 != null && filter.value1[0].ToString().IndexOf(',') == -1)
                    {
                        firstListItemID = filter.value1[0].ToString();
                    }
                    else
                    {
                        if (filter.value1 != null)
                        {
                            firstListItemID = filter.value1[0].ToString()
                                .Substring(
                                    0, filter.value1[0].ToString().IndexOf(','));
                        }
                        else
                        {
                            firstListItemID = string.Empty;
                        }
                    }

                    if (firstListItemID.Length > 0)
                    {
                        if (filter.field.FieldType == "CL")
                        {
                            firstListItemText =
                                Currencies.GetCurrencyList(user.AccountID, user.CurrentSubAccountId)
                                    .Find(x => x.Value == firstListItemID)
                                    .Text;
                        }
                        else if (filter.field.ValueList)
                        {
                            cField field = fields.GetFieldByID(filter.field.FieldID);

                            firstListItemText =
                                field.ListItems.Where(x => x.Key.ToString() == firstListItemID)
                                    .Select(x => x.Value)
                                    .FirstOrDefault();
                        }
                        else if (filter.field.GenList)
                        {
                            firstListItemText =
                                FieldFilters.GetGenListItemsForField(filter.field.FieldID, user)
                                    .Find(x => x.Value == firstListItemID)
                                    .Text;
                        }
                        else
                        {
                            foreach (var listItem in filter.field.ListItems)
                            {
                                if (listItem.Key.ToString() == firstListItemID)
                                {
                                    firstListItemText = listItem.Value;
                                }
                            }
                        }
                    }
                }
            }

            var value1 = new StringBuilder();
            if (filter.value1 != null)
            {
                for (int index = 0; index < filter.value1.Length; index++)
                {
                    object obj = filter.value1[index];
                    if (obj != null)
                    {
                        obj = Formatting.FormatDateValue(filter, obj);

                        value1.Append(obj);
                        if (index < filter.value1.Length - 1)
                        {
                            value1.Append(",");
                        }
                    }
                }
            }

            var value2 = new StringBuilder();
            if (filter.value2 != null)
            {
                for (int index = 0; index < filter.value2.Length; index++)
                {
                    object obj = filter.value2[index];
                    if (obj != null)
                    {
                        obj = Formatting.FormatDateValue(filter, obj);

                        value2.Append(obj);
                        if (index < filter.value2.Length - 1)
                        {
                            value2.Append(",");
                        }
                    }
                }
            }

            var fieldFilter = new FieldFilter(
                filter.field, filter.condition, value1.ToString(), value2.ToString(), 0, filter.JoinVia);

            string conditionTypeText = FieldFilters.GetConditionTypeTextForFilter(
                fieldFilter, user.AccountID);

            var metadata = CreateCriteriaMetadata(
                genListMe ? ConditionType.AtMe : (usingMyHeirachy ? ConditionType.AtMyHierarchy : fieldFilter.Conditiontype),
                genListMe ? "@ME" : (usingMyHeirachy ? "@MY_HIERARCHY" : fieldFilter.ValueOne),
                fieldFilter.ValueTwo,
                genListMe || usingMyHeirachy ? ConditionType.Equals.ToString() : conditionTypeText,
                fieldFilter.Field.FieldType,
                nodeIsListItem,
                firstListItemText,
                filter.runtime,
                filter.joiner,
                filter.groupnumber,
                fieldFilter.IsParentFilter);

            node.attr.id = nodeID.Append("n").Append(filter.field.FieldID).ToString();
            node.attr.internalId = node.attr.id;
            node.attr.fieldid = filter.field.FieldID.ToString();
            node.data = filter.field.Description;
            node.attr.fieldtype = filter.field.FieldType;
            node.attr.comment = HttpUtility.HtmlEncode(fieldFilter.Field.Comment);
            node.metadata = metadata;
            return node;
        }

        /// <summary>
        /// The create Filter criteria for a field based on the filter settings.
        /// </summary>
        /// <param name="conditionType">
        /// The condition type to filter
        /// </param>
        /// <param name="criterionOne">
        /// The criterion one.
        /// </param>
        /// <param name="criterionTwo">
        /// The criterion two.
        /// </param>
        /// <param name="conditionTypeText">
        /// The condition type text.
        /// </param>
        /// <param name="fieldType">
        /// The field type.
        /// </param>
        /// <param name="isListItem">
        /// The is list item.
        /// </param>
        /// <param name="firstListItemText">
        /// The first list item text.
        /// </param>
        /// <param name="runtime">
        /// The runtime.
        /// </param>
        /// <param name="conditionJoiner">
        /// The condition joiner.
        /// </param>
        /// <param name="group">
        /// The group.
        /// </param>
        /// <param name="isParentFilter">
        /// Determine if the Filter type is Parent filter.
        /// </param>
        /// <returns>
        /// The <see>
        /// <cref>Dictionary</cref>
        /// </see>
        /// Criteria dictionary
        /// </returns>
        private static Dictionary<string, object> CreateCriteriaMetadata(ConditionType conditionType, string criterionOne, string criterionTwo, string conditionTypeText, string fieldType, bool isListItem, string firstListItemText, bool runtime, ConditionJoiner conditionJoiner, int group, bool isParentFilter)
        {
            return new Dictionary<string, object>
            {
                { "conditionType", conditionType },
                { "criterionOne", criterionOne },
                { "criterionTwo", criterionTwo },
                { "conditionTypeText", conditionTypeText },
                { "fieldType", fieldType },
                { "isListItem", isListItem },
                { "firstListItemText", firstListItemText },
                { "runtime", runtime },
                { "conditionJoiner", conditionJoiner },
                { "group", group },
                { "isParentFilter", isParentFilter }
            };
        }

        /// <summary>
        ///     Create a JavascriptTreeViewNode for a custom entity view field.
        /// </summary>
        /// <param name="customEntityViewField"></param>
        /// <param name="baseTableId">The base table used for related table joins.</param>
        /// <returns></returns>
        public static JavascriptTreeData.JavascriptTreeNode CreateCustomEntityViewFieldJavascriptNode(cCustomEntityViewField customEntityViewField, Guid baseTableId)
        {
            return CreateFilterNode(customEntityViewField.JoinVia, customEntityViewField.Field, baseTableId);
        }

        /// <summary>
        /// Create a Group Node for Userdefined fields.
        /// </summary>
        /// <param name="prefixID"></param>
        /// <param name="field"></param>
        /// <param name="preCrumb"></param>
        /// <returns></returns>
        public static JavascriptTreeData.JavascriptTreeNode CreateGroupJavascriptNode(string prefixID, cField field,
            string preCrumb)
        {
            JavascriptTreeData.JavascriptTreeNode node = NewJavascriptTreeNode();
            GroupNode(node, prefixID, field.GetRelatedTable().UserDefinedTableID.ToString(), field.Description, preCrumb);
            return node;
        }

        /// <summary>
        /// Create a Group Node for Userdefined fields.
        /// </summary>
        /// <param name="relTable"></param>
        /// <returns></returns>
        public static JavascriptTreeData.JavascriptTreeNode CreateGroupJavascriptNode(cTable relTable)
        {
            JavascriptTreeData.JavascriptTreeNode node = NewJavascriptTreeNode();
            GroupNode(node, "", relTable.UserDefinedTableID.ToString(), "", "");
            return node;
        }

        /// <summary>
        /// Create a Node for a related table field.
        /// </summary>
        /// <param name="nodeId"></param>
        /// <param name="field"></param>
        /// <returns></returns>
        public static JavascriptTreeData.JavascriptTreeNode CreateRelatedTableJavascriptNode(string nodeId, cField field)
        {
            JavascriptTreeData.JavascriptTreeNode node = NewJavascriptTreeNode();
            cTable table = field.GetParentTable();
            if (table == null || string.IsNullOrEmpty(table.Description) || !table.AllowReportOn)
            {
                return node;
            }

            // g for Group - table join, k for node linK - foreign key join, n for node - field, x related table link
            var guidPrefix = "x";
            ClosedNode(node);

            var fieldComment = field.Comment;
            if (string.IsNullOrEmpty(fieldComment))
            {
                fieldComment = field.Description;
            }

            var tableDescriptionEmpty = string.IsNullOrEmpty(table.Description);

            node.data = 
                string.Format(
                "{0}{1}{2}{3}",
                table.Description,
                !tableDescriptionEmpty ? "(" : string.Empty,
                tableDescriptionEmpty ? string.Empty : fieldComment,
                !tableDescriptionEmpty ? ")" : string.Empty);
            node.attr.id = nodeId + "_" + guidPrefix + field.FieldID;
            node.attr.internalId = node.attr.id;
            node.attr.crumbs = HttpUtility.HtmlEncode(node.data.Replace(table.Description, string.Empty));
            node.attr.fieldid = field.FieldID.ToString();
            node.attr.fieldtype = field.FieldType;
            node.attr.comment = HttpUtility.HtmlEncode(field.Comment);
            node.metadata = SetMetadataDefaults();
            return node;
        }

        /// <summary>
        /// Create a JavascriptTreeViewNode for a report criteria column.
        /// </summary>
        /// <param name="standard">The report column to convert.</param>
        /// <param name="baseTableId">The guid of the base table - used for related table links</param>
        /// <returns></returns>
        internal static JavascriptTreeData.JavascriptTreeNode CreateReportCriteriaJavascriptNode(cStandardColumn standard, Guid baseTableId)
        {
            var node = CreateFilterNode(standard.JoinVia, standard.field, baseTableId);
            if (!string.IsNullOrEmpty(standard.DisplayName))
            {
                node.data = standard.DisplayName;
            }

            node.attr.columnid = standard.reportcolumnid.ToString();
            node.metadata = new Dictionary<string, object>
            {
                { "SortOrder", standard.sort.ToString() },
                { "Hidden", standard.hidden },
                { "Count", standard.funccount },
                { "Average", standard.funcavg },
                { "Sum", standard.funcsum },
                { "Max", standard.funcmax },
                { "Min", standard.funcmin },
                { "GroupBy", standard.groupby }
            };
            return node;
        }

        /// <summary>
        /// Create default properties for a normal node.
        /// </summary>
        /// <param name="field"></param>
        /// <param name="node"></param>
        /// <returns></returns>
        private static string NormalNode(cField field, JavascriptTreeData.JavascriptTreeNode node)
        {
            string guidPrefix = NormalNode(node);
            node.data = field.Description;
            return guidPrefix;
        }

        /// <summary>
        /// Create default properties for a normal node.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private static string NormalNode(JavascriptTreeData.JavascriptTreeNode node)
        {
            node.attr.rel = "node";
            const string guidPrefix = "n";
            return guidPrefix;
        }

        /// <summary>
        /// Create default properties for a close (expandable) node.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private static string ClosedNode(JavascriptTreeData.JavascriptTreeNode node)
        {
            node.state = "closed";
            node.attr.rel = "nodelink";
            const string guidPrefix = "k";

            return guidPrefix;
        }

        /// <summary>
        /// Create a filter node for reports and custom entity views.
        /// </summary>
        /// <param name="joinVia"></param>
        /// <param name="field"></param>
        /// <param name="baseTableId"></param>
        /// <returns></returns>
        private static JavascriptTreeData.JavascriptTreeNode CreateFilterNode(JoinVia joinVia, cField field, Guid baseTableId)
        {
            JavascriptTreeData.JavascriptTreeNode node = NewJavascriptTreeNode();
            node.attr.rel = "node";
            var nodeId = new StringBuilder("copy_");

            // g for Group - table join, k for node linK - foreign key join, n for node - field
            if (joinVia != null)
            {
                {
                    if (joinVia.JoinViaList.Count > 0 && joinVia.JoinViaList.Values[0].ViaIDType == JoinViaPart.IDType.RelatedTable)
                    {
                        nodeId.AppendFormat("{0}_", baseTableId);
                    }

                    nodeId = FormatNodeIdFromJoinViaParts(joinVia, nodeId);
                    node.attr.crumbs = HttpUtility.HtmlEncode(joinVia.Description);
                    node.attr.joinviaid = joinVia.JoinViaID;
                }
            }

            node.attr.id = nodeId.Append("n").Append(field.FieldID).ToString();
            node.attr.internalId = node.attr.id;
            node.attr.fieldid = field.FieldID.ToString();
            node.attr.fieldtype = field.FieldType;
            node.attr.comment = HttpUtility.HtmlEncode(field.Comment);
            node.data = field.Description;
            return node;
        }

        /// <summary>
        /// Create a group node, used for folders e.g. user defined fields.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="prefixId"></param>
        /// <param name="userDefinedTableId"></param>
        /// <param name="fieldDescription"></param>
        /// <param name="preCrumb"></param>
        private static void GroupNode(JavascriptTreeData.JavascriptTreeNode node, string prefixId,
            string userDefinedTableId, string fieldDescription, string preCrumb)
        {
            node.state = "closed";
            node.attr.rel = "group";
            const string guidPrefix = "g";
            if (string.IsNullOrEmpty(prefixId))
            {
                node.attr.id = guidPrefix + userDefinedTableId;
                node.attr.internalId = node.attr.id;
                node.attr.crumbs = string.Empty;
            }
            else
            {
                node.attr.id = prefixId + "_" + guidPrefix + userDefinedTableId;
                node.attr.internalId = node.attr.id;
                node.attr.crumbs = HttpUtility.HtmlEncode(preCrumb + fieldDescription);
            }

            node.attr.fieldid = userDefinedTableId;
            
            node.data = "User Defined Fields";
        }

        /// <summary>
        /// Create a new java script tree node with attributes.
        /// </summary>
        /// <returns></returns>
        private static JavascriptTreeData.JavascriptTreeNode NewJavascriptTreeNode()
        {
            var node = new JavascriptTreeData.JavascriptTreeNode
            {
                attr = new JavascriptTreeData.JavascriptTreeNode.LiAttributes()
            };
            return node;
        }

        #endregion

        #region Metadata

        /// <summary>
        ///     The set metadata defaults.
        /// </summary>
        /// <returns>
        ///     The
        ///     <see>
        ///         <cref>Dictionary</cref>
        ///     </see>
        ///     .
        /// </returns>
        public static Dictionary<string, object> SetMetadataDefaults()
        {
            return new Dictionary<string, object>
            {
                {"SortOrder", "[None]"},
                {"Hidden", false},
                {"Count", false},
                {"Average", false},
                {"Sum", false},
                {"Max", false},
                {"Min", false},
                {"GroupBy", false}
            };
        }

        /// <summary>
        ///     Get the named attribute value from the METADATA of the node.
        /// </summary>
        /// <param name="javascriptNode">
        ///     Tree node to extract data from.
        /// </param>
        /// <param name="key">
        ///     The named attribute to extract.
        /// </param>
        /// <param name="defaultValue">
        ///     The default value.
        /// </param>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        public static string GetMetadataValue(
            JavascriptTreeData.JavascriptTreeNode javascriptNode, string key, string defaultValue)
        {
            object value;
            javascriptNode.metadata.TryGetValue(key, out value);
            if (value != null)
            {
                return value.ToString();
            }

            return defaultValue;
        }

        /// <summary>
        ///     Get the named attribute value from the METADATA of the node.
        /// </summary>
        /// <param name="javascriptNode">
        ///     Tree node to extract data from.
        /// </param>
        /// <param name="key">
        ///     The named attribute to extract.
        /// </param>
        /// <param name="defaultValue">
        ///     The default value.
        /// </param>
        /// <returns>
        ///     The <see cref="Guid" />.
        /// </returns>
        public static Guid GetMetadataValue(
            JavascriptTreeData.JavascriptTreeNode javascriptNode, string key, Guid defaultValue)
        {
            Guid result;
            object value;
            javascriptNode.metadata.TryGetValue(key, out value);
            if (value != null && Guid.TryParse(value.ToString(), out result))
            {
                return result;
            }

            return defaultValue;
        }

        /// <summary>
        ///     Get the named attribute value from the METADATA of the node.
        /// </summary>
        /// <param name="javascriptNode">
        ///     Tree node to extract data from.
        /// </param>
        /// <param name="key">
        ///     The named attribute to extract.
        /// </param>
        /// <param name="defaultValue">
        ///     The default value.
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public static bool GetMetadataValue(
            JavascriptTreeData.JavascriptTreeNode javascriptNode, string key, bool defaultValue)
        {
            bool result;
            object value;
            javascriptNode.metadata.TryGetValue(key, out value);
            if (value != null && Boolean.TryParse(value.ToString(), out result))
            {
                return result;
            }

            return defaultValue;
        }

        /// <summary>
        ///     Get the named attribute value from the METADATA of the node.
        /// </summary>
        /// <param name="javascriptNode">
        ///     Tree node to extract data from.
        /// </param>
        /// <param name="key">
        ///     The named attribute to extract.
        /// </param>
        /// <param name="defaultValue">
        ///     The default value.
        /// </param>
        /// <returns>
        ///     The <see cref="int" />.
        /// </returns>
        public static int GetMetadataValue(
            JavascriptTreeData.JavascriptTreeNode javascriptNode, string key, int defaultValue)
        {
            int result;
            object value;
            javascriptNode.metadata.TryGetValue(key, out value);
            if (value != null && Int32.TryParse(value.ToString(), out result))
            {
                return result;
            }

            return defaultValue;
        }

        /// <summary>
        ///     Get the named attribute value from the METADATA of the node.
        /// </summary>
        /// <param name="javascriptNode">
        ///     Tree node to extract data from.
        /// </param>
        /// <param name="key">
        ///     The named attribute to extract.
        /// </param>
        /// <param name="defaultValue">
        ///     The default value.
        /// </param>
        /// <returns>
        ///     The
        ///     <see>
        ///         <cref>object[]</cref>
        ///     </see>
        ///     .
        /// </returns>
        public static object[] GetMetadataValue(
            JavascriptTreeData.JavascriptTreeNode javascriptNode, string key, object[] defaultValue)
        {
            object value;
            javascriptNode.metadata.TryGetValue(key, out value);
            if (value != null && !String.IsNullOrEmpty(value.ToString()))
            {
                return new object[] {value.ToString()};
            }

            return defaultValue;
        }

        #endregion

        /// <summary>
        ///     Create a node id based on the current join via parts list.
        /// </summary>
        /// <param name="joinVia"></param>
        /// <param name="nodeId"></param>
        /// <returns></returns>
        public static StringBuilder FormatNodeIdFromJoinViaParts(JoinVia joinVia, StringBuilder nodeId)
        {
            foreach (JoinViaPart p in joinVia.JoinViaList.Values)
            {
                switch (p.ViaIDType)
                {
                    case JoinViaPart.IDType.Table:
                        nodeId.Append("g");
                        break;
                    case JoinViaPart.IDType.Field:
                        nodeId.Append("k");
                        break;
                    case JoinViaPart.IDType.RelatedTable:
                        nodeId.Append("x");
                        break;
                    default:
                        continue;
                }

                nodeId.Append(p.ViaID).Append("_");
            }

            return nodeId;
        }

        /// <summary>
        /// Create a tree view node for group criteria.
        /// </summary>
        /// <param name="filter">The criteria to convert into a node</param>
        /// <returns></returns>
        public static JavascriptTreeData.JavascriptTreeNode CreateFilterGroupNode(cReportCriterion filter)
        {
            JavascriptTreeData.JavascriptTreeNode node = NewJavascriptTreeNode();

            var metadata = CreateCriteriaMetadata(
                filter.condition,
                null,
                null,
                string.Empty,
                null,
                false,
                string.Empty,
                filter.runtime,
                filter.joiner,
                filter.groupnumber,
                false);

            node.attr.id = filter.groupnumber.ToString();
            node.attr.fieldid = Guid.Empty.ToString();
            node.data = filter.groupnumber.ToString();
            node.attr.fieldtype = filter.groupnumber.ToString();
            node.attr.comment = HttpUtility.HtmlEncode(filter.field.Comment);
            node.metadata = metadata;
            return node;
        }
    }
}