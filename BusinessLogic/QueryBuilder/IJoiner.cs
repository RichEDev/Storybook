namespace BusinessLogic.QueryBuilder
{
    /// <summary>
    /// Implement a <see cref="IJoiner"/> which returns Join text for an <see cref="IQueryField"/>
    /// </summary>
    public interface IJoiner
    {
        /// <summary>
        /// Gets the Join text ( AND OR etc)
        /// </summary>
        string JoinText { get; }
    }

    /// <summary>
    /// An implementation of <see cref="IJoiner"/>
    /// </summary>
    public class Joiner : IJoiner
    {
        /// <summary>
        /// Gets the Join text ( AND OR etc)
        /// </summary>
        public string JoinText => string.Empty;
    }
}