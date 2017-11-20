namespace BusinessLogic.QueryBuilder
{
    /// <summary>
    /// An instance of <see cref="IQueryFilterString"/>
    /// </summary>
    public class QueryFilterString : IQueryFilterString
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QueryFilterString"/> class. 
        /// </summary>
        /// <param name="filterString">
        /// The filter string to use.
        /// </param>
        public QueryFilterString(string filterString)
        {
            this.FilterString = filterString;
        }

        /// <summary>
        /// Gets the Filter string
        /// </summary>
        public string FilterString { get; }
    }
}