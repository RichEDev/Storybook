namespace ApiLibrary.ApiObjects.Base
{
    using System;
    using System.Threading.Tasks;

    using ApiLibrary.Interfaces;

    /// <summary>
    /// The logger class.
    /// </summary>
    public class Log
    {
        /// <summary>
        /// The debug level.
        /// </summary>
        private readonly MessageLevel debugLevel;

        /// <summary>
        /// The connection for Unit tests.
        /// </summary>
        private readonly IApiDbConnection connection;

        /// <summary>
        /// Initialises a new instance of the <see cref="Log"/> class.
        /// </summary>
        public Log()
        {
            this.connection = null;
            this.debugLevel = this.GetMessageLevel();
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="Log"/> class.
        /// </summary>
        /// <param name="connection">
        /// The connection for Unit Tests.
        /// </param>
        public Log(IApiDbConnection connection)
        {
            this.connection = connection;
            this.debugLevel = this.GetMessageLevel();
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
        /// Write a normal entry to the log.
        /// </summary>
        /// <param name="metaBase">
        /// The meta Base.
        /// </param>
        /// <param name="nhsVpd">
        /// The NHS VPD.
        /// </param>
        /// <param name="accountid">
        /// The account Id.
        /// </param>
        /// <param name="logItemType">
        /// The log Item Type.
        /// </param>
        /// <param name="transferType">
        /// The transfer Type.
        /// </param>
        /// <param name="logId">
        /// The log Id.
        /// </param>
        /// <param name="fileName">
        /// The file Name.
        /// </param>
        /// <param name="logReasonType">
        /// The email Sent.
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool Write(string metaBase, int nhsVpd, int accountid, LogRecord.LogItemTypes logItemType, LogRecord.TransferTypes transferType, int logId, string fileName, LogRecord.LogReasonType logReasonType, string message,  string source)
        {
            var logRecord = new LogRecord
                                {
                                    AccountId = accountid,
                                    LogReason = logReasonType,
                                    Filename = fileName,
                                    Message = message,
                                    Source = source,
                                    MetaBase = metaBase,
                                    LogItemType = logItemType,
                                    TransferType = transferType,
                                    LogId = logId,
                                    NhsVpd = nhsVpd
                                };
            Task.Run(() => Logger.Log(MessageLevel.Normal, logRecord, this.connection));
            return true;
        }

        /// <summary>
        /// Write a debug entry to the log.
        /// </summary>
        /// <param name="metaBase">
        /// The meta Base.
        /// </param>
        /// <param name="nhsVpd">
        /// The NHS VPD.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="logItemType">
        /// The log Item Type.
        /// </param>
        /// <param name="transferType">
        /// The transfer Type.
        /// </param>
        /// <param name="logId">
        /// The log Id.
        /// </param>
        /// <param name="fileName">
        /// The file Name.
        /// </param>
        /// <param name="logReason">
        /// The email Sent.
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool WriteDebug(string metaBase, int nhsVpd, int accountid, LogRecord.LogItemTypes logItemType, LogRecord.TransferTypes transferType, int logId, string fileName, LogRecord.LogReasonType logReason, string message, string source)
        {
            if (this.debugLevel == MessageLevel.Debug || this.debugLevel == MessageLevel.Extended)
            {
                var logRecord = new LogRecord
                {
                    AccountId = accountid,
                    LogReason = logReason,
                    Filename = fileName,
                    Message = message,
                    Source = source,
                    MetaBase = metaBase,
                    LogItemType = logItemType,
                    TransferType = transferType,
                    LogId = logId,
                    NhsVpd = nhsVpd
                };

                Task.Run(() => Logger.Log(MessageLevel.Debug, logRecord, this.connection));
                return true;
            }

            return false;
        }

        /// <summary>
        /// Write extra info to the log.
        /// </summary>
        /// <param name="metaBase">
        /// The meta Base.
        /// </param>
        /// <param name="nhsVpd">
        /// The NHS VPD.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="logItemType">
        /// The log Item Type.
        /// </param>
        /// <param name="transferType">
        /// The transfer Type.
        /// </param>
        /// <param name="logId">
        /// The log Id.
        /// </param>
        /// <param name="fileName">
        /// The file Name.
        /// </param>
        /// <param name="logReason">
        /// The email Sent.
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool WriteExtra(string metaBase, int nhsVpd, int accountid, LogRecord.LogItemTypes logItemType, LogRecord.TransferTypes transferType, int logId, string fileName, LogRecord.LogReasonType logReason, string message, string source)
        {
            if (this.debugLevel == MessageLevel.Extended)
            {
                var logRecord = new LogRecord
                {
                    AccountId = accountid,
                    LogReason = logReason,
                    Filename = fileName,
                    Message = message,
                    Source = source,
                    MetaBase = metaBase,
                    LogItemType = logItemType,
                    TransferType = transferType,
                    LogId = logId,
                    NhsVpd = nhsVpd
                };

                Task.Run(() => Logger.Log(MessageLevel.Extended, logRecord, this.connection));
                return true;
            }

            return false;
        }

        /// <summary>
        /// The get message level.
        /// </summary>
        /// <returns>
        /// The <see cref="MessageLevel"/>.
        /// </returns>
        private MessageLevel GetMessageLevel()
        {
            try
            {
                var dbgLevel = Logger.GetMessageLevel(this.connection);
                return dbgLevel;
            }
            catch (Exception)
            {
                return 0;
            }
        }
    }
}
