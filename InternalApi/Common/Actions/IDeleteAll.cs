namespace InternalApi.Common.Actions
{
    using System.Web.Http;

    /// <summary>
    /// Defines that an object implments the Delete All action.
    /// </summary>
    /// <typeparam name="TComplexType">The data type this controller opperates on.</typeparam>
    public interface IDeleteAll<TComplexType>
    {
        /// <summary>
        /// Controller action to delete all <typeparam name="TComplexType" />.
        /// </summary>
        /// <remarks>
        /// DELETE: <a href="https://api.hostname/{controller}">https://api.hostname/{controller}</a>
        /// </remarks>
        /// <returns>A <see cref="string"/> confirmation response.</returns>
        IHttpActionResult Delete();
    }
}