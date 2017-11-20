namespace Common.Logging.NullLogger
{
    using System;

    /// <summary>
    /// Creates an instance of <see cref="NullLoggerWrapper"/> which never logs anything. Good when you want to disable logging.
    /// </summary>
    public class NullLoggerWrapper : ILog
    {
        /// <summary>
        /// Checks if this logger is enabled for the Debug level (always false).
        /// </summary>
        public bool IsDebugEnabled => false;

        /// <summary>
        /// Checks if this logger is enabled for the Info level (always false).
        /// </summary>
        public bool IsInfoEnabled => false;

        /// <summary>
        /// Checks if this logger is enabled for the Warn level (always false).
        /// </summary>
        public bool IsWarnEnabled => false;

        /// <summary>
        /// Checks if this logger is enabled for the Error level (always false).
        /// </summary>
        public bool IsErrorEnabled => false;

        /// <summary>
        /// Checks if this logger is enabled for the Fatal level (always false).
        /// </summary>
        public bool IsFatalEnabled => false;

        /// <summary>
        /// Null object for logging a message object with the Debug level.
        /// </summary>
        /// <param name="message">The message object to log.</param>
        public void Debug(object message)
        {
        }

        /// <summary>
        /// Null object for logging a message object with the Debug level including the stack 
        /// trace of the <see cref="System.Exception" /> passed as a parameter.
        /// </summary>
        /// <param name="message">The message object to log.</param>
        /// <param name="exception">The exception to log, including its stack trace.</param>
        public void Debug(object message, Exception exception)
        {
        }

        /// <summary>
        /// Null object for logging a formatted message string with the Debug level.
        /// </summary>
        /// <param name="format">A string containing zero or more format items</param>
        /// <param name="arg0">An object array containing zero or more objects to format</param>
        /// <remarks>
        /// The message is formatted using the <see cref="string.Format(string, object)"/> method. See <see cref="string.Format(string, object[])"/> 
        /// for details of the syntax of the format string and the behavior of the formatting.
        /// This method does not take an <see cref="System.Exception" /> object to include in the log event.
        /// To pass an <see cref="System.Exception" /> use one of the <see cref="ILog.Debug(object,System.Exception)"/> methods instead.
        /// </remarks>
        public void DebugFormat(string format, object arg0)
        {
        }

        /// <summary>
        /// Null object for logging a formatted message string with the Debug level.
        /// </summary>
        /// <param name="format">A string containing zero or more format items</param>
        /// <param name="arg0">An object array containing zero or more objects to format</param>
        /// <param name="arg1">An object array containing zero or more objects to format</param>
        /// <remarks>
        /// The message is formatted using the <see cref="string.Format(string, object, object)"/> method. See <see cref="string.Format(string, object[])"/> 
        /// for details of the syntax of the format string and the behavior of the formatting.
        /// This method does not take an <see cref="System.Exception" /> object to include in the log event.
        /// To pass an <see cref="System.Exception" /> use one of the <see cref="ILog.Debug(object,System.Exception)"/> methods instead.
        /// </remarks>
        public void DebugFormat(string format, object arg0, object arg1)
        {
        }

        /// <summary>
        /// Null object for logging a formatted message string with the Debug level.
        /// </summary>
        /// <param name="format">A string containing zero or more format items</param>
        /// <param name="arg0">An object array containing zero or more objects to format</param>
        /// <param name="arg1">An object array containing zero or more objects to format</param>
        /// <param name="arg2">An object array containing zero or more objects to format</param>
        /// <remarks>
        /// The message is formatted using the <see cref="string.Format(string, object, object, object)"/> method. See <see cref="string.Format(string, object[])"/> 
        /// for details of the syntax of the format string and the behavior of the formatting.
        /// This method does not take an <see cref="System.Exception" /> object to include in the log event.
        /// To pass an <see cref="System.Exception" /> use one of the <see cref="ILog.Debug(object,System.Exception)"/> methods instead.
        /// </remarks>
        public void DebugFormat(string format, object arg0, object arg1, object arg2)
        {
        }

        /// <summary>
        /// Null object for logging a formatted message string with the Debug level.
        /// </summary>
        /// <param name="format">A string containing zero or more format items</param>
        /// <param name="args">An object array containing zero or more objects to format</param>
        /// <remarks>
        /// The message is formatted using the string.Format method. See <see cref="string.Format(string, object[])"/> 
        /// for details of the syntax of the format string and the behavior of the formatting.
        /// This method does not take an <see cref="System.Exception" /> object to include in the log event.
        /// To pass an <see cref="System.Exception" /> use one of the <see cref="ILog.Debug(object,System.Exception)"/> methods instead.
        /// </remarks>
        public void DebugFormat(string format, params object[] args)
        {
        }

        /// <summary>
        /// Null object for logging a formatted message string with the Debug level.
        /// </summary>
        /// <param name="provider">A <see cref="System.IFormatProvider"/> that supplies culture-specific formatting information</param>
        /// <param name="format">A string containing zero or more format items</param>
        /// <param name="args">An object array containing zero or more objects to format</param>
        /// <remarks>
        /// The message is formatted using the string.Format method. See <see cref="string.Format(string, object[])"/> 
        /// for details of the syntax of the format string and the behavior of the formatting.
        /// This method does not take an <see cref="System.Exception" /> object to include in the log event.
        /// To pass an <see cref="System.Exception" /> use one of the <see cref="ILog.Debug(object,System.Exception)"/> methods instead.
        /// </remarks>
        public void DebugFormat(IFormatProvider provider, string format, params object[] args)
        {
        }

        /// <summary>
        /// Null object for logging a message object with the Info level.
        /// </summary>
        /// <param name="message">The message object to log.</param>
        public void Info(object message)
        {
        }

        /// <summary>
        /// Null object for logging a message object with the INFO level including the stack trace of the <see cref="System.Exception" /> passed as a parameter.
        /// </summary>
        /// <param name="message">The message object to log.</param>
        /// <param name="exception">The exception to log, including its stack trace.</param>
        public void Info(object message, Exception exception)
        {
        }

        /// <summary>
        /// Null object for logging a formatted message string with the Info level.
        /// </summary>
        /// <param name="format">A string containing zero or more format items</param>
        /// <param name="arg0">An object to format</param>
        /// <remarks>
        /// The message is formatted using the string.Format method. See <see cref="string.Format(string, object[])"/> 
        /// for details of the syntax of the format string and the behavior of the formatting.
        /// This method does not take an <see cref="System.Exception" /> object to include in the log event.
        /// To pass an <see cref="System.Exception" /> use one of the Info(object,Exception) methods instead.
        /// </remarks>
        public void InfoFormat(string format, object arg0)
        {
        }

        /// <summary>
        /// Null object for logging a formatted message string with the Info level.
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
        public void InfoFormat(string format, object arg0, object arg1)
        {
        }

        /// <summary>
        /// Null object for logging a formatted message string with the Info level.
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
        public void InfoFormat(string format, object arg0, object arg1, object arg2)
        {
        }

        /// <summary>
        /// Null object for logging a formatted message string with the Info level.
        /// </summary>
        /// <param name="format">A string containing zero or more format items</param>
        /// <param name="args">An object array containing zero or more objects to format</param>
        /// <remarks>
        /// The message is formatted using the string.Format method. See <see cref="string.Format(string, object[])"/> 
        /// for details of the syntax of the format string and the behavior of the formatting.
        /// This method does not take an <see cref="System.Exception" /> object to include in the log event.
        /// To pass an <see cref="System.Exception" /> use one of the Info(object) methods instead.
        /// </remarks>
        public void InfoFormat(string format, params object[] args)
        {
        }

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
        public void InfoFormat(IFormatProvider provider, string format, params object[] args)
        {
        }

        /// <summary>
        /// Null object for logging a message object with the Warn level.
        /// </summary>
        /// <param name="message">The message object to log.</param>
        public void Warn(object message)
        {
        }

        /// <summary>
        /// Null object for logging a message object with the Warn level including the stack trace of the <see cref="System.Exception" /> passed as a parameter.
        /// </summary>
        /// <param name="message">The message object to log.</param>
        /// <param name="exception">The exception to log, including its stack trace.</param>
        public void Warn(object message, Exception exception)
        {
        }

        /// <summary>
        /// Null object for logging a formatted message string with the Warn level.
        /// </summary>
        /// <param name="format">A string containing zero or more format items</param>
        /// <param name="arg0">An object to format</param>
        /// <remarks>
        /// The message is formatted using the string.Format method. See <see cref="string.Format(string, object[])"/> 
        /// for details of the syntax of the format string and the behavior of the formatting.
        /// This method does not take an <see cref="System.Exception" /> object to include in the log event.
        /// To pass an <see cref="System.Exception" /> use one of the Warn(object,Exception) methods instead.
        /// </remarks>
        public void WarnFormat(string format, object arg0)
        {
        }

        /// <summary>
        /// Null object for logging a formatted message string with the Warn level.
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
        public void WarnFormat(string format, object arg0, object arg1)
        {
        }

        /// <summary>
        /// Null object for logging a formatted message string with the Warn level.
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
        public void WarnFormat(string format, object arg0, object arg1, object arg2)
        {
        }

        /// <summary>
        /// Null object for logging a formatted message string with the Warn level.
        /// </summary>
        /// <param name="format">A string containing zero or more format items</param>
        /// <param name="args">An object array containing zero or more objects to format</param>
        /// <remarks>
        /// The message is formatted using the string.Format method. See <see cref="string.Format(string, object[])"/> 
        /// for details of the syntax of the format string and the behavior of the formatting.
        /// This method does not take an <see cref="System.Exception" /> object to include in the log event.
        /// To pass an <see cref="System.Exception" /> use one of the Warn(object) methods instead.
        /// </remarks>
        public void WarnFormat(string format, params object[] args)
        {
        }

        /// <summary>
        /// Null object for logging a formatted message string with the Warn level.
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
        public void WarnFormat(IFormatProvider provider, string format, params object[] args)
        {
        }

        /// <summary>
        /// Null object for logging a message object with the Error level.
        /// </summary>
        /// <param name="message">The message object to log.</param>
        public void Error(object message)
        {
        }

        /// <summary>
        /// Null object for logging a message object with the Error level including the stack trace of the <see cref="System.Exception" /> passed as a parameter.
        /// </summary>
        /// <param name="message">The message object to log.</param>
        /// <param name="exception">The exception to log, including its stack trace.</param>
        public void Error(object message, Exception exception)
        {
        }

        /// <summary>
        /// Null object for logging a formatted message string with the Error level.
        /// </summary>
        /// <param name="format">A string containing zero or more format items</param>
        /// <param name="arg0">An object to format</param>
        /// <remarks>
        /// The message is formatted using the <see cref="string.Format(string, object)"/> method."/> 
        /// for details of the syntax of the format string and the behavior of the formatting.
        /// This method does not take an <see cref="System.Exception" /> object to include in the log event.
        /// To pass an <see cref="System.Exception" /> use one of the Error(object,Exception) methods instead.
        /// </remarks>
        public void ErrorFormat(string format, object arg0)
        {
        }

        /// <summary>
        /// Null object for logging a formatted message string with the Error level.
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
        public void ErrorFormat(string format, object arg0, object arg1)
        {
        }

        /// <summary>
        /// Null object for logging a formatted message string with the Error level.
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
        public void ErrorFormat(string format, object arg0, object arg1, object arg2)
        {
        }

        /// <summary>
        /// Null object for logging a formatted message string with the Error level.
        /// </summary>
        /// <param name="format">A string containing zero or more format items</param>
        /// <param name="args">An object array containing zero or more objects to format</param>
        /// <remarks>
        /// The message is formatted using the string.Format method. <see cref="string.Format(string, object[])"/>
        /// for details of the syntax of the format string and the behavior of the formatting.
        /// This method does not take an <see cref="System.Exception" /> object to include in the log event.
        /// To pass an <see cref="System.Exception" /> use one of the Error(object) methods instead.
        /// </remarks>
        public void ErrorFormat(string format, params object[] args)
        {
        }

        /// <summary>
        /// Null object for logging a formatted message string with the Error level.
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
        public void ErrorFormat(IFormatProvider provider, string format, params object[] args)
        {
        }

        /// <summary>
        /// Null object for logging a message object with the Fatal level.
        /// </summary>
        /// <param name="message">The message object to log.</param>
        public void Fatal(object message)
        {
        }

        /// <summary>
        /// Null object for logging a message object with the Fatal level including the stack trace of the <see cref="System.Exception" /> passed as a parameter.
        /// </summary>
        /// <param name="message">The message object to log.</param>
        /// <param name="exception">The exception to log, including its stack trace.</param>
        public void Fatal(object message, Exception exception)
        {
        }

        /// <summary>
        /// Null object for logging a formatted message string with the Fatal level.
        /// </summary>
        /// <param name="format">A string containing zero or more format items</param>
        /// <param name="arg0">An object to format</param>
        /// <remarks>
        /// The message is formatted using the string.Format method. See <see cref="string.Format(string, object[])"/> 
        /// for details of the syntax of the format string and the behavior of the formatting.
        /// This method does not take an <see cref="System.Exception" /> object to include in the log event.
        /// To pass an <see cref="System.Exception" /> use one of the Fatal(object,Exception) methods instead.
        /// </remarks>
        public void FatalFormat(string format, object arg0)
        {
        }

        /// <summary>
        /// Null object for logging a formatted message string with the Fatal level.
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
        public void FatalFormat(string format, object arg0, object arg1)
        {
        }

        /// <summary>
        /// Null object for logging a formatted message string with the Fatal level.
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
        public void FatalFormat(string format, object arg0, object arg1, object arg2)
        {
        }

        /// <summary>
        /// Null object for logging a formatted message string with the Fatal level.
        /// </summary>
        /// <param name="format">A string containing zero or more format items</param>
        /// <param name="args">An object array containing zero or more objects to format</param>
        /// <remarks>
        /// The message is formatted using the string.Format method. See <see cref="string.Format(string, object[])"/> 
        /// for details of the syntax of the format string and the behavior of the formatting.
        /// This method does not take an <see cref="System.Exception" /> object to include in the log event.
        /// To pass an <see cref="System.Exception" /> use one of the Fatal(object) methods instead.
        /// </remarks>
        public void FatalFormat(string format, params object[] args)
        {
        }

        /// <summary>
        /// Null object for logging a formatted message string with the Fatal level.
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
        public void FatalFormat(IFormatProvider provider, string format, params object[] args)
        {
        }
    }
}