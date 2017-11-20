namespace PublicAPI.Common.Actions
{
    using System.Web.Http;

    /// <summary>
    /// Defines that a controller implments the Archive action.
    /// </summary>
    /// <typeparam name="TPrimaryKeyDataType">The primary key data type for the object implementing this.</typeparam>
    interface IArchive<in TPrimaryKeyDataType>
    {
        /// <summary>
        /// Controller action to archive an object.
        /// </summary>
        /// <remarks>
        /// GET: <a href="https://api.hostname/{controller}/Archive/{id}">https://api.hostname/{controller}/Archive/{id}</a>
        /// </remarks>
        /// <param name="id">The id of the object to archive.</param>
        /// <returns>A <see cref="bool"/> exposing the current archive state after execution.</returns>
        IHttpActionResult Archive(TPrimaryKeyDataType id);
    }
}
