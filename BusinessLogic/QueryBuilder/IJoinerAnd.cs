namespace BusinessLogic.QueryBuilder
{
    /// <summary>
    /// An implemenation of a Joiner with AND
    /// </summary>
    public interface IJoinerAnd : IJoiner
    {

    }

    /// <summary>
    /// An implemenation of a Joiner with AND
    /// </summary>
    public class JoinerAnd : IJoinerAnd
    {
        /// <summary>
        /// Get the AND text
        /// </summary>
        public string JoinText => " AND (";
    }
}