namespace Common.Logging
{
    using Common.Logging.Log4Net;
    using Common.Logging.NullLogger;

    /// <summary>
    /// Gets an instance of <see cref="ILog"/> to enable logging of information.
    /// </summary>
    /// <typeparam name="T">The class type that will be doing the logging.</typeparam>
    public class LogFactory<T> : ILogFactory<T>
    {
        /// <summary>
        /// Get an instance of <see cref="ILog"/>.
        /// </summary>
        /// <returns>An instance of <see cref="ILog"/>.</returns>
        public ILog GetLogger()
        {
            return new Log4NetAdapter<T>();
            //return new NullLoggerWrapper();
        }
    }
}
