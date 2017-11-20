namespace BusinessLogic.QueryBuilder
{
    /// <summary>
    /// An implementation of <see cref="IQueryFilter"/> that includes an <see cref="IQueryField"/> and values to check against.
    /// </summary>
    public interface IQueryFilter : IQueryField
    {
        /// <summary>
        /// Gets or sets the <see cref="IJoiner"/> that defines the query element as AND or OR.
        /// </summary>
        IJoiner Joiner { get; set; }

        /// <summary>
        /// Gets or sets the value to query against.
        /// </summary>
        object[] Value1 { get; set; }

        /// <summary>
        /// Gets or sets the second value (used for between).
        /// </summary>
        object[] Value2 { get; set; }

        /// <summary>
        /// Gets or sets the index of the <see cref="IQueryFilter"/>
        /// </summary>
        int Index { get; set; }
    }
}