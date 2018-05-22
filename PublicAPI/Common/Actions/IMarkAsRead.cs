namespace PublicAPI.Common.Actions
{
    using System.Web.Http;

    /// <summary>
    /// Defines that an object implements a MarkAsRead action
    /// </summary>
    /// <typeparam name="TComplexType">The data type this interface operates on</typeparam>
    public interface IMarkAsRead<TComplexType>
    {
        /// <summary>
        /// Controller action to add read receipts to all available instance of <typeparam cref="TComplexType">.</typeparam>
        /// </summary>
        /// <returns>An Okay message</returns>
        IHttpActionResult MarkAsRead();
    }
}