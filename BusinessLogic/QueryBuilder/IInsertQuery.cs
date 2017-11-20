namespace BusinessLogic.QueryBuilder
{
    /// <summary>
    /// Implenetation of an Insert Query
    /// </summary>
    public interface IInsertQuery
    {
        /// <summary>
        /// Execute the INSERT statement to the current data source.
        /// </summary>
        /// <returns>The return code from the insert.</returns>
        int Execute();

    }
}
