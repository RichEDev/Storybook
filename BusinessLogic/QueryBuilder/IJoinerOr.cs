namespace BusinessLogic.QueryBuilder
{
    /// <summary>
    /// An implemenation of a Joiner with OR
    /// </summary>
    public interface IJoinerOr : IJoiner
    {

    }

    /// <summary>
    /// An implemenation of a Joiner with OR
    /// </summary>
    public class JoinerOr : IJoinerOr
    {
        /// <summary>
        /// Get the OR text
        /// </summary>
        public string JoinText => " OR (";
    }
}