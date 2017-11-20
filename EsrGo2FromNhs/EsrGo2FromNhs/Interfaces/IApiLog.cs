
namespace EsrGo2FromNhs.Interfaces
{
    using System.ServiceModel;

    using EsrGo2FromNhs.Base;

    /// <summary>
    /// The APILOG interface.
    /// </summary>
    [ServiceContract]
    public interface IApiLog
    {
        /// <summary>
        /// The log.
        /// </summary>
        /// <param name="logRecord">
        /// The log Record.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        [OperationContract]
        bool Log(LogRecord logRecord);

        /// <summary>
        /// The log debug.
        /// </summary>
        /// <param name="logRecord">
        /// The log Record.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        [OperationContract]
        bool LogDebug(LogRecord logRecord);

        /// <summary>
        /// The log extended.
        /// </summary>
        /// <param name="logRecord">
        /// The log Record.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        [OperationContract]
        bool LogExtended(LogRecord logRecord);

        /// <summary>
        /// The get logging level.
        /// </summary>
        /// <returns>
        /// The <see cref="ApiLog.MessageLevel"/>.
        /// </returns>
        [OperationContract]
        ApiLog.MessageLevel GetLoggingLevel();
    }
}
