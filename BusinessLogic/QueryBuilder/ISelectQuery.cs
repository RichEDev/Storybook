namespace BusinessLogic.QueryBuilder
{
    using System.Data;

    /// <summary>
    /// Defines a SELECT query which is designed to return data from the current data source.
    /// </summary>
    public interface ISelectQuery
    {
        /// <summary>
        /// Get the number of rows or records.
        /// </summary>
        /// <returns></returns>
        int GetCount();

        /// <summary>
        /// Get a <see cref="DataSet"/> as defined by the Query.
        /// </summary>
        /// <returns></returns>
        DataSet GetDataSet();
    }
}
