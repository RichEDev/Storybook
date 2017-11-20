namespace Common.Logging
{
    using System;

    /// <summary>
    /// The ILog interface is used by the client to log messages.
    /// </summary>
    /// <remarks>Use the our IoC to programmatically assign logger implementations.</remarks>
    public interface ILog
    {
        /// <summary>
        /// Gets a value indicating whether this logger is enabled for the Debug level.
        /// </summary>
        /// <remarks>
        /// This function is intended to lessen the computational cost of disabled log debug
        /// statements.
        /// For some ILog interface log, when you write:
        /// log.Debug("This is entry number: " + i );
        /// You incur the cost constructing the message, string construction and concatenation
        /// in this case, regardless of whether the message is logged or not.
        /// If you are worried about speed (who isn't), then you should write:
        /// if (log.IsDebugEnabled) { log.Debug("This is entry number: " + i ); }
        /// This way you will not incur the cost of parameter construction if debugging is
        /// disabled for log. On the other hand, if the log is debug enabled, you will incur
        /// the cost of evaluating whether the logger is debug enabled twice. Once in ILog.IsDebugEnabled
        /// and once in the Debug(object). This is an insignificant overhead since evaluating
        /// a logger takes about 1% of the time it takes to actually log. This is the preferred
        /// style of logging.
        /// </remarks>
        bool IsDebugEnabled { get; }

        /// <summary>
        /// Gets a value indicating whether this logger is enabled for the Error level.
        /// </summary>
        /// <remarks>For more information see <see cref="ILog.IsDebugEnabled"/>.</remarks>
        bool IsErrorEnabled { get; }

        /// <summary>
        /// Gets a value indicating whether this logger is enabled for the Fatal level.
        /// </summary>
        /// <remarks>For more information see <see cref="ILog.IsDebugEnabled"/>.</remarks>
        bool IsFatalEnabled { get; }

        /// <summary>
        /// Gets a value indicating whether this logger is enabled for the Info level.
        /// </summary>
        /// <remarks>For more information see <see cref="ILog.IsDebugEnabled"/>.</remarks>
        bool IsInfoEnabled { get; }

        /// <summary>
        /// Gets a value indicating whether this logger is enabled for the Warn level.
        /// </summary>
        /// <remarks>For more information see <see cref="ILog.IsDebugEnabled"/>.</remarks>
        bool IsWarnEnabled { get; }

        /// <summary>
        /// Log a message object with the Debug level.
        /// </summary>
        /// <param name="message">The message object to log.</param>
        void Debug(object message);

        /// <summary>
        /// Log a message object with the Debug level including the stack 
        /// trace of the <see cref="System.Exception" /> passed as a parameter.
        /// </summary>
        /// <param name="message">The message object to log.</param>
        /// <param name="exception">The exception to log, including its stack trace.</param>
        void Debug(object message, Exception exception);

        /// <summary>
        /// Logs a formatted message string with the Debug level.
        /// </summary>
        /// <param name="format">A string containing zero or more format items</param>
        /// <param name="arg0">An object array containing zero or more objects to format</param>
        /// <remarks>
        /// The message is formatted using the <see cref="string.Format(string, object)"/> method. See <see cref="string.Format(string, object[])"/> 
        /// for details of the syntax of the format string and the behavior of the formatting.
        /// This method does not take an <see cref="System.Exception" /> object to include in the log event.
        /// To pass an <see cref="System.Exception" /> use one of the <see cref="Debug(object,Exception)"/> methods instead.
        /// </remarks>
        void DebugFormat(string format, object arg0);

        /// <summary>
        /// Logs a formatted message string with the Debug level.
        /// </summary>
        /// <param name="format">A string containing zero or more format items</param>
        /// <param name="arg0">An object array containing zero or more objects to format</param>
        /// <param name="arg1">An object array containing zero or more objects to format</param>
        /// <remarks>
        /// The message is formatted using the <see cref="string.Format(string, object, object)"/> method. See <see cref="string.Format(string, object[])"/> 
        /// for details of the syntax of the format string and the behavior of the formatting.
        /// This method does not take an <see cref="System.Exception" /> object to include in the log event.
        /// To pass an <see cref="System.Exception" /> use one of the <see cref="Debug(object,Exception)"/> methods instead.
        /// </remarks>
        void DebugFormat(string format, object arg0, object arg1);

        /// <summary>
        /// Logs a formatted message string with the Debug level.
        /// </summary>
        /// <param name="format">A string containing zero or more format items</param>
        /// <param name="arg0">An object array containing zero or more objects to format</param>
        /// <param name="arg1">An object array containing zero or more objects to format</param>
        /// <param name="arg2">An object array containing zero or more objects to format</param>
        /// <remarks>
        /// The message is formatted using the <see cref="string.Format(string, object, object, object)"/> method. See <see cref="string.Format(string, object[])"/> 
        /// for details of the syntax of the format string and the behavior of the formatting.
        /// This method does not take an <see cref="System.Exception" /> object to include in the log event.
        /// To pass an <see cref="System.Exception" /> use one of the <see cref="Debug(object,Exception)"/> methods instead.
        /// </remarks>
        void DebugFormat(string format, object arg0, object arg1, object arg2);

        /// <summary>
        /// Logs a formatted message string with the Debug level.
        /// </summary>
        /// <param name="format">A string containing zero or more format items</param>
        /// <param name="args">An object array containing zero or more objects to format</param>
        /// <remarks>
        /// The message is formatted using the string.Format method. See <see cref="string.Format(string, object[])"/> 
        /// for details of the syntax of the format string and the behavior of the formatting.
        /// This method does not take an <see cref="System.Exception" /> object to include in the log event.
        /// To pass an <see cref="System.Exception" /> use one of the <see cref="Debug(object,Exception)"/> methods instead.
        /// </remarks>
        void DebugFormat(string format, params object[] args);

        /// <summary>
        /// Logs a formatted message string with the Debug level.
        /// </summary>
        /// <param name="provider">A <see cref="System.IFormatProvider"/> that supplies culture-specific formatting information</param>
        /// <param name="format">A string containing zero or more format items</param>
        /// <param name="args">An object array containing zero or more objects to format</param>
        /// <remarks>
        /// The message is formatted using the string.Format method. See <see cref="string.Format(string, object[])"/> 
        /// for details of the syntax of the format string and the behavior of the formatting.
        /// This method does not take an <see cref="System.Exception" /> object to include in the log event.
        /// To pass an <see cref="System.Exception" /> use one of the <see cref="Debug(object,Exception)"/> methods instead.
        /// </remarks>
        void DebugFormat(IFormatProvider provider, string format, params object[] args);

        /// <summary>
        /// Logs a message object with the Error level.
        /// </summary>
        /// <param name="message">The message object to log.</param>
        void Error(object message);

        /// <summary>
        /// Log a message object with the Error level including the stack trace of the <see cref="System.Exception" /> passed as a parameter.
        /// </summary>
        /// <param name="message">The message object to log.</param>
        /// <param name="exception">The exception to log, including its stack trace.</param>
        void Error(object message, Exception exception);

        /// <summary>
        /// Logs a formatted message string with the Error level.
        /// </summary>
        /// <param name="format">A string containing zero or more format items</param>
        /// <param name="arg0">An object to format</param>
        /// <remarks>
        /// The message is formatted using the <see cref="string.Format(string, object)"/> method."/> 
        /// for details of the syntax of the format string and the behavior of the formatting.
        /// This method does not take an <see cref="System.Exception" /> object to include in the log event.
        /// To pass an <see cref="System.Exception" /> use one of the Error(object,Exception) methods instead.
        /// </remarks>
        void ErrorFormat(string format, object arg0);

        /// <summary>
        /// Logs a formatted message string with the Error level.
        /// </summary>
        /// <param name="format">A string containing zero or more format items</param>
        /// <param name="arg0">An object to format</param>
        /// <param name="arg1">An object to format</param>
        /// <remarks>
        /// The message is formatted using the string.Format method. See <see cref="string.Format(string, object[])"/> 
        /// for details of the syntax of the format string and the behavior of the formatting.
        /// This method does not take an <see cref="System.Exception" /> object to include in the log event.
        /// To pass an <see cref="System.Exception" /> use one of the Error(object,Exception) methods instead.
        /// </remarks>
        void ErrorFormat(string format, object arg0, object arg1);

        /// <summary>
        /// Logs a formatted message string with the Error level.
        /// </summary>
        /// <param name="format">A string containing zero or more format items</param>
        /// <param name="arg0">An object to format</param>
        /// <param name="arg1">An object to format</param>
        /// <param name="arg2">An object to format</param>
        /// <remarks>
        /// The message is formatted using the string.Format method. See <see cref="string.Format(string, object[])"/> 
        /// for details of the syntax of the format string and the behavior of the formatting.
        /// This method does not take an <see cref="System.Exception" /> object to include in the log event.
        /// To pass an <see cref="System.Exception" /> use one of the Error(object,Exception) methods instead.
        /// </remarks>
        void ErrorFormat(string format, object arg0, object arg1, object arg2);

        /// <summary>
        /// Logs a formatted message string with the Error level.
        /// </summary>
        /// <param name="format">A string containing zero or more format items</param>
        /// <param name="args">An object array containing zero or more objects to format</param>
        /// <remarks>
        /// The message is formatted using the string.Format method. <see cref="string.Format(string, object[])"/>
        /// for details of the syntax of the format string and the behavior of the formatting.
        /// This method does not take an <see cref="System.Exception" /> object to include in the log event.
        /// To pass an <see cref="System.Exception" /> use one of the Error(object) methods instead.
        /// </remarks>
        void ErrorFormat(string format, params object[] args);

        /// <summary>
        /// Logs a formatted message string with the Error level.
        /// </summary>
        /// <param name="provider">An <see cref="System.IFormatProvider"/> that supplies culture-specific formatting information</param>
        /// <param name="format">A string containing zero or more format items</param>
        /// <param name="args">An object array containing zero or more objects to format</param>
        /// <remarks>
        /// The message is formatted using the string.Format method. See <see cref="string.Format(string, object[])"/> 
        /// for details of the syntax of the format string and the behavior of the formatting.
        /// This method does not take an <see cref="System.Exception" /> object to include in the log event.
        /// To pass an <see cref="System.Exception" /> use one of the Error(object) methods instead.
        /// </remarks>
        void ErrorFormat(IFormatProvider provider, string format, params object[] args);

        /// <summary>
        /// Log a message object with the Fatal level.
        /// </summary>
        /// <param name="message">The message object to log.</param>
        void Fatal(object message);

        /// <summary>
        /// Log a message object with the Fatal level including the stack trace of the <see cref="System.Exception" /> passed as a parameter.
        /// </summary>
        /// <param name="message">The message object to log.</param>
        /// <param name="exception">The exception to log, including its stack trace.</param>
        void Fatal(object message, Exception exception);

        /// <summary>
        /// Logs a formatted message string with the Fatal level.
        /// </summary>
        /// <param name="format">A string containing zero or more format items</param>
        /// <param name="arg0">An object to format</param>
        /// <remarks>
        /// The message is formatted using the string.Format method. See <see cref="string.Format(string, object[])"/> 
        /// for details of the syntax of the format string and the behavior of the formatting.
        /// This method does not take an <see cref="System.Exception" /> object to include in the log event.
        /// To pass an <see cref="System.Exception" /> use one of the Fatal(object,Exception) methods instead.
        /// </remarks>
        void FatalFormat(string format, object arg0);

        /// <summary>
        /// Logs a formatted message string with the Fatal level.
        /// </summary>
        /// <param name="format">A string containing zero or more format items</param>
        /// <param name="arg0">An object to format</param>
        /// <param name="arg1">An object to format</param>
        /// <remarks>
        /// The message is formatted using the string.Format method. See <see cref="string.Format(string, object[])"/> 
        /// for details of the syntax of the format string and the behavior of the formatting.
        /// This method does not take an <see cref="System.Exception" /> object to include in the log event.
        /// To pass an <see cref="System.Exception" /> use one of the Fatal(object,Exception) methods instead.
        /// </remarks>
        void FatalFormat(string format, object arg0, object arg1);

        /// <summary>
        /// Logs a formatted message string with the Fatal level.
        /// </summary>
        /// <param name="format">A string containing zero or more format items</param>
        /// <param name="arg0">An object to format</param>
        /// <param name="arg1">An object to format</param>
        /// <param name="arg2">An object to format</param>
        /// <remarks>
        /// The message is formatted using the string.Format method. See <see cref="string.Format(string, object[])"/> 
        /// for details of the syntax of the format string and the behavior of the formatting.
        /// This method does not take an <see cref="System.Exception" /> object to include in the log event.
        /// To pass an <see cref="System.Exception" /> use one of the Fatal(object,Exception) methods instead.
        /// </remarks>
        void FatalFormat(string format, object arg0, object arg1, object arg2);

        /// <summary>
        /// Logs a formatted message string with the Fatal level.
        /// </summary>
        /// <param name="format">A string containing zero or more format items</param>
        /// <param name="args">An object array containing zero or more objects to format</param>
        /// <remarks>
        /// The message is formatted using the string.Format method. See <see cref="string.Format(string, object[])"/> 
        /// for details of the syntax of the format string and the behavior of the formatting.
        /// This method does not take an <see cref="System.Exception" /> object to include in the log event.
        /// To pass an <see cref="System.Exception" /> use one of the Fatal(object) methods instead.
        /// </remarks>
        void FatalFormat(string format, params object[] args);

        /// <summary>
        /// Logs a formatted message string with the Fatal level.
        /// </summary>
        /// <param name="provider">An <see cref="System.IFormatProvider"/> that supplies culture-specific formatting information</param>
        /// <param name="format">A string containing zero or more format items</param>
        /// <param name="args">An object array containing zero or more objects to format</param>
        /// <remarks>
        /// The message is formatted using the string.Format method. See <see cref="string.Format(string, object[])"/> 
        /// for details of the syntax of the format string and the behavior of the formatting.
        /// This method does not take an <see cref="System.Exception" /> object to include in the log event.
        /// To pass an <see cref="System.Exception" /> use one of the Fatal(object) methods instead.
        /// </remarks>
        void FatalFormat(IFormatProvider provider, string format, params object[] args);

        /// <summary>
        /// Logs a message object with the Info level.
        /// </summary>
        /// <param name="message">The message object to log.</param>
        void Info(object message);

        /// <summary>
        /// Logs a message object with the INFO level including the stack trace of the <see cref="System.Exception" /> passed as a parameter.
        /// </summary>
        /// <param name="message">The message object to log.</param>
        /// <param name="exception">The exception to log, including its stack trace.</param>
        void Info(object message, Exception exception);

        /// <summary>
        /// Logs a formatted message string with the Info level.
        /// </summary>
        /// <param name="format">A string containing zero or more format items</param>
        /// <param name="arg0">An object to format</param>
        /// <remarks>
        /// The message is formatted using the string.Format method. See <see cref="string.Format(string, object[])"/> 
        /// for details of the syntax of the format string and the behavior of the formatting.
        /// This method does not take an <see cref="System.Exception" /> object to include in the log event.
        /// To pass an <see cref="System.Exception" /> use one of the Info(object,Exception) methods instead.
        /// </remarks>
        void InfoFormat(string format, object arg0);

        /// <summary>
        /// Logs a formatted message string with the Info level.
        /// </summary>
        /// <param name="format">A string containing zero or more format items</param>
        /// <param name="arg0">An object to format</param>
        /// <param name="arg1">An object to format</param>
        /// <remarks>
        /// The message is formatted using the string.Format method. See <see cref="string.Format(string, object[])"/> 
        /// for details of the syntax of the format string and the behavior of the formatting.
        /// This method does not take an <see cref="System.Exception" /> object to include in the log event.
        /// To pass an <see cref="System.Exception" /> use one of the Info(object,Exception) methods instead.
        /// </remarks>
        void InfoFormat(string format, object arg0, object arg1);

        /// <summary>
        /// Logs a formatted message string with the Info level.
        /// </summary>
        /// <param name="format">A string containing zero or more format items</param>
        /// <param name="arg0">An object to format</param>
        /// <param name="arg1">An object to format</param>
        /// <param name="arg2">An object to format</param>
        /// <remarks>
        /// The message is formatted using the string.Format method. See <see cref="string.Format(string, object[])"/> 
        /// for details of the syntax of the format string and the behavior of the formatting.
        /// This method does not take an <see cref="System.Exception" /> object to include in the log event.
        /// To pass an <see cref="System.Exception" /> use one of the Info(object,Exception) methods instead.
        /// </remarks>
        void InfoFormat(string format, object arg0, object arg1, object arg2);

        /// <summary>
        /// Logs a formatted message string with the Info level.
        /// </summary>
        /// <param name="format">A string containing zero or more format items</param>
        /// <param name="args">An object array containing zero or more objects to format</param>
        /// <remarks>
        /// The message is formatted using the string.Format method. See <see cref="string.Format(string, object[])"/> 
        /// for details of the syntax of the format string and the behavior of the formatting.
        /// This method does not take an <see cref="System.Exception" /> object to include in the log event.
        /// To pass an <see cref="System.Exception" /> use one of the Info(object) methods instead.
        /// </remarks>
        void InfoFormat(string format, params object[] args);

        /// <summary>
        /// Logs a formatted message string with the Info level.
        /// </summary>
        /// <param name="provider">An <see cref="System.IFormatProvider"/> that supplies culture-specific formatting information</param>
        /// <param name="format">A string containing zero or more format items</param>
        /// <param name="args">An object array containing zero or more objects to format</param>
        /// <remarks>
        /// The message is formatted using the string.Format method. See <see cref="string.Format(string, object[])"/> 
        /// for details of the syntax of the format string and the behavior of the formatting.
        /// This method does not take an <see cref="System.Exception" /> object to include in the log event.
        /// To pass an <see cref="System.Exception" /> use one of the Info(object) methods instead.
        /// </remarks>
        void InfoFormat(IFormatProvider provider, string format, params object[] args);

        /// <summary>
        /// Log a message object with the Warn level.
        /// </summary>
        /// <param name="message">The message object to log.</param>
        void Warn(object message);

        /// <summary>
        /// Log a message object with the Warn level including the stack trace of the <see cref="System.Exception" /> passed as a parameter.
        /// </summary>
        /// <param name="message">The message object to log.</param>
        /// <param name="exception">The exception to log, including its stack trace.</param>
        void Warn(object message, Exception exception);

        /// <summary>
        /// Logs a formatted message string with the Warn level.
        /// </summary>
        /// <param name="format">A string containing zero or more format items</param>
        /// <param name="arg0">An object to format</param>
        /// <remarks>
        /// The message is formatted using the string.Format method. See <see cref="string.Format(string, object[])"/> 
        /// for details of the syntax of the format string and the behavior of the formatting.
        /// This method does not take an <see cref="System.Exception" /> object to include in the log event.
        /// To pass an <see cref="System.Exception" /> use one of the Warn(object,Exception) methods instead.
        /// </remarks>
        void WarnFormat(string format, object arg0);

        /// <summary>
        /// Logs a formatted message string with the Warn level.
        /// </summary>
        /// <param name="format">A string containing zero or more format items</param>
        /// <param name="arg0">An object to format</param>
        /// <param name="arg1">An object to format</param>
        /// <remarks>
        /// The message is formatted using the string.Format method. See <see cref="string.Format(string, object[])"/> 
        /// for details of the syntax of the format string and the behavior of the formatting.
        /// This method does not take an <see cref="System.Exception" /> object to include in the log event.
        /// To pass an <see cref="System.Exception" /> use one of the Warn(object,Exception) methods instead.
        /// </remarks>
        void WarnFormat(string format, object arg0, object arg1);

        /// <summary>
        /// Logs a formatted message string with the Warn level.
        /// </summary>
        /// <param name="format">A string containing zero or more format items</param>
        /// <param name="arg0">An object to format</param>
        /// <param name="arg1">An object to format</param>
        /// <param name="arg2">An object to format</param>
        /// <remarks>
        /// The message is formatted using the string.Format method. See <see cref="string.Format(string, object[])"/> 
        /// for details of the syntax of the format string and the behavior of the formatting.
        /// This method does not take an <see cref="System.Exception" /> object to include in the log event.
        /// To pass an <see cref="System.Exception" /> use one of the Warn(object,Exception) methods instead.
        /// </remarks>
        void WarnFormat(string format, object arg0, object arg1, object arg2);

        /// <summary>
        /// Logs a formatted message string with the Warn level.
        /// </summary>
        /// <param name="format">A string containing zero or more format items</param>
        /// <param name="args">An object array containing zero or more objects to format</param>
        /// <remarks>
        /// The message is formatted using the string.Format method. See <see cref="string.Format(string, object[])"/> 
        /// for details of the syntax of the format string and the behavior of the formatting.
        /// This method does not take an <see cref="System.Exception" /> object to include in the log event.
        /// To pass an <see cref="System.Exception" /> use one of the Warn(object) methods instead.
        /// </remarks>
        void WarnFormat(string format, params object[] args);

        /// <summary>
        /// Logs a formatted message string with the Warn level.
        /// </summary>
        /// <param name="provider">An <see cref="System.IFormatProvider"/> that supplies culture-specific formatting information</param>
        /// <param name="format">A string containing zero or more format items</param>
        /// <param name="args">An object array containing zero or more objects to format</param>
        /// <remarks>
        /// The message is formatted using the string.Format method. See <see cref="string.Format(string, object[])"/> 
        /// for details of the syntax of the format string and the behavior of the formatting.
        /// This method does not take an <see cref="System.Exception" /> object to include in the log event.
        /// To pass an <see cref="System.Exception" /> use one of the Warn(object) methods instead.
        /// </remarks>
        void WarnFormat(IFormatProvider provider, string format, params object[] args);
    }
}
