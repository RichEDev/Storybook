namespace BusinessLogic.QueryBuilder
{
    using BusinessLogic.Fields.Type.Base;
    using BusinessLogic.JoinVia;
    using BusinessLogic.QueryBuilder.Conditions;

    /// <summary>
    /// Implement <see cref="IQueryFilters"/> which allows adding of Filters to create a Query.
    /// </summary>
    public interface IQueryFilters
    {
        /// <summary>
        /// Add a filter to the <see cref="IQueryFilters"/> with no join.
        /// </summary>
        /// <typeparam name="T">The <see cref="ICondition"/> to use for this Filter</typeparam>
        /// <param name="field">The <see cref="IField"/> to apply the filter to.</param>
        /// <param name="value1">The value to test against</param>
        /// <param name="value2">The second value to test against (for between)</param>
        /// <param name="joinVia">The <see cref="JoinVia"/> to use to get the data for this <see cref="IQueryField"/></param>
        void AddFilterNoJoin<T>(
            IField field,
            object[] value1,
            object[] value2,
            JoinVia joinVia) where T : ICondition;

        /// <summary>
        /// Add a filter to the <see cref="IQueryFilters"/> with AND join.
        /// </summary>
        /// <typeparam name="T">The <see cref="ICondition"/> to use for this Filter</typeparam>
        /// <param name="field">The <see cref="IField"/> to apply the filter to.</param>
        /// <param name="value1">The value to test against</param>
        /// <param name="value2">The second value to test against (for between)</param>
        /// <param name="joinVia">The <see cref="JoinVia"/> to use to get the data for this <see cref="IQueryField"/></param>
        void AddFilterAndJoin<T>(
            IField field,
            object[] value1,
            object[] value2,
            JoinVia joinVia) where T : ICondition;

        /// <summary>
        /// Add a filter to the <see cref="IQueryFilters"/> with OR join.
        /// </summary>
        /// <typeparam name="T">The <see cref="ICondition"/> to use for this Filter</typeparam>
        /// <param name="field">The <see cref="IField"/> to apply the filter to.</param>
        /// <param name="value1">The value to test against</param>
        /// <param name="value2">The second value to test against (for between)</param>
        /// <param name="joinVia">The <see cref="JoinVia"/> to use to get the data for this <see cref="IQueryField"/></param>
        void AddFilterOrJoin<T>(
            IField field,
            object[] value1,
            object[] value2,
            JoinVia joinVia) where T : ICondition;

        /// <summary>
        /// Add an <see cref="ICondition"/> to the <see cref="IQueryFilters"/>
        /// </summary>
        /// <param name="filter">The <see cref="ICondition"/> to add</param>
        void AddFilter(ICondition filter);

        /// <summary>
        /// Add a <see cref="IQueryFilterGroup"/> to the <see cref="IQueryFilters"/> list.
        /// </summary>
        /// <param name="filterGroup">The <see cref="IQueryFilterGroup"/> to add</param>
        void AddFilterGroup(IQueryFilterGroup filterGroup);
    }
}