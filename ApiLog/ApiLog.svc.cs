namespace ApiLog
{
    using ApiLibrary.ApiObjects.Base;
    using global::ApiLog.Classes;
    using global::ApiLog.Interfaces;

    /// <summary>
    /// The API log service.
    /// </summary>
    public class ApiLog : IApiLog
    {
        /// <summary>
        /// The logger.
        /// </summary>
        private readonly Logger logger;

        /// <summary>
        /// The connection.
        /// </summary>
        private IApiDbConnection connection;

        /// <summary>
        /// Initialises a new instance of the <see cref="ApiLog"/> class.
        /// </summary>
        /// <param name="connection">
        /// The connection.
        /// </param>
        public ApiLog(IApiDbConnection connection)
        {
            this.connection = connection;
            this.logger = new Logger(this.connection);
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="ApiLog"/> class.
        /// </summary>
        public ApiLog()
        {
            this.connection = new LogDbConnection();
            this.logger = new Logger(this.connection);
        }

        /// <summary>
        /// The message level.
        /// </summary>
        public enum MessageLevel
        {
            /// <summary>
            /// The normal logging level.
            /// </summary>
            Normal = 0,

            /// <summary>
            /// The debug logging level.
            /// </summary>
            Debug = 1,

            /// <summary>
            /// The extended debug logging level.
            /// </summary>
            Extended = 2
        }

        /// <summary>
        /// The log.
        /// </summary>
        /// <param name="logRecord">
        /// The log Record.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool Log(LogRecord logRecord)
        {
            return this.logger.Log(MessageLevel.Normal, logRecord);
        }

        /// <summary>
        /// The log debug.
        /// </summary>
        /// <param name="logRecord">
        /// The log Record.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool LogDebug(LogRecord logRecord)
        {
            if (this.logger.ConnectionValid)
            {
                return this.logger.Log(MessageLevel.Debug, logRecord);    
            }

            return false;
        }

        /// <summary>
        /// The log extended.
        /// </summary>
        /// <param name="logRecord">
        /// The log Record.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool LogExtended(LogRecord logRecord)
        {
            return this.logger.Log(MessageLevel.Extended, logRecord);
        }

        /// <summary>
        /// The get logging level.
        /// </summary>
        /// <returns>
        /// The <see cref="MessageLevel"/>.
        /// </returns>
        public MessageLevel GetLoggingLevel()
        {
            if (this.connection == null)
            {
                this.connection = new LogDbConnection();
            }

            return this.connection.DebugLevel;
        }
    }
}
