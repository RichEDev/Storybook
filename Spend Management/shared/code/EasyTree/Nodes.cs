namespace Spend_Management.shared.code.EasyTree
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Concurrent;
    using System.Linq;

    using SpendManagementLibrary;
    using SpendManagementLibrary.Helpers;
    using SpendManagementLibrary.Interfaces;
    using SpendManagementLibrary.Logic_Classes.Fields;
    using SpendManagementLibrary.Logic_Classes.Tables;
    using SpendManagementLibrary.Report;

    public class Nodes
    {
        /// <summary>
        /// A private instance of <see cref="ICurrentUser"/>
        /// </summary>
        private readonly ICurrentUser _user;

        /// <summary>
        /// a private instance of <see cref="ITreeGroups"/>
        /// </summary>
        private ITreeGroups _treeGroups;

        private readonly IFields _fields;

        private readonly ITables _tables;

        private readonly int _levelLimit;

        /// <summary>
        /// A local instance of <see cref="cCustomEntities"/>.
        /// </summary>
        private readonly ICustomEntities _customEntities;

        internal Guid baseTable;

        /// <summary>
        /// A list of Table IDs that are allowed to have related table links at any level
        /// </summary>
        private readonly IAllowedTables _allowedTables;

        /// <summary>
        /// A list of the common fields for the current request.
        /// </summary>
        private ReadOnlyCollection<ReportCommonColumn> _reportCommonColumns;

        /// <summary>
        /// A private instance of <see cref="FilteredFields"/> used to excluded specific fields from the tree.
        /// </summary>
        private FilteredFields _nonRequiredFields;

        /// <summary>
        /// A private list of licenced elements for the current Account / subaccount
        /// </summary>
        private List<int> _lstLicencedElementIDs;

        /// <summary>
        /// A <see cref="ConcurrentDictionary{TKey,TValue}"/> of table plus account and the unexpanded tree.
        /// </summary>
        private static ConcurrentDictionary<string, List<EasyTreeNode>> _trees;

        static Nodes()
        {
            _trees = new ConcurrentDictionary<string, List<EasyTreeNode>>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Nodes"/> class. 
        /// </summary>
        /// <param name="user">
        /// The current user
        /// </param>
        /// <param name="fields">
        /// The fields object
        /// </param>
        /// <param name="tables">
        /// The tables object
        /// </param>
        /// <param name="levelLimit">
        /// The limit (depth) of levels to load.
        /// </param>
        /// <param name="customEntities">An instance of <see cref="cCustomEntities"/> for generating a list of fields per table.
        /// </param>
        /// <param name="allowedTables">An instance of <see cref="IAllowedTables"/></param>
        /// <param name="treeGroups">An instance of <see cref="ITreeGroups"/></param>
        public Nodes(
            ICurrentUser user,
            IFields fields,
            ITables tables,
            int levelLimit,
            ICustomEntities customEntities,
            IAllowedTables allowedTables,
            ITreeGroups treeGroups)
        {
            this._user = user;
            this._treeGroups = treeGroups;
            this._fields = fields;
            this._tables = tables;
            this._levelLimit = levelLimit;
            this._customEntities = customEntities;
            this._allowedTables = allowedTables;
            this._reportCommonColumns = new ReadOnlyCollection<ReportCommonColumn>(new List<ReportCommonColumn>());
            this._nonRequiredFields = new FilteredFields(user.CurrentActiveModule);
            this._lstLicencedElementIDs = new cElements().GetLicencedModuleIDs(user.AccountID, user.CurrentActiveModule);
        }

        /// <summary>
        /// Get the loweset level of tree items plus their children down to the specified level.
        /// </summary>
        /// <param name="baseTableString">The base table to get fields for</param>
        /// <returns></returns>
        public List<EasyTreeNode> GetInitialTreeNodes(string baseTableString)
        {
            var result = new List<EasyTreeNode>();
            var tableKey = $"{baseTableString}_{this._user.AccountID}";
            this.baseTable =  new Guid(baseTableString);
            var table = this._tables.GetTableByID(this.baseTable);
            if (_trees.ContainsKey(tableKey))
            {
                return _trees[tableKey];
            }

            // system entity
            var exportedTables = new List<Guid>();
            var level = 0;
            var faves = this.CreateFavorites();
            this.GetNodesForTable(this.baseTable, result, string.Empty, exportedTables, level);
            result = result.OrderBy(x => x.text).ToList();

            if (faves.Count == 0)
            {
                return result;
            }

            var node = this.CreateGroupNode("Other Columns", result);
            faves.Add(node);
            if (table.TableSource == cTable.TableSourceType.Metabase)
            {
                _trees[tableKey] = faves;
            }
            
            return faves;
        }

        /// <summary>
        /// Get the list of favorite Columns
        /// </summary>
        /// <returns>A List of <see cref="EasyTreeNode"/> matching the favourites defined for the selected table.</returns>
        private List<EasyTreeNode> CreateFavorites()
        {
            var commonColumns = new ReportCommonColumns();
            this._reportCommonColumns = commonColumns.GetFavourites(this.baseTable);
            var result = new List<EasyTreeNode>();
            foreach (ReportCommonColumn reportCommonColumn in this._reportCommonColumns)
            {
                var entityField = this._fields.GetFieldByID(reportCommonColumn.FieldId);
                var node = new ReportNode
                               {
                                   internalId = reportCommonColumn.JoinString,
                                   fieldType = entityField.ValueList ? "Y" : entityField.FieldType,
                                   fieldid = entityField.FieldID,
                                   isFolder = false,
                                   isLazy = false,
                                   text = entityField.Description,
                                   liClass = "field f1",
                                   crumbs = string.Empty
                               };
                result.Add(node);
            }

            return result.OrderBy(node => node.text).ToList();
        }

        /// <summary>
        /// Get the nodes for a specified table.
        /// </summary>
        /// <param name="thisTable">The table</param>
        /// <param name="result">The node.children to populate</param>
        /// <param name="idPreFix">The owner ID</param>
        /// <param name="exportedTables">A list of tables already exported in this branch</param>
        /// <param name="level">The current level.</param>
        public void GetNodesForTable(
            Guid thisTable,
            List<EasyTreeNode> result,
            string idPreFix,
            List<Guid> exportedTables,
            int level)
        {
            level++;
            var currentTable = this._tables.GetTableByID(thisTable);
            var entityFields = this._fields.GetFieldsByTableIDForViews(thisTable);
            entityFields = this.FilterOutNonViewFields(entityFields);
            entityFields = this.FilterOutNonRequiredFields(entityFields, idPreFix);
            entityFields.AddRange(this.FilterOutNonRequiredFields(this.FilterOutNonViewFields(this.AddRelatedTables(thisTable)), idPreFix));
            result.AddRange(this.AddOneToManyFields(thisTable, idPreFix, exportedTables, level));
            
            var treeFields = this.ConvertFieldsToTreeFields(entityFields);
            treeFields = this.FilterFieldsBasedOnAllowedTables(treeFields, thisTable);
            
            var groups = this.CreateGroupsFromFields(treeFields);
            foreach (TreeField treeField in treeFields)
            {
                var prefix = "n";
                var text = treeField.Description;
                var exclude = false;
                var skipNode = false;
                var lazy = false;
                if (treeField.IsForeignKey && !this.IsCreatedByModifiedBy(treeField) && this.SkipDuplicateTables(idPreFix, treeField, thisTable))
                {
                    continue;
                }

                if (currentTable.LinkingTable && (this.IsCreatedByModifiedBy(treeField) || this.IsCreatedOnModifiedOn(treeField) || currentTable.SubAccountIDFieldID == treeField.FieldID))
                {
                    continue;
                }

                var customEntity = this._customEntities.getAttributeByFieldId(treeField.FieldID);
                if (customEntity != null && customEntity.fieldtype == FieldType.Comment)
                {
                    skipNode = true;
                    continue;
                }

                if (treeField.IsForeignKey)
                {
                    var relatedTable = treeField.GetRelatedTable();
                    if (relatedTable.HasUserdefinedFields)
                    {
                        if (!exportedTables.Contains(relatedTable.UserDefinedTableID))
                        {
                            exportedTables.Add(relatedTable.UserDefinedTableID);
                        }
                    }
                    skipNode = true;
                    if (relatedTable != null)
                    {
                        var allowed = !relatedTable.ElementID.HasValue || this._lstLicencedElementIDs.Any(elementId => elementId == relatedTable.ElementID.Value);

                        if (allowed && relatedTable.PrimaryKeyID != Guid.Empty && relatedTable.AllowReportOn)
                        {
                            lazy = true;
                            prefix = "k";

                            text = this.SetNodeText(currentTable, relatedTable, treeField, customEntity);
                            skipNode = false;
                        }
                    }

                    if (this._levelLimit > 1)
                    {
                        exclude = this.IsCreatedByModifiedBy(treeField) || this.IsCreatedOnModifiedOn(treeField);
                    }
                }

                if (!string.IsNullOrEmpty(idPreFix))
                {
                    prefix = "_" + prefix;
                }

                EasyTreeNode node;

                if (treeField.TableID != thisTable)
                {
                    node = this.CreateRelatedTableNode(idPreFix, treeField, exportedTables, level);
                    if (node.text == null)
                    {
                        skipNode = true;
                    }
                }
                else
                {
                    node = new ReportNode
                               {
                                   internalId = idPreFix + prefix + treeField.FieldID.ToString(),
                                   fieldType = treeField.ValueList ? "Y" : treeField.FieldType,
                                   fieldid = treeField.FieldID,
                                   isFolder = lazy,
                                   isLazy = lazy,
                                   text = text,
                                   liClass = lazy ? "field" : "field f1",
                                   crumbs = treeField.GroupName
                    };
                }

                if (node.isLazy)
                {
                    if (!exclude && !skipNode)
                    {
                        node.children = new List<EasyTreeNode>();
                        var tableId = this.GetTableIdFromNodeId(node.internalId);
                        if (exportedTables.Count >= level)
                        {
                            exportedTables.RemoveRange(level - 1, exportedTables.Count - level);
                        }

                        if (currentTable.LinkingTable || (!exportedTables.Contains(tableId) && level < this._levelLimit && this.baseTable != tableId))
                        {
                            node.isLazy = false;
                            exportedTables.Add(tableId);
                            this.GetNodesForTable(tableId, node.children, node.internalId, exportedTables, level);
                            if (currentTable.LinkingTable && currentTable.TableID != this.baseTable)
                            {
                                skipNode = true;
                                result.AddRange(node.children);
                            }
                            else
                            {
                                node.children = node.children.OrderBy(x => x.text).ToList();
                                if (node.children.Count == 0)
                                {
                                    skipNode = true;
                                }
                            }
                        }
                    }
                }

                if (!skipNode)
                {
                    if (string.IsNullOrEmpty(node.crumbs))
                    {
                        result.Add(node);
                    }
                    else
                    {
                        this.AddNodeToGroup(node, groups);
                    }
                }
            }

            this.AddGroupsToChildren(groups, result);
        }

        private List<cField> FilterOutNonRequiredFields(List<cField> entityFields, string currentId)
        {
            var result = entityFields.Where(f => !this._nonRequiredFields.FilterField(f, currentId));
            result = result.Where(f => !f.Description.StartsWith("Old "));
            return result.ToList();
        }

        /// <summary>
        /// Get the Text for the current node.
        /// </summary>
        /// <param name="currentTable">An instance of <see cref="cTable"/>that represents the current table</param>
        /// <param name="relatedTable">An instance of <see cref="cTable"/>that represents the table we are joining to.</param>
        /// <param name="treeField">An instance of <see cref="TreeField"/>used as the joining key</param>
        /// <param name="customEntity">An instance of <see cref="cCustomEntity"/>If the Field is a custom entity.</param>
        /// <returns>The text for the current node.</returns>
        private string SetNodeText(
            cTable currentTable,
            cTable relatedTable,
            TreeField treeField,
            cAttribute customEntity)
        {
            var text = treeField.Description.TrimEnd(" id");
            if (currentTable.TableID != treeField.TableID)
            {
                // foreign key join to another table
                if (!string.IsNullOrEmpty(treeField.FriendlyNameTo))
                {
                    text = treeField.FriendlyNameTo;
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(treeField.FriendlyNameFrom))
                {
                    text = treeField.FriendlyNameFrom;
                }
                // foreign key from another table to current table
            }

            if (string.IsNullOrEmpty(text))
            {
                text = treeField.Description.ToLower().EndsWith("id")
                           ? relatedTable.Description
                           : treeField.Description;
            }

            if (customEntity != null)
            {
                text = treeField.Description;
            }
            return text;
        }

        /// <summary>
        /// Add the current <see cref="EasyTreeNode"/> groups to the tree. Combining any existing branches where the property text is the same.
        /// </summary>
        /// <param name="groups">The Groups to add</param>
        /// <param name="children">The children of the current node that should be added to.</param>
        private void AddGroupsToChildren(List<EasyTreeNode> groups, List<EasyTreeNode> children)
        {
            foreach (EasyTreeNode groupNode in groups.Where(g => g.children.Count > 0))
            {
                var existing = children.FirstOrDefault(n => n.text == groupNode.text && n.children != null);
                if (existing != null)
                {
                    existing.children.AddRange(groupNode.children);
                    existing.children = existing.children.OrderBy(x => x.text).ToList();
                }
                else
                {
                    if (groupNode.children.Count == 1 && groupNode.children[0].children != null)
                    {
                        children.Add(groupNode.children[0]);
                    }
                    else
                    {
                        children.Add(groupNode);
                    }
                }
            }

            children = children.OrderBy(x => x.text).ToList();
        }

        /// <summary>
        /// Add the given node to a named group
        /// </summary>
        /// <param name="node">The <see cref="EasyTreeNode"/>to add to a group defined by the property GroupName</param>
        /// <param name="groups">A <see cref="List{T}"/>of <seealso cref="EasyTreeNode"/>which the current node will be added to.</param>
        private void AddNodeToGroup(EasyTreeNode node, List<EasyTreeNode> groups)
        {
            var group = groups.FirstOrDefault(g => g.crumbs == node.crumbs);
            if (group != null)
            {
                group.children.Add(node);
                group.children = group.children.OrderBy(x => x.text).ToList();
            }
        }

        /// <summary>
        /// Create a list of <see cref="EasyTreeNode"/> based on the Group Names of a given list of <see cref="TreeField"/>
        /// </summary>
        /// <param name="treeFields">The list of <see cref="TreeField"/>for the current table</param>
        /// <returns>A list of <see cref="EasyTreeNode"/>with the text set as the group name from the given list of fields.</returns>
        private List<EasyTreeNode> CreateGroupsFromFields(List<TreeField> treeFields)
        {
            var groupNames =
                treeFields.Where(item => !string.IsNullOrEmpty(item.GroupName))
                    .Select(field => field.GroupName)
                    .Distinct();
            return groupNames.Select(groupName => this.CreateGroupNode(groupName, new List<EasyTreeNode>())).ToList();
        }

        /// <summary>
        /// Create a single <see cref="EasyTreeNode"/> used for creating groping branches.
        /// </summary>
        /// <param name="fieldGroupName">The name of the group to create</param>
        /// <param name="childNodes">A list of <see cref="EasyTreeNode"/>To include as the "children" property if needed.</param>
        /// <returns></returns>
        private EasyTreeNode CreateGroupNode(string fieldGroupName, List<EasyTreeNode> childNodes)
        {
            var result = new ReportNode
                             {
                                 text = fieldGroupName,
                                 children = childNodes ?? new List<EasyTreeNode>(),
                                 liClass = string.Empty,
                                 crumbs = fieldGroupName, 
                                 isFolder = true
            };
            return result;
        }

        /// <summary>
        /// Convert a List of <see cref="cField"/> to a list of <see cref="TreeField"/>
        /// </summary>
        /// <param name="entityFields">The list to convert</param>
        /// <returns>A List of <see cref="TreeField"/></returns>
        private List<TreeField> ConvertFieldsToTreeFields(List<cField> entityFields)
        {
            return entityFields.Select(entityField => new TreeField(entityField, this._treeGroups)).ToList();
        }

        /// <summary>
        /// Filter the given list and return fields that have either a view group or are foreign keys
        /// </summary>
        /// <param name="entityFields">The initiali <see cref="List{T}"/>of <seealso cref="cField"/></param>
        /// <returns>A list of fields that have view groups of are foreign keys unless no fields have view groups and no tree groups then return all fields</returns>
        private List<cField> FilterOutNonViewFields(List<cField> entityFields)
        {
            if (entityFields.Any(field => field.ViewGroupID != Guid.Empty ) && entityFields.Any(field => field.TreeGroup.HasValue))
            {
                var result = entityFields.Where(f => f.ViewGroupID != Guid.Empty || f.TreeGroup.HasValue || f.IsForeignKey);
                return result.ToList();
            }

            return entityFields;
        }

        /// <summary>
        /// Filter the list of <see cref="TreeField"/> by the "reports_allowedtables_base" table
        /// </summary>
        /// <param name="entityFields">The complete list of fields</param>
        /// <param name="thisTable">The current table <see cref="Guid"/></param>
        /// <returns>A filtered <see cref="List{T}"/>of <seealso cref="TreeField"/>Where the table or related table are in the allowed tables list.</returns>
        private List<TreeField> FilterFieldsBasedOnAllowedTables(List<TreeField> entityFields, Guid thisTable)
        {
            var result = new List<TreeField>();
            var allowedTable = this._allowedTables.Get(thisTable);
            if (allowedTable == null)
            {
                foreach (var entityField in entityFields)
                {
                    if (entityField.IsForeignKey)
                    {
                        var relatedTable = this._tables.GetTableByID(entityField.RelatedTableID);
                        if (relatedTable != null && relatedTable.PrimaryKeyID != Guid.Empty
                            && relatedTable.AllowReportOn)
                        {
                            if (new Guid(ReportTable.Employees)
                                == (entityField.TableID == thisTable ? entityField.RelatedTableID : entityField.TableID))
                            {
                                if (entityField.FieldName.ToLower() == "createdby"
                                   || entityField.FieldName.ToLower() == "modifiedby" || entityField.FieldSource == cField.FieldSourceType.CustomEntity || entityField.FieldID == new Guid(ReportFields.TeamEmployeeId))
                                {
                                    result.Add(entityField);
                                }
                            }
                            else
                            {
                                result.Add(entityField);
                            }
                        }
                    }
                    else
                    {
                        result.Add(entityField);
                    }
                }

                return result;
            }

            foreach (var entityField in entityFields)
            {
                if (entityField.IsForeignKey)
                {
                    if (new Guid(ReportTable.Employees)
                        == (entityField.TableID == thisTable ? entityField.RelatedTableID : entityField.TableID)
                        && (entityField.FieldName.ToLower() == "createdby"
                            || entityField.FieldName.ToLower() == "modifiedby"))
                    {
                        result.Add(entityField);
                    }
                    else if (
                        allowedTable.Contains(
                            entityField.TableID == thisTable ? entityField.RelatedTableID : entityField.TableID))
                    {
                        result.Add(entityField);
                    }
                }
                else
                {
                    result.Add(entityField);
                }
            }

            return result;
        }

        /// <summary>
        /// Examine the join tree and skip if the ID has already been used.
        /// Except if the id is the last key used.
        /// </summary>
        /// <param name="idPreFix">The current tree ID</param>
        /// <param name="entityField">The current Field</param>
        /// <param name="thisTable"></param>
        /// <returns>True if this field has already been used in the tree.</returns>
        internal bool SkipDuplicateTables(string idPreFix, cField entityField, Guid thisTable)
        {
            if (entityField.TableID == new Guid(ReportTable.Employees))
            {
                return false;
            }

            if (entityField.TableID == new Guid(ReportTable.Tasks) && entityField.FieldID == new Guid(ReportFields.TaskCreatorId))
            {
                return false;
            }

            if (entityField.FieldID == new Guid(ReportFields.TeamEmployeeId)
                || entityField.FieldID == new Guid(ReportFields.BudgetHolderEmployeeId)
                || entityField.FieldID == new Guid(ReportFields.TeamLeaderId)
                || entityField.FieldID == new Guid(ReportFields.EsrAssignmentSignOffEmployeeId))
            {
                return false;
            }

            if (idPreFix.Contains(entityField.FieldID.ToString().ToLower()))
            {
                return true;
            }

            var tablesUsed = this.GetTablesFromIds(idPreFix);
            var currentTable = entityField.TableID != thisTable ? entityField.TableID : entityField.RelatedTableID;

            return !this.VerifyTablesAreNotReusedInTheTree(tablesUsed, currentTable);
        }

        /// <summary>
        /// Verify that tables are only used once, unless they are the next stgep down in the join.
        /// e.g. savedexpenses joining directly to savedexpenses.
        /// </summary>
        /// <param name="tablesUsed">A <see cref="List{T}"/>of table <seealso cref="Guid"/>used in thie branch.</param>
        /// <param name="currentTable">The current table</param>
        /// <returns>True if the tables are used only once, or are used one after the other.</returns>
        internal bool VerifyTablesAreNotReusedInTheTree(List<Guid> tablesUsed, Guid currentTable)
        {
            var tableCount = new Dictionary<Guid, List<int>>();
            var currentTables = new List<Guid>();
            currentTables.AddRange(tablesUsed);
            currentTables.Add(currentTable);

            foreach (Guid guid in currentTables.Distinct())
            {
                var occurances = new List<int>();
                var idx = 0;
                foreach (Guid guid1 in currentTables)
                {
                    if (guid1 == guid)
                    {
                        occurances.Add(idx);
                    }

                    idx++;
                }

                tableCount.Add(guid, occurances);

                if (!this.IntegersAreSequential(tableCount))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Return True if all the integers each list of the dictionary are sequential. 
        /// </summary>
        /// <param name="tableCount">A <see cref="Dictionary{TKey,TValue}"/>of Table Guid and the occurances of each table as a <seealso cref="List{T}"/>of <seealso cref="int"/></param>
        /// <returns>True if all lists of occurances are sequential.</returns>
        internal bool IntegersAreSequential(Dictionary<Guid, List<int>> tableCount)
        {
            foreach (List<int> occuranceList in tableCount.Values)
            {
                var item = -1;
                foreach (int i in occuranceList)
                {
                    if (item != -1)
                    {
                        if (item + 1 != i)
                        {
                            return false;
                        }
                    }

                    item = i;
                }
            }

            return true;
        }

        /// <summary>
        /// Return a list if <see cref="Guid"/> that relate to the tables used in this branch of a tree.
        /// </summary>
        /// <param name="id">The current join tree for this item.</param>
        /// <returns>A <see cref="List{T}"/> of table <seealso cref="Guid"/> used for this branch.</returns>
        internal List<Guid> GetTablesFromIds(string id)
        {
            var parts = id.TrimStart('_').Split('_');
            var result = new List<Guid> { this.baseTable };
            if (string.IsNullOrEmpty(id))
            {
                return result;
            }

            for (int i = 0; i <= parts.GetUpperBound(0); i++)
            {
                var preFix = parts[i].Substring(0, 1);
                Guid idItem;
                if (Guid.TryParse(parts[i].Substring(1), out idItem))
                {
                    switch (preFix)
                    {
                        case "g":
                            result.Add(idItem);
                            break;
                        case "k":
                            result.Add(this._fields.GetFieldByID(idItem).GetRelatedTable().TableID);
                            break;
                        case "x":
                            result.Add(this._fields.GetFieldByID(idItem).GetParentTable().TableID);
                            break;
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Add one to many fields (from custom entities) to the list as they are not handled within fields.
        /// </summary>
        /// <param name="thisTable">
        ///     the current table in the tree.
        /// </param>
        /// <param name="preFix">
        ///     The id pre fix.
        /// </param>
        /// <param name="exportedTables">A list of already exported tables in this operation.</param>
        /// <param name="level">The level to export down to in this operation.</param>
        /// <returns>
        /// The <see>
        ///         <cref>IEnumerable</cref>
        ///     </see>
        ///     of <see cref="EasyTreeNode"/> that are relationship fields on the current table if it is a <see cref="cCustomEntity"/>.
        /// </returns>
        private IEnumerable<EasyTreeNode> AddOneToManyFields(
            Guid thisTable,
            string preFix,
            List<Guid> exportedTables,
            int level)
        {
            var result = new List<EasyTreeNode>();
            var customEntity = this._customEntities.getEntityByTableId(thisTable);
            if (customEntity == null)
            {
                return result;
            }

            foreach (cAttribute attribute in customEntity.attributes.Values)
            {
                if (attribute.fieldtype == FieldType.Relationship && attribute is cOneToManyRelationship)
                {
                    var entityField = this._fields.GetFieldByID(attribute.fieldid);
                    var prefix = "n";
                    var text = entityField.Description;
                    var exclude = false;
                    var skipNode = false;
                    var lazy = false;
                    var relatedTable = entityField.GetRelatedTable();
                    if (relatedTable != null && relatedTable.PrimaryKeyID != Guid.Empty && relatedTable.AllowReportOn)
                    {
                        lazy = true;
                        prefix = "k";
                        text = $"{this._tables.GetTableByID(entityField.RelatedTableID).Description}({text})";
                        if (text.ToLower().EndsWith("id"))
                        {
                            text = text.Substring(0, text.Length - 2).TrimEnd();
                        }

                        // update text to description after adding it to crumbs
                        text = entityField.Description;

                        if (this._levelLimit > 1)
                        {
                            exclude = this.IsCreatedByModifiedBy(entityField);
                        }
                    }

                    if (!string.IsNullOrEmpty(preFix))
                    {
                        prefix = "_" + prefix;
                    }

                    var node = new ReportNode
                                   {
                                       internalId = preFix + prefix + entityField.FieldID.ToString(),
                                       fieldType = entityField.ValueList ? "Y" : entityField.FieldType,
                                       fieldid = entityField.FieldID,
                                       isFolder = true,
                                       isLazy = lazy,
                                       text = text,
                                       liClass = lazy ? "field" : "field f1",
                                       crumbs = string.Empty // Gl fields have no groups
                                   };

                    if (node.isLazy)
                    {
                        if (!exclude)
                        {
                            node.children = new List<EasyTreeNode>();
                            var tableId = this.GetTableIdFromNodeId(node.internalId);
                            if (exportedTables.Count >= level)
                            {
                                exportedTables.RemoveRange(level - 1, exportedTables.Count - level);
                            }

                            if (!exportedTables.Contains(tableId) && level < this._levelLimit
                                && this.baseTable != tableId)
                            {
                                node.isLazy = false;
                                exportedTables.Add(tableId);
                                this.GetNodesForTable(tableId, node.children, node.internalId, exportedTables, level);
                                node.children = node.children.OrderBy(x => x.text).ToList();
                            }
                        }
                    }

                    if (!skipNode)
                    {
                        result.Add(node);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Is the given field Created On, Modified On or SubAccountId?
        /// </summary>
        /// <param name="field">The <see cref="cField"/>to check</param>
        /// <returns>True if the field is Created On, Modified On or SubAccountId</returns>
        private bool IsCreatedOnModifiedOn(cField field)
        {
            var fieldName = field.FieldName.ToLower();
            if (fieldName == "createdon" || fieldName == "modifiedon" || fieldName == "subaccountid")
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Is the given field Created By, or Modified By?
        /// </summary>
        /// <param name="field">The <see cref="cField"/>to examine</param>
        /// <returns>True if the field is Created by or Modified BY</returns>
        private bool IsCreatedByModifiedBy(cField field)
        {
            var fieldName = field.FieldName.ToLower();
            if (fieldName == "createdby" || fieldName == "modifiedby")
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Derive the table id of the curernt node from the node id
        /// </summary>
        /// <param name="stringid">A node id showing the fields used for the join between the base table of the query and the current node</param>
        /// <returns>The <see cref="Guid"/> ID of the table of the current field</returns>
        internal Guid GetTableIdFromNodeId(string stringid)
        {
            var parts = stringid.Split('_');
            var lastLink = parts.LastOrDefault();
            if (lastLink != null)
            {
                var preFix = lastLink.Substring(0, 1);
                Guid id;
                if (Guid.TryParse(lastLink.Substring(1), out id))
                {
                    switch (preFix)
                    {
                        case "g":
                            return this._tables.GetTableByID(id).TableID;
                            break;
                        case "k":
                            return this._fields.GetFieldByID(id).GetRelatedTable().TableID;
                            break;
                        case "x":
                            return this._fields.GetFieldByID(id).GetParentTable().TableID;
                            break;
                    }
                }
            }

            return Guid.Empty;
        }

        /// <summary>
        /// Find any field where the related table property matches the current table ID
        /// </summary>
        /// <param name="tableId">The current table <see cref="Guid"/>ID</param>
        /// <returns>A List of <see cref="cField"/>Where the field Related Table equals the given table id.</returns>
        private List<cField> AddRelatedTables(Guid tableId)
        {
            var table = this._tables.GetTableByID(tableId);
            var result = new List<cField>();

            var relatedTable = Guid.Empty;
            if (table == null)
            {
                var thisField = this._fields.GetFieldByID(tableId);
                table = thisField.GetParentTable();
                if (table.TableSource == cTable.TableSourceType.CustomEntites || !table.AllowReportOn)
                {
                    return new List<cField>();
                }

                relatedTable = thisField.RelatedTableID;
                tableId = table.TableID;
            }
            else
            {
                if (table.TableSource == cTable.TableSourceType.CustomEntites || !table.AllowReportOn)
                {
                    return new List<cField>();
                }
            }
            if (this._allowedTables.Get(table.TableID) == null)
            {
                return new List<cField>();
            }

            result.AddRange(this.FilterRelatedFields(this._fields.GetAllRelatedFields(table.TableID)));
            if (relatedTable != Guid.Empty)
            {
                result.AddRange(this._fields.getFieldsByViewGroup(relatedTable).Values);
            }

            return result;
        }

        /// <summary>
        /// Filter Related tables based on the current base table.
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        private IEnumerable<cField> FilterRelatedFields(List<cField> list)
        {
            if (this.baseTable != new Guid(ReportTable.Employees))
            {
                return list;
            }

            return list.Where(f => f.FieldID != new Guid(ReportFields.TaskCreatorId));
        }

        /// <summary>
        /// Create an <see cref="EasyTreeNode"/> from a related table <see cref="cField"/>
        /// </summary>
        /// <param name="nodeId">The current Node ID (the fields / tables used to get from the base to the current field)</param>
        /// <param name="field">The <see cref="cField"/>to create a node for</param>
        /// <param name="exportedTables">A List of <see cref="Guid"/>which are the table ID that have already been created in this request.</param>
        /// <param name="level">The current level of the request</param>
        /// <returns></returns>
        private EasyTreeNode CreateRelatedTableNode(
            string nodeId,
            TreeField field,
            List<Guid> exportedTables,
            int level)
        {
            var node = new ReportNode { isFolder = true  };
            var table = field.GetParentTable();
            if (table == null || string.IsNullOrEmpty(table.Description) || !table.AllowReportOn)
            {
                return node;
            }

            // g for Group - table join, k for node linK - foreign key join, n for node - field, x related table link
            var guidPrefix = "x";

            node.text = this.SetNodeText( this._tables.GetTableByID(this.baseTable), table, field, null);

            node.internalId = nodeId + "_" + guidPrefix + field.FieldID;
            node.crumbs = field.GroupName;
            node.fieldid = field.FieldID;
            node.fieldType = field.ValueList ? "Y" : field.FieldType;
            node.isLazy = true;
            node.liClass = field.IsForeignKey ? "field" : "field f1";
            node.children = new List<EasyTreeNode>();
            if (exportedTables.Count >= level)
            {
                exportedTables.RemoveRange(level - 1, exportedTables.Count - level);
            }

            return node;
        }
    }
}