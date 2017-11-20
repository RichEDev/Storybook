namespace PublicAPI.Common.Actions
{
    using System.Collections.Generic;
    using System.Web.Http;

    /// <summary>
    /// Defines that an object implments the standard CRUD actions.
    /// </summary>
    /// <typeparam name="TComplexType">The data type this controller opperates on.</typeparam>
    /// <typeparam name="TPrimaryKeyDataType">The primary key data type for <typeparamref name="TComplexType"/>.</typeparam>
    interface ICrud<TComplexType, in TPrimaryKeyDataType>
    {
        /// <summary>
        /// Controller action to get all available instances of <typeparam cref="TComplexType"/>.
        /// </summary>
        /// <remarks>
        /// GET: <a href="https://api.hostname/{controller}">https://api.hostname/{controller}</a>
        /// </remarks>
        /// <returns>A <see cref="IEnumerable{T}"/> containing all instances of <typeparam cref="TComplexType"/>.</returns>
        IHttpActionResult Get();

        /// <summary>
        /// Controller action to get a specific <typeparam name="TComplexType" /> with a matching <typeparam name="TPrimaryKeyDataType"></typeparam> Id.
        /// </summary>
        /// <remarks>
        /// GET: <a href="https://api.hostname/{controller}/{id}">https://api.hostname/{controller}/{id}</a>
        /// </remarks>
        /// <param name="id">The id to match on</param>
        /// <returns>An instance of <typeparam name="TComplexType" /> with a matching id, of nothing is not matched.</returns>
        IHttpActionResult Get(TPrimaryKeyDataType id);

        /// <summary>
        /// Controller action to add (or update) an instance of <typeparamref name="TComplexType"/>.
        /// </summary>
        /// <remarks>
        /// POST: <a href="https://api.hostname/{controller}">https://api.hostname/{controller}</a>
        ///  Body: <typeparamref name="TComplexType"/>
        /// </remarks>
        /// <param name="value">The <typeparam name="TComplexType"/> to update with it's associated values.</param>
        /// <returns>An instance of <typeparam name="TComplexType" /> with updated properties post add (or update).</returns>
        IHttpActionResult Post([FromBody] TComplexType value);

        /// <summary>
        /// Controller action to add (or update) an instance of <typeparamref name="TComplexType"/>.
        /// </summary>
        /// <remarks>
        /// PUT: <a href="https://api.hostname/{controller}">https://api.hostname/{controller}</a>
        ///  Body: <typeparamref name="TComplexType"/>
        /// </remarks>
        /// <param name="value">The <typeparam name="TComplexType"/> to update with it's associated values.</param>
        /// <returns>An instance of <typeparam name="TComplexType" /> with updated properties post add (or update).</returns>
        IHttpActionResult Put([FromBody] TComplexType value);

        /// <summary>
        /// Controller action to delete a specific <typeparam name="TComplexType" /> with a matching <typeparam name="TPrimaryKeyDataType"></typeparam> Id.
        /// </summary>
        /// <remarks>
        /// DELETE: <a href="https://api.hostname/{controller}/{id}">https://api.hostname/{controller}/{id}</a>
        /// </remarks>
        /// <param name="id">The id to match on</param>
        /// <returns>An <see cref="int"/> code with the response of the delete.</returns>
        IHttpActionResult Delete(TPrimaryKeyDataType id);
    }
}
