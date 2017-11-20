namespace BusinessLogic.QueryBuilder
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    using BusinessLogic.Fields;
    using BusinessLogic.Fields.Type;
    using BusinessLogic.Fields.Type.Base;
    using BusinessLogic.FinancialYears;
    using BusinessLogic.JoinVia;
    using BusinessLogic.QueryBuilder.Conditions;
    using BusinessLogic.Tables;

    using QueryFields;

    /// <summary>
    /// an instance of <see cref="IQueryBuilder"/>.
    /// </summary>
    public abstract class QueryBuilder : IQueryBuilder
    {
        /// <summary>
        /// The _index.
        /// </summary>
        private int _index;

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryBuilder"/> class. 
        /// </summary>
        /// <param name="tableRepository">
        /// An instance of <see cref="TableRepository"/>
        /// </param>
        /// <param name="fieldRepository">
        /// An instance of <see cref="FieldRepository"/>
        /// </param>
        /// <param name="conditionRepository">
        /// An instance of <see cref="ConditionRepository"/>
        /// </param>
        /// <param name="financialYearRepository">
        /// An instance of <see cref="FinancialYearRepository"/>
        /// </param>
        protected QueryBuilder(TableRepository tableRepository, FieldRepository fieldRepository, ConditionRepository conditionRepository, FinancialYearRepository financialYearRepository)
        {
            this.TableRepository = tableRepository;
            this.FieldRepository = fieldRepository;
            this.ConditionRepository = conditionRepository;
            this.FinancialYearRepository = financialYearRepository;
            this.QueryFields = new List<IQueryField>();
            this.ConditionFields = new List<ICondition>();
            this.QueryFilterGroups = new List<IQueryFilterGroup>();
            this.QueryFilterStrings = new List<IQueryFilterString>();
        }

        /// <summary>
        /// The select type of each field.
        /// </summary>
        public enum SelectType
        {
            Fields,
            Count,
            Distinct,
            Min,
            Max,
            Sum,
            NoSort,
            Ascending,
            Descending
        }

        public enum SortDirection
        {
            None,

            Ascending,

            Descending
        }

        /// <summary>
        /// Gets the list of <see cref="ICondition"/> used in this <see cref="IQueryBuilder"/>
        /// </summary>
        public ReadOnlyCollection<ICondition> Conditions => this.ConditionFields.AsReadOnly();

        /// <summary>
        /// Gets the list of <see cref="IQueryFilterGroup"/> in the <see cref="IQueryBuilder"/>
        /// </summary>
        public ReadOnlyCollection<IQueryFilterGroup> FilterGroups => this.QueryFilterGroups.AsReadOnly();

        /// <summary>
        /// Gets or sets the query fields.
        /// </summary>
        protected List<IQueryField> QueryFields { get; set; }

        /// <summary>
        /// Gets or sets the condition fields.
        /// </summary>
        protected List<ICondition> ConditionFields { get; set; }

        /// <summary>
        /// Gets or sets the query filter groups.
        /// </summary>
        protected List<IQueryFilterGroup> QueryFilterGroups { get; set; }

        /// <summary>
        /// Gets the table repository.
        /// </summary>
        protected TableRepository TableRepository { get;  }

        /// <summary>
        /// Gets the field repository.
        /// </summary>
        protected FieldRepository FieldRepository { get;  }

        /// <summary>
        /// Gets the condition repository.
        /// </summary>
        protected ConditionRepository ConditionRepository { get;  }

        /// <summary>
        /// Gets the financial year repository.
        /// </summary>
        protected FinancialYearRepository FinancialYearRepository { get;  }

        /// <summary>
        /// Gets or sets the query filter strings.
        /// </summary>
        protected List<IQueryFilterString> QueryFilterStrings { get; set; }

        /// <summary>
        /// Add a <see cref="IField"/> to the query
        /// </summary>
        /// <param name="field">The <see cref="IField"/> to add</param>
        public void AddColumn(IField field)
        {
            this.AddColumn(field, null, string.Empty, false, SelectType.Fields);
        }

        /// <summary>
        /// Add a <see cref="IField"/> to the query
        /// </summary>
        /// <param name="field">The <see cref="IField"/> to add</param>
        /// <param name="alias">The alias of the <see cref="IField"/></param>
        public void AddColumn(IField field, string alias)
        {
            this.AddColumn(field, null, alias, false, SelectType.Fields);
        }

        /// <summary>
        /// Add a <see cref="IField"/> to the query
        /// </summary>
        /// <param name="field">The <see cref="IField"/> to add</param>
        /// <param name="selType">The <see cref="QueryBuilder.SelectType"/> to use</param>
        public void AddColumn(IField field, SelectType selType)
        {
            this.AddColumn(field, null, string.Empty, false, selType);
        }

        /// <summary>
        /// Add a <see cref="IField"/> to the query
        /// </summary>
        /// <param name="field">The <see cref="IField"/> to add</param>
        /// /// <param name="alias">The alias of the <see cref="IField"/></param>
        /// <param name="selType">The <see cref="QueryBuilder.SelectType"/> to use</param>
        public void AddColumn(IField field, string alias, SelectType selType)
        {
            this.AddColumn(field, null, alias, false, selType);
        }

        /// <summary>
        /// Add a <see cref="IField"/> to the query
        /// </summary>
        /// <param name="field">The <see cref="IField"/> to add</param>
        /// <param name="useListItemText">True to use the List Item text and not the item ID.</param>
        public void AddColumn(IField field, bool useListItemText)
        {
            this.AddColumn(field, null, string.Empty, useListItemText, SelectType.Fields);
        }

        /// <summary>
        /// Add a <see cref="IField"/> to the query
        /// </summary>
        /// <param name="field">The <see cref="IField"/> to add</param>
        /// <param name="alias">The alias of the <see cref="IField"/></param>
        /// <param name="useListItemText">True to use the List Item text and not the item ID.</param>
        public void AddColumn(IField field, string alias, bool useListItemText)
        {
            this.AddColumn(field, null, alias, useListItemText, SelectType.Fields);
        }

        /// <summary>
        /// Add a <see cref="IField"/> to the query
        /// </summary>
        /// <param name="field">The <see cref="IField"/> to add</param>
        /// <param name="joinVia">The <see cref="JoinVia"/> to use to get the data.</param>
        /// <param name="alias">The alias of the <see cref="IField"/></param>
        public void AddColumn(IField field, JoinVia joinVia, string alias)
        {
            this.AddColumn(field, joinVia, alias, false, SelectType.Fields);
        }

        /// <summary>
        /// Add a <see cref="IField"/> to the query
        /// </summary>
        /// <param name="field">The <see cref="IField"/> to add</param>
        /// <param name="joinVia">The <see cref="JoinVia"/> to use to get the data.</param>
        public void AddColumn(IField field, JoinVia joinVia)
        {
            this.AddColumn(field, joinVia, string.Empty, false, SelectType.Fields);
        }

        /// <summary>
        /// Add a <see cref="IField"/> that is sortable.
        /// </summary>
        /// <param name="field">The <see cref="IField"/> to add</param>
        /// <param name="sortdirection">The <see cref="QueryBuilder.SortDirection"/> to apply.</param>
        public void AddSortableColumn(IField field, SortDirection sortdirection)
        {
            this.AddSortableColumn(field, null, string.Empty, sortdirection);
        }

        /// <summary>
        /// Add a <see cref="IField"/> that is sortable.
        /// </summary>
        /// <param name="field">The <see cref="IField"/> to add</param>
        /// <param name="sortdirection">The <see cref="QueryBuilder.SortDirection"/> to apply.</param>
        /// <param name="alias">The column alias to use.</param>
        public void AddSortableColumn(IField field, SortDirection sortdirection, string alias)
        {
            this.AddSortableColumn(field, null, alias, sortdirection);
        }

        /// <summary>
        /// Add a <see cref="IField"/> that is sortable.
        /// </summary>
        /// <param name="field">The <see cref="IField"/> to add</param>
        /// <param name="sortdirection">The <see cref="QueryBuilder.SortDirection"/> to apply.</param>
        /// <param name="joinVia">The <see cref="JoinVia"/> to use to get the data.</param>
        public void AddSortableColumn(IField field, SortDirection sortdirection, JoinVia joinVia)
        {
            this.AddSortableColumn(field, joinVia, string.Empty, sortdirection);
        }

        /// <summary>
        /// The add sortable column.
        /// </summary>
        /// <param name="field">
        /// The field.
        /// </param>
        /// <param name="joinVia">
        /// The join via.
        /// </param>
        /// <param name="alias">
        /// The alias.
        /// </param>
        /// <param name="sortdirection">
        /// The sortdirection.
        /// </param>
        public void AddSortableColumn(IField field, JoinVia joinVia, string alias, SortDirection sortdirection)
        {
            SelectType sort;
            switch (sortdirection)
            {
                case SortDirection.None:
                    sort = SelectType.NoSort;
                    break;
                case SortDirection.Ascending:
                    sort = SelectType.Ascending;
                    break;
                case SortDirection.Descending:
                    sort = SelectType.Descending;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(sortdirection), sortdirection, null);
            }

            this.AddColumn(field, joinVia, alias, false, sort);
        }

        /// <summary>
        /// Add a static column to the <see cref="IQueryBuilder"/>
        /// </summary>
        /// <param name="staticValue">The Value to display</param>
        /// <param name="staticFieldname">The column name</param>
        public void AddStaticColumn(string staticValue, string staticFieldname)
        {
            var queryField = new Queryfield(new StaticField(staticFieldname, staticValue), string.Empty, null, false);
            this.QueryFields.Add(queryField);
        }

        /// <summary>
        /// Add a filter to the <see cref="IQueryFilters"/> with no join.
        /// </summary>
        /// <typeparam name="T">The <see cref="ICondition"/> to use for this Filter</typeparam>
        /// <param name="field">The <see cref="IField"/> to apply the filter to.</param>
        /// <param name="value1">The value to test against</param>
        /// <param name="value2">The second value to test against (for between)</param>
        /// <param name="joinVia">The <see cref="JoinVia"/> to use to get the data for this <see cref="IQueryField"/></param>
        public void AddFilterNoJoin<T>(
            IField field,
            object[] value1,
            object[] value2,
            JoinVia joinVia) where T : ICondition
        {
            IQueryField queryField = new Queryfield(field, string.Empty, joinVia, false);
            var filterField = (IQueryFilter)queryField;
            filterField.Value1 = value1;
            filterField.Value2 = value2;
            filterField.JoinVia = joinVia;
            filterField.Joiner = new Joiner();
            var condition = this.ConditionRepository.New<T>(filterField);
            this.AddFilter(condition);
        }

        /// <summary>
        /// Add a filter to the <see cref="IQueryFilters"/> with AND join.
        /// </summary>
        /// <typeparam name="T">The <see cref="ICondition"/> to use for this Filter</typeparam>
        /// <param name="field">The <see cref="IField"/> to apply the filter to.</param>
        /// <param name="value1">The value to test against</param>
        /// <param name="value2">The second value to test against (for between)</param>
        /// <param name="joinVia">The <see cref="JoinVia"/> to use to get the data for this <see cref="IQueryField"/></param>
        public void AddFilterAndJoin<T>(IField field, object[] value1, object[] value2, JoinVia joinVia) where T : ICondition
        {
            var filterField = new QueryFilter(new Queryfield(field, string.Empty, joinVia, false), new JoinerAnd(), value1, value2);
            var condition = this.ConditionRepository.New<T>(filterField);

            this.AddFilter(condition);
        }

        /// <summary>
        /// Add a filter to the <see cref="IQueryFilters"/> with OR join.
        /// </summary>
        /// <typeparam name="T">The <see cref="ICondition"/> to use for this Filter</typeparam>
        /// <param name="field">The <see cref="IField"/> to apply the filter to.</param>
        /// <param name="value1">The value to test against</param>
        /// <param name="value2">The second value to test against (for between)</param>
        /// <param name="joinVia">The <see cref="JoinVia"/> to use to get the data for this <see cref="IQueryField"/></param>
        public void AddFilterOrJoin<T>(IField field, object[] value1, object[] value2, JoinVia joinVia) where T : ICondition
        {
            var filterField = new QueryFilter(new Queryfield(field, string.Empty, joinVia, false), new JoinerOr(), value1, value2);
            var condition = this.ConditionRepository.New<T>(filterField);

            this.AddFilter(condition);
        }

        /// <summary>
        /// Add an <see cref="ICondition"/> to the <see cref="IQueryFilters"/>
        /// </summary>
        /// <param name="filter">The <see cref="ICondition"/> to add</param>
        public void AddFilter(ICondition filter)
        {
            this._index++;
            filter.QueryFilter.Index = this._index;
            this.ConditionFields.Add(filter);
        }

        /// <summary>
        /// Add a <see cref="IQueryFilterGroup"/> to the <see cref="IQueryFilters"/> list.
        /// </summary>
        /// <param name="filterGroup">The <see cref="IQueryFilterGroup"/> to add</param>
        public void AddFilterGroup(IQueryFilterGroup filterGroup)
        {
            this.QueryFilterGroups.Add(filterGroup);
        }

        /// <summary>
        /// Remove a column from the <see cref="IQueryBuilder"/>
        /// </summary>
        /// <param name="field">The <see cref="IField"/> to remove.</param>
        /// <returns>The column index of the removed column.</returns>
        public int RemoveColumn(IField field)
        {
            var idx = 0;
            foreach (IQueryField queryField in this.QueryFields)
            {
                if (queryField.Field == field)
                {
                    this.QueryFields.Remove(queryField);
                    return idx;
                }

                idx++;
            }

            return -1;
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>1</filterpriority>
        public IEnumerator<IQueryField> GetEnumerator()
        {
            return this.QueryFields.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        /// <summary>
        /// The add column.
        /// </summary>
        /// <param name="field">
        /// The field.
        /// </param>
        /// <param name="joinVia">
        /// The join via.
        /// </param>
        /// <param name="alias">
        /// The alias.
        /// </param>
        /// <param name="useListItemText">
        /// The use list item text.
        /// </param>
        /// <param name="selectType">
        /// The select type.
        /// </param>
        private void AddColumn(IField field, JoinVia joinVia, string alias, bool useListItemText, SelectType selectType)
        {
            IQueryField queryField = new Queryfield(field, alias, joinVia, useListItemText);
            switch (selectType)
            {
                case SelectType.Fields:
                    break;
                case SelectType.Count:
                    queryField = (IQueryFieldCount)queryField;
                    break;
                case SelectType.Distinct:
                    queryField = (IQueryFieldDistinct)queryField;
                    break;
                case SelectType.Min:
                    queryField = (IQueryFieldMin)queryField;
                    break;
                case SelectType.Max:
                    queryField = (IQueryFieldMax)queryField;
                    break;
                case SelectType.Sum:
                    queryField = (IQueryFieldSum)queryField;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(selectType), selectType, null);
            }

            queryField.JoinVia = joinVia;
            queryField.Alias = alias;
            queryField.UseListItemText = useListItemText;
            this.QueryFields.Add(queryField);
        }
    }
}
