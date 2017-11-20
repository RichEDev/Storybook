namespace SpendManagementLibrary.Report
{
    public interface IGet<T, TId>
    {
        /// <summary>
        /// Get a matching <see cref="T"/> from the internal list if possible.
        /// </summary>
        /// <param name="id">The <see cref="TId"/>to search for.</param>
        /// <returns>An instance of <see cref="T"/> or null if not found.</returns>
        T Get(TId id);
    }
}