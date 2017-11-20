namespace BusinessLogic.DataConnections
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// States an object can retrieve a collection of objects.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IGetAll<T> 
    {
        /// <summary>
        /// Gets an instance of <see cref="IList{T}"/> containing all available <see cref="T"/>.
        /// </summary>    
        /// <returns>The list of <see cref="T"/>.</returns>
        IList<T> Get();

        /// <summary>
        /// Gets an instance of <see cref="IList{T}"/> containing all available <see cref="T"/> that match <paramref name="predicate"/>.
        /// </summary>
        /// <param name="predicate">Criteria to match on.</param>
        /// <returns>An instance of <see cref="IList{T}"/> containing all available <see cref="T"/> that match <paramref name="predicate"/>.</returns>
        IList<T> Get(Predicate<T> predicate);
    }
}