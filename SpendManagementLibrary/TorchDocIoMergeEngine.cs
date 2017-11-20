using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;

namespace SpendManagementLibrary
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data;
    using System.Linq;
    using System.Text.RegularExpressions;
    using SpendManagementLibrary.DocumentMerge;

    using Syncfusion.DocIO.DLS; 

    /// <summary>
    /// A data class to hold a master data set of torch report tables, alongside the join information held in an arraylist of commands.
    /// </summary>
    public class TorchDocIoMergeEngine : IDisposable
    {
        private const string RemoveWithPreviousSiblingToken = @"@@REMOVEWIHPREVIOUSSIBLING@@";

        private const string RemoveTableRowToken = @"@@REMOVEROWTOKEN@@";

        private const string AutoSummaryTableName = "AutoSummary";

        private const string SortedTableSuffix = "-(Sorted)";

        /// <summary>
        /// Gets or sets the data set holding the torch report data tables.
        /// </summary>
        public DataSet DataSet { get; set; }

        public string[] SummarySortColumns { get; set; }

        public TorchGroupingConfiguration GroupingConfiguration { get; set; }

        public int NumberOfSummaries
        {
            get
            {
                return SummarySortColumns == null ? 1 : SummarySortColumns.Length;
            }
            set
            {
                SummarySortColumns = new string[value];
            }
        }

        public List<string> Groups { get; set; }

        private const string SummaryDelimiter = "_";

        private readonly List<string> _addedCommandTables;

        private DataSet GroupingTablesDataSet { get; set; }

        private List<TorchJoin> TableJoins { get; set; }

        private string _majorGroupingJoin;

        private readonly int _accountId;

        /// <summary>
        /// Gets the join information for the data tables held in the data set.
        /// </summary>
        public ArrayList Commands { get; set; }

        /// <summary>
        /// The relationships between the data tables are pulled from the document template structure and held in this tree structure.
        /// </summary>
        private TreeNode<string> _relationshipHierarchy;

        private const string GroupingTablePlaceHolder = "GROUPINGTABLEPLACEHOLDER";

        /// <summary>
        /// Initialiser for the TorchDocIoMergeEngine class.
        /// </summary>
        public TorchDocIoMergeEngine(TorchGroupingConfiguration torchGroupingConfiguration, int accountId)
        {
            this._accountId = accountId;
            this._addedCommandTables = new List<string>();
            this.DataSet = new DataSet("root");
            this.GroupingTablesDataSet = new DataSet("GroupingTables");
            this.TableJoins = new List<TorchJoin>();
            this.Commands = new ArrayList();
            this._relationshipHierarchy = null;
            this.NumberOfSummaries = 1;
            this.SummarySortColumns = null;
            this.GroupingConfiguration = torchGroupingConfiguration;
        }

        /// <summary>
        /// Adds a join information command to the collection. 
        /// Takes the form:
        /// "tableA", "IDField = %tableB.IDField% AND IDField2 = %tableB.IDField%"
        /// </summary>
        /// <param name="tableName">The name of the table the join information is for</param>
        /// <param name="command">The array list of dictionary entries representing the join information.</param>
        /// <param name="rootLevel">Specifies whether the command is group level or root level</param>
        public void AddCommand(string tableName, string command, bool rootLevel = false)
        {
            var entry = new DictionaryEntry(tableName, command);
            if (!rootLevel)
            {
                Commands.Add(entry);
            }
            else
            {
                Commands.Insert(1, entry);
            }
            this._addedCommandTables.Add(tableName);
        }


        /// <summary>
        /// Rename all GROUPX Tags to be the grouping column also rename GROUPXText to be the column data from group.
        /// </summary>
        /// <param name="wordDocument"></param>
        internal void RenameGroupTagsInDocument(IWordDocument wordDocument)
        {
            foreach (WSection section in wordDocument.Sections)
            {
                RenameGroupTags(section.ChildEntities);
            }
        }

        private void RenameGroupTags(EntityCollection childEntities)
        {
            for (int i = childEntities.Count - 1; i >= 0; i--)
            {
                var childEntity = childEntities[i];
                if (childEntity.EntityType == EntityType.MergeField)
                {
                    var mergeField = (WMergeField)childEntity;
                    mergeField.FieldName = mergeField.FieldName.Replace(" ", string.Empty);

                    var findGroup = new Regex(@"GROUP[0-9]+");
                    if (findGroup.IsMatch(mergeField.FieldName))
                    {
                        var levelText = mergeField.FieldName.Substring(5);
                        int level;
                        if (int.TryParse(levelText, out level))
                        {
                            if (level <= this.GroupingConfiguration.GroupingColumnsList.Count)
                            {
                                mergeField.FieldName = (mergeField.Prefix != string.Empty ? "GROUP" : string.Empty) +
                                                       this.GroupingConfiguration.GroupingColumnsList[level - 1];
                            }
                            else
                            {
                                try
                                {
                                    mergeField.OwnerParagraph.OwnerTextBody.ChildEntities.Remove(mergeField.OwnerParagraph);
                                }
                                catch (Exception e)
                                {
                                    mergeField.OwnerParagraph.ChildEntities.Remove(childEntity);
                                }
                            }
                        }
                    }
                }

                var compositeEntity = childEntity as ICompositeEntity;
                if (compositeEntity != null)
                {
                    RenameGroupTags(compositeEntity.ChildEntities);
                }

            }
        }

        /// <summary>
        /// This will recurse the tree structure created from the document template and build the join information back in to our data set.
        /// </summary>
        /// <param name="currentNode">The current node in the recursion.</param>
        /// <param name="level">The current level of recursion.</param>
        private void RecurseAndCreateStandardJoins(TreeNode<string> currentNode, int level)
        {
            string nodeValue = currentNode.Value;

            if (level != 0 && this.DataSet.Tables[currentNode.Value] != null)
            {
                if (!nodeValue.StartsWith("GROUP")
                    && !this.GroupingConfiguration.GroupingSources.Contains(nodeValue)
                    && !this.GroupingConfiguration.GroupingColumnsList.Contains(nodeValue)
                    && !this._addedCommandTables.Contains(nodeValue))
                {
                    bool addedMajorJoin = false;
                    string join = string.Empty;
                    string validParentJoin = this.GetFirstValidParentGroupName(currentNode);
                    int countdownStop = this.GroupingConfiguration.GroupingColumnsList.Count > 0 ? 1 : 0;
                    string baseJoin = string.Format("ID0=%{0}.ID0% AND ", validParentJoin);
                    int numberOfIdColumns = NumberOfIdColumnsInDataTable(this.DataSet.Tables[currentNode.Value]);
                    for (int i = numberOfIdColumns - 2; i >= countdownStop; i--)
                    {
                        join += string.Format("ID{0}=%{1}.ID{0}% AND ", i, validParentJoin);
                    }

                    if (this.TableContainsAllGroupingColumns(this.DataSet.Tables[currentNode.Value]) && !string.IsNullOrEmpty(this._majorGroupingJoin))
                    {
                        join += this._majorGroupingJoin.Replace(GroupingTablePlaceHolder, validParentJoin);
                        addedMajorJoin = true;
                    }

                    if (!addedMajorJoin)
                    {
                        join = baseJoin + join;
                    }

                    join = RemoveLast(join, " AND ");
                    this.TableJoins.Add(new TorchJoin(nodeValue, join, level));
                }
            }

            level++;

            foreach (var childNode in currentNode.Children)
            {
                this.RecurseAndCreateStandardJoins(childNode, level);
            }
        }

        private string GetFirstValidParentGroupName(TreeNode<string> node)
        {
            while (true)
            {
                int invalidCheck;

                if (node.Parent.Value == this.Groups[0])
                {
                    return this.Groups[0];
                }

                string strippedParentGroupName = node.Parent.Value.Replace("GROUP", string.Empty);

                if (!int.TryParse(strippedParentGroupName, out invalidCheck))
                {
                    return strippedParentGroupName;
                }
                node = node.Parent;
            }
        }

        private void CleanSummaries()
        {
            foreach (DataTable dataTable in this.DataSet.Tables.Cast<DataTable>().Where(dataTable => dataTable.TableName.Contains(AutoSummaryTableName) || dataTable.TableName.EndsWith("*")))
            {
                CleanSummary(dataTable);
            }

            this.DataSet.AcceptChanges();
        }

        private static void CleanSummary(DataTable dataTable)
        {
            int filteredIdColumn = NumberOfIdColumnsInDataTable(dataTable) - 1;
            dataTable.Delete(string.Format("ID{0} is null or ID{0} = ''", filteredIdColumn));
        }

        /// <summary>
        /// Returns the number of ID columns in a datatable.... ID0, ID1 and ID2 would return 3.
        /// </summary>
        /// <param name="dataTable"></param>
        /// <returns></returns>
        private static int NumberOfIdColumnsInDataTable(DataTable dataTable)
        {
            int found = 0;

            for (int i = 0; i < dataTable.Columns.Count; i++)
            {
                DataColumn column = dataTable.Columns[i];

                if (column.ColumnName.StartsWith("ID"))
                {
                    int idValue;
                    if (int.TryParse(column.ColumnName.Replace("ID", string.Empty).Trim(), out idValue))
                    {
                        found++;
                    }
                }
            }

            return found;
        }

        private bool TableContainsAllGroupingColumns(DataTable table)
        {
            int numberOfGroupingColumns = this.GroupingConfiguration.GroupingColumnsList.Count;

            int foundCount = this.GroupingConfiguration.GroupingColumnsList.Count(groupingColumn => table.Columns.Contains(groupingColumn));

            return foundCount == numberOfGroupingColumns;
        }

        /// <summary>
        /// Adds in the standard joins to the Commands collection in the correct level order.
        /// </summary>
        private void CombineStandardAndGroupJoins()
        {
            if (this.TableJoins.Count > 0)
            {
                int maxLevel = this.TableJoins.Select(tableJoin => tableJoin.DepthLevel).Concat(new[] { 1 }).Max();

                for (int i = 1; i <= maxLevel; i++)
                {
                    List<TorchJoin> joins = this.TableJoins.Where(x => x.DepthLevel == i).ToList();

                    foreach (TorchJoin torchJoin in joins)
                    {
                        bool found = this.Commands.Cast<DictionaryEntry>().Any(dictionaryEntry => String.Equals(dictionaryEntry.Key.ToString(), torchJoin.TableName, StringComparison.CurrentCultureIgnoreCase));

                        if (!found)
                        {
                            this.AddCommand(torchJoin.TableName, torchJoin.Join, torchJoin.DepthLevel == 1);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Creates the joins for the grouping tables (inserts them between the base level data table and the standard tables/
        /// </summary>
        private void CreateGroupingJoins()
        {
            string baseLevelTable = this.Groups[0];
            this.AddCommand(baseLevelTable, string.Empty);

            if (this.GroupingTablesDataSet.Tables.Count > 0)
            {
                string majorGroupingTable = this.GroupingTablesDataSet.Tables[this.GroupingTablesDataSet.Tables.Count - 1].TableName;
                string join;

                for (int i = 0; i < this.GroupingTablesDataSet.Tables.Count; i++)
                {
                    string joinFromTable = this.GroupingTablesDataSet.Tables[i].TableName;
                    string joinToTable = i == 0 ? baseLevelTable : this.GroupingTablesDataSet.Tables[i - 1].TableName;

                    join = string.Format("ID0=%{0}.ID0% AND ", joinToTable);

                    if (i > 0)
                    {
                        string jTable = string.Empty;
                        for (int j = i; j > 0; j--)
                        {
                            string columnAndTableName = this.GroupingConfiguration.GroupingColumnsList[j - 1];
                            if (string.IsNullOrEmpty(jTable))
                            {
                                jTable = columnAndTableName;
                            }

                            join += string.Format("{0}=%GROUP{1}.{0}% AND ", this.GroupingConfiguration.GroupingColumnsList[j - 1], jTable);
                        }
                    }

                    join = RemoveLast(join, " AND ");
                    this.AddCommand(joinFromTable, join);
                }

                foreach (var groupingSource in this.GroupingConfiguration.GroupingSources)
                {
                    join = string.Format("ID0=%{0}.ID0% AND ", majorGroupingTable);
                    string groupingJoin = this.GroupingConfiguration.GroupingColumnsList.Aggregate(@join, (current, s) => current + string.Format("{0}=%{1}.{0}% AND ", s, majorGroupingTable));
                    this._majorGroupingJoin = groupingJoin.Replace(majorGroupingTable, GroupingTablePlaceHolder);
                    groupingJoin = RemoveLast(groupingJoin, " AND ");
                    this.AddCommand(groupingSource, groupingJoin);
                }
            }
        }

        /// <summary>
        /// Applies sorting to the standard tables given a sorting configuration
        /// </summary>
        private void SortStandardDataTables()
        {
            const string SortedTag = "@@Sorted@@";
            foreach (TorchReportSorting reportSortingConfiguration in this.GroupingConfiguration.ReportSortingConfigurations)
            {
                string tableToSort = reportSortingConfiguration.ReportName;
                List<TorchReportSortingColumn> sortingColumns = reportSortingConfiguration.TorchReportSortingColumns;
                string sortingSql = string.Empty;

                foreach (TorchReportSortingColumn torchReportSortingColumn in sortingColumns)
                {
                    sortingSql += torchReportSortingColumn.ColumnName;
                    sortingSql += torchReportSortingColumn.SortingOrder == TorchSummaryColumnSortDirection.Ascending
                        ? " ASC, "
                        : " DESC, ";
                }

                sortingSql = RemoveLast(sortingSql, ", ");

                DataTable sortedTable = this.DataSet.Tables[tableToSort];
                sortedTable.DefaultView.Sort = sortingSql;
                sortedTable.DefaultView.ApplyDefaultSort = true;
                sortedTable = sortedTable.DefaultView.ToTable();
                sortedTable.TableName = string.Format("{0}{1}", sortedTable.TableName, SortedTag);
                sortedTable.AcceptChanges();
                this.DataSet.Tables.Remove(tableToSort);
                sortedTable.TableName = sortedTable.TableName.Replace(SortedTag, string.Empty);
                this.DataSet.Tables.Add(sortedTable);
                this.DataSet.AcceptChanges();
            }
        }

        /// <summary>
        /// Creates the grouping tables from the merge sources and grouping columns
        /// </summary>
        private void CreateGroupingDataTables()
        {
            this.GroupingTablesDataSet.Tables.Clear();

            var idColumn = new List<string> { "ID0" };
            var filteredColumns = this.GroupingConfiguration.FilteringColumns.Select(filteringConfig => filteringConfig.ColumnName).ToList();
            var sortedColumns = this.GroupingConfiguration.SortingColumnsList.Select(sortingConfig => sortingConfig.Name).ToList();
            var groupedFilteredAndSortedCommonColumnsList = idColumn.Union(this.GroupingConfiguration.GroupingColumnsList.Union(filteredColumns.Union(sortedColumns)));
            var filteredAndSortedCommonColumnsList = groupedFilteredAndSortedCommonColumnsList as string[] ?? groupedFilteredAndSortedCommonColumnsList.ToArray();
            var collectionTable = new DataTable();
            int level = 0;

            foreach (var groupingColumn in this.GroupingConfiguration.GroupingColumnsList.Where(x => !string.IsNullOrEmpty(x)))
            {
                foreach (var groupingSource in this.GroupingConfiguration.GroupingSources)
                {
                    var currentTable = this.DataSet.Tables[groupingSource].DefaultView.ToTable(
                        true,
                        filteredAndSortedCommonColumnsList.ToArray());

                    if (level++ == 0)
                    {
                        collectionTable = currentTable.Copy();
                    }

                    foreach (DataRow dr in currentTable.Rows)
                    {
                        collectionTable.Rows.Add(dr.ItemArray);
                    }

                    currentTable.Rows.Clear();
                    currentTable.Clear();
                    currentTable.Dispose();
                }

                var groupingTable = collectionTable.DefaultView.ToTable(true);
                collectionTable.Rows.Clear();
                collectionTable.Clear();
                collectionTable.Dispose();
                int requiredNumberOfColumns = this.GroupingTablesDataSet.Tables.Count + 2;

                for (int i = groupingTable.Columns.Count - 1; i > requiredNumberOfColumns - 1; i--)
                {
                    groupingTable.Columns.RemoveAt(i);
                }

                groupingTable.AcceptChanges();
                groupingTable = groupingTable.DefaultView.ToTable(true);
                groupingTable.TableName = string.Format("GROUP" + "{0}", groupingColumn);
                groupingTable.AcceptChanges();
                groupingTable = ApplyGroupingTableSorting(groupingTable);
                this.GroupingTablesDataSet.Tables.Add(groupingTable);
                this.DataSet.AcceptChanges();
            }
        }

        /// <summary>
        /// Moves the grouping tables in to the main data set ready for merge
        /// </summary>
        private void MergeStandardAndGroupingDataSets()
        {
            foreach (DataTable dataTable in this.GroupingTablesDataSet.Tables)
            {
                this.DataSet.Tables.Add(dataTable.Copy());
                this.DataSet.AcceptChanges();
                dataTable.Rows.Clear();
                dataTable.Clear();
                dataTable.Dispose();
            }

            this.GroupingTablesDataSet.Clear();
            this.GroupingTablesDataSet.Dispose();
        }

        /// <summary>
        /// This will set up the recursion environment for parsing the document and building the hierarchy of relationships.
        /// The relationships are defined by BeginGroup/EndGroup merge tags along with TableStart/TableEnd tags.
        /// </summary>
        private void BuildGroupHierarchyFromDocument(IWordDocument wordDocument)
        {
            string rootGroupName = this.Groups[0];

            var found = new List<string>();
            this.RecurseAndBuildHierarchyFromDocument(wordDocument, rootGroupName, this._relationshipHierarchy, found);
        }

        /// <summary>
        /// This performs the recursive parse on all the group/table tags found.
        /// </summary>
        /// <param name="wordDocument">The word document to be processed</param>
        /// <param name="groupName">The current BeginGroup/EndGroup or TableStart/TableEnd</param>
        /// <param name="currentNode">The current node in the hierarchy</param>
        /// <param name="found">The collection of start group tags found i.e. just the BeginGroup and TableStart</param>
        private void RecurseAndBuildHierarchyFromDocument(
            IWordDocument wordDocument,
            string groupName,
            TreeNode<string> currentNode,
            ICollection<string> found)
        {
            TreeNode<string> node;
            string treeValue = groupName.Replace(" ", string.Empty);

            if (currentNode == null)
            {
                this._relationshipHierarchy = new TreeNode<string>(treeValue);
                node = this._relationshipHierarchy;
            }
            else
            {
                if (groupName.StartsWith("GROUP"))
                {
                    int level;
                    int.TryParse(groupName.ToUpper().Replace("GROUP", string.Empty).Trim(), out level);

                    if (level > 0 && level <= this.GroupingConfiguration.GroupingColumnsList.Count)
                    {
                        treeValue = this.GroupingConfiguration.GroupingColumnsList[level - 1];
                    }
                }

                // perform replace operation again since contents may have changed
                node = currentNode.AddChild(treeValue.Replace(" ", string.Empty));
            }

            string[] mergeFields = wordDocument.MailMerge.GetMergeFieldNames(groupName);

            foreach (string mergeField in
                from mergeField in mergeFields
                let selection = wordDocument.Find(mergeField, true, true)
                where selection != null && this.Groups.Contains(mergeField) && !found.Contains(mergeField)
                select mergeField)
            {
                found.Add(mergeField);
                this.RecurseAndBuildHierarchyFromDocument(wordDocument, mergeField, node, found);
            }
        }

        /// <summary>
        /// This function builds a data table with just the summaries reports. The code at present will check report names in the dataset for reports ending with _* or _** or _*** etc. depending on how many summaries there are.
        /// This functionality will change when we move the ability to specify which reports are part of which summaries in the configuration section of the torch project.
        /// It searches the dataset for reports marked for summary with the previously mentioned delimters and adds them in to the merge hierarchy.
        /// </summary>
        private void BuildSummarys()
        {
            string delimiter = SummaryDelimiter;
            DataTable unsortedSummaryTable = null;

            if (this.SummarySortColumns == null)
            {
                // default to ID0 column for sorting
                this.SummarySortColumns = new string[this.NumberOfSummaries];
                for (int i = 0; i < this.NumberOfSummaries; i++)
                {
                    this.SummarySortColumns[i] = "ID0";
                }
            }

            bool summaryFound = false;

            for (int i = 1; i <= this.NumberOfSummaries; i++)
            {
                delimiter += "*";

                foreach (DataTable table in this.DataSet.Tables)
                {
                    if (!table.TableName.EndsWith(delimiter))
                    {
                        continue;
                    }

                    summaryFound = true;

                    if (unsortedSummaryTable == null)
                    {
                        unsortedSummaryTable = table.Copy();
                        unsortedSummaryTable.TableName = string.Format("{0}{1}", AutoSummaryTableName, i);
                    }

                    foreach (DataRow dr in table.Rows)
                    {
                        unsortedSummaryTable.Rows.Add(dr.ItemArray);
                    }
                }

                if (!summaryFound)
                {
                    continue;
                }

                string hackSort = string.Empty;
                if (i == 1)
                {
                    hackSort = " desc, [NameofCompany] asc";
                }

                if (unsortedSummaryTable != null)
                {
                    DataView dataView = unsortedSummaryTable.DefaultView;
                    try
                    {
                        if (!string.IsNullOrEmpty(this.SummarySortColumns[i - 1]) && dataView.Table.Columns.Contains(this.SummarySortColumns[i - 1]))
                        {
                            dataView.Sort = string.Format("[{0}] {1}", this.SummarySortColumns[i - 1], hackSort);
                        }

                        DataTable sortedSummaryTable = dataView.ToTable(true);
                        sortedSummaryTable = ApplyGroupingTableSorting(sortedSummaryTable);
                        this.DataSet.Tables.Add(sortedSummaryTable);
                        this.AddCommand(sortedSummaryTable.TableName, string.Format("ID0=%{0}.ID0%", this.Groups[0]));
                    }
                    finally
                    {
                        unsortedSummaryTable.Rows.Clear();
                        unsortedSummaryTable.Reset();
                        unsortedSummaryTable.Dispose();
                        unsortedSummaryTable = null;
                    }
                }
            }

            this.DataSet.AcceptChanges();
            this.CleanSummaries();
        }

        /// <summary>
        /// Applies the filters to the grouping column tables
        /// </summary>
        /// <param name="tableToFilter">The data table to filter</param>
        private void ApplyGroupingTableFilters(DataTable tableToFilter)
        {
            string currentFilterExpression = string.Empty;

            foreach (TorchGroupingFieldFilter column in this.GroupingConfiguration.FilteringColumns)
            {
                if (tableToFilter.Columns.Contains(column.ColumnName.Replace(" ", string.Empty)))
                {
                    currentFilterExpression = column.GetReverseFilterQuery(column.ColumnName.Replace(" ", string.Empty), this._accountId);
                }

                if (!string.IsNullOrEmpty(currentFilterExpression))
                {
                    tableToFilter.Delete(currentFilterExpression);
                    tableToFilter.AcceptChanges();
                }
            }
        }

        /// <summary>
        /// Applies the sorting to the grouping column tables
        /// </summary>
        /// <param name="tableToSort">The data table to filter</param>
        /// <returns>DataTable sorted</returns>
        private DataTable ApplyGroupingTableSorting(DataTable tableToSort)
        {
            string sortingSql = null;

            foreach (SortingColumn sortingColumn in this.GroupingConfiguration.SortingColumnsList)
            {
                string columnToSort = sortingColumn.Name.Replace(" ", string.Empty);
                if (tableToSort.Columns.Contains(columnToSort))
                {
                    if (sortingColumn.DocumentSortType == "1")
                    {
                        sortingColumn.DocumentSortType = "ASC";
                    }

                    if (sortingColumn.DocumentSortType == "2")
                    {
                        sortingColumn.DocumentSortType = "DESC";
                    }

                    sortingSql += string.Format("{0} {1}, ", columnToSort, sortingColumn.DocumentSortType.ToUpper());
                }
            }

            sortingSql = RemoveLast(sortingSql, ", ");
            tableToSort.DefaultView.Sort = sortingSql;
            tableToSort.DefaultView.ApplyDefaultSort = true;
            tableToSort = tableToSort.DefaultView.ToTable();
            return tableToSort;
        }

        /// <summary>
        /// Performs all pre processing required before the merge engine can start work.
        /// </summary>
        private void PreProcess(IWordDocument wordDocument)
        {
            this.Groups = wordDocument.MailMerge.GetMergeGroupNames().ToList();
            this.BuildGroupHierarchyFromDocument(wordDocument);
            this.PreProcessGroups();
            this.PreProcessDataSet();
            this.PreProcessConfiguration();
            this.RenameGroupTagsInDocument(wordDocument);
            this.RefineDataSet();
        }

        private void RefineDataSet()
        {
            string rootLevelId = this.DataSet.Tables[this.Groups[0]].Rows[0]["ID0"].ToString();

            foreach (DataTable dataTable in this.DataSet.Tables.Cast<DataTable>().Where(dataTable => dataTable.Columns.Count > 0 && dataTable.Columns.Contains("ID0")))
            {
                dataTable.Delete(string.Format("ID0<>{0}", rootLevelId));
                dataTable.AcceptChanges();
            }
        }

        /// <summary>
        /// Removes spaces from all group names
        /// </summary>
        private void PreProcessGroups()
        {
            for (int i = 0; i < this.Groups.Count; i++)
            {
                this.Groups[i] = this.Groups[i].Replace(" ", string.Empty);
            }
        }

        /// <summary>
        /// Pre process all data tables
        /// </summary>
        private void PreProcessDataSet()
        {
            for (int i = 0; i < this.DataSet.Tables.Count; i++)
            {
                DataTable dataTable = this.DataSet.Tables[i];
                DataTableDeSpacerize(dataTable);
            }
        }

        /// <summary>
        /// Pre process as much as possible within the configuration.
        /// </summary>
        private void PreProcessConfiguration()
        {
            if (this.GroupingConfiguration.GroupingColumnsList.Count == 0)
            {
                this.GroupingConfiguration.FilteringColumns.Clear();
                this.GroupingConfiguration.SortingColumnsList.Clear();
                this.GroupingConfiguration.GroupingSources.Clear();
            }

            if (this.GroupingConfiguration.GroupingSources.Count > 0)
            {
                for (int i = 0; i < this.GroupingConfiguration.GroupingSources.Count; i++)
                {
                    this.GroupingConfiguration.GroupingSources[i] =
                        this.GroupingConfiguration.GroupingSources[i].Replace(" ", string.Empty);
                }
            }

            if (this.GroupingConfiguration.GroupingColumnsList.Count > 0)
            {
                if (this.GroupingConfiguration.GroupingColumnsList.Contains(""))
                {
                    throw new Exception("Grouping columns are invalid. Please provide a valid configuration");
                }

                for (int i = 0; i < this.GroupingConfiguration.GroupingColumnsList.Count; i++)
                {
                    this.GroupingConfiguration.GroupingColumnsList[i] =
                        this.GroupingConfiguration.GroupingColumnsList[i].Replace(" ", string.Empty);
                }
            }

            if (this.GroupingConfiguration.SortingColumnsList.Count > 0)
            {
                foreach (SortingColumn column in this.GroupingConfiguration.SortingColumnsList)
                {
                    column.Name = column.Name.Replace(" ", string.Empty);
                }
            }

            if (this.GroupingConfiguration.ReportSortingConfigurations.Count > 0)
            {
                foreach (TorchReportSorting reportSortingConfiguration in this.GroupingConfiguration.ReportSortingConfigurations)
                {
                    reportSortingConfiguration.ReportName = reportSortingConfiguration.ReportName.Replace(" ", string.Empty).Replace(SortedTableSuffix, string.Empty);
                    foreach (TorchReportSortingColumn torchReportSortingColumn in reportSortingConfiguration.TorchReportSortingColumns)
                    {
                        torchReportSortingColumn.ColumnName = torchReportSortingColumn.ColumnName.Replace(
                            " ",
                            string.Empty);
                    }
                }
            }

            if (this.GroupingConfiguration.FilteringColumns.Count > 0)
            {
                foreach (TorchGroupingFieldFilter torchGroupingFieldFilter in this.GroupingConfiguration.FilteringColumns)
                {
                    torchGroupingFieldFilter.ColumnName = torchGroupingFieldFilter.ColumnName.Replace(" ", string.Empty);
                }
            }
        }

        /// <summary>
        /// Removes spaces from all column names in a datatable, then removes any spaces from the table name itself.
        /// </summary>
        /// <param name="dataTable">The data table to process</param>
        /// <returns>The data table with spaces removed from the columns and table name.</returns>
        public static DataTable DataTableDeSpacerize(DataTable dataTable)
        {
            foreach (DataColumn dataColumn in dataTable.Columns)
            {
                dataColumn.ColumnName = dataColumn.ColumnName.Replace(" ", string.Empty);
            }

            dataTable.TableName = dataTable.TableName.Replace(" ", string.Empty);

            return dataTable;
        }

        private static void CreateTableOfContents(WordDocument document)
        {
            var tableOfContentsCollection = document.FindAll(typeof(TableOfContent));
            if (tableOfContentsCollection.Count != 0)
            {
                foreach (TableOfContent tableOfContent in tableOfContentsCollection)
                {
                    IWParagraph tableOfContentsParagraph = tableOfContent.OwnerParagraph;
                    var fields = document.FindAll(typeof(WField));

                    foreach (WField entity in fields)
                    {
                        if (entity.Owner is ICompositeEntity && (entity.FieldType == Syncfusion.DocIO.FieldType.FieldHyperlink || entity.FieldType == Syncfusion.DocIO.FieldType.FieldPageRef) && entity.FieldValue.Contains("_Toc"))
                        {
                            var entityOwner = (ICompositeEntity)entity.Owner;
                            int entityOwnerIndex;
                            RemoveEntityFromOwner(entityOwner.Owner as ICompositeEntity, entityOwner, out entityOwnerIndex);
                        }
                    }

                    CreateNewTableOfContentsFromExistingTableOfContents(document, tableOfContentsParagraph, tableOfContent);
                    if (!GlobalVariables.GetAppSettingAsBoolean("DocMergeSkipTableOfContents"))
                    {
                        try
                        {
                            document.UpdateTableOfContents();
                        }
                        catch (Exception e)
                        {
                        }
                    }
                }
            }
        }

        private static EntityCollection RemoveEntityFromOwner(ICompositeEntity paragraphOwner, IEntity tableOfContentsParagraph,
            out int tableOfContentsParagraphIndex)
        {
            if (paragraphOwner != null)
            {
                var siblings = paragraphOwner.ChildEntities;
                tableOfContentsParagraphIndex = siblings.IndexOf(tableOfContentsParagraph);
                siblings.Remove(tableOfContentsParagraph);
                return siblings;
            }
            tableOfContentsParagraphIndex = -1;
            return null;
        }

        private static void AddHeadingStyle(IWordDocument document, TableOfContent tableOfContents, string headingName, int level)
        {
            var stylesFound = document.FindAll(document.Styles.FindByName(headingName));
            if (stylesFound.Count > 0)
            {
                tableOfContents.SetTOCLevelStyle(level, headingName);
            }
        }

        private static void CreateNewTableOfContentsFromExistingTableOfContents(IWordDocument document, IWParagraph tableofContentsParagraph, TableOfContent existingTableOfContent)
        {
            tableofContentsParagraph.ApplyStyle(BuiltinStyle.Hyperlink);
            var formattingString =
                (string)
                    existingTableOfContent.GetType()
                        .GetProperty("FormattingString", BindingFlags.NonPublic | BindingFlags.Instance)
                        .GetValue(existingTableOfContent, null);
            TableOfContent tableOfContents = tableofContentsParagraph.AppendTOC(
                existingTableOfContent.LowerHeadingLevel, existingTableOfContent.UpperHeadingLevel);
            tableOfContents.IncludePageNumbers = existingTableOfContent.IncludePageNumbers;
            tableOfContents.RightAlignPageNumbers = existingTableOfContent.RightAlignPageNumbers;
            tableOfContents.UseHyperlinks = existingTableOfContent.UseHyperlinks;
            tableOfContents.LowerHeadingLevel = existingTableOfContent.LowerHeadingLevel;
            tableOfContents.UpperHeadingLevel = existingTableOfContent.UpperHeadingLevel;
            tableOfContents.UseOutlineLevels = existingTableOfContent.UseOutlineLevels;
            tableOfContents.UseHeadingStyles = existingTableOfContent.UseHeadingStyles;
            tableOfContents.UseTableEntryFields = existingTableOfContent.UseTableEntryFields;
            UpdateHeadingStyles(document, tableOfContents, formattingString);
        }

        private static void UpdateHeadingStyles(IWordDocument document, TableOfContent tableOfContents, string formattingString)
        {
            var formatSplit = formattingString.Split('"');
            if (formatSplit.Length == 3)
            {
                var headings = formatSplit[1].Split(',');
                if (headings.Length % 2 == 0)
                {
                    var highestLevel = 1;
                    for (int i = 0; i < headings.Length; i = i + 2)
                    {
                        var heading = headings[i];
                        var level = headings[i + 1];
                        var intLevel = int.Parse(level);
                        if (intLevel > highestLevel)
                        {
                            highestLevel = intLevel;
                        }

                        AddHeadingStyle(document, tableOfContents, heading, intLevel);
                    }

                    tableOfContents.UpperHeadingLevel = highestLevel;
                }
            }
        }

        private static void RemoveEmptyParagraphs(IWSection curSection)
        {
            int j = 0;
            int paraCount = 0;
            while (j < curSection.Body.ChildEntities.Count)
            {
                //Read the section text body
                WTextBody textBody = curSection.Body;

                //Read all the body items collection
                var textbodycollection = textBody.ChildEntities as BodyItemCollection;
                if (textbodycollection != null && textbodycollection[j].EntityType == EntityType.Paragraph)
                {
                    var paragraph = textbodycollection[j] as IWParagraph;
                    bool remove = true;

                    if (paragraph != null && paragraph.Items.Count == 0)
                    {
                        if (j > 0)
                        {
                            remove = !(j > 0) && textbodycollection[j - 1].EntityType == EntityType.Table;
                        }

                        if (remove)
                        {
                            curSection.Paragraphs.RemoveAt(paraCount);
                        }
                        else
                        {
                            paraCount++;
                            j++;
                        }
                    }
                    else
                    {
                        paraCount++;
                        j++;
                    }
                }
                else
                {
                    j++;
                }
            }
        }

        /// <summary>
        /// Performs the nested mail merge usingk the dataset and the commands array
        /// </summary>
        /// <param name="wordDocument">The syncfusion docio word document</param>
        public void ExecuteNestedMerge(WordDocument wordDocument)
        {
            wordDocument.SaveOptions.MaintainCompatibilityMode = true;
            SetupEventHandlers(wordDocument);
            this.PreProcess(wordDocument);
            this.FilterDataTables();
            this.CreateGroupingDataTables();
            this.CreateGroupingJoins();
            this.RecurseAndCreateStandardJoins(this._relationshipHierarchy, 0);
            this.CombineStandardAndGroupJoins();
            this.MergeStandardAndGroupingDataSets();
            this.SortStandardDataTables();
            this.BuildSummarys();
            this.RemoveSuperflousDataTables();
            this.OutputDebugInformation();
            wordDocument.MailMerge.ClearFields = true;
            wordDocument.MailMerge.RemoveEmptyGroup = true;
            wordDocument.MailMerge.RemoveEmptyParagraphs = true;
            try
            {
                wordDocument.MailMerge.ExecuteNestedGroup(this.DataSet, this.Commands);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }

        private void FilterDataTables()
        {
            foreach (DataTable dataTable in this.DataSet.Tables)
            {
                this.ApplyGroupingTableFilters(dataTable);
            }
        }

        private void OutputDebugInformation()
        {
            if (ConfigurationManager.AppSettings["TorchDataDebugLocation"] != null)
            {
                string dataLocation = ConfigurationManager.AppSettings["TorchDataDebugLocation"];

                dataLocation = dataLocation.EndsWith("\\")
                    ? dataLocation
                    : dataLocation + "\\";

                this.DataSet.WriteXml(dataLocation + "data.xml");
                var sb = new StringBuilder();
                foreach (string @group in this.Groups)
                {
                    sb.Append(@group);
                    sb.Append(Environment.NewLine);
                }

                File.WriteAllText(dataLocation + "groups.txt", sb.ToString());


                sb = new StringBuilder();
                foreach (DictionaryEntry command in this.Commands)
                {
                    sb.Append($"Key[{command.Key}]Value[{command.Value}]");
                    sb.Append(Environment.NewLine);
                }

                File.WriteAllText(dataLocation + "commands.txt", sb.ToString());
            }
        }

        private void RemoveSuperflousDataTables()
        {
            for (int i = this.DataSet.Tables.Count - 1; i >= 0; i--)
            {
                bool found = this.Commands.Cast<DictionaryEntry>().Any(dictionaryEntry => String.Equals(dictionaryEntry.Key.ToString(), this.DataSet.Tables[i].TableName, StringComparison.CurrentCultureIgnoreCase));

                if (this.DataSet.Tables[i].TableName != this.Groups[0] && !found)
                {
                    this.DataSet.Tables.RemoveAt(i);
                }
            }

            this.DataSet.AcceptChanges();
        }

        /// <summary>
        /// Removes the last occurence of the specified string
        /// </summary>
        /// <param name="original"></param>
        /// <param name="removeLastWhat"></param>
        /// <returns></returns>
        private static string RemoveLast(string original, string removeLastWhat)
        {
            if (String.IsNullOrEmpty(original))
            {
                return string.Empty;
            }

            string returnString = original;

            if (returnString.EndsWith(removeLastWhat))
            {
                returnString = returnString.Remove(returnString.LastIndexOf(removeLastWhat, StringComparison.Ordinal));
            }

            return returnString;
        }

        /// <summary>
        /// Functionality to perform immediately after the nested merge.
        /// </summary>
        /// <param name="wordDocument"></param>
        /// <param name="blankTableText">The text to use in place of removed summary tables.</param>
        public void PostProcessDocument(WordDocument wordDocument, string blankTableText)
        {
            for (int i = wordDocument.Sections.Count - 1; i >= 0; i--)
            {
                IWSection currentSection = wordDocument.Sections[i];
                RemoveEmptyParagraphs(currentSection);
                PostProcessTables(currentSection.ChildEntities, blankTableText);
            }

            RemoveEmptyParagraphsAndSiblingAutoNumberFields(wordDocument);
            foreach (var group in Groups)
            {
                RemoveParagraphsBasedOnSearch(wordDocument, $"«EndGroup:{group}»");
                RemoveParagraphsBasedOnSearch(wordDocument, $"«BeginGroup:{group}»");
            }
            
            wordDocument.AcceptChanges();
            CreateTableOfContents(wordDocument);
        }

        private static List<WParagraph> FindRepeatingEmptyParagraphs(WParagraph currentParagraph)
        {
            var paragraphsToRemove = new List<WParagraph>();
            if (!ParagraphIsBlank(currentParagraph))
            {
                return paragraphsToRemove;
            }

            var sibling = currentParagraph.NextSibling;

            while (sibling != null && sibling.EntityType == EntityType.Paragraph)
            {
                if (sibling.EntityType == EntityType.Paragraph && ParagraphIsBlank((WParagraph)sibling))
                {
                    paragraphsToRemove.Add((WParagraph)sibling);
                }

                sibling = sibling.NextSibling;
            }

            return paragraphsToRemove;
        }

        private static bool ParagraphIsBlank(WParagraph paragraph)
        {
            if (paragraph.Text == string.Empty && !paragraph.IsEndOfSection && !paragraph.IsEndOfDocument && !paragraph.IsInCell && paragraph.ChildEntities.Count == 0)
            {
                return true;
            }

            return false;
        }

        private static void PostProcessTables(EntityCollection childEntities, string blankTableText)
        {
            for (int i = childEntities.Count - 1; i >= 0; i--)
            {
                var childEntity = childEntities[i];
                if (childEntity.EntityType == EntityType.Table)
                {
                    var table = childEntity as WTable;

                    if (table != null)
                    {
                        string tableType = table.Title == null
                            ? string.Empty
                            : table.Title.Trim().ToUpper().Replace(" ", string.Empty);

                        if (tableType == string.Empty)
                        {
                            if (table.Rows.Count == 1 && table.LastCell.Tables.Count > 0)
                            {
                                // switch to the nested table
                                table = (WTable)table.LastCell.Tables[0];
                            }

                            // standard report table
                            for (int r = table.Rows.Count - 1; r >= 0; r--)
                            {
                                int populatedCells =
                                    table.Rows[r].Cells.Cast<WTableCell>()
                                        .Count(
                                            cell =>
                                                cell.Paragraphs.Count > 0
                                                && cell.Paragraphs[0].Text.Trim() != string.Empty);
                                int numberOfVerticallyMergedDefinitionColumns = 0;
                                bool foundToken = false;

                                for (int z = 0; z < table.Rows[r].Cells.Count; z++)
                                {
                                    if (table.Rows[r].Cells[z].CellFormat.VerticalMerge == CellMerge.Continue)
                                    {
                                        numberOfVerticallyMergedDefinitionColumns++;
                                    }

                                    if (table.Rows[r].Cells[z].Paragraphs.Count > 0 && table.Rows[r].Cells[z].Paragraphs[0].Text.Contains(RemoveTableRowToken))
                                    {
                                        foundToken = true;
                                        break;
                                    }
                                }

                                if (foundToken)
                                {
                                    if (table.Rows.Count > 1)
                                    {
                                        table.Rows.RemoveAt(r);
                                    }
                                    else
                                    {
                                        table.OwnerTextBody.ChildEntities.Remove(table);
                                        break;
                                    }
                                }
                                else
                                {
                                    if (populatedCells + numberOfVerticallyMergedDefinitionColumns
                                        < table.Rows[r].Cells.Count)
                                    {
                                        RemoveTableRow(table, table.Rows[r]);
                                    }
                                }
                            }
                        }
                        else
                        {
                            var emptyParagraph = new WParagraph(table.Document);
                            emptyParagraph.Text = tableType != "SUMMARYTABLE" || string.IsNullOrEmpty(blankTableText)
                                ? string.Empty
                                : blankTableText;

                            // summary table etc...
                            if (table.Rows.Count == 1)
                            {
                                if (!string.IsNullOrEmpty(emptyParagraph.Text))
                                {
                                    table.OwnerTextBody.ChildEntities.Insert(
                                        table.OwnerTextBody.ChildEntities.IndexOf(table), emptyParagraph);
                                }
                                table.OwnerTextBody.ChildEntities.Remove(table);
                            }

                            bool remove = false;

                            if (table.Rows.Count == 2)
                            {
                                WTableRow tableRow = table.Rows[1];

                                if (
                                    tableRow.Cells.Cast<WTableCell>()
                                        .All(tableCell => tableCell.Paragraphs[0].Text.Trim() == string.Empty))
                                {
                                    remove = true;
                                }

                                if (
                                    tableRow.Cells.Cast<WTableCell>()
                                        .All(tableCell => tableCell.Paragraphs[0].Text.Trim().Contains("«")))
                                {
                                    remove = true;
                                }

                                if (remove)
                                {
                                    if (!string.IsNullOrEmpty(emptyParagraph.Text))
                                    {
                                        table.OwnerTextBody.ChildEntities.Insert(
                                            table.OwnerTextBody.ChildEntities.IndexOf(table), emptyParagraph);
                                    }
                                    table.OwnerTextBody.ChildEntities.Remove(table);
                                }
                            }
                        }
                    }
                }

                var compositeEntity = childEntity as ICompositeEntity;
                if (compositeEntity != null)
                {
                    PostProcessTables(compositeEntity.ChildEntities, blankTableText);
                }
            }
            var paragraphsToRemove = new List<WParagraph>();

            for (int i = childEntities.Count - 1; i >= 0; i--)
            {
                var childEntity = childEntities[i];
                if (childEntity.EntityType == EntityType.Table)
                {
                    var table = childEntity as WTable;

                    if (table != null)
                    {
                        string tableType = table.Title == null
                            ? string.Empty
                            : table.Title.Trim().ToUpper().Replace(" ", string.Empty);

                        if (tableType == string.Empty)
                        {
                            if (table.Rows.Count == 1 && table.LastCell.Paragraphs[0].Text.Trim() == string.Empty && (table.LastCell.Tables.Count == 0 || table.LastCell.Tables[0].LastCell.Paragraphs[0].Text.Trim() == string.Empty))
                            {
                                table.OwnerTextBody.ChildEntities.Remove(table);
                            }
                        }
                    }

                    var sibling = childEntity.NextSibling;
                    if (sibling != null && sibling.EntityType == EntityType.Paragraph)
                    {
                        paragraphsToRemove.AddRange(FindRepeatingEmptyParagraphs((WParagraph)childEntity.NextSibling));
                    }

                }
            }
            foreach (WParagraph paragraph in paragraphsToRemove)
            {
                var owner = (WTextBody)paragraph.Owner;
                owner.ChildEntities.Remove(paragraph);
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // Not really necessary now that the class doesn't own unmanaged resources,
        // but kudos to manny for asking the important question "do we need a finaliser aka defensive programming"?
        ~TorchDocIoMergeEngine()
        {
            // Finalizer calls Dispose(false)
            Dispose(false);
        }

        public virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.Commands.Clear();
                this._relationshipHierarchy = null;

                foreach (DataTable table in this.DataSet.Tables)
                {
                    table.Clear();
                    table.Reset();
                    table.Dispose();
                }

                this.DataSet.Tables.Clear();
                this.DataSet.Clear();
                this.DataSet.Reset();
                this.DataSet.Dispose();

                foreach (DataTable table in this.GroupingTablesDataSet.Tables)
                {
                    table.Clear();
                    table.Reset();
                    table.Dispose();
                }

                this.GroupingTablesDataSet.Tables.Clear();
                this.GroupingTablesDataSet.Clear();
                this.GroupingTablesDataSet.Reset();
                this.GroupingTablesDataSet.Dispose();
            }
        }

        private static void OnImageMerge(object sender, MergeImageFieldEventArgs args)
        {
            string fileName = args.FieldValue.ToString();

            if (!string.IsNullOrEmpty(fileName))
            {
                try
                {
                    args.Image = System.Drawing.Image.FromFile(fileName, true);
                }
                catch (Exception)
                {
                    if (args.Text.Contains("<div"))
                    {
                        args.CurrentMergeField.OwnerParagraph.AppendHTML(args.Text);
                    }
                    else
                    {
                        args.Text = string.Empty;    
                    }
                }
            }
            else
            {
                args.Text = string.Empty;
            }
        }

        /// <summary>
        ///  Replaces empty values with an empty placeholder value, runs every time an MS Word merge field is merged
        /// </summary>
        /// <param name="sender">The sender object</param>
        /// <param name="arguments">The merge field event arguments</param>
        private static void OnFieldMerge(object sender, MergeFieldEventArgs arguments)
        {
            if (arguments.Text != string.Empty)
            {
                return;
            }

            Entity fieldOwner = arguments.CurrentMergeField.Owner;

            if (fieldOwner.EntityType != EntityType.Paragraph)
            {
                return;
            }

            if (fieldOwner.Owner == null || fieldOwner.Owner.EntityType != EntityType.TableCell)
            {
                var ownerParagraph = fieldOwner as IWParagraph;

                if (ownerParagraph != null && ownerParagraph.ListFormat.ListType == ListType.Numbered)
                {
                    arguments.Text = string.Format("[{0}] not supplied", arguments.FieldName);
                }

                if (ownerParagraph != null && ownerParagraph.PreviousSibling.EntityType == EntityType.Paragraph && ownerParagraph.Items.Count == 1)
                {
                    var previousSibling = ownerParagraph.PreviousSibling as WParagraph;

                    if (previousSibling != null && previousSibling.ListFormat.ListType == ListType.Numbered && previousSibling.ListFormat.ListLevelNumber > 0)
                    {
                        if (ownerParagraph.NextSibling.EntityType == EntityType.Paragraph)
                        {
                            var nextSibling = ownerParagraph.NextSibling as WParagraph;

                            if (nextSibling != null && (nextSibling.ListFormat.ListType == ListType.Numbered || nextSibling.Text == string.Empty))
                            {
                                var selectionContainer = ownerParagraph.Owner as WTextBody;

                                if (selectionContainer != null)
                                {
                                    arguments.Text = RemoveWithPreviousSiblingToken;
                                }
                            }
                        }
                    }
                }

                return;
            }

            var tableCell = fieldOwner.Owner as WTableCell;
            int cellIndex = -1;

            if (tableCell != null)
            {
                cellIndex = tableCell.GetCellIndex();
            }

            if (fieldOwner.Owner.Owner == null || fieldOwner.Owner.Owner.EntityType != EntityType.TableRow)
            {
                return;
            }

            var tableRow = fieldOwner.Owner.Owner as WTableRow;
            if (tableRow == null)
            {
                return;
            }

            int numberOfCellsInRow = tableRow.Cells.Count;

            if (numberOfCellsInRow != cellIndex + 1)
            {
                return;
            }

            var table = tableRow.Owner as WTable;
            if (table == null)
            {
                return;
            }

            if (table.Title != null)
            {
                return;
            }

            string token = RemoveTableRow(table, tableRow);

            if (token != string.Empty)
            {
                arguments.Text = token;
            }
        }

        private static string RemoveTableRow(WTable table, WTableRow tableRow)
        {
            if (table.Title != null && table.Title.Trim() != string.Empty)
            {
                // non standard report table
                if (table.Rows.Count == 1)
                {
                    RemoveEmptyParagraphAfterTable(table);
                    table.OwnerTextBody.ChildEntities.Remove(table);
                }
                else
                {
                    table.Rows.Remove(tableRow);
                }
            }
            else
            {
                // standard report table
                if (table.Rows.Count == 1)
                {
                    //table.OwnerTextBody.ChildEntities.Remove(table);
                }
                else
                {
                    table.Rows.Remove(tableRow);
                }
            }

            return string.Empty;
        }

        private static void RemoveEmptyParagraphsAndSiblingAutoNumberFields(WordDocument wordDocument)
        {
            TextSelection[] textSelections = wordDocument.FindAll(RemoveWithPreviousSiblingToken, true, true);
            if (textSelections != null && textSelections.Any())
            {
                foreach (TextSelection textSelection in textSelections)
                {
                    WParagraph paragraph = textSelection.GetAsOneRange().OwnerParagraph;
                    var previousSibling = paragraph.PreviousSibling as WParagraph;
                    if (previousSibling != null)
                    {
                        paragraph.OwnerTextBody.ChildEntities.Remove(previousSibling);
                        paragraph.OwnerTextBody.ChildEntities.Remove(paragraph);
                    }
                }
            }
        }

        /// <summary>
        /// Search for specific text and remove any paragraphs containing that text.
        /// </summary>
        /// <param name="wordDocument">The <see cref="WordDocument"/>to search</param>
        /// <param name="search">The <see cref="string"/> to search for</param>
        private static void RemoveParagraphsBasedOnSearch(WordDocument wordDocument, string search)
        {
            TextSelection[] textSelections = wordDocument.FindAll(search, true, true);
            if (textSelections != null && textSelections.Any())
            {
                foreach (TextSelection textSelection in textSelections)
                {
                    WParagraph paragraph = textSelection.GetAsOneRange().OwnerParagraph;
                    if (paragraph != null)
                    {
                        paragraph.OwnerTextBody.ChildEntities.Remove(paragraph);
                    }
                }
            }
        }


        private static void RemoveEmptyParagraphAfterTable(IEntity table)
        {
            if (table.NextSibling.EntityType != EntityType.Paragraph)
            {
                return;
            }

            var paragraph = table.NextSibling as IWParagraph;
            if (paragraph == null || paragraph.Text.Trim() != string.Empty
                || paragraph.ListFormat.ListType == ListType.Numbered || paragraph.IsEndOfSection || paragraph.IsEndOfDocument || paragraph.IsInCell)
            {
                return;
            }

            var owner = paragraph.Owner as WTextBody;
            if (owner != null)
            {
                owner.ChildEntities.Remove(paragraph);
            }
        }

        private static void SetupEventHandlers(IWordDocument wordDocument)
        {
            wordDocument.MailMerge.MergeImageField += OnImageMerge;
            wordDocument.MailMerge.MergeField += OnFieldMerge;
        }
    }
}
