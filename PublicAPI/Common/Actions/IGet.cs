namespace PublicAPI.Common.Actions
{
    using System.Collections.Generic;
    using System.Web.Http;

    /// <summary>
    /// Defines that an object implements get and get by id actions.
    /// </summary>
    /// <typeparam name="TPrimaryKeyDataType">The primary key data type for <typeparamref name="TComplexType"/>.</typeparam>
    public interface IGet<in TPrimaryKeyDataType>
    {
        /// <summary>
        /// Controller action to get all available instances of <typeparam cref="TComplexType"/>.
        /// </summary>
        /// <remarks>
        /// GET: <a href="https://api.hostname/{controller}">https://api.hostname/{controller}</a>
        /// </remarks>
        /// <returns>A <see cref="IEnumerable{T}"/> containing all instances of <typeparam cref="TComplexType"/>.</returns>
        IHttpActionResult Get();
    }
}