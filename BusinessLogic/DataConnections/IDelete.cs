namespace BusinessLogic.DataConnections
{
    /// <summary>
    /// States an object can delete.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IDelete<in T>
    {
        /// <summary>
        /// Deletes the instance of <typeparamref name="T"/>.
        /// </summary>
        /// <param name="id">The id of the entity to delete.</param>
        /// <returns>An <see cref="int"/> containing the result of the delete</returns>
        int Delete(T id);
    }
}