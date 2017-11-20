namespace BusinessLogic.QueryBuilder
{
    using BusinessLogic.Fields.Type.Base;
    using BusinessLogic.JoinVia;
    using BusinessLogic.Tables.Type;

    /// <summary>
    /// Defines and update <see cref="IUpdateQuery"/> query which is filterable <see cref="IQueryFilter"/>
    /// </summary>
    public interface IUpdateQuery : IQueryFilters
    {
        /// <summary>
        /// Run the update as defined in the <see cref="IUpdateQuery"/>
        /// </summary>
        /// <param name="baseTable">The <see cref="ITable"/> which is the base table of the Query</param>
        /// <returns>The result of the query <see cref="int"/>.</returns>
        int Execute(ITable baseTable);

        /// <summary>
        /// Add a column to the <see cref="IUpdateQuery"/>
        /// </summary>
        /// <param name="field">The <see cref="IField"/> to apply the value to.</param>
        /// <param name="value">The value of the <see cref="IField"/></param>
        void AddColumn(IField field, object value);

        /// <summary>
        /// Add a column to the <see cref="IUpdateQuery"/>
        /// </summary>
        /// <param name="field">The <see cref="IField"/> to apply the value to.</param>
        /// <param name="alias">The alias of the column.</param>
        /// <param name="value">The value of the <see cref="IField"/></param>
        void AddColumn(IField field, string alias, object value);

        /// <summary>
        /// Add a column to the <see cref="IUpdateQuery"/>
        /// </summary>
        /// <param name="field">The <see cref="IField"/> to apply the value to.</param>
        /// <param name="selType">The <see cref="QueryBuilder.SelectType"/> of the <see cref="IField"/></param>
        /// <param name="value">The value of the <see cref="IField"/></param>
        void AddColumn(IField field, QueryBuilder.SelectType selType, object value);

        /// <summary>
        /// Add a column to the <see cref="IUpdateQuery"/>
        /// </summary>
        /// <param name="field">The <see cref="IField"/> to apply the value to.</param>
        /// <param name="alias">The alias of the column.</param>
        /// <param name="selType">The <see cref="QueryBuilder.SelectType"/> of the <see cref="IField"/></param>
        /// <param name="value">The value of the <see cref="IField"/></param>
        void AddColumn(IField field, string alias, QueryBuilder.SelectType selType, object value);

        /// <summary>
        /// Add a column to the <see cref="IUpdateQuery"/>
        /// </summary>
        /// <param name="field">The <see cref="IField"/> to apply the value to.</param>
        /// <param name="useListItemText"></param>
        /// <param name="value">The value of the <see cref="IField"/></param>
        void AddColumn(IField field, bool useListItemText, object value);

        /// <summary>
        /// Add a column to the <see cref="IUpdateQuery"/>
        /// </summary>
        /// <param name="field">The <see cref="IField"/> to apply the value to.</param>
        /// <param name="alias">The alias of the column.</param>
        /// <param name="useListItemText">True to use the List Item text and not the ID value</param>
        /// <param name="value">The value of the <see cref="IField"/></param>
        void AddColumn(IField field, string alias, bool useListItemText, object value);

        /// <summary>
        /// Add a column to the <see cref="IUpdateQuery"/>
        /// </summary>
        /// <param name="field">The <see cref="IField"/> to apply the value to.</param>
        /// <param name="joinVia">The <see cref="JoinVia"/> used to get the data for this <see cref="IField"/></param>
        /// <param name="alias">The alias of the column.</param>
        /// <param name="value">The value of the <see cref="IField"/></param>
        void AddColumn(IField field, JoinVia joinVia, string alias, object value);

        /// <summary>
        /// Add a column to the <see cref="IUpdateQuery"/>
        /// </summary>
        /// <param name="field">The <see cref="IField"/> to apply the value to.</param>
        /// <param name="joinVia">The <see cref="JoinVia"/> used to get the data for this <see cref="IField"/></param>
        /// <param name="value">The value of the <see cref="IField"/></param>
        void AddColumn(IField field, JoinVia joinVia, object value);
    }
}
