namespace BusinessLogic.DataConnections
{
    using System.Collections.Generic;

    /// <summary>
    /// Supports a simple iteration over a generic collection.
    /// </summary>
    /// <typeparam name="T">The type of object to iterate over.</typeparam>
    public interface IEnumerate<out T>
    {
        /// <summary>
        /// Supports a simple iteration over a generic collection.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerable{T}">IEnumerable{T}></see>
        /// </returns>
        IEnumerator<T> GetEnumerator();
    }
}
