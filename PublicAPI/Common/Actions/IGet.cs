namespace PublicAPI.Common.Actions
{
    using System.Collections.Generic;

    using System.Web.Http;    

    /// <summary>
    /// Defines that an object implements a standard Get action
    /// </summary>
    /// <typeparam name="TComplexType">The data type this interface operates on</typeparam>
    public interface IGet<TComplexType>
    {
        /// <summary>
        /// Controller action to get all available instances of <typeparam cref="TComplexType">.</typeparam>
        /// </summary>
        /// <remarks>
        /// GET: <a href= "https://api.hostname/{controller}">https://api.hostname/{controller}</a>
        /// </remarks>
        /// <returns>A <see cref="IEnumerable{T}"/> containing all instances of <typeparam cref="TComplexType"/>.</returns>
        IHttpActionResult Get();
    }
}