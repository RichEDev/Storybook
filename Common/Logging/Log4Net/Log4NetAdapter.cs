namespace Common.Logging.Log4Net
{
    using System;
    using System.IO;

    /// <summary>
    /// Enables the use of log4net logging.
    /// </summary>
    /// <typeparam name="T">What class type this will be monitoring.</typeparam>
    public class Log4NetAdapter<T> : ILog
    {
        /// <summary>
        /// log4net <see cref="log4net.ILog"/> that this adapter uses.
        /// </summary>
        private readonly log4net.ILog _log;

        /// <summary>
        /// Initializes static members of the <see cref="Log4NetAdapter{T}"/> class. 
        /// Sets the configuration if it has not yet been configured and monitors it for changes, if the file changes (log4net.config) it will be reloaded (does not restart app domain)
        /// </summary>
        static Log4NetAdapter()
        {
            if (log4net.LogManager.GetCurrentLoggers().Length == 0)
            {
                FileInfo configFile = new FileInfo(AppDomain.CurrentDomain.BaseDirectory + @"\log4net.config");

                if (configFile.Exists == false)
                {
                    throw new FileNotFoundException("Unable to find log4net configuration file.", configFile.FullName);    
                }

                log4net.Config.XmlConfigurator.ConfigureAndWatch(configFile);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Log4NetAdapter{T}"/> class. 
        /// </summary>
        /// <remarks>
        /// Note that when the calling class uses generics a unique logger is created for each unique signature
        /// </remarks>
        public Log4NetAdapter()
        {
            this._log = log4net.LogManager.GetLogger(typeof(T).ToString());
        }

        /// <summary>
        /// Checks if this logger is enabled for the Debug level.
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
        public bool IsDebugEnabled => this._log.IsDebugEnabled;

        /// <summary>
        /// Checks if this logger is enabled for the Info level.
        /// </summary>
        /// <remarks>For more information see <see cref="ILog.IsDebugEnabled"/>.</remarks>
        public bool IsInfoEnabled => this._log.IsInfoEnabled;

        /// <summary>
        /// Checks if this logger is enabled for the Warn level.
        /// </summary>
        /// <remarks>For more information see <see cref="ILog.IsDebugEnabled"/>.</remarks>
        public bool IsWarnEnabled => this._log.IsWarnEnabled;

        /// <summary>
        /// Checks if this logger is enabled for the Error level.
        /// </summary>
        /// <remarks>For more information see <see cref="ILog.IsDebugEnabled"/>.</remarks>
        public bool IsErrorEnabled => this._log.IsErrorEnabled;

        /// <summary>
        /// Checks if this logger is enabled for the Fatal level.
        /// </summary>
        /// <remarks>For more information see <see cref="ILog.IsDebugEnabled"/>.</remarks>
        public bool IsFatalEnabled => this._log.IsFatalEnabled;

        /// <summary>
        /// Log a message object with the Debug level.
        /// </summary>
        /// <param name="message">The message object to log.</param>
        public void Debug(object message)
        {
            this._log.Debug(message);
        }

        /// <summary>
        /// Log a message object with the Debug level including the stack 
        /// trace of the <see cref="System.Exception" /> passed as a parameter.
        /// </summary>
        /// <param name="message">The message object to log.</param>
        /// <param name="exception">The exception to log, including its stack trace.</param>
        public void Debug(object message, Exception exception)
        {
            this._log.Debug(message, exception);
        }

        /// <summary>
        /// Logs a formatted message string with the Debug level.
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
            this._log.DebugFormat(format, arg0);
        }

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
        /// To pass an <see cref="System.Exception" /> use one of the <see cref="ILog.Debug(object,System.Exception)"/> methods instead.
        /// </remarks>
        public void DebugFormat(string format, object arg0, object arg1)
        {
            this._log.DebugFormat(format, arg0, arg1);
        }

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
        /// To pass an <see cref="System.Exception" /> use one of the <see cref="ILog.Debug(object,System.Exception)"/> methods instead.
        /// </remarks>
        public void DebugFormat(string format, object arg0, object arg1, object arg2)
        {
            this._log.DebugFormat(format, arg0, arg1, arg2);
        }

        /// <summary>
        /// Logs a formatted message string with the Debug level.
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
            this._log.DebugFormat(format, args);
        }

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
        /// To pass an <see cref="System.Exception" /> use one of the <see cref="ILog.Debug(object,System.Exception)"/> methods instead.
        /// </remarks>
        public void DebugFormat(IFormatProvider provider, string format, params object[] args)
        {
            this._log.DebugFormat(provider, format, args);
        }

        /// <summary>
        /// Logs a message object with the Info level.
        /// </summary>
        /// <param name="message">The message object to log.</param>
        public void Info(object message)
        {
            this._log.Info(message);
        }

        /// <summary>
        /// Logs a message object with the INFO level including the stack trace of the <see cref="System.Exception" /> passed as a parameter.
        /// </summary>
        /// <param name="message">The message object to log.</param>
        /// <param name="exception">The exception to log, including its stack trace.</param>
        public void Info(object message, Exception exception)
        {
            this._log.Info(message, exception);
        }

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
        public void InfoFormat(string format, object arg0)
        {
            this._log.InfoFormat(format, arg0);
        }

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
        public void InfoFormat(string format, object arg0, object arg1)
        {
            this._log.InfoFormat(format, arg0, arg1);
        }

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
        public void InfoFormat(string format, object arg0, object arg1, object arg2)
        {
            this._log.InfoFormat(format, arg0, arg1, arg2);
        }

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
        public void InfoFormat(string format, params object[] args)
        {
            this._log.InfoFormat(format, args);
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
            this._log.InfoFormat(provider, format, args);
        }

        /// <summary>
        /// Log a message object with the Warn level.
        /// </summary>
        /// <param name="message">The message object to log.</param>
        public void Warn(object message)
        {
            this._log.Warn(message);
        }

        /// <summary>
        /// Log a message object with the Warn level including the stack trace of the <see cref="System.Exception" /> passed as a parameter.
        /// </summary>
        /// <param name="message">The message object to log.</param>
        /// <param name="exception">The exception to log, including its stack trace.</param>
        public void Warn(object message, Exception exception)
        {
            this._log.Warn(message, exception);
        }

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
        public void WarnFormat(string format, object arg0)
        {
            this._log.WarnFormat(format, arg0);
        }

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
        public void WarnFormat(string format, object arg0, object arg1)
        {
            this._log.WarnFormat(format, arg0, arg1);
        }

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
        public void WarnFormat(string format, object arg0, object arg1, object arg2)
        {
            this._log.WarnFormat(format, arg0, arg1, arg2);
        }

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
        public void WarnFormat(string format, params object[] args)
        {
            this._log.WarnFormat(format, args);
        }

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
        public void WarnFormat(IFormatProvider provider, string format, params object[] args)
        {
            this._log.WarnFormat(provider, format, args);
        }

        /// <summary>
        /// Logs a message object with the Error level.
        /// </summary>
        /// <param name="message">The message object to log.</param>
        public void Error(object message)
        {
            this._log.Error(message);
        }

        /// <summary>
        /// Log a message object with the Error level including the stack trace of the <see cref="System.Exception" /> passed as a parameter.
        /// </summary>
        /// <param name="message">The message object to log.</param>
        /// <param name="exception">The exception to log, including its stack trace.</param>
        public void Error(object message, Exception exception)
        {
            this._log.Error(message, exception);
        }

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
        public void ErrorFormat(string format, object arg0)
        {
            this._log.ErrorFormat(format, arg0);
        }

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
        public void ErrorFormat(string format, object arg0, object arg1)
        {
            this._log.ErrorFormat(format, arg0, arg1);
        }

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
        public void ErrorFormat(string format, object arg0, object arg1, object arg2)
        {
            this._log.ErrorFormat(format, arg0, arg1, arg2);
        }

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
        public void ErrorFormat(string format, params object[] args)
        {
            this._log.ErrorFormat(format, args);
        }

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
        public void ErrorFormat(IFormatProvider provider, string format, params object[] args)
        {
            this._log.ErrorFormat(provider, format, args);
        }

        /// <summary>
        /// Log a message object with the Fatal level.
        /// </summary>
        /// <param name="message">The message object to log.</param>
        public void Fatal(object message)
        {
            this._log.Fatal(message);
        }

        /// <summary>
        /// Log a message object with the Fatal level including the stack trace of the <see cref="System.Exception" /> passed as a parameter.
        /// </summary>
        /// <param name="message">The message object to log.</param>
        /// <param name="exception">The exception to log, including its stack trace.</param>
        public void Fatal(object message, Exception exception)
        {
            this._log.Fatal(message, exception);
        }

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
        public void FatalFormat(string format, object arg0)
        {
            this._log.FatalFormat(format, arg0);
        }

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
        public void FatalFormat(string format, object arg0, object arg1)
        {
            this._log.FatalFormat(format, arg0, arg1);
        }

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
        public void FatalFormat(string format, object arg0, object arg1, object arg2)
        {
            this._log.FatalFormat(format, arg0, arg1, arg2);
        }

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
        public void FatalFormat(string format, params object[] args)
        {
            this._log.FatalFormat(format, args);
        }

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
        public void FatalFormat(IFormatProvider provider, string format, params object[] args)
        {
            this._log.FatalFormat(provider, format, args);
        }
    }
}
