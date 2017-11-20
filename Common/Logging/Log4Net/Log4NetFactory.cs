namespace Common.Logging.Log4Net
{
    /// <summary>
    /// Creates a new instance of <see cref="Log4NetAdapter{T}"/> to use for logging.
    /// </summary>
    /// <typeparam name="T">The class type that will be doing the logging.</typeparam>
    public class Log4NetFactory<T> : ILogFactory<T>
    {
        /// <summary>
        /// Get an instance of <see cref="ILog"/> using the <see cref="Log4NetAdapter{T}"/> provider.
        /// </summary>
        /// <returns>An instance of <see cref="ILog"/> using the <see cref="Log4NetAdapter{T}"/> provider</returns>
        public ILog GetLogger()
        {
            return new Log4NetAdapter<T>();
        }
    }
}
