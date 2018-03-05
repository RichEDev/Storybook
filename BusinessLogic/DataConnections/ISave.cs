namespace BusinessLogic.DataConnections
{
    /// <summary>
    /// Specifies members and methods an object must have to be added to the collections.
    /// </summary>
    /// <typeparam name="T">
    /// The type of entity which is being added.
    /// </typeparam>
    public interface ISave<T> where T : class
    {
        /// <summary>
        /// Saves an instance of <see cref="T"/> to the collection
        /// </summary>
        /// <param name="entity">
        /// The <see cref="T"/> you want to save
        /// </param>
        /// <returns>
        /// The <see cref="T"/> saved to the collection.
        /// </returns>
        T Save(T entity);
    }
}
