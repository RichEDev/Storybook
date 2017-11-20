namespace EsrGo2FromNhs.Crud
{
    using EsrGo2FromNhs.Base;
    using EsrGo2FromNhs.Interfaces;

    /// <summary>
    /// The data access factory.
    /// </summary>
    public static class DataAccessFactory
    {
        /// <summary>
        /// The create data access.
        /// </summary>
        /// <param name="baseUrl">
        /// The base Url.
        /// </param>
        /// <param name="apiDbConnection">
        /// The API DB Connection.
        /// </param>
        /// <typeparam name="T">
        /// The data class to use to create the Data Access Class
        /// </typeparam>
        /// <returns>
        /// The <see cref="DataAccess"/>.
        /// </returns>
        public static DataAccess<T> CreateDataAccess<T>(IApiDbConnection apiDbConnection) where T : DataClassBase, new()
        {
            var dataAccessClass = new DataAccess<T>(apiDbConnection);

            return dataAccessClass;
        }
    }
}