namespace Common.Logging
{
    /// <summary>
    /// Implement this interface to instantiate ILog implementation
    /// </summary>
    /// <typeparam name="T">The class type that will be doing the logging.</typeparam>
    public interface ILogFactory<T>
    {
        /// <summary>
        /// Get an instance of <see cref="ILog"/> using the provider you choose.
        /// </summary>
        /// <returns>An instance of <see cref="ILog"/> using the chosen provider.</returns>
        ILog GetLogger();
    }
}
