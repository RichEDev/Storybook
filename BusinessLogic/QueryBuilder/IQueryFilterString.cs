namespace BusinessLogic.QueryBuilder
{
    /// <summary>
    /// Defines a filter string for use in an <see cref="IQueryBuilder"/>
    /// </summary>
    public interface IQueryFilterString
    {
        /// <summary>
        /// Get the Filter string
        /// </summary>
        string FilterString { get; }
    }
}
