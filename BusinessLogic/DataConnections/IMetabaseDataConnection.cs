namespace BusinessLogic.DataConnections
{
    /// <summary>
    /// An interface for a metabase data connection
    /// </summary>
    public interface IMetabaseDataConnection<T> : IDataConnection<T> where T : class
    {
    }
}