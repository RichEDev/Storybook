namespace BusinessLogic.DataConnections
{
    /// <summary>
    /// Get an instance of <typeparamref name="T"/> by its <c>Id</c> of type <see cref="TK"/>.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the entity to return.
    /// </typeparam>
    /// <typeparam name="TK">
    /// The <c>Id</c> data type of <typeparamref name="T"/>.
    /// </typeparam>
    public interface IGetBy<out T, in TK>
    {
        /// <summary>
        /// Gets an instance of <see cref="T"/> with a matching <c>Id</c>.
        /// </summary>
        /// <param name="id">The name of the <see cref="T"/> you want to retrieve</param>
        /// <returns>The required <see cref="T"/> or <see langword="null"/> if it cannot be found</returns>
        T this[TK id] { get; }
    }
}