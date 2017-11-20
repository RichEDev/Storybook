namespace BusinessLogic.QueryBuilder
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    using BusinessLogic.Fields.Type.Base;
    using BusinessLogic.JoinVia;
    using BusinessLogic.QueryBuilder.Conditions;

    /// <summary>
    /// A definition of a <see cref="IQueryBuilder"/> 
    /// </summary>
    public interface IQueryBuilder : IEnumerable<IQueryField>, IQueryFilters
    {
        /// <summary>
        /// Gets the list of <see cref="ICondition"/> used in this <see cref="IQueryBuilder"/>
        /// </summary>
        ReadOnlyCollection<ICondition> Conditions { get; }

        /// <summary>
        /// Gets the list of <see cref="IQueryFilterGroup"/> in the <see cref="IQueryBuilder"/>
        /// </summary>
        ReadOnlyCollection<IQueryFilterGroup> FilterGroups { get; }

        /// <summary>
        /// Add a <see cref="IField"/> to the query
        /// </summary>
        /// <param name="field">The <see cref="IField"/> to add</param>
        void AddColumn(IField field);

        /// <summary>
        /// Add a <see cref="IField"/> to the query
        /// </summary>
        /// <param name="field">The <see cref="IField"/> to add</param>
        /// <param name="alias">The alias of the <see cref="IField"/></param>
        void AddColumn(IField field, string alias);

        /// <summary>
        /// Add a <see cref="IField"/> to the query
        /// </summary>
        /// <param name="field">The <see cref="IField"/> to add</param>
        /// <param name="selType">The <see cref="QueryBuilder.SelectType"/> to use</param>
        void AddColumn(IField field, QueryBuilder.SelectType selType);

        /// <summary>
        /// Add a <see cref="IField"/> to the query
        /// </summary>
        /// <param name="field">The <see cref="IField"/> to add</param>
        /// /// <param name="alias">The alias of the <see cref="IField"/></param>
        /// <param name="selType">The <see cref="QueryBuilder.SelectType"/> to use</param>
        void AddColumn(IField field, string alias, QueryBuilder.SelectType selType);

        /// <summary>
        /// Add a <see cref="IField"/> to the query
        /// </summary>
        /// <param name="field">The <see cref="IField"/> to add</param>
        /// <param name="useListItemText">True to use the List Item text and not the item ID.</param>
        void AddColumn(IField field, bool useListItemText);

        /// <summary>
        /// Add a <see cref="IField"/> to the query
        /// </summary>
        /// <param name="field">The <see cref="IField"/> to add</param>
        /// <param name="alias">The alias of the <see cref="IField"/></param>
        /// <param name="useListItemText">True to use the List Item text and not the item ID.</param>
        void AddColumn(IField field, string alias, bool useListItemText);

        /// <summary>
        /// Add a <see cref="IField"/> to the query
        /// </summary>
        /// <param name="field">The <see cref="IField"/> to add</param>
        /// <param name="joinVia">The <see cref="JoinVia"/> to use to get the data.</param>
        /// <param name="alias">The alias of the <see cref="IField"/></param>
        void AddColumn(IField field, JoinVia joinVia, string alias);

        /// <summary>
        /// Add a <see cref="IField"/> to the query
        /// </summary>
        /// <param name="field">The <see cref="IField"/> to add</param>
        /// <param name="joinVia">The <see cref="JoinVia"/> to use to get the data.</param>
        void AddColumn(IField field, JoinVia joinVia);

        /// <summary>
        /// Add a <see cref="IField"/> that is sortable.
        /// </summary>
        /// <param name="field">The <see cref="IField"/> to add</param>
        /// <param name="sortdirection">The <see cref="QueryBuilder.SortDirection"/> to apply.</param>
        void AddSortableColumn(IField field, QueryBuilder.SortDirection sortdirection);

        /// <summary>
        /// Add a <see cref="IField"/> that is sortable.
        /// </summary>
        /// <param name="field">The <see cref="IField"/> to add</param>
        /// <param name="sortdirection">The <see cref="QueryBuilder.SortDirection"/> to apply.</param>
        /// <param name="alias">The column alias to use.</param>
        void AddSortableColumn(IField field, QueryBuilder.SortDirection sortdirection, string alias);

        /// <summary>
        /// Add a <see cref="IField"/> that is sortable.
        /// </summary>
        /// <param name="field">The <see cref="IField"/> to add</param>
        /// <param name="sortdirection">The <see cref="QueryBuilder.SortDirection"/> to apply.</param>
        /// <param name="joinVia">The <see cref="JoinVia"/> to use to get the data.</param>
        void AddSortableColumn(IField field, QueryBuilder.SortDirection sortdirection, JoinVia joinVia);

        /// <summary>
        /// Add a <see cref="IField"/> that is sortable.
        /// </summary>
        /// <param name="field">
        /// The <see cref="IField"/> to add
        /// </param>
        /// <param name="joinVia">
        /// The <see cref="JoinVia"/> to use to get the data.
        /// </param>
        /// <param name="alias">
        /// The column alias to use.
        /// </param>
        /// <param name="sortdirection">
        /// The <see cref="QueryBuilder.SortDirection"/> to apply.
        /// </param>
        void AddSortableColumn(IField field, JoinVia joinVia, string alias, QueryBuilder.SortDirection sortdirection);

        /// <summary>
        /// Add a static column to the <see cref="IQueryBuilder"/>
        /// </summary>
        /// <param name="staticValue">The Value to display</param>
        /// <param name="staticFieldname">The column name</param>
        void AddStaticColumn(string staticValue, string staticFieldname);

        /// <summary>
        /// Remove a column from the <see cref="IQueryBuilder"/>
        /// </summary>
        /// <param name="field">The <see cref="IField"/> to remove.</param>
        /// <returns>The column index of the removed column.</returns>
        int RemoveColumn(IField field);
    }
}